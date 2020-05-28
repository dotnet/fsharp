namespace System.Runtime.CompilerServices

    open System
    open System.Runtime.CompilerServices
    open System.Runtime.InteropServices
    [<AttributeUsage(AttributeTargets.All,AllowMultiple=false)>]
    [<Sealed>]
    type IsReadOnlyAttribute() =
        inherit System.Attribute()

    [<AttributeUsage(AttributeTargets.All,AllowMultiple=false)>]
    [<Sealed>]
    type IsByRefLikeAttribute() =
        inherit System.Attribute()

