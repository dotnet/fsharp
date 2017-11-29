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


#nowarn "3220"

module TestsExplicitUseOfTupleProperties = 
    // all give a warning, suppressed in this file
    let x1 = 
       [ (1,2).Item1 
         (1,2).Item2
         (1,2,3).Item1
         (1,2,3).Item2
         (1,2,3).Item3
         (1,2,3,4).Item1
         (1,2,3,4).Item2
         (1,2,3,4).Item3
         (1,2,3,4).Item4
         (1,2,3,4,5).Item1
         (1,2,3,4,5).Item2
         (1,2,3,4,5).Item3
         (1,2,3,4,5).Item4
         (1,2,3,4,5).Item5
         (1,2,3,4,5,6).Item1
         (1,2,3,4,5,6).Item2
         (1,2,3,4,5,6).Item3
         (1,2,3,4,5,6).Item4
         (1,2,3,4,5,6).Item5
         (1,2,3,4,5,6).Item6
         (1,2,3,4,5,6,7).Item1
         (1,2,3,4,5,6,7).Item2
         (1,2,3,4,5,6,7).Item3
         (1,2,3,4,5,6,7).Item4
         (1,2,3,4,5,6,7).Item5
         (1,2,3,4,5,6,7).Item6
         (1,2,3,4,5,6,7).Item7
         (1,2,3,4,5,6,7,8).Item1
         (1,2,3,4,5,6,7,8).Item2
         (1,2,3,4,5,6,7,8).Item3
         (1,2,3,4,5,6,7,8).Item4
         (1,2,3,4,5,6,7,8).Item5
         (1,2,3,4,5,6,7,8).Item6
         (1,2,3,4,5,6,7,8).Item7
         (1,2,3,4,5,6,7,8,9).Item1
         (1,2,3,4,5,6,7,8,9).Item2
         (1,2,3,4,5,6,7,8,9).Item3
         (1,2,3,4,5,6,7,8,9).Item4
         (1,2,3,4,5,6,7,8,9).Item5
         (1,2,3,4,5,6,7,8,9).Item6
         (1,2,3,4,5,6,7,8,9).Item7
         (1,2).get_Item1()
         (1,2).get_Item2()
         (1,2,3).get_Item1()
         (1,2,3).get_Item2()
         (1,2,3).get_Item3()
         (1,2,3,4).get_Item1()
         (1,2,3,4).get_Item2()
         (1,2,3,4).get_Item3()
         (1,2,3,4).get_Item4()
         (1,2,3,4,5).get_Item1()
         (1,2,3,4,5).get_Item2()
         (1,2,3,4,5).get_Item3()
         (1,2,3,4,5).get_Item4()
         (1,2,3,4,5).get_Item5()
         (1,2,3,4,5,6).get_Item1()
         (1,2,3,4,5,6).get_Item2()
         (1,2,3,4,5,6).get_Item3()
         (1,2,3,4,5,6).get_Item4()
         (1,2,3,4,5,6).get_Item5()
         (1,2,3,4,5,6).get_Item6()
         (1,2,3,4,5,6,7).get_Item1()
         (1,2,3,4,5,6,7).get_Item2()
         (1,2,3,4,5,6,7).get_Item3()
         (1,2,3,4,5,6,7).get_Item4()
         (1,2,3,4,5,6,7).get_Item5()
         (1,2,3,4,5,6,7).get_Item6()
         (1,2,3,4,5,6,7).get_Item7()
         (1,2,3,4,5,6,7,8).get_Item1()
         (1,2,3,4,5,6,7,8).get_Item2()
         (1,2,3,4,5,6,7,8).get_Item3()
         (1,2,3,4,5,6,7,8).get_Item4()
         (1,2,3,4,5,6,7,8).get_Item5()
         (1,2,3,4,5,6,7,8).get_Item6()
         (1,2,3,4,5,6,7,8).get_Item7()
         (1,2,3,4,5,6,7,8,9).get_Item1()
         (1,2,3,4,5,6,7,8,9).get_Item2()
         (1,2,3,4,5,6,7,8,9).get_Item3()
         (1,2,3,4,5,6,7,8,9).get_Item4()
         (1,2,3,4,5,6,7,8,9).get_Item5()
         (1,2,3,4,5,6,7,8,9).get_Item6()
         (1,2,3,4,5,6,7,8,9).get_Item7() ]

    printfn "x1 = %A" x1
    check "vwhnwrvep01" x1 [1; 2; 1; 2; 3; 1; 2; 3; 4; 1; 2; 3; 4; 5; 1; 2; 3; 4; 5; 6; 1; 2; 3; 4; 5; 6; 7; 1; 2; 3; 4; 5; 6; 7; 1; 2; 3; 4; 5; 6; 7;
                            1; 2; 1; 2; 3; 1; 2; 3; 4; 1; 2; 3; 4; 5; 1; 2; 3; 4; 5; 6; 1; 2; 3; 4; 5; 6; 7; 1; 2; 3; 4; 5; 6; 7; 1; 2; 3; 4; 5; 6; 7]

    let x2 = (1,2,3,4,5,6,7,8).Rest // gives a warning, suppressed in this file
    printfn "x2 = %A" x2
    check "vwhnwrvep02" x2 (System.Tuple(8))

    let x3 = (1,2,3,4,5,6,7,8,9).Rest // gives a warning, suppressed in this file
    printfn "x3 = %A" x3
    check "vwhnwrvep03" x3 (unbox (box (8,9)))
    
    // check a quotation of these
    let tup = (1,2)
    let x4 = <@ tup.Item1 @>

    let text = sprintf "%A" x4
    printfn "%s" text
    check "vewjwervwver" text "PropertyGet (Some (PropertyGet (None, tup, [])), Item1, [])"

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



