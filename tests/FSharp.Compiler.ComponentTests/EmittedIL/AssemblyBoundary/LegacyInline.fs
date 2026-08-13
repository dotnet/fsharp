// This source is the provenance record for LegacyInline.dll, which is checked in as a binary
// and must not be regenerated with a current compiler. It was built with the official .NET SDK
// 3.1.100 / F# 4.7 / fsc 10.7.0.0:
//   dotnet fsc.dll --target:library --targetprofile:netcore --optimize+ -o:LegacyInline.dll LegacyInline.fs
// The resulting DLL must retain the pre-F#6 PseudoVal zero-bit inline metadata that
// https://github.com/dotnet/fsharp/issues/20253 depends on.
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
