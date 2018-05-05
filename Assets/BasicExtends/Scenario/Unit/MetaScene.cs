namespace BasicExtends.Scenario {

    /// <summary>
    /// このシナリオシステムを使う場合に、
    /// シーンとして扱うオブジェクトの管理を行う。
    /// </summary>
    public class MetaScene: SenarioIterator<ScenarioScene> {

        protected override void StartProcess () {
            TypeName = GetType().Name;
        }
    }
}