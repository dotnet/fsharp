// #Warnings
//<Expects status="Error" id="FS0039">The field, constructor or member 'Cas1' is not defined.</Expects>
//<Expects>Maybe you want one of the following:\s+Case1</Expects>

[<RequireQualifiedAccess>]
type MyUnion =
| Case1
| Case2

let y = MyUnion.Case1

let x =
    match y with
    | MyUnion.Cas1 -> 1
    | _ -> 2

exit 0