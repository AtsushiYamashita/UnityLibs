namespace BasicExtends.Scenario {
    using BasicExtends;

    public class Cut: StateEvent, IFactoryReceiver,ICanSkip {
        protected IFactory mFactory = null;

        // インタフェース設計上の都合で作った関数。実際には呼ばれない
        void IFactoryReceiver.InstanceSet<T> ( T t ) {
            throw new System.Exception("this allow only type of scene");
        }

        protected override void StartProcess () {
            mFactory = new TokenbaseParser<Actor>();
            mFactory.SetTarget(this);

            Messenger.Assign(msg => {
                if (msg.Unmatch("To", gameObject.name)) { return; }
                if (msg.Unmatch("as", GetType().Name)) { return; }
                if (msg.Match("act", "SetScript")) {
                    mFactory.SetScript(msg.TryGet("script"));
                    return;
                }
            });
        }

        public void InstanceSet( Actor actor ) {
            actor.transform.parent = transform; 
        }

        public void Skip () {
            throw new System.NotImplementedException();
        }
    }
}