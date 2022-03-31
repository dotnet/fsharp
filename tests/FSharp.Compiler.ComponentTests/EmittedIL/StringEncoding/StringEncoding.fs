namespace FSharp.Compiler.ComponentTests.EmittedIL

open System
open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module StringEncoding =

    //
    // What we are doing is is really making sure that strings in F# assemblies are 
    // encoded according to the specifications (which are, essentially, "do what C#
    // does even if the C# is not fully ECMA compliant")
    // 
    // Using the Normalize(...) method is just an indirect way to test this. The direct verification
    // would have been something like:
    // - compile F# code with strings (UNICODE 0x0000 -> 0xffff)
    // - open assembly with binary editor
    // - look at the encoding
    // - make sure the trailing byte is set to 0/1 accordingly.
    // 
    // Note: to keep the execution time within a reasonable limit, we only consider the range 0x0000 - 0x0123
    //       (if you look at the code you'll see that nothing really interesting happens after 0xff... so the
    //       odds of screwing up there are really small and unlikely to happen)
    //

    let verifyCompilation compilation =
        compilation
        |> asFs
        |> compile
        |> shouldSucceed

    let normalization_CSharp (form:string) start =
        let lines = [|
            yield "namespace FSharp.Compiler.ComponentTests.EmittedIL.StringEncoding.NormalizationForm"
            yield "{"
            yield "    using System;"
            yield "    using System.Collections.Generic;"
            yield "    using System.Text;"
            yield ""
            yield $"    public class Normalization{form}CSharp_%04x{start}"
            yield "    {"
            yield "        static public IEnumerable<int> Values ()"
            yield "        {"
            // BUGBUG: should be 0x123
            for a in [start .. start + 0x07F] do
                yield sprintf """            yield return (int)("\u%04x".Normalize(NormalizationForm.%s)[0]);""" a form
            yield "        }"
            yield "    }"
            yield "}"
        |]

        CSharp(lines |> Seq.fold(fun acc v -> acc + Environment.NewLine + v) "")
        |> withName (sprintf "Normalization%sLibrary_cs_%04x" form start)

    let normalization_FSharp (form:string) start =

        let lines = [|
            yield $"module FSharp.Compiler.ComponentTests.EmittedIL.StringEncoding.NormalizationForm.Normalization{form}FSharp_%04x{start}"
            yield "open System.Collections.Generic"
            yield "open System.Text"
            yield ""
            yield "let Values () = [|"
            // BUGBUG: should be 0x123
            for a in [start .. start + 0x07F] do
                yield sprintf """        yield (int)("\u%04x".Normalize(NormalizationForm.%s)[0])""" a form
            yield "    |]"
        |]

        FSharp(lines |> Seq.fold(fun acc v -> acc + Environment.NewLine + v) "")
        |> withName (sprintf "Normalization%sLibrary_fs_%04x" form start)

    //SOURCE="dummy.fs testcase.fs" PRECMD="\$FSI_PIPE --exec NormalizationFormKC.fsx > oracle.cs && \$CSC_PIPE oracle.cs && oracle.exe>testcase.fs"	# FormKC
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [| "NormalizationFormC.fs" |])>]
    let ``NormalizationForm_C`` compilation =

        compilation
        |> withReferences([normalization_CSharp "FormC" 0; normalization_FSharp "FormC" 0])
        |> withReferences([normalization_CSharp "FormC" 0x80; normalization_FSharp "FormC" 0x80])
        |>  compileExeAndRun
        |> shouldSucceed

    //SOURCE="dummy.fs testcase.fs" PRECMD="\$FSI_PIPE --exec NormalizationFormD.fsx > oracle.cs && \$CSC_PIPE oracle.cs && oracle.exe>testcase.fs"	# FormD
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NormalizationFormD.fs"|])>]
    let ``NormalizationForm_D`` compilation =
        compilation
        |> withReferences([normalization_CSharp "FormD" 0; normalization_FSharp "FormD" 0])
        |> withReferences([normalization_CSharp "FormD" 0x80; normalization_FSharp "FormD" 0x80])
        |>  compileExeAndRun
        |> shouldSucceed

    //SOURCE="dummy.fs testcase.fs" PRECMD="\$FSI_PIPE --exec NormalizationFormKC.fsx > oracle.cs && \$CSC_PIPE oracle.cs && oracle.exe>testcase.fs"	# FormKC
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NormalizationFormKC.fs"|])>]
    let ``NormalizationForm_KC`` compilation =
        compilation
        |> withReferences([normalization_CSharp "FormKC" 0; normalization_FSharp "FormKC" 0])
        |> withReferences([normalization_CSharp "FormKC" 0x80; normalization_FSharp "FormKC" 0x80])
        |>  compileExeAndRun
        |> shouldSucceed

    //SOURCE="dummy.fs testcase.fs" PRECMD="\$FSI_PIPE --exec NormalizationFormKD.fsx > oracle.cs && \$CSC_PIPE oracle.cs && oracle.exe>testcase.fs"	# FormKD
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NormalizationFormKD.fs"|])>]
    let ``NormalizationForm_KD`` compilation =
        compilation
        |> withReferences([normalization_CSharp "FormKD" 0; normalization_FSharp "FormKD" 0])
        |> withReferences([normalization_CSharp "FormKD" 0x80; normalization_FSharp "FormKD" 0x80])
        |>  compileExeAndRun
        |> shouldSucceed
