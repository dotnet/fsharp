open System.Diagnostics.CodeAnalysis

type Class() =
    [<return: NotNull>]
    static member ClassMethod () = obj()
