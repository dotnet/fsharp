module Tests_Tasks

#nowarn "57"
open FSharp.Core.CompilerServices
open FSharp.Core.CompilerServices.StateMachineHelpers
open System.Runtime.CompilerServices

let failures = ref []

let report_failure (s : string) = 
    stderr.Write" NO: "
    stderr.WriteLine s
    failures := !failures @ [s]

let test s b = stderr.Write(s:string);  if b then stderr.WriteLine " OK" else report_failure s

let check s v1 v2 = 
   stderr.Write(s:string);  
   if (v1 = v2) then 
       stderr.WriteLine " OK" 
   else
       eprintf " FAILED: got %A, expected %A" v1 v2 
       report_failure s

[<AbstractClass>]
type SyncMachine<'T>() =

    [<DefaultValue(false)>]
    val mutable PC : int

    abstract Step : unit -> 'T

    member this.Start() = this.Step()

[<Struct; NoEquality; NoComparison>]
type SyncMachineStruct<'TOverall> =

    [<DefaultValue(false)>]
    val mutable PC : int

    [<DefaultValue(false)>]
    val mutable Result : 'TOverall

    //member sm.Address = 0
        //let addr = &&sm.Dummy
        //Microsoft.FSharp.NativeInterop.NativePtr.toNativeInt addr

    // MoveNext without boxing
    static member inline MoveNext(sm: byref<'T> when 'T :> IAsyncStateMachine) = sm.MoveNext()
    interface IAsyncStateMachine with 
        member sm.MoveNext() = failwith "no dynamic impl"
        member sm.SetStateMachine(state: IAsyncStateMachine) = failwith "no dynamic impl"

module ResumableObject_OuterConditional =

    // RO with outer "if __useResumableCode then" conditional
    let makeStateMachine() = 
        if __useResumableCode then
            (
                { new SyncMachine<int>() with 
                    [<ResumableCode>]
                    member _.Step ()  = 1
                }
            ).Start()
        else
            failwith "should have been compiled to resumable code, no interpretation available"
    check "ResumableObject_OuterConditional" (makeStateMachine()) 1

module ResumableObject_SimpleImmediateExpression =
    let makeStateMachine()  = 
        if __useResumableCode then
            (
                { new SyncMachine<int>() with 
                    [<ResumableCode>]
                    member _.Step ()  =
                        // TEST: a resumable object may contain simple expressions
                        1 
                }
            ).Start()
        else
            failwith "should have been compiled to resumable code, no interpretation available"
    check "ResumableObject_SimpleImmediateExpression" (makeStateMachine()) 1

module ResumableObject_FreeVariable =
    let makeStateMachine x = 
        if __useResumableCode then
            (
                { new SyncMachine<int>() with 
                    [<ResumableCode>]
                    member _.Step ()  =
                       // TEST: a resumable object may contain free variables
                        1 + x
                }
            ).Start()
        else
            failwith "should have been compiled to resumable code, no interpretation available"
    let sm3 = makeStateMachine 3
    check "ResumableObject_FreeVariable" (makeStateMachine 3) 4

module ResumableObject_WithoutStart =
    let makeStateMachine x = 
        if __useResumableCode then
                { new SyncMachine<int>() with 
                    [<ResumableCode>]
                    member _.Step ()  =
                       // TEST: a resumable object may contain free variables
                        1 + x }
        else
            failwith "should have been compiled to resumable code, no interpretation available"
    let sm3 = makeStateMachine 3
    check "ResumableObject_WithoutStart" (sm3.Start()) 4

module ResumableObject_OuterLet =
    let makeStateMachine x = 
        // TEST: a resumable object may be just after a non-macro 'let'
        let rnd = System.Random().Next(4)
        if __useResumableCode then
            (
                { new SyncMachine<int>() with 
                    [<ResumableCode>]
                    member _.Step ()  =  
                       x + rnd - rnd
                }
            ).Start()
        else
            failwith "should have been compiled to resumable code, no interpretation available"
    check "ResumableObject_OuterLet" (makeStateMachine 4) 4

