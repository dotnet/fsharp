using System;
using Microsoft.FSharp;
using Microsoft.FSharp.Core;
using Microsoft.FSharp.Collections;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("a")]

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

    public static FSharpList<int> li1 = FSharpList<int>.Cons(3, FSharpList<int>.Empty);
    public static FSharpList<Lib.recd1> lr1 = FSharpList<Lib.recd1>.Cons(r1, FSharpList<Lib.recd1>.Empty);

    public static FSharpOption<int> oi1 = FSharpOption<int>.Some(3);
    public static FSharpOption<Lib.recd1> or1 = FSharpOption<Lib.recd1>.Some(r1);

    public static FSharpRef<int> ri1 = new FSharpRef<int>(3);
    public static FSharpRef<Lib.recd1> rr1 = new FSharpRef<Lib.recd1>(r1);

    public static Lib.StructUnionsTests.U0 u0 = Lib.StructUnionsTests.U0.U0;
    public static Lib.StructUnionsTests.U1 u1 = Lib.StructUnionsTests.U1.NewU1(3);
    public static Lib.StructUnionsTests.U2 u2 = Lib.StructUnionsTests.U2.NewU2(3, 4);

    static Lib2() { r3.recd3field3 = r3; }

}


namespace CustomExtensions
{
    public static class Extensions
    {

        /// Extend an F# type
        static public ref readonly DateTime ExtendCSharpTypeWithInRefReturnExtension(in this DateTime inp) { return ref inp; }
        static public ref DateTime ExtendCSharpTypeWithRefReturnExtension(ref this DateTime inp) { return ref inp; }
        static public void ExtendCSharpTypeWithOutRefExtension(ref this DateTime inp) { inp = inp.Date; }
        static public int ExtendCSharpTypeWithInRefExtension(ref this DateTime inp) { return inp.Year;  }
        static public int ExtendFSharpType(this Lib.recd1 recd) { return 5; }
        static public int ExtendCSharpType(this Lib2 recd) { return 4; }
    }
}

namespace Fields
{
    public class Fields
    {

        /// Extend an F# type
        static public int StaticIntField => 3;
        static public System.DateTime StaticDateTimeField => System.DateTime.Now.Date;
    }
}

namespace Newtonsoft.Json.Converters
{
    internal class SomeClass
    {
        public SomeClass() { }
        public static void SomeMethod() { }
    }
    public class ContainerClass
    {
        public ContainerClass() { }
        internal class SomeClass
        {
            public SomeClass() { }
            public static void SomeMethod() { }
        }

    }
}

namespace FSharpOptionalTests
{
    public class ApiWrapper 
    {
        public static Tuple<FSharpOption<T>, FSharpOption<int>> MethodWithOptionalParams<T>(T value1, int value2)
        {
            return Lib.OptionalParameterTests.MethodWithOptionalParams<T>(value1: value1, value2: value2);
        }

        public static FSharpOption<T> ConsumeOptionalParam<T>(FSharpOption<T> param)
        {
            if (param == null)
                return Lib.OptionalParameterTests.MethodWithOptionalParam<T>(value: null);
            else
                return Lib.OptionalParameterTests.MethodWithOptionalParam<T>(value: param.Value);
        }

        public static int MethodThatImplicitlyConvertsFSharpOption(int x)
        {
            FSharpOption<int> opt = x;
            return opt.Value;
        }
    }
}

namespace FSharpFuncTests
{
    public class ApiWrapper
    {
        public static Func<int, int> f1 = new Func<int, int>((int arg) => arg + 1);
        public static Func<int, string, int> f2 = new Func<int, string, int>((int arg1, string arg2) => arg1 + arg2.Length + 1);
        public static Func<int, string, byte, int> f3 = new Func<int, string, byte, int>((int arg1, string arg2, byte arg3) => arg1 + arg2.Length + 1 + arg3);
        public static Func<int, string, byte, sbyte, int> f4 = new Func<int, string, byte, sbyte, int>((int arg1, string arg2, byte arg3, sbyte arg4) => arg1 + arg2.Length + 1 + arg3 + arg4);
        public static Func<int, string, byte, sbyte, Int16, int> f5 = new Func<int, string, byte, sbyte, Int16, int>((int arg1, string arg2, byte arg3, sbyte arg4, Int16 arg5) => arg1 + arg2.Length + 1 + arg3 + arg4 + arg5);
    }
}

namespace StructTests
{
    public struct NonReadOnlyStruct
    {
        public int X { get; set; }

        public void M(int x)
        {
            X = x;
        }
    }
}