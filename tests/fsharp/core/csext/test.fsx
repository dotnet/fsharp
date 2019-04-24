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

// BUGBUG: https://github.com/Microsoft/visualfsharp/issues/6601
//[<Struct>]
//type S(v:int) =
//    interface System.Collections.Generic.IEnumerable<int> with 
//        member x.GetEnumerator() = (Seq.singleton v).GetEnumerator() 
//    interface System.Collections.IEnumerable with 
//        member x.GetEnumerator() = ((Seq.singleton v).GetEnumerator() :> System.Collections.IEnumerator)
//
//let s : S = S(3)
//s.Average()

        
[<Struct>]
type Struct(i:int) = 
    static let yellowStruct  = Struct(1)
    static let blueStruct  = Struct(0)

    static member YellowStruct  = yellowStruct
    static member BlueStruct  = blueStruct


#nowarn "3220"

// See https://github.com/Microsoft/visualfsharp/pull/3729
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

    let x2 : System.Tuple<int> = (1,2,3,4,5,6,7,8).Rest // gives a warning, suppressed in this file
    printfn "x2 = %A" x2
    check "vwhnwrvep02" x2 (System.Tuple(8))

    let x3 : (int * int) = (1,2,3,4,5,6,7,8,9).Rest // gives a warning, suppressed in this file
    printfn "x3 = %A" x3
    check "vwhnwrvep03" x3 (unbox (box (8,9)))
    
    // check a quotation of these
    let tup = (1,2)
    let x4 = <@ tup.Item1 @>

    let text = sprintf "%A" x4
    printfn "%s" text
    check "vewjwervwver" text "PropertyGet (Some (PropertyGet (None, tup, [])), Item1, [])"

