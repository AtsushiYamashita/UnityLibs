
namespace BasicExtends {
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    
    /// <summary>
    /// Msgオブジェクトのやり取りを中継するオブザーバー
    /// </summary>
    public class Messenger: Singleton<Messenger> {

        private List<Action<Msg>> mCallBacks = new List<Action<Msg>>();
        private Queue<Msg> mMessages = new Queue<Msg>();
        private System.Object mLock = new System.Object();

        public static void Pool ( Msg msg ) {
            lock (Instance.mLock) {
                Instance.mMessages.Enqueue(msg);
            }
        }

        public static void Flash () {
            var msg = Instance.mMessages;
            while (msg.Count > 0) {
                var m = msg.Dequeue();
                Push(m);
            }
        }

        /// <summary>
        /// MsgをMessengerに送信する。
        /// 特定のKeyとValueならMessenger内部で処理し、
        /// そうでない場合は登録されているコールバック全体にメッセージを送る。
        /// </summary>
        /// <param name="msg"></param>
        public static void Push ( Msg msg ) {
            if (msg.Match("To", "Messanger") && msg.Match("Action", "ClearAll")) {
                Debug.Log("Data clear <= " + msg.ToJson());
                msg.Clear();
                return;
            }
            foreach (var cb in Instance.mCallBacks) {
                cb.Invoke(msg);
            }
            if (msg.Match("to", "Debug") && msg.Match("act", "log")) {
                Debug.Log("Debug from Msg :: " + msg.ToJson());
                return;
            }
        }

        /// <summary>
        /// PushされたMsgを受け取るための関数を入れる。
        /// この中に入れられたアクション経由で、
        /// メッセージの処理を書くことになる。
        /// </summary>
        public static void Assign ( Action<Msg> callback ) {
            /* Usage : Copy%Paste this
               Messenger.Assign(msg =>{
                    if (msg.Unmatch("To", gameObject.name)) { return; }
                    if (msg.Unmatch("as", GetType().Name)) { return; }
                    if (msg.Match("act", "A")) {
                        return;
                    }
               });
             */
            Instance.mCallBacks.Add(callback);
        }
    }
}
