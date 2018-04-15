namespace Microsoft.FSharp.Compiler

module internal ProviderAlias =

    open Microsoft.FSharp.Core.CompilerServices
    open ExtensionTyping
    open PrettyNaming
    open Range

    type Provider = Provider of ITypeProvider * Tainted<ProvidedType>

    let unwrapStaticArgs = Array.map (fun (StaticArg x) -> x)

    let getStaticParametersOfProvider (Provider (p, pt)) m = pt.PUntaint((fun x -> x.GetStaticParameters p), m)

    let tryApplyStaticArgsToProvider (Provider (_p, pt)) args m =
        TryApplyProvidedType(pt, None, args, m)

    type Variable =
        | Variable of string

    type Argument =
        | Var of Variable
        | Const of StaticArg

    type ProviderExpression =
        | Terminal of Argument
        | Provider of Provider * ProviderExpression[]

    type ProviderAlias =
        ProviderAlias of Variable[] * (Provider * ProviderExpression[])

    let myProvider : Provider = failwith ""
    let myOtherProvider : Provider = failwith ""

    let exampleAlias =
        ProviderAlias ([|Variable "a"|],
            (myProvider,
                [|
                    Provider (myOtherProvider, [| Terminal (Var (Variable "a")) |])
                |]
            )
        )

    let rec combineApplicationsToSubExpressions map m p cs =
        let combine2 = fun (sas, checkerAcc) (sa, checker) -> (sas @ [sa], checkerAcc >> checker)
        let args = Array.map (tryApplyStaticArgsToExpr map m) cs
        if Array.forall Option.isSome args then
            let args = Array.choose id args
            let (args, checker) = Array.fold combine2 ([], ignore) args
            tryApplyStaticArgsToProvider p (Array.ofList args) m
            |> Option.map (fun (pt, c) -> pt, checker >> c)
        else
            None
    
    and tryApplyStaticArgsToExpr (map : Map<Variable, StaticArg>) (m : range) : ProviderExpression -> (StaticArg * (unit -> unit)) option = function
        | Terminal (Const c) -> Some (c, ignore)
        | Terminal (Var v) -> Some (map.[v], ignore)
        | Provider (p, cs) ->
            combineApplicationsToSubExpressions map m p cs
            |> Option.map (fun (pt, c) -> pt.PUntaint((fun x -> StaticArg <| box x.RawSystemType), m), c)

    let tryApplyStaticArgsToAlias (ProviderAlias(vs, (p, cs))) (args : StaticArg[]) (m : range) : (Tainted<ProvidedType> * (unit -> unit)) option =
        let map = Array.zip vs args |> Map.ofArray
        combineApplicationsToSubExpressions map m p cs

