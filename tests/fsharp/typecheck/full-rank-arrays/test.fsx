// #TypeInference
(* Avoid ;; since it allows fsi to recover from type errors *)

#r "HighRankArrayTests.dll"
open HighRankArrayTests

(* In the F# 3.1.2 release and earlier versions, the compiler's overload resolution
   breaks on the following class definition. The referenced assembly defines
   several classes with various overloaded methods and constructors; some of these
   overloads include parameters whose type is a "high-rank" array -- that is,
   an array of rank 5-32. Arrays with these ranks are not currently supported by
   F#, but it should still be possible to call one of the overloads which does
   not include such a parameter. *)
type Class1() = 
    member this.X1 =
        GenericStaticClassWithMethod<int>.Example [| 2;3;4;5 |]

    member this.X2 =
        StaticClassWithGenericMethod.Example [| 1;2;3;4 |]

    member this.X3 =
        let foo = ClassWithArrayCtor ([| 1;3;5;7 |])
        ()
        
    member this.X4 =
        let y1 = MethodsReturningHighDArrays.Example1<int>()
        let y2 = MethodsReturningHighDArrays.Example1<int>(2)

        StaticClassWithGenericMethod.Example(y1)
        StaticClassWithGenericMethod.Example(y2)
        ClassWithArrayCtor(y1) |> ignore
        ClassWithArrayCtor(y2) |> ignore

        ()
        
    member this.X5 =
        let y1 = MethodsReturningHighDArrays.Example2<_>()
        let y2 = MethodsReturningHighDArrays.Example2<_,_>()

        StaticClassWithGenericMethod.Example(y1)
        StaticClassWithGenericMethod.Example(y2)
        ClassWithArrayCtor(y1) |> ignore
        ClassWithArrayCtor(y2) |> ignore

        ()

(* avoid ;; -- to avoid fsi error recovery *)
          
let _ =
  (* May not be enough if fsi does error recovery...
   * So avoid using ;;
   *)
  System.Console.WriteLine "Test Passed"; 
  System.IO.File.WriteAllText ("test.ok", "ok"); 
  exit 0
