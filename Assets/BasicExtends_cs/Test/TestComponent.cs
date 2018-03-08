using UnityEngine;
using System;
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
    /// Tにテスト対象のクラスを指定し、
    /// このクラスを継承する形でxxxTestという関数を実装するだけ。
    /// ただし関数はstringを返し、publicであることが必要。
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TestComponent<T>: MonoBehaviour {
        Type mType;

        int size = 0;
        int ok = 0;

        private void Start () {
            mType = typeof(T);
            var methods = this.GetType().GetMethods();
            StringBuilder sb = new StringBuilder();
            foreach (var m in methods) {
                var name = m.Name;
                if (name.IndexOf("Test") == -1) { continue; }
                sb.Append(Print(name, (string) m.Invoke(this, null)));
            }
            var t_name = mType.Name;
            if (t_name.IndexOf("1") > -1) {
                t_name = t_name.Substring(0, t_name.Length - 2);
            }
            var format = " Test [ {0} ] end ({1}/{2})\n{3}\n ";
            var str = string.Format(format, t_name, ok, size, sb.ToString());
            var type = ok - size == 0 ? DebugLog.Log : DebugLog.Error;
            type.Print(str);
        }

        private string Print ( string name, string ret ) {
            size++;
            var based = "{0}に{1}しました。{2}{3}{4}\n";
            if (ret.Length != 0) {
                return string.Format(
                    based, name, "失敗",
                    "(", ret, ")");
            }
            ok++;
            return string.Format(
                based, name, "成功",
                "", "", "");
        }

        protected string Fail () {
            return "Fail";
        }

        protected string Fail ( string str ) {
            return "Fail : " + str;
        }

        protected string Fail ( string str, params object [] obj ) {
            return "Fail : " + string.Format(str, obj);
        }

        protected string Pass() {
            return "";
        }
    }
}

/*
  public string Test () {
            return "false";
        }
 */
