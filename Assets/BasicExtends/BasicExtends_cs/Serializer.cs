namespace BasicExtends {

    using System;
    using System.Collections.Generic;
    using UnityEngine.Assertions;

    /// <summary>
    /// 指定した情報の直列化を行います
    /// 基本的に ([総情報長][型辞書長][型辞書配列(len:str)]) [型番号][[情報長]情報]です。
    /// </summary>
    public class Serializer: Singleton<Serializer> {

        List<string> mTypeIndex = new List<string>();
        Dictionary<int, Func<ByteList, object>> mParser = new Dictionary<int, Func<ByteList, object>>();
        Dictionary<int, Func<object, ByteList>> mSerializer = new Dictionary<int, Func<object, ByteList>>();
        
        public ByteList TypeList () {
            var count = mTypeIndex.Count;
            var bytes = ByteList.Gen().Add(count);
            foreach(var t in mTypeIndex) {
                bytes.Add(Serialize(t));
            }
            return bytes;
        }

        public void AssgnTypeDics(ByteList bytes ) {
            var len = bytes.DropInt32();
            for (int i = 0; i < len; i++) {
                var type = Deserial(bytes);
                if (type.Key == false) { throw new UnDeserializableException(); }
                mTypeIndex.Add((string)type.Value);
            }
        }

        /// <summary>
        /// 直列化された配列からインスタンスを作るための手続きを登録する
        /// </summary>
        public static Serializer AssignDeserializer ( string type, Func<ByteList, object> func ) {
            return Instance.SetDeserializer(type, func);
        }

        /// <summary>
        /// 直列化するための手続きを登録する
        /// </summary>
        public static Serializer AssignSerializer ( string type, Func<object, ByteList> func ) {
            return Instance.SetSerializer(type, func);
        }

        public static ByteList Serialize ( object obj ) {
            return Instance.ToSerial(obj);
        }

        public static CheckedRet<object> Deserialize ( ByteList bytes ) {
            return Instance.Deserial(bytes);
        }

        public Serializer SetSerializer ( string type, Func<object, ByteList> func ) {
            var id = GetTypeId(type);
            mTypeIndex.Add(type);
            mSerializer.Add(id, func);
            return this;
        }

        public Serializer SetDeserializer ( string type, Func<ByteList, object> func ) {
            var id = GetTypeId(type);
            mTypeIndex.Add(type);
            mParser.Add(id, func);
            return this;
        }

        private CheckedRet<object> Deserial ( ByteList bytes ) {
            var type_name_len = bytes.DropInt32();
            var type_name = bytes.DropString(0, type_name_len);
            var id = mTypeIndex.IndexOf(type_name);
            if (id < 0) { return CheckedRet<object>.Fail(); }
            var parser = mParser [id];
            var instance = parser(bytes);
            return new CheckedRet<object>().Set(true, instance);
        }

        /// <summary>
        /// 登録された手続を使ってオブジェクトを直列化する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        private ByteList ToSerial ( object obj ) {
            var type_n = obj.GetType().Name;
            int id = mTypeIndex.IndexOf(type_n);
            Assert.IsTrue(id < 0, string.Format("This type({0}) is not assigned serializer", type_n));
            var bytes = Serialize(type_n);
            bytes.Add( Serialize (obj));
            return bytes; 
        }

        private string GetTypeName ( Type type ) {
            return type.Name;
        }

        private int GetTypeId ( string type ) {
            var id = mTypeIndex.IndexOf(type);
            return id < 0 ? mTypeIndex.Count : id;
        }
    }

    /// <summary>
    /// 直列化からの復帰に失敗した場合のエラーです
    /// </summary>
    public class UnDeserializableException: Exception { }

    public static class StandardSerialization {
        public const int INT_SIZE = sizeof(Int32);
        public static class Int {
            public static ByteList Serial ( object obj ) {
                return ByteList.Gen().Add(BitConverter.GetBytes((Int32) obj));
            }
            public static object Deserial ( ByteList bytes ) {
                return bytes.DropInt32();
            }
        }
        public static class Float {
            public static ByteList Serial ( object obj ) {
                var data = BitConverter.GetBytes((double) obj);
                return ByteList.Gen().Add(data.Length).Add(obj);
            }
            public static object Deserial ( ByteList bytes ) {
                var size = bytes.DropInt32();
                var data = BitConverter.ToDouble(bytes.DropRange(0, size),0);
                return data ;
            }
        }
        public static class String {
            public static ByteList Serial ( object obj ) {
                // 文字列の長さではなく、文字列を持つByte配列の長さであることに注意
                var str = System.Text.Encoding.UTF8.GetBytes((string) obj);
                var bytes =  Int.Serial(str.Length).Add(str);
                return bytes;
            }
            public static object Deserial ( ByteList bytes ) {
                var size = bytes.DropInt32();
                var data = System.Text.Encoding.UTF8.GetString(bytes.DropRange(0, size));
                return data;
            }
        }
        public static class ArrayData {
            public static ByteList Serial ( object obj ) {
                var bytes = ByteList.Gen();
                var arr = (Array) obj;
                bytes.Add(arr.Length);
                foreach (var e in arr) {
                    var d = Serializer.Serialize(e);
                }
                return bytes;
            }
            public static object Deserial ( ByteList bytes ) {
                var size = bytes.DropInt32();
                object[] arr = new object[size];
                for(int i = 0; i < size; i++) {
                    arr [i] = Serializer.Deserialize(bytes);
                }
                return arr;
            }
        }


    }
}
