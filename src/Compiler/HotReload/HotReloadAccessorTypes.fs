namespace FSharp.Compiler.HotReload

open FSharp.Compiler.HotReloadBaseline
open FSharp.Compiler.TypedTreeDiff

[<NoEquality; NoComparison>]
type internal AccessorUpdate =
    {
        Symbol: SymbolId
        ContainingType: string
        MemberKind: SymbolMemberKind
        Method: MethodDefinitionKey option
    }
