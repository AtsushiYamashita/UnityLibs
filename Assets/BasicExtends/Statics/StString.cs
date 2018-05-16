namespace BasicExtends {
    using System.Text;
    using System.Collections.Generic;
    using UnityEngine;

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
    }
}

