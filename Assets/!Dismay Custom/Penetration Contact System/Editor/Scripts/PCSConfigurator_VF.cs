using UnityEngine;
using UnityEditor;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;
using System.Linq;
using VRC.SDK3.Dynamics.Constraint.Components;
using UnityEditor.Search;
using System;
using System.Collections.Generic;
using VRC.Dynamics;

namespace DMCustom
{
    public class PCSConfigurator_VF : EditorWindow
    {
        //###
        private GUIStyle paramStyle, infoStyle;
        private VRCAvatarDescriptor targetAvatar;
        private Animator animator;
        private Texture2D logo;
        private Vector2 scrollPosition = new(0, 300);
        //###
        private readonly string thisGimmick = "Penetration Contact System";
        private readonly string version = "1.8.0";
        private readonly string[] customPos_menuName = new string[] { "Custom #1", "Custom #2", "Custom #3", "Custom #4", "Custom #5", "Custom #6", "Custom #7", "Custom #8" };
        private readonly string[] customPos_choiceName = new string[] { "Disable", "1", "2", "3", "4", "5", "6", "7", "8" };
        private readonly int[] customPos_sizes = { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
        private int selected_customPos;
        private float smashSensitivity = 1, lustMultiplierValue = 0.4f;
        private bool lustFeature = true, useMouth = true, useBoobs= true, usePussy = true, useAss = true;
        private bool hidePlacement = true, flag1 = false, flag2 = false, flag3 = false, isError = false;
        //###
        private GameObject ref_mouth = null, ref_boobs = null, ref_pussy = null, ref_ass = null;
        private readonly GameObject[] ref_soundPosition = new GameObject[8];
        private enum SetupTool { VRCFury }
        private SetupTool setupTool = SetupTool.VRCFury;
        private enum VoicePack
        {
            Disable, Anime, Mature
        }
        private VoicePack voicePack = VoicePack.Disable;
        private enum Preset
        {
            GENERIC,
            REFERENCE,
            Airi,
            Anon,
            Chiffon,
            Aria,
            Eyo,
            Imeris,
            Karin,
            Kikyo,
            Lasyusha,
            Leefa,
            Lime,
            Manuka,
            Maya,
            Mizuki,
            Moe,
            Mophira,
            Rindo,
            Runa_Robotic,
            Rurune,
            Shinano,
            Sio,
            Selestia,
            Shinra,
            UltimateKissMa,
            Uzuki,
            Velle,
            Wolferia,
        }
        private Preset preset = Preset.GENERIC;
        //##

        [MenuItem("Tools/Dismay Custom/Penetration Contact System/Install with VRCFury")]
        public static void ShowpWindow()
        {
            var window = GetWindow(typeof(PCSConfigurator_VF));

            window.titleContent = new GUIContent("Penetration Contact System");
            Rect main = EditorGUIUtility.GetMainWindowPosition();
            Rect pos = window.position;
            float centerWidth = (main.width - pos.width) * 0.5f;
            float centerHeight = (main.height - pos.height) * 0.3f;
            pos.x = main.x + centerWidth + 360;
            pos.y = main.y + centerHeight;
            window.position = pos;
            window.minSize = new Vector2(512, 640);
            window.maxSize = new Vector2(512, 740);
            window.Show();
        }
        private void OnGUI()
        {
            paramStyle = new GUIStyle()
            {
                fontSize = 10,
                fontStyle = FontStyle.Normal,
                normal = { textColor = new Color(0.8f, 0.8f, 0.8f) },
            };

            infoStyle = new GUIStyle()
            {
                fontSize = 10,
                fontStyle = FontStyle.Normal,
                normal = { textColor = new Color(0.5f, 0.5f, 0.5f) },
                alignment = TextAnchor.LowerLeft
            };
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            logo = Resources.Load<Texture2D>("Components/" + thisGimmick + "_banner");
            GUILayout.Label(logo, new GUIStyle { fixedWidth = 512, fixedHeight = 115});
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            targetAvatar = EditorGUILayout.ObjectField(targetAvatar, typeof(VRCAvatarDescriptor), true, GUILayout.Height(30)) as VRCAvatarDescriptor;

            if (targetAvatar)
            {
                //Getting avatar properties
                animator = targetAvatar.GetComponent<Animator>();
                var prefab = targetAvatar.transform.Find(thisGimmick);

                ShowMenuList();
                ShowParameter();

                if (prefab != null)
                {
                    ShowButtons(prefab.gameObject);
                }
                else
                {
                    ShowButtons(null);
                    GUI.enabled = true;
                }

                ShowQuickMenu();
            }
            ShowFooter();
        }
        private void ShowMenuList()
        {
            EditorStyles.label.fontStyle = FontStyle.Bold;
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);
            GUI.enabled = false;
            setupTool = (SetupTool)EditorGUILayout.EnumPopup("Installer:", setupTool);
            GUI.enabled = true;

            EditorGUILayout.BeginHorizontal();
            if(preset == Preset.REFERENCE)
            {
                preset = (Preset)EditorGUILayout.EnumPopup(new GUIContent("Alignment Preset:", "Placement preset for some avatars. Select Generic if there is no preset for your selected avatar. Select Reference to place them on your preferred location instead."), preset);
                if (GUILayout.Button("Find SPS Sockets", GUILayout.Width(130)))
                {
                    GameObject find_mouth, find_boobs, find_pussy, find_ass;

                    find_mouth = GameObject.Find("SPS/Blowjob");
                    find_boobs = GameObject.Find("SPS/Special/Titjob");
                    find_pussy = GameObject.Find("SPS/Pussy");
                    find_ass = GameObject.Find("SPS/Anal");

                    if (find_mouth != null && find_mouth.transform.IsChildOf(targetAvatar.transform))
                    {
                        ref_mouth = find_mouth;
                    }
                    if (find_boobs != null && find_boobs.transform.IsChildOf(targetAvatar.transform))
                    {
                        ref_boobs = find_boobs;
                    }
                    if (find_pussy != null && find_pussy.transform.IsChildOf(targetAvatar.transform))
                    {
                        ref_pussy = find_pussy;
                    }
                    if (find_ass != null && find_ass.transform.IsChildOf(targetAvatar.transform))
                    {
                        ref_ass = find_ass;
                    }
                }
            }
            else
            {
                preset = (Preset)EditorGUILayout.EnumPopup(new GUIContent("Alignment Preset:", "Placement preset for some avatars. Select Generic if there is no preset for your selected avatar. Select Reference to place them on your preferred location instead."), preset);
            }
            EditorGUILayout.EndHorizontal();

            if (preset != Preset.REFERENCE)
            {
                flag2 = true;

                GUILayout.BeginVertical("ProgressBarBack");

                GUILayout.BeginHorizontal();
                useMouth = EditorGUILayout.Toggle("Mouth:", useMouth);
                useBoobs = EditorGUILayout.Toggle("Boobs:", useBoobs);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                usePussy = EditorGUILayout.Toggle("Pussy:", usePussy);
                useAss = EditorGUILayout.Toggle("Ass:", useAss);
                GUILayout.EndHorizontal();

                if (!useMouth && !useBoobs && !usePussy && !useAss)
                {
                    EditorGUILayout.HelpBox("You must select at least one location and sound. Otherwise, this system will be useless!!", MessageType.Error);
                    flag1 = false;
                }
                else
                {
                    flag1 = true;
                }

                GUILayout.EndVertical();
            }
            else
            {
                flag1 = true;
                GUILayout.BeginVertical("ProgressBarBack");                
                ref_mouth = EditorGUILayout.ObjectField("Mouth Position:",ref_mouth, typeof(GameObject), true) as GameObject;
                ref_boobs = EditorGUILayout.ObjectField("Boobs Position:", ref_boobs, typeof(GameObject), true) as GameObject;
                ref_pussy = EditorGUILayout.ObjectField("Pussy Position:", ref_pussy, typeof(GameObject), true) as GameObject;
                ref_ass = EditorGUILayout.ObjectField("Ass Position:", ref_ass, typeof(GameObject), true) as GameObject;
                GUILayout.EndVertical();

                bool[] check_pass = new bool[5];
                if (ref_mouth == null && ref_boobs == null && ref_pussy == null && ref_ass == null) //If all slots are empty
                {
                    EditorGUILayout.HelpBox("Please specify at least one reference location. Leave it as \"None\" if you want to disable.", MessageType.Warning);
                    check_pass[4] = false;
                }
                else if (ref_mouth != null && ref_boobs != null && ref_pussy != null && ref_ass != null)
                {
                    check_pass[4] = true;
                }
                else if (ref_mouth != null || ref_boobs != null || ref_pussy != null || ref_ass != null) //If some slots are filled
                {
                    check_pass[4] = true;
                }
                if (ref_mouth != null)
                {
                    if (!ref_mouth.transform.IsChildOf(targetAvatar.transform))
                    {
                        check_pass[0] = false;
                        EditorGUILayout.HelpBox("Mouth reference target is not a child gameObject of your avatar.", MessageType.Warning);
                    }
                    else
                    {
                        check_pass[0] = true;
                    }
                }
                else
                {
                    check_pass[0] = true;
                }
                if (ref_boobs != null)
                {
                    if (!ref_boobs.transform.IsChildOf(targetAvatar.transform))
                    {
                        check_pass[1] = false;
                        EditorGUILayout.HelpBox("Boobs reference target is not a child gameObject of your avatar.", MessageType.Warning);
                    }
                    else
                    {
                        check_pass[1] = true;
                    }
                }
                else
                {
                    check_pass[1] = true;
                }
                if (ref_pussy != null)
                {
                    if (!ref_pussy.transform.IsChildOf(targetAvatar.transform))
                    {
                        check_pass[2] = false;
                        EditorGUILayout.HelpBox("Pussy reference target is not a child gameObject of your avatar.", MessageType.Warning);
                    }
                    else
                    {
                        check_pass[2] = true;
                    }
                }
                else
                {
                    check_pass[2] = true;
                }
                if (ref_ass != null)
                {
                    if (!ref_ass.transform.IsChildOf(targetAvatar.transform))
                    {
                        check_pass[3] = false;
                        EditorGUILayout.HelpBox("Ass reference target is not a child gameObject of your avatar.", MessageType.Warning);
                    }
                    else
                    {
                        check_pass[3] = true;
                    }
                }
                else
                {
                    check_pass[3] = true;
                }

                if (check_pass[0] && check_pass[1] && check_pass[2] && check_pass[3] && check_pass[4])
                {
                    flag2 = true;
                }
                else
                {
                    flag2 = false;
                }
            }

            smashSensitivity = EditorGUILayout.Slider(new GUIContent("Smash Sensitivity:", "This option allows you to adjust the sensitivity for impact detection. Lower this value requires more thrust to trigger the sound."), smashSensitivity, 0.1f, 1);
            smashSensitivity = Mathf.Round(smashSensitivity * Mathf.Pow(10, 1)) / Mathf.Pow(10,1);
            selected_customPos = EditorGUILayout.IntPopup("Custom Position:", selected_customPos, customPos_choiceName, customPos_sizes);
            if (selected_customPos == 0)
            {
                flag3 = true;
            }
            else
            {   
                GUILayout.BeginVertical("ProgressBarBack");
                if (selected_customPos == 1)
                {
                    if (ref_soundPosition[0] == null)
                    {
                        flag3 = false;
                    }
                    else
                    {
                        flag3 = true;
                    }
                    ShowSourceSetup(1);
                }
                else if (selected_customPos == 2)
                {
                    if (ref_soundPosition[0] == null || ref_soundPosition[1] == null)
                    {
                        flag3 = false;
                    }
                    else
                    {
                        flag3 = true;
                    }
                    ShowSourceSetup(1);
                    ShowSourceSetup(2);
                }
                else if (selected_customPos == 3)
                {
                    if (ref_soundPosition[0] == null || ref_soundPosition[1] == null || ref_soundPosition[2] == null)
                    {
                        flag3 = false;
                    }
                    else
                    {
                        flag3 = true;
                    }
                    ShowSourceSetup(1);
                    ShowSourceSetup(2);
                    ShowSourceSetup(3);
                }
                else if (selected_customPos == 4)
                {
                    if (ref_soundPosition[0] == null || ref_soundPosition[1] == null || ref_soundPosition[2] == null || ref_soundPosition[3] == null)
                    {
                        flag3 = false;
                    }
                    else
                    {
                        flag3 = true;
                    }
                    ShowSourceSetup(1);
                    ShowSourceSetup(2);
                    ShowSourceSetup(3);
                    ShowSourceSetup(4);
                }
                else if (selected_customPos == 5)
                {
                    if (ref_soundPosition[0] == null || ref_soundPosition[1] == null || ref_soundPosition[2] == null || ref_soundPosition[3] == null || ref_soundPosition[4] == null)
                    {
                        flag3 = false;
                    }
                    else
                    {
                        flag3 = true;
                    }
                    ShowSourceSetup(1);
                    ShowSourceSetup(2);
                    ShowSourceSetup(3);
                    ShowSourceSetup(4);
                    ShowSourceSetup(5);
                }
                else if (selected_customPos == 6)
                {
                    if (ref_soundPosition[0] == null || ref_soundPosition[1] == null || ref_soundPosition[2] == null || ref_soundPosition[3] == null || ref_soundPosition[4] == null || ref_soundPosition[5] == null)
                    {
                        flag3 = false;
                    }
                    else
                    {
                        flag3 = true;
                    }
                    ShowSourceSetup(1);
                    ShowSourceSetup(2);
                    ShowSourceSetup(3);
                    ShowSourceSetup(4);
                    ShowSourceSetup(5);
                    ShowSourceSetup(6);
                }
                else if (selected_customPos == 7)
                {
                    if (ref_soundPosition[0] == null && ref_soundPosition[1] == null || ref_soundPosition[2] == null || ref_soundPosition[3] == null || ref_soundPosition[4] == null || ref_soundPosition[5] == null || ref_soundPosition[6] == null)
                    {
                        flag3 = false;
                    }
                    else
                    {
                        flag3 = true;
                    }
                    ShowSourceSetup(1);
                    ShowSourceSetup(2);
                    ShowSourceSetup(3);
                    ShowSourceSetup(4);
                    ShowSourceSetup(5);
                    ShowSourceSetup(6);
                    ShowSourceSetup(7);
                }
                else if (selected_customPos == 8)
                {
                    if (ref_soundPosition[0] == null || ref_soundPosition[1] == null || ref_soundPosition[2] == null || ref_soundPosition[3] == null || ref_soundPosition[4] == null || ref_soundPosition[5] == null || ref_soundPosition[6] == null || ref_soundPosition[7] == null)
                    {
                        flag3 = false;
                    }
                    else
                    {
                        flag3 = true;
                    }
                    ShowSourceSetup(1);
                    ShowSourceSetup(2);
                    ShowSourceSetup(3);
                    ShowSourceSetup(4);
                    ShowSourceSetup(5);
                    ShowSourceSetup(6);
                    ShowSourceSetup(7);
                    ShowSourceSetup(8);
                }
                GUILayout.EndVertical();
                if (flag3 == false)
                {
                    EditorGUILayout.HelpBox("Please assign all custom target and menu name. You can set it to anywhere you want.", MessageType.Warning);
                }
            }

            lustFeature = EditorGUILayout.Toggle(new GUIContent("Lust Feature", "This feature allows you to gain and stack lust value from being penetrated, which will make you moan, cum, and squirt when it's full."), lustFeature);

            if (lustFeature)
            {
                GUILayout.BeginVertical("ProgressBarBack");
                voicePack = (VoicePack)EditorGUILayout.EnumPopup("Voice Pack:", voicePack);
                lustMultiplierValue = EditorGUILayout.Slider(new GUIContent("Lust Multiplier:", "Adjust this to determine how quickly you will get 1 point of lust value from a penetrating stroke. Increasing this value and you'll reach climax faster."), lustMultiplierValue, 0.1f, 1);
                lustMultiplierValue = Mathf.Round(lustMultiplierValue * Mathf.Pow(10, 1)) / Mathf.Pow(10, 1);
                GUILayout.EndVertical();
            }

            //end of menu list
            EditorGUILayout.EndScrollView();
        }
        private void ShowSourceSetup(int slot)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Target #" + (slot) + ":", GUILayout.MaxWidth(90));

