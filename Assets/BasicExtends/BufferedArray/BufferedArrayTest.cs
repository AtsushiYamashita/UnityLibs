using UnityEngine;
using UnityEngine.Assertions;
using BasicExtends;


public class BufferedArrayTest : TestComponent<BufferedArray<string>> {

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
        Assert.IsTrue(arr.Arr.Length == SIZE);
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
        Assert.IsTrue(arr.Arr.Length == SIZE);
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
        Assert.IsTrue(arr.Arr.Length == SIZE);
        var time = arr.Arr.Length;
        for (int i = 0; i < time + 1; i++) {
            arr.Add("test", ( e ) => { return e.Length < 1; });
        }
        return arr.Arr.Length == SIZE + add_size ? "" : "指定した容量の追加";
    }

    public string ForEachTest () {
        return "fail";
    }

    public string MapTest () {
        return "fail";
    }

    public string RemoveTest () {
        return "fail";
    }

    public string ExpendTestD () {
        return "fail";
    }

    public string ExpendTest0 () {
        return "fail";
    }

    public string ExpendTest1 () {
        return "fail";
    }
    public string ExpendTest1000 () {
        return "fail";
    }

    public string SetFrontTest () {
        return "fail";
    }

    public string SetFrontTest0 () {
        return "fail";
    }
}
