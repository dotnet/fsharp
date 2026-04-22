namespace FSharp.Compiler.LanguageServer

type FSharpLanguageServerFeatures =
    {
        Diagnostics: bool
        CodeActions: bool
    }

    static member Default =
        {
            Diagnostics = true
            CodeActions = false
        }

type FSharpLanguageServerConfig =
    {
        EnabledFeatures: FSharpLanguageServerFeatures
    }

    static member Default =
        {
            EnabledFeatures = FSharpLanguageServerFeatures.Default
        }
