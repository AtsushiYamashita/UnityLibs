namespace BasicExtends {

    using UnityEngine;

    [RequireComponent(typeof(DelayTransform))]
    public class SyncTransform: MonoBehaviour {

        [SerializeField]
        private string ip = "0";

        [SerializeField]
        private int port = 0;

        [SerializeField]
        private int mPase = 3;
        private int mCount = 0;

        [SerializeField]
        private string mSyncTo = "";

        void Reset () {
            mSyncTo = name;
        }

        void Start () {
            MultiTask.Push(( obj ) =>
            {
                if (mCount++ % mPase != 0) {
                    return MultiTask.End.FALSE;
                }
                Msg.Gen()
                    .To( mSyncTo )
                    .As(GetType().Name)
                    .Act("Sync")
                    .Netwrok(ip, port)
                    .SetObjectData(Trfm.Convert(transform)).Pool();
                return MultiTask.End.FALSE;
            });

            MessengerSetup();
        }

        private void MessengerSetup () {
            Messenger.Assign(( Msg msg ) =>
            {
                if (msg.Match("Network", "true")) { return; }
                if (msg.Unmatch("to", gameObject.name)) { return; }
                if (msg.Unmatch("as", GetType().Name)) { return; }
                if (msg.Match("act", "Sync")) {
                    var rec = msg.TryObjectGet<Trfm>();
                    Sync(rec);
                    return;
                }
            });
        }

        private void Sync ( Trfm rec ) {
            Msg.Gen().To(gameObject.name)
                .As("DelayTransform")
                .Act("DelayTrans")
                .Set("time",mPase)
                .SetObjectData(rec).Push();
        }
    }

}