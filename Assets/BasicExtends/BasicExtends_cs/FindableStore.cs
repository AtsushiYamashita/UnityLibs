using System;
using System.Collections.Generic;
using UnityEngine;

namespace BasicExtends {

    class NamedObject: Dictionary<string, BufferedArray> { }
    class TypedArray: Dictionary<Type, NamedObject> { }

    public class FindableStore :Singleton<FindableStore>{

        private TypedArray mDic = new TypedArray();

        public T[] TryGetArr<T>(string name) where T:class{
            var t = typeof(T);
            NamedObject dic2 = null;
            if (mDic.TryGetValue(t, out dic2)) {
                throw new Exception("Target type is missing.");
            };
            if(dic2 == null) {
                return null;
            }

            BufferedArray arr = null;
            if (dic2.TryGetValue(name, out arr)) {
                return null;
            };
            if (arr == null) {
                return null;
            }

            return arr.Arr as T[];
        }

        /// <summary>
        /// 可変長なので単数にも対応
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        public void Add<T> ( params Pair<string,T>[] arr ) where T : class {
            var type = typeof(T);
            if (mDic.ContainsKey(type)==false) {
                mDic.Add(type, new NamedObject() );
            }
            var ba = mDic [type];
            foreach(var e in arr) {
                if (ba.ContainsKey(e.Key) == false) {
                    ba.TrySet(e.Key, new BufferedArray());
                }
                ba [e.Key].Add(e.Value);
                ba [e.Key].SetFront();
            }
        }
    }
}