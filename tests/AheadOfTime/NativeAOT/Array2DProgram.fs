module Array2DProgram

open System

let private check name (actual: int) (expected: int) =
    if actual <> expected then
        Console.WriteLine $"FAILED {name}: expected {expected} but got {actual}"

// Consume every grid, so ILC cannot drop the allocations this test exists to exercise.
let private sum (grid: int[,]) =
    let total = ref 0
    grid |> Array2D.iteri (fun _ _ v -> total.Value <- total.Value + v)
    total.Value

[<System.Diagnostics.CodeAnalysis.UnconditionalSuppressMessage("AOT", "IL3050",
    Justification = "Deliberately calls the unsupported API to assert it throws PlatformNotSupportedException under AOT.")>]
let private createBasedThrows rows cols =
    try
        Array2D.createBased 1 2 rows cols 7 |> ignore
        0
    with :? PlatformNotSupportedException ->
        1

let run (argv: string[]) =
    // Derive the dimensions from argv, so ILC cannot constant-fold the allocations away.
    // The expected values below assume check.ps1 runs the app with no arguments.
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