            EditorGUI.BeginChangeCheck();
            ref_soundPosition[slot - 1] = EditorGUILayout.ObjectField(ref_soundPosition[slot - 1], typeof(GameObject), true) as GameObject;
            if (EditorGUI.EndChangeCheck())
            {
                for(int i = 0; i < ref_soundPosition.Length; i++)
                {
                    if(ref_soundPosition[i] != null)
                    {
                        customPos_menuName[i] = ref_soundPosition[i].name;
                    }
                }
            }

            EditorGUILayout.LabelField("Menu Name:", GUILayout.MaxWidth(80));
            customPos_menuName[slot - 1] = EditorGUILayout.TextField(customPos_menuName[slot - 1]);
            EditorGUILayout.EndHorizontal();
        }
        private void ShowQuickMenu()
        {
            //ADD QUICK ACCESS!
            var prefab = targetAvatar.transform.Find(thisGimmick);
            if (prefab != null)
            {
                EditorGUILayout.BeginVertical("ProgressBarBack");              

                EditorGUILayout.LabelField("Quick Access");

                //Line1
                GUI.enabled = true;
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Hide/Show Placement Icon",GUILayout.Width(246)))
                {
                    if (!hidePlacement)
                    {
                        hidePlacement = true;
                    }
                    else
                    {
                        hidePlacement = false;
                    }

                    foreach (var gameObj in FindObjectsOfType(typeof(GameObject), true) as GameObject[])
                    {
                        if (gameObj.name == "Placement Icons (Auto Remove)")
                        {
                            if (gameObj.transform.IsChildOf(targetAvatar.transform))
                            {
                                if (!hidePlacement)
                                {
                                    //SceneVisibilityManager.instance.Hide(gameObj, true);
                                    gameObj.SetActive(false);
                                }
                                else
                                {
                                    gameObj.SetActive(true);
                                }
                            }
                        }
                    }
                }
                if (GUILayout.Button("Spawn a Test Penetrator", GUILayout.Width(246)))
                {
                    GameObject x = Instantiate(Resources.Load<GameObject>("PCS Test Penetrator")) as GameObject;
                    x.name = "PCS Test Penetrator";
                    var hips = animator.GetBoneTransform(HumanBodyBones.Hips);
                    var legR = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
                    float dist = Vector3.Distance(hips.position, legR.position);
                    x.transform.localPosition = new Vector3(hips.position.x, hips.position.y - (dist - 0.01f), hips.position.z);
                    Tools.pivotMode = PivotMode.Pivot;
                    Tools.pivotRotation = PivotRotation.Local;
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Locate Mouth", GUILayout.Width(122)))
                {
                    foreach (var gameObj in FindObjectsOfType(typeof(GameObject), true) as GameObject[])
                    {
                        if (gameObj.name == "<PCS Target> Mouth")
                        {
                            if (gameObj.transform.IsChildOf(targetAvatar.transform))
                            {
                                EditorGUIUtility.PingObject(gameObj);
                                Selection.activeObject = gameObj;
                                Tools.pivotMode = PivotMode.Pivot;
                                Tools.pivotRotation = PivotRotation.Local;
                            }
                        }
                    }
                }
                if (GUILayout.Button("Locate Boobs", GUILayout.Width(122)))
                {
                    foreach (var gameObj in FindObjectsOfType(typeof(GameObject), true) as GameObject[])
                    {
                        if (gameObj.name == "<PCS Target> Boobs")
                        {
                            if (gameObj.transform.IsChildOf(targetAvatar.transform))
                            {
                                EditorGUIUtility.PingObject(gameObj);
                                Selection.activeObject = gameObj;
                                Tools.pivotMode = PivotMode.Pivot;
                                Tools.pivotRotation = PivotRotation.Local;
                            }
                        }
                    }
                }
                if (GUILayout.Button("Locate Pussy", GUILayout.Width(122)))
                {
                    foreach (var gameObj in FindObjectsOfType(typeof(GameObject), true) as GameObject[])
                    {
                        if (gameObj.name == "<PCS Target> Pussy")
                        {
                            if (gameObj.transform.IsChildOf(targetAvatar.transform))
                            {
                                EditorGUIUtility.PingObject(gameObj);
                                Selection.activeObject = gameObj;
                                Tools.pivotMode = PivotMode.Pivot;
                                Tools.pivotRotation = PivotRotation.Local;
                            }
                        }
                    }
                }
                if (GUILayout.Button("Locate Ass", GUILayout.Width(122)))
                {
                    foreach (var gameObj in FindObjectsOfType(typeof(GameObject), true) as GameObject[])
                    {
                        if (gameObj.name == "<PCS Target> Ass")
                        {
                            if (gameObj.transform.IsChildOf(targetAvatar.transform))
                            {
                                EditorGUIUtility.PingObject(gameObj);
                                Selection.activeObject = gameObj;
                                Tools.pivotMode = PivotMode.Pivot;
                                Tools.pivotRotation = PivotRotation.Local;
                            }
                        }
                    }
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Custom #1", GUILayout.Width(122)))
                    {
                        foreach (var gameObj in FindObjectsOfType(typeof(GameObject), true) as GameObject[])
                        {
                            if (gameObj.name.Contains("<PCS Target> Custom (1)"))
                            {
                                if (gameObj.transform.IsChildOf(targetAvatar.transform))
                                {
                                    EditorGUIUtility.PingObject(gameObj);
                                    Selection.activeObject = gameObj;
                                    Tools.pivotMode = PivotMode.Pivot;
                                    Tools.pivotRotation = PivotRotation.Local;
                                }
                            }
                        }
                    }
                    if (GUILayout.Button("Custom #2", GUILayout.Width(122)))
                    {
                        foreach (var gameObj in FindObjectsOfType(typeof(GameObject), true) as GameObject[])
                        {
                            if (gameObj.name.Contains("<PCS Target> Custom (2)"))
                            {
                                if (gameObj.transform.IsChildOf(targetAvatar.transform))
                                {
                                    EditorGUIUtility.PingObject(gameObj);
                                    Selection.activeObject = gameObj;
                                    Tools.pivotMode = PivotMode.Pivot;
                                    Tools.pivotRotation = PivotRotation.Local;
                                }
                            }
                        }
                    }
                    if (GUILayout.Button("Custom #3", GUILayout.Width(122)))
                    {
                        foreach (var gameObj in FindObjectsOfType(typeof(GameObject), true) as GameObject[])
                        {
                            if (gameObj.name.Contains("<PCS Target> Custom (3)"))
                            {
                                if (gameObj.transform.IsChildOf(targetAvatar.transform))
                                {
                                    EditorGUIUtility.PingObject(gameObj);
                                    Selection.activeObject = gameObj;
                                Tools.pivotMode = PivotMode.Pivot;
                                Tools.pivotRotation = PivotRotation.Local;
                                }
                            }
                        }
                    }
                    if (GUILayout.Button("Custom #4", GUILayout.Width(122)))
                    {
                        foreach (var gameObj in FindObjectsOfType(typeof(GameObject), true) as GameObject[])
                        {
                            if (gameObj.name.Contains("<PCS Target> Custom (4)"))
                            {
                                if (gameObj.transform.IsChildOf(targetAvatar.transform))
                                {
                                    EditorGUIUtility.PingObject(gameObj);
                                    Selection.activeObject = gameObj;
                                    Tools.pivotMode = PivotMode.Pivot;
                                    Tools.pivotRotation = PivotRotation.Local;
                                }
                            }
                        }
                    }
                GUILayout.FlexibleSpace();        
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Custom #5", GUILayout.Width(122)))
                    {
                        foreach (var gameObj in FindObjectsOfType(typeof(GameObject), true) as GameObject[])
                        {
                            if (gameObj.name.Contains("<PCS Target> Custom (5)"))
                            {
                                if (gameObj.transform.IsChildOf(targetAvatar.transform))
                                {
                                    EditorGUIUtility.PingObject(gameObj);
                                    Selection.activeObject = gameObj;
                                    Tools.pivotMode = PivotMode.Pivot;
                                    Tools.pivotRotation = PivotRotation.Local;
                                }
                            }
                        }
                    }
                    if (GUILayout.Button("Custom #6", GUILayout.Width(122)))
                    {
                        foreach (var gameObj in FindObjectsOfType(typeof(GameObject), true) as GameObject[])
                        {
                            if (gameObj.name.Contains("<PCS Target> Custom (6)"))
                            {
                                if (gameObj.transform.IsChildOf(targetAvatar.transform))
                                {
                                    EditorGUIUtility.PingObject(gameObj);
                                    Selection.activeObject = gameObj;
                                    Tools.pivotMode = PivotMode.Pivot;
                                    Tools.pivotRotation = PivotRotation.Local;
                            }
                            }
                        }
                    }
                    if (GUILayout.Button("Custom #7", GUILayout.Width(122)))
                    {
                        foreach (var gameObj in FindObjectsOfType(typeof(GameObject), true) as GameObject[])
                        {
                            if (gameObj.name.Contains("<PCS Target> Custom (7)"))
                            {
                                if (gameObj.transform.IsChildOf(targetAvatar.transform))
                                {
                                    EditorGUIUtility.PingObject(gameObj);
                                    Selection.activeObject = gameObj;
                                    Tools.pivotMode = PivotMode.Pivot;
                                    Tools.pivotRotation = PivotRotation.Local;
                                }
                            }
                        }
                    }
                    if (GUILayout.Button("Custom #8", GUILayout.Width(122)))
                    {
                        foreach (var gameObj in FindObjectsOfType(typeof(GameObject), true) as GameObject[])
                        {
                            if (gameObj.name.Contains("<PCS Target> Custom (8)"))
                            {
                                if (gameObj.transform.IsChildOf(targetAvatar.transform))
                                {
                                    EditorGUIUtility.PingObject(gameObj);
                                    Selection.activeObject = gameObj;
                                    Tools.pivotMode = PivotMode.Pivot;
                                    Tools.pivotRotation = PivotRotation.Local;
                                }
                            }
                        }
                    }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }
        }
        private void ShowButtons(GameObject prefab)
        {
            GUI.color = new Color32(255, 255, 255, 255);
            if (prefab == null)
            {
                if (flag1 == true && flag2 == true && flag3 == true)
                {
                    GUI.enabled = true;
                }
                else
                {
                    GUI.enabled = false;
                }
                if (GUILayout.Button("Apply"))
                {
                    Apply(true);
                }

                GUI.enabled = false;
                if (GUILayout.Button("Remove"))
                {
                    Remove(true);
                }
            }
            else
            {
                if (flag1 == true && flag2 == true && flag3 == true)
                {
                    GUI.enabled = true;
                }
                else
                {
                    GUI.enabled = false;
                }
                if (GUILayout.Button("Replace"))
                {
                    Remove(false);
                    Apply(false);
                }

                GUI.enabled = true;
                if (GUILayout.Button("Remove"))
                {
                    Remove(true);
                }
            }
            GUI.color = new Color32(255, 255, 255, 255);
        }
        private void Apply(bool confirm)
        {
            var head = animator.GetBoneTransform(HumanBodyBones.Head);
            var hips = animator.GetBoneTransform(HumanBodyBones.Hips);

            hidePlacement = true;

            //Copy prefab
            Vector3 tempTransform;
            GameObject x = Instantiate(Resources.Load<GameObject>("Main Prefab/PCS VF Prefab")) as GameObject;
            x.name = thisGimmick;

            tempTransform = targetAvatar.transform.localScale;
            targetAvatar.transform.localScale = new Vector3(1, 1, 1);
            x.transform.parent = targetAvatar.transform;

            var mouth = targetAvatar.transform.Find(thisGimmick + "/<PCS Target> Mouth").gameObject;
            var boobs = targetAvatar.transform.Find(thisGimmick + "/<PCS Target> Boobs").gameObject;
            var pussy = targetAvatar.transform.Find(thisGimmick + "/<PCS Target> Pussy").gameObject;
            var ass = targetAvatar.transform.Find(thisGimmick + "/<PCS Target> Ass").gameObject;
            var pcsContact = targetAvatar.transform.Find(thisGimmick + "/PCS Contacts");
            VRCParentConstraint vRCParentConstraint = pcsContact.GetComponent<VRCParentConstraint>();
            vRCParentConstraint.IsActive = true;

            //Expressions
            GenerateFolder();
            GenerateMenu();
            GenerateParameter();

            var voiceSource = targetAvatar.transform.Find(thisGimmick + "/Voice Pack/Audio Source").gameObject;
            var voiceObj = targetAvatar.transform.Find(thisGimmick + "/Voice Pack").gameObject;
            var voiceAnime = targetAvatar.transform.Find(thisGimmick + "/Voice Pack/Voice Anime").gameObject;
            var voiceMature = targetAvatar.transform.Find(thisGimmick + "/Voice Pack/Voice Mature").gameObject;
            var voiceDisable = targetAvatar.transform.Find(thisGimmick + "/Voice Pack/Voice Disable").gameObject;

            //Voice pack
            voiceObj.transform.position = head.position;

            //################ Must delete when Modular fix audio path issue next patch.
            VRCConstraintSource newSource = new()
            {
                SourceTransform = head.transform,
                Weight = 1
            };
            VRCPositionConstraint voiceConst = voiceObj.GetComponent<VRCPositionConstraint>();
            voiceConst.Sources.Add(newSource);
            voiceConst.IsActive = true;

            //If VF, need to instantiate a separate voice prefab
            if (voicePack == VoicePack.Anime)
            {
                DestroyImmediate(voiceMature);
                DestroyImmediate(voiceDisable);
            }
            else if (voicePack == VoicePack.Mature)
            {
                DestroyImmediate(voiceAnime);
                DestroyImmediate(voiceDisable);
            }
            else
            {
                DestroyImmediate(voiceMature);
                DestroyImmediate(voiceAnime);
                DestroyImmediate(voiceSource);
            }
            if (!lustFeature)
            {
                DestroyImmediate(voiceObj);
            }

            AlignmentPreset(mouth.transform, boobs.transform, pussy.transform, ass.transform);
            targetAvatar.transform.localScale = tempTransform;

            //Place Custom Position
            Transform[] customPos = new Transform[9];
            if(selected_customPos != 0)
            {
                for (int i = 0; i < selected_customPos; i++)
                {
                    customPos[i] = targetAvatar.transform.Find(thisGimmick + "/<PCS Target> Custom (" + (i + 1) + ")");
                    customPos[i].transform.position = ref_soundPosition[i].transform.position;
                    customPos[i].transform.parent = ref_soundPosition[i].transform;
                    customPos[i].transform.localPosition = Vector3.zero;
                    customPos[i].transform.localEulerAngles = Vector3.zero;
                    Debug.Log("PCS Custom Location #" + (i + 1) + " has been placed under <" + SearchUtils.GetHierarchyPath(ref_soundPosition[i]) + ">");
                }
            }

            //Remove Custom Position
            Transform[] target = new Transform[9];
            GameObject[] targets = new GameObject[9];
            Transform[] guide = new Transform[9];
            GameObject[] guides = new GameObject[9];
            for (int i = 8; i > selected_customPos; i--)
            {
                target[i] = targetAvatar.transform.Find(thisGimmick + "/<PCS Target> Custom (" + i + ")");
                targets[i] = target[i].gameObject;
                DestroyImmediate(targets[i]);

                guide[i] = targetAvatar.transform.Find(thisGimmick + "/Placement Icons (Auto Remove)/Custom (" + i + ")");
                guides[i] = guide[i].gameObject;
                DestroyImmediate(guides[i]);
            }
            vRCParentConstraint.Sources.SetLength(4 + selected_customPos);

            //Particle Position
            var squirtObj = targetAvatar.transform.Find(thisGimmick + "/<PCS Particle> Squirt").gameObject;
            if (preset != Preset.REFERENCE)
            {
                if (usePussy != false)
                {
                    var squirtObj_target = targetAvatar.transform.Find(thisGimmick + "/<PCS Target> Pussy").gameObject;
                    squirtObj.transform.position = squirtObj_target.transform.position;
                    squirtObj.transform.eulerAngles = squirtObj_target.transform.eulerAngles;
                }
                else
                {
                    squirtObj.transform.position = hips.transform.position;
                    squirtObj.transform.eulerAngles = new Vector3(90, 0, 0);
                }
            }
            else
            {
                if (ref_pussy != null)
                {
                    var squirtObj_target = targetAvatar.transform.Find(thisGimmick + "/<PCS Target> Pussy").gameObject;
                    squirtObj.transform.position = squirtObj_target.transform.position;
                    squirtObj.transform.eulerAngles = squirtObj_target.transform.eulerAngles;
                }
                else
                {
                    squirtObj.transform.position = hips.transform.position;
                    squirtObj.transform.eulerAngles = new Vector3(90, 0, 0);
                }
            }
            var heartObj = targetAvatar.transform.Find(thisGimmick + "/<PCS Particle> Heart").gameObject;
            heartObj.transform.position = head.transform.position;

            //Set Reference Target
            if (preset == Preset.REFERENCE)
            {
                var mouth_comp = mouth.GetComponents(typeof(Component)).Where(o => !(o is Transform));
                foreach (var comp in mouth_comp)
                {
                    DestroyImmediate(comp);
                }
                var boobs_comp = boobs.GetComponents(typeof(Component)).Where(o => !(o is Transform));
                foreach (var comp in boobs_comp)
                {
                    DestroyImmediate(comp);
                }
                var pussy_comp = pussy.GetComponents(typeof(Component)).Where(o => !(o is Transform));
                foreach (var comp in pussy_comp)
                {
                    DestroyImmediate(comp);
                }
                var assh_comp = ass.GetComponents(typeof(Component)).Where(o => !(o is Transform));
                foreach (var comp in assh_comp)
                {
                    DestroyImmediate(comp);
                }

                var line = targetAvatar.transform.Find(thisGimmick + "/#########################").gameObject;
                DestroyImmediate(line);

                if (ref_mouth != null)  
                {     
                    mouth.transform.parent = ref_mouth.transform;
                    Debug.Log("PCS Mouth Target has been moved to <" + SearchUtils.GetHierarchyPath(ref_mouth) + "> due to using Reference preset.");
                }
                if (ref_boobs != null)
                {
                    boobs.transform.parent = ref_boobs.transform;
                    Debug.Log("PCS Boobs Target has been moved to <" + SearchUtils.GetHierarchyPath(ref_boobs) + "> due to using Reference preset.");
                }
                if (ref_pussy != null)
                {
                    pussy.transform.parent = ref_pussy.transform;
                    Debug.Log("PCS Pussy Target has been moved to <" + SearchUtils.GetHierarchyPath(ref_pussy) + "> due to using Reference preset.");
                }
                if (ref_ass != null)
                {
                    ass.transform.parent = ref_ass.transform;
                    Debug.Log("PCS Ass Target has been moved to <" + SearchUtils.GetHierarchyPath(ref_ass) + "> due to using Reference preset.");
                }
            }

            //Clear target
            if (preset != Preset.REFERENCE)
            {
                if (useMouth == false)
                {
                    DestroyImmediate(mouth);
                    var guide_target = targetAvatar.transform.Find(thisGimmick + "/Placement Icons (Auto Remove)/Mouth").gameObject;
                    DestroyImmediate(guide_target);
                }
                if (useBoobs == false)
                {
                    DestroyImmediate(boobs);
                    var guide_target = targetAvatar.transform.Find(thisGimmick + "/Placement Icons (Auto Remove)/Boobs").gameObject;
                    DestroyImmediate(guide_target);
                }
                if (usePussy == false)
                {
                    DestroyImmediate(pussy);
                    var guide_target = targetAvatar.transform.Find(thisGimmick + "/Placement Icons (Auto Remove)/Pussy").gameObject;
                    DestroyImmediate(guide_target);
                }
                if (useAss == false)
                {
                    DestroyImmediate(ass);
                    var guide_target = targetAvatar.transform.Find(thisGimmick + "/Placement Icons (Auto Remove)/Ass").gameObject;
                    DestroyImmediate(guide_target);
                }
            }
            else
            {
                if (ref_mouth == null)
                {
                    DestroyImmediate(mouth);
                    var guide_target = targetAvatar.transform.Find(thisGimmick + "/Placement Icons (Auto Remove)/Mouth").gameObject;
                    DestroyImmediate(guide_target);
                }
                if (ref_boobs == null)
                {
                    DestroyImmediate(boobs);
                    var guide_target = targetAvatar.transform.Find(thisGimmick + "/Placement Icons (Auto Remove)/Boobs").gameObject;
                    DestroyImmediate(guide_target);
                }
                if (ref_pussy == null)
                {
                    DestroyImmediate(pussy);
                    var guide_target = targetAvatar.transform.Find(thisGimmick + "/Placement Icons (Auto Remove)/Pussy").gameObject;
                    DestroyImmediate(guide_target);
                }
                if (ref_ass == null)
                {
                    DestroyImmediate(ass);
                    var guide_target = targetAvatar.transform.Find(thisGimmick + "/Placement Icons (Auto Remove)/Ass").gameObject;
                    DestroyImmediate(guide_target);
                }
            }

            //Smash Sensitivity
            var smashObj = targetAvatar.transform.Find(thisGimmick + "/PCS Contacts/Receiver/Receiver Local Position/Smash Hit");
            
            float sensitivity;
            sensitivity = 1 - smashSensitivity;
            smashObj.transform.localPosition = new Vector3(0, 0, - sensitivity/40);

            if (isError)
            {
                var pcs = targetAvatar.transform.Find(thisGimmick).gameObject;
                DestroyImmediate(pcs);
                isError = false;
            }
            else
            {
                if (confirm)
                {
                    EditorUtility.DisplayDialog(thisGimmick, "Setup Complete. PCS has been installed!", "OK");
                }
                Debug.Log("PCS has successfully installed and created an asset folder for \"" + targetAvatar.name + "\".");
            }

            //VFCFury locate assets
            Selection.activeObject = x;
        }
        private void GenerateFolder()
        {
            if (AssetDatabase.IsValidFolder("Assets/!Dismay Custom/" + thisGimmick + "/#GENERATED") == false)
            {
                AssetDatabase.CreateFolder("Assets/!Dismay Custom/" + thisGimmick, "#GENERATED");
            }
            if (AssetDatabase.IsValidFolder("Assets/!Dismay Custom/" + thisGimmick + "/#GENERATED/" + targetAvatar.name) == false)
            {                
                AssetDatabase.CreateFolder("Assets/!Dismay Custom/" + thisGimmick + "/#GENERATED", targetAvatar.name);
            }
        }
        private void AlignmentPreset(Transform mouth, Transform boobs, Transform pussy, Transform ass)
        {
            var head = animator.GetBoneTransform(HumanBodyBones.Head);
            var chest = animator.GetBoneTransform(HumanBodyBones.Chest);
            var hips = animator.GetBoneTransform(HumanBodyBones.Hips);
            var neck = animator.GetBoneTransform(HumanBodyBones.Neck);
            var legR = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
            var spine = animator.GetBoneTransform(HumanBodyBones.Spine);

            mouth.localScale = new(1, 1, 1);
            boobs.localScale = new(1, 1, 1);
            pussy.localScale = new(1, 1, 1);
            ass.localScale = new(1, 1, 1);

            switch (preset)
            {
                case Preset.REFERENCE:
                    if (ref_mouth != null)
                    {
                        mouth.transform.position = ref_mouth.transform.position;
                        mouth.transform.eulerAngles = ref_mouth.transform.eulerAngles;
                    }
                    if (ref_boobs != null)
                    {
                        boobs.transform.position = ref_boobs.transform.position;
                        boobs.transform.eulerAngles = ref_boobs.transform.eulerAngles;
                    }
                    if (ref_pussy != null)
                    {
                        pussy.transform.position = ref_pussy.transform.position;
                        pussy.transform.eulerAngles = ref_pussy.transform.eulerAngles;
                    }
                    if (ref_ass != null)
                    {
                        ass.transform.position = ref_ass.transform.position;
                        ass.transform.eulerAngles = ref_ass.transform.eulerAngles;
                    }
                    break;

                case Preset.GENERIC:
                    float mouthDist = Vector3.Distance(neck.position, head.position);
                    mouth.transform.position = new Vector3(head.transform.position.x, head.transform.position.y, head.transform.position.z + mouthDist);
                    mouth.transform.eulerAngles = new Vector3(0, 0, 0);

                    float boobsDist = Vector3.Distance(spine.position, chest.position);
                    boobs.transform.position = new Vector3(chest.transform.position.x, chest.transform.position.y + boobsDist / 1.3f, chest.transform.position.z + boobsDist/1.2f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);

                    float pussyDist = Vector3.Distance(hips.position, legR.position);
                    pussy.transform.position = new Vector3(hips.transform.position.x, hips.transform.position.y - pussyDist*1.25f, hips.transform.position.z + 0.02f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);

                    ass.transform.position = new Vector3(hips.transform.position.x, hips.transform.position.y - pussyDist*1.25f, hips.transform.position.z - 0.05f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);

                    break;

                case Preset.Shinano:
                    mouth.transform.position = new Vector3(0, 1.14f, 0.067f);
                    mouth.transform.eulerAngles = new Vector3(20, 0, 0);
                    boobs.transform.position = new Vector3(0, 0.96f, 0.0875f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.67f, 0.0124f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.685f, -0.0355f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Chiffon:
                    mouth.transform.position = new Vector3(0, 1.002f, 0.038f);
                    mouth.transform.eulerAngles = new Vector3(0, 0, 0);
                    boobs.transform.position = new Vector3(0, 0.86f, 0.065f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.59f, -0.001f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.598f, -0.04f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Mizuki:
                    mouth.transform.position = new Vector3(0, 1.258f, 0.094f);
                    mouth.transform.eulerAngles = new Vector3(0, 0, 0);
                    boobs.transform.position = new Vector3(0, 1.08f, 0.123f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.76f, 0.0445f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.766f, -0.0035f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Rurune:
                    mouth.transform.position = new Vector3(0, 1.202f, 0.095f);
                    mouth.transform.eulerAngles = new Vector3(0, 0, 0);
                    boobs.transform.position = new Vector3(0, 1.0282f, 0.118f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.72f, 0.042f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.725f, -0.0055f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Lasyusha:
                    mouth.transform.position = new Vector3(0, 1.395f, 0.065f);
                    mouth.transform.eulerAngles = new Vector3(0, 0, 0);
                    boobs.transform.position = new Vector3(0, 1.2f, 0.1f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.865f, 0.025f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.875f, -0.032f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Airi:
                    mouth.transform.position = new Vector3(0, 1.085f, 0.087f);
                    mouth.transform.eulerAngles = new Vector3(0, 0, 0);
                    boobs.transform.position = new Vector3(0, 0.93f, 0.105f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.663f, 0.028f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.67f, -0.002f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Uzuki:
                    mouth.transform.position = new Vector3(0, 1.13f, 0.055f);
                    mouth.transform.eulerAngles = new Vector3(23, 0, 0);
                    boobs.transform.position = new Vector3(0, 0.975f, 0.1035f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.702f, 0.035f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.705f, -0.006f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Aria:
                    mouth.transform.position = new Vector3(0, 1.105f, 0.093f);
                    mouth.transform.eulerAngles = new Vector3(23, 0, 0);
                    boobs.transform.position = new Vector3(0, 0.945f, 0.12f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.65f, 0.045f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.66f, -0.009f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Kikyo:
                    mouth.transform.position = new Vector3(0, 1.187f, 0.0745f);
                    mouth.transform.eulerAngles = new Vector3(23, 0, 0);
                    boobs.transform.position = new Vector3(0, 1.02f, 0.08f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.6825f, 0.0185f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.69f, -0.03f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Maya:
                    mouth.transform.position = new Vector3(0, 1.119f, 0.11f);
                    mouth.transform.eulerAngles = new Vector3(25, 0, 0);
                    boobs.transform.position = new Vector3(0, 0.95f, 0.12f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.653f, 0.035f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.66f, 0.002f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Rindo:
                    mouth.transform.position = new Vector3(0, 1.1265f, 0.076f);
                    mouth.transform.eulerAngles = new Vector3(25, 0, 0);
                    boobs.transform.position = new Vector3(0, 0.97f, 0.075f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.6685f, 0.036f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.678f, -0.015f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Selestia:
                    mouth.transform.position = new Vector3(0, 1.124f, 0.078f);
                    mouth.transform.eulerAngles = new Vector3(25, 0, 0);
                    boobs.transform.position = new Vector3(0, 0.96f, 0.1f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.67f, 0.03f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.68f, -0.02f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.UltimateKissMa:
                    mouth.transform.position = new Vector3(0, 1.115f, 0.065f);
                    mouth.transform.eulerAngles = new Vector3(25, 0, 0);
                    boobs.transform.position = new Vector3(0, 0.95f, 0.098f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.644f, 0.017f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.645f, -0.03f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Moe:
                    mouth.transform.position = new Vector3(0, 1.219f, 0.088f);
                    mouth.transform.eulerAngles = new Vector3(23, 0, 0);
                    boobs.transform.position = new Vector3(0, 1.038f, 0.125f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.717f, 0.03f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.723f, -0.02f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Karin:
                    mouth.transform.position = new Vector3(0, 1.061f, 0.052f);
                    mouth.transform.eulerAngles = new Vector3(28, 0, 0);
                    boobs.transform.position = new Vector3(0, 0.905f, 0.063f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.607f, 0.0115f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.612f, -0.027f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Lime:
                    mouth.transform.position = new Vector3(0, 1.1205f, 0.039f);
                    mouth.transform.eulerAngles = new Vector3(28, 0, 0);
                    boobs.transform.position = new Vector3(0, 0.955f, 0.0555f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.652f, -0.0045f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.66f, -0.0475f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Anon:
                    mouth.transform.position = new Vector3(0, 1.13f, 0.078f);
                    mouth.transform.eulerAngles = new Vector3(30, 0, 0);
                    boobs.transform.position = new Vector3(0, 0.965f, 0.093f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.663f, 0.035f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.665f, -0.02f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Leefa:
                    mouth.transform.position = new Vector3(0, 1.104f, 0.0755f);
                    mouth.transform.eulerAngles = new Vector3(25, 0, 0);
                    boobs.transform.position = new Vector3(0, 0.95f, 0.083f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.654f, 0.0215f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.656f, -0.024f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Imeris:
                    mouth.transform.position = new Vector3(0, 1.22f, 0.0655f);
                    mouth.transform.eulerAngles = new Vector3(28, 0, 0);
                    boobs.transform.position = new Vector3(0, 1.02f, 0.13f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.699f, 0.0105f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.705f, -0.034f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Manuka:
                    mouth.transform.position = new Vector3(0, 1.092f, 0.072f);
                    mouth.transform.eulerAngles = new Vector3(30, 0, 0);
                    boobs.transform.position = new Vector3(0, 0.945f, 0.1f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.672f, 0.0225f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.68f, -0.014f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Velle:
                    mouth.transform.position = new Vector3(0, 1.194f, 0.0835f);
                    mouth.transform.eulerAngles = new Vector3(30, 0, 0);
                    boobs.transform.position = new Vector3(0, 1.02f, 0.11f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.698f, 0.025f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.712f, -0.018f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Shinra:
                    mouth.transform.position = new Vector3(0, 1.295f, 0.07f);
                    mouth.transform.eulerAngles = new Vector3(20, 0, 0);
                    boobs.transform.position = new Vector3(0, 1.09f, 0.1f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.7495f, 0);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.756f, -0.05f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Eyo:
                    mouth.transform.position = new Vector3(0, 1.17f, 0.07f);
                    mouth.transform.eulerAngles = new Vector3(25, 0, 0);
                    boobs.transform.position = new Vector3(0, 0.99f, 0.11f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.673f, 0.013f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.68f, -0.04f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Mophira:
                    mouth.transform.position = new Vector3(0, 1.218f, 0.11f);
                    mouth.transform.eulerAngles = new Vector3(25, 0, 0);
                    boobs.transform.position = new Vector3(0, 1.03f, 0.14f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.717f, 0.055f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.745f, 0.005f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Runa_Robotic:
                    mouth.transform.position = new Vector3(0, 1.188f, 0.05f);
                    mouth.transform.eulerAngles = new Vector3(30, 0, 0);
                    boobs.transform.position = new Vector3(0, 1.035f, 0.075f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.685f, 0.005f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.685f, -0.05f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Sio:
                    mouth.transform.position = new Vector3(0, 1.2f, 0.045f);
                    mouth.transform.eulerAngles = new Vector3(20, 0, 0);
                    boobs.transform.position = new Vector3(0, 1.03f, 0.085f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.726f, -0.01f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.74f, -0.05f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Wolferia:
                    mouth.transform.position = new Vector3(0, 1.196f, 0.085f);
                    mouth.transform.eulerAngles = new Vector3(23, 0, 0);
                    boobs.transform.position = new Vector3(0, 1, 0.12f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.688f, 0.0125f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.69f, -0.03f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;
            }
        }
        private void Remove(bool confirm)
        {
            if (confirm)
            {
                if (EditorUtility.DisplayDialog(thisGimmick, "Are you sure you want to remove this gimmick?", "Yes", "No"))
                {
                    RemoveFunction();
                }
            }
            else
            {
                RemoveFunction();
            }
        }
        private void RemoveFunction()
        {
            var removeTarget = targetAvatar.transform.Find(thisGimmick).gameObject;
            DestroyImmediate(removeTarget);

            //var children = FindObjectsOfType(typeof(GameObject), true) as GameObject[];
            var children = targetAvatar.GetComponentsInChildren<Component>(true);
            GameObject[] target = new GameObject[children.Length];
            for (int i = 0; i < children.Length; i++)
            {
                if (children[i].name.Contains("<PCS Target>") && (children[i].transform.IsChildOf(targetAvatar.transform)))
                {
                    target[i] = children[i].gameObject;
                }
            }
            for (int i = 0; i < children.Length; i++)
            {
                DestroyImmediate((target[i]));
            }

            AssetDatabase.DeleteAsset("Assets/!Dismay Custom/" + thisGimmick + "/#GENERATED/" + targetAvatar.name);
        }
        private void GenerateParameter()
        {
            //Add parameters for VRCFury
            List<VRCExpressionParameters.Parameter> parameterList = new(); //Make new empty list of parameters

            //Add main parameters
            var dummy = Resources.Load("Expression Menu/PCS Blank Param", typeof(VRCExpressionParameters)) as VRCExpressionParameters;
            string folderPath = "Assets/!Dismay Custom/Penetration Contact System/#GENERATED/" + targetAvatar.name;
            if (AssetDatabase.IsValidFolder(folderPath) == true)
            {
                AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(dummy), folderPath + "/!Install Parameter_" + targetAvatar.name + ".asset");
                VRCExpressionParameters generateParam = (VRCExpressionParameters)AssetDatabase.LoadAssetAtPath(folderPath + "/!Install Parameter_" + targetAvatar.name + ".asset", typeof(VRCExpressionParameters));
                VRCExpressionParameters.Parameter[] parameterArray = generateParam.parameters;
                parameterArray = parameterArray.Where(x => !x.name.StartsWith("pcs/")).ToArray();
                generateParam.parameters = parameterArray;

                //Default
                VRCExpressionParameters.Parameter param_default1 = new()
                {
                    name = "pcs/isEnable",
                    valueType = VRCExpressionParameters.ValueType.Bool,
                    defaultValue = 0,
                    networkSynced = true,
                    saved = false
                };
                parameterList.Add(param_default1);

                VRCExpressionParameters.Parameter param_default2 = new()
                {
                    name = "pcs/mode/smashHit",
                    valueType = VRCExpressionParameters.ValueType.Bool,
                    defaultValue = 1,
                    networkSynced = true,
                    saved = true
                };
                parameterList.Add(param_default2);

                VRCExpressionParameters.Parameter param_default3 = new()
                {
                    name = "pcs/mode/selfService",
                    valueType = VRCExpressionParameters.ValueType.Bool,
                    defaultValue = 0,
                    networkSynced = true,
                    saved = true
                };
                parameterList.Add(param_default3);

                VRCExpressionParameters.Parameter param_default4 = new()
                {
                    name = "pcs/mode/selfTouch",
                    valueType = VRCExpressionParameters.ValueType.Bool,
                    defaultValue = 1,
                    networkSynced = true,
                    saved = true
                };
                parameterList.Add(param_default4);

                VRCExpressionParameters.Parameter param_default5 = new()
                {
                    name = "pcs/sound/smash",
                    valueType = VRCExpressionParameters.ValueType.Bool,
                    defaultValue = 1,
                    networkSynced = true,
                    saved = true
                };
                parameterList.Add(param_default5);

                VRCExpressionParameters.Parameter param_default6 = new()
                {
                    name = "pcs/mode/insertSquirt",
                    valueType = VRCExpressionParameters.ValueType.Bool,
                    defaultValue = 0,
                    networkSynced = true,
                    saved = true
                };
                parameterList.Add(param_default6);

                VRCExpressionParameters.Parameter param_default7 = new()
                {
                    name = "pcs/satisfaction/orgasm",
                    valueType = VRCExpressionParameters.ValueType.Bool,
                    defaultValue = 0,
                    networkSynced = true,
                    saved = false
                };
                parameterList.Add(param_default7);

                VRCExpressionParameters.Parameter param_default8 = new()
                {
                    name = "pcs/reset",
                    valueType = VRCExpressionParameters.ValueType.Bool,
                    defaultValue = 0,
                    networkSynced = true,
                    saved = false
                };
                parameterList.Add(param_default8);

                VRCExpressionParameters.Parameter param_default9 = new()
                {
                    name = "pcs/mode/autoDetect",
                    valueType = VRCExpressionParameters.ValueType.Bool,
                    defaultValue = 0,
                    networkSynced = true,
                    saved = true
                };
                parameterList.Add(param_default9);

                //Satisfaction
                VRCExpressionParameters.Parameter param_satis1 = new()
                {
                    name = "pcs/local/lustMultiplier",
                    valueType = VRCExpressionParameters.ValueType.Float,
                    defaultValue = lustMultiplierValue,
                    networkSynced = false,
                    saved = false
                };
                parameterList.Add(param_satis1);

                if (lustFeature)
                {
                    VRCExpressionParameters.Parameter param_satis2 = new()
                    {
                        name = "pcs/satisfaction/lust",
                        valueType = VRCExpressionParameters.ValueType.Float,
                        defaultValue = -1,
                        networkSynced = true,
                        saved = true
                    };
                    parameterList.Add(param_satis2);

                    VRCExpressionParameters.Parameter param_satis3 = new()
                    {
                        name = "pcs/satisfaction/edging",
                        valueType = VRCExpressionParameters.ValueType.Bool,
                        defaultValue = 0,
                        networkSynced = true,
                        saved = false
                    };
                    parameterList.Add(param_satis3);
                }

                //Add moan param if use
                if (voicePack != VoicePack.Disable)
                {
                    VRCExpressionParameters.Parameter param_voice = new()
                    {
                        name = "pcs/sound/moan",
                        valueType = VRCExpressionParameters.ValueType.Bool,
                        defaultValue = 1,
                        networkSynced = true,
                        saved = true
                    };
                    parameterList.Add(param_voice);
                }

                //Add selection parameters
                if (preset != Preset.REFERENCE)
                {
                    if (useMouth)
                    {
                        VRCExpressionParameters.Parameter param_select = new()
                        {
                            name = "pcs/select/mouth",
                            valueType = VRCExpressionParameters.ValueType.Bool,
                            defaultValue = 0,
                            networkSynced = true,
                            saved = true
                        };
                        parameterList.Add(param_select);
                    }
                    if (useBoobs)
                    {
                        VRCExpressionParameters.Parameter param_select = new()
                        {
                            name = "pcs/select/boobs",
                            valueType = VRCExpressionParameters.ValueType.Bool,
                            defaultValue = 0,
                            networkSynced = true,
                            saved = true
                        };
                        parameterList.Add(param_select);
                    }
                    if (usePussy)
                    {
                        VRCExpressionParameters.Parameter param_select = new()
                        {
                            name = "pcs/select/pussy",
                            valueType = VRCExpressionParameters.ValueType.Bool,
                            defaultValue = 0,
                            networkSynced = true,
                            saved = true
                        };
                        parameterList.Add(param_select);
                    }
                    if (useAss)
                    {
                        VRCExpressionParameters.Parameter param_select = new()
                        {
                            name = "pcs/select/ass",
                            valueType = VRCExpressionParameters.ValueType.Bool,
                            defaultValue = 0,
                            networkSynced = true,
                            saved = true
                        };
                        parameterList.Add(param_select);
                    }
                }
                else
                {
                    if (ref_mouth != null)
                    {
                        VRCExpressionParameters.Parameter param_select = new()
                        {
                            name = "pcs/select/mouth",
                            valueType = VRCExpressionParameters.ValueType.Bool,
                            defaultValue = 0,
                            networkSynced = true,
                            saved = true
                        };
                        parameterList.Add(param_select);
                    }
                    if (ref_boobs != null)
                    {
                        VRCExpressionParameters.Parameter param_select = new()
                        {
                            name = "pcs/select/boobs",
                            valueType = VRCExpressionParameters.ValueType.Bool,
                            defaultValue = 0,
                            networkSynced = true,
                            saved = true
                        };
                        parameterList.Add(param_select);
                    }
                    if (ref_pussy != null)
                    {
                        VRCExpressionParameters.Parameter param_select = new()
                        {
                            name = "pcs/select/pussy",
                            valueType = VRCExpressionParameters.ValueType.Bool,
                            defaultValue = 0,
                            networkSynced = true,
                            saved = true
                        };
                        parameterList.Add(param_select);
                    }
                    if (ref_ass != null)
                    {
                        VRCExpressionParameters.Parameter param_select = new()
                        {
                            name = "pcs/select/ass",
                            valueType = VRCExpressionParameters.ValueType.Bool,
                            defaultValue = 0,
                            networkSynced = true,
                            saved = true
                        };
                        parameterList.Add(param_select);
                    }
                }

                //Add Custom positions
                if (selected_customPos != 0)
                {
                    for (int i = 0; i < selected_customPos; i++)
                    {
                        VRCExpressionParameters.Parameter[] param_cusotm = new VRCExpressionParameters.Parameter[8];
                        param_cusotm[i] = new VRCExpressionParameters.Parameter
                        {
                            name = "pcs/select/custom" + (i+1),
                            defaultValue = 0,
                            networkSynced = true,
                            saved = true,
                            valueType = VRCExpressionParameters.ValueType.Bool
                        };
                        parameterList.Add(param_cusotm[i]);
                    }
                }

                generateParam.parameters = generateParam.parameters.Concat(parameterList.ToArray()).ToArray();
                EditorUtility.SetDirty(generateParam);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
        private void GenerateMenu()
        {
            string iconPath = "Assets/!Dismay Custom/Penetration Contact System/Assets/Icons/";
            string folderPath = "Assets/!Dismay Custom/Penetration Contact System/#GENERATED/" + targetAvatar.name;
            Texture2D icon_custom = (Texture2D)AssetDatabase.LoadAssetAtPath(iconPath + "custom.png", typeof(Texture2D));
            Texture2D icon_mouth = (Texture2D)AssetDatabase.LoadAssetAtPath(iconPath + "mouth.png", typeof(Texture2D));
            Texture2D icon_boobs = (Texture2D)AssetDatabase.LoadAssetAtPath(iconPath + "boobs.png", typeof(Texture2D));
            Texture2D icon_pussy = (Texture2D)AssetDatabase.LoadAssetAtPath(iconPath + "pussy.png", typeof(Texture2D));
            Texture2D icon_ass = (Texture2D)AssetDatabase.LoadAssetAtPath(iconPath + "ass.png", typeof(Texture2D));
            Texture2D icon_heart = (Texture2D)AssetDatabase.LoadAssetAtPath(iconPath + "heart.png", typeof(Texture2D));
            Texture2D icon_pcs = (Texture2D)AssetDatabase.LoadAssetAtPath(iconPath + "PCS Icon.png", typeof(Texture2D));

            if (AssetDatabase.IsValidFolder(folderPath) == true)
            {
                var selectionMenu_load = Resources.Load("Expression Menu/PCS Blank Menu") as VRCExpressionsMenu;
                AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(selectionMenu_load), folderPath + "/Selection Menu1.asset");
                var selectionMenu = (VRCExpressionsMenu)AssetDatabase.LoadAssetAtPath(folderPath + "/Selection Menu1.asset", typeof(VRCExpressionsMenu));

                //Generate main selection submenu
                string mouth_label, boobs_label, pussy_label, ass_label;
                mouth_label = "Mouth";
                boobs_label = "Boobs";
                pussy_label = "Pussy";
                ass_label = "Ass";

                //Main Poosition
                if (preset != Preset.REFERENCE)
                {
                    if (useMouth)
                    {
                        VRCExpressionsMenu.Control.Parameter selection_parameter = new()
                        {
                            name = "pcs/select/mouth",
                        };
                        VRCExpressionsMenu.Control selection_menu_control = new()
                        {
                            name = mouth_label,
                            parameter = selection_parameter,
                            value = 1,
                            type = VRCExpressionsMenu.Control.ControlType.Toggle,
                            icon = icon_mouth
                        };
                        selectionMenu.controls.Add(selection_menu_control);
                    }
                    if (useBoobs)
                    {
                        VRCExpressionsMenu.Control.Parameter selection_parameter = new()
                        {
                            name = "pcs/select/boobs",
                        };
                        VRCExpressionsMenu.Control selection_menu_control = new()
                        {
                            name = boobs_label,
                            parameter = selection_parameter,
                            value = 1,
                            type = VRCExpressionsMenu.Control.ControlType.Toggle,
                            icon = icon_boobs
                        };
                        selectionMenu.controls.Add(selection_menu_control);
                    }
                    if (usePussy)
                    {
                        VRCExpressionsMenu.Control.Parameter selection_parameter = new()
                        {
                            name = "pcs/select/pussy",
                        };
                        VRCExpressionsMenu.Control selection_menu_control = new()
                        {
                            name = pussy_label,
                            parameter = selection_parameter,
                            value = 1,
                            type = VRCExpressionsMenu.Control.ControlType.Toggle,
                            icon = icon_pussy
                        };
                        selectionMenu.controls.Add(selection_menu_control);
                    }
                    if (useAss)
                    {
                        VRCExpressionsMenu.Control.Parameter selection_parameter = new()
                        {
                            name = "pcs/select/ass",
                        };
                        VRCExpressionsMenu.Control selection_menu_control = new()
                        {
                            name = ass_label,
                            parameter = selection_parameter,
                            value = 1,
                            type = VRCExpressionsMenu.Control.ControlType.Toggle,
                            icon = icon_ass
                        };
                        selectionMenu.controls.Add(selection_menu_control);
                    }
                }
                else
                {
                    if (ref_mouth != null)
                    {
                        VRCExpressionsMenu.Control.Parameter selection_parameter = new()
                        {
                            name = "pcs/select/mouth",
                        };
                        VRCExpressionsMenu.Control selection_menu_control = new()
                        {
                            name = mouth_label,
                            parameter = selection_parameter,
                            value = 1,
                            type = VRCExpressionsMenu.Control.ControlType.Toggle,
                            icon = icon_mouth
                        };
                        selectionMenu.controls.Add(selection_menu_control);
                    }
                    if (ref_boobs != null)
                    {
                        VRCExpressionsMenu.Control.Parameter selection_parameter = new()
                        {
                            name = "pcs/select/boobs",
                        };
                        VRCExpressionsMenu.Control selection_menu_control = new()
                        {
                            name = boobs_label,
                            parameter = selection_parameter,
                            value = 1,
                            type = VRCExpressionsMenu.Control.ControlType.Toggle,
                            icon = icon_boobs
                        };
                        selectionMenu.controls.Add(selection_menu_control);
                    }
                    if (ref_pussy != null)
                    {
                        VRCExpressionsMenu.Control.Parameter selection_parameter = new()
                        {
                            name = "pcs/select/pussy",
                        };
                        VRCExpressionsMenu.Control selection_menu_control = new()
                        {
                            name = pussy_label,
                            parameter = selection_parameter,
                            value = 1,
                            type = VRCExpressionsMenu.Control.ControlType.Toggle,
                            icon = icon_pussy
                        };
                        selectionMenu.controls.Add(selection_menu_control);
                    }
                    if (ref_ass != null)
                    {
                        VRCExpressionsMenu.Control.Parameter selection_parameter = new()
                        {
                            name = "pcs/select/ass",
                        };
                        VRCExpressionsMenu.Control selection_menu_control = new()
                        {
                            name = ass_label,
                            parameter = selection_parameter,
                            value = 1,
                            type = VRCExpressionsMenu.Control.ControlType.Toggle,
                            icon = icon_ass
                        };
                        selectionMenu.controls.Add(selection_menu_control);
                    }
                }
                //End main selection

                //Generate custom selection submenu
                if (selected_customPos != 0)
                {
                    int menuCount, mainCount;
                    int mouth = Convert.ToInt32(this.useMouth);
                    int boobs = Convert.ToInt32(this.useBoobs);
                    int pussy = Convert.ToInt32(this.usePussy);
                    int ass = Convert.ToInt32(this.useAss);

                    mainCount = (mouth + boobs + pussy + ass);
                    menuCount = selected_customPos + (mouth + boobs + pussy + ass);
                    if (menuCount < 9)
                    {
                        Texture2D iconX = (Texture2D)AssetDatabase.LoadAssetAtPath(iconPath + "custom.png", typeof(Texture2D));
                        for (int i = 0; i < selected_customPos; i++)
                        {
                            VRCExpressionsMenu.Control.Parameter[] selection_menu_parameter1 = new VRCExpressionsMenu.Control.Parameter[8];
                            selection_menu_parameter1[i] = new VRCExpressionsMenu.Control.Parameter
                            {
                                name = "pcs/select/custom" + (i + 1),
                            };

                            VRCExpressionsMenu.Control[] selection_menu_control1 = new VRCExpressionsMenu.Control[8];
                            selection_menu_control1[i] = new VRCExpressionsMenu.Control
                            {
                                name = customPos_menuName[i],
                                parameter = selection_menu_parameter1[i],
                                value = 1,
                                type = VRCExpressionsMenu.Control.ControlType.Toggle,
                                icon = iconX
                            };
                            selectionMenu.controls.Add(selection_menu_control1[i]);
                        }
                    }
                    else
                    {
                        Texture2D iconX = (Texture2D)AssetDatabase.LoadAssetAtPath(iconPath + "custom.png", typeof(Texture2D));
                        for (int i = 0; i < 7 - mainCount; i++)
                        {
                            VRCExpressionsMenu.Control.Parameter[] selection_menu_parameter2 = new VRCExpressionsMenu.Control.Parameter[8];
                            selection_menu_parameter2[i] = new VRCExpressionsMenu.Control.Parameter
                            {
                                name = "pcs/select/custom" + (i + 1),
                            };

                            VRCExpressionsMenu.Control[] selection_menu_control2 = new VRCExpressionsMenu.Control[8];
                            selection_menu_control2[i] = new VRCExpressionsMenu.Control
                            {
                                name = customPos_menuName[i],
                                parameter = selection_menu_parameter2[i],
                                value = 1,
                                type = VRCExpressionsMenu.Control.ControlType.Toggle,
                                icon = iconX
                            };
                            selectionMenu.controls.Add(selection_menu_control2[i]);
                        }
                        //Next page
                        var nextPage_blank = Resources.Load("Expression Menu/PCS Blank Menu") as VRCExpressionsMenu;
                        AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(nextPage_blank), folderPath + "/Selection Menu2.asset");
                        var nextPage = (VRCExpressionsMenu)AssetDatabase.LoadAssetAtPath(folderPath + "/Selection Menu2.asset", typeof(VRCExpressionsMenu));
                        Texture2D icon_nextPage = (Texture2D)AssetDatabase.LoadAssetAtPath(iconPath + "next.png", typeof(Texture2D));

                        VRCExpressionsMenu.Control selection_menu_next;
                        selection_menu_next = new VRCExpressionsMenu.Control
                        {
                            name = "Next >",
                            type = VRCExpressionsMenu.Control.ControlType.SubMenu,
                            icon = icon_nextPage,
                            subMenu = nextPage,
                        };
                        selectionMenu.controls.Add(selection_menu_next);

                        for (int i = (7 - mainCount); i < selected_customPos; i++)
                        {
                            VRCExpressionsMenu.Control.Parameter[] selection_menu_parameter3 = new VRCExpressionsMenu.Control.Parameter[8];
                            selection_menu_parameter3[i] = new VRCExpressionsMenu.Control.Parameter
                            {
                                name = "pcs/select/custom" + (i + 1),
                            };

                            VRCExpressionsMenu.Control[] selection_menu_control3 = new VRCExpressionsMenu.Control[8];
                            selection_menu_control3[i] = new VRCExpressionsMenu.Control
                            {
                                name = customPos_menuName[i],
                                parameter = selection_menu_parameter3[i],
                                value = 1,
                                type = VRCExpressionsMenu.Control.ControlType.Toggle,
                                icon = iconX
                            };
                            nextPage.controls.Add(selection_menu_control3[i]);
                        }
                        EditorUtility.SetDirty(nextPage);
                    }
                }

                //Generate main menu
                var menu_ref = Resources.Load("Expression Menu/PCS Main Menu") as VRCExpressionsMenu;
                AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(menu_ref), folderPath + "/Main Menu.asset");
                var main_menu = (VRCExpressionsMenu)AssetDatabase.LoadAssetAtPath(folderPath + "/Main Menu.asset", typeof(VRCExpressionsMenu));

                //Add selection menu to main menu
                VRCExpressionsMenu.Control control_selection = new()
                {
                    name = "Sound & Location", //Selection menu name
                    icon = icon_custom,
                    type = VRCExpressionsMenu.Control.ControlType.SubMenu,
                    subMenu = selectionMenu,
                };
                main_menu.controls.Add(control_selection);

                //Satisfaction menu
                var satis_ref = Resources.Load("Expression Menu/PCS Satisfaction Menu") as VRCExpressionsMenu;
                AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(satis_ref), folderPath + "/Satisfaction Menu.asset");
                var satisMenu = (VRCExpressionsMenu)AssetDatabase.LoadAssetAtPath(folderPath + "/Satisfaction Menu.asset", typeof(VRCExpressionsMenu));
                VRCExpressionsMenu.Control control_satisfaction = new()
                {
                    name = "Satisfaction",
                    icon = icon_heart,
                    type = VRCExpressionsMenu.Control.ControlType.SubMenu,
                    subMenu = satisMenu,
                };
                main_menu.controls.Add(control_satisfaction);

                //Remove unused menu if lust is not in use
                if (!lustFeature)
                {
                    VRCExpressionsMenu.Control[] array = satisMenu.controls.ToArray();

                    array = array.Where(x => !x.name.StartsWith("Edging (Pause Event)")).ToArray();
                    satisMenu.controls = array.ToList();

                    array = array.Where(x => !x.name.StartsWith("Voice")).ToArray();
                    satisMenu.controls = array.ToList();
                }
                else
                {
                    if (voicePack == VoicePack.Disable)
                    {
                        VRCExpressionsMenu.Control[] array = satisMenu.controls.ToArray();
                        array = array.Where(x => !x.name.StartsWith("Voice")).ToArray();
                        satisMenu.controls = array.ToList();
                    }
                }

                //Placeholder Top Menu
                var topMenu_blank = Resources.Load("Expression Menu/PCS Blank Menu") as VRCExpressionsMenu;
                AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(topMenu_blank), folderPath + "/!Install Menu_" + targetAvatar.name + ".asset");
                var topMenu = (VRCExpressionsMenu)AssetDatabase.LoadAssetAtPath(folderPath + "/!Install Menu_" + targetAvatar.name + ".asset", typeof(VRCExpressionsMenu));

                VRCExpressionsMenu.Control control_mainMenu = new()
                {
                    name = "<b>PCS v" + version + "</b>",
                    icon = icon_pcs,
                    type = VRCExpressionsMenu.Control.ControlType.SubMenu,
                    subMenu = main_menu,
                };
                topMenu.controls.Add(control_mainMenu);

                EditorUtility.SetDirty(main_menu);
                EditorUtility.SetDirty(selectionMenu);
                EditorUtility.SetDirty(topMenu);
                EditorUtility.SetDirty(satisMenu);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                //VFCFury locate assets
                EditorGUIUtility.PingObject(topMenu);

            }
            else
            {
                isError = true;
                Debug.LogError("PCS could not find the resource folder path for the avatar you selected. Please check that your avatar name does not contain any prohibited characters for folder naming, such as <>:\"|?*. Please rename and try again.");
                EditorUtility.DisplayDialog(thisGimmick, "PCS could not find the resource folder path for the avatar you selected.\n\nPlease check that your avatar name does not contain any prohibited characters for folder naming, such as <>:\"|?*. Please rename and try again.", "OK");
            }
        }
        private int CalculateParamUse()
        {
            int defaultUsage = 9;
            int useLust;
            int useVoice;
            int result;

            if (lustFeature)
            {
                useLust = 9;
                if(voicePack != VoicePack.Disable)
                {
                    useVoice = 1;
                }
                else
                {
                    useVoice = 0;
                }
            }
            else
            {
                useLust = 0;
                useVoice = 0;
            }

            if(preset != Preset.REFERENCE)
            {
                result = defaultUsage + useLust + useVoice + Convert.ToInt32(useMouth) + Convert.ToInt32(useBoobs) + Convert.ToInt32(usePussy) + Convert.ToInt32(useAss) + selected_customPos;
                return result;
            }
            else
            {
                result = defaultUsage + useLust + useVoice + Convert.ToInt32(ref_mouth) + Convert.ToInt32(ref_boobs) + Convert.ToInt32(ref_pussy) + Convert.ToInt32(ref_ass) + selected_customPos;
                return result;
            }
        }
        private void ShowParameter()
        {
            EditorGUILayout.Space();

            int paramCost;
            paramCost = CalculateParamUse();
            GUILayout.Label("  Memory Usage: <color=lime>" + paramCost + "</color>", paramStyle, GUILayout.Width(495));
        }
        private void ShowFooter()
        {
            EditorGUILayout.Space();

            if (!targetAvatar)
            {
                EditorGUILayout.HelpBox("Please drag and drop your avatar into the box.", MessageType.Warning);

                var upadate = Resources.Load<TextAsset>("Components/" + thisGimmick + "_update").ToString();
                EditorGUILayout.HelpBox(upadate, MessageType.Info);
            }

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            var info = Resources.Load<TextAsset>("Components/" + thisGimmick + "_info").ToString();
            GUILayout.Label(info.Replace("$", "v" + version), infoStyle, GUILayout.Width(295));
            if (GUILayout.Button("Tutorial"))
            {
                Application.OpenURL("https://youtube.com/playlist?list=PLEvAOTfSR8u0x16IbOgqZI-rE6b5NsoJJ&si=R6OipQwOnObVz9h8");
            }
            if (GUILayout.Button("Discord"))
            {
                Application.OpenURL("https://discord.gg/TkfRyQDNQC");
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.Space(5);
        }
    }
}
