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


[<CompilationRepresentation(CompilationRepresentationFlags.UseNullAsTrueValue)>]
[<NoEquality; NoComparison>]
type NonGenericNullAsTrueValue = 
    | MyNone
    | MySome of nullableString:(string|null)