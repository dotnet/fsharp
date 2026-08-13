// Provenance record for checked-in LegacyInline.dll.
// Build with official .NET SDK 3.1.100 / F# 4.7 / fsc 10.7.0.0 from a netstandard2.0 SDK project via:
//   dotnet build LegacyInline.fsproj -c Release
// with Optimize=true, Deterministic=true, and PathMap=<project-root>=/src.
// The DLL must retain the pre-F#6 PseudoVal zero-bit inline metadata for
// https://github.com/dotnet/fsharp/issues/20253 and must not be regenerated with a current compiler.
namespace LegacyInline

type Lens<'a, 'b> =
    ('a -> 'b) * ('b -> 'a -> 'a)

type Prism<'a, 'b> =
    ('a -> 'b option) * ('b -> 'a -> 'a)

module Library =
    type Set =
        | Set with
            static member (^=) (Set, (_, set): Lens<'a, 'b>) =
                fun value -> set value

            static member (^=) (Set, (_, set): Prism<'a, 'b>) =
                fun value -> set value

    let inline invoke optic value =
        (Set ^= optic) value
