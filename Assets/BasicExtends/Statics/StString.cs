namespace BasicExtends {
    using System.Text;

    public static class StString {
        public static ByteList ToUTF8Lst(this string str) {
            return ByteList.Gen().Add(Encoding.UTF8.GetBytes(str));
        }

        public static byte [] ToUTF8 ( this string str ) {
            return Encoding.UTF8.GetBytes(str);
        }

        public static string ToUTF8String(this byte[] bytes ) {
            return Encoding.UTF8.GetString(bytes);
        }
        public static string ToUTF8String ( this ByteList bytes ) {
            return Encoding.UTF8.GetString(bytes.ToArray());
        }

        /// <summary>
        /// 文字列を簡易に整数へ変換する
        /// </summary>
        public static int ParseInt(this string str ) {
            return int.Parse(str);
        }

        /// <summary>
        /// 文字列を簡易に浮動小数点数へ変換する
        /// </summary>
        public static float ParseFloat ( this string str ) {
            return float.Parse(str);
        }
    }
}

