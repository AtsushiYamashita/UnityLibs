namespace BasicExtends {

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Mcrophone: MonoBehaviour {

        [SerializeField]
        private int mMicIndex = 0;
        private string mMicName = null;

        [SerializeField]
        private int mRecLength = 1;

        [SerializeField]
        private int mFrequency = 0;
        private int mPrevSamplePos = 0;

        private int mChannels = 2;

        private void SetMicTarget(int index ) {
            mMicIndex = index;
            mMicName = Microphone.devices [mMicIndex];
            int min, max;
            Microphone.GetDeviceCaps(mMicName, out min, out max);
            mFrequency = min;
        }

        void Reset () {
            SetMicTarget(mMicIndex);
        }

        void Start () {
            int min, max;
            var names = Microphone.devices;
            foreach (var name in names) {
                Microphone.GetDeviceCaps(name, out min, out max);
                Debug.Log(name + " " + min + "," + max);
            }
            Microphone.Start(mMicName, true, mRecLength, mFrequency);
        }

        byte[] ToBit ( AudioClip clip, int start, int end, Msg msg ) {
            mChannels = clip.channels;
            float [] samples = new float [clip.samples * clip.channels];
            clip.GetData(samples, 0);
            byte [] data = new byte [end - start];
            for(int i = start; i < end; i++) {
                data [i - start] = (byte)((samples [i - start] + 1) * 0.5f * 255); 
            }
            return data;
        }

        private void FixedUpdate () {
            var now = Microphone.GetPosition(mMicName);
            var nowPosNormal = mPrevSamplePos / mFrequency;
            var prevPosNormal = mPrevSamplePos / mFrequency;
            var delta = nowPosNormal - prevPosNormal;
            if (delta == 0) { return; }

            Msg msg = new Msg().Set("samples", mFrequency)
                .Set("channels", mChannels);
            if (delta < 0) {

            }

        }
    }

}