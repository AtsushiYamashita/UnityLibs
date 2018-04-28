namespace BasicExtends.SerializeImp {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using BasicExtends;

    public class Binary2: ISerializer {
        private static bool mSetuped = false;

        public Binary2 () {
            if (mSetuped) { return; }
            mSetuped = true;
        }

        public ByteList GetTypeList ( List<string> list ) {
            var count = list.Count;
            var bytes = ByteList.Gen().Add(count);
            foreach (var t in list) {
                bytes.Add(ToSerial(t));
            }
            return bytes;
        }

        public void SetSerializer ( string type, Func<object, ByteList> func ) { }

        public void SetDeserializer ( string type, Func<ByteList, object> func ) { }

        /// <summary>
        /// 基本的に [型番号] [[情報長]情報]です。
        /// </summary>
        public CheckedRet<object> Deserial ( ByteList bytes ) {

            object obj = null;
            var ms = new MemoryStream(bytes.ToArray());
            BinaryFormatter bf = new BinaryFormatter();
            obj = bf.Deserialize(ms);
            return new CheckedRet<object>().Set(true, obj);
        }

        /// <summary>
        /// 登録された手続を使ってオブジェクトを直列化する
        /// 基本的に [型番号] [[情報長]情報]です。
        /// </summary>
        public ByteList ToSerial ( object obj ) {
            var ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, obj);
            var arr = ByteList.Gen().Add(ms.ToArray());
            return arr;
        }
    }
}
