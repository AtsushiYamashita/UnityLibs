namespace BasicExtends {
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;

    public class StringDict : Dictionary<string, string> { }
    public class GameObjDict : Dictionary<string, GameObject> { }
    public class ObjDict : Dictionary<string, object> { }

    /// <summary>
    /// 辞書型コレクションに対する
    /// スタンダードな拡張関数を提供しています。
    /// </summary>
    public static class StDic {
        public static string basicHead = "{";
        public static string basicTail = "}";
        public static string basicSeparator1 = ":";
        public static string basicSeparator2 = ",";


        public static string Stringify<K, V> (
            this Dictionary<K, V> dic,
            string head, string sep1,string sep2, string tail ) {
            StringBuilder sb = new StringBuilder(dic.Values.Count);
            sb.Append(head);
            foreach (var val in dic) {
                sb.Append(val.Key)
                    .Append(sep1)
                    .Append(val.Value)
                    .Append(sep2);
            }
            var ret = sb.ToString();
            return ret.Substring(0, ret.Length - sep2.Length) + tail;
        }

        public static string Stringify<K, V> (
            this Dictionary<K, V> dic, string sep1, string sep2 ) {
            return dic.Stringify(basicHead, sep1, sep2, basicTail);
        }

        public static string Stringify<K, V> (
            this Dictionary<K, V> dic ) {
            return dic.Stringify(basicHead,
                basicSeparator1,
                basicSeparator2,
                basicTail);
        }
    }

}
