
using ConsumeFromCS;
using System;

public class MainClass {

    public static void CheckMethod<T>(Func<T, T> f, T value, Func<T> defaultFun, T defaultValue) {
        if (!f(value).Equals(value)) { Environment.Exit(1); }
        if (!defaultFun().Equals(defaultValue)) { Environment.Exit(1); }
    }

    public static int Main(string[] args) {
        CheckMethod(v => Class.Method1(v),  (sbyte) 1,  () => Class.Method1(), (sbyte) 42);
        CheckMethod(v => Class.Method2(v),  (byte)1,    () => Class.Method2(), (byte)42);
        CheckMethod(v => Class.Method3(v),  (Int16)1,   () => Class.Method3(), (Int16)42);
        CheckMethod(v => Class.Method4(v),  (UInt16)1,  () => Class.Method4(), (UInt16)42);
        CheckMethod(v => Class.Method5(v),  1,          () => Class.Method5(), 42);
        CheckMethod(v => Class.Method6(v),  1U,         () => Class.Method6(), 42U);
        CheckMethod(v => Class.Method7(v),  1L,         () => Class.Method7(), 42L);
        CheckMethod(v => Class.Method8(v),  1UL,        () => Class.Method8(), 42UL);
        CheckMethod(v => Class.Method9(v),  1.1f,       () => Class.Method9(), 42.42f);
        CheckMethod(v => Class.Method10(v), 1.1d,       () => Class.Method10(), 42.42d);
        CheckMethod(v => Class.Method11(v), false,      () => Class.Method11(), true);
        CheckMethod(v => Class.Method12(v), 'c',        () => Class.Method12(), 'q');
        CheckMethod(v => Class.Method13(v), "noo",      () => Class.Method13(), "ono");
        //calling CheckMethod => NullRefExc :)
        if (Class.Method14() != null) return 1;
        CheckMethod(v => Class.Method15(v), new DateTime(12300L), () => Class.Method15(), new DateTime());
        
        if (Class.MethodNullable1() != null) return 1;
        if (Class.MethodNullable1(4) != 4) return 1;
        if (Class.MethodNullable2() != null) return 1;
        if (Class.MethodNullable2(true) != true) return 1;
        if (Class.MethodNullable3() != null) return 1;
        if (Class.MethodNullable3(new DateTime(1234L)) != new DateTime(1234L)) return 1;

        CheckMethod(v => Class.Mix1(1, "1", v), 100, () => Class.Mix1(2, "2"),-12);
        CheckMethod(v => Class.Mix2(1, "1", v, 101), 100, () => Class.Mix2(2, "2", d: 12), -12);
        
        if (!Class.Mix3(1, "1").Equals(Tuple.Create("1",-12,-123))) return 1;
        if (!Class.Mix3(1).Equals(Tuple.Create("str", -12, -123))) return 1;
        if (!Class.Mix3(1, c: 1, b: "123").Equals(Tuple.Create("123",1,-123))) return 1;

        //C# can omit the argument.
        if (Class.Optional1() != 0) return 1;
        if (Class.Optional2() != System.Reflection.Missing.Value) return 1;
        if (Class.Optional3() != new DateTime()) return 1;
        if (Class.Optional4() != null) return 1;
        if (Class.Optional5() != null) return 1;
        //C# can't omit the argument, but can call the method.
        if (Class.OnlyDefault(2) != 2) return 1;
        return 0;
    }
}