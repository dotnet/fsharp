// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.NativeInterop

    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Collections

    [<RequireQualifiedAccess>]
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    /// <summary>Contains operations on native pointers. Use of these operators may
    /// result in the generation of unverifiable code.</summary>
    module NativePtr =

        [<Unverifiable>]
        [<CompiledName("OfNativeIntInlined")>]
        /// <summary>Returns a typed native pointer for a given machine address.</summary>
        /// <param name="address">The pointer address.</param>
        /// <returns>A typed pointer.</returns>
        val inline ofNativeInt : address:nativeint -> nativeptr<'T>

        [<Unverifiable>]
        [<CompiledName("ToVoidPtrInlined")>]
        /// <summary>Returns an untyped native pointer for a given typed pointer.</summary>
        /// <param name="address">The pointer address.</param>
        /// <returns>A typed pointer.</returns>
        val inline toVoidPtr : address:nativeptr<'T> -> voidptr

        [<Unverifiable>]
        [<CompiledName("OfVoidPtrInlined")>]
        /// <summary>Returns a typed native pointer for a untyped native pointer.</summary>
        /// <param name="address">The untyped pointer.</param>
        /// <returns>A typed pointer.</returns>
        val inline ofVoidPtr : voidptr -> nativeptr<'T>

        [<Unverifiable>]
        [<CompiledName("ToNativeIntInlined")>]
        /// <summary>Returns a machine address for a given typed native pointer.</summary>
        /// <param name="address">The input pointer.</param>
        /// <returns>The machine address.</returns>
        val inline toNativeInt : address:nativeptr<'T> -> nativeint


        [<Unverifiable>]
        [<CompiledName("AddPointerInlined")>]
        /// <summary>Returns a typed native pointer by adding index * sizeof&lt;'T&gt; to the 
        /// given input pointer.</summary>
        /// <param name="address">The input pointer.</param>
        /// <param name="index">The index by which to offset the pointer.</param>
        /// <returns>A typed pointer.</returns>
        val inline add : address:nativeptr<'T> -> index:int -> nativeptr<'T>

        [<Unverifiable>]
        [<CompiledName("GetPointerInlined")>]
        /// <summary>Dereferences the typed native pointer computed by adding index * sizeof&lt;'T&gt; to the 
        /// given input pointer.</summary>
        /// <param name="address">The input pointer.</param>
        /// <param name="index">The index by which to offset the pointer.</param>
        /// <returns>The value at the pointer address.</returns>
        val inline get : address:nativeptr<'T> -> index:int -> 'T

        [<Unverifiable>]
        [<CompiledName("ReadPointerInlined")>]
        /// <summary>Dereferences the given typed native pointer.</summary>
        /// <param name="address">The input pointer.</param>
        /// <returns>The value at the pointer address.</returns>
        val inline read : address:nativeptr<'T> -> 'T

        [<Unverifiable>]
        [<CompiledName("WritePointerInlined")>]
        /// <summary>Assigns the <c>value</c> into the memory location referenced by the given typed native pointer.</summary>
        /// <param name="address">The input pointer.</param>
        /// <param name="value">The value to assign.</param>
        val inline write : address:nativeptr<'T> -> value:'T -> unit

        [<Unverifiable>]
        [<CompiledName("SetPointerInlined")>]
        /// <summary>Assigns the <c>value</c> into the memory location referenced by the typed native 
        /// pointer computed by adding index * sizeof&lt;'T&gt; to the given input pointer.</summary>
        /// <param name="address">The input pointer.</param>
        /// <param name="index">The index by which to offset the pointer.</param>
        /// <param name="value">The value to assign.</param>
        val inline set : address:nativeptr<'T> -> index:int -> value:'T -> unit

        /// <summary>Allocates a region of memory on the stack.</summary>
        /// <param name="count">The number of objects of type T to allocate.</param>
        /// <returns>A typed pointer to the allocated memory.</returns>
        [<Unverifiable>]
        [<CompiledName("StackAllocate")>]
        val inline stackalloc : count:int -> nativeptr<'T>

        /// <summary>Converts a given typed native pointer to a managed pointer.</summary>
        /// <param name="address">The input pointer.</param>
        /// <returns>The managed pointer.</returns>
        [<Unverifiable>]
        [<CompiledName("ToByRefInlined")>]
        val inline toByRef : nativeptr<'T> -> byref<'T>        
