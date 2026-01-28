// #Regression #Conformance #TypeConstraints 
// Verify warning when a type is constrained further than is annotated
// FSB 1436
//<Expects id="FS0064" span="(16,28-16,32)" status="warning">This construct causes code to be less generic than indicated by its type annotations. The type variable implied by the use of a '#', '_' or other type annotation at or near</Expects>

open System
open Microsoft.FSharp.Math


open System
type Node() = 
    member this.Parent = this
    member this.ChildList = [this] |> List.toSeq
    
let rec processNode (node : #Node) =
    let mutable t : Node = node // [1] cast node of type 'a (where 'a :> Node) to type node.
    while true do
        t <- t.Parent
        Console.Write("\t")
    Console.WriteLine(node.GetType().ToString())
    let childList = List.ofSeq node.ChildList         // [2] .GetEnumerator() 
    List.iter (fun n -> processNode n) childList       // [3] Problem...

exit 0
