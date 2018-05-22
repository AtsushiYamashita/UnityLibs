namespace BasicExtends {
    using System;
    using System.Threading;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Assertions;

    public enum ThreadState { Continue, End }

    public class LifeThread {
        internal LifeThread ( int id ) {
            ID = id;
        }

        private LifelimitedThreadManager mHolder;
        private Thread mThread = null;
        public int ID { private get; set; }

        public void Work ( object obj, Func<object, ThreadState> action ) {
            mThread = new Thread(() =>
            {
                while (action(obj) == ThreadState.Continue) { Thread.Sleep(1); }
                mThread.Abort();
                mThread = null;
            });
        }
    }

    public class ThreadManager: Singleton<ThreadManager> {
        private Dictionary<int, LifeThread> mHolding = new Dictionary<int, LifeThread>();
        private StackLikeList<LifeThread> mUnWorking = new StackLikeList<LifeThread>();
        private object mLock = new object();
        public int Holding { get { return mHolding.Count; } }
        public long ThreadDead { internal set; get; }
        internal bool Enable = false;

        public void PoolFill ( int num ) {
            for (int i = 0; i < num; i++) {
                lock (mLock) {
                    var h = Holding;
                    var t = new LifeThread(h);
                    mHolding.TrySet(h, t);
                }
            }
        }

        public static LifeThread Get () {
            Assert.IsTrue(Instance.Enable);
            CheckedRet<LifeThread> ret = null;
            lock (Instance.mLock) {
                ret = Instance.mUnWorking.Pop();
            }
            if (ret.Key == false) { Instance.PoolFill(2); }
            return ret.Value;
        }
    }

    public class LifelimitedThreadManager: MonoBehaviour {
        private void DeadTimeUpdate () {
            ThreadManager.Instance.ThreadDead
                = DateTime.Now.Ticks + 1000 * 1000;
        }

        private void Start () {
            ThreadManager.Instance.Enable = true;
            DeadTimeUpdate();
        }

        private void Update () {
            DeadTimeUpdate();
        }
    }
}
