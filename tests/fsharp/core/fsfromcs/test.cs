

using System;
using System.Linq;
using Microsoft.FSharp;
using Microsoft.FSharp.Core;
using Microsoft.FSharp.Collections;

class Maine
{
   static int Main()
   {

      // Some typical code

        {
            Microsoft.FSharp.Core.FSharpOption<int> x = Microsoft.FSharp.Core.FSharpOption<int>.Some(3) ;
            System.Console.WriteLine("{0}",x.Value);
            Microsoft.FSharp.Collections.FSharpList<int> x2 = 
                Microsoft.FSharp.Collections.FSharpList<int>.Cons(3, Microsoft.FSharp.Collections.FSharpList<int>.Empty);
            System.Console.WriteLine("{0}",x2.Head);
        }
       {
           FSharpList<int> x = FSharpList<int>.Cons(3,(FSharpList<int>.Empty));

          Console.WriteLine("x - IsCons = {0}", x != null);
          Console.WriteLine("x - IsNil = {0}", x == null);
          Console.WriteLine("x.Head = {0}", x.Head);
          Console.WriteLine("x.Tail = {0}", x.Tail);
          Console.WriteLine("x.Tail - IsNil = {0}", x.Tail);
          switch (x.Tag) 
          {
              case FSharpList<int>.Tags.Cons: 
                  Console.WriteLine("Cons({0},{1})", x.Head, x.Tail);
                  break;
              case FSharpList<int>.Tags.Empty: 
                  Console.WriteLine("[]");
                  break;
          }
       }

       {
           FSharpList<int> x = FSharpList<int>.Cons(3, (FSharpList<int>.Empty));

          foreach (int i in x) {
              Console.WriteLine("i = {0}", i);
          }
       }


      {
           FSharpList<int> myList = ListModule.OfArray(new int[] { 4, 5, 6 });

           ListModule.Iterate
              (FuncConvert.ToFSharpFunc((Action<int>) delegate(int i) 
                                        { Console.WriteLine("i = {0}", i);}),
               myList);

          // tests op_Implicit
          ListModule.Iterate<int>
              ((Converter<int,Unit>) delegate(int i) { Console.WriteLine("i = {0} (2nd technique)", i); return null; },
               myList);

          // tests op_Implicit
          FSharpList<string> myList2 = 
            ListModule.Map
              (FuncConvert.ToFSharpFunc((Converter<int,string>) delegate(int i) 
                                        { return i.ToString() + i.ToString(); }),
               myList);

          // tests op_Implicit
          ListModule.Iterate
              (FuncConvert.ToFSharpFunc((Action<string>) delegate(string s) 
                                        { Console.WriteLine("i after duplication = {0}", s);}),
               myList2);

          // tests op_Implicit
          myList2 = 
            ListModule.Map<int,string>
              ((Converter<int,string>) delegate(int i) { return i.ToString() + i.ToString(); },
               myList);

          ListModule.Iterate<string>
              (FuncConvert.ToFSharpFunc((Action<string>) delegate(string s) 
                                        { Console.WriteLine("i after duplication (2nd technique) = {0}", s);}),
               myList2);

          myList2 = 
            ListModule.Map<int,string>
              (FuncConvert.ToFSharpFunc(delegate(int i) { return i.ToString() + i.ToString(); }),
               myList);

          myList2 = 
            ListModule.Map<int,string>
              (FuncConvert.FromFunc((Func<int,string>) delegate(int i) { return i.ToString() + i.ToString(); }),
               myList);

          ListModule.Iterate<string>(FuncConvert.ToFSharpFunc<string>(s => { Console.WriteLine("s = {0}", s);}),myList2);
          ListModule.Iterate<string>(FuncConvert.FromAction<string>(s => { Console.WriteLine("s = {0}", s);}),myList2);
          ListModule.Iterate<string>(FuncConvert.FromFunc<string, Microsoft.FSharp.Core.Unit>(s => null),myList2);

          myList2 = ListModule.Map<int,string>(FuncConvert.ToFSharpFunc<int,string>(i => i.ToString() + i.ToString()),myList);
          myList2 = ListModule.Map<int,string>(FuncConvert.FromFunc<int,string>(i => i.ToString() + i.ToString()),myList);
          myList2 = ListModule.MapIndexed<int,string>(FuncConvert.FromFunc<int,int,string>((i,j) => i.ToString() + j),myList);

          var trans3 = FuncConvert.FromFunc<int,int,int,int>((i,j,k) => i + j + k);
          var trans4 = FuncConvert.FromFunc<int,int,int,int,int>((i,j,k,l) => i + j + k + l);
          var trans5 = FuncConvert.FromFunc<int,int,int,int,int,int>((i,j,k,l,m) => i + j + k + l + m);

          var action3 = FuncConvert.FromAction<int,int,int>((i,j,k) => System.Console.WriteLine("action! {0}", i + j + k));
          var action4 = FuncConvert.FromAction<int,int,int,int>((i,j,k,l) => System.Console.WriteLine("action! {0}", i + j + k + l));
          var action5 = FuncConvert.FromAction<int,int,int,int,int>((i,j,k,l,m) => System.Console.WriteLine("action! {0}", i + j + k + l + m));
          ListModule.Iterate<string>(FuncConvert.ToFSharpFunc<string>(s => { Console.WriteLine("i after duplication (2nd technique) = {0}", s);}),myList2);
          ListModule.Iterate<string>(FuncConvert.FromAction<string>(s => { Console.WriteLine("i after duplication (2nd technique) = {0}", s);}),myList2);


      }

       // Construct a value of each type from the library

      Lib.Recd1 r1 = new Lib.Recd1(3);
      Lib.Recd2 r2 = new Lib.Recd2(3, "a");
      Lib.RevRecd2 rr2 = new Lib.RevRecd2("a", 3);
      Lib.Recd3<string> r3 = new Lib.Recd3<string>(4, "c", null);
      r3.recd3field3 = r3;

      Lib.One d10a = Lib.One.One;
      Lib.Int d11a = Lib.Int.NewInt(3);
      Lib.IntPair ip = Lib.IntPair.NewIntPair(3, 4);
      Console.WriteLine("{0}", ip.Item1);
      Console.WriteLine("{0}", ip.Item2);

      Lib.IntPear ip2 = Lib.IntPear.NewIntPear(3,4);
      Console.WriteLine("{0}", ip2.Fst);
      Console.WriteLine("{0}", ip2.Snd);

      Lib.Bool b = Lib.Bool.True;
      Console.WriteLine("{0}", Lib.Bool.True);
      Console.WriteLine("{0}", Lib.Bool.False);
      //Console.WriteLine("{0}", Lib.Bool.IsTrue(b));
      //Console.WriteLine("{0}", Lib.Bool.IsFalse(b));
      switch (b.Tag) 
      {
          case Lib.Bool.Tags.True: 
              Console.WriteLine("True");
              break;
          case Lib.Bool.Tags.False: 
              Console.WriteLine("False");
              break;
      }

      Lib.OptionalInt oint = Lib.OptionalInt.NewSOME(3);
      Console.WriteLine("oint - IsSOME = {0}", oint != null);
      Console.WriteLine("oint - IsNONE = {0}", oint == null);
      Console.WriteLine("{0}", (oint as Lib.OptionalInt.SOME).Item);
      switch (oint.Tag) 
      {
          case Lib.OptionalInt.Tags.SOME:
              var c = oint as Lib.OptionalInt.SOME;
              Console.WriteLine("SOME({0})", c.Item);
              break;
          case Lib.OptionalInt.Tags.NONE: 
              Console.WriteLine("NONE");
              break;
      }

      Lib.IntOption iopt = Lib.IntOption.Nothing;
      Console.WriteLine("iopt - IsSomething = {0}", iopt != null);
      Console.WriteLine("iopt - IsNothing = {0}", iopt == null);
      switch (iopt.Tag) 
      {
          case Lib.IntOption.Tags.Something: 
              Console.WriteLine("Something({0})", (iopt as Lib.IntOption.Something).Item);
              break;
          case Lib.IntOption.Tags.Nothing: 
              Console.WriteLine("Nothing");
              break;
      }

      Lib.GenericUnion<int, string> gu1 = Lib.GenericUnion<int, string>.Nothing;
      Lib.GenericUnion<int, string> gu2 = Lib.GenericUnion<int, string>.NewSomething(3, "4");
      Lib.GenericUnion<int, string> gu3 = Lib.GenericUnion<int, string>.NewSomethingElse(3);
      Lib.GenericUnion<int, string> gu4 = Lib.GenericUnion<int, string>.NewSomethingElseAgain(4);
      //Console.WriteLine("{0}", (gu1 as Lib.GenericUnion<int,string>.Cases.Nothing));
      Console.WriteLine("{0}", (gu2 as Lib.GenericUnion<int,string>.Something).Item1);
      Console.WriteLine("{0}", (gu3 as Lib.GenericUnion<int,string>.SomethingElse).Item);
      Console.WriteLine("{0}", (gu4 as Lib.GenericUnion<int,string>.SomethingElseAgain).Item);
      switch (gu1.Tag)
      {
          case Lib.GenericUnion<int, string>.Tags.Nothing:
              Console.WriteLine("OK");
              break;
          case Lib.GenericUnion<int, string>.Tags.Something:
          case Lib.GenericUnion<int, string>.Tags.SomethingElse:
          case Lib.GenericUnion<int, string>.Tags.SomethingElseAgain:
              Console.WriteLine("NOT OK");
              throw (new System.Exception("ERROR - INCORRECT CASE TAG"));
      }

      switch (gu2.Tag)
      {
          case Lib.GenericUnion<int, string>.Tags.Something:
              Console.WriteLine("OK");
              break;
          case Lib.GenericUnion<int, string>.Tags.Nothing:
          case Lib.GenericUnion<int, string>.Tags.SomethingElse:
          case Lib.GenericUnion<int, string>.Tags.SomethingElseAgain:
              Console.WriteLine("NOT OK");
              throw (new System.Exception("ERROR - INCORRECT CASE TAG"));
      }

      Lib.BigUnion bu1 = Lib.BigUnion.NewA1(3);
      Lib.BigUnion bu2 = Lib.BigUnion.NewA2(3);
      Lib.BigUnion bu3 = Lib.BigUnion.NewA3(3);
      Lib.BigUnion bu4 = Lib.BigUnion.NewA4(3);
      Lib.BigUnion bu5 = Lib.BigUnion.NewA5(3);
      Lib.BigUnion bu6 = Lib.BigUnion.NewA6(3);
      Lib.BigUnion bu7 = Lib.BigUnion.NewA7(3);
      Lib.BigUnion bu8 = Lib.BigUnion.NewA8(3);
      Lib.BigUnion bu9 = Lib.BigUnion.NewA9(3);
      switch (bu1.Tag)
      {
          case Lib.BigUnion.Tags.A1:
              Console.WriteLine("OK");
              break;
          case Lib.BigUnion.Tags.A2:
          case Lib.BigUnion.Tags.A3:
          case Lib.BigUnion.Tags.A4:
          case Lib.BigUnion.Tags.A5:
          case Lib.BigUnion.Tags.A6:
          case Lib.BigUnion.Tags.A7:
          case Lib.BigUnion.Tags.A8:
          case Lib.BigUnion.Tags.A9:
              Console.WriteLine("NOT OK");
              throw (new System.Exception("ERROR - INCORRECT CASE TAG"));
      }


      Lib.BigEnum be1 = Lib.BigEnum.E1;
      Lib.BigEnum be2 = Lib.BigEnum.E2;
      Lib.BigEnum be3 = Lib.BigEnum.E3;
      Lib.BigEnum be4 = Lib.BigEnum.E4;
      Lib.BigEnum be5 = Lib.BigEnum.E5;
      Lib.BigEnum be6 = Lib.BigEnum.E6;
      Lib.BigEnum be7 = Lib.BigEnum.E7;
      Lib.BigEnum be8 = Lib.BigEnum.E8;
      Lib.BigEnum be9 = Lib.BigEnum.E9;
      switch (be1.Tag)
      {
          case Lib.BigEnum.Tags.E1:
              Console.WriteLine("OK");
              break;
          case Lib.BigEnum.Tags.E2:
          case Lib.BigEnum.Tags.E3:
          case Lib.BigEnum.Tags.E4:
          case Lib.BigEnum.Tags.E5:
          case Lib.BigEnum.Tags.E6:
          case Lib.BigEnum.Tags.E7:
          case Lib.BigEnum.Tags.E8:
          case Lib.BigEnum.Tags.E9:
              Console.WriteLine("NOT OK");
              throw (new System.Exception("ERROR - INCORRECT CASE TAG"));
      }

      Lib.Index d211a = Lib.Index.NewIndex_A(3);

      Lib.Bool d200b = Lib.Bool.False;
      Lib.OptionalInt d210b = Lib.OptionalInt.NONE;
      Lib.IntOption d201b = Lib.IntOption.NewSomething(3);
      Lib.Index d211b = Lib.Index.NewIndex_B(4);

/*

type discr2_0_0 = True | False
type discr2_0_1 = Nothing | Something of int
type discr2_1_0 = SOME of int | NONE
type discr2_1_1 = Index_A of int | Index_B of int

type discr3_0_0_0 = Discr3_0_0_0_A | Discr3_0_0_0_B | Discr3_0_0_0_C
type discr3_0_1_0 = Discr3_0_1_0_A | Discr3_0_1_0_B of int | Discr3_0_0_0_C
type discr3_1_0_0 = Discr3_1_0_0_A of int | Discr3_1_0_0_B | Discr3_0_0_0_C
type discr3_1_1_0 = Discr3_1_1_0_A of int | Discr3_1_1_0_B of int | Discr3_0_0_0_C
type discr3_0_0_1 = Discr3_0_0_0_A | Discr3_0_0_0_B | Discr3_0_0_0_C of string
type discr3_0_1_1 = Discr3_0_1_0_A | Discr3_0_1_0_B of int | Discr3_0_0_0_C of string
type discr3_1_0_1 = Discr3_1_0_0_A of int | Discr3_1_0_0_B | Discr3_0_0_0_C of string
type discr3_1_1_1 = Discr3_1_1_0_A of int | Discr3_1_1_0_B of int | Discr3_0_0_0_C of string
*/

// Toplevel functions *
      int f_1  = Lib.f_1(1);
      int f_1_1 = Lib.f_1_1(1,2);
      int f_1_1_1 = Lib.f_1_1_1(1,2,3);
      int f_1_1_1_1 = Lib.f_1_1_1_1(1,2,3,4);
      int f_1_1_1_1_1 = Lib.f_1_1_1_1_1(1,2,3,4,5);
#if DELEGATES
      int f_1_effect_1 = Lib.f_1_effect_1(1)(2);
#else
      int f_1_effect_1 = Lib.f_1_effect_1(1).Invoke(2);
#endif

      //let f_2 x y = x+y
      //let f_3 x y z = x+y+z
      //let f_4 x1 x2 x3 x4 = x1+x2+x3+x4
      //let f_5 x1 x2 x3 x4 x5 = x1+x2+x3+x4+x5

      // Function returning a function 
      //let f_1_1 x = let x = ref 1 in fun y -> !x+y+1

      // Tuple value 
      //let tup2 = (2,3)
      //let tup3 = (2,3,4)
      //let tup4 = (2,3,4,5)

        System.Console.WriteLine("TEST PASSED OK");

      return 0;
   }





}
