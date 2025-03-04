using UnityEngine;
using com.vrcfury.api.Components;
using VRC.SDK3.Avatars.ScriptableObjects;
using UnityEditor.Animations;
using UnityEditor;

namespace DMCustom
{
    public class PCSPrefabProcess : MonoBehaviour
    {
        //This script is made for Installer: VRCFury
        public enum Installer
        {
            ModularAvatar, VRCFury
        }
        public static Installer installer = Installer.VRCFury;
        public static void ShowInstaller()
        {
            if (installer == Installer.VRCFury)
            {
                GUI.enabled = false;
                GUI.color = new Color32(255, 138, 0, 255);
                PCSPrefabProcess.installer = (PCSPrefabProcess.Installer)EditorGUILayout.EnumPopup(new GUIContent("Installer", ""), PCSPrefabProcess.installer);
                GUI.color = new Color32(255, 255, 255, 255);
                GUI.enabled = true;
            }
        }
        public static void AddGeneratedAssetToPrefab(GameObject PCS, AnimatorController controler, VRCExpressionsMenu menu, VRCExpressionParameters param, AnimatorController direct)
        {
            FuryFullController furyFullController = com.vrcfury.api.FuryComponents.CreateFullController(PCS);
            furyFullController.AddController(direct);
            furyFullController.AddController(controler);          
            furyFullController.AddMenu(menu);
            furyFullController.AddParams(param);
            furyFullController.AddGlobalParam("*");
            furyFullController.AddPathRewrite(PCS.name, "");
        }
    }
}
