namespace BasicExtends {
    using UnityEngine;

    /// <summary>
    /// Messangerから配布されたMsgを別の対象に転送する。
    /// 主にカメラなどからデータを使うときの中継に使う。
    /// </summary>
    [System.Serializable]
    public class MessageTransporter: MonoBehaviour {

        [SerializeField]
        private string[] mMatchWords =  new string[0];

        [SerializeField]
        private string[] mRewriteWords = new string[0];

        /// <summary>
        /// メッセージの中身が全条件に当てはまるか確かめる
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private bool MatchCheck ( Msg msg ) {
            if (msg.Count < 4) { return true; }
            string k = null;
            foreach (var p in mMatchWords) {
                if (k == null) {
                    k = p;
                    continue;
                }
                if (msg.Match(k, p) == false) { return false; }
            }
            return true;
        }

        /// <summary>
        /// メッセージの中身に上書き・追記する。
        /// </summary>
        /// <param name="msg"></param>
        public void Rewrite ( Msg msg ) {
            string k = null;
            foreach (var p in mRewriteWords) {
                if (k == null) {
                    k = p;
                    continue;
                }
                msg.Set(k, p);
            }
            Debug.Log("     --2---    " + msg.ToJson());
        }

        private void Start () {

            MessengerSetup();
        }

        private void MessengerSetup () {
            Messenger.Assign(( Msg msg ) =>
            {
                if (msg.Unmatch("to", gameObject.name)) { return; }
                if (msg.Unmatch("as", GetType().Name)) { return; }
                Debug.Log("     -----    " + msg.ToJson());

                if (MatchCheck(msg) == false) { return; }
                Rewrite(msg);
                msg.Push();
            });
        }
    }

}