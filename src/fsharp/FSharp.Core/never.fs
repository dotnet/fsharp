namespace Microsoft.FSharp.Core

module TypeRegulate =


    // Type for can't show up
    [<Struct; NoComparison; NoEquality>]
    type Never = 
          |Never
          static member NoneNeverId(_: Never) = (); 
          static member NoneNeverId(x: int) = x
          static member NoneNeverId<'t>(x: 't) = x

    // checked if param is `Never`, when `Never` a error should raise
    let inline nnid x = 
        let inline fob(_:^t) x = ((^t or ^x or ^r): (static member NoneNeverId: ^x -> ^r) x)
        if true then x
            else (fob Never) Unchecked.defaultof<_>


    // A Goup function for setting type range: the type should belong to `TBelong`, but not `TAgainst`
    type TypeDiffer<'TBelong, 'TAgainst> =
            
        static member Assign(x: 'TBelong) = x
        static member Assign(_: 'TAgainst) = Never 

        static member inline Id x =     
            let inline assign(_:^t) x = ((^t or ^x or ^r): (static member Assign: ^x -> ^r) x)
            (assign Unchecked.defaultof<TypeDiffer<'TBelong, 'TAgainst>>) x
