namespace Microsoft.FSharp.Math.Experimental

open Microsoft.FSharp.Math

module LinearAlgebra = 
  // This module is the dispatcher to BLAS/LAPACK service implementation vs managed implementation
  // It is the user facing module.
  open Microsoft.FSharp.Math.Bindings.Internals
  open Microsoft.FSharp.Math.Experimental
  
  type Permutation = Microsoft.FSharp.Compatibility.permutation

  let Lapack         = LinearAlgebraService.LAPACKService // The service/provider object
  let MKLProvider    = LapackMKL.MKLProvider              // MKL provider
  let NetlibProvider = LapackNetlib.NetlibProvider        // Netlib provider

  module Locals = 
    let HaveService() = Lapack.Available()
  open Locals

  let SolveTriangularLinearSystem (A:matrix) (b:vector) (isLower:bool) =
    if HaveService() then LinearAlgebraService.solveTriangularForVector A b isLower
                     else LinearAlgebraManaged.SolveTriangularLinearSystem A b isLower
                   
  let SolveTriangularLinearSystems (A:matrix) (B:matrix) (isLower:bool) =
    if HaveService() then LinearAlgebraService.solveTriangularForMatrix A B isLower
                     else LinearAlgebraManaged.SolveTriangularLinearSystems A B isLower
  
  let SolveLinearSystem (A:matrix) (b:vector) =
    if HaveService() then LinearAlgebraService.preDivideByVector A b
                     else LinearAlgebraManaged.SolveLinearSystem A b
                   
  let SolveLinearSystems (A:matrix) (B:matrix) =
    if HaveService() then LinearAlgebraService.preDivideByMatrix A B
                     else LinearAlgebraManaged.SolveLinearSystems A B

  /// Given A[n,m] and B[n] solve for x[m] such that Ax = B
  /// This call may fail.
  let preDivideByVector A b = 
    if HaveService() then LinearAlgebraService.preDivideByVector A b
                     else LinearAlgebraManaged.SolveLinearSystem A b
    

  /// Given A[n,m] and B[n,k] solve for X[m,k] such that AX = B
  /// This call may fail.
  let preDivideByMatrix a b = 
    if HaveService() then LinearAlgebraService.preDivideByMatrix a b
                     else LinearAlgebraManaged.SolveLinearSystems a b
    
  /// Compute eigenvalue/eigenvector decomposition of a square real matrix.
  /// Returns two arrays containing the eigenvalues and eigenvectors, which may be complex.
  /// This call may fail.
  let EigenSpectrum m = 
    if HaveService() then let evals, evecs = LinearAlgebraService.eigenvectors m
                          let n = evals.Length
                          Vector.Generic.init n (fun i -> evals.[i]), Matrix.Generic.init n n (fun i j -> (evecs.[i]).[j])
                     else LinearAlgebraManaged.eigenvectors m
                           
  /// Compute eigenvalues of a square real matrix.
  /// Returns arrays containing the eigenvalues which may be complex.
  /// This call may fail.
  let EigenValues m =
    if HaveService() then let evals = LinearAlgebraService.eigenvalues m
                          let n = evals.Length
                          Vector.Generic.init n (fun i -> evals.[i])
                     else LinearAlgebraManaged.eigenvalues m

  /// Compute eigenvalues for a real symmetric matrix.
  /// Returns array of real eigenvalues.
  /// This call may fail.
  let EigenValuesWhenSymmetric a =
    if HaveService() then let evals = LinearAlgebraService.symmetricEigenvalues a
                          let n = evals.Length
                          Vector.init n (fun i -> evals.[i])
                     else LinearAlgebraManaged.symmetricEigenvalues a
    
  /// Compute eigenvalues and eigenvectors for a real symmetric matrix.
  /// Returns arrays of the values and vectors (both based on reals).
  /// This call may fail.
  let EigenSpectrumWhenSymmetric a =
    if HaveService() then let evals, evecs = LinearAlgebraService.symmetricEigenvectors a
                          let n = evals.Length
                          Vector.init n (fun i -> evals.[i]), Matrix.init n n (fun i j -> (evecs.[i]).[j])
                     else LinearAlgebraManaged.symmetricEigenvectors a
    
  /// Given A[n,n] find it's inverse.
  /// This call may fail.
  let Inverse a = 
    if HaveService() then LinearAlgebraService.inverse a
                     else LinearAlgebraManaged.Inverse a

  /// Given A[m,n] and B[m] solves AX = B for X[n].
  /// When m=>n, have over constrained system, finds least squares solution for X.
  /// When m<n, have under constrained system, finds least norm solution for X.
  let LeastSquares a b =
    if HaveService() then LinearAlgebraService.leastSquares a b
                     else LinearAlgebraManaged.leastSquares a b
    
  /// Given A[n,n] real symmetric positive definite.
  /// Finds the cholesky decomposition L such that L' * L = A.
  /// May fail if not positive definite.
  let Cholesky a = 
    if HaveService() then LinearAlgebraService.Cholesky a
                     else LinearAlgebraManaged.Cholesky a
      
  /// Given A[n,n] real matrix.
  /// Finds P,L,U such that L*U = P*A with L,U lower/upper triangular.
  let LU a = 
    if HaveService() then LinearAlgebraService.LU a
                     else LinearAlgebraManaged.LU a
      

  /// Given A[m,n] finds Q[m,m] and R[k,n] where k = min m n.
  /// Have A = Q.R  when m<=n.
  /// Have A = Q.RX when m>n and RX[m,n] is R[n,n] row extended with (m-n) zero rows.
  let QR a = 
    if HaveService() then LinearAlgebraService.QR a
                     else LinearAlgebraManaged.QR a
  
  let SVD a = 
    if HaveService() then let U,D,V = LinearAlgebraService.SVD a
                          U,Vector.ofArray D,V
                     else LinearAlgebraManaged.SVD a

  let Hessenberg A =
    if HaveService() then failwith "Not implemented yet."// REVIEW LinearAlgebraService.Hessenberg A
                     else LinearAlgebraManaged.Hessenberg A

  /// This method computes the condition number by just dividing the largest singular value
  /// by the smallest.
  let Condition (A:matrix) =
    let _,D,_ = SVD A
    D.[0] / D.[D.Length-1]

  /// Compute the determinant of a matrix by performing an LU decomposition since if A = P'LU,
  /// then det(A) = det(P') * det(L) * det(U).
  let Determinant A =
    let P,_,U = LU A
    // Compute the sign of a permutation REVIEW maybe this should go into permutation?
    let PermutationSign (len,p) =
        let a = Array.init len (fun i -> p i)                        // Get an array representation of the permutation
        let v = Array.init len                                         // Find the positions of all the positions in the permutation
                            (fun i -> Array.findIndex (fun x -> x = i) a)
        let mutable sign = 1.0                                              // Remember the sign
        for i=0 to len-2 do                                            // Start transposing elements keeping track of how many
            if a.[i] <> i then                                              // transpositions we have taken.
                a.[v.[i]] <- a.[i]
                a.[i] <- i
                v.[a.[v.[i]]] <- v.[i]
                v.[i] <- i
                sign <- -sign
        assert(a = [|0 .. len-1|])
        assert(v = [|0 .. len-1|])
        sign
    let n = A.NumRows
    let P = (fun i -> P i)
    (PermutationSign (n,P)) * (Vector.prod U.Diagonal)
        