module ResumableCode_Macro1 =
    let makeStateMachine y = 
        // TEST: resumable object may declare macros
        let __expand_code x = x + 1
        if __useResumableCode then
            (
                { new SyncMachine<int>() with 
                    [<ResumableCode>]
                    member _.Step ()  =  
                       // TEST: resumable code may invoke macros
                       __expand_code y
                }
            ).Start()
        else
            failwith "should have been compiled to resumable code, no interpretation available"
    check "ResumableCode_Macro1" (makeStateMachine 3) 4

module ResumableCode_Macro2 =
    let makeStateMachine y = 
        if __useResumableCode then
            // TEST: resumable object may declare macros
            let __expand_code x = x + 1
            (
                { new SyncMachine<int>() with 
                    [<ResumableCode>]
                    member _.Step ()  =  
                       // TEST: resumable code may invoke macros
                       __expand_code y
                 }
            ).Start()
        else
            failwith "should have been compiled to resumable code, no interpretation available"
    check "ResumableCode_Macro2" (makeStateMachine 3) 4

module ResumableCode_Macro3 =
    let makeStateMachine y = 
        if __useResumableCode then
            (
                { new SyncMachine<int>() with 
                    [<ResumableCode>]
                    member _.Step ()  =  
                       // TEST: resumable object may declare macros
                       let __expand_code x = x + 1
                       // TEST: resumable code may invoke macros
                       __expand_code y
                }
            ).Start()
        else
            failwith "should have been compiled to resumable code, no interpretation available"
    check "ResumableCode_Macro3" (makeStateMachine 3) 4

module ResumableCode_TwoMacro =
    let makeStateMachine y = 
        // TEST: resumable object may declare macros
        let __expand_code1 x = x + 1
        let __expand_code2 x = __expand_code1 x + 2
        if __useResumableCode then
            (
                { new SyncMachine<int>() with 
                    [<ResumableCode>]
                    member _.Step ()  =  
                       // TEST: resumable code may invoke a macro that invokes another macro
                       __expand_code2 y
                }
            ).Start()
        else
            failwith "should have been compiled to resumable code, no interpretation available"
    check "ResumableCode_TwoMacro" (makeStateMachine 3) 6

module ResumableCode_Let =
    let makeStateMachine x = 
        if __useResumableCode then
            (
                { new SyncMachine<int>() with 
                    [<ResumableCode>]
                    member _.Step ()  =  
                       // TEST: resumable code may let statements
                       let y = 1 - x
                       if x > 3 then 1 + x else y
                }
            ).Start()
        else
            failwith "should have been compiled to resumable code, no interpretation available"
    check "ResumableCodeLet" (makeStateMachine 3) -2

#if NEGATIVE
// Resumable code may NOT have let rec statements
module ResumableCode_LetRec =
    let makeStateMachine inputValue = 
        if __useResumableCode then
            (
                { new SyncMachine<int>() with 
                    [<ResumableCode>]
                    member _.Step ()  =  
                       let rec f x = if x > 0 then f (x-1) else inputValue + 13
                       f 10 + f 2
                 }
            ).Start()
        else
            failwith "should have been compiled to resumable code, no interpretation available"
    check "vwepojwve" (makeStateMachine 13) 32
#endif

module ResumableCode_Sequential =
    let makeStateMachine y = 
        let __expand_code1 () = printfn "step1" 
        let __expand_code2 x = x + 2
        if __useResumableCode then
            (
                { new SyncMachine<int>() with 
                    [<ResumableCode>]
                    member _.Step ()  =  
                       // TEST: resumable code may sequentially do one thing then another
                       __expand_code1 ()
                       __expand_code2 y
                }
            ).Start()
        else
            failwith "should have been compiled to resumable code, no interpretation available"

    check "ResumableCode_Sequential" (makeStateMachine 3) 5

