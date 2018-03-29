using UnityEngine;
using System.Text;

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

        int _Size = 0;
        int _ResEnd = 0;
        int _Ok = 0;
        StringBuilder _Buffer = new StringBuilder();


        protected virtual void Init () { }
        protected virtual void Close () { }

        private void Result ( string name, string res ) {
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
            var methods = GetType().GetMethods();
            object [] arr = new object [2];
            arr [1] = (System.Action<string, string>) Result;
            foreach (var m in methods) {
                var name = m.Name;
                if (name.IndexOf("Test") == -1) { continue; }
                arr [0] = name;
                _Size++;
                try {
                    m.Invoke(this, arr);
                } catch (System.Exception e) {
                    Result(name, "Error:" + e.Message);
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


        protected string Fail () {
            return "Fail";
        }

        protected string Fail ( int data ) {
            return "Fail : " + data;
        }

        protected string Fail ( float data ) {
            return "Fail : " + data;
        }

        protected string Fail ( string str ) {
            return "Fail : " + str;
        }

        protected string Fail ( string str, params object [] obj ) {
            return "Fail : " + string.Format(str, obj);
        }

        protected string Pass () {
            return "";
        }
    }
}

/*
  public string Test () {
            return "false";
        }
 */
