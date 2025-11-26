using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using System.Linq;
using System;
using UnityEditorInternal;
using static DMCustom.PCSAudioManager;
using static DMCustom.PCSConfigurator;
using System.Reflection.Emit;
using System.Xml.Linq;
using System.Configuration;
using static UnityEngine.EventSystems.EventTrigger;
using System.Reflection;

/* MIT License (MIT)

“Copyright © <2023>, <Dismay Custom>

Permission is hereby granted, free of charge, to any person obtaining a copy of this
software and associated documentation files (the "Software"),to deal in the Software
without restriction, including without limitation the rights to use, copy, modify, merge,
publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to
whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or
substantial portions of the Software. */

namespace DMCustom
{
    public class PCSAudioManager : EditorWindow
    {
        //reference objects
        private static UnityEditor.Animations.AnimatorController animator;
        public static GUIStyle infoStyle;
        private Texture2D logo;
        private Vector2 scrollPosition = new(0, 300);
        private readonly int maxSFX = 10, maxVoice = 12;

        //Language
        private GUIStyle rightAlignedStyle;
        public enum Language
        {
            English,
            ภาษาไทย,
            日本語,
            한국어,
            繁體中文
        }
        private static Language currentLanguage = Language.English;
        private readonly Dictionary<string, string> english = new()
        {
            {"tip1", "Choose any sound type you want to replace the audio and drop your Audio Clip(s) into the field." },
            {"tip2", "This audio replacement cannot be undone. Please make sure before clicking Apply." },
            {"tip3", "Please drop your !PCS Controller_XXX Animator Controller into the box." },
            {"apply1", "Apply New Audio" },
            {"apply2", "Apply New Voice" },
            {"sound1","Sound Effect" },
            {"sound2","Voice Pack" },
            {"drop","Drag & Drop Audio(s) Here" },
            {"confirm","Apply Completed!\r\n\nThe new audio has been successfully applied to your animator. You can still manually edit it anytime inside the animator." },
        };
        private readonly Dictionary<string, string> thai = new()
        {
            {"tip1", "เลือกประเภทเสียงที่ต้องการแทนที่ แล้ววางคลิปเสียงลงในช่อง" },
            {"tip2", "การแทนที่เสียงนี้จะไม่สามารถย้อนกลับได้ โปรดเช็คความถูกต้องก่อนกดปุ่ม Apply" },
            {"tip3", "โปรดวาง !PCS Controller_XXX Animator Controller ของคุณลงในช่อง" },
            {"apply1", "เปลี่ยนเสียงเอฟเฟกต์" },
            {"apply2", "เปลี่ยนเสียงคราง" },
            {"sound1","เสียงเอฟเฟกต์" },
            {"sound2","เสียงคราง" },
            {"drop","ลากและวางเสียงที่นี่" },
            {"confirm","เปลี่ยนเสียงเรียบร้อยแล้ว!\r\n\nเสียงใหม่ถูกนำไปแทนที่เสียงเก่าเรียบร้อยแล้ว ถ้าอยากแก้ไขเองเพิ่มเติมก็ทำได้ในแอนิเมเตอร์ทุกเมื่อ" },
        };
        private readonly Dictionary<string, string> japanese = new()
        {
            {"tip1", "置き換えるサウンドタイプを選択し、オーディオクリップをフィールドに置いてください" },
            {"tip2", "このオーディオの置き換えは元に戻せません。Apply をクリックする前に必ず確認してください。" },
            {"tip3", "!PCS Controller_XXX Animator Controller をボックスに入れてください。" },
            {"apply1", "新しいオーディオを適用" },
            {"apply2", "新しいボイスを適用" },
            {"sound1","効果音" },
            {"sound2","ボイスパック" },
            {"drop","ここにドラッグ＆ドロップ" },
            {"confirm","適用完了！\r\n\n新しいオーディオがアニメーターに正常に適用されました。アニメーター内でいつでも手動で編集できます。" },
        };
        private readonly Dictionary<string, string> korean = new()
        {
            {"tip1", "오디오를 교체할 사운드 유형을 선택하고 오디오 클립을 필드에 놓으세요" },
            {"tip2", "이 오디오 교체는 되돌릴 수 없습니다. Apply 버튼을 누르기 전에 반드시 확인하세요." },
            {"tip3", "!PCS Controller_XXX Animator Controller 를 박스에 놓으세요." },
            {"apply1", "새 오디오 적용" },
            {"apply2", "새 음성 적용" },
            {"sound1","사운드 효과" },
            {"sound2","음성 팩" },
            {"drop","여기에 드래그 앤 드롭" },
            {"confirm","적용 완료!\r\n\n새 오디오가 애니메이터에 성공적으로 적용되었습니다. 애니메이터에서 언제든 직접 수정할 수 있습니다." },
        };
        private readonly Dictionary<string, string> chinese = new()
        {
            {"tip1", "選擇要替換音訊的聲音類型，並將音訊剪輯放到欄位中" },
            {"tip2", "此音訊替換無法復原。請在按下 Apply 前務必確認。" },
            {"tip3", "請將您的 !PCS Controller_XXX Animator Controller 拖放到框中。" },
            {"apply1", "套用新音訊" },
            {"apply2", "套用新語音" },
            {"sound1","音效" },
            {"sound2","語音包" },
            {"drop","拖曳到此處" },
            {"confirm","套用完成！\r\n\n新音訊已成功套用到動畫控制器。您仍可在動畫控制器內隨時手動編輯。" },
        };
        private string L(string key)
        {
            return currentLanguage switch
            {
                Language.English => english.TryGetValue(key, out var valEng) ? valEng : key,
                Language.日本語 => japanese.TryGetValue(key, out var valJpn) ? valJpn : key,
                Language.ภาษาไทย => thai.TryGetValue(key, out var valTha) ? valTha : key,
                Language.한국어 => korean.TryGetValue(key, out var valKor) ? valKor : key,
                Language.繁體中文 => chinese.TryGetValue(key, out var valCn) ? valCn : key,
                _ => key
            };
        }
        public static AnimatorState FindAnimatorStateByName(string nameToFind, UnityEditor.Animations.AnimatorController controller)
        {
            AnimatorState resultState;
            UnityEditor.Animations.AnimatorControllerLayer[] acLayers = controller.layers;
            List<AnimatorState> allStates = new();
            foreach (UnityEditor.Animations.AnimatorControllerLayer i in acLayers)
            {
                ChildAnimatorState[] animStates = i.stateMachine.states;
                foreach (ChildAnimatorState j in animStates)
                {
                    allStates.Add(j.state);
                    if (j.state.name == nameToFind)
                    {
                        resultState = j.state;
                        return resultState;
                    }
                }
            }
            return null;
        }
        private static AudioClip[] VerifyAudioClips(AudioClip[] clips, int select) //To remove empty array slot and keep only the existings.
        {
            var temp_clips = new AudioClip[20];

            for (int i = 0; i < select; i++)
            {
                if (clips[i] != null)
                {
                    temp_clips[i] = clips[i];
                }
            }

            for (int o = temp_clips.Length - 1; o > select; o--)
            {
                RemoveAt(ref temp_clips, o);
            }
            temp_clips = temp_clips.Where(x => x != null).ToArray();
            return temp_clips;
        }
        private static void RemoveAt<T>(ref T[] arr, int index) //a function for VerifyAudioClips()
        {
            for (int a = index; a < arr.Length - 1; a++)
            {
                arr[a] = arr[a + 1];
            }
            Array.Resize(ref arr, arr.Length - 1);
        } 

