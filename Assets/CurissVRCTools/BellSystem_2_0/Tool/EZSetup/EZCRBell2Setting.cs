using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;


namespace Curiss.CRBell2
{
    [SerializeField]
    public class EZCRBell2Setting : CRBell2Setting
    {
        public Object root;

        public VRCExpressionsMenu asset_Menu;
        public VRCExpressionParameters asset_Parameters;

        public bool writeDefault = true;
    }
}