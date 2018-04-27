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

        private float mCutoff = 0.05f;
        private float mMoveOnFrame = 0.8f;

        Vector3 mPos = Vector3.zero;
        Vector3 mRot = Vector3.zero;
        Vector3 mSca = Vector3.zero;


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
                    .To(mSyncTo)
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

        private Vector3 VecFill ( Vector3 to, Vector3 local ) {
            if (to == Vector3.zero) { return local; }
            if (transform.localPosition == to) { return local; }
            var dif = to - local;
            var move = dif * mMoveOnFrame;
            return move.magnitude > mCutoff ? move + local : to;
        }

        private void Sync ( Trfm rec ) {
            mPos = rec.POS.Convert();
            mRot = rec.ROT.Convert();
            mSca = rec.SCA.Convert();

        }

        private void Update () {
            transform.localPosition = VecFill(mPos, transform.localPosition);
            transform.localEulerAngles = VecFill(mRot, transform.localEulerAngles);
            transform.localScale = VecFill(mSca, transform.localScale);
        }
    }

}