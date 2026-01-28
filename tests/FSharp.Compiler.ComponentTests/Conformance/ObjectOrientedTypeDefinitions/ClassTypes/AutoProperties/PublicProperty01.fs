// #Conformance #ObjectOrientedTypes #Classes #MethodsAndProperties #Accessibility
module M =
    type T() =
        member val public PublicProperty = 0 with get, set
        member val private PrivateProperty = 0 with get, set
        member val internal InternalProperty = 0 with get, set
        static member val public StaticPublicProperty = 0 with get, set
        static member val private StaticPrivateProperty = 0 with get, set
        static member val internal StaticInternalProperty = 0 with get, set

module M2 =
    open M
    let x = T()

    x.PublicProperty <- 1
    if x.PublicProperty <> 1 then exit 1

    x.InternalProperty <- 1
    if x.InternalProperty <> 1 then exit 1
    
    T.StaticPublicProperty <- 1
    if T.StaticPublicProperty <> 1 then exit 1

    T.StaticInternalProperty <- 1
    if T.StaticInternalProperty <> 1 then exit 1

