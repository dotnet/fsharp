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

let test s b =
    stderr.Write(s:string)
    if b then stderr.WriteLine " OK" else report_failure s
    stderr.WriteLine "" 

let check s v1 v2 = 
   stderr.Write(s:string);  
   if (v1 = v2) then 
       stderr.WriteLine " OK" 
       stderr.WriteLine "" 
   else
       eprintf " FAILED: got %A, expected %A" v1 v2 
       stderr.WriteLine "" 
       report_failure s

[<AbstractClass>]
type SyncMachine<'T>() =

    [<DefaultValue(false)>]
    val mutable PC : int

    [<DefaultValue(false)>]
    val mutable Result : 'T

    abstract Step : unit -> 'T

    member this.Start() = this.Step()

[<Struct; NoEquality; NoComparison>]
type SyncMachineStruct<'T> =

    [<DefaultValue(false)>]
    val mutable PC : int

    [<DefaultValue(false)>]
    val mutable Result : 'T

    member inline sm.GetAddress() =
        let addr = &&sm.PC
        Microsoft.FSharp.NativeInterop.NativePtr.toNativeInt addr

    // MoveNext without boxing
    static member inline MoveNext(sm: byref<'Machine> when 'Machine :> IAsyncStateMachine) = sm.MoveNext()

    interface IAsyncStateMachine with 
        member sm.MoveNext() = failwith "no dynamic impl"
        member sm.SetStateMachine(state: IAsyncStateMachine) = failwith "no dynamic impl"

module ``Check resumable ref state machine can contain outer if`` =

    let makeStateMachine() = 
        if __useResumableCode then
            (
                { new SyncMachine<int>() with 
                    [<ResumableCode>]
                    member _.Step ()  = 
                        if __useResumableCode then
                            1 // we expect this result for successful resumable code compilation
                        else
                            0xdeadbeef // if we get this result it means we've failed to compile as resumable code
                }
            ).Start()
        else
            0xdeadbeef // if we get this result it means we've failed to compile as resumable code
    check "ResumableObject_OuterConditional" (makeStateMachine()) 1

module ``Check simple ref state machine`` =
    let makeStateMachine()  = 
        { new SyncMachine<int>() with 
            [<ResumableCode>]
            member _.Step ()  =
                // TEST: resumable code may contain simple expressions
                if __useResumableCode then
                    1 // we expect this result for successful resumable code compilation
                else
                    0xdeadbeef // if we get this result it means we've failed to compile as resumable code
        }.Start()
    check "ResumableObject_SimpleImmediateExpression" (makeStateMachine()) 1

module ``Check resumable code can capture variables`` =
    let makeStateMachine x = 
        { new SyncMachine<int>() with 
            [<ResumableCode>]
            member _.Step ()  =
                // TEST: resumable code may contain free variables
                if __useResumableCode then
                    1 + x // we expect this result for successful resumable code compilation
                else
                    0xdeadbeef // if we get this result it means we've failed to compile as resumable code
        }.Start()
    let sm3 = makeStateMachine 3
    check "vwervwkhevh" (makeStateMachine 3) 4

module ``Check resumable code doesn't need a Start invocation`` =
    let makeStateMachine x = 
        { new SyncMachine<int>() with 
            [<ResumableCode>]
            member _.Step ()  =
                // TEST: resumable code may contain free variables
                if __useResumableCode then
                    1 + x // we expect this result for successful resumable code compilation
                else
                    0xdeadbeef // if we get this result it means we've failed to compile as resumable code
                }
    let sm3 = makeStateMachine 3
    check "vwvwekwevl" (sm3.Start()) 4

module ``Check resumable code may be preceeded by value definitions`` =
    let makeStateMachine x = 
        // TEST: resumable code may be just after a non-function 'let'
        let rnd = System.Random().Next(4)
        { new SyncMachine<int>() with 
            [<ResumableCode>]
            member _.Step ()  =  
                if __useResumableCode then
                    x + rnd - rnd // we expect this result for successful resumable code compilation
                else
                    0xdeadbeef // if we get this result it means we've failed to compile as resumable code
                       
        }.Start()
    check "cvwvweewvhjkv" (makeStateMachine 4) 4

