namespace BasicExtends {
    using System.Collections.Generic;

    /// <summary>
    /// シナリオスクリプトをFactoryに読ませるためのQueueラッパー
    /// </summary>
    public class ScriptTokenizer: Queue<string> {
        public string Head { get; private set; }
        public int Line { get; private set; }
        public const string EOS = "EOS";

        /// <summary>
        /// デバッグのための行数確認に使う
        /// </summary>
        public const string LINE = "LINE@";

        private ScriptTokenizer () { } // seal

        public bool Next () {
            if (this.Count < 1) { throw new System.Exception(); }
            var ret = Head;
            string temp;
            while (true) {
                temp = Dequeue();
                if (temp.IndexOf(LINE) < 0) { break; }
                Line = int.Parse(temp.Replace(LINE, ""));
            }
            Head = temp;
            return Head != EOS;
        }

        public static ScriptTokenizer Load ( string script ) {
            var ret = new ScriptTokenizer();
            var arr = script.Replace("\n", "\n ").Split(" ".ToCharArray());
            int count = 0;
            foreach (var e in arr) {
                var token = e.Replace("\n", LINE + count++);
                ret.Enqueue(token); }
            ret.Enqueue(EOS);
            ret.Head = string.Empty;
            return ret;
        }
    }
}