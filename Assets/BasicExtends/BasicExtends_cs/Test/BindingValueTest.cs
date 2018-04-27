using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BasicExtends {

    public class BindingValueTest: TestComponent {

        public string BindTest () {
            string old = "", now = "";
            //var val = "".Bind(( o, n ) => { old = o; now = n; });
            if (old != now) { return "望まない処理かゴミが発生しています。"; }
            return "";
        }

        public string BindingActionTest () {
            string old = "", now = "";
            var val = "".Bind(( o, n ) => { old = o; now = n; });
            var isEmpty = false;
            val.Bind(( o, n ) => { isEmpty = val.Get() == ""; });
            val.Set("updated");
            if (isEmpty) { return "拘束された関数の呼び出しタイミングが不適切です。"; }
            if (old != "") { return "望まない変更が発生しています"; }
            if (now != "updated") { return "望まない変更が発生しています"; }
            if (val.Get() != "updated") { return "発生すべき変更が発生していません"; }
            return "";
        }

        public string SyncBindTest () {
            int aUpdate = 0;
            int bUpdate = 0;
            var a = "a".Bind(( o, n ) => { aUpdate++; });
            var b = "b".Bind(( o, n ) => { bUpdate++; });
            a.Set("a2");
            b.Set("b2");
            a.BindSync(b);
            if (a.Get() != "a2") { return "正しく変更が反映されていません"; }
            if (b.Get() != "b2") { return "正しく変更が反映されていません"; }
            if (aUpdate != 1) { return "正しくバインディングが発生していません。"; }
            if (bUpdate != 1) { return "正しくバインディングが発生していません。"; }
            a.Set("3");
            if (a.Get() != "3") { return "正しく変更が反映されていません"; }
            if (b.Get() != "3") { return "正しく変更が反映されていません"; }
            if (aUpdate != 2) { return "正しくバインディングが発生していません。"; }
            if (bUpdate != 2) { return "正しくバインディングが発生していません。"; }
            return "";
        }

    }
}
