namespace BasicExtends.Scenario {

    /// <summary>
    /// このシナリオシステムを使う場合に、
    /// シーンとして扱うオブジェクトの管理を行う。
    /// </summary>
    public class MetaScene: ScenarioProcess {

        protected override void Init () {
            TypeName = GetType().Name;
            SetState("start");
        }
    }
}