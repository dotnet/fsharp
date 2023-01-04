// #Regression #NoMT #CompilerOptions 
#light

// Regression test for FSharp1.0:2249 - defaultof<> unexpectedly requires parens

let (f : unit -> int) = Unchecked.defaultof<unit->int>
if (f |> box |> unbox) <> null then exit 1

let (g : unit -> int) = if true then Unchecked.defaultof<unit->int> else fun() -> 0
if (g |> box |> unbox) <> null then exit 1

exit 0
