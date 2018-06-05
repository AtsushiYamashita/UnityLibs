namespace BasicExtends {
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public enum PortState {
        Listening, Sending, Free
    }

    public class PortStateManager: MonoBehaviour {

        [SerializeField]
        private Pair<int, int> mPortRange = new Pair<int, int>();

        private Dictionary<int, PortState> mPortState = new Dictionary<int, PortState>();
        private object mLock = new object();

        private void Start () {
            for (int i = mPortRange.Min; i < mPortRange.Max + 1; i++) {
                mPortState.TrySet(i, PortState.Free);
            }
            Messenger.Assign(( Msg msg ) =>
            {
                if (msg.Unmatch(Msg.TO, "Manager")) { return; }
                if (msg.Unmatch(Msg.AS, "NetworkManager")) { return; }
                if (msg.Match(Msg.ACT, "GetFree")) {
                    Action<int> cb = msg.TryObjectGet<Action<int>>();
                    cb(GetFreeport());
                    return;
                }
                if (msg.Match(Msg.ACT, "SetPortState")) {
                    //var port = msg.TryGet("port").ParseInt();
                    return;
                }

            });
        }

        /// <summary>
        /// 管理しているポートの情報を取得する
        /// </summary>
        public int GetFreeport (  ) {
            foreach (var e in mPortState) {
                if (e.Value != PortState.Free) { continue; }
                return e.Key;
            }
            return -1;
        }

        /// <summary>
        /// 管理しているポートの情報を更新する
        /// スレッドセーフ
        /// </summary>
        public void SetPortState (int port, PortState state) {
            lock (mLock) {
                mPortState [port] = state;
            }
        }
    }
}
