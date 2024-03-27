// <testmetadata>
// { "optimization": { "reported_in": "#5291", "reported_by": "@jack-pappas", "last_know_version_not_optimizing": "8", "first_known_version_optimizing": null } }
// </testmetadata>

open System.Collections.Generic
open OptimizedClosures

[<CompiledName("FoldStrong")>]
let foldStrong (folder : 'State -> 'T -> 'State) state (enumerator : ('TEnumerator :> IEnumerator<'T>)) : 'State =
    let folder = FSharpFunc<_,_,_>.Adapt folder

    let mutable enumerator' = enumerator
    let mutable state = state
    while enumerator'.MoveNext () do
        state <- folder.Invoke (state, enumerator.Current)
    state

[<CompiledName("Fold")>]
let inline fold< ^State, ^T, ^TSeq, ^TEnumerator
    when ^TSeq: (member GetEnumerator: unit -> ^TEnumerator)
    and ^TEnumerator :> IEnumerator< ^T > >
        (folder : ^State -> ^T -> ^State) state (source : ^TSeq) : 'State =
    // This 'use' binding causes unnecessary boxing of 'enumerator'.
    use enumerator = (^TSeq : (member GetEnumerator: unit -> ^TEnumerator) (source))
    foldStrong folder state enumerator


let testFoldOnListT () =
    let items = ResizeArray ([| 1; 1; 2; 3; 5; 8 |])

    ("", items)
    ||> fold (fun state x ->
        state + string x)
    
System.Console.WriteLine(testFoldOnListT())