module ``Check resumable code may contain local function definitions`` =
    let makeStateMachine y = 
        { new SyncMachine<int>() with 
            [<ResumableCode>]
            member _.Step ()  =  
                if __useResumableCode then
                    // TEST: resumable object may declare functions
                    let someFunction x = x + 1
                    // TEST: resumable code may invoke functions
                    someFunction y
                else
                    0xdeadbeef // if we get this result it means we've failed to compile as resumable code
        }.Start()
    check "vewwevbrklbre" (makeStateMachine 3) 4

module ``Check resumable code may be preceeded by function definitions`` =
    let makeStateMachine y = 
        // TEST: resumable object may declare functions
        let someFunction1 x = x + 1
        let someFunction2 x = someFunction1 x + 2
        { new SyncMachine<int>() with 
            [<ResumableCode>]
            member _.Step ()  =  
                if __useResumableCode then
                    someFunction2 y
                else
                    0xdeadbeef // if we get this result it means we've failed to compile as resumable code
        }.Start()
    check "vwekvewkhvew" (makeStateMachine 3) 6

module ``Check resumable code may contain let statements without resumption points`` =
    let makeStateMachine x = 
        { new SyncMachine<int>() with 
            [<ResumableCode>]
            member _.Step ()  =  
                // TEST: resumable code may let statements
                if __useResumableCode then
                    let y = 1 - x
                    if x > 3 then 1 + x else y
                else
                    0xdeadbeef // if we get this result it means we've failed to compile as resumable code
        }.Start()
    check "vwelvewl" (makeStateMachine 3) -2

module ``Check resumable code may contain sequential statements without resumption points`` =
    let makeStateMachine y = 
        { new SyncMachine<int>() with 
            [<ResumableCode>]
            member _.Step ()  =  
                if __useResumableCode then
                        // TEST: resumable code may sequentially do one thing then another
                        printfn "step1" 
                        y + 2
                else
                    0xdeadbeef // if we get this result it means we've failed to compile as resumable code
        }.Start()
    check "ewvkwekvwevwvek" (makeStateMachine 3) 5

module ``Check resuming at start of method`` =
    let makeStateMachine y = 
        // TEST: resumable object may declare functions
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
                else 
                    failwith "no" }.Start()
    check "vwrvwlkelwe" (makeStateMachine 3) 13

module ``Check resuming at drop through`` =
    let makeStateMachine y = 
        // TEST: resumable object may declare functions
        { new SyncMachine<int>() with 
            [<ResumableCode>]
            member _.Step ()  =  
                if __useResumableCode then
                    // TEST: resumable code may have __resumeAt at the start of the code
                    __resumeAt 100
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
                else 
                    0xdeadbeef // if we get this result it means we've failed to compile as resumable code
        }.Start()
    check "cvqelekckeq" (makeStateMachine 3) 13

module ``Check resuming at a resumption point`` =
    let makeStateMachine y = 
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
                    0xdeadbeef // if we get this result it means we've failed to compile as resumable code
        }.Start()
    check "vwlvkjlewjwevj" (makeStateMachine 3) 23

module ``Check resumable code may contain conditionals`` =
    let makeStateMachine x = 
        { new SyncMachine<int>() with 
            [<ResumableCode>]
            member _.Step ()  =  
                if __useResumableCode then
                    // TEST: resumable code may contain conditionals
                    if x > 3 then 1 + x else 1 - x
                else
                    0xdeadbeef // if we get this result it means we've failed to compile as resumable code
                      
        }.Start()
    check "vwwevwejvlj" (makeStateMachine 3) -2


module ``Check resumable code may contain try-with without resumption points`` =
    let makeStateMachine inputValue = 
        { new SyncMachine<int>() with 
            [<ResumableCode>]
            member sm.Step ()  =  
                if __useResumableCode then
                    try 
                        inputValue + 10
                    with e -> 12
                else
                    0xdeadbeef // if we get this result it means we've failed to compile as resumable code
                       
            }.Start()

    check "fvwewejkhkwe" (makeStateMachine 13) 23

