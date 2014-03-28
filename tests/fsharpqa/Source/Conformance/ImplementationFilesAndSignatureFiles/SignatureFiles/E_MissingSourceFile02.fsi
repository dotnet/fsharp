// #Regression #Conformance #SignatureFiles 
#light


// Same as E-MissingSourceFile01.fsi, but only testing the deprecation message

//<Expects status="error" id="FS0010" span="(47,23)">Unexpected keyword 'lazy' in signature file\. Expected incomplete structured construct at or before this point or other token\.$</Expects>

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

    val (|Lazy|) : 'a lazy -> 'a

    /// Build a lazy (delayed) value from the given computation
    val create : (unit -> 'a) -> Lazy<'a>
