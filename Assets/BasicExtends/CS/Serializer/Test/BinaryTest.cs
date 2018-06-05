using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BasicExtends;

public class BinaryTest : TestComponent {

    public string IntTest_LittleNumber () {
        Serializer.SetDatatype(Serializer.SerialType.Binary);
        var encoded = Serializer.Serialize(1);
        var decoded = Serializer.Deserialize<System.Int32>(encoded);
        if (decoded.Key == false) { return Fail("Parse fail"); }
        if (decoded.Value != 1) { return Fail("Parsed value is " + decoded.Value); }
        return Pass();
    }

    public string IntTest_BigNumber () {
        Serializer.SetDatatype(Serializer.SerialType.Binary);
        var encoded = Serializer.Serialize(19923878);
        var decoded = Serializer.Deserialize<System.Int32>(encoded);
        if (decoded.Key == false) { return Fail("Parse fail"); }
        if (decoded.Value != 19923878) { return Fail("Parsed value is " + decoded.Value); }
        return Pass();
    }

    public string IntTest_Negative () {
        Serializer.SetDatatype(Serializer.SerialType.Binary);
        var encoded = Serializer.Serialize(-19923878);
        var decoded = Serializer.Deserialize<System.Int32>(encoded);
        if (decoded.Key == false) { return Fail("Parse fail"); }
        if (decoded.Value != -19923878) { return Fail("Parsed value is " + decoded.Value); }
        return Pass();
    }

    public string FloatTest_little () {
        Serializer.SetDatatype(Serializer.SerialType.Binary);
        var encoded = Serializer.Serialize(3.1415f);
        var decoded = Serializer.Deserialize<float>(encoded);
        if (decoded.Key == false) { return Fail("Parse fail"); }
        if (decoded.Value != 3.1415f) { return Fail("Parsed value is " + decoded.Value); }
        return Pass();
    }

    public string FloatTest_Negative () {
        Serializer.SetDatatype(Serializer.SerialType.Binary);
        var encoded = Serializer.Serialize(-3.1415f);
        var decoded = Serializer.Deserialize<float>(encoded);
        if (decoded.Key == false) { return Fail("Parse fail"); }
        if (decoded.Value != -3.1415f) { return Fail("Parsed value is " + decoded.Value); }
        return Pass();
    }


    public string StringTest_Little () {
        Serializer.SetDatatype(Serializer.SerialType.Binary);
        var encoded = Serializer.Serialize("test");
        var decoded = Serializer.Deserialize<string>(encoded);
        if (decoded.Key == false) { return Fail("Parse fail"); }
        if (decoded.Value != "test") { return Fail("Parsed value is " + decoded.Value); }
        return Pass();
    }

    public string StringTest_Big () {
        Serializer.SetDatatype(Serializer.SerialType.Binary);
        var encoded = Serializer.Serialize("3.14159265358979ajtiesajtljs");
        var decoded = Serializer.Deserialize<string>(encoded);
        if (decoded.Key == false) { return Fail("Parse fail"); }
        if (decoded.Value != "3.14159265358979ajtiesajtljs") { return Fail("Parsed value is " + decoded.Value); }
        return Pass();
    }

    public string StringTest_StringOnly () {
        Serializer.SetDatatype(Serializer.SerialType.Binary);
        var msg = Msg.Gen()
            .Set(Msg.TO, "test")
            .Set(Msg.ACT, "test");
        var encoded = Serializer.Serialize(msg);
        var decoded = Serializer.Deserialize<Msg>(encoded);
        if (decoded.Key == false) { return Fail("Parse fail"); }
        var msg2 = decoded.Value;
        if (decoded.Value == null) { return Fail("Parse fail Null"); }

        if (msg2.Unmatch(Msg.TO, "test")) { return Fail("Parsed value is " + decoded.Value); }
        return Pass();
    }

    public string StringTest_WithString () {
        Serializer.SetDatatype(Serializer.SerialType.Binary);
        var msg = Msg.Gen()
            .Set(Msg.TO, "test")
            .Set(Msg.ACT, "test")
            .Set("data2json", "False")
            .SetObjectData("test");
        var encoded = Serializer.Serialize(msg);
        var decoded = Serializer.Deserialize<Msg>(encoded);
        if (decoded.Key == false) { return Fail("Parse fail"); }
        if (decoded.Value == null) { return Fail("Parse fail NULL"); }
        var msg2 = decoded.Value;
        if (msg2.Unmatch(Msg.TO, "test")) { return Fail("Parsed value is " + decoded.Value); }
        var obj = msg2.TryObjectGet<string>();
        if (obj != "test") { return Fail("Parse mismatch"); }
        return Pass();
    }

    //public string StringTest_WithIntArray () {
    //    Serializer.SetDatatype(Serializer.SerialType.Binary);
    //    int [] arr = new int [3];
    //    for (int i = 0; i < 3; i++) {
    //        arr [i] = i;
    //    }
    //    var msg = Msg.Gen()
    //        .Set(Msg.TO, "test")
    //        .Set(Msg.ACT, "test")
    //        .Set("data2json", "False")
    //        .SetObjectData(arr);
    //    var encoded = Serializer.Serialize(msg);
    //    var decoded = Serializer.Deserialize<Msg>(encoded);
    //    if (decoded.Key == false) { return Fail("Parse fail"); }
    //    var msg2 = decoded.Value;
    //    if (msg2.Unmatch(Msg.TO, "test")) { return Fail("Parsed value is " + decoded.Value); }
    //    var obj = msg2.TryObjectGet<int []>();
    //    for (int i = 0; i < 3; i++) {
    //        if (obj [i] != i) { return Fail("Parse mismatch"); }
    //    }
    //    return Pass();
    //}

    public string Vec3Test () {
        Serializer.SetDatatype(Serializer.SerialType.Binary);
        var target = new Vec3();
        target.Z = 1;
        var encoded = Serializer.Serialize(target);
        var decoded = Serializer.Deserialize<Vec3>(encoded);
        if (decoded.Key == false) { return Fail("Parse fail"); }
        var msg2 = decoded.Value;
        if (msg2.X != 0) { return Fail("Parsed value is " + decoded.Value); }
        if (msg2.Y != 0) { return Fail("Parsed value is " + decoded.Value); }
        if (msg2.Z != 1) { return Fail("Parsed value is " + decoded.Value); }
        return Pass();
    }
    public string TrfmTest () {
        Serializer.SetDatatype(Serializer.SerialType.Binary);
        var target = Trfm.Convert(transform, Trfm.Type.World);
        var encoded = Serializer.Serialize(target);
        var decoded = Serializer.Deserialize<Trfm>(encoded);
        if (decoded.Key == false) { return Fail("Parse fail"); }
        var msg2 = decoded.Value;
        if (msg2.POS.X != 0) { return Fail("Parsed value is " + decoded.Value); }
        if (msg2.POS.Y != 1) { return Fail("Parsed value is " + decoded.Value); }
        if (msg2.POS.Z != -10) { return Fail("Parsed value is " + decoded.Value); }
        return Pass();
    }
}
