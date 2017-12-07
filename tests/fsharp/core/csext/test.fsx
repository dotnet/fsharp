// #Conformance #Structs #Interop 
#if TESTS_AS_APP
module Core_csext
#endif

#light


let failures = ref []

let report_failure (s : string) = 
    stderr.Write" NO: "
    stderr.WriteLine s
    failures := !failures @ [s]

let test (s : string) b = 
    stderr.Write(s)
    if b then stderr.WriteLine " OK"
    else report_failure (s)

let check s b1 b2 = test s (b1 = b2)


open System.Linq

let x = [1;2;3]

let xie = (x :> seq<_>)

xie.All(fun x -> x > 1)

xie.Average()
x.Average()

[<Struct>]
type S(v:int) =
    interface System.Collections.Generic.IEnumerable<int> with 
        member x.GetEnumerator() = (Seq.singleton v).GetEnumerator() 
    interface System.Collections.IEnumerable with 
        member x.GetEnumerator() = ((Seq.singleton v).GetEnumerator() :> System.Collections.IEnumerator)

let s : S = S(3)

s.Average()

        
[<Struct>]
type Struct(i:int) = 
    static let yellowStruct  = Struct(1)
    static let blueStruct  = Struct(0)

    static member YellowStruct  = yellowStruct
    static member BlueStruct  = blueStruct


(*--------------------*)  

#if TESTS_AS_APP
let RUN() = !failures
#else
let aa =
  match !failures with 
  | [] -> 
      stdout.WriteLine "Test Passed"
      System.IO.File.WriteAllText("test.ok","ok")
      exit 0
  | _ -> 
      stdout.WriteLine "Test Failed"
      exit 1
#endif



