namespace BasicExtends {

    using UnityEngine;

    public class MicrophoneRec: MonoBehaviour {

        /// <summary>
        /// 送信時に扱うデータ単位。重複ありのバッファ管理にも使う
        /// </summary>
        private class DataUnit {
            public const int BUFF_SIZE = 2500;
            public byte [] DataByte { set; get; }
            public int Start { set; get; }
            public int End { set; get; }
            public int Looped { set; get; }
            public int Index { set; get; }
            public Msg ToMsg () {
                return Msg.Gen()
                    .Set("Start", Start)
                    .Set("End", End)
                    .Set("Looped", Looped)
                    .Set("Index", Index)
                    .SetObjectData(DataByte);
            }
        }

        [SerializeField]
        private int mMicIndex = 0;
        private string mMicName = null;

        [SerializeField]
        private int mFrequency = 0;
        private int mPrevSamplePos = 0;

        [SerializeField]
        private string mTo = "";

        [SerializeField]
        private string mAs = "";

        private int mChannels = 2;
        private float mStartTime = 0;
        private const int REC_LENGTH = 1;
        private AudioClip mClip = null;
        private int mDataSize = 0;
        private const int BUF_SIZE = 3;
        private int mDataItr = 0;
        float [] mSamples;

        DataUnit [] mDatas = new DataUnit [BUF_SIZE];

        private void Reset () {
            SetMicTarget(mMicIndex);
        }

        private void Start () {
            int min, max;
            var names = Microphone.devices;
            foreach (var name in names) {
                Microphone.GetDeviceCaps(name, out min, out max);
                Debug.Log(name + " " + min + "," + max);
            }
            for (int i = 0; i < mDatas.Length; i++) {
                mDatas [i] = new DataUnit();
            }
        }

        /// <summary>
        /// 小さく精密に音声データを補足しておきたい。
        /// 
        /// </summary>
        private void FixedUpdate () {
            if (mClip == null) { return; }
            var now = Microphone.GetPosition(mMicName);
            if (now == mPrevSamplePos) { return; }
            var div = mPrevSamplePos > now
                ? mDataSize - 1 : (now - mPrevSamplePos) / 2 + mPrevSamplePos;
            AddDataSet(mClip, mPrevSamplePos, div);
            AddDataSet(mClip, mPrevSamplePos, now);

            SendData();
        }

        /// <summary>
        /// マイクターゲットの指定をインデックスから行う
        /// </summary>
        public void SetMicTarget ( int index ) {
            mMicIndex = index;
            mMicName = Microphone.devices [mMicIndex];
            int min, max;
            Microphone.GetDeviceCaps(mMicName, out min, out max);
            mFrequency = min;
        }

        /// <summary>
        /// 録音の開始
        /// </summary>
        public void RecStart () {
            mClip = Microphone.Start(mMicName, true, REC_LENGTH, mFrequency);
            mStartTime = Time.time;
            Msg.Gen()
                .Netwrok()
                .To(mTo).As(mAs)
                .Act("PlaySeup")
                .Set("samples", mFrequency)
                .Set("channels", mChannels).Push();
            mDataSize = mClip.samples * mClip.channels;
            mSamples = new float [mDataSize];
            for (int i = 0; i < mDatas.Length; i++) {
                mDatas [i].DataByte = new byte [DataUnit.BUFF_SIZE];
            }
        }

        private void AddDataSet ( AudioClip clip, int start, int now ) {
            ToBit(mClip, start, now, mDatas [mDataItr % BUF_SIZE]);
            mDatas [mDataItr % BUF_SIZE].Start = start;
            mDatas [mDataItr % BUF_SIZE].End = now;
            mDatas [mDataItr % BUF_SIZE].Index = mDataItr;
            mDatas [mDataItr % BUF_SIZE].Looped = (int) (Time.time - mStartTime);
            mDataItr++;

            mPrevSamplePos = now >= mDataSize - 1 ? 0 : now;
        }

        /// <summary>
        /// オーディオクリップの指定区間をByteに量子化する
        /// </summary>
        private void ToBit ( AudioClip clip, int start, int end, DataUnit unit ) {
            mChannels = clip.channels;
            clip.GetData(mSamples, 0);
            for (int i = start; i < end; i++) {
                unit.DataByte [i - start] = (byte) ((mSamples [i - start] + 1) * 0.5f * 255);
            }
        }

        private void SendData () {
            foreach (var d in mDatas) {
                d.ToMsg()
                    .Netwrok()
                    .To(mTo).As(mAs)
                    .Act("PlaySound").Push();
            }
        }
    }
}
