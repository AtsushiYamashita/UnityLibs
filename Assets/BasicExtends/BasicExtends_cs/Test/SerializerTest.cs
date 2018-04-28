using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BasicExtends;

public class SerializerTest: TestComponent {

    public string String_Test_Int0 () {
        int num = 0;
        Serializer.SetDatatype(Serializer.SerialType.String);

        var bytes = Serializer.Serialize(num);
        var arr = bytes.ToArray();
        var arr2 = ByteList.Gen()
            .Add(System.BitConverter.GetBytes(Serializer.GetTypeId(0.GetType().Name).Value))
            .Add(System.BitConverter.GetBytes(0)).ToArray();

        if (arr.Length != arr2.Length) { return Fail("Failed convert ({0},{1})", arr.Length, arr2.Length); }
        for (int i = 0; i < arr.Length; i++) {
            if (arr [i] != arr2 [i]) { return Fail("Failed bytes in {0}", i); }
        }

        var s2 = Serializer.Deserialize<int>(bytes);
        if (s2.Key == false) { return Fail("Faild deserialize" + s2.ToJson()); }
        if (s2.Value != 0) { return Fail("Faild deserialize" + s2.ToJson()); }
        return Pass();
    }


    public string String_Test_Int1 () {
        int num = 1;
        int target = 1;
        Serializer.SetDatatype(Serializer.SerialType.String);

        var bytes = Serializer.Serialize(num);
        var arr = bytes.ToArray();
        var arr2 = ByteList.Gen()
            .Add(System.BitConverter.GetBytes(Serializer.GetTypeId(0.GetType().Name).Value))
            .Add(System.BitConverter.GetBytes(target)).ToArray();

        if (arr.Length != arr2.Length) { return Fail("Failed convert ({0},{1})", arr.Length, arr2.Length); }
        for (int i = 0; i < arr.Length; i++) {
            if (arr [i] != arr2 [i]) { return Fail("Failed bytes in {0}", i); }
        }

        var s2 = Serializer.Deserialize<int>(bytes);
        if (s2.Key == false) { return Fail("Faild deserialize" + s2.ToJson()); }
        if (s2.Value != target) { return Fail("Faild deserialize" + s2.ToJson()); }
        return Pass();
    }

    public string String_Test_Int_Somany () {
        int num = 329482958;
        int target = num;
        Serializer.SetDatatype(Serializer.SerialType.String);

        var bytes = Serializer.Serialize(num);
        var s2 = Serializer.Deserialize<int>(bytes);
        if (s2.Key == false) { return Fail("Faild deserialize" + s2.ToJson()); }
        if (s2.Value != target) { return Fail("Faild deserialize" + s2.ToJson()); }
        return Pass();
    }
}
