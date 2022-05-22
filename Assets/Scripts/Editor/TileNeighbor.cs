using Carbon;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor
{
    public class TileNeighbor
    {
        [MenuItem("GameObject/InitTileNeighbor")]
        private static void InitTileNeighbor()
        {
            Debug.Log(Selection.activeObject.name);
            var componentsInChildren = Selection.activeObject.GetComponentsInChildren<TileController>();
            foreach (var mainTile in componentsInChildren)
            {
                foreach (var otherTile in componentsInChildren)
                {
                    if (Vector2.Distance(mainTile.transform.position, otherTile.transform.position) < 4)
                    {
                        mainTile.NeighborTiles.Add(otherTile.transform);
                    }
                }
                PrefabUtility.RecordPrefabInstancePropertyModifications(mainTile);
            }

            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }
        [MenuItem("GameObject/ResetTileNeighbor")]
        private static void ResetTileNeighbor()
        {
            Debug.Log(Selection.activeObject.name);
            var componentsInChildren = Selection.activeObject.GetComponentsInChildren<TileController>();
            foreach (var mainTile in componentsInChildren)
            {
                mainTile.NeighborTiles.Clear();
                PrefabUtility.RecordPrefabInstancePropertyModifications(mainTile);
            }
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }
    }
}