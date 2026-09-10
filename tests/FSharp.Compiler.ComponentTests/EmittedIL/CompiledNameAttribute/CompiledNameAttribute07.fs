// #NoMono #NoMT #CodeGen #EmittedIL #Attributes
// Regression test for https://github.com/dotnet/fsharp/issues/19604
// EXPECTED: [<CompiledName>] with a different value on one overload renames only that overload.
module Program

type Builder() =
    member _.X = 1

module Ext =
    type Builder with
        [<CompiledName "Renamed">]
        member builder.UseDb (i: int) = i

        member builder.UseDb (s: string) = s