(*  
// See https://github.com/Microsoft/visualfsharp/pull/3729, struct tuple access to Item* and Rest* are not supported
module TestsExplicitUseOfStructTupleProperties = 
    // all give a warning, suppressed in this file
    let x1 = 
       [ (struct (1,2)).Item1 
         (struct (1,2)).Item2
         (struct (1,2,3)).Item1
         (struct (1,2,3)).Item2
         (struct (1,2,3)).Item3
         (struct (1,2,3,4)).Item1
         (struct (1,2,3,4)).Item2
         (struct (1,2,3,4)).Item3
         (struct (1,2,3,4)).Item4
         (struct (1,2,3,4,5)).Item1
         (struct (1,2,3,4,5)).Item2
         (struct (1,2,3,4,5)).Item3
         (struct (1,2,3,4,5)).Item4
         (struct (1,2,3,4,5)).Item5
         (struct (1,2,3,4,5,6)).Item1
         (struct (1,2,3,4,5,6)).Item2
         (struct (1,2,3,4,5,6)).Item3
         (struct (1,2,3,4,5,6)).Item4
         (struct (1,2,3,4,5,6)).Item5
         (struct (1,2,3,4,5,6)).Item6
         (struct (1,2,3,4,5,6,7)).Item1
         (struct (1,2,3,4,5,6,7)).Item2
         (struct (1,2,3,4,5,6,7)).Item3
         (struct (1,2,3,4,5,6,7)).Item4
         (struct (1,2,3,4,5,6,7)).Item5
         (struct (1,2,3,4,5,6,7)).Item6
         (struct (1,2,3,4,5,6,7)).Item7
         (struct (1,2,3,4,5,6,7,8)).Item1
         (struct (1,2,3,4,5,6,7,8)).Item2
         (struct (1,2,3,4,5,6,7,8)).Item3
         (struct (1,2,3,4,5,6,7,8)).Item4
         (struct (1,2,3,4,5,6,7,8)).Item5
         (struct (1,2,3,4,5,6,7,8)).Item6
         (struct (1,2,3,4,5,6,7,8)).Item7
         (struct (1,2,3,4,5,6,7,8,9)).Item1
         (struct (1,2,3,4,5,6,7,8,9)).Item2
         (struct (1,2,3,4,5,6,7,8,9)).Item3
         (struct (1,2,3,4,5,6,7,8,9)).Item4
         (struct (1,2,3,4,5,6,7,8,9)).Item5
         (struct (1,2,3,4,5,6,7,8,9)).Item6
         (struct (1,2,3,4,5,6,7,8,9)).Item7
         (struct (1,2)).get_Item1()
         (struct (1,2)).get_Item2()
         (struct (1,2,3)).get_Item1()
         (struct (1,2,3)).get_Item2()
         (struct (1,2,3)).get_Item3()
         (struct (1,2,3,4)).get_Item1()
         (struct (1,2,3,4)).get_Item2()
         (struct (1,2,3,4)).get_Item3()
         (struct (1,2,3,4)).get_Item4()
         (struct (1,2,3,4,5)).get_Item1()
         (struct (1,2,3,4,5)).get_Item2()
         (struct (1,2,3,4,5)).get_Item3()
         (struct (1,2,3,4,5)).get_Item4()
         (struct (1,2,3,4,5)).get_Item5()
         (struct (1,2,3,4,5,6)).get_Item1()
         (struct (1,2,3,4,5,6)).get_Item2()
         (struct (1,2,3,4,5,6)).get_Item3()
         (struct (1,2,3,4,5,6)).get_Item4()
         (struct (1,2,3,4,5,6)).get_Item5()
         (struct (1,2,3,4,5,6)).get_Item6()
         (struct (1,2,3,4,5,6,7)).get_Item1()
         (struct (1,2,3,4,5,6,7)).get_Item2()
         (struct (1,2,3,4,5,6,7)).get_Item3()
         (struct (1,2,3,4,5,6,7)).get_Item4()
         (struct (1,2,3,4,5,6,7)).get_Item5()
         (struct (1,2,3,4,5,6,7)).get_Item6()
         (struct (1,2,3,4,5,6,7)).get_Item7()
         (struct (1,2,3,4,5,6,7,8)).get_Item1()
         (struct (1,2,3,4,5,6,7,8)).get_Item2()
         (struct (1,2,3,4,5,6,7,8)).get_Item3()
         (struct (1,2,3,4,5,6,7,8)).get_Item4()
         (struct (1,2,3,4,5,6,7,8)).get_Item5()
         (struct (1,2,3,4,5,6,7,8)).get_Item6()
         (struct (1,2,3,4,5,6,7,8)).get_Item7()
         (struct (1,2,3,4,5,6,7,8,9)).get_Item1()
         (struct (1,2,3,4,5,6,7,8,9)).get_Item2()
         (struct (1,2,3,4,5,6,7,8,9)).get_Item3()
         (struct (1,2,3,4,5,6,7,8,9)).get_Item4()
         (struct (1,2,3,4,5,6,7,8,9)).get_Item5()
         (struct (1,2,3,4,5,6,7,8,9)).get_Item6()
         (struct (1,2,3,4,5,6,7,8,9)).get_Item7() ]

    printfn "x1 = %A" x1
    check "vwhnwrvep01" x1 [1; 2; 1; 2; 3; 1; 2; 3; 4; 1; 2; 3; 4; 5; 1; 2; 3; 4; 5; 6; 1; 2; 3; 4; 5; 6; 7; 1; 2; 3; 4; 5; 6; 7; 1; 2; 3; 4; 5; 6; 7;
                            1; 2; 1; 2; 3; 1; 2; 3; 4; 1; 2; 3; 4; 5; 1; 2; 3; 4; 5; 6; 1; 2; 3; 4; 5; 6; 7; 1; 2; 3; 4; 5; 6; 7; 1; 2; 3; 4; 5; 6; 7]

    let x2 = (struct (1,2,3,4,5,6,7,8)).Rest // gives a warning, suppressed in this file
    printfn "x2 = %A" x2
    check "vwhnwrvep02" x2 (System.Tuple(8))

    let x3 = (struct (1,2,3,4,5,6,7,8,9)).Rest // gives a warning, suppressed in this file
    printfn "x3 = %A" x3
    check "vwhnwrvep03" x3 (unbox (box (struct (8,9))))
    
    // check a quotation of these
    let tup = (struct (1,2))
    let x4 = <@ tup.Item1 @>

    let text = sprintf "%A" x4
    printfn "%s" text
    check "vewjwervwver" text "PropertyGet (Some (PropertyGet (None, tup, [])), Item1, [])"
*)

