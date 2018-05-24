namespace BasicExtends {
    using System.Net;

    /// <summary>
    /// ネットワークに関係する汎用機能を集めたクラス
    /// </summary>
    public static class NetowrkUtil {

        /// <summary>
        /// 自分のローカルIPを取得する。
        /// 値が取れない場合はEmptyが返る
        /// </summary>
        public static string GetOwnIP () {
            System.Exception exception = new System.Exception();
            try {
                //ホスト名を取得
                string hostname = Dns.GetHostName();

                //ホスト名からIPアドレスを取得
                IPAddress [] addr_arr = Dns.GetHostAddresses(hostname);

                foreach (IPAddress addr in addr_arr) {
                    string addr_str = addr.ToString();
                    bool isIPv4 = addr_str.IndexOf(".") > 0;
                    bool notLocalLoopback = addr_str.StartsWith("127.") == false;
                    if (isIPv4 && notLocalLoopback) { return addr_str; }
                }
            } catch (System.Exception e) {
                exception = e;
            }
            Msg.Gen().Set(Msg.TO, "Debug")
                .Set(Msg.ACT, "log")
                .Set(Msg.MSG, exception.ToString()).Pool();
            return string.Empty;
        }
    }

}