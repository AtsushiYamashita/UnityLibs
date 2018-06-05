namespace BasicExtends.SerializeImp {
    using System;
    using System.Collections.Generic;
    using UnityEngine.Assertions;
    using UnityEngine;
    using BasicExtends;

    public class Binary: ISerializer {

        Dictionary<int, Func<ByteList, CheckedRet<object>>> mParser;
        Dictionary<int, Func<object, ByteList>> mSerializer;

        public Binary () {
            mParser = new Dictionary<int, Func<ByteList, CheckedRet<object>>>();
            mSerializer = new Dictionary<int, Func<object, ByteList>>();
            SetSerializer("Int32", Standards.Int.Serial);
            SetDeserializer("Int32", Standards.Int.Deserial);
            SetSerializer("Float", Standards.Float.Serial);
            SetDeserializer("Float", Standards.Float.Deserial);
            SetSerializer("Single", Standards.Float.Serial);
            SetDeserializer("Single", Standards.Float.Deserial);
            SetSerializer("String", Standards.String.Serial);
            SetDeserializer("String", Standards.String.Deserial);
            SetSerializer("ArrayList", Standards.ArrayList.Serial);
            SetDeserializer("ArrayList", Standards.ArrayList.Deserial);
            SetSerializer("Dictionary", Standards.Dictionary.Serial);
            SetDeserializer("Dictionary", Standards.Dictionary.Deserial);
        }

        public ByteList GetTypeList ( List<string> list ) {
            var count = list.Count;
            var bytes = ByteList.Gen().Add(count);
            foreach (var t in list) {
                bytes.Add(ToSerial(t));
            }
            return bytes;
        }

        public void SetSerializer ( string type, Func<object, ByteList> func ) {
            var id = Serializer.GetTypeId(type);
            mSerializer.TrySet(id.Value, func);
        }

        public void SetDeserializer ( string type, Func<ByteList, CheckedRet<object>> func ) {
            var id = Serializer.GetTypeId(type);
            mParser.TrySet(id.Value, func);
        }

        /// <summary>
        /// 基本的に [型番号] [[情報長]情報]です。
        /// </summary>
        public CheckedRet<object> Deserial ( ByteList bytes ) {
            int id = bytes.DropInt32();
            var func = mParser.TryGet(id);
            if (func.Key == false) {
                throw new Exception("id(" + id + ")is cannot find(byte.c is "
                    + bytes.Count + "& dic.c" + mParser.Count + ")");
            }
            //            DebugLog.Log.Print("Deserial id({0}) Count({1})", id, bytes.Count);
            return func.Value(bytes);
        }

        /// <summary>
        /// 登録された手続を使ってオブジェクトを直列化する
        /// 基本的に [型番号] [[情報長]情報]です。
        /// </summary>
        public ByteList ToSerial ( object obj ) {
            if (obj.IsNull()) { return ByteList.Zero; }
            var type_n = obj.GetType().Name;
            var id = Serializer.GetTypeId(type_n);
            Assert.IsTrue(id.Key, string.Format("This type({0}) is not assigned serializer", type_n));
            if (id.Key == false) { return null; }
            var bytes = ByteList.Zero.Add(id.Value);
            bytes.Add(mSerializer [id.Value](obj));
            return bytes;
        }

        public static class Standards {
            public static class Int {
                public static ByteList Serial ( object obj ) {
                    return ByteList.Gen().Add(BitConverter.GetBytes((Int32) obj));
                }
                public static CheckedRet<object> Deserial ( ByteList bytes ) {
                    try {
                        return CheckedRet<object>.Gen(true, bytes.DropInt32());
                    } catch (Exception e) {
                        Debug.Log("Int parse error" + e);
                        return CheckedRet<object>.Fail();
                    }
                }
            }

            public static class Float {
                public static ByteList Serial ( object obj ) {
                    var data = BitConverter.GetBytes((Single) obj);
                    return ByteList.Gen().Add(data.Length).Add(data);
                }
                public static CheckedRet<object> Deserial ( ByteList bytes ) {
                    try {
                        var size = bytes.DropInt32();
                        var data = BitConverter.ToSingle(bytes.DropRange(0, size), 0);
                        return CheckedRet<object>.Gen(true, data);
                    } catch (Exception e) {
                        Debug.Log("parse error" + e);
                        return CheckedRet<object>.Fail();
                    }
                }
            }

            /// <summary>
            /// 基本的に [[情報長]情報]です。
            /// </summary>
            public static class String {
                public static ByteList Serial ( object obj ) {
                    // 文字列の長さではなく、文字列を持つByte配列の長さであることに注意
                    var str = System.Text.Encoding.UTF8.GetBytes((string) obj);
                    var bytes = Int.Serial(str.Length).Add(str);
                    return bytes;
                }
                public static CheckedRet<object> Deserial ( ByteList bytes ) {
                    try {
                        var size = bytes.DropInt32();
                        var data = System.Text.Encoding.UTF8.GetString(bytes.DropRange(0, size));
                        return CheckedRet<object>.Gen(true, data);
                    } catch (Exception e) {
                        Debug.Log("parse error" + e);
                        return CheckedRet<object>.Fail();
                    }
                }
            }

            /// <summary>
            /// 基本的に [[情報長]情報]です。
            /// </summary>
            public static class ArrayList {
                public static ByteList Serial ( object obj ) {
                    var bytes = ByteList.Gen();
                    var arr = (Array) obj;
                    bytes.Add(arr.Length);
                    foreach (var e in arr) {
                        bytes.Add(Serializer.Serialize(e));
                    }
                    return bytes;
                }

                public static CheckedRet<object> Deserial ( ByteList bytes ) {
                    try {
                        var size = bytes.DropInt32();
                        object [] arr = new object [size];
                        for (int i = 0; i < size; i++) {
                            arr [i] = Serializer.Deserialize(bytes);
                        }
                        return CheckedRet<object>.Gen(true, arr);
                    } catch (Exception e) {
                        Debug.Log("parse error" + e);
                        return CheckedRet<object>.Fail();
                    }
                }
            }

            /// <summary>
            /// [len] [ [ k v ] [ ] ]
            /// </summary>
            public static class Dictionary {
                public static ByteList Serial ( object obj ) {
                    var bytes = ByteList.Gen();
                    var arr = (Dictionary<object, object>) obj;
                    bytes.Add(arr.Count);
                    foreach (var e in arr) {
                        bytes.Add(Serializer.Serialize(e.Key));
                        bytes.Add(Serializer.Serialize(e.Value));
                    }
                    return bytes;
                }
                /// <summary>
                /// Dictionary < object , object >として戻ることに注意
                /// </summary>
                public static CheckedRet<object> Deserial ( ByteList bytes ) {
                    try {
                        var size = bytes.DropInt32();
                        Dictionary<object, object> dic = new Dictionary<object, object>();
                        for (int i = 0; i < size; i++) {
                            var key = Serializer.Deserialize(bytes);
                            var val = Serializer.Deserialize(bytes);
                            dic.Add(key, val);
                        }
                        return CheckedRet<object>.Gen(true, dic);
                    } catch (Exception e) {
                        Debug.Log("parse error" + e);
                        return CheckedRet<object>.Fail();
                    }
                }
            }
        }
    }
}