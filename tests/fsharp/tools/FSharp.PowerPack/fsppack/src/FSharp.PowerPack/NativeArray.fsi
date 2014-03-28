namespace Microsoft.FSharp.NativeInterop

open System.Runtime.InteropServices

/// This type wraps a pointer to a blob of unmanaged memory assumed to contain
/// a C-style one-dimensional array of items compatible with the (presumably blittable) 
/// type 'T.  The blob of memory must be allocated and managed externally, 
/// e.g. by a computation routine written in C.
///
/// All operations on this type are marked inlined
/// because the code used to implement the operations is not verifiable.  
///
/// Any code that uses these operations will be unverifiable and may 
/// cause memory corruption if not used with extreme care.
[<Sealed>]
type NativeArray<'T when 'T : unmanaged> =

    /// Creates a C-style one dimensional array from a native pointer and the length of the array
    /// Nothing is actually copied.
    new : startAddress: nativeptr<'T> * length: int -> NativeArray<'T>

    /// Pointer to the C-style one-dimensional array
    member Ptr: nativeptr<'T>

    /// Get or set an entry in the array
    [<Unverifiable>]
    [<NoDynamicInvocation>]
    member inline Item : int -> 'T with get,set

    /// Length of the C-style one-dimensional array
    member Length : int

/// This type wraps a pointer to a blob of unmanaged memory assumed to contain
/// a C-style row major two-dimensional matrix of items compatible with the (presumably blittable) 
/// type 'T. The blob of memory must be allocated and managed externally, 
/// e.g. by a computation routine written in C.
///
/// All operations on this type are marked inlined
/// because the code used to implement the operations is not verifiable.  
///
/// Any code that uses these operations will be unverifiable and may 
/// cause memory corruption if not used with extreme care.

[<Sealed>]
type NativeArray2<'T when 'T : unmanaged> =
    /// Creates a C-style row major two-dimensional array from a native pointer, the number of rows and the number of columns.  
    /// Nothing is actually copied.
    new : nativeptr<'T> * nrows:int * ncols:int -> NativeArray2<'T>

    /// Pointer to the C-style row major two-dimensional array 
    member Ptr: nativeptr<'T>

    /// Get the number of rows of the native array
    member NumRows : int

    /// Get the number of columns of the native array
    member NumCols : int

    /// Get or set an entry in the array
    [<Unverifiable>]
    [<NoDynamicInvocation>]
    member inline Item : int * int -> 'T with get,set

    /// View a CMatrix as a FortranMatrix.  Doesn't actually allocate
    /// a new matirx - just gives a different label to the same bits, and swaps the
    /// row/column count information associated with the bits.
    member NativeTranspose : FortranMatrix<'T>

/// See NativeArray2
and CMatrix<'T  when 'T : unmanaged> = NativeArray2<'T> 

/// This type wraps a pointer to a blob of unmanaged memory assumed to contain
/// a Fortran-style column major two-dimensional matrix of items compatible with the (presumably blittable) 
/// type 'T. The blob of memory must be allocated and managed externally, 
/// e.g. by a computation routine written in C.
///
/// All operations on this type are marked inlined
/// because the code used to implement the operations is not verifiable.  
///
/// Any code that uses these operations will be unverifiable and may 
/// cause memory corruption if not used with extreme care.
and 
   [<Sealed>]
   FortranMatrix<'T when 'T : unmanaged> =
    new : nativeptr<'T> * nrows:int * ncols:int -> FortranMatrix<'T>

    member Ptr: nativeptr<'T>

    member NumRows : int
    member NumCols : int

    /// Get or set an entry in the array
    [<Unverifiable>]
    [<NoDynamicInvocation>]
    member inline Item : int * int -> 'T with get,set
    
    /// View a FortranMatrix as a CMatrix.  Doesn't actually allocate
    /// a new matirx - just gives a different label to the same bits, and swaps the
    /// row/column count information associated with the bits.
    member NativeTranspose : CMatrix<'T>
  
module Ref =
    /// Pin the given ref for the duration of a single call to the given function.  A native pointer to
    /// the contents of the ref is passed to the given function.  Cleanup the GCHandle associated with the 
    /// pin when the function completes, even if an exception is raised.
    [<Unverifiable>]
    [<NoDynamicInvocation>]
    val inline pin : 'T ref -> (nativeptr<'T> -> 'U) -> 'U

/// Represents a pinned handle to a structure with an underlying 1D array, i.e. an underlying NativeArray.
/// Used when interfacing with native code math libraries such as LAPACK.
[<Sealed>]
type PinnedArray<'T  when 'T : unmanaged> =

    new : NativeArray<'T> * GCHandle -> PinnedArray<'T>

    interface System.IDisposable 
    member Ptr : nativeptr<'T> 

    member Length : int 

    member NativeArray : NativeArray<'T>

    /// For native interop. Pin the given object
    [<NoDynamicInvocation>]
    static member inline of_array : 'T[] -> PinnedArray<'T>

    member Free : unit -> unit

/// Represents a pinned handle to a structure with an underlying 2D array, i.e. an underlying NativeArray2.
/// Used when interfacing with native code math libraries such as LAPACK.
[<Sealed>]
type PinnedArray2<'T when 'T : unmanaged> =

    interface System.IDisposable 
    new : NativeArray2<'T> * GCHandle -> PinnedArray2<'T> 

    member Ptr : nativeptr<'T> 

    member NumRows : int 

    member NumCols : int 

    member NativeArray : NativeArray2<'T>

    /// For native interop. Pin the given object
    [<NoDynamicInvocation>]
    [<System.Obsolete("This method has been renamed to of_array2D")>]
    static member inline of_array2 : 'T[,] -> PinnedArray2<'T>

    /// For native interop. Pin the given object
    [<NoDynamicInvocation>]
    static member inline of_array2D : 'T[,] -> PinnedArray2<'T>

    member Free : unit -> unit

