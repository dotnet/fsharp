// <testmetadata>
// { "optimization": { "reported_in": "#8983", "reported_by": "@NinoFloris", "last_know_version_not_optimizing": "8", "first_known_version_optimizing": null } }
// </testmetadata>

let inline mapOption mapper option = 
    match option with 
    | Some x -> Some(mapper x)
    | None -> None

let almostErasedOption() =
    let y () = 3
    
    match Some(y()) |> mapOption (fun x -> x + 2) with 
    | Some x -> 5
    | None -> 4
    
let unnecessaryOption() =
    let y () = 3
    
    match Some(y()) |> mapOption (fun x -> x + 2) with 
    | Some x -> x
    | None -> 4
    
System.Console.WriteLine()