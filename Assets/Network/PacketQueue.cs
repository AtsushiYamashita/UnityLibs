namespace BasicExtends {
    using System.IO;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Object = System.Object;

    public interface IPacketQueue {
        int Enqueue ( byte [] data, int size );
        int Dequeue ( ref byte [] buffer, int size );
        void Clear ();
    }

    public class PacketQueue : IPacketQueue {
        struct PacketInfo {
            public int offset;
            public int size;
        }

        private MemoryStream mStreamBuffer;
        private List<PacketInfo> mOffsetList;
        private int mOffset = 0;
        private Object lockObj = new Object();

        public PacketQueue () {
            mStreamBuffer = new MemoryStream();
            mOffsetList = new List<PacketInfo>();
        }

        public int Enqueue ( byte [] data, int size ) {
            var info = new PacketInfo();
            info.offset = mOffset;
            info.size = size;
            lock (lockObj) {
                mOffsetList.Add(info);
                mStreamBuffer.Position = mOffset;
                mStreamBuffer.Write(data, 0, size);
                mStreamBuffer.Flush();
                mOffset += size;
            }
            return size;
        }
        public int Dequeue ( ref byte [] buffer, int size ) {
            if (mOffsetList.Count <= 0) { return -1; }
            int recvSize = 0;
            lock (lockObj) {
                PacketInfo info = mOffsetList [0];
                int dataSize = Mathf.Min(size, info.size);
                recvSize = mStreamBuffer.Read(buffer, 0, dataSize);
                if (recvSize > 0) { mOffsetList.RemoveAt(0); }
                if (mOffsetList.Count == 0) { Clear(); mOffset = 0; }
            }
            return recvSize;
        }
        public void Clear () {
            byte [] buffer = mStreamBuffer.GetBuffer();
            Array.Clear(buffer, 0, buffer.Length);
            mStreamBuffer.Position = 0;
            mStreamBuffer.SetLength(0);
        }
    }

}