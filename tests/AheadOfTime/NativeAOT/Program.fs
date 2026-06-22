module Program

open System

// Check a rendering against an expected string literal; a mismatch prints a "FAILED" line.
let check (actual: string, expected: string) =
    if actual <> expected then
        Console.WriteLine $"FAILED: expected '{expected}' but got '{actual}'"

let runChecks () =
    let x = 42
    let name = "world"
    let pi = 3.14159
    let initial = 'F'
    check ($"answer = {x}", "answer = 42")
    check ($"hello {name}", "hello world")
    check ($"pi ~ {pi:F2}", "pi ~ 3.14")
    check ($"padded:{x,6}", "padded:    42")
    check ($"greeting %s{name}", "greeting world")
    // Bare '%d'/'%i'/'%c'/'%M' specifiers lower to the same reflection-free path as a plain hole.
    check ($"answer = %d{x}", "answer = 42")
    check ($"initial = %c{initial}", "initial = F")

    // The following use printf specifiers that still route through 'sprintf', so they would make the
    // NativeAOT publish fail with IL2026/IL2070/IL3050.
    // check ($"pi ~ %.2f{pi}", "pi ~ 3.14")
    // check ($"value = %A{x}", "value = 42")

[<EntryPoint>]
let main _ =
    runChecks ()
    // Success sentinel; a failed check above printed a "FAILED" line first, so the output won't be just this.
    Console.WriteLine "Finished"
    0
