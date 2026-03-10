// #Regression #Conformance #SignatureFiles 
// Test you get an error if you specify an .fsi file but not the corresponding .fs file.

//<Expects id="FS0240" status="error" span="(6,1)">The signature file 'E_MissingSourceFile01' does not have a corresponding implementation file\. If an implementation file exists then check the 'module' and 'namespace' declarations in the signature and implementation files match</Expects>

namespace FSharp.Testing.MissingSourceFile01

open Microsoft.FSharp.Core

#nowarn "0057";; // active patterns

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
/// Delayed computations.
module Lazy = 

    type 'a t = Lazy<'a>

    exception Undefined = Microsoft.FSharp.Control.Undefined

    /// See Lazy.Force
    val force: Lazy<'a> -> 'a

    /// See Lazy.Force.
    val force_val: Lazy<'a> -> 'a

    /// Build a lazy (delayed) value from the given computation
    val lazy_from_fun: (unit -> 'a) -> Lazy<'a>

    /// Build a lazy (delayed) value from the given pre-computed value.
    val lazy_from_val: 'a -> Lazy<'a>

    /// Check if a lazy (delayed) value has already been computed
    val lazy_is_val: Lazy<'a> -> bool

    /// See Lazy.SynchronizedForce.
    val force_with_lock: Lazy<'a> -> 'a

    /// See Lazy.UnsynchronizedForce
    val force_without_lock: Lazy<'a> -> 'a

    //--------------------------------------------------------------------------
    // Active patterns for working with lazy values

    val (|Lazy|) : Lazy<'a> -> 'a

    /// Build a lazy (delayed) value from the given computation
    val create : (unit -> 'a) -> Lazy<'a>
