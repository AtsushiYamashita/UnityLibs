namespace BasicExtends {
    using System;

    /// <summary>
    /// スレッドセーフな値を作り、関数経由で処理を行うようにする
    /// Serializeはできないようにしている
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class SafeAccessValue<T> {
        [NonSerialized]
        object mLock = new object();
        [NonSerialized]
        private T mValue;
        public T Val { get; }

        public SafeAccessValue<T> Action ( Func<T, T> func ) {
            var temp = func(mValue);
            lock (mLock) {
                mValue = temp;
            }
            return this;
        }

        public SafeAccessValue<T> Set ( T t ) {
            Action(( val ) => { return t; });
            return this;
        }
    }
}
