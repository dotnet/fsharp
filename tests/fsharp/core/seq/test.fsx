// #Regression #Conformance #Sequences 
#if Portable
module Core_seq
#endif

#nowarn "62"
#nowarn "44"

let mutable failures = []
let reportFailure s = 
  stdout.WriteLine "\n................TEST FAILED...............\n"; failures <- failures @ [s]

(* TEST SUITE FOR STANDARD LIBRARY *)

#if NetCore
#else
let argv = System.Environment.GetCommandLineArgs() 
let SetCulture() = 
  if argv.Length > 2 && argv.[1] = "--culture" then  begin
    let cultureString = argv.[2] in 
    let culture = new System.Globalization.CultureInfo(cultureString) in 
    stdout.WriteLine ("Running under culture "+culture.ToString()+"...");
    System.Threading.Thread.CurrentThread.CurrentCulture <-  culture
  end 
  
do SetCulture()    
#endif

let check s e r = 
  if r = e then  stdout.WriteLine (s^": YES") 
  else (stdout.WriteLine ("\n***** "^s^": FAIL\n"); reportFailure s)

let test s b = 
  if b then ( (* stdout.WriteLine ("passed: " + s) *) ) 
  else (stderr.WriteLine ("failure: " + s); 
        reportFailure s)

  
check "rwfsjkla"
   (let mutable results = []
    let ys =
        seq {
            try
                try
                    failwith "foo"
                finally
                    results <- 1::results
                    failwith "bar"
            finally
                results <- 2::results
        }        
    try
        for _ in ys do ()
    with
        Failure "bar" -> results <- 3::results
    results)
    [3;2;1]
    
check "fgyeyrkerkl"
   (let mutable results = []
    let xs = 
        seq {
            try
                try
                    failwith "foo"
                finally
                    results <- "a"::results
                    failwith "bar"
            finally
              results <- "c"::results
              failwith "bar1"
        }

    let ys =
        seq {
            yield 1
            yield! xs
        }
    try
        for _ in ys do ()
    with
        Failure "bar1" -> results <- "with"::results
    results)
    ["with";"c";"a"]


check "rwfsfsdgba"
   (let results = ref []
    let ys =
        seq {
            try
                try
                    do ()
                finally
                    results := 1::!results
                    failwith "bar"
            finally
                results := 2::!results
                failwith "bar2"
        }        
    try
        for _ in ys do ()
    with
        Failure "bar2" -> results := 3::!results
    !results)
    [3;2;1]

check "fgwehyr1"
   (let mutable results = []
    let mutable outerFinallyCalled = false
    let mutable innerFinallyCalled = false
    let ys =
       seq {
          try 
             try
                yield 1
                failwith "kaboom"
                yield 2
             finally
                innerFinallyCalled <- true
          finally
             outerFinallyCalled <- true
       }
    // Capturing precisely when what happens
    let yIter = ys.GetEnumerator()
    yIter.MoveNext() |> ignore
    try
        yIter.MoveNext() |> ignore
    with
        Failure "kaboom" ->
            results <- "kaboom"::results
            
    match innerFinallyCalled, outerFinallyCalled with
    |   false,false -> 
            results <- "beforeFinallyOk"::results
    |   _ -> ()            
    yIter.Dispose()
    match innerFinallyCalled, outerFinallyCalled with
    |   true,true -> 
            results <- "afterFinallyOk"::results
    |   _ -> ()
    results)
    ["afterFinallyOk";"beforeFinallyOk";"kaboom"]

check "fgwehyr2"
   (let results = ref []
    let outerFinallyCalled = ref false
    let innerFinallyCalled = ref false
    let ys =
       seq {
          try 
             try
                yield 1
                yield 2
             finally
                innerFinallyCalled := true
          finally
             outerFinallyCalled := true
       }
    // Capturing precisely when what happens
    let yIter = ys.GetEnumerator()
    yIter.MoveNext() |> ignore
    yIter.MoveNext() |> ignore
    yIter.MoveNext() |> ignore            
    match !innerFinallyCalled, !outerFinallyCalled with
    |   true,true -> 
            results := "finallyOk"::!results
    |   _ -> ()            
    innerFinallyCalled := false
    outerFinallyCalled := false
    yIter.Dispose()
    match !innerFinallyCalled, !outerFinallyCalled with
    |   false,false -> 
            results := "disposeOk"::!results
    |   _ -> ()            
    
    !results)
    ["disposeOk";"finallyOk"]

