module Tests_Tasks

#nowarn "57"
open FSharp.Core.CompilerServices
open FSharp.Core.CompilerServices.StateMachineHelpers
open System.Runtime.CompilerServices

//vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv
// No warnings expected in the declarations

module ``No warning for resumable code that is just a simple expression`` =
    let inline f1 () : [<ResumableCode>] (unit -> int) =
        fun () -> 
             //printfn "hello"
             1 // check we don't get a warning here

module ``No warning for resumable code that is just a sequential expression`` =
    let inline f1 () : [<ResumableCode>] (unit -> int) =
        fun () -> 
             printfn "hello"
             1 // check we don't get a warning here

module ``No warning for protected use of __resumeAt`` =
    let inline f1 () : [<ResumableCode>] (unit -> int) =
        fun () -> 
             if __useResumableCode then
                __resumeAt 0 // we don't get a warning here
                2
             else
                 1 

module ``No warning for protected use of __resumableEntry`` =
    let inline f1 () : [<ResumableCode>] (unit -> int) =
        fun () -> 
             if __useResumableCode then
                 match __resumableEntry() with 
                 | Some contID -> 1
                 | None -> 2
             else
                 3

    let inline f2 () : [<ResumableCode>] (unit -> int) =
        f1()

//^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
// No warnings expected in code before this line

[<AbstractClass>]
type SyncMachine<'T>() =

    [<DefaultValue(false)>]
    val mutable PC : int

    abstract Step : unit -> 'T

    member this.Start() = this.Step()

[<Struct; NoEquality; NoComparison>]
type SyncMachineStruct1<'TOverall> =

    [<DefaultValue(false)>]
    val mutable PC : int

    // MoveNext without being inlined
    static member MoveNext(sm: byref<'T> when 'T :> IAsyncStateMachine) = sm.MoveNext()
    interface IAsyncStateMachine with 
        member sm.MoveNext() = failwith "no dynamic impl"
        member sm.SetStateMachine(state: IAsyncStateMachine) = failwith "no dynamic impl"

[<Struct>]
type SyncMachineStruct2<'TOverall> =

    [<DefaultValue(false)>]
    val mutable PC : int

    interface IAsyncStateMachine with 
        member sm.MoveNext() = failwith "no dynamic impl"
        member sm.SetStateMachine(state: IAsyncStateMachine) = failwith "no dynamic impl"

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

module ``Warning because resumable parameter not named correctly`` =
    let inline f ([<ResumableCode>] code : unit -> unit) =
        ()

module ``Warning on unprotected __resumeAt`` =
    let inline f1 () : [<ResumableCode>] (unit -> int) =
        fun () -> 
             __resumeAt 0 // we don't get a warning here
             1 

module ``Warning because for loop doesn't supported resumable code`` =
    let inline f1 () : [<ResumableCode>] (unit -> int) =
        fun () -> 
             if __useResumableCode then
                 for i in 0 .. 1 do
                    __resumeAt 0 // we expect an error here - this is not valid resumable code
             1


module ``Error because function with resumable parameter not inlined`` =
    let f ([<ResumableCode>] __expand_code : unit -> unit) =
        ()

module ``Error because unprotected __resumableEntry`` =
    let inline f1 () : [<ResumableCode>] (unit -> int) =
        fun () -> 
                match __resumableEntry() with 
                | Some contID -> 1
                | None -> 2

    let inline f2 () : [<ResumableCode>] (unit -> int) =
        f1()

module ``Error for struct state machine with non-inline method`` =
    let makeStateMachine inputValue = 
        if __useResumableCode then
            __structStateMachine<SyncMachineStruct1<int>, int>
                (MoveNextMethod<_>(fun sm -> ()))
                (SetMachineStateMethod<_>(fun sm state -> ()))
                (AfterMethod<_,_>(fun sm -> 1))
        else
            failwith "should have been compiled to resumable code, no interpretation available"


module ``Error for struct state machine without NoEquality, NoComparison`` =
    let makeStateMachine inputValue = 
        if __useResumableCode then
            __structStateMachine<SyncMachineStruct2<int>, int>
                (MoveNextMethod<_>(fun sm -> ()))
                (SetMachineStateMethod<_>(fun sm state -> ()))
                (AfterMethod<_,_>(fun sm -> 1))
        else
            failwith "should have been compiled to resumable code, no interpretation available"

module ``let bound __expand can't use resumable code constructs`` =
    let makeStateMachine inputValue = 
            (
                { new SyncMachine<int>() with 
                    [<ResumableCode>]
                    member sm.Step ()  =  
                      if __useResumableCode then
                       __resumeAt sm.PC
                       // TEST: a resumable object may contain a macro defining beta-reducible code with a resumption point
                       let __expand_code = (fun () -> 
                            // Specify a resumption point
                            match __resumableEntry() with
                            | Some contID ->
                                // This is the suspension path. It is not executed in this test because we jump straight to label 1
                                __resumeAt contID
                            | None -> 
                                // This is the resumption path
                                // Label 1 goes here
                                20+inputValue)
                       __expand_code () 
                      else failwith "dynamic"
               }
            ).Start()