module ResumableCode_Sequential2 =
    let makeStateMachine y = 
        if __useResumableCode then
            (
                { new SyncMachine<int>() with 
                    [<ResumableCode>]
                    member _.Step ()  =  
                       // TEST: resumable code may sequentially do one thing then another
                       printfn "step1" 
                       y + 2
                }
            ).Start()
        else
            failwith "should have been compiled to resumable code, no interpretation available"
    check "ResumableCode_Sequential2" (makeStateMachine 3) 5

module ResumableCode_ResumeAt0 =
    let makeStateMachine y = 
        // TEST: resumable object may declare macros
        if __useResumableCode then
            (
                { new SyncMachine<int>() with 
                    [<ResumableCode>]
                    member _.Step ()  =  
                        if __useResumableCode then
                            // TEST: resumable code may have __resumeAt at the start of the code
                            __resumeAt 0
                            // Label 0 goes here
                            // Specify a resumption point
                            match __resumableEntry() with
                            | Some contID ->
                                // This is the suspension path. This gets executed as we start at label 0
                                printfn "contID = %d" contID
                                10+y
                            | None -> 
                                // This is the resumption path
                                // Label 1 goes here
                                20+y 
                        else failwith "no" }).Start()
        else
            failwith "should have been compiled to resumable code, no interpretation available"
    check "ResumableCode_ResumeAt0" (makeStateMachine 3) 13

module ResumableCode_ResumeAt1 =
    let makeStateMachine y = 
        if __useResumableCode then
            (
                { new SyncMachine<int>() with 
                    [<ResumableCode>]
                    member _.Step ()  =  
                        if __useResumableCode then
                            // TEST: resumable code may have __resumeAt at the start of the code
                            __resumeAt 1
                            // Specify a resumption point
                            match __resumableEntry() with
                            | Some contID ->
                                // This is the suspension path. It is not executed in this test because we jump straight to label 1
                                printfn "contID = %d" contID
                                10+y
                            | None -> 
                                // This is the resumption path
                                // Label 1 goes here
                                20+y
                        else
                          failwith "dynamic"
                }
            ).Start()
        else
            failwith "should have been compiled to resumable code, no interpretation available"
    check "ResumableCode_ResumeAt1" (makeStateMachine 3) 23


// Resumptions must use byrefs taking the 'this' pointer of the struct state machine. Nothing else is allowed.
module ResumableCode_Struct =
    let mutable res = 0
    let makeStateMachine inputValue = 
        if __useResumableCode then
            __structStateMachine<SyncMachineStruct<int>, int>
                (MoveNextMethod<SyncMachineStruct<int>>(fun sm -> 
                    if __useResumableCode then
                       //printfn "resuming %d" sm.Address
                       __resumeAt 1
                       failwith "unreachable in this test"
                       match __resumableEntry() with
                       | Some contID ->
                           // This is the suspension path. It is not executed in this test because we jump straight to label 1
                           failwith "unreachable in this test"
                       | None -> 
                           // This is the resumption path
                           // Label 1 goes here.
                           // This gets executed once in this test.
                           // The answer is 20 + 400
                           let res = 20 + inputValue
                           //printfn "setting result for %d to %d" sm.Address res 
                           sm.Result <- 20 + inputValue
                    else failwith "dynamic"
                    ))

                // SetStateMachine
                (SetMachineStateMethod<_>(fun sm state -> 
                    ()))

                // Start
                (AfterMethod<_,_>(fun sm -> 
                    SyncMachineStruct<_>.MoveNext(&sm)
                    //printfn "after method for %d = %d" sm.Address sm.Result
                    sm.Result))
        else
            failwith "should have been compiled to resumable code, no interpretation available"

    check "ResumableCode_Struct" (makeStateMachine 400) 420


