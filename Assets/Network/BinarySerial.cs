using System;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using BasicExtends;


[Serializable]
public class SaveData {
    public int Id;
    public string Name;
    [NonSerialized()]
    public float TemporaryData;
}



public class BinarySerial: MonoBehaviour {

    private static string SavePath { set; get; }

    private bool IsEnd = false;

    void Update () {
        if (IsEnd) { return; }

        // iOSでは下記設定を行わないとエラーになる
#if UNITY_IPHONE
        Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
#endif

        ///// 保存 /////////////////////////////////////
        {
            SaveData obj = new SaveData {
                Id = 1,
                Name = "tempura",
                TemporaryData = 3.5f
            };

            using (FileStream fs = new FileStream(SavePath, FileMode.Create, FileAccess.Write)) {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, obj);
            }

            DebugLog.Log.Print("[Save]Id:" + obj.Id);
            DebugLog.Log.Print("[Save]Name:" + obj.Name);
            DebugLog.Log.Print("[Save]TemporaryData:" + obj.TemporaryData);
        }

        ///// 読み込み /////////////////////////////////////
        {
            // 読み込み
            SaveData obj = null;
            using (FileStream fs = new FileStream(SavePath, FileMode.Open, FileAccess.Read)) {
                BinaryFormatter bf = new BinaryFormatter();
                obj = bf.Deserialize(fs) as SaveData;

                DebugLog.Log.Print("[Load]Id:" + obj.Id);
                DebugLog.Log.Print("[Load]Name:" + obj.Name);
                DebugLog.Log.Print("[Load]TemporaryData:" + obj.TemporaryData);
            }
        }
        IsEnd = true;
    }

    // Use this for initialization
    void Start () {
        SavePath = Application.dataPath + "/save.bytes";
    }


    public void Save ( object obj ) {
        using (FileStream fs = new FileStream(SavePath, FileMode.Create, FileAccess.Write)) {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, obj);
        }
    }

    public T Load<T> () where T:class {
        object obj = null;
        using (FileStream fs = new FileStream(SavePath, FileMode.Open, FileAccess.Read)) {
            BinaryFormatter bf = new BinaryFormatter();
            obj = bf.Deserialize(fs) ;
        }
        return obj as T;
    }
}