(*TESTING
#load "lapack_base.fs";;
#load "lapack.fs";;
#load "lapack_service_mkl.fs";;
//#r "fsharp.math.lapack.dll";;
open Microsoft.FSharp.Math.Experimental.LinearAlgebra
open Microsoft.FSharp.Math;;

Microsoft.FSharp.Math.Bindings.Internals.LapackMKL.register();;
verbose := true;;
let A = (Matrix.of_list [[10.0; 1.0];[1.0;10.0]]);;
let B = symmetricPositiveDefiniteCholeskyLU A
let A = (Matrix.of_list [[10.0;  1.0; 0.0];
                         [ 1.0; 10.0; 0.0];
                         [ 1.0;  0.0; 2.0]])
let r,vs,tau = QR A
let h v tau = Matrix.iden

#q;;

let A = (Matrix.of_list [[10.0; 12.0; 0.3];
                         [ 1.0; 13.0; 0.2];
                         [99.0;-12.0; 2.0]])
let m,n = matrixDims A
// QR check
let q,r = QR A
q * r - A
let [|h1;h2;h3|] = hs
let Q = h1 * (h2 * h3)
Q * r - A
let h (v:vector) tau = Matrix.identity 3 - (tau:float) $* (v * v.Transpose)
let hs = Array.map2 h vs tau

// SVD check
let vs,U,W = SVD A
let VS = Matrix.init 3 3 (fun r c -> if r=c then vs.[r] else 0.0)
U * VS * W - A

*)
