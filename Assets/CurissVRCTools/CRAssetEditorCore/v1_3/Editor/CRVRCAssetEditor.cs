using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace CRAssetEditorCore_v1_3
{
    public class CRVRCAssetSetting
    {
        public Object root = null;
        public RuntimeAnimatorController assetFX = null;
        public VRCExpressionsMenu targetMenu = null;
        public VRCExpressionsMenu assetMenu = null;
        public VRCExpressionParameters assetParameter = null;
        public GameObject assetPrefab = null;
        public bool writeDefault = true;
    }

    public class CRVRCAssetEditor : Editor
    {
        protected const string COMMON_DATA_VRC_BASE_PATH  = "/VRCBase/";
        protected const string COMMON_DATA_VRC_BASE_FX    = "BaseFX.controller";
        protected const string COMMON_DATA_VRC_BASE_MENU  = "BaseMenu.asset";
        protected const string COMMON_DATA_VRC_BASE_PARAM = "BaseParam.asset";

        protected const string ROOT_NULL_DATA_PATH = "Assets/CRAssetData/";

        #region Utility
        // 타이틀 이미지
        protected static void TitleImage(Texture2D background, Texture2D content, int height=70)
        {
            if (background || content)
            {
                Rect rect = EditorGUILayout.GetControlRect(false, GUILayout.Height(height));

                if (background)
                    GUI.DrawTexture(rect, background, ScaleMode.ScaleAndCrop);

                if (content)
                    GUI.DrawTexture(rect, content, ScaleMode.ScaleToFit);
            }
        }
        #endregion

        #region Apply
        // 에셋 적용.
        protected bool Apply(VRCAvatarDescriptor avatar, CRVRCAssetSetting setting)
        {
            bool result;

            // 에러 체크
            if (!avatar)
            {
                Debug.LogError("아바타를 찾을 수 없습니다.");
                return false;
            }
            if (setting == null)
            {
                Debug.LogError("Asset Setting을 찾을 수 없습니다.");
                return false;
            }

            // Data 폴더 생성
            string dataPath = ROOT_NULL_DATA_PATH + avatar.name;
            if (setting.root)
            {
                dataPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(setting.root));
                dataPath += "/Data";
                dataPath += "/" + avatar.name;
            }
            CREditorUtility.CreateFolders(dataPath);

            // 오브젝트 적용
            result = ApplyPrefab(avatar, dataPath);
            if (!result)
            {
                Debug.LogError("프리펩 적용 실패");
                return false;
            }

            // FX 적용
            if (setting.assetFX)
            {
                result = ApplyFX(avatar, setting.assetFX, dataPath, setting.writeDefault);
                if (!result)
                {
                    Debug.LogError("FX 적용 실패");
                    return false;
                }
            }

            // 메뉴 적용
            if (setting.assetMenu)
            {
                result = ApplyMenu(avatar, setting.targetMenu, setting.assetMenu, dataPath);
                if (!result)
                {
                    Debug.LogError("메뉴 적용 실패");
                    return false;
                }
            }

            // 파라미터 적용
            if (setting.assetParameter)
            {
                result = ApplyParam(avatar, setting.assetParameter, dataPath);
                if (!result)
                {
                    Debug.LogError("파라미터 적용 실패");
                    return false;
                }
            }

            return true;
        }

        // 오브젝트 적용 (Override 필요)
        protected virtual bool ApplyPrefab(VRCAvatarDescriptor avatar, string dataPath)
        {
            Debug.Log("asdd");

            return true;
        }

        // FX 적용
        protected virtual bool ApplyFX(VRCAvatarDescriptor avatar, RuntimeAnimatorController fx, string dataPath, bool writeDefault)
        {
            avatar.customizeAnimationLayers = true;
            avatar.baseAnimationLayers[4].isDefault = false;
            avatar.baseAnimationLayers[4].isEnabled = true;

            // 아바타 FX.
            RuntimeAnimatorController avatarFX = avatar.baseAnimationLayers[4].animatorController;
            if (!avatarFX)
            {
                string path = CREditorUtility.GetScriptPath() + COMMON_DATA_VRC_BASE_PATH + COMMON_DATA_VRC_BASE_FX;
                RuntimeAnimatorController fxBase = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(path);
                if (!fxBase)
                {
                    EditorUtility.DisplayDialog("에러", COMMON_DATA_VRC_BASE_FX + "를 찾을 수 없습니다.", "확인");
                    return false;
                }

                avatarFX = (RuntimeAnimatorController)CREditorUtility.CopyAsset(fxBase, dataPath + "/AvatarFX.controller");

                AnimatorState[] avatarFXStates = CRAnimatorControllerUtility.GetAllStates(avatarFX);
                for (int i = 0; i < avatarFXStates.Length; i++)
                {
                    avatarFXStates[i].writeDefaultValues = writeDefault;
                }

                avatar.baseAnimationLayers[4].animatorController = avatarFX;
            }

            // 에셋 FX.
            AnimatorState[] assteFXStates = CRAnimatorControllerUtility.GetAllStates(fx);
            for (int i = 0; i < assteFXStates.Length; i++)
            {
                assteFXStates[i].writeDefaultValues = writeDefault;
            }

            // 합치기.
            CRAnimatorControllerUtility.Merge(avatarFX, fx);

            return true;
        }

        // Menu 적용
        protected virtual bool ApplyMenu(VRCAvatarDescriptor avatar, VRCExpressionsMenu targetMenu , VRCExpressionsMenu assetMenu, string dataPath)
        {
            avatar.customExpressions = true;
            VRCExpressionsMenu menu;

            // targetMenu 우선 적용. 없으면 아바타 메뉴를 가져와서 적용.
            if (targetMenu)
            {
                menu = targetMenu;
            }
            else
            {
                VRCExpressionsMenu avatarMenu = avatar.expressionsMenu;
                if (!avatarMenu)
                {
                    string path = CREditorUtility.GetScriptPath() + COMMON_DATA_VRC_BASE_PATH + COMMON_DATA_VRC_BASE_MENU;
                    VRCExpressionsMenu menuBase = AssetDatabase.LoadAssetAtPath<VRCExpressionsMenu>(path);
                    if (!menuBase)
                    {
                        EditorUtility.DisplayDialog("에러", COMMON_DATA_VRC_BASE_MENU + "를 찾을 수 없습니다.", "확인");
                        return false;
                    }

                    avatarMenu = (VRCExpressionsMenu)CREditorUtility.CopyAsset(menuBase, dataPath + "/AvatarMenu.asset");
                    avatar.expressionsMenu = avatarMenu;
                }

                menu = avatarMenu;
            }

            CREditorVRCUtility.CopyALLMenu(menu, assetMenu);

            return true;
        }

        // 파라미터 적용
        protected virtual bool ApplyParam(VRCAvatarDescriptor avatar, VRCExpressionParameters parameters, string dataPath)
        {
            avatar.customExpressions = true;

            VRCExpressionParameters avatarParam = avatar.expressionParameters;
            if (!avatarParam)
            {
                string path = CREditorUtility.GetScriptPath() + COMMON_DATA_VRC_BASE_PATH + COMMON_DATA_VRC_BASE_PARAM;
                VRCExpressionParameters parametersBase = AssetDatabase.LoadAssetAtPath<VRCExpressionParameters>(path);
                if (!parametersBase)
                {
                    EditorUtility.DisplayDialog("에러", COMMON_DATA_VRC_BASE_PARAM + "를 찾을 수 없습니다.", "확인");
                    return false;
                }

                avatarParam = (VRCExpressionParameters)CREditorUtility.CopyAsset(parametersBase, dataPath + "/AvatarParam.asset");
                avatar.expressionParameters = avatarParam;
            }

            CREditorVRCUtility.CopyAllParamater(avatarParam, parameters);

            return true;
        }
        #endregion
    }
}