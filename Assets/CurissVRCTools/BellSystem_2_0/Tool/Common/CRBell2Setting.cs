using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;
using VRC.SDK3.Dynamics.Contact.Components;
using VRC.SDKBase;

namespace Curiss.CRBell2
{
    public class CRBell2Setting : MonoBehaviour, IEditorOnly
    {
        // Scene에 올라가있는 오브젝트
        public Transform bellTarget;
        public VRCContactReceiver sensorReceiver;
        public VRCContactReceiver touchReceicer;
        public AudioSource audioSource;

        // 에셋
        public RuntimeAnimatorController asset_bellFX;
        public AnimationClip asset_AnimClip_BellDelay_ON;

        // 타이틀 이미지
        public Texture2D titleImageContent;
        public Texture2D titleImageContent2;
        public Texture2D titleImageBackground;

        // 세팅 값
        public Transform targetBone;
        public float sensorRadious;
        public bool toggleDefault;
        public VRCExpressionsMenu targetMenu;

        public enum BellMode { Normal, TouchOnly }
        public BellMode defaultBellMode = BellMode.Normal;

        // 오디오 데이터
        [System.Serializable]
        public class AudioData
        {
            public AudioClip clip = null;
            public float playTime = 1;
            [Range(0, 1)] public float volum = 1;
        };
        public AudioData[] audioList = new AudioData[5];
        [Delayed] public int AudioCount = 5;
        [Range(0, 30)] public float audioMin = 0;
        [Range(0, 30)] public float audioMax = 3.5f;
    }
}