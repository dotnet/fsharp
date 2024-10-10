// #Conformance #ComputationExpressions #Async 
#if TESTS_AS_APP
module Core_controlWebExt
#endif
#light

let failuresFile =
   let f = System.Environment.GetEnvironmentVariable("CONTROL_FAILURES_LOG")
   match f with
   | "" | null -> "failures.log"
   | _ -> f

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

System.AppDomain.CurrentDomain.UnhandledException.AddHandler(
       fun _ (args:System.UnhandledExceptionEventArgs) ->
          lock syncObj (fun () ->
                failures <- (args.ExceptionObject :?> System.Exception).ToString() :: failures
             )
)

let test s b = stderr.Write(s:string);  if b then stderr.WriteLine " OK" else report_failure s 
let check s x1 x2 = 
    if x1 = x2 then test s true
    else (test s false; printfn "expected: %A, got %A" x2 x1)

open System.Threading
open Microsoft.FSharp.Control
open Microsoft.FSharp.Control.WebExtensions

module WebResponseTests = 
    open System
    open System.IO
    open System.Net
    open Microsoft.FSharp.Control.WebExtensions

    /// Fetch the contents of a web page
    let httpAsync(url: string) = 
        async { 
            let req    = System.Net.WebRequest.Create(url) 
            use! resp   = req.AsyncGetResponse()         // note 'use' = C# 'using'
            use stream = resp.GetResponseStream() 
            use reader = new StreamReader(stream) 
            let html   = reader.ReadToEnd()
            return html
        }


    //This test starts ~600 web requests plus routines to cancel them. The tests are robust 
    //to whether there is a web connection or not � we  just catch all exceptions, and use one good URL 
    //and one bad URL, so we know we work under both conditions. The aim of the tests is to get coverage 
    //for cancellation logic paths � we count the number of request routines that have successfully entered 
    //a try/finally that increments/decrements a counter. We check that the counter is zero at the end.  
    let repeatedFetchAndCancelTest() = 
        printfn "starting requests and cancellation routines..." 
        let active = ref 0
        for i = 0 to 100 do 
          for sleep in [0;2;10] do 
           for url in ["http://www.live.com"; "http://www.badnonexistenturl-da-da-da-da-da.com"] do 
             let cts = new CancellationTokenSource()
             let result = ref 0
             
             Async.Start (async { let incremented = ref false
                               try 
                                 try 
                                   lock active (fun () -> incr active; incremented := true)
                                   let! p1 = httpAsync(url)
                                   let! p2 = httpAsync(url)
                                   result := p1.Length + p2.Length 
                                 finally
                                   lock active (fun () -> if !incremented then decr active) 
                               with _ -> 
                                  return () }, cancellationToken=cts.Token)
             System.Threading.Thread.Sleep(sleep)
             Async.Start ( async {  do! Async.Sleep(100)
                                    cts.Cancel() })
        let wait = ref 0 
        while !active <> 0 && !wait < 500 do
            incr wait
            printfn "final: active = %d, waiting %d" !active (!wait * 3)
            System.Threading.Thread.Sleep(!wait * 3)
        
        check "WebRequest cancellation test final result" !active 0



