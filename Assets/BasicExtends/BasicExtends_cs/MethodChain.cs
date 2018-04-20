namespace BasicExtends {
    using System;
    using System.Collections.Generic;
    using UnityEngine.Assertions;

    public interface IMethodChain<A> where A : class {
        void Invoke ( A arg );
        List<Func<A, bool>> GetProcessHolder ();
        void SetDefault ( Func<A, bool> action );
    }

    /// <summary>
    /// このメソッドチェーン実装は、
    /// 分岐を関数オブジェクトとして保持するSwitch文のようなもの。
    /// 動的に処理を追加、削除したりできる。
    /// ただし、処理はあまり高速ではない。
    /// 
    /// メソッドチェーンに入れる関数はbool値を戻す
    /// これは「この関数で処理を完了するか(isEndProcess)」として解釈する。
    /// 
    /// つまりtrueを返せば、他のチェーンでの処理を行わないでたらいまわしを終了する。
    /// 逆に、処理をしない状態でtrueを返すと予期しないことが発生する。
    /// 
    /// すべてのチェーンに入った関数を処理して、
    /// それでも処理が行われていなければDefault処理関数を呼び出す。
    /// ただし、上記の「処理が完了している」状態であればDefaultは呼び出されない。
    /// </summary>
    /// <typeparam name="A"></typeparam>
    public class MethodChain<A> : IMethodChain<A> where A:class {
        private List<Func<A, bool>> mList = new List<Func<A, bool>>();
        private Func<A, bool> mDefault = delegate ( A a ) { throw new Exception(a.ToString()); };

        public void Invoke (A arg) {
            foreach(var func in mList) {
                var end = func(arg);
                if (end) { return; }
            }
            Assert.IsNotNull(mDefault);
            mDefault(arg);
        }

        /// <summary>
        /// 名前がGetListでないのは、
        /// 高速化のためにDictionaryで処理する場合もあろうくらいの考えです。
        /// </summary>
        /// <returns></returns>
        public List<Func<A, bool>> GetProcessHolder () {
            return mList;
        }

        public void SetDefault ( Func<A, bool> action ) {
            mDefault = action;
        }

    }
}