module TupleCtors = 
    let t1 x = new System.Tuple<_>(x)
    let t2 (x1,x2) = new System.Tuple<_,_>(x1,x2)
    let t3 (x1,x2,x3) = new System.Tuple<_,_,_>(x1,x2,x3)
    let t4 (x1,x2,x3,x4) = new System.Tuple<_,_,_,_>(x1,x2,x3,x4)
    let t5 (x1,x2,x3,x4,x5) = new System.Tuple<_,_,_,_,_>(x1,x2,x3,x4,x5)
    let t6 (x1,x2,x3,x4,x5,x6) = new System.Tuple<_,_,_,_,_,_>(x1,x2,x3,x4,x5,x6)
    let t7 (x1,x2,x3,x4,x5,x6,x7) = new System.Tuple<_,_,_,_,_,_,_>(x1,x2,x3,x4,x5,x6,x7)
    let cp (x1,x2,x3,x4,x5,x6,x7) t = new System.Tuple<_,_,_,_,_,_,_,_>(x1,x2,x3,x4,x5,x6,x7, t)

module TupleSRTP = 
    let t1 x = new System.Tuple<_>(x)
    let t2 (x1,x2) = new System.Tuple<_,_>(x1,x2)
    let t3 (x1,x2,x3) = new System.Tuple<_,_,_>(x1,x2,x3)
    let t4 (x1,x2,x3,x4) = new System.Tuple<_,_,_,_>(x1,x2,x3,x4)
    let t5 (x1,x2,x3,x4,x5) = new System.Tuple<_,_,_,_,_>(x1,x2,x3,x4,x5)
    let t6 (x1,x2,x3,x4,x5,x6) = new System.Tuple<_,_,_,_,_,_>(x1,x2,x3,x4,x5,x6)
    let t7 (x1,x2,x3,x4,x5,x6,x7) = new System.Tuple<_,_,_,_,_,_,_>(x1,x2,x3,x4,x5,x6,x7)
    let cp (x1,x2,x3,x4,x5,x6,x7) t = new System.Tuple<_,_,_,_,_,_,_,_>(x1,x2,x3,x4,x5,x6,x7, t)

    type T = T with
        static member inline ($) (T, t:System.Tuple<_,_,_,_,_,_,_,'rst>) = fun x -> cp (x,t.Item1, t.Item2,t.Item3,t.Item4,t.Item5,t.Item6) ((T $ t.Rest) t.Item7)
        static member        ($) (T, ())                          = fun x -> t1 (x)
        static member        ($) (T, t:System.Tuple<_>)                  = fun x -> t2 (x,t.Item1)
        static member        ($) (T, t:System.Tuple<_,_>)                = fun x -> t3 (x,t.Item1, t.Item2)
        static member        ($) (T, t:System.Tuple<_,_,_>)              = fun x -> t4 (x,t.Item1, t.Item2,t.Item3)
        static member        ($) (T, t:System.Tuple<_,_,_,_>)            = fun x -> t5 (x,t.Item1, t.Item2,t.Item3,t.Item4)
        static member        ($) (T, t:System.Tuple<_,_,_,_,_>)          = fun x -> t6 (x,t.Item1, t.Item2,t.Item3,t.Item4,t.Item5)
        static member        ($) (T, t:System.Tuple<_,_,_,_,_,_>)        = fun x -> t7 (x,t.Item1, t.Item2,t.Item3,t.Item4,t.Item5,t.Item6)
        static member        ($) (T, t:System.Tuple<_,_,_,_,_,_,_>)      = fun x -> cp (x,t.Item1, t.Item2,t.Item3,t.Item4,t.Item5,t.Item6) (t1(t.Item7))


    let v1 =  (^T : (member get_Item1 : unit -> _ ) (new System.Tuple<int,int>(1,3)))
    let v2 =  (^T : (member get_Item1 : unit -> _ ) (System.Tuple<int,int>(1,3)))
    let v3 =  (^T : (member get_Item1 : unit -> _ ) ((1,3)))



    let v1b =  (^T : (member get_Item2 : unit -> _ ) (new System.Tuple<int,int>(1,3)))
    let v2b =  (^T : (member get_Item2 : unit -> _ ) (System.Tuple<int,int>(1,3)))
    let v3b =  (^T : (member get_Item2 : unit -> _ ) ((1,3)))

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



