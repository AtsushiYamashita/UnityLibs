using System;
using System.Collections.Generic;
using UnityEngine;

namespace BasicExtends {
    public class MessageSender: MonoBehaviour {

        [SerializeField]
        private int mSended = 0;
        public string mTo = "";
        public string mAs = "";
        public string mAct = "";

        public List<string> mList = new List<string> (); 

        public void Send () {
            if (mTo == "") { return; }
            var msg = Msg.Gen().Set(Msg.TO, mTo).Set("as", mAs).Set("act", mAct);

            var k = "";
            foreach(var e in mList) {
                if (k == "") { k = e; continue; }
                msg.Set(k, e);
                k = "";
            }

            msg.Push();
            mSended++;
        }
    }
}