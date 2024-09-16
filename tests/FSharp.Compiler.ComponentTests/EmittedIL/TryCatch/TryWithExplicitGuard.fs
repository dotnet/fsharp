module ExplicitGuardCase

open System

let addWithExplicitGuardInWithClause (a:int) (b:int) =
    try
        a / b
    with
    | anyOther when anyOther.GetType() <> typeof<OperationCanceledException> -> a + b