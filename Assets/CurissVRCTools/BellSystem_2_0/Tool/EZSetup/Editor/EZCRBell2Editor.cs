using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CRAssetEditorCore_v1_3;
using VRC.SDK3.Avatars.ScriptableObjects;
using VRC.SDK3.Avatars.Components;
using UnityEditor.Animations;
using VRC.SDK3.Dynamics.PhysBone.Components;


namespace Curiss.CRBell2
{
    [CustomEditor(typeof(EZCRBell2Setting))]
    public class EZCRBell2Editor : CRVRCAssetEditor
    {
        int toolbar = 0;

        public void OnSceneGUI()
        {
            EZCRBell2Setting script = (EZCRBell2Setting)target;

            // Range Gizmo.
            EditorGUI.BeginChangeCheck();
            Vector3 sysPosition = script.bellTarget.position;

            float range = Handles.RadiusHandle(Quaternion.Euler(Vector3.zero), sysPosition, script.sensorRadious * script.bellTarget.lossyScale.x);
            if (EditorGUI.EndChangeCheck())
            {
                script.sensorRadious = range;
            }
        }

        public override void OnInspectorGUI()
        {
            EZCRBell2Setting script = (EZCRBell2Setting)target;

            // 타이틀 이미지
            float height = 70;
            Rect rect = EditorGUILayout.GetControlRect(false, GUILayout.Height(height));

            if (script.titleImageBackground)
                GUI.DrawTexture(rect, script.titleImageBackground, ScaleMode.ScaleAndCrop);

            if (script.titleImageContent)
            {
                float content1Ratio = (float)script.titleImageContent.width / (float)script.titleImageContent.height;
                GUI.DrawTexture(new Rect(rect.x, rect.y, content1Ratio * height, height), script.titleImageContent);
            }

            if (script.titleImageContent2)
            {
                float content1Width = (float)script.titleImageContent.width / (float)script.titleImageContent.height * height;

                GUI.DrawTexture(new Rect(rect.x + content1Width, rect.y, rect.width - content1Width, rect.height), script.titleImageContent2, ScaleMode.ScaleToFit);
            }

            // 툴바
            toolbar = GUILayout.Toolbar(toolbar, new string[] { "Main", "Audio" });

            // 설정
            if (toolbar == 0) // Main
            {
                CREditorUtility.GuiLine();

                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(script.targetBone)), new GUIContent("Target Bone"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(script.sensorRadious)), new GUIContent("Sensor Radious"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(script.defaultBellMode)), new GUIContent("Default Bell Mode"));