module ``Check resuming at resumption point in try block`` =
    let makeStateMachine inputValue = 
        { new SyncMachine<int>() with 
            [<ResumableCode>]
            member _.Step ()  =  
                if __useResumableCode then
                    __resumeAt 1 // this goes straight to label 1
                    try 
                        failwith "unreachable #1"
                        match __resumableEntry() with
                        | Some contID ->
                            // This is the suspension path. It is not executed in this test because we jump straight to label 1
                            check "vwehewvhk" contID 1
                            failwith "unreachable #2"
                        | None -> 
                            // This is the resumption path
                            // Label 1 goes here
                            20+inputValue // this is the result
                    with e -> 
                        12 
                else
                    0xdeadbeef // if we get this result it means we've failed to compile as resumable code
                           
           }.Start()

    check "ResumableCode_TryWithResumption" (makeStateMachine 13) 33


module ``Check exception thrown in try block after resumption is caught`` =
    let makeStateMachine inputValue = 
        { new SyncMachine<int>() with 
            [<ResumableCode>]
            member _.Step ()  =  
                if __useResumableCode then
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
                    0xdeadbeef // if we get this result it means we've failed to compile as resumable code
        }.Start()

    check "ResumableCode_TryWithResumptionException" (makeStateMachine 13) 25

module ``Check exception thrown in try block with no resumption points is caught`` =
    let makeStateMachine inputValue = 
        { new SyncMachine<int>() with 
            [<ResumableCode>]
            member _.Step ()  =  
                if __useResumableCode then
                    // TEST: resumable code may contain try-with statements with no resumption
                    try 
                        failwith "fail"
                    with e -> inputValue + 12 
                else 0xdeadbeef // if we get this result it means we've failed to compile as resumable code
        }.Start()

    check "ResumableCode_TryWithExceptionNoResumption" (makeStateMachine 13) 25

module ``Check resumable code may contain try-finally without resumption points`` =
    let makeStateMachine inputValue = 
        { new SyncMachine<int>() with 
            [<ResumableCode>]
            member _.Step ()  =  
                if __useResumableCode then
                    let mutable res = 0
                    try 
                        res <- inputValue + 10
                    finally 
                        res <- inputValue + 40 
                    res 
                else 0xdeadbeef // if we get this result it means we've failed to compile as resumable code
        }.Start()

    check "ResumableCode_TryFinallyNoResumption" (makeStateMachine 13) 53

// try-finally may **not** contain resumption points.  This is because it's 
// very difficult to codegen them in IL. Instead you have to code try/finally logic as shown below.
// See also tasks.fs.
module ``Check can simulate try-finally via try-with`` =
    let makeStateMachine inputValue = 
        { new SyncMachine<int>() with 
            [<ResumableCode>]
            member _.Step ()  =  
                if __useResumableCode then
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
                else 
                    0xdeadbeef // if we get this result it means we've failed to compile as resumable code
           }.Start()

    check "vwevewewlvjve" (makeStateMachine 13) 43

module ``Check resumable code may contain try-finally without resumption points and raising exception`` =
    let mutable res = 0
    let makeStateMachine inputValue = 
        { new SyncMachine<int>() with 
            [<ResumableCode>]
            member _.Step ()  =  
                if __useResumableCode then
                   // TEST: resumable code may contain try-finally statements with no resumption
                    try 
                        failwith "fail"
                    finally 
                        res <- inputValue + 10 
                else
                    0xdeadbeef // if we get this result it means we've failed to compile as resumable code
        }.Start()

    check "cvwekjwejkl" (try makeStateMachine 13 with _ -> res ) 23

module ``Check resumable code may contain while loop without resumption`` =
    let makeStateMachine inputValue = 
        { new SyncMachine<int>() with 
            [<ResumableCode>]
            member _.Step ()  =  
                if __useResumableCode then
                    // TEST: resumable code may contain while loops (here with no resumption point in the loop)
                    let mutable count = 0
                    while count < 10 do 
                        count <- count + 1
                    inputValue + 12 + count 
                else
                    0xdeadbeef // if we get this result it means we've failed to compile as resumable code
            }.Start()

    check "fvewhjkvekwhk" (makeStateMachine 13) 35

module ``Check resumable code may contain while loop with resumption`` =
    let makeStateMachine inputValue = 
        { new SyncMachine<int>() with 
            [<ResumableCode>]
            member _.Step ()  =  
                if __useResumableCode then
                    __resumeAt 1 // this goes straight to label 1
                    let mutable count = 0 // note, this initialization doesn't actually happen since we go straight to label 1. However this is the zero-init value.
                    let mutable res = 0 // note, this initialization doesn't actually happen since we go straight to label 1. However this is the zero-init value.
                    while count < 10 do 
                        // Specify a resumption point in the loop
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
                else
                    0xdeadbeef // if we get this result it means we've failed to compile as resumable code
            }.Start()

    check "wejlcewjllkjwce" (makeStateMachine 13) 203


