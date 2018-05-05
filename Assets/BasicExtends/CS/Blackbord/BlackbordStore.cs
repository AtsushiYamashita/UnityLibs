using System;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;

namespace BasicExtends.Blackbord {

    class NamedObjectDict: Dictionary<string, object> { }
    class TypeDict: Dictionary<Type, NamedObjectDict> { }

    /// <summary>
    /// 値は追加する時点でまず型を調べられる。
    /// そのうえで、TypeDictの中にあるNamaedObjectDictへ入る。
    /// つまり型を記録したうえで型の集合の中に要素を作っている
    /// </summary>
    public class ValueStore: Singleton<ValueStore> {

        private TypeDict mDic = new TypeDict();

        public static CheckedRet<T> TryGet<T> ( string name ) {
            return Instance.TryGet_<T>(name);
        }
        public static CheckedRet<T> TryGet<T> ( Component com, string name ) {
            return Instance.TryGet_<T>(name);
        }

        public static void Add<T> ( params Pair<string, T> [] arr ) {
            ArgCheck(arr);
            Instance.Add_(arr);
        }

        public static void Add<T> (Component com, params Pair<string, T> [] arr ) {
            ArgCheck(arr);
            Instance.Add_(com,arr);
        }

        private static void ArgCheck<T> ( Pair<string, T> [] arr ) {
            Assert.IsTrue(arr.IsNotNull());
            Assert.IsTrue(arr.Length >= 0);
        }

        private CheckedRet<T> TryGet_<T> ( Component com, string name ) {
            return TryGet_<T>(com.IID() + "." + name);
        }

        private CheckedRet<T> TryGet_<T> ( string name ) {
            var t = typeof(T);
            NamedObjectDict dic2 = null;
            if (mDic.TryGetValue(t, out dic2)) {
                throw new Exception("Target type is missing.");
            };
            if (dic2.IsNull()) {
                return CheckedRet<T>.Fail();
            }

            object value = null;
            if (dic2.TryGetValue(name, out value)) {
                return CheckedRet<T>.Fail();
            };
            if (value.IsNull()) {
                return CheckedRet<T>.Fail();
            }
            var ret = (T) value;
            return CheckedRet<T>.Gen(true, ret);
        }


        private Type TypeCheck<T> (  ) {
            var type = typeof(T);
            if (mDic.ContainsKey(type) == false) {
                mDic.Add(type, new NamedObjectDict());
            }
            return type;
        }

        /// <summary>
        /// 可変長なので単数にも対応
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        private void Add_<T> ( params Pair<string, T> [] arr ) {
            var type = TypeCheck<T>();
            var ba = mDic [type];
            foreach (var e in arr) {
                ba.TrySet(e.Key, e.Value);
            }
        }

        private void Add_<T> ( Component com,  params Pair<string, T> [] arr ) {
            var type = TypeCheck<T>();
            var object_key = com.name + ".";
            var ba = mDic [type];
            foreach (var e in arr) {
                ba.TrySet(object_key  + e.Key, e.Value);
            }
        }
    }
}