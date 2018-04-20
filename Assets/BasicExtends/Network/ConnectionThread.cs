using System.Threading;
using System;
using UnityEngine;

namespace BasicExtends {

    public class ConectionThread: INetConnectThread {
        protected bool mThreadLoop = false;
        protected Thread mThread = null;
        protected IConnectionData mData = null;
        public bool IsLooping { set; get; }
        private Action mLoopAction;

        private void ThreadLoop () {
            mData.ConnectThread.IsLooping = true;
            mData.IsConnected = true;
            while (mData.ConnectThread.IsLooping && mData.IsConnected) {
                mData.ClientLifeCheck();
                try {
                    mLoopAction();
                    Thread.Sleep(1);
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
        public bool LaunchThread ( IConnectionData data, Action action ) {
            mData = data;
            mLoopAction = action;
            try {
                // Dispatch用のスレッド起動.
                mThreadLoop = true;

                mThread = new Thread(ThreadLoop);
                mThread.Start();
            } catch {
                Debug.Log("Cannot launch thread.");
                Msg.Gen().To("Manager").As("NetworkManager")
                    .Set("type", "LaunchThread")
                    .Set("result", "Fail").Pool();
                return false;
            }
            return true;
        }


        public void ThreadStop () {
            mThreadLoop = false;
            if (mThread == null) { return; }
            mThread.Join();
            mThread = null;
            mData.Disconnect();
            Msg.Gen().To("Manager").As("NetworkManager")
                .Set("type", "ThreadStop")
                .Set("result", "Success").Pool();
        }
    }

}