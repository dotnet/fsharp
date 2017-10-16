// Copied from https://github.com/dungpa/fantomas and modified by Vasily Kirichenko

namespace Microsoft.FSharp.Compiler.SourceCodeServices.ServiceFormatting

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

    static member MakePos(line, col) = CodeFormatterImpl.makePos line col

    static member MakeRange(fileName, startLine, startCol, endLine, endCol) = 
        CodeFormatterImpl.makeRange fileName startLine startCol endLine endCol
