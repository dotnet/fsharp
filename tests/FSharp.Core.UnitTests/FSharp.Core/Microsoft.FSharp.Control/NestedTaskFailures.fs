namespace FSharp.Core.UnitTests.Control.Tasks

// The tasks below fail state machine comilation.  This failure was causing subsequent problems in code generation.
// See https://github.com/dotnet/fsharp/issues/13404

#nowarn "3511"  // state machine not staticlly compilable - this is a separate issue, see  https://github.com/dotnet/fsharp/issues/13404

open System
open Microsoft.FSharp.Control
open Xunit

module NestedTasksFailingStateMachine =
    module Example1 =
        let transfers = [| Some 2,1 |]

        let FetchInternalTransfers (includeConfirmeds: int) =
            task {

                let! mapPrioritiesTransfers = 
                    task {
                        if includeConfirmeds > 1 then

                            transfers
                            |> Array.map(fun (loanid,c) -> loanid.Value, 4)
                            |> Array.map(fun (k,vs) -> k, 1)
                            |> Array.map(fun (id,c) -> c,true)
                            |> ignore

                    }

                return [| 1 |], 1

            }

    module Example2 =
        open System.Linq

        let ``get pending internal transfers`` nonAllowedPriority (loanIds:Guid[]) =
            task { return [||] }

        let FetchInternalTransfers (includeConfirmeds: bool) (transferStep: string) (inform: bool) (workflow: string) =
            task {
                let canReserve = true

                let! transfers =
                    task { // This is the only real async here
                        do! System.Threading.Tasks.Task.Delay 500
                        return [| // simulates data from external source
                            Some (Guid.NewGuid()),DateTime.Now,"3","4",5m,Some 6,Some "1",Some 71,Some "7",true,DateTime.Now;
                            Some (Guid.NewGuid()),DateTime.Now,"3","4",5m,Some 6,Some "1",Some 72,Some "7",true,DateTime.Now;
                            Some (Guid.NewGuid()),DateTime.Now,"3","4",5m,Some 6,Some "1",Some 73,Some "7",true,DateTime.Now;
                        |]
                    }

                let totalCount = transfers |> Array.length

                let checkIfTransfersPending notAllowedPriority =
                    task {
                        let transferIds = transfers |> Array.filter(fun (id,c,fa,ta,ts,ir,eb,o,r,me,rm) -> id.IsSome) |> Array.map(fun (id,c,fa,ta,ts,ir,eb,o,r,me,rm) -> id.Value) |> Array.distinct
                        let! pendingTransfers = ``get pending internal transfers`` notAllowedPriority transferIds
                        return
                            transfers
                            |> Array.map(fun (id,c,fa,ta,ts,ir,eb,o,r,me,rm) ->
                                c,fa,ta,ts,ir,eb, id.IsNone || (not (pendingTransfers.Contains id.Value)), r,me,rm
                            )
                    }

                let! mapPrioritiesTransfers =
                    task {
                        match transferStep with
                        | "All" ->

                            let minOrder =
                                transfers
                                |> Array.filter(fun (loanid,c,fa,ta,ts,ir,eb,o,r,me,rm) -> loanid.IsSome && o.IsSome)
                                |> Array.map(fun (loanid,c,fa,ta,ts,ir,eb,o,r,me,rm) -> loanid.Value, o.Value)
                                |> Array.groupBy(fun (loanid,_) -> loanid)
                                |> Array.map(fun (k,vs) -> k, vs |> Array.map(fun (_,o) -> o) |> Array.min)
                                |> Map.ofArray

                            let mappedTransfers =
                                transfers |> Array.map(fun (id,c,fa,ta,ts,ir,eb,o,r,me,rm) ->
                                    let isPrio = includeConfirmeds || o.IsNone || id.IsNone || minOrder.[id.Value] = o.Value
                                    c,fa,ta,ts,ir,eb, isPrio, r, me, rm
                                )

                            return mappedTransfers
                        | "Step1"
                        | "Postprocessing" ->
                            return
                                transfers |> Array.map(fun (id, c, fa, ta, ts, ir, eb, o, r, me, rm) ->
                                    c, fa, ta, ts, ir, eb, true, r, me, rm
                                )
                        | "Step2" ->
                            return! checkIfTransfersPending 1
                        | "Step3" ->
                            return! checkIfTransfersPending 2
                        | "Rebalancing" ->
                            return! checkIfTransfersPending 4
                        | _ -> return failwith ("Unknown internal transfer step: " + transferStep)
                    }

                return canReserve, mapPrioritiesTransfers, totalCount

            }

        let test = FetchInternalTransfers false "All" true "Bank2"
        System.Threading.Tasks.Task.WaitAll test
        let result = test.Result |> printfn "%A"

type NestedStateMachineTests() = 
    [<Fact>]
    member _.NestedStateMachineFailure1() =
        let test = NestedTasksFailingStateMachine.Example1.FetchInternalTransfers 2
        test.Result |> printfn "%A"

    [<Fact>]
    member _.NestedStateMachineFailure2() =
        let test = NestedTasksFailingStateMachine.Example2.FetchInternalTransfers false "All" true "Bank2"
        System.Threading.Tasks.Task.WaitAll test
        let (a, b, c) = test.Result
        if a <> true then failwith "failed - expected true"
        if b.Length <> 3 then failwith "failed - expected results of length 3"
        if c <> 3 then failwith "failed - expected 3"
