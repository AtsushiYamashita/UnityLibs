namespace BasicExtends {

    using System;
    using System.Collections.Generic;
    using UnityEngine.Assertions;

    public class BinarySerializeImplement: ISerializer {
        Dictionary<int, Func<ByteList, object>> mParser = new Dictionary<int, Func<ByteList, object>>();
        Dictionary<int, Func<object, ByteList>> mSerializer = new Dictionary<int, Func<object, ByteList>>();

        public BinarySerializeImplement () {
            Serializer.AssignSerializer("int", Standards.Int.Serial);
            Serializer.AssignDeserializer("int", Standards.Int.Deserial);
            Serializer.AssignSerializer("float", Standards.Float.Serial);
            Serializer.AssignDeserializer("float", Standards.Float.Deserial);
            Serializer.AssignSerializer("string", Standards.String.Serial);
            Serializer.AssignDeserializer("string", Standards.String.Deserial);
            Serializer.AssignSerializer("Array", Standards.String.Serial);
            Serializer.AssignDeserializer("Array", Standards.String.Deserial);
        }

        public ByteList GetTypeList ( List<string> list ) {
            var count = list.Count;
            var bytes = ByteList.Gen().Add(count);
            foreach (var t in list) {
                bytes.Add(ToSerial(t));
            }
            return bytes;
        }

        public Serializer SetSerializer ( string type, Func<object, ByteList> func ) {
            var id = Serializer.GetTypeId(type);
            mSerializer.Add(id, func);
            return Serializer.Instance;
        }

        public Serializer SetDeserializer ( string type, Func<ByteList, object> func ) {
            var id = Serializer.GetTypeId(type);
            mParser.Add(id, func);
            return Serializer.Instance;
        }

        public CheckedRet<object> Deserial (int id, ByteList bytes ) {
            var instance = mParser [id](bytes);
            return new CheckedRet<object>().Set(true, instance);
        }

        /// <summary>
        /// 登録された手続を使ってオブジェクトを直列化する
        /// </summary>
        public ByteList ToSerial ( object obj ) {
            var type_n = obj.GetType().Name;
            int id = Serializer.GetTypeId(type_n);
            Assert.IsTrue(id < 0, string.Format("This type({0}) is not assigned serializer", type_n));
            var bytes = ToSerial(type_n);
            bytes.Add(ToSerial(obj));
            return bytes;
        }

        public static class Standards {

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
                    var data = BitConverter.ToDouble(bytes.DropRange(0, size), 0);
                    return data;
                }
            }

            public static class String {
                public static ByteList Serial ( object obj ) {
                    // 文字列の長さではなく、文字列を持つByte配列の長さであることに注意
                    var str = System.Text.Encoding.UTF8.GetBytes((string) obj);
                    var bytes = Int.Serial(str.Length).Add(str);
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
                        bytes.Add(Serializer.Serialize(e));
                    }
                    return bytes;
                }

                public static object Deserial ( ByteList bytes ) {
                    var size = bytes.DropInt32();
                    object [] arr = new object [size];
                    for (int i = 0; i < size; i++) {
                        arr [i] = Serializer.Deserialize(bytes);
                    }
                    return arr;
                }
            }
        }
    }
}