// #Conformance #Regression 
#if Portable 
module Core_lazy
#endif

#if CoreClr
open coreclrutilities
#endif

let failures = ref false
let report_failure () = 
  stderr.WriteLine " NO"; failures := true
let test s b = stderr.Write(s:string);  if b then stderr.WriteLine " OK" else report_failure() 



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

#if Portable
#else
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

let aa =
  if !failures then (stdout.WriteLine "Test Failed"; exit 1) 

do (stdout.WriteLine "Test Passed"; 
    System.IO.File.WriteAllText("test.ok","ok"); 
    exit 0)