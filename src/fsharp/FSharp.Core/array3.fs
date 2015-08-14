// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Collections

    open System.Diagnostics
    open Microsoft.FSharp.Collections
    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
    open Microsoft.FSharp.Core.Operators
    open Microsoft.FSharp.Core.Operators.Checked

    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    [<RequireQualifiedAccess>]
    module Array3D =

        let inline checkNonNull argName arg = 
            match box arg with 
            | null -> nullArg argName 
            | _ -> ()

        [<CompiledName("Length1")>]
        let length1 (array: 'T[,,]) =  (# "ldlen.multi 3 0" array : int #)  
 
        [<CompiledName("Length2")>]
        let length2 (array: 'T[,,]) =  (# "ldlen.multi 3 1" array : int #)  
 
        [<CompiledName("Length3")>]
        let length3 (array: 'T[,,]) =  (# "ldlen.multi 3 2" array : int #)  
 
        [<CompiledName("Get")>]
        let get (array: 'T[,,]) n1 n2 n3 = array.[n1,n2,n3]
 
        [<CompiledName("Set")>]
        let set (array: 'T[,,]) n1 n2 n3 x = array.[n1,n2,n3] <- x

        [<CompiledName("ZeroCreate")>]
        let zeroCreate (n1:int) (n2:int) (n3:int) = 
            if n1 < 0 then invalidArg "n1" (SR.GetString(SR.inputMustBeNonNegative))
            if n2 < 0 then invalidArg "n2" (SR.GetString(SR.inputMustBeNonNegative))
            if n3 < 0 then invalidArg "n3" (SR.GetString(SR.inputMustBeNonNegative))
            (# "newarr.multi 3 !0" type ('T) n1 n2 n3 : 'T[,,] #)
 
        [<CompiledName("Create")>]
        let create (n1:int) (n2:int) (n3:int) (x:'T) =
            let arr = (zeroCreate n1 n2 n3 : 'T[,,])
            for i = 0 to n1 - 1 do 
              for j = 0 to n2 - 1 do 
                for k = 0 to n3 - 1 do 
                  arr.[i,j,k] <- x
            arr

        [<CompiledName("Initialize")>]
        let init n1 n2 n3 f = 
            let arr = (zeroCreate n1 n2 n3 : 'T[,,])
            let f = OptimizedClosures.FSharpFunc<_,_,_,_>.Adapt(f)
            for i = 0 to n1 - 1 do 
              for j = 0 to n2 - 1 do 
                for k = 0 to n3 - 1 do 
                  arr.[i,j,k] <- f.Invoke(i, j, k)
            arr

        [<CompiledName("Iterate")>]
        let iter f array =
            checkNonNull "array" array
            let len1 = length1 array
            let len2 = length2 array
            let len3 = length3 array
            for i = 0 to len1 - 1 do 
              for j = 0 to len2 - 1 do 
                for k = 0 to len3 - 1 do 
                  f array.[i,j,k]

        [<CompiledName("Map")>]
        let map f array =
            checkNonNull "array" array
            let len1 = length1 array
            let len2 = length2 array
            let len3 = length3 array
            let res = (zeroCreate len1 len2 len3 : 'b[,,])
            for i = 0 to len1 - 1 do 
              for j = 0 to len2 - 1 do 
                for k = 0 to len3 - 1 do 
                  res.[i,j,k] <-  f array.[i,j,k]
            res

        [<CompiledName("IterateIndexed")>]
        let iteri f array =
            checkNonNull "array" array
            let len1 = length1 array
            let len2 = length2 array
            let len3 = length3 array
            let f = OptimizedClosures.FSharpFunc<_,_,_,_,_>.Adapt(f)
            for i = 0 to len1 - 1 do 
              for j = 0 to len2 - 1 do 
                for k = 0 to len3 - 1 do 
                  f.Invoke(i, j, k, array.[i,j,k]) 

        [<CompiledName("MapIndexed")>]
        let mapi f array =
            checkNonNull "array" array
            let len1 = length1 array
            let len2 = length2 array
            let len3 = length3 array
            let res = (zeroCreate len1 len2 len3 : 'b[,,])
            let f = OptimizedClosures.FSharpFunc<_,_,_,_,_>.Adapt(f)
            for i = 0 to len1 - 1 do 
              for j = 0 to len2 - 1 do 
                for k = 0 to len3 - 1 do 
                  res.[i,j,k] <- f.Invoke(i, j, k, array.[i,j,k])
            res

    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    [<RequireQualifiedAccess>]
    module Array4D =

        [<CompiledName("Length1")>]
        let length1 (array: 'T[,,,]) =  (# "ldlen.multi 4 0" array : int #)  
 
        [<CompiledName("Length2")>]
        let length2 (array: 'T[,,,]) =  (# "ldlen.multi 4 1" array : int #)  
 
        [<CompiledName("Length3")>]
        let length3 (array: 'T[,,,]) =  (# "ldlen.multi 4 2" array : int #)  
 
        [<CompiledName("Length4")>]
        let length4 (array: 'T[,,,]) =  (# "ldlen.multi 4 3" array : int #)  
 
        [<CompiledName("ZeroCreate")>]
        let zeroCreate (n1:int) (n2:int) (n3:int) (n4:int) = 
            if n1 < 0 then invalidArg "n1" (SR.GetString(SR.inputMustBeNonNegative))
            if n2 < 0 then invalidArg "n2" (SR.GetString(SR.inputMustBeNonNegative))
            if n3 < 0 then invalidArg "n3" (SR.GetString(SR.inputMustBeNonNegative))
            if n4 < 0 then invalidArg "n4" (SR.GetString(SR.inputMustBeNonNegative))
            (# "newarr.multi 4 !0" type ('T) n1 n2 n3 n4 : 'T[,,,] #)
 
        [<CompiledName("Create")>]
        let create n1 n2 n3 n4 (x:'T) =
            let arr = (zeroCreate n1 n2 n3 n4 : 'T[,,,])
            for i = 0 to n1 - 1 do 
              for j = 0 to n2 - 1 do 
                for k = 0 to n3 - 1 do 
                  for m = 0 to n4 - 1 do 
                    arr.[i,j,k,m] <- x
            arr

        [<CompiledName("Initialize")>]
        let init n1 n2 n3 n4 f = 
            let arr = (zeroCreate n1 n2 n3 n4 : 'T[,,,]) 
            let f = OptimizedClosures.FSharpFunc<_,_,_,_,_>.Adapt(f)
            for i = 0 to n1 - 1 do 
              for j = 0 to n2 - 1 do 
                for k = 0 to n3 - 1 do 
                  for m = 0 to n4 - 1 do 
                    arr.[i,j,k,m] <- f.Invoke(i, j, k, m)
            arr


        [<CompiledName("Get")>]
        let get (array: 'T[,,,]) n1 n2 n3 n4 =  array.[n1,n2,n3,n4]
 
        [<CompiledName("Set")>]
        let set (array: 'T[,,,]) n1 n2 n3 n4 x =  array.[n1,n2,n3,n4] <- x
