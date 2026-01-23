namespace FSharp.Compiler.LanguageServer

type FSharpLanguageServerFeatures =
    {
        Diagnostics: bool
        SemanticHighlighting: bool
        Completion: bool
    }

    static member Default =
        {
            Diagnostics = true
            SemanticHighlighting = true
            Completion = true
        }

type FSharpLanguageServerConfig =
    {
        EnabledFeatures: FSharpLanguageServerFeatures
    }

    static member Default =
        {
            EnabledFeatures = FSharpLanguageServerFeatures.Default
        }
