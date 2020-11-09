// #Conformance #SyntacticSugar #ReqNOMT 
#light

// Verify 2D slices.  Note that in F# you (currently?) cannot
// declare square array literals, so we need to convert expected
// values by hand.
let toSquare (jagged : 'a[][]) : 'a[,] =
    if jagged = null then         raise <| new System.ArgumentNullException()
    if jagged.Length = 0 then     raise <| new System.ArgumentException()
    if jagged.[0].Length = 0 then raise <| new System.ArgumentException()
    
    let cols = jagged.Length
    let rows = jagged.[0].Length
    
    let square = Array2D.create cols rows (Unchecked.defaultof<'a>)
    // Copy the contents over
    Array.iteri
        (fun yIdx row -> 
            Array.iteri
                (fun xIdx value -> square.[yIdx, xIdx] <- value)
                row
        )
        jagged

    square

let test = [|
              [| 0; 1; 2 |];
              [| 3; 4; 5 |];
              [| 6; 7; 8 |];
           |] |> toSquare

// Full ranges and empty ranges
let expectedSS = [| [|1; 2|]; [|4; 5|] |] |> toSquare
if test.[0..1, 1..2] <> expectedSS then exit 1

let expectedSN = [| [| 6; 7; 8|] |] |> toSquare
if test.[2..2, *] <> expectedSN then exit 1

let expectedNS = [| [|1; 2|]; [|4; 5|]; [|7; 8|] |] |> toSquare
if test.[*, 1..2] <> expectedNS then exit 1

if test.[*,*] <> test then exit 1

// Partial ranges
let expectedPP = [|[|6; 7|]|] |> toSquare
if test.[2.., ..1] <> expectedPP then exit 1

let expectedPP2 = [| [|0; 1|] |] |> toSquare
if test.[..0, ..1] <> expectedPP2 then exit 1

exit 0
