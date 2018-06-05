using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BasicExtends;

public class SocketUDPTestScript : MonoBehaviour {

    public SocketUDPReceiver mRec;
    public SocketUDPSender mSend;

	public void Setup () {
        mRec.Setup();
        mSend.Setup();
    }

    public void SendTest1 () {
        mSend.SendStack(Msg.Gen()
            .Set(Msg.TO, "Debug").Set(Msg.ACT, "log")
            .Set(Msg.MSG, "=================="));
    }

}
