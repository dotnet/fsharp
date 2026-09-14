// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition

open Microsoft.CodeAnalysis.CodeRefactorings

open CancellableTasks

[<ExportCodeRefactoringProvider(FSharpConstants.FSharpLanguageName, Name = "ConvertTuple"); Shared>]
type internal FSharpConvertTupleRefactoring [<ImportingConstructor>] () =
    inherit CodeRefactoringProvider()

    override _.ComputeRefactoringsAsync context =
        StructPropagation.registerConversion
            context
            TupleConversion.kind
            SR.ConvertToStructTuple
            SR.ConvertToReferenceTuple
            (nameof FSharpConvertTupleRefactoring)
        |> CancellableTask.startAsTask context.CancellationToken
