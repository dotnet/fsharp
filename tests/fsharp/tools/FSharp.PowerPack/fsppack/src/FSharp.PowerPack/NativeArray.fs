namespace Microsoft.FSharp.NativeInterop

#nowarn "44"
#nowarn "9" // unverifiable constructs
#nowarn "51" // The address-of operator may result in non-verifiable code. Its use is restricted to passing byrefs to functions that require them

open System
open System.Runtime.InteropServices
open Microsoft.FSharp.NativeInterop

module NativeOps = 
    [<NoDynamicInvocation>]
    let inline pinObjUnscoped (obj: obj) =  GCHandle.Alloc(obj,GCHandleType.Pinned) 
    [<NoDynamicInvocation>]
    let inline pinObj (obj: obj) f = 
        let gch = pinObjUnscoped obj 
        try f gch
        finally
            gch.Free()

[<Sealed>]
type NativeArray<'T when 'T : unmanaged>(ptr : nativeptr<'T>, len: int) =
    member x.Ptr = ptr
    [<NoDynamicInvocation>]
    member inline x.Item 
       with get n = NativePtr.get x.Ptr n
       and  set n v = NativePtr.set x.Ptr n v
    member x.Length = len

[<Sealed>]
type FortranMatrix<'T when 'T : unmanaged>(ptr : nativeptr<'T>, nrows: int, ncols:int) = 
    member x.NumCols = ncols
    member x.NumRows = nrows
    member x.Ptr = ptr
    [<NoDynamicInvocation>]
    member inline x.Item 
       with get (row,col) = NativePtr.get x.Ptr (row + col*x.NumRows)
       and  set (row,col) v = NativePtr.set x.Ptr (row + col*x.NumRows) v
    member x.NativeTranspose = new CMatrix<_>(ptr,ncols,nrows)
  
and 
  [<Sealed>]
  NativeArray2<'T when 'T : unmanaged>(ptr : nativeptr<'T>, nrows:int, ncols: int) = 
    member x.Ptr = ptr
    member x.NumRows = nrows
    member x.NumCols = ncols
    [<NoDynamicInvocation>]
    member inline x.Item 
       with get (row,col) = NativePtr.get x.Ptr (row*x.NumCols + col)
       and  set (row,col) v = NativePtr.set x.Ptr (row*x.NumCols + col) v
    member x.NativeTranspose = new FortranMatrix<_>(x.Ptr,ncols,nrows)
  
and CMatrix<'T when 'T : unmanaged> = NativeArray2<'T> 

module Ref = 
    [<NoDynamicInvocation>]
    let inline pin (ref: 'T ref) (f : nativeptr<'T> -> 'b) = 
        NativeOps.pinObj (box ref) (fun gch -> 
            f (gch.AddrOfPinnedObject() |> NativePtr.ofNativeInt))

open Microsoft.FSharp.Math

[<Sealed>]
type PinnedArray<'T when 'T : unmanaged>(narray: NativeArray<'T>, gch: GCHandle) =
    [<NoDynamicInvocation>]
    static member inline of_array(arr: 'T[]) =
        let gch = NativeOps.pinObjUnscoped (box arr) 
        let ptr = &&arr.[0]
        new PinnedArray<'T>(new NativeArray<_>(ptr,Array.length arr),gch)

    member x.Ptr = narray.Ptr
    member x.Free() = gch.Free()
    member x.Length = narray.Length
    member x.NativeArray = narray
    interface System.IDisposable with 
        member x.Dispose() = gch.Free()
        

[<Sealed>]
type PinnedArray2<'T when 'T : unmanaged>(narray: NativeArray2<'T>, gch: GCHandle) =

    [<NoDynamicInvocation>]
    static member inline of_array2(arr: 'T[,]) = 
        let gch = NativeOps.pinObjUnscoped (box arr) 
        let ptr = &&arr.[0,0]
        new PinnedArray2<'T>(new NativeArray2<_>(ptr,Array2D.length1 arr,Array2D.length2 arr),gch)

    [<NoDynamicInvocation>]
    static member inline of_array2D(arr: 'T[,]) = 
        let gch = NativeOps.pinObjUnscoped (box arr) 
        let ptr = &&arr.[0,0]
        new PinnedArray2<'T>(new NativeArray2<_>(ptr,Array2D.length1 arr,Array2D.length2 arr),gch)


    member x.Ptr = narray.Ptr
    member x.Free() = gch.Free()
    member x.NumRows = narray.NumRows
    member x.NumCols = narray.NumCols
    member x.NativeArray = narray
    interface System.IDisposable with 
        member x.Dispose() = gch.Free()