                CREditorUtility.GuiLine();
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(script.toggleDefault)), new GUIContent("Default State"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(script.targetMenu)), new GUIContent("Expressions Menu"));

                CREditorUtility.GuiLine();

                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(script.writeDefault)), new GUIContent("Write Default"));

                CREditorUtility.GuiLine();
            }
            else // Audio
            {
                CREditorUtility.GuiLine();

                // 오디오 갯수.
                EditorGUI.BeginChangeCheck();
                SerializedProperty audioCountProperty = serializedObject.FindProperty(nameof(script.AudioCount));
                EditorGUILayout.PropertyField(audioCountProperty, new GUIContent("Audio Count"));
                if (EditorGUI.EndChangeCheck())
                {
                    if (audioCountProperty.intValue < 1)
                        audioCountProperty.intValue = 1;

                    if (audioCountProperty.intValue >= 10)
                        audioCountProperty.intValue = 10;

                    serializedObject.ApplyModifiedProperties();

                    System.Array.Resize(ref script.audioList, audioCountProperty.intValue);

                    return;
                }

                // 오디오 거리.
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(script.audioMin)), new GUIContent("Min Distance"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(script.audioMax)), new GUIContent("Max Distance"));

                CREditorUtility.GuiLine();

                // 오디오 목록.
                for (int i = 0; i < script.audioList.Length; i++)
                {
                    var listProperty = serializedObject.FindProperty(nameof(script.audioList));

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(listProperty.GetArrayElementAtIndex(i).FindPropertyRelative("clip"), new GUIContent("Clip"));
                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();
                        script.audioList[i].playTime = script.audioList[i].clip != null ? script.audioList[i].clip.length : 1.0f;
                    }

                    EditorGUILayout.PropertyField(listProperty.GetArrayElementAtIndex(i).FindPropertyRelative("playTime"), new GUIContent("Play Time"));
                    if (script.audioList[i].playTime < 0.1f)
                        script.audioList[i].playTime = 0.1f;

                    EditorGUILayout.PropertyField(listProperty.GetArrayElementAtIndex(i).FindPropertyRelative("volum"), new GUIContent("Volum"));

                    CREditorUtility.GuiLine();
                }
            }

            serializedObject.ApplyModifiedProperties();

            // 적용
            EditorGUI.BeginDisabledGroup(ErrorCheck());
            if (GUILayout.Button("Apply"))
            {
                CRVRCAssetSetting setting = new()
                {
                    root = script.root,
                    writeDefault = script.writeDefault,
                    assetFX = script.asset_bellFX,
                    targetMenu = script.targetMenu,
                    assetMenu = script.asset_Menu,
                    assetParameter = script.asset_Parameters
                };

                VRCAvatarDescriptor avatar = script.GetComponentInParent<VRCAvatarDescriptor>();

                if (avatar)
                {
                    bool result = Apply(avatar, setting);
                    if (result)
                    {
                        DestroyImmediate(script);
                        AssetDatabase.SaveAssets();
                    }
                }
            }
            EditorGUI.EndDisabledGroup();
        }

        bool ErrorCheck()
        {
            bool result = false;
            EZCRBell2Setting script = (EZCRBell2Setting)target;

            // 아바타
            VRCAvatarDescriptor avatar = script.GetComponentInParent<VRCAvatarDescriptor>();
            if (!avatar)
            {
                EditorGUILayout.HelpBox("This component need to be placed inside the avatar", MessageType.Error);
                result = true;
            }

            // TargetBone
            if (!script.targetBone)
            {
                EditorGUILayout.HelpBox("Target Bone is empty", MessageType.Error);
                result = true;
            }

            return result;
        }

        protected override bool ApplyPrefab(VRCAvatarDescriptor avatar, string dataPath)
        {
            EZCRBell2Setting script = (EZCRBell2Setting)target;

            // 프리펩 해체
            CREditorUtility.UnpackPrefab(script.gameObject);

            // 루트 위치 설정
            script.transform.parent = avatar.transform;
            script.gameObject.name = "SoundCatBell";

            // 타겟 부모 설정
            if (!script.bellTarget) return false;
                script.bellTarget.parent = script.targetBone;

            // 센서 크기
            if (!script.sensorReceiver) return false;
            script.sensorReceiver.radius = script.sensorRadious;
            if (!script.touchReceicer) return false;
            script.touchReceicer.radius = script.sensorRadious;

            // 오디오 설정
            if (script.audioSource)
            {
                script.audioSource.minDistance = script.audioMin;
                script.audioSource.maxDistance = script.audioMax;
                script.audioSource.GetComponent<VRCSpatialAudioSource>().Near = script.audioMin;
                script.audioSource.GetComponent<VRCSpatialAudioSource>().Far = script.audioMax;
            }

            // 피직스본 무시
            var physBones = script.targetBone.gameObject.GetComponentsInParent<VRCPhysBone>();
            for (int i = 0; i < physBones.Length; i++)
            {
                physBones[i].ignoreTransforms.Add(script.bellTarget);
                EditorUtility.SetDirty(physBones[i]);
            }

            return true;
        }

        protected override bool ApplyFX(VRCAvatarDescriptor avatar, RuntimeAnimatorController fx, string dataPath, bool writeDefault)
        {
            EZCRBell2Setting script = (EZCRBell2Setting)target;

            // FX 애니메이터 생성
            if (script.asset_bellFX)
            {
                // FX 파일 생성
                string newFXPass = dataPath + "/FXTemp.controller";
                newFXPass = AssetDatabase.GenerateUniqueAssetPath(newFXPass);
                AnimatorController cloneFX = (AnimatorController)CREditorUtility.CopyAsset(script.asset_bellFX, newFXPass);

                // 랜덤 변수 범위 설정
                AnimatorState endState = CRAnimatorControllerUtility.FindState(cloneFX.layers[1].stateMachine, "BellSound End");
                VRCAvatarParameterDriver pd = (VRCAvatarParameterDriver)endState.behaviours[0];
                pd.parameters[0].valueMax = script.audioList.Length - 1;
                EditorUtility.SetDirty(pd);

                // State 추가
                for (int i = 0; i < script.audioList.Length; i++)
                {
                    AnimatorState state = new()
                    {
                        name = "BellSound " + i
                    };
                    cloneFX.layers[1].stateMachine.AddState(state, Vector3.zero + new Vector3(300, 200 + i * 100, 0));

                    // PlayTime 설정
                    state.speed = 1.0f / script.audioList[i].playTime;

                    // Motion 설정
                    state.motion = script.asset_AnimClip_BellDelay_ON;

                    // VRC Audio 추가
                    VRCAnimatorPlayAudio vrcAudio = state.AddStateMachineBehaviour<VRCAnimatorPlayAudio>();
                    vrcAudio.Clips = new AudioClip[] { script.audioList[i].clip };
                    vrcAudio.Volume = new Vector2(script.audioList[i].volum, script.audioList[i].volum);

                    // VRC Audio - AudioSource 경로 설정
                    vrcAudio.SourcePath = CREditorUtility.GetGameObjectPath(script.audioSource.transform, avatar.transform);

                    // EntryTransition 추가
                    AnimatorTransition entryTransition = cloneFX.layers[1].stateMachine.AddEntryTransition(state);
                    entryTransition.AddCondition(AnimatorConditionMode.Equals, i, "Bell/RandomSound");

                    // Exit Transition 추가
                    AnimatorStateTransition exitTransition = state.AddTransition(endState);
                    exitTransition.hasExitTime = true;
                    exitTransition.exitTime = 0;
                    exitTransition.hasFixedDuration = false;
                    exitTransition.duration = 0;
                    exitTransition.offset = 0;
                    exitTransition.AddCondition(AnimatorConditionMode.If, 1.0f, "Bell/SoundTrigger");
                }

                fx = cloneFX;

                base.ApplyFX(avatar, fx, dataPath, writeDefault);

                // 임시 FX 제거
                AssetDatabase.DeleteAsset(newFXPass);
            }

            return true;
        }

        protected override bool ApplyParam(VRCAvatarDescriptor avatar, VRCExpressionParameters parameters, string dataPath)
        {
            EZCRBell2Setting script = (EZCRBell2Setting)target;

            VRCExpressionParameters.Parameter param;
            param = parameters.FindParameter("Bell/Toggle");
            param.defaultValue = script.toggleDefault ? 1 : 0;

            param = parameters.FindParameter("Bell/TouchMode");
            param.defaultValue = script.defaultBellMode == CRBell2Setting.BellMode.TouchOnly ? 1 : 0;

            return base.ApplyParam(avatar, parameters, dataPath);
        }
    }
}