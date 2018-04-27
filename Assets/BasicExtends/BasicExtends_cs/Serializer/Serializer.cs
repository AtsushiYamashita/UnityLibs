namespace BasicExtends {

    using System;
    using System.Collections.Generic;
    using SerializeImp;

    /// <summary>
    /// 直列化からの復帰に失敗した場合のエラーです
    /// </summary>
    public class UnDeserializableException : Exception { }

    /// <summary>
    /// 指定した情報の直列化を行います
    /// 基本的に ([総情報長][型辞書長][型辞書配列(len:str)]) [型番号][[情報長]情報]です。
    /// </summary>
    public class Serializer: Singleton<Serializer> {

        public ISerializer Implement { set; private get; }
        private List<string> mTypeIndex = new List<string>();

        public enum SerialType { Binary,Binary2,String }

        public static Serializer SetDatatypeBinary(SerialType type)
        {
            if (type == SerialType.Binary) { Instance.Implement = new SerializeImp.Binary(); }
            if (type == SerialType.Binary2) { Instance.Implement = new SerializeImp.Binary2(); }
            if (type == SerialType.String) { Instance.Implement = new SerializeImp.String(); }
            return Instance;
        }

        public static ByteList GetTypeList () {
            return Instance.Implement.GetTypeList(Instance.mTypeIndex);
        }

        public static int GetTypeId(string type)
        {
            var index = Instance.mTypeIndex.IndexOf(type);
            if (index > 0) { return index; }
            index = Instance.mTypeIndex.Count;
            Instance.mTypeIndex.Add(type);
            return index;
        }

        /// <summary>
        /// 直列化された配列からインスタンスを作るための手続きを登録する
        /// </summary>
        public static Serializer AssignDeserializer ( string match, Func<ByteList, object> func ) {
            Instance.Implement.SetDeserializer(match, func);
            return Instance;
        }

        /// <summary>
        /// 直列化するための手続きを登録する
        /// </summary>
        public static Serializer AssignSerializer ( string match,  Func<object, ByteList> func ) {
             Instance.Implement.SetSerializer(match, func);
            return Instance;
        }

        public static ByteList Serialize ( object obj ) {
            return Instance.Implement.ToSerial(obj);
        }

        public static CheckedRet<object> Deserialize ( ByteList bytes ) {
            return Instance.Implement.Deserial(bytes);
        }
    }
}
