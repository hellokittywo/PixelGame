using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;

namespace LuaFramework {
    public class TimerInfo {
        public long tick;
        public bool stop;
        public bool delete;
        public Object target;
        public string className;

        public LuaTable luaTable;
        public LuaFunction luafunc;

        public TimerInfo(LuaTable luaTable, LuaFunction luafunc)
        {
            this.luaTable = luaTable;
            this.luafunc = luafunc;
            delete = false;
        }
    }


    public class TimerManager : Manager {
        public int GameTime = 0;//��Ϸ����������ʱ��
        private float interval = 0;
        private List<TimerInfo> objects = new List<TimerInfo>();

        public float Interval {
            get { return interval; }
            set { interval = value; }
        }

        // Use this for initialization
        void Start() {
            StartTimer(AppConst.TimerInterval);
        }

        /// <summary>
        /// ������ʱ��
        /// </summary>
        /// <param name="interval"></param>
        public void StartTimer(float value) {
            interval = value;
            InvokeRepeating("Run", 0, interval);
        }

        /// <summary>
        /// ֹͣ��ʱ��
        /// </summary>
        public void StopTimer() {
            CancelInvoke("Run");
        }

        /// <summary>
        /// ��Ӽ�ʱ���¼�
        /// </summary>
        /// <param name="name"></param>
        /// <param name="o"></param>
        public void AddTimerEvent(TimerInfo info) {
            if (!objects.Contains(info)) {
                objects.Add(info);
            }
        }

        /// <summary>
        /// ɾ����ʱ���¼�
        /// </summary>
        /// <param name="name"></param>
        public void RemoveTimerEvent(TimerInfo info) {
            if (objects.Contains(info) && info != null) {
                info.delete = true;
            }
        }

        /// <summary>
        /// ֹͣ��ʱ���¼�
        /// </summary>
        /// <param name="info"></param>
        public void StopTimerEvent(TimerInfo info) {
            if (objects.Contains(info) && info != null) {
                info.stop = true;
            }
        }

        /// <summary>
        /// ������ʱ���¼�
        /// </summary>
        /// <param name="info"></param>
        public void ResumeTimerEvent(TimerInfo info) {
            if (objects.Contains(info) && info != null) {
                info.delete = false;
            }
        }

        /// <summary>
        /// ��ʱ������
        /// </summary>
        void Run() {
            GameTime++;
            if (objects.Count == 0) return;
            for (int i = 0; i < objects.Count; i++) {
                TimerInfo o = objects[i];
                if (o.delete || o.stop) { continue; }
                //this.luaTable = luaTable;
                //this.luafunc = luafunc;
                o.luafunc.Call(o.luaTable);
                //ITimerBehaviour timer = o.target as ITimerBehaviour;
                //timer.TimerUpdate();
                o.tick++;
            }
            /////////////////////////������Ϊɾ�����¼�///////////////////////////
            for (int i = objects.Count - 1; i >= 0; i--) {
                if (objects[i].delete) { objects.Remove(objects[i]); }
            }
        }
    }
}