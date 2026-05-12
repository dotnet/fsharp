module Conformance.BasicGrammarElements.UseBindExtensionMethodCapture

open Xunit
open FSharp.Test.Compiler

[<Fact>]
let ``Use binding doesn't capture an extension method with generic type``() =
    FSharp """
        open System
        open System.Runtime.CompilerServices

        type FooClass() = class end

        type Disposable() =
            interface IDisposable with 
                member _.Dispose() = ()

        [<Extension>]
        type PublicExtensions =
            [<Extension>]
            static member inline Dispose(this: #FooClass) =
                this
    
        let foo() =
            use a = new Disposable()
            ()
    
        foo()
    """
    |> asExe
    |> compile
    |> shouldSucceed