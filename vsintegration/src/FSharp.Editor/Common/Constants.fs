// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System

[<RequireQualifiedAccess>]
module internal FSharpConstants =

    [<Literal>]
    /// "871D2A70-12A2-4e42-9440-425DD92A4116"
    let packageGuidString = "871D2A70-12A2-4e42-9440-425DD92A4116"

    [<Literal>]
    /// "871D2A70-12A2-4e42-9440-425DD92A4116" - FSharp Package
    let fsiPackageGuidString = "871D2A70-12A2-4e42-9440-425DD92A4116"

    [<Literal>]
    /// "BC6DD5A5-D4D6-4dab-A00D-A51242DBAF1B"
    let languageServiceGuidString = "BC6DD5A5-D4D6-4dab-A00D-A51242DBAF1B"

    [<Literal>]
    /// "91a04a73-4f2c-4e7c-ad38-c1a68e7da05c"
    let projectPackageGuidString = "91a04a73-4f2c-4e7c-ad38-c1a68e7da05c"

    [<Literal>]
    /// "F#"
    let FSharpLanguageName = "F#"

    [<Literal>]
    /// "F#"
    let FSharpContentTypeName = "F#"

    [<Literal>]
    /// ".fs"
    let FSharpFileExtension = ".fs"

    [<Literal>]
    /// "F# Signature Help"
    let FSharpSignatureHelpContentTypeName = "F# Signature Help"

    [<Literal>]
    /// "F# Language Service"
    let FSharpLanguageServiceCallbackName = "F# Language Service"

    [<Literal>]
    let copilotSymbolProviderName =
        "Microsoft.VisualStudio.FSharp.CopilotSymbolContextProvider"

    [<Literal>]
    /// "FSharp"
    let FSharpLanguageLongName = "FSharp"

    [<Literal>]
    /// "F# Miscellaneous Files"
    let FSharpMiscellaneousFilesName = "F# Miscellaneous Files"

    [<Literal>]
    /// "F# Metadata"
    let FSharpMetadataName = "F# Metadata"

[<RequireQualifiedAccess>]
module internal FSharpProviderConstants =

    [<Literal>]
    /// "Session Capturing Quick Info Source Provider"
    let SessionCapturingProvider = "Session Capturing Quick Info Source Provider"

[<RequireQualifiedAccess>]
module internal Guids =

    let blueHighContrastThemeId = Guid "{ce94d289-8481-498b-8ca9-9b6191a315b9}"

[<RequireQualifiedAccess>]
module internal CodeFix =

    [<Literal>]
    let AddParentheses = "AddParentheses"

    [<Literal>]
    let AddMissingAttributeToSignature = "AddMissingAttributeToSignature"

    [<Literal>]
    let RemoveExtraAttributeFromImplementation =
        "RemoveExtraAttributeFromImplementation"

    [<Literal>]
    let AddTypeAnnotationToObjectOfIndeterminateType =
        "AddTypeAnnotationToObjectOfIndeterminateType"

    [<Literal>]
    let AddMissingRecToMutuallyRecFunctions = "AddMissingRecToMutuallyRecFunctions"

    [<Literal>]
    let ConvertToAnonymousRecord = "ConvertToAnonymousRecord"

    [<Literal>]
    let AddInstanceMemberParameter = "AddInstanceMemberParameter"

    [<Literal>]
    let ConvertCSharpLambdaToFSharpLambda = "ConvertCSharpLambdaToFSharpLambda"

    [<Literal>]
    let ConvertToNotEqualsEqualityExpression = "ConvertToNotEqualsEqualityExpression"

    [<Literal>]
    let UseTripleQuotedInterpolation = "UseTripleQuotedInterpolation"

    [<Literal>]
    let SimplifyName = "SimplifyName"

    [<Literal>]
    let RemoveUnusedBinding = "RemoveUnusedBinding"

    [<Literal>]
    let ChangeToUpcast = "ChangeToUpcast"

    [<Literal>]
    let ChangeEqualsInFieldTypeToColon = "ChangeEqualsInFieldTypeToColon"

    [<Literal>]
    let UseMutationWhenValueIsMutable = "UseMutationWhenValueIsMutable"

    [<Literal>]
    let RenameUnusedValue = "RenameUnusedValue"

    [<Literal>]
    let PrefixUnusedValue = "PrefixUnusedValue"

    [<Literal>]
    let FixIndexerAccess = "FixIndexerAccess"

    [<Literal>]
    let ImplementInterface = "ImplementInterface"

    [<Literal>]
    let RemoveReturnOrYield = "RemoveReturnOrYield"

    [<Literal>]
    let ReplaceWithSuggestion = "ReplaceWithSuggestion"

    [<Literal>]
    let MakeOuterBindingRecursive = "MakeOuterBindingRecursive"

    [<Literal>]
    let ConvertToSingleEqualsEqualityExpression =
        "ConvertToSingleEqualsEqualityExpression"

    [<Literal>]
    let MakeDeclarationMutable = "MakeDeclarationMutable"

    [<Literal>]
    let MissingReference = "MissingReference"

    [<Literal>]
    let ChangePrefixNegationToInfixSubtraction =
        "ChangePrefixNegationToInfixSubtraction"

    [<Literal>]
    let AddMissingFunKeyword = "AddMissingFunKeyword"

    [<Literal>]
    let ProposeUppercaseLabel = "ProposeUppercaseLabel"

    [<Literal>]
    let AddNewKeyword = "AddNewKeyword"

    [<Literal>]
    let RemoveUnusedOpens = "RemoveUnusedOpens"

    [<Literal>]
    let AddOpen = "AddOpen"

    [<Literal>]
    let ConvertCSharpUsingToFSharpOpen = "ConvertCSharpUsingToFSharpOpen"

    [<Literal>]
    let ChangeRefCellDerefToNotExpression = "ChangeRefCellDerefToNotExpression"

    [<Literal>]
    let AddMissingEqualsToTypeDefinition = "AddMissingEqualsToTypeDefinition"

    [<Literal>]
    let FSharpRenameParamToMatchSignature = "FSharpRenameParamToMatchSignature"

    [<Literal>]
    let RemoveIndexerDotBeforeBracket = "RemoveIndexerDotBeforeBracket"

    [<Literal>]
    let RemoveSuperfluousCapture = "RemoveSuperfluousCapture"

    [<Literal>]
    let RemoveUnnecessaryParentheses = "RemoveUnnecessaryParentheses"

    [<Literal>]
    let AddMissingSeq = "AddMissingSeq"
