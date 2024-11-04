// #Conformance #ComputationExpressions #Async 

#if TESTS_AS_APP
module Core_controlStackOverflow
#endif

#light

#nowarn "40" // recursive references

#if NETCOREAPP
open System.Threading.Tasks
#endif

let biggerThanTrampoliningLimit = 10000

let failuresFile =
   let f = System.Environment.GetEnvironmentVariable("CONTROL_FAILURES_LOG")
   match f with
   | "" | null -> "failures.log"
   | _ -> f

printfn "failures file: %s" failuresFile

let log msg = 
  printfn "%s" msg
  System.IO.File.AppendAllText(failuresFile, sprintf "%A: %s\r\n" System.DateTime.Now msg)


let mutable failures = []
let syncObj = new obj()
let report_failure s = 
  stderr.WriteLine " NO"; 
  lock syncObj (fun () ->
     failures <- s :: failures;
     log (sprintf "FAILURE: %s failed" s)
  )

#if !NETCOREAPP
System.AppDomain.CurrentDomain.UnhandledException.AddHandler(
       fun _ (args:System.UnhandledExceptionEventArgs) ->
          lock syncObj (fun () ->
                failures <- (args.ExceptionObject :?> System.Exception).ToString() :: failures
             )
)
#endif

let test s b = stderr.Write(s:string);  if b then stderr.WriteLine " OK" else report_failure s 

let checkQuiet s x1 x2 = 
    if x1 <> x2 then 
        (test s false; printfn "expected: %A, got %A" x2 x1)

let check s x1 x2 = 
    if x1 = x2 then test s true
    else (test s false; printfn "expected: %A, got %A" x2 x1)

(*******************************************************************************)
open Microsoft.FSharp.Control
open Microsoft.FSharp.Control.WebExtensions


