// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

[<Sealed>]
type internal CodeFormatter =
    static member FormatDocumentAsync(fileName, source, config, projectOptions, checker) =
        CodeFormatterImpl.createFormatContext fileName source projectOptions checker
        |> CodeFormatterImpl.formatDocument config
    
    static member FormatDocument(fileName, source, config) =
        CodeFormatterImpl.createFormatContextNoChecker fileName source
        |> CodeFormatterImpl.formatDocument config
        |> Async.RunSynchronously
   
    static member FormatSelectionAsync(fileName, selection, source, config, projectOptions, checker) =
        CodeFormatterImpl.createFormatContext fileName source projectOptions checker
        |> CodeFormatterImpl.formatSelection selection config
    
    static member FormatSelection(fileName, selection, source, config) =
        CodeFormatterImpl.createFormatContextNoChecker fileName source
        |> CodeFormatterImpl.formatSelection selection config
        |> Async.RunSynchronously

    static member FormatAroundCursorAsync(fileName, cursorPos, source, config, projectOptions, checker) =
        CodeFormatterImpl.createFormatContext fileName source projectOptions checker
        |> CodeFormatterImpl.formatAroundCursor cursorPos config
    
    static member InferSelectionFromCursorPos(fileName, cursorPos, source) =
        CodeFormatterImpl.inferSelectionFromCursorPos cursorPos fileName source
        
    static member internal FormatSelectionInDocumentAsync(fileName, selection, source, config, projectOptions, checker) =
        CodeFormatterImpl.createFormatContext fileName source projectOptions checker
        |> CodeFormatterImpl.formatSelectionInDocument selection config

    static member FormatAST(ast, fileName, source, config) = 
        CodeFormatterImpl.formatAST ast fileName source config

    static member ParseAsync(fileName, source, projectOptions, checker) = 
        CodeFormatterImpl.createFormatContext fileName source projectOptions checker
        |> CodeFormatterImpl.parse
    
    static member Parse(fileName, source) = 
        CodeFormatterImpl.createFormatContextNoChecker fileName source
        |> CodeFormatterImpl.parse
        |> Async.RunSynchronously

    static member IsValidAST ast = 
        CodeFormatterImpl.isValidAST ast

    static member IsValidFSharpCodeAsync(fileName, source, projectOptions, checker) =
        CodeFormatterImpl.createFormatContext fileName source projectOptions checker
        |> CodeFormatterImpl.isValidFSharpCode

    static member IsValidFSharpCode(fileName, source) =
        CodeFormatterImpl.createFormatContextNoChecker fileName source
        |> CodeFormatterImpl.isValidFSharpCode
        |> Async.RunSynchronously

    static member MakePos(line, col) = 
        CodeFormatterImpl.makePos line col

    static member MakeRange(fileName, startLine, startCol, endLine, endCol) = 
        CodeFormatterImpl.makeRange fileName startLine startCol endLine endCol