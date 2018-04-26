﻿namespace BasicExtends {
    using UnityEngine;
    using UnityEngine.Assertions;
    using System;
    using System.Text;
    using System.Collections.Generic;

    public class ByteList: List<byte> {
        private static readonly Func<byte, byte> NULL_PIPE = e => e;
        public static ByteList Zero { get { return new ByteList(); } }

        public ByteList Add ( byte [] obj ) {
            AddRange(obj);
            return this;
        }

        public static ByteList Gen () {
            return new ByteList();
        }

        public ByteList Add<T> ( T obj ) {
            return Add(Serializer.Serialize(obj));
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
            return BitConverter.ToInt32(DropRange(0, sizeof(UInt32)), 0);
        }

        public byte[] DropRange(int start, int end, Func<byte,byte> pipe = null ) {
            pipe = pipe ?? NULL_PIPE;
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
    }
}