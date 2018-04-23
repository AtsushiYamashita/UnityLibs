namespace BasicExtends {
    using System.Threading;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Assertions;

    /// <summary>
    /// 指定したアクションを繰り貸すスレッド処理のラッピング
    /// </summary>
    public class LoopThread {

        private SafeAccessValue<bool> mLoop = new SafeAccessValue<bool>();
        public bool CanLoop { private set { mLoop.Set(value); } get { return mLoop.Get(); } }

        protected Thread mThread = null;
        private int mSleepTime = 1;
        private Action mLoopAction = null;
        private List<Func<bool>> mContinuable = new List<Func<bool>>();

        public LoopThread () { }

        public LoopThread ( int sleep ) {
            mSleepTime = sleep;
        }

        public LoopThread AddContinueableCheck ( Func<bool> func ) {
            mContinuable.Add(func);
            return this;
        }

        private void ThreadLoop () {
            while (true) {
                foreach (var isContinue in mContinuable) {
                    if (isContinue()) { continue; }
                    CanLoop = false;
                    break;
                }
                if (CanLoop == false) { break; }
                try {
                    mLoopAction();
                    Thread.Sleep(mSleepTime);
                } catch (Exception e) {
                    Msg.Gen().To("Manager")
                .As("NetworkManager")
                .Set("type", "ThreadLoop2")
                .Set("state", e.ToString())
                .Set("result", "Fail").Pool();
                    return;
                }
            }
        }

        /// <summary>
        /// スレッド起動関数
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public LoopThread LaunchThread ( Action action ) {
            Assert.IsNotNull(action);
            mLoopAction = action;

            try {
                CanLoop = true;
                mThread = new Thread(ThreadLoop);
                Debug.Log("TEst sdfsd=" + CanLoop);
                mThread.Start();
            } catch {
                CanLoop = false;
                Debug.Log("Cannot launch thread.");
                Msg.Gen().To("Manager").As("NetworkManager")
                    .Set("type", "LaunchThread")
                    .Set("result", "Fail").Pool();
                return this;
            }
            return this;
        }

        public void ThreadStop () {
            CanLoop = false;
            if (mThread == null) { return; }
            mThread.Join();
            mThread = null;
            mLoopAction = null;
            Msg.Gen().To("Manager").As("NetworkManager")
                .Set("type", "ThreadStop")
                .Set("result", "Success").Pool();
        }
    }
}
