namespace StaticStringParameterProvider

#load "../helloWorld/TypeMagic.fs"

open System
open System.Collections.Generic
open System.Reflection
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Quotations
open FSharp.TypeMagic

/// Provides 'Provided.Text<Value>', whose static property 'Value' returns the static argument.
[<TypeProvider>]
type Provider() =
    let modul = typeof<Provider>.Module
    let container = TypeContainer.Namespace(modul, "Provided")
    let unappliedType = TypeBuilder.CreateType(container, "Text")
    let appliedTypes = Dictionary<string, struct (Type * string)>(StringComparer.Ordinal)

    let invalidation = Event<EventHandler, EventArgs>()

    interface IProvidedNamespace with
        member _.NamespaceName = "Provided"
        member _.GetTypes() = [| unappliedType |]
        member _.GetNestedNamespaces() = [||]
        member _.ResolveTypeName name = if name = unappliedType.Name then unappliedType else null

    interface ITypeProvider with
        member this.GetNamespaces() = [| this |]

        member _.GetStaticParameters typeWithoutArguments =
            if typeWithoutArguments.Name = unappliedType.Name then
                [| TypeBuilder.CreateStaticParameter("Value", typeof<string>, 0) |]
            else
                [||]

        member _.ApplyStaticArguments(_, typePathWithArguments, staticArguments) =
            let name = typePathWithArguments[typePathWithArguments.Length - 1]

            lock appliedTypes (fun () ->
                match appliedTypes.TryGetValue name with
                | true, struct (appliedType, _) -> appliedType
                | _ ->
                    let appliedType =
                        TypeBuilder.CreateType(
                            container,
                            name,
                            members = TypeBuilder.CacheMembers(fun declaringType ->
                                TypeBuilder.JoinPropertiesIntoMemberInfos
                                    [ TypeBuilder.CreateSyntheticProperty(declaringType, "Value", typeof<string>, isStatic = true) ]))

                    appliedTypes[name] <- struct (appliedType, staticArguments[0] :?> string)
                    appliedType)

        member _.GetInvokerExpression(getter, _) =
            let struct (_, value) = lock appliedTypes (fun () -> appliedTypes[getter.DeclaringType.Name])
            Expr.Value value

        member _.GetGeneratedAssemblyContents _ = failwith "Only erased types are provided."

        [<CLIEvent>]
        member _.Invalidate = invalidation.Publish

    interface IDisposable with
        member _.Dispose() = ()

[<assembly: TypeProviderAssembly>]
do ()
