// #Conformance #LetBindings #Recursion #TypeInference #ObjectConstructors #Classes #Records 

module rec Core_letrec_mutrec2

let failures = ref false
let report_failure s = 
  stderr.WriteLine ("FAIL: "+s); failures := true

let test t s1 s2 = 
  if s1 <> s2 then 
    (stderr.WriteLine ("test "+t+" failed");
     failures := true)
  else
    stdout.WriteLine ("test "+t+" succeeded")   

let a1 = 1 
let b = a1
do if a1 <> 1 then report_failure "celkewieds32w8w"
do if b <> a1 then report_failure "cel3f98u8w"



let a2 = test "grekjre" (b2 + 1 ) 3
let b2  = 2



let nonRecursiveImmediate () = 
  stdout.WriteLine "Testing nonRecursiveImmediate";
  let x = ref 1 in 
  let rec a1 = (x := 3; !x) 
  and b = a1 in 
  if a1 <> 3 then report_failure "dqwij";
  if b <> 3 then report_failure "dqwecqwij"

do nonRecursiveImmediate()
do nonRecursiveImmediate()

let recObj = {new System.Object() with member __.GetHashCode() = (recObj.ToString()).Length}

do Printf.printf "recObj.GetHashCode() = %d\n" (recObj.GetHashCode())
do Printf.printf "recObj.ToString() = %s\n" (recObj.ToString())
do if recObj.GetHashCode() <> (recObj.ToString()).Length then report_failure "dqwij"


let WouldFailAtRuntimeTest () = 
  let rec a2 = (fun x -> stdout.WriteLine "a2app"; stderr.Flush();  a2 + 2) (stdout.WriteLine "a2arg"; stderr.Flush(); 1) in 
  a2

do try WouldFailAtRuntimeTest (); report_failure "fwoi-03" with _ -> stdout.WriteLine "caught ok!"

let WouldFailAtRuntimeTest2 () = 
  let rec a2 = (fun x -> a3 + 2) 1 
  and a3 = (fun x -> a2 + 2) 1 in 
  a2 + a3



module InitializationGraphAtTopLevel = 

    let nyi2 (callback) = callback
    let aaa = nyi2 (fun () -> ggg(); )
    let ggg ()  = (bbb = false)
    let bbb  = true
      


module ClassInitTests = 
    // one initial do bindings - raises exception
    type FooFail1() as this =
        do 
            printfn "hi"
            this.Bar()
        let x = 3
        do 
            printfn "bye"
            this.Bar()

        member this.Bar() = printfn "Bar %d" x

    // two initial do bindings - raises exception
    type FooFail2() as this =
        do 
            printfn "hi"
            this.Bar()
        do 
            printfn "hi"
            this.Bar()
        let x = 3
        do 
            printfn "bye"
            this.Bar()

        member this.Bar() = printfn "Bar %d" x
    
    // one initial let _ bindings - raises exception
    type FooFail3() as this =        
        let _ = 
            printfn "hi"
            this.Bar()
        let x = 3
        do 
            printfn "bye"
            this.Bar()

        member this.Bar() = printfn "Bar %d" x

    // one initial let _ bindings then one initial do binding - raises exception
    type FooFail4() as this =
        let _ =
            printfn "hi"
        do 
            printfn "hi"
            this.Bar()
        let x = 3
        do 
            printfn "bye"
            this.Bar()

        member this.Bar() = printfn "Bar %d" x

    // two initial let _ bindings - raises exception
    type FooFail5() as this =
        let _ =
            printfn "hi"
            this.Bar()
        let _ = 
            printfn "hi"
            this.Bar()
        let x = 3
        do 
            printfn "bye"
            this.Bar()

        member this.Bar() = printfn "Bar %d" x

    // one initial let _ bindings then one initial do binding - raises exception
    type FooFail6() as this =
        let _ =
            printfn "hi"
            this.Bar()
        do 
            printfn "hi"
        let x = 3
        do 
            printfn "bye"
            this.Bar()

        member this.Bar() = printfn "Bar %d" x

    
    // no initial do bindings - succeeds
    type FooSucceeds() as this =
        let x = 3
        do 
            printfn "bye"
            this.Bar()

        member this.Bar() = printfn "Bar %d" x

    test "cneqec21" (try new FooFail1() |> ignore; false with :? System.InvalidOperationException -> true) true
    test "cneqec22" (try new FooFail2() |> ignore; false with :? System.InvalidOperationException -> true) true
    test "cneqec23" (try new FooFail3() |> ignore; false with :? System.InvalidOperationException -> true) true
    test "cneqec24" (try new FooFail4() |> ignore; false with :? System.InvalidOperationException -> true) true
    test "cneqec25" (try new FooFail5() |> ignore; false with :? System.InvalidOperationException -> true) true
    test "cneqec26" (try new FooFail6() |> ignore; false with :? System.InvalidOperationException -> true) true
    test "cneqec27" (try new FooSucceeds() |> ignore; false with :? System.InvalidOperationException -> true) false


