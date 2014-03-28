namespace Microsoft.FSharp.Compatibility

    open System

    [<RequireQualifiedAccess>]
    module Array2D = 

        /// Pin the given array for the duration of a single call to the given function.  A native pointer to
        /// the first element in the array is passed to the given function.  Cleanup the GCHandle associated with the 
        /// pin when the function completes, even if an exception is raised.
        [<Unverifiable>]
        [<NoDynamicInvocation>]
        val inline pin: 'T[,] -> (nativeptr<'T> -> 'Result) -> 'Result

        /// As for Array2D.pin, except that the caller is responsible for calling Free on the returned GCHandle in order
        /// to release the pin.
        [<Unverifiable>]
        [<NoDynamicInvocation>]
        val inline pinUnscoped: 'T[,] -> nativeptr<'T> *  System.Runtime.InteropServices.GCHandle
        
        [<Unverifiable>]
        [<NoDynamicInvocation>]
        [<Obsolete("This function has been renamed to 'pinUnscoped'")>]
        val inline pin_unscoped: 'T[,] -> nativeptr<'T> *  System.Runtime.InteropServices.GCHandle
