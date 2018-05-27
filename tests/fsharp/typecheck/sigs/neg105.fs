// rewording of FS0019 (given for wrong number of args on DU cases)
module M

type Rect = Rect of width: float * height: float

let f = function
    | Rect -> ()

let g = function
    | Rect () -> ()

let h = function
    | Rect 1.0 -> ()

// If the first letter of the DU field name is capital, it should be printed in lowercase in the warning
// because it would cause the compiler to suggest a bad (though not invalid) pattern name.
// This is especially important since the default names look like "Item1", "Item2", etc.

// implicit uppercase (Item1 * Item2)
type Point = Point of int * int

let i = function
    | Point -> ()

// explicit uppercase
type Triangle = Triangle of Base:int * Height:int

let j = function
    | Triangle -> ()