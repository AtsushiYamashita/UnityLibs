namespace BasicExtends.Scenario {
    using BasicExtends;

    public class Cut: ScenarioProcess {

        protected override void ProcessStart () {
            mFactory = new TokenbaseParser();
            mFactory.SetTarget(this);

            Messenger.Assign(msg => {
                if (msg.Unmatch("To", gameObject.name)) { return; }
                if (msg.Unmatch("as", GetType().Name)) { return; }
                if (msg.Match("act", "SetScript")) {
                    mFactory.SetScript(msg.TryGet("script"));
                    return;
                }
            });

            base.ProcessStart();
        }

        public void InstanceSet( Actor actor ) {
            actor.transform.parent = transform; 
        }
    }
}