module StackDiveTests =
  [<Sealed>]
  type AsyncResultCell<'T>() =
    let mutable result = None
    let mutable savedConts : ('T -> unit) list = []
    
    let syncRoot = new obj()
            
    member x.RegisterResult (res:'T) =
        let grabbedConts = 
            lock syncRoot (fun () ->
                if result.IsSome then  
                    []
                else
                    result <- Some res;
                    List.rev savedConts)
        match grabbedConts with
        |   [] -> ()
        |   [cont] -> cont res 
        |   otherwise -> failwith "other"

    member x.AsyncResult =
        Async.FromContinuations(fun (cont,_,_) -> 
            let grabbedResult = 
                lock syncRoot (fun () ->
                    match result with
                    | Some res -> 
                        result
                    | None ->
                        savedConts <- cont::savedConts
                        None)
            match grabbedResult with 
            | None -> ()
            | Some res -> cont res) 
  let test() =
      let Main() =
        async {

            // Create the channels
            let channels = [| for i in 0 .. 100000 -> new AsyncResultCell<int>() |]

            // For each channel pair, start a task that takes the result from the right and sends it to the left
            channels 
                |> Seq.pairwise 
                |> Seq.iter (fun (c1,c2) -> 
                      Async.Start (async { let! res = c2.AsyncResult 
                                           c1.RegisterResult(res + 1); }))


            // Send the first message to the last on the right
            channels.[100000].RegisterResult(0);

            // Wait for the result from the first on the left
            let! res = channels.[0].AsyncResult

            return res
        } |> Async.RunSynchronously
  
      check "albatross"
         (Main())
         100000

      let rec so n = 
        async { if n = 0 then return 1 
                else
                  let! res = so (n-1)
                  return res + 1  }

      check "whale-1"
         (so 1000000 |> Async.RunSynchronously)
         1000001


      let rec so2 n = 
        async { if n = 0 then return 1 
                else return! so2 (n-1) }

      check "whale-2"
         (so2 1000000 |> Async.RunSynchronously)
         1


      let rec so3 n = 
        async { try 
                  if n = 0 then return 1 
                  else return! so3 (n-1) 
                with e -> return 3 }

      check "whale-3"
         (so3 1000000 |> Async.RunSynchronously)
         1


      let rec so4 n = 
        async { try 
                  if n = 0 then return 1 
                  else return! so4 (n-1) 
                finally 
                   () }

      check "whale-4"
         (so4 1000000 |> Async.RunSynchronously)
         1
  
      let justRun x = Async.FromContinuations(fun (c,e,cc) -> c x)
      let rec so5 n =
        async {
            if n = 0 then return 1
            else 
                let! x = justRun n
                return! so5 (n-1)
        }
      check "whale-5"
         (so5 1000000 |> Async.RunSynchronously)
         1
     
      let contAsync x = 
        Async.FromContinuations(fun (c,e,cc) ->
            Async.StartWithContinuations (async { return x },c,e,cc))
        
      let rec so6 n =
            async {
                if n = 0 then return 6
                else 
                    let! x = contAsync n
                    return! so6 (n-1)
            }
      check "whale-6"
          (so6 1000000 |> Async.RunSynchronously)
          6

      let throwingAsync =
            Async.FromContinuations(fun (c,e,_) -> e <| System.InvalidOperationException())
        
      let rec so7 n =
            async {
                if n = 0 then return 7
                else
                    try 
                        do! throwingAsync
                    with
                    | :? System.InvalidOperationException -> 
                        let! _ = so7 (n-1)
                        ()
                    return 85
            }
      check "whale-7"
          (so7 100000 |> Async.RunSynchronously)
          85
      
      let throwingAsync' =
            Async.FromContinuations(fun (c,e,_) -> raise <| System.InvalidOperationException())
        
      let rec so7' n =
            async {
                if n = 0 then return "7'"
                else
                    try 
                        do! throwingAsync'
                    with
                    | :? System.InvalidOperationException -> 
                        let! _ = so7' (n-1)
                        ()
                    return "7'"
            }
      check "whale-7'"
          (so7' 100000 |> Async.RunSynchronously)
          "7'"

      let quwiAsync x =
            Async.FromContinuations(fun (c,_,_) ->
#if NETCOREAPP
                Task.Run(
                    fun _ -> 
                        async { 
                            do c x
                            return()
                        } |> Async.StartImmediate
                ) |> ignore)
#else            
                System.Threading.ThreadPool.QueueUserWorkItem(
                    fun _ -> 
                        async { 
                            do c x
                            return()
                        } |> Async.StartImmediate
                ) |> ignore)
#endif
  
      let rec so8 n =
            async {
                if n = 0 then return 8
                else
                    let! n = quwiAsync (n-1)
                    for i in 1..biggerThanTrampoliningLimit do
                        ()
                    return! so8 n
            }
      check "whale-8"
          (so8 10000 |> Async.RunSynchronously)
          8


module ReturnStackoverflow =
    let test () =
        let rec so n = 
          async { if n = 0 then return 1 
                  else 
                    try 
                        return! so (n-1)
                    finally ()  }

        check "so-return"
            (so 2000000 |> Async.RunSynchronously)
            1

        let rec so2 n = 
          async { if n = 0 then new System.InvalidOperationException() |> raise
                  else 
                    try 
                        return! so2 (n-1)
                    finally ()  }

        check "so-return2"
            (try
                so2 2000000 |> Async.RunSynchronously
                "fail"
             with
             | :? System.InvalidOperationException -> "success")
            "success"

        let rec so3 n = 
          async { if n = 0 then return 1 
                  else 
                    try 
                        return! so3 (n-1)
                    with _ -> return 3  }

        check "so-return3"
            (so3 2000000 |> Async.RunSynchronously)
            1

        let rec so4 n = 
          async { if n = 0 then return (new System.InvalidOperationException() |> raise)
                  else 
                    try 
                        return! so4 (n-1)
                    with _ -> return 1  }
        check "so-return4"
            (so4 2000000 |> Async.RunSynchronously)
            1

        let rec so5 n = 
          async { if n = 0 then return 1
                  else 
                    try 
                        raise (new System.InvalidOperationException())
                        return 27
                    with _ -> 
                        let! i = so5 (n-1)
                        return i
                }
        check "so-return5"
            (so5 1000000 |> Async.RunSynchronously)
            1

        let rec so6 n = 
          async { if n = 0 then raise (new System.InvalidOperationException())
                  else 
                    try 
                        raise (new System.InvalidOperationException())
                    with _ -> 
                        let! i = so6 (n-1)
                        return i
                }
        check "so-return6"
            (try
                so6 1000000 |> Async.RunSynchronously
                1
             with 
             | :? System.InvalidOperationException -> 42)
            42

        let cts = new System.Threading.CancellationTokenSource() in
        let rec so7 n =
            Async.TryCancelled(
                async {
                    if n = 0 then 
                        cts.Cancel()
                        return ()
                    else
                        do! so7 (n-1)
                },
                fun cexn -> ()
            ) in
        check "so-return7" 
            (try
                Async.RunSynchronously(so7 100000,cancellationToken=cts.Token)
                "fail"
             with
             |  :? System.OperationCanceledException -> "success")
            "success"

        let rec so8 n = 
          async { if n = 0 then return (new System.InvalidOperationException() |> raise)
                  else 
                    try 
                        return! so8 (n-1)
                    with _ -> 
                        raise (new System.InvalidOperationException())
                        return 1  }
        check "so-return8"
            (try
                so8 2000000 |> Async.RunSynchronously            
             with
             | :? System.InvalidOperationException -> 56
             )
            56

        let cts1 = new System.Threading.CancellationTokenSource() in
        let rec so9 n =
            Async.TryCancelled(
                async {
                    if n = 0 then 
                        cts1.Cancel()
                        return ()
                    else
                        do! so9 (n-1)
                },
                fun cexn -> raise (new System.InvalidOperationException())
            ) in
        check "so-return9" 
            (try
                Async.RunSynchronously(so9 2000000,cancellationToken=cts1.Token)
                "fail"
             with
             |  :? System.OperationCanceledException -> "success")
            "success"

(*******************************************************************************)
let RunAll() = 
    StackDiveTests.test()
    ReturnStackoverflow.test()

#if TESTS_AS_APP
let RUN() = RunAll(); failures
#else
RunAll()
let aa =
  if not failures.IsEmpty then 
      stdout.WriteLine "Test Failed"
      exit 1
  else   
      stdout.WriteLine "Test Passed"
      log "ALL OK, HAPPY HOLIDAYS, MERRY CHRISTMAS!"
      printf "TEST PASSED OK" ;
      exit 0
#endif