module BasicPermutations = 

    module Perm1 = 
        let A1 = 1 
        let A2 = A1
        
        test "vsdlknv01" (A1,A2) (1,1)
    
    module Perm2 = 
        let A1 = A2
        let A2 = 1
        
        test "vsdlknv02" (A1,A2) (1,1)
    
    module Perm3a = 
        let A1 = A2
        let A2 = 1
        let A3 = 1
        
        test "vsdlknv03" (A1,A2,A3) (1,1,1)
        
    module Perm3b = 
        let A1 = A2
        let A2 = A3
        let A3 = 1
        
        test "vsdlknv04" (A1,A2,A3) (1,1,1)
        
    module Perm3c = 
        let A1 = A2
        let A2 = 1
        let A3 = A2
        
        test "vsdlknv05" (A1,A2,A3) (1,1,1)
        
    module Perm3d = 
        let A1 = A3
        let A2 = 1
        let A3 = 1
        
        test "vsdlknv06" (A1,A2,A3) (1,1,1)

    module Perm3e = 
        let A1 = A3
        let A2 = A3
        let A3 = 1
        
        test "vsdlknv07" (A1,A2,A3) (1,1,1)

    module Perm3f = 
        let A1 = A3
        let A2 = 1
        let A3 = A2
        
        test "vsdlknv08" (A1,A2,A3) (1,1,1)

    module Perm3g = 
        let A1 = A3
        let A2 = A3
        let A3 = 1
        
        test "vsdlknv09" (A1,A2,A3) (1,1,1)

    module Perm3h = 
        let A1 = 1
        let A2 = A1
        let A3 = A2
        
        test "vsdlknv0q" (A1,A2,A3) (1,1,1)

    module Perm3i = 
        let A1 = 1
        let A2 = A3
        let A3 = 1
        
        test "vsdlknv0w" (A1,A2,A3) (1,1,1)

    module Perm4i = 
        let A1 = A4
        let A2 = 1
        let A3 = A2
        let A4 = A3
        
        test "vsdlknv0e" (A1,A2,A3,A4) (1,1,1,1)

    module PermMisc = 
        let A1 = A4 + 1
        let A2 = 1
        let A3 = A2 + 1
        let A4 = A3 + 1
        
        test "vsdlknv0r" (A1,A2,A3,A4) (4,1,2,3)
        
    module bug162155 =

        type SuperType1() =

          abstract Foo : int -> int

          default x.Foo a = a + 1

         

        type SubType1() =

          inherit SuperType1()

          override x.Foo a = base.Foo(a)

         

        type SuperType2() =

          abstract Foo : int -> int

          default x.Foo a = a + 1

         

        type SubType2 =

          inherit SuperType2

          new () = { inherit SuperType2() }

          override x.Foo a = base.Foo(a)

         
        type SuperType3() =

         abstract Foo : int -> int

          default x.Foo a = a + 1

         

        type SubType3() =

          inherit SuperType3()

          override x.Foo a = base.Foo(a)



        type SuperType4() =
          abstract Foo : int -> int
          default x.Foo a = a + 1
         
        type SubType4 =
          inherit SuperType4
          new () = { inherit SuperType4() }
         
          // With visual studio SP1 this will not compile
          // with the following error:
          // 
          // Error    1      'base' values may only be used to make direct calls 
          // to the base implementations of overridden members <file> <line>
          override x.Foo a = base.Foo(a)


#if TESTS_AS_APP
let aa = 
    if !failures then (stdout.WriteLine "Test Failed"; exit 1) 
    else (stdout.WriteLine "Test Passed"; exit 0)
#else
let aa = 
  if !failures then (stdout.WriteLine "Test Failed"; exit 1) 


do (stdout.WriteLine "Test Passed"; 
    printf "TEST PASSED OK"; 
    exit 0)
#endif