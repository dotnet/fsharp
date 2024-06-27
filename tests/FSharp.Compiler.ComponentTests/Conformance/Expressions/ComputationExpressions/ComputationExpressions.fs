module Conformance.Expressions.ComputationExpressions

open Xunit
open FSharp.Test.Compiler

[<Fact>]
let ``[<CustomOperation>] without explicit name is allowed, uses method name as operation name`` () =
    FSharp """
        module CustomOperationTest
        type CBuilder() =
            [<CustomOperation>]
            member this.Foo _ = "Foo"
            [<CustomOperation>]
            member this.foo _ = "foo"
            [<CustomOperation("")>]
            member this.bar _ = "bar"
            member this.Yield _ = ()
            member this.Zero _ = ()


        [<EntryPoint>]
        let main _ =
            let cb = CBuilder()

            let x = cb { Foo }
            let y = cb { foo }
            let z = cb { bar }
            printfn $"{x}"
            printfn $"{y}"

            if x <> "Foo" then
                failwith "not Foo"
            if y <> "foo" then
                failwith "not foo"
            if z <> "bar" then
                failwith "not bar"
            0
    """
    |> asExe
    |> compileAndRun
    |> shouldSucceed

/// Tests for empty-bodied computation expressions: builder { }
module EmptyBodied =
    /// F# 8.0 and below do not support empty-bodied computation expressions.
    module Unsupported =
        [<Fact>]
        let ``seq { } does not compile`` () =
            Fsx """
            let xs : int seq = seq { }
            """
            |> withLangVersion80
            |> asExe
            |> compile
            |> shouldFail
            |> withErrorCode 789
            |> withErrorMessage "'{ }' is not a valid expression. Records must include at least one field. Empty sequences are specified by using Seq.empty or an empty list '[]'."

        [<Fact>]
        let ``async { } does not compile`` () =
            Fsx """
            let a : Async<unit> = async { }
            """
            |> withLangVersion80
            |> asExe
            |> compile
            |> shouldFail
            |> withErrorCode 3
            |> withErrorMessage "This value is not a function and cannot be applied."

        [<Fact>]
        let ``task { } does not compile`` () =
            Fsx """
            open System.Threading.Tasks

            let t : Task<unit> = task { }
            """
            |> withLangVersion80
            |> asExe
            |> compile
            |> shouldFail
            |> withErrorCode 3
            |> withErrorMessage "This value is not a function and cannot be applied."

        [<Fact>]
        let ``builder { } does not compile`` () =
            FSharp """
            type Builder () =
                member _.Zero () = Seq.empty
                member _.Delay f = f
                member _.Run f = f ()

            let builder = Builder ()

            let xs : int seq = builder { }
            """
            |> withLangVersion80
            |> asExe
            |> compile
            |> shouldFail
            |> withErrorCode 3
            |> withErrorMessage "This value is not a function and cannot be applied."

        [<Fact>]
        let ``builder { () } and no Zero: FS0708`` () =
            FSharp """
            type Builder () =
                member _.Delay f = f
                member _.Run f = f ()

            let builder = Builder ()

            let xs : int seq = builder { () }
            """
            |> withLangVersion80
            |> asExe
            |> compile
            |> shouldFail
            |> withErrorCode 708
            |> withErrorMessage "This control construct may only be used if the computation expression builder defines a 'Zero' method"

        [<Fact>]
        let ``builder { () } ≡ seq { () } when Zero () = Seq.empty`` () =
            Fsx """
            type Builder () =
                member _.Zero () = Seq.empty
                member _.Delay f = f
                member _.Run f = f ()

            let builder = Builder ()

            if List.ofSeq (builder { () }) <> List.ofSeq (seq { () }) then
                failwith "builder { () } ≢ seq { () }"
            """
            |> withLangVersion80
            |> runFsi
            |> shouldSucceed

        [<Fact>]
        let ``Unchecked﹒defaultof<'a> { }`` () =
            FSharp """
            Unchecked.defaultof<'a> { }
            """
            |> withLangVersion80
            |> asExe
            |> compile
            |> shouldFail
            |> withErrorCode 789
            |> withErrorMessage "'{ }' is not a valid expression. Records must include at least one field. Empty sequences are specified by using Seq.empty or an empty list '[]'."

    /// F# 9.0 and above support empty-bodied computation expressions.
    module Supported =
        /// The language version that supports empty-bodied computation expressions.
        /// TODO: Update this to the appropriate version when the feature comes out of preview.
        let [<Literal>] SupportedLanguageVersion = "preview"

        [<Fact>]
        let ``seq { } ≡ seq { () }`` () =
            Fsx """
            if List.ofSeq (seq { }) <> List.ofSeq (seq { () }) then
                failwith "seq { } ≢ seq { () }"
            """
            |> withLangVersion SupportedLanguageVersion
            |> runFsi
            |> shouldSucceed

        [<Fact>]
        let ``async { } ≡ async { () }`` () =
            Fsx """
            if
                [|(); ()|] <> (
                                  [|async { }; async { () }|]
                                  |> Async.Parallel
                                  |> Async.RunSynchronously
                              )
            then
                failwith "async { } ≢ async { () }"
            """
            |> withLangVersion SupportedLanguageVersion
            |> runFsi
            |> shouldSucceed

        [<Fact>]
        let ``task { } ≡ task { () }`` () =
            Fsx """
            open System.Threading.Tasks

            // We wrap this in a function to avoid https://github.com/dotnet/fsharp/issues/12038
            let f () =
                if
                    [|(); ()|] <> Task.WhenAll(task { }, task { () }).GetAwaiter().GetResult()
                then
                    failwith "task { } ≢ task { () }"

            f ()
            """
            |> withLangVersion SupportedLanguageVersion
            |> runFsi
            |> shouldSucceed

        [<Fact>]
        let ``builder { () } and no Zero: FS0708`` () =
            FSharp """
            type Builder () =
                member _.Delay f = f
                member _.Run f = f ()

            let builder = Builder ()

            let xs : int seq = builder { () }
            """
            |> withLangVersion SupportedLanguageVersion
            |> asExe
            |> compile
            |> shouldFail
            |> withErrorCode 708
            |> withErrorMessage "This control construct may only be used if the computation expression builder defines a 'Zero' method"

        [<Fact>]
        let ``builder { } and no Zero: FS0708 and new message`` () =
            FSharp """
            type Builder () =
                member _.Delay f = f
                member _.Run f = f ()

            let builder = Builder ()

            let xs : int seq = builder { }
            """
            |> withLangVersion SupportedLanguageVersion
            |> asExe
            |> compile
            |> shouldFail
            |> withErrorCode 708
            |> withErrorMessage "An empty body may only be used if the computation expression builder defines a 'Zero' method."

        [<Fact>]
        let ``builder { } ≡ seq { } when Zero () = Seq.empty`` () =
            Fsx """
            type Builder () =
                member _.Zero () = Seq.empty
                member _.Delay f = f
                member _.Run f = f ()

            let builder = Builder ()

            if List.ofSeq (builder { }) <> List.ofSeq (seq { }) then
                failwith "builder { } ≢ seq { }"
            """
            |> withLangVersion SupportedLanguageVersion
            |> runFsi
            |> shouldSucceed

        [<Fact>]
        let ``Unchecked﹒defaultof<'a> { }`` () =
            FSharp """
            Unchecked.defaultof<'a> { }
            """
            |> withLangVersion SupportedLanguageVersion
            |> asExe
            |> compile
            |> shouldFail
            |> withErrorCode 789
            |> withErrorMessage "'{ }' is not a valid expression. Records must include at least one field. Empty sequences are specified by using Seq.empty or an empty list '[]'."
