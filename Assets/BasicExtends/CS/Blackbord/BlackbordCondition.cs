
namespace BasicExtends.Blackbord {
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    public static class FloatCondition {

        /// <summary>
        /// この判定における「型」はすべてfloatで扱う。
        /// </summary>
        private static float ToValue ( Component target, string str ) {
            if (str.IndexOf("$") < 0) {
                float r = 0;
                float.TryParse(str, out r);
                return r;
            }
            var key1 = (target.IsNotNull() ? target.name + "." : "");
            var key2 = str.Substring(1);
            var v = ValueStore.TryGet<float>(key1 + key2);
            if (v.Check) {
                return v.Value;
            }
            var e = string.Format("({0}) is missing", str);
            throw new Exception(e);
        }

        private static Func<float, float, bool> ToFunc ( string type ) {
            switch (type) {
                case "<": return ( a, b ) => a < b;
                case ">": return ( a, b ) => a > b;
                case "<=": return ( a, b ) => a <= b;
                case ">=": return ( a, b ) => a >= b;
                case "==": return ( a, b ) => a == b;
                case "!=": return ( a, b ) => a != b;
            }
            var e = string.Format("({0}) is missing", type);
            throw new Exception(e);
        }

        public static bool Check ( Component target, string script ) {
            var queue = new Queue<string>(script.Split(" ".ToCharArray() [0]));
            var v1 = ToValue(target, queue.Dequeue());
            var f = ToFunc(queue.Dequeue());
            var v2 = ToValue(target, queue.Dequeue());
            return f(v1, v2);
        }
    }
}
