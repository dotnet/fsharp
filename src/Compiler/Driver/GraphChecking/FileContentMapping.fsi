module internal rec FSharp.Compiler.GraphChecking.FileContentMapping

open FSharp.Compiler.Syntax

/// Extract the FileContentEntries from the ParsedInput of a file.
val mkFileContent: f: FileInProject -> FileContentEntry list

/// The entries of one declaration of an implementation file, including those only declaration content reads.
val visitSynModuleDecl: decl: SynModuleDecl -> FileContentEntry list

/// The entries of attributes, including those only declaration content reads.
val visitSynAttributes: attributes: SynAttributes -> FileContentEntry list
