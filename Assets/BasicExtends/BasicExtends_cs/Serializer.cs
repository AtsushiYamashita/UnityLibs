namespace BasicExtends {

    using System;
    using System.Collections.Generic;

    public interface ISerializer {
        ByteList GetTypeList ( List<string> mTypeIndex );
        Serializer SetSerializer ( string type, Func<object, ByteList> func );
        Serializer SetDeserializer ( string type, Func<ByteList, object> func );
        CheckedRet<object> Deserial (int id , ByteList bytes );
        ByteList ToSerial ( object obj );
    }

    /// <summary>
    /// 指定した情報の直列化を行います
    /// 基本的に ([総情報長][型辞書長][型辞書配列(len:str)]) [型番号][[情報長]情報]です。
    /// </summary>
    public class Serializer: Singleton<Serializer> {

        public ISerializer Implement { set; private get; }
        private List<string> mTypeIndex = new List<string>();
        
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
            return Instance.Implement.SetDeserializer(match, func);
        }

        /// <summary>
        /// 直列化するための手続きを登録する
        /// </summary>
        public static Serializer AssignSerializer ( string match,  Func<object, ByteList> func ) {
            return Instance.Implement.SetSerializer(match, func);
        }

        public static ByteList Serialize ( object obj ) {
            return Instance.Implement.ToSerial(obj);
        }

        public static CheckedRet<object> Deserialize ( ByteList bytes ) {
            return Instance.Implement.Deserial(bytes.DropInt32(), bytes);
        }
    }

    /// <summary>
    /// 直列化からの復帰に失敗した場合のエラーです
    /// </summary>
    public class UnDeserializableException: Exception { }

}
