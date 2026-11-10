// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition

open Microsoft.CodeAnalysis.CodeRefactorings

open CancellableTasks

[<ExportCodeRefactoringProvider(FSharpConstants.FSharpLanguageName, Name = "ConvertAnonymousRecord"); Shared>]
type internal FSharpConvertAnonymousRecordRefactoring [<ImportingConstructor>] () =
    inherit CodeRefactoringProvider()

    override _.ComputeRefactoringsAsync context =
        StructPropagation.registerConversion
            context
            AnonymousRecordConversion.kind
            SR.ConvertToStructAnonymousRecord
            SR.ConvertToReferenceAnonymousRecord
            (nameof FSharpConvertAnonymousRecordRefactoring)
        |> CancellableTask.startAsTask context.CancellationToken