// Resumptions must use byrefs taking the 'this' pointer of the struct state machine. Nothing else is allowed.
// TODO: negative check for code that violates this.
module ResumableCode_StructMacro =
    type CodeSpec = delegate of byref<SyncMachineStruct<int>> -> unit
    let mutable res = 0
    let makeStateMachine inputValue = 
        
        // This approximately simualtes the code you get from Task.Delaylet __expand_code  = 
        let __expand_code = 
            let __expand_f () = CodeSpec(fun sm -> sm.Result <- 420)
            CodeSpec(fun sm -> (__expand_f ()).Invoke(&sm))

        if __useResumableCode then
            __structStateMachine<SyncMachineStruct<int>, int>
                (MoveNextMethod<SyncMachineStruct<int>>(fun sm -> 
                       __expand_code.Invoke(&sm)))

                // SetStateMachine
                (SetMachineStateMethod<_>(fun sm state -> 
                    ()))

                // Start
                (AfterMethod<_,_>(fun sm -> 
                    SyncMachineStruct<_>.MoveNext(&sm)
                    //printfn "after method for %d = %d" sm.Address sm.Result
                    sm.Result))
        else
            failwith "should have been compiled to resumable code, no interpretation available"

    check "ResumableCode_StructMacro" (makeStateMachine 400) 420

module ResumableCode_FSharpFuncBetaReduction =
    let makeStateMachine x = 
            (
                { new SyncMachine<int>() with 
                    [<ResumableCode>]
                    member _.Step ()  =  
                       // TEST: a resumable object may contain beta-reducible code
                       (fun () -> 1 + x) () 
                }
            ).Start()

    check "ResumableCode_FSharpFuncBetaReduction" (makeStateMachine 3) 4
    

module ResumableCode_DelegateBetaReduction =
    let makeStateMachine x = 
        if __useResumableCode then
            (
                { new SyncMachine<int>() with 
                    [<ResumableCode>]
                    member _.Step ()  =  
                       // TEST: a resumable object may contain a beta-reduction of a delegate
                       (new System.Func<_, _>(fun a -> a + x)).Invoke 6
                }
            ).Start()
        else
            failwith "should have been compiled to resumable code, no interpretation available"

    check "ResumableCode_DelegateBetaReduction" (makeStateMachine 3) 9

module ResumableCode_ByrefDelegateBetaReduction =
    type CodeSpec = delegate of byref<int> -> unit
    let mutable res = 0
    let makeStateMachine x = 
        if __useResumableCode then
            (
                { new SyncMachine<int>() with 
                    [<ResumableCode>]
                    member _.Step ()  =  
                       // TEST: a resumable object may contain a beta-reduction of a delegate
                       (new CodeSpec(fun a -> a <- a + 3 + x)).Invoke &res
                       (new CodeSpec(fun a -> a <- a + 4)).Invoke &res
                       res
                }
            ).Start()
        else
            failwith "should have been compiled to resumable code, no interpretation available"

    check "ResumableCode_ByrefDelegateBetaReduction" (makeStateMachine 3) 10


module ResumableCode_Conditional =
    let makeStateMachine x = 
        if __useResumableCode then
            (
                { new SyncMachine<int>() with 
                    [<ResumableCode>]
                    member _.Step ()  =  
                       // TEST: a resumable object may contain conditionals
                      if x > 3 then 1 + x else 1 - x
                }
            ).Start()
        else
            failwith "should have been compiled to resumable code, no interpretation available"
    check "ResumableCode_Conditional" (makeStateMachine 3) -2

module ResumableCode_TryWithNoResumption =
    let makeStateMachine inputValue = 
        if __useResumableCode then
            (
                { new SyncMachine<int>() with 
                    [<ResumableCode>]
                    member _.Step ()  =  
                       // TEST: resumable code may contain try-with statements with no resumption
                       try 
                           inputValue + 10
                       with e -> 12
                 }
            ).Start()
        else
            failwith "should have been compiled to resumable code, no interpretation available"

    check "ResumableCode_TryWithNoResumption" (makeStateMachine 13) 23

