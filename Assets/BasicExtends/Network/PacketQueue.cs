namespace BasicExtends {
    using System.Collections.Generic;
    using Object = System.Object;
    using System.Text;
    using UnityEngine;

    public interface IPacketQueue {
        MsgToByte MsgToByte { get; }
    }

    public class MsgToByte {
        private System.Object mLock = new Object();
        private Queue<byte[]> mQueue = new Queue<byte []>();
        private static readonly byte [] mZero = new byte [0];

        public void Enqueue ( byte [] msg ) {
            lock (mLock) {
                mQueue.Enqueue(msg);
            }
        }


        public byte [] Dequeue () {
            byte[] msg = null;
            if (mQueue.Count < 1) { return mZero; }
            lock (mLock) {
                msg = mQueue.Dequeue(); 
            }
            if (msg == null) { return mZero; }
            // Debug.Log("Dequeue ==>" + msg.ToJson());
            return msg;
        }

        public void Clear () {
            mQueue = new Queue<byte[]>();
        }
    }

    //public class ByteToMsg {
    //    private System.Object mLock = new Object();
    //    private Queue<byte []> mQueue = new Queue<byte []>();

    //    public void Enqueue ( byte [] msg ) {
    //        lock (mLock) {
    //            mQueue.Enqueue(msg);
    //        }
    //    }

    //    public void Dequeue () {
    //        byte [] msg = null;
    //        if (mQueue.Count < 1) { return; }
    //        lock (mLock) {
    //            msg = mQueue.Dequeue();
    //        }
    //        if (msg == null) { return; }
    //        if (msg.Length < 1) { return; }
    //        var str = Encoding.UTF8.GetString(msg);
    //        JsonNode json = JsonNode.Parse(str);
    //        Msg.Gen().To(json ["To"].Get<string>())
    //            .As(json ["As"].Get<string>())
    //            .Act(json ["Act"].Get<string>())
    //            .Set("Id", "" + json ["Msg"].Get<string>())
    //            .Set("Msg", json ["Msg"].Get<string>())
    //            .Set("Data", json ["Data"].Get<string>()).Pool();
    //    }

    //    public void Clear () {
    //        mQueue = new Queue<byte []>();
    //    }
    //}

    public class PacketQueue: IPacketQueue {
        public MsgToByte MsgToByte { private set; get; }
        public PacketQueue () {
            MsgToByte = new MsgToByte();
        }
    }
}