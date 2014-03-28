namespace Microsoft.FSharp.NativeInterop

open Microsoft.FSharp.Math
open System.Runtime.InteropServices

[<AutoOpen>]
module NativArrayExtensionsForMatrix =

    type Microsoft.FSharp.NativeInterop.PinnedArray<'T when 'T : unmanaged> with

        /// For native interop. Pin the given object
        [<NoDynamicInvocation>]
        static member inline of_vector : Vector<'T> -> PinnedArray<'T>

        /// For native interop. Pin the given object
        [<NoDynamicInvocation>]
        static member inline of_rowvec : RowVector<'T> -> PinnedArray<'T>


    type Microsoft.FSharp.NativeInterop.PinnedArray2<'T when 'T : unmanaged> with

        /// For native interop. Pin the given object
        [<NoDynamicInvocation>]
        static member inline of_matrix : Matrix<'T> -> PinnedArray2<'T>