module ResumableCode_TryWithResumption =
    let makeStateMachine inputValue = 
            (
                { new SyncMachine<int>() with 
                    [<ResumableCode>]
                    member _.Step ()  =  
                      if __useResumableCode then
                       // TEST: resumable code may contain try-with statements with resumption
                       __resumeAt 1 // this goes straight to label 1
                       try 
                           failwith "unreachable #1"
                           match __resumableEntry() with
                           | Some contID ->
                                // This is the suspension path. It is not executed in this test because we jump straight to label 1
                                failwith "unreachable #2"
                           | None -> 
                                // This is the resumption path
                                // Label 1 goes here
                                20+inputValue // this is the result
                       with e -> 
                           12 
                      else
                        failwith "should have been compiled to resumable code, no interpretation available"
                           
                           }).Start()

    check "ResumableCode_TryWithResumption" (makeStateMachine 13) 33

module ResumableCode_TryWithResumptionException =
    let makeStateMachine inputValue = 
            (
                { new SyncMachine<int>() with 
                    [<ResumableCode>]
                    member _.Step ()  =  
                      if __useResumableCode then
                       // TEST: resumable code may contain try-with statements with resumption
                       __resumeAt 1 // this goes straight to label 1
                       try 
                           // Specify a resumption point
                           match __resumableEntry() with
                           | Some contID ->
                                // This is the suspension path. It is not executed in this test because we jump straight to label 1
                                10+inputValue
                           | None -> 
                                // This is the resumption path
                                // Label 1 goes here
                                failwith "raise"
                                inputValue + 15
                       with e -> 
                           inputValue + 12  // this is the result
                      else
                        failwith "should have been compiled to resumable code, no interpretation available"
                }).Start()

    check "ResumableCode_TryWithResumptionException" (makeStateMachine 13) 25

module ResumableCode_TryWithExceptionNoResumption =
    let makeStateMachine inputValue = 
        if __useResumableCode then
            (
                { new SyncMachine<int>() with 
                    [<ResumableCode>]
                    member _.Step ()  =  
                     if __useResumableCode then
                       // TEST: resumable code may contain try-with statements with no resumption
                       try 
                           failwith "fail"
                       with e -> inputValue + 12 
                     else failwith "dynamic" }).Start()
        else
            failwith "should have been compiled to resumable code, no interpretation available"

    check "ResumableCode_TryWithExceptionNoResumption" (makeStateMachine 13) 25

module ResumableCode_TryFinallyNoResumption =
    let makeStateMachine inputValue = 
        if __useResumableCode then
            (
                { new SyncMachine<int>() with 
                    [<ResumableCode>]
                    member _.Step ()  =  
                     if __useResumableCode then
                       // TEST: resumable code may contain try-with statements with no resumption
                       let mutable res = 0
                       try 
                           res <- inputValue + 10
                       finally 
                           res <- inputValue + 40 
                       res 
                      else
                       failwith "dynamic" }).Start()
        else
            failwith "should have been compiled to resumable code, no interpretation available"

    check "ResumableCode_TryFinallyNoResumption" (makeStateMachine 13) 53

#if NEGATIVE
// try-finally may **not** contain resumption points.  This is because it's 
// very difficult to codegen them in IL. Instead you have to code try/finally logic as shown in next test.
// 
module ResumableCode_TryFinallyResumption =
    let makeStateMachine inputValue = 
        if __useResumableCode then
            (
                { new SyncMachine<int>() with 
                    [<ResumableCode>]
                    member _.Step ()  =  
                       // TEST: resumable code may contain try-finally statements with a resumption in the try block
                       __resumeAt 1 // this goes straight to label 1
                       let mutable res = inputValue
                       try 
                           failwith "unreachable" // this code is not executed as the __resumeAt goes straight to resumption label 1

                           // Specify a resumption point
                           match __resumableEntry() with
                           | Some contID ->
                               // This is the suspension path. It is not executed in this test because we jump straight to label 1
                               failwith "unreachable"
                               res <- 0
                           | None -> 
                                // This is the resumption path
                                // Label 1 goes here
                                res <- res + 10 // this gets executed and contributes to the result
                       finally 
                           res <- res + 20 // this gets executed and contributes to the result
                       res }).Start()
        else
            failwith "should have been compiled to resumable code, no interpretation available"

    check "ResumableCode_TryFinallyResumption" (makeStateMachine 13) 43
#endif

