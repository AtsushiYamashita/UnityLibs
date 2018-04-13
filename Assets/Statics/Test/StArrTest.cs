using BasicExtends;

public class StArrTest: TestComponent {

    string h = StArr.BasicHead;
    string t = StArr.BasicTail;

    public string ToArrayTest () {
        var arr = StArr.To("a", 1, 2, 3, "t");
        if (!arr.GetType().IsArray) { return "配列の生成に失敗しています";  }
        return "";
    }

    public string ToObjArrTest () {
        var arr = StArr.To("a", 1, 2, 3, "t");
        // if (ar2 [0].GetType().Name != "object") 
        //   { return "型変換に失敗しています" + ar2 [0].GetType().Name; }
        // インスタンスの実装を掘り返しちゃう
        var ar3 = arr as object [];
        if (ar3 == null) { return "型変換に失敗しています"; }
        return "";
    }

    public string StringifyTestHST0 () {
        var arr = StArr.To();
        var str = arr.StringifyBasic(",");
        if (str != h + t) { return Fail("文字列化に失敗 " + str); }
        return Pass();
    }

    public string StringifyTestHST1 () {
        var arr = StArr.To("a");
        var str = arr.StringifyBasic(",");
        if (str != h + "\"a\""+ t) { return Fail("文字列化に失敗 " + str); }
        return Pass();
    }

    public string StringifyTestHST1_2 () {
        var arr = StArr.To("a","b");
        var str = arr.StringifyBasic(",");
        if (str != h + "\"a\",\"b\"" + t) { return Fail("文字列化に失敗 " + str); }
        return Pass();
    }

    public string StringifyTestHST2 () {
        var arr = StArr.To("a",1);
        var str = arr.StringifyBasic(",");
        if (str != h + "\"a\",1" + t) { return Fail("文字列化に失敗 " + str); }
        return Pass();
    }

    public string StringifyTest () {
        var arr = StArr.To("a", 1,"b");
        var str = arr.Stringify(",");
        if (str != "\"a\",1,\"b\"") { return Fail("文字列化に失敗 " + str); }
        return Pass();
    }
}
