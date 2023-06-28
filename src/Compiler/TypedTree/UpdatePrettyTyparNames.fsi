/// The Typars of a Val in the signature data should also be pretty named.
/// This will happen for the implementation file contents, but not for the signature data.
/// In this module some helpers will traverse the ModuleOrNamespaceType and update all the typars of each found Val.
module internal FSharp.Compiler.UpdatePrettyTyparNames

open FSharp.Compiler.TypedTree

val updateModuleOrNamespaceType: signatureData: ModuleOrNamespaceType -> unit
