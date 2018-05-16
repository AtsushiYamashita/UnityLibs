namespace BasicExtends {
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class ConditionCheck: MonoBehaviour {

        //[SerializeField]
        //private bool mResult = false;
        [SerializeField]
        private Component mTarget = null;

        public Component Target { get { return mTarget; } }

        public void Check () {
            throw new System.NotImplementedException();

        }

        public void BlackbordUpdate (Component com, string key,string value) {
            Blackbord.ValueStore.Add(Pair<string,string>.Gen(key, value));
        }
        public void BlackbordUpdate ( Component com, string key, float value ) {
            Blackbord.ValueStore.Add(Pair<string, float>.Gen(key, value));
        }
    }
}
