using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
namespace BasicExtends {

    public class SimpleTransform: MonoBehaviour {

        [SerializeField]
        Transform mTarget = null;

        private void Start () {
            Assert.IsNotNull(mTarget);
            MessengerSetup();
        }

        private void MessengerSetup () {
            //Msg.Gen().To(gameObject.name).As(GetType().Name).Push();
            Messenger.Assign(( Msg msg ) =>
            {
                if (msg.Unmatch("to", gameObject.name)) { return; }
                if (msg.Unmatch("as", GetType().Name)) { return; }
                
                if (msg.Match("act", "SetPos")) {
                    Rotate(msg.TryGet("vec"));
                    return;
                }
                if (msg.Match("act", "SetAngle")) {
                    Rotate(msg.TryGet("vec"));
                    return;
                }
                if (msg.Match("act", "Move")) {
                    Move(msg.TryGet("vec"));
                    return;
                }
                if (msg.Match("act", "MoveRUF")) {
                    Rotate(msg.TryGet("vec"));
                    return;
                }
                if (msg.Match("act", "Rotate")) {
                    Rotate(msg.TryGet("vec"));
                    return;
                }
            });
        }

        private void SetPos ( string json ) {
            JsonNode node = new JsonNode(json);
            SetPosX(node ["x"].Get<float>());
            SetPosY(node ["y"].Get<float>());
            SetPosZ(node ["z"].Get<float>());
        }

        private void SetAngle ( string json ) {
            JsonNode node = new JsonNode(json);
            SetRotX(node ["x"].Get<float>());
            SetRotY(node ["y"].Get<float>());
            SetRotZ(node ["z"].Get<float>());
        }

        private void Move ( string json ) {
            JsonNode node = new JsonNode(json);
            MoveX(node ["x"].Get<float>());
            MoveY(node ["y"].Get<float>());
            MoveZ(node ["z"].Get<float>());
        }

        private void MoveRUF ( string json ) {
            JsonNode node = new JsonNode(json);
            MoveRight(node ["x"].Get<float>());
            MoveUp(node ["y"].Get<float>());
            MoveForward(node ["z"].Get<float>());
        }

        private void Rotate ( string json ) {
            JsonNode node = new JsonNode(json);
            LocalRotateXD(node ["x"].Get<float>());
            LocalRotateYR(node ["y"].Get<float>());
            LocalRotateZL(node ["z"].Get<float>());
        }


        /// <summary>
        ///  use rad
        /// </summary>
        /// <param name="size"></param>
        public void LocalRotateXD ( float size ) {
            mTarget.Rotate(size * 180, 0, 0);
        }
        /// <summary>
        /// not rad
        /// </summary>
        /// <param name="size"></param>
        public void LocalRotateYR ( float size ) {
            mTarget.Rotate(0, size * 180, 0);
        }
        /// <summary>
        /// not rad
        /// </summary>
        /// <param name="size"></param>
        public void LocalRotateZL ( float size ) {
            mTarget.Rotate(0, 0, size * 180);
        }

        public void SetPosX ( float size ) {
            var p = mTarget.localPosition;
            p.x = size;
            mTarget.localPosition = p;
        }
        public void SetPosY ( float size ) {
            var p = mTarget.localPosition;
            p.y = size;
            mTarget.localPosition = p;
        }
        public void SetPosZ ( float size ) {
            var p = mTarget.localPosition;
            p.z = size;
            mTarget.localPosition = p;
        }

        public void SetRotX ( float size ) {
            var p = mTarget.localEulerAngles;
            p.x = size;
            mTarget.localEulerAngles = p;
        }
        public void SetRotY ( float size ) {
            var p = mTarget.localEulerAngles;
            p.y = size;
            mTarget.localEulerAngles = p;
        }
        public void SetRotZ ( float size ) {
            var p = mTarget.localEulerAngles;
            p.z = size;
            mTarget.localEulerAngles = p;
        }

        public void MoveX ( float size ) {
            var p = mTarget.position;
            p.x += size;
            mTarget.position = p;
        }
        public void MoveY ( float size ) {
            var p = mTarget.position;
            p.y += size;
            mTarget.position = p;
        }
        public void MoveZ ( float size ) {
            var p = mTarget.position;
            p.z += size;
            mTarget.position = p;
        }

        public void MoveForward ( float size ) {
            var p = mTarget.position;
            p += mTarget.forward * size;
            mTarget.position = p;
        }
        public void MoveUp ( float size ) {
            var p = mTarget.position;
            p += mTarget.up * size;
            mTarget.position = p;
        }
        public void MoveRight ( float size ) {
            var p = mTarget.position;
            p += mTarget.right * size;
            mTarget.position = p;
        }

        public void DebugPrintTransform () {
            DebugLog.Log.Print("{0}{1} "
                , gameObject.name
                , Trfm.Convert(transform).ToJson());
        }
    }
}

