using UnityEngine;
using UnityEditor;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;
using System.Linq;
using VRC.SDK3.Dynamics.Constraint.Components;
using UnityEditor.Search;
using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using VRC.Dynamics;
using System.Data;

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
    public class PCSConfigurator : EditorWindow
    {
        #region Variables
        //GUI
        private GUIStyle paramStyle, infoStyle;
        private GUIStyle rightAlignedStyle;
        private VRCAvatarDescriptor targetAvatar;
        private Animator animator;
        private Texture2D logo;
        private Vector2 scrollPosition = new(0, 300);

        //Misc
        private readonly string thisGimmick = "Penetration Contact System";
        public static readonly string version = "1.10";
        private static readonly string[] customPos_menuName = new string[] { "Custom #1", "Custom #2", "Custom #3", "Custom #4", "Custom #5", "Custom #6", "Custom #7", "Custom #8" };
        private readonly string[] customPos_choiceName = new string[] { "Disable", "1", "2", "3", "4", "5", "6", "7", "8" };
        private readonly int[] customPos_sizes = { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
        private static int selected_customPos;
        private static float smashSensitivity = 1, lustMultiplierValue = 0.4f;
        private static bool lustFeature = false, useMouth = true, useBoobs = true, usePussy = true, useAss = true, setLocal, useDirectionOffset, spawnSPSsocket;
        private bool hidePlacement = true, flag1 = false, flag2 = false, flag3 = false, isError = false, advancedTab;
        private static GameObject ref_mouth = null, ref_boobs = null, ref_pussy = null, ref_ass = null;
        private static readonly GameObject[] ref_soundPosition = new GameObject[8];

        //Enum
        public enum VoicePack
        {
            Disable, Misuzugon, LewdHeart, NekoNyan,
        }
        public static VoicePack voicePack = VoicePack.Misuzugon;
        private readonly string[] customEnumStrings = new string[]
        {
        "Disable",
        "Misuzugon (Soft, Gentle, Young)",
        "LewdHeart (Assertive, Naughty, Mature)",
        "NekoNyan (Sultry, Rough, Mommy)"
        };
        int selectedIndex = (int)voicePack;

        private enum Preset
        {
            Generic,
            Reference,
            Airi,
            Anon,            
            Aria,
            Chiffon,
            Chocolat,
            Eyo,
            Ichigo,
            Imeris,
            Karin,
            Kikyo,
            Lasyusha,
            Leefa,
            Lime,
            Mafuyu,
            Manuka,
            Maya,
            Milfy,
            Milltina,
            Mizuki,
            Moe,
            Mophira,
            Rindo,
            RunaRobotic,
            Rurune,
            Shinano,
            Sio,
            Selestia,
            Shinra,
            UltimateKissMa,
            Uruki,
            Uzuki,
            Velle,
            Wolferia,
        }
        private static Preset preset = Preset.Generic;

        //Language
        public enum Language
        {
            English,
            ภาษาไทย,
            日本語,
            한국어,
            简体中文
        }
        private static Language currentLanguage = Language.English;
        private readonly Dictionary<string, string> english = new()
        {
            {"alignment", "Alignment Preset" },
            {"position", "Position & Sounds" },
            {"mouth","Mouth " },
            {"boobs","Boobs " },
            {"pussy","Pussy " },
            {"ass","Ass " },
            {"mouth_pos","Mouth Position" },
            {"boobs_pos","Boobs Position" },
            {"pussy_pos","Pussy Position" },
            {"ass_pos","Ass Position" },
            {"findSPS","Find SPS Socket" },
            {"smashSensitivity","Smash Sensitivity" },
            {"customPos","Custom Position" },
            {"customTarget","Target #" },
            {"menuName","Menu Name" },
            {"handjob","Handjob" },
            {"footjob","Footjob" },
            {"bothjob","Both" },
            {"lustFeat","Lust Feature (Voice)" },
            {"lustMul","Lust Multiplier" },
            {"voicepack","Voice Pack" },
            {"directionOffset","Direction Offset" },
            {"addSPS","Add SPS Socket" },
            {"tooltip_sps","This option adds an SPS socket to each <PCS Target>, allowing it to be toggled ON/OFF along with the PCS sound menu. It’s an optional and convenient way to link the PCS menu to SPS without manual setup. Alternatively, you can manually place an existing SPS socket into each <PCS Target>. If you choose this method, make sure to uncheck \"Enable Menu Toggle\" to prevent the SPS menu from being generated automatically." },
            {"advancedOption","Tip: How to link PCS menu with SPS socket" },
            {"localOnly","Remove Menu Toggle" },
            {"localOnly2","<color=red>The sound toggle menu has been disabled." },

            //help box
            {"helpbox1","You must select at least one location and sound. Otherwise, this system will be useless!!" },
            {"helpbox2","Please specify at least one reference location. Leave it as \"None\" if you want to disable." },
            {"helpbox3","Please assign all custom target and menu name. You can set it to anywhere you want." },
            {"helpbox4","This option adds a Direction Offset menu. This radial menu allows you to rotate PCS to align with the penetrator’s direction for more accurate detection." },
            {"helpbox5","This option will automatically create SPS Sockets for PCS and link their menus together, so you don’t have to set it up manually. This option is helpful when setting up SPS for the first time." },
            {"helpbox6","Warning! This option will remove all PCS sound menu toggles and their parameters (pcs/menu/xxx). Use this only if you plan to control the PCS sound menu via Avatar Parameter Driver or SPS Socket." },
            {"helpbox7","Please drop your avatar into the box. Hover your mouse over the text to view the description." },
            {"helpbox_pos1","Mouth reference target is not a child gameObject of your avatar." },
            {"helpbox_pos2","Boobs reference target is not a child gameObject of your avatar." },
            {"helpbox_pos3","Pussy reference target is not a child gameObject of your avatar." },
            {"helpbox_pos4","Ass reference target is not a child gameObject of your avatar." },

            //tooltip
            {"tooltip1", "Placement preset for some avatars. Select Generic if there is no preset for your selected avatar. Select Reference to place them on your preferred location instead." },
            {"tooltip2", "This option allows you to adjust the sensitivity for impact detection. Lower this value requires more thrust to trigger the sound. 1 is recommended :)" },
            {"tooltip3", "This let you gain lust value from being penetrated and make you moan and squirt when lust is full." },
            {"tooltip4", "Adjust this to determine how quickly you will get 1 point of lust value from a penetrating stroke. Increasing this value and you'll reach climax faster." },

            {"confirm1", "Setup Complete. PCS has been installed!\n\nWhat you can do next is adjust the position and rotation of the PCS Target to match your desired location. Please use the Quick Access menu to find all adjustable voice positions.\r\n" },
            {"confirm2", "Are you sure you want to remove this gimmick?" },

            {"error1", "PCS resource folder not found. Don’t move !Dismay Custom folder and avoid using invalid characters in avatar name such as (<>:\"|?*)" },

            //button
            {"quickAccess", "Quick Access" },
            {"button1", "Apply" },
            {"button2", "Replace" },
            {"button3", "Remove" },
            {"button4", "Hide/Show Placement Icons" },
            {"button5", "Spawn a Test Penetrator" },
            {"button6", "Locate Mouth" },
            {"button7", "Locate Boobs" },
            {"button8", "Locate Pussy" },
            {"button9", "Locate Ass" },
        };
        private readonly Dictionary<string, string> japanese = new()
        {
            {"alignment", "プリセット" },
            {"position", "サウンド" },
            {"mouth","口 " },
            {"boobs","胸 " },
            {"pussy","膣 " },
            {"ass","尻 " },
            {"mouth_pos","口の位置" },
            {"boobs_pos","胸の位置" },
            {"pussy_pos","膣の位置" },
            {"ass_pos","尻の位置" },
            {"findSPS","[SPSソケット] を探す" },
            {"smashSensitivity","スマッシュ感度" },
            {"customPos","カスタムポジション" },
            {"customTarget","ターゲット＃" },
            {"menuName","メニュー名" },
            {"handjob","ハンドジョブ" },
            {"footjob","フットジョブ" },
            {"bothjob","両方" },
            {"lustFeat","Lust Feature (ボイス)" },
            {"lustMul","欲望倍率" },
            {"voicepack","ボイスパック" },
            {"directionOffset","Direction Offset" },
            {"addSPS","[SPSソケット] を追加" },
            {"tooltip_sps","このオプションを有効にすると、各 <PCS Target> に SPS ソケットが追加され、PCS のサウンドメニューと一緒に ON/OFF を切り替えることができます。これは、PCS メニューを SPS に手動でリンクする必要がない便利なオプションです。既存の SPS ソケットを各 <PCS Target> に手動で配置することも可能です。その場合は、SPS メニューが自動的に生成されないように、「Enable Menu Toggle」 のチェックを外してください。" },
            {"advancedOption","ヒント：SPSソケットでPCSメニューを制御する方法" },
            {"localOnly","メニュー切り替えを無効化" },
            {"localOnly2","<color=red>サウンド切り替えメニューが無効になりました。" },

            //help box
            {"helpbox1","少なくとも1つの位置とサウンドを選択する必要があります。そうでないと、このシステムは機能しません！" },
            {"helpbox2","少なくとも1つの参照位置を指定してください。無効にしたい場合は「None」のままにしてください。"},
            {"helpbox3","すべてのカスタムターゲットとメニュー名を指定してください。どこにでも設定可能です。" },
            {"helpbox4","このオプションは「方向オフセット」メニューを追加します。このラジアルメニューを使用して、PCSを貫通方向に合わせて回転させ、より正確な検出が可能になります。" },
            {"helpbox5","このオプションを選択すると、PCS 用の SPS ソケットが自動的に作成され、両方のメニューが自動でリンクされます。手動で設定する必要はありません。SPS を初めて設定する場合に便利です。" },
            {"helpbox6","警告！ このオプションは、PCS のサウンド切り替えメニューとそのパラメーター（pcs/menu/xxx）をすべて削除します。PCS サウンドメニューを Avatar Parameter Driver または SPS Socket で制御する場合のみ使用してください。" },
            {"helpbox7","アバターをこのボックスにドラッグ＆ドロップしてください。テキストにマウスを乗せると説明が表示されます。" },
            {"helpbox_pos1","口の参照ターゲットはアバターの子オブジェクトではありません。" },
            {"helpbox_pos2","胸の参照ターゲットはアバターの子オブジェクトではありません。" },
            {"helpbox_pos3","膣の参照ターゲットはアバターの子オブジェクトではありません。" },
            {"helpbox_pos4","尻の参照ターゲットはアバターの子オブジェクトではありません。" },
            
            //tooltip
            {"tooltip1", "一部のアバターには配置プリセットがあります。選択したアバターにプリセットがない場合は「Generic」を選んでください。自分の好みの位置に配置したい場合は「Reference」を選んでください。" },
            {"tooltip2", "このオプションでは衝撃検出の感度を調整できます。値を下げると音を鳴らすにはより強い動きが必要になります。推奨値は1です :)" },
            {"tooltip3", "挿入されることで欲望値が増加し、満たされると喘ぎ声やスプラッシュ効果が発動します。" },
            {"tooltip4", "挿入1回あたりに得られる欲望値の速度を調整します。値を上げると早くクライマックスに達します。" },

            {"confirm1", "セットアップ完了。PCSがインストールされました！\n\n今後できることは、PCSターゲットの位置と回転を調整して、希望する位置に合わせることです。調整可能なすべての音声位置を見つけるには、クイックアクセスメニューをご利用ください。" },
            {"confirm2", "このギミックを削除してもよろしいですか？" },

            {"error1", "PCSのリソースフォルダーが見つかりません。!Dismay Customフォルダーを移動しないでください。また、アバター名に（<>:\"|?*）などの無効な文字を使用しないでください。" },

            //button
            {"quickAccess", "クイックアクセス" },
            {"button1", "適用" },
            {"button2", "置き換え" },
            {"button3", "削除" },
            {"button4", "配置アイコンの表示／非表示" },
            {"button5", "テスト用ペネトレーターを出現させる" },
            {"button6", "口の位置を特定" },
            {"button7", "胸の位置を特定" },
            {"button8", "膣の位置を特定" },
            {"button9", "尻の位置を特定" },
        };
        private readonly Dictionary<string, string> thai = new()
        {
            {"alignment", "รูปแบบการจัดวาง" },
            {"position", "ตำแหน่งและเสียง" },
            {"mouth","ปาก " },
            {"boobs","ร่องนม " },
            {"pussy","ช่องคลอด " },
            {"ass","ประตูหลัง " },
            {"mouth_pos","ตำแหน่งของ ปาก" },
            {"boobs_pos","ตำแหน่งของ ร่องนม" },
            {"pussy_pos","ตำแหน่งของ ช่องคลอด" },
            {"ass_pos","ตำแหน่งของ ประตูหลัง" },
            {"findSPS","ค้นหา SPS Socket" },
            {"smashSensitivity","ความง่ายในการกระแทก" },
            {"customPos","ตำแหน่งเพิ่มเติม" },
            {"customTarget","ตำแหน่งที่ " },
            {"menuName","ชื่อเมนู" },
            {"handjob","Handjob" },
            {"footjob","Footjob" },
            {"bothjob","ทั้งสองอย่าง" },
            {"lustFeat","Lust Feature (เสียงคราง)" },
            {"lustMul","ตัวคูณค่า Lust" },
            {"voicepack","เสียงคราง" },
            {"directionOffset","Direction Offset" },
            {"addSPS","เพิ่ม SPS Socket" },
            {"tooltip_sps","ตัวเลือกนี้จะเพิ่ม SPS socket ให้กับ <PCS Target> แต่ละตัว ซึ่งจะถูกเปิด/ปิดพร้อมกับเมนูเสียงของ PCS นี่เป็นทางเลือกที่ช่วยให้เราไม่ต้องเชื่อมเมนู PCS กับ SPS เอง หากคุณต้องการวาง Socket ที่มีอยู่เดิมลงใน <PCS Target> เองก็สามารถทำได้ เพียงอย่าลืม ติ๊กช่อง \"Enable Menu Toggle\" ใน SPS ออกเพื่อป้องกันไม่ให้ระบบสร้างเมนู SPS ขึ้นมา" },
            {"advancedOption","เคล็ดลับ: วิธีควบคุมเมนู PCS ผ่าน SPS Socket" },
            {"localOnly","ปิดเมนูสลับเสียง" },
            {"localOnly2","<color=red>เมนูสลับเสียงได้ถูกปิดการใช้งานแล้ว" },

            //help box
            {"helpbox1","คุณต้องเลือกใช้งานอย่างน้อยหนึ่งตำแหน่งเสียง มิเช่นนั้นระบบจะไม่ทำงาน!!" },
            {"helpbox2","โปรดเลือกตำแหน่งแบบกำหนดเองอย่างน้อยหนึ่งที่ เว้นช่องว่างไว้หากไม่ได้ใช้ตำแหน่งหรือเสียงนั้นๆ" },
            {"helpbox3","โปรดกำหนดตำแหน่งและตั้งชื่อเมนูให้ครบทุกช่อง เพียงลากวางวัตถุอ้างอิงตำแหน่งลงไปในช่อง" },
            {"helpbox4","ตัวเลือกนี้จะเพิ่มเมนู Direction Offset เข้ามาให้คุณสามารถหมุน PCS ได้อย่างอิสระตามท่วงท่า เพื่อการตรวจจับที่ดีขึ้น" },
            {"helpbox5","ตัวเลือกนี้จะสร้าง SPS Sockets สำหรับ PCS และเชื่อมเมนูของทั้งสองเข้าด้วยกันโดยอัตโนมัติ คุณจึงไม่จำเป็นต้องตั้งค่าเอง เหมาะอย่างยิ่งสำหรับคนที่กำลังเพิ่มระบบ SPS เป็นครั้งแรก" },
            {"helpbox6","คำเตือน! ตัวเลือกนี้จะลบเมนูสลับเสียงของ PCS ทั้งหมดและพารามิเตอร์ของมัน (pcs/menu/xxx) ควรใช้ก็ต่อเมื่อคุณต้องการควบคุมเมนูเสียงของ PCS ผ่าน Avatar Parameter Driver หรือ SPS Socket เท่านั้น"},
            {"helpbox7","โปรดลางและวางอวาตาร์ของคุณลงในช่อง เลื่อนเมาส์ไปที่ข้อความเพื่อดูคำอธิบาย" },
            {"helpbox_pos1","ตำแหน่งของ ปาก ไม่ได้เป็นส่วนหนึ่งของอวตารคุณ (มันอยู่นอก Prefab อวาตาร์)" },
            {"helpbox_pos2","ตำแหน่งของ ร่องนม ไม่ได้เป็นส่วนหนึ่งของอวตารคุณ (มันอยู่นอก Prefab อวาตาร์)" },
            {"helpbox_pos3","ตำแหน่งของ ช่องคลอด ไม่ได้เป็นส่วนหนึ่งของอวตารคุณ (มันอยู่นอก Prefab อวาตาร์)" },
            {"helpbox_pos4","ตำแหน่งของ ประตูหลัง ไม่ได้เป็นส่วนหนึ่งของอวตารคุณ (มันอยู่นอก Prefab อวาตาร์)" },

            //tooltip
            {"tooltip1", "พรีเซ็ตตำแหน่งการจัดวาง เลือก Generic ถ้าไม่มีพรีเซ็ตให้อวาตาร์ของคุณ เลือก Reference เพื่อกำหนดตำแหน่งเอง" },
            {"tooltip2", "ตัวเลือกนี้ใช้ปรับแรงที่ต้องใช้ในการกระแทก ยิ่งค่าต่ำลงก็จะใช้แรงเยอะขึ้น แนะนำให้ตั้งไว้ 1 นั่นแหละ เดี๋ยวจะปวดเอว" },
            {"tooltip3", "ตัวเลือกนี้จะทำให้คุณสามารถสะสมแต้มค่าความเงี่ยน (Lust) ได้จากการโดนตอก เมื่อค่าเต็มจะถึงจุดสุดยอดโดยอัตโนมัติ ตัวเลือกนี้ยังปลดล็อคระบบเสียงครางด้วยนะ" },
            {"tooltip4", "ตัวเลือกนี้ใช้ปรับตัวคูณของค่า Lust ยิ่งปรับเยอะ อวาตาร์จะเสร็จไวขึ้น" },

            {"confirm1", "ติดตั้ง PCS สำเร็จแล้ว!\n\nสิ่งที่คุณทำได้ต่อจากนี้คือการปรับตำแหน่งและหมุน PCS Target ให้ตรงตามตำแหน่งที่คุณต้องการ โปรดใช้ เมนูลัด เพื่อค้นหาตำแหน่งเสียงทั้งหมดที่ปรับได้" },
            {"confirm2", "คุณแน่ใจหรอว่าจะลบ PCS?" },

            {"error1", "ระบบหาโฟลเดอร์ของ PCS ห้ามย้ายโฟลเดอร์ชื่อ !Dismay Custom ไปไว้ที่อื่น แล้วก็อย่าใช้ตัวอักษรแปลกๆตั้งชื่ออวาตาร์ด้วย เช่นพวก (<>:\"|?*)" },

            //button
            {"quickAccess", "เมนูลัด" },
            {"button1", "ติดตั้งระบบ" },
            {"button2", "แทนที่ระบบเดิม" },
            {"button3", "ถอนการติดตั้ง" },
            {"button4", "แสดง/ซ่อนเครื่องหมาย" },
            {"button5", "เสกแท่งหรรษา (ใช้เทสระบบ)" },
            {"button6", "ระบุตำแหน่ง ปาก" },
            {"button7", "ระบุตำแหน่ง ร่องนม" },
            {"button8", "ระบุตำแหน่ง ช่องคลอด" },
            {"button9", "ระบุตำแหน่ง ประตูหลัง" },
        };
        private readonly Dictionary<string, string> korean = new()
        {
            {"alignment", "프리셋" },
            {"position", "위치" },
            {"mouth","입 " },
            {"boobs","가슴 " },
            {"pussy","보지 " },
            {"ass","엉덩이 " },
            {"mouth_pos","입 위치" },
            {"boobs_pos","가슴 위치" },
            {"pussy_pos","보지 위치" },
            {"ass_pos","엉덩이 위치" },
            {"findSPS","SPS Socket 찾기" },
            {"smashSensitivity","충돌 민감도" },
            {"customPos","사용자 정의 위치" },
            {"customTarget","타겟 번호" },
            {"menuName","메뉴 이름" },
            {"handjob","Handjob" },
            {"footjob","Footjob" },
            {"bothjob","둘 다" },
            {"lustFeat","Lust Feature (음성)" },
            {"lustMul","욕망 배수" },
            {"voicepack","음성 팩" },
            {"directionOffset","Direction Offset" },
            {"addSPS","SPS Socket 추가" },
            {"tooltip_sps","이 옵션을 사용하면 각 <PCS Target>에 SPS 소켓이 추가되어 PCS 사운드 메뉴와 함께 ON/OFF를 제어할 수 있습니다. 이는 PCS 메뉴를 SPS에 수동으로 연결하지 않고도 간편하게 설정할 수 있는 선택사항입니다.기존의 SPS 소켓을 직접 <PCS Target>에 배치할 수도 있으며, 이 경우에는 SPS 메뉴가 자동으로 생성되지 않도록 \"Enable Menu Toggle\" 옵션을 반드시 해제하세요." },
            {"advancedOption","팁: SPS 소켓으로 PCS 메뉴 제어하기" },
            {"localOnly","메뉴 전환 비활성화" },
            {"localOnly2","<color=red>사운드 전환 메뉴가 비활성화되었습니다." },

            //help box
            {"helpbox1","위치와 사운드를 최소한 하나 이상 선택해야 합니다. 그렇지 않으면 이 시스템은 쓸모가 없습니다!!" },
            {"helpbox2","참조 위치를 최소 하나 이상 지정해 주세요. 비활성화하려면 \"None\"으로 두세요." },
            {"helpbox3","모든 커스텀 타겟과 메뉴 이름을 지정해야 합니다. 원하는 위치로 설정할 수 있습니다." },
            {"helpbox4","이 옵션은 Direction Offset 메뉴를 추가합니다. 이 원형 메뉴를 통해 PCS를 관통 방향에 맞춰 회전시켜 더 정확한 감지를 가능하게 합니다." },
            {"helpbox5","이 옵션을 선택하면 PCS용 SPS 소켓이 자동으로 생성되고 두 메뉴가 자동으로 연결됩니다. 수동으로 설정할 필요가 없습니다. SPS를 처음 설정할 때 유용합니다." },
            {"helpbox6","경고! 이 옵션은 PCS 사운드 전환 메뉴와 해당 매개변수(pcs/menu/xxx)를 모두 제거합니다. PCS 사운드 메뉴를 Avatar Parameter Driver 또는 SPS Socket으로 제어하려는 경우에만 사용하세요."},
            {"helpbox7","아바타를 상자에 드래그 앤 드롭하세요. 텍스트 위에 마우스를 올리면 설명이 표시됩니다." },
            {"helpbox_pos1","입 참조 타겟이 아바타의 자식 gameObject가 아닙니다." },
            {"helpbox_pos2","가슴 참조 타겟이 아바타의 자식 gameObject가 아닙니다." },
            {"helpbox_pos3","보지 참조 타겟이 아바타의 자식 gameObject가 아닙니다." },
            {"helpbox_pos4","엉덩이 참조 타겟이 아바타의 자식 gameObject가 아닙니다." },

            //tooltip
            {"tooltip1", "일부 아바타에 대한 배치 프리셋입니다. 선택한 아바타에 프리셋이 없으면 \"Generic\"을 선택하세요. 선호하는 위치에 배치하려면 \"Reference\"를 선택하세요." },
            {"tooltip2", "이 옵션을 통해 충돌 감지의 민감도를 조절할 수 있습니다. 값이 낮을수록 더 강한 관통이 있어야 사운드가 재생됩니다. 권장 값은 1입니다 :)" },
            {"tooltip3", "이 기능은 삽입될 때 욕정 수치를 증가시키며, 욕정이 가득 차면 신음하고 사정하게 만듭니다." },
            {"tooltip4", "이 값을 조절하여 관통 스트로크당 1포인트의 욕망 수치를 얼마나 빨리 얻을지를 결정할 수 있습니다. 이 값을 높이면 더 빨리 클라이맥스에 도달합니다." },

            {"confirm1", "설정 완료. PCS가 설치되었습니다!\n\n이제 할 수 있는 것은 PCS 타겟의 위치와 회전을 원하는 위치에 맞게 조정하는 것입니다. 조정 가능한 모든 음성 위치를 찾으려면 빠른 액세스 메뉴를 사용하세요." },
            {"confirm2", "이 기믹을 정말로 제거하시겠습니까?" },

            {"error1", "PCS 리소스 폴더를 찾을 수 없습니다. !Dismay Custom 폴더를 이동하지 마시고, 아바타 이름에 (<>:\"|?*) 같은 잘못된 문자를 사용하지 마세요." },

            //button
            {"quickAccess", "빠른 액세스" },
            {"button1", "적용" },
            {"button2", "교체" },
            {"button3", "제거" },
            {"button4", "배치 아이콘 숨기기/표시" },
            {"button5", "테스트용 삽입기 생성" },
            {"button6", "입 위치 찾기" },
            {"button7", "가슴 위치 찾기" },
            {"button8", "질 위치 찾기" },
            {"button9", "엉덩이 위치 찾기" },
        };
        private readonly Dictionary<string, string> chinese = new()
        {
            {"alignment", "对齐预设"},
            {"position", "位置与音效"},
            {"mouth", "口部"},
            {"boobs", "上半身"},
            {"pussy", "下半身"},
            {"ass", "后部"},
            {"mouth_pos", "口部位置"},
            {"boobs_pos", "上半身位置"},
            {"pussy_pos", "下半身位置"},
            {"ass_pos", "后部位置"},
            {"findSPS", "查找 SPS 插槽"},
            {"smashSensitivity", "冲击灵敏度"},
            {"customPos", "自定义位置"},
            {"customTarget", "目标编号"},
            {"menuName", "菜单名称"},
            {"handjob", "手部动作"},
            {"footjob", "脚部动作"},
            {"bothjob", "双重动作"},
            {"lustFeat", "情绪功能（语音）"},
            {"lustMul", "情绪倍率"},
            {"voicepack", "语音包"},
            {"directionOffset", "方向偏移"},
            {"addSPS", "添加 SPS 插槽"},
            {"tooltip_sps", "此选项会为每个 <PCS 目标> 添加一个 SPS 插槽，使其可以与 PCS 声音菜单一起开关。这是将 PCS 菜单与 SPS 连接的便捷方式，也可以手动在每个 <PCS 目标> 中放置现有的 SPS 插槽。如果使用手动方式，请取消勾选 “启用菜单切换”，以防止自动生成 SPS 菜单。"},
            {"advancedOption", "提示：如何将 PCS 菜单与 SPS 插槽链接"},
            {"localOnly", "移除菜单切换"},
            {"localOnly2", "<color=red>声音切换菜单已被禁用。</color>"},

            // help box
            {"helpbox1", "你必须至少选择一个位置和一个声音，否则此系统将无法工作！"},
            {"helpbox2", "请至少指定一个参考位置。如果想禁用，请保持为 “无”。"},
            {"helpbox3", "请为所有自定义目标和菜单名称分配值。你可以随意设置它们的位置。"},
            {"helpbox4", "此选项会添加一个方向偏移菜单。该径向菜单允许你旋转 PCS，以与检测方向对齐，实现更准确的检测。"},
            {"helpbox5", "此选项会自动为 PCS 创建 SPS 插槽并将其菜单关联在一起，无需手动设置。首次配置 SPS 时非常有用。"},
            {"helpbox6", "警告！此选项将移除所有 PCS 声音菜单开关及其参数（pcs/menu/xxx）。仅当你打算通过 Avatar Parameter Driver 或 SPS 插槽控制声音菜单时使用此选项。"},
            {"helpbox7", "请将你的头像拖入方框中。将鼠标悬停在文字上即可查看说明。"},
            {"helpbox_pos1", "口部参考目标不是你的头像的子对象。"},
            {"helpbox_pos2", "上半身参考目标不是你的头像的子对象。"},
            {"helpbox_pos3", "下半身参考目标不是你的头像的子对象。"},
            {"helpbox_pos4", "后部参考目标不是你的头像的子对象。"},

            // tooltip
            {"tooltip1", "用于部分头像的放置预设。如果所选头像没有预设，请选择通用模式（Generic）。若想手动指定位置，请选择参考模式（Reference）。"},
            {"tooltip2", "此选项可调整冲击检测的灵敏度。值越低，需要更强的动作才能触发声音。建议值为 1。"},
            {"tooltip3", "该功能可根据互动获得情绪值，并在情绪值满时触发特定语音效果。"},
            {"tooltip4", "调整此项以决定获得 1 点情绪值的速度。值越高，积累速度越快。"},

            // confirm
            {"confirm1", "设置完成。PCS 已安装！\n\n接下来你可以调整 PCS 目标的位置与旋转，以匹配所需的位置。请使用快速访问菜单来查看所有可调语音位置。"},
            {"confirm2", "确定要移除此功能吗？"},

            // error
            {"error1", "未找到 PCS 资源文件夹。请勿移动 !Dismay Custom 文件夹，并避免在头像名称中使用无效字符（如 <>:\"|?*）。"},

            // button
            {"quickAccess", "快速访问"},
            {"button1", "应用"},
            {"button2", "替换"},
            {"button3", "移除"},
            {"button4", "显示/隐藏放置图标"},
            {"button5", "生成测试物件"},
            {"button6", "定位口部"},
            {"button7", "定位上半身"},
            {"button8", "定位下半身"},
            {"button9", "定位后部"}
        };

        private string L(string key)
        {
            return currentLanguage switch
            {
                Language.English => english.TryGetValue(key, out var valEng) ? valEng : key,
                Language.日本語 => japanese.TryGetValue(key, out var valJpn) ? valJpn : key,
                Language.ภาษาไทย => thai.TryGetValue(key, out var valTha) ? valTha : key,
                Language.한국어 => korean.TryGetValue(key, out var valKor) ? valKor : key,
                Language.简体中文 => chinese.TryGetValue(key, out var valCn) ? valCn : key,
                _ => key
            };
        }

        #endregion

        [MenuItem("Dismay Custom/Penetration Contact System/Setup Tool")]
        #region Functions

        public static void ShowpWindow()
        {
            var window = GetWindow(typeof(PCSConfigurator));

            window.titleContent = new GUIContent("Penetration Contact System");
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
        private void OnGUI()
        {
            Color backgroundColor = new(0.031f, 0.031f, 0.031f);
            EditorGUI.DrawRect(new(0, 0, position.width, 115), backgroundColor);

            rightAlignedStyle = new(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleRight
            };

            paramStyle = new GUIStyle()
            {
                fontSize = 10,
                fontStyle = FontStyle.Normal,
                normal = { textColor = new Color(0.7f, 0.7f, 0.7f) },
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
            GUILayout.Label(logo, new GUIStyle { fixedWidth = 512, fixedHeight = 115 });
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            targetAvatar = EditorGUILayout.ObjectField(targetAvatar, typeof(VRCAvatarDescriptor), true, GUILayout.Height(30)) as VRCAvatarDescriptor;

            if (targetAvatar)
            {
                //Getting avatar properties
                animator = targetAvatar.GetComponent<Animator>();
                var prefab = targetAvatar.transform.Find(thisGimmick);

                scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);
                ShowMenuList();
                GUILayout.EndScrollView();

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

            PCSPrefabProcess.ShowInstaller();

            EditorGUILayout.BeginHorizontal();
            if (preset == Preset.Reference)
            {
                preset = (Preset)EditorGUILayout.EnumPopup(new GUIContent(L("alignment"), L("tooltip1")), preset);
                if (GUILayout.Button(L("findSPS"), GUILayout.Width(130)))
                {
                    spawnSPSsocket = false;

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
                preset = (Preset)EditorGUILayout.EnumPopup(new GUIContent(L("alignment"), L("tooltip1")), preset);
            }
            EditorGUILayout.EndHorizontal();

            if (preset != Preset.Reference)
            {
                flag2 = true;

                GUILayout.BeginVertical("ProgressBarBack");

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(L("position"), GUILayout.MaxWidth(150));
                EditorGUILayout.LabelField(L("mouth"), rightAlignedStyle, GUILayout.MaxWidth(60));
                useMouth = EditorGUILayout.Toggle(useMouth, GUILayout.MaxWidth(30));
                EditorGUILayout.LabelField(L("boobs"), rightAlignedStyle, GUILayout.MaxWidth(60));
                useBoobs = EditorGUILayout.Toggle( useBoobs, GUILayout.MaxWidth(30));
                EditorGUILayout.LabelField(L("pussy"), rightAlignedStyle, GUILayout.MaxWidth(60));
                usePussy = EditorGUILayout.Toggle(usePussy, GUILayout.MaxWidth(30));
                EditorGUILayout.LabelField(L("ass"), rightAlignedStyle, GUILayout.MaxWidth(60));
                useAss = EditorGUILayout.Toggle(useAss, GUILayout.MaxWidth(30));
                GUILayout.EndHorizontal();

                if (!useMouth && !useBoobs && !usePussy && !useAss)
                {
                    EditorGUILayout.HelpBox(L("helpbox1"), MessageType.Warning);
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
                ref_mouth = EditorGUILayout.ObjectField(L("mouth_pos"), ref_mouth, typeof(GameObject), true) as GameObject;
                ref_boobs = EditorGUILayout.ObjectField(L("boobs_pos"), ref_boobs, typeof(GameObject), true) as GameObject;
                ref_pussy = EditorGUILayout.ObjectField(L("pussy_pos"), ref_pussy, typeof(GameObject), true) as GameObject;
                ref_ass = EditorGUILayout.ObjectField(L("ass_pos"), ref_ass, typeof(GameObject), true) as GameObject;
                GUILayout.EndVertical();

                bool[] check_pass = new bool[5];
                if (ref_mouth == null && ref_boobs == null && ref_pussy == null && ref_ass == null) //If all slots are empty
                {
                    EditorGUILayout.HelpBox(L("helpbox2"), MessageType.Info);
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
                        EditorGUILayout.HelpBox(L("helpbox_pos1"), MessageType.Warning);
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
                        EditorGUILayout.HelpBox(L("helpbox_pos2"), MessageType.Warning);
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
                        EditorGUILayout.HelpBox(L("helpbox_pos3"), MessageType.Warning);
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
                        EditorGUILayout.HelpBox(L("helpbox_pos4"), MessageType.Warning);
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

            GUI.color = new Color(0.8f, 0.8f, 0.8f, 1);
            smashSensitivity = EditorGUILayout.Slider(new GUIContent(L("smashSensitivity"), L("tooltip2")), smashSensitivity, 0.1f, 1);
            smashSensitivity = Mathf.Round(smashSensitivity * Mathf.Pow(10, 1)) / Mathf.Pow(10, 1);
            GUI.color = Color.white;

            GUILayout.BeginHorizontal();
            selected_customPos = EditorGUILayout.IntPopup(L("customPos"), selected_customPos, customPos_choiceName, customPos_sizes);
            GUILayout.EndHorizontal();

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

                GUILayout.BeginHorizontal();
                //Find custom preset
                if (GUILayout.Button(L("handjob")))
                {
                    selected_customPos = 2;
                    var handL = animator.GetBoneTransform(HumanBodyBones.LeftHand);
                    var handR = animator.GetBoneTransform(HumanBodyBones.RightHand);
                    ref_soundPosition[0] = handL.gameObject;
                    ref_soundPosition[1] = handR.gameObject;
                    customPos_menuName[0] = "Left Hand";
                    customPos_menuName[1] = "Right Hand";

                    GameObject find_hjL, find_hjR;

                    find_hjL = GameObject.Find("SPS/Handjob/Handjob Left");
                    find_hjR = GameObject.Find("SPS/Handjob/Handjob Right");

                    if (find_hjL != null && find_hjL.transform.IsChildOf(targetAvatar.transform))
                    {
                        ref_soundPosition[0] = find_hjL;
                    }
                    if (find_hjR != null && find_hjR.transform.IsChildOf(targetAvatar.transform))
                    {
                        ref_soundPosition[1] = find_hjR;
                    }
                }
                if (GUILayout.Button(L("footjob")))
                {
                    selected_customPos = 2;
                    var footL = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
                    var footR = animator.GetBoneTransform(HumanBodyBones.RightFoot);
                    ref_soundPosition[0] = footL.gameObject;
                    ref_soundPosition[1] = footR.gameObject;
                    customPos_menuName[0] = "Left Foot";
                    customPos_menuName[1] = "Right Foot";

                    GameObject find_fjL, find_fjR;

                    find_fjL = GameObject.Find("SPS/Feet/Footjob/Footjob Target Left");
                    find_fjR = GameObject.Find("SPS/Feet/Footjob/Footjob Target Right");

                    if (find_fjL != null && find_fjL.transform.IsChildOf(targetAvatar.transform))
                    {
                        ref_soundPosition[0] = find_fjL;
                    }
                    if (find_fjR != null && find_fjR.transform.IsChildOf(targetAvatar.transform))
                    {
                        ref_soundPosition[1] = find_fjR;
                    }
                }
                if (GUILayout.Button(L("bothjob")))
                {
                    selected_customPos = 4;
                    var handL = animator.GetBoneTransform(HumanBodyBones.LeftHand);
                    var handR = animator.GetBoneTransform(HumanBodyBones.RightHand);
                    ref_soundPosition[0] = handL.gameObject;
                    ref_soundPosition[1] = handR.gameObject;
                    customPos_menuName[0] = "Left Hand";
                    customPos_menuName[1] = "Right Hand";

                    GameObject find_hjL, find_hjR;

                    find_hjL = GameObject.Find("SPS/Handjob/Handjob Left");
                    find_hjR = GameObject.Find("SPS/Handjob/Handjob Right");

                    if (find_hjL != null && find_hjL.transform.IsChildOf(targetAvatar.transform))
                    {
                        ref_soundPosition[0] = find_hjL;
                    }
                    if (find_hjR != null && find_hjR.transform.IsChildOf(targetAvatar.transform))
                    {
                        ref_soundPosition[1] = find_hjR;
                    }

                    var footL = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
                    var footR = animator.GetBoneTransform(HumanBodyBones.RightFoot);
                    ref_soundPosition[2] = footL.gameObject;
                    ref_soundPosition[3] = footR.gameObject;
                    customPos_menuName[2] = "Left Foot";
                    customPos_menuName[3] = "Right Foot";

                    GameObject find_fjL, find_fjR;

                    find_fjL = GameObject.Find("SPS/Feet/Footjob/Footjob Target Left");
                    find_fjR = GameObject.Find("SPS/Feet/Footjob/Footjob Target Right");

                    if (find_fjL != null && find_fjL.transform.IsChildOf(targetAvatar.transform))
                    {
                        ref_soundPosition[2] = find_fjL;
                    }
                    if (find_fjR != null && find_fjR.transform.IsChildOf(targetAvatar.transform))
                    {
                        ref_soundPosition[3] = find_fjR;
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
                if (flag3 == false)
                {
                    EditorGUILayout.HelpBox(L("helpbox3"), MessageType.Warning);
                }
            }

            GUILayout.BeginVertical("ProgressBarBack");
            lustFeature = EditorGUILayout.Toggle(new GUIContent(L("lustFeat"), L("tooltip3")), lustFeature);
            if (lustFeature)
            {
                
                GUI.color = new Color(0.8f, 0.8f, 0.8f, 1);
                lustMultiplierValue = EditorGUILayout.Slider(new GUIContent(L("lustMul"), L("tooltip4")), lustMultiplierValue, 0.1f, 1);
                GUI.color = Color.white;
                lustMultiplierValue = Mathf.Round(lustMultiplierValue * Mathf.Pow(10, 1)) / Mathf.Pow(10, 1);

                selectedIndex = EditorGUILayout.Popup(L("voicepack"), selectedIndex, customEnumStrings);
                voicePack = (VoicePack)selectedIndex;            
            }
            GUILayout.EndVertical();

            useDirectionOffset = EditorGUILayout.Toggle(new GUIContent(L("directionOffset")), useDirectionOffset);
            if (useDirectionOffset)
            {
                EditorGUILayout.HelpBox(L("helpbox4"), MessageType.Info);
            }

            spawnSPSsocket = EditorGUILayout.Toggle(new GUIContent(L("addSPS"), ""), spawnSPSsocket);
            if (spawnSPSsocket)
            {
                EditorGUILayout.HelpBox(L("helpbox5"), MessageType.Info);
            }

            setLocal = EditorGUILayout.Toggle(L("localOnly"), setLocal);
            if (setLocal)
            {
                EditorGUILayout.HelpBox(L("helpbox6"), MessageType.Warning);
            }

            /*advancedTab = EditorGUILayout.Foldout(advancedTab, L("advancedOption"));
            if (advancedTab)
            {
                EditorGUILayout.HelpBox(L("helpbox6"), MessageType.Info);
            }
            */

            //end of menu list
        }
        private void ShowSourceSetup(int slot)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(L("customTarget") + (slot), GUILayout.MaxWidth(90));

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

            EditorGUILayout.LabelField(L("menuName"), GUILayout.MaxWidth(80));
            customPos_menuName[slot - 1] = EditorGUILayout.TextField(customPos_menuName[slot - 1]);
            EditorGUILayout.EndHorizontal();
        }
        private void ShowQuickMenu()
        {
            GUI.color = new Color32(200, 200, 200, 255);
            //ADD QUICK ACCESS!
            var prefab = targetAvatar.transform.Find(thisGimmick);
            if (prefab != null)
            {
                EditorGUILayout.BeginVertical("ProgressBarBack");              

                EditorGUILayout.LabelField(L("quickAccess"));

                //Line1
                GUI.enabled = true;
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(L("button4"),GUILayout.Width(246)))
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
                if (GUILayout.Button(L("button5"), GUILayout.Width(246)))
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
                if (GUILayout.Button(L("button6"), GUILayout.Width(122)))
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
                if (GUILayout.Button(L("button7"), GUILayout.Width(122)))
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
                if (GUILayout.Button(L("button8"), GUILayout.Width(122)))
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
                if (GUILayout.Button(L("button9"), GUILayout.Width(122)))
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
            GUI.color = new Color32(255, 255, 255, 255);
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
                if (GUILayout.Button(L("button1")))
                {
                    Apply(true);
                }

                GUI.enabled = false;
                if (GUILayout.Button(L("button3")))
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
                if (GUILayout.Button(L("button2")))
                {
                    Remove(false);
                    Apply(false);
                }

                GUI.enabled = true;
                if (GUILayout.Button(L("button3")))
                {
                    Remove(true);
                }
            }
            GUI.color = new Color32(255, 255, 255, 255);
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
                case Preset.Reference:
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

                case Preset.Generic:
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

                case Preset.Milltina:
                    mouth.transform.position = new Vector3(0, 1.02f, 0.06f);
                    mouth.transform.eulerAngles = new Vector3(20, 0, 0);
                    boobs.transform.position = new Vector3(0, 0.83f, 0.095f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.59f, 0.024f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.6f, -0.0255f);
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

                case Preset.RunaRobotic:
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

                case Preset.Ichigo:
                    mouth.transform.position = new Vector3(0, 1.005f, 0.06f);
                    mouth.transform.eulerAngles = new Vector3(15, 0, 0);
                    boobs.transform.position = new Vector3(0, 0.84f, 0.1f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.635f, 0.025f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.64f, -0.01f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Uruki:
                    mouth.transform.position = new Vector3(0, 1.135f, 0.085f);
                    mouth.transform.eulerAngles = new Vector3(15, 0, 0);
                    boobs.transform.position = new Vector3(0, 0.96f, 0.11f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.675f, 0.018f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.686f, -0.02f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Chocolat:
                    mouth.transform.position = new Vector3(0, 1.006f, 0.034f);
                    mouth.transform.eulerAngles = new Vector3(15, 0, 0);
                    boobs.transform.position = new Vector3(0, 0.84f, 0.06f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.585f, -0.008f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.593f, -0.034f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Mafuyu:
                    mouth.transform.position = new Vector3(0, 1.135f, 0.0665f);
                    mouth.transform.eulerAngles = new Vector3(15, 0, 0);
                    boobs.transform.position = new Vector3(0, 0.955f, 0.1f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.675f, 0.0165f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.678f, -0.0191f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;

                case Preset.Milfy:
                    mouth.transform.position = new Vector3(0, 1.08f, 0.064f);
                    mouth.transform.eulerAngles = new Vector3(15, 0, 0);
                    boobs.transform.position = new Vector3(0, 0.915f, 0.105f);
                    boobs.transform.eulerAngles = new Vector3(80, 0, 0);
                    pussy.transform.position = new Vector3(0, 0.643f, 0.0185f);
                    pussy.transform.eulerAngles = new Vector3(90, 0, 0);
                    ass.transform.position = new Vector3(0, 0.65f, -0.0145f);
                    ass.transform.eulerAngles = new Vector3(100, 0, 0);
                    break;
            }
        }
        private void Remove(bool confirm)
        {
            if (confirm)
            {
                if (EditorUtility.DisplayDialog(thisGimmick, L("confirm2"), "Yes", "No"))
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
        private int CalculateParamUse()
        {
            int defaultUsage = 9;
            int useLust;
            int useVoice;
            int useDioffset;
            int result;

            if (lustFeature)
            {
                useLust = 10;
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

            if (useDirectionOffset)
            {
                useDioffset = 8;
            }
            else
            {
                useDioffset = 0;
            }

            if (!setLocal)
            {
                if (preset != Preset.Reference)
                {
                    result = defaultUsage + useLust + useVoice + useDioffset + Convert.ToInt32(useMouth) + Convert.ToInt32(useBoobs) + Convert.ToInt32(usePussy) + Convert.ToInt32(useAss) + selected_customPos;
                    return result;
                }
                else
                {
                    result = defaultUsage + useLust + useVoice + useDioffset + Convert.ToInt32(ref_mouth) + Convert.ToInt32(ref_boobs) + Convert.ToInt32(ref_pussy) + Convert.ToInt32(ref_ass) + selected_customPos;
                    return result;
                }
            }
            else
            {
                if (preset != Preset.Reference)
                {
                    result = defaultUsage + useLust + useVoice + useDioffset + selected_customPos;
                    return result;
                }
                else
                {
                    result = defaultUsage + useLust + useVoice + useDioffset + selected_customPos;
                    return result;
                }
            }
        }
        private void ShowParameter()
        {
            EditorGUILayout.Space();

            int paramCost;
            paramCost = CalculateParamUse();
            if (setLocal)
            {
                GUILayout.Label("  Memory Usage: <color=cyan>" + paramCost + "</color>", paramStyle, GUILayout.Width(495));
            }
            else
            {
                GUILayout.Label("  Memory Usage: <color=lime>" + paramCost + "</color>", paramStyle, GUILayout.Width(495));
            }
        }
        private void ShowFooter()
        {
            Rect enumRect = new(position.width - 75 - 10,10,75,18);

            currentLanguage = (Language)EditorGUI.EnumPopup(enumRect, currentLanguage, rightAlignedStyle);

            if (!targetAvatar)
            {
                EditorGUILayout.HelpBox(L("helpbox7"), MessageType.Warning);

                var upadate = Resources.Load<TextAsset>("Components/" + thisGimmick + "_update").ToString();
                var lines = upadate.Split('\n');
                var line1 = lines[0];
               //EditorGUILayout.HelpBox(line1, MessageType.Info);
            }

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            var info = Resources.Load<TextAsset>("Components/" + thisGimmick + "_info").ToString();
            GUILayout.Label(info.Replace("$", "v" + version), infoStyle, GUILayout.Width(285));

            if (GUILayout.Button("Tutorial"))
            {
                Application.OpenURL("https://youtube.com/playlist?list=PLEvAOTfSR8u2fdM_HnFtkXuaqvAEp2WtS&si=LIWgV0EezFQOCnJN");
            }
            if (GUILayout.Button("Discord"))
            {
                Application.OpenURL("https://discord.gg/TkfRyQDNQC");
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.Space(5);
        }

        #endregion

        #region Applying Functions
        private void Apply(bool confirm)
        {
            var head = animator.GetBoneTransform(HumanBodyBones.Head);
            var hips = animator.GetBoneTransform(HumanBodyBones.Hips);

            hidePlacement = true;

            //Copy prefab
            Vector3 tempScale, tempPosition;
            tempScale = targetAvatar.transform.localScale;
            tempPosition = targetAvatar.transform.localPosition;
            targetAvatar.transform.localScale = new Vector3(1, 1, 1);
            targetAvatar.transform.localPosition = new Vector3(0, 0, 0);

            if (PCSPrefabProcess.installer == PCSPrefabProcess.Installer.ModularAvatar)
            {
                GameObject x = Instantiate(Resources.Load<GameObject>("Main Prefab/PCS MA Prefab")) as GameObject;
                x.name = thisGimmick;
                x.transform.parent = targetAvatar.transform;
            }
            else
            {
                GameObject x = Instantiate(Resources.Load<GameObject>("Main Prefab/PCS VF Prefab")) as GameObject;
                x.name = thisGimmick;
                x.transform.parent = targetAvatar.transform;
            }

            var y = "Target Objects";   

            var mouth = targetAvatar.transform.Find(thisGimmick + "/" + y + "/<PCS Target> Mouth").gameObject;
            var boobs = targetAvatar.transform.Find(thisGimmick + "/" + y + "/<PCS Target> Boobs").gameObject;
            var pussy = targetAvatar.transform.Find(thisGimmick + "/" + y + "/<PCS Target> Pussy").gameObject;
            var ass = targetAvatar.transform.Find(thisGimmick + "/" + y + "/<PCS Target> Ass").gameObject;
            var pcsContact = targetAvatar.transform.Find(thisGimmick + "/PCS Contacts");
            VRCParentConstraint vRCParentConstraint = pcsContact.GetComponent<VRCParentConstraint>();
            vRCParentConstraint.IsActive = true;

            if (spawnSPSsocket)
            {
                if (preset != Preset.Reference)
                {
                    if (useMouth)
                    {
                        GameObject socket = Instantiate(Resources.Load<GameObject>("Main Prefab/SPS Sockets/Mouth Socket")) as GameObject;
                        socket.name = "Mouth Socket";
                        socket.transform.parent = mouth.transform;
                    }
                    if (useBoobs)
                    {
                        GameObject socket = Instantiate(Resources.Load<GameObject>("Main Prefab/SPS Sockets/Boobs Socket")) as GameObject;
                        socket.name = "Boobs Socket";
                        socket.transform.parent = boobs.transform;
                    }
                    if (usePussy)
                    {
                        GameObject socket = Instantiate(Resources.Load<GameObject>("Main Prefab/SPS Sockets/Pussy Socket")) as GameObject;
                        socket.name = "Pussy Socket";
                        socket.transform.parent = pussy.transform;
                    }
                    if (useAss)
                    {
                        GameObject socket = Instantiate(Resources.Load<GameObject>("Main Prefab/SPS Sockets/Ass Socket")) as GameObject;
                        socket.name = "Ass Socket";
                        socket.transform.parent = ass.transform;
                    }
                }
                else
                {
                    if(mouth != null)
                    {
                        GameObject socket = Instantiate(Resources.Load<GameObject>("Main Prefab/SPS Sockets/Mouth Socket")) as GameObject;
                        socket.name = "Mouth Socket";
                        socket.transform.parent = mouth.transform;
                    }
                    if (boobs != null)
                    {
                        GameObject socket = Instantiate(Resources.Load<GameObject>("Main Prefab/SPS Sockets/Boobs Socket")) as GameObject;
                        socket.name = "Boobs Socket";
                        socket.transform.parent = boobs.transform;
                    }
                    if (pussy != null)
                    {
                        GameObject socket = Instantiate(Resources.Load<GameObject>("Main Prefab/SPS Sockets/Pussy Socket")) as GameObject;
                        socket.name = "Pussy Socket";
                        socket.transform.parent = pussy.transform;
                    }
                    if (ass != null)
                    {
                        GameObject socket = Instantiate(Resources.Load<GameObject>("Main Prefab/SPS Sockets/Ass Socket")) as GameObject;
                        socket.name = "Ass Socket";
                        socket.transform.parent = ass.transform;
                    }
                }
            }

            //Fix Parent Constraint slot for guide targets
            GameObject guide_mouth, guide_boobs, guide_pussy, guide_ass, guide_squirt;
            GameObject[] guide_custom = new GameObject[8];
            VRCParentConstraint guide_mouth_p, guide_boobs_p, guide_pussy_p, guide_ass_p, guide_squirt_p;
            VRCParentConstraint[] guide_custom_p = new VRCParentConstraint[8];

            guide_mouth = targetAvatar.transform.Find(thisGimmick + "/Target Objects/Placement Icons (Auto Remove)/Mouth").gameObject;
            guide_boobs = targetAvatar.transform.Find(thisGimmick + "/Target Objects/Placement Icons (Auto Remove)/Boobs").gameObject;
            guide_pussy = targetAvatar.transform.Find(thisGimmick + "/Target Objects/Placement Icons (Auto Remove)/Pussy").gameObject;
            guide_ass = targetAvatar.transform.Find(thisGimmick + "/Target Objects/Placement Icons (Auto Remove)/Ass").gameObject;
            guide_squirt = targetAvatar.transform.Find(thisGimmick + "/Target Objects/Placement Icons (Auto Remove)/Squirt").gameObject;
            for (int i = 0; i < 8; i++)
            {
                guide_custom[i] = targetAvatar.transform.Find(thisGimmick + "/Target Objects/Placement Icons (Auto Remove)/Custom (" + (i+1) + ")").gameObject;
            }

            guide_mouth_p = guide_mouth.GetComponent<VRCParentConstraint>();
            guide_boobs_p = guide_boobs.GetComponent<VRCParentConstraint>();
            guide_pussy_p = guide_pussy.GetComponent<VRCParentConstraint>();
            guide_ass_p = guide_ass.GetComponent<VRCParentConstraint>();
            guide_squirt_p = guide_squirt.GetComponent<VRCParentConstraint>();
            for (int i = 0; i < 8; i++)
            {
                guide_custom_p[i] = guide_custom[i].GetComponent<VRCParentConstraint>();
            }

            VRCConstraintSource guide_mouth_s = new()
            {
                Weight = 1,
                SourceTransform = mouth.transform,
            };
            VRCConstraintSource guide_boobs_s = new()
            {
                Weight = 1,
                SourceTransform = boobs.transform,
            };
            VRCConstraintSource guide_pussy_s = new()
            {
                Weight = 1,
                SourceTransform = pussy.transform,
            };
            VRCConstraintSource guide_ass_s = new()
            {
                Weight = 1,
                SourceTransform = ass.transform,
            };
            var guide_squirt_a = targetAvatar.transform.Find(thisGimmick + "/<PCS Particle> Squirt").gameObject;
            VRCConstraintSource guide_squirt_s = new()
            {
                Weight = 1,
                SourceTransform = guide_squirt_a.transform,
            };

            GameObject[] custom =new GameObject[8];

            for(int i = 0; i < custom.Length; i++)
            {
                custom[i] = targetAvatar.transform.Find(thisGimmick + "/" + y + "/<PCS Target> Custom ("+ (i+1) + ")").gameObject;
            }

            VRCConstraintSource[] guide_custom_s = new VRCConstraintSource[8];
            for(int i = 0;i < guide_custom_s.Length; i++)
            {
                guide_custom_s[i] = new()
                {
                    Weight = 1,
                    SourceTransform = custom[i].transform,
                };
            }
        
            guide_mouth_p.Sources.Add(guide_mouth_s);
            guide_boobs_p.Sources.Add(guide_boobs_s);
            guide_pussy_p.Sources.Add(guide_pussy_s);
            guide_ass_p.Sources.Add(guide_ass_s);
            guide_squirt_p.Sources.Add(guide_squirt_s);

            for(int i = 0; i < 8; i++)
            {
                guide_custom_p[i].Sources.Add(guide_custom_s[i]);
            }

            //Expressions
            GenerateFolder();
            GenerateMenu();
            GenerateParameter();

            //Copy Main FX to gen folder
            string folderPath = "Assets/!Dismay Custom/Penetration Contact System/#GENERATED/" + targetAvatar.name;

            var mainfx = Resources.Load("FX Controller/PCS - Main Controller") as AnimatorController;
            AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(mainfx), folderPath + "/!PCS Controller_" + targetAvatar.name + ".asset");

            //Avatar tools process
            var PCS = targetAvatar.transform.Find("Penetration Contact System").gameObject;
            var menu = (VRCExpressionsMenu)AssetDatabase.LoadAssetAtPath(folderPath + "/!Install Menu_" +targetAvatar.name + ".asset", typeof(VRCExpressionsMenu));
            var param = (VRCExpressionParameters)AssetDatabase.LoadAssetAtPath(folderPath + "/!Install Parameter_" + targetAvatar.name + ".asset", typeof(VRCExpressionParameters));
            var PCSCtrl = (AnimatorController)AssetDatabase.LoadAssetAtPath(folderPath + "/!PCS Controller_" + targetAvatar.name + ".asset", typeof(AnimatorController));
            var PCSDirect = Resources.Load("FX Controller/PCS - Direct Blendtree") as AnimatorController;

            if (lustFeature == false)
            {
                voicePack = VoicePack.Disable;
            }

            PCSPrefabProcess.AddGeneratedAssetToPrefab(PCS, PCSCtrl, menu, param, PCSDirect);
            
            //Voice pack
            var voiceObj = targetAvatar.transform.Find(thisGimmick + "/Voice Pack").gameObject;
            var voiceAudio = targetAvatar.transform.Find(thisGimmick + "/Voice Pack/Audio Source").gameObject;
            voiceObj.transform.position = head.position;

            if (voicePack == VoicePack.Disable || !lustFeature)
            {
                var voice_spatial = voiceAudio.GetComponent(typeof(VRCSpatialAudioSource));
                DestroyImmediate(voice_spatial);

                var voice_audio = voiceAudio.GetComponent(typeof(AudioSource));
                DestroyImmediate(voice_audio);

                voiceObj.tag = "EditorOnly";
            }

            AlignmentPreset(mouth.transform, boobs.transform, pussy.transform, ass.transform);
            targetAvatar.transform.localScale = tempScale;
            targetAvatar.transform.localPosition = tempPosition;

            if (voicePack != VoicePack.Disable)
            {
                PCSAudioManager.FinalizeVoicePack(PCSCtrl);
            }

            //Place Custom Position
            Transform[] customPos = new Transform[9];
            if (selected_customPos != 0)
            {
                VRCConstraintSource[] src_custom = new VRCConstraintSource[8];
                GameObject[] socket = new GameObject[8];
                for (int i = 0; i < selected_customPos; i++)
                {
                    customPos[i] = targetAvatar.transform.Find(thisGimmick + "/" + y + "/<PCS Target> Custom (" + (i + 1) + ")");
                    customPos[i].transform.position = ref_soundPosition[i].transform.position;
                    customPos[i].transform.parent = ref_soundPosition[i].transform;
                    customPos[i].transform.localPosition = Vector3.zero;
                    customPos[i].transform.localEulerAngles = Vector3.zero;
                    Debug.Log("PCS Custom Location #" + (i + 1) + " has been placed under <" + SearchUtils.GetHierarchyPath(ref_soundPosition[i]) + ">");                  

                    if (spawnSPSsocket)
                    {
                        socket[i] = GameObject.Instantiate(Resources.Load<GameObject>("Main Prefab/SPS Sockets/Custom Socket " + (i + 1))) as GameObject;
                        socket[i].name = "Custom Socket " + (i+1);
                        socket[i].transform.parent = customPos[i].transform;
                        socket[i].transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                    }
                }
            }

            //Remove Custom Position
            Transform[] target = new Transform[9];
            GameObject[] targets = new GameObject[9];
            Transform[] guide = new Transform[9];
            GameObject[] guides = new GameObject[9];
            for (int i = 8; i > selected_customPos; i--)
            {
                target[i] = targetAvatar.transform.Find(thisGimmick + "/" + y + "/<PCS Target> Custom (" + i + ")");
                targets[i] = target[i].gameObject;
                DestroyImmediate(targets[i]);

                guide[i] = targetAvatar.transform.Find(thisGimmick + "/" + y + "/Placement Icons (Auto Remove)/Custom (" + i + ")");
                guides[i] = guide[i].gameObject;
                DestroyImmediate(guides[i]);
            }
            vRCParentConstraint.Sources.SetLength(4 + selected_customPos);

            //Particle Position
            var squirtObj = targetAvatar.transform.Find(thisGimmick + "/<PCS Particle> Squirt").gameObject;
            if (preset != Preset.Reference)
            {
                if (usePussy != false)
                {
                    var squirtObj_target = targetAvatar.transform.Find(thisGimmick + "/" + y + "/<PCS Target> Pussy").gameObject;
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
                    var squirtObj_target = targetAvatar.transform.Find(thisGimmick + "/" + y + "/<PCS Target> Pussy").gameObject;
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
            if (preset == Preset.Reference)
            {
                /*
                 * var mouth_comp = mouth.GetComponents(typeof(Component)).Where(o => o is not Transform);
                foreach (var comp in mouth_comp)
                {
                    DestroyImmediate(comp);
                }
                */

                var boobs_comp = boobs.GetComponents(typeof(Component)).Where(o => o is not Transform && o is not VRC.SDK3.Dynamics.Contact.Components.VRCContactReceiver);
                foreach (var comp in boobs_comp)
                {
                    DestroyImmediate(comp);
                }
                var pussy_comp = pussy.GetComponents(typeof(Component)).Where(o => o is not Transform && o is not VRC.SDK3.Dynamics.Contact.Components.VRCContactReceiver);
                foreach (var comp in pussy_comp)
                {
                    DestroyImmediate(comp);
                }
                var assh_comp = ass.GetComponents(typeof(Component)).Where(o => o is not Transform && o is not VRC.SDK3.Dynamics.Contact.Components.VRCContactReceiver);
                foreach (var comp in assh_comp)
                {
                    DestroyImmediate(comp);
                }

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
            if (preset != Preset.Reference)
            {
                if (useMouth == false)
                {
                    DestroyImmediate(mouth);
                    var guide_target = targetAvatar.transform.Find(thisGimmick + "/" + y + "/Placement Icons (Auto Remove)/Mouth").gameObject;
                    DestroyImmediate(guide_target);
                }
                if (useBoobs == false)
                {
                    DestroyImmediate(boobs);
                    var guide_target = targetAvatar.transform.Find(thisGimmick + "/" + y + "/Placement Icons (Auto Remove)/Boobs").gameObject;
                    DestroyImmediate(guide_target);
                }
                if (usePussy == false)
                {
                    DestroyImmediate(pussy);
                    var guide_target = targetAvatar.transform.Find(thisGimmick + "/" + y + "/Placement Icons (Auto Remove)/Pussy").gameObject;
                    DestroyImmediate(guide_target);
                }
                if (useAss == false)
                {
                    DestroyImmediate(ass);
                    var guide_target = targetAvatar.transform.Find(thisGimmick + "/" + y + "/Placement Icons (Auto Remove)/Ass").gameObject;
                    DestroyImmediate(guide_target);
                }
            }
            else
            {
                if (ref_mouth == null)
                {
                    DestroyImmediate(mouth);
                    var guide_target = targetAvatar.transform.Find(thisGimmick + "/" + y + "/Placement Icons (Auto Remove)/Mouth").gameObject;
                    DestroyImmediate(guide_target);
                }
                if (ref_boobs == null)
                {
                    DestroyImmediate(boobs);
                    var guide_target = targetAvatar.transform.Find(thisGimmick + "/" + y + "/Placement Icons (Auto Remove)/Boobs").gameObject;
                    DestroyImmediate(guide_target);
                }
                if (ref_pussy == null)
                {
                    DestroyImmediate(pussy);
                    var guide_target = targetAvatar.transform.Find(thisGimmick + "/" + y + "/Placement Icons (Auto Remove)/Pussy").gameObject;
                    DestroyImmediate(guide_target);
                }
                if (ref_ass == null)
                {
                    DestroyImmediate(ass);
                    var guide_target = targetAvatar.transform.Find(thisGimmick + "/" + y + "/Placement Icons (Auto Remove)/Ass").gameObject;
                    DestroyImmediate(guide_target);
                }
            }

            //Smash Sensitivity
            var smashObj = targetAvatar.transform.Find(thisGimmick + "/PCS Contacts/Receiver/Motion/Smash Hit");

            float sensitivity;
            sensitivity = 1 - smashSensitivity;
            smashObj.transform.localPosition = new Vector3(0, 0, -sensitivity / 40);

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
                    EditorUtility.DisplayDialog(thisGimmick, L("confirm1"), "OK");
                }
            }
        }
        #endregion

        #region Generate Fucntions
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
        private void GenerateParameter()
        {
            //Add parameters
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
                if (lustFeature)
                {
                    VRCExpressionParameters.Parameter param_satis1 = new()
                    {
                        name = "pcs/local/lustMultiplier",
                        valueType = VRCExpressionParameters.ValueType.Float,
                        defaultValue = lustMultiplierValue,
                        networkSynced = false,
                        saved = false
                    };
                    parameterList.Add(param_satis1);

                    VRCExpressionParameters.Parameter param_satis4 = new()
                    {
                        name = "pcs/local/eventIsReady",
                        valueType = VRCExpressionParameters.ValueType.Bool,
                        defaultValue = 0,
                        networkSynced = true,
                        saved = false
                    };
                    parameterList.Add(param_satis4);
                }

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
                if (lustFeature && voicePack != VoicePack.Disable)
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

                if (!setLocal)
                {
                    //Add selection parameters
                    if (preset != Preset.Reference)
                    {
                        if (useMouth)
                        {
                            VRCExpressionParameters.Parameter param_select = new()
                            {
                                name = "pcs/menu/mouth",
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
                                name = "pcs/menu/boobs",
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
                                name = "pcs/menu/pussy",
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
                                name = "pcs/menu/ass",
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
                                name = "pcs/menu/mouth",
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
                                name = "pcs/menu/boobs",
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
                                name = "pcs/menu/pussy",
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
                                name = "pcs/menu/ass",
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
                                name = "pcs/menu/custom" + (i + 1),
                                defaultValue = 0,
                                networkSynced = true,
                                saved = true,
                                valueType = VRCExpressionParameters.ValueType.Bool
                            };
                            parameterList.Add(param_cusotm[i]);
                        }
                    }
                }

                //Add direction offset
                if (useDirectionOffset)
                {
                    VRCExpressionParameters.Parameter param_dioffset = new()
                    {
                        name = "pcs/directionOffset",
                        valueType = VRCExpressionParameters.ValueType.Float,
                        defaultValue = 0,
                        networkSynced = true,
                        saved = true
                    };
                    parameterList.Add(param_dioffset);
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
            Texture2D icon_options = (Texture2D)AssetDatabase.LoadAssetAtPath(iconPath + "options.png", typeof(Texture2D));

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

                if (!setLocal)
                {
                    //Main Poosition
                    if (preset != Preset.Reference)
                    {
                        if (useMouth)
                        {
                            VRCExpressionsMenu.Control.Parameter selection_parameter = new()
                            {
                                name = "pcs/menu/mouth",
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
                                name = "pcs/menu/boobs",
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
                                name = "pcs/menu/pussy",
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
                                name = "pcs/menu/ass",
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
                                name = "pcs/menu/mouth",
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
                                name = "pcs/menu/boobs",
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
                                name = "pcs/menu/pussy",
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
                                name = "pcs/menu/ass",
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
                        int mouth = Convert.ToInt32(useMouth);
                        int boobs = Convert.ToInt32(useBoobs);
                        int pussy = Convert.ToInt32(usePussy);
                        int ass = Convert.ToInt32(useAss);

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
                                    name = "pcs/menu/custom" + (i + 1),
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
                                    name = "pcs/menu/custom" + (i + 1),
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
                                    name = "pcs/menu/custom" + (i + 1),
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
                }

                //Generate main menu
                var menu_ref = Resources.Load("Expression Menu/PCS Main Menu") as VRCExpressionsMenu;
                AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(menu_ref), folderPath + "/Main Menu.asset");
                var main_menu = (VRCExpressionsMenu)AssetDatabase.LoadAssetAtPath(folderPath + "/Main Menu.asset", typeof(VRCExpressionsMenu));

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

                    array = array.Where(x => !x.name.StartsWith("Edging (Stop Lust)")).ToArray();
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

                //Add selection menu to main menu
                if (!setLocal)
                {
                    VRCExpressionsMenu.Control control_selection = new()
                    {
                        name = "Sound & Location", //Selection menu name
                        icon = icon_custom,
                        type = VRCExpressionsMenu.Control.ControlType.SubMenu,
                        subMenu = selectionMenu,
                    };
                    main_menu.controls.Add(control_selection);
                }
                else
                {
                    VRCExpressionsMenu.Control control_selection = new()
                    {
                        name = L("localOnly2"), //Selection menu name
                        icon = icon_custom,
                        type = VRCExpressionsMenu.Control.ControlType.Button,
                    };
                    main_menu.controls.Add(control_selection);
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

                //If use direction offset
                if (useDirectionOffset)
                {
                    var option2 = Resources.Load("Expression Menu/PCS Options Menu 2") as VRCExpressionsMenu;
                    main_menu.controls[2].subMenu = option2;
                }

                EditorUtility.SetDirty(main_menu);
                EditorUtility.SetDirty(selectionMenu);
                EditorUtility.SetDirty(topMenu);
                EditorUtility.SetDirty(satisMenu);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            else
            {
                isError = true;
                Debug.LogError("PCS could not find the resource folder path for the avatar you selected. Please don't move !Dismay Custom folder, or your avatar name might contain prohibited characters for folder naming, such as <>:\"|?*. Please rename and try again.");
                EditorUtility.DisplayDialog(thisGimmick, "PCS could not find the resource folder path for the avatar you selected. Please don't move !Dismay Custom folder, or your avatar name might contain prohibited characters for folder naming, such as <>:\"|?*. Please rename and try again.", "OK");
            }
        }
        #endregion

    }
}
