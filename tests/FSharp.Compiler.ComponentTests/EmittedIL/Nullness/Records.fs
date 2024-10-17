module MyTestModule

let maybeString : string | null = null

[<NoComparison;NoEquality>]
type MyRecord<'X,'Y,'Z when 'Y:null and 'Z:not null> = 
    {
        JustInt : int
        NullInt : System.Nullable<int>
        JustString : string
        NullableString : string | null
        GenericNormalField : 'X
        GenericNullableField : 'Y
        GenericNotNullField : 'Z }

let createAnInstance () =
    {
        JustInt = 42
        NullInt = System.Nullable.op_Implicit(42)
        JustString = ""
        NullableString = null
        GenericNormalField = 42
        GenericNullableField = maybeString
        GenericNotNullField = ""}
    

let stringOfInst() : string = createAnInstance().ToString()