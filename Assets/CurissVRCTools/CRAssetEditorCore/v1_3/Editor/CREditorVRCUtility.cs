using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace CRAssetEditorCore_v1_3
{
    public class CREditorVRCUtility
    {
        #region 찾기
        // Control 찾기.
        public static VRCExpressionsMenu.Control FindControl(VRCExpressionsMenu target, VRCExpressionsMenu.Control control)
        {
            if (!target || control == null)
                return null;

            return FindControl(target, control.name);
        }
        public static VRCExpressionsMenu.Control FindControl(VRCExpressionsMenu target, string name)
        {
            if (!target)
                return null;

            foreach (VRCExpressionsMenu.Control control in target.controls)
            {
                if (control.name.Equals(name))
                    return control;
            }

            return null;
        }

        // Parameter 찾기
        public static VRCExpressionParameters.Parameter FindParameter(VRCExpressionParameters target, VRCExpressionParameters.Parameter parameter)
        {
            if (!target || parameter == null)
                return null;

            return target.FindParameter(parameter.name);
        }
        #endregion

        #region 복사
        // 메뉴 내용 복사
        public static void CopyMenu(VRCExpressionsMenu target, VRCExpressionsMenu.Control control, bool setDirty)
        {
            if (!target || control == null)
                return;

            VRCExpressionsMenu.Control targetControl = FindControl(target, control);
            if (targetControl != null)
            {
                target.controls.Remove(targetControl);
            }

            target.controls.Add(CopyControlObject(control));

            if (setDirty)
                EditorUtility.SetDirty(target);
        }

        // 메뉴 내용 전체 복사
        public static void CopyALLMenu(VRCExpressionsMenu target, VRCExpressionsMenu source)
        {
            if (!target || !source)
                return;

            foreach (VRCExpressionsMenu.Control control in source.controls)
            {
                CopyMenu(target, control, false);
            }

            EditorUtility.SetDirty(target);
        }

        public static void CopyParamater(VRCExpressionParameters target, VRCExpressionParameters.Parameter source)
        {
            List<VRCExpressionParameters.Parameter> parameterList = new List<VRCExpressionParameters.Parameter>(target.parameters);

            VRCExpressionParameters.Parameter targetParamter = target.FindParameter(source.name);
            if (targetParamter != null)
            {
                parameterList.Remove(targetParamter);
            }

            parameterList.Add(CopyParameterObject(source));
        }

        // 파라미터 전체 복사
        public static void CopyAllParamater(VRCExpressionParameters target, VRCExpressionParameters source)
        {
            List<VRCExpressionParameters.Parameter> parameterList = new List<VRCExpressionParameters.Parameter>(target.parameters);

            foreach (VRCExpressionParameters.Parameter parameter in source.parameters)
            {
                VRCExpressionParameters.Parameter targetParamter = target.FindParameter(parameter.name);
                if (targetParamter != null)
                {
                    parameterList.Remove(targetParamter);
                }

                parameterList.Add(CopyParameterObject(parameter));
            }

            target.parameters = parameterList.ToArray();

            EditorUtility.SetDirty(target);
        }
        #endregion

        #region 오브젝트 복사
        // Contorl 오브젝트 복사
        public static VRCExpressionsMenu.Control CopyControlObject(VRCExpressionsMenu.Control source)
        {
            if (source == null)
                return null;

            VRCExpressionsMenu.Control control = new VRCExpressionsMenu.Control()
            {
                icon = source.icon,
                labels = source.labels,
                name = source.name,
                parameter = source.parameter,
                style = source.style,
                subParameters = source.subParameters,
                type = source.type,
                value = source.value,
                subMenu = source.subMenu
            };

            return control;
        }

        // Parameter 오브젝트 복사
        public static VRCExpressionParameters.Parameter CopyParameterObject(VRCExpressionParameters.Parameter source)
        {
            if (source == null)
                return null;

            VRCExpressionParameters.Parameter parameter = new VRCExpressionParameters.Parameter()
            {
                name = source.name,
                valueType = source.valueType,
                defaultValue = source.defaultValue,
                saved = source.saved,
                networkSynced = source.networkSynced
            };

            return parameter;
        }
        #endregion

        #region 계산
        // 필요한 파라미터 수 체크
        public static int CountRequireParam(VRCExpressionParameters target, VRCExpressionParameters source)
        {
            int count = 0;

            if (!target || !source)
                return 0;

            for (int i = 0; i < source.parameters.Length; i++)
            {                
                if (source.parameters[i].networkSynced == false)    // Synced만 카운팅
                    continue;

                string paramName = source.GetParameter(i).name;

                if (target.FindParameter(paramName) == null)
                {
                    count += 1;
                }
            }

            return count;
        }

        // 파라미터 공간 확인
        public static bool CheckParameterCount(VRCExpressionParameters target, VRCExpressionParameters source)
        {
            if (target)
            {
                int remainingParams = VRCExpressionParameters.MAX_PARAMETER_COST - target.CalcTotalCost();
                int requireParams = CountRequireParam(target, source);

                if (remainingParams < requireParams)
                {
                    return false;
                }
            }

            return true;
        }
        #endregion
    }
}