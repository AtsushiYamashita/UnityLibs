namespace BasicExtends.CharacterAction {
    using BasicExtends;
    using BasicExtends.Blackbord;
    using UnityEngine;

    public class Move : CharaAction{

        [SerializeField]
        public string mTo;

        public override float GetCost () {
            var f = ValueStore.TryGet<string>(Target,"Pos.now");
            var v = ValueStore.TryGet<float>(Target, "Move.velocity");
            if (f.Key == false || v.Key == false) { throw new System.Exception(); }
            if (mTo.IsNull()) { throw new System.Exception(); }
            return GetCost(f.Value, mTo, v.Value);
        }

        /// <summary>
        /// インスタンスに依存しない部分を切り分けた
        /// </summary>
        private static float GetCost( string from, string to, float velocity ) {
            var f = ValueStore.TryGet<Vector3>(from);
            var t = ValueStore.TryGet<Vector3>(from);
            if (t.Key == false || t.Key == false) { throw new System.Exception(); }
            var v = t.Value - f.Value;
            return v.magnitude / velocity;
        }



    }
}