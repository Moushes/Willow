using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEditor;

namespace CRAssetEditorCore_v1_3
{
    public class CREditorUtility
    {
        // GUI Line 그리기.
        public static void GuiLine(int i_height = 1, int padding = 5, float x = 0)
        {
            GUILayout.Space(padding);
            Rect rect = EditorGUILayout.GetControlRect(false, i_height);
            rect.x -= x;
            rect.width += x;
            rect.height = i_height;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
            GUILayout.Space(padding);
        }

        // 폴더 생성.
        public static void CreateFolders(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
                return;

            string parent = System.IO.Path.GetDirectoryName(path);
            string child = System.IO.Path.GetFileName(path);
            CreateFolders(parent);
            AssetDatabase.CreateFolder(parent, child);
        }

        // 게임 오브젝트 경로 찾기.
        public static string GetGameObjectPath(Transform transform, Transform root)
        {
            string path = transform.name;
            transform = transform.parent;

            if (transform == null)
                return "";

            while (transform != null && transform != root)
            {
                path = transform.name + "/" + path;
                transform = transform.parent;
            }
            return path;
        }

        // 모든 하위 오브젝트 제거.
        public static void RemoveAllChilds(Transform target)
        {
            if (!target) return;

            for (int i = 0; i < target.childCount; i++)
            {
                Object.DestroyImmediate(target.GetChild(i).gameObject);
            }
        }

        // 프리펩 해제.
        public static void UnpackPrefab(GameObject target)
        {
            if (PrefabUtility.IsAnyPrefabInstanceRoot(target))
            {
                PrefabUtility.UnpackPrefabInstance(target, PrefabUnpackMode.Completely, InteractionMode.UserAction);
            }
        }

        // 에셋 제작.
        public static Object CreateAsset(Object source, string path)
        {
            AssetDatabase.CreateAsset(source, path);
            Object asset = AssetDatabase.LoadAssetAtPath<Object>(path);

            return asset ? asset : null;
        }

        // 에셋 복사.
        public static Object CopyAsset(Object source, string path)
        {
            if (!source)
                return null;

            string assetPath = AssetDatabase.GetAssetPath(source);

            return CopyAsset(assetPath, path);
        }
        public static Object CopyAsset(string source, string path)
        {
            if (AssetDatabase.CopyAsset(source, path))
            {
                return AssetDatabase.LoadAssetAtPath<Object>(path);
            }

            return null;
        }

        // 스크립트 파일 경로
        public static string GetScriptPath([CallerFilePath] string sourceFilePath = "")
        {
            string fullPath = System.IO.Path.GetDirectoryName(sourceFilePath);

            int rootIndex = fullPath.IndexOf(@"Assets\");
            if (rootIndex > -1)
            {
                return fullPath.Substring(rootIndex, fullPath.Length - rootIndex);
            }

            return null;
        }
    }
}