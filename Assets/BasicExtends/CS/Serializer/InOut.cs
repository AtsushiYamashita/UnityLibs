namespace BasicExtends
{
    using System.IO;
    using System.Collections.Generic;
    using UnityEngine;

    public class InOut : Singleton<InOut>
    {

        /// <summary>
        /// シリアライズして保管するデータと補足情報辞書のセットです。
        /// </summary>
        public class SaveData
        {
            public string DataType { private set; get; }
            public object Data { private set; get; }
            public StringDict Dict { private set; get; }
            public SaveData(object data)
            {
                Dict = new StringDict();
                DataType = data.GetType().Name;
                Data = data;
            }
        }

        private static byte[] buf = new byte[1024];
        private LifedThread mThread = null;
        private Stack<SaveData> mDataStack = new Stack<SaveData>();

        /// <summary>
        /// Msgで送られたデータをファイルに書き込む
        /// 基本的に ([総情報長][Hash][型辞書長][型辞書配列(len:str)]) [型番号][[情報長]情報]です。
        /// </summary>
        public void Write(string filename)
        {
            if (mThread != null || mDataStack.Count == 0) { return; }
            mThread = ThreadManager.Get();
            string path = DataPath(filename);
            int data_first = mDataStack.Count;

            FileStream fs_w = new FileStream(path, FileMode.Create, FileAccess.Write);
            mThread.Work("InOut write", null,(e) =>
            {
                if (mDataStack.Count == 0 || fs_w.CanWrite == false)
                {
                    return ThreadState.End;
                }
                var obj = mDataStack.Pop().Data;
                var bytes = Serializer.GetTypeList();
                bytes.Add(Serializer.Serialize(obj));
                bytes.AddHead(bytes.Count);
                fs_w.WriteAsync(bytes.ToArray(), 0, bytes.Count);
                return ThreadState.Continue;
            });
        }

        /// <summary>
        /// [総情報長]
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private ByteList ReadFromFile(string filename)
        {
            string path = DataPath(filename);
            FileStream fs_r = new FileStream(path, FileMode.Open, FileAccess.Read);
            if (mDataStack.Count == 0 || fs_r.CanRead == false)
            {
                return null;
            }
            fs_r.Read(buf, 0, sizeof(int));

            ByteList data = new ByteList();
            var data_size = System.BitConverter.ToInt32(buf, 0);

            byte[] b = new byte[data_size];
            fs_r.Read(b, sizeof(int), data_size);
            data.AddRange(b);
            return data;
        }

        private bool CheckHash(ByteList data)
        {
            throw new System.NotImplementedException();
        }

        private ByteList ParseToDic(ByteList data)
        {
            var length = data.DropInt32();
            for (int i = 0; i < length; i++)
            {
                var size = data.DropInt32();
                var str = data.DropString(0, size);
                Serializer.GetTypeId(str);
            }
            return data;
        }


        /// <summary>
        /// 読み取ったデータを指定した先にMsgで送る
        /// 基本的に ([総情報長][Hash][型辞書長][型辞書配列(len:str)]) [型番号][[情報長]情報]です。
        /// </summary>
        /// <param name="filename"></param>
        public void Read(string filename, string to, string a_s, string act)
        {
            if (mThread != null) { return; }
            mThread = ThreadManager.Get();

            var data = ReadFromFile(filename);
            if (data == null || CheckHash(data) == false) { return; }
            ParseToDic(data);

            mThread.Work("InOut Read", null,(e) =>
            {
                if (data.Count < 1)
                {
                    return ThreadState.End;
                }
                var type_data = Serializer.Deserialize(data);
                Msg.Gen().Set(Msg.TO, to)
                    .Set(Msg.AS, a_s)
                    .Set(Msg.ACT, act)
                    .SetObjectData(type_data).Pool();
                return ThreadState.Continue;
            });
        }

        private static string DataPath(string filename)
        {
            return Application.dataPath + "/" + filename + ".bin"; ;
        }

    }
}
