// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Collections

open Microsoft.FSharp.Collections
open Microsoft.FSharp.Core
open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
open Microsoft.FSharp.Core.Operators
open Microsoft.FSharp.Core.Operators.Checked

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Array3D =

    let inline checkNonNull argName arg = 
        if isNull arg then
            nullArg argName

    [<CompiledName("Length1")>]
    let length1 (array: 'T[,,]) = (# "ldlen.multi 3 0" array : int #)

    [<CompiledName("Length2")>]
    let length2 (array: 'T[,,]) = (# "ldlen.multi 3 1" array : int #)

    [<CompiledName("Length3")>]
    let length3 (array: 'T[,,]) = (# "ldlen.multi 3 2" array : int #)

    [<CompiledName("Get")>]
    let get (array: 'T[,,]) index1 index2 index3 = array.[index1,index2,index3]

    [<CompiledName("Set")>]
    let set (array: 'T[,,]) index1 index2 index3 value = array.[index1,index2,index3] <- value

    [<CompiledName("ZeroCreate")>]
    let zeroCreate length1 length2 length3 = 
        if length1 < 0 then invalidArgInputMustBeNonNegative "n1" length1
        if length2 < 0 then invalidArgInputMustBeNonNegative "n2" length2
        if length3 < 0 then invalidArgInputMustBeNonNegative "n3" length3
        (# "newarr.multi 3 !0" type ('T) length1 length2 length3 : 'T[,,] #)

    [<CompiledName("Create")>]
    let create length1 length2 length3 (initial:'T) =
        let arr = (zeroCreate length1 length2 length3 : 'T[,,])
        for i = 0 to length1 - 1 do 
          for j = 0 to length2 - 1 do 
            for k = 0 to length3 - 1 do 
              arr.[i,j,k] <- initial
        arr

    [<CompiledName("Initialize")>]
    let init length1 length2 length3 initializer = 
        let arr = (zeroCreate length1 length2 length3 : 'T[,,])
        let f = OptimizedClosures.FSharpFunc<_,_,_,_>.Adapt(initializer)
        for i = 0 to length1 - 1 do 
          for j = 0 to length2 - 1 do 
            for k = 0 to length3 - 1 do 
              arr.[i,j,k] <- f.Invoke(i, j, k)
        arr

    [<CompiledName("Iterate")>]
    let iter action array =
        checkNonNull "array" array
        let len1 = length1 array
        let len2 = length2 array
        let len3 = length3 array
        for i = 0 to len1 - 1 do 
          for j = 0 to len2 - 1 do 
            for k = 0 to len3 - 1 do 
              action array.[i,j,k]

    [<CompiledName("Map")>]
    let map mapping array =
        checkNonNull "array" array
        let len1 = length1 array
        let len2 = length2 array
        let len3 = length3 array
        let res = (zeroCreate len1 len2 len3 : 'b[,,])
        for i = 0 to len1 - 1 do 
          for j = 0 to len2 - 1 do 
            for k = 0 to len3 - 1 do 
              res.[i,j,k] <-  mapping array.[i,j,k]
        res

    [<CompiledName("IterateIndexed")>]
    let iteri action array =
        checkNonNull "array" array
        let len1 = length1 array
        let len2 = length2 array
        let len3 = length3 array
        let f = OptimizedClosures.FSharpFunc<_,_,_,_,_>.Adapt(action)
        for i = 0 to len1 - 1 do 
          for j = 0 to len2 - 1 do 
            for k = 0 to len3 - 1 do 
              f.Invoke(i, j, k, array.[i,j,k]) 

    [<CompiledName("MapIndexed")>]
    let mapi mapping array =
        checkNonNull "array" array
        let len1 = length1 array
        let len2 = length2 array
        let len3 = length3 array
        let res = (zeroCreate len1 len2 len3 : 'b[,,])
        let f = OptimizedClosures.FSharpFunc<_,_,_,_,_>.Adapt(mapping)
        for i = 0 to len1 - 1 do 
          for j = 0 to len2 - 1 do 
            for k = 0 to len3 - 1 do 
              res.[i,j,k] <- f.Invoke(i, j, k, array.[i,j,k])
        res

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Array4D =

    [<CompiledName("Length1")>]
    let length1 (array: 'T[,,,]) = (# "ldlen.multi 4 0" array : int #)

    [<CompiledName("Length2")>]
    let length2 (array: 'T[,,,]) = (# "ldlen.multi 4 1" array : int #)

    [<CompiledName("Length3")>]
    let length3 (array: 'T[,,,]) = (# "ldlen.multi 4 2" array : int #)

    [<CompiledName("Length4")>]
    let length4 (array: 'T[,,,]) = (# "ldlen.multi 4 3" array : int #)

    [<CompiledName("ZeroCreate")>]
    let zeroCreate length1 length2 length3 length4 = 
        if length1 < 0 then invalidArgInputMustBeNonNegative "n1" length1
        if length2 < 0 then invalidArgInputMustBeNonNegative "n2" length2
        if length3 < 0 then invalidArgInputMustBeNonNegative "n3" length3
        if length4 < 0 then invalidArgInputMustBeNonNegative "n4" length4
        (# "newarr.multi 4 !0" type ('T) length1 length2 length3 length4 : 'T[,,,] #)

    [<CompiledName("Create")>]
    let create length1 length2 length3 length4 (initial:'T) =
        let arr = (zeroCreate length1 length2 length3 length4 : 'T[,,,])
        for i = 0 to length1 - 1 do 
          for j = 0 to length2 - 1 do 
            for k = 0 to length3 - 1 do 
              for m = 0 to length4 - 1 do 
                arr.[i,j,k,m] <- initial
        arr

    [<CompiledName("Initialize")>]
    let init length1 length2 length3 length4 initializer = 
        let arr = (zeroCreate length1 length2 length3 length4 : 'T[,,,]) 
        let f = OptimizedClosures.FSharpFunc<_,_,_,_,_>.Adapt(initializer)
        for i = 0 to length1 - 1 do 
          for j = 0 to length2 - 1 do 
            for k = 0 to length3 - 1 do 
              for m = 0 to length4 - 1 do 
                arr.[i,j,k,m] <- f.Invoke(i, j, k, m)
        arr

    [<CompiledName("Get")>]
    let get (array: 'T[,,,]) index1 index2 index3 index4 = array.[index1,index2,index3,index4]

    [<CompiledName("Set")>]
    let set (array: 'T[,,,]) index1 index2 index3 index4 value = array.[index1,index2,index3,index4] <- value
