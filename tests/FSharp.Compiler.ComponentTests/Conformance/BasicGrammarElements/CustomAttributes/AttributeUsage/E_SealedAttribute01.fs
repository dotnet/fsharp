[<Sealed>]
type UnnecessarilySealedStruct = 
    struct
        member x.P = 1
    end

[<Sealed>]
type BadSealedInterface = 
    interface
        abstract P : int
    end

[<Sealed>]
type UnnecessarilySealedDelegate = delegate of int -> int