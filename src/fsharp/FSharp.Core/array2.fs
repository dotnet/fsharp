// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Collections

    open System
    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
    open Microsoft.FSharp.Core.Operators
    open Microsoft.FSharp.Core.Operators.Checked

    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    [<RequireQualifiedAccess>]
    module Array2D =

        let inline checkNonNull argName arg = 
            match box arg with 
            | null -> nullArg argName 
            | _ -> ()

        // Define the primitive operations. 
        // Note: the "type" syntax is for the type parameter for inline 
        // polymorphic IL. This helps the compiler inline these fragments, 
        // i.e. work out the correspondence between IL and F# type variables. 

        [<CompiledName("Length1")>]
        let length1 (array: 'T[,]) =  (# "ldlen.multi 2 0" array : int #)  
        [<CompiledName("Length2")>]
        let length2 (array: 'T[,]) =  (# "ldlen.multi 2 1" array : int #)  
        [<CompiledName("Base1")>]
        let base1 (array: 'T[,]) = array.GetLowerBound(0)  
        [<CompiledName("Base2")>]
        let base2 (array: 'T[,]) = array.GetLowerBound(1) 
        [<CompiledName("Get")>]
        let get (array: 'T[,]) (n:int) (m:int) =  (# "ldelem.multi 2 !0" type ('T) array n m : 'T #)  
        [<CompiledName("Set")>]
        let set (array: 'T[,]) (n:int) (m:int) (x:'T) =  (# "stelem.multi 2 !0" type ('T) array n m x #)  

        [<CompiledName("ZeroCreate")>]
        let zeroCreate (n:int) (m:int) = 
            if n < 0 then invalidArgInputMustBeNonNegative "length1" n 
            if m < 0 then invalidArgInputMustBeNonNegative "length2" m 
            (# "newarr.multi 2 !0" type ('T) n m : 'T[,] #)

        [<CompiledName("ZeroCreateBased")>]
        let zeroCreateBased (b1:int) (b2:int) (n1:int) (n2:int) = 
            if (b1 = 0 && b2 = 0) then 
#if FX_ATLEAST_PORTABLE
                zeroCreate n1 n2
#else                
                // Note: this overload is available on Compact Framework and Silverlight, but not Portable
                (System.Array.CreateInstance(typeof<'T>, [|n1;n2|]) :?> 'T[,])
#endif                
            else
#if FX_NO_BASED_ARRAYS
                raise (NotSupportedException(SR.GetString(SR.nonZeroBasedDisallowed)))
#else
                (Array.CreateInstance(typeof<'T>, [|n1;n2|],[|b1;b2|]) :?> 'T[,])
#endif

        [<CompiledName("CreateBased")>]
        let createBased b1 b2 n m (x:'T) = 
            let array = (zeroCreateBased b1 b2 n m : 'T[,])  
            for i = b1 to b1+n - 1 do 
              for j = b2 to b2+m - 1 do 
                array.[i,j] <- x
            array

        [<CompiledName("InitializeBased")>]
        let initBased b1 b2 n m f = 
            let array = (zeroCreateBased b1 b2 n m : 'T[,])
            let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)
            for i = b1 to b1+n - 1 do 
              for j = b2 to b2+m - 1 do 
                array.[i,j] <- f.Invoke(i, j)
            array


        [<CompiledName("Create")>]
        let create n m (x:'T) = 
            createBased 0 0 n m x

        [<CompiledName("Initialize")>]
        let init n m f = 
            initBased 0 0 n m f

        [<CompiledName("Iterate")>]
        let iter f array = 
            checkNonNull "array" array
            let count1 = length1 array 
            let count2 = length2 array 
            let b1 = base1 array 
            let b2 = base2 array 
            for i = b1 to b1+count1 - 1 do 
              for j = b2 to b2+count2 - 1 do 
                f array.[i,j]

        [<CompiledName("IterateIndexed")>]
        let iteri (f : int -> int -> 'T -> unit) (array:'T[,]) =
            checkNonNull "array" array
            let count1 = length1 array 
            let count2 = length2 array 
            let b1 = base1 array 
            let b2 = base2 array 
            let f = OptimizedClosures.FSharpFunc<_,_,_,_>.Adapt(f)
            for i = b1 to b1+count1 - 1 do 
              for j = b2 to b2+count2 - 1 do 
                f.Invoke(i, j, array.[i,j])

        [<CompiledName("Map")>]
        let map f array = 
            checkNonNull "array" array
            initBased (base1 array) (base2 array) (length1 array) (length2 array) (fun i j -> f array.[i,j])

        [<CompiledName("MapIndexed")>]
        let mapi f array = 
            checkNonNull "array" array
            let f = OptimizedClosures.FSharpFunc<_,_,_,_>.Adapt(f)
            initBased (base1 array) (base2 array) (length1 array) (length2 array) (fun i j -> f.Invoke(i, j, array.[i,j]))

        [<CompiledName("Copy")>]
        let copy array = 
            checkNonNull "array" array
            initBased (base1 array) (base2 array) (length1 array) (length2 array) (fun i j -> array.[i,j])
            
        [<CompiledName("Rebase")>]
        let rebase array = 
            checkNonNull "array" array
            let b1 = base1 array
            let b2 = base2 array
            init (length1 array) (length2 array) (fun i j -> array.[b1+i,b2+j])

        [<CompiledName("CopyTo")>]
        let blit (source : 'T[,])  sourceIndex1 sourceIndex2 (target: 'T[,]) targetIndex1 targetIndex2 count1 count2 = 
            checkNonNull "source" source
            checkNonNull "target" target

            let sourceX0, sourceY0 = source.GetLowerBound 0     , source.GetLowerBound 1
            let sourceXN, sourceYN = (length1 source) + sourceX0, (length2 source) + sourceY0  
            let targetX0, targetY0 = target.GetLowerBound 0     , target.GetLowerBound 1
            let targetXN, targetYN = (length1 target) + targetX0, (length2 target) + targetY0  

            if sourceIndex1 < sourceX0 then invalidArgOutOfRange "sourceIndex1" sourceIndex1 "source axis-0 lower bound" sourceX0
            if sourceIndex2 < sourceY0 then invalidArgOutOfRange "sourceIndex2" sourceIndex2 "source axis-1 lower bound" sourceY0
            if targetIndex1 < targetX0 then invalidArgOutOfRange "targetIndex1" targetIndex1 "target axis-0 lower bound" targetX0
            if targetIndex2 < targetY0 then invalidArgOutOfRange "targetIndex2" targetIndex2 "target axis-1 lower bound" targetY0
            if sourceIndex1 + count1 > sourceXN then 
                invalidArgOutOfRange "count1" count1 ("source axis-0 end index = " + string(sourceIndex1+count1) + " source axis-0 upper bound") sourceXN
            if sourceIndex2 + count2 > sourceYN then 
                invalidArgOutOfRange "count2" count2 ("source axis-1 end index = " + string(sourceIndex2+count2) + " source axis-1 upper bound") sourceYN
            if targetIndex1 + count1 > targetXN then 
                invalidArgOutOfRange "count1" count1 ("target axis-0 end index = " + string(targetIndex1+count1) + " target axis-0 upper bound") targetXN
            if targetIndex2 + count2 > targetYN then 
                invalidArgOutOfRange "count2" count2 ("target axis-1 end index = " + string(targetIndex2+count2) + " target axis-1 upper bound") targetYN

            for i = 0 to count1 - 1 do
                for j = 0 to count2 - 1 do
                    target.[targetIndex1+i,targetIndex2+j] <- source.[sourceIndex1+i,sourceIndex2+j]

        
