// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.NativeInterop

open Microsoft.FSharp.Core
open Microsoft.FSharp.Collections

/// <summary>Contains operations on native pointers. Use of these operators may
/// result in the generation of unverifiable code.</summary>
///
/// <namespacedoc><summary>
///   Library functionality for native interopability. See 
///   also <a href="https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/functions/external-functions">F# External Functions</a> in 
///   the F# Language Guide.
/// </summary></namespacedoc>
[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module NativePtr =

    /// <summary>Returns a typed native pointer for a given machine address.</summary>
    ///
    /// <param name="address">The machine address.</param>
    ///
    /// <returns>A typed native pointer.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<Unverifiable>]
    [<CompiledName("OfNativeIntInlined")>]
    val inline ofNativeInt : address: nativeint -> nativeptr<'T>

    /// <summary>Returns a machine address for a given typed native pointer.</summary>
    ///
    /// <param name="address">The typed native pointer.</param>
    ///
    /// <returns>The machine address.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<Unverifiable>]
    [<CompiledName("ToNativeIntInlined")>]
    val inline toNativeInt : address: nativeptr<'T> -> nativeint

    /// <summary>Returns a typed native pointer for a untyped native pointer.</summary>
    ///
    /// <param name="address">The untyped native pointer.</param>
    ///
    /// <returns>A typed native pointer.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<Unverifiable>]
    [<CompiledName("OfVoidPtrInlined")>]
    val inline ofVoidPtr : address: voidptr -> nativeptr<'T>

    /// <summary>Returns an untyped native pointer for a given typed native pointer.</summary>
    ///
    /// <param name="address">The typed native pointer.</param>
    ///
    /// <returns>An untyped native pointer.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<Unverifiable>]
    [<CompiledName("ToVoidPtrInlined")>]
    val inline toVoidPtr : address: nativeptr<'T> -> voidptr

    /// <summary>Returns a typed native pointer for a Common IL (Intermediate Language) signature pointer.</summary>
    ///
    /// <param name="address">The Common IL signature pointer.</param>
    ///
    /// <returns>A typed native pointer.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<Unverifiable>]
    [<CompiledName("OfILSigPtrInlined")>]
    val inline ofILSigPtr : address: ilsigptr<'T> -> nativeptr<'T>

    /// <summary>Returns a Common IL (Intermediate Language) signature pointer for a given typed native pointer.</summary>
    ///
    /// <param name="address">The typed native pointer.</param>
    ///
    /// <returns>A Common IL signature pointer.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<Unverifiable>]
    [<CompiledName("ToILSigPtrInlined")>]
    val inline toILSigPtr : address: nativeptr<'T> -> ilsigptr<'T>

    /// <summary>Converts a given typed native pointer to a managed pointer.</summary>
    ///
    /// <param name="address">The typed native pointer.</param>
    ///
    /// <returns>The managed pointer.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<Unverifiable>]
    [<CompiledName("ToByRefInlined")>]
    val inline toByRef: address: nativeptr<'T> -> byref<'T>

    /// <summary>Returns a typed native pointer by adding index * sizeof&lt;'T&gt; to the 
    /// given input pointer.</summary>
    ///
    /// <param name="address">The input pointer.</param>
    /// <param name="index">The index by which to offset the pointer.</param>
    ///
    /// <returns>A typed pointer.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<Unverifiable>]
    [<CompiledName("AddPointerInlined")>]
    val inline add : address: nativeptr<'T> -> index: int -> nativeptr<'T>

    /// <summary>Dereferences the typed native pointer computed by adding index * sizeof&lt;'T&gt; to the 
    /// given input pointer.</summary>
    ///
    /// <param name="address">The input pointer.</param>
    /// <param name="index">The index by which to offset the pointer.</param>
    ///
    /// <returns>The value at the pointer address.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<Unverifiable>]
    [<CompiledName("GetPointerInlined")>]
    val inline get : address: nativeptr<'T> -> index: int -> 'T

    /// <summary>Dereferences the given typed native pointer.</summary>
    ///
    /// <param name="address">The input pointer.</param>
    ///
    /// <returns>The value at the pointer address.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<Unverifiable>]
    [<CompiledName("ReadPointerInlined")>]
    val inline read : address: nativeptr<'T> -> 'T

    /// <summary>Assigns the <c>value</c> into the memory location referenced by the given typed native pointer.</summary>
    ///
    /// <param name="address">The input pointer.</param>
    /// <param name="value">The value to assign.</param>
    /// 
    /// <example-tbd></example-tbd>
    [<Unverifiable>]
    [<CompiledName("WritePointerInlined")>]
    val inline write : address: nativeptr<'T> -> value: 'T -> unit

    /// <summary>Assigns the <c>value</c> into the memory location referenced by the typed native 
    /// pointer computed by adding index * sizeof&lt;'T&gt; to the given input pointer.</summary>
    ///
    /// <param name="address">The input pointer.</param>
    /// <param name="index">The index by which to offset the pointer.</param>
    /// <param name="value">The value to assign.</param>
    /// 
    /// <example-tbd></example-tbd>
    [<Unverifiable>]
    [<CompiledName("SetPointerInlined")>]
    val inline set : address: nativeptr<'T> -> index: int -> value: 'T -> unit

    /// <summary>Allocates a region of memory on the stack.</summary>
    ///
    /// <param name="count">The number of objects of type T to allocate.</param>
    ///
    /// <returns>A typed pointer to the allocated memory.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<Unverifiable>]
    [<CompiledName("StackAllocate")>]
    val inline stackalloc: count: int -> nativeptr<'T>
    
    /// <summary>Gets the null native pointer.</summary>
    ///
    /// <returns>The null native pointer.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<Unverifiable>]
    [<GeneralizableValue>]
    [<CompiledName("NullPointer")>]
    val inline nullPtr<'T when 'T : unmanaged> : nativeptr<'T>
    
    /// <summary>Tests whether the given native pointer is null.</summary>
    ///
    /// <param name="address">The input pointer.</param>
    ///
    /// <returns>Whether the given native pointer is null.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<Unverifiable>]
    [<CompiledName("IsNullPointer")>]
    val inline isNullPtr: address: nativeptr<'T> -> bool
    
    /// <summary>Clears the value stored at the location of a given native pointer.</summary>
    ///
    /// <param name="address">The input pointer.</param>
    /// 
    /// <example-tbd></example-tbd>
    [<Unverifiable>]
    [<CompiledName("ClearPointerInlined")>]
    val inline clear : address: nativeptr<'T> -> unit

    /// <summary>Initializes a specified block of memory starting at a specific address to a given byte count and initial byte value.</summary>
    ///
    /// <param name="address">The input pointer.</param>
    /// <param name="value">The initial byte value.</param>
    /// <param name="count">The total repeat count of the byte value.</param>
    /// 
    /// <example-tbd></example-tbd>
    [<Unverifiable>]
    [<CompiledName("InitializeBlockInlined")>]
    val inline initBlock : address: nativeptr<'T> -> value: byte -> count: uint32 -> unit

    /// <summary>Copies a value to a specified destination address from a specified source address.</summary>
    ///
    /// <param name="destination">The destination pointer.</param>
    /// <param name="source">The source pointer.</param>
    /// 
    /// <example-tbd></example-tbd>
    [<Unverifiable>]
    [<CompiledName("CopyPointerInlined")>]
    val inline copy : destination: nativeptr<'T> -> source: nativeptr<'T> -> unit

    /// <summary>Copies a block of memory to a specified destination address starting from a specified source address until a specified byte count of (count * sizeof&lt;'T&gt;).</summary>
    ///
    /// <param name="destination">The destination pointer.</param>
    /// <param name="source">The source pointer.</param>
    /// <param name="count">The source pointer.</param>
    /// 
    /// <example-tbd></example-tbd>
    [<Unverifiable>]
    [<CompiledName("CopyBlockInlined")>]
    val inline copyBlock : destination: nativeptr<'T> -> source: nativeptr<'T> -> count: int -> unit

