namespace RequireNamedArgumentsProvider

#load "../helloWorld/TypeMagic.fs"

open System
open System.Reflection
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Quotations
open FSharp.TypeMagic

[<TypeProvider>]
type Provider() =
    let modul = typeof<Provider>.Module
    let attributeType =
        TypeBuilder.CreateType(
            TypeContainer.Namespace(modul, "System.Runtime.CompilerServices"),
            "RequireNamedArgumentAttribute",
            baseType = typeof<Attribute>)

    let attribute =
        { new CustomAttributeData() with
            member _.Constructor = TypeBuilder.CreateConstructor(attributeType, fun () -> [||])
            member _.ConstructorArguments = upcast [||]
            member _.NamedArguments = upcast [||] }

    let providedType =
        TypeBuilder.CreateType(
            TypeContainer.Namespace(modul, "Provided"),
            "C",
            members = TypeBuilder.CacheMembers(fun declaringType ->
                [| TypeBuilder.CreateMethod(
                       declaringType,
                       "M",
                       typeof<int>,
                       isStatic = true,
                       parameters = [| TypeBuilder.CreateParameter("x", typeof<int>); TypeBuilder.CreateParameter("y", typeof<int>) |],
                       getCustomAttributes = fun () -> [| attribute |]) |]))

    let invalidation = Event<EventHandler, EventArgs>()

    interface IProvidedNamespace with
        member _.NamespaceName = "Provided"
        member _.GetTypes() = [| providedType |]
        member _.GetNestedNamespaces() = [||]
        member _.ResolveTypeName name = if name = providedType.Name then providedType else null

    interface ITypeProvider with
        member this.GetNamespaces() = [| this |]
        member _.GetStaticParameters _ = [||]
        member _.ApplyStaticArguments(typ, _, _) = typ
        member _.GetInvokerExpression(_, _) = Expr.Value 3
        member _.GetGeneratedAssemblyContents _ = failwith "Only erased types are provided."
        [<CLIEvent>]
        member _.Invalidate = invalidation.Publish

    interface IDisposable with
        member _.Dispose() = ()

[<assembly: TypeProviderAssembly>]
do ()