module ``Check resumable code may contain integer for loop without resumption`` =
    let makeStateMachine inputValue = 
        { new SyncMachine<int>() with 
            [<ResumableCode>]
            member _.Step ()  =  
                if __useResumableCode then
                    let mutable count = 0
                    for i = 0 to 9 do 
                        count <- count + 1
                    inputValue + 12 + count 
                else
                    0xdeadbeef // if we get this result it means we've failed to compile as resumable code
            }.Start()

    check "celjkcwljeljwecl" (makeStateMachine 13) 35

module ``Check resumable code can contain a conditional with a resumption point`` =
    let makeStateMachine y = 
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
                else
                    0xdeadbeef // if we get this result it means we've failed to compile as resumable code
            }.Start()
    check "vewowevewoi" (makeStateMachine 13) 33

module ``Check resumable code can contain a conditional with a resumption point in else branch`` =
    let makeStateMachine y = 
        { new SyncMachine<int>() with 
            [<ResumableCode>]
            member _.Step ()  =  
                if __useResumableCode then
                    // TEST: resumable code may have __resumeAt at the start of the code
                    __resumeAt 1
                    failwith "fail"
                    if y <= 10 then 
                        30 + y 
                    else
                        match __resumableEntry() with
                        | Some contID ->
                            // This is the suspension path. It is not executed in this test as we go straight to label 1
                            failwith "fail"
                        | None -> 
                            // This is the resumption path
                            // Label 1 goes here
                            20+y  // this is the answer
                else
                    0xdeadbeef // if we get this result it means we've failed to compile as resumable code
            }.Start()
    check "vewowevewoi" (makeStateMachine 13) 33


module ``Check delegate accepting byref parameter doesn't copy struct`` =
    [<Struct; NoEquality; NoComparison>]
    type SyncStruct =

        [<DefaultValue(false)>]
        val mutable PC : int

        [<DefaultValue(false)>]
        val mutable Result : int

        [<DefaultValue(false)>]
        val mutable someFunction: CodeSpec

        member inline sm.GetAddress() =
            let addr = &&sm.PC
            Microsoft.FSharp.NativeInterop.NativePtr.toNativeInt addr

        // MoveNext without boxing
        static member inline MoveNext(sm: byref<'T> when 'T :> IAsyncStateMachine) = sm.MoveNext()
        interface IAsyncStateMachine with 
            member sm.MoveNext() = 
                    sm.someFunction.Invoke(&sm)
            member sm.SetStateMachine(state: IAsyncStateMachine) = failwith "no dynamic impl"

    and CodeSpec = delegate of byref<SyncStruct> -> unit
    let runDelegateCode () = 
        
        let mutable sm = Unchecked.defaultof<SyncStruct>
        sm.someFunction <-
            CodeSpec(fun sm -> 
                
                printfn "before set, sm.GetAddress() = %d, sm.Result = %d" (sm.GetAddress()) sm.Result
                sm.Result <- 420
                printfn "after set, sm.GetAddress() = %d, sm.Result = %d" (sm.GetAddress()) sm.Result
            )

        printfn "before invoke, sm.GetAddress() = %d, sm.Result = %d" (sm.GetAddress()) sm.Result
        SyncStruct.MoveNext(&sm)

        printfn "after invoke, sm.GetAddress() = %d, sm.Result = %d" (sm.GetAddress()) sm.Result
        sm.Result

    check "DelegateCode_StructFunction0" (runDelegateCode()) 420

module ``Check struct machine with explicit ResumableCode`` =
    let makeStateMachine inputValue = 
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

    check "ResumableCode_Struct" (makeStateMachine 400) 420


