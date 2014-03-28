namespace Microsoft.FSharp.Math.Bindings.Internals
#nowarn "51"

open System
open System.Runtime.InteropServices
open Microsoft.FSharp.NativeInterop
open Microsoft.FSharp.Math

///This is an internal interface and not for user usage.
///It exposes a specialised subset of BLAS/LAPACK functionality.
///This functionality is used by us to build the exposed APIs.
///It is those exposed APIs that should be used.
type ILapack = interface
    //Matrix-Matrix Multiplication
    abstract dgemm_ : Math.matrix * Math.matrix -> Math.matrix

    //Matrix-Vector Multiplication
    abstract dgemv_ : Math.matrix * Math.vector -> Math.vector

    //Solve (linear equations)
    abstract dgesv_ : Math.matrix * Math.matrix -> Math.matrix * int array * Math.matrix

    //Solve symmetric positive definite matrix (linear equations)
    abstract dposv_ : Math.matrix * Math.matrix -> Math.matrix * Math.matrix

    //Solve triangular (linear equations)
    abstract dtrsv_ : char * Math.matrix * Math.vector -> Math.vector

    //Solve triangular (linear equations)
    abstract dtrsm_ : char * Math.matrix * Math.matrix -> Math.matrix

    //Solve (linear equations) using LU factorization
    abstract dgesvx_ :
      Math.matrix * Math.matrix ->
      Math.matrix * Math.matrix * int array * char * double array * double array *
      Math.matrix * Math.matrix * float * double array * double array

    //Eigen Value Non-Symmetric
    abstract dgeev_ : char * Math.matrix -> double array * double array * double [,]    

    //Eigen Value of Symmetric Matrix
    abstract dsyev_ : char * char * Math.matrix -> Math.matrix * double array

    //Eigen Value of Symmetric Matrix - Divide and Conquer
    abstract dsyevd_ : char * char * Math.matrix -> Math.matrix * double array

    //Eigen Value for a pair of general matrices
    abstract dggev_ :
      Math.matrix * Math.matrix ->
      Math.matrix * Math.matrix * double array * double array * double array *
      double [,]

    //Solve least-squares/min-norm.
    //Note the dimension requirements on second input to match second output.
    abstract dgels_ : Math.matrix * Math.matrix -> Math.matrix * Math.matrix

    //Solve least-squares/min-norm (with linear equality constraint)
    abstract dgglse_ :
      Math.matrix * Math.matrix * Math.vector * Math.vector ->
      Math.matrix * Math.vector * double array

    //Singular Value Decomposition
    abstract dgesvd_ :
      Math.matrix -> double array * Math.matrix * Math.matrix

    //Singular Value Decomposition Divide- Conquer
    abstract dgesdd_ : Math.matrix -> double array * Math.matrix * Math.matrix

    //Single Value Decomposition for Symmetric Matrices
    abstract dsygv_ :
      Math.matrix * Math.matrix -> Math.matrix * Math.matrix * double array

    //Single Value Decomposition for Symetric Matrices Divide and Conquer
    abstract dsygvd_ :
      Math.matrix * Math.matrix -> Math.matrix * Math.matrix * double array

    //Cholesky Factorisation
    abstract dpotrf_ : char * Math.matrix -> Math.matrix
    
    abstract dgetrf_ : matrix -> matrix * int[]

    //Cholesky Factorisation - Expert
    abstract dposvx_ :
      Math.matrix * Math.matrix ->
      Math.matrix * Math.matrix * char * double array * Math.matrix * Math.matrix *
      float * double array * double array

    //QR Factorisation
    abstract dgeqrf_ : Math.matrix -> Math.matrix * double array

end


module NativeUtilities = begin
    let nativeArray_as_CMatrix_colvec (arr: 'T NativeArray) = new CMatrix<_>(arr.Ptr,arr.Length,1)
    let nativeArray_as_FortranMatrix_colvec (arr: 'T NativeArray) = new FortranMatrix<_>(arr.Ptr,arr.Length,1)
    let pinM m = PinnedArray2.of_matrix(m)
    let pinV v = PinnedArray.of_vector(v)
    let pinA arr = PinnedArray.of_array(arr)
    
    let pinA2 arr = PinnedArray2.of_array2D(arr)
    
    let pinMV m1 v2 = pinM m1,pinV v2
    let pinVV v1 v2 = pinV v1,pinV v2
    let pinAA v1 v2 = pinA v1,pinA v2
    let pinMVV m1 v2 m3 = pinM m1,pinV v2,pinV m3
    let pinMM m1 m2  = pinM m1,pinM m2
    let pinMMM m1 m2 m3 = pinM m1,pinM m2,pinM m3
    let freeM (pA: PinnedArray2<'T>) = pA.Free()
    let freeV (pA: PinnedArray<'T>) = pA.Free()
    let freeA (pA: PinnedArray<'T>) = pA.Free()
    
    let freeA2 a = freeM a
    
    let freeMV (pA: PinnedArray2<'T>,pB : PinnedArray<'T>) = pA.Free(); pB.Free()
    let freeVV (pA: PinnedArray<'T>,pB : PinnedArray<'T>) = pA.Free(); pB.Free()
    let freeAA (pA: PinnedArray<'T>,pB : PinnedArray<'T>) = pA.Free(); pB.Free()
    let freeMM (pA: PinnedArray2<'T>,(pB: PinnedArray2<'T>)) = pA.Free();pB.Free()
    let freeMMM (pA: PinnedArray2<'T>,(pB: PinnedArray2<'T>),(pC: PinnedArray2<'T>)) = pA.Free();pB.Free();pC.Free()
    let freeMVV (pA: PinnedArray2<'T>,(pB: PinnedArray<'T>),(pC: PinnedArray<'T>)) = pA.Free();pB.Free();pC.Free()
    
    let matrixDims (m:Matrix<_>) = m.NumRows, m.NumCols
    let matrixDim1 (m:Matrix<_>) = m.NumRows
    let matrixDim2 (m:Matrix<_>) = m.NumCols
    let vectorDim  (v:Vector<_>) = v.Length
    
    let assertDimensions functionName (aName,bName) (a,b) =
      if a=b then () else
      failwith (sprintf "Require %s = %s, but %s = %d and %s = %d in %s" aName bName aName a bName b functionName)
end
