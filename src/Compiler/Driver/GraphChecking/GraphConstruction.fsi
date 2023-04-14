module internal FSharp.Compiler.GraphChecking.GraphConstructor

open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.Syntax

/// Auxiliary type for re-using signature information in TcEnvFromImpls.
///
/// TcState has two typing environments: TcEnvFromSignatures && TcEnvFromImpls
/// When type checking a file, depending on the type (implementation or signature), it will use one of these typing environments (TcEnv).
/// Checking a file will populate the respective TcEnv.
///
/// When a file has a dependencies, the information of the signature file in case a pair (implementation file backed by a signature) will suffice to type-check that file.
/// Example: if `B.fs` has a dependency on `A`, the information of `A.fsi` is enough for `B.fs` to type-check, on condition that information is available in the TcEnvFromImpls.
/// We introduce a special ArtificialImplFile node in the graph to satisfy this. `B.fs -> [ A.fsi ]` becomes `B.fs -> [ ArtificialImplFile A ].
/// The `ArtificialImplFile A` node will duplicate the signature information which A.fsi provided earlier.
/// Processing a `ArtificialImplFile` node will add the information from the TcEnvFromSignatures to the TcEnvFromImpls.
/// This means `A` will be known in both TcEnvs and therefor `B.fs` can be type-checked.
/// By doing this, we can speed up the graph processing as type checking a signature file is less expensive than its implementation counterpart.
///
/// When we need to actually type-check an implementation file backed by a signature, we cannot have the duplicate information of the signature file present in TcEnvFromImpls.
/// Example `A.fs -> [ A.fsi ]`. An implementation file always depends on its signature.
/// Type-checking `A.fs` will add the actual information to TcEnvFromImpls and we do not depend on the `ArtificialImplFile A` for `A.fs`.
///
/// In order to deal correctly with the `ArtificialImplFile` logic, we need to transform the resolved graph to contain the additional pair nodes.
/// After we have type-checked the graph, we exclude the ArtificialImplFile nodes as they are not actual physical files in our project.
[<RequireQualifiedAccess>]
type NodeToTypeCheck =
    /// A real physical file in the current project.
    /// This can be either an implementation or a signature file.
    | PhysicalFile of fileIndex: FileIndex
    /// An artificial node that will add the earlier processed signature information to the TcEnvFromImpls.
    /// Dependants on this type of node will perceive that a file is known in both TcEnvFromSignatures and TcEnvFromImpls.
    /// Even though the actual implementation file was not type-checked.
    | ArtificialImplFile of signatureFileIndex: FileIndex

    member FileIndex: FileIndex

val constructGraphs: tcConfig: TcConfig option -> sourceFiles: FileInProject array -> Graph<NodeToTypeCheck>

val getDependencyGraph: inputs: ParsedInput list -> FSharp.Compiler.GraphChecking.Graph<int * string>