check "fgwehyr3"
   (let results = ref []
    let outerFinallyCalled = ref false
    let innerFinallyCalled = ref false
    let ys =
       seq {
          try 
             try
                yield 1
                yield 2
             finally
                innerFinallyCalled := true
                failwith "Kaboom"
          finally
             outerFinallyCalled := true
       }
    // Capturing precisely when what happens
    let yIter = ys.GetEnumerator()
    yIter.MoveNext() |> ignore
    yIter.MoveNext() |> ignore
    try
        yIter.MoveNext() |> ignore            
    with
        Failure "Kaboom" ->
            match !innerFinallyCalled, !outerFinallyCalled with
            |   true,false -> 
                    results := "innerFinally@lastMoveNext"::!results
            |   _ -> ()            
    innerFinallyCalled := false
    outerFinallyCalled := false
    yIter.Dispose()        
    match !innerFinallyCalled, !outerFinallyCalled with
    |   false,true -> 
            results := "outerFinally@Dispose"::!results
    |   _ -> ()            
    
    !results)
    ["outerFinally@Dispose";"innerFinally@lastMoveNext"]

check "fgryerwfre"
   (let results = ref []
    let outerFinallyCalled = ref false
    let innerFinallyCalled = ref false
    let ys i =
        seq {
            match i with
            |   1 ->
                    try
                        try 
                            failwith "foo"
                        finally
                            innerFinallyCalled := true
                    finally
                        outerFinallyCalled := true
            |   _ ->
                    do ()
        }
    try
        for _ in ys 1 do ()
    with
        Failure "foo" -> 
            match !innerFinallyCalled, !outerFinallyCalled with
            |   true, true -> results := "ok1"::!results
            |   _ -> ()

    innerFinallyCalled := false
    outerFinallyCalled := false
    for _ in ys 2 do ()
    match !innerFinallyCalled, !outerFinallyCalled with
    |   false, false -> results := "ok2"::!results
    |   _ -> ()
    !results)
    ["ok2";"ok1"]          
                        
check "rt6we56qera"
   (let results = ref []
    let innerFinallyCalled = ref false
    let middleFinallyCalled = ref false
    let outerFinallyCalled = ref false
    let ys =
        seq {
            try
                try
                    try
                        yield 0
                        failwith "inner"
                        yield 1
                    finally
                        innerFinallyCalled := true
                        failwith "middle"
                    yield 2
                finally
                    middleFinallyCalled := true
                    failwith "outer"
                yield 3
            finally 
                outerFinallyCalled := true
                failwith "outermost"
        }
    let l = ref []
    try
        for y in ys do l := y::!l
    with
        Failure "outermost" ->
            results := "expected failure"::!results
    match !l,!innerFinallyCalled,!middleFinallyCalled,!outerFinallyCalled with
    |   [0],true,true,true -> results := "ok"::!results
    |   _ -> ()
    !results)
    ["ok";"expected failure"]  
    
check "dg676rd44t"
   (let results = ref []    
    let f1 = ref false
    let f2 = ref false 
    let ys xs =
        seq {
            try
            match xs with
            | 1,h
            | h,1 -> 
                try
                    yield h
                    failwith "1"
                finally
                    f1 := true
                    failwith "2"
            | _ -> ()
            finally
                f2 := true
        }
    try
        for _ in ys (1,1) do ()
    with
        Failure "2" -> 
            match !f1,!f2 with
            |   true,true -> results := "ok"::!results
            |   _ -> ()
    !results)
    ["ok"]
type t =
|   A of int
|   B of int
|   C of string