// try-finally may **not** contain resumption points.  This is because it's 
// very difficult to codegen them in IL. Instead you have to code try/finally logic as shown below.
// See also tasks.fs.
module ResumableCode_TryFinallySimulatedResumption =
    let makeStateMachine inputValue = 
        if __useResumableCode then
            (
                { new SyncMachine<int>() with 
                    [<ResumableCode>]
                    member _.Step ()  =  
                     if __useResumableCode then
                       // TEST: resumable code may contain try-finally statements with a resumption in the try block
                       __resumeAt 1 // this goes straight to label 1
                       let mutable res = 0 // note, this initialization doesn't actually happen since we go straight to label 1. However this is the zero-init value.
                       let mutable __stack_completed = false // note, this initialization doesn't actually happen since we go straight to label 1. However this is the zero-init value.
                       try 
                           failwith "unreachable" // this code is not executed as the __resumeAt goes straight to resumption label 1

                           // Specify a resumption point
                           match __resumableEntry() with
                           | Some contID ->
                                // This is the suspension path. It is not executed in this test because we jump straight to label 1
                                failwith "unreachable"
                                res <- 0
                                __stack_completed <- false
                           | None -> 
                                // This is the resumption path
                                // Label 1 goes here
                                res <- res + + inputValue + 10 // this gets executed and contributes to the result
                                __stack_completed <- true
                       with _ ->
                           res <- res + 20 
                           reraise()
                       if __stack_completed then 
                           res <- res + 20 
                       res 
                     else failwith "dynamic"}).Start()
        else
            failwith "should have been compiled to resumable code, no interpretation available"

    check "ResumableCode_TryFinallySimulatedResumption" (makeStateMachine 13) 43

module ResumableCode_TryFinallyExceptionNoResumption =
    let mutable res = 0
    let makeStateMachine inputValue = 
        if __useResumableCode then
            (
                { new SyncMachine<int>() with 
                    [<ResumableCode>]
                    member _.Step ()  =  
                       // TEST: resumable code may contain try-finally statements with no resumption
                       try 
                           failwith "fail"
                       finally 
                           res <- inputValue + 10 }).Start()
        else
            failwith "should have been compiled to resumable code, no interpretation available"

    check "ResumableCode_TryFinallyExceptionNoResumption" (try makeStateMachine 13 with _ -> res ) 23

module ResumableCode_WhileLoopNoResumption =
    let makeStateMachine inputValue = 
        if __useResumableCode then
            (
                { new SyncMachine<int>() with 
                    [<ResumableCode>]
                    member _.Step ()  =  
                       // TEST: resumable code may contain while loops (here with no resumption point in the loop)
                       let mutable count = 0
                       while count < 10 do 
                           count <- count + 1
                       inputValue + 12 + count }).Start()
        else
            failwith "should have been compiled to resumable code, no interpretation available"

    check "ResumableCode_WhileLoopNoResumption" (makeStateMachine 13) 35

module ResumableCode_WhileLoopWithResumption =
    let makeStateMachine inputValue = 
        if __useResumableCode then
            (
                { new SyncMachine<int>() with 
                    [<ResumableCode>]
                    member _.Step ()  =  
                     if __useResumableCode then
                       // TEST: resumable code may contain while loops (here with a resumption point in the loop)
                       __resumeAt 1 // this goes straight to label 1
                       let mutable count = 0 // note, this initialization doesn't actually happen since we go straight to label 1. However this is the zero-init value.
                       let mutable res = 0 // note, this initialization doesn't actually happen since we go straight to label 1. However this is the zero-init value.
                       while count < 10 do 
                           // Specify a resumption point
                           match __resumableEntry() with
                           | Some contID ->
                                // This is the suspension path. It is executed in this test for subsequent iterations in this loop
                                count <- count + 1
                                res <- res + 20 // gets executed 9 times
                           | None -> 
                                // This is the resumption path
                                // Label 1 goes here
                                count <- count + 1
                                res <- res + 10 // gets executed once
                       // res ends up as 190
                       inputValue + res 
                     else failwith "dyanmic" }).Start()
        else
            failwith "should have been compiled to resumable code, no interpretation available"

    check "ResumableCode_WhileLoopWithResumption" (makeStateMachine 13) 203


