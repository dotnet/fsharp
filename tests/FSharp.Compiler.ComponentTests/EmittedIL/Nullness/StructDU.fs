module MyTestModule

[<NoComparison;NoEquality>]
[<Struct>]
type MyStructDU = 
    | A
    | B of nonNullableString:string         // Tricky as the field WILL be null for tags other than B
    | C of nullableString:(string | null)   // The field behind this is always nullable

let getVal x = 
    match x with
    | C text -> text
    | _ -> failwith "fail"