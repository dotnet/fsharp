// #Regression #Conformance #ComputationExpressions #Async #Regression #Events #Stress
#if TESTS_AS_APP
module Core_control
#endif
#light

#if NETCOREAPP
open System.Threading.Tasks
#endif

#nowarn "40" // recursive references

let biggerThanTrampoliningLimit = 10000

let failuresFile =
   let f = 
        System.Environment.GetEnvironmentVariable("CONTROL_FAILURES_LOG")
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

#if !NETCOREAPP
System.AppDomain.CurrentDomain.UnhandledException.AddHandler(
       fun _ (args:System.UnhandledExceptionEventArgs) ->
          lock syncObj (fun () ->
                let e = args.ExceptionObject :?> System.Exception
                printfn "Exception: %s at %s" (e.ToString()) e.StackTrace
                failures <- (args.ExceptionObject :?> System.Exception).ToString() :: failures
             )
)
#endif

let test s b = stderr.Write(s:string);  if b then stderr.WriteLine " OK" else report_failure s 

let checkQuiet s x1 x2 = 
    if x1 <> x2 then 
        (test s false; 
         log (sprintf "expected: %A, got %A" x2 x1))

let check s x1 x2 = 
    if x1 = x2 then test s true
    else (test s false; log (sprintf "expected: %A, got %A" x2 x1))

open Microsoft.FSharp.Control
open Microsoft.FSharp.Control.WebExtensions