module ResumableCode_ForLoopNoResumption =
    let makeStateMachine inputValue = 
        if __useResumableCode then
            (
                { new SyncMachine<int>() with 
                    [<ResumableCode>]
                    member _.Step ()  =  
                       // TEST: resumable code may contain for loops (here with no resumption point in the loop)
                       let mutable count = 0
                       for i = 0 to 9 do 
                           count <- count + 1
                       inputValue + 12 + count }).Start()
        else
            failwith "should have been compiled to resumable code, no interpretation available"

    check "ResumableCode_ForLoopNoResumption" (makeStateMachine 13) 35

#if NEGATIVE
// For loops may not contain resumption points.  You have to code it as a while loop.
module ResumableCode_ForLoopWithResumption =
    let makeStateMachine inputValue = 
        if __useResumableCode then
            (
                { new SyncMachine<int>() with 
                    [<ResumableCode>]
                    member _.Step ()  =  
                       // TEST: resumable code may contain while loops (here with a resumption point in the loop)
                       __resumeAt 1 // this goes straight to label 1
                       
                       let mutable res = 0 // note, this initialization doesn't actually happen since we go straight to label 1. However this is the zero-init value.
                       // note, in this test the loop variable initialization to 0 doesn't happen since we go straight to label 1. However the 'count' storage is
                       // zero-initialized so that's ok
                       for count in 0 .. 9 do 
                           // Specify a resumption point
                           match __resumableEntry() with
                           | Some contID ->
                               // This is the suspension path. It is executed in this test for subsequent iterations in this loop
                               res <- res + 20 // gets executed 9 times
                           | None -> 
                               // This is the resumption path
                               // Label 1 goes here
                               res <- res + 10 // gets executed once
                       // res ends up as 190
                       inputValue + res }).Start()
        else
            failwith "should have been compiled to resumable code, no interpretation available"

    check "ResumableCode_ForLoopWithResumption" (makeStateMachine 13) 203
#endif


module ResumableCode_ResumeAtIntoConditional =
    let makeStateMachine y = 
        if __useResumableCode then
            (
                { new SyncMachine<int>() with 
                    [<ResumableCode>]
                    member _.Step ()  =  
                      if __useResumableCode then
                        // TEST: resumable code may have __resumeAt at the start of the code
                        __resumeAt 1
                        failwith "fail"
                        if y > 10 then 
                            match __resumableEntry() with
                            | Some contID ->
                                // This is the suspension path. It is not executed in this test as we go straight to label 1
                                failwith "fail"
                            | None -> 
                                // This is the resumption path
                                // Label 1 goes here
                                20+y  // this is the answer
                        else
                            30 + y 
                      else failwith "dynamic" }).Start()
        else
            failwith "should have been compiled to resumable code, no interpretation available"
    check "vewowevewoi" (makeStateMachine 13) 33


module ResumableCode_Explicit1 =
    let inline f1 () : [<ResumableCode>] (unit -> int) =
        fun () -> 
             printfn "hello"
             1

// The main tests for tasks are in tests\FSharp.Core.UnitTests\FSharp.Core\Microsoft.FSharp.Control\Tasks.fs
module TaskCompilationSmokeTests =
    open System.Threading.Tasks
    let test1() = 

        task {
            return 1
         }

    let test2() = 

        task {
            let! v1 = Task.FromResult 1
            return 1
         }

    let syncTask() = Task.FromResult 100

    let tenBindSync_Task() =
        task {
            let! res1 = syncTask()
            let! res2 = syncTask()
            let! res3 = syncTask()
            let! res4 = syncTask()
            let! res5 = syncTask()
            let! res6 = syncTask()
            let! res7 = syncTask()
            let! res8 = syncTask()
            let! res9 = syncTask()
            let! res10 = syncTask()
            return res1 + res2 + res3 + res4 + res5 + res6 + res7 + res8 + res9 + res10
         }

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

