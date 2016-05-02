// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

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
    let inline ofNativeInt (x:nativeint)      = (# "" x : nativeptr<'T> #)
    
    [<NoDynamicInvocation>]
    [<CompiledName("ToNativeIntInlined")>]
    let inline toNativeInt (x: nativeptr<'T>) = (# "" x : nativeint    #)

    [<NoDynamicInvocation>]
    [<CompiledName("AddPointerInlined")>]
    let inline add (x : nativeptr<'T>) (n:int) : nativeptr<'T> = toNativeInt x + nativeint n * (# "sizeof !0" type('T) : nativeint #) |> ofNativeInt
    
    [<NoDynamicInvocation>]
    [<CompiledName("GetPointerInlined")>]
    let inline get (p : nativeptr<'T>) n = (# "ldobj !0" type ('T) (add p n) : 'T #) 
    
    [<NoDynamicInvocation>]
    [<CompiledName("SetPointerInlined")>]
    let inline set (p : nativeptr<'T>) n (x : 'T) = (# "stobj !0" type ('T) (add p n) x #)  

    [<NoDynamicInvocation>]
    [<CompiledName("ReadPointerInlined")>]
    let inline read (p : nativeptr<'T>) = (# "ldobj !0" type ('T) p : 'T #) 
    
    [<NoDynamicInvocation>]
    [<CompiledName("WritePointerInlined")>]
    let inline write (p : nativeptr<'T>) (x : 'T) = (# "stobj !0" type ('T) p x #)  
    
    [<NoDynamicInvocation>]
    [<CompiledName("StackAllocate")>]
    let inline stackalloc (count:int) : nativeptr<'T> = (# "localloc" (count * sizeof<'T>) : nativeptr<'T> #)

