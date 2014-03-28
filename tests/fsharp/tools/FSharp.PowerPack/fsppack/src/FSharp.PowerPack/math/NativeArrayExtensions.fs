namespace Microsoft.FSharp.NativeInterop

#nowarn "44"
#nowarn "9" // unverifiable constructs
#nowarn "51" // unverifiable constructs

open System
open System.Runtime.InteropServices
open Microsoft.FSharp.NativeInterop
open Microsoft.FSharp.Math
open Microsoft.FSharp.Compatibility

[<AutoOpen>]
module NativArrayExtensionsForMatrix =

    [<NoDynamicInvocation>]
    let inline pinObjUnscoped (obj: obj) =  GCHandle.Alloc(obj,GCHandleType.Pinned) 
    [<NoDynamicInvocation>]
    let inline pinObj (obj: obj) f = 
        let gch = pinObjUnscoped obj 
        try f gch
        finally
            gch.Free()

    type Microsoft.FSharp.NativeInterop.PinnedArray<'T when 'T : unmanaged> with

        [<NoDynamicInvocation>]
        static member inline of_vector(m:Vector<'T>) = 
            let gch = pinObjUnscoped (box m.InternalValues) 
            let ptr = &&m.InternalValues.[0]
            new PinnedArray<'T>(new NativeArray<_>(ptr,m.Length),gch)

        [<NoDynamicInvocation>]
        static member inline of_rowvec(m:RowVector<'T>) = 
            let gch = pinObjUnscoped (box m.InternalValues) 
            let ptr = &&m.InternalValues.[0]
            new PinnedArray<'T>(new NativeArray<_>(ptr,m.Length),gch)
            

    type Microsoft.FSharp.NativeInterop.PinnedArray2<'T when 'T : unmanaged> with

        [<NoDynamicInvocation>]
        static member inline of_matrix(m:Matrix<'T>) = 
            if m.IsDense then
                let gch = pinObjUnscoped (box m.InternalDenseValues) 
                let ptr = && m.InternalDenseValues.[0,0]
                new PinnedArray2<'T>(new NativeArray2<_>(ptr,m.NumRows,m.NumCols),gch) 
            else
                invalidArg "m" "cannot pin sparse matrices"
            
