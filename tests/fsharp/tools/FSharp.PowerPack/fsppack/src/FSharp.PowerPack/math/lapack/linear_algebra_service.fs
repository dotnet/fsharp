namespace Microsoft.FSharp.Math.Experimental

/// This module is for internal use only.
module LinearAlgebraService = 

  /// Attribution and acknowledgement:
  ///
  /// These modules provide linear algebra functionality on F# matrix types.
  /// The functionality is implemented using a provider model.
  /// The providers dynamic link to BLAS and Lapack DLLs.
  ///
  /// There are currently providers for:
  /// a) blas.dll and lapack.dll which can be built from netlib.org sources.
  /// b) the High Performance Intel(R) Math Kernel Library (MKL) runtime DLLs.
  ///
  /// For information on the Intel(R) MKL High Performance library, please see:
  /// http://www.intel.com/cd/software/products/asmo-na/eng/307757.htm
  ///

  open Microsoft.FSharp.Math
  open Microsoft.FSharp.Math.Bindings
  open Microsoft.FSharp.Math.Bindings.Internals
  open Microsoft.FSharp.Math.Bindings.Internals.NativeUtilities
  open Microsoft.FSharp.Collections
  open System.IO
  
  open Microsoft.FSharp.Math.Experimental

  let MKLProvider    = LapackMKL.MKLProvider
  let NetlibProvider = LapackNetlib.NetlibProvider    
  let LAPACKService = new Service<ILapack>([MKLProvider;NetlibProvider])

  let Service() = 
    match LAPACKService.Service() with
    | Some svc -> svc
    | None     -> failwith "LAPACK service either not available, or not started"
  
  /// Compute eigenvalue/eigenvector decomposition of a square real matrix.
  /// Returns two arrays containing the eigenvalues and eigenvectors, which may be complex.
  /// This call may fail.
  let eigenvectors m = 
    let complex r = Complex.mkRect(r,0.0)
    let makeEigenVectors (wr:float[],wi:float[],vr:float[,]) =
      let n = wr.Length
      let eigenValues  = Array.init n (fun i -> Complex.mkRect(wr.[i],wi.[i]))
      let cvr = Array.init n (fun i -> Vector.Generic.init n (fun j -> complex vr.[i,j]))
      let eigenVectors = Array.zeroCreate n : Vector<Complex> array // zeros....
      let mutable i = 0
      while (i < n) do      
        if wi.[i] = 0.0 then
          eigenVectors.[i] <-  cvr.[i]
          i <- i + 1
        else
          assert(wi.[i] = -wi.[i+1]) // better error message?
          eigenVectors.[i]   <- cvr.[i] + Complex.OneI * cvr.[i+1]
          eigenVectors.[i+1] <- cvr.[i] - Complex.OneI * cvr.[i+1]
          i <- i + 2
      done
      eigenValues,eigenVectors
    Service().dgeev_ ('V',m) |> makeEigenVectors
      
  /// Compute eigenvalues of a square real matrix.
  /// Returns arrays containing the eigenvalues which may be complex.
  /// This call may fail.
  let eigenvalues m = 
    let complex r = Complex.mkRect(r,0.0)
    let makeEigenValues (wr:float[],wi:float[],_) =
      let n = wr.Length
      let eigenValues  = Array.init n (fun i -> Complex.mkRect(wr.[i],wi.[i]))
      eigenValues
    Service().dgeev_ ('N',m) |> makeEigenValues

  /// Compute eigenvalues for a real symmetric matrix.
  /// Returns array of real eigenvalues.
  /// This call may fail.
  let symmetricEigenvalues a =
    let a,lambdas = Service().dsyev_ ('N','U',a)
    lambdas

  /// Compute eigenvalues and eigenvectors for a real symmetric matrix.
  /// Returns arrays of the values and vectors (both based on reals).
  /// This call may fail.
  let symmetricEigenvectors a =
    let n,m = matrixDims a
    let a,lambdas = Service().dsyev_ ('V','U',a)
    let vs = Array.init n (fun j -> Vector.init n (fun i -> a.[i,j]))
    lambdas,vs

  /// Given A[n,m] and B[n] solve for x[m] such that Ax = B
  /// This call may fail.
  let preDivideByVector A b = 
    let _,_,x  = Service().dgesv_(A,Matrix.ofVector b)
    Matrix.toVector x

  /// Given A[n,m] and B[n,k] solve for X[m,k] such that AX = B
  /// This call may fail.
  let preDivideByMatrix a b = 
    let _,_,x  = Service().dgesv_(a,b)
    x

  let solveTriangularForVector a b isLower =
    // NOTE: L and U transposed. Check. Is this the right away around?
    Service().dtrsv_((if isLower then 'u' else 'l'),a,b) (* CHECK: parser error without brackets? *)

  let solveTriangularForMatrix a b isLower = 
    // NOTE: L and U transposed. Check. Is this the right away around?
    Service().dtrsm_((if isLower then 'u' else 'l'),a,b) (* CHECK: parser error without brackets? *)

  /// Given A[n,n] find it's inverse.
  /// This call may fail.
  let inverse a = 
    let n,m = matrixDims a
    NativeUtilities.assertDimensions "inverse" ("rows","columns") (n,m)
    let _,_,x  = Service().dgesv_(a,Matrix.identity n)
    x

  /// Given A[m,n] and B[m] solves AX = B for X[n].
  /// When m=>n, have over constrained system, finds least squares solution for X.
  /// When m<n, have under constrained system, finds least norm solution for X.
  let leastSquares a b =
    let m,n = matrixDims a
    let b = if m>n then Matrix.ofVector b else Matrix.init n 1 (fun i j -> if i<m then b.[i] else 0.0)
    let a,b = Service().dgels_(a,b)
    Vector.init n (fun i -> b.[i,0])

  /// Given A[n,n] real symmetric positive definite.
  /// Finds the cholesky decomposition L such that L' * L = A.
  /// May fail if not positive definite.
  let Cholesky a = 
    let n,_ = matrixDims a  
    let b = Service().dpotrf_('U',a)
    let b = Matrix.init n n (fun i j -> if j>=i then b.[i,j] else 0.0)
    b
  
  /// Given A[n,n] real matrix.
  /// Finds P,L,U such that L*U = P*A with L,U lower/upper triangular.
  let LU a = 
    // This method translates a BLAS/LAPACK permutation into an FSharp.Collections.Permutation.
    // The BLAS/LAPACK permutation (P: int array) encodes that in step i, row i and P.[i] were
    // swapped. 
    let TranslatePermutation P =
        let n = Array.length P
        let x = [|0 .. n-1|]
        for l=0 to n-1 do
            let t = x.[l]
            x.[l] <- x.[P.[l]]
            x.[P.[l]] <- t
        ((*n,*)Permutation.ofArray(x))
    let n,m = matrixDims a  
    let lu,p = Service().dgetrf_(a)
    let l = Matrix.init n m (fun i j -> if j<i then lu.[i,j] elif j = i then 1.0 else 0.0)
    let u = Matrix.init n m (fun i j -> if j>=i then lu.[i,j] else 0.0)
    p |> Array.map (fun i -> i-1) |> TranslatePermutation,l,u

  /// Given A[m,n] finds Q[m,m] and R[k,n] where k = min m n.
  /// Have A = Q.R  when m<=n.
  /// Have A = Q.RX when m>n and RX[m,n] is R[n,n] row extended with (m-n) zero rows.
  let QR a = 
    let m,n = matrixDims a
    let k = min m n 
    let res,tau = Service().dgeqrf_(a)
    let R  = Matrix.init k n (fun r c -> if r <= c then res.[r,c] else 0.0)
    let ks = [| 0 .. (k-1) |]
    let vs = ks |> Array.map (fun i -> Vector.init m (fun j -> if j < i then 0.0 else
                                                               if j = i then 1.0 else
                                                               res.[j,i]))
    let h (v:vector) tau = Matrix.identity m - (tau:float) * (v * v.Transpose)
    let hs = Array.map2 h vs tau
    let Q  = Array.reduceBack ( * ) hs
    Q,R (*,tau,vs,hs*)

  let SVD a = 
    let vs,u,w = Service().dgesvd_ a
    u,vs,w
