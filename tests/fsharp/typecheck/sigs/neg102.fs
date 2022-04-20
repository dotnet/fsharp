module M
type EnumABC = A = 0 | B = 1 | C = 42
type UnionAB = A | B

module FS0025 =
    // All of these should emit warning FS0025 ("Incomplete pattern match....")


    let f1 = function
        | UnionAB.A -> "A"

    let f2 = function
        | 42 -> "forty-two"

    let f3 = function
        | EnumABC.A -> "A"
    
    let f4 = function
        | EnumABC.A -> "A"
        | EnumABC.B -> "B"

    let f5 = function
        | Some(EnumABC.A) | Some(EnumABC.B) -> "A|B"
        | None -> "neither"

    // try a non-F#-defined enum
    let f6 = function System.DateTimeKind.Unspecified -> 0

module FS0104 =
    // These should emit warning FS0104 ("Enums may take values outside of known cases....")

    let f1 = function
        | EnumABC.A -> "A"
        | EnumABC.B -> "B"
        | EnumABC.C -> "C"

    let f2 = function
        | Some(EnumABC.A) -> "A"
        | Some(EnumABC.B) -> "B"
        | Some(EnumABC.C) -> "C"
        | None -> "none"

    // try a non-F#-defined enum
    let f3 = function
        | System.DateTimeKind.Unspecified -> "Unspecified"
        | System.DateTimeKind.Utc -> "Utc"
        | System.DateTimeKind.Local -> "Local"

module IncompleteMatchWarningAtRightPlace =
    let f() =
        match (
            let value = 5
            value + 4
            |> Some
        )
        with
        | None -> ()
