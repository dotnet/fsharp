module internal rec FSharp.Compiler.GraphChecking.FileContentMapping

/// Extract the FileContentEntries from the ParsedInput of a file.
val mkFileContent: input: FSharp.Compiler.Syntax.ParsedInput -> FileContentEntry list
