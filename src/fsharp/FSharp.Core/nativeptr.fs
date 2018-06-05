// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.NativeInterop

open Microsoft.FSharp.Core
open Microsoft.FSharp.Collections
open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
open Microsoft.FSharp.Primitives.Basics
open Microsoft.FSharp.Core.Operators

open System
open System.Diagnostics
open System.Runtime.InteropServices

[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module NativePtr = 


    [<NoDynamicInvocation>]
    [<CompiledName("OfNativeIntInlined")>]
    let inline ofNativeInt (address:nativeint)      = (# "" address : nativeptr<'T> #)
    
    [<NoDynamicInvocation>]
    [<CompiledName("ToNativeIntInlined")>]
    let inline toNativeInt (address: nativeptr<'T>) = (# "" address : nativeint    #)

    [<NoDynamicInvocation>]
    [<CompiledName("ToVoidPtrInlined")>]
    let inline toVoidPtr (address: nativeptr<'T>) = (# "" address : voidptr #)

    [<NoDynamicInvocation>]
    [<CompiledName("OfVoidPtrInlined")>]
    let inline ofVoidPtr (address: voidptr) = (# "" address : nativeptr<'T> #)

    [<NoDynamicInvocation>]
    [<CompiledName("AddPointerInlined")>]
    let inline add (address : nativeptr<'T>) (index:int) : nativeptr<'T> = toNativeInt address + nativeint index * (# "sizeof !0" type('T) : nativeint #) |> ofNativeInt
    
    [<NoDynamicInvocation>]
    [<CompiledName("GetPointerInlined")>]
    let inline get (address : nativeptr<'T>) index = (# "ldobj !0" type ('T) (add address index) : 'T #) 
    
    [<NoDynamicInvocation>]
    [<CompiledName("SetPointerInlined")>]
    let inline set (address : nativeptr<'T>) index (value : 'T) = (# "stobj !0" type ('T) (add address index) value #)  

    [<NoDynamicInvocation>]
    [<CompiledName("ReadPointerInlined")>]
    let inline read (address : nativeptr<'T>) = (# "ldobj !0" type ('T) address : 'T #) 
    
    [<NoDynamicInvocation>]
    [<CompiledName("WritePointerInlined")>]
    let inline write (address : nativeptr<'T>) (value : 'T) = (# "stobj !0" type ('T) address value #)  
    
    [<NoDynamicInvocation>]
    [<CompiledName("StackAllocate")>]
    let inline stackalloc (count:int) : nativeptr<'T> = (# "localloc" (count * sizeof<'T>) : nativeptr<'T> #)

    [<NoDynamicInvocation>]
    [<CompiledName("ToByRefInlined")>]
    let inline toByRef (address: nativeptr<'T>) : byref<'T> = (# "" address : 'T byref  #)
