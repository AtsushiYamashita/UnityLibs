using System;
using System.Collections.Generic;
using UnityEngine;


namespace BasicExtends {

    public class CurriedFunc<R> where R : class {
        private Func<object [], R> mCaller = null;
        private object [] mObjects = NULL.NullArr;

        public CurriedFunc<R> SetFunc ( Func<object [], R> caller ) {
            mCaller = caller;
            return this;
        }

        public CurriedFunc<R> SetArg ( params object [] arg ) {
            mObjects = arg;
            return this;
        }

        public CurriedFunc<R> AddArg ( params object [] arg ) {
            mObjects = StArr.To(mObjects, arg).ToObjArr();
            return this;
        }

        public R Invoke ( params object [] obj ) {
            mCaller.NullThrow();
            return mCaller(obj) as R;
        }
        public object Invoke () {
            NullChecker.NullThrow(mCaller, mObjects);
            return mCaller(mObjects);
        }
    }

    public class CurriedAction: CurriedFunc<NULL> {

        public CurriedAction SetFunc ( Action<object []> caller ) {
            SetFunc(( object [] objs ) => { caller(objs); return NULL.Null; });
            return this;
        }
    }
    public static class Curry {
        public static CurriedAction Curring (
            Action<object []> caller ) {
            return new CurriedAction().SetFunc(caller);
        }

        public static CurriedFunc<R> Curring<R> (
            Func<object [], R> caller ) where R : class {
            return new CurriedFunc<R>().SetFunc(caller);
        }
    }
}