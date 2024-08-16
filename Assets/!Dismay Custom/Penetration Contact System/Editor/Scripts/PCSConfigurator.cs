/*
MIT License

“Copyright © 2023, Dismay

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using UnityEngine;
using UnityEditor;
using VRC.SDK3.Avatars.Components;
using UnityEngine.Animations;
using VRC.SDK3.Avatars.ScriptableObjects;
using System.Collections.Generic;
using System.Linq;

namespace DMCustom
{
    public class PCSConfigurator : EditorWindow
    {
        private VRCAvatarDescriptor targetAvatar;
        private GameObject mouthPosition, customPosition, mouth, boobs, pussy, ass;
        private GameObject ref_mouth, ref_boobs, ref_pussy, ref_ass;
        private bool satisFeature;
        private ConstraintSource constraintSource, guideSource;
        private GUIStyle paramStyle, infoStyle;
        private Texture2D logo;
        private int TotalCost;
        private float lustMultiplierValue = 0.4f, smashSensitivity = 0.01f;
        public readonly string[] prefabPath =
{
            "Modular Avatar/PCS Modular (wLust)",
            "Modular Avatar/PCS Modular (woLust)",
            "VRCFury/PCS VRCF (wLust)",
            "VRCFury/PCS VRCF (woLust)",
        };

        public readonly string[] voicePath =
        {
            "Voice Pack/#PCS - Voice Pack (Anime)",
            "Voice Pack/#PCS - Voice Pack (Mature)",
            "Voice Pack/#PCS - Voice Pack (Custom)",
        };
        private readonly int[] paramCost = { 9, 22 };
        private enum SetupTool { ModularAvatar, VRCFury }
        private SetupTool setupTool = SetupTool.ModularAvatar;
        private enum Moaning { Disable, Anime, Mature, Custom }
        private Moaning moaning = Moaning.Disable;
        private Preset preset = Preset.Generic;
        private enum Preset
        {
            Generic,
            Reference,

            Anon,
            Aria,
            Eyo,
            Fiona,
            Imeris,
            Karin,
            Kikyo,
            Kokoa,
            Kuronatu,
            Leefa,
            Lime,
            Manuka,
            Maya,
            Moe,
            Mophira,
            Rindo,
            Runa_Robotic,
            Selestia,
            Shinra,
            Soraha,
            Tien,
            UltimateKissMa,
            Uzuki,
            Velle,
            Wolferia,

            Body_LexzBase,
            Body_Panda,
            Body_ToriBase,
            Body_TVF,
            Body_ZinFit,
            Body_ZinRP,

        }
        //ends variables ***************************************************************************************************************

        [MenuItem("Tools/Dismay Custom/Penetration Contact System")]
        public static void ShowpWindow()
        {
            var window = GetWindow(typeof(PCSConfigurator));
            float winWidth = 360, winHeight = 510;
            window.titleContent = new GUIContent("Penetration Contact System");
            window.position = new Rect((Screen.width / 2) + winWidth / 1.555f, (Screen.height / 2) - winHeight / 4f, winWidth, winHeight);
            window.minSize = new Vector2(winWidth, winHeight);
            window.maxSize = new Vector2(winWidth, winHeight);
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

            logo = Resources.Load<Texture2D>("logo");
            GUILayout.Label(logo, new GUIStyle { fixedWidth = 360, fixedHeight = 170 });
            EditorGUILayout.Space(1);

            targetAvatar = EditorGUILayout.ObjectField(targetAvatar, typeof(VRCAvatarDescriptor), true, GUILayout.Height(30)) as VRCAvatarDescriptor;
            Processing();
        }
        private void Processing()
        {
            if (targetAvatar)
            {
                EditorGUILayout.Space(1);
                setupTool = (SetupTool)EditorGUILayout.EnumPopup("Setup Tools: ", setupTool);
                preset = (Preset)EditorGUILayout.EnumPopup("Alignment Preset: ", preset);

                if(preset == Preset.Reference)
                {
                    ref_mouth = EditorGUILayout.ObjectField("├ Mouth Position:", ref_mouth, typeof(GameObject), true) as GameObject;
                    ref_boobs = EditorGUILayout.ObjectField("├ Boobs Position:", ref_boobs, typeof(GameObject), true) as GameObject;
                    ref_pussy = EditorGUILayout.ObjectField("├ Pussy Position:", ref_pussy, typeof(GameObject), true) as GameObject;
                    ref_ass = EditorGUILayout.ObjectField("└ Anal Position:", ref_ass, typeof(GameObject), true) as GameObject;
                }

                customPosition = EditorGUILayout.ObjectField("Custom Target (Optional):", customPosition, typeof(GameObject), true) as GameObject;
                smashSensitivity = EditorGUILayout.Slider("Smash Sensitivity: ", smashSensitivity, 0, 0.025f);

                satisFeature = EditorGUILayout.Toggle("Satisfaction Feature:", satisFeature);

                if (satisFeature == true)
                {
                    moaning = (Moaning)EditorGUILayout.EnumPopup("├ Voice Pack:", moaning);
                    lustMultiplierValue = EditorGUILayout.Slider("└ Lust Multiplier:", lustMultiplierValue, 0, 1);
                }

                var PCS = targetAvatar.transform.Find("Penetration Contact System");
                if (TotalCost <= 256)
                {
                    EditorGUILayout.Space();
                    if (PCS == null)
                    {
                        GUI.enabled = true;
                        if (GUILayout.Button("Apply"))
                        {
                            Apply(true, false);
                        }
                        GUI.enabled = false;
                        if (GUILayout.Button("Remove")) { }
                    }
                    else
                    {
                        GUI.enabled = true;
                        if (GUILayout.Button("Replace"))
                        {
                            Apply(false, true);
                        }
                        if (GUILayout.Button("Remove"))
                        {
                            Remove(true);
                        }
                    }
                    GUI.enabled = true;
                }
                else
                {
                    EditorGUILayout.Space();
                    if (PCS == null)
                    {
                        GUI.enabled = false;
                        if (GUILayout.Button("Apply")) { }
                        if (GUILayout.Button("Remove")) { }
                        GUI.enabled = true;
                    }
                    else
                    {
                        GUI.enabled = false;
                        if (GUILayout.Button("Replace")) { }
                        GUI.enabled = true;
                        if (GUILayout.Button("Remove"))
                        {
                            Remove(true);
                        }
                    }
                }

                if (satisFeature == false)
                {
                    GUILayout.Label("   Memory Usage: " + paramCost[0], paramStyle);
                }
                else
                {
                    GUILayout.Label("   Memory Usage: " + paramCost[1], paramStyle);
                }
                if (TotalCost <= 256)
                {
                    GUILayout.Label("   Total Memory: <color=lime>" + TotalCost.ToString() + "</color>/256", paramStyle);
                }
                else
                {
                    GUILayout.Label("   Total Memory: <color=red>" + TotalCost.ToString() + "</color>/256", paramStyle);
                }

                if (targetAvatar.expressionParameters != null)
                {
                    if (satisFeature == false)
                    {
                        TotalCost = targetAvatar.expressionParameters.CalcTotalCost() + paramCost[0];
                    }
                    else
                    {
                        TotalCost = targetAvatar.expressionParameters.CalcTotalCost() + paramCost[1];
                    }
                }
                else
                {
                    TotalCost = paramCost[0];
                }
            }

            ShowFooter();
        }
        private void Apply(bool showDialog, bool isReplace)
        {
            if(isReplace == false)
            {
                Remove(false);
            }

            if (preset == Preset.Reference)
            {
                if (ref_mouth == null || ref_boobs == null || ref_pussy == null || ref_ass == null)
                {
                    EditorUtility.DisplayDialog("Penetration Contact System", "Please assign all reference positions first if you choose \"Reference\" preset.", "OK");
                }
                else
                {
                    Remove(false);
                    ApplySeconary(showDialog);
                }
            }
            else
            {
                Remove(false);
                ApplySeconary(showDialog);
            }
        }
        private void ApplySeconary(bool showDialog)
        {
            if (showDialog == true)
            {
                EditorUtility.DisplayDialog("Penetration Contact System", "PCS has been applied. Now you can make any further adjustment for each sound targets as usual.", "OK");

            }

            if (setupTool == SetupTool.ModularAvatar)
            {
                if (satisFeature == true)
                {
                    GameObject _ = PrefabUtility.InstantiatePrefab(Resources.Load<GameObject>(prefabPath[0]), targetAvatar.transform) as GameObject;
                    _.name = "Penetration Contact System";
                }
                else
                {
                    GameObject _ = PrefabUtility.InstantiatePrefab(Resources.Load<GameObject>(prefabPath[1]), targetAvatar.transform) as GameObject;
                    _.name = "Penetration Contact System";
                }
            }
            else
            {
                if (satisFeature == true)
                {
                    GameObject _ = PrefabUtility.InstantiatePrefab(Resources.Load<GameObject>(prefabPath[2]), targetAvatar.transform) as GameObject;
                    _.name = "Penetration Contact System";
                }
                else
                {
                    GameObject _ = PrefabUtility.InstantiatePrefab(Resources.Load<GameObject>(prefabPath[3]), targetAvatar.transform) as GameObject;
                    _.name = "Penetration Contact System";
                }
            }

            //if Satisfaction feature is check!
            var slime = GameObject.Find("$PCS - Slime");
            if (satisFeature == true)
            {
                slime.SetActive(false);
                mouthPosition = GameObject.Find("Penetration Contact System/<PCS - Mouth>/Mouth Position");
                if (moaning == Moaning.Anime)
                {
                    var voicePack = PrefabUtility.InstantiatePrefab(Resources.Load<GameObject>(voicePath[0]), mouthPosition.transform) as GameObject;
                    voicePack.name = "#PCS - Voice Pack";
                    MoanDurationCalculation(false);
                }
                else if (moaning == Moaning.Mature)
                {
                    var voicePack = PrefabUtility.InstantiatePrefab(Resources.Load<GameObject>(voicePath[1]), mouthPosition.transform) as GameObject;
                    voicePack.name = "#PCS - Voice Pack";
                    MoanDurationCalculation(false);
                }
                else if (moaning == Moaning.Custom)
                {
                    var voicePack = PrefabUtility.InstantiatePrefab(Resources.Load<GameObject>(voicePath[2]), mouthPosition.transform) as GameObject;
                    voicePack.name = "#PCS - Voice Pack";
                    MoanDurationCalculation(false);
                }
                else
                {
                    MoanDurationCalculation(true);
                }
                LustMultiplier();
            }

            var version = Resources.Load<TextAsset>("PCSversion").ToString();
            GameObject prefabVersion = GameObject.Find("----------------- v0.0.0 -------------------");
            prefabVersion.name = "----------------- v" + version + " -------------------";
            prefabVersion.SetActive(false);

            AutoAlignment((int)preset);
            PlaceCustomPosition();
            SetSmashSensitivity();

            GameObject PCSContact = GameObject.Find("PCS Contacts");
            PCSContact.SetActive(false);
        }
        private void Remove(bool showDialog)
        {
            GameObject targetPCS = GameObject.Find("Penetration Contact System");
            GameObject targetPCSRemove = GameObject.Find("<PCS Custom Target>");

            if (showDialog == true)
            {
                if (EditorUtility.DisplayDialog("Penetration Contact System", "Are you sure you want to remove PCS?", "Yes", "No"))
                {
                    DestroyImmediate(targetPCS);
                    DestroyImmediate(targetPCSRemove);

                    VRCExpressionParameters.Parameter[] parameterArray = targetAvatar.expressionParameters.parameters;
                    parameterArray = parameterArray.Where(x => !x.name.StartsWith("pcs/")).ToArray();
                    targetAvatar.expressionParameters.parameters = parameterArray;
                    EditorUtility.SetDirty(targetAvatar.expressionParameters);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
            else
            {
                DestroyImmediate(targetPCS);
                DestroyImmediate(targetPCSRemove);

                VRCExpressionParameters.Parameter[] parameterArray = targetAvatar.expressionParameters.parameters;
                parameterArray = parameterArray.Where(x => !x.name.StartsWith("pcs/")).ToArray();
                targetAvatar.expressionParameters.parameters = parameterArray;
                EditorUtility.SetDirty(targetAvatar.expressionParameters);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

        }
        private void PlaceCustomPosition()
        {
            GameObject PCSContact = GameObject.Find("PCS Contacts");
            GameObject CustomTargetGuide = GameObject.Find("pcs-guide-custom");
            if (customPosition == null)
            {
                CustomTargetGuide.SetActive(false);
            }
            else
            {
                GameObject targetPCSRemove = GameObject.Find("<PCS Custom Target>");
                DestroyImmediate(targetPCSRemove);

                GameObject _ = PrefabUtility.InstantiatePrefab(Resources.Load<GameObject>("PCS Custom Target"), customPosition.transform) as GameObject;
                _.name = "<PCS Custom Target>";

                ParentConstraint parentConstraint = PCSContact.GetComponent<ParentConstraint>();
                constraintSource.sourceTransform = _.transform;
                parentConstraint.SetSource(4, constraintSource);

                ParentConstraint parentConstraintGuide = CustomTargetGuide.GetComponent<ParentConstraint>();
                guideSource.sourceTransform = _.transform;
                guideSource.weight = 1;
                parentConstraintGuide.SetSource(0, guideSource);

            }
        }
        private void AutoAlignment(int preset)
        {
            mouth = GameObject.Find("Penetration Contact System/<PCS - Mouth>");
            boobs = GameObject.Find("Penetration Contact System/<PCS - Boobs>");
            pussy = GameObject.Find("Penetration Contact System/<PCS - Pussy>");
            ass = GameObject.Find("Penetration Contact System/<PCS - Ass>");

            if (preset != (int)Preset.Reference)
            {
                switch (preset)
                {
                    case (int)Preset.Generic:
                        mouth.transform.localPosition = new Vector3(0, 1.2f, 0.15f);
                        mouth.transform.eulerAngles = new Vector3(20, 0, 0);
                        boobs.transform.localPosition = new Vector3(0, 1.05f, 0.1f);
                        boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                        pussy.transform.localPosition = new Vector3(0, 0.65f, 0.018f);
                        pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                        ass.transform.localPosition = new Vector3(0, 0.65f, -0.05f);
                        ass.transform.eulerAngles = new Vector3(100, 0, 0);
                        break;

                    case (int)Preset.Uzuki:
                        mouth.transform.localPosition = new Vector3(0, 1.13f, 0.055f);
                        mouth.transform.eulerAngles = new Vector3(23, 0, 0);
                        boobs.transform.localPosition = new Vector3(0, 0.975f, 0.1035f);
                        boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                        pussy.transform.localPosition = new Vector3(0, 0.702f, 0.035f);
                        pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                        ass.transform.localPosition = new Vector3(0, 0.705f, -0.006f);
                        ass.transform.eulerAngles = new Vector3(100, 0, 0);
                        break;

                    case (int)Preset.Aria:
                        mouth.transform.localPosition = new Vector3(0, 1.105f, 0.093f);
                        mouth.transform.eulerAngles = new Vector3(23, 0, 0);
                        boobs.transform.localPosition = new Vector3(0, 0.945f, 0.12f);
                        boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                        pussy.transform.localPosition = new Vector3(0, 0.65f, 0.045f);
                        pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                        ass.transform.localPosition = new Vector3(0, 0.66f, -0.009f);
                        ass.transform.eulerAngles = new Vector3(100, 0, 0);
                        break;

                    case (int)Preset.Soraha:
                        mouth.transform.localPosition = new Vector3(0, 0.978f, 0.087f);
                        mouth.transform.eulerAngles = new Vector3(28, 0, 0);
                        boobs.transform.localPosition = new Vector3(0, 0.83f, 0.09f);
                        boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                        pussy.transform.localPosition = new Vector3(0, 0.58f, 0.03f);
                        pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                        ass.transform.localPosition = new Vector3(0, 0.585f, -0.015f);
                        ass.transform.eulerAngles = new Vector3(100, 0, 0);
                        break;

                    case (int)Preset.Kikyo:
                        mouth.transform.localPosition = new Vector3(0, 1.187f, 0.0745f);
                        mouth.transform.eulerAngles = new Vector3(23, 0, 0);
                        boobs.transform.localPosition = new Vector3(0, 1.02f, 0.08f);
                        boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                        pussy.transform.localPosition = new Vector3(0, 0.6825f, 0.0185f);
                        pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                        ass.transform.localPosition = new Vector3(0, 0.69f, -0.03f);
                        ass.transform.eulerAngles = new Vector3(100, 0, 0);
                        break;

                    case (int)Preset.Maya:
                        mouth.transform.localPosition = new Vector3(0, 1.119f, 0.11f);
                        mouth.transform.eulerAngles = new Vector3(25, 0, 0);
                        boobs.transform.localPosition = new Vector3(0, 0.95f, 0.12f);
                        boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                        pussy.transform.localPosition = new Vector3(0, 0.653f, 0.035f);
                        pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                        ass.transform.localPosition = new Vector3(0, 0.66f, 0.002f);
                        ass.transform.eulerAngles = new Vector3(100, 0, 0);
                        break;

                    case (int)Preset.Rindo:
                        mouth.transform.localPosition = new Vector3(0, 1.1265f, 0.076f);
                        mouth.transform.eulerAngles = new Vector3(25, 0, 0);
                        boobs.transform.localPosition = new Vector3(0, 0.97f, 0.075f);
                        boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                        pussy.transform.localPosition = new Vector3(0, 0.6685f, 0.036f);
                        pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                        ass.transform.localPosition = new Vector3(0, 0.678f, -0.015f);
                        ass.transform.eulerAngles = new Vector3(100, 0, 0);
                        break;

                    case (int)Preset.Selestia:
                        mouth.transform.localPosition = new Vector3(0, 1.124f, 0.078f);
                        mouth.transform.eulerAngles = new Vector3(25, 0, 0);
                        boobs.transform.localPosition = new Vector3(0, 0.96f, 0.1f);
                        boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                        pussy.transform.localPosition = new Vector3(0, 0.67f, 0.03f);
                        pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                        ass.transform.localPosition = new Vector3(0, 0.68f, -0.02f);
                        ass.transform.eulerAngles = new Vector3(100, 0, 0);
                        break;

                    case (int)Preset.UltimateKissMa:
                        mouth.transform.localPosition = new Vector3(0, 1.115f, 0.065f);
                        mouth.transform.eulerAngles = new Vector3(25, 0, 0);
                        boobs.transform.localPosition = new Vector3(0, 0.95f, 0.098f);
                        boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                        pussy.transform.localPosition = new Vector3(0, 0.644f, 0.017f);
                        pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                        ass.transform.localPosition = new Vector3(0, 0.645f, -0.03f);
                        ass.transform.eulerAngles = new Vector3(100, 0, 0);
                        break;

                    case (int)Preset.Fiona:
                        mouth.transform.localPosition = new Vector3(0, 1.1275f, 0.074f);
                        mouth.transform.eulerAngles = new Vector3(28, 0, 0);
                        boobs.transform.localPosition = new Vector3(0, 0.97f, 0.062f);
                        boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                        pussy.transform.localPosition = new Vector3(0, 0.701f, 0.0135f);
                        pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                        ass.transform.localPosition = new Vector3(0, 0.705f, -0.02f);
                        ass.transform.eulerAngles = new Vector3(100, 0, 0);
                        break;

                    case (int)Preset.Moe:
                        mouth.transform.localPosition = new Vector3(0, 1.219f, 0.088f);
                        mouth.transform.eulerAngles = new Vector3(23, 0, 0);
                        boobs.transform.localPosition = new Vector3(0, 1.038f, 0.125f);
                        boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                        pussy.transform.localPosition = new Vector3(0, 0.717f, 0.03f);
                        pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                        ass.transform.localPosition = new Vector3(0, 0.723f, -0.02f);
                        ass.transform.eulerAngles = new Vector3(100, 0, 0);
                        break;

                    case (int)Preset.Karin:
                        mouth.transform.localPosition = new Vector3(0, 1.061f, 0.052f);
                        mouth.transform.eulerAngles = new Vector3(28, 0, 0);
                        boobs.transform.localPosition = new Vector3(0, 0.905f, 0.063f);
                        boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                        pussy.transform.localPosition = new Vector3(0, 0.607f, 0.0115f);
                        pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                        ass.transform.localPosition = new Vector3(0, 0.612f, -0.027f);
                        ass.transform.eulerAngles = new Vector3(100, 0, 0);
                        break;

                    case (int)Preset.Lime:
                        mouth.transform.localPosition = new Vector3(0, 1.1205f, 0.039f);
                        mouth.transform.eulerAngles = new Vector3(28, 0, 0);
                        boobs.transform.localPosition = new Vector3(0, 0.955f, 0.0555f);
                        boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                        pussy.transform.localPosition = new Vector3(0, 0.652f, -0.0045f);
                        pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                        ass.transform.localPosition = new Vector3(0, 0.66f, -0.0475f);
                        ass.transform.eulerAngles = new Vector3(100, 0, 0);
                        break;

                    case (int)Preset.Anon:
                        mouth.transform.localPosition = new Vector3(0, 1.13f, 0.078f);
                        mouth.transform.eulerAngles = new Vector3(30, 0, 0);
                        boobs.transform.localPosition = new Vector3(0, 0.965f, 0.093f);
                        boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                        pussy.transform.localPosition = new Vector3(0, 0.663f, 0.035f);
                        pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                        ass.transform.localPosition = new Vector3(0, 0.665f, -0.02f);
                        ass.transform.eulerAngles = new Vector3(100, 0, 0);
                        break;

                    case (int)Preset.Leefa:
                        mouth.transform.localPosition = new Vector3(0, 1.104f, 0.0755f);
                        mouth.transform.eulerAngles = new Vector3(25, 0, 0);
                        boobs.transform.localPosition = new Vector3(0, 0.95f, 0.083f);
                        boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                        pussy.transform.localPosition = new Vector3(0, 0.654f, 0.0215f);
                        pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                        ass.transform.localPosition = new Vector3(0, 0.656f, -0.024f);
                        ass.transform.eulerAngles = new Vector3(100, 0, 0);
                        break;

                    case (int)Preset.Imeris:
                        mouth.transform.localPosition = new Vector3(0, 1.22f, 0.0655f);
                        mouth.transform.eulerAngles = new Vector3(28, 0, 0);
                        boobs.transform.localPosition = new Vector3(0, 1.02f, 0.13f);
                        boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                        pussy.transform.localPosition = new Vector3(0, 0.699f, 0.0105f);
                        pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                        ass.transform.localPosition = new Vector3(0, 0.705f, -0.034f);
                        ass.transform.eulerAngles = new Vector3(100, 0, 0);
                        break;

                    case (int)Preset.Manuka:
                        mouth.transform.localPosition = new Vector3(0, 1.092f, 0.072f);
                        mouth.transform.eulerAngles = new Vector3(30, 0, 0);
                        boobs.transform.localPosition = new Vector3(0, 0.945f, 0.1f);
                        boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                        pussy.transform.localPosition = new Vector3(0, 0.672f, 0.0225f);
                        pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                        ass.transform.localPosition = new Vector3(0, 0.68f, -0.014f);
                        ass.transform.eulerAngles = new Vector3(100, 0, 0);
                        break;

                    case (int)Preset.Body_Panda:
                        mouth.transform.localPosition = new Vector3(0, 1.15f, 0.15f);
                        mouth.transform.eulerAngles = new Vector3(20, 0, 0);
                        boobs.transform.localPosition = new Vector3(0, 0.98f, 0.12f);
                        boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                        pussy.transform.localPosition = new Vector3(0, 0.652f, 0.038f);
                        pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                        ass.transform.localPosition = new Vector3(0, 0.6525f, -0.0105f);
                        ass.transform.eulerAngles = new Vector3(100, 0, 0);
                        break;

                    case (int)Preset.Body_ZinRP:
                        mouth.transform.localPosition = new Vector3(0, 1.15f, 0.15f);
                        mouth.transform.eulerAngles = new Vector3(20, 0, 0);
                        boobs.transform.localPosition = new Vector3(0, 0.95f, 0.07f);
                        boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                        pussy.transform.localPosition = new Vector3(0, 0.65f, 0.005f);
                        pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                        ass.transform.localPosition = new Vector3(0, 0.66f, -0.05f);
                        ass.transform.eulerAngles = new Vector3(100, 0, 0);
                        break;

                    case (int)Preset.Body_ZinFit:
                        mouth.transform.localPosition = new Vector3(0, 1.15f, 0.15f);
                        mouth.transform.eulerAngles = new Vector3(20, 0, 0);
                        boobs.transform.localPosition = new Vector3(0, 0.95f, 0.068f);
                        boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                        pussy.transform.localPosition = new Vector3(0, 0.636f, 0.005f);
                        pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                        ass.transform.localPosition = new Vector3(0, 0.637f, -0.045f);
                        ass.transform.eulerAngles = new Vector3(100, 0, 0);
                        break;

                    case (int)Preset.Body_LexzBase:
                        mouth.transform.localPosition = new Vector3(0, 1.15f, 0.15f);
                        mouth.transform.eulerAngles = new Vector3(20, 0, 0);
                        boobs.transform.localPosition = new Vector3(0, 1.01f, 0.145f);
                        boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                        pussy.transform.localPosition = new Vector3(0, 0.659f, 0.048f);
                        pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                        ass.transform.localPosition = new Vector3(0, 0.663f, 0.0145f);
                        ass.transform.eulerAngles = new Vector3(100, 0, 0);
                        break;

                    case (int)Preset.Body_ToriBase:
                        mouth.transform.localPosition = new Vector3(0, 1.3f, 0.15f);
                        mouth.transform.eulerAngles = new Vector3(25, 0, 0);
                        boobs.transform.localPosition = new Vector3(0, 1.075f, 0.13f);
                        boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                        pussy.transform.localPosition = new Vector3(0, 0.733f, 0.0395f);
                        pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                        ass.transform.localPosition = new Vector3(0, 0.74f, 0);
                        ass.transform.eulerAngles = new Vector3(100, 0, 0);
                        break;

                    case (int)Preset.Body_TVF:
                        mouth.transform.localPosition = new Vector3(0, 1.15f, 0.15f);
                        mouth.transform.eulerAngles = new Vector3(20, 0, 0);
                        boobs.transform.localPosition = new Vector3(0, 1, 0.126f);
                        boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                        pussy.transform.localPosition = new Vector3(0, 0.662f, 0.032f);
                        pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                        ass.transform.localPosition = new Vector3(0, 0.672f, -0.021f);
                        ass.transform.eulerAngles = new Vector3(110, 0, 0);
                        break;

                    case (int)Preset.Velle:
                        mouth.transform.localPosition = new Vector3(0, 1.194f, 0.0835f);
                        mouth.transform.eulerAngles = new Vector3(30, 0, 0);
                        boobs.transform.localPosition = new Vector3(0, 1.02f, 0.11f);
                        boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                        pussy.transform.localPosition = new Vector3(0, 0.698f, 0.025f);
                        pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                        ass.transform.localPosition = new Vector3(0, 0.712f, -0.018f);
                        ass.transform.eulerAngles = new Vector3(100, 0, 0);
                        break;

                    case (int)Preset.Kuronatu:
                        mouth.transform.localPosition = new Vector3(0, 1.32f, 0.075f);
                        mouth.transform.eulerAngles = new Vector3(25, 0, 0);
                        boobs.transform.localPosition = new Vector3(0, 1.0815f, 0.11f);
                        boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                        pussy.transform.localPosition = new Vector3(0, 0.74f, -0.004f);
                        pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                        ass.transform.localPosition = new Vector3(0, 0.74f, -0.045f);
                        ass.transform.eulerAngles = new Vector3(100, 0, 0);
                        break;

                    case (int)Preset.Shinra:
                        mouth.transform.localPosition = new Vector3(0, 1.295f, 0.07f);
                        mouth.transform.eulerAngles = new Vector3(20, 0, 0);
                        boobs.transform.localPosition = new Vector3(0, 1.09f, 0.1f);
                        boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                        pussy.transform.localPosition = new Vector3(0, 0.7495f, 0);
                        pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                        ass.transform.localPosition = new Vector3(0, 0.756f, -0.05f);
                        ass.transform.eulerAngles = new Vector3(100, 0, 0);
                        break;

                    case (int)Preset.Wolferia:
                        mouth.transform.localPosition = new Vector3(0, 1.2f, 0.083f);
                        mouth.transform.eulerAngles = new Vector3(25, 0, 0);
                        boobs.transform.localPosition = new Vector3(0, 1.015f, 0.115f);
                        boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                        pussy.transform.localPosition = new Vector3(0, 0.693f, 0.02f);
                        pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                        ass.transform.localPosition = new Vector3(0, 0.691f, -0.025f);
                        ass.transform.eulerAngles = new Vector3(100, 0, 0);
                        break;

                    case (int)Preset.Kokoa:
                        mouth.transform.localPosition = new Vector3(0, 1.191f, 0.115f);
                        mouth.transform.eulerAngles = new Vector3(25, 0, 0);
                        boobs.transform.localPosition = new Vector3(0, 1, 0.145f);
                        boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                        pussy.transform.localPosition = new Vector3(0, 0.676f, 0.033f);
                        pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                        ass.transform.localPosition = new Vector3(0, 0.688f, -0.01f);
                        ass.transform.eulerAngles = new Vector3(100, 0, 0);
                        break;

                    case (int)Preset.Eyo:
                        mouth.transform.localPosition = new Vector3(0, 1.17f, 0.07f);
                        mouth.transform.eulerAngles = new Vector3(25, 0, 0);
                        boobs.transform.localPosition = new Vector3(0, 0.99f, 0.11f);
                        boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                        pussy.transform.localPosition = new Vector3(0, 0.673f, 0.013f);
                        pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                        ass.transform.localPosition = new Vector3(0, 0.68f, -0.04f);
                        ass.transform.eulerAngles = new Vector3(100, 0, 0);
                        break;

                    case (int)Preset.Tien:
                        mouth.transform.localPosition = new Vector3(0, 1.019f, 0.0725f);
                        mouth.transform.eulerAngles = new Vector3(30, 0, 0);
                        boobs.transform.localPosition = new Vector3(0, 0.88f, 0.08f);
                        boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                        pussy.transform.localPosition = new Vector3(0, 0.598f, 0.013f);
                        pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                        ass.transform.localPosition = new Vector3(0, 0.598f, -0.02f);
                        ass.transform.eulerAngles = new Vector3(100, 0, 0);
                        break;

                    case (int)Preset.Mophira:
                        mouth.transform.localPosition = new Vector3(0, 1.218f, 0.11f);
                        mouth.transform.eulerAngles = new Vector3(25, 0, 0);
                        boobs.transform.localPosition = new Vector3(0, 1.03f, 0.14f);
                        boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                        pussy.transform.localPosition = new Vector3(0, 0.717f, 0.055f);
                        pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                        ass.transform.localPosition = new Vector3(0, 0.745f, 0.005f);
                        ass.transform.eulerAngles = new Vector3(100, 0, 0);
                        break;

                    case (int)Preset.Runa_Robotic:
                        mouth.transform.localPosition = new Vector3(0, 1.188f, 0.05f);
                        mouth.transform.eulerAngles = new Vector3(30, 0, 0);
                        boobs.transform.localPosition = new Vector3(0, 1.035f, 0.075f);
                        boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                        pussy.transform.localPosition = new Vector3(0, 0.685f, 0.005f);
                        pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                        ass.transform.localPosition = new Vector3(0, 0.685f, -0.05f);
                        ass.transform.eulerAngles = new Vector3(100, 0, 0);
                        break;
                }

            }
            else
            {
                if (ref_mouth != null && ref_boobs != null && ref_pussy != null && ref_ass != null)
                {
                    mouth.transform.localPosition = ref_mouth.transform.position;
                    mouth.transform.eulerAngles = ref_mouth.transform.rotation.eulerAngles;
                    boobs.transform.localPosition = ref_boobs.transform.position;
                    boobs.transform.eulerAngles = ref_boobs.transform.rotation.eulerAngles;
                    pussy.transform.localPosition = ref_pussy.transform.position;
                    pussy.transform.eulerAngles = ref_pussy.transform.rotation.eulerAngles;
                    ass.transform.localPosition = ref_ass.transform.position;
                    ass.transform.eulerAngles = ref_ass.transform.rotation.eulerAngles;
                }
            }
        }
        private void LustMultiplier()
        {
            VRCExpressionParameters parametersOriginal = (VRCExpressionParameters)targetAvatar.expressionParameters; //Get Parameters
            List<VRCExpressionParameters.Parameter> addparams = new List<VRCExpressionParameters.Parameter>(); //Make new empty list of parameters

            VRCExpressionParameters.Parameter multiply_Param = new VRCExpressionParameters.Parameter //Crate new parameters to add
            {
                name = "pcs/lustMultiplier",
                valueType = VRCExpressionParameters.ValueType.Float,
                defaultValue = (0.12f + lustMultiplierValue),
                networkSynced = false,
                saved = false
            };

            addparams.Add(multiply_Param);
            EditorUtility.SetDirty(targetAvatar.expressionParameters);
            targetAvatar.expressionParameters.parameters = parametersOriginal.parameters.Concat(addparams.ToArray()).ToArray();
        }
        private void SetSmashSensitivity()
        {
            var sensitivityObj = GameObject.Find("#PCS - Smash Sensitivity");
            sensitivityObj.transform.localPosition = new Vector3(0, 0, 0.025f-smashSensitivity);
        }
        private void MoanDurationCalculation(bool isMoanDisable)
        {
            VRCExpressionParameters parametersOriginal = (VRCExpressionParameters)targetAvatar.expressionParameters;
            List<VRCExpressionParameters.Parameter> addparams_EM = new List<VRCExpressionParameters.Parameter>();
            List<VRCExpressionParameters.Parameter> addparams_SM = new List<VRCExpressionParameters.Parameter>();
            List<VRCExpressionParameters.Parameter> addparams_RM = new List<VRCExpressionParameters.Parameter>();
            var eventMoan = GameObject.Find("#PCS - Event Moans");
            var randomMoan = GameObject.Find("#PCS - Random Moans");

            if(isMoanDisable == false)
            {
                //For Event Moans (25 sounds)
                for (int i = 0; i < 25; i++)
                {
                    //Find each sound elements inside Event Moan. Like event1, event2, event3,...
                    var em = eventMoan.transform.Find("event" + (i + 1));

                    AudioSource[] emAudio = new AudioSource[26];
                    emAudio[i + 1] = em.GetComponent<AudioSource>();

                    float tempDuration;
                    if (emAudio[i + 1].clip.length != 0)
                    {
                        tempDuration = 1 / (emAudio[i + 1].clip.length);
                    }
                    else
                    {
                        tempDuration = 1;
                    }

                    VRCExpressionParameters.Parameter eventMoan_Param = new VRCExpressionParameters.Parameter
                    {
                        name = "pcs/clipSpeed/em" + (i + 1),
                        valueType = VRCExpressionParameters.ValueType.Float,
                        defaultValue = (tempDuration), //do match here Dismay; Convert audio duration to state speed.
                        networkSynced = false,
                        saved = false
                    };
                    addparams_EM.Add(eventMoan_Param);
                }

                //For Soft Random Moans (10 sounds)
                for (int i = 0; i < 10; i++)
                {
                    //Find each sound elements inside Event Moan. Like event1, event2, event3,...
                    var sm = randomMoan.transform.Find("soft" + (i + 1));

                    AudioSource[] smAudio = new AudioSource[11];
                    smAudio[i + 1] = sm.GetComponent<AudioSource>();

                    float tempDuration;
                    if (smAudio[i + 1].clip.length != 0)
                    {
                        tempDuration = 0.17f / (smAudio[i + 1].clip.length);
                    }
                    else
                    {
                        tempDuration = 1;
                    }

                    VRCExpressionParameters.Parameter softMoan_Param = new VRCExpressionParameters.Parameter
                    {
                        name = "pcs/clipSpeed/sm" + (i + 1),
                        valueType = VRCExpressionParameters.ValueType.Float,
                        defaultValue = (tempDuration), //do match here Dismay; Convert audio duration to state speed.
                        networkSynced = false,
                        saved = false
                    };
                    addparams_SM.Add(softMoan_Param);
                }

                //For Soft Rough Moans (10 sounds)
                for (int i = 0; i < 10; i++)
                {
                    //Find each sound elements inside Event Moan. Like event1, event2, event3,...
                    var rm = randomMoan.transform.Find("rough" + (i + 1));

                    AudioSource[] rmAudio = new AudioSource[11];
                    rmAudio[i + 1] = rm.GetComponent<AudioSource>();

                    float tempDuration;
                    if (rmAudio[i + 1].clip.length != 0)
                    {
                        tempDuration = 0.17f / (rmAudio[i + 1].clip.length);
                    }
                    else
                    {
                        tempDuration = 1;
                    }

                    VRCExpressionParameters.Parameter roughtMoan_Param = new VRCExpressionParameters.Parameter
                    {
                        name = "pcs/clipSpeed/rm" + (i + 1),
                        valueType = VRCExpressionParameters.ValueType.Float,
                        defaultValue = (tempDuration),
                        networkSynced = false,
                        saved = false
                    };
                    addparams_RM.Add(roughtMoan_Param);
                }

                EditorUtility.SetDirty(targetAvatar.expressionParameters);
                targetAvatar.expressionParameters.parameters = parametersOriginal.parameters.Concat(addparams_EM.ToArray()).ToArray();
                targetAvatar.expressionParameters.parameters = parametersOriginal.parameters.Concat(addparams_SM.ToArray()).ToArray();
                targetAvatar.expressionParameters.parameters = parametersOriginal.parameters.Concat(addparams_RM.ToArray()).ToArray();

            }
            else
            {
                //For Event Moans (25 sounds)
                for (int i = 0; i < 25; i++)
                {
                    VRCExpressionParameters.Parameter eventMoan_Param = new VRCExpressionParameters.Parameter
                    {
                        name = "pcs/clipSpeed/em" + (i + 1),
                        valueType = VRCExpressionParameters.ValueType.Float,
                        defaultValue = (1), //do match here Dismay; Convert audio duration to state speed.
                        networkSynced = false,
                        saved = false
                    };
                    addparams_EM.Add(eventMoan_Param);
                }

                //For Soft Random Moans (10 sounds)
                for (int i = 0; i < 10; i++)
                {
                    VRCExpressionParameters.Parameter softMoan_Param = new VRCExpressionParameters.Parameter
                    {
                        name = "pcs/clipSpeed/sm" + (i + 1),
                        valueType = VRCExpressionParameters.ValueType.Float,
                        defaultValue = (1), //do match here Dismay; Convert audio duration to state speed.
                        networkSynced = false,
                        saved = false
                    };
                    addparams_SM.Add(softMoan_Param);
                }

                //For Soft Rough Moans (10 sounds)
                for (int i = 0; i < 10; i++)
                {
                    VRCExpressionParameters.Parameter roughtMoan_Param = new VRCExpressionParameters.Parameter
                    {
                        name = "pcs/clipSpeed/rm" + (i + 1),
                        valueType = VRCExpressionParameters.ValueType.Float,
                        defaultValue = (1),
                        networkSynced = false,
                        saved = false
                    };
                    addparams_RM.Add(roughtMoan_Param);
                }

                EditorUtility.SetDirty(targetAvatar.expressionParameters);
                targetAvatar.expressionParameters.parameters = parametersOriginal.parameters.Concat(addparams_EM.ToArray()).ToArray();
                targetAvatar.expressionParameters.parameters = parametersOriginal.parameters.Concat(addparams_SM.ToArray()).ToArray();
                targetAvatar.expressionParameters.parameters = parametersOriginal.parameters.Concat(addparams_RM.ToArray()).ToArray();
            }

        }
        private void ShowFooter()
        {
            EditorGUILayout.Space();

            if (!targetAvatar)
            {
                EditorGUILayout.HelpBox("Please drag and drop your avatar into the box.", MessageType.Warning);
            }

            if (TotalCost > 256)
            {
                EditorGUILayout.HelpBox("It seems that you do not have enough parameters to install PCS." +
                    " Please note that this does not include parameters from non-destructive tools as they will be added during upload.", MessageType.Warning);
            }
            else
            {
                /*EditorGUILayout.HelpBox("PCS requires \"Modular Avatar\" or \"VRCFury\" to be installed." +
                    "\nThis tool just makes it easier to align and install voice packs.", MessageType.Info);*/
                EditorGUILayout.HelpBox("PCS requires \"Modular Avatar\" or \"VRCFury\" to be installed." +
                    "\nPlease note that it's recommended to use the newest version.", MessageType.Info);
            }

            GUILayout.FlexibleSpace();
            var info = Resources.Load<TextAsset>("PCSinfo").ToString();
            var version = Resources.Load<TextAsset>("PCSversion").ToString();
            GUILayout.Label(info.Replace("$", "v" + version), infoStyle);
            EditorGUILayout.Space();
        }
    }
}

