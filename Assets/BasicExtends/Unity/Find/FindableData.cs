namespace BasicExtends {
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// This data include for you can find this object.
    /// </summary>
    [System.Serializable]
    public class FindableData {
        private GameObject mObject = null;
        public GameObject Object
        {
            get { return mObject; }
            set { this.mObject = value; }
        }

        [SerializeField]
        private string mSarchKey = string.Empty;
        public string SarchKey
        {
            get { return mSarchKey; }
            set { this.mSarchKey = value; }
        }

        [SerializeField]
        private List<string> mTags = new List<string>();
        public void SetTags ( params string [] args ) {
            mTags = new List<string>();
            AddTags(args);
        }
        public void AddTags ( params string [] args ) {
            mTags.AddRange(args);
            FindStore.Instance.AddByTag(this);
        }

        public List<string> Tags
        {
            get { return mTags; }
        }

        [SerializeField]
        private int mIndex = -1;
        public int Index
        {
            get { return mIndex; }
            set { this.mIndex = value; }
        }

        public override string ToString () {
            string ret;
            ret = string.Format("GameObject:{0}\n", Object.GetObjectPath());
            for (int i = 0; i < mTags.Count; i++) {
                ret += string.Format("tag{0}:{1}\n", i, mTags [i]);
            }
            ret += string.Format("index:{0}\n", mIndex);
            return ret;
        }
    }

}