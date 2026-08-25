module Array2DProgram

open System

// A small grid program, run under NativeAOT so that the publish itself is the assertion.
// check.ps1 explains which calls are guarded and why.

let private check name (actual: int) (expected: int) =
    if actual <> expected then
        Console.WriteLine $"FAILED {name}: expected {expected} but got {actual}"

// Summing through iteri gives every grid below an observable result. Without that, the optimizer
// or ILC could drop an unused allocation and silently take the code this guards out of the
// analyzed program.
let private sum (grid: int[,]) =
    let total = ref 0
    grid |> Array2D.iteri (fun _ _ v -> total.Value <- total.Value + v)
    total.Value

// Array2D.createBased is annotated RequiresDynamicCode, so this is the one call in the program
// that has to suppress IL3050. Everything above it must publish clean.
[<System.Diagnostics.CodeAnalysis.UnconditionalSuppressMessage("Aot", "IL3050",
    Justification = "Deliberately calls the unsupported API to assert it throws PlatformNotSupportedException under AOT.")>]
let private createBasedThrows rows cols =
    try
        Array2D.createBased 1 2 rows cols 7 |> ignore
        0
    with :? PlatformNotSupportedException ->
        1

let run (argv: string[]) =
    // An empty argv means 3x4, but neither the optimizer nor ILC can prove that, so the
    // allocations cannot be folded to constants. The expected values below are the ones for an
    // empty argv, so check.ps1 has to run the app without arguments.
    let rows = 3 + argv.Length
    let cols = 4 + argv.Length

    let table = Array2D.init rows cols (fun i j -> (i + 1) * (j + 1))
    check "init.rows" (Array2D.length1 table) 3
    check "init.cols" (Array2D.length2 table) 4
    check "init" (sum table) 60

    let board = Array2D.create rows cols 2
    check "create" (sum board) 24

    Array2D.blit table 0 0 board 1 1 2 2
    check "blit" (sum board) 25
    check "blit.cell" board.[2, 2] 4

    let rebased = Array2D.rebase table
    check "rebase" (sum rebased) 60
    check "rebase.base" (Array2D.base1 rebased + Array2D.base2 rebased) 0

    let doubled = Array2D.map (fun v -> v * 2) table
    check "map" (sum doubled) 120

    let skewed = Array2D.mapi (fun i j v -> v + i - j) table
    check "mapi" (sum skewed) 54

    let copied = Array2D.copy table
    check "copy" (sum copied) 60
    check "copy.distinct" (if Object.ReferenceEquals(copied, table) then 1 else 0) 0

    check "createBased.throws" (createBasedThrows rows cols) 1
