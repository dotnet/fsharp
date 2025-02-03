// #Conformance #Regression 
#if TESTS_AS_APP
module Core_lazy
#endif

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



(* TEST SUITE FOR STANDARD LIBRARY *)

let x = lazy 3

module Lazy =

    type 'a t = 'a Microsoft.FSharp.Control.Lazy

    let force (x: Microsoft.FSharp.Control.Lazy<'T>) = x.Force()
    let force_val (x: Microsoft.FSharp.Control.Lazy<'T>) = x.Force()
    let lazy_from_fun f = Microsoft.FSharp.Control.Lazy.Create(f)
    let create f = Microsoft.FSharp.Control.Lazy.Create(f)
    let lazy_from_val v = Microsoft.FSharp.Control.Lazy.CreateFromValue(v)
    let lazy_is_val (x: Microsoft.FSharp.Control.Lazy<'T>) = x.IsValueCreated

do test "fewoin" (Lazy.force x = 3)

do test "fedeoin" (Lazy.force (Lazy.force (lazy (lazy 3))) = 3)

do test "fedeoin" (let x = 3 in Lazy.force (Lazy.force (lazy (lazy x))) = 3)
do test "fedeoin" (let x = 3 in Lazy.force (Lazy.force (lazy (lazy (x+x)))) = 6)
do test "fedeoin" (let x = ref 3 in let y = lazy (x := !x + 1; 6) in ignore (Lazy.force y); ignore (Lazy.force y); !x = 4)
do test "fedeoin" (let x = ref 3 in let y = lazy (x := !x + 1; "abc") in ignore (Lazy.force y); ignore (Lazy.force y); !x = 4)

#if !NETCOREAPP
module Bug5770 =
    open System.Threading
    do
        for i in 1..10 do
            let act () =
                let foo = (lazy (new obj()))
                let threadCount = 100
                let arr : exn array = Array.create threadCount null
                let count = ref 1
                let thread i = new Thread(fun () -> 
                                            let exn =
                                                try let v = foo.Value in if v = null then raise (System.NullReferenceException()) else null
                                                with e -> e
                                            lock arr (fun () -> arr.[i] <- exn)
                                            Interlocked.Increment(count) |> ignore)
                for i = 1 to threadCount do (thread (i-1)).Start()
                while !count < threadCount do ()
                Array.forall (fun x -> match x with null -> true | e -> printfn "%A" e; false) arr
            test (sprintf "fweuy42374: %d" i) (act())

#endif
        
// Check these support 'null' since they are .NET types
let x1 : System.IObservable<int> = null
let x2 : System.IObserver<int> = null
let x3 : System.Lazy<int> = null

#if TESTS_AS_APP
let RUN() = !failures
#else
let aa =
  match !failures with 
  | [] -> 
      stdout.WriteLine "Test Passed"
      printf "TEST PASSED OK" ;
      exit 0
  | _ -> 
      stdout.WriteLine "Test Failed"
      exit 1
#endif

