namespace FSharp.Compiler.LanguageServer

type FSharpLanguageServerFeatures =
    {
        Diagnostics: bool
        SemanticHighlighting: bool
        Definition: bool
    }

    static member Default =
        {
            Diagnostics = true
            SemanticHighlighting = true
            Definition = true
        }

type FSharpLanguageServerConfig =
    {
        EnabledFeatures: FSharpLanguageServerFeatures
    }

    static member Default =
        {
            EnabledFeatures = FSharpLanguageServerFeatures.Default
        }
