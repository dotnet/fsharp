namespace StaticStringValueProvider

#load "../helloWorld/TypeMagic.fs"

open System
open Microsoft.FSharp.Core.CompilerServices
open FSharp.TypeMagic

[<TypeProvider>]
type Provider() =
    let container = TypeContainer.Namespace(typeof<Provider>.Module, "Provided")
    let text = TypeBuilder.CreateType(container, "Text")
    let invalidation = Event<EventHandler, EventArgs>()

    interface IProvidedNamespace with
        member _.NamespaceName = "Provided"
        member _.GetTypes() = [| text |]
        member _.GetNestedNamespaces() = [||]
        member _.ResolveTypeName name = if name = text.Name then text else null

    interface ITypeProvider with
        member this.GetNamespaces() = [| this |]
        member _.GetStaticParameters _ = [| TypeBuilder.CreateStaticParameter("Value", typeof<string>, 0) |]

        member _.ApplyStaticArguments(_, typePathWithArguments, staticArguments) =
            if string staticArguments[0] <> "api/v2/items" then
                failwith $"Unexpected static argument '{staticArguments[0]}'"

            TypeBuilder.CreateType(container, typePathWithArguments[typePathWithArguments.Length - 1])

        member _.GetInvokerExpression(_, _) = failwith "No members are provided."
        member _.GetGeneratedAssemblyContents _ = failwith "Only erased types are provided."

        [<CLIEvent>]
        member _.Invalidate = invalidation.Publish

    interface IDisposable with
        member _.Dispose() = ()

[<assembly: TypeProviderAssembly>]
do ()