        //pcs sound type
        public enum SoundType
        {
            Mouth,
            Boobs,
            Pussy,
            Ass,
        }
        public enum VoiceType
        {
            Voice, Oral
        }

        //sfx sub types
        public enum Mouth_SubType { In, Out, Smash, Exit, Gagging }
        public enum Boobs_SubType { In, Out, Smash }
        public enum Pussy_SubType { In, Out, Exit, SoftSmash, MediumSmash, HardSmash }
        public enum Ass_SubType { In, Out, Exit, SoftSmash, MediumSmash, HardSmash }
        public enum Voice_SubType { Soft, Rough, Relax, Events, Combo1, Combo2, Combo3 }
        public enum Oral_SubType { Shallow, Deep}
        private string[] GetSFXSubTypes(SoundType type)
        {
            return type switch
            {
                SoundType.Mouth => System.Enum.GetNames(typeof(Mouth_SubType)),
                SoundType.Boobs => System.Enum.GetNames(typeof(Boobs_SubType)),
                SoundType.Pussy => System.Enum.GetNames(typeof(Pussy_SubType)),
                SoundType.Ass => System.Enum.GetNames(typeof(Ass_SubType)),
                _ => new string[0],
            };
        }
        private string[] GetVoiceSubTypes(VoiceType type)
        {
            return type switch
            {
                VoiceType.Voice => System.Enum.GetNames(typeof(Voice_SubType)),
                VoiceType.Oral => System.Enum.GetNames(typeof(Oral_SubType)),
                _ => new string[0],
            };
        }

        [System.Serializable]
        //SFX
        public class SoundEffectEntry
        {
            public SoundType soundType;
            public int subTypeIndex;
            public AudioClip audioClip;
            public int audioClipAmount;
            public List<AudioClip> audioClips = new();
        }
        private readonly List<SoundEffectEntry> soundEffects = new();
        private ReorderableList reorderableList;

        //Voice
        public class VoiceEffectEntry
        {
            public VoiceType voiceType;
            public int subTypeIndex;
            public AudioClip audioClip;
            public int audioClipAmount;
            public List<AudioClip> audioClips = new();
        }
        private readonly List<VoiceEffectEntry> voiceEfects = new();
        private ReorderableList reorderableList_voice;

        [MenuItem("Dismay Custom/Penetration Contact System/Audio Manager")]
        public static void ShowpWindow()
        {
            var window = GetWindow(typeof(PCSAudioManager));

            window.titleContent = new GUIContent("PCS: Audio Manager");
            Rect main = EditorGUIUtility.GetMainWindowPosition();
            Rect pos = window.position;
            float centerWidth = (main.width - pos.width) * 0.5f;
            float centerHeight = (main.height - pos.height) * 0.3f;
            pos.x = main.x + centerWidth; //+ 360/2;
            pos.y = main.y + centerHeight;
            window.position = pos;
            window.minSize = new Vector2(512, 700);
            window.maxSize = new Vector2(512, 960);
            window.Show();
        }
        private void OnEnable()
        {
            OrderList_SFX();
            OrderList_Voice();
        }
        private void OrderList_SFX()
        {
            reorderableList = new(soundEffects, typeof(SoundEffectEntry), true, true, true, true)
            {
                drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, L("sound1"));
            },

                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var element = soundEffects[index];
                    rect.y += 2;

                    float colWidth = rect.width / 4;

                    EditorGUI.BeginChangeCheck();
                    //sound type
                    element.soundType = (SoundType)EditorGUI.EnumPopup(
                        new Rect(rect.x, rect.y, colWidth, EditorGUIUtility.singleLineHeight),
                        element.soundType);

                    //sub type
                    string[] subTypes = GetSFXSubTypes(element.soundType);
                    element.subTypeIndex = EditorGUI.Popup(
                        new Rect(rect.x + (colWidth + 3), rect.y, colWidth, EditorGUIUtility.singleLineHeight),
                        element.subTypeIndex,
                        subTypes);

                    if (subTypes == null || subTypes.Length == 0)
                    {
                        element.subTypeIndex = -1;
                    }
                    else
                    {
                        if (element.subTypeIndex < 0 || element.subTypeIndex >= subTypes.Length)
                        {
                            element.subTypeIndex = 0;
                        }
                    }

                    //drop Area
                    Rect dropArea = new(rect.x + (colWidth + 3) * 2, rect.y, colWidth * 1.95f, EditorGUIUtility.singleLineHeight);
                    GUI.Box(dropArea, L("drop"), "AvatarMappingBox");
                    HandleDragAndDrop_SFX(dropArea, element);

                    //if changed, resize              
                    element.audioClipAmount = element.audioClips.Count;

                    while (element.audioClips.Count < element.audioClipAmount)
                        element.audioClips.Add(null);

                    while (element.audioClips.Count > element.audioClipAmount)
                        element.audioClips.RemoveAt(element.audioClips.Count - 1);

                    EditorGUI.BeginChangeCheck();
                    //draw audio clips sllot
                    for (int i = 0; i < element.audioClips.Count; i++)
                    {
                        Rect clipRect = new(
                            rect.x,
                            rect.y + (i + 1) * (EditorGUIUtility.singleLineHeight + 2),
                            rect.width,
                            EditorGUIUtility.singleLineHeight);

                        element.audioClips[i] = (AudioClip)EditorGUI.ObjectField(
                            clipRect,
                            $"#{i + 1}",
                            element.audioClips[i],
                            typeof(AudioClip),
                            false);
                    }
                    if (EditorGUI.EndChangeCheck())
                    {
                        for (int i = 0; i < element.audioClips.Count; i++)
                        {
                            if (element.audioClips[i] == null)
                            {
                                element.audioClips.RemoveAt(i);

                            }
                        }

                        if (element.audioClips.Count == 0)
                        {
                            element.audioClips.Add(null);
                        }
                    }
                },

                elementHeightCallback = index =>
                {
                    var element = soundEffects[index];
                    return EditorGUIUtility.singleLineHeight * (1 + element.audioClips.Count) + (2 * (1 + element.audioClips.Count));
                },

