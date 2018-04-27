using System;
using BasicExtends;

public class NULL : Singleton<NULL> {
    public override string ToString () {
        return "NULL";
    }
    public static NULL Null
    {
        get { return Instance; }
    }
    public static object[] NullArr
    {
        get { return StArr.To().ToObjArr(); }
    }
}

public static class NullChecker {
    public static Type Type = typeof(NULL);

    public static bool IsNull ( this object n ) {
        if (n == null) { return true; }
        if (n.GetType() == Type) { return true; }
        return false;
    }

    public static bool IsNotNull ( this object n ) {
        return !n.IsNull();
    }

    public static void NullThrow (  params  object[] n ) {
        foreach(var e in n) {
            if (n.IsNotNull()) { continue; }
            throw new Exception("Error : null exception.");
        }
    }

    public static T NullThrow<T> ( this T n ) {
        if (n.IsNotNull()) { return n; }
        throw new Exception("Error : null exception.");
    }

    public static T Default<T> ( this T n,T def ) {
        return n.IsNull() ? def : n;
    }
}