module ``Check struct machine where code is given by a ResumableCode delegate implementation`` =
    [<ResumableCode>]
    type CodeSpec = delegate of byref<SyncMachineStruct<int>> -> unit

    let makeStateMachine () = 
        
        // Note, this delegate is *not* labelled 'inline'.  However the state machine compilation knows
        // how to reduce this delegate
        let someFunction = 
            CodeSpec(fun sm -> 
                
                printfn "before set, sm.GetAddress() = %d, sm.Result = %d" (sm.GetAddress()) sm.Result
                // check code is flattened
                check "vewohevroerhn1" (System.Reflection.MethodBase.GetCurrentMethod().Name) "MoveNext"
                sm.Result <- 420
                printfn "after set, sm.GetAddress() = %d, sm.Result = %d" (sm.GetAddress()) sm.Result
            )

        __structStateMachine<SyncMachineStruct<int>, int>
            (MoveNextMethod<SyncMachineStruct<int>>(fun sm -> 
                printfn "before invoke, sm.GetAddress() = %d, sm.Result = %d" (sm.GetAddress()) sm.Result
                someFunction.Invoke(&sm)))

            // SetStateMachine
            (SetMachineStateMethod<_>(fun sm state -> 
                ()))

            // Start
            (AfterMethod<_,_>(fun sm -> 
                SyncMachineStruct<_>.MoveNext(&sm)
                printfn "after invoke, sm.GetAddress() = %d, sm.Result = %d" (sm.GetAddress()) sm.Result
                sm.Result))

    check "ResumableCode_StructMachine_CodeSpec_Reduction" (makeStateMachine()) 420


module ``Check ref state machine where code is given by a ResumableCode delegate implementation`` =
    [<ResumableCode>]
    type CodeSpec = delegate of SyncMachine<int> -> unit

    let makeStateMachine () = 

        // Note, this delegate is *not* labelled 'inline'.  However the state machine compilation knows
        // how to reduce this delegate
        let someFunction = 
            CodeSpec(fun sm -> 
                // check code is flattened
                check "vewohevbvwern1" (System.Reflection.MethodBase.GetCurrentMethod().Name) "Step"
                sm.Result <- 420
            )
        { new SyncMachine<int>() with 
            [<ResumableCode>]
            member sm.Step ()  =  
                // TEST: resumable code may contain a beta-reduction of a delegate
                if __useResumableCode then
                    someFunction.Invoke(sm)
                    sm.Result
                else
                    100
        }.Start()

    check "ResumableCode_RefMachine_CodeSpec_Reduction" (makeStateMachine ()) 420


module ``Check code is not flattened because missing ResumableCode attribute`` =
    //[<ResumableCode>]
    type CodeSpec = delegate of SyncMachine<int> -> unit
    let makeStateMachine x = 
        { new SyncMachine<int>() with 
            [<ResumableCode>]
            member sm.Step ()  =  
                (new CodeSpec(fun sm -> 
                    // Check the code has NOT been flattened since '[<ResumableCode>]' wasn't specified on 'CodeSpec'
                    check "vewohevroerhn1" (System.Reflection.MethodBase.GetCurrentMethod().Name) "Invoke"
                    sm.Result <- sm.Result + 3 + x)).Invoke sm
                (new CodeSpec(fun sm -> 
                    check "vewohevroerhn2" (System.Reflection.MethodBase.GetCurrentMethod().Name) "Invoke"
                    sm.Result <- sm.Result + 4)).Invoke sm
                sm.Result
        }.Start()

    check "fvewleewcljwecl" (makeStateMachine 3) 10

module ``Check code is flattened because ResumableCode attribute`` =
    [<ResumableCode>]
    type CodeSpec = delegate of SyncMachine<int> -> unit
    let makeStateMachine x = 
        { new SyncMachine<int>() with 
            [<ResumableCode>]
            member sm.Step ()  =  
                // TEST: resumable code may contain a beta-reduction of a ResumableCode delegate
                (new CodeSpec(fun sm -> 
                    // Check the code HAS been flattened since '[<ResumableCode>]' was specified on 'CodeSpec'
                    check "vewohevroerhn12" (System.Reflection.MethodBase.GetCurrentMethod().Name) "Step"
                    sm.Result <- sm.Result + 3 + x)).Invoke sm

                // TEST: resumable code may contain a beta-reduction of a ResumableCode delegate
                (new CodeSpec(fun sm -> 
                    check "vewohevroerhn22" (System.Reflection.MethodBase.GetCurrentMethod().Name) "Step"
                    sm.Result <- sm.Result + 4)).Invoke sm
                sm.Result
        }.Start()

    check "evcwewcjlkwejfvlj" (makeStateMachine 3) 10


