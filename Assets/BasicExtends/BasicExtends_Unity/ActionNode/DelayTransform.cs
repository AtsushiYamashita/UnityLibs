namespace BasicExtends {

    using UnityEngine;
    using UnityEngine.Assertions;

    public class DelayTransform: MonoBehaviour {

        [SerializeField]
        private Transform mTarget = null;

        [SerializeField]
        private int mDefaultTime = 0;

        private void Start () {
            Assert.IsNotNull(mTarget);
            MessengerSetup();
        }

        private void MessengerSetup () {
            Messenger.Assign(( Msg msg ) =>
            {
                if (msg.Unmatch("to", gameObject.name)) { return; }
                if (msg.Unmatch("as", GetType().Name)) { return; }

                var time = int.Parse(msg.TryGet("time"));
                if (msg.Match("act", "DelayMove")) {
                    var vec = new Vec3();
                    vec.FromJson(JsonNode.Parse(msg.TryGet("vec")));
                    DelayMove(vec, time);
                    return;
                }
                if (msg.Match("act", "DelayRot")) {
                    var vec = new Vec3();
                    vec.FromJson(JsonNode.Parse(msg.TryGet("vec")));
                    DelayRotate(vec, time);
                    return;
                }
                if (msg.Match("act", "DelayScale")) {
                    var vec = new Vec3();
                    vec.FromJson(JsonNode.Parse(msg.TryGet("vec")));
                    DelayScale(vec, time);
                    return;
                }

                var trfm = msg.TryObjectGet<Trfm>();
                if (msg.Match("act", "DelayTrans") && trfm != null) {
                    DelayMove(trfm.POS, time);
                    DelayRotate(trfm.ROT, time);
                    DelayScale(trfm.SCA, time);
                    return;
                }
                if (msg.Match("act", "DelayTrans")) {
                    var pos = new Vec3();
                    pos.FromJson(JsonNode.Parse(msg.TryGet("pos")));
                    var rot = new Vec3();
                    rot.FromJson(JsonNode.Parse(msg.TryGet("rot")));
                    var sca = new Vec3();
                    sca.FromJson(JsonNode.Parse(msg.TryGet("sca")));
                    DelayMove(pos, time);
                    DelayRotate(rot, time);
                    DelayScale(sca, time);
                    return;
                }
            });
        }

        private void DelayMove ( Vec3 vec, int time =  -1) {
            var t = time != -1 ? time : mDefaultTime;
            var v = new Vector3(vec.X / t, vec.Y / t, vec.Z / t);
            MultiTask.Push(( obj ) =>
            {
                t--;
                if (t == 0) { return MultiTask.End.TRUE; }
                var loc = transform.localPosition;
                transform.localPosition = loc + v;
                return MultiTask.End.FALSE;
            });
        }

        private void DelayRotate ( Vec3 vec, int time = 0 ) {
            var t = time != -1 ? time : mDefaultTime;
            var v = new Vector3(vec.X / t, vec.Y / t, vec.Z / t);
            MultiTask.Push(( obj ) =>
            {
                t--;
                if (t == 0) { return MultiTask.End.TRUE; }
                transform.Rotate(v);
                return MultiTask.End.FALSE;
            });
        }

        private void DelayScale ( Vec3 vec, int time  = 0) {
            var t = time != -1 ? time : mDefaultTime;
            var v = new Vector3(vec.X / t, vec.Y / t, vec.Z / t);
            MultiTask.Push(( obj ) =>
            {
                t--;
                if (t == 0) { return MultiTask.End.TRUE; }
                var loc = transform.localScale;
                transform.localPosition = loc + v;
                return MultiTask.End.FALSE;
            });
        }

    }

}