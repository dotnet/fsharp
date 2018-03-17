module M
type EnumAB = A = 0 | B = 1
type UnionAB = A | B

module FS0025 =
    // All of these should emit warning FS0025 ("Incomplete pattern match....")
        
    let f1 = function
        | EnumAB.A -> "A"

    let f2 = function
        | UnionAB.A -> "A"

    let f3 = function
        | 42 -> "forty-two"        

module FS0104 =
    // These should emit warning FS0104 ("Enums may take values outside of known cases....")

    let f1 = function
        | EnumAB.A -> "A"
        | EnumAB.B -> "B"

    let f2 = function
        | Some(EnumAB.A) -> "A"
        | Some(EnumAB.B) -> "B"
        | None -> "none"