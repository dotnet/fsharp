module Verify13043

let list = [1; 2; 3]
let condition n = n < 3

let dropWhileWithMatch condition list =
  let rec f (l : List<int>) : List<int> =
    match l with
    | [] -> []
    | head :: tail ->
      match condition head with
      | true -> f tail
      | false -> head :: tail

  f list

(*
  This function caused an Unhandled exception at execution time:

  Unhandled exception. System.NullReferenceException: Object reference not set to an instance of an object.
   at Program.f@14-1.Invoke(FSharpList`1 _arg1) in /workspaces/function-rec-fsharp-dotnet7-bug-demo/Program.fs:line 18
   at Program.dropWhileWithFunction(FSharpFunc`2 condition, FSharpList`1 list) in /workspaces/function-rec-fsharp-dotnet7-bug-demo/Program.fs:line 21
   at <StartupCode$function-rec-fsharp-dotnet7-bug-demo>.$Program.main@() in /workspaces/function-rec-fsharp-dotnet7-bug-demo/Program.fs:line 29
*)
let dropWhileWithFunction condition list =
  let rec f : List<int> -> List<int> =
    function
    | [] -> []
    | head :: tail ->
      match condition head with
      | true -> f tail
      | false -> head :: tail

  f list


// this runs fine:
let matchResult = dropWhileWithMatch condition list
printfn "Match: %A" matchResult


// and this results in a null reference exception
let functionResult = dropWhileWithFunction condition list
printfn "Function: %A" functionResult
