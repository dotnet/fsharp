namespace Microsoft.FSharp.Compatibility

type permutation = int -> int

[<RequireQualifiedAccess>]
module Permutation =
    /// Create a permutation by specifying the result of permuting [| 0 .. n-1 |]. For example, 
    /// Permutation.ofArray [| 1;2;0 |]  specifies a permutation that rotates all elements right one place.
    val ofArray : destinations:int array -> permutation
    [<System.Obsolete("This function has been renamed. Use 'Permutation.ofArray' instead")>]
    val of_array : destinations:int array -> permutation

    /// Create a permutation by specifying (source,destination) index pairs. For example,
    /// Permutation(3,[ (0,2);(1,0); (2,1) ]) specifies a permutation that rotates 
    /// all elements left one place. Not all elements need be given, e.g. 
    /// Permutation(5,[ (1,2);(2,1) |]) specifies a permutation that swaps elements at indexes
    /// 1 and 2.
    val ofPairs : mappings: seq<int * int> -> permutation

    [<System.Obsolete("This function has been renamed. Use 'Permutation.ofPairs' instead")>]
    val of_pairs : mappings: seq<int * int> -> permutation
    
    /// Return a swaps the given two elements over any size
    val swap: n:int -> m:int -> permutation

    /// Return a permutation that, when applied, maps index 0 to size-1, size-1 to 0 etc.
    val reversal: size:int -> permutation

    /// Return a permutation that rotates right by the given distance. If the distance
    /// is negative then a left rotation results.
    val rotation: size:int -> distance:int -> permutation
    
    /// The identity permutation over any size
    val identity : permutation
    
    val inverse : size: int -> p:permutation -> permutation
    