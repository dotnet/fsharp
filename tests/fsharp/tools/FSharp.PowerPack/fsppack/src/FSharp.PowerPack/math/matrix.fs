//----------------------------------------------------------------------------
// An implementation of generic dense and sparse matrix types.
//
// Overview and suffix documentation
//    _GU  = generic unspecialized (Matrix<T>, Vector<T> etc.) 
//    _GUA = generic unspecialized op on (underlying) array
//    _DS  = Double specialized (Matrix<float> = matrix, Vector<float> = vector etc.)
//
//    DM   = dense matrix
//    SM   = sparse matrix
//    V    = vector (dense)
//    RV   = row vector (dense)


namespace Microsoft.FSharp.Math

    #nowarn "60" // implementations in augmentations
    #nowarn "69" // implementations in augmentations

    open Microsoft.FSharp.Math
    open System
    open System.Globalization
    open System.Collections
    open System.Collections.Generic
    open System.Diagnostics
    type permutation = int -> int

    module Helpers = 
        let sparseNYI() = failwith "this operation is not supported on sparse matrices"
        let sparseNotMutable() = failwith "sparse matrices are not mutable"
        
        [<RequiresExplicitTypeArguments>]
        let opsdata<'T> = GlobalAssociations.TryGetNumericAssociation<'T>()
    
    open Helpers
    
    /// The value stored for the dictionary of numeric operations. If none is present then this indicates
    /// no operations are known for this type.
    type OpsData<'T> = INumeric<'T> option

    type DenseMatrix<'T>(opsData : OpsData<'T>, values : 'T[,]) = 
        member m.OpsData =  opsData
        member m.Values =  values
        member m.NumRows = values.GetLength(0)
        member m.NumCols = values.GetLength(1)

        member m.ElementOps = 
            match opsData with 
            | None -> raise (new System.NotSupportedException("The element type carried by this matrix does not support numeric operations"))
            | Some a -> a

        member m.Item
           with get (i,j) = values.[i,j]
           and  set (i,j) x = values.[i,j] <- x



    type SparseMatrix<'T>(opsData : OpsData<'T>, sparseValues : 'T array, sparseRowOffsets : int array, ncols:int, columnValues: int array) = 
        member m.OpsData = opsData; 
        member m.NumCols = ncols
        member m.NumRows = sparseRowOffsets.Length - 1
        member m.SparseColumnValues = columnValues
        member m.SparseRowOffsets =  sparseRowOffsets (* nrows + 1 elements *)
        member m.SparseValues =  sparseValues

        member m.ElementOps = 
              match opsData with 
              | None -> raise (new System.NotSupportedException("The element type carried by this matrix does not support numeric operations"))
              | Some a -> a

        member m.MinIndexForRow i = m.SparseRowOffsets.[i]
        member m.MaxIndexForRow i = m.SparseRowOffsets.[i+1]
              

        member m.Item 
            with get (i,j) = 
                let imax = m.NumRows
                let jmax = m.NumCols
                if j < 0 || j >= jmax || i < 0 || i >= imax then raise (new System.ArgumentOutOfRangeException()) else
                let kmin = m.MinIndexForRow i
                let kmax = m.MaxIndexForRow i
                let rec loopRow k =
                    (* note: could do a binary chop here *)
                    if k >= kmax then m.ElementOps.Zero else
                    let j2 = columnValues.[k]
                    if j < j2 then m.ElementOps.Zero else
                    if j = j2 then sparseValues.[k] else 
                    loopRow (k+1)
                loopRow kmin

#if FX_NO_DEBUG_DISPLAYS
#else
    [<System.Diagnostics.DebuggerDisplay("{DebugDisplay}")>]
#endif
    [<StructuredFormatDisplay("matrix {StructuredDisplayAsArray}")>]
    [<CustomEquality; CustomComparison>]
    //[<System.Diagnostics.DebuggerTypeProxy(typedefof<MatrixDebugView<_>>)>]
    type Matrix<'T> = 
        | DenseRepr of DenseMatrix<'T>
        | SparseRepr of SparseMatrix<'T>
        interface System.IComparable
        interface IStructuralComparable
        interface IStructuralEquatable
        interface IEnumerable<'T> 
        interface IEnumerable

        member m.ElementOps = match m with DenseRepr mr -> mr.ElementOps | SparseRepr mr -> mr.ElementOps
        member m.NumRows    = match m with DenseRepr mr -> mr.NumRows    | SparseRepr mr ->  mr.NumRows
        member m.NumCols    = match m with DenseRepr mr -> mr.NumCols    | SparseRepr mr ->  mr.NumCols

        member m.Item 
            with get (i,j) = 
                match m with 
                | DenseRepr dm -> dm.[i,j]
                | SparseRepr sm -> sm.[i,j]
            and set (i,j) x = 
              match m with 
              | DenseRepr dm -> dm.[i,j] <- x
              | SparseRepr _ -> sparseNotMutable()


#if FX_NO_DEBUG_DISPLAYS
#else
        [<DebuggerBrowsable(DebuggerBrowsableState.Collapsed)>]
#endif
        member m.IsDense = match m with DenseRepr _ -> true | SparseRepr _ -> false

#if FX_NO_DEBUG_DISPLAYS
#else
        [<DebuggerBrowsable(DebuggerBrowsableState.Collapsed)>]
#endif
        member m.IsSparse = match m with DenseRepr _ -> false | SparseRepr _ -> true

#if FX_NO_DEBUG_DISPLAYS
#else
        [<DebuggerBrowsable(DebuggerBrowsableState.Collapsed)>]
#endif
        member m.InternalSparseColumnValues = match m with DenseRepr _ -> invalidOp "not a sparse matrix" | SparseRepr mr -> mr.SparseColumnValues

#if FX_NO_DEBUG_DISPLAYS
#else
        [<DebuggerBrowsable(DebuggerBrowsableState.Collapsed)>]
#endif
        member m.InternalSparseRowOffsets = match m with DenseRepr _ -> invalidOp "not a sparse matrix" | SparseRepr mr -> mr.SparseRowOffsets

#if FX_NO_DEBUG_DISPLAYS
#else
        [<DebuggerBrowsable(DebuggerBrowsableState.Collapsed)>]
#endif
        member m.InternalSparseValues = match m with DenseRepr _ -> invalidOp "not a sparse matrix" | SparseRepr mr -> mr.SparseValues

#if FX_NO_DEBUG_DISPLAYS
#else
        [<DebuggerBrowsable(DebuggerBrowsableState.Collapsed)>]
#endif
        member m.InternalDenseValues = match m with DenseRepr mr -> mr.Values | SparseRepr _ -> invalidOp "not a dense matrix"

#if FX_NO_DEBUG_DISPLAYS
#else
    [<System.Diagnostics.DebuggerDisplay("{DebugDisplay}")>]
#endif
#if FX_NO_DEBUG_PROXIES
#else
    [<System.Diagnostics.DebuggerTypeProxy(typedefof<RowVectorDebugView<_>>)>]
#endif
    [<StructuredFormatDisplay("rowvec {StructuredDisplayAsArray}")>]
    [<Sealed>]
    type RowVector<'T>(opsRV : INumeric<'T> option, arrRV : 'T array ) =
        interface System.IComparable
        interface IStructuralComparable
        interface IStructuralEquatable 


#if FX_NO_DEBUG_DISPLAYS
#else
        [<DebuggerBrowsable(DebuggerBrowsableState.Collapsed)>]
#endif
        member x.InternalValues = arrRV
        member x.Values = arrRV
        member x.OpsData = opsRV
        
        
        interface IEnumerable<'T> with 
            member x.GetEnumerator() = (arrRV :> seq<_>).GetEnumerator()
        interface IEnumerable  with 
            member x.GetEnumerator() = (arrRV :> IEnumerable).GetEnumerator()

        member x.Length = arrRV.Length
        member x.NumCols = arrRV.Length
        member x.ElementOps = 
            match opsRV with 
            | None -> raise (new System.NotSupportedException("The element type carried by this row vector does not support numeric operations"))
            | Some a -> a

        member v.Item
           with get i = arrRV.[i]
           and  set i x = arrRV.[i] <- x

    and 
        [<Sealed>]
        RowVectorDebugView<'T>(v: RowVector<'T>)  =  

#if FX_NO_DEBUG_DISPLAYS
#else
             [<System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.RootHidden)>]
#endif
             member x.Items = v |> Seq.truncate 1000 |> Seq.toArray 

#if FX_NO_DEBUG_DISPLAYS
#else
    [<System.Diagnostics.DebuggerDisplay("{DebugDisplay}")>]
#endif
#if FX_NO_DEBUG_PROXIES
#else
    [<System.Diagnostics.DebuggerTypeProxy(typedefof<VectorDebugView<_>>)>]
#endif
    [<StructuredFormatDisplay("vector {StructuredDisplayAsArray}")>]
    [<Sealed>]
    type Vector<'T>(opsV : INumeric<'T> option, arrV : 'T array) =

#if FX_NO_DEBUG_DISPLAYS
#else
        [<DebuggerBrowsable(DebuggerBrowsableState.Collapsed)>]
#endif
        member x.InternalValues = arrV
        member x.Values = arrV
        member x.OpsData = opsV
        interface System.IComparable
        interface IStructuralComparable
        interface IStructuralEquatable 

        interface IEnumerable<'T> with 
            member x.GetEnumerator() = (arrV :> seq<_>).GetEnumerator()
        interface IEnumerable  with 
            member x.GetEnumerator() = (arrV :> IEnumerable).GetEnumerator()
        

        member m.Length = arrV.Length
        member m.NumRows = arrV.Length
        member m.ElementOps = 
            match opsV with 
            | None -> raise (new System.NotSupportedException("The element type carried by this vector does not support numeric operations"))
            | Some a -> a
        member v.Item
           with get i = arrV.[i]
           and  set i x = arrV.[i] <- x

#if FX_NO_DEBUG_PROXIES
#else
    and 
        [<Sealed>]
        VectorDebugView<'T>(v: Vector<'T>)  =  

             [<System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.RootHidden)>]
             member x.Items = v |> Seq.truncate 1000 |> Seq.toArray 
