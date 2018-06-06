
// Tests that async stack traces contain certain method names 

// See https://github.com/Microsoft/visualfsharp/pull/4867

// The focus of the tests is on the synchronous parts of async execution, and on exceptions.

let mutable failures = []
let syncObj = new obj()
let report_failure s = 
  stderr.WriteLine " NO"; 
  lock syncObj (fun () ->
     failures <- s :: failures;
     printfn "FAILURE: %s failed" s
  )

let test s b = stderr.Write(s:string);  if b then stderr.WriteLine " OK" else report_failure s 

let checkQuiet s x1 x2 = 
    if x1 <> x2 then 
        (test s false; 
         printfn "expected: %A, got %A" x2 x1)

let check s x1 x2 = 
    if x1 = x2 then test s true
    else (test s false; printfn "expected: %A, got %A" x2 x1)


let rec async_syncLoopViaTailCallFail(n) = async {
    if n > 10 then 
        let! res = failwith "fail"
        return res
    else
        return! async_syncLoopViaTailCallFail(n+1)
}

let rec async_syncLoopViaNonTailCallFail(n) = async {
    if n > 10 then 
        let! res = failwith "fail"
        return Unchecked.defaultof<_>
    else
        let! n2 = async_syncLoopViaNonTailCallFail(n+1)
        return n2
}

let rec async_syncWhileLoopFail() = async {
    let mutable n = 0
    while true do
        if n > 10 then 
            let! res = failwith "fail"
            return res
        else
            n <- n + 1
}

let rec async_syncTryFinallyFail() = async {
    try 
       failwith "fail"
    finally
        ()
}

// Raising an exception counts as an "asynchronous action" which wipes out the stack. 
//
// This is because of a limitation in the .NET exception mechanism where stack traces are only populated
// up to the point where they are caught, so we need to catch them in the trampoline handler to get a good stack.
//
// This means that re-raising that exception (e.g. in a failed pattern match for a try-with)
// or throwing an exception from the "with" handler will not get a good stack. 
// There is not yet any good workaround for this.
//
//let rec async_syncTryWithFail() = async {
//    try 
//       failwith "fail"
//    with _ -> ()
//}

let rec async_syncPreAsyncSleepFail() = async {
    let! x = failwith "fail" // failure is in synchronous part of code
    do! Async.Sleep 10
    return Unchecked.defaultof<_>
}

let rec async_syncFail() = async {
    failwith "fail"
}

let asyncCheckEnvironmentStackTracesBottom() = async {
    let stack = System.Diagnostics.StackTrace(true).ToString()
    //test "vwerv0re0reer: stack = %s",  stack);
    test "clncw09ew09c1" (stack.Contains("asyncCheckEnvironmentStackTracesBottom"))
    test "clncw09ew09c2" (stack.Contains(string (int __LINE__ - 3)))
    test "clncw09ew09d3" (stack.Contains("asyncCheckEnvironmentStackTracesMid"))
    test "clncw09ew09e4" (stack.Contains("asyncCheckEnvironmentStackTracesTop"))
    return 1
}

let asyncCheckEnvironmentStackTracesMid() = async {
    let! res =  asyncCheckEnvironmentStackTracesBottom()
    let stack = System.Diagnostics.StackTrace(true).ToString()
    test "clncw09ew09d2" (stack.Contains("asyncCheckEnvironmentStackTracesMid"))
    test "clncw09ew09c" (stack.Contains(string (int __LINE__ - 2)))
    test "clncw09ew09e2" (stack.Contains("asyncCheckEnvironmentStackTracesTop"))
    return res
}

let asyncCheckEnvironmentStackTracesTop() = async {
    let! res = asyncCheckEnvironmentStackTracesMid()
    let stack = System.Diagnostics.StackTrace(true).ToString()
    test "clncw09ew09f" (stack.Contains("asyncCheckEnvironmentStackTracesTop"))
    test "clncw09ew09c" (stack.Contains(string (int __LINE__ - 2)))

    do! Async.Sleep 10
    let stack = System.Diagnostics.StackTrace(true).ToString()
    test "clncw09ew09f" (stack.Contains("asyncCheckEnvironmentStackTracesTop"))
    test "clncw09ew09c" (stack.Contains(string (int __LINE__ - 2)))
    
}

let asyncMid(f) = async {
    
    let! res = f()
    ()
}
let asyncTop2(f) = async {
    
    let! res = asyncMid(f)
    return ()
}

let asyncTop3(f) = async {

    do! Async.Sleep 10    
    let! res = asyncMid(f)
    return ()
}



asyncCheckEnvironmentStackTracesTop() |> Async.RunSynchronously

let testCasesThatRaiseExceptions = 
    [ ("async_syncFail", async_syncFail)
      ("async_syncLoopViaTailCallFail", (fun () -> async_syncLoopViaTailCallFail(0)))
      ("async_syncLoopViaNonTailCallFail", (fun () -> async_syncLoopViaNonTailCallFail(0))) 
      ("async_syncWhileLoopFail", async_syncWhileLoopFail)
      ("async_syncTryFinallyFail", async_syncTryFinallyFail) 
      ("async_syncPreAsyncSleepFail", async_syncPreAsyncSleepFail) ]

for (asyncTopName, asyncTop) in [("asyncTop2", asyncTop2); ("asyncTop3", asyncTop3) ] do
    for functionName, asyncFunction in testCasesThatRaiseExceptions do 
    try 
        asyncTop(asyncFunction) |> Async.RunSynchronously |> ignore
        failwith "should have raised exception"
    with e -> 
        let stack = e.StackTrace
        test (sprintf "case %s: clncw09ew09m0" functionName) (not (stack.Contains("line 0")))
        test (sprintf "case %s: clncw09ew09m1" functionName) (stack.Contains(functionName))
        test (sprintf "case %s: clncw09ew09n2" functionName) (stack.Contains("asyncMid"))
        test (sprintf "case %s: clncw09ew09n3" functionName) (stack.Contains(asyncTopName))

let aa =
  if not failures.IsEmpty then 
      stdout.WriteLine "Test Failed"
      exit 1
  else   
      stdout.WriteLine "Test Passed"
      System.IO.File.WriteAllText("test.ok","ok")
      exit 0

