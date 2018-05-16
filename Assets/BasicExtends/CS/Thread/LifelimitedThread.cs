namespace BasicExtends
{
    using System;
    using System.Threading;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Assertions;

    public enum ThreadState { Continue, End }

    public class LifelimitedThread
    {
        internal LifelimitedThread(int id)
        {
            ID = id;
        }

        private LifelimitedThreadManager mHolder;
        private Thread mThread = null;
        public int ID { private get; set; }

        public void Work(Func<object, ThreadState> action, object obj)
        {
            mThread = new Thread(() =>
            {
                while (action(obj) == ThreadState.Continue) { }
                mThread.Abort();
                mThread = null;
            });
        }
    }

    public class ThreadManager : Singleton<ThreadManager>
    {
        private Dictionary<int, LifelimitedThread> mHolding = new Dictionary<int, LifelimitedThread>();
        private StackLikeList<LifelimitedThread> mUnWorking = new StackLikeList<LifelimitedThread>();
        private object mLock = new object();
        public int Holding { get { return mHolding.Count; } }
        public long ThreadDead { internal set; get; }
        internal bool Enable = false;

        public void PoolFill(int num)
        {
            for (int i = 0; i < num; i++)
            {
                lock (mLock)
                {
                    var h = Holding;
                    var t = new LifelimitedThread(h);
                    mHolding.TrySet(h, t);
                }
            }
        }

        public static LifelimitedThread Get()
        {
            Assert.IsTrue(Instance.Enable);
            var ret = Instance.mUnWorking.Pop();
            if (ret.Key == false) { Instance.PoolFill(2); }
            return ret.Value;
        }
    }

    public class LifelimitedThreadManager : MonoBehaviour
    {
        private void DeadTimeUpdate()
        {
            ThreadManager.Instance.ThreadDead = DateTime.Now.Ticks + 100 * 1000;
        }

        private void Start()
        {
            ThreadManager.Instance.Enable = true;
            DeadTimeUpdate();
        }

        private void Update()
        {
            DeadTimeUpdate();
        }
    }
}
