// #Conformance #ObjectOrientedTypes #Classes #MethodsAndProperties
module M

type T() =
    member val public PublicProperty = 0 with get, set
    member val private PrivateProperty = 0 with get, set
    member val internal InternalProperty = 0 with get, set
    static member val public StaticPublicProperty = 0 with get, set
    static member val private StaticPrivateProperty = 0 with get, set
    static member val internal StaticInternalProperty = 0 with get, set