// #Conformance #TypesAndModules #Unions 
// Anonymous Union Types
// Anonymous union types used in method overloading
//<Expects status="success"></Expects>

type FontFamily = 
    | SansSerif
    | Serif

type View =
    static member Font(name: (string|FontFamily), ?size: (float|int|string)) =
        printfn "Font: %A, Size: %A" name size

View.Font("Sans Serif", 12.0)
View.Font(FontFamily.SansSerif, "10px")
View.Font("Test", 12)