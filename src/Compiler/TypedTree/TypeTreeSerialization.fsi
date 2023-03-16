/// Helper code to serialize the typed tree to json
/// This code is invoked via the `--test:DumpSignatureData` flag.
module internal FSharp.Compiler.TypeTreeSerialization

open FSharp.Compiler.TypedTree

/// Serialize an entity to a very basic json structure.
val serializeEntity: path: string -> entity: Entity -> unit
