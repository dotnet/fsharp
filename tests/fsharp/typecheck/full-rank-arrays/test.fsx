// #TypeInference
(* Avoid ;; since it allows fsi to recover from type errors *)

#r "full-rank-arrays.dll"
open HighRankArrayTests
open System

type 't``[,,,,,,,,,]`` with
    member this.GetFirst() =
        // not supported
        // this.[0,0,0,0,0,0,0,0,0,0]
        this.GetValue(0,0,0,0,0,0,0,0,0,0)
    member this.SetFirst(x) =
        // not supported
        // this.[0,0,0,0,0,0,0,0,0,0] <- x
        this.SetValue(x,0,0,0,0,0,0,0,0,0,0)

(* In the F# 3.1.2 release and earlier versions, the compiler's overload resolution
   breaks on the following class definition. The referenced assembly defines
   several classes with various overloaded methods and constructors; some of these
   overloads include parameters whose type is a "high-rank" array -- that is,
   an array of rank 5-32. Arrays with these ranks are not currently supported by
   F#, but it should still be possible to call one of the overloads which does
   not include such a parameter. *)
type Class1() = 
    member this.X1 () =
        GenericStaticClassWithMethod<int>.Example [| 2;3;4;5 |]
        let x = Array.CreateInstance(typeof<int>, 2, 2, 2, 2, 2, 2, 2) :?> ``[,,,,,,]``<int>
        GenericStaticClassWithMethod<int>.Example(x)

    member this.X2 () =
        StaticClassWithGenericMethod.Example [| 1;2;3;4 |]
        let x = Array.CreateInstance(typeof<int>, 2, 2, 2, 2, 2, 2, 2) :?> ``[,,,,,,]``<int>
        // not supported
        //  x.[0,1,0,1,0,1,0] <- 42
        //  x.[1,0,1,0,1,0,1] |> printfn "%d"
        x.SetValue(42,0,1,0,1,0,1,0)
        downcast (x.GetValue(1,0,1,0,1,0,1)) |> printfn "%d"
        StaticClassWithGenericMethod.Example(x)

    member this.X3 () =
        ClassWithArrayCtor ([| 1;3;5;7 |]) |> printfn "%A"
        let x = Array.CreateInstance(typeof<int>, 2, 2, 2, 2, 2, 2) :?> ``[,,,,,]``<int>
        // not supported
        //   x.[0,1,0,1,0,1] <- 42
        //   x.[1,0,1,0,1,0] |> printfn "%d"
        x.SetValue(42,0,1,0,1,0,1)
        downcast (x.GetValue(1,0,1,0,1,0)) |> printfn "%d"
        ClassWithArrayCtor(x) |> printfn "%A"
        ()
        
    member this.X4 () =
        let y1 = MethodsReturningHighDArrays.Example1<int>()
        let y2 = MethodsReturningHighDArrays.Example1<int>(2)

        StaticClassWithGenericMethod.Example(y1)
        StaticClassWithGenericMethod.Example(y2)
        let x = Array.CreateInstance(typeof<int>, 2, 2, 2, 2, 2, 2, 2, 2) :?> ``[,,,,,,,]``<int>
        // not supported
        //    x.[0,1,0,1,0,1,0,1] <- 42
        //   x.[1,0,1,0,1,0,1,0] |> printfn "%d"
        x.SetValue(42,0,1,0,1,0,1,0,1)
        downcast (x.GetValue(1,0,1,0,1,0,1,0)) |> printfn "%d"
        StaticClassWithGenericMethod.Example(x)

        ClassWithArrayCtor(y1) |> printfn "%A"
        ClassWithArrayCtor(y2) |> printfn "%A"

        ()
        
    member this.X5 () =
        let y1 = MethodsReturningHighDArrays.Example2<_>()
        let y2 = MethodsReturningHighDArrays.Example2<_,_>()

        StaticClassWithGenericMethod.Example(y1)
        StaticClassWithGenericMethod.Example(y2)
        ClassWithArrayCtor(y1) |> printfn "%A"
        ClassWithArrayCtor(y2) |> printfn "%A"

        ()

    member this.X6 () = 
        let getFirst (x:``[,,,,,,,,,]``<'t>) : 't = 
            downcast (x.GetFirst())
        let setFirst y (x:``[,,,,,,,,,]``<'t>) = 
            x.SetFirst(y)

        let x = Array.CreateInstance(typeof<string>, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2) :?> ``[,,,,,,,,,]``<string>
        x |> setFirst "foo"
        x |> getFirst |> printfn "%s"
            

    member this.RunTests() =
        this.X1()
        this.X2()
        this.X3()
        this.X4()
        this.X5()
        this.X6()

(* avoid ;; -- to avoid fsi error recovery *)
          
let _ =
  (* May not be enough if fsi does error recovery...
   * So avoid using ;;
   *)
  let x = Class1()
  x.RunTests()
  System.Console.WriteLine "Test Passed"; 
  printf "TEST PASSED OK" 
  exit 0
