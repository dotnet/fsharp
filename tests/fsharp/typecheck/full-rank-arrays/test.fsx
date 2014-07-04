// #TypeInference
(* Avoid ;; since it allows fsi to recover from type errors *)

#r "HighRankArrayTests.dll"
open HighRankArrayTests

(* In the F# 3.1 release and earlier versions, the compiler's overload resolution
   breaks on the following class definition. The referenced assembly defines
   several classes with various overloaded methods and constructors; some of these
   overloads include parameters whose type is a "high-rank" array -- that is,
   an array of rank 5-32. Arrays with these ranks are not currently supported by
   F#, but it should still be possible to call one of the overloads which does
   not include such a parameter. *)
type Class1() = 
    member this.X =
        GenericStaticClassWithMethod<int>.Example [| 2;3;4;5 |]

    member this.Y =
        StaticClassWithGenericMethod.Example [| 1;2;3;4 |]

    member this.Z =
        let foo = ClassWithArrayCtor ([| 1;3;5;7 |])
        ()
        
    member this.W (foo : int[,,,,,,,,,,]) =
        printfn "Hello World!"
        ()
        
(* avoid ;; -- to avoid fsi error recovery *)
          
let _ =
  (* May not be enough if fsi does error recovery...
   * So avoid using ;;
   *)
  System.Console.WriteLine "Test Passed"; 
  System.IO.File.WriteAllText ("test.ok", "ok"); 
  exit 0
