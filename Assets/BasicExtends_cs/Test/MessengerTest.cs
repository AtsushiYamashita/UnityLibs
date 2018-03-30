using BasicExtends;

public class MessengerTest: TestComponent {

    public string SetTest () {
        var msg = new Msg()
            .Set("test", "a")
            .Set("To", "b");
        if (msg.TryGet("test") != "a") { return "正しくセットできていません"; }
        if (msg.TryGet("To") != "b") { return "正しくセットできていません"; }
        if (msg.TryGet("to") == "b") { return "正しくセットできていません"; }
        return Pass();
    }
    public string MatchTest () {
        var msg = new Msg()
             .Set("test", "a")
             .Set("To", "b");
        if (!msg.Match("test", "a")) { return "正しく判定できていません"; }
        if (msg.Match("to", "b")) { return "正しく判定できていません"; }
        if (msg.Match("To", "a")) { return "正しく判定できていません"; }
        return Pass();
    }
    public string AssignPushTest () {
        string n = string.Empty;
        Messenger.Assign(( Msg msg ) => {
            if (!msg.Match("to", "n")) { return; }
            n = msg.TryGet("msg");
        });

        if (n != string.Empty) { return "ゴミが入っています"; }
        //var m = Msg.Gen().To("n").Message("test").Push();
        if (n == string.Empty) { return "正しく送信できていません"; }
        if (n != "test") { return "正しく送信できていません"; }
        return Pass();
    }
    public string AssignPushTest2 () {
        string n = string.Empty;
        Messenger.Assign(( Msg msg ) => {
            if (!msg.Match("to", "n")) { return; }
            n = msg.TryGet("msg");
        });

        if (n != string.Empty) { return "ゴミが入っています"; }
        //var m = Msg.Gen().To("m").Message("test").Push();
        if (n != string.Empty) { return "ゴミが入っています"; }
        return Pass();
    }

    public string ToIsTest () {
        string n = string.Empty;
        int received = 0;
        Messenger.Assign(( Msg msg ) => {
            received++;
            if (!msg.ToIs("n")) { return; }
            n = msg.TryGet("msg");
        });

        if (n != string.Empty) { return "ゴミが入っています"; }
        var m = Msg.Gen().To("m").Message("test").Push();
        if (n != string.Empty) { return "ゴミが入っています"; }
        m.To("n").Push();
        if (received != 2) { return "通信回数に異常があります。"; }
        if (n != "test") { return "正しく送信できていません"; }
        return Pass();
    }
}