check "rt7we6djksagd"
   (let results = ref []
    let ys x = 
       seq {
         try
           try
              match x with
              | A y | B y -> 
                     yield y
                     failwith "1"
              | _ -> 
                     yield 27
                     failwith "2"
           finally
              results := "A"::!results
              failwith "3"
         finally
           results := "B"::!results
       }

    try
        for _ in ys (A 0) do ()
    with
        Failure "3" -> results := "catch"::!results
    !results)
    ["catch";"B";"A"]

check "f6er76r23784"
   (let results = ref []
    let ys xs =
       seq {
            match xs with
            | 1,h
            | h,1 ->
                try 
                  try
                    yield h
                    failwith "1"
                  finally
                    results := "inner"::!results
                    failwith "2"
                finally
                  results := "outer"::!results
                  failwith "3"
            | _ -> ()
       }

    try
        for _ in ys (1,1) do ()
    with
        Failure "3" -> results := "catch"::!results
    !results)
    ["catch";"outer";"inner"]

check "hdweuiyrfiwe"
   (let results = ref []
    let ys xs =
        seq {
            match xs with
            | 1,h
            | h,1 ->
                use a = { new System.IDisposable with member this.Dispose() = results :=  "A"::!results }
                use b = { new System.IDisposable with member this.Dispose() = results :=  "B"::!results }
                while true do
                        failwith "boom"
            | _ -> ()
        }
    try
        for _ in ys (1,1) do ()
    with
        Failure "boom" -> results := "boom"::!results
    !results)
    ["boom";"A";"B"]

check "hdweuiyrfiwe1"
   (let results = ref []
    let ys xs =
        seq {
            use a = { new System.IDisposable with member this.Dispose() = results :=  "A"::!results }
            use b = { new System.IDisposable with member this.Dispose() = results :=  "B"::!results }
            while true do
                    failwith "boom"
        }
    try
        for _ in ys (1,1) do ()
    with
        Failure "boom" -> results := "boom"::!results
    !results)
    ["boom";"A";"B"]

check "fhduweyf"
   (let s : seq<int> = 
        seq { 
            for i in 0..3 do
                failwith "74"
                ()
            }
    let e = s.GetEnumerator()

    e.Dispose()
    try
        if e.MoveNext() then "fail" else "ok"
    with
        _ -> "exn")
    "ok"


check "fderuy" 
   (let f1 = ref false
    let f2 = ref false
    let s f s =
        seq {
            try                
                yield s
            finally
                f := true
                failwith ("foo" + s)
        }
    let result =
        try
            for _ in Seq.map2 (fun x y -> ()) (s f1 "1") (s f2 "2") do ()
            "exception didnt propagate"
        with
            _ -> 
                match !f1,!f2 with
                |   true,true -> "ok"
                |   _ -> "not all finallies run"
    result)
    "ok"
                
check "hfhdfsjkfur34"
   (let results = ref []
    let e =
        let enum () = 
            {   new System.Collections.Generic.IEnumerator<int> with
                    member this.Current = invalidOp "current"
                interface System.IDisposable with
                    member this.Dispose() =
                        results := "eDispose"::!results
                        failwith "e!!!"
                interface System.Collections.IEnumerator with
                    member this.Current = invalidOp "current"
                    member this.MoveNext() = false
                    member this.Reset() = invalidOp "reset"
            }

        {   new System.Collections.Generic.IEnumerable<int> with
                member this.GetEnumerator() = enum ()
            interface System.Collections.IEnumerable with
                member this.GetEnumerator() = enum () :> System.Collections.IEnumerator
        }
    let ss = 
        seq {
            try
                yield! e
            finally
                results := "ssDispose"::!results
                failwith "ss!!!"
        }
    try
        for _ in ss do ()
    with
        Failure "ss!!!" -> results := "caught"::!results
    !results)
    ["caught";"ssDispose";"eDispose"]
    
(*---------------------------------------------------------------------------
!* wrap up
 *--------------------------------------------------------------------------- *)

let aa =
  if not failures.IsEmpty then (printfn "Test Failed, failures = %A" failures; exit 1) 

do (stdout.WriteLine "Test Passed"; 
    System.IO.File.WriteAllText("test.ok","ok"); 
    exit 0)