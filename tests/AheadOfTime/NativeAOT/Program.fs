module Program

open System

let print (s: string) = Console.WriteLine(s)

let printInterpolated() =
    let x = 42
    let name = "world"
    let pi = 3.14159
    let initial = 'F'
    print $"answer = {x}"
    print $"hello {name}"
    print $"pi ~ {pi:F2}"
    print $"padded:{x,6}"
    print $"greeting %s{name}"
    // Bare '%d'/'%i'/'%c'/'%M' specifiers lower to the same reflection-free path as a plain hole.
    print $"answer = %d{x}"
    print $"initial = %c{initial}"

    // The following use printf specifiers that still route through 'sprintf' and so generate
    // AOT IL2026/IL2070/IL3050 warnings.
    // print $"pi ~ %.2f{pi}"
    // print $"value = %A{x}"

[<EntryPoint>]
let main _ =
    printInterpolated()
    0
