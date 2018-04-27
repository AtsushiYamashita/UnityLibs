using UnityEngine;
using BasicExtends;


public class BufferedArrayTest : TestComponent {

    private static readonly uint SIZE = 5;

    public string ConstructTest () {
        var arr = new BufferedArray<string>();
        return arr.Arr.Length == SIZE ? "" : "配列のサイズ指定に失敗しています";
    }

    public string ConstructTest_1 () {
        float size = 1;
        var arr = new BufferedArray<string>((uint) size);
        var len = arr.Arr.Length;
        return len == size ? "" : "生成サイズ指定";
    }
    public string ConstructTest_1000 () {
        float size = 1000;
        var arr = new BufferedArray<string>((uint)size);
        var len = arr.Arr.Length;
        return len == size ? "" : "生成サイズ指定";
    }

    public string AddTest_1 () {
        var arr = new BufferedArray<string>();
        var time = arr.Arr.Length;
        for (int i = 0; i < time + 1; i++) {
            arr.Add("test");
        }
        var len = arr.Arr.Length;
        return (len == SIZE * 2) ? "" : "デフォルトの容量追加";
    }

    public string AddTest_2 () {
        uint add_size = 2;
        var arr = new BufferedArray<string> {
            AddSize = add_size
        };
        var time = arr.Arr.Length;
        for (int i = 0; i < time + 1; i++) {
            arr.Add("test", ( e ) => { return e.Length < 1; });
        }
        return arr.Arr.Length == SIZE + add_size ? "" : "指定した容量の追加";
    }

    public string AddTest_200 () {
        uint add_size = 200;
        var arr = new BufferedArray<string> {
            AddSize = add_size
        };
        var time = arr.Arr.Length;
        for (int i = 0; i < time + 1; i++) {
            arr.Add("test", ( e ) => { return e.Length < 1; });
        }
        return arr.Arr.Length == SIZE + add_size ? "" : "指定した容量の追加";
    }

    public string ForEachTest () {
        return Fail();
    }

    public string MapTest () {
        return Fail();
    }

    public string RemoveTest () {
        return Fail();
    }

    public string ExpendTestD () {
        return Fail();
    }

    public string ExpendTest0 () {
        return Fail();
    }

    public string ExpendTest1 () {
        return Fail();
    }
    public string ExpendTest1000 () {
        return Fail();
    }

    public string SetFrontTest () {
        return Fail();
    }

    public string SetFrontTest0 () {
        return Fail();
    }

    public string TryGetTest () {
        return Fail();
    }

    public string TryGetTest2 () {
        return Fail();
    }
}
