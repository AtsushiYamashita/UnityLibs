namespace BasicExtends {
    using UnityEngine.Assertions;
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// 実質的なJSにおけるオブジェクト。
    /// </summary>
    [Serializable]
    public class Msg: StringDict, IJsonable, IFromJsonNode, ISerializable {

        private object mObjectData = null;
        private const int HEAD_SIZE = 4;

        public const string TO = "to";
        public const string AS = "as";
        public const string ACT = "act";
        public const string MSG = "msg";

        public Msg () {
            Serializer.SetDatatype(Serializer.SerialType.Binary);
            Serializer.AssignSerializer(GetType().Name, Serial);
            Serializer.AssignDeserializer(GetType().Name, Deserial);
        }

        protected Msg ( SerializationInfo info, StreamingContext context ) {
            mObjectData = info.GetValue("mObjectData", typeof(object));
            var count = (int) info.GetValue("count", typeof(int));
            var s = typeof(string);
            for (int i = 0; i < count; i++) {
                var k = (string) info.GetValue("key" + i, s);
                var v = (string) info.GetValue("val" + i, s);
                Set(k, v);
            }
        }

        public override void GetObjectData ( SerializationInfo info, StreamingContext context ) {
            info.AddValue("mObjectData", mObjectData, typeof(object));
            info.AddValue("count", Count, typeof(int));
            int i = 0;
            foreach (var p in this) {
                info.AddValue("key" + i, p.Key, typeof(string));
                info.AddValue("val" + i, p.Value, typeof(string));
                i++;
            }
        }

        public ByteList Serial ( object obj ) {
            var self = obj as Msg;
            var bytes = ByteList.Zero;
            Assert.IsNotNull(self);

            bytes.Add(Count);
            foreach (var p in this) {
                bytes
                    .Add(Serializer.Serialize(p.Key).WriteCountToHead())
                    .Add(Serializer.Serialize(p.Value).WriteCountToHead());
            }

            if (mObjectData != null) {
                var data = Serializer.Serialize(mObjectData);
                bytes.Add(data.WriteCountToHead());
            } else {
                bytes.Add(0);
            }

            bytes.Add(GetHashCode())
                .WriteCountToHead();

            return bytes;
        }


        public static CheckedRet<object> Deserial ( ByteList bytes ) {
            var size = bytes.DropInt32();
            if (bytes.Count != size) {
                throw new Exception("1");
             //   return CheckedRet<object>.Fail();
            }
            var ret = new Msg();

            var dic_size = bytes.DropInt32();
            for (int i = 0; i < dic_size; i++) {
                var k_l = bytes.DropInt32();
                var k = Serializer.Deserialize<string>(bytes.DropRange(0, k_l));
                if (k.Key == false) { throw new Exception("5"); }
                var v_l = bytes.DropInt32();
                var v = Serializer.Deserialize<string>(bytes.DropRange(0, v_l));
                if (v.Key == false) { throw new Exception("6"); }
                ret.Set(k.Value, v.Value);
            }

            var data_size = bytes.DropInt32();
            var h_s = 0;
            if (data_size < 4) {
                h_s = bytes.DropInt32();
                if (ret.GetHashCode() != h_s) {
                    throw new Exception("2:" + ret.ToJson());
                }
                return (ret.GetHashCode() != h_s)
                    ? CheckedRet<object>.Fail()
                    : CheckedRet<object>.Gen(true, ret);
            }

            var data_bytes = bytes.DropRange(0, data_size);
            var obj = Serializer.Deserialize<object>( data_bytes);
            if(obj.Key == false) {
                throw new Exception("3");
                //return CheckedRet<object>.Fail();
            }
            ret.mObjectData = obj.Value;

            h_s = bytes.DropInt32();
            if (ret.GetHashCode() != h_s) {
                throw new Exception("4");
            }

            return (ret.GetHashCode() != h_s)
                ? CheckedRet<object>.Fail()
                : CheckedRet<object>.Gen(true, ret);
        }

        public Msg ( byte [] bytes ) : this() {
            int json_len = BitConverter.ToInt32(bytes, 0);
            byte [] dis = new byte [json_len];
            Array.Copy(bytes, HEAD_SIZE, dis, 0, json_len);
        }

        public static Msg Gen () { return new Msg(); }

        public Msg SetObjectData ( object obj ) {
            mObjectData = obj;
            return this;
        }

        /// <summary>
        /// 値の有無にかかわらずセットする。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Msg Set ( string key, string value ) {
            if (key.Length < 1) { throw new Exception("Error, illigal MSG key set"); }
            key = key.ToUpper();
            if (ContainsKey(key) == false) { Add(key, value); return this; }
            this [key] = value;
            return this;
        }

        public Msg Set ( string key, int value ) {
            return Set(key, "" + value);
        }

        public Msg Set ( string key, float value ) {
            return Set(key, "" + value);
        }

        public Msg Netwrok ( string ip = "", int port = 0 ) {
            return Set("Network", "True").Set("ToIp", ip).Set("port", port);
        }

        /// <summary>
        /// when key-value is matched, then this function return true.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Match ( string key, string value ) {
            if (key.Length < 1) { throw new Exception("Error, illigal MSG key set"); }
            key = key.ToUpper();
            if (ContainsKey(key) == false) { return false; }
            var insideValue = this [key];
            if (insideValue.ToUpper() != value.ToUpper()) { return false; }
            return true;
        }

        /// <summary>
        /// when key-value is unmatched, then this function return true.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Unmatch ( string key, string value ) {
            return !Match(key, value);
        }

        /// <summary>
        /// Toに単体で入れられている場合を想定。
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public bool ToIs ( string v ) {
            return Match("to", v);
        }

        /// <summary>
        /// Toに配列で入れられている場合を想定
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public bool ToContain ( string v ) {
            var s = TryGet("to");
            return s.Contains(v);
        }
        /// <summary>
        /// this function do not return null.
        /// If this cannnot the param,
        /// then return Empty string.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string TryGet ( string key ) {
            if (key.Length < 1) { throw new Exception("Error, illigal MSG key set"); }
            key = key.ToUpper();
            if (ContainsKey(key) == false) { return string.Empty; }
            var insideValue = this [key];
            if (insideValue == null) { return string.Empty; }
            return insideValue;
        }

        public T TryObjectGet<T> () {
            try { return (T) mObjectData; } catch { return default(T); }
        }

        /// <summary>
        /// thread un-safe but realtime
        /// </summary>
        /// <returns></returns>
        public Msg Push () {
            Messenger.Push(this);
            return this;
        }

        /// <summary>
        /// thread safe but include delay
        /// </summary>
        /// <returns></returns>
        public Msg Pool () {
            Messenger.Pool(this);
            return this;
        }

        public string ToJson () {
            // デフォルトでは付属オブジェクトを直列化するものとして
            // data2jsonにFalseを指定している場合だけ拒否する
            // 既存コードに変更を加えないで対応するため
            var data2json = Match("data2json", "False") == false;
            if (mObjectData != (object) NULL.Null && data2json) {
                Set("mObjectData", mObjectData.ToJson());
            }
            var str = JsonStringify.Stringify(this);
            return str;
        }

        public void FromJson ( JsonNode node ) {
            if (node.IsNull()) { throw new Exception(); }
            if (node.Count < 1) { throw new Exception(); }
            var ret = Gen();
            int i = 0;

            try {
                while (true) {
                    var key = node ["key" + i].Get<string>();
                    var value = node ["value" + i].Get<string>();
                    i++;
                    ret.TrySet(key, value);
                }
            } catch { }

            DebugLog.Log.Print(node.ToString());
            throw new Exception();
        }

        public override int GetHashCode () {
            var ret =  System.Int32.MaxValue ^ Count;
            foreach(var p in this) {
                ret += p.Key.GetHashCode();
                ret ^= p.Value.GetHashCode();
            }
            ret ^= mObjectData.IsNull() ? 0 : mObjectData.GetHashCode(); 
            return ret;
        }
    }
}
