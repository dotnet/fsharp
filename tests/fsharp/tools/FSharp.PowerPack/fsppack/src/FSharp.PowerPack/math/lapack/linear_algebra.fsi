namespace Microsoft.FSharp.Math.Experimental

open Microsoft.FSharp.Math
open Microsoft.FSharp.Math.Experimental
open Microsoft.FSharp.Math.Bindings.Internals
open Microsoft.FSharp.Collections

/// Attribution and acknowledgement:
///
/// These modules provide linear algebra functionality on F# matrix types.
/// Some functionality is implemented using a provider model.
/// The providers dynamic link to BLAS and Lapack DLLs.
///
/// There are currently providers for:
/// a) blas.dll and lapack.dll which can be built from netlib.org sources.
/// b) the High Performance Intel(R) Math Kernel Library (MKL) runtime DLLs.
///
/// For information on the Intel(R) MKL High Performance library, please see:
/// http://www.intel.com/cd/software/products/asmo-na/eng/307757.htm
  
module LinearAlgebra =
    type Permutation = Microsoft.FSharp.Compatibility.permutation
    
    /// The LAPACK and BLAS service.
    /// The default providers are already registered.
    /// The service is primed to auto start on the first service request.
    val Lapack         : Service<ILapack>

    /// The MKL provider for the BLAS/LAPACK service
    val MKLProvider    : Provider<ILapack>

    /// The Netlib provider for the BLAS/LAPACK service
    val NetlibProvider : Provider<ILapack>
    
    /// Computes the determinant of a matrix. Uses an LU factorization to compute the determinant.
    val Determinant : matrix -> float
    /// Estimates the condition number of matrix A in 2-norm using an SVD decomposition of A.
    val Condition : A:matrix -> float
    
    /// Solves a system of linear equations for vector x when A is triangular: Ax=b. The isLower
    /// flag specifies whether the input argument A is lower triangular. Uses BLAS *trsv if possible.
    val SolveTriangularLinearSystem : A:matrix -> b:vector -> isLower:bool -> vector
    /// Solves a system of linear equations for every column vector of B when A is triangular: AX=B. The isLower
    /// flag specifies whether the input argument A is lower triangular. Uses BLAS *trsm if possible.
    val SolveTriangularLinearSystems : A:matrix -> B:matrix -> isLower:bool -> matrix
    
    /// Solves the linear system Ax=b when given A and b with no particular structure for A assumed. The method may
    /// fail when A is singular.
    val SolveLinearSystem : A:matrix -> b:vector -> vector
    /// Solves the linear system Ax=B when given A and B with no particular structure for A nor B assumed. The method may
    /// fail when A is singular.
    val SolveLinearSystems : A:matrix -> B:matrix -> matrix
    
    /// Compute the LU decomposition of matrix A: PA=LU. Uses LAPACK *getrf if possible.
    val LU : A:matrix -> P:Microsoft.FSharp.Compatibility.permutation * L:matrix * U:matrix
    /// Compute the cholesky decomposition of matrix A: A = C’C. Uses LAPACK *potrf if possible.
    val Cholesky : A:matrix -> matrix
    /// Computes the inverse of matrix A. Uses LAPACK *gesv on an identity matrix if possible.
    val Inverse : A:matrix -> matrix
    
    /// Compute the QR factorization of matrix A: A=QR. Uses LAPACK *geqrf if possible.
    val QR : A:matrix -> Q:matrix * R:matrix
    
    /// Compute the SVD of matrix A: A=UDV'. The singular values are returned in descending
    /// order. Uses LAPACK *gesvd if possible.
    val SVD : A:matrix -> U:matrix * D:Vector<float> * V:matrix
    
    /// Compute the eigenvalues of matrix A: A=UVU'. Uses LAPACK *geev if possible.
    val EigenValues : A:matrix -> Vector<complex>
    /// Compute the eigenvalue-eigenvector decomposition of matrix A: A=UVU'. Uses LAPACK *geev if possible.
    val EigenSpectrum : A:matrix -> V:Vector<complex> * U:Matrix<complex>
    /// Compute the eigenvalues of matrix A: A=UVU'. Uses LAPACK *syev if possible.
    val EigenValuesWhenSymmetric : A:matrix -> vector
    /// Compute the eigenvalue-eigenvector decomposition of matrix A: A=UVU'. Uses LAPACK *syev if possible.
    val EigenSpectrumWhenSymmetric : A:matrix -> V:vector * U:matrix
    /// Compute a Hessenberg form of the matrix A. Uses LAPACK *gehrd if possible.
    val Hessenberg : A:matrix -> Q:matrix * H:matrix
    
    /// Computes a least squares solution using a QR factorization; e.g. min_x ||b - Ax||
    /// using the 2-norm. Uses LAPACK *gels if possible.
    val LeastSquares : A:matrix -> b:vector -> vector
