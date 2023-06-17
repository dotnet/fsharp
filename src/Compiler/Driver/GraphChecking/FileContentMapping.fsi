module internal rec FSharp.Compiler.GraphChecking.FileContentMapping

open System.Collections.Immutable

/// Extract the FileContentEntries from the ParsedInput of a file.
val mkFileContent: f: FileInProject -> ImmutableArray<FileContentEntry>
