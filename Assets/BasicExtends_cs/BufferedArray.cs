using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace BasicExtends {

    [Serializable]
    public class BufferedArray: BufferedArray<object> { }

        /// <summary>
        /// 可変長配列。ただしシンプルな実装なので重い。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        [Serializable]
    public class BufferedArray<T> where T : class {

        private static readonly uint SIZE = 5;
        private uint mSize = SIZE;

        [SerializeField]
        private T [] mArr;

        public T [] Arr
        {
            get { return mArr; }
        }

        public uint AddSize
        {
            get { return mSize; }
            set { mSize = value; }
        }
        public int Length
        {
            get { return mArr.Length; }
        }
        public BufferedArray () : this(SIZE) { }
        public BufferedArray ( uint first ) : this(first, SIZE) { }

        /// <summary>
        /// 要素数firstの配列を作り、
        /// 拡張する場合app_sizeを使うようにする
        /// </summary>
        /// <param name="first"></param>
        /// <param name="app_size"></param>
        public BufferedArray ( uint first, uint app_size ) {
            Assert.IsTrue(first > 0);
            Assert.IsTrue(app_size > 0);
            mArr = new T [first];
            mSize = app_size;
        }

        /// <summary>
        /// 要素を末尾に追加する。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual BufferedArray<T> Add ( T obj, Func<T, bool> nullLike ) {
            for (int i = 0; i < mArr.Length; i++) {
                var target = mArr [i];
                if (target.IsNotNull() && !nullLike(target)) { continue; }
                mArr [i] = obj;
                return this;
            }
            return Expend().Add(obj, nullLike);
        }
        /// <summary>
        /// 要素を末尾に追加する。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual BufferedArray<T> Add ( string obj ) {
            if (obj.IsNull()) { return this; }

            // 疑似的な構文糖衣
            return Add(obj as T, ( e ) =>
            {
                string s = e as string;
                if (s.IsNull()) { return false; }
                return s.Length < 1;
            });

        }

        /// <summary>
        /// 要素を末尾に追加する。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual BufferedArray<T> Add ( T obj ) {
            if (obj.IsNull()) { return this; }

            for (int i = 0; i < mArr.Length; i++) {
                var target = mArr [i];
                if (target.IsNotNull()) { continue; }
                mArr [i] = obj;
                return this;
            }
            return Expend().Add(obj);
        }

        /// <summary>
        /// 要素の配列を末尾に追加する。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual BufferedArray<T> Add ( params T [] obj ) {
            var old = mArr;
            var size = mSize + old.Length + obj.Length;
            mArr = new T [size];

            for (int i = 0; i < obj.Length; i++) {
                mArr [old.Length + i] = obj [i];
            }
            return this;
        }

        /// <summary>
        /// 要素の順番を保ったまま、なるべく若いインデックスに詰める。
        /// </summary>
        /// <returns></returns>
        public BufferedArray<T> SetFront () {
            var filled = 0;
            var itr = 0;
            for (; itr < mArr.Length; itr++) {
                if (mArr [itr].IsNull()) { continue; }
                mArr [filled] = mArr [itr];
                filled++;
            }
            for (; filled < mArr.Length; filled++) {
                mArr [filled] = null;
            }
            return this;
        }

        /// <summary>
        /// 要素数の大きな配列を作成し、中身をすべて移動する。
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public virtual BufferedArray<T> Expend ( uint size ) {
            var old = mArr;
            var now = new T [mArr.Length + size];
            for (int i = 0; i < mArr.Length; i++) {
                now [i] = old [i];
            }
            mArr = now;
            return this;
        }

        /// <summary>
        /// 要素数の大きな配列を作成し、中身をすべて移動する。
        /// </summary>
        /// <returns></returns>
        public virtual BufferedArray<T> Expend () {
            return Expend(mSize);
        }

        /// <summary>
        /// objに一致する要素すべてをemptyに置き換える
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="empty"></param>
        /// <returns></returns>
        public virtual BufferedArray<T> Remove ( T obj, T empty ) {
            Map(( e ) => { return e == obj ? empty : e; });
            return this;
        }

        /// <summary>
        /// funcをフィルターとして要素を置き換える
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public BufferedArray<T> Map ( Func<T, T> func ) {
            for (int i = 0; i < mArr.Length; i++) {
                if (mArr [i].IsNull()) { continue; }
                mArr [i] = func(mArr [i]);
            }
            return this;
        }

        /// <summary>
        /// 要素すべてに処理を行う
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public BufferedArray<T> ForEach ( Action<T> func ) {
            Assert.IsNotNull(mArr);
            Assert.IsNotNull(func);
            for (int i = 0; i < mArr.Length; i++) {
                func(mArr [i]);
            }
            return this;
        }

        public T Get ( int index ) {
            Assert.IsTrue(index < mArr.Length);
            return mArr [index];
        }

        public C TryGet<C> ( int index ) where C : class {
            Assert.IsTrue(index < mArr.Length);
            return mArr [index] as C;
        }

        public C [] TryGet<C> () where C : class {
            return mArr as C [];
        }

        public BufferedArray<T> Reset () {
            mArr = new T [mArr.Length];
            return this;
        }
    }
}
