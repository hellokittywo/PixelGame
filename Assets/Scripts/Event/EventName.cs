using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Scripts.Event
{
    public class EventName
    {
        //下载更新事件
        public static string ADD_DOWNLOAD_FILE = "ADD_DOWNLOAD_FILE";

        public static string DOWNLOAD_FILE_OVER = "download_file_over";

        public static string AddSOLDIERBYTYPE = "addsoldierbytype";

        //重置其他位置
        public static string RESET_POSITION = "reset_position";

        //BasicView事件
        public static string Up_View = "up_view";

        public static string Up_Effect_View= "up_effect_view";

        //释放技能
        public static string PlaySkill = "play_skill";

        public static string GetReward = "get_reward";

        public static string ArrowHurt = "Arrow_hurt";

        //建筑界面
        public static string BUILD_UPGRADE = "build_upgrade";

        public static string RemoveSoldierBtn = "RemoveSoldierBtn";

        //防御塔
        public static string TURRET_EVENT = "Turret_Event";
        //研究院
        public static string ACADEMY_EVENT = "Academy_Event";

        public static string Ios_GetSysBitComplete = "Ios_GetSysBitComplete";
    }
}
