module internal FSharp.Compiler.GraphChecking.ExposedNames

open FSharp.Compiler.Syntax

/// The names of the types, exceptions, union cases, record fields and modules a file declares in its namespaces,
/// when a later file can only use the file by mentioning one of them.
val tryGet: input: ParsedInput -> Set<Identifier> voption
