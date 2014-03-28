module TPTest

open Microsoft.FSharp.TypeProvider.Emit
open Microsoft.FSharp.Core.CompilerServices

[<TypeProvider>]
type TestProvider() =
    inherit TypeProviderForNamespaces()
    let asm = typeof<TestProvider>.Assembly
    let ns = "TPTest"
    let t = ProvidedTypeDefinition(asm, ns, "Test", None)
    do
        t.AddMember(ProvidedMethod("StaticMethod", [ProvidedParameter("i", typeof<int>)], typeof<string>, IsStaticMethod = true, InvokeCode = fun [i] -> <@@ sprintf "%i" %%i @@>))
        t.AddMember(ProvidedMethod("StaticMethod3", [], typeof<int>, IsStaticMethod = true, InvokeCode = fun _ -> <@@ "boo" @@>))
        t.AddMember(ProvidedMethod("StaticMethod4", [ProvidedParameter("i", typeof<int>)], typeof<int>, IsStaticMethod = true, InvokeCode = fun [i] -> <@@ (%%i : int) :> obj @@>))
        t.AddMember(ProvidedMethod("StaticMethod5", [ProvidedParameter("o", typeof<obj>, optionalValue = null)], typeof<obj>, IsStaticMethod=true, InvokeCode = fun [o] -> o))

        base.AddNamespace(ns, [t])

[<assembly:TypeProviderAssembly>]
do ()
