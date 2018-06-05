namespace BasicExtends {

    using System;
    using System.Collections.Generic;
    using SerializeImp;
    using UnityEngine.Assertions;

    /// <summary>
    /// 直列化からの復帰に失敗した場合のエラーです
    /// </summary>
    public class UnDeserializableException: Exception { }

    /// <summary>
    /// 指定した情報の直列化を行います
    /// 基本的に ([総情報長][型辞書長][型辞書配列(len:str)]) [型番号][[情報長]情報]です。
    /// </summary>
    public class Serializer: Singleton<Serializer> {

        public ISerializer Implement { set; private get; }
        private List<string> mTypeIndex = new List<string>();

        public enum SerialType { Binary, Binary2, String, YetSet }

        public SerializeImp.String mStr = null;
        public SerializeImp.Binary mBin = null;
        public SerializeImp.Binary2 mBin2 = null;

        public static Serializer SetDatatype ( SerialType type ) {
            if (type == SerialType.Binary) {
                Instance.mBin = Instance.mBin ?? new SerializeImp.Binary();
                Instance.Implement = Instance.mBin;
            }
            if (type == SerialType.Binary2) {
                Instance.mBin2 = Instance.mBin2 ?? new SerializeImp.Binary2();
                Instance.Implement = Instance.mBin2;
            }
            if (type == SerialType.String) {
                Instance.mStr = Instance.mStr ?? new SerializeImp.String();
                Instance.Implement = Instance.mStr;
            }
            return Instance;
        }

        public static ByteList GetTypeList () {
            return Instance.Implement.GetTypeList(Instance.mTypeIndex);
        }

        public static Pair<bool, int> GetTypeId ( object obj ) {
            return Instance.TypeId(obj.GetType().Name);
        }

        public static Pair<bool, int> GetTypeId ( string type ) {
            return Instance.TypeId(type);
        }

        private Pair<bool, int> TypeId ( string type ) {
            var index = mTypeIndex.IndexOf(type);
            if (index >= 0) { return Pair<bool, int>.Gen(true, index); }
            mTypeIndex.Add(type);
            index = mTypeIndex.IndexOf(type);
            //DebugLog.Log.Print("type({0},{1})", type, index);
            return Pair<bool, int>.Gen(false, index);
        }

        /// <summary>
        /// 直列化された配列からインスタンスを作るための手続きを登録する
        /// </summary>
        public static Serializer AssignDeserializer ( string match, Func<ByteList, CheckedRet<object>> func ) {
            Assert.IsNotNull(Instance);
            Assert.IsNotNull(Instance.Implement);
            Instance.Implement.SetDeserializer(match, func);
            return Instance;
        }

        /// <summary>
        /// 直列化するための手続きを登録する
        /// </summary>
        public static Serializer AssignSerializer ( string match, Func<object, ByteList> func ) {
            Assert.IsNotNull(Instance);
            Assert.IsNotNull(Instance.Implement);
            Instance.Implement.SetSerializer(match, func);
            return Instance;
        }

        public static ByteList Serialize ( object obj ) {
            return Instance.Implement.ToSerial(obj);
        }

        public static CheckedRet<object> Deserialize ( ByteList bytes ) {
            return Instance.Implement.Deserial(bytes);
        }

        public static CheckedRet<T> Deserialize<T> ( byte [] bytes ) {
            var b = ByteList.Zero.Add(bytes);
            return Deserialize<T>(b);
        }

        public static CheckedRet<T> Deserialize<T> ( ByteList bytes ) {
            var v = Instance.Implement.Deserial(bytes);
            if (v.Key == false) {
                return CheckedRet<T>.Fail();
            }
            try {
                var val = (T) v.Value;
                return CheckedRet<T>.Gen(true, val);
            } catch (Exception e) {
                DebugLog.Error.Print("parse fail<{0}>{1}", typeof(T).Name, e);
                return CheckedRet<T>.Fail();
            }
        }
    }
}
