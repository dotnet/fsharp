module Tests_Tasks

#nowarn "57"
open FSharp.Core.CompilerServices
open FSharp.Core.CompilerServices.StateMachineHelpers
open System.Runtime.CompilerServices

//vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv
// No warnings expected in the declarations

module ``No warning for resumable code that is just a simple expression`` =
    [<ResumableCode>]
    type CodeSpec = delegate of unit -> int
    let inline f1 ()  = CodeSpec(fun () ->  1 )

module ``No warning for resumable code that is just a sequential expression`` =
    [<ResumableCode>]
    type CodeSpec = delegate of unit -> int
    let inline f1 () =
        CodeSpec(fun () -> 
             printfn "hello"
             1)

module ``No warning for protected use of __resumeAt`` =
    [<ResumableCode>]
    type CodeSpec = delegate of unit -> int
    let inline f1 () =
        CodeSpec(fun () -> 
             if __useResumableCode then
                __resumeAt 0 // we don't get a warning here
                2
             else
                 1 )

module ``No warning for protected use of __resumableEntry`` =
    [<ResumableCode>]
    type CodeSpec = delegate of unit -> int
    let inline f1 () =
        CodeSpec(fun () -> 
             if __useResumableCode then
                 match __resumableEntry() with 
                 | Some contID -> 1
                 | None -> 2
             else
                 3)

[<AbstractClass>]
type SyncMachine<'T>() =

    [<DefaultValue(false)>]
    val mutable PC : int

    abstract Step : unit -> 'T

    member this.Start() = this.Step()

[<Struct; NoEquality; NoComparison>]
type SyncMachineStructWithNonInlineMethod<'TOverall> =

    [<DefaultValue(false)>]
    val mutable PC : int

    // MoveNext without being inlined
    static member MoveNext(sm: byref<'T> when 'T :> IAsyncStateMachine) = sm.MoveNext()
    interface IAsyncStateMachine with 
        member sm.MoveNext() = failwith "no dynamic impl"
        member sm.SetStateMachine(state: IAsyncStateMachine) = failwith "no dynamic impl"

[<Struct>]
type SyncMachineStructWithOtherInterfaces<'TOverall> =

    [<DefaultValue(false)>]
    val mutable PC : int

    interface IAsyncStateMachine with 
        member sm.MoveNext() = failwith "no dynamic impl"
        member sm.SetStateMachine(state: IAsyncStateMachine) = failwith "no dynamic impl"

//^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
// No warnings expected in code before this line


// Resumable code may not have let rec statements
module ``Error on let rec in resumable code`` =
    let makeStateMachine inputValue = 
        { new SyncMachine<int>() with 
            [<ResumableCode>]
            member _.Step ()  =  
                if __useResumableCode then
                    let rec f x = if x > 0 then f (x-1) else inputValue + 13
                    f 10 + f 2
                else
                    failwith "should have been compiled to resumable code, no interpretation available"
        }.Start()

module ``Error on unprotected __resumeAt`` =
    let inline f1 () =
        __resumeAt 0 
        1 

module ``Warning on unprotected __resumeAt in resumable code`` =
    [<ResumableCode>]
    type CodeSpec = delegate of unit -> int
    let inline f1 () =
        CodeSpec(fun () -> __resumeAt 0)

module ``Warning because for loop doesn't supported resumable code`` =
    [<ResumableCode>]
    type CodeSpec = delegate of unit -> int
    let inline f1 () =
        CodeSpec(fun () -> 
            if __useResumableCode then
                 for i in 0 .. 1 do
                    __resumeAt 0 // we expect an error here - this is not valid resumable code
            1)


module ``Error because unprotected __resumableEntry`` =
    [<ResumableCode>]
    type CodeSpec = delegate of unit -> int
    let inline f1 () =
        CodeSpec(fun () -> 
                match __resumableEntry() with 
                | Some contID -> 1
                | None -> 2
        )

module ``Error for struct state machine with non-inline method`` =
    let makeStateMachine inputValue = 
        if __useResumableCode then
            __structStateMachine<SyncMachineStructWithNonInlineMethod<int>, int>
                (MoveNextMethod<_>(fun sm -> ()))
                (SetMachineStateMethod<_>(fun sm state -> ()))
                (AfterMethod<_,_>(fun sm -> 1))
        else
            failwith "should have been compiled to resumable code, no interpretation available"


module ``Error for struct state machine without NoEquality, NoComparison`` =
    let makeStateMachine inputValue = 
        if __useResumableCode then
            __structStateMachine<SyncMachineStructWithOtherInterfaces<int>, int>
                (MoveNextMethod<_>(fun sm -> ()))
                (SetMachineStateMethod<_>(fun sm state -> ()))
                (AfterMethod<_,_>(fun sm -> 1))
        else
            failwith "should have been compiled to resumable code, no interpretation available"

module ``let bound function can't use resumable code constructs`` =
    let makeStateMachine inputValue = 
        { new SyncMachine<int>() with 
            [<ResumableCode>]
            member sm.Step ()  =  
                if __useResumableCode then
                    __resumeAt sm.PC
                    // TEST: a resumable object may contain a macro defining beta-reducible code with a resumption point
                    let f () =
                        // Specify a resumption point
                        match __resumableEntry() with
                        | Some contID ->
                            // This is the suspension path. It is not executed in this test because we jump straight to label 1
                            __resumeAt contID
                        | None -> 
                            // This is the resumption path
                            // Label 1 goes here
                            20+inputValue
                    f() 
                else failwith "dynamic"
        }

module ``Check no resumption point in try-finally `` =
    let makeStateMachine inputValue = 
        { new SyncMachine<int>() with 
            [<ResumableCode>]
            member _.Step ()  =  
                if __useResumableCode then
                    __resumeAt 1 
                    let mutable res = inputValue
                    try 
                        // Attempt to specify a resumption point in the try-finally
                        match __resumableEntry() with
                        | Some contID ->
                            failwith "unreachable"
                            res <- 0
                        | None -> 
                            res <- res + 10
                    finally 
                        res <- res + 20 
                    res
               
                else
                    0xdeadbeef 
         }.Start()

module ``Check no resumption point in for loop`` =
    let makeStateMachine inputValue = 
        { new SyncMachine<int>() with 
            [<ResumableCode>]
            member _.Step ()  =  
                if __useResumableCode then
                    __resumeAt 1 
                    let mutable res = inputValue
                    let mutable count = 0
                    for i = 0 to 9 do 
                        // Attempt to specify a resumption point in the for loop. THis is not permitted
                        // and while loops should be used instead
                        match __resumableEntry() with
                        | Some contID ->
                            failwith "unreachable"
                            res <- 0
                        | None -> 
                            res <- res + 10
                    res
               
                else
                    0xdeadbeef 
         }.Start()

