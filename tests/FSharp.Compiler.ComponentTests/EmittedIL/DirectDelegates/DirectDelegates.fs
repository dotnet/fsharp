namespace EmittedIL.RealInternalSignature

open System.IO
open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module DirectDelegates =

    let private coreOptions compilation =
        compilation
        |> withOptions [ "--test:EmitFeeFeeAs100001" ]
        |> asExe
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> ignoreWarnings

    // Default langversion: direct delegates must NOT be emitted (a closure is still generated).
    // Baselines: <source>.<suffix>.il.bsl  -- these should remain unchanged when the feature lands.
    let verifyCompilation compilation =
        compilation
        |> coreOptions
        |> compile
        |> verifyILBaseline

    // Redirect the IL baseline to a distinct *.Preview.il.bsl path so the preview variant can reuse the
    // very same input .fs file (no input duplication / drift) without clobbering the default baseline.
    let private withPreviewBaseline (cUnit: CompilationUnit) : CompilationUnit =
        match cUnit with
        | FS src ->
            let baseline =
                src.Baseline
                |> Option.map (fun bsl ->
                    let path = bsl.ILBaseline.BslSource.Replace(".il.bsl", ".Preview.il.bsl")
                    let content = if File.Exists path then Some(File.ReadAllText path) else None
                    { bsl with ILBaseline = { bsl.ILBaseline with BslSource = path; Content = content } })
            FS { src with Baseline = baseline }
        | other -> other

    // Preview langversion: direct delegates ARE emitted once the feature lands.
    // Baselines: <source>.<suffix>.Preview.il.bsl  -- these are the ones that will change.
    let verifyPreviewCompilation compilation =
        compilation
        |> coreOptions
        |> withLangVersionPreview
        |> withPreviewBaseline
        |> compile
        |> verifyILBaseline

    [<Theory; FileInlineData("DelegateKnownFunction.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``DelegateKnownFunction_fs`` compilation =
        compilation |> getCompilation |> verifyCompilation

    [<Theory; FileInlineData("DelegateKnownFunction.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``DelegateKnownFunction_fs preview`` compilation =
        compilation |> getCompilation |> verifyPreviewCompilation

    [<Theory; FileInlineData("DelegateStaticMethod.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``DelegateStaticMethod_fs`` compilation =
        compilation |> getCompilation |> verifyCompilation

    [<Theory; FileInlineData("DelegateStaticMethod.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``DelegateStaticMethod_fs preview`` compilation =
        compilation |> getCompilation |> verifyPreviewCompilation

    [<Theory; FileInlineData("DelegateGenericStaticMethod.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``DelegateGenericStaticMethod_fs`` compilation =
        compilation |> getCompilation |> verifyCompilation

    [<Theory; FileInlineData("DelegateGenericStaticMethod.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``DelegateGenericStaticMethod_fs preview`` compilation =
        compilation |> getCompilation |> verifyPreviewCompilation

    [<Theory; FileInlineData("DelegateInstanceMethod.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``DelegateInstanceMethod_fs`` compilation =
        compilation |> getCompilation |> verifyCompilation

    [<Theory; FileInlineData("DelegateInstanceMethod.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``DelegateInstanceMethod_fs preview`` compilation =
        compilation |> getCompilation |> verifyPreviewCompilation

    [<Theory; FileInlineData("DelegateGenericInstanceMethod.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``DelegateGenericInstanceMethod_fs`` compilation =
        compilation |> getCompilation |> verifyCompilation

    [<Theory; FileInlineData("DelegateGenericInstanceMethod.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``DelegateGenericInstanceMethod_fs preview`` compilation =
        compilation |> getCompilation |> verifyPreviewCompilation

    [<Theory; FileInlineData("DelegateUnitArg.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``DelegateUnitArg_fs`` compilation =
        compilation |> getCompilation |> verifyCompilation

    [<Theory; FileInlineData("DelegateUnitArg.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``DelegateUnitArg_fs preview`` compilation =
        compilation |> getCompilation |> verifyPreviewCompilation

    [<Theory; FileInlineData("DelegateNegativeCases.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``DelegateNegativeCases_fs`` compilation =
        compilation |> getCompilation |> verifyCompilation

    [<Theory; FileInlineData("DelegateNegativeCases.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``DelegateNegativeCases_fs preview`` compilation =
        compilation |> getCompilation |> verifyPreviewCompilation

    [<Theory; FileInlineData("DelegateNonInlinable.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``DelegateNonInlinable_fs`` compilation =
        compilation |> getCompilation |> verifyCompilation

    [<Theory; FileInlineData("DelegateNonInlinable.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``DelegateNonInlinable_fs preview`` compilation =
        compilation |> getCompilation |> verifyPreviewCompilation

    [<Theory; FileInlineData("DelegatePartialApplication.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``DelegatePartialApplication_fs`` compilation =
        compilation |> getCompilation |> verifyCompilation

    [<Theory; FileInlineData("DelegatePartialApplication.fs", Realsig=BooleanOptions.True, Optimize=BooleanOptions.Both)>]
    let ``DelegatePartialApplication_fs preview`` compilation =
        compilation |> getCompilation |> verifyPreviewCompilation
