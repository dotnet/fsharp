// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open Microsoft.CodeAnalysis.Classification

[<RequireQualifiedAccess>]
module internal FSharpClassificationTypes =
    let [<Literal>] UnionCase = "FSharp.UnionCase"
    let [<Literal>] Function = "FSharp.Function"
    let [<Literal>] MutableVar = "FSharp.MutableVar"
    let [<Literal>] Printf = "FSharp.Printf"
    let [<Literal>] ReferenceType = ClassificationTypeNames.ClassName
    let [<Literal>] Module = ClassificationTypeNames.ModuleName
    let [<Literal>] ValueType = ClassificationTypeNames.StructName
    let [<Literal>] Keyword = ClassificationTypeNames.Keyword
    let [<Literal>] Enum = ClassificationTypeNames.EnumName