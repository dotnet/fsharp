namespace FSharp.Compiler.LanguageServer

type FSharpLanguageServerFeatures =
    {
        Diagnostics: bool
        CodeActions: bool
        SemanticTokens: bool
    }

    static member Default =
        {
            Diagnostics = true
            CodeActions = true
            SemanticTokens = true
        }

type FSharpLanguageServerConfig =
    {
        EnabledFeatures: FSharpLanguageServerFeatures
    }

    static member Default =
        {
            EnabledFeatures = FSharpLanguageServerFeatures.Default
        }
