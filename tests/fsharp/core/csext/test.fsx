// #Conformance #Structs #Interop 
#if Portable
module Core_csext
#endif

#light


let failures = ref false
let report_failure () = 
  stderr.WriteLine " NO"; failures := true
let test s b = stderr.Write(s:string);  if b then stderr.WriteLine " OK" else report_failure() 


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

let aa =
  if !failures then (stdout.WriteLine "Test Failed"; exit 1) 

do (stdout.WriteLine "Test Passed"; 
    System.IO.File.WriteAllText("test.ok","ok"); 
    exit 0)