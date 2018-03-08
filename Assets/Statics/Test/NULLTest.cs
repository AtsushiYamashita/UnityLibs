using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BasicExtends;

public class NULLTest: TestComponent<NULL> {

    public string ToStringTest () {
        if (NULL.Null.ToString() != "NULL") { return "Error, this isn't null."; }
        return "";
    }

    public string NullArrayTest () {
        var arr = NULL.NullArr;
        if (arr.Length != 0) { return "ゴミが入ってます"; }
        foreach (var a in arr) {
            return "ゴミが入ってます";
        }
        try {
            arr [0] = new object();
            return "要素が入れられてしまいます";
        } catch {

        }
        return "";
    }

    public string NULLCheckTest_isNull () {
        string s = null;
        if (s.IsNotNull()) { return "nullを見逃しています"; }
        var n = NULL.Null;
        if (n.IsNotNull()) { return "nullを見逃しています"; }
        return "";
    }

    public string NULLCheckTest_Default () {
        string s = null;
        string s2 = s.Default("I was null");
        if (s2 != "I was null") { return "デフォルト設定ができていません"; }
        return "";
    }
}
