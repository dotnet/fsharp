#if INTERACTIVE
#r "author.dll"
#else
module Foo
#endif

Test.init1 4 (fun _ -> 4.) |> printfn "%A"
Test.init2 4 (fun _ -> 4.) |> printfn "%A"

Test.reduce1 (fun acc n -> acc + n) [| 1 .. 10 |] |> printfn "%A"
Test.reduce2 (fun acc n -> acc + n) [| 1 .. 10 |] |> printfn "%A"

#if INTERACTIVE
#q ;;
#endif