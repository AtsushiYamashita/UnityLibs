using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using BasicExtends;

//[Serializable]
//public class SaveDataSample {
//    public int Id;
//    public string Name;
//    public byte [] ByteArr;
//    [NonSerialized()]
//    public float TemporaryData;
//}


public class BinarySerial :Singleton<BinarySerial> {

    private static bool mSetuped = false;

    private static void Setup () {
        if (mSetuped) { return; }
        mSetuped = true;
#if UNITY_IPHONE
        Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
#endif
    }

    private static string DataPath ( string filename ) {
        return Application.dataPath + "/" + filename + ".bin"; ;
    }

    public  static void Save ( string filename, object obj ) {
        Setup();
        string path = DataPath(filename);
        using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write)) {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, obj);
        }
    }

    public static T Load<T> ( string filename ) where T:class {
        Setup();
        string path = DataPath(filename);
        object obj = null;
        using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read)) {
            BinaryFormatter bf = new BinaryFormatter();
            obj = bf.Deserialize(fs) ;
        }
        return obj as T;
    }

    public static byte[] Serialize (object obj) {
        Setup();
        var ms = new MemoryStream();
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(ms, obj);
        return ms.ToArray();
    }

    public static T Deserialize<T> ( byte[] bytes ) where T : class {
        Setup();
        object obj = null;
        var ms = new MemoryStream(bytes);
        BinaryFormatter bf = new BinaryFormatter();
        obj = bf.Deserialize(ms);
        return obj as T;
    }
}
