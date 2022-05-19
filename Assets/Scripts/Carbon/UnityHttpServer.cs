using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

namespace Server
{
    public class UnityHttpServer : MonoBehaviour
    {
        //Singleton variables
        private static UnityHttpServer _instance;
        //Singleton public variable to allow global access
        public static UnityHttpServer Instance;

        [Header("Server related information")] 
        [SerializeField] private string url = $"localhost";
        [SerializeField] private int port = 8000;
        [SerializeField] private bool initServerOnStart;

        [Header("Example related")] 
        [SerializeField] private UnityEvent exampleEvents;
        [SerializeField] private List<GameObject> toggleObjects;
        
        [Header("Debug Related")] 
        [SerializeField] private bool debug;

        //Private variables
        private HttpListener _listener;
        private Thread _serverThread;
        private Queue<HttpListenerContext> _requests;
        private bool _serverIsUp;

        public delegate void ReceiveEnergyDelegate(int energy);

        public ReceiveEnergyDelegate ReceiveEnergyListener;

        public bool ServerIsUp => _serverIsUp;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(this);
            Instance = _instance;
        }

        private void Start()
        {
            if (initServerOnStart)
            {
                InitializeServer();
            }
        }

        public void InitializeServer()
        {
            if (ServerIsUp) return;
            
            //Initialize server at the URL and Port set in the Editor
            var serverUrl = $"http://{url}:{port}/";
            DebugMessage($"Setting up server at: {serverUrl}");
            
            _listener = new HttpListener();
            _listener.Prefixes.Add(serverUrl);
            _listener.Start();
            DebugMessage($"Listening for connections on: {serverUrl}");

            _serverIsUp = true;

            //Initialize the server thread
            _serverThread = new Thread(StartServerThread);
            _serverThread.Start();

            //Initialize the queue of requests. Every new request is added to the queue and handled in the order in
            //which they entered the queue (First In, First Out).
            _requests = new Queue<HttpListenerContext>();
            
            //Initialize the server coroutine
            StartCoroutine(ServerCoroutine());
        }

        public void StopServer()
        {
            if (!ServerIsUp) return;
            
            //Stop the server and kill the server thread and coroutine.
            _serverThread.Abort();
            StopAllCoroutines();
            _listener.Stop();
            _serverIsUp = false;
        }

        /// <summary>
        /// Server thread. While the server is up, it will call the ServerCallback method for each new request received.
        /// It does nothing with the requests received yet. 
        /// </summary>
        private void StartServerThread()
        {
            while (ServerIsUp)
            {
                var result = _listener.BeginGetContext(ServerCallback, _listener);
                result.AsyncWaitHandle.WaitOne();
            }
        }

        /// <summary>
        /// Method that deals with each incoming request received by the server.
        /// </summary>
        /// <param name="result">Received request.</param>
        private void ServerCallback(IAsyncResult result)
        {
            var context = _listener.EndGetContext(result);
            //Enqueue every incoming request to be handled during the server Coroutine.
            //This ensures that the requests will be handled during Unity's thread.
            _requests.Enqueue(context);
        }
        
        /// <summary>
        /// Server coroutine. It runs together with Unity's thread and makes sure that the requests are handled correctly.
        /// </summary>
        /// <returns></returns>
        private IEnumerator ServerCoroutine()
        {
            while (ServerIsUp)
            {
                //Waits until there is at least 1 request to be handled.
                yield return new WaitUntil(() => _requests.Count > 0);
                //Iterates over the lists of requests removing them one by one until the queue is empty.
                while (_requests.Count > 0)
                {
                    //Resolves the requests in the queue.
                    ResolveRequest(_requests.Dequeue());
                }
            }
        }
        
