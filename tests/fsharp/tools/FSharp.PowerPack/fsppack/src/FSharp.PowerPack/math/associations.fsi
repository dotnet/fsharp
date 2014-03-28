namespace Microsoft.FSharp.Math

/// Associations are a way of associating dictionaries of
/// operations with given types at runtime.  Associations are global to a 
/// .NET application domain.  Once specified an association may not be deleted
/// or modified.
///
/// In this release the system of associations is simply 
/// limited to a registry of types that support dictionaries (i.e. interface objects)
/// of numeric operations.  The following types are pre-registered with associated numeric
/// operations: float, int32, int64, bigint, float32, Complex, bignum.  Other types must be
/// registered explicitly by user code.
///
module GlobalAssociations =

    open Microsoft.FSharp.Math

    /// Attempt to determine a numeric association for the given type, i.e. a registered dictionary of
    /// numeric operations.  The interface can be queried dynamically for additional functionality in the numerics
    /// hierarchy.
    val GetNumericAssociation : unit -> INumeric<'a> 

    val TryGetNumericAssociation : unit -> INumeric<'a>  option
    /// Record an AppDomain-wide association between the given type and the given dictionary of
    /// numeric operations.  Raise an error if an existing association already exists. 
    val RegisterNumericAssociation : INumeric<'a> -> unit

