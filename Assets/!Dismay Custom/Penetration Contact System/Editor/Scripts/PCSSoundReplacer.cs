using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UIElements;
using VRC.SDK3.Avatars.Components;

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
    public class PCSSoundReplacer : EditorWindow
    {
        private static UnityEditor.Animations.AnimatorController animator;
        private Vector2 scrollPosition = new(0, 300);
        private Texture2D logo;
        private AnimatorState animatorCheck1, animatorCheck2;

        [MenuItem("Tools/Dismay Custom/Penetration Contact System/Replace SFX")]
        public static void ShowpWindow()
        {
            var window = GetWindow(typeof(PCSSoundReplacer));

            window.titleContent = new GUIContent("PCS: Replace Audio");
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

        #region parameters
        //Mouth
        private bool mouth_fold;

        private static bool mouth_in, mouth_in_hide;
        private int mouth_in_select = 1;
        private static AudioClip[] mouth_in_clips = new AudioClip[10];

        private static bool mouth_out, mouth_out_hide;
        private int mouth_out_select = 1;
        private static AudioClip[] mouth_out_clips = new AudioClip[10];

        private static bool mouth_sm, mouth_sm_hide;
        private int mouth_sm_select = 1;
        private static AudioClip[] mouth_sm_clips = new AudioClip[10];

        private static bool mouth_ex, mouth_ex_hide;
        private int mouth_ex_select = 1;
        private static AudioClip[] mouth_ex_clips = new AudioClip[10];

        //boobs
        private bool boobs_fold;

        private static bool boobs_in, boobs_in_hide;
        private int boobs_in_select = 1;
        private static AudioClip[] boobs_in_clips = new AudioClip[10];

        private static bool boobs_out, boobs_out_hide;
        private int boobs_out_select = 1;
        private static AudioClip[] boobs_out_clips = new AudioClip[10];

        private static bool boobs_smash, boobs_smash_hide;
        private int boobs_smash_select = 1;
        private static AudioClip[] boobs_smash_clips = new AudioClip[10];

        //pussy
        private bool pussy_fold;

        private static bool pussy_in, pussy_in_hide;
        private int pussy_in_select = 1;
        private static AudioClip[] pussy_in_clips = new AudioClip[10];

        private static bool pussy_out, pussy_out_hide;
        private int pussy_out_select = 1;
        private static AudioClip[] pussy_out_clips = new AudioClip[10];

        private static bool pussy_exit, pussy_exit_hide;
        private int pussy_exit_select = 1;
        private static AudioClip[] pussy_exit_clips = new AudioClip[10];

        //ass
        private bool ass_fold;

        private static bool ass_in, ass_in_hide;
        private int ass_in_select = 1;
        private static AudioClip[] ass_in_clips = new AudioClip[10];

        private static bool ass_out, ass_out_hide;
        private int ass_out_select = 1;
        private static AudioClip[] ass_out_clips = new AudioClip[10];

        private static bool ass_exit, ass_exit_hide;
        private int ass_exit_select = 1;
        private static AudioClip[] ass_exit_clips = new AudioClip[10];

        //generic
        private bool generic_fold;

        private static bool generic_smash_1, generic_smash_1_hide;
        private int generic_smash_1_select = 1;
        private static AudioClip[] generic_smash_1_clips = new AudioClip[10];

        private static bool generic_smash_2, generic_smash_2_hide;
        private int generic_smash_2_select = 1;
        private static AudioClip[] generic_smash_2_clips = new AudioClip[10];

        private static bool generic_smash_3, generic_smash_3_hide;
        private int generic_smash_3_select = 1;
        private static AudioClip[] generic_smash_3_clips = new AudioClip[10];

        private static bool generic_smash_4, generic_smash_4_hide;
        private int generic_smash_4_select = 1;
        private static AudioClip[] generic_smash_4_clips = new AudioClip[10];
        #endregion

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            logo = Resources.Load<Texture2D>("Components/" + "PCS_ChangeSFX" + "_banner");
            GUILayout.Label(logo, new GUIStyle { fixedWidth = 512, fixedHeight = 115 });
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();            

            EditorGUI.BeginChangeCheck();
            animator = EditorGUILayout.ObjectField(animator, typeof(UnityEditor.Animations.AnimatorController), true, GUILayout.Height(30)) as UnityEditor.Animations.AnimatorController;
            if (EditorGUI.EndChangeCheck())
            {
                animatorCheck1 = FindAnimatorStateByName("Mouth - custom 1", animator);
                animatorCheck2 = FindAnimatorStateByName("state_mouth_allExit", animator);
            }

            if (animatorCheck1 != null && animatorCheck2 != null)
            {
                EditorGUILayout.HelpBox("Choose any sound types you want to replace and drop your Audio Clip(s) into the field. Uncheck the box will force PCS to use the original sound effects.", MessageType.Info);

                scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);
                ShowMenu();           
                GUILayout.EndScrollView();

                GUILayout.Space(5);

                if (GUILayout.Button("Apply New Sound Effects"))
                {
                    ApplySFX(true);
                }
                GUILayout.FlexibleSpace();
            }
            else
            {
                if(animator != null)
                {
                    EditorGUILayout.HelpBox("Audio Controller not found! You might put the wrong animator controller. Please make sure you drop \"PCS Controller_[your avatar name]\" into the box.", MessageType.Warning);
                }
                else
                {
                    EditorGUILayout.HelpBox("Please drop your PCS Controller into the box. It can be found in #GENERATE folder.", MessageType.Info);
                }
                GUILayout.FlexibleSpace();
            }
            if(animator == null)
            {
                animatorCheck1 = null;
                animatorCheck2 = null;
            }
        }
        private void ShowMenu()
        {          
            mouth_fold = EditorGUILayout.Foldout(mouth_fold, "Mouth");
            
            if (mouth_fold)
            {
                #region mouth in
                EditorGUILayout.BeginHorizontal();
                EditorGUI.indentLevel++;
                mouth_in = EditorGUILayout.Toggle("Mouth Inward", mouth_in);
                EditorGUI.indentLevel--;

                if (!mouth_in)
                {
                    GUI.enabled = false;
                    mouth_in_hide = false;
                }

                EditorGUILayout.LabelField("Amount", GUILayout.MaxWidth(50));
                mouth_in_select = EditorGUILayout.IntSlider(mouth_in_select, 1, 10, GUILayout.MinWidth(160));

                EditorGUILayout.LabelField("  Hide Tab", GUILayout.MaxWidth(80));
                mouth_in_hide = EditorGUILayout.Toggle(mouth_in_hide);

                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();
                CreateAudioSlot(mouth_in, mouth_in_hide, mouth_in_select, mouth_in_clips, "mouth_in");
                #endregion

                #region mouth out
                EditorGUILayout.BeginHorizontal();
                EditorGUI.indentLevel++;
                mouth_out = EditorGUILayout.Toggle("Mouth Outward", mouth_out);
                EditorGUI.indentLevel--;

                if (!mouth_out)
                {
                    GUI.enabled = false;
                    mouth_out_hide = false;
                }

                EditorGUILayout.LabelField("Amount", GUILayout.MaxWidth(50));
                mouth_out_select = EditorGUILayout.IntSlider(mouth_out_select, 1, 10, GUILayout.MinWidth(160));

                EditorGUILayout.LabelField("  Hide Tab", GUILayout.MaxWidth(80));
                mouth_out_hide = EditorGUILayout.Toggle(mouth_out_hide);

                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();
                CreateAudioSlot(mouth_out, mouth_out_hide, mouth_out_select, mouth_out_clips, "mouth_out");
                #endregion

                #region mouth smash
                EditorGUILayout.BeginHorizontal();
                EditorGUI.indentLevel++;
                mouth_sm = EditorGUILayout.Toggle("Mouth Smash", mouth_sm);
                EditorGUI.indentLevel--;

                if (!mouth_sm)
                {
                    GUI.enabled = false;
                    mouth_sm_hide = false;
                }

                EditorGUILayout.LabelField("Amount", GUILayout.MaxWidth(50));
                mouth_sm_select = EditorGUILayout.IntSlider(mouth_sm_select, 1, 10, GUILayout.MinWidth(160));

                EditorGUILayout.LabelField("  Hide Tab", GUILayout.MaxWidth(80));
                mouth_sm_hide = EditorGUILayout.Toggle(mouth_sm_hide);

                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();
                CreateAudioSlot(mouth_sm, mouth_sm_hide, mouth_sm_select, mouth_sm_clips, "mouth_smash");
                #endregion

                #region mouth exit
                EditorGUILayout.BeginHorizontal();
                EditorGUI.indentLevel++;
                mouth_ex = EditorGUILayout.Toggle("Mouth Exit", mouth_ex);
                EditorGUI.indentLevel--;

                if (!mouth_ex)
                {
                    GUI.enabled = false;
                    mouth_ex_hide = false;
                }

                EditorGUILayout.LabelField("Amount", GUILayout.MaxWidth(50));
                mouth_ex_select = EditorGUILayout.IntSlider(mouth_ex_select, 1, 10, GUILayout.MinWidth(160));

                EditorGUILayout.LabelField("  Hide Tab", GUILayout.MaxWidth(80));
                mouth_ex_hide = EditorGUILayout.Toggle(mouth_ex_hide);

                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();
                CreateAudioSlot(mouth_ex, mouth_ex_hide, mouth_ex_select, mouth_ex_clips, "mouth_exit");
                #endregion
            }

            boobs_fold = EditorGUILayout.Foldout(boobs_fold, "Boobs");
            if (boobs_fold)
            {
                #region boobs in
                EditorGUILayout.BeginHorizontal();
                EditorGUI.indentLevel++;
                boobs_in = EditorGUILayout.Toggle("Boobs Inward", boobs_in);
                EditorGUI.indentLevel--;

                if (!boobs_in)
                {
                    GUI.enabled = false;
                    boobs_in_hide = false;
                }

                EditorGUILayout.LabelField("Amount", GUILayout.MaxWidth(50));
                boobs_in_select = EditorGUILayout.IntSlider(boobs_in_select, 1, 10, GUILayout.MinWidth(160));

                EditorGUILayout.LabelField("  Hide Tab", GUILayout.MaxWidth(80));
                boobs_in_hide = EditorGUILayout.Toggle(boobs_in_hide);

                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();
                CreateAudioSlot(boobs_in, boobs_in_hide, boobs_in_select, boobs_in_clips, "boobs_in");
                #endregion

                #region boobs out
                EditorGUILayout.BeginHorizontal();
                EditorGUI.indentLevel++;
                boobs_out = EditorGUILayout.Toggle("Boobs Outward", boobs_out);
                EditorGUI.indentLevel--;

                if (!boobs_out)
                {
                    GUI.enabled = false;
                    boobs_out_hide = false;
                }

                EditorGUILayout.LabelField("Amount", GUILayout.MaxWidth(50));
                boobs_out_select = EditorGUILayout.IntSlider(boobs_out_select, 1, 10, GUILayout.MinWidth(160));

                EditorGUILayout.LabelField("  Hide Tab", GUILayout.MaxWidth(80));
                boobs_out_hide = EditorGUILayout.Toggle(boobs_out_hide);

                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();
                CreateAudioSlot(boobs_out, boobs_out_hide, boobs_out_select, boobs_out_clips, "boobs_out");
                #endregion

                #region boobs smash
                EditorGUILayout.BeginHorizontal();
                EditorGUI.indentLevel++;
                boobs_smash = EditorGUILayout.Toggle("Boobs Smash", boobs_smash);
                EditorGUI.indentLevel--;

                if (!boobs_smash)
                {
                    GUI.enabled = false;
                    boobs_smash_hide = false;
                }

                EditorGUILayout.LabelField("Amount", GUILayout.MaxWidth(50));
                boobs_smash_select = EditorGUILayout.IntSlider(boobs_smash_select, 1, 10, GUILayout.MinWidth(160));

                EditorGUILayout.LabelField("  Hide Tab", GUILayout.MaxWidth(80));
                boobs_smash_hide = EditorGUILayout.Toggle(boobs_smash_hide);

                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();
                CreateAudioSlot(boobs_smash, boobs_smash_hide, boobs_smash_select, boobs_smash_clips, "boobs_smash");
                #endregion
            }

            pussy_fold = EditorGUILayout.Foldout(pussy_fold, "Pussy");
            if (pussy_fold)
            {
                #region pussy in
                EditorGUILayout.BeginHorizontal();
                EditorGUI.indentLevel++;
                pussy_in = EditorGUILayout.Toggle("Pussy Inward", pussy_in);
                EditorGUI.indentLevel--;
                if (!pussy_in)
                {
                    GUI.enabled = false;
                    pussy_in_hide = false;
                }

                EditorGUILayout.LabelField("Amount", GUILayout.MaxWidth(50));
                pussy_in_select = EditorGUILayout.IntSlider(pussy_in_select, 1, 10, GUILayout.MinWidth(160));

                EditorGUILayout.LabelField("  Hide Tab", GUILayout.MaxWidth(80));
                pussy_in_hide = EditorGUILayout.Toggle(pussy_in_hide);

                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();
                CreateAudioSlot(pussy_in, pussy_in_hide, pussy_in_select, pussy_in_clips, "pussy_in");
                #endregion

                #region pussy out
                EditorGUILayout.BeginHorizontal();
                EditorGUI.indentLevel++;
                pussy_out = EditorGUILayout.Toggle("Pussy Outward", pussy_out);
                EditorGUI.indentLevel--;

                if (!pussy_out)
                {
                    GUI.enabled = false;
                    pussy_out_hide = false;
                }

                EditorGUILayout.LabelField("Amount", GUILayout.MaxWidth(50));
                pussy_out_select = EditorGUILayout.IntSlider(pussy_out_select, 1, 10, GUILayout.MinWidth(160));

                EditorGUILayout.LabelField("  Hide Tab", GUILayout.MaxWidth(80));
                pussy_out_hide = EditorGUILayout.Toggle(pussy_out_hide);

                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();
                CreateAudioSlot(pussy_out, pussy_out_hide, pussy_out_select, pussy_out_clips, "pussy_out");
                #endregion

                #region pussy exit
                EditorGUILayout.BeginHorizontal();
                EditorGUI.indentLevel++;
                pussy_exit = EditorGUILayout.Toggle("Pussy Exit", pussy_exit);
                EditorGUI.indentLevel--;

                if (!pussy_exit)
                {
                    GUI.enabled = false;
                    pussy_exit_hide = false;
                }

                EditorGUILayout.LabelField("Amount", GUILayout.MaxWidth(50));
                pussy_exit_select = EditorGUILayout.IntSlider(pussy_exit_select, 1, 10, GUILayout.MinWidth(160));

                EditorGUILayout.LabelField("  Hide Tab", GUILayout.MaxWidth(80));
                pussy_exit_hide = EditorGUILayout.Toggle(pussy_exit_hide);

                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();
                CreateAudioSlot(pussy_exit, pussy_exit_hide, pussy_exit_select, pussy_exit_clips, "pussy_exit");
                #endregion
            }

            ass_fold = EditorGUILayout.Foldout(ass_fold, "Anal");
            if (ass_fold)      
            {
                #region ass in
                EditorGUILayout.BeginHorizontal();
                EditorGUI.indentLevel++;
                ass_in = EditorGUILayout.Toggle("Anal Inward", ass_in);
                EditorGUI.indentLevel--;

                if (!ass_in)
                {
                    GUI.enabled = false;
                    ass_in_hide = false;
                }

                EditorGUILayout.LabelField("Amount", GUILayout.MaxWidth(50));
                ass_in_select = EditorGUILayout.IntSlider(ass_in_select, 1, 10, GUILayout.MinWidth(160));

                EditorGUILayout.LabelField("  Hide Tab", GUILayout.MaxWidth(80));
                ass_in_hide = EditorGUILayout.Toggle(ass_in_hide);

                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();
                CreateAudioSlot(ass_in, ass_in_hide, ass_in_select, ass_in_clips, "ass_in");
                #endregion

                #region ass out
                EditorGUILayout.BeginHorizontal();
                EditorGUI.indentLevel++;
                ass_out = EditorGUILayout.Toggle("Anal Outward", ass_out);
                EditorGUI.indentLevel--;
                if (!ass_out)
                {
                    GUI.enabled = false;
                    ass_out_hide = false;
                }

                EditorGUILayout.LabelField("Amount", GUILayout.MaxWidth(50));
                ass_out_select = EditorGUILayout.IntSlider(ass_out_select, 1, 10, GUILayout.MinWidth(160));

                EditorGUILayout.LabelField("  Hide Tab", GUILayout.MaxWidth(80));
                ass_out_hide = EditorGUILayout.Toggle(ass_out_hide);

                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();
                CreateAudioSlot(ass_out, ass_out_hide, ass_out_select, ass_out_clips, "ass_out");
                #endregion

                #region ass exit
                EditorGUILayout.BeginHorizontal();
                EditorGUI.indentLevel++;
                ass_exit = EditorGUILayout.Toggle("Anal Exit", ass_exit);
                EditorGUI.indentLevel--;

                if (!ass_exit)
                {
                    GUI.enabled = false;
                    ass_exit_hide = false;
                }

                EditorGUILayout.LabelField("Amount", GUILayout.MaxWidth(50));
                ass_exit_select = EditorGUILayout.IntSlider(ass_exit_select, 1, 10, GUILayout.MinWidth(160));

                EditorGUILayout.LabelField("  Hide Tab", GUILayout.MaxWidth(80));
                ass_exit_hide = EditorGUILayout.Toggle(ass_exit_hide);

                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();
                CreateAudioSlot(ass_exit, ass_exit_hide, ass_exit_select, ass_exit_clips, "ass_exit");
                #endregion
            }

            generic_fold = EditorGUILayout.Foldout(generic_fold, "Smash Hits");
            if (generic_fold)
            {
                #region generic smash 1
                EditorGUILayout.BeginHorizontal();
                EditorGUI.indentLevel++;
                generic_smash_1 = EditorGUILayout.Toggle("Soft Smash", generic_smash_1);
                EditorGUI.indentLevel--;

                if (!generic_smash_1)
                {
                    GUI.enabled = false;
                    generic_smash_1_hide = false;
                }

                EditorGUILayout.LabelField("Amount", GUILayout.MaxWidth(50));
                generic_smash_1_select = EditorGUILayout.IntSlider(generic_smash_1_select, 1, 10, GUILayout.MinWidth(160));

                EditorGUILayout.LabelField("  Hide Tab", GUILayout.MaxWidth(80));
                generic_smash_1_hide = EditorGUILayout.Toggle(generic_smash_1_hide);

                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();
                CreateAudioSlot(generic_smash_1, generic_smash_1_hide, generic_smash_1_select, generic_smash_1_clips, "generic_smash_1");
                #endregion

                #region generic smash 2
                EditorGUILayout.BeginHorizontal();
                EditorGUI.indentLevel++;
                generic_smash_2 = EditorGUILayout.Toggle("Medium Smash", generic_smash_2);
                EditorGUI.indentLevel--;

                if (!generic_smash_2)
                {
                    GUI.enabled = false;
                    generic_smash_2_hide = false;
                }

                EditorGUILayout.LabelField("Amount", GUILayout.MaxWidth(50));
                generic_smash_2_select = EditorGUILayout.IntSlider(generic_smash_2_select, 1, 10, GUILayout.MinWidth(160));

                EditorGUILayout.LabelField("  Hide Tab", GUILayout.MaxWidth(80));
                generic_smash_2_hide = EditorGUILayout.Toggle(generic_smash_2_hide);

                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();
                CreateAudioSlot(generic_smash_2, generic_smash_2_hide, generic_smash_2_select, generic_smash_2_clips, "generic_smash_2");
                #endregion

                #region generic smash 3
                EditorGUILayout.BeginHorizontal();
                EditorGUI.indentLevel++;
                generic_smash_3 = EditorGUILayout.Toggle("Hard Smash", generic_smash_3);
                EditorGUI.indentLevel--;

                if (!generic_smash_3)
                {
                    GUI.enabled = false;
                    generic_smash_3_hide = false;
                }

                EditorGUILayout.LabelField("Amount", GUILayout.MaxWidth(50));
                generic_smash_3_select = EditorGUILayout.IntSlider(generic_smash_3_select, 1, 10, GUILayout.MinWidth(160));

                EditorGUILayout.LabelField("  Hide Tab", GUILayout.MaxWidth(80));
                generic_smash_3_hide = EditorGUILayout.Toggle(generic_smash_3_hide);

                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();
                CreateAudioSlot(generic_smash_3, generic_smash_3_hide, generic_smash_3_select, generic_smash_3_clips, "generic_smash_3");
                #endregion

                #region generic smash 4
                EditorGUILayout.BeginHorizontal();
                EditorGUI.indentLevel++;
                generic_smash_4 = EditorGUILayout.Toggle("Heavy Smash", generic_smash_4);
                EditorGUI.indentLevel--;

                if (!generic_smash_4)
                {
                    GUI.enabled = false;
                    generic_smash_4_hide = false;
                }

                EditorGUILayout.LabelField("Amount", GUILayout.MaxWidth(50));
                generic_smash_4_select = EditorGUILayout.IntSlider(generic_smash_4_select, 1, 10, GUILayout.MinWidth(160));

                EditorGUILayout.LabelField("  Hide Tab", GUILayout.MaxWidth(80));
                generic_smash_4_hide = EditorGUILayout.Toggle(generic_smash_4_hide);

                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();
                CreateAudioSlot(generic_smash_4, generic_smash_4_hide, generic_smash_4_select, generic_smash_4_clips, "generic_smash_4");
                #endregion
            }
        }
        private void ApplySFX(bool showDialog)
        {          
            if (showDialog)
            {
                EditorUtility.DisplayDialog("PCS: Sound Replacer", "Replace Complete!\n\nYou can also change this yourself later in PCS Audio Controller layer.", "Okay");
            }

            //Mouth
            var mouth_in_clipsX = VerifyAudioClips(mouth_in_clips, mouth_in_select,"mi");
            var state_mouth_in = FindAnimatorStateByName("state_mouth_in", animator);
            VRCAnimatorPlayAudio audioCtrl1 = state_mouth_in.behaviours[0] as VRCAnimatorPlayAudio;
            audioCtrl1.Clips = mouth_in_clipsX;

            var mouth_out_clipsX = VerifyAudioClips(mouth_out_clips, mouth_out_select, "mo");
            var state_mouth_out = FindAnimatorStateByName("state_mouth_out", animator);
            VRCAnimatorPlayAudio audioCtrl2 = state_mouth_out.behaviours[0] as VRCAnimatorPlayAudio;
            audioCtrl2.Clips = mouth_out_clipsX;

            var mouth_sm_clipsX = VerifyAudioClips(mouth_sm_clips, mouth_sm_select, "ms");
            var state_mouth_sm = FindAnimatorStateByName("state_mouth_smash", animator);
            VRCAnimatorPlayAudio audioCtrl3 = state_mouth_sm.behaviours[0] as VRCAnimatorPlayAudio;
            audioCtrl3.Clips = mouth_sm_clipsX;

            var mouth_ex_clipsX = VerifyAudioClips(mouth_ex_clips, mouth_ex_select, "mx");
            var state_mouth_ex = FindAnimatorStateByName("state_mouth_allExit", animator);
            VRCAnimatorPlayAudio audioCtrl4 = state_mouth_ex.behaviours[0] as VRCAnimatorPlayAudio;
            audioCtrl4.Clips = mouth_ex_clipsX;

            //Boobs
            var boobs_in_clipsX = VerifyAudioClips(boobs_in_clips, boobs_in_select, "bi");
            var state_boobs_in = FindAnimatorStateByName("state_boobs_in", animator);
            VRCAnimatorPlayAudio audioCtrl5 = state_boobs_in.behaviours[0] as VRCAnimatorPlayAudio;
            audioCtrl5.Clips = boobs_in_clipsX;

            var boobs_out_clipsX = VerifyAudioClips(boobs_out_clips, boobs_out_select,"bo");
            var state_boobs_out = FindAnimatorStateByName("state_boobs_out", animator);
            VRCAnimatorPlayAudio audioCtrl6 = state_boobs_out.behaviours[0] as VRCAnimatorPlayAudio;
            audioCtrl6.Clips = boobs_out_clipsX;

            var boobs_smash_clipsX = VerifyAudioClips(boobs_smash_clips, boobs_smash_select,"bs");
            var state_boobs_smash = FindAnimatorStateByName("state_boobs_smash", animator);
            VRCAnimatorPlayAudio audioCtrl7 = state_boobs_smash.behaviours[0] as VRCAnimatorPlayAudio;
            audioCtrl7.Clips = boobs_smash_clipsX;

            //Pussy
            var pussy_in_clipsX = VerifyAudioClips(pussy_in_clips, pussy_in_select,"pi");
            var state_pussy_in = FindAnimatorStateByName("state_pussy_in", animator);
            VRCAnimatorPlayAudio audioCtrl8 = state_pussy_in.behaviours[0] as VRCAnimatorPlayAudio;
            audioCtrl8.Clips = pussy_in_clipsX;

            var pussy_out_clipsX = VerifyAudioClips(pussy_out_clips, pussy_out_select,"po");
            var state_pussy_out = FindAnimatorStateByName("state_pussy_out", animator);
            VRCAnimatorPlayAudio audioCtrl9 = state_pussy_out.behaviours[0] as VRCAnimatorPlayAudio;
            audioCtrl9.Clips = pussy_out_clipsX;

            var pussy_exit_clipsX = VerifyAudioClips(pussy_exit_clips, pussy_exit_select,"px");
            var state_pussy_exit = FindAnimatorStateByName("state_pussy_allExit", animator);
            VRCAnimatorPlayAudio audioCtrl10 = state_pussy_exit.behaviours[0] as VRCAnimatorPlayAudio;
            audioCtrl10.Clips = pussy_exit_clipsX;

            //Ass
            var ass_in_clipsX = VerifyAudioClips(ass_in_clips, ass_in_select,"ai");
            var state_ass_in = FindAnimatorStateByName("state_ass_in", animator);
            VRCAnimatorPlayAudio audioCtrl11 = state_ass_in.behaviours[0] as VRCAnimatorPlayAudio;
            audioCtrl11.Clips = ass_in_clipsX;

            var ass_out_clipsX = VerifyAudioClips(ass_out_clips, ass_out_select,"ao");
            var state_ass_out = FindAnimatorStateByName("state_ass_out", animator);
            VRCAnimatorPlayAudio audioCtrl12 = state_ass_out.behaviours[0] as VRCAnimatorPlayAudio;
            audioCtrl12.Clips = ass_out_clipsX;

            var ass_exit_clipsX = VerifyAudioClips(ass_exit_clips, ass_exit_select,"ax");
            var state_ass_exit = FindAnimatorStateByName("state_ass_allExit", animator);
            VRCAnimatorPlayAudio audioCtrl13 = state_ass_exit.behaviours[0] as VRCAnimatorPlayAudio;
            audioCtrl13.Clips = ass_exit_clipsX;

            //Smash
            var generic_smash_1_clipsX = VerifyAudioClips(generic_smash_1_clips, generic_smash_1_select,"s1");
            var state_generic_smash_1 = FindAnimatorStateByName("state_smash_soft", animator);
            VRCAnimatorPlayAudio audioCtrl14 = state_generic_smash_1.behaviours[0] as VRCAnimatorPlayAudio;
            audioCtrl14.Clips = generic_smash_1_clipsX;

            var generic_smash_2_clipsX = VerifyAudioClips(generic_smash_2_clips, generic_smash_2_select, "s2");
            var state_generic_smash_2 = FindAnimatorStateByName("state_smash_medium", animator);
            VRCAnimatorPlayAudio audioCtrl15 = state_generic_smash_2.behaviours[0] as VRCAnimatorPlayAudio;
            audioCtrl15.Clips = generic_smash_2_clipsX;

            var generic_smash_3_clipsX = VerifyAudioClips(generic_smash_3_clips, generic_smash_3_select, "s3");
            var state_generic_smash_3 = FindAnimatorStateByName("state_smash_hard", animator);
            VRCAnimatorPlayAudio audioCtrl16 = state_generic_smash_3.behaviours[0] as VRCAnimatorPlayAudio;
            audioCtrl16.Clips = generic_smash_3_clipsX;

            var generic_smash_4_clipsX = VerifyAudioClips(generic_smash_4_clips, generic_smash_4_select, "s4");
            var state_generic_smash_4 = FindAnimatorStateByName("state_smash_heavy", animator);
            VRCAnimatorPlayAudio audioCtrl17 = state_generic_smash_4.behaviours[0] as VRCAnimatorPlayAudio;
            audioCtrl17.Clips = generic_smash_4_clipsX;

            //#####################################
            EditorUtility.SetDirty(animator);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        private void CreateAudioSlot(bool show, bool hide, int amount, AudioClip[] audioClips, string tag)
        {
            if (show && !hide)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginVertical("ProgressBarBack");
                EditorGUI.BeginChangeCheck();
                for (int i = 0; i < amount; i++)
                {
                    audioClips[i] = EditorGUILayout.ObjectField("", audioClips[i], typeof(AudioClip), true) as AudioClip;
                }
                EditorGUILayout.EndVertical();
                if (EditorGUI.EndChangeCheck())
                {
                    int dragLength = DragAndDrop.objectReferences.Length;

                    amount = dragLength;
                    if (amount > 10)
                    {
                        amount = 10;
                    }
                    
                    if (dragLength > 1) //If drag multiple
                    {
                        int x = 0;
                        for (int i = 0; i < dragLength; i++)
                        {
                            if (x < amount)
                            {
                                audioClips[i] = DragAndDrop.objectReferences.ElementAt(i) as AudioClip as AudioClip;
                                x++;
                            }
                        }

                        //Set Amount
                        if (tag == "mouth_in")
                        {
                            mouth_in_select = amount;
                        }
                        if (tag == "mouth_out")
                        {
                            mouth_out_select = amount;
                        }
                        if (tag == "mouth_mash")
                        {
                            mouth_sm_select = amount;
                        }
                        if (tag == "mouth_exit")
                        {
                            mouth_ex_select = amount;
                        }
                        if (tag == "boobs_in")
                        {
                            boobs_in_select = amount;
                        }
                        if (tag == "boobs_out")
                        {
                            boobs_out_select = amount;
                        }
                        if (tag == "boobs_smash")
                        {
                            boobs_smash_select = amount;
                        }
                        if (tag == "pussy_in")
                        {
                            pussy_in_select = amount;
                        }
                        if (tag == "pussy_out")
                        {
                            pussy_out_select = amount;
                        }
                        if (tag == "pussy_exit")
                        {
                            pussy_exit_select = amount;
                        }
                        if (tag == "ass_in")
                        {
                            ass_in_select = amount;
                        }
                        if (tag == "ass_out")
                        {
                            ass_out_select = amount;
                        }
                        if (tag == "ass_exit")
                        {
                            ass_exit_select = amount;
                        }
                        if (tag == "generic_smash_1")
                        {
                            generic_smash_1_select = amount;
                        }
                        if (tag == "generic_smash_2")
                        {
                            generic_smash_2_select = amount;
                        }
                        if (tag == "generic_smash_3")
                        {
                            generic_smash_3_select = amount;
                        }
                        if (tag == "generic_smash_4")
                        {
                            generic_smash_4_select = amount;
                        }
                    }
                }
                EditorGUI.indentLevel--;
            }
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
        private static AudioClip[] VerifyAudioClips(AudioClip[] clips, int select, string ID) //To remove empty array slot and keep only the existings.
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

            //Set Original
            string folderPath = "Assets/!Dismay Custom/Penetration Contact System/Assets/SFX/";

            if(ID == "mi")
            {
                int num = 5;
                if (!mouth_in || temp_clips.Length == 0)
                {
                    temp_clips = new AudioClip[num];
                    for (int i = 0; i < num; i++) //Change original audio clips amount
                    {
                        temp_clips[i]                                              //Change files name here
                        = (AudioClip)AssetDatabase.LoadAssetAtPath(folderPath + "Mouth/mouth_in (" + (i + 1) + ").wav", typeof(AudioClip));
                    }
                    return temp_clips;
                }
            }
            if (ID == "mo")
            {
                int num = 5;
                if (!mouth_out || temp_clips.Length == 0)
                {
                    temp_clips = new AudioClip[num];
                    for (int i = 0; i < num; i++) //Change original audio clips amount
                    {
                        temp_clips[i]                                              //Change files name here
                        = (AudioClip)AssetDatabase.LoadAssetAtPath(folderPath + "Mouth/mouth_out (" + (i + 1) + ").wav", typeof(AudioClip));
                    }
                    return temp_clips;
                }
            }
            if (ID == "ms")
            {
                int num = 3;
                if (!mouth_sm || temp_clips.Length == 0)
                {
                    temp_clips = new AudioClip[num];
                    for (int i = 0; i < num; i++) //Change original audio clips amount
                    {
                        temp_clips[i]                                              //Change files name here
                        = (AudioClip)AssetDatabase.LoadAssetAtPath(folderPath + "Mouth/mouth_smash (" + (i + 1) + ").wav", typeof(AudioClip));
                    }
                    return temp_clips;
                }
            }
            if (ID == "mx")
            {
                int num = 5;
                if (!mouth_ex || temp_clips.Length == 0)
                {
                    temp_clips = new AudioClip[num];
                    for (int i = 0; i < num; i++) //Change original audio clips amount
                    {
                        temp_clips[i]                                              //Change files name here
                        = (AudioClip)AssetDatabase.LoadAssetAtPath(folderPath + "Mouth/mouth_exit (" + (i + 1) + ").wav", typeof(AudioClip));
                    }
                    return temp_clips;
                }
            }

            if (ID == "bi")
            {
                int num = 5;
                if (!boobs_in || temp_clips.Length == 0)
                {
                    temp_clips = new AudioClip[num];
                    for (int i = 0; i < num; i++) //Change original audio clips amount
                    {
                        temp_clips[i]                                              //Change files name here
                        = (AudioClip)AssetDatabase.LoadAssetAtPath(folderPath + "Boobs/boobs_in (" + (i + 1) + ").wav", typeof(AudioClip));
                    }
                    return temp_clips;
                }
            }
            if (ID == "bo")
            {
                int num = 5;
                if (!boobs_out || temp_clips.Length == 0)
                {
                    temp_clips = new AudioClip[num];
                    for (int i = 0; i < num; i++) //Change original audio clips amount
                    {
                        temp_clips[i]                                              //Change files name here
                        = (AudioClip)AssetDatabase.LoadAssetAtPath(folderPath + "Boobs/boobs_out (" + (i + 1) + ").wav", typeof(AudioClip));
                    }
                    return temp_clips;
                }
            }
            if (ID == "bs")
            {
                int num = 5;
                if (!boobs_smash || temp_clips.Length == 0)
                {
                    temp_clips = new AudioClip[num];
                    for (int i = 0; i < num; i++) //Change original audio clips amount
                    {
                        temp_clips[i]                                              //Change files name here
                        = (AudioClip)AssetDatabase.LoadAssetAtPath(folderPath + "Boobs/boobs_smash (" + (i + 1) + ").wav", typeof(AudioClip));
                    }
                    return temp_clips;
                }
            }

            if (ID == "pi")
            {
                int num = 10;
                if (!pussy_in || temp_clips.Length == 0)
                {
                    temp_clips = new AudioClip[num];
                    for (int i = 0; i < num; i++) //Change original audio clips amount
                    {
                        temp_clips[i]                                              //Change files name here
                        = (AudioClip)AssetDatabase.LoadAssetAtPath(folderPath + "Pussy/pussy_in (" + (i + 1) + ").wav", typeof(AudioClip));
                    }
                    return temp_clips;
                }
            }
            if (ID == "po")
            {
                int num = 10;
                if (!pussy_out || temp_clips.Length == 0)
                {
                    temp_clips = new AudioClip[num];
                    for (int i = 0; i < num; i++) //Change original audio clips amount
                    {
                        temp_clips[i]                                              //Change files name here
                        = (AudioClip)AssetDatabase.LoadAssetAtPath(folderPath + "Pussy/pussy_out (" + (i + 1) + ").wav", typeof(AudioClip));
                    }
                    return temp_clips;
                }
            }
            if (ID == "px")
            {
                int num = 5;
                if (!pussy_exit || temp_clips.Length == 0)
                {
                    temp_clips = new AudioClip[num];
                    for (int i = 0; i < num; i++) //Change original audio clips amount
                    {
                        temp_clips[i]                                              //Change files name here
                        = (AudioClip)AssetDatabase.LoadAssetAtPath(folderPath + "Generic/generic_exit (" + (i + 1) + ").wav", typeof(AudioClip));
                    }
                    return temp_clips;
                }
            }

            if (ID == "ai")
            {
                int num = 10;
                if (!ass_in || temp_clips.Length == 0)
                {
                    temp_clips = new AudioClip[num];
                    for (int i = 0; i < num; i++) //Change original audio clips amount
                    {
                        temp_clips[i]                                              //Change files name here
                        = (AudioClip)AssetDatabase.LoadAssetAtPath(folderPath + "Ass/ass_in (" + (i + 1) + ").wav", typeof(AudioClip));
                    }
                    return temp_clips;
                }
            }
            if (ID == "ao")
            {
                int num = 5;
                if (!ass_out || temp_clips.Length == 0)
                {
                    temp_clips = new AudioClip[num];
                    for (int i = 0; i < num; i++) //Change original audio clips amount
                    {
                        temp_clips[i]                                              //Change files name here
                        = (AudioClip)AssetDatabase.LoadAssetAtPath(folderPath + "Ass/ass_out (" + (i + 1) + ").wav", typeof(AudioClip));
                    }
                    return temp_clips;
                }
            }
            if (ID == "ax")
            {
                int num = 5;
                if (!ass_exit || temp_clips.Length == 0)
                {
                    temp_clips = new AudioClip[num];
                    for (int i = 0; i < num; i++) //Change original audio clips amount
                    {
                        temp_clips[i]                                              //Change files name here
                        = (AudioClip)AssetDatabase.LoadAssetAtPath(folderPath + "Generic/generic_exit (" + (i + 1) + ").wav", typeof(AudioClip));
                    }
                    return temp_clips;
                }
            }

            if (ID == "s1")
            {
                int num = 10;
                if (!generic_smash_1 || temp_clips.Length == 0)
                {
                    temp_clips = new AudioClip[num];
                    for (int i = 0; i < num; i++) //Change original audio clips amount
                    {
                        temp_clips[i]                                              //Change files name here
                        = (AudioClip)AssetDatabase.LoadAssetAtPath(folderPath + "Smash Hit/smash_soft (" + (i + 1) + ").wav", typeof(AudioClip));
                    }
                    return temp_clips;
                }
            }
            if (ID == "s2")
            {
                int num = 10;
                if (!generic_smash_2 || temp_clips.Length == 0)
                {
                    temp_clips = new AudioClip[num];
                    for (int i = 0; i < num; i++) //Change original audio clips amount
                    {
                        temp_clips[i]                                              //Change files name here
                        = (AudioClip)AssetDatabase.LoadAssetAtPath(folderPath + "Smash Hit/smash_medium (" + (i + 1) + ").wav", typeof(AudioClip));
                    }
                    return temp_clips;
                }
            }
            if (ID == "s3")
            {
                int num = 10;
                if (!generic_smash_3 || temp_clips.Length == 0)
                {
                    temp_clips = new AudioClip[num];
                    for (int i = 0; i < num; i++) //Change original audio clips amount
                    {
                        temp_clips[i]                                              //Change files name here
                        = (AudioClip)AssetDatabase.LoadAssetAtPath(folderPath + "Smash Hit/smash_hard (" + (i + 1) + ").wav", typeof(AudioClip));
                    }
                    return temp_clips;
                }
            }
            if (ID == "s4")
            {
                int num = 10;
                if (!generic_smash_4 || temp_clips.Length == 0)
                {
                    temp_clips = new AudioClip[num];
                    for (int i = 0; i < num; i++) //Change original audio clips amount
                    {
                        temp_clips[i]                                              //Change files name here
                        = (AudioClip)AssetDatabase.LoadAssetAtPath(folderPath + "Smash Hit/smash_heavy (" + (i + 1) + ").wav", typeof(AudioClip));
                    }
                    return temp_clips;
                }
            }

            //
            return temp_clips;
        }
        private static void RemoveAt<T>(ref T[] arr, int index)
        {
            for (int a = index; a < arr.Length - 1; a++)
            {
                arr[a] = arr[a + 1];
            }
            Array.Resize(ref arr, arr.Length - 1);
        }

        //Temp method for Voice Pack setup... **TODO** Make replacer for Voice Pack.
        private static AnimatorState[] voice_state_event = new AnimatorState[12];
        private static AnimatorState voice_softMoan, voice_roughMoan, voice_relax;
        private static AudioClip[] voice_combo_clip1, voice_combo_clip2 = new AudioClip[1], voice_combo_clip3 = new AudioClip[1];
        public static void ApplyVoiceFX(AnimatorController controller)
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
                voice_state_event[i].speed = 1/ voice_event_clip[i].length;
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
            voice_softMoan_clip = VerifyAudioClips(voice_softMoan_clip, 20, "");
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
            voice_roughMoan_clip = VerifyAudioClips(voice_roughMoan_clip, 20, "");
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

            float FindMidpoint(float num1, float num2, float num3)
            {
                return (num1 + num2 + num3) / 3f;
            }
            voice_relax.speed = 1/FindMidpoint(voice_relax_clip[0].length, voice_relax_clip[1].length, voice_relax_clip[2].length);
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
                VRCAvatarParameterDriver.Parameter x = new VRCAvatarParameterDriver.Parameter()
                {
                    type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Random,
                    name = "pcs/local/moan-index",
                    valueMin = 1,
                    valueMax = combo_set_amount,
                };
                voice_combo_set.parameters.Add(x);
                voice_combo_clip1 = new AudioClip[9];
                voice_combo_clip2 = new AudioClip[9];

                for(int i = 0; i < 9; i++)
                {
                    voice_combo_clip1[i] = (AudioClip)AssetDatabase.LoadAssetAtPath("Assets/!Dismay Custom/Penetration Contact System/Assets/Voice Pack/" + PCSConfigurator.voicePack.ToString() + "/Combo/moan_combo_a (" + (i + 1) + ").wav", typeof(AudioClip));
                    voice_combo_clip2[i] = (AudioClip)AssetDatabase.LoadAssetAtPath("Assets/!Dismay Custom/Penetration Contact System/Assets/Voice Pack/" + PCSConfigurator.voicePack.ToString() + "/Combo/moan_combo_b (" + (i + 1) + ").wav", typeof(AudioClip));
                    //voice_combo_clip3[i] = This voice pack has only 2 combo set
                }
            }

            if (PCSConfigurator.voicePack == PCSConfigurator.VoicePack.LewdHeart)
            {
                int combo_set_amount = 2;
                VRCAvatarParameterDriver.Parameter x = new VRCAvatarParameterDriver.Parameter()
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
                VRCAvatarParameterDriver.Parameter x = new VRCAvatarParameterDriver.Parameter()
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
