namespace BasicExtends {

    public interface IFactory {
        void SetTarget ( IFactoryReceiver c );
        void SetScript ( string str );
        bool Parse ();
        bool IsParsable { get; }
    }

    public interface IFactoryReceiver {
        void InstanceSet<T> ( T t );
    }

    public class FactoryArgs : Pair<IFactoryReceiver, ScriptTokenizer> { }

    public class ScriptUnparsable: System.Exception {
        public ScriptUnparsable () : base() { }
        public ScriptUnparsable ( string str, int line, System.Exception e )
            : base(string.Format("Error, unparsable({0}:{1})", str, line) + e) { }
    }

    public class TokenbaseParser<T>: IFactory where T : class {
        public bool IsParsable { get { return mScript == null; } }
        private IMethodChain<FactoryArgs> mChain = new MethodChain<FactoryArgs>();
        private ScriptTokenizer mScript = null;
        private FactoryArgs mArgs = new FactoryArgs();

        public void SetTarget ( IFactoryReceiver c ) {
            mArgs.Key = c;
        }

        public void SetScript ( string str ) {
            mScript = ScriptTokenizer.Load(str);
            mArgs.Value = mScript;
        }

        public bool Parse () {
            if (mScript == null) { return false; }
            if (mScript.Count < 1) { mScript = null; return false; }
            if (mScript.Next() == false) { mScript = null; return false; }
            var head = mScript.Head;
            try {
                mChain.Invoke(mArgs);
            } catch (System.Exception e) {
                throw new ScriptUnparsable(head, mScript.Line, e);
            }
            return true;
        }
    }
}