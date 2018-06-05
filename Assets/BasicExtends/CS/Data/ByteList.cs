namespace BasicExtends {
    using UnityEngine;
    using UnityEngine.Assertions;
    using System;
    using System.Text;
    using System.Collections.Generic;

    public class ByteList: List<byte> {
        private static readonly Func<byte, byte> NULL_PIPE = e => e;
        public static ByteList Zero { get { return new ByteList(); } }


        public static ByteList Gen () {
            return new ByteList();
        }

        public new ByteList Add ( byte obj ) {
            return Add(obj);
        }

        public ByteList Add ( int obj ) {
            Add(BitConverter.GetBytes(obj));
            return this;
        }
        public ByteList Add ( float obj ) {
            Add(BitConverter.GetBytes(obj));
            return this;
        }
        public ByteList Add ( ByteList obj ) {
            AddRange(obj);
            return this;
        }

        public ByteList Add ( string obj ) {
            AddRange(Encoding.UTF8.GetBytes(obj));
            return this;
        }

        public ByteList Add ( byte [] obj ) {
            AddRange(obj);
            return this;
        }

        public ByteList WriteCountToHead (  ) {
            var size = Count ;
            AddHead(size);
            DropInt32();
            return this;
        }

        public ByteList Insert ( int index, byte [] obj ) {
            InsertRange(index, obj);
            return this;
        }
        public ByteList Insert<T> ( int index, T obj ) {
            InsertRange(index, (IEnumerable<byte>)Serializer.Serialize(obj));
            return this;
        }
        public ByteList AddHead<T> ( T obj ) {
            InsertRange(0, (IEnumerable<byte>) Serializer.Serialize(obj));
            return this;
        }
        public ByteList ConvertFromInt32 ( int num ) {
            AddRange(BitConverter.GetBytes(num));
            return this;
        }

        /// <summary>
        /// 情報を取得したらそれに合わせて値に該当するByte部分を削除する。
        /// </summary>
        public int DropInt32 ( int start = 0 , Func<byte,byte> pipe = null) {
            return BitConverter.ToInt32(DropRange(start, sizeof(Int32)), 0);
        }
        /// <summary>
        /// 情報を取得したらそれに合わせて値に該当するByte部分を削除する。
        /// </summary>
        public float DropFloat ( int start = 0, Func<byte, byte> pipe = null ) {
            return BitConverter.ToSingle(DropRange(0, sizeof(Single)), 0);
        }

        public byte [] DropRange(int start, int end, Func<byte,byte> pipe = null ) {
            pipe = pipe ?? NULL_PIPE;
            end = end < Count ? end : Count;
            byte [] buf = new byte [end - start];
            for (int i = start; i < end; i++) {
                buf [i] = this [start];
                RemoveAt(start);
            }
            return buf;
        }

        /// <summary>
        /// 情報を取得したらそれに合わせて値に該当するByte部分を削除する。
        /// </summary>
        public string DropString ( int start, int length ) {
            return BitConverter.ToString(DropRange(0,length), 0, length);
        }

        public ByteList ConvertString ( string str ) {
            var b = Encoding.UTF8.GetBytes(str);
            ConvertFromInt32(b.Length);
            AddRange(b);
            return this;
        }

        public new ByteList RemoveRange ( int s, int e ) {
            Assert.IsTrue(s < e);
            for (int i = s; i < e; i++) {
                RemoveAt(i);
            }
            return this;
        }
        public ByteList RemoveRangeBase ( int s, int c ) {
            base.RemoveRange(s, c);
            return this;
        }
    }
}