#endif


    /// Implementations of operations that will work for any type
    module GenericImpl = 

        type OpsData<'T> = INumeric<'T> option

        let opsOfOpsData (d : OpsData<'T>)  =
             match d with 
             | None -> raise (new System.NotSupportedException("The element type '"+(typeof<'T>).ToString()+"' carried by this vector or matrix does not support numeric operations (i.e. does not have a registered numeric association)"))
             | Some a -> a

        let getNormOps (ops:INumeric<'T>) = 
            match box ops with
              | :? INormFloat<'T> as ops -> ops
              | _ -> raise (new System.NotSupportedException("The element type '"+(typeof<'T>.ToString())+"' carried by this vector or matrix does not support the INormFloat<_> operation (i.e. does not have a registered numeric association that supports this type)"))

        let mkDenseMatrixGU ops arr = DenseMatrix(ops,arr)
        let mkRowVecGU ops arr = RowVector(ops, arr)
        let mkVecGU ops arr = Vector(ops,arr)

        let inline getArray2D  (arrDM : _[,]) i j   = arrDM.[i,j]
        let inline setArray2D  (arrDM  : _[,]) i j x = arrDM.[i,j] <- x

        let inline createArray m = Array.zeroCreate m

        let inline createArray2D m n = Array2D.zeroCreate m n

        let inline assignArray2D m n f arr =  
            for i = 0 to m - 1 do 
                for j = 0 to n - 1 do 
                    (arr  : _[,]).[i,j] <- f i j

        let inline assignConstArray2D m n x arr =  
            for i = 0 to m - 1 do 
                for j = 0 to n - 1 do 
                    (arr  : _[,]).[i,j] <- x

        let inline assignDenseMatrixGU f (a:DenseMatrix<_>) = 
            assignArray2D a.NumRows a.NumCols f a.Values
        
        let inline assignArray m f (arr : _[]) = 
            for i = 0 to m - 1 do 
                arr.[i] <- f i

        let inline assignConstArray m x (arr : _[]) = 
            for i = 0 to m - 1 do 
                arr.[i] <- x

        let inline assignVecGU f (a:Vector<_>) = 
            assignArray a.NumRows f a.Values
        
        let inline assignRowVecGU f (a:RowVector<_>) = 
            assignArray a.NumCols f a.Values
        
        let createConstDenseMatrixGU ops m n x = 
            let arr = createArray2D m n 
            assignConstArray2D m n x arr;
            DenseMatrix(ops,arr)
        
        let createConstRowVecGU ops m x = 
            let arr = createArray m 
            assignConstArray m x arr;
            mkRowVecGU ops arr
        
        let createConstVecGU ops m x = 
            let arr = createArray m 
            assignConstArray m x arr;
            mkVecGU ops arr


        let inline createDenseMatrixGU ops m n f = (* inline eliminates unknown f call *)
            let arr = createArray2D m n 
            assignArray2D m n f arr;
            DenseMatrix(ops,arr)
        
        let createRowVecGU ops m f = 
            let arr = createArray m 
            assignArray m f arr;
            mkRowVecGU ops arr
        
        let inline createVecGU ops m f = (* inline eliminates unknown f call *)
            let arr = createArray m 
            assignArray m f arr;
            mkVecGU ops arr

        /// Create a matrix from a sparse sequence 
        let initSparseMatrixGU maxi maxj ops s = 

            (* nb. could use sorted dictionary but that is in System.dll *)
            let tab = Array.create maxi null
            let count = ref 0
            for (i,j,v) in s do
                if i < 0 || i >= maxi || j <0 || j >= maxj then failwith "initial value out of range";
                count := !count + 1;
                let tab2 = 
                    match tab.[i] with 
                    | null -> 
                        let tab2 = new Dictionary<_,_>(3) 
                        tab.[i] <- tab2;
                        tab2
                    | tab2 -> tab2
                tab2.[j] <- v
            // optimize this line....
            let offsA =  
               let rowsAcc = Array.zeroCreate (maxi + 1)
               let mutable acc = 0 
               for i = 0 to maxi-1 do 
                  rowsAcc.[i] <- acc;
                  acc <- match tab.[i] with 
                          | null -> acc
                          | tab2 -> acc+tab2.Count
               rowsAcc.[maxi] <- acc;
               rowsAcc
               
            let colsA,valsA = 
               let colsAcc = new ResizeArray<_>(!count)
               let valsAcc = new ResizeArray<_>(!count)
               for i = 0 to maxi-1 do 
                  match tab.[i] with 
                  | null -> ()
                  | tab2 -> tab2 |> Seq.toArray |> Array.sortBy (fun kvp -> kvp.Key) |> Array.iter (fun kvp -> colsAcc.Add(kvp.Key); valsAcc.Add(kvp.Value));
               colsAcc.ToArray(), valsAcc.ToArray()

            SparseMatrix(opsData=ops, sparseValues=valsA, sparseRowOffsets=offsA, ncols=maxj, columnValues=colsA)
        
        let zeroizeDenseMatrixGUA arr  m n : DenseMatrix<'T> = 
            let opsData = opsdata<'T> 
            let ops = opsOfOpsData opsData 
            let zero = ops.Zero 
            assignArray2D m n (fun _ _ -> zero) arr;
            DenseMatrix(opsData,arr)

        let zeroizeArray opsData arr m  = 
            let ops = opsOfOpsData opsData 
            let zero = ops.Zero 
            assignArray m (fun _ -> zero) arr

        let zeroizeVecGUA arr m  : Vector<'T> = 
            let opsData = opsdata<'T> 
            zeroizeArray opsData arr m;
            mkVecGU opsData arr

        let zeroizeRowVecGUA arr m  : RowVector<'T> = 
            let opsData = opsdata<'T> 
            zeroizeArray opsData arr m;
            mkRowVecGU opsData arr

        let listDenseMatrixGU ops xss =
            let m = List.length xss
            match xss with 
            | [] -> invalidArg "xss" "unexpected empty list"
            | h :: t -> 
              let n = List.length h
              if not (List.forall (fun xs -> List.length xs=n) t) then invalidArg "xss" "the lists are not all of the same length";
              let values = Array2D.zeroCreate m n
              List.iteri (fun i rw -> List.iteri (fun j x -> values.[i,j] <- x) rw) xss;
              DenseMatrix(ops,values)
        
        let listRowVecGU ops xs = mkRowVecGU ops (Array.ofList xs) 
        let listVecGU ops xs = mkVecGU ops (Array.ofList xs) 

        let seqDenseMatrixGU ops xss = listDenseMatrixGU ops (xss |> Seq.toList |> List.map Seq.toList)
        let seqVecGU  ops xss = listVecGU ops (xss |> Seq.toList)
        let seqRowVecGU ops xss = listRowVecGU ops (xss |> Seq.toList)

        let inline binaryOpDenseMatrixGU f (a:DenseMatrix<_>) (b:DenseMatrix<_>) = (* pointwise binary operator *)
            let nA = a.NumCols
            let mA = a.NumRows
            let nB = b.NumCols 
            let mB = b.NumRows
            if nA<>nB || mA<>mB then invalidArg "a" "the two matrices do not have compatible dimensions";
            let arrA = a.Values 
            let arrB = b.Values 
            createDenseMatrixGU a.OpsData mA nA (fun i j -> f (getArray2D arrA i j) (getArray2D arrB i j))


        let nonZeroEntriesSparseMatrixGU  (a:SparseMatrix<_>) = 
            // This is heavily used, and this version is much faster than
            // the sequence operators.
            let entries = new ResizeArray<_>(a.SparseColumnValues.Length)
            let imax = a.NumRows
            let ops = a.ElementOps 
            let zero = ops.Zero
            for i in 0 .. imax - 1 do
              let kmin = a.MinIndexForRow i
              let kmax = a.MaxIndexForRow i
              for k in kmin .. kmax - 1 do
                  let j = a.SparseColumnValues.[k]
                  let v = a.SparseValues.[k]
                  if not (ops.Equals(v,zero)) then
                    entries.Add((i,j,v))
            (entries :> seq<_>)

        let nonzeroEntriesDenseMatrixGU  (a:DenseMatrix<_>) = 
            let imax = a.NumRows
            let jmax = a.NumCols
            let ops = a.ElementOps 
            let zero = ops.Zero
            seq { for i in 0 .. imax - 1 do 
                    for j in 0 .. jmax - 1 do 
                        let v = a.[i,j] 
                        if not (ops.Equals(v, zero)) then
                             yield (i,j,v) }


        // pointwise operation on two sparse matrices. f must be zero-zero-preserving, i.e. (f 0 0 = 0) 
        let binaryOpSparseMatrixGU f (a:SparseMatrix<_>) (b:SparseMatrix<_>) = 
            let ops = a.ElementOps 
            let zero = ops.Zero
            let imax1 = a.NumRows  
            let imax2 = b.NumRows
            let jmax1 = a.NumCols
            let jmax2 = b.NumCols
            if imax1 <> imax2 || jmax1 <> jmax2 then invalidArg "b" "the two matrices do not have compatible dimensions";
            let imin = 0
            let imax = imax1
            let jmax = jmax1
            let rowsR = Array.zeroCreate (imax+1)
            let colsR = new ResizeArray<_>(max a.SparseColumnValues.Length b.SparseColumnValues.Length)
            let valsR = new ResizeArray<_>(max a.SparseValues.Length b.SparseValues.Length)
            let rec loopRows i  = 
                rowsR.[i] <- valsR.Count;            
                if i >= imax1 then () else
                let kmin1 = a.MinIndexForRow i
                let kmax1 = a.MaxIndexForRow i 
                let kmin2 = b.MinIndexForRow i
                let kmax2 = b.MaxIndexForRow i
                let rec loopRow k1 k2  =
                    if k1 >= kmax1 && k2 >= kmax2 then () else
                    let j1 = if k1 >= kmax1 then jmax else a.SparseColumnValues.[k1]
                    let j2 = if k2 >= kmax2 then jmax else b.SparseColumnValues.[k2]
                    let v1 = if j1 <= j2 then a.SparseValues.[k1] else zero
                    let v2 = if j2 <= j1 then b.SparseValues.[k2] else zero
                    let jR = min j1 j2
                    let vR = f v1 v2
                    (* if vR <> zero then  *)
                    colsR.Add(jR);
                    valsR.Add(vR);
                    loopRow (if j1 <= j2 then k1+1 else k1) (if j2 <= j1 then k2+1 else k2)
                loopRow kmin1 kmin2;
                loopRows (i+1) 
            loopRows imin;
            SparseMatrix(opsData= a.OpsData, 
                         sparseRowOffsets=rowsR, 
                         ncols= a.NumCols, 
                         columnValues=colsR.ToArray(), 
                         sparseValues=valsR.ToArray())

        let inline binaryOpRowVecGU f (a:RowVector<_>) (b:RowVector<_>) = (* pointwise binary operator *)
            let mA = a.NumCols
            let mB = b.NumCols
            if mA<>mB then invalidArg "b" "the two vectors do not have compatible dimensions"
            createRowVecGU a.OpsData mA (fun i -> f a.[i] b.[i])

        let inline binaryOpVecGU f (a:Vector<_>) (b:Vector<_>) = (* pointwise binary operator *)
            let mA = a.NumRows
            let mB = b.NumRows
            if mA<>mB then invalidArg "b" "the two vectors do not have compatible dimensions"
            createVecGU a.OpsData mA (fun i -> f a.[i] b.[i])

        let inline unaryOpDenseMatrixGU f (a:DenseMatrix<_>) =
            let nA = a.NumCols 
            let mA = a.NumRows 
            let arrA = a.Values 
            createDenseMatrixGU a.OpsData mA nA (fun i j -> f (getArray2D arrA i j))

        let inline unaryOpRowVecGU f (a:RowVector<_>) =
            let mA = a.NumCols
            let arrA = a.Values 
            createRowVecGU a.OpsData mA (fun j -> f arrA.[j])

        let inline unaryOpVectorGU f (a:Vector<_>) =
            let mA = a.NumRows 
            let arrA = a.Values 
            createVecGU a.OpsData mA (fun i -> f arrA.[i])

        let unaryOpSparseGU f (a:SparseMatrix<_>) = (* pointwise zero-zero-preserving binary operator (f 0 = 0) *)
            SparseMatrix(opsData=a.OpsData,
                         sparseRowOffsets=Array.copy a.SparseRowOffsets, 
                         columnValues=Array.copy a.SparseColumnValues, 
                         sparseValues=Array.map f a.SparseValues, 
                         ncols=a.NumCols)

        // Strictly speaking, sparse arrays are non mutable so no copy is ever needed. But implementing it *)
        // anyway in case we move to mutability *)
        let copySparseGU (a:SparseMatrix<_>) = 
            SparseMatrix(opsData=a.OpsData,
                         sparseRowOffsets=Array.copy a.SparseRowOffsets, 
                         columnValues=Array.copy a.SparseColumnValues,
                         sparseValues=Array.copy a.SparseValues, 
                         ncols=a.NumCols)

        let addDenseMatrixGU  (a:DenseMatrix<_>)  b = let ops = a.ElementOps in binaryOpDenseMatrixGU (fun x y -> ops.Add(x, y)) a b
        let addSparseMatrixGU (a:SparseMatrix<_>) b = let ops = a.ElementOps in binaryOpSparseMatrixGU (fun x y -> ops.Add(x, y)) a b
        let addRowVecGU       (a:RowVector<_>)    b = let ops = a.ElementOps in binaryOpRowVecGU (fun x y -> ops.Add(x, y)) a b
        let addVecGU          (a:Vector<_>)       b = let ops = a.ElementOps in binaryOpVecGU  (fun x y -> ops.Add(x, y)) a b 

        let subDenseMatrixGU  (a:DenseMatrix<_>)  b = let ops = a.ElementOps in binaryOpDenseMatrixGU (fun x y -> ops.Subtract(x, y)) a b
        let subSparseMatrixGU (a:SparseMatrix<_>) b = let ops = a.ElementOps in binaryOpSparseMatrixGU (fun x y -> ops.Subtract(x, y)) a b
        let subRowVecGU       (a:RowVector<_>)    b = let ops = a.ElementOps in binaryOpRowVecGU (fun x y -> ops.Subtract(x, y)) a b
        let subVecGU          (a:Vector<_>)       b = let ops = a.ElementOps in binaryOpVecGU  (fun x y -> ops.Subtract(x, y)) a b 

        ///Point-wise multiplication 
        let cptMulDenseMatrixGU  (a:DenseMatrix<_>)  b = let ops = a.ElementOps in binaryOpDenseMatrixGU  (fun x y -> ops.Multiply(x, y)) a b
        let cptMulSparseMatrixGU (a:SparseMatrix<_>) b = let ops = a.ElementOps in binaryOpSparseMatrixGU  (fun x y -> ops.Multiply(x, y)) a b
        let cptMulRowVecGU       (a:RowVector<_>)    b = let ops = a.ElementOps in binaryOpRowVecGU (fun x y -> ops.Multiply(x, y)) a b
        let cptMulVecGU          (a:Vector<_>)       b = let ops = a.ElementOps in binaryOpVecGU  (fun x y -> ops.Multiply(x, y)) a b

        let cptMaxDenseMatrixGU  (a:DenseMatrix<_>) b = binaryOpDenseMatrixGU  max a b
        let cptMinDenseMatrixGU  (a:DenseMatrix<_>) b = binaryOpDenseMatrixGU  min a b
        let cptMaxSparseMatrixGU (a:SparseMatrix<_>) b = binaryOpSparseMatrixGU  max a b
        let cptMinSparseMatrixGU (a:SparseMatrix<_>) b = binaryOpSparseMatrixGU  min a b

        let cptMaxVecGU (a:Vector<_>) b = binaryOpVecGU max a b
        let cptMinVecGU (a:Vector<_>) b = binaryOpVecGU min a b

        let add (ops : INumeric<'T>) x y = ops.Add(x,y) 
        let sub (ops : INumeric<'T>) x y = ops.Subtract(x,y) 
        let mul (ops : INumeric<'T>) x y = ops.Multiply(x,y) 

        let inline foldR f z (a,b) = 
            let mutable res = z in
            for i = a to b do
                res <- f res i
            res

        let inline sumfR f (a,b) =
            let mutable res = 0.0 
            for i = a to b do
                res <- res + f i
            res
          

        let inline sumRGU (ops : INumeric<_>) f r = 
            let zero = ops.Zero 
            r |> foldR (fun z k -> add ops z (f k)) zero

        let genericMulDenseMatrix (a:DenseMatrix<_>) (b:DenseMatrix<_>) =
            let nA = a.NumCols 
            let mA = a.NumRows
            let nB = b.NumCols 
            let mB = b.NumRows
            if nA<>mB then invalidArg "b" "the two matrices do not have compatible dimensions"
            let ops = a.ElementOps 
            let arrA = a.Values 
            let arrB = b.Values 
            createDenseMatrixGU a.OpsData mA nB
              (fun i j -> (0,nA - 1) |> sumRGU ops (fun k -> mul ops (getArray2D arrA i k) (getArray2D arrB k j)))

        let debug = false
        
        // SParse matrix multiplication algorithm. inline to get specialization at the 'double' type
        let inline genericMulSparse zero add mul (a:SparseMatrix<_>) (b:SparseMatrix<_>) =
            let nA = a.NumCols
            let mA = a.NumRows
            let nB = b.NumCols 
            let mB = b.NumRows
            if nA<>mB then invalidArg "b" "the two matrices do not have compatible dimensions"
            let C = new ResizeArray<_>()
            let jC = new ResizeArray<_>()
            let MA1 = mA + 1 
            let offsAcc = Array.zeroCreate MA1
            let index = Array.zeroCreate mA
            let temp = Array.create mA zero
            let ptr = new Dictionary<_,_>(11)
            if debug then printf "start, #items in result = %d, #offsAcc = %d, mA = %d\n" jC.Count offsAcc.Length mA;

            let mutable mlast = 0
            for i = 0 to mA-1 do
                if debug then printf "i = %d, mlast = %d\n" i mlast;
                offsAcc.[i] <- mlast
                
                let kmin1 = a.MinIndexForRow i
                let kmax1 = a.MaxIndexForRow i
                if kmin1 < kmax1 then 
                    let mutable itemp = 0
                    let mutable ptrNeedsClear = true // clear the ptr table on demand. 
                    for j = kmin1 to kmax1 - 1 do
                        if debug then printf "  j = %d\n" j;
                        let ja_j = a.SparseColumnValues.[j]
                        let kmin2 = b.MinIndexForRow ja_j
                        let kmax2 = b.MaxIndexForRow ja_j
                        for k = kmin2 to kmax2 - 1 do
                            let jb_k = b.SparseColumnValues.[k]
                            if debug then printf "    i = %d, j = %d, k = %d, ja_j = %d, jb_k = %d\n" i j k ja_j jb_k;
                            let va = a.SparseValues.[j] 
                            let vb = b.SparseValues.[k]
                            if debug then printf "    va = %O, vb = %O\n" va vb;
                            let summand = mul va vb
                            if debug then printf "    summand = %O\n" summand;
                            if ptrNeedsClear then (ptr.Clear();ptrNeedsClear <- false);

                            if not (ptr.ContainsKey(jb_k)) then
                                if debug then printf "    starting entry %d\n" jb_k;
                                ptr.[jb_k] <- itemp
                                let ptr_jb_k = itemp
                                temp.[ptr_jb_k] <- summand
                                index.[ptr_jb_k] <- jb_k
                                itemp <- itemp + 1
                            else
                                if debug then printf "    adding to entry %d\n" jb_k;
                                let ptr_jb_k = ptr.[jb_k]
                                temp.[ptr_jb_k] <- add temp.[ptr_jb_k] summand
                        done
                    done
                    if itemp > 0 then 
                        // Sort by index. 
                        // REVIEW: avoid the allocations here
                        let sorted = (temp.[0..itemp-1],index.[0..itemp-1]) ||> Array.zip 
                        Array.sortInPlaceBy (fun (_,idx) -> idx) sorted
                        for s = 0 to itemp-1 do
                            let (v,idx) = sorted.[s]
                            if debug then printf "  writing value %O at index %d to result matrix\n" v idx;
                            C.Add(v)
                            jC.Add(idx)
                        if debug then printf " itemp = %d, mlast = %d\n" itemp mlast;
                        mlast <- mlast + itemp 
            done
            offsAcc.[mA] <- mlast;
            if debug then printf "done, #items in result = %d, #offsAcc = %d, mA = %d\n" jC.Count offsAcc.Length mA;
            SparseMatrix(opsData = a.OpsData,
                         sparseRowOffsets=offsAcc,
                         ncols= nB,
                         columnValues=jC.ToArray(),
                         sparseValues=C.ToArray())

        let mulSparseMatrixGU (a: SparseMatrix<_>) b =
            let ops = a.ElementOps 
            let zero = ops.Zero
            genericMulSparse zero (add ops) (mul ops) a b


        let mulRowVecVecGU (a:RowVector<_>) (b:Vector<_>) =
            let mA = a.NumCols 
            let nB = b.NumRows 
            if mA<>nB then invalidArg "b" "the two vectors do not have compatible dimensions"
            let ops = a.ElementOps 
            (0,mA - 1) |> sumRGU ops (fun k -> mul ops a.[k] b.[k])

        let rowvecDenseMatrixGU (x:RowVector<_>) = createDenseMatrixGU x.OpsData 1         x.NumCols (fun _ j -> x.[j]) 
        let vectorDenseMatrixGU (x:Vector<_>)    = createDenseMatrixGU x.OpsData  x.NumRows 1         (fun i _ -> x.[i]) 

        let mulVecRowVecGU a b = genericMulDenseMatrix (vectorDenseMatrixGU a) (rowvecDenseMatrixGU b)

        let mulRowVecDenseMatrixGU (a:RowVector<_>) (b:DenseMatrix<_>) =
            let    nA = a.NumCols 
            let nB = b.NumCols
            let mB = b.NumRows 
            if nA<>mB then invalidArg "b" "the two vectors do not have compatible dimensions"
            let ops = a.ElementOps 
            let arrA = a.Values 
            let arrB = b.Values 
            createRowVecGU a.OpsData nB 
              (fun j -> (0,nA - 1) |> sumRGU ops (fun k -> mul ops arrA.[k] (getArray2D arrB k j)))

        let mulDenseMatrixVecGU (a:DenseMatrix<_>) (b:Vector<_>) =
            let nA = a.NumCols 
            let mA = a.NumRows 
            let mB    = b.NumRows
            if nA<>mB then invalidArg "b" "the two inputs do not have compatible dimensions"
            let ops = b.ElementOps 
            let arrA = a.Values 
            let arrB = b.Values 
            createVecGU b.OpsData mA
              (fun i -> (0,nA - 1) |> sumRGU ops (fun k -> mul ops (getArray2D arrA i k) arrB.[k]))

        let mulSparseVecGU (a:SparseMatrix<_>) (b:Vector<_>) =
            let nA = a.NumCols 
            let mA = a.NumRows 
            let mB    = b.NumRows 
            if nA<>mB then invalidArg "b" "the two inputs do not have compatible dimensions"
            let ops = b.ElementOps 
            let zero = ops.Zero
            createVecGU b.OpsData mA (fun i -> 
                let mutable acc = zero
                for k = a.MinIndexForRow i to a.MaxIndexForRow i - 1 do
                    let j = a.SparseColumnValues.[k]
                    let v = a.SparseValues.[k] 
                    acc <- add ops acc (mul ops v b.[j]);
                acc)

        let mulRVSparseMatrixGU (a:RowVector<_>) (b:SparseMatrix<_>) =
            let nA = b.NumCols
            let mA = b.NumRows 
            let mB    = a.NumCols 
            if mA<>mB then invalidArg "b" "the two inputs do not have compatible dimensions"
            let ops = b.ElementOps 
            let arr = createArray nA 
            zeroizeArray a.OpsData arr nA;
            for i = 0 to mA - 1 do
                for k = b.MinIndexForRow i to b.MaxIndexForRow i - 1 do
                    let j = b.SparseColumnValues.[k]
                    let v = b.SparseValues.[k] 
                    arr.[j] <- add ops arr.[j] (mul ops a.[i] v)
            mkRowVecGU a.OpsData arr


        let scaleDenseMatrixGU  k (a:DenseMatrix<_>)  = let ops = a.ElementOps in unaryOpDenseMatrixGU (fun x -> ops.Multiply(k,x)) a
        let scaleRowVecGU       k (a:RowVector<_>)    = let ops = a.ElementOps in unaryOpRowVecGU (fun x -> ops.Multiply(k,x)) a
        let scaleVecGU          k (a:Vector<_>)       = let ops = a.ElementOps in unaryOpVectorGU  (fun x -> ops.Multiply(k,x)) a
        let scaleSparseMatrixGU k (a:SparseMatrix<_>) = let ops = a.ElementOps in unaryOpSparseGU (fun x -> ops.Multiply(k,x)) a
        let negDenseMatrixGU  (a:DenseMatrix<_>)  = let ops = a.ElementOps in unaryOpDenseMatrixGU (fun x -> ops.Negate(x)) a
        let negRowVecGU       (a:RowVector<_>)    = let ops = a.ElementOps in unaryOpRowVecGU (fun x -> ops.Negate(x)) a
        let negVecGU          (a:Vector<_>)       = let ops = a.ElementOps in unaryOpVectorGU  (fun x -> ops.Negate(x)) a
        let negSparseMatrixGU (a:SparseMatrix<_>) = let ops = a.ElementOps in unaryOpSparseGU (fun x -> ops.Negate(x)) a

        let mapDenseMatrixGU f (a : DenseMatrix<'T>) : DenseMatrix<'T> = 
            let arrA = a.Values 
            createDenseMatrixGU a.OpsData a.NumRows a.NumCols (fun i j -> f (getArray2D arrA i j))

        let mapVecGU f (a:Vector<_>) = 
            let mA= a.NumRows
            createVecGU a.OpsData mA (fun i -> f a.[i])

        let copyDenseMatrixGU (a : DenseMatrix<'T>) : DenseMatrix<'T> = 
            let arrA = a.Values 
            createDenseMatrixGU a.OpsData a.NumRows a.NumCols (fun i j -> getArray2D arrA i j)

        let copyVecGU (a:Vector<_>) = 
            createVecGU a.OpsData a.NumRows (fun i -> a.[i])

        let copyRowVecGU (a:RowVector<_>) = 
            createRowVecGU a.OpsData a.NumCols (fun i -> a.[i])

        let toDenseSparseMatrixGU (a:SparseMatrix<_>) = 
            createDenseMatrixGU a.OpsData a.NumRows a.NumCols  (fun i j -> a.[i,j])
          
        let mapiDenseMatrixGU f (a: DenseMatrix<'T>) : DenseMatrix<'T> = 
            let arrA = a.Values 
            createDenseMatrixGU a.OpsData a.NumRows a.NumCols (fun i j -> f i j (getArray2D arrA i j))

        let mapiRowVecGU f (a:RowVector<_>) = 
            createRowVecGU a.OpsData a.NumCols (fun i -> f i a.[i])

        let mapiVecGU f (a:Vector<_>) = 
            createVecGU a.OpsData a.NumRows (fun i -> f i a.[i])

        let permuteVecGU (p:permutation) (a:Vector<_>) = 
            createVecGU a.OpsData a.NumRows (fun i -> a.[p i])

        let permuteRowVecGU (p:permutation) (a:RowVector<_>) = 
            createRowVecGU a.OpsData a.NumCols (fun i -> a.[p i])

        let inline inplace_mapiDenseMatrixGU f (a:DenseMatrix<_>) = 
            let arrA = a.Values 
            assignDenseMatrixGU (fun i j -> f i j (getArray2D arrA i j)) a

        let inline inplace_mapiRowVecGU f (a:RowVector<_>) = 
            assignRowVecGU (fun i -> f i a.[i]) a

        let inline inplace_mapiVecGU f (a:Vector<_>) = 
            assignVecGU (fun i -> f i a.[i]) a

        let inline foldDenseMatrixGU f z (a:DenseMatrix<_>) =
            let nA = a.NumCols 
            let mA = a.NumRows
            let arrA = a.Values 
            let mutable acc = z
            for i = 0 to mA-1 do
                for j = 0 to nA-1 do 
                   acc <- f acc (getArray2D arrA i j)
            acc
        
        let inline foldVecGU f z (a:Vector<_>) =
            let mutable acc = z
            for i = 0 to a.NumRows-1 do acc <- f acc a.[i]
            acc
        
        let inline foldiDenseMatrixGU f z (a:DenseMatrix<_>) =
            let nA = a.NumCols 
            let mA = a.NumRows
            let arrA = a.Values 
            let mutable acc = z
            for i = 0 to mA-1 do
                for j = 0 to nA-1 do 
                   acc <- f i j acc (getArray2D arrA i j)
            acc
        
        let inline foldiVecGU f z (a:Vector<_>) =
            let mA = a.NumRows
            let mutable acc = z
            for i = 0 to mA-1 do acc <- f i acc a.[i]
            acc
        
        let rec forallR f (n,m) = (n > m) || (f n && forallR f (n+1,m))
        let rec existsR f (n,m) = (n <= m) && (f n || existsR f (n+1,m))
        
        let foralliDenseMatrixGU pred (a:DenseMatrix<_>) =
            let nA = a.NumCols 
            let mA = a.NumRows
            let arrA = a.Values 
            (0,mA-1) |> forallR  (fun i ->
            (0,nA-1) |> forallR  (fun j ->
            pred i j (getArray2D arrA i j)))

        let foralliVecGU pred (a:Vector<_>) =
            let mA = a.NumRows
            (0,mA-1) |> forallR  (fun i ->
            pred i a.[i])

        let existsiDenseMatrixGU pred (a:DenseMatrix<_>) =
            let nA = a.NumCols 
            let mA = a.NumRows
            let arrA = a.Values 
            (0,mA-1) |> existsR (fun i ->
            (0,nA-1) |> existsR (fun j ->
            pred i j (getArray2D arrA i j)))

        let existsiVecGU pred (a:Vector<_>) =
            let mA = a.NumRows
            (0,mA-1) |> existsR (fun i ->
            pred i a.[i])

        let sumDenseMatrixGU  (a:DenseMatrix<_>) = 
            let ops = a.ElementOps 
            foldDenseMatrixGU (fun acc aij -> add ops acc aij) ops.Zero a

        let sumSparseMatrixGU  (a:SparseMatrix<_>) = 
            let ops = a.ElementOps 
            a |> nonZeroEntriesSparseMatrixGU |> Seq.fold (fun acc (_,_,aij) -> add ops acc aij) ops.Zero

        let sumVecGU (a:Vector<_>) = 
            let ops = a.ElementOps 
            foldVecGU (fun acc ai -> add ops acc ai) ops.Zero a

        let prodDenseMatrixGU (a:DenseMatrix<_>) = 
            let ops = a.ElementOps 
            foldDenseMatrixGU (fun acc aij -> mul ops acc aij) ops.One a

        let prodSparseMatrixGU  (a:SparseMatrix<_>) = a |> toDenseSparseMatrixGU |> prodDenseMatrixGU

        let inline fold2DenseMatrixGU f z (a:DenseMatrix<_>) (b:DenseMatrix<_>) =
            let nA = a.NumCols 
            let mA = a.NumRows
            let nB = b.NumCols 
            let mB = b.NumRows
            if nA <> nB || mA <> mB then invalidArg "b" "the two matrices do not have compatible dimensions"
            let arrA = a.Values 
            let arrB = b.Values 
            let mutable acc = z
            for i = 0 to mA-1 do
                for j = 0 to nA-1 do 
                   acc <- f acc (getArray2D arrA i j) (getArray2D arrB i j)
            acc

        let inline fold2VecGU f z (a:Vector<_>) (b:Vector<_>) =
            let mA = a.NumRows
            let mB = b.NumRows
            if  mA <> mB then invalidArg "b" "the two vectors do not have compatible dimensions"
            let mutable acc = z
            for i = 0 to mA-1 do acc <- f acc a.[i] b.[i]
            acc

        let dotDenseMatrixGU (a:DenseMatrix<_>) b =
            let ops = a.ElementOps 
            fold2DenseMatrixGU (fun z va vb -> add ops z (mul ops va vb)) ops.Zero a b

        let dotVecGU (a:Vector<_>) b =
            let ops =   a.ElementOps
            let zero = ops.Zero 
            fold2VecGU  (fun z va vb -> add ops z (mul ops va vb)) zero a b 

        let normDenseMatrixGU (a:DenseMatrix<_>) = 
            let normOps = getNormOps a.ElementOps
            foldDenseMatrixGU (fun z aij -> z + ((normOps.Norm aij)**2.0)) 0.0 a |> sqrt

        let normSparseMatrixGU (a:SparseMatrix<_>) = 
            let normOps = getNormOps a.ElementOps
            a |> nonZeroEntriesSparseMatrixGU |> Seq.fold (fun acc (_,_,aij) -> acc + ((normOps.Norm aij)**2.0)) 0.0 |> sqrt

        let inplaceAddDenseMatrixGU  (a:DenseMatrix<_>) (b:DenseMatrix<_>) = 
            let ops = a.ElementOps 
            let arrB = b.Values 
            inplace_mapiDenseMatrixGU  (fun i j x -> add ops x (getArray2D arrB i j)) a
        
        let inplaceAddVecGU  (a:Vector<_>) (b:Vector<_>) = 
            let ops = a.ElementOps 
            inplace_mapiVecGU  (fun i x   -> add ops x b.[i]) a

        let inplaceAddRowVecGU (a:RowVector<_>) (b:Vector<_>) = 
            let ops = a.ElementOps 
            inplace_mapiRowVecGU (fun i x   -> add ops x b.[i]) a

        let inplaceSubDenseMatrixGU  (a:DenseMatrix<_>) (b:DenseMatrix<_>) = 
            let ops = a.ElementOps 
            let arrB = b.Values 
            inplace_mapiDenseMatrixGU  (fun i j x -> sub ops x (getArray2D  arrB i j)) a

        let inplaceSubVecGU (a:Vector<_>) (b:Vector<_>) = 
            let ops = a.ElementOps
            inplace_mapiVecGU  (fun i x   -> sub ops x b.[i]) a

        let inplaceSubRowVecGU (a:RowVector<_>) (b:Vector<_>) = 
            let ops = a.ElementOps 
            inplace_mapiRowVecGU (fun i x   -> sub ops x b.[i]  ) a

        let inplaceCptMulDenseMatrixGU  (a:DenseMatrix<_>) (b:DenseMatrix<_>) = 
            let ops = a.ElementOps 
            let arrB = b.Values 
            inplace_mapiDenseMatrixGU  (fun i j x -> mul ops x (getArray2D  arrB i j)) a

        let inplaceCptMulVecGU (a:Vector<_>) (b:Vector<_>) = 
            let ops = a.ElementOps  
            inplace_mapiVecGU  (fun i x   -> mul ops x b.[i]) a

        let inplaceCptMulRowVecGU (a:RowVector<_>) (b:Vector<_>) = 
            let ops = a.ElementOps 
            inplace_mapiRowVecGU (fun i x   -> mul ops x b.[i]  ) a

        let inplaceScaleDenseMatrixGU  x (a:DenseMatrix<_>) = 
            let ops = a.ElementOps 
            inplace_mapiDenseMatrixGU  (fun _ _ y -> ops.Multiply(x,y)) a

        let inplaceScaleVecGU  x (a:Vector<_>) = 
            let ops = a.ElementOps  
            inplace_mapiVecGU  (fun _ y   -> ops.Multiply(x,y)) a

        let inplaceScaleRowVecGU x (a:RowVector<_>) = 
            let ops = a.ElementOps 
            inplace_mapiRowVecGU (fun _ y   -> ops.Multiply(x,y)) a


        let wrapList (pre,mid,post,trim) show l = 
            let post = if trim then "; ..." + post else post
            match l with 
            | []    -> [pre;post]
            | [x]   -> [pre;show x;post]
            | x::xs -> [pre;show x] @ (List.collect (fun x -> [mid;show x]) xs) @ [post]

        let showItem opsData  x = 
            try 
              let ops = opsOfOpsData opsData 
              ops.ToString(x,"g10",System.Globalization.CultureInfo.InvariantCulture) 
            with :? System.NotSupportedException -> (box x).ToString()
        
        let mapR f (n,m) = if m < n then [] else List.init (m-n+1) (fun i -> f (n+i))

        let primShowDenseMatrixGU (sepX,sepR) (a : DenseMatrix<'e>) =
            let nA = min a.NumCols 50
            let mA = min a.NumRows 50
            let ops = a.OpsData 
            let showLine i = wrapList ("[",";","]",a.NumCols > nA) (showItem ops) ((0,nA-1) |> mapR  (fun j -> a.[i,j])) |> Array.ofList |> System.String.Concat
            wrapList ("matrix [",";"+sepX,"]"+sepR,a.NumRows > mA) showLine [0..mA-1] |> Array.ofList |> System.String.Concat

        let showDenseMatrixGU     m = primShowDenseMatrixGU ("\n","\n") m
        let debugShowDenseMatrixGU m = primShowDenseMatrixGU (""  ,""  ) m
        
        let showVecGU s (a : Vector<_>) =
            let mA = min a.NumRows 100
            let ops = a.OpsData 
            wrapList (s+" [",";","]",a.NumRows > mA) (showItem ops) ((0,mA-1) |> mapR  (fun i -> a.[i])) |> Array.ofList |> System.String.Concat 

        let showRowVecGU s (a : RowVector<_>) =
            let mA = min a.NumCols 100
            let ops = a.OpsData 
            wrapList (s+" [",";","]",a.NumCols > mA) (showItem ops) ((0,mA-1) |> mapR  (fun i -> a.[i])) |> Array.ofList |> System.String.Concat 


    /// Implementations of operations specific to floating point types
    module DoubleImpl = 

        module GU = GenericImpl
        open Instances
        
        // Element type OpsData
        //type elem = float
        let zero = 0.0
        let one  = 1.0
        let inline sub (x:float) (y:float) = x - y
        let inline add (x:float) (y:float) = x + y
        let inline mul (x:float) (y:float) = x * y
        let inline neg (x:float) = -x

        // Specialized: these know the relevant set of 
        // ops without doing a table lookup based on runtime type
        let FloatOps = Some (FloatNumerics :> INumeric<float>)
        let inline initDenseMatrixDS m n f = GU.createDenseMatrixGU FloatOps m n f
        let inline createRowVecDS m f      = GU.createRowVecGU      FloatOps m f
        let inline createVecDS m f         = GU.createVecGU         FloatOps m f
        let inline mkDenseMatrixDS  arr    = GU.mkDenseMatrixGU     FloatOps arr
        let inline mkRowVecDS arr          = GU.mkRowVecGU          FloatOps arr
        let inline mkVecDS  arr            = GU.mkVecGU             FloatOps arr
        let inline listDenseMatrixDS  ll   = GU.listDenseMatrixGU   FloatOps ll
        let inline listRowVecDS l          = GU.listRowVecGU        FloatOps l
        let inline listVecDS  l            = GU.listVecGU           FloatOps l
        let inline seqDenseMatrixDS  ll    = GU.seqDenseMatrixGU    FloatOps ll
        let inline seqRowVecDS l           = GU.seqRowVecGU         FloatOps l
        let inline seqVecDS  l             = GU.seqVecGU            FloatOps l

        let constDenseMatrixDS  m n x      = GU.createDenseMatrixGU  FloatOps m n (fun _ _ -> x)
        let constRowVecDS m x              = GU.createRowVecGU FloatOps m   (fun _ -> x)
        let constVecDS  m x                = GU.createVecGU  FloatOps m   (fun _ -> x)
        let scalarDenseMatrixDS   x        = constDenseMatrixDS  1 1 x 
        let scalarRowVecDS  x              = constRowVecDS 1   x 
        let scalarVecDS   x                = constVecDS  1   x 

        // Beware - when compiled with non-generic code createArray2D creates an array of null values,
        // not zero values. Hence the optimized version can only be used when compiling with generics.
        let inline zeroDenseMatrixDS m n = 
          let arr = GU.createArray2D m n 
          GU.mkDenseMatrixGU FloatOps arr
        // Specialized: these inline down to the efficient loops we need
        let addDenseMatrixDS     a b = GU.binaryOpDenseMatrixGU  add a b
        let addSparseDS     a b = GU.binaryOpSparseMatrixGU  add a b
        let addRowVecDS    a b = GU.binaryOpRowVecGU add a b
        let addVecDS     a b = GU.binaryOpVecGU  add a b
        let subDenseMatrixDS     a b = GU.binaryOpDenseMatrixGU  sub a b 
        let subSparseDS     a b = GU.binaryOpSparseMatrixGU  sub a b 
        let mulSparseDS     a b = GU.genericMulSparse zero add mul a b
        let subRowVecDS    a b = GU.binaryOpRowVecGU sub a b 
        let subVecDS     a b = GU.binaryOpVecGU  sub a b 
        let cptMulDenseMatrixDS  a b = GU.binaryOpDenseMatrixGU  mul a b
        let cptMulSparseDS  a b = GU.binaryOpSparseMatrixGU  mul a b
        let cptMulRowVecDS a b = GU.binaryOpRowVecGU mul a b
        let cptMulVecDS  a b = GU.binaryOpVecGU  mul a b
        type smatrix = SparseMatrix<float>
        type dmatrix = DenseMatrix<float>
        type vector = Vector<float>
        type rowvec = RowVector<float>
        let cptMaxDenseMatrixDS  (a:dmatrix) (b:dmatrix) = GU.binaryOpDenseMatrixGU  max a b
        let cptMinDenseMatrixDS  (a:dmatrix) (b:dmatrix) = GU.binaryOpDenseMatrixGU  min a b
        let cptMaxSparseDS  (a:smatrix) (b:smatrix) = GU.binaryOpSparseMatrixGU  max a b
        let cptMinSparseDS  (a:smatrix) (b:smatrix) = GU.binaryOpSparseMatrixGU  min a b
        let cptMaxVecDS  (a:vector) (b:vector) = GU.binaryOpVecGU  max a b
        let cptMinVecDS  (a:vector) (b:vector) = GU.binaryOpVecGU  min a b

        // Don't make any mistake about these ones re. performance.
        let mulDenseMatrixDS (a:dmatrix) (b:dmatrix) =
            let nA = a.NumCols 
            let mA = a.NumRows
            let nB = b.NumCols 
            let mB = b.NumRows
            if nA<>mB then invalidArg "b" "the two matrices do not have compatible dimensions"
            let arr = GU.createArray2D mA nB 
            let arrA = a.Values 
            let arrB = b.Values 
            for i = 0 to mA - 1 do 
                for j = 0 to nB - 1 do 
                    let mutable r = 0.0 
                    for k = 0 to mB - 1 do 
                        r <- r + mul (GU.getArray2D arrA i k) (GU.getArray2D arrB k j)
                    GU.setArray2D arr i j r
            mkDenseMatrixDS arr

        let mulRowVecDenseMatrixDS (a:rowvec) (b:dmatrix) =
            let nA = a.NumCols 
            let nB = b.NumCols 
            let mB = b.NumRows
            if nA<>mB then invalidArg "b" "the two inputs do not have compatible dimensions"
            let arr = Array.zeroCreate nB 
            let arrA = a.Values 
            let arrB = b.Values 
            for j = 0 to nB - 1 do 
                let mutable r = 0.0 
                for k = 0 to mB - 1 do 
                    r <- r + mul arrA.[k] (GU.getArray2D arrB k j)
                arr.[j] <- r
            mkRowVecDS arr

        let mulDenseMatrixVecDS (a:dmatrix) (b:vector) =
            let nA = a.NumCols 
            let mA = a.NumRows
            let mB = b.NumRows 
            if nA<>mB then invalidArg "b" "the two inputs do not have compatible dimensions"
            let arr = Array.zeroCreate mA 
            let arrA = a.Values 
            let arrB = b.Values 
            for i = 0 to mA - 1 do 
                let mutable r = 0.0 
                for k = 0 to nA - 1 do 
                    r <- r + mul (GU.getArray2D arrA i k) arrB.[k]
                arr.[i] <- r
            mkVecDS arr

        let mulRowVecVecDS (a:rowvec) (b:vector) =
            let nA = a.NumCols 
            let mB = b.NumRows 
            if nA<>mB then invalidArg "b" "the two vectors do not have compatible dimensions"
            let arrA = a.Values 
            let arrB = b.Values 
            let mutable r = 0.0 
            for k = 0 to nA - 1 do 
                r <- r + mul arrA.[k] arrB.[k]
            r

        let rowvecDenseMatrixDS (x:rowvec) = initDenseMatrixDS 1          x.NumCols (fun _ j -> x.[j]) 
        let vectorDenseMatrixDS (x:vector) = initDenseMatrixDS x.NumRows  1         (fun i _ -> x.[i]) 
        let mulVecRowVecDS a b = mulDenseMatrixDS (vectorDenseMatrixDS a) (rowvecDenseMatrixDS b) 

        let scaleDenseMatrixDS   k m = GU.unaryOpDenseMatrixGU  (fun x -> mul k x) m
        let scaleSparseDS   k m = GU.unaryOpSparseGU  (fun x -> mul k x) m
        let scaleRowVecDS  k m = GU.unaryOpRowVecGU (fun x -> mul k x) m
        let scaleVecDS   k m = GU.unaryOpVectorGU  (fun x -> mul k x) m
        let negDenseMatrixDS     m   = GU.unaryOpDenseMatrixGU  (fun x -> neg x) m
        let negSparseDS     m   = GU.unaryOpSparseGU  (fun x -> neg x) m
        let negRowVecDS    m   = GU.unaryOpRowVecGU (fun x -> neg x) m
        let negVecDS     m   = GU.unaryOpVectorGU  (fun x -> neg x) m

        let traceDenseMatrixDS (a:dmatrix) =
            let nA = a.NumCols 
            let mA = a.NumRows
            if nA<>mA then invalidArg "a" "expected a square matrix";
            let arrA = a.Values 
            (0,nA-1) |> GU.sumfR (fun i -> GU.getArray2D arrA i i) 

        let sumDenseMatrixDS  a = GU.foldDenseMatrixGU add zero a
        let sumVecDS   a = GU.foldVecGU  add zero a
        let prodDenseMatrixDS a = GU.foldDenseMatrixGU mul one  a
        let prodVecDS  a = GU.foldVecGU  mul one  a

        let dotDenseMatrixDS a b = GU.fold2DenseMatrixGU (fun z va vb -> add z (mul va vb)) zero a b
        let dotVecDS a b = GU.fold2VecGU (fun z va vb -> add z (mul va vb)) zero a b
        let sumfDenseMatrixDS  f m = GU.foldDenseMatrixGU (fun acc aij -> add acc (f aij)) zero m
        let normDenseMatrixDS m = sqrt (sumfDenseMatrixDS (fun x -> x*x) m)

        let inplaceAddDenseMatrixDS  a (b:DenseMatrix<_>) = let arrB = b.Values  in GU.inplace_mapiDenseMatrixGU  (fun i j x -> x + GU.getArray2D arrB i j) a
        let inplaceAddVecDS    a (b:Vector<_>) = let arrB = b.Values  in GU.inplace_mapiVecGU  (fun i x   -> x + arrB.[i]) a
        let inplace_addRowVecDS a (b:RowVector<_>) = let arrB = b.Values in GU.inplace_mapiRowVecGU (fun i x   -> x + arrB.[i]) a
        let inplaceSubDenseMatrixDS  a (b:DenseMatrix<_>) = let arrB = b.Values  in GU.inplace_mapiDenseMatrixGU  (fun i j x -> x - GU.getArray2D  arrB i j) a
        let inplaceSubVecDS  a (b:Vector<_>) = let arrB = b.Values  in GU.inplace_mapiVecGU  (fun i x   -> x - arrB.[i]) a
        let inplace_subRowVecDS a (b:RowVector<_>) = let arrB = b.Values in GU.inplace_mapiRowVecGU (fun i x   -> x - arrB.[i]) a
        let inplaceCptMulDenseMatrixDS  a (b:DenseMatrix<_>) = let arrB = b.Values  in GU.inplace_mapiDenseMatrixGU  (fun i j x -> x * GU.getArray2D arrB i j) a
        let inplaceCptMulVecDS  a (b:Vector<_>) = let arrB = b.Values  in GU.inplace_mapiVecGU  (fun i x   -> x * arrB.[i]) a
        let inplace_cptMulRowVecDS a (b:RowVector<_>) = let arrB = b.Values in GU.inplace_mapiRowVecGU (fun i x   -> x * arrB.[i]) a
        let inplaceScaleDenseMatrixDS  (a:float) b = GU.inplace_mapiDenseMatrixGU  (fun _ _ x -> a * x) b
        let inplaceScaleVecDS  (a:float) b = GU.inplace_mapiVecGU  (fun _ x   -> a * x) b
        let inplace_scaleRowVecDS (a:float) b = GU.inplace_mapiRowVecGU (fun _ x   -> a * x) b



    /// Generic operations that, when used on floating point types, use the specialized versions in DoubleImpl
    module SpecializedGenericImpl = 

        open Microsoft.FSharp.Math.Instances
        open Microsoft.FSharp.Math.GlobalAssociations

        module GU = GenericImpl
        module DS = DoubleImpl

          
        type smatrix = SparseMatrix<float>
        type dmatrix = DenseMatrix<float>
        type vector = Vector<float>
        type rowvec = RowVector<float>
        let inline dense x = DenseRepr(x)
        let inline sparse x = SparseRepr(x)
        let inline createMx  ops m n f = GU.createDenseMatrixGU ops m n f |> dense
        let inline createVx  ops m f   = GU.createVecGU ops m f
        let inline createRVx ops m f   = GU.createRowVecGU ops m f

        let nonZeroEntriesM a   = 
            match a with 
            | DenseRepr a -> GU.nonzeroEntriesDenseMatrixGU a 
            | SparseRepr a -> GU.nonZeroEntriesSparseMatrixGU a 

        /// Merge two sorted sequences
        let mergeSorted cf (s1: seq<'T>) (s2: seq<'b>) =
            seq { use e1 = s1.GetEnumerator()
                  use e2 = s2.GetEnumerator()
                  let havee1 = ref (e1.MoveNext())
                  let havee2 = ref (e2.MoveNext())
                  while !havee1 || !havee2 do
                    if !havee1 && !havee2 then
                        let v1 = e1.Current
                        let v2 = e2.Current
                        let c = cf v1 v2 
                        if c < 0 then 
                            do havee1 := e1.MoveNext()
                            yield Some(v1),None
                        elif c = 0 then
                            do havee1 := e1.MoveNext()
                            do havee2 := e2.MoveNext()
                            yield Some(v1),Some(v2)
                        else 
                            do havee2 := e2.MoveNext()
                            yield (None,Some(v2))
                    elif !havee1 then 
                        let v1 = e1.Current
                        do havee1 := e1.MoveNext()
                        yield (Some(v1),None)
                    else 
                        let v2 = e2.Current
                        do havee2 := e2.MoveNext()
                        yield (None,Some(v2)) }

        /// Non-zero entries from two sequences
        let mergedNonZeroEntriesM  (a:Matrix<_>) (b:Matrix<_>) = 
            let ops = a.ElementOps 
            let zero = ops.Zero
            mergeSorted (fun (i1,j1,_) (i2,j2,_) -> let c = compare i1 i2 in if c <> 0 then c else compare j1 j2) (nonZeroEntriesM a) (nonZeroEntriesM b)
            |> Seq.map (function | Some(i,j,v1),Some(_,_,v2) -> (v1,v2)
                                 | Some(i,j,v1),None         -> (v1,zero)
                                 | None,        Some(i,j,v2) -> (zero,v2)
                                 | None,        None          -> failwith "unreachable")


        
        // Creation
        let listM    xss : Matrix<'T>    = GU.listDenseMatrixGU opsdata<'T> xss |> dense
        let listV    xss : Vector<'T>    = GU.listVecGU         opsdata<'T> xss
        let listRV   xss : RowVector<'T> = GU.listRowVecGU      opsdata<'T> xss

        let arrayM    xss : Matrix<'T>    = GU.mkDenseMatrixGU  opsdata<'T> (Array2D.copy xss) |> dense
        let arrayV    xss : Vector<'T>    = GU.mkVecGU          opsdata<'T> (Array.copy xss)
        let arrayRV   xss : RowVector<'T> = GU.mkRowVecGU       opsdata<'T> (Array.copy xss)

        let seqM    xss : Matrix<'T>    = GU.seqDenseMatrixGU   opsdata<'T> xss |> dense
        let seqV    xss : Vector<'T>    = GU.seqVecGU           opsdata<'T> xss
        let seqRV   xss : RowVector<'T> = GU.seqRowVecGU        opsdata<'T> xss

        let initM  m n f : Matrix<'T>    = GU.createDenseMatrixGU opsdata<'T> m n f |> dense
        let initRV m   f : RowVector<'T> = GU.createRowVecGU      opsdata<'T> m   f
        let initV  m   f : Vector<'T>    = GU.createVecGU         opsdata<'T> m   f

        let constM  m n x : Matrix<'T>    = GU.createConstDenseMatrixGU opsdata<'T> m n x |> dense
        let constRV m   x : RowVector<'T> = GU.createConstRowVecGU      opsdata<'T> m   x
        let constV  m   x : Vector<'T>    = GU.createConstVecGU         opsdata<'T> m   x

        let inline inplaceAssignM  f a = 
            match a with 
            | SparseRepr _ -> sparseNotMutable()
            | DenseRepr a -> GU.assignDenseMatrixGU  f a
        let inline assignV  f a = GU.assignVecGU  f a

        let coerce2 x = unbox(box(x))
        let loosenDM (x: dmatrix) : DenseMatrix<_>  = coerce2 x
        let loosenSM (x: smatrix) : SparseMatrix<_> = coerce2 x
        let loosenV  (x: vector)  : Vector<_>       = coerce2 x
        let loosenRV (x: rowvec)  : RowVector<_>    = coerce2 x
        let loosenF  (x: float)   : 'T              = coerce2 x

        let tightenDM (x: DenseMatrix<_>)  : dmatrix = coerce2 x
        let tightenSM (x: SparseMatrix<_>) : smatrix = coerce2 x
        let tightenV  (x: Vector<_>)       : vector  = coerce2 x
        let tightenRV (x: RowVector<_>)    : rowvec  = coerce2 x
        let tightenF  (x: 'T)              : float   = coerce2 x

        let zeroM m n = 
            let arr = GU.createArray2D m n
            // This is quite performance critical
            // Avoid assigining zeros into the array
            match box arr with 
            | :? (float[,])   as arr -> GU.mkDenseMatrixGU DS.FloatOps arr |> loosenDM |> dense
            | _ -> 
            GU.zeroizeDenseMatrixGUA arr m n  |> dense

        let zeroV m  : Vector<'T> = 
            let arr = GU.createArray m 
            // Avoid assigining zeros into the array
            match box (arr: 'T[]) with 
            | :? (float[])   as arr -> GU.mkVecGU DS.FloatOps arr |> loosenV
            | _ -> 
            GU.zeroizeVecGUA arr m

        let zeroRV m  : RowVector<'T> = 
            let arr = GU.createArray m 
            // Avoid assigining zeros into the array
            match box (arr: 'T[]) with 
            | :? (float[])   as arr -> GU.mkRowVecGU DS.FloatOps arr |> loosenRV
            | _ -> 
            GU.zeroizeRowVecGUA arr m
            
        let initNumericM m n f   = 
            let arr = GU.createArray2D m n 
            let opsData = opsdata<'T> 
            let ops = GU.opsOfOpsData opsData 
            GU.assignArray2D m n (f ops) arr;
            GU.mkDenseMatrixGU opsData arr |> dense

        let identityM m   = 
            let arr = GU.createArray2D m m 
            // This is quite performance critical
            // Avoid assigining zeros into the array
            match box arr with 
            | :? (float[,])   as arr -> 
                for i = 0 to m - 1 do 
                   arr.[i,i] <- 1.0 
                GU.mkDenseMatrixGU DS.FloatOps arr |> loosenDM |> dense
            | _ -> 
            let opsData = opsdata<'T> 
            let ops = GU.opsOfOpsData opsData 
            let zero = ops.Zero 
            let one = ops.One 
            GU.assignArray2D m m (fun i j -> if i = j then one else zero) arr;
            GU.mkDenseMatrixGU opsData arr |> dense

        let createNumericV m f  : Vector<'T> = 
            let arr = GU.createArray m 
            let opsData = opsdata<'T> 
            let ops = GU.opsOfOpsData opsData 
            GU.assignArray m (f ops) arr;
            GU.mkVecGU opsData arr
            
        let scalarM   x = constM 1 1 x 
        let scalarRV  x = constRV 1 x 
        let scalarV   x = constV 1 x 

        let diagnM (v:Vector<_>) n = 
            let ops = v.ElementOps
            let zero = ops.Zero 
            let nV = v.NumRows + (if n < 0 then -n else n) 
            createMx v.OpsData nV nV (fun i j -> if i+n=j then v.[i] else zero)

        let diagM v = diagnM v 0

        let constDiagM  n x : Matrix<'T> = 
            let opsData = opsdata<'T> 
            let ops = GU.opsOfOpsData opsData 
            let zero = ops.Zero 
            createMx opsData n n (fun i j -> if i=j then x else zero) 

        // Note: we drop sparseness on pointwise multiplication of sparse and dense.
        let inline binaryOpM opDenseDS opDenseGU opSparseDS opSparseMatrixGU a b = 
            match a,b with 
            | DenseRepr a,DenseRepr b -> 
                match box a with 
                | (:? dmatrix as a) -> opDenseDS   a (tightenDM b) |> loosenDM |> dense
                | _                 -> opDenseGU a b                           |> dense
            | SparseRepr a,SparseRepr b ->
                match box a with 
                | (:? smatrix as a) -> opSparseDS a (tightenSM b) |> loosenSM |> sparse
                | _                 -> opSparseMatrixGU a b                         |> sparse
            | SparseRepr a, DenseRepr b     -> opDenseGU (GU.toDenseSparseMatrixGU a) b         |> dense
            | DenseRepr  a, SparseRepr b    -> opDenseGU a (GU.toDenseSparseMatrixGU b)         |> dense

        let inline unaryOpM opDenseDS opDenseGU opSparseDS opSparseMatrixGU  b = 
            match b with 
            | DenseRepr b -> 
                match box b with 
                | (:? dmatrix as b)  -> opDenseDS b |> loosenDM |> dense
                | _                  -> opDenseGU b             |> dense
            | SparseRepr b ->             
                match box b with 
                | (:? smatrix as b) -> opSparseDS b |> loosenSM |> sparse
                | _                 -> opSparseMatrixGU b             |> sparse

        let inline floatUnaryOpM opDenseDS opDenseGU opSparseDS opSparseMatrixGU  b = 
            match b with 
            | DenseRepr b -> 
                match box b with 
                | (:? dmatrix as b)  -> opDenseDS b |> loosenF
                | _                  -> opDenseGU b             
            | SparseRepr b ->             
                match box b with 
                | (:? smatrix as b) -> opSparseDS b |> loosenF 
                | _                 -> opSparseMatrixGU b             

        let addM a b = binaryOpM DS.addDenseMatrixDS GU.addDenseMatrixGU DS.addSparseDS GU.addSparseMatrixGU a b
        let subM a b = binaryOpM DS.subDenseMatrixDS GU.subDenseMatrixGU DS.subSparseDS GU.subSparseMatrixGU a b
        let mulM a b = binaryOpM DS.mulDenseMatrixDS GU.genericMulDenseMatrix DS.mulSparseDS GU.mulSparseMatrixGU a b
        let cptMulM a b = binaryOpM DS.cptMulDenseMatrixDS GU.cptMulDenseMatrixGU DS.cptMulSparseDS GU.cptMulSparseMatrixGU a b
        let cptMaxM a b = binaryOpM DS.cptMaxDenseMatrixDS GU.cptMaxDenseMatrixGU DS.cptMaxSparseDS GU.cptMaxSparseMatrixGU a b
        let cptMinM a b = binaryOpM DS.cptMinDenseMatrixDS GU.cptMinDenseMatrixGU DS.cptMinSparseDS GU.cptMinSparseMatrixGU a b

        let addRV a b = 
            match box a with 
            | (:? rowvec as a) -> DS.addRowVecDS a (tightenRV b) |> loosenRV
            | _                -> GU.addRowVecGU a b

        let addV a b = 
            match box a with 
            | (:? vector as a) -> DS.addVecDS a (tightenV b) |> loosenV
            | _                -> GU.addVecGU a b

        let subRV a b = 
            match box a with 
            | (:? rowvec as a) -> DS.subRowVecDS   a (tightenRV b) |> loosenRV
            | _                -> GU.subRowVecGU a b

        let subV a b = 
            match box a with 
            | (:? vector as a) -> DS.subVecDS   a (tightenV b) |> loosenV
            | _                -> GU.subVecGU a b

        let mulRVM a b = 
            match b with 
            | DenseRepr b -> 
                match box a with 
                | (:? rowvec as a) -> DS.mulRowVecDenseMatrixDS   a (tightenDM b) |> loosenRV
                | _                -> GU.mulRowVecDenseMatrixGU a b
            | SparseRepr b -> GU.mulRVSparseMatrixGU a b

        let mulMV a b = 
            match a with 
            | DenseRepr a -> 
                match box a with 
                | (:? dmatrix as a) -> DS.mulDenseMatrixVecDS   a (tightenV b) |> loosenV
                | _                 -> GU.mulDenseMatrixVecGU a b
            | SparseRepr a -> GU.mulSparseVecGU a b 

        let mulRVV a b = 
            match box a with 
            | (:? rowvec as a) -> DS.mulRowVecVecDS   a (tightenV b) |> loosenF
            | _                -> GU.mulRowVecVecGU a b

        let mulVRV a b = 
            match box a with 
            | (:? vector as a) -> DS.mulVecRowVecDS   a (tightenRV b) |> loosenDM |> dense
            | _                -> GU.mulVecRowVecGU a b |> dense

        let cptMulRV a b = 
            match box a with 
            | (:? rowvec as a) -> DS.cptMulRowVecDS   a (tightenRV b) |> loosenRV
            | _                -> GU.cptMulRowVecGU a b

        let cptMulV a b = 
            match box a with 
            | (:? vector as a) -> DS.cptMulVecDS   a (tightenV b) |> loosenV
            | _                -> GU.cptMulVecGU a b

        let cptMaxV a b = 
            match box a with 
            | (:? vector as a) -> DS.cptMaxVecDS   a (tightenV b) |> loosenV
            | _                -> GU.cptMaxVecGU a b

        let cptMinV a b = 
            match box a with 
            | (:? vector as a) -> DS.cptMinVecDS   a (tightenV b) |> loosenV
            | _                -> GU.cptMinVecGU a b

        let scaleM a b = unaryOpM (fun b -> DS.scaleDenseMatrixDS (tightenF a) b) (GU.scaleDenseMatrixGU a)
                                  (fun b -> DS.scaleSparseDS (tightenF a) b) (GU.scaleSparseMatrixGU a) b

        let scaleRV a b = 
            match box b with 
            | (:? rowvec as b)  -> DS.scaleRowVecDS (tightenF a) b |> loosenRV 
            | _                 -> GU.scaleRowVecGU a b

        let scaleV a b = 
            match box b with 
            | (:? vector as b)  -> DS.scaleVecDS (tightenF a) b |> loosenV
            | _                 -> GU.scaleVecGU a b

        let dotM a b = 
            match a,b with 
            | DenseRepr a,DenseRepr b -> 
                match box b with 
                | (:? dmatrix as b)  -> DS.dotDenseMatrixDS   (tightenDM a) b |> loosenF
                | _                  -> GU.dotDenseMatrixGU a b
            | _ ->  
                let ops = a.ElementOps 
                mergedNonZeroEntriesM a b |> Seq.fold (fun z (va,vb) -> GU.add ops z (GU.mul ops va vb)) ops.Zero 

        let dotV a b = 
            match box b with 
            | (:? vector as b)  -> DS.dotVecDS   (tightenV a) b |> loosenF
            | _                 -> GU.dotVecGU a b

        let negM a = unaryOpM DS.negDenseMatrixDS GU.negDenseMatrixGU DS.negSparseDS GU.negSparseMatrixGU a

        let negRV a = 
            match box a with 
            | (:? rowvec as a) -> DS.negRowVecDS a |> loosenRV
            | _               ->  GU.negRowVecGU a

        let negV a = 
            match box a with 
            | (:? vector as a) -> DS.negVecDS a |> loosenV
            | _               ->  GU.negVecGU a

        let traceMGU (a:Matrix<_>) =
            let nA = a.NumCols  
            let mA = a.NumRows 
            if nA<>mA then invalidArg "a" "expected a square matrix";
            let ops = a.ElementOps 
            (0,nA-1) |> GU.sumRGU ops (fun i -> a.[i,i]) 

        let traceM a = floatUnaryOpM DS.traceDenseMatrixDS (dense >> traceMGU) (sparse >> traceMGU) (sparse >> traceMGU) a
        let sumM a = floatUnaryOpM DS.sumDenseMatrixDS GU.sumDenseMatrixGU GU.sumSparseMatrixGU GU.sumSparseMatrixGU a
        let prodM a = floatUnaryOpM DS.prodDenseMatrixDS GU.prodDenseMatrixGU GU.prodSparseMatrixGU GU.prodSparseMatrixGU a
        let normM a = floatUnaryOpM DS.normDenseMatrixDS GU.normDenseMatrixGU GU.normSparseMatrixGU GU.normSparseMatrixGU a

        let opsM a = 
            match a with 
            | DenseRepr a -> a.OpsData 
            | SparseRepr a -> a.OpsData 
        
        let transM a = 
            match a with 
            | DenseRepr a -> 
                createMx a.OpsData a.NumCols a.NumRows (fun i j -> a.[j,i])
            | SparseRepr a -> 
                a |> GU.nonZeroEntriesSparseMatrixGU  |> Seq.map (fun (i,j,v) -> (j,i,v)) |> GU.initSparseMatrixGU a.NumCols a.NumRows a.OpsData |> sparse
        
        let permuteRows (p: permutation) a =
            match a with
            | DenseRepr a ->
                createMx a.OpsData a.NumCols a.NumRows (fun i j -> a.[p i,j])
            | SparseRepr a ->
                a |> GU.nonZeroEntriesSparseMatrixGU  |> Seq.map (fun (i,j,v) -> (p i,j,v)) |> GU.initSparseMatrixGU a.NumCols a.NumRows a.OpsData |> sparse

        let permuteColumns (p: permutation) a =
            match a with
            | DenseRepr a ->
                createMx a.OpsData a.NumCols a.NumRows (fun i j -> a.[i,p j])
            | SparseRepr a ->
                a |> GU.nonZeroEntriesSparseMatrixGU  |> Seq.map (fun (i,j,v) -> (i,p j,v)) |> GU.initSparseMatrixGU a.NumCols a.NumRows a.OpsData |> sparse

        let transRV (a:RowVector<_>) = 
            createVx a.OpsData  a.NumCols (fun i -> a.[i])

        let transV (a:Vector<_>)  = 
            createRVx a.OpsData a.NumRows (fun i -> a.[i])

        let inplaceAddM a b = 
            match a,b with 
            | DenseRepr a,DenseRepr b -> 
                match box a with 
                | (:? dmatrix as a) -> DS.inplaceAddDenseMatrixDS   a (tightenDM b)
                | _                 -> GU.inplaceAddDenseMatrixGU a b
            | _ -> sparseNotMutable()

        let inplaceAddV a b = 
            match box a with 
            | (:? vector as a) -> DS.inplaceAddVecDS   a (tightenV b)
            | _                -> GU.inplaceAddVecGU a b

        let inplaceSubM a b = 
            match a,b with 
            | DenseRepr a,DenseRepr b -> 
                match box a with 
                | (:? dmatrix as a) -> DS.inplaceSubDenseMatrixDS   a (tightenDM b)
                | _                -> GU.inplaceSubDenseMatrixGU a b
            | _ -> sparseNotMutable()

        let inplaceSubV a b = 
            match box a with 
            | (:? vector as a) -> DS.inplaceSubVecDS   a (tightenV b)
            | _                -> GU.inplaceSubVecGU a b


        let inplaceCptMulM a b = 
            match a,b with 
            | DenseRepr a,DenseRepr b -> 
                match box a with 
                | (:? dmatrix as a) -> DS.inplaceCptMulDenseMatrixDS   a (tightenDM b)
                | _                -> GU.inplaceCptMulDenseMatrixGU a b
            | _ -> sparseNotMutable()

        let inplaceCptMulV a b = 
            match box a with 
            | (:? vector as a) -> DS.inplaceCptMulVecDS   a (tightenV b)
            | _                -> GU.inplaceCptMulVecGU a b

        let inplaceScaleM a b = 
            match b with 
            | DenseRepr b -> 
                match box b with 
                | (:? dmatrix as b)  -> DS.inplaceScaleDenseMatrixDS   (tightenF a) b
                | _                 -> GU.inplaceScaleDenseMatrixGU a b
            | _ -> sparseNotMutable()

        let inplaceScaleV a b = 
            match box b with 
            | (:? vector as b)  -> DS.inplaceScaleVecDS   (tightenF a) b
            | _                 -> GU.inplaceScaleVecGU a b

        let existsM  f a = 
            match a with 
            | SparseRepr _ -> sparseNYI() // note: martin says "run f on a token element if it's not full"
            | DenseRepr a -> GU.existsiDenseMatrixGU  (fun _ _ -> f) a

        let existsV  f a = GU.existsiVecGU  (fun _ -> f) a

        let forallM  f a = 
            match a with 
            | SparseRepr _ -> sparseNYI()
            | DenseRepr a -> GU.foralliDenseMatrixGU  (fun _ _ -> f) a

        let forallV  f a = GU.foralliVecGU  (fun _ -> f) a

        let existsiM  f a = 
            match a with 
            | SparseRepr _ -> sparseNYI()
            | DenseRepr a -> GU.existsiDenseMatrixGU  f a

        let existsiV  f a = GU.existsiVecGU  f a

        let foralliM  f a = 
            match a with 
            | SparseRepr _ -> sparseNYI()
            | DenseRepr a -> GU.foralliDenseMatrixGU  f a

        let foralliV  f a = GU.foralliVecGU  f a

        let mapM  f a = 
            match a with 
            | SparseRepr _ -> sparseNYI()
            | DenseRepr a -> DenseRepr(GU.mapDenseMatrixGU f a)

        let mapV  f a = GU.mapVecGU f a

        let copyM  a = 
            match a with 
            | SparseRepr a -> SparseRepr (GU.copySparseGU a)
            | DenseRepr a -> DenseRepr (GU.copyDenseMatrixGU a)

        let copyV  a = GU.copyVecGU a

        let copyRV  a = GU.copyRowVecGU a

        let mapiM  f a = 
            match a with 
            | SparseRepr _ -> sparseNYI()
            | DenseRepr a -> DenseRepr (GU.mapiDenseMatrixGU f a)

        let mapiV  f a = GU.mapiVecGU f a
        let permuteV p a = GU.permuteVecGU p a
        let permuteRV p a = GU.permuteRowVecGU p a

        let mapiRV  f a = GU.mapiRowVecGU f a

        let toDenseM a = 
            match a with 
            | SparseRepr a -> GU.toDenseSparseMatrixGU a |> dense
            | DenseRepr _ -> a

        let initSparseM i j x : Matrix<'T> = 
            let opsData = opsdata<'T> 
            GU.initSparseMatrixGU i j opsData x |> sparse
          
        let initDenseM i j x : Matrix<'T> = 
            let r = zeroM i j
            x |> Seq.iter (fun (i,j,v) -> r.[i,j] <- v);
            r

        let getDiagnM (a:Matrix<_>) n =
            let nA = a.NumCols 
            let mA = a.NumRows
            if nA<>mA then invalidArg "a" "expected a square matrix";
            let ni = if n < 0 then -n else 0 
            let nj = if n > 0 then  n else 0 
            GU.createVecGU (opsM a) (max (nA-abs(n)) 0) (fun i -> a.[i+ni,i+nj]) 

        let getDiagM  a = getDiagnM a 0

        let inline inplace_mapiM  f a = 
            match a with 
            | SparseRepr _ -> sparseNotMutable()
            | DenseRepr a -> GU.inplace_mapiDenseMatrixGU f a

        let inline inplace_mapiV  f a = GU.inplace_mapiVecGU f a
        
        let inline foldM  f z a = 
            match a with 
            | SparseRepr _ -> sparseNYI()
            | DenseRepr a -> GU.foldDenseMatrixGU f z a

        let inline foldV  f z a = GU.foldVecGU f z a

        let inline foldiM  f z a = 
            match a with 
            | SparseRepr _ -> sparseNYI()
            | DenseRepr a -> GU.foldiDenseMatrixGU f z a

        let inline foldiV  f z a = GU.foldiVecGU f z a

        let compareM (comp: IComparer) (a:Matrix<'T>) (b:Matrix<'T>) = 
            let nA = a.NumCols 
            let mA = a.NumRows 
            let nB = b.NumCols 
            let mB = b.NumRows 
            let c = compare mA mB 
            if c <> 0 then c else
            let c = compare nA nB 
            if c <> 0 then c else
            match a,b with 
            | DenseRepr a, DenseRepr b -> 
              let rec go2 i j = 
                 if j < nA then 
                   let c = comp.Compare( a.[i,j], b.[i,j])
                   if c <> 0 then c else 
                   go2 i (j+1) 
                 else 0 
              let rec go1 i = 
                 if i < mA then 
                   let c = go2 i 0 
                   if c <> 0 then c 
                   else go1 (i+1) 
                 else 0 
              go1 0
            | _ -> 
              match (mergedNonZeroEntriesM a b |> Seq.tryPick (fun (v1,v2) -> let c = comp.Compare(v1,v2) in if c = 0 then None else Some(c))) with
              | None -> 0
              | Some(c) -> c
             
        let equalsM (comp: IEqualityComparer) (a:Matrix<'T>) (b:Matrix<'T>) = 
            let nA = a.NumCols 
            let mA = a.NumRows 
            let nB = b.NumCols 
            let mB = b.NumRows 
            (mA = mB ) && (nA = nB) && 
            match a,b with 
            | DenseRepr a, DenseRepr b -> 
                let rec go2 i j =  j >= nA || (comp.Equals( a.[i,j], b.[i,j]) && go2 i (j+1) )
                let rec go1 i = i >= mA || (go2 i 0  && go1 (i+1))
                go1 0
            | _ -> 
                mergedNonZeroEntriesM a b |> Seq.forall (fun (v1,v2) -> comp.Equals(v1,v2))
             

        let compareV (comp: IComparer) (a:Vector<'T>) (b:Vector<'T>) = 
            let mA = a.NumRows
            let mB = b.NumRows 
            let c = compare mA mB 
            if c <> 0 then c else
            let rec go2 j = 
               if j < mA then 
                 let c = comp.Compare(a.[j],b.[j])
                 if c <> 0 then c else go2 (j+1) 
               else 0 
            go2 0

        let equalsV (comp: IEqualityComparer) (a:Vector<'T>) (b:Vector<'T>) = 
            let mA = a.NumRows
            let mB = b.NumRows 
            (mA = mB) && 
            let rec go2 j = (j >= mA) || (comp.Equals(a.[j],b.[j]) && go2 (j+1))
            go2 0

        let equalsRV (comp: IEqualityComparer) (a:RowVector<'T>) (b:RowVector<'T>) = 
            let mA = a.NumCols 
            let mB = b.NumCols 
            (mA = mB) && 
            let rec go2 j = (j >= mA) || (comp.Equals(a.[j],b.[j]) && go2 (j+1))
            go2 0

        let compareRV (comp: IComparer) (a:RowVector<'T>) (b:RowVector<'T>) = 
            let mA = a.NumCols 
            let mB = b.NumCols 
            let c = compare mA mB 
            if c <> 0 then c else
            let rec go2 j = 
               if j < mA then 
                 let c = comp.Compare(a.[j],b.[j])
                 if c <> 0 then c else go2 (j+1) 
               else 0 
            go2 0

        let inline combineHash x y = (x <<< 1) + y + 631 

        let hashM (comp:IEqualityComparer) (a:Matrix<_>) = 
            let nA = a.NumCols 
            let mA = a.NumRows 
            let acc = hash mA + hash nA
            a |> nonZeroEntriesM |> Seq.truncate 20 |> Seq.fold (fun z v -> combineHash z (comp.GetHashCode v)) acc
          
        let hashV (comp:IEqualityComparer) (a:Vector<_>) = 
            let mA = a.NumRows 
            hash mA +
            (let mutable c = 0 
             for i = 0 to mA - 1 do
                 c <- combineHash c (comp.GetHashCode a.[i])
             c)
          
        let hashRV (comp:IEqualityComparer) (a:RowVector<_>) = 
            let mA = a.NumCols 
            hash mA +
            (let mutable c = 0 
             for i = 0 to mA - 1 do
                 c <- combineHash c (comp.GetHashCode a.[i])
             c)
          
        type range = int * int

        let startR ((a,_) : range)   = a
        let countR ((a,b) : range)   = (b-a)+1
        let idxR    ((a,_) : range) i = a+i
        let inR    ((a,b) : range) i = a <= i && i <= b
        
        let getRowM  (a:Matrix<_>) i = createRVx (opsM a) a.NumCols (fun j -> a.[i,j])
        let selColM  (a:Matrix<_>) j = createVx (opsM a) a.NumRows (fun i -> a.[i,j]) 
        let getRegionV  (a:Vector<_>)    r      = createVx a.OpsData (countR r) (fun i -> a.[idxR r i]) 
        let getRegionRV (a:RowVector<_>) r      = createRVx a.OpsData (countR r) (fun i -> a.[idxR r i]) 

        let getRegionM  a ri rj    = 
            match a with 
            | DenseRepr a -> createMx a.OpsData (countR ri) (countR rj) (fun i j -> a.[idxR ri i, idxR rj j]) 
            | _ -> nonZeroEntriesM a 
                   |> Seq.filter (fun (i,j,_) -> inR ri i && inR rj j) 
                   |> Seq.map (fun (i,j,v) -> (i-startR ri,j-startR rj,v)) 
                   |> initSparseM (countR ri) (countR rj)

        let getColsM (a:Matrix<_>) rj         = getRegionM a (0,a.NumRows - 1) rj
        let getRowsM (a:Matrix<_>) ri         = getRegionM a ri (0,a.NumCols - 1)

        let rowvecM (x:RowVector<_>) = initM 1         x.NumCols (fun _ j -> x.[j]) 
        let vectorM (x:Vector<_>) = initM x.NumRows  1         (fun i _ -> x.[i])  
        let toVectorM x = selColM x 0 
        let toRowVectorM x = getRowM x 0 
        let toScalarM (x:Matrix<_>) = x.[0,0]



//----------------------------------------------------------------------------
// type Matrix<'T> augmentation
//--------------------------------------------------------------------------*)

    type Matrix<'T> with
        static member ( +  )(a: Matrix<'T>,b) = SpecializedGenericImpl.addM a b
        static member ( -  )(a: Matrix<'T>,b) = SpecializedGenericImpl.subM a b
        static member ( *  )(a: Matrix<'T>,b) = SpecializedGenericImpl.mulM a b
        static member ( *  )(a: Matrix<'T>,b : Vector<'T>) = SpecializedGenericImpl.mulMV a b

        static member ( * )((m: Matrix<'T>),k : 'T) = SpecializedGenericImpl.scaleM k m

        static member ( .* )(a: Matrix<'T>,b) = SpecializedGenericImpl.cptMulM a b
        static member ( * )(k,m: Matrix<'T>) = SpecializedGenericImpl.scaleM k m
        static member ( ~- )(m: Matrix<'T>)     = SpecializedGenericImpl.negM m
        static member ( ~+ )(m: Matrix<'T>)     = m

        member m.GetSlice (start1,finish1,start2,finish2) = 
            let start1 = match start1 with None -> 0 | Some v -> v 
            let finish1 = match finish1 with None -> m.NumRows - 1 | Some v -> v 
            let start2 = match start2 with None -> 0 | Some v -> v 
            let finish2 = match finish2 with None -> m.NumCols - 1 | Some v -> v 
            SpecializedGenericImpl.getRegionM m (start1,finish1) (start2,finish2)

        member m.SetSlice (start1,finish1,start2,finish2,vs:Matrix<_>) = 
            let start1 = match start1 with None -> 0 | Some v -> v 
            let finish1 = match finish1 with None -> m.NumRows - 1 | Some v -> v 
            let start2 = match start2 with None -> 0 | Some v -> v 
            let finish2 = match finish2 with None -> m.NumCols - 1 | Some v -> v 
            for i = start1 to finish1  do 
               for j = start2 to finish2 do
                   m.[i,j] <- vs.[i-start1,j-start2]

        override m.ToString() = 
           match m with 
           | DenseRepr m -> GenericImpl.showDenseMatrixGU m
           | SparseRepr _ -> "<sparse>"

        member m.DebugDisplay = 
           let txt = 
               match m with 
               | DenseRepr m -> GenericImpl.debugShowDenseMatrixGU m
               | SparseRepr _ -> "<sparse>"
           new System.Text.StringBuilder(txt)  // return an object with a ToString with the right value, rather than a string. (strings get shown using quotes)

        member m.StructuredDisplayAsArray =  
            let rec layout m = 
                match m with 
                | DenseRepr _ -> box (Array2D.init m.NumRows m.NumCols (fun i j -> m.[i,j]))
                | SparseRepr _ -> (if m.NumRows < 20 && m.NumCols < 20 then layout (SpecializedGenericImpl.toDenseM m) else box(SpecializedGenericImpl.nonZeroEntriesM m))
            layout m
        member m.Dimensions = m.NumRows,m.NumCols

        member m.Transpose = SpecializedGenericImpl.transM m
        member m.PermuteRows (p: permutation) : Matrix<'T> = SpecializedGenericImpl.permuteRows p m
        member m.PermuteColumns (p: permutation) : Matrix<'T> = SpecializedGenericImpl.permuteColumns p m

        interface IEnumerable<'T> with 
            member m.GetEnumerator() = 
               (seq { for i in 0 .. m.NumRows-1 do
                        for j in 0 .. m.NumCols - 1 do 
                            yield m.[i,j] }).GetEnumerator()

        interface IEnumerable with 
            member m.GetEnumerator() =  ((m :> IEnumerable<_>).GetEnumerator() :> IEnumerator)
                                    
        interface System.IComparable with 
             member m.CompareTo(yobj:obj) = SpecializedGenericImpl.compareM LanguagePrimitives.GenericComparer m (yobj :?> Matrix<'T>)
             
        interface IStructuralComparable with
            member m.CompareTo(yobj:obj,comp:System.Collections.IComparer) = SpecializedGenericImpl.compareM comp m (yobj :?> Matrix<'T>)
            
        override m.GetHashCode() = SpecializedGenericImpl.hashM LanguagePrimitives.GenericEqualityComparer m 
        override m.Equals(yobj:obj) = 
            match yobj with 
            | :? Matrix<'T> as m2 -> SpecializedGenericImpl.equalsM LanguagePrimitives.GenericEqualityComparer m m2
            | _ -> false
        
        interface IStructuralEquatable with
            member m.GetHashCode(comp:System.Collections.IEqualityComparer) = SpecializedGenericImpl.hashM comp m
            member m.Equals(yobj:obj,comp:System.Collections.IEqualityComparer) = 
                match yobj with 
                | :? Matrix<'T> as m2 -> SpecializedGenericImpl.equalsM comp m m2
                | _ -> false


//----------------------------------------------------------------------------
// type Vector<'T> augmentation
//--------------------------------------------------------------------------*)

    type Vector<'T> with
        static member ( +  )(a: Vector<'T>,b) = SpecializedGenericImpl.addV a b
        static member ( -  )(a: Vector<'T>,b) = SpecializedGenericImpl.subV a b
        static member ( .* )(a: Vector<'T>,b) = SpecializedGenericImpl.cptMulV a b
        
        static member ( * )(k,m: Vector<'T>) = SpecializedGenericImpl.scaleV k m
        
        static member ( * )(a: Vector<'T>,b) = SpecializedGenericImpl.mulVRV a b
        
        static member ( * )(m: Vector<'T>,k) = SpecializedGenericImpl.scaleV k m
        
        static member ( ~- )(m: Vector<'T>)     = SpecializedGenericImpl.negV m
        static member ( ~+ )(m: Vector<'T>)     = m

        member m.GetSlice (start,finish) = 
            let start = match start with None -> 0 | Some v -> v 
            let finish = match finish with None -> m.NumRows - 1 | Some v -> v 
            SpecializedGenericImpl.getRegionV m (start,finish)

        member m.SetSlice (start,finish,vs:Vector<_>) = 
            let start = match start with None -> 0 | Some v -> v 
            let finish = match finish with None -> m.NumRows - 1 | Some v -> v 
            for i = start to finish  do 
                   m.[i] <- vs.[i-start]


        override m.ToString() = GenericImpl.showVecGU "vector" m

        member m.DebugDisplay = 
            let txt = GenericImpl.showVecGU "vector" m
            new System.Text.StringBuilder(txt)  // return an object with a ToString with the right value, rather than a string. (strings get shown using quotes)

        member m.StructuredDisplayAsArray =  Array.init m.NumRows (fun i -> m.[i])

        member m.Details = m.Values

        member m.Transpose = SpecializedGenericImpl.transV m
        
        member m.Permute (p:permutation) = SpecializedGenericImpl.permuteV p m
      
        interface System.IComparable with 
             member m.CompareTo(y:obj) = SpecializedGenericImpl.compareV LanguagePrimitives.GenericComparer m (y :?> Vector<'T>)
        
        interface IStructuralComparable with
            member m.CompareTo(y:obj,comp:System.Collections.IComparer) = SpecializedGenericImpl.compareV comp m (y :?> Vector<'T>)

        interface IStructuralEquatable with
            member x.GetHashCode(comp) = SpecializedGenericImpl.hashV comp x
            member x.Equals(yobj,comp) = 
                match yobj with 
                | :? Vector<'T> as v2 -> SpecializedGenericImpl.equalsV comp x v2
                | _ -> false

        override x.GetHashCode() = 
            SpecializedGenericImpl.hashV LanguagePrimitives.GenericEqualityComparer x

        override x.Equals(yobj) = 
            match yobj with 
            | :? Vector<'T> as v2 -> SpecializedGenericImpl.equalsV LanguagePrimitives.GenericEqualityComparer x v2
            | _ -> false

//----------------------------------------------------------------------------
// type RowVector<'T> augmentation
//--------------------------------------------------------------------------*)

    type RowVector<'T> with
        static member ( +  )(a: RowVector<'T>,b) = SpecializedGenericImpl.addRV a b
        static member ( -  )(a: RowVector<'T>,b) = SpecializedGenericImpl.subRV a b
        static member ( .* )(a: RowVector<'T>,b) = SpecializedGenericImpl.cptMulRV a b
        static member ( * )(k,v: RowVector<'T>) = SpecializedGenericImpl.scaleRV k v
        
        static member ( * )(a: RowVector<'T>,b: Matrix<'T>) = SpecializedGenericImpl.mulRVM a b
        static member ( * )(a: RowVector<'T>,b:Vector<'T>) = SpecializedGenericImpl.mulRVV a b
        static member ( * )(v: RowVector<'T>,k:'T) = SpecializedGenericImpl.scaleRV k v
        
        static member ( ~- )(v: RowVector<'T>)     = SpecializedGenericImpl.negRV v
        static member ( ~+ )(v: RowVector<'T>)     = v

        member m.GetSlice (start,finish) = 
            let start = match start with None -> 0 | Some v -> v
            let finish = match finish with None -> m.NumCols - 1 | Some v -> v 
            SpecializedGenericImpl.getRegionRV m (start,finish)

        member m.SetSlice (start,finish,vs:RowVector<_>) = 
            let start = match start with None -> 0 | Some v -> v 
            let finish = match finish with None -> m.NumCols - 1 | Some v -> v 
            for i = start to finish  do 
                   m.[i] <- vs.[i-start]

        override m.ToString() = GenericImpl.showRowVecGU "rowvec" m

        member m.DebugDisplay = 
            let txt = GenericImpl.showRowVecGU "rowvec" m
            new System.Text.StringBuilder(txt)  // return an object with a ToString with the right value, rather than a string. (strings get shown using quotes)

        member m.StructuredDisplayAsArray =  Array.init m.NumCols (fun i -> m.[i])

        member m.Details = m.Values

        member m.Transpose = SpecializedGenericImpl.transRV m
        
        member m.Permute (p:permutation) = SpecializedGenericImpl.permuteRV p m
      
        interface System.IComparable with 
            member m.CompareTo(y) = SpecializedGenericImpl.compareRV LanguagePrimitives.GenericComparer m (y :?> RowVector<'T>)
        
        interface IStructuralComparable with
            member m.CompareTo(y,comp) = SpecializedGenericImpl.compareRV comp m (y :?> RowVector<'T>)

        interface IStructuralEquatable with
            member x.GetHashCode(comp) = SpecializedGenericImpl.hashRV comp x
            member x.Equals(yobj,comp) = 
                match yobj with 
                | :? RowVector<'T> as rv2 -> SpecializedGenericImpl.equalsRV comp x rv2
                | _ -> false

        override x.GetHashCode() = 
            SpecializedGenericImpl.hashRV LanguagePrimitives.GenericEqualityComparer x

        override x.Equals(yobj) = 
            match yobj with 
            | :? RowVector<'T> as rv2 -> SpecializedGenericImpl.equalsRV LanguagePrimitives.GenericEqualityComparer x rv2
            | _ -> false

    type matrix = Matrix<float>
    type vector = Vector<float>
    type rowvec = RowVector<float>

    module MRandom = 
        let seed = 99
        let randomGen = new System.Random(seed)
        let float f = randomGen.NextDouble() * f 


    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module Matrix = begin
        
        module Generic = begin

            module MS = SpecializedGenericImpl

            // Accessors
            let get (a:Matrix<_>) i j   = a.[i,j]
            let set (a:Matrix<_>) i j x = a.[i,j] <- x
            
            // Creation
            let ofList    xss      = MS.listM  xss
            let ofSeq     xss      = MS.seqM  xss
            let init  m n f       = MS.initM  m n f
            let ofArray2D (arr: 'T[,])  : Matrix<'T>       = MS.arrayM arr
            let toArray2D (m:Matrix<_>) = Array2D.init m.NumRows m.NumCols (fun i j -> get m i j)
            let initNumeric m n f = MS.initNumericM m n f
            let zero m n            = MS.zeroM m n
            let identity m          = MS.identityM m
            let create  m n x       = MS.constM m n x

            let ofScalar   x        = MS.scalarM x

            let diag v              = MS.diagM v
            let initDiagonal v      = MS.diagM v
            let constDiag   n x     = MS.constDiagM n x
          
            // Operators
            let add a b = MS.addM a b
            let sub a b = MS.subM a b
            let mul a b = MS.mulM a b
            let mulRV a b = MS.mulRVM a b
            let mulV a b = MS.mulMV a b
            let cptMul a b = MS.cptMulM a b
            let cptMax a b = MS.cptMaxM a b
            let cptMin a b = MS.cptMinM a b
            let scale a b = MS.scaleM a b
            let dot a b = MS.dotM a b
            let neg a = MS.negM a 
            let trace a = MS.traceM a
            let sum a = MS.sumM a
            let prod a = MS.prodM a
            let norm a = MS.normM a
            let transpose a = MS.transM a
            let inplaceAdd a b = MS.inplaceAddM a b
            let inplaceSub a b = MS.inplaceSubM a b

            let exists  f a = MS.existsM  f a
            let forall  f a = MS.forallM  f a
            let existsi  f a = MS.existsiM  f a
            let foralli  f a = MS.foralliM  f a
            let map  f a = MS.mapM f a
            let copy a = MS.copyM a
            let mapi  f a = MS.mapiM f a
            let getDiagN  a n = MS.getDiagnM a n
            let getDiag  a = MS.getDiagnM a 0
            let toDense a = MS.toDenseM a 

            let initDense i j a = MS.initDenseM i j a 
            let initSparse i j a = MS.initSparseM i j a 

            let fold  f z a = MS.foldM f z a
            let foldi f z a = MS.foldiM f z a
          
            let compare a b = MS.compareM LanguagePrimitives.GenericComparer a b
            let hash a      = MS.hashM LanguagePrimitives.GenericEqualityComparer a
            let getRow    a i           = MS.getRowM a i
            let getCol    a j           = MS.selColM a j
            let getCols   a i1 i2       = MS.getColsM a (i1,i1+i2-1)
            let getRows   a j1 j2       = MS.getRowsM a (j1,j1+j2-1)
            let getRegion a i1 j1 i2 j2 = MS.getRegionM a (i1,i1+i2-1) (j1,j1+j2-1)
            
            let ofRowVector x = MS.rowvecM x
            let ofVector    x = MS.vectorM x
            let toVector    x = MS.toVectorM x
            let toRowVector x = MS.toRowVectorM x
            let toScalar    x = MS.toScalarM x

            let inplace_assign f a  = MS.inplaceAssignM  f a
            let inplace_cptMul a b = MS.inplaceCptMulM a b
            let inplace_scale a b = MS.inplaceScaleM a b
            let inplace_mapi  f a = MS.inplace_mapiM f a
            let of_rowvec x           = ofRowVector x
            let of_vector x           = ofVector x
            let to_vector x           = toVector x
            let to_rowvec x           = toRowVector x
            let to_scalar x           = toScalar x
            let inplace_add a b       = inplaceAdd a b
            let inplace_sub a b       = inplaceSub a b
            let of_scalar   x         = ofScalar x
            let of_list    xss        = ofList xss
            let of_seq     xss        = ofSeq xss
            let inline of_array2D arr = ofArray2D arr
            let inline to_array2D m   = toArray2D m
            let init_diagonal v       = initDiagonal v
            let to_dense a            = toDense a
            let init_dense i j a      = initDense i j a
            let init_sparse i j a     = initSparse i j a
            let nonzero_entries a     = MS.nonZeroEntriesM a 
         
        end

        module MG = Generic
        module DS = DoubleImpl
        module GU = GenericImpl
        module MS = SpecializedGenericImpl

        // Element type OpsData
        type elem = float

        // Accessors
        let get (a:matrix) i j   = MG.get a i j
        let set (a:matrix) i j x = MG.set a i j x
        
        // Creation
        let init  m n f = DS.initDenseMatrixDS  m n f |> MS.dense 
        let ofList    xss   = DS.listDenseMatrixDS    xss |> MS.dense
        let ofSeq     xss   = DS.seqDenseMatrixDS    xss |> MS.dense
        let diag  (v:vector)   = MG.diag v 
        let initDiagonal  (v:vector)   = MG.diag v 
        let constDiag  n x : matrix  = MG.constDiag n x 
        let create  m n x  = DS.constDenseMatrixDS  m n x |> MS.dense
        let ofScalar x     = DS.scalarDenseMatrixDS x |> MS.dense

        let ofArray2D arr : matrix = MG.ofArray2D arr
        let toArray2D (m : matrix) = MG.toArray2D m

        let getDiagN  (a:matrix) n = MG.getDiagN a n
        let getDiag  (a:matrix) = MG.getDiag a

        // Operators
        let add (a:matrix) (b:matrix) = MS.addM   a b
        let sub (a:matrix) (b:matrix) = MS.subM   a b
        let mul (a:matrix) (b:matrix) = MS.mulM   a b
        let mulV (a:matrix) (b:vector) = MS.mulMV   a b
        let mulRV (a:rowvec) (b:matrix) = MS.mulRVM   a b
        let cptMul (a:matrix) (b:matrix) = MS.cptMulM   a b
        let cptMax (a:matrix) (b:matrix) = MS.cptMaxM a b
        let cptMin (a:matrix) (b:matrix) = MS.cptMinM a b
        let scale a (b:matrix) = MS.scaleM   a b
        let neg (a:matrix)  = MS.negM a
        let trace (a:matrix)  = MS.traceM a
        let transpose  (a:matrix) = MG.transpose a
        let forall f (a:matrix) = MG.forall f a
        let exists  f (a:matrix) = MG.exists f a
        let foralli f (a:matrix) = MG.foralli f a
        let existsi  f (a:matrix) = MG.existsi f a
        let map  f (a:matrix) = MG.map f a
        let copy  (a:matrix) = MG.copy a
        let mapi  f (a:matrix) : matrix = MG.mapi f a
        let fold  f z (a:matrix) = MG.fold f z a
        let foldi  f z (a:matrix) = MG.foldi f z a

        let toDense (a:matrix) = MG.toDense a 
        let initDense i j a : matrix = MG.initDense i j a 
        let initSparse i j a : matrix = MG.initSparse i j a 
        let nonzero_entries (a:matrix) = MG.nonzero_entries a 

        let zero m n  = DS.zeroDenseMatrixDS m n |> MS.dense
        let identity m  : matrix = MG.identity m 
        
        let ones m n  = create m n 1.0
        
        let getRow (a:matrix) i      = MG.getRow a i
        let getCol (a:matrix) j      = MG.getCol a j
        let getCols (a:matrix) i1 i2    = MG.getCols a i1 i2
        let getRows (a:matrix) j1 j2    = MG.getRows a j1 j2
        let getRegion (a:matrix) i1 j1 i2 j2    = MG.getRegion a i1 j1 i2 j2

        let rowRange (a:Matrix<_>) = (0,a.NumRows - 1)
        let colRange (a:Matrix<_>) = (0,a.NumCols - 1)
        let wholeRegion a = (colRange a, rowRange a)
        
        let foldByRow f (z:Vector<'T>) (a:matrix) = 
          colRange a |> GU.foldR (fun z j -> MS.mapiV (fun i z -> f z (get a i j)) z) z
        let foldByCol f (z:RowVector<'T>) (a:matrix) = 
          rowRange a |> GU.foldR (fun z i -> MS.mapiRV (fun j z -> f z (get a i j)) z) z

        let foldRow f (z:'T) (a:matrix) i = 
          colRange a |> GU.foldR (fun (z:'T) j -> f z (get a i j)) z
        let foldCol f (z:'T) (a:matrix) j = 
          rowRange a |> GU.foldR (fun (z:'T) i -> f z (get a i j)) z

        let sum (a:matrix)  = MS.sumM a
        let prod (a:matrix)  = MS.prodM a
        let norm  (a:matrix) = MS.normM  a
        let dot (a:matrix) b = MS.dotM a b

        let cptPow  a y = map (fun x -> x ** y) a
        
        // Functions that only make sense on this type
        let randomize v = map (fun vij -> MRandom.float vij) v      (* res_ij = random [0,vij] values *)

        let ofRowVector x : matrix = MS.rowvecM x
        let ofVector    x : matrix = MS.vectorM x
        let toVector    x : vector = MS.toVectorM x
        let toRowVector x : rowvec = MS.toRowVectorM x
        let toScalar    x : float  = MS.toScalarM x

        let inplaceAdd  (a:matrix) b = MS.inplaceAddM a b
        let inplaceSub  (a:matrix) b = MS.inplaceSubM a b

        // Mutation
        let inplace_assign  f (a:matrix) = MG.inplace_assign f a
        let inplace_mapi  f (a:matrix) = MG.inplace_mapi f a
        let inplace_cptMul (a:matrix) b = MS.inplaceCptMulM a b
        let inplace_scale  a (b:matrix) = MS.inplaceScaleM a b

        let inplace_add  a b = inplaceAdd a b
        let inplace_sub  a b = inplaceSub a b
        let of_rowvec x = ofRowVector x
        let of_vector x = ofVector x
        let to_vector x = toVector x
        let to_rowvec x = toRowVector x
        let to_scalar x = toScalar x
        let inline of_array2D arr  = ofArray2D arr
        let inline to_array2D m = toArray2D m
        let of_list    xss   = ofList xss
        let of_seq     xss   = ofSeq xss
        let init_diagonal v   = initDiagonal   v
        let of_scalar x     = ofScalar x
        let to_dense x = toDense x
        let init_dense i j a = initDense i j a
        let init_sparse i j a = initSparse i j a


    end


//----------------------------------------------------------------------------
// module Vector
//--------------------------------------------------------------------------*)
      
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module Vector = 

        module Generic = 

            module OpsS = SpecializedGenericImpl

            let get (a:Vector<_>) i   = a.[i]
            let set (a:Vector<_>) i x = a.[i] <- x
            let length (v:Vector<_>) = v.Length
            let ofList    xss   = OpsS.listV xss
            let ofSeq    xss   = OpsS.seqV xss
            let init  m   f = OpsS.initV m f
            let initNumeric  m   f = OpsS.createNumericV m f
            let ofArray arr       = OpsS.arrayV arr
            let toArray (v:Vector<_>) = Array.init v.Length (get v)

            let create  m x   = OpsS.constV m x
            let zero n = OpsS.zeroV n
            let ones n = OpsS.createNumericV n (fun ops _ -> ops.One)
            let ofScalar   x = OpsS.scalarV x
            let add a b = OpsS.addV a b
            let sub a b = OpsS.subV a b
            let mulRVV a b = OpsS.mulRVV a b
            let mulVRV a b = OpsS.mulVRV a b
            let cptMul a b = OpsS.cptMulV a b
            let cptMax a b = OpsS.cptMaxV a b
            let cptMin a b = OpsS.cptMinV a b
            let scale a b = OpsS.scaleV a b
            let dot a b = OpsS.dotV a b
            let neg a = OpsS.negV a 
            let transpose a = OpsS.transV a 
            let inplaceAdd a b = OpsS.inplaceAddV a b
            let inplaceSub a b = OpsS.inplaceSubV a b
            let inplace_cptMul a b = OpsS.inplaceCptMulV a b
            let inplace_scale a b = OpsS.inplaceScaleV a b



            let exists  f a = OpsS.existsV  f a
            let forall  f a = OpsS.forallV  f a
            let existsi  f a = OpsS.existsiV  f a
            let foralli  f a = OpsS.foralliV  f a
            let map  f a = OpsS.mapV f a
            let mapi f a = OpsS.mapiV f a
            let copy a = OpsS.copyV a
            let inplace_mapi  f a = OpsS.inplace_mapiV f a
            let fold  f z a = OpsS.foldV f z a
            let foldi  f z a = OpsS.foldiV f z a
            let compare a b = OpsS.compareV a b
            let hash a = OpsS.hashV a
            let inplace_assign  f a = OpsS.assignV f a
            let sum  (a:Vector<_>) = let ops = a.ElementOps in fold (fun x y -> ops.Add(x,y)) ops.Zero a
            let prod (a:Vector<_>) = let ops = a.ElementOps in fold (fun x y -> ops.Multiply(x,y)) ops.One a
            let norm (a:Vector<_>) = 
                let normOps = GenericImpl.getNormOps a.ElementOps 
                sqrt (fold (fun x y -> x + normOps.Norm(y)**2.0) 0.0 a)

            let of_list    xss  = ofList xss
            let of_seq    xss   = ofSeq xss
            let of_array arr    = ofArray arr
            let to_array v      = toArray v
            let of_scalar   x   = ofScalar x
            let inplace_add a b = inplaceAdd a b
            let inplace_sub a b = inplaceSub a b

        module VG = Generic
        module VecDS = DoubleImpl
        module VecGU = GenericImpl

        let get (a:vector) j   = VG.get a j 
        let set (a:vector) j x = VG.set a j x
        let length (a:vector)     = VG.length a
        let nrows (a:vector)   = VG.length a
        let init  m   f = VecDS.createVecDS  m   f
        let ofArray arr : vector = VG.ofArray arr
        let toArray (m : vector) = VG.toArray m

        type range = int * int
        let countR ((a,b) : range)   = (b-a)+1
        let idxR    ((a,_) : range) i = a+i
        type rangef = float * float * float // start, skip, end
        let countRF ((a,d,b) : rangef)   = System.Convert.ToInt32((b-a)/d) + 1
        //let countRF ((a,d,b) : rangef)   = Float.to_int((b-a)/d) + 1
        let idxRF  ((a,d,b) : rangef) i = System.Math.Min (a + d * float(i),b)

        let range n1 n2    = let r = (n1,n2)   in init (countR  r) (fun i -> float(idxR r i)) 

        let rangef a b c  = let r = (a,b,c) in init (countRF r) (fun i -> idxRF r i)

        let ofList    xs    = VecDS.listVecDS    xs
        let ofSeq    xs    = VecDS.seqVecDS    xs
        let create  m   x  = VecDS.constVecDS  m   x
        let ofScalar x     = VecDS.scalarVecDS x
        let add a b = VecDS.addVecDS   a b
        let sub a b = VecDS.subVecDS   a b
        let mulRVV a b = VecDS.mulRowVecVecDS   a b
        let mulVRV a b = VecDS.mulVecRowVecDS   a b 
        let cptMul a b = VecDS.cptMulVecDS   a b
        let cptMax a b = VecDS.cptMaxVecDS a b
        let cptMin a b = VecDS.cptMinVecDS a b
        let scale a b = VecDS.scaleVecDS   a b
        let neg a  = VecDS.negVecDS a
        let dot a b = VecDS.dotVecDS a b
        let transpose  (a:vector) = VG.transpose a
        let exists  f (a:vector) = VG.exists f a
        let forall  f (a:vector) = VG.forall f a
        let existsi  f (a:vector) = VG.existsi f a
        let foralli  f (a:vector) = VG.foralli f a
        let map  f (a:vector) = VG.map f a
        let copy (a:vector) = VG.copy a
        let mapi  f (a:vector) : vector = VG.mapi f a
        let fold  f z (a:vector) = VG.fold f z a
        let foldi  f z (a:vector) = VG.foldi f z a
        let zero n = create n 0.0
        let ones n = create n 1.0
        let sum a  = VecDS.sumVecDS a
        let prod a   = fold      (fun x y -> x * y) 1.0 a
        let norm  (a:vector) = sqrt (fold (fun x y -> x + y * y) 0.0 a) (* fixed *)
        let cptPow  a y = map  (fun x -> x ** y) a
        let inplace_assign  f (a:vector) = VG.inplace_assign f a
        let inplace_mapi f (a:vector) = VG.inplace_mapi f a
        let inplace_add a b = VecDS.inplaceAddVecDS a b
        let inplace_sub a b = VecDS.inplaceSubVecDS a b
        let inplace_cptMul a b = VecDS.inplaceCptMulVecDS a b
        let inplace_scale a b = VecDS.inplaceScaleVecDS a b  

        let of_array arr   = ofArray arr
        let to_array m     = toArray m
        let of_list    xs  = ofList xs
        let of_seq    xs   = ofSeq xs
        let of_scalar x    = ofScalar x



//----------------------------------------------------------------------------
// module RowVector
//--------------------------------------------------------------------------*)

    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module RowVector = 

        module Generic = 

            module OpsS = SpecializedGenericImpl

            let get (a:RowVector<_>) i          = a.[i]
            let set (a:RowVector<_>) i x        = a.[i] <- x
            let zero n           = OpsS.zeroRV n
            let length (v:RowVector<_>) = v.Length
            let init m   f       = OpsS.initRV m   f
            let create  m x      = OpsS.constRV m x
            let transpose a      = OpsS.transRV a
            let copy a           = OpsS.copyRV a
            let ofList a         = OpsS.listRV a
            let ofArray a        = OpsS.arrayRV a
            let ofSeq a          = OpsS.seqRV a
            let toArray m        = Array.init (length m) (get m)

            let of_list a        = ofList a
            let of_array a       = ofArray a
            let of_seq a         = ofSeq a
            let to_array m       = toArray m


        module RVG = Generic

        let get (a:rowvec) i   = RVG.get a i 
        let set (a:rowvec) i x = RVG.set a i x
        let length (a:rowvec)  = RVG.length a
        let ncols (a:rowvec)   = RVG.length a
        let ofArray arr : rowvec = RVG.ofArray arr
        let toArray (m : rowvec) = RVG.toArray m
        
        let init m   f : rowvec      = RVG.init m   f
        let create m   f : rowvec    = RVG.create m   f
        let zero n = create n 0.0
        let ofList x : rowvec       = RVG.ofList x
        let ofSeq x : rowvec       = RVG.ofSeq x
        let transpose x : vector     = RVG.transpose x
        let copy x : rowvec          = RVG.copy x

        let of_list x    = ofList x
        let of_seq x     = ofSeq x
        let of_array arr = ofArray arr
        let to_array m   = toArray m


    type Matrix<'T> with 
        member x.ToArray2()        = Matrix.Generic.toArray2D x
        member x.ToArray2D()        = Matrix.Generic.toArray2D x

#if FX_NO_DEBUG_DISPLAYS
#else
        [<DebuggerBrowsable(DebuggerBrowsableState.Collapsed)>]
#endif

        member x.NonZeroEntries    = Matrix.Generic.nonzero_entries x
        member x.ToScalar()        = Matrix.Generic.toScalar x
        member x.ToRowVector()     = Matrix.Generic.toRowVector x               
        member x.ToVector()        = Matrix.Generic.toVector x

#if FX_NO_DEBUG_DISPLAYS
#else
        [<DebuggerBrowsable(DebuggerBrowsableState.Collapsed)>]
#endif
        member x.Norm              = Matrix.Generic.norm x

        member x.Column(n)         = Matrix.Generic.getCol x n
        member x.Row(n)            = Matrix.Generic.getRow x n
        member x.Columns (i,ni)    = Matrix.Generic.getCols x i ni
        member x.Rows (j,nj)       = Matrix.Generic.getRows x j nj
        member x.Region(i,j,ni,nj) = Matrix.Generic.getRegion x i j ni nj
        member x.GetDiagonal(i)    = Matrix.Generic.getDiagN x i

#if FX_NO_DEBUG_DISPLAYS
#else
        [<DebuggerBrowsable(DebuggerBrowsableState.Collapsed)>]
#endif
        member x.Diagonal          = Matrix.Generic.getDiag x

        member x.Copy () = Matrix.Generic.copy x


    type Vector<'T> with 
        member x.ToArray() = Vector.Generic.toArray x
        member x.Norm      = Vector.Generic.norm x
        member x.Copy ()   = Vector.Generic.copy x


    type RowVector<'T> with 
        member x.ToArray() = RowVector.Generic.toArray x
        member x.Copy ()   = RowVector.Generic.copy x


namespace Microsoft.FSharp.Core

    /// The type of floating-point matrices. See Microsoft.FSharp.Math
    type matrix = Microsoft.FSharp.Math.matrix
    /// The type of floating-point vectors. See Microsoft.FSharp.Math
    type vector = Microsoft.FSharp.Math.vector
    /// The type of floating-point row vectors. See Microsoft.FSharp.Math
    type rowvec = Microsoft.FSharp.Math.rowvec

    [<AutoOpen>]
    module MatrixTopLevelOperators = 

        let matrix ll = Microsoft.FSharp.Math.Matrix.ofSeq ll
        let vector l  = Microsoft.FSharp.Math.Vector.ofSeq  l
        let rowvec l  = Microsoft.FSharp.Math.RowVector.ofSeq l

