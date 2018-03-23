namespace BasicExtends {
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using UnityEngine;

    public class StringPair: Pair<string, string> { }

    public static class StPair {
        public static Dictionary<K, V> GenDic<K, V>
            ( this Pair<K, V> a )
            where V : class {
            var dic = new Dictionary<K, V>();
            dic.TrySet(a.mKey, a.mValue);
            return dic;
        }

        public static Dictionary<K, V> ToDic<K, V>
            ( this List<Pair<K, V>> list )
            where V : class {
            var dic = new Dictionary<K, V>();
            foreach (var a in list) {
                dic.TrySet(a.mKey, a.mValue);
            }
            return dic;
        }

        public static Pair<K, V> Gen<K, V> ( K k, V v )
            where V : class {
            return new Pair<K, V>().Set(k, v);
        }

        public static List<Pair<K, V>> Add<K, V> ( this List<Pair<K, V>> p, K k, V v )
            where V : class {
            p.Add(Gen(k, v));
            return p;
        }


        public static List<Pair<K, V>> ToList<K, V> ( this Pair<K, V> p )
            where V : class {
            List<Pair<K, V>> arr = new List<Pair<K, V>> { p };
            return arr;
        }

        public static List<Pair<K, V>> ToList<K, V> ( this Pair<K, V> [] arr )
            where V : class {
            if (arr.Length == 0) { return new List<Pair<K, V>>(); }
            if (arr.Length == 1) { return new List<Pair<K, V>> { arr [0] }; }

            var ret = new List<Pair<K, V>> { arr [0], arr [1] };
            foreach (var i in Enumerable.Range(2, arr.Length)) {
                ret.Add(arr [i]);
            }
            return ret;
        }
    }

    public class Pair<K, V>: IDictionaryDataConvertable<K, V>
        where V : class {
        public K mKey;
        public V mValue = null;
        public Pair () { }
        public Pair<K, V> Set ( K key, V value ) { mKey = key; mValue = value; return this; }
        public K GetKey () { return mKey; }
        public V GetValue () { return mValue; }
    }
}