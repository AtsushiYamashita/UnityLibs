
namespace BasicExtends {
    using System.Collections.Generic;
    using UnityEngine;
    using System;

    /// <summary>
    /// 実質的なJSにおけるオブジェクト。
    /// 
    /// </summary>
    public class Msg: StringDict {

        public static Msg Gen () { return new Msg(); }

        /// <summary>
        /// 値の有無にかかわらずセットする。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Msg Set ( string key, string value ) {
            if (key.Length < 1) { throw new Exception("Error, illigal MSG key set"); }
            key = key.ToUpper();
            if (ContainsKey(key) == false) { Add(key, value); return this; }
            this [key] = value;
            return this;
        }

        public Msg To ( string v ) {
            return Set("to", v);
        }

        public Msg As ( string v ) {
            return Set("as", v);
        }

        public Msg Message ( string v ) {
            return Set("msg", v);
        }

        /// <summary>
        /// when key-value is matched, then this function return true.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Match ( string key, string value ) {
            if (key.Length < 1) { throw new Exception("Error, illigal MSG key set"); }
            key = key.ToUpper();
            if (ContainsKey(key) == false) { return false; }
            var insideValue = this [key];
            if (insideValue.ToUpper() != value.ToUpper()) { return false; }
            return true;
        }

        /// <summary>
        /// when key-value is unmatched, then this function return true.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Unmatch ( string key, string value ) {
            return !Match(key,value);
        }

        /// <summary>
        /// Toに単体で入れられている場合を想定。
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public bool ToIs ( string v ) {
            return Match("to", v);
        }

        /// <summary>
        /// Toに配列で入れられている場合を想定
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public bool ToContain ( string v ) {
            var s = TryGet("to");
            return s.Contains(v);
        }
        /// <summary>
        /// this function do not return null.
        /// If this cannnot the param,
        /// then return Empty string.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string TryGet ( string key ) {
            if (key.Length < 1) { throw new Exception("Error, illigal MSG key set"); }
            key = key.ToUpper();
            if (ContainsKey(key) == false) { return string.Empty; }
            var insideValue = this [key];
            if (insideValue == null) { return string.Empty; }
            return insideValue;
        }

        public Msg Push () {
            Messenger.Push(this);
            return this;
        }
    }

    /// <summary>
    /// Msgオブジェクトのやり取りを中継するオブザーバー
    /// </summary>
    public class Messenger: Singleton<Messenger> {

        private List<Action<Msg>> mCallBacks = new List<Action<Msg>>();

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
        }

        /// <summary>
        /// PushされたMsgを受け取るための関数を入れる。
        /// この中に入れられたアクション経由で、
        /// メッセージの処理を書くことになる。
        /// </summary>
        /// <param name="callback"></param>
        public static void Assign ( Action<Msg> callback ) {
            Instance.mCallBacks.Add(callback);
        }
    }
}
