module Program

open System

let print (s: string) = Console.WriteLine(s)

let printInterpolated() =
    let x = 42
    let name = "world"
    let pi = 3.14159
    print $"answer = {x}"
    print $"hello {name}"
    print $"pi ~ {pi:F2}"
    print $"padded:{x,6}"

    // The following use printf specifiers and will generate AOT IL2026/IL2070/IL3050 warnings.
    // print $"answer = %d{x}"
    // print $"hello %s{name}"
    // print $"pi ~ %.2f{pi}"
    // print $"value = %A{x}"

[<EntryPoint>]
let main _ =
    printInterpolated()
    0
