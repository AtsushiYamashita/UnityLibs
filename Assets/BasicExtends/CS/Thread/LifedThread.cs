namespace BasicExtends {
    using System;
    using System.Threading;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Assertions;

    public enum ThreadState { Continue, End, Sleep }

    public class LifedThread {
        internal LifedThread ( int id ) {
            ID = id;
        }

        private LifedThreadManager mHolder;
        private Thread mThread = null;
        public int ID { get; private set; }
        public string mName = "";
        public bool IsWorking { get { return mThread != null; } }
        public bool mEnd = false;
        private object mLock = new object();

        /// <summary>
        /// 死活管理の関係で、Manager以外から生成されないようにしている。
        /// </summary>
        internal LifedThread () { }

        public void Work ( string name, object obj, Func<object, ThreadState> action ) {
            mName = name;
            Debug.Log("Thread start:" + name);
            lock (mLock) {
                try {
                    mThread = new Thread(Loop);
                    mThread.IsBackground = true;
                    var args = Tapple.Gen.Add(this, action, obj);
                    mThread.Start(args);
                }catch(Exception e) {
                    throw new Exception(e.ToString());
                }

            }
        }

        private static void Loop ( object obj ) {
            var tapple = (Tapple) obj;
            var self = tapple.Get<LifedThread>();
            var action = tapple.Get<Func<object, ThreadState>>();

            while (self.mEnd == false) {
                if (self.mThread.IsNull()) { break; }
                var ret = action(tapple.Get<object>(2));
                if (ret == ThreadState.End) { break; }
                if (ret == ThreadState.Sleep) { Thread.Sleep(8); }
                if (ThreadManager.ThreadLiving == false) { break; }
                Thread.Sleep(1);
                //Msg.Gen().Set(Msg.TO, "Debug").Set(Msg.ACT, "log").Set(Msg.MSG, "Thread loop:" + self.mName).Pool();
            }
            Msg.Gen().Set(Msg.TO, "Debug").Set(Msg.ACT, "log").Set(Msg.MSG, "Thread end:" + self.mName).Pool();
            self.mThread = null;
            Thread.Sleep(0);
        }

        public void Abort () {
            if (this.IsNull()) { return; }
            if (mThread.IsNull()) { return; }
            mEnd = true;
            mThread.Interrupt();
            Debug.Log("Abort@thread" + mName + " Alive" + mThread.IsAlive);
            mThread = null;
        }
    }


    /// <summary>
    /// LifedThreadを生成し、その死活管理を
    /// </summary>
    public class ThreadManager: Singleton<ThreadManager> {
        private Dictionary<int, LifedThread> mHolding = new Dictionary<int, LifedThread>();
        private StackLikeList<LifedThread> mUnWorking = new StackLikeList<LifedThread>();
        private object mLock = new object();
        public int Holding { get { return mHolding.Count; } }
        public long ThreadDeadTime { internal set; get; }
        public static bool ThreadLiving
        {
            get { return Instance.ThreadDeadTime > DateTime.Now.Ticks; }
        }
        internal bool Enable = false;

        public void PoolFill ( int num ) {
            for (int i = 0; i < num; i++) {
                lock (mLock) {
                    var h = Holding;
                    var t = new LifedThread(h);
                    mHolding.TrySet(h, t);
                    mUnWorking.Push(t);
                }
            }
        }

        public static void DeadTimeUpdate () {
            Instance.ThreadDeadTime
                = DateTime.Now.Ticks + 1000 * 1000 * 1000;
        }

        public static LifedThread Get () {
            DeadTimeUpdate();
            Assert.IsTrue(Instance.Enable, "Must contain the LifedThreadManager in this scene.");
            if (Instance.ThreadDeadTime < 1) { Instance.ThreadDeadTime = DateTime.Now.Ticks * 10; }
            lock (Instance.mLock) {
                CheckedRet<LifedThread> ret = null;
                lock (Instance.mLock) { ret = Instance.mUnWorking.Pop(); }
                if (ret.Key != false) { return ret.Value; }
                Instance.PoolFill(2);
                lock (Instance.mLock) { ret = Instance.mUnWorking.Pop(); }
                return ret.Value;
            }
        }

        public static void Abort () {
            foreach (var t in Instance.mHolding.Values) {
                t.Abort();
            }
        }

        public static void PrintState () {
            var text = "Is working? \n";
            foreach (var t in Instance.mHolding.Values) {
                text += (t.mName + "is " + (t.IsWorking ? "working" : "waiting") + "\n");
            }
            Debug.Log(text);
        }
    }
}
