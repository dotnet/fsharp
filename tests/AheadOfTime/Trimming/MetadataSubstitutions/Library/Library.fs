namespace MetadataSubstitutions

type Library =
    [<System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)>]
    static member Value() = System.DateTime.UtcNow.Year
