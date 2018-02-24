using UnityEngine;
using System;

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
            Logger.Log(" ====== Test<{0}> start ====== ", mType.Name);
            var methods = this.GetType().GetMethods();
            foreach (var m in methods) {
                var name = m.Name;
                if (name.IndexOf("Test") == -1) { continue; }
                Print(name, (string) m.Invoke(this, null));
            }
            Logger.Log(" ====== Test<{0}> end ====== ", mType.Name);
        }

        private void Print ( string name, string ret ) {
            size++;
            if (ret.Length == 0) {
                ok++;
                Debug.LogFormat("Test:{0}に{1}しました。", name, "成功");
                return;
            }
            Debug.LogErrorFormat("Test:{0}に{1}しました。({2})", name, "失敗", ret);
        }
    }
}

/*
  public string Test () {
            return "false";
        }
 */
