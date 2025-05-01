module MyTestModule

[<NoComparison;NoEquality>]
[<Struct>]
type MyStructDU = 
    | A
    | B of nonNullableString:string         // Tricky as the field WILL be null for tags other than B
    | C of nullableString:(string | null)   // The field behind this is always nullable

    with override this.ToString() = 
            match this with
            | A -> "A"
            | B _ -> "B"
            | C _ -> "C"

let printMyDu(x:MyStructDU) : string = x.ToString()

let getVal x = 
    match x with
    | C text -> text
    | _ -> failwith "fail"

[<NoComparison;NoEquality;Struct>]
type SingleCaseStructDu = SingleCaseItIs of nullableString:(string|null)