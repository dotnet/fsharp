// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Compiler.SourceCodeServices.ServiceFormatting

open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Range

[<Sealed>]
type internal CodeFormatter =
    static member FormatSelection(fileName, selection, source, config, ast) =
        CodeFormatterImpl.formatSelection selection config fileName source ast

    static member FormatAroundCursor(fileName, cursorPos, source, config, ast) =
        CodeFormatterImpl.formatAroundCursor cursorPos config fileName source ast
    
    static member InferSelectionFromCursorPos(fileName, cursorPos, source) =
        CodeFormatterImpl.inferSelectionFromCursorPos cursorPos fileName source
        
    static member internal FormatSelectionInDocument(fileName, selection, source, config, ast) =
        CodeFormatterImpl.formatSelectionInDocument selection config fileName source ast

    static member FormatAST(ast, fileName, source, config) = CodeFormatterImpl.formatAST ast fileName source config

    static member IsValidAST ast = CodeFormatterImpl.isValidAST ast
    
    static member MakePos(line, col) = CodeFormatterImpl.makePos line col

    static member MakeRange(fileName, startLine, startCol, endLine, endCol) = 
        CodeFormatterImpl.makeRange fileName startLine startCol endLine endCol