module WebClientTests = 

    //This test starts ~600 web requests plus routines to cancel them. The tests are robust 
    //to whether there is a web connection or not � we  just catch all exceptions, and use one good URL 
    //and one bad URL, so we know we work under both conditions. The aim of the tests is to get coverage 
    //for cancellation logic paths � we count the number of request routines that have successfully entered 
    //a try/finally that increments/decrements a counter. We check that the counter is zero at the end.  
    let repeatedFetchStringAndCancelTest() = 
        printfn "starting requests and cancellation routines..." 
        let active = ref 0
        for i = 0 to 100 do 
          for sleep in [0;2;10] do 
           for url in ["http://www.live.com"; "http://www.badnonexistenturl-da-da-da-da-da.com"] do 
             let cts = new CancellationTokenSource()
             let result = ref 0
             
             Async.Start (async { let incremented = ref false
                               try 
                                 try 
                                   lock active (fun () -> incr active; incremented := true)
                                   let c = new System.Net.WebClient()
                                   let! p1 = c.AsyncDownloadString(System.Uri(url))
                                   let! p2 = c.AsyncDownloadString(System.Uri(url))
                                   result := p1.Length + p2.Length 
                                 finally
                                   lock active (fun () -> if !incremented then decr active) 
                               with _ -> 
                                   return ()}, cancellationToken=cts.Token)

             System.Threading.Thread.Sleep(sleep)
             Async.Start ( async {  do! Async.Sleep(100)
                                    cts.Cancel() })
        let wait = ref 0 
        while !active <> 0 && !wait < 500 do
            incr wait
            printfn "final: active = %d, waiting %d" !active (!wait * 3)
            System.Threading.Thread.Sleep(!wait * 3)

        printfn "final: active = %d" !active
        check "WebClient.AsyncDownloadString cancellation test final result" !active 0

    //This test starts ~600 web requests plus routines to cancel them. The tests are robust 
    //to whether there is a web connection or not � we  just catch all exceptions, and use one good URL 
    //and one bad URL, so we know we work under both conditions. The aim of the tests is to get coverage 
    //for cancellation logic paths � we count the number of request routines that have successfully entered 
    //a try/finally that increments/decrements a counter. We check that the counter is zero at the end.  
    let repeatedFetchDataAndCancelTest() = 
        printfn "starting requests and cancellation routines..." 
        let active = ref 0
        for i = 0 to 100 do 
          for sleep in [0;2;10] do 
           for url in ["http://www.live.com"; "http://www.badnonexistenturl-da-da-da-da-da.com"] do 
             let cts = new CancellationTokenSource()
             let result = ref 0
             
             Async.Start (async { let incremented = ref false
                               try 
                                 try 
                                   lock active (fun () -> incr active; incremented := true)
                                   let c = new System.Net.WebClient()
                                   let! p1 = c.AsyncDownloadData(System.Uri(url))
                                   let! p2 = c.AsyncDownloadData(System.Uri(url))
                                   result := p1.Length + p2.Length
                                 finally
                                   lock active (fun () -> if !incremented then decr active) 
                               with _ -> 
                                   return ()}, cancellationToken=cts.Token)

             System.Threading.Thread.Sleep(sleep)
             Async.Start ( async {  do! Async.Sleep(100)
                                    cts.Cancel() })
        let wait = ref 0 
        while !active <> 0 && !wait < 500 do
            incr wait
            printfn "final: active = %d, waiting %d" !active (!wait * 3)
            System.Threading.Thread.Sleep(!wait * 3)

        printfn "final: active = %d" !active
        check "WebClient.AsyncDownloadData cancellation test final result" !active 0

    //This test starts ~600 web requests plus routines to cancel them. The tests are robust 
    //to whether there is a web connection or not � we  just catch all exceptions, and use one good URL 
    //and one bad URL, so we know we work under both conditions. The aim of the tests is to get coverage 
    //for cancellation logic paths � we count the number of request routines that have successfully entered 
    //a try/finally that increments/decrements a counter. We check that the counter is zero at the end.  
    let repeatedFetchFileAndCancelTest() = 
        printfn "starting requests and cancellation routines..." 
        let active = ref 0
        for i = 0 to 100 do 
          for sleep in [0;2;10] do 
           for url in ["http://www.live.com"; "http://www.badnonexistenturl-da-da-da-da-da.com"] do 
             let cts = new CancellationTokenSource()

             Async.Start (async { let incremented = ref false
                               try
                                 let p1 = sprintf "%i_%i_p1.data" i sleep
                                 let p2 = sprintf "%i_%i_p2.data" i sleep
                                 try 
                                   lock active (fun () -> incr active; incremented := true)
                                   let c = new System.Net.WebClient()
                                   do! c.AsyncDownloadFile(System.Uri(url), p1)
                                   do! c.AsyncDownloadFile(System.Uri(url), p2)
                                 finally
                                   lock active (fun () -> if !incremented then decr active)
                                   System.IO.File.Delete(p1)
                                   System.IO.File.Delete(p2)
                               with _ -> 
                                   return ()}, cancellationToken=cts.Token)

             System.Threading.Thread.Sleep(sleep)
             Async.Start ( async {  do! Async.Sleep(100)
                                    cts.Cancel() })
        let wait = ref 0 
        while !active <> 0 && !wait < 500 do
            incr wait
            printfn "final: active = %d, waiting %d" !active (!wait * 3)
            System.Threading.Thread.Sleep(!wait * 3)

        printfn "final: active = %d" !active
        check "WebClient.AsyncDownloadFile cancellation test final result" !active 0

    repeatedFetchStringAndCancelTest()
    repeatedFetchDataAndCancelTest()
    repeatedFetchFileAndCancelTest()

let RunAll() = 
    WebClientTests.repeatedFetchAndCancelTest()
    WebResponseTests.repeatedFetchAndCancelTest()

#if TESTS_AS_APP
let RUN() = RunAll(); failures
#else
RunAll()

let aa = 
  if not failures.IsEmpty then (stdout.WriteLine("Test Failed, failures = {0}", failures); exit 1) 
  else (stdout.WriteLine "Test Passed"; 
        log "ALL OK, HAPPY HOLIDAYS, MERRY CHRISTMAS!"
        printf "TEST PASSED OK"; 
// debug: why is the fsi test failing?  is it because test.ok does not exist?
        if System.IO.File.Exists("test.ok") then
            stdout.WriteLine ("test.ok found at {0}", System.IO.FileInfo("test.ok").FullName)
        else
            stdout.WriteLine ("test.ok not found")
        exit 0)

#endif
