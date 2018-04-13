using UnityEngine;
using UnityEngine.Assertions;
using System.Text;
using System.Linq;

namespace BasicExtends {

    /// <summary>
    /// 作った計算処理のテストを簡易的に行う。
    /// すべてのテストはStartタイミングで処理される。
    /// 通常のAssertを使ったテストにしないのは、
    /// TDDをまわしやすくするため。
    /// リズムのためにエラー以外の情報が必要だった。
    /// 
    /// このコンポネントを使うには
    /// このクラスを継承する形でxxxTestという関数を実装する。
    /// ただし関数はpublicであることが必要で、
    /// 内部でコールバックに対してFailやPassで返答を行う必要がある。
    /// </summary>
    public class TestComponentMulti: MonoBehaviour {

        public class Result {
            private System.Action<string, string> mAction;
            private string mName;
            private int mTimes = 0;
            public Result ( System.Action<string, string> action, string name ) {
                mAction = action;
                mName = name;
            }
            public void Invoke ( string str ) {
                if (IsActive() == false) { return; }
                mTimes++;
                mAction(mName, str);
            }
            public bool IsActive () { return mTimes == 0; }
        }

        int _Size = 1;
        int _ResEnd = 0;
        int _Ok = 0;
        StringBuilder _Buffer = new StringBuilder();

        protected virtual void Init () { }
        protected virtual void Close () { }

        private void ResultAction ( string name, string res ) {
            // Debug.Log("Result:" + res);
            _ResEnd++;
            var based = "{0}に{1}しました。{2}{3}{4}\n";
            if (res.Length != 0) {
                _Buffer.AppendFormat(
                    based, name, "失敗",
                    "(", res, ")");
            } else {
                _Ok++;
                _Buffer.AppendFormat(
                    based, name, "成功",
                    "", "", "");
            }
            // Debug.LogFormat("Result call(res.{0}/size.{1})", resEnd, size);
            if (_ResEnd < _Size) { return; }
            if (_ResEnd > _Size) { Debug.LogError("Error, res size bigger then test size."); }
            TestResult(ref _Buffer);
        }

        private void TestProcess () {
            var methods = GetType().GetMethods().Where((m)=> {
                return m.Name.IndexOf("Test") != -1;
            });
            object [] arr = new object [1];

            _Size = methods.Count() ;
            foreach (var m in methods) {
                var name = m.Name;
                var res = new Result(ResultAction, name); ;
                arr [0] = res;
                try {
                    m.Invoke(this, arr);
                } catch (System.Exception e) {
                    res.Invoke("Error" + e.Message);
                }
            }
        }

        private void TestResult ( ref StringBuilder sb ) {
            var t_name = GetType().Name;
            if (t_name.IndexOf("1") > -1) {
                t_name = t_name.Substring(0, t_name.Length - 2);
            }

            var format = " Test [ {0} ] end ({1}/{2})\n{3}\n ";
            var str = string.Format(format, t_name, _Ok, _Size, sb.ToString());
            var type = _Ok - _Size == 0 ? DebugLog.Log : DebugLog.Error;
            type.Print(str);
            Close();
        }

        private void Start () {
            Init();
            TestProcess();
        }


        protected static string Fail () {
            return "Fail";
        }

        protected static string Fail ( int data ) {
            return "Fail : " + data;
        }

        protected static string Fail ( float data ) {
            return "Fail : " + data;
        }

        protected static string Fail ( string str ) {
            return "Fail : " + str;
        }

        protected static string Fail ( string str, params object [] obj ) {
            return "Fail : " + string.Format(str, obj);
        }

        protected static string Pass () {
            return "";
        }
    }
}

/*
    public void FailTest(string name, Action<string,string> result ) {
        result.Invoke(name, Fail());
    }

 */
