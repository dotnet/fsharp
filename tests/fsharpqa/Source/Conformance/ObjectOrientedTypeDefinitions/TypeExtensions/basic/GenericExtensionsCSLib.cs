using System;
using System.Linq;
using Microsoft.FSharp.Core;
using Microsoft.FSharp.Collections;
using System.Collections.Generic;

namespace CSExtensionMethods
{
    static public class C
    {
        static public T ExtendGenericUnconstrainedNoArg<T>(this T s1) { return s1; }
        static public T ExtendGenericUnconstrainedOneArg<T>(this T s1, T s2) { return s1; }
        static public T ExtendGenericConstrainedNoArg<T>(this T s1) where T : System.IComparable { return s1; }
        static public T ExtendGenericConstrainedOneArg<T>(this T s1, T s2) where T : System.IComparable { return s1; }
        static public T ExtendGenericConstrainedTightNoArg<T>(this T s1) where T : System.IComparable<T> { return s1; }
        static public T ExtendGenericConstrainedTightOneArg<T>(this T s1, T s2) where T : System.IComparable<T> { return s1; }
        static public FSharpList<T> ExtendFSharpListNoArg<T>(this FSharpList<T> s1) { return s1; }
        static public FSharpList<T> ExtendFSharpListOneArg<T>(this FSharpList<T> s1, FSharpList<T> s2) { return s1; }
        static public FSharpList<int> ExtendFSharpIntListNoArg(this FSharpList<int> s1) { return s1; }
        static public FSharpList<int> ExtendFSharpIntListOneArg(this FSharpList<int> s1, FSharpList<int> s2) { return s1; }

        static public T[] ExtendArrayNoArg<T>(this T[] s1) { return s1; }
        static public T[] ExtendArrayOneArg<T>(this T[] s1, FSharpList<T> s2) { return s1; }
        static public int[] ExtendIntArrayNoArg(this int[] s1) { return s1; }
        static public int[] ExtendIntArrayOneArg(this int[] s1, int[] s2) { return s1; }

        static public T[,] ExtendArray2DNoArg<T>(this T[,] s1) { return s1; }
        static public T[,] ExtendArray2DOneArg<T>(this T[,] s1, FSharpList<T> s2) { return s1; }
        static public int[,] ExtendIntArray2DNoArg(this int[,] s1) { return s1; }
        static public int[,] ExtendIntArray2DOneArg(this int[,] s1, int[,] s2) { return s1; }

        static public int ExtendFSharpFuncIntIntNoArg(this FSharpFunc<int, int> s1) { return s1.Invoke(3); }
        static public FSharpFunc<T, U> ExtendFSharpFuncGenericNoArg<T, U>(this FSharpFunc<T, U> s1) { return s1; }
        static public U ExtendFSharpFuncGenericOneArg<T, U>(this FSharpFunc<T, U> s1, T arg) { return s1.Invoke(arg); }

        static public T1 ExtendTupleItem1<T1, T2>(this Tuple<T1, T2> s1) { return s1.Item1; }
        static public T2 ExtendTupleItem2<T1, T2>(this Tuple<T1, T2> s1) { return s1.Item2; }

        static public T1 GItem1<T1, T2>(this Tuple<T1, T2> s1) { return s1.Item1; }
        static public T1 GItem1<T1, T2, T3>(this Tuple<T1, T2, T3> s1) { return s1.Item1; }
        static public T1 GItem1<T1, T2, T3, T4>(this Tuple<T1, T2, T3, T4> s1) { return s1.Item1; }
        static public T1 GItem1<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> s1) { return s1.Item1; }
        static public T1 GItem1<T1, T2, T3, T4, T5, T6>(this Tuple<T1, T2, T3, T4, T5, T6> s1) { return s1.Item1; }
        static public T1 GItem1<T1, T2, T3, T4, T5, T6, T7>(this Tuple<T1, T2, T3, T4, T5, T6, T7> s1) { return s1.Item1; }
        static public T1 GItem1<T1, T2, T3, T4, T5, T6, T7, T8>(this Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>> s1) { return s1.Item1; }
        static public T1 GItem1<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9>> s1) { return s1.Item1; }

        /* methods that add extra type parameters not captured by the 'this' argument */
        static public U ExtendGenericIgnore<T, U>(this T s1, U s2) { return s2; }
        static public U ExtendCollListIgnore<T, U>(this List<T> s1, U s2) { return s2; }
        static public int ExtendGenericIgnoreBoth<T, U>(this T s1, U s2) { return 5; }
    }
}