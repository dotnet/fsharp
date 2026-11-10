// Anonymous Union Types
// Anonymous union types used in method overloading

type FontFamily = 
    | SansSerif
    | Serif

type View =
    static member Font(name: (string|FontFamily), size: (float|int|string)): string =
        sprintf "Font: %A, Size: %A" name size

if not (View.Font("Sans Serif", 12.0) = (sprintf "Font: %A, Size: %A" "Sans Serif" 12.0)) then failwith "Test failed"
if not (View.Font(FontFamily.SansSerif, "10px") = (sprintf "Font: %A, Size: %A" FontFamily.SansSerif "10px")) then failwith "Test failed"
if not (View.Font("Test", 12) = (sprintf "Font: %A, Size: %A" "Test" 12)) then failwith "Test failed"