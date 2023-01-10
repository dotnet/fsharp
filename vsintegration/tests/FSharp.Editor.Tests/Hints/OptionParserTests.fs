// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests.Hints

open Xunit
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.Hints
open Microsoft.VisualStudio.FSharp.Editor.Hints.Hints

// best tests ever - very scalable
module OptionParserTests =

    [<Fact>]
    let ``Type hints off, parameter name hints off`` () =
        let options =
            { AdvancedOptions.Default with
                IsInlineTypeHintsEnabled = false
                IsInlineParameterNameHintsEnabled = false
            }

        let expected = []

        let actual = OptionParser.getHintKinds options

        Assert.Equal(expected, actual)

    [<Fact>]
    let ``Type hints on, parameter name hints off`` () =
        let options =
            { AdvancedOptions.Default with
                IsInlineTypeHintsEnabled = true
                IsInlineParameterNameHintsEnabled = false
            }

        let expected = [ HintKind.TypeHint ]

        let actual = OptionParser.getHintKinds options

        Assert.Equal(expected, actual)

    [<Fact>]
    let ``Type hints off, parameter name hints on`` () =
        let options =
            { AdvancedOptions.Default with
                IsInlineTypeHintsEnabled = false
                IsInlineParameterNameHintsEnabled = true
            }

        let expected = [ HintKind.ParameterNameHint ]

        let actual = OptionParser.getHintKinds options

        Assert.Equal(expected, actual)

    [<Fact>]
    let ``Type hints on, parameter name hints on`` () =
        let options =
            { AdvancedOptions.Default with
                IsInlineTypeHintsEnabled = true
                IsInlineParameterNameHintsEnabled = true
            }

        let expected = [ HintKind.TypeHint; HintKind.ParameterNameHint ]

        let actual = OptionParser.getHintKinds options

        Assert.Equal(expected, actual)
