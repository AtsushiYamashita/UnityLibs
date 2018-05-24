namespace BasicExtends {
    using UnityEngine;
    using UnityEngine.Assertions;
    using System;

    /// <summary>
    /// 同期処理に必要なデータを管理するクラス。
    /// </summary>
    [Serializable]
    public class SyncTo {
        public string mObjectName = string.Empty;
        public string mComponentName = string.Empty;
        public string mIpid = "0";
        public int mPase = 3;
    }

    /// <summary>
    /// 同期を取りたいデータ型を指定して簡易的に同期する。
    /// 一つのGameObjectから複数のデータを同期したい場合にはあまり向いていない。
    /// (どこかのコンポネントでデータを一つのクラスにまとめるのが望ましい)
    /// </summary>
    public class SyncData<T> {
        /// <summary>
        /// 現在他のクライアントからデータを受け取って、同期処理をしていることを表す。
        /// 干渉権限がない状態ともいえる。
        /// </summary>
        public bool IsSyncronizing { get { return mControllLocked > 0; } }

        private Func<T> TryGet { set; get; }
        private SyncTo mSyncTo = null;
        private object mObj = null;
        private int mPaseCount = 0;
        private float mControllLocked = 0;
        private const float LOCK_Time = 0.3f;
        private Action<T> mWhenStart = null;
        private Action<T> mUpdate = null;
        private T mPrev = default(T);

        public T Prev
        {
            get { return mPrev; }
            private set { mPrev = value; }
        }
        public SyncTo SyncTo
        {
            set { mSyncTo = value; }
        }
        public Action<T> WhenStart
        {
            set { mWhenStart = value; }
        }
        public Action<T> Update
        {
            set { mUpdate = value; }
        }

        /// <summary>
        /// 同期をとる対象
        /// 同期の条件(あれば/無ければnull可)
        /// 一度だけ実行する処理
        /// 複数回かけて実行する処理
        /// </summary>
        /// <returns></returns>
        public SyncData<T> Setup ( Func<Msg, bool> receiveMatch ) {
            Assert.IsNotNull(mSyncTo);
            Assert.IsNotNull(mWhenStart);
            Assert.IsNotNull(mUpdate);
            Messenger.Assign(( Msg msg ) =>
            {
                if (msg.Match("Network", "true")) { return; }
                if (msg.Unmatch(Msg.TO, mSyncTo.mObjectName)) { return; }
                if (msg.Unmatch(Msg.AS, mSyncTo.mComponentName)) { return; }
                if (msg.Match("ReceiveCheck", "true")) {
                    Msg.Gen().Set(Msg.TO, "Manager")
                    .Set(Msg.AS, "NetworkManager")
                    .Set(Msg.MSG, "OK")
                    .Netwrok().Pool();
                }
                var match_ret = receiveMatch == null ? true : receiveMatch(msg);
                Debug.Log("Recesive => " + msg.ToJson());
                Debug.Log("match_ret => " + match_ret);
                if (msg.Match(Msg.ACT, "Sync") && msg.ContainsKey("FROM") && match_ret) {
                    TryGet = msg.TryObjectGet<T>;
                    mControllLocked = LOCK_Time;
                    mObj = TryGet();
                    Debug.Log("Sync mObj => " + mObj.ToJson());
                    return;
                }
            });
            return this;
        }

        /// <summary>
        /// 同期受信処理
        /// Updateで呼び出すことを想定
        /// </summary>
        public void Sync () {
            if (mObj.IsNull()) { return; }
            mControllLocked -= Time.deltaTime;
            var data = TryGet();
            if (mControllLocked == LOCK_Time && mWhenStart.IsNotNull()) { mWhenStart(data); }
            mUpdate(data);
            if (mControllLocked <= 0) { mObj = null; }
        }

        /// <summary>
        /// 送信処理
        /// Updateで呼び出す。
        /// 指定した感覚で同期情報を送る。
        /// </summary>
        public void UpdateSend ( Func<bool> isSend, T obj ) {
            if (IsSyncronizing) { return; }
            if (mSyncTo.mPase < 1) { return; }
            if (mPaseCount++ % mSyncTo.mPase != 0) { return; }
            var is_send = isSend == null ? true : isSend();
            if (is_send == false) { return; }
            Prev = obj;
            Msg.Gen()
                .Set(Msg.TO, mSyncTo.mObjectName)
                .Set(Msg.AS, mSyncTo.mComponentName)
                .Set(Msg.ACT, "Sync")
                .Set("IPID", mSyncTo.mIpid)
                .Netwrok()
                .SetObjectData(obj).Pool();
        }

        /// <summary>
        /// 送信処理
        /// Updateでは呼び出さない。
        /// 強制的に同期命令を送る。
        /// </summary>
        public void Send ( T obj ) {
            Msg.Gen()
                .Set(Msg.TO, mSyncTo.mObjectName)
                .Set(Msg.AS, mSyncTo.mComponentName)
                .Set(Msg.ACT, "Sync")
                .Set("IPID", mSyncTo.mIpid)
                .Netwrok()
                .SetObjectData(obj).Pool();
        }
    }
}
