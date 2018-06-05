namespace BasicExtends {
    using System.Collections.Generic;
    using System;

    /// <summary>
    /// 複数の型のオブジェクトをまとめて扱う。
    /// 関数の戻り値などで使う。
    /// 型情報から問い合わせを行っているので、型ごとに１つのインスタンスしか持てない点に注意
    /// </summary>
    public class Tapple {

        private Dictionary<Type, object> mList = new Dictionary<Type, object>();
        public static Tapple Gen { get { return new Tapple(); } }

        /// <summary>
        /// 関数の戻り値として、何らかの処理が失敗していた時に使いやすいようにした糖衣
        /// </summary>
        public static Tapple Fail { get { return mFail; } }
        private static Tapple mFail = Gen.Add(false);

        public Tapple () { }

        public Tapple Add ( params object [] arr ) {
            foreach (var obj in arr) {
                if (obj == null) { continue; }
                mList.Add(obj.GetType(), obj);
            }
            return this;
        }

        /// <summary>
        /// Tappleの中から要素を取り出す。
        /// 内部に指定の要素がなかったり、キャストに失敗した時のため、デフォルトを指定する必要がある。
        /// ただし、classの取り出しであればオーバーロードでnullが返るようにもできる。
        /// </summary>
        public T Get<T> ( T def ) {
            try {
                return (T) mList [typeof(T)];
            } catch {
                return def;
            }
        }

        /// <summary>
        /// Tappleの中から要素を取り出す。
        /// </summary>
        public T Get<T> () where T : class {
            try {
                return (T) mList [typeof(T)];
            } catch {
                return null;
            }
        }

        public Tapple Update ( Func<Dictionary<Type, object>, Tapple> func ) {
            return func(mList);
        }
    }
}
