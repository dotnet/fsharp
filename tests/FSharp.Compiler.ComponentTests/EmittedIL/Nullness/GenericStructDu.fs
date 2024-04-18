module TestModule

[<NoEquality; NoComparison; Struct>]
type MyStructOption<'T when 'T: not null> = 
    | MyStructNone
    | MyStructSome of nestedGenericField : list<list<string | null>> * notNullField2 : string * canBeNullField : (string | null) * notNullField1 : 'T

let mapStructContents f myOpt =
    match myOpt with
    | MyStructNone -> MyStructNone
    | MyStructSome(ngf,s,ns,x) -> MyStructSome (ngf,s,ns,f x)