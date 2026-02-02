using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;


namespace CRAssetEditorCore_v1_3
{
    public class CEditorReference : ScriptableObject
    {
        public Object root;

        public Texture2D titleImage;
        public Texture2D bgImage;

        public RuntimeAnimatorController fx;
        public VRCExpressionsMenu vrcMenu;
        public VRCExpressionParameters vrcParams;
    }
}
