
namespace ProviderImplementation

open ProviderImplementation
open ProviderImplementation.ProvidedTypes
open FSharp.Quotations
open FSharp.Core.CompilerServices
open System.Reflection


type SomeRuntimeHelper() = 
    static member Help() = "help"

[<AllowNullLiteral>]
type SomeRuntimeHelper2() = 
    static member Help() = "help"

[<TypeProvider>]
type ComboProvider (config : TypeProviderConfig) as this =
    inherit TypeProviderForNamespaces (config, assemblyReplacementMap=[("ComboProvider.DesignTime", "ComboProvider")])

    let ns = "ComboProvider.Provided"
    let asm = Assembly.GetExecutingAssembly()

    // check we contain a copy of runtime files, and are not referencing the runtime DLL
    do assert (typeof<SomeRuntimeHelper>.Assembly.GetName().Name = asm.GetName().Name)  

    let createTypes () =
        let myType = ProvidedTypeDefinition(asm, ns, "MyType", Some typeof<obj>)

        let ctor = ProvidedConstructor([], invokeCode = fun args -> <@@ "My internal state" :> obj @@>)
        myType.AddMember(ctor)

        let ctor2 = ProvidedConstructor([ProvidedParameter("InnerState", typeof<string>)], invokeCode = fun args -> <@@ (%%(args.[0]):string) :> obj @@>)
        myType.AddMember(ctor2)

        let innerState = ProvidedProperty("InnerState", typeof<string>, getterCode = fun args -> <@@ (%%(args.[0]) :> obj) :?> string @@>)
        myType.AddMember(innerState)

        let meth = ProvidedMethod("StaticMethod", [], typeof<SomeRuntimeHelper>, isStatic=true, invokeCode = (fun args -> <@@ SomeRuntimeHelper() @@>))
        myType.AddMember(meth)

        let meth2 = ProvidedMethod("StaticMethod2", [], typeof<SomeRuntimeHelper2>, isStatic=true, invokeCode = (fun args -> Expr.Value(null, typeof<SomeRuntimeHelper2>)))
        myType.AddMember(meth2)

        [myType]

    do
        this.AddNamespace(ns, createTypes())

[<assembly:CompilerServices.TypeProviderAssembly()>]
do ()