let boxed(a:Async<'b>) : Async<obj> = async { let! res = a in return box res }

type Microsoft.FSharp.Control.Async with 
        static member Parallel2 (a:Async<'T1>,b:Async<'T2>) =
            async { let! res = Async.Parallel [boxed a; boxed b]
                    return (unbox<'T1>(res.[0]), unbox<'T2>(res.[1])) }

        static member Parallel3 (a:Async<'T1>,b:Async<'T2>,c:Async<'T3>) =
            async { let! res = Async.Parallel [boxed a; boxed b; boxed c]
                    return (unbox<'T1>(res.[0]), unbox<'T2>(res.[1]), unbox<'T3>(res.[2])) }

        static member Generate (n,f, ?numChunks) : Async<'T array> =
            async { let procs = defaultArg numChunks System.Environment.ProcessorCount
                    let resArray = Array.zeroCreate n
                    let! res = Async.Parallel
                                  [ for pid in 0 .. procs-1 ->
                                        async { for i in 0 .. (n/procs+(if n%procs > pid then 1 else 0)) - 1 do
                                                    let elem = (i + pid * (n/procs) + min (n%procs) pid)
                                                    let! res = f elem
                                                    do resArray.[elem] <- res;  } ]
                    return resArray }

module BasicTests = 
  let Run() =
    check "cew23242g" (Async.RunSynchronously (async { do () })) ()
    check "32o8f43k1" (Async.RunSynchronously (async { return () })) ()
    check "32o8f43k2" (Async.RunSynchronously (async { return 1 })) 1

    check "32o8f43k3" (Async.RunSynchronously (async { return 1 })) 1

    check "32o8f43k9" (Async.RunSynchronously (async { do! Async.Sleep(30) 
                                                       return 1 })) 1

    check "32o8f43kq" (Async.RunSynchronously (async { let x = 1 in return x })) 1

    check "32o8f43kw" (try Async.RunSynchronously (async {  let x = failwith "error" in return x+1 }) with _ -> 2) 2

    check "32o8f43kr" (try Async.RunSynchronously (async {  let x = failwith "error" 
                                                            return x+1 }) with _ -> 2) 2

    //check "32o8f43kt" (Async.RunSynchronously (Async.Catch (async {  do failwith "error" }))) (Choice2Of2 (Failure "error"))
    check "32o8f43kta" (Async.RunSynchronously (Async.Catch (async {  return 1 }))) (Choice1Of2 1)

    check "32o8f43ktb" (Async.RunSynchronously (async { try 
                                                            do failwith "error" 
                                                            return 3
                                                        with _ -> 
                                                            return 2 
                                                      })) 2

    check "32o8f43ktc" 
        (Async.RunSynchronously
             (async {  try 
                          do failwith "error" 
                          return 3
                       with Failure _ -> 
                          return 2 
                    })) 
        2

    check "32o8f43ktd" 
        (Async.RunSynchronously
            (async {  try 
                         try 
                            do failwith "error" 
                            return 3
                         with :? System.ArgumentNullException -> 
                            return 4 
                       with Failure _ -> 
                          return 2 
                    })) 
        2

    check "32o8f43kt" (let mutable x = 0 
                       Async.RunSynchronously 
                           (async {  try 
                                        return ()
                                     finally 
                                        x <- 10
                                  });
                       x) 10

    check "32o8f43kt" (let mutable x = 0 
                       (try 
                           Async.RunSynchronously 
                               (async {  try 
                                            do failwith ""
                                         finally 
                                            x <- 10
                                      })
                        with Failure _ -> ());
                       x) 10


    check "32o8f43kt" (let mutable x = 0 
                       (try 
                           Async.RunSynchronously 
                               (async {  try 
                                           try 
                                              do failwith ""
                                           finally 
                                              x <- x + 1
                                         finally 
                                            x <- x + 1
                                      })
                        with Failure _ -> ());
                       x) 2

    check "32o8f43kt" (let mutable x = 0 
                       (try 
                           Async.RunSynchronously 
                               (async {  try 
                                           try 
                                              return ()
                                           finally 
                                              do failwith ""
                                              x <- x + 1
                                         finally 
                                            x <- x + 1
                                      })
                        with Failure _ -> ());
                       x) 1

    check "32o8f43ky" (try Async.RunSynchronously
                             (async {  try 
                                         try 
                                           do failwith "error" 
                                           return 3
                                         with _ -> 
                                           do failwith "error" 
                                           return 4
                                       with _ -> 
                                         return 2 
                                    }) with _ -> 6) 2

    check "32o8f43ku" (try Async.RunSynchronously 
                               (async {  let x = failwith "error" 
                                         do! Async.Sleep(30) 
                                         return x+1 }) with _ -> 2) 2

    check "32o8f43ki" (let p = async {  let x = failwith "error" 
                                        return x+1 } 
                       try Async.RunSynchronously p with _ -> 2) 2
                       
                        
    check "32o8f43ko" (Async.RunSynchronously
                          (Async.Parallel [| async { let x = 10+10 in return x+x }; 
                                             async { let y = 20+20 in return y+y } |])) [| 40; 80 |]
      
    check "46sdhksdjf" 
        begin
            for i in 1..1000 do
                begin
                    Async.TryCancelled (async { while true do do! Async.Sleep(10) }, fun _ -> failwith "fail!") |> Async.Start
                    Async.CancelDefaultToken()
                end
            true                    
        end
        true



module JoinTests = 
    let Join (a1: Async<'a>) (a2: Async<'b>) = async {
      let! task1 = a1 |> Async.StartChild
      let! task2 = a2 |> Async.StartChild
      
      let! res1 = task1
      let! res2 = task2 
      return (res1,res2) }

    let JoinNoFailTest() = 
        check "cvew0-9rn1" 
             (Async.RunSynchronously (Join (async { return 1 + 1 }) 
                                           (async { return 2 + 2 } ))) 
             (2,4)
        check "cvew0-9rn2" 
             (Async.RunSynchronously (Join (async { do! Async.Sleep(30) 
                                                    return 1 + 1 }) 
                                           (async { do! Async.Sleep(30) 
                                                    return 2 + 2 } ))) 
             (2,4)

    let JoinFirstFailTest() = 
        check "cvew0-9rn3" 
             (try 
                 Async.RunSynchronously (Join (async { do! Async.Sleep(30) 
                                                       failwith "fail"
                                                       return 3+3 }) 
                                              (async { do! Async.Sleep(30) 
                                                       return 2 + 2 } ))
              with _ -> 
                 (0,0))                                               
             (0,0)

    let JoinSecondFailTest() = 
        check "cvew0-9rn4" 
             (try 
                 Async.RunSynchronously (Join (async { do! Async.Sleep(30) 
                                                       return 3+3 }) 
                                              (async { do! Async.Sleep(30) 
                                                       failwith "fail"
                                                       return 2 + 2 } ))
              with _ -> 
                 (0,0))                                               
             (0,0)


    let JoinBothFailTest() = 
        check "cvew0-9rn5d" 
             (try 
                 Async.RunSynchronously (Join (async { do! Async.Sleep(30) 
                                                       failwith "fail"
                                                       return 3+3 }) 
                                              (async { do! Async.Sleep(30) 
                                                       failwith "fail"
                                                       return 2 + 2 } ))
              with _ -> 
                 (0,0))                                               
             (0,0)

    JoinNoFailTest()
    JoinFirstFailTest()
    JoinSecondFailTest()
    JoinBothFailTest()

module StartChildWaitMultipleTimes =
    let a = async {
                let! a = Async.StartChild(
                            async { 
                                do! Async.Sleep(500) 
                                return 27 
                            })
                let! result  = Async.Parallel [ a; a; a; a ]
                return result
            }
    check "dfhdu34y734"
        (a |> Async.RunSynchronously)
        [| 27; 27; 27; 27 |]

module StartChildTrampoliningCheck =
    let a = async {
                let! a = Async.StartChild(
                            async { 
                                do! Async.Sleep(500) 
                                return 27 
                            })
                let! result = a
                let mutable x = result
                for i in 1..biggerThanTrampoliningLimit do
                    x <- x + 1
                return x
            }
    check "ft6we56sgfw"
        (a |> Async.RunSynchronously)
        (biggerThanTrampoliningLimit+27)


module StartChildOutsideOfAsync =
  open System.Threading
  let Run() =

    check "dshfukeryhu8we"
        (let b = async {return 27} |> Async.StartChild
        (b |> Async.RunSynchronously |> Async.RunSynchronously)
        )
        27

    check "gf6dhasdfmkerio57: StartChild cancellation"
        begin
            use ev0 = new ManualResetEvent(false)
            use ev = new ManualResetEvent(false)
            let cts = new CancellationTokenSource()
            let child = async {
                            try
                                ev0.Set() |> ignore
                                while true do
                                    ()
                            finally 
                                ev.Set() |> ignore
                        } |> Async.StartChild
            Async.RunSynchronously(child,cancellationToken=cts.Token) |> ignore
            ev0.WaitOne() |> ignore// wait until cancellation handler is set
            cts.Cancel()
            ev.WaitOne(5000)
        end
        true
   

module SpawnTests = 
   let Run() = 
    check "32o8f43kaI: Spawn" 
        (let mutable result = 0
         Async.Start(async { do printfn "hello 1"
                             do! Async.Sleep(30) 
                             do result <- 1 });
         while result = 0 do 
             printf "."
#if NETCOREAPP
             Task.Delay(10).Wait()
#else
             System.Threading.Thread.Sleep(10)
#endif
         result) 1


#if !NETCOREAPP
module FromBeginEndTests = 
    // FromBeginEnd 
    let FromBeginEndTest() = 
        for completeSynchronously in [true;false] do
         for sleep in [0;50;100] do
          for expectedResult in [300;600] do
           for useCancelAction in [true;false] do
            for argCount in [0;1;2;3] do
                let name = sprintf "cvew0-9rn5a, completeSynchronously = %A, sleep = %A, expectedResult = %A,useCancelAction = %A, argCount = %A" completeSynchronously sleep expectedResult useCancelAction argCount
                printfn "running %s" name
                check 
                     name
                     (try 
                         Async.RunSynchronously 
                             (async { let mutable completed = completeSynchronously
                                      let mutable result = 0
                                      let ev = new System.Threading.ManualResetEvent(completeSynchronously)
                                      let iar = 
                                          { new System.IAsyncResult with
                                                member x.IsCompleted = completed
                                                member x.CompletedSynchronously = completeSynchronously
                                                member x.AsyncWaitHandle = ev :> System.Threading.WaitHandle
                                                member x.AsyncState = null }
                                      let savedCallback = ref (None : System.AsyncCallback option)
                                      let complete(r) = 
                                          result <- r
                                          if completeSynchronously then 
                                              completed <- true 
                                              if (!savedCallback).IsNone then failwith "expected a callback (loc cwowen903)"
                                              (!savedCallback).Value.Invoke iar
                                          else 
#if NETCOREAPP
                                              Task.Run(fun _ -> 
                                                   Task.Delay(sleep).Wait()
#else
                                              System.Threading.ThreadPool.QueueUserWorkItem(fun _ -> 
                                                   System.Threading.Thread.Sleep sleep
#endif
                                                   completed <- true 
                                                   ev.Set() |> ignore
                                                   if (!savedCallback).IsNone then failwith "expected a callback (loc cwowen903)"
                                                   (!savedCallback).Value.Invoke iar) |> ignore
                                      let beginAction0(callback:System.AsyncCallback,state) = 
                                          savedCallback := Some callback
                                          complete(expectedResult)
                                          iar
                                      let beginAction1(x:int,callback:System.AsyncCallback,state) = 
                                          savedCallback := Some callback
                                          complete(expectedResult+x)
                                          iar
                                      let beginAction2(x1:int,x2:int,callback:System.AsyncCallback,state) = 
                                          savedCallback := Some callback
                                          complete(expectedResult+x1+x2)
                                          iar
                                      let beginAction3(x1:int,x2:int,x3:int,callback:System.AsyncCallback,state) = 
                                          savedCallback := Some callback
                                          complete(expectedResult+x1+x2+x3)
                                          iar
                                      let endAction(iar:System.IAsyncResult) = result
                                      let cancelAction = 
                                          if useCancelAction then 
                                              Some(fun () ->  complete(expectedResult))
                                          else
                                              None
                                          
                                      let! res = 
                                         match argCount with 
                                         | 0 -> Async.FromBeginEnd(beginAction0,endAction,?cancelAction=cancelAction)
                                         | 1 -> Async.FromBeginEnd(0,beginAction1,endAction,?cancelAction=cancelAction)
                                         | 2 -> Async.FromBeginEnd(7,-7,beginAction2,endAction,?cancelAction=cancelAction)
                                         | 3 -> Async.FromBeginEnd(7,-3,-4,beginAction3,endAction,?cancelAction=cancelAction)
                                         | _ -> failwith "bad argCount"
                                      return res })
                      with _ -> 
                         4)                                               
                     expectedResult

    FromBeginEndTest()
#endif
module Bug6078 =
    open System
    
    let Test() =
        let beginAction, endAction, _ =  Async.AsBeginEnd(fun () -> Async.Sleep(500))
        let sleepingAsync = Async.FromBeginEnd((fun (a,b) -> beginAction((),a,b)),endAction)
        let throwingAsync = async { raise <| new InvalidOperationException("foo") }
        
        for i in 1..100 do
            check ("5678w6r78w"+string i) 
                begin
                    try
                        [   for j in 1..i do yield sleepingAsync; 
                            yield throwingAsync;
                            for j in i..1000 do yield sleepingAsync; ] |> Async.Parallel |> Async.RunSynchronously |> ignore
                        "weird"
                    with
                    |    :? InvalidOperationException as e -> e.Message
                end
                "foo"
    Test()

#if !MONO && !NETCOREAPP
module AwaitEventTests = 
    let AwaitEventTest() = 
        // AwaitEvent
        for completeSynchronously in [ (* true; *) false] do
         for sleep in [0;50;100] do
          for expectedResult in [300;600] do
           for useCancelAction in [true;false] do
                let name = sprintf "cvew0-9rn5b, completeSynchronously = %A, sleep = %A, expectedResult = %A,useCancelAction = %A" completeSynchronously sleep expectedResult useCancelAction 
                printfn "running %s" name
                
                check 
                     name
                     (try 
                         Async.RunSynchronously 
                             (async { let ev = new Event<_>()
                                      let mutable over = false
                                      let complete(r) = 
                                          if completeSynchronously then 
                                              ev.Trigger(r)
                                          else 
#if NETCOREAPP
                                              Task.Run(fun _ -> 
                                                   Task.Delay(sleep).Wait()
#else
                                              System.Threading.ThreadPool.QueueUserWorkItem(fun _ -> 
                                                   System.Threading.Thread.Sleep sleep
#endif
                                                   ev.Trigger(r)) |> ignore
                                      
                                      let cancelAction = 
                                          if useCancelAction then 
                                              Some(fun () ->  ev.Trigger(expectedResult))
                                          else
                                              None
                                      // The completion must come after the event is triggered
                                      let! child = 
                                           async { do! Async.Sleep 200;  // THIS TIME MUST BE LONG ENOUGH FOR THE EVENT HANDLER TO WIRE UP
                                                   complete(expectedResult)   }
                                           |> Async.StartChild
                                      let! res = Async.AwaitEvent(ev.Publish)
                                      for i in 1..biggerThanTrampoliningLimit do ()
                                      over <- true 
                                      return res })
                      with e -> 
                         printfn "ERROR: %A" e
                         4)                                               
                     expectedResult


    //AwaitEventTest()

#endif

module AsBeginEndTests = 

    type AsyncRequest<'T>(p:Async<'T>) = 
        
        let beginAction,endAction,cancelAction = Async.AsBeginEnd (fun () -> p)
        member this.BeginAsync(callback,state:obj) = 
            beginAction((),callback,state)

        member this.EndAsync(iar: System.IAsyncResult)  = endAction(iar)

        member this.CancelAsync(iar)           = cancelAction(iar)
        
    type AsyncRequest1<'T>(p:int -> Async<'T>) = 
        
        let beginAction,endAction,cancelAction = Async.AsBeginEnd p
        member this.BeginAsync(arg1,callback,state:obj) = 
            beginAction(arg1,callback,state)

        member this.EndAsync(iar)  = endAction(iar)

        member this.CancelAsync(iar)           = cancelAction(iar)

    type AsyncRequest2<'T>(p:int -> string -> Async<'T>) = 
        
        let beginAction,endAction,cancelAction = Async.AsBeginEnd (fun (arg1,arg2) -> p arg1 arg2)
        member this.BeginAsync(arg1,arg2,callback,state:obj) = beginAction((arg1,arg2),callback,state)

        member this.EndAsync(iar)  = endAction(iar)

        member this.CancelAsync(iar)           = cancelAction(iar)

    type AsyncRequest3<'T>(p:int -> string -> int -> Async<'T>) = 
        
        let beginAction,endAction,cancelAction = Async.AsBeginEnd (fun (arg1,arg2,arg3) -> p arg1 arg2 arg3)
        member this.BeginAsync(arg1,arg2,arg3,callback,state:obj) = 
            beginAction((arg1,arg2,arg3),callback,state)

        member this.EndAsync(iar: System.IAsyncResult)  = 
            endAction(iar)

        member this.CancelAsync(iar)           = cancelAction(iar)




    let AsBeginEndTest() = 
        check
           (sprintf "cvew0-9rn1")
           (let req = AsyncRequest( async { return 2087 } ) 
            let iar = req.BeginAsync(null,null)
            iar.CompletedSynchronously)
           true


        check
           (sprintf "cvew0-9rn2")
           (let req = AsyncRequest( async { return 2087 } ) 
            let iar = req.BeginAsync(null,null)
            iar.IsCompleted)
           true

        check
           (sprintf "cvew0-9rn2-state")
           (let req = AsyncRequest( async { return 2087 } ) 
            let iar = req.BeginAsync(null,box 1)
            iar.AsyncState |> unbox<int>)
           1

        check
           (sprintf "cvew0-9rn3")
           (let req = AsyncRequest( async { do! Async.Sleep 100 } ) 
            let iar = req.BeginAsync(null,null)
            iar.IsCompleted)
           false

        check
           (sprintf "cvew0-9rn3-state")
           (let req = AsyncRequest( async { do! Async.Sleep 100 } ) 
            let iar = req.BeginAsync(null,box 1)
            iar.AsyncState |> unbox<int>)
           1

        check
           (sprintf "cvew0-9rn4")
           (let req = AsyncRequest( async { do! Async.Sleep 100 } ) 
            let iar = req.BeginAsync(null,null)
            iar.CompletedSynchronously)
           false

        check
           (sprintf "cvew0-9rn5c")
           (let req = AsyncRequest( async { return 2087 } ) 
            let iar = req.BeginAsync(null,null)
            req.EndAsync(iar))
           2087

        check
           (sprintf "cvew0-9rn5c-1")
           (let req = AsyncRequest1( fun i -> async { return i } ) 
            let iar = req.BeginAsync(2087, null,null)
            req.EndAsync(iar))
           2087

        check
           (sprintf "cvew0-9rn5c-1-state")
           (let req = AsyncRequest1( fun i -> async { return i } ) 
            let iar = req.BeginAsync(2087, null,box 1)
            iar.AsyncState |> unbox<int>)
           1

        check
           (sprintf "cvew0-9rn5c-2")
           (let req = AsyncRequest2( fun i j -> async { return i + int j } ) 
            let iar = req.BeginAsync(2087, "1", null,null)
            req.EndAsync(iar))
           2088


        check
           (sprintf "cvew0-9rn5c-2-state")
           (let req = AsyncRequest2( fun i j -> async { return i + int j } ) 
            let iar = req.BeginAsync(2087, "1", null,box 10)
            iar.AsyncState |> unbox<int>)
           10

        check
           (sprintf "cvew0-9rn5c-3")
           (let req = AsyncRequest3( fun i j k -> async { return i + int j + k} ) 
            let iar = req.BeginAsync(2087, "1", 2, null,null)
            req.EndAsync(iar))
           2090


        check
           (sprintf "cvew0-9rn5c-3-state")
           (let req = AsyncRequest3( fun i j k -> async { return i + int j + k} ) 
            let iar = req.BeginAsync(2087, "1", 17, null,box "10")
            iar.AsyncState |> unbox<string>)
           "10"

        check
           (sprintf "cvew0-9rn6")
           (let req = AsyncRequest( async { return 2087 } ) 
            let iar = req.BeginAsync(null,null)
            req.EndAsync(iar) |> ignore
            try req.EndAsync(iar) with :? System.ObjectDisposedException -> 100)
           100

        check
           (sprintf "cvew0-9rn7")
           (let req = AsyncRequest( async { return 2087 } ) 
            let iar = req.BeginAsync(null,null)
            iar.AsyncWaitHandle.WaitOne(100) |> ignore
            req.EndAsync(iar))
           2087

        check
           (sprintf "cvew0-9rn8")
           (let req = AsyncRequest( async { return 2087 } ) 
            let mutable called = 0
            let iar = req.BeginAsync(System.AsyncCallback(fun _ -> called <- 10),null)
            iar.AsyncWaitHandle.WaitOne(100) |> ignore
            let v = req.EndAsync(iar)
            v + called)
           2097

        check
           (sprintf "cvew0-9rn9")
           (let req = AsyncRequest( async { return 2087 } ) 
            let mutable called = 0
            let iar = req.BeginAsync(System.AsyncCallback(fun iar -> called <- req.EndAsync(iar)),null)
            while not iar.IsCompleted do
                 iar.AsyncWaitHandle.WaitOne(100) |> ignore
            
            called)
           2087



        check
           (sprintf "cvew0-9rnA")
           (let req = AsyncRequest( async { do! Async.SwitchToNewThread()
                                            while true do 
                                                do! Async.Sleep 10 
                                            return 10 } ) 
            let iar = req.BeginAsync(null,null)
            printfn "waiting"
            iar.AsyncWaitHandle.WaitOne(100) |> ignore
            printfn "cancelling"
            req.CancelAsync(iar)
            (try req.EndAsync(iar) with :? System.OperationCanceledException as e -> 100 ))
           100

    //AsBeginEndTest()

(*

check "32o8f43kaO: Cancel a While loop" 
    (let count = ref 0
     let res = ref 0
     let asyncGroup = new System.Threading.CancellationTokenSource()
     Async.Spawn(async { do! async { use! holder = Async.OnCancel (fun msg -> printfn "got cancellation...."; incr res) 
                                     while true do
                                         do! Async.Sleep(10) 
                                         do printfn "in loop..."
                                         do count := !count + 1 
                                     do printfn "exited loop..." } 
                         do printfn "exited use!..." },
                 asyncGroup=asyncGroup);
     while !count = 0 do 
         do printfn "waiting to enter loop..."
         Async.Sleep(10)
     
     asyncGroup.TriggerCancel "cancel"
     while !res= 0 do 
         do printfn "32o8f43kaP: waiting to for cancellation...."
         Async.Sleep(10)
     !res) 1


check "32o8f43ka2: Cancel a For loop" 
    (let count = ref 0
     let res = ref 0
     let asyncGroup = new System.Threading.CancellationTokenSource()
     Async.Spawn(async { do! async { use! holder = Async.OnCancel (fun msg -> printfn "got cancellation...."; incr res) 
                                     for x in Seq.initInfinite (fun i -> i) do
                                         do! Async.Sleep(10) 
                                         do printfn "in loop..."
                                         do count := !count + 1 
                                     do printfn "exited loop without cancellation!" }
                         do printfn "exited use without cancellation!" },
                 asyncGroup=asyncGroup);
     while !count = 0 do 
         do printfn "waiting to enter loop..."
         Async.Sleep(10)
     
     asyncGroup.TriggerCancel "cancel"
     while !res= 0 do 
         do printfn "32o8f43ka2: waiting to for cancellation...."
         Async.Sleep(10)
     !res) 1
*)

module OnCancelTests = 
  let Run() = 
    check "32o8f43ka1: No cancellation" 
        (let mutable count = 0
         let mutable res = 0
         let asyncGroup = new System.Threading.CancellationTokenSource ()
         Async.Start(async { use! holder = Async.OnCancel (fun msg -> printfn "got cancellation...."; res <- (res + 1)) 
                             do count <- (count + 1)
                             return () }, asyncGroup.Token);
         while count = 0 do 
             do printfn "waiting to enter cancellation section"
#if NETCOREAPP
             Task.Delay(10).Wait()
#else
             System.Threading.Thread.Sleep(10)
#endif
        
         res) 0


#if !TESTS_AS_APP && !NETCOREAPP
module SyncContextReturnTests = 

    let p() = printfn "running on %A" System.Threading.SynchronizationContext.Current

    let run p = Async.RunSynchronously p


    let rec fakeCtxt = { new System.Threading.SynchronizationContext() with 
                             member x.Post(work,state) = 
                                 //printfn "Posting..."


                                 System.Threading.ThreadPool.QueueUserWorkItem(System.Threading.WaitCallback(fun _ -> 
                                     //printfn "Setting..."
                                     System.Threading.SynchronizationContext.SetSynchronizationContext fakeCtxt
                                     work.Invoke(state))) |> ignore }

    let checkOn s expectedCtxt = 
        check s System.Threading.SynchronizationContext.Current expectedCtxt

    let setFakeContext() = 
        async { do! Async.SwitchToNewThread()
                let ctxt = System.Threading.SynchronizationContext.Current 
                System.Threading.SynchronizationContext.SetSynchronizationContext fakeCtxt
                return { new System.IDisposable with 
                             member x.Dispose() = 
                                 printfn "Disposing..."
                                 // Set the synchronization context back to its original value
                                 System.Threading.SynchronizationContext.SetSynchronizationContext ctxt }  }
            
    // CHeck that Async.Sleep returns to the sync context
    async { use! holder = setFakeContext()
            p()
            checkOn "evrher921" fakeCtxt
            p()
            do! Async.Sleep 100
            p()
            checkOn "evrher962" fakeCtxt }
       |> run

    // CHeck that Async.AwaitWaitHandle returns to the sync context
    async { use! holder = setFakeContext()
            checkOn "evrher923" fakeCtxt
            p()
            let wh = new System.Threading.ManualResetEvent(true)
            p()
            let! ok = Async.AwaitWaitHandle(wh,0)
            p()
            checkOn "evrher964" fakeCtxt }
       |> run

    // CHeck that Async.AwaitWaitHandle returns to the sync context
    async { use! holder = setFakeContext()
            checkOn "evrher925" fakeCtxt
            p()
            let wh = new System.Threading.ManualResetEvent(true)
            p()
            let! ok = Async.AwaitWaitHandle(wh,-1)
            p()
            checkOn "evrher966" fakeCtxt }
       |> run

    // CHeck that Async.AwaitWaitHandle returns to the sync context
    async { use! holder = setFakeContext()
            for timeout in [-1; 0; 100] do
                checkOn "evrher927" fakeCtxt
                p()
                let wh = new System.Threading.ManualResetEvent(true)
                p()
                let! ok = Async.AwaitWaitHandle(wh,timeout)
                p()
                checkOn "evrher968" fakeCtxt }
       |> run

    // CHeck that Async.AwaitWaitHandle returns to the sync context
    async { use! holder = setFakeContext()
            for timeout in [100] do
                checkOn "evrher927" fakeCtxt
                p()
                let wh = new System.Threading.ManualResetEvent(false)
                p()
                // this will timeout
                let! ok = Async.AwaitWaitHandle(wh,timeout)
                p()
                checkOn "evrher968" fakeCtxt }
       |> run


    // CHeck that Async.AwaitWaitHandle returns to the sync context
    async { use! holder = setFakeContext()
            for timeout in [1;10] do
                checkOn "evrher929" fakeCtxt
                let wh = new System.Threading.ManualResetEvent(false)
                let! ok = Async.AwaitWaitHandle(wh,timeout)
                checkOn "evrher96Q" fakeCtxt }
       |> run

    // CHeck that Async.AwaitEvent returns to the sync context
    async { use! holder = setFakeContext()
            for timeout in [1;10] do
                checkOn "evrher92W" fakeCtxt
                let ev = new Event<int>()
                // Trigger the event in 400ms
                async { do! Async.Sleep 400
                        ev.Trigger 10 } |> Async.Start

                let! args = Async.AwaitEvent(ev.Publish)
                checkOn "evrher96E" fakeCtxt }
       |> run


    // CHeck that Async.FromBeginEnd returns to the sync context
    async { use! holder = setFakeContext()
            for completeSynchronously in [true;false] do
             for sleep in [0;50;100] do
              for expectedResult in [300;600] do
               for useCancelAction in [true;false] do
                for argCount in [0;1;2;3] do
                    let name = sprintf "vwwegbwerben5a, completeSynchronously = %A, sleep = %A, expectedResult = %A,useCancelAction = %A, argCount = %A" completeSynchronously sleep expectedResult useCancelAction argCount
                    printfn "running %s" name
                    
                    // THIS IS ONE CHECK
                    checkOn name fakeCtxt
                    
                    let mutable completed = completeSynchronously
                    let mutable result = 0
                    let ev = new System.Threading.ManualResetEvent(completeSynchronously)
                    let iar = 
                        { new System.IAsyncResult with
                              member x.IsCompleted = completed
                              member x.CompletedSynchronously = completeSynchronously
                              member x.AsyncWaitHandle = ev :> System.Threading.WaitHandle
                              member x.AsyncState = null }
                    let savedCallback = ref (None : System.AsyncCallback option)
                    let complete(r) = 
                        result <- r
                        if completeSynchronously then 
                            completed <- true 
                            if (!savedCallback).IsNone then failwith "expected a callback (loc cwowen903)"
                            (!savedCallback).Value.Invoke iar
                        else 
                            System.Threading.ThreadPool.QueueUserWorkItem(fun _ -> 
                                 System.Threading.Thread.Sleep sleep
                                 completed <- true 
                                 ev.Set() |> ignore
                                 if (!savedCallback).IsNone then failwith "expected a callback (loc cwowen903)"
                                 (!savedCallback).Value.Invoke iar) |> ignore
                    let beginAction0(callback:System.AsyncCallback,state) = 
                        savedCallback := Some callback
                        complete(expectedResult)
                        iar
                    let beginAction1(x:int,callback:System.AsyncCallback,state) = 
                        savedCallback := Some callback
                        complete(expectedResult+x)
                        iar
                    let beginAction2(x1:int,x2:int,callback:System.AsyncCallback,state) = 
                        savedCallback := Some callback
                        complete(expectedResult+x1+x2)
                        iar
                    let beginAction3(x1:int,x2:int,x3:int,callback:System.AsyncCallback,state) = 
                        savedCallback := Some callback
                        complete(expectedResult+x1+x2+x3)
                        iar
                    let endAction(iar:System.IAsyncResult) = result
                    let cancelAction = 
                        if useCancelAction then 
                            Some(fun () ->  complete(expectedResult))
                        else
                            None

                    // THIS IS ONE CHECK
                    checkOn name fakeCtxt
                    
                        
                    let! res = 
                       match argCount with 
                       | 0 -> Async.FromBeginEnd(beginAction0,endAction,?cancelAction=cancelAction)
                       | 1 -> Async.FromBeginEnd(0,beginAction1,endAction,?cancelAction=cancelAction)
                       | 2 -> Async.FromBeginEnd(7,-7,beginAction2,endAction,?cancelAction=cancelAction)
                       | 3 -> Async.FromBeginEnd(7,-3,-4,beginAction3,endAction,?cancelAction=cancelAction)
                       | _ -> failwith "bad argCount"

                    // THIS IS ONE CHECK
                    checkOn name fakeCtxt }
       |> run

    // CHeck that Async.Parallel returns to the sync context
    async { use! holder = setFakeContext()
            for timeout in [1;10] do
                checkOn "evrher92R" fakeCtxt
                let ev = new Event<int>()

                let! args = Async.Parallel [ for i in 0 .. 10 -> async { return i * 2 } ]
                checkOn "evrher96T" fakeCtxt }
       |> run


    // CHeck that Async.Parallel returns to the sync context
    async { use! holder = setFakeContext()
            for timeout in [1;10] do
                checkOn "evrher92R" fakeCtxt
                let ev = new Event<int>()

                try 
                   let! args = Async.Parallel [ for i in 0 .. 10 -> async { failwith "" } ]
                   checkOn "evrher96T" fakeCtxt
                   return ()
                with _ -> 
                   checkOn "evrher96T" fakeCtxt }
       |> run
#endif

module GenerateTests = 
  let Run() = 
    for n = 0 to 20 do
        check (sprintf "32o8f43ka2: Async.Generate, n = %d" n)
            (Async.RunSynchronously (Async.Generate(n, (fun i -> async { return i })))) [| 0..n-1 |]
        
    for n = 0 to 20 do
        check (sprintf "32o8f43ka3: Async.Generate w/- Sleep, n = %d" n) 
            (Async.RunSynchronously (Async.Generate(n, (fun i -> async { do! Async.Sleep(1) 
                                                                         return i })))) [| 0..n-1 |]

    for n = 1 to 20 do
        check 
            (sprintf "32o8f43ka4: Async.Generate w/- last failure, n = %d" n)
            (try Async.RunSynchronously (Async.Generate(n, (fun i -> async { if i=n-1 then return failwith "last failure" else return i }))) 
             with Failure "last failure" -> [| 0xdeadbeef |])
            [| 0xdeadbeef |]

    for n = 1 to 20 do
        check 
            (sprintf "32o8f43ka5: Async.Generate w/- all failure, n = %d" n)
            (try Async.RunSynchronously (Async.Generate(n, (fun i -> async { return failwith "failure" }))) 
             with Failure "failure" -> [| 0xdeadbeef |])
            [| 0xdeadbeef |]

module ParallelTests = 
  let Run() = 
    for n = 1 to 20 do
        check 
            (sprintf "32o8f43ka6: Async.Parallel w/- last failure, n = %d" n)
            (try Async.RunSynchronously (Async.Parallel [ for i in 0 .. n-1 -> async { if i=n-1 then return failwith "last failure" else return i }]) 
             with Failure "last failure" -> [| 0xdeadbeef |])
            [| 0xdeadbeef |]


    for n = 1 to 20 do
        check 
            (sprintf "32o8f43ka7: Async.Parallel w/- last failure and Sleep, n = %d" n)
            (try Async.RunSynchronously (Async.Parallel [ for i in 0 .. n-1 -> async { do! Async.Sleep(1) 
                                                                                       if i=n-1 then return failwith "last failure" else return i }]) 
             with Failure "last failure" -> [| 0xdeadbeef |])
            [| 0xdeadbeef |]

    // This test checks that sub-processes are successfully cancelled
    for n = 1 to 20 do
        check 
            (sprintf "32o8f43ka8: Async.Parallel w/- last failure and force cancellation, n = %d" n)
            (try Async.RunSynchronously 
                     (Async.Parallel 
                         [ for i in 0 .. n-1 -> 
                            async { // The last process is the one that causes the failure. 
                                    do! Async.Sleep(1) 
                                    if i=n-1 then 
                                      return failwith "last failure" 
                                    else 
                                      // All the other processes just loop until they are cancelled
                                      // Note - this doesn't return - it must be cancelled
                                      while true do 
                                        do! Async.Sleep(1) 
                                      return 1 }]) 
             with Failure "last failure" -> [| 0xabbaabba |])
            // the expected result
            [| 0xabbaabba |]

    // This test checks that sub-processes are successfully cancelled, AND that we wait for all processes
    // to be cancelled and finish before we return the overall result
    for n = 2 to 20 do
        check 
            (sprintf "32o8f43ka9: Async.Parallel w/- last failure and cancellation with check that cleanup occurs, n = %d" n)
            (let cleanedUp = ref false
             try Async.RunSynchronously 
                    (Async.Parallel 
                        [ for i in 0 .. n-1 -> 
                            async { // The last process is the one that causes the failure. It waits 50ms to allow
                                    // at least one of the other processes to commence and enter its "use" region
                                    if i=n-1 then 
                                         do! Async.Sleep(50) 
                                         return failwith "last failure" 
                                    else
                                        // All the other processes just loop until they are cancelled
                                        // They record the fact that they were cancelled in a try/finally
                                        // compensation handler (via a 'use'). 
                                        //
                                        use r = { new System.IDisposable with 
                                                    member x.Dispose() = 
                                                       // This gets run when the cancel happens
                                                       // Sleep a bit to check we wait for the sleep after the cancel
#if NETCOREAPP
                                                       Task.Delay(10).Wait()
#else
                                                       System.Threading.Thread.Sleep(10)
#endif
                                                       cleanedUp := true } 
                                        // Note - this doesn't return - it must be cancelled
                                        while true do 
                                            do! Async.Sleep(1) 
                                        return 1 }]) 
             with Failure "last failure" -> [| (if !cleanedUp then 0xabbaabba else 0xdeaddead) |])
            // the expected result
            [| 0xabbaabba |]

    for n = 1 to 20 do
        check 
            (sprintf "32o8f43kaQ: Async.Parallel w/- all failure, n = %d" n)
            (try Async.RunSynchronously (Async.Parallel [ for i in 0 .. n-1 -> async { return failwith "failure" }]) 
             with Failure "failure" -> [| 0xdeadbeef |])
            [| 0xdeadbeef |]
            
    for n = 0 to 20 do
        check (sprintf "32o8f43kaW: Async.Parallel, n = %d" n) 
            (Async.RunSynchronously (Async.Parallel( [ for i in 0 .. n-1 -> async { return i } ]))) [| 0..n-1 |]
        
    for n = 0 to 20 do
        check (sprintf "32o8f43kaE: Async.Parallel with Sleep, n = %d" n) 
            (Async.RunSynchronously (Async.Parallel( [ for i in 0 .. n-1 -> async { do! Async.Sleep(1) 
                                                                                    return i } ]))) [| 0..n-1 |]
        
    check "328onic4: Async.Parallel2" (Async.RunSynchronously (Async.Parallel2(async { return 1 }, async { return 2 }))) (1,2)
    check "328onic4: Async.Parallel3" (Async.RunSynchronously (Async.Parallel3(async { return 1 }, async { return 2 }, async { return 3 }))) (1,2,3)

    for n = 0 to 20 do
        check (sprintf "32o8f43kaR: Async.Parallel with SwitchToNewThread, n = %d" n) 
            (Async.RunSynchronously 
                (Async.Parallel( [ for i in 0 .. n-1 -> async { do! Async.SwitchToNewThread()
                                                                return i } ]))) [| 0..n-1 |]
        
    for n = 0 to 20 do
        check (sprintf "32o8f43kaT: Async.Parallel with SwitchToThreadPool, n = %d" n) 
            (Async.RunSynchronously
                (Async.Parallel( [ for i in 0 .. n-1 -> async { do! Async.SwitchToThreadPool()
                                                                return i } ]))) [| 0..n-1 |]
 
    for n = 0 to 20 do
        check 
            (sprintf "32o8f43kaY: Async.Parallel with FromContinuations, n = %d" n) 
            (Async.RunSynchronously (Async.Parallel( [ for i in 0 .. n-1 -> Async.FromContinuations(fun (cont,econt,ccont) -> cont i) ])))
            [| 0..n-1 |]


#if !NETCOREAPP
module AsyncWaitOneTest1 = 
  let Run() = 
        
    check 
        "c32398u1: AsyncWaitOne"
        (let wh = new System.Threading.ManualResetEvent(true)
         Async.RunSynchronously (async { return! Async.AwaitWaitHandle(wh,millisecondsTimeout = -1) })) 
        true

    check 
        "c32398u2: AsyncWaitOne"
        (let wh = new System.Threading.ManualResetEvent(true)
         Async.RunSynchronously (async { return! Async.AwaitWaitHandle(wh,millisecondsTimeout = 0) })) 
        true

    check 
        "c32398u3: AsyncWaitOne"
        (let wh = new System.Threading.ManualResetEvent(false)
         Async.RunSynchronously (async { return! Async.AwaitWaitHandle(wh,millisecondsTimeout = 0) })) 
        false // we never set the event, so the result is false
        
    check 
        "c32398u4: AsyncWaitOne"
        (let wh = new System.Threading.ManualResetEvent(false)
         Async.RunSynchronously (async { return! Async.AwaitWaitHandle(wh,millisecondsTimeout = 10) })) 
        false // we never set the event, so the result is false

    check 
        "c32398u5: AsyncWaitOne"
        (let wh = new System.Threading.AutoResetEvent(true)
         Async.RunSynchronously (async { return! Async.AwaitWaitHandle(wh,millisecondsTimeout = -1) })) 
        true

    check 
        "c32398u6: AsyncWaitOne"
        (let wh = new System.Threading.AutoResetEvent(true)
         Async.RunSynchronously (async { return! Async.AwaitWaitHandle(wh,millisecondsTimeout = 10) })) 
        true

    check 
        "c32398u7: AsyncWaitOne"
        (let wh = new System.Threading.AutoResetEvent(true)
         Async.RunSynchronously (async { return! Async.AwaitWaitHandle(wh,millisecondsTimeout = 0) })) 
        true

    check 
        "c32398u8: AsyncWaitOne"
        (let wh = new System.Threading.ManualResetEvent(true)
         Async.RunSynchronously
             (async { let! ok1 = Async.AwaitWaitHandle(wh,millisecondsTimeout = -1) 
                      // check the event is still set (it's a ManualResetEvent)
                      let! ok2 = Async.AwaitWaitHandle(wh,millisecondsTimeout = -1) 
                      return ok1 && ok2 })) 
        true

    check 
        "c32398u9: AsyncWaitOne"
        (let wh = new System.Threading.ManualResetEvent(false)
         Async.RunSynchronously 
             (async { let! _ = Async.StartChild (async { 
                                                        System.Threading.Thread.Sleep(100)
                                                        let _ = wh.Set() in () })
                      let! ok1 = Async.AwaitWaitHandle(wh,millisecondsTimeout = -1) 
                      // check the event is still set (it's a ManualResetEvent)
                      let! ok2 = Async.AwaitWaitHandle(wh,millisecondsTimeout = -1) 
                      return ok1 && ok2 })) 
        true

    check 
        "c32398u10: AsyncWaitOne"
        (let wh = new System.Threading.ManualResetEvent(false)
         Async.RunSynchronously 
             (async { let! _ = Async.StartChild (async { let _ = wh.Set() in () })
                      // 1000 milliseconds should be enough to get the result
                      let! ok1 = Async.AwaitWaitHandle(wh,millisecondsTimeout = 1000) 
                      // check the event is still set (it's a ManualResetEvent)
                      let! ok2 = Async.AwaitWaitHandle(wh,millisecondsTimeout = 0) 
                      return ok1 && ok2 })) 
        true

    check 
        "c32398u11: AsyncWaitOne"
        (let wh = new System.Threading.ManualResetEvent(false)
         Async.RunSynchronously 
             (async { let! _ = Async.StartChild (async {  System.Threading.Thread.Sleep(100); let _ = wh.Set() in () })
                      // no timeout = infinite
                      let! ok1 = Async.AwaitWaitHandle(wh) 
                      // check the event is still set (it's a ManualResetEvent)
                      let! ok2 = Async.AwaitWaitHandle(wh,millisecondsTimeout = 0) 
                      return ok1 && ok2 })) 
        true

    check 
        "c32398u12: AsyncWaitOne"
        (let wh = new System.Threading.AutoResetEvent(false)
         Async.RunSynchronously 
             (async { let! _ = Async.StartChild (async {  System.Threading.Thread.Sleep(100); let _ = wh.Set() in () })
                      // no timeout = infinite
                      let! ok1 = Async.AwaitWaitHandle(wh) 
                      // check the event is not still set (it's an AutoResetEvent)
                      let! ok2 = Async.AwaitWaitHandle(wh,millisecondsTimeout = 0) 
                      return ok1 && not ok2 })) 
        true

(*
let s = new System.IO.MemoryStream(1000)
let buffer = Array.create<byte> 1000 32uy

s.Read(buffer,0,10)
Async.RunSynchronously (async { let! n = s.AsyncRead(buffer,0,10) in return n })

s.Write(buffer,0,10)
Async.RunSynchronously (async { let! n = s.AsyncRead(buffer,0,10) in return n })
s.Length
s.ReadByte()
s.CanRead
s.CanWrite
s.GetBuffer()
s.Seek(0L,System.IO.SeekOrigin.Begin)
Async.RunSynchronously (async { let! n = s.AsyncRead(buffer,0,9) in return n }) = 9
Async.RunSynchronously (async { let! n = s.AsyncRead(buffer,0,9) in return n }) = 1
*)
#endif

#if !NETCOREAPP
module AsyncGenerateTests = 
  let Run() = 
    for length in 1 .. 10 do
        for chunks in 1 .. 10 do 
            check (sprintf "c3239438u: Run/Generate, length=%d, chunks=%d" length chunks)
                (Async.RunSynchronously(Async.Generate(length, (fun i -> async {return i}), chunks)))
                [| 0 .. length-1|];;


    for length in 0 .. 10 do
        for chunks in 1 .. 10 do 
            check (sprintf "c3239438v: Run/Generate, length=%d, chunks=%d" length chunks)
                (try Async.RunSynchronously
                         (Async.Generate(length, (fun i -> async { do System.Threading.Thread.Sleep(chunks)
                                                                   return i}), chunks), timeout=length) 
                 with :? System.TimeoutException -> [| 0 .. length-1|])
                [| 0 .. length-1|];;

    for length in 1 .. 10 do
        for chunks in 1 .. 10 do 
            let action i : Async<int> = 
                async { 
                    do System.Threading.Thread.Sleep(chunks)
                    return i
                } 
            let a = async { 
                let! child = Async.Generate(length, action, chunks) |> Async.StartChild
                return! child
            } 
            check (sprintf "c3239438w: Run/StartChild/Generate, length=%d, chunks=%d" length chunks)
                (a |>Async.RunSynchronously)
                [| 0 .. length-1|];;


    for length in 0 .. 10 do
        for chunks in 1 .. 10 do 
            let action i : Async<int> = 
                async { 
                    do System.Threading.Thread.Sleep(chunks)
                    return i
                } 
            let a = 
                async { 
                    try
                      let! result = Async.StartChild(
                                                Async.Generate(length, action, chunks), 
                                                millisecondsTimeout=length)
                      return! result
                    with :? System.TimeoutException -> return [| 0 .. length-1|]
                }
            check (sprintf "c3239438x: Run/StartChild/Generate, length=%d, chunks=%d" length chunks)
                (a |>Async.RunSynchronously)
                [| 0 .. length-1|];;
#endif

#if !NETCOREAPP
(*
#This part of control suite disabled under bug#1809
module ThreadAbortTests = 
    open System.Threading

    for i = 1 to 100 do 
        let t = new Thread(new ThreadStart(fun _ -> 
            for j in 1 .. 10 do
                Async.RunSynchronously(async { 
                    for i in 1 .. 10 do
                        do printfn "running async ... j = %d, i = %d" j i
                        do Async.Sleep(10)
                    return () })), IsBackground=true)

        t.Start()
        Async.Sleep(1)
        t.Abort()


    open System.Threading

    type msg = Fetch of AsyncReplyChannel<int>

    let rec bp : MailboxProcessor<msg>
       = new MailboxProcessor<msg>(fun inbox -> 
                  let rec loop () = async { 
                      do printfn "receiving... #msg = %d" (Seq.length bp.UnsafeMessageQueueContents)

                      let! (Fetch msg) = inbox.Receive()   
                      do Async.Sleep(10)
                      
                      do msg.Post(3)
                      return! loop () }
                  loop())

    bp.Start()

    bp.UnsafeMessageQueueContents              
    for k = 1 to 100 do
        let t = new Thread(new ThreadStart(fun _ -> 
            for j in 1 .. 10 do
                printfn "res = %d" (bp.PostAndReply(fun reply -> Fetch(reply)))
            ), IsBackground=true)

        printfn "starting..." 
        t.Start()
        Async.Sleep(10)
        printfn "aborting..." 
        t.Abort()
*)
#endif

let time f x = 
    let timer = System.Diagnostics.Stopwatch.StartNew ()
    let res = f x
    res, timer.Elapsed

// some long computation...
let rec fibo = function
  | 0 | 1 -> 1
  | n -> fibo (n - 1) + fibo (n - 2)


let fail = async { do failwith "fail" }


let expect_failure a =
  async { try
            do! a
            return false
          with _ -> return true }

// Exception catching test
let test1 () =
    test "test1" (expect_failure fail |> Async.RunSynchronously)


let catch a =
    Async.RunSynchronously
        (async {try let! _ = a
                    return ()
                with _ -> return ()})

let to_be_cancelled n flag1 flag2 =
  async { use! holder = Async.OnCancel(fun _ -> incr flag1)
#if NETCOREAPP
          do Task.Delay(n/8).Wait()
#else
          do System.Threading.Thread.Sleep (n / 8)
#endif
          let! _ = async { return 1 } // implicit cancellation check
          do incr flag2}


// Cancellation test
let test2 () =
    let flag1 = ref 0
    let flag2 = ref 0
    let n = 400
    for i in 1 .. n / 2 do
       catch (Async.Parallel2(fail, to_be_cancelled i flag1 flag2))
       catch (Async.Parallel2(to_be_cancelled i flag1 flag2, fail))
    printfn "%d, %d" !flag1 !flag2
    test "test2 - OnCancel" (!flag1 >= 0 && !flag1 < n && !flag2 >= 0 && !flag2 < n)


#if !NETCOREAPP
// SwitchToNewThread
let test3 () =
    let ids = ref []
    let rec a n =
        async { do ids := System.Threading.Thread.CurrentThread.ManagedThreadId :: !ids
                do! Async.SwitchToNewThread()
                if n < 100 then 
                    do! a (n + 1) }
                    
    // on low-resource test VMs, this might not pass on the first try.  Give a few chances.               
    let rec doTest maxAttempts =
        printfn "doTest, maxAttempts %d" maxAttempts
        match maxAttempts with
        | n when n <= 0 -> ()  // give up
        | _ ->
            ids := []
            Async.RunSynchronously (a 1)
            let res = (set !ids).Count
            if res > 1 then ()
            else 
               printfn "res = %d" res
               doTest (maxAttempts - 1)

    doTest 5
    let res = (set !ids).Count
    test (sprintf "test3 - SwitchToNewThread  res = %d" res) (res > 1)
#endif

(*
// Performance test (only if multi-core)
let test4 () =
    let fibo_1() =
        fibo 32 |> ignore;
        fibo 32 |> ignore;
        fibo 32 |> ignore;
        fibo 32

    // should be at least 20% faster if multi-core
    let fibo_2() =
        let a1 = Async.SpawnFuture(async { return fibo 32 })
        let a2 = Async.SpawnFuture(async { return fibo 32 })
        let a3 = Async.SpawnFuture(async { return fibo 32 })
        fibo 32 |> ignore
        a1.Value |> ignore
        a2.Value |> ignore
        a3.Value

    let (r1, t1), (r2, t2) = time fibo_1 (), time fibo_2 ()
    test "test4 - future (multi-core)" (r1 = r2)
    printfn "test4 - performance, ratio = %f, expect < 0.8" (float t2.Ticks / float t1.Ticks)
    if t2.Ticks > t1.Ticks * 80L / 100L then
        printfn "  Warning: performance test failed."

*)

// test thread safety (using a lock)
let test8() =
    printfn "test8 started"
    let syncRoot = System.Object()
    let k = ref 0
    let comp _ = async { return lock syncRoot (fun () -> incr k
#if NETCOREAPP
                                                         Task.Delay(1).Wait()
#else
                                                         System.Threading.Thread.Sleep(1)
#endif
                                                         !k ) }
    let arr = Async.RunSynchronously (Async.Parallel(Seq.map comp [1..50]))
    test "test8 - lock" ((Array.sortWith compare arr) = [|1..50|])

// without lock, there "must" be concurrent problems
let test9() =
    let syncRoot = System.Object()
    let k = ref 0
    let comp _ = async { do incr k
                         do! Async.Sleep(10)
                         return !k }
    let arr = Async.RunSynchronously (Async.Parallel(Seq.map comp [1..100]))
    test "test9 - no lock" ((Array.sortWith compare arr) <> [|1..100|])


// performance of Async.Parallel
let test10() =
    let fibo_1() =
        Array.init 37 fibo

    // should be at least 30% faster if multi-core
    let fibo_2() =
        let compute n = async { return fibo n }
        let arr = Array.init 37 compute
        Async.RunSynchronously (Async.Parallel arr)

    try
        let (r1, t1), (r2, t2) = time fibo_1 (), time fibo_2 ()
        test "test10 - Async.Parallel" (r1 = r2)
        printfn "test10 - performance, ratio = %f, expect < 0.8" (float t2.Ticks / float t1.Ticks)
        if t2.Ticks > t1.Ticks * 80L / 100L then
            printfn "  Warning: performance test failed."
    with
    | e ->
        test "test10 - Async.Parallel" false
        printfn "%s" (e.ToString())

(*
// performance of Async.Future
let test11() =
    let fibo_1() =
        Array.init 37 fibo

    // should be at least 30% faster if multi-core
    let fibo_2() =
        let compute n = async { return fibo n }
        let arr = Array.init 37 (fun n -> Async.SpawnFuture (compute n))
        arr |> Array.map (fun a -> a.Value)
  
    let (r1, t1), (r2, t2) = time fibo_1 (), time fibo_2 ()
    test "test11 - AsyncFuture" (r1 = r2)
    printfn "test11 - performance, ratio = %f, expect < 0.8" (float t2.Ticks / float t1.Ticks)
    if t2.Ticks > t1.Ticks * 80L / 100L then
        printfn "  Warning: performance test failed."
*)


// performance of Async.Generate
let test12() =
    let fibo_1() =
        Array.init 37 fibo

    let fibo_2() =
        let compute n = async { return fibo n }
        Async.RunSynchronously (Async.Generate(37, compute, 37))

    try
        let (r1, t1), (r2, t2) = time fibo_1 (), time fibo_2 ()
        test "test12 - Async.Generate" (r1 = r2)
        printfn "test12 - performance, ratio = %f, expect < 0.8" (float t2.Ticks / float t1.Ticks)
        if t2.Ticks > t1.Ticks * 80L / 100L then
            printfn "  Warning: performance test failed."
    with
    | e ->
        test "test12 - performance" false
        printfn "%s" (e.ToString())


// Self-cancellation
let test13() =
    let a = async {
        try
            do Async.CancelDefaultToken()
            let! _ = async { return 1 } 
            do test "test13" false
        with
        | _ -> do test "test13" false }

    try       
        a |> Async.RunSynchronously |> ignore
        test "test13" false
    with
    | :? System.OperationCanceledException -> test "test13" true
    | _ -> test "test13" false


// Exceptions
let test14() =
    try
        fail |> Async.RunSynchronously |> ignore
        test "test14" false
    with
    | Failure "fail" -> test "test14" true
    | e -> 
        test "test14" false
        printfn "test14 caught exception was: %s" (e.ToString())


// Useful class: put "checkpoints" in the code.
// Check they are called in the right order.
type Path(str) =
    let mutable current = 0
    member p.Check n = check (str + " #" + string (current+1)) n (current+1)
                       current <- n

// Cancellation - TryCancelled
let test15() =
    let p = Path "test15 - cancellation"

    let cancel = async {
        do! Async.Sleep 100
        do failwith "fail" }

    let a = async {
        try
            use! holder = Async.OnCancel (fun _ -> p.Check 1)
            do! Async.Sleep 200
            do p.Check 2
            let! _ = async { return 1 } 
            do p.Check (-1)
        finally
            p.Check 3}
    try
        let a = Async.TryCancelled(a, (fun _ -> p.Check 4))
        let a = Async.TryCancelled(a, (fun _ -> p.Check 5))
        let a = Async.TryCancelled(a, (fun _ -> p.Check 6))
        Async.Parallel2(a, cancel)
        |> Async.RunSynchronously |> ignore
    with _ -> ()
#if NETCOREAPP
    Task.Delay(300).Wait()
#else
    System.Threading.Thread.Sleep(300)
#endif
    p.Check 7

// The same, without the cancellation
let test15b() =
    let p = Path "test15b, no cancellation"
    let a = async {
        try
            use! holder = Async.OnCancel (fun _ -> p.Check -1)
            do! Async.Sleep 200
            do p.Check 1
            let! _ = async { return 1 } 
            do p.Check 2
        finally
            p.Check 3}
    try
        let a = Async.TryCancelled(a, (fun _ -> p.Check -1))
        a |> Async.RunSynchronously |> ignore
    with _ -> ()
#if NETCOREAPP
    Task.Delay(100).Wait()
#else
    System.Threading.Thread.Sleep(100)
#endif

test1()
test2()
#if !NETCOREAPP
test3()
#endif
test8()
test9()
test13()
test14()
// ToDo: 7/31/2008: Disabled because of probable timing issue.  QA needs to re-enable post-CTP.
// Tracked by bug FSharp 1.0:2891
//test15()
// ToDo: 7/31/2008: Disabled because of probable timing issue.  QA needs to re-enable post-CTP.
// Tracked by bug FSharp 1.0:2891
//test15b()

// performance (multi-core only)
if System.Environment.ProcessorCount > 1 then
    test10()
    test12()



// test cancellation, with a sub-computation
let test22() =
    let p = Path "test22"
    let a = async {
        do p.Check 1
#if NETCOREAPP
        do Task.Delay(200).Wait()
#else
        do System.Threading.Thread.Sleep(200)
#endif
        do p.Check 3
        let! _ = async { return 1 } 
        do p.Check -1
    }
    let run = async {
        try
            do! a
            do p.Check -1
        with _ -> do p.Check -1
    }
    let run = Async.TryCancelled(run, fun _ -> p.Check 4)
    let group = new System.Threading.CancellationTokenSource()
    Async.Start(run,group.Token)
#if NETCOREAPP
    Task.Delay(100).Wait()
#else
    System.Threading.Thread.Sleep(100)
#endif
    p.Check 2
    group.Cancel()
#if NETCOREAPP
    Task.Delay(200).Wait()
#else
    System.Threading.Thread.Sleep(200)
#endif
    p.Check 5

// ToDo: 7/25/2008: Disabled because of probable timing issue.  QA needs to re-enable post-CTP.
// Tracked by bug FSharp 1.0:2891
// test22()
    
module ParallelTest = 

    let Test() =
        let n = 10
        let controlWasReturnedToClientAlready = ref false
        // goal is invariant that client sees invariant 
        // numOutstandingWorkers=0 after Async.RunSynchronously call
        let numOutstandingWorkers = ref 0  
        let ex = new System.Exception()
        let failureHappened = ref false
        try
            let res = 
                Async.RunSynchronously 
                    (Async.Parallel 
                        [for i in 0..n-1 ->
                            async {
                                if i=0 then
                                    // let all other threads spin up before we fail
                                    while !numOutstandingWorkers <> n-1 do
                                        do! Async.Sleep(100) 
                                    failureHappened := true
                                    return raise ex
                                else
                                    use r = { new System.IDisposable with 
                                                  member x.Dispose() = // This gets run when the cancel happens
                                                      if i=n-1 then
                                                          // last guy waits a long time to ensure client is blocked
#if NETCOREAPP
                                                          Task.Delay(200).Wait()
#else
                                                          System.Threading.Thread.Sleep(2000)
#endif
                                                      if not(!controlWasReturnedToClientAlready) then
                                                          lock numOutstandingWorkers (fun() -> numOutstandingWorkers := !numOutstandingWorkers - 1) }
                                    // mark that we began
                                    lock numOutstandingWorkers (fun() -> numOutstandingWorkers := !numOutstandingWorkers + 1)
                                    // Note - this doesn't return - it must be cancelled
                                    while true do 
                                        do! Async.Sleep(100*n) 
                                    return 1 }]) 
            ()
        with
            | e when obj.ReferenceEquals(e,ex) -> ()
            | e2 -> 
                failureHappened := true
                check (sprintf "first exception was not returned to client, instead got: %s" (e2.ToString())) 1 0

        controlWasReturnedToClientAlready := true

        if !numOutstandingWorkers<> 0 then
            failureHappened := true
            check  "test failed because client got ahead of cleanup"    1 0

        if !failureHappened = false then
            check  "parallel-test-success"    1 1

    Test()    


#if !TESTS_AS_APP && !NETCOREAPP
// See bug 5570, check we do not switch threads
module CheckNoPumpingOrThreadSwitchingBecauseWeTrampolineSynchronousCode =
    let checkOnThread  msg expectedThreadId = 
        let currentId = System.Threading.Thread.CurrentThread.ManagedThreadId
        checkQuiet msg currentId expectedThreadId 

    async { let tid = System.Threading.Thread.CurrentThread.ManagedThreadId
            let ctxt = System.Threading.SynchronizationContext.Current
            let state =  ref 1
            for i = 0 to 10000 do 
                let! res = async { return 1+i } // a bind point to a synchronous process, we do not expect this to switch threads or pump
                
                
                // The execution of these Posts should be delayed until after the async has exited its synchronous code
                if ctxt <> null then 
                    ctxt.Post((fun _ -> 
                        //printfn "setting, tid = %d" System.Threading.Thread.CurrentThread.ManagedThreadId; 
                        state := 2), null) 
                if i % 50 = 0 then printfn "tick %d..." i
                checkOnThread "clkneoiwen thread check" tid 
                checkQuiet "cwnewe9wecokm" !state 1 } |> Async.StartImmediate

open System.Windows.Forms

#if COMPILED
// See bug 5570, check we do not switch threads
module CheckNoPumpingBecauseWeTrampolineSynchronousCode =
    let checkOnThread  msg expectedThreadId = 
        let currentId = System.Threading.Thread.CurrentThread.ManagedThreadId
        checkQuiet msg currentId expectedThreadId 

    let form = new System.Windows.Forms.Form()
    form.Load.Add(fun _ -> 
    
        async { let tid = System.Threading.Thread.CurrentThread.ManagedThreadId
                let ctxt = System.Threading.SynchronizationContext.Current
                let state =  ref 1
                for i = 0 to 10000 do 
                    let! res = async { return 1+i } // a bind point to a synchronous process, we do not expect this to switch threads or pump

                    // The execution of these Posts should be delayed until after the async has exited its synchronous code
                    if ctxt <> null then 
                        ctxt.Post((fun _ -> 
                            //printfn "setting, tid = %d" System.Threading.Thread.CurrentThread.ManagedThreadId; 
                            state := 2), null) 
                    if i % 50 = 0 then printfn "tick %d..." i
                    checkOnThread "clkneoiwen thread check" tid 
                    checkQuiet "cwnewe9wecokm" !state 1 
                Application.Exit() } 
             |> Async.StartImmediate)

    form.WindowState <- FormWindowState.Minimized
    form.ShowInTaskbar <- false
    Application.Run form             
    // Set the synchronization context back to its original value
    System.Threading.SynchronizationContext.SetSynchronizationContext(null);
    
#endif

#endif


module CheckContinuationsMayNotBeCalledMoreThanOnce = 

    (try 
             Async.FromContinuations(fun (cont,_,_) -> cont(); cont()) |> Async.StartImmediate
             false
     with :? System.InvalidOperationException -> true)
    |> check "celkner091" true

    (try 
             Async.FromContinuations(fun (cont,econt,_) -> cont (); econt (Failure "fail")) |> Async.StartImmediate
             false
     with :? System.InvalidOperationException -> true)
    |> check "celkner092" true


    (try 
             Async.FromContinuations(fun (cont,econt,ccont) -> cont (); ccont (System.OperationCanceledException())) |> Async.StartImmediate 
             false
     with :? System.InvalidOperationException -> true)
    |> check "celkner093" true


    (try 
             Async.FromContinuations(fun (cont,econt,ccont) -> ccont (System.OperationCanceledException()); ccont (System.OperationCanceledException())) |> Async.StartImmediate 
             false
     with :? System.InvalidOperationException -> true)
    |> check "celkner094" true


module CheckStartImmediate = 

    // Check the async is executed immediately
    (let res = ref 1
     async { res := 2 } |> Async.StartImmediate
     res.Value)
    |> check "celkner091" 2


    // Check we catch the failure immediately
    (try 
         async { failwith "fail" } |> Async.StartImmediate 
         1
     with Failure _ -> 2)
    |> check "celkner091" 2

module Bug5770 =
    type exp = 
        | Var of string           
        | Lambda of string * exp           
        | Apply of exp * exp
    
    let pfoldExpr varF lamF appF exp =     
        let rec Loop e = async {
            match e with        
            | Var x -> return (varF x)        
            | Lambda (x, body) -> let! bodyAcc = Loop body
                                  return (lamF x bodyAcc)       
            | Apply (l, r) -> let! [| lAcc; rAcc |] = [| Loop l; Loop r |] |> Async.Parallel 
                              return (appF lAcc rAcc) }  
        Loop exp 
        
    let ptoString e =    
        pfoldExpr        
            (fun x -> sprintf "%s" x)        
            (fun x y -> sprintf "(\\%s.%s)" x y)        
            (fun x y -> sprintf "(%s %s)" x y)   
            e     
        |> Async.RunSynchronously 

    let e = Apply(Lambda("x", Var "x"), Lambda("y", Var "y"))
    let C e = Apply(e,e)
    let bigE = C(C(C(C(C(C(C(C(C(C(C(C(C(e)))))))))))))
    
    try
        for i = 1 to 10 do
            check (sprintf "dfgeyrtwq26: Async.Parallel spawning Parallel %d" i)
                (ptoString bigE |> ignore; "Complete")
                "Complete"
    with
    | e ->
        check "Bug 5770" true false
        printfn "%s" (e.ToString())



module Bug6086 =
    try
        let count = 200000
        let rec loop i : Async<string> = 
            async { if i <= 0 then return "finished"
                    else return! loop (i-1) }
        
        check "4dyeyuiqyhi3 - async"
            (loop count |> Async.RunSynchronously)
            "finished"
        
        let rec p i : Async<string> = async.Delay(fun () -> if i > 0 then  p (i-1) else async.Return("finished"))
        check "4dyeyuiqyhi3 - async.Delay"
            (loop count |> Async.RunSynchronously)
            "finished"
    with
    | e ->
        check "Bug6086" true false
        printfn "%s" (e.ToString())
    
module Bug850869 =
  let f1() = 
    async
      { use! a = Unchecked.defaultof<Async<System.IDisposable>>
        return true }

  let f2() = 
    async
      { use! A = Unchecked.defaultof<Async<System.IDisposable>>
        return true }         

module ExceptionInAsyncParallelOrHowToInvokeContinuationTwice = 
    
    let test() = 
        let pair = async {
            try
                let! r = 
                    [
                        async {
                            do! Async.Sleep 10000
                            return failwith "Should not be exercised"
                        }
                        async {
                            ignore (42/0) //this will throw
                            return 42
                        }
                    ] |> Async.Parallel
 
                return "OK"
            with :? System.DivideByZeroException -> return "OOPS"
        }
        let counter = ref 0
        let driver = async {
            let! r = pair
            incr counter
        }
        try
            Async.RunSynchronously driver
            !counter = 1
        with
            _ -> false

    check "ExceptionInAsyncParallelOrHowToInvokeContinuationTwice" (Seq.init 30 (ignore >> test) |> Seq.forall id) true
    
// [Asyncs] Better stack traces for Async
module BetterStacksTest1 = 

    let FunctionRaisingException() = 
       async { do failwith "" }

    let f2() = 
       async { for i in 0 .. 10 do 
                 do! FunctionRaisingException() }


    let v = 
        try 
            f2() |> Async.StartImmediate 
            ""
        with e -> e.StackTrace

    //printfn "STACK 1\n------------------------------------\n%s\n----------------------------------" v

    test "BetterStacks1" (v.Contains("FunctionRaisingException"))

module BetterStacksTest2 = 

    let FunctionRaisingException() = 
       async { do failwith "" }

    let f2() = 
       Async.FromContinuations(fun (cont, econt, ccont) -> Async.StartWithContinuations(FunctionRaisingException(), cont, econt, ccont))

    let v = 
        try 
            f2() |> Async.StartImmediate 
            ""
        with e -> e.StackTrace

    //printfn "STACK #2\n------------------------------------\n%s\n----------------------------------" v

    test "BetterStacks2" (v.Contains("FunctionRaisingException"))

// [Asyncs] Cancellation inside Async.AwaitWaitHandle may release source WaitHandle
module Bug391710 =
    open System
    open System.Threading
 
    let mutable trace = List.empty<string>
    let Bug391710() = 
        use s = new ManualResetEvent(false)
        let cts = new CancellationTokenSource()

        let a1 = 
            async { let! _ok = Async.AwaitWaitHandle s
                    trace <- List.Cons("a1", trace)
            }

        let a2 = 
            async { let! _ok = Async.AwaitWaitHandle s
                    trace <- List.Cons("a2", trace)
            }

        Async.Start(a1, cancellationToken = cts.Token)
        Async.Start(a2)
#if NETCOREAPP
        Task.Delay(500).Wait();
#else
        System.Threading.Thread.Sleep(500)
#endif
        cts.Cancel()

    try
        Bug391710()
#if NETCOREAPP
        Task.Delay(500).Wait();
#else
        System.Threading.Thread.Sleep(2000)
#endif
        check "Check that cancellation of 'a1' does not trigger the signalling of 's' (Bug391710)" trace []
    with
    | e ->
        check "Bug391710" true false
        printfn "%s" (e.ToString())

// Some tests should not be run in the static constructor
let RunAll() = 
    BasicTests.Run()
    StartChildOutsideOfAsync.Run()
    SpawnTests.Run()
    AsBeginEndTests.AsBeginEndTest()
#if !MONO && !NETCOREAPP
    AwaitEventTests.AwaitEventTest()
#endif
    OnCancelTests.Run()
    GenerateTests.Run()
    ParallelTests.Run()
#if !NETCOREAPP
    AsyncWaitOneTest1.Run()
    AsyncGenerateTests.Run()
#endif

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
      printf "TEST PASSED OK" ;
      exit 0
#endif

