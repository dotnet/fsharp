// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.CompilerOptions.fsc

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module Removed =

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/Removed)
    //<Expects id="FS0243" status="error">Unrecognized option: '--no-power-pack'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/Removed", Includes=[|"nopowerpack02.fs"|])>]
    let ``Removed - nopowerpack02.fs - --no-power-pack`` compilation =
        compilation
        |> withOptions ["--no-power-pack"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0243
        |> withDiagnosticMessageMatches "Unrecognized option: '--no-power-pack'"

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/Removed)
    //<Expects id="FS0243" status="error">Unrecognized option: '--nopowerpack'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/Removed", Includes=[|"nopowerpack01.fs"|])>]
    let ``Removed - nopowerpack01.fs - --nopowerpack`` compilation =
        compilation
        |> withOptions ["--nopowerpack"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0243
        |> withDiagnosticMessageMatches "Unrecognized option: '--nopowerpack'"

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/Removed)
    //<Expects id="FS0243" status="error">Unrecognized option: '--namespace'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/Removed", Includes=[|"namespace01.fs"|])>]
    let ``Removed - namespace01.fs - --namespace`` compilation =
        compilation
        |> withOptions ["--namespace"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0243
        |> withDiagnosticMessageMatches "Unrecognized option: '--namespace'"

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/Removed)
    //<Expects id="FS0243" status="error">Unrecognized option: '--namespace'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/Removed", Includes=[|"namespace01.fs"|])>]
    let ``Removed - namespace01.fs - --namespace Foo`` compilation =
        compilation
        |> withOptions ["--namespace"; "Foo"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0243
        |> withDiagnosticMessageMatches "Unrecognized option: '--namespace'"

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/Removed)
    //<Expects id="FS0243" status="error"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/Removed", Includes=[|"unrecognized_argument01.fs"|])>]
    let ``Removed - unrecognized_argument01.fs - -R`` compilation =
        compilation
        |> withOptions ["-R"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0243

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/Removed)
    //<Expects id="FS0243" status="error"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/Removed", Includes=[|"unrecognized_argument01.fs"|])>]
    let ``Removed - unrecognized_argument01.fs - --open`` compilation =
        compilation
        |> withOptions ["--open"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0243

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/Removed)
    //<Expects id="FS0243" status="error"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/Removed", Includes=[|"unrecognized_argument01.fs"|])>]
    let ``Removed - unrecognized_argument01.fs - --clr-mscorlib`` compilation =
        compilation
        |> withOptions ["--clr-mscorlib"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0243

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/Removed)
    //<Expects id="FS0243" status="error"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/Removed", Includes=[|"unrecognized_argument01.fs"|])>]
    let ``Removed - unrecognized_argument01.fs - --quotation-data`` compilation =
        compilation
        |> withOptions ["--quotation-data"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0243

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/Removed)
    //<Expects id="FS0243" status="error"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/Removed", Includes=[|"unrecognized_argument01.fs"|])>]
    let ``Removed - unrecognized_argument01.fs - --all-tailcalls`` compilation =
        compilation
        |> withOptions ["--all-tailcalls"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0243

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/Removed)
    //<Expects id="FS0243" status="error"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/Removed", Includes=[|"unrecognized_argument01.fs"|])>]
    let ``Removed - unrecognized_argument01.fs - --no-tailcalls`` compilation =
        compilation
        |> withOptions ["--no-tailcalls"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0243

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/Removed)
    //<Expects id="FS0243" status="error"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/Removed", Includes=[|"unrecognized_argument01.fs"|])>]
    let ``Removed - unrecognized_argument01.fs - --closures-as-virtuals`` compilation =
        compilation
        |> withOptions ["--closures-as-virtuals"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0243

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/Removed)
    //<Expects id="FS0243" status="error"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/Removed", Includes=[|"unrecognized_argument01.fs"|])>]
    let ``Removed - unrecognized_argument01.fs - --multi-entrypoint-closures`` compilation =
        compilation
        |> withOptions ["--multi-entrypoint-closures"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0243

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/Removed)
    //<Expects id="FS0243" status="error"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/Removed", Includes=[|"unrecognized_argument01.fs"|])>]
    let ``Removed - unrecognized_argument01.fs - --generate-debug-file`` compilation =
        compilation
        |> withOptions ["--generate-debug-file"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0243

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/Removed)
    //<Expects id="FS0243" status="error"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/Removed", Includes=[|"unrecognized_argument01.fs"|])>]
    let ``Removed - unrecognized_argument01.fs - --sscli`` compilation =
        compilation
        |> withOptions ["--sscli"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0243

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/Removed)
    //<Expects id="FS0243" status="error"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/Removed", Includes=[|"unrecognized_argument01.fs"|])>]
    let ``Removed - unrecognized_argument01.fs - --no-inner-polymorphism`` compilation =
        compilation
        |> withOptions ["--no-inner-polymorphism"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0243

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/Removed)
    //<Expects id="FS0243" status="error"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/Removed", Includes=[|"unrecognized_argument01.fs"|])>]
    let ``Removed - unrecognized_argument01.fs - --permit-inner-polymorphism`` compilation =
        compilation
        |> withOptions ["--permit-inner-polymorphism"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0243

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/Removed)
    //<Expects id="FS0243" status="error"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/Removed", Includes=[|"unrecognized_argument01.fs"|])>]
    let ``Removed - unrecognized_argument01.fs - --fast-sublanguage-only`` compilation =
        compilation
        |> withOptions ["--fast-sublanguage-only"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0243

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/Removed)
    //<Expects id="FS0243" status="error"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/Removed", Includes=[|"unrecognized_argument01.fs"|])>]
    let ``Removed - unrecognized_argument01.fs - --generate-config-file`` compilation =
        compilation
        |> withOptions ["--generate-config-file"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0243

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/Removed)
    //<Expects id="FS0243" status="error"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/Removed", Includes=[|"unrecognized_argument01.fs"|])>]
    let ``Removed - unrecognized_argument01.fs - --no-banner`` compilation =
        compilation
        |> withOptions ["--no-banner"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0243

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/Removed)
    //<Expects id="FS0243" status="error"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/Removed", Includes=[|"unrecognized_argument01.fs"|])>]
    let ``Removed - unrecognized_argument01.fs - --nobanner`` compilation =
        compilation
        |> withOptions ["--nobanner"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0243

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/Removed)
    //<Expects id="FS0243" status="error">Unrecognized option: '-Ooff'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/Removed", Includes=[|"deprecated_Ooff.fs"|])>]
    let ``Removed - deprecated_Ooff.fs - -Ooff`` compilation =
        compilation
        |> withOptions ["-Ooff"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0243
        |> withDiagnosticMessageMatches "Unrecognized option: '-Ooff'"

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/Removed)
    //<Expects id="FS0075" status="warning">The command-line option '--statistics' has been deprecated</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/Removed", Includes=[|"deprecated_statistics01.fs"|])>]
    let ``Removed - deprecated_statistics01.fs - --statistics`` compilation =
        compilation
        |> withOptions ["--statistics"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0075
        |> withDiagnosticMessageMatches "The command-line option '--statistics' has been deprecated"

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/Removed)
    //<Expects id="FS0075" status="warning">The command-line option '--debug-file' has been deprecated\. Use '--pdb' instead</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/Removed", Includes=[|"deprecated_debug-file01.fs"|])>]
    let ``Removed - deprecated_debug-file01.fs - -g --debug-file:foo.pdb`` compilation =
        compilation
        |> withOptions ["-g"; "--debug-file:foo.pdb"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0075
        |> withDiagnosticMessageMatches "The command-line option '--debug-file' has been deprecated\. Use '--pdb' instead"

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/Removed)
    //<Expects id="FS0075" status="warning">The command-line option '--generate-filter-blocks' has been deprecated</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/Removed", Includes=[|"deprecated_generate-filter-blocks01.fs"|])>]
    let ``Removed - deprecated_generate-filter-blocks01.fs - --generate-filter-blocks`` compilation =
        compilation
        |> withOptions ["--generate-filter-blocks"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0075
        |> withDiagnosticMessageMatches "The command-line option '--generate-filter-blocks' has been deprecated"

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/Removed)
    //<Expects id="FS0075" status="warning">The command-line option '--max-errors' has been deprecated\. Use '--maxerrors' instead\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/Removed", Includes=[|"deprecated_max-errors01.fs"|])>]
    let ``Removed - deprecated_max-errors01.fs - --max-errors:1`` compilation =
        compilation
        |> withOptions ["--max-errors:1"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0075
        |> withDiagnosticMessageMatches "The command-line option '--max-errors' has been deprecated\. Use '--maxerrors' instead\.$"

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/Removed)
    //<Expects id="FS0224" status="error">Option requires parameter: --max-errors:<n></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/Removed", Includes=[|"deprecated_max-errors02.fs"|])>]
    let ``Removed - deprecated_max-errors02.fs - --max-errors`` compilation =
        compilation
        |> withOptions ["--max-errors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0224

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/Removed)
    //<Expects id="FS0075" status="warning">The command-line option '--no-string-interning' has been deprecated</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/Removed", Includes=[|"deprecated_no-string-interning01.fs"|])>]
    let ``Removed - deprecated_no-string-interning01.fs - --no-string-interning`` compilation =
        compilation
        |> withOptions ["--no-string-interning"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0075
        |> withDiagnosticMessageMatches "The command-line option '--no-string-interning' has been deprecated"

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/Removed)
    //<Expects id="FS0075" status="warning">The command-line option '--ml-keywords' has been deprecated</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/Removed", Includes=[|"deprecated_ml-keywords01.fs"|])>]
    let ``Removed - deprecated_ml-keywords01.fs - --ml-keywords`` compilation =
        compilation
        |> withOptions ["--ml-keywords"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0075
        |> withDiagnosticMessageMatches "The command-line option '--ml-keywords' has been deprecated"

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/Removed)
    //<Expects id="FS0075" status="warning">The command-line option '--gnu-style-errors' has been deprecated</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/Removed", Includes=[|"deprecated_gnu-style-errors01.fs"|])>]
    let ``Removed - deprecated_gnu-style-errors01.fs - --gnu-style-errors`` compilation =
        compilation
        |> withOptions ["--gnu-style-errors"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0075
        |> withDiagnosticMessageMatches "The command-line option '--gnu-style-errors' has been deprecated"