open Microsoft.FSharp.Control
open System.Threading.Tasks

let inline checkStateMachine nm = 
    check nm (System.Reflection.MethodBase.GetCurrentMethod().Name) "MoveNext"

// The main tests for tasks are in tests\FSharp.Core.UnitTests\FSharp.Core\Microsoft.FSharp.Control\Tasks.fs
module ``Check simple task compiles to state machine`` =

    let test1() = 

        task {
            checkStateMachine "vevroerhn11" 
            return 1
         }

    test1().Wait()

module ``Check simple task with bind compiles to state machine`` =
    let test2() = 

        task {
            checkStateMachine "vevroerhn12" 
            let! v1 = Task.FromResult 1
            checkStateMachine "vevroerhn13" 
            return 1
         }

    test2().Wait()


module ``Check task with multiple bind doesn't cause code explosion from inlining and compiles to state machine`` =

    let syncTask() = Task.FromResult 100

    let tenBindSync_Task() =
        task {
            let! res1 = syncTask()
            checkStateMachine "vevroerhn121" 
            let! res2 = syncTask()
            checkStateMachine "vevroerhn122" 
            let! res3 = syncTask()
            checkStateMachine "vevroerhn123" 
            let! res4 = syncTask()
            checkStateMachine "vevroerhn124" 
            let! res5 = syncTask()
            checkStateMachine "vevroerhn125" 
            let! res6 = syncTask()
            checkStateMachine "vevroerhn126" 
            let! res7 = syncTask()
            checkStateMachine "vevroerhn127" 
            let! res8 = syncTask()
            checkStateMachine "vevroerhn128" 
            let! res9 = syncTask()
            checkStateMachine "vevroerhn129" 
            let! res10 = syncTask()
            checkStateMachine "vevroerhn120" 
            return res1 + res2 + res3 + res4 + res5 + res6 + res7 + res8 + res9 + res10
         }

    tenBindSync_Task().Wait()

module ``Check task with try with compiles to state machine`` =
    let t67() =
        task {
            checkStateMachine "vevroerhn180" 
            try
                checkStateMachine "vevroerhn181" 
                do! Task.Delay(0)
                checkStateMachine "vevroerhn182" 
            with
            | :? System.ArgumentException -> 
                ()
            | _ -> 
                ()
        }

    t67().Wait()

module ``Check task with try with and incomplete match compiles to state machine`` =
    let t68() =
        task {
            try
                checkStateMachine "vevroerhn190" 
                do! Task.Delay(0)
                checkStateMachine "vevroerhn191" 
            with
            | :? System.ArgumentException -> 
                ()
        }

    t68().Wait()

module ``Check task with while loop with resumption points compiles to state machine`` =
    let t68() : Task<int> =
        task {
            checkStateMachine "vevroerhn200" 
            let mutable i = 0
            while i < 1 do
                checkStateMachine "vevroerhn201" 
                i <- i + 1
                do! Task.Yield()
                ()
            checkStateMachine "vevroerhn202" 
            return i
        }

    t68().Wait()

module ``Check task with try finally compiles to state machine`` =
    let t68() =
        task {
            let mutable ran = false
            try
                checkStateMachine "vevroerhn210" 
                do! Task.Delay(100)
                checkStateMachine "vevroerhn211" 

            finally
                ran <- true
            return ran
        }

    t68().Wait()

module ``Check task with use compiles to state machine`` =
    let t68() =
                task {
                    let mutable disposed = false
                    use d = { new System.IDisposable with member __.Dispose() = disposed <- true }
                    checkStateMachine "vevroerhn221" 
                    do! Task.Delay(100)
                    checkStateMachine "vevroerhn221" 
                }

    t68().Wait()

module ``Check nested task compiles to state machine`` =
    let t68() =
            task {
                let mutable n = 0
                checkStateMachine "vevroerhn230" 
                while n < 10 do
                    let! _ =
                        task {
                            do! Task.Yield()
                            checkStateMachine "vevroerhn231" 
                            let! _ = Task.FromResult(0)
                            n <- n + 1
                        }
                    n <- n + 1
            }

    t68().Wait()


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

