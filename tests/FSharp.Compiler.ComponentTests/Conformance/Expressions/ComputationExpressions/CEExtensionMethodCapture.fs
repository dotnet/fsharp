module Conformance.Expressions.CEExtensionMethodCapture

open Xunit
open FSharp.Test.Compiler

[<Fact>]
let ``CE doesn't capture an extension method beyond the access domain``() =
    FSharp """
        open System.Runtime.CompilerServices

        type AsyncSeq<'T>(i: 'T) = 
            class
            let l = [i]
            member this.Data = l
            end

        type AsyncSeqBuilder() =
            member _.Yield(x: 'T) : AsyncSeq<'T> =
                AsyncSeq(x)

        [<Extension>]
        type PrivateExtensions =
            [<Extension>]
            static member inline private Run(this: AsyncSeqBuilder) =
                this

        let asyncSeq = AsyncSeqBuilder()

        let xs : AsyncSeq<int> =
            asyncSeq {
                yield 1
            }
    """
    |> asExe
    |> compile
    |> shouldSucceed

[<Fact>]
let ``CE doesn't capture an extension method with generic type``() =
    FSharp """
        open System.Runtime.CompilerServices

        type FooClass = class end

        type AsyncSeq<'T>(i: 'T) = 
            class
            let l = [i]
            member this.Data = l
            end

        type AsyncSeqBuilder() =
            member _.Yield(x: 'T) : AsyncSeq<'T> =
                AsyncSeq(x)

        [<Extension>]
        type PublicExtensions =
            [<Extension>]
            static member inline Run(this: #FooClass) =
                this

        let asyncSeq = AsyncSeqBuilder()

        let xs : AsyncSeq<int> =
            asyncSeq {
                yield 1
            }
    """
    |> asExe
    |> compile
    |> shouldSucceed

// Deliberately trigger an error to ensure that a method is captured
[<Fact>]
let ``CE captures a public extension method and procudes an error due to invalid args``() =
    FSharp """
        open System.Runtime.CompilerServices

        type AsyncSeq<'T>(i: 'T) = 
            class
            let l = [i]
            member this.Data = l
            end

        type AsyncSeqBuilder() =
            member _.Yield(x: 'T) : AsyncSeq<'T> =
                AsyncSeq(x)

        [<Extension>]
        type PublicExtensions =
            [<Extension>]
            static member inline Run(this: AsyncSeqBuilder, invalidArg: string) =
                this

        let asyncSeq = AsyncSeqBuilder()

        let xs : AsyncSeq<int> =
            asyncSeq {
                yield 1
            }
    """
    |> asExe
    |> compile
    |> shouldFail

// Deliberately trigger an error to ensure that a method is captured
[<Fact>]
let ``CE captures a public extension method with valid generic constrainted type and procudes an error due to invalid args``() =
    FSharp """
        open System.Runtime.CompilerServices

        type AsyncSeq<'T>(i: 'T) = 
            class
            let l = [i]
            member this.Data = l
            end

        type AsyncSeqBuilder() =
            member _.Yield(x: 'T) : AsyncSeq<'T> =
                AsyncSeq(x)

        [<Extension>]
        type PublicExtensions =
            [<Extension>]
            static member inline Run(this: #AsyncSeqBuilder, invalidArg: string) =
                this

        let asyncSeq = AsyncSeqBuilder()

        let xs : AsyncSeq<int> =
            asyncSeq {
                yield 1
            }
    """
    |> asExe
    |> compile
    |> shouldFail

// Deliberately trigger an error to ensure that a method is captured
[<Fact>]
let ``CE captures a public extension method with generic type and procudes an error due to invalid args``() =
    FSharp """
        open System.Runtime.CompilerServices

        type AsyncSeq<'T>(i: 'T) = 
            class
            let l = [i]
            member this.Data = l
            end

        type AsyncSeqBuilder() =
            member _.Yield(x: 'T) : AsyncSeq<'T> =
                AsyncSeq(x)

        [<Extension>]
        type PublicExtensions =
            [<Extension>]
            static member Run(this: 'T, invalidArg: string) =
                this

        let asyncSeq = AsyncSeqBuilder()

        let xs : AsyncSeq<int> =
            asyncSeq {
                yield 1
            }
    """
    |> asExe
    |> compile
    |> shouldFail
