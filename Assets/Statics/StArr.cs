using System;
using System.Text;


namespace BasicExtends {

    /// <summary>
    /// Arrayに関係するstaticな拡張関数を提供
    /// </summary>
    public static class StArr {

        public static string BasicHead = "[";
        public static string BasicTail = "]";

        /// <summary>
        /// Arrayをobject配列に変換する
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static object [] ToObjArr ( this Array args ) {
            return (object []) args;
        }

        /// <summary>
        /// 配列を簡単に生成する
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Array To ( params object [] args ) {
            return args;
        }

        /// <summary>
        /// 配列を読みやすい形に文字列化する
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static string Stringify ( this Array arr ) {
            return arr.Stringify(" ");
        }

        /// <summary>
        /// 配列を読みやすい形に文字列化する
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static string Stringify ( this Array arr, string sepalator ) {
            return arr.Stringify("", sepalator, "");
        }

        /// <summary>
        /// 配列を読みやすい形に文字列化する
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static string StringifyBasic ( this Array arr, string sepalator ) {
            return arr.Stringify(BasicHead, sepalator, BasicTail);
        }

        /// <summary>
        /// 配列を読みやすい形に文字列化する
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static string Stringify ( this Array arr,
            string head, string sepalator, string tail ) {
            if (arr.IsNull()) { return BasicHead + BasicTail; }
            if (arr.Length < 1) { return BasicHead + BasicTail; }
            StringBuilder sb = new StringBuilder(arr.Length);
            sb.Append(head);

            foreach (var e in arr) {
                sb.Append(e + sepalator);
            }
            var ret = sb.ToString();
            return ret.Substring(0, ret.Length - sepalator.Length) + tail;
        }
    }
}
