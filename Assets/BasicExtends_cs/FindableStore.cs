using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace BasicExtends {

    class TypedArray: Dictionary<Type, BufferedArray> { }

    public class FindableStore :Singleton<FindableStore>{

        private TypedArray dic = new TypedArray();

        public T TryGet<T>(int index) where T:class{
            var t = typeof(T);
            Assert.IsTrue(dic.ContainsKey(t));

            var arr = dic [t];
            Assert.IsTrue(arr.Length > index);

            var v = arr.TryGet<T>(index);
            if (v != null) { return v; }

            v = (T) Activator.CreateInstance(typeof(T), true);
            arr.Arr [index] = v;
            return v;
        }

        public void Add<T> ( params T[] arr ) where T : class {
            Assert.IsTrue(arr.IsNotNull());
            Assert.IsTrue(arr.Length >= 0);
            var type = typeof(T);
            if (dic.ContainsKey(type)==false) { dic.Add(type, new BufferedArray() ); }
            var ba = dic [type];
            ba.Add(arr);
            ba.SetFront();
        }

        public T[] TryGetArray<T> (  ) where T : class {
            var t = typeof(T);
            Assert.IsTrue(dic.ContainsKey(t));

            var arr = dic [t];
            return arr.Arr as T[];
        }

    }
}