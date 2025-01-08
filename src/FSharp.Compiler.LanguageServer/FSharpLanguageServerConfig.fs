namespace FSharp.Compiler.LanguageServer

type FSharpLanguageServerFeatures =
    {
        Diagnostics: bool
        SemanticHighlighting: bool
    }

    static member Default =
        {
            Diagnostics = true
            SemanticHighlighting = true
        }

type FSharpLanguageServerConfig =
    {
        EnabledFeatures: FSharpLanguageServerFeatures
    }

    static member Default =
        {
            EnabledFeatures = FSharpLanguageServerFeatures.Default
        }