        private void ResolveRequest(HttpListenerContext request)
        {
            //Reads the HTTP related information
            var method = request.Request.HttpMethod;
            var localPath = request.Request.Url.LocalPath;
            var contentType = request.Request.ContentType;
            
            //Loads the content from the request into a string        
            string content;
            using (var reader = new StreamReader(request.Request.InputStream, request.Request.ContentEncoding))
            {
                content = reader.ReadToEnd();
            }
            
            DebugMessage($"Method {method} - Local Path: {localPath}");
            DebugMessage($"Content Type {contentType} - Content: {content}");
                    
            //Hardcoded response back to 200 (Success according to HTTP). Ideally you want to move this back to the
            //switch-statement before this and use a proper code depending on each case.
            request.Response.StatusCode = 200;

            //Solve the request. This is a RESTfull like approach:
            // GET:   methods are simple commands with one line and no content. Useful for pings, tests, and other requests
            //        that require no data to be sent by the request.
            // POST:  methods which receive some sort of data to solve the request. Useful for more complex iterations that
            //        require the request to send additional information, such as sending player information.
            switch (method)
            {
                case "GET":
                {
                    switch (localPath)
                    {
                        case "/":
                            ReceiveEnergyListener?.Invoke(Convert.ToInt32(request.Request.QueryString["energy"]));
                            Debug.Log($"req: {request.Request.QueryString["energy"]}");
                            break;
                        case "/ping":
                        {
                            /*
                             * Example of command to test on the terminal using CURL
                             * curl http://localhost:8000/ping
                             *
                             * Example of command to test on the browser
                             * http://localhost:8000/ping
                             */
                            DebugMessage($"Ping from {request.Request.UserHostName} - {request.Request.UserHostAddress}.");
                        }
                            break;
                        case "/executeEvents":
                        {
                            /*
                             * Example of command to test on the terminal using CURL
                             * curl http://localhost:8000/executeEvents
                             *
                             * Example of command to test on the browser
                             * http://localhost:8000/executeEvents
                             */
                            
                            DebugMessage($"Execute Events from {request.Request.UserHostName} - {request.Request.UserHostAddress}.");
                            exampleEvents.Invoke();
                        }
                            break;
                    }
                }
                    break;
                case "POST":
                {
                    switch (localPath)
                    {
                        case "/toggleObject":
                        {
                            /*
                             * Example of command to test on the terminal using CURL
                             * curl -X POST http://localhost:8000/toggleObject -H 'Content-Type: application/json' -d '{"index":1}'
                             *
                             * Using the index as a string will also work. The parse will manage to parse the string to an int
                             * curl -X POST http://localhost:8000/toggleObject -H 'Content-Type: application/json' -d '{"index":"1"}'
                             *
                             * The try-catch avoids breaking the application if the json is badly formatted or if the fields are not correct
                             * curl -X POST http://localhost:8000/toggleObject -H 'Content-Type: application/json' -d '{"index":"test}'
                             * The command above will trigger the catch and show the error message, but the server will continue just fine
                             *
                             * Post commands cannot be executed with a simple URL on the browser.
                             */

                            //Verifies if the content is not null and if the content of the request is a JSON
                            if (contentType != null && contentType.Equals("application/json"))
                            {
                                DebugMessage($"Content: {content}");
                                
                                //There are way better ways to parse a JSON into a consistent struct or C# class, but 
                                //this option is very generic and you can later expand to achieve a more stable code
                                // try
                                // {
                                //     //Convert the json into a struct of type ToggleObject
                                //     var result = JsonUtility.FromJson<ToggleObject>(content);
                                //     DebugMessage($"Request JSON: {result}");
                                //
                                //     //Read the index value
                                //     var index = result.index;
                                //     DebugMessage($"Index is {index}");
                                //
                                //     //Clamp the index to be a valid index according to the List's content size
                                //     var clampedIndex = Mathf.Clamp(index, 0, toggleObjects.Count - 1);
                                //     DebugMessage($"Clamped index is {clampedIndex}");
                                //
                                //     var active = !toggleObjects[clampedIndex].activeSelf;
                                //     
                                //     //Toggle the active state of the object from the given index
                                //     toggleObjects[clampedIndex].SetActive(active);
                                //
                                //     //Reply to the request with a ToggleStatus struct in the form of a JSON
                                //     var responseJson = JsonUtility.ToJson(new ToggleStatus {active = active, index = clampedIndex});
                                //     
                                //     //Transforms the json into a byte array buffer
                                //     var buffer = System.Text.Encoding.UTF8.GetBytes(responseJson);
                                //     //Sets the response content length
                                //     request.Response.ContentLength64 = buffer.Length;
                                //     //Writes the byte array into the response
                                //     request.Response.OutputStream.Write(buffer, 0, buffer.Length);
                                //     //Closes the response data
                                //     request.Response.OutputStream.Close();
                                // }
                                // catch (Exception e)
                                // {
                                //     DebugMessage($"Parsing gone wrong. Exception: {e.Message}");
                                // }
                            }
                        }
                        break;
                    }
                }
                break;
        }

        //Prepares the response back - do not mind the bloated code. It tries to bypass some CORS related issues.
        //Read about CORS if you want to know more about it ;)
        request.Response.AppendHeader("Access-Control-Allow-Origin", "*");
        request.Response.AppendHeader("Access-Control-Allow-Credentials", "true");
        request.Response.AppendHeader("Access-Control-Allow-Headers", "Content-Type, X-CSRF-Token, X-Requested-With, Accept, Accept-Version, Content-Length, Content-MD5, Date, X-Api-Version, X-File-Name");
        request.Response.AppendHeader("Access-Control-Allow-Methods", "POST,GET,PUT,PATCH,DELETE,OPTIONS");

        //Close the response back to the sender.
        request.Response.Close();
    }

        #region Debug Related Code
        private void DebugMessage(string message)
        {
            if (!debug) return;
            Debug.Log(message);
        }
        #endregion
    }
}
