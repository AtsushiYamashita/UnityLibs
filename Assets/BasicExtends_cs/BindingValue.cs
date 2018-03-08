using System;
using System.Collections.Generic;
using UnityEngine;

namespace BasicExtends {

    public delegate void BindedAction<T> ( T old, T now );

    public static class StaticBind {
        public static BindingValue<T> Bind<T> ( this T t, BindedAction<T> a ) where T : class {
            return new BindingValue<T>().Set(t).Bind(a);
        }
    }

    [Serializable]
    public class BindingValue: BindingValue<object> { }

    [Serializable]
    public class BindingValue<T> where T : class {

        [SerializeField]
        private T mInstance = null;

        List<BindedAction<T>> mAction 
            = new List<BindedAction<T>>();

        public BindingValue<T> Set ( T t ) {
            if (mInstance == t) { return this; }
            var old = mInstance;
            mInstance = t;
            foreach (var a in mAction) {
                // この呼び出しが発生した時点で切り替わっていないとまずい。
                a.Invoke(old, mInstance);
            }
            return this;
        }

        public BindingValue<T> Bind ( BindedAction<T> action ) {
            mAction.Add(action);
            return this;
        }

        public T Get () {
            return mInstance;
        }

        public BindingValue<T> BindSync ( BindingValue<T> a ) {
            Action<BindingValue<T>, BindingValue<T>> set =
                ( BindingValue<T> f, BindingValue<T> t ) =>
                {
                    f.Bind(( T o, T n ) => { t.Set(n); });
                };
            set(a, this);
            set(this, a);
            return this;
        }
    }
}