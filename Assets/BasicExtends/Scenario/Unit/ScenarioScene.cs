namespace BasicExtends.Scenario {

    public class ScenarioScene: SenarioIterator<StateEvent> {

        protected override void StartProcess () {
            TypeName = GetType().Name;
        }

    }
}