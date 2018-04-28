namespace BasicExtends.SerializeImp
{
    using System;
    using System.Collections.Generic;
    using UnityEngine.Assertions;

    public class String : ISerializer
    {
        Dictionary<int, Func<ByteList, object>> mParser;
        Dictionary<int, Func<object, ByteList>> mSerializer;


        public String  () {
            mParser = new Dictionary<int, Func<ByteList, object>>();
            mSerializer =  new Dictionary<int, Func<object, ByteList>>();
            SetSerializer("Int32", Standards.Int.Serial);
            SetDeserializer("Int32", Standards.Int.Deserial);
            SetSerializer("Float", Standards.Float.Serial);
            SetDeserializer("Float", Standards.Float.Deserial);
            SetSerializer("String", Standards.String.Serial);
            SetDeserializer("String", Standards.String.Deserial);
            SetSerializer("ArrayList", Standards.ArrayList.Serial);
            SetDeserializer("ArrayList", Standards.ArrayList.Deserial);
            SetSerializer("Dictionary", Standards.Dictionary.Serial);
            SetDeserializer("Dictionary", Standards.Dictionary.Deserial);
        }


        public ByteList GetTypeList(List<string> list)
        {
            var count = list.Count;
            var bytes = ByteList.Gen().Add(count);
            foreach (var t in list)
            {
                bytes.Add(ToSerial(t));
            }
            return bytes;
        }

        public void SetSerializer(string type, Func<object, ByteList> func)
        {
            var id = Serializer.GetTypeId(type);
            mSerializer.Add(id.Value, func);
        }

        public void SetDeserializer(string type, Func<ByteList, object> func)
        {
            var id = Serializer.GetTypeId(type);
            mParser.Add(id.Value, func);
        }

        /// <summary>
        /// 基本的に [型番号] [[情報長]情報]です。
        /// </summary>
        public CheckedRet<object> Deserial(ByteList bytes)
        {
            int id = bytes.DropInt32();
            var instance = mParser[id](bytes);
            return new CheckedRet<object>().Set(true, instance);
        }

        /// <summary>
        /// 登録された手続を使ってオブジェクトを直列化する
        /// 基本的に [型番号] [[情報長]情報]です。
        /// </summary>
        public ByteList ToSerial(object obj)
        {
            var type_n = obj.GetType().Name;
            var id = Serializer.GetTypeId(type_n);
            Assert.IsTrue(id.Key, string.Format("This type({0}) is not assigned serializer", type_n));
            if (id.Key == false) { return null; }
            var bytes = ByteList.Zero.Add(id.Value);
            bytes.Add( mSerializer [id.Value](obj));
            return bytes;
        }

        public static class Standards
        {

            public const int INT_SIZE = sizeof(Int32);
            public const int DOUBLE_SIZE = sizeof(double);

            public static class Int
            {
                public static ByteList Serial(object obj)
                {
                    return ByteList.Gen().Add((int)obj);
                }
                public static object Deserial(ByteList bytes)
                {
                    return BitConverter.ToInt32(bytes.DropRange(0, INT_SIZE),0);
                }
            }

            /// <summary>
            /// 基本的に [情報]です。
            /// </summary>
            public static class Float
            {
                public static ByteList Serial(object obj)
                {
                    return ByteList.Gen().Add((float)obj);
                }
                public static object Deserial(ByteList bytes)
                {
                    var data = BitConverter.ToDouble(bytes.DropRange(0, DOUBLE_SIZE), 0);
                    return data;
                }
            }

            /// <summary>
            /// 基本的に [[情報長]情報]です。
            /// </summary>
            public static class String
            {
                public static ByteList Serial(object obj)
                {
                    // 文字列の長さではなく、文字列を持つByte配列の長さであることに注意
                    var str = System.Text.Encoding.UTF8.GetBytes((string)obj);
                    var bytes = Int.Serial(str.Length).Add(str);
                    return bytes;
                }
                public static object Deserial(ByteList bytes)
                {
                    var size = bytes.DropInt32();
                    var data = System.Text.Encoding.UTF8.GetString(bytes.DropRange(0, size));
                    return data;
                }
            }

            /// <summary>
            /// 基本的に [[情報長]情報]です。
            /// </summary>
            public static class ArrayList
            {
                public static ByteList Serial(object obj)
                {
                    var bytes = ByteList.Gen();
                    var arr = (Array)obj;
                    bytes.Add(arr.Length);
                    foreach (var e in arr)
                    {
                        bytes.Add(Serializer.Serialize(e));
                    }
                    return bytes;
                }

                public static object Deserial(ByteList bytes)
                {
                    var size = bytes.DropInt32();
                    object[] arr = new object[size];
                    for (int i = 0; i < size; i++)
                    {
                        arr[i] = Serializer.Deserialize(bytes);
                    }
                    return arr;
                }
            }

            /// <summary>
            /// [len] [ [ k v ] [ ] ]
            /// </summary>
            public static class Dictionary
            {
                public static ByteList Serial(object obj)
                {
                    var bytes = ByteList.Gen();
                    var arr = (Dictionary<object, object>)obj;
                    bytes.Add(arr.Count);
                    foreach (var e in arr)
                    {
                        bytes.Add(Serializer.Serialize(e.Key));
                        bytes.Add(Serializer.Serialize(e.Value));
                    }
                    return bytes;
                }
                /// <summary>
                /// Dictionary < object , object >として戻ることに注意
                /// </summary>
                public static object Deserial(ByteList bytes)
                {
                    var size = bytes.DropInt32();
                    Dictionary<object, object> dic = new Dictionary<object, object>();
                    for (int i = 0; i < size; i++)
                    {
                        var key = Serializer.Deserialize(bytes);
                        var val = Serializer.Deserialize(bytes);
                        dic.Add(key, val);
                    }
                    return dic;
                }
            }
        }
    }
}