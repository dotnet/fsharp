namespace TypeProviderLibrary

open Microsoft.FSharp.Core.CompilerServices
open System

[<TypeProvider>]
type FakeTypeProvider() = 
    interface ITypeProvider with
        member this.GetStaticParameters _ = [||]
        member this.ApplyStaticArguments(_,_,_) = raise <| System.InvalidOperationException()
        member this.GetNamespaces() = [| |]
        member this.GetInvokerExpression(_,_) = failwith "GetInvokerExpression"
        [<CLIEvent>]
        member this.Invalidate = (new Event<_,_>()).Publish
        member this.GetGeneratedAssemblyContents(assembly) = failwith "GetGeneratedAssemblyContents"

    interface ITypeProvider2 with
        member this.GetStaticParametersForMethod _ = [||]
        member this.ApplyStaticArgumentsForMethod(_,_,_) = raise <| System.InvalidOperationException()

    interface IDisposable with
        member __.Dispose() = ()
    
[<assembly:TypeProviderAssembly>] 
do()
