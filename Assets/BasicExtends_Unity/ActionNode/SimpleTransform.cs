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
            Messenger.Assign(( Msg msg ) => {
                if (msg.Unmatch("to", gameObject.name)) { return; }
                if (msg.Unmatch("as", GetType().Name)) { return; }
                var m = msg.TryGet("msg");
            });
        }

        /// <summary>
        ///  use rad
        /// </summary>
        /// <param name="size"></param>
        public void LocalRotateXD ( float size ) { mTarget.Rotate(size * 180, 0, 0); }

        /// <summary>
        /// not rad
        /// </summary>
        /// <param name="size"></param>
        public void LocalRotateYR ( float size ) { mTarget.Rotate(0, size * 180, 0); }

        /// <summary>
        /// not rad
        /// </summary>
        /// <param name="size"></param>
        public void LocalRotateZL ( float size ) { mTarget.Rotate(0, 0, size * 180); }

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

        public void Set(Trfm trfm ) {
            
        }

        public void DebugPrintTransform (  ) {
            DebugLog.Log.Print("{0}{1} "
                , gameObject.name
                , Trfm.Convert(transform).ToJson());
        }
    }
}

