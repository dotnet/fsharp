using Microsoft.FSharp;
using Microsoft.FSharp.Core;
using Microsoft.FSharp.Collections;

public class Lib2
{
    public static Lib.recd1 r1 = new Lib.recd1(3);
    public static Lib.recd2 r2 = new Lib.recd2(3, "a");
    public static Lib.rrecd2 rr2 = new Lib.rrecd2("a", 3);
    public static Lib.recd3<string> r3 = new Lib.recd3<string>(4, "c", null);
    
    public static Lib.discr1_0 d10a = Lib.discr1_0.Discr1_0_A;
    public static Lib.discr1_1 d11a = Lib.discr1_1.NewDiscr1_1_A(3);
    public static Lib.discr1_2 d12a = Lib.discr1_2.NewDiscr1_2_A(3, 4);
    
    public static Lib.discr2_0_0 d200a = Lib.discr2_0_0.Discr2_0_0_A;
    public static Lib.discr2_1_0 d210a = Lib.discr2_1_0.NewDiscr2_1_0_A(3);
    public static Lib.discr2_0_1 d201a = Lib.discr2_0_1.Discr2_0_1_A;
    public static Lib.discr2_1_1 d211a = Lib.discr2_1_1.NewDiscr2_1_1_A(3);
    
    public static Lib.discr2_0_0 d200b = Lib.discr2_0_0.Discr2_0_0_B;
    public static Lib.discr2_1_0 d210b = Lib.discr2_1_0.Discr2_1_0_B;
    public static Lib.discr2_0_1 d201b = Lib.discr2_0_1.NewDiscr2_0_1_B(3);
    public static Lib.discr2_1_1 d211b = Lib.discr2_1_1.NewDiscr2_1_1_B(4);

    public static FSharpList<int> li1 = FSharpList<int>.Cons(3,FSharpList<int>.Empty);
    public static FSharpList<Lib.recd1> lr1 = FSharpList<Lib.recd1>.Cons(r1,FSharpList<Lib.recd1>.Empty);

    public static FSharpOption<int> oi1 = FSharpOption<int>.Some(3);
    public static FSharpOption<Lib.recd1> or1 = FSharpOption<Lib.recd1>.Some(r1);

    public static FSharpRef<int> ri1 = new FSharpRef<int>(3);
    public static FSharpRef<Lib.recd1> rr1 = new FSharpRef<Lib.recd1>(r1);

    public static Lib.StructUnionsTests.U0 u0 = Lib.StructUnionsTests.U0.U0;
    public static Lib.StructUnionsTests.U1 u1 = Lib.StructUnionsTests.U1.NewU1(3);
    public static Lib.StructUnionsTests.U2 u2 = Lib.StructUnionsTests.U2.NewU2(3,4);

    static Lib2() {     r3.recd3field3 = r3; }

}