                onAddCallback = list =>
                {
                    soundEffects.Add(new SoundEffectEntry()
                    {
                        soundType = SoundType.Mouth,
                        subTypeIndex = 0,
                        audioClipAmount = 1,
                        audioClips = new List<AudioClip>() { null }
                    });
                }
            };
        }
        private void OrderList_Voice()
        {
            reorderableList_voice = new ReorderableList(voiceEfects, typeof(VoiceEffectEntry), true, true, true, true)
            {
                drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, L("sound2"));
            },

                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var element = voiceEfects[index];
                    rect.y += 2;

                    float colWidth = rect.width / 4;

                    //sound type
                    element.voiceType = (VoiceType)EditorGUI.EnumPopup(
                        new Rect(rect.x, rect.y, colWidth, EditorGUIUtility.singleLineHeight),
                        element.voiceType);

                    //sub type
                    string[] subTypes = GetVoiceSubTypes(element.voiceType);
                    element.subTypeIndex = EditorGUI.Popup(
                        new Rect(rect.x + (colWidth + 3), rect.y, colWidth, EditorGUIUtility.singleLineHeight),
                        element.subTypeIndex,
                        subTypes);

                    if (subTypes == null || subTypes.Length == 0)
                    {
                        element.subTypeIndex = -1;
                    }
                    else
                    {
                        if (element.subTypeIndex < 0 || element.subTypeIndex >= subTypes.Length)
                        {
                            element.subTypeIndex = 0;
                        }
                    }

                    //drop Area
                    Rect dropArea = new(rect.x + (colWidth + 3) * 2, rect.y, colWidth * 1.95f, EditorGUIUtility.singleLineHeight);
                    GUI.Box(dropArea, L("drop"), "AvatarMappingBox");
                    HandleDragAndDrop_Voice(dropArea, element);

                    //if changed, resize              
                    element.audioClipAmount = element.audioClips.Count;

                    while (element.audioClips.Count < element.audioClipAmount)
                        element.audioClips.Add(null);

                    while (element.audioClips.Count > element.audioClipAmount)
                        element.audioClips.RemoveAt(element.audioClips.Count - 1);

                    EditorGUI.BeginChangeCheck();
                    //draw audio clips sllot
                    for (int i = 0; i < element.audioClips.Count; i++)
                    {
                        Rect clipRect = new(
                            rect.x,
                            rect.y + (i + 1) * (EditorGUIUtility.singleLineHeight + 2),
                            rect.width,
                            EditorGUIUtility.singleLineHeight);

                        element.audioClips[i] = (AudioClip)EditorGUI.ObjectField(
                            clipRect,
                            $"#{i + 1}",
                            element.audioClips[i],
                            typeof(AudioClip),
                            false);
                    }
                    if (EditorGUI.EndChangeCheck())
                    {
                        for (int i = 0; i < element.audioClips.Count; i++)
                        {
                            if (element.audioClips[i] == null)
                            {
                                element.audioClips.RemoveAt(i);

                            }
                        }

                        if (element.audioClips.Count == 0)
                        {
                            element.audioClips.Add(null);
                        }
                    }
                },

                elementHeightCallback = index =>
                {
                    var element = voiceEfects[index];
                    return EditorGUIUtility.singleLineHeight * (1 + element.audioClips.Count) + (2 * (1 + element.audioClips.Count));
                },

                onAddCallback = list =>
                {
                    voiceEfects.Add(new VoiceEffectEntry()
                    {
                        voiceType = VoiceType.Voice,
                        subTypeIndex = 0,
                        audioClipAmount = 1,
                        audioClips = new List<AudioClip>() { null }
                    });
                }
            };
        }
        private void OnGUI()
        {
            infoStyle = new GUIStyle()
            {
                fontSize = 10,
                fontStyle = FontStyle.Normal,
                normal = { textColor = new Color(0.5f, 0.5f, 0.5f) },
                alignment = TextAnchor.LowerLeft
            };

            rightAlignedStyle = new(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleRight
            };

            Rect enumRect = new(position.width - 75 - 10, 10, 75, 18);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            logo = Resources.Load<Texture2D>("Components/" + "PCS_ChangeSFX" + "_banner");
            GUILayout.Label(logo, new GUIStyle { fixedWidth = 512, fixedHeight = 115 });
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            currentLanguage = (Language)EditorGUI.EnumPopup(enumRect, currentLanguage, rightAlignedStyle);

            animator = EditorGUILayout.ObjectField(animator, typeof(UnityEditor.Animations.AnimatorController), true, GUILayout.Height(30)) as UnityEditor.Animations.AnimatorController;

            if (animator == null)
            {
                EditorGUILayout.HelpBox(L("tip3"), MessageType.Warning);
            }

            if(animator != null)
            {
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);

                reorderableList?.DoLayoutList();
                reorderableList_voice?.DoLayoutList();

                GUILayout.EndScrollView();

                int elementCount1 = soundEffects.Count;
                if (elementCount1 == 0 || animator == null)
                {
                    GUI.enabled = false;
                }
                else
                {
                    GUI.enabled = true;
                }

                GUILayout.BeginHorizontal();
                if (GUILayout.Button(L("apply1")))
                {
                    if (elementCount1 > 0)
                    {
                        for (int i = 0; i < elementCount1; i++)
                        {
                            ApplyNewSFX(i);
                            EditorUtility.SetDirty(animator);
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                        }
                        EditorUtility.DisplayDialog("Penetration Contact System", L("confirm"), "OK");
                    }
                }

                int elementCount2 = voiceEfects.Count;
                if (elementCount2 == 0 || animator == null)
                {
                    GUI.enabled = false;
                }
                else
                {
                    GUI.enabled = true;
                }
                if (GUILayout.Button(L("apply2")))
                {
                    if (elementCount2 > 0)
                    {
                        for (int i = 0; i < elementCount2; i++)
                        {
                            ApplyNewVoice(i);
                            EditorUtility.SetDirty(animator);
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                        }
                        EditorUtility.DisplayDialog("Penetration Contact System", L("confirm"), "ON");
                    }
                }
                GUILayout.EndHorizontal();
                GUI.enabled = true;
                EditorGUILayout.HelpBox(L("tip1") +" "+ L("tip2"), MessageType.Info);            
            }
            ShowFooter();
        }
        private void HandleDragAndDrop_SFX(Rect dropArea, SoundEffectEntry element)
        {
            Event evt = Event.current;

            if (evt.type == EventType.DragUpdated || evt.type == EventType.DragPerform)
            {
                if (!dropArea.Contains(evt.mousePosition))
                    return;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    element.audioClips.Clear();

                    int clipsToAdd = Mathf.Min(DragAndDrop.objectReferences.Length, maxSFX);

                    for (int i = 0; i < clipsToAdd; i++)
                    {
                        if (DragAndDrop.objectReferences[i] is AudioClip clip)
                        {
                            element.audioClips.Add(clip);
                        }
                    }

                    evt.Use();
                }
            }
        }
        private void HandleDragAndDrop_Voice(Rect dropArea, VoiceEffectEntry element)
        {
            Event evt = Event.current;

            if (evt.type == EventType.DragUpdated || evt.type == EventType.DragPerform)
            {
                if (!dropArea.Contains(evt.mousePosition))
                    return;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    element.audioClips.Clear();

                    int clipsToAdd = Mathf.Min(DragAndDrop.objectReferences.Length, maxVoice);

                    for (int i = 0; i < clipsToAdd; i++)
                    {
                        if (DragAndDrop.objectReferences[i] is AudioClip clip)
                        {
                            element.audioClips.Add(clip);
                        }
                    }

                    evt.Use();
                }
            }
        }
        private void ShowFooter()
        {
            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            var info = Resources.Load<TextAsset>("Components/Penetration Contact System_info").ToString();
            GUILayout.Label(info.Replace("$", "v" + PCSConfigurator.version), infoStyle, GUILayout.Width(285));

            if (GUILayout.Button("Tutorial"))
            {
                Application.OpenURL("https://docs.google.com/document/d/1br6pOHx9P6T52AO1eKCw3VrfSUGKJOS_NA_MIwUNjWY/edit?usp=sharing");
            }
            if (GUILayout.Button("Discord"))
            {
                Application.OpenURL("https://discord.gg/TkfRyQDNQC");
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.Space(5);
        }
        private float AudioDurationToAnimSpeed(float duration)
        {
            float speed = 1/duration;
            return speed;
        }
        //Get each elements
        public void ApplyNewSFX(int index)
        {
            if (index < 0 || index >= soundEffects.Count)
            {
                Debug.LogWarning("PCS: There is no SFX element.");
                return;
            }

            //entry parameters
            SoundEffectEntry entry = soundEffects[index];

            SoundType soundType = entry.soundType;
            int subTypeIndex = entry.subTypeIndex;
            List<AudioClip> clips = entry.audioClips;

            /*TODO step
            1. Find AnimatorState by name from animator
            2. Reference VRC AnimatorAudio beavior
            3. Set new audio clip
            */

            if(soundType == SoundType.Mouth)
            {
                if(subTypeIndex == 0)
                {
                    var state_mouth_in = FindAnimatorStateByName("state_mouth_in", animator);
                    var audioPlayer = state_mouth_in.behaviours[0] as VRCAnimatorPlayAudio;
                    if (clips.ElementAt(0) != null)
                    {
                        audioPlayer.Clips = clips.ToArray();
                    }
                }
                else if (subTypeIndex == 1)
                {
                    var state_mouth_out = FindAnimatorStateByName("state_mouth_out", animator);
                    var audioPlayer = state_mouth_out.behaviours[0] as VRCAnimatorPlayAudio;
                    if (clips.ElementAt(0) != null)
                    {
                        audioPlayer.Clips = clips.ToArray();
                    }
                }
                else if (subTypeIndex == 2)
                {
                    var state_mouth_smash = FindAnimatorStateByName("state_mouth_smash", animator);
                    var audioPlayer = state_mouth_smash.behaviours[0] as VRCAnimatorPlayAudio;
                    if (clips.ElementAt(0) != null)
                    {
                        audioPlayer.Clips = clips.ToArray();
                    }
                }
                else if (subTypeIndex == 3)
                {
                    var state_mouth_allExit = FindAnimatorStateByName("state_mouth_allExit", animator);
                    var audioPlayer = state_mouth_allExit.behaviours[0] as VRCAnimatorPlayAudio;
                    if (clips.ElementAt(0) != null)
                    {
                        audioPlayer.Clips = clips.ToArray();
                    }
                }
                else if (subTypeIndex == 4)
                {
                    var state_mouth_deep = FindAnimatorStateByName("state_mouth_deep", animator);
                    var audioPlayer = state_mouth_deep.behaviours[0] as VRCAnimatorPlayAudio;
                    if (clips.ElementAt(0) != null)
                    {
                        audioPlayer.Clips = clips.ToArray();
                    }
                }
            }

            if (soundType == SoundType.Boobs)
            {
                if (subTypeIndex == 0)
                {
                    var state_boobs_in = FindAnimatorStateByName("state_boobs_in", animator);
                    var audioPlayer = state_boobs_in.behaviours[0] as VRCAnimatorPlayAudio;
                    if (clips.ElementAt(0) != null)
                    {
                        audioPlayer.Clips = clips.ToArray();
                    }
                }
                else if (subTypeIndex == 1)
                {
                    var state_boobs_out = FindAnimatorStateByName("state_boobs_out", animator);
                    var audioPlayer = state_boobs_out.behaviours[0] as VRCAnimatorPlayAudio;
                    if (clips.ElementAt(0) != null)
                    {
                        audioPlayer.Clips = clips.ToArray();
                    }
                }
                else if (subTypeIndex == 2)
                {
                    var state_boobs_smash = FindAnimatorStateByName("state_boobs_smash", animator);
                    var audioPlayer = state_boobs_smash.behaviours[0] as VRCAnimatorPlayAudio;
                    if (clips.ElementAt(0) != null)
                    {
                        audioPlayer.Clips = clips.ToArray();
                    }
                }
            }

            if (soundType == SoundType.Pussy)
            {
                if (subTypeIndex == 0)
                {
                    var state_pussy_in = FindAnimatorStateByName("state_pussy_in", animator);
                    var audioPlayer = state_pussy_in.behaviours[0] as VRCAnimatorPlayAudio;
                    if (clips.ElementAt(0) != null)
                    {
                        audioPlayer.Clips = clips.ToArray();
                    }
                }
                else if (subTypeIndex == 1)
                {
                    var state_pussy_out = FindAnimatorStateByName("state_pussy_out", animator);
                    var audioPlayer = state_pussy_out.behaviours[0] as VRCAnimatorPlayAudio;
                    if (clips.ElementAt(0) != null)
                    {
                        audioPlayer.Clips = clips.ToArray();
                    }
                }
                else if (subTypeIndex == 2)
                {
                    var state_pussy_allExit = FindAnimatorStateByName("state_pussy_allExit", animator);
                    var audioPlayer = state_pussy_allExit.behaviours[0] as VRCAnimatorPlayAudio;
                    if (clips.ElementAt(0) != null)
                    {
                        audioPlayer.Clips = clips.ToArray();
                    }
                }
                else if (subTypeIndex == 3)
                {
                    var state_pussy_smash1 = FindAnimatorStateByName("pussy_smash_soft", animator);
                    var audioPlayer = state_pussy_smash1.behaviours[0] as VRCAnimatorPlayAudio;
                    if (clips.ElementAt(0) != null)
                    {
                        audioPlayer.Clips = clips.ToArray();
                    }
                }
                else if (subTypeIndex == 4)
                {
                    var state_pussy_smash2 = FindAnimatorStateByName("pussy_smash_medium", animator);
                    var audioPlayer = state_pussy_smash2.behaviours[0] as VRCAnimatorPlayAudio;
                    if (clips.ElementAt(0) != null)
                    {
                        audioPlayer.Clips = clips.ToArray();
                    }
                }
                else if (subTypeIndex == 5)
                {
                    var state_pussy_smash3 = FindAnimatorStateByName("pussy_smash_hard", animator);
                    var audioPlayer = state_pussy_smash3.behaviours[0] as VRCAnimatorPlayAudio;
                    if (clips.ElementAt(0) != null)
                    {
                        audioPlayer.Clips = clips.ToArray();
                    }
                }
            }

            if (soundType == SoundType.Ass)
            {
                if (subTypeIndex == 0)
                {
                    var state_ass_in = FindAnimatorStateByName("state_ass_in", animator);
                    var audioPlayer = state_ass_in.behaviours[0] as VRCAnimatorPlayAudio;
                    if (clips.ElementAt(0) != null)
                    {
                        audioPlayer.Clips = clips.ToArray();
                    }
                }
                else if (subTypeIndex == 1)
                {
                    var state_ass_out = FindAnimatorStateByName("state_ass_out", animator);
                    var audioPlayer = state_ass_out.behaviours[0] as VRCAnimatorPlayAudio;
                    if (clips.ElementAt(0) != null)
                    {
                        audioPlayer.Clips = clips.ToArray();
                    }                      
                }
                else if (subTypeIndex == 2)
                {
                    var state_ass_allExit = FindAnimatorStateByName("state_ass_allExit", animator);
                    var audioPlayer = state_ass_allExit.behaviours[0] as VRCAnimatorPlayAudio;
                    if(clips.ElementAt(0) != null)
                    {
                        audioPlayer.Clips = clips.ToArray();
                    }                
                }
                else if (subTypeIndex == 3)
                {
                    var state_ass_smash1 = FindAnimatorStateByName("ass_smash_soft", animator);
                    var audioPlayer = state_ass_smash1.behaviours[0] as VRCAnimatorPlayAudio;
                    if (clips.ElementAt(0) != null)
                    {
                        audioPlayer.Clips = clips.ToArray();
                    }
                }
                else if (subTypeIndex == 4)
                {
                    var state_ass_smash2 = FindAnimatorStateByName("ass_smash_medium", animator);
                    var audioPlayer = state_ass_smash2.behaviours[0] as VRCAnimatorPlayAudio;
                    if (clips.ElementAt(0) != null)
                    {
                        audioPlayer.Clips = clips.ToArray();
                    }
                }
                else if (subTypeIndex == 5)
                {
                    var state_ass_smash3 = FindAnimatorStateByName("ass_smash_hard", animator);
                    var audioPlayer = state_ass_smash3.behaviours[0] as VRCAnimatorPlayAudio;
                    if (clips.ElementAt(0) != null)
                    {
                        audioPlayer.Clips = clips.ToArray();
                    }
                }
            }
        }
        public void ApplyNewVoice(int index)
        {
            if (index < 0 || index >= voiceEfects.Count)
            {
                Debug.LogWarning("PCS: There is no SFX element.");
                return;
            }

            //entry parameters
            VoiceEffectEntry entry = voiceEfects[index];

            VoiceType soundType = entry.voiceType;
            int subTypeIndex = entry.subTypeIndex;
            List<AudioClip> clips = entry.audioClips;

            if (soundType == VoiceType.Voice)
            {
                if (subTypeIndex == 0) //soft
                {
                    var state = FindAnimatorStateByName("Soft Moan", animator);
                    var audioPlayer = state.behaviours[0] as VRCAnimatorPlayAudio;
                    if (clips.ElementAt(0) != null)
                    {
                        audioPlayer.Clips = clips.ToArray();
                    }
                }
                else if (subTypeIndex == 1) //rough
                {
                    var state = FindAnimatorStateByName("Rough Moan", animator);
                    var audioPlayer = state.behaviours[0] as VRCAnimatorPlayAudio;
                    if (clips.ElementAt(0) != null)
                    {
                        audioPlayer.Clips = clips.ToArray();
                    }
                }
                else if (subTypeIndex == 2) //relax
                {
                    var state = FindAnimatorStateByName("Voice Relax", animator);
                    var audioPlayer = state.behaviours[0] as VRCAnimatorPlayAudio;
                    if (clips.ElementAt(0) != null)
                    {
                        audioPlayer.Clips = clips.ToArray();
                    }
                }
                else if (subTypeIndex == 3) //event
                {
                    AnimatorState[] state = new AnimatorState[12];
                    AudioClip[] empty_audio = new AudioClip[1];
                    VRCAnimatorPlayAudio[] audio = new VRCAnimatorPlayAudio[12];
                    
                    for (int i = 0; i < state.Length; i++)
                    {
                        state[i] = FindAnimatorStateByName("Event " + (i + 1), animator);
                        audio[i] = state[i].behaviours[0] as VRCAnimatorPlayAudio;
                        audio[i].Clips = empty_audio;
                    }
                   
                    AudioClip[] event1 = new AudioClip[1];
                    AudioClip[] event2 = new AudioClip[1];
                    AudioClip[] event3 = new AudioClip[1];
                    AudioClip[] event4 = new AudioClip[1];
                    AudioClip[] event5 = new AudioClip[1];
                    AudioClip[] event6 = new AudioClip[1];
                    AudioClip[] event7 = new AudioClip[1];
                    AudioClip[] event8 = new AudioClip[1];
                    AudioClip[] event9 = new AudioClip[1];
                    AudioClip[] event10 = new AudioClip[1];
                    AudioClip[] event11 = new AudioClip[1];
                    AudioClip[] event12 = new AudioClip[1];

                    if(clips.ElementAt(0) != null) //if at least 1 slot has audio
                    {
                        AudioClip[] temp_audio = new AudioClip[12];

                        for (int i = 0;  i < clips.Count; i++)
                        {
                            temp_audio[i] = clips.ElementAt(i);
                        }

                        event1[0] = temp_audio.ElementAt(0);
                        audio[0].Clips = event1;
                        if (temp_audio.ElementAt(0) != null)
                        {
                            state[01].speed = AudioDurationToAnimSpeed(temp_audio.ElementAt(0).length);
                        }
                        else
                        {
                            state[0].speed = 1;
                        }

                        event2[0] = temp_audio.ElementAt(1);
                        audio[1].Clips = event2;
                        if (temp_audio.ElementAt(1) != null)
                        {
                            state[1].speed = AudioDurationToAnimSpeed(temp_audio.ElementAt(1).length);
                        }
                        else
                        {
                            state[1].speed = 1;
                        }

                        event3[0] = temp_audio.ElementAt(2);
                        audio[2].Clips = event3;
                        if (temp_audio.ElementAt(2) != null)
                        {
                            state[2].speed = AudioDurationToAnimSpeed(temp_audio.ElementAt(2).length);
                        }
                        else
                        {
                            state[2].speed = 1;
                        }
                        event4[0] = temp_audio.ElementAt(3);
                        audio[3].Clips = event4;
                        if (temp_audio.ElementAt(3) != null)
                        {
                            state[3].speed = AudioDurationToAnimSpeed(temp_audio.ElementAt(3).length);
                        }
                        else
                        {
                            state[3].speed = 1;
                        }

                        event5[0] = temp_audio.ElementAt(4);
                        audio[4].Clips = event5;
                        if (temp_audio.ElementAt(4) != null)
                        {
                            state[4].speed = AudioDurationToAnimSpeed(temp_audio.ElementAt(4).length);
                        }
                        else
                        {
                            state[4].speed = 1;
                        }

                        event6[0] = temp_audio.ElementAt(5);
                        audio[5].Clips = event6;
                        if (temp_audio.ElementAt(5) != null)
                        {
                            state[5].speed = AudioDurationToAnimSpeed(temp_audio.ElementAt(5).length);
                        }
                        else
                        {
                            state[5].speed = 1;
                        }

                        event7[0] = temp_audio.ElementAt(6);
                        audio[6].Clips = event7;
                        if (temp_audio.ElementAt(6) != null)
                        {
                            state[6].speed = AudioDurationToAnimSpeed(temp_audio.ElementAt(6).length);
                        }
                        else
                        {
                            state[6].speed = 1;
                        }

                        event8[0] = temp_audio.ElementAt(7);
                        audio[7].Clips = event8;
                        if (temp_audio.ElementAt(7) != null)
                        {
                            state[7].speed = AudioDurationToAnimSpeed(temp_audio.ElementAt(7).length);
                        }
                        else
                        {
                            state[7].speed = 1;
                        }

                        event9[0] = temp_audio.ElementAt(8);
                        audio[8].Clips = event9;
                        if (temp_audio.ElementAt(8) != null)
                        {
                            state[8].speed = AudioDurationToAnimSpeed(temp_audio.ElementAt(8).length);
                        }
                        else
                        {
                            state[8].speed = 1;
                        }

                        event10[0] = temp_audio.ElementAt(9);
                        audio[9].Clips = event10;
                        if (temp_audio.ElementAt(9) != null)
                        {
                            state[9].speed = AudioDurationToAnimSpeed(temp_audio.ElementAt(9).length);
                        }
                        else
                        {
                            state[9].speed = 1;
                        }

                        event11[0] = temp_audio.ElementAt(10);
                        audio[10].Clips = event11;
                        if (temp_audio.ElementAt(10) != null)
                        {
                            state[10].speed = AudioDurationToAnimSpeed(temp_audio.ElementAt(10).length);
                        }
                        else
                        {
                            state[10].speed = 1;
                        }

                        event12[0] = temp_audio.ElementAt(11);
                        audio[11].Clips = event12;
                        if (temp_audio.ElementAt(11) != null)
                        {
                            state[11].speed = AudioDurationToAnimSpeed(temp_audio.ElementAt(11).length);
                        }
                        else
                        {
                            state[11].speed = 1;
                        }
                    }                                
                }
                else if (subTypeIndex == 4) //combo 1
                {
                    var state1 = FindAnimatorStateByName("Combo Start 1", animator);
                    var state2 = FindAnimatorStateByName("Combo End 1", animator);

                    var audioPlayer1 = state1.behaviours[0] as VRCAnimatorPlayAudio;
                    var audioPlayer2 = state2.behaviours[0] as VRCAnimatorPlayAudio;

                    AudioClip[] empty_audio= new AudioClip[1];

                    if (clips.ElementAt(0) != null)
                    {
                        audioPlayer1.Clips = clips.ToArray();
                        empty_audio[0] = clips.ElementAt(clips.Count - 1);
                        audioPlayer2.Clips = empty_audio.ToArray();
                        state2.speed = AudioDurationToAnimSpeed(empty_audio.ElementAt(0).length);
                    }              
                }
            }
            if (soundType == VoiceType.Oral)
            {
                if (subTypeIndex == 0) //soft
                {
                    var state = FindAnimatorStateByName("Oral Soft", animator);
                    var audioPlayer = state.behaviours[0] as VRCAnimatorPlayAudio;
                    if (clips.ElementAt(0) != null)
                    {
                        audioPlayer.Clips = clips.ToArray();
                    }
                }
                else if (subTypeIndex == 1) //rough
                {
                    var state = FindAnimatorStateByName("Oral Rough", animator);
                    var audioPlayer = state.behaviours[0] as VRCAnimatorPlayAudio;
                    if (clips.ElementAt(0) != null)
                    {
                        audioPlayer.Clips = clips.ToArray();
                    }
                }
            }
        }

        //Method for Voice Pack Setup
        private static readonly AnimatorState[] voice_state_event = new AnimatorState[12];
        private static AnimatorState voice_softMoan, voice_roughMoan, voice_relax, voice_oralSoft, voice_oralRough;
        private static AudioClip[] voice_combo_clip1, voice_combo_clip2 = new AudioClip[1], voice_combo_clip3 = new AudioClip[1];
        public static void FinalizeVoicePack(UnityEditor.Animations.AnimatorController controller)
        {
            #region Event
            VRCAnimatorPlayAudio[] voice_event = new VRCAnimatorPlayAudio[12];
            AudioClip[] voice_event_clip = new AudioClip[12];
            AudioClip[] voice_event_c1 = new AudioClip[1], voice_event_c2 = new AudioClip[1], voice_event_c3 = new AudioClip[1],
            voice_event_c4 = new AudioClip[1], voice_event_c5 = new AudioClip[1], voice_event_c6 = new AudioClip[1],
            voice_event_c7 = new AudioClip[1], voice_event_c8 = new AudioClip[1], voice_event_c9 = new AudioClip[1],
            voice_event_c10 = new AudioClip[1], voice_event_c11 = new AudioClip[1], voice_event_c12 = new AudioClip[1];

            for (int i = 0; i < voice_state_event.Length; i++)
            {
                voice_state_event[i] = FindAnimatorStateByName("Event " + (i + 1), controller);
                voice_event[i] = voice_state_event[i].behaviours[0] as VRCAnimatorPlayAudio;
                voice_event_clip[i] = (AudioClip)AssetDatabase.LoadAssetAtPath("Assets/!Dismay Custom/Penetration Contact System/Assets/Voice Pack/" + PCSConfigurator.voicePack.ToString() + "/Event/moan_event (" + (i + 1) + ").wav", typeof(AudioClip));
                voice_state_event[i].speed = 1 / voice_event_clip[i].length;
            }

            voice_event_c1[0] = voice_event_clip[0];
            voice_event_c2[0] = voice_event_clip[1];
            voice_event_c3[0] = voice_event_clip[2];
            voice_event_c4[0] = voice_event_clip[3];
            voice_event_c5[0] = voice_event_clip[4];
            voice_event_c6[0] = voice_event_clip[5];
            voice_event_c7[0] = voice_event_clip[6];
            voice_event_c8[0] = voice_event_clip[7];
            voice_event_c9[0] = voice_event_clip[8];
            voice_event_c10[0] = voice_event_clip[9];
            voice_event_c11[0] = voice_event_clip[10];
            voice_event_c12[0] = voice_event_clip[11];

            voice_event[0].Clips = voice_event_c1;
            voice_event[1].Clips = voice_event_c2;
            voice_event[2].Clips = voice_event_c3;
            voice_event[3].Clips = voice_event_c4;
            voice_event[4].Clips = voice_event_c5;
            voice_event[5].Clips = voice_event_c6;
            voice_event[6].Clips = voice_event_c7;
            voice_event[7].Clips = voice_event_c8;
            voice_event[8].Clips = voice_event_c9;
            voice_event[9].Clips = voice_event_c10;
            voice_event[10].Clips = voice_event_c11;
            voice_event[11].Clips = voice_event_c12;
            #endregion

            #region Soft
            VRCAnimatorPlayAudio voice_softMoan_audio;
            AudioClip[] voice_softMoan_clip = new AudioClip[20];
            voice_softMoan = FindAnimatorStateByName("Soft Moan", controller);
            voice_softMoan_audio = voice_softMoan.behaviours[0] as VRCAnimatorPlayAudio;
            for (int i = 0; i < voice_softMoan_clip.Length; i++)
            {
                voice_softMoan_clip[i] = (AudioClip)AssetDatabase.LoadAssetAtPath("Assets/!Dismay Custom/Penetration Contact System/Assets/Voice Pack/" + PCSConfigurator.voicePack.ToString() + "/Random/moan_random_soft (" + (i + 1) + ").wav", typeof(AudioClip));
            }
            voice_softMoan_clip = VerifyAudioClips(voice_softMoan_clip, 20);
            voice_softMoan_audio.Clips = voice_softMoan_clip;
            #endregion

            #region Rough
            VRCAnimatorPlayAudio voice_roughMoan_audio;
            AudioClip[] voice_roughMoan_clip = new AudioClip[20];
            voice_roughMoan = FindAnimatorStateByName("Rough Moan", controller);
            voice_roughMoan_audio = voice_roughMoan.behaviours[0] as VRCAnimatorPlayAudio;
            for (int i = 0; i < voice_roughMoan_clip.Length; i++)
            {
                voice_roughMoan_clip[i] = (AudioClip)AssetDatabase.LoadAssetAtPath("Assets/!Dismay Custom/Penetration Contact System/Assets/Voice Pack/" + PCSConfigurator.voicePack.ToString() + "/Random/moan_random_rough (" + (i + 1) + ").wav", typeof(AudioClip));
            }
            voice_roughMoan_clip = VerifyAudioClips(voice_roughMoan_clip, 20);
            voice_roughMoan_audio.Clips = voice_roughMoan_clip;
            #endregion

            #region Relax
            VRCAnimatorPlayAudio voice_relax_audio;
            AudioClip[] voice_relax_clip = new AudioClip[3];
            voice_relax = FindAnimatorStateByName("Voice Relax", controller);
            voice_relax_audio = voice_relax.behaviours[0] as VRCAnimatorPlayAudio;
            for (int i = 0; i < voice_relax_clip.Length; i++)
            {
                voice_relax_clip[i] = (AudioClip)AssetDatabase.LoadAssetAtPath("Assets/!Dismay Custom/Penetration Contact System/Assets/Voice Pack/" + PCSConfigurator.voicePack.ToString() + "/Relax/moan_relax (" + (i + 1) + ").wav", typeof(AudioClip));
            }
            voice_relax_audio.Clips = voice_relax_clip;

            static float FindMidpoint(float num1, float num2, float num3)
            {
                return (num1 + num2 + num3) / 3f;
            }
            voice_relax.speed = 1 / FindMidpoint(voice_relax_clip[0].length, voice_relax_clip[1].length, voice_relax_clip[2].length);
            #endregion

            #region Oral 1
            VRCAnimatorPlayAudio voice_oral1_audio;
            AudioClip[] voice_oral1_clip = new AudioClip[10];
            voice_oralSoft = FindAnimatorStateByName("Oral Soft", controller);
            voice_oral1_audio = voice_oralSoft.behaviours[0] as VRCAnimatorPlayAudio;
            for (int i = 0; i < voice_oral1_clip.Length; i++)
            {
                voice_oral1_clip[i] = (AudioClip)AssetDatabase.LoadAssetAtPath("Assets/!Dismay Custom/Penetration Contact System/Assets/Voice Pack/" + PCSConfigurator.voicePack.ToString() + "/Oral/moan_oral_soft (" + (i + 1) + ").wav", typeof(AudioClip));
            }
            voice_oral1_audio.Clips = voice_oral1_clip;
            #endregion

            #region Oral 2
            VRCAnimatorPlayAudio voice_oral2_audio;
            AudioClip[] voice_oral2_clip = new AudioClip[10];
            voice_oralRough = FindAnimatorStateByName("Oral Rough", controller);
            voice_oral2_audio = voice_oralRough.behaviours[0] as VRCAnimatorPlayAudio;
            for (int i = 0; i < voice_oral2_clip.Length; i++)
            {
                voice_oral2_clip[i] = (AudioClip)AssetDatabase.LoadAssetAtPath("Assets/!Dismay Custom/Penetration Contact System/Assets/Voice Pack/" + PCSConfigurator.voicePack.ToString() + "/Oral/moan_oral_rough (" + (i + 1) + ").wav", typeof(AudioClip));
            }
            voice_oral2_audio.Clips = voice_oral2_clip;
            #endregion

            #region Combo           **Need Assign Value!**
            VRCAnimatorPlayAudio voice_combo_audio1_start, voice_combo_audio1_end, voice_combo_audio2_start, voice_combo_audio2_end, voice_combo_audio3_start, voice_combo_audio3_end;

            //Make Random Combo Set
            var voice_randomCombo = FindAnimatorStateByName("Random Combo Set", controller);
            var voice_combo_set = voice_randomCombo.behaviours[0] as VRCAvatarParameterDriver;

            //ASSIGN VOCE PACK INFO HERE!! #############################################
            if (PCSConfigurator.voicePack == PCSConfigurator.VoicePack.Misuzugon)
            {
                int combo_set_amount = 2;
                VRCAvatarParameterDriver.Parameter x = new()
                {
                    type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Random,
                    name = "pcs/local/moan-index",
                    valueMin = 1,
                    valueMax = combo_set_amount,
                };
                voice_combo_set.parameters.Add(x);
                voice_combo_clip1 = new AudioClip[9];
                voice_combo_clip2 = new AudioClip[9];

                for (int i = 0; i < 9; i++)
                {
                    voice_combo_clip1[i] = (AudioClip)AssetDatabase.LoadAssetAtPath("Assets/!Dismay Custom/Penetration Contact System/Assets/Voice Pack/" + PCSConfigurator.voicePack.ToString() + "/Combo/moan_combo_a (" + (i + 1) + ").wav", typeof(AudioClip));
                    voice_combo_clip2[i] = (AudioClip)AssetDatabase.LoadAssetAtPath("Assets/!Dismay Custom/Penetration Contact System/Assets/Voice Pack/" + PCSConfigurator.voicePack.ToString() + "/Combo/moan_combo_b (" + (i + 1) + ").wav", typeof(AudioClip));
                    //voice_combo_clip3[i] = This voice pack has only 2 combo set
                }
            }

            if (PCSConfigurator.voicePack == PCSConfigurator.VoicePack.LewdHeart)
            {
                int combo_set_amount = 2;
                VRCAvatarParameterDriver.Parameter x = new()
                {
                    type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Random,
                    name = "pcs/local/moan-index",
                    valueMin = 1,
                    valueMax = combo_set_amount,
                };
                voice_combo_set.parameters.Add(x);
                voice_combo_clip1 = new AudioClip[9];
                voice_combo_clip2 = new AudioClip[9];

                for (int i = 0; i < 9; i++)
                {
                    voice_combo_clip1[i] = (AudioClip)AssetDatabase.LoadAssetAtPath("Assets/!Dismay Custom/Penetration Contact System/Assets/Voice Pack/" + PCSConfigurator.voicePack.ToString() + "/Combo/moan_combo_a (" + (i + 1) + ").wav", typeof(AudioClip));
                    voice_combo_clip2[i] = (AudioClip)AssetDatabase.LoadAssetAtPath("Assets/!Dismay Custom/Penetration Contact System/Assets/Voice Pack/" + PCSConfigurator.voicePack.ToString() + "/Combo/moan_combo_b (" + (i + 1) + ").wav", typeof(AudioClip));
                    //voice_combo_clip3[i] = This voice pack has only 2 combo set
                }
            }

            if (PCSConfigurator.voicePack == PCSConfigurator.VoicePack.NekoNyan)
            {
                int combo_set_amount = 3;
                VRCAvatarParameterDriver.Parameter x = new()
                {
                    type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Random,
                    name = "pcs/local/moan-index",
                    valueMin = 1,
                    valueMax = combo_set_amount,
                };
                voice_combo_set.parameters.Add(x);
                voice_combo_clip1 = new AudioClip[4];
                voice_combo_clip2 = new AudioClip[6];
                voice_combo_clip3 = new AudioClip[6];

                for (int i = 0; i < 4; i++)
                {
                    voice_combo_clip1[i] = (AudioClip)AssetDatabase.LoadAssetAtPath("Assets/!Dismay Custom/Penetration Contact System/Assets/Voice Pack/" + PCSConfigurator.voicePack.ToString() + "/Combo/moan_combo_a (" + (i + 1) + ").wav", typeof(AudioClip));
                }
                for (int i = 0; i < 6; i++)
                {
                    voice_combo_clip2[i] = (AudioClip)AssetDatabase.LoadAssetAtPath("Assets/!Dismay Custom/Penetration Contact System/Assets/Voice Pack/" + PCSConfigurator.voicePack.ToString() + "/Combo/moan_combo_b (" + (i + 1) + ").wav", typeof(AudioClip));
                    voice_combo_clip3[i] = (AudioClip)AssetDatabase.LoadAssetAtPath("Assets/!Dismay Custom/Penetration Contact System/Assets/Voice Pack/" + PCSConfigurator.voicePack.ToString() + "/Combo/moan_combo_c (" + (i + 1) + ").wav", typeof(AudioClip));
                }
            }
            //##########################################################################################

            //Combo 1
            var voice_combo_start1 = FindAnimatorStateByName("Combo Start 1", controller);
            var voice_combo_bridge1 = FindAnimatorStateByName("Combo Bridge 1", controller);
            var voice_combo_end1 = FindAnimatorStateByName("Combo End 1", controller);

            voice_combo_audio1_start = voice_combo_start1.behaviours[0] as VRCAnimatorPlayAudio;
            voice_combo_audio1_end = voice_combo_end1.behaviours[0] as VRCAnimatorPlayAudio;
            voice_combo_audio1_start.Clips = voice_combo_clip1;

            AudioClip[] temp_end1 = new AudioClip[1];
            temp_end1[0] = voice_combo_clip1[^1];
            voice_combo_audio1_end.Clips = temp_end1;
            if (temp_end1[0] != null)
            {
                voice_combo_end1.speed = 1 / temp_end1[0].length;
            }

            var voice_combo_bridge1_TO_start = voice_combo_bridge1.AddTransition(voice_combo_start1);
            voice_combo_bridge1_TO_start.duration = 0;
            voice_combo_bridge1_TO_start.exitTime = 0;
            voice_combo_bridge1_TO_start.AddCondition(AnimatorConditionMode.If, 1, "pcs/contact/hit");
            voice_combo_bridge1_TO_start.AddCondition(AnimatorConditionMode.NotEqual, voice_combo_clip1.Length - 1, "pcs/local/moan-combo");

            var voice_combo_bridge1_TO_end = voice_combo_bridge1.AddTransition(voice_combo_end1);
            voice_combo_bridge1_TO_end.duration = 0;
            voice_combo_bridge1_TO_end.exitTime = 0;
            voice_combo_bridge1_TO_end.AddCondition(AnimatorConditionMode.If, 1, "pcs/contact/hit");
            voice_combo_bridge1_TO_end.AddCondition(AnimatorConditionMode.Equals, voice_combo_clip1.Length - 1, "pcs/local/moan-combo");

            //Combo 2
            var voice_combo_start2 = FindAnimatorStateByName("Combo Start 2", controller);
            var voice_combo_bridge2 = FindAnimatorStateByName("Combo Bridge 2", controller);
            var voice_combo_end2 = FindAnimatorStateByName("Combo End 2", controller);

            voice_combo_audio2_start = voice_combo_start2.behaviours[0] as VRCAnimatorPlayAudio;
            voice_combo_audio2_end = voice_combo_end2.behaviours[0] as VRCAnimatorPlayAudio;
            voice_combo_audio2_start.Clips = voice_combo_clip2;

            AudioClip[] temp_end2 = new AudioClip[1];
            temp_end2[0] = voice_combo_clip2[^1];
            voice_combo_audio2_end.Clips = temp_end2;
            if (temp_end2[0] != null)
            {
                voice_combo_end2.speed = 1 / temp_end2[0].length;
            }

            var voice_combo_bridge2_TO_start = voice_combo_bridge2.AddTransition(voice_combo_start2);
            voice_combo_bridge2_TO_start.duration = 0;
            voice_combo_bridge2_TO_start.exitTime = 0;
            voice_combo_bridge2_TO_start.AddCondition(AnimatorConditionMode.If, 1, "pcs/contact/hit");
            voice_combo_bridge2_TO_start.AddCondition(AnimatorConditionMode.NotEqual, voice_combo_clip2.Length - 1, "pcs/local/moan-combo");

            var voice_combo_bridge2_TO_end = voice_combo_bridge2.AddTransition(voice_combo_end2);
            voice_combo_bridge2_TO_end.duration = 0;
            voice_combo_bridge2_TO_end.exitTime = 0;
            voice_combo_bridge2_TO_end.AddCondition(AnimatorConditionMode.If, 1, "pcs/contact/hit");
            voice_combo_bridge2_TO_end.AddCondition(AnimatorConditionMode.Equals, voice_combo_clip2.Length - 1, "pcs/local/moan-combo");

            //Combo 3
            var voice_combo_start3 = FindAnimatorStateByName("Combo Start 3", controller);
            var voice_combo_bridge3 = FindAnimatorStateByName("Combo Bridge 3", controller);
            var voice_combo_end3 = FindAnimatorStateByName("Combo End 3", controller);

            voice_combo_audio3_start = voice_combo_start3.behaviours[0] as VRCAnimatorPlayAudio;
            voice_combo_audio3_end = voice_combo_end3.behaviours[0] as VRCAnimatorPlayAudio;
            voice_combo_audio3_start.Clips = voice_combo_clip3;

            AudioClip[] temp_end3 = new AudioClip[1];
            temp_end3[0] = voice_combo_clip3[^1];
            voice_combo_audio3_end.Clips = temp_end3;
            if (temp_end3[0] != null)
            {
                voice_combo_end3.speed = 1 / temp_end3[0].length;
            }

            var voice_combo_bridge3_TO_start = voice_combo_bridge3.AddTransition(voice_combo_start3);
            voice_combo_bridge3_TO_start.duration = 0;
            voice_combo_bridge3_TO_start.exitTime = 0;
            voice_combo_bridge3_TO_start.AddCondition(AnimatorConditionMode.If, 1, "pcs/contact/hit");
            voice_combo_bridge3_TO_start.AddCondition(AnimatorConditionMode.NotEqual, voice_combo_clip3.Length - 1, "pcs/local/moan-combo");

            var voice_combo_bridge3_TO_end = voice_combo_bridge3.AddTransition(voice_combo_end3);
            voice_combo_bridge3_TO_end.duration = 0;
            voice_combo_bridge3_TO_end.exitTime = 0;
            voice_combo_bridge3_TO_end.AddCondition(AnimatorConditionMode.If, 1, "pcs/contact/hit");
            voice_combo_bridge3_TO_end.AddCondition(AnimatorConditionMode.Equals, voice_combo_clip3.Length - 1, "pcs/local/moan-combo");

            #endregion

            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}

