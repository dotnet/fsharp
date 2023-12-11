module TestModule

[<CompilationRepresentation(CompilationRepresentationFlags.UseNullAsTrueValue)>]
[<NoEquality; NoComparison>]
type MyNullableOption<'T when 'T:null> = 
    | MyNone
    | MySome of value:'T

let mapPossiblyNullable f myOpt =
    match myOpt with
    | MyNone -> MyNone
    | MySome x -> MySome (f x)

[<CompilationRepresentation(CompilationRepresentationFlags.UseNullAsTrueValue)>]
[<NoEquality; NoComparison>]
type MyOptionWhichCannotHaveNullInTheInside<'T when 'T:not null> = 
    | MyNotNullNone
    | MyNotNullSome of value:'T

let mapNotNullableContents f myOpt =
    match myOpt with
    | MyNotNullNone -> MyNotNullNone
    | MyNotNullSome x -> MyNotNullSome (f x)

[<NoEquality; NoComparison; Struct>]
type MyStructOption<'T when 'T: not null> = 
    | MyStructNone
    | MyStructSome of nestedGenericField : list<list<string | null>> * notNullField2 : string * canBeNullField : (string | null) * notNullField1 : 'T

let mapStructContents f myOpt =
    match myOpt with
    | MyStructNone -> MyStructNone
    | MyStructSome(ngf,s,ns,x) -> MyStructSome (ngf,s,ns,f x)