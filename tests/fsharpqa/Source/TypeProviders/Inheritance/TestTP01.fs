namespace TPBug

#nowarn "25"

open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.TypeProvider.Emit
open Microsoft.FSharp.Quotations

module BaseGeneratedType = 
    type private Marker = interface end
    let Assembly = typeof<Marker>.Assembly
    let Namespace = "GeneratedTypes"

    let ProvidedType = 
        let ty = ProvidedTypeDefinition(Assembly, Namespace, "Root", Some typeof<obj>, IsErased = false)

        do
            ty.SetAttributes (ty.Attributes &&& (~~~System.Reflection.TypeAttributes.Sealed))

        do
            let ctor = ProvidedConstructor([ProvidedParameter("s", typeof<string>)])
            ty.AddMember ctor
        do
            let m = ProvidedMethod("Get", [], typeof<string>, IsStaticMethod = false)
            m.InvokeCode <- fun [this] -> Expr.Var(Var("s", typeof<string>)) // return field
            ty.AddMember m

        let tempFileName = 
            let fn = System.IO.Path.GetTempFileName()
            System.IO.Path.ChangeExtension(fn, ".dll")
        ty.ConvertToGenerated tempFileName
        ty

[<TypeProvider>]
type BaseGeneratedTypeProvider() = 
    inherit TypeProviderForNamespaces(BaseGeneratedType.Namespace, [BaseGeneratedType.ProvidedType])

[<assembly: TypeProviderAssembly>]
do()