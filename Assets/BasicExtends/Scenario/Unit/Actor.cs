namespace BasicExtends.Scenario {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public enum ActorType {
        Character, Prop, Effect, Environment, UI
    }
    public class Actor: MonoBehaviour {
        public ActorType Type { get; set; }
    }

}