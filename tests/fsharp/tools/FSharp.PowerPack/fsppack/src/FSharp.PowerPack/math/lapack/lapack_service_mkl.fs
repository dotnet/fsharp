namespace Microsoft.FSharp.Math.Bindings.Internals
#nowarn "51"
open Microsoft.FSharp.Math
open Microsoft.FSharp.Math.Bindings.Internals
/// Warning:
/// IMPORTANT WARNING NOTICE:
/// INTEL MATH KERNEL LIBRARY 9.1 FOR WINDOWS DOES NOT BELONG TO MICROSOFT - IT IS THIRD PARTY TECHNOLOGY.
/// IT IS CLEARED ONLY FOR USE BY A SPECIFIC MSR RESEARCH TEAM.
/// DO NOT USE IT UNTIL YOU HAVE CLEARED ITS USE FOR YOUR PROJECT WITH YOUR LEGAL CONTACT.
/// 
/// The following stubs bind directly to Intel MKL functionality.
/// You should not use them without:
/// a) Intel MKL developer licenses.
/// b) Seeking local legal approval.
module LapackMKLStubs = begin
  [<System.Runtime.InteropServices.DllImport(@"mkl_def.dll",EntryPoint="dgemm")>]
  extern void dgemm_(char *transa, char *transb, int *m, int *n, int *k, double *alpha, double *a, int *lda, double *b, int *ldb, double *beta, double *c, int *ldc);

  [<System.Runtime.InteropServices.DllImport(@"mkl_def.dll",EntryPoint="dtrsv")>]
  extern void dtrsv_(char *uplo,char *trans,char *diag,int *n,double *a,int *lda,double *x,int *incx);

  [<System.Runtime.InteropServices.DllImport(@"mkl_def.dll",EntryPoint="dtrsm")>]
  extern void dtrsm_(char *side,char *uplo,char *trans,char *diag,int *m,int *n,double *alpha,double *a,int *lda,double *b,int *ldb);   
   
  [<System.Runtime.InteropServices.DllImport(@"mkl_lapack.dll",EntryPoint="dgesv")>]
  extern void dgesv_(int *n, int *nrhs, double *a, int *lda, int *ipiv, double *b, int *ldb, int *info);

  [<System.Runtime.InteropServices.DllImport(@"mkl_lapack.dll",EntryPoint="dgeev")>]
  extern void dgeev_(char *jobvl, char *jobvr, int *n, double *a, int *lda, double *wr, double *wi, double *vl, int *ldvl, double *vr, int *ldvr, double *work, int *lwork, int *info);
  [<System.Runtime.InteropServices.DllImport(@"mkl_lapack.dll",EntryPoint="dposv")>]
  extern void dposv_(char *uplo, int *n, int *nrhs, double *a, int *lda, double *b, int *ldb, int *info);
  
  [<System.Runtime.InteropServices.DllImport(@"mkl_lapack.dll",EntryPoint="dgels")>]
  extern void dgels_(char *trans, int *m,int *n, int *nrhs, double *a, int *lda, double *b, int *ldb, double *work, int *lwork, int *info);
  
  [<System.Runtime.InteropServices.DllImport(@"mkl_lapack.dll",EntryPoint="dgglse")>]
  extern void dgglse_(int *m, int *n, int *p, double *a, int *lda, double *b, int *ldb, double *c, double *d, double *x, double *work, int *lwork, int *info);
  
  [<System.Runtime.InteropServices.DllImport(@"mkl_lapack.dll",EntryPoint="dsyev")>]
  extern void dsyev_(char *jobz, char *uplo, int *n, double *a,int *lda, double *w, double *work, int *lwork, int *info);
  
  [<System.Runtime.InteropServices.DllImport(@"mkl_lapack.dll",EntryPoint="dsyevd")>]
  extern void dsyevd_(char *jobz, char *uplo, int *n, double *a, int *lda, double *w, double *work, int *lwork, int *iwork, int *liwork, int *info);
  
  [<System.Runtime.InteropServices.DllImport(@"mkl_lapack.dll",EntryPoint="dgesvd")>]
  extern void dgesvd_(char *jobu, char *jobvt, int  *m, int *n, double *a, int *lda, double *s, double *u, int *ldu, double *vt, int *ldvt, double *work, int *lwork, int *info);
  
  [<System.Runtime.InteropServices.DllImport(@"mkl_lapack.dll",EntryPoint="dgesdd")>]
  extern void dgesdd_(char *JOBZ, int  *m, int *n, double *a, int *lda, double *s, double *u, int *ldu, double *vt, int *ldvt, double *work, int *lwork,int *iwork, int *info);
  
  [<System.Runtime.InteropServices.DllImport(@"mkl_lapack.dll",EntryPoint="dsygv")>]
  extern void dsygv_(int *itype, char *jobz, char *uplo, int *n, double *a, int *lda, double *b, int *ldb, double *w, double *work, int *lwork, int *info);
  
  [<System.Runtime.InteropServices.DllImport(@"mkl_lapack.dll",EntryPoint="dsygvd")>]     
  extern void dsygvd_(int *itype, char *jobz, char *uplo, int *n, double *a, int *lda, double *b, int *ldb, double *w, double *work, int *lwork,int *iwork, int *liwork, int *info);
  
  [<System.Runtime.InteropServices.DllImport(@"mkl_lapack.dll",EntryPoint="dggev")>]     
  extern void dggev_( char *jobvl, char *jobvr, int *n, double *a, int *lda, double *b, int *ldb, double *alphar, double *alphai,double *beta,double *vl,int *ldvl,double *vr,int *ldvr,double *work, int *lwork,int *info);
  
  [<System.Runtime.InteropServices.DllImport(@"mkl_lapack.dll",EntryPoint="dgesvx")>]     
  extern void dgesvx_(char *fact, char *trans, int *n, int *nrhs, double *a, int *lda, double *af, int *ldaf, int *ipiv, char *equed, double *r, double *c, double *b, int *ldb, double *x, int *ldx, double *rcond, double *ferr, double *berr, double *work, int *iwork, int *info);
  
  [<System.Runtime.InteropServices.DllImport(@"mkl_lapack.dll",EntryPoint="dposvx")>]     
  extern void  dposvx_(char *fact, char *uplo, int *n, int *nrhs, double *a, int *lda, double *af, int *ldaf, char *equed, double *s, double *b, int *ldb, double *x, int *ldx, double *rcond, double  *ferr, double *berr, double *work, int *iwork, int *info);

  [<System.Runtime.InteropServices.DllImport(@"mkl_lapack.dll",EntryPoint="dpotrf")>]     
  extern void  dpotrf_(char *uplo, int *n, double *a, int *lda, int *info);
  
  [<System.Runtime.InteropServices.DllImport(@"mkl_lapack.dll",EntryPoint="dgetrf")>]     
  extern void  dgetrf_(int *m, int *n, double *a, int *lda, int *ipiv, int *info);
  
  [<System.Runtime.InteropServices.DllImport(@"mkl_lapack.dll",EntryPoint="dgeqrf")>]     
  extern void dgeqrf_(int  *m, int *n, double *a, int *lda, double *tau, double *work, int *lwork, int *info);
  
  [<System.Runtime.InteropServices.DllImport(@"mkl_def.dll",EntryPoint="dgemv")>]
  extern void dgemv_(char* trans, int* m, int* n,double* alpha, double* A, int* lda,double* x, int* incx, double* beta,double* y, int* incy);
  
end
/// Internal provider of Lapack functionality, not for direct user usage.
type LapackMKLService() = class
 interface ILapack with 
///Matrix-Matrix Multiplication
 member this.dgemm_((a:matrix),(b:matrix)) = 
  // input copies
  let a = Matrix.copy a
  let b = Matrix.copy b
  // dimensions
  let m = NativeUtilities.matrixDim1 a in
  let k = NativeUtilities.matrixDim2 a in
  NativeUtilities.assertDimensions "dgemm_" ("k","Dim1(b)") (k,NativeUtilities.matrixDim1 b);
  let n = NativeUtilities.matrixDim2 b in
  // allocate results
  let c = Matrix.zero (m) (n)
  // transpose
  let c = Matrix.transpose c
  // setup actuals
  let mutable arg_transa = 't'
  let mutable arg_transb = 't'
  let mutable arg_m = m
  let mutable arg_n = n
  let mutable arg_k = k
  let mutable arg_alpha = 1.0
  let arg_a = NativeUtilities.pinM a
  let mutable arg_ldk = k
  let arg_b = NativeUtilities.pinM b
  let mutable arg_ldn = n
  let mutable arg_beta = 1.0
  let arg_c = NativeUtilities.pinM c
  let mutable arg_ldm = m
  // call function
  try
    LapackMKLStubs.dgemm_(&&arg_transa,&&arg_transb,&&arg_m,&&arg_n,&&arg_k,&&arg_alpha,arg_a.Ptr,&&arg_ldk,arg_b.Ptr,&&arg_ldn,&&arg_beta,arg_c.Ptr,&&arg_ldm)
  finally
    NativeUtilities.freeM arg_a
    NativeUtilities.freeM arg_b
    NativeUtilities.freeM arg_c
  // INFO
  // fixups
  let c = Matrix.transpose c
  // result tuple
  c


///Solve
 member this.dgesv_((a:matrix),(b:matrix)) = 
  // input copies
  let a = Matrix.copy a
  let b = Matrix.copy b
  // dimensions
  let n = NativeUtilities.matrixDim1 a in
  NativeUtilities.assertDimensions "dgesv_" ("n","Dim2(a)") (n,NativeUtilities.matrixDim2 a);
  NativeUtilities.assertDimensions "dgesv_" ("n","Dim1(b)") (n,NativeUtilities.matrixDim1 b);
  let NRHS = NativeUtilities.matrixDim2 b in
  // allocate results
  let ipiv = Array.zeroCreate  (n)
  // transpose
  let a = Matrix.transpose a
  let b = Matrix.transpose b
  // setup actuals
  let mutable arg_n = n
  let mutable arg_NRHS = NRHS
  let arg_a = NativeUtilities.pinM a
  let mutable arg_lda = max 1 n
  let arg_ipiv = NativeUtilities.pinA ipiv
  let arg_b = NativeUtilities.pinM b
  let mutable arg_ldb = max 1 n
  let mutable arg_info = 0
  // call function
  try
    LapackMKLStubs.dgesv_(&&arg_n,&&arg_NRHS,arg_a.Ptr,&&arg_lda,arg_ipiv.Ptr,arg_b.Ptr,&&arg_ldb,&&arg_info)
  finally
    NativeUtilities.freeM arg_a
    NativeUtilities.freeA arg_ipiv
    NativeUtilities.freeM arg_b
  // INFO
  match arg_info with
   | -1  -> invalid_arg "dgesv_: n (argument 1)"
   | -2  -> invalid_arg "dgesv_: NRHS (argument 2)"
   | -3  -> invalid_arg "dgesv_: a (argument 3)"
   | -4  -> invalid_arg "dgesv_: lda (argument 4)"
   | -5  -> invalid_arg "dgesv_: ipiv (argument 5)"
   | -6  -> invalid_arg "dgesv_: b (argument 6)"
   | -7  -> invalid_arg "dgesv_: ldb (argument 7)"
   | -8  -> invalid_arg "dgesv_: info (argument 8)"
   | 0   -> ()
   | n   -> failwith (sprintf "dgesv_ : returned %d. The computation failed." n)
  // fixups
  let a = Matrix.transpose a
  let b = Matrix.transpose b
  // result tuple
  a,ipiv,b


///Solve dtrsv
 member this.dtrsv_((uplo),(a:matrix),(x:vector)) = 
  // input copies
  let a = Matrix.copy a
  let x = Vector.copy x
  // dimensions
  let n = NativeUtilities.matrixDim1 a in
  NativeUtilities.assertDimensions "dtrsv_" ("n","Dim2(a)") (n,NativeUtilities.matrixDim2 a);
  NativeUtilities.assertDimensions "dtrsv_" ("n","Dim(x)") (n,NativeUtilities.vectorDim  x);
  // allocate results
  // transpose
  // setup actuals
  let mutable arg_uplo = uplo
  let mutable arg_transa = 't'
  let mutable arg_diag = 'N'
  let mutable arg_n = n
  let arg_a = NativeUtilities.pinM a
  let mutable arg_lda = max 1 n
  let arg_x = NativeUtilities.pinV x
  let mutable arg_incx = 1
  // call function
  try
    LapackMKLStubs.dtrsv_(&&arg_uplo,&&arg_transa,&&arg_diag,&&arg_n,arg_a.Ptr,&&arg_lda,arg_x.Ptr,&&arg_incx)
  finally
    NativeUtilities.freeM arg_a
    NativeUtilities.freeV arg_x
  // INFO
  // fixups
  // result tuple
  x


///Solve dtrsm
 member this.dtrsm_((uplo),(a:matrix),(b:matrix)) = 
  // input copies
  let a = Matrix.copy a
  let b = Matrix.copy b
  // dimensions
  let m = NativeUtilities.matrixDim1 a in
  let k = NativeUtilities.matrixDim2 a in
  NativeUtilities.assertDimensions "dtrsm_" ("m","Dim1(b)") (m,NativeUtilities.matrixDim1 b);
  let n = NativeUtilities.matrixDim2 b in
  // allocate results
  // transpose
  let b = Matrix.transpose b
  // setup actuals
  let mutable arg_side = 'L'
  let mutable arg_uplo = uplo
  let mutable arg_transa = 't'
  let mutable arg_diag = 'N'
  let mutable arg_m = m
  let mutable arg_n = n
  let mutable arg_alpha = 1.0
  let arg_a = NativeUtilities.pinM a
  let mutable arg_lda = m
  let arg_b = NativeUtilities.pinM b
  let mutable arg_ldb = m
  // call function
  try
    LapackMKLStubs.dtrsm_(&&arg_side,&&arg_uplo,&&arg_transa,&&arg_diag,&&arg_m,&&arg_n,&&arg_alpha,arg_a.Ptr,&&arg_lda,arg_b.Ptr,&&arg_ldb)
  finally
    NativeUtilities.freeM arg_a
    NativeUtilities.freeM arg_b
  // INFO
  // fixups
  let b = Matrix.transpose b
  // result tuple
  b


///Solve LSE using GRQ
 member this.dgglse_((a:matrix),(b:matrix),(c:vector),(d:vector)) = 
  // input copies
  let a = Matrix.copy a
  let b = Matrix.copy b
  let c = Vector.copy c
  let d = Vector.copy d
  // dimensions
  let m = NativeUtilities.matrixDim1 a in
  let n = NativeUtilities.matrixDim2 a in
  let p = NativeUtilities.matrixDim1 b in
  NativeUtilities.assertDimensions "dgglse_" ("n","Dim2(b)") (n,NativeUtilities.matrixDim2 b);
  NativeUtilities.assertDimensions "dgglse_" ("m","Dim(c)") (m,NativeUtilities.vectorDim  c);
  NativeUtilities.assertDimensions "dgglse_" ("p","Dim(d)") (p,NativeUtilities.vectorDim  d);
  // allocate results
  let x = Array.zeroCreate  (n)
  let work = Array.zeroCreate  (1)
  // transpose
  let a = Matrix.transpose a
  let b = Matrix.transpose b
  // setup actuals
  let mutable arg_m = m
  let mutable arg_n = n
  let mutable arg_p = p
  let arg_a = NativeUtilities.pinM a
  let mutable arg_lda = max 1 m
  let arg_b = NativeUtilities.pinM b
  let mutable arg_ldb = max 1 p
  let arg_c = NativeUtilities.pinV c
  let arg_d = NativeUtilities.pinV d
  let arg_x = NativeUtilities.pinA x
  let arg_work = NativeUtilities.pinA work
  let mutable arg_lwork = -1
  let mutable arg_info = 0
  // ask for work array size
  try
    LapackMKLStubs.dgglse_(&&arg_m,&&arg_n,&&arg_p,arg_a.Ptr,&&arg_lda,arg_b.Ptr,&&arg_ldb,arg_c.Ptr,arg_d.Ptr,arg_x.Ptr,arg_work.Ptr,&&arg_lwork,&&arg_info)
  finally
    NativeUtilities.freeA arg_work
  if arg_info = 0   || arg_info=(-12) then
    arg_lwork <- int32 work.[0]
  else assert(false)
  let arg_work = NativeUtilities.pinA (Array.zeroCreate arg_lwork : float[])
  // call function
  try
    LapackMKLStubs.dgglse_(&&arg_m,&&arg_n,&&arg_p,arg_a.Ptr,&&arg_lda,arg_b.Ptr,&&arg_ldb,arg_c.Ptr,arg_d.Ptr,arg_x.Ptr,arg_work.Ptr,&&arg_lwork,&&arg_info)
  finally
    NativeUtilities.freeM arg_a
    NativeUtilities.freeM arg_b
    NativeUtilities.freeV arg_c
    NativeUtilities.freeV arg_d
    NativeUtilities.freeA arg_x
    NativeUtilities.freeA arg_work
  // INFO
  match arg_info with
   | -1  -> invalid_arg "dgglse_: m (argument 1)"
   | -2  -> invalid_arg "dgglse_: n (argument 2)"
   | -3  -> invalid_arg "dgglse_: p (argument 3)"
   | -4  -> invalid_arg "dgglse_: a (argument 4)"
   | -5  -> invalid_arg "dgglse_: lda (argument 5)"
   | -6  -> invalid_arg "dgglse_: b (argument 6)"
   | -7  -> invalid_arg "dgglse_: ldb (argument 7)"
   | -8  -> invalid_arg "dgglse_: c (argument 8)"
   | -9  -> invalid_arg "dgglse_: d (argument 9)"
   | -10 -> invalid_arg "dgglse_: x (argument 10)"
   | -11 -> invalid_arg "dgglse_: work (argument 11)"
   | -12 -> invalid_arg "dgglse_: lwork (argument 12)"
   | -13 -> invalid_arg "dgglse_: info (argument 13)"
   | 0   -> ()
   | n   -> failwith (sprintf "dgglse_ : returned %d. The computation failed." n)
  // fixups
  let a = Matrix.transpose a
  // result tuple
  a,c,x


///EigenValue Non-Symetrix
 member this.dgeev_((jobvr),(a:matrix)) = 
  // input copies
  let a = Matrix.copy a
  // dimensions
  let n = NativeUtilities.matrixDim1 a in
  NativeUtilities.assertDimensions "dgeev_" ("n","Dim2(a)") (n,NativeUtilities.matrixDim2 a);
  // allocate results
  let wr = Array.zeroCreate  (n)
  let wi = Array.zeroCreate  (n)
  let vl = Array2D.zeroCreate (n) (n)
  let vr = Array2D.zeroCreate (n) (n)
  let work = Array.zeroCreate  ((4*n))
  // transpose
  let a = Matrix.transpose a
  // setup actuals
  let mutable arg_jobvl = 'N'
  let mutable arg_jobvr = jobvr
  let mutable arg_n = n
  let arg_a = NativeUtilities.pinM a
  let mutable arg_lda = n
  let arg_wr = NativeUtilities.pinA wr
  let arg_wi = NativeUtilities.pinA wi
  let arg_vl = NativeUtilities.pinA2 vl
  let mutable arg_ldvl = n
  let arg_vr = NativeUtilities.pinA2 vr
  let mutable arg_ldvr = n
  let arg_work = NativeUtilities.pinA work
  let mutable arg_lwork = (4*n)
  let mutable arg_info = 0
  // call function
  try
    LapackMKLStubs.dgeev_(&&arg_jobvl,&&arg_jobvr,&&arg_n,arg_a.Ptr,&&arg_lda,arg_wr.Ptr,arg_wi.Ptr,arg_vl.Ptr,&&arg_ldvl,arg_vr.Ptr,&&arg_ldvr,arg_work.Ptr,&&arg_lwork,&&arg_info)
  finally
    NativeUtilities.freeM arg_a
    NativeUtilities.freeA arg_wr
    NativeUtilities.freeA arg_wi
    NativeUtilities.freeA2 arg_vl
    NativeUtilities.freeA2 arg_vr
    NativeUtilities.freeA arg_work
  // INFO
  match arg_info with
   | -1  -> invalid_arg "dgeev_: jobvl (argument 1)"
   | -2  -> invalid_arg "dgeev_: jobvr (argument 2)"
   | -3  -> invalid_arg "dgeev_: n (argument 3)"
   | -4  -> invalid_arg "dgeev_: a (argument 4)"
   | -5  -> invalid_arg "dgeev_: lda (argument 5)"
   | -6  -> invalid_arg "dgeev_: wr (argument 6)"
   | -7  -> invalid_arg "dgeev_: wi (argument 7)"
   | -8  -> invalid_arg "dgeev_: vl (argument 8)"
   | -9  -> invalid_arg "dgeev_: ldvl (argument 9)"
   | -10 -> invalid_arg "dgeev_: vr (argument 10)"
   | -11 -> invalid_arg "dgeev_: ldvr (argument 11)"
   | -12 -> invalid_arg "dgeev_: work (argument 12)"
   | -13 -> invalid_arg "dgeev_: lwork (argument 13)"
   | -14 -> invalid_arg "dgeev_: info (argument 14)"
   | 0   -> ()
   | n   -> failwith (sprintf "dgeev_ : returned %d. The computation failed." n)
  // fixups
  // result tuple
  wr,wi,vr


///Solve Cholesky
 member this.dposv_((a:matrix),(b:matrix)) = 
  // input copies
  let a = Matrix.copy a
  let b = Matrix.copy b
  // dimensions
  let n = NativeUtilities.matrixDim1 a in
  NativeUtilities.assertDimensions "dposv_" ("n","Dim2(a)") (n,NativeUtilities.matrixDim2 a);
  NativeUtilities.assertDimensions "dposv_" ("n","Dim1(b)") (n,NativeUtilities.matrixDim1 b);
  let nrhs = NativeUtilities.matrixDim2 b in
  // allocate results
  // transpose
  let a = Matrix.transpose a
  let b = Matrix.transpose b
  // setup actuals
  let mutable arg_uplo = 'U'
  let mutable arg_n = n
  let mutable arg_nrhs = nrhs
  let arg_a = NativeUtilities.pinM a
  let mutable arg_lda = max 1 n
  let arg_b = NativeUtilities.pinM b
  let mutable arg_ldb = max 1 n
  let mutable arg_info = 0
  // call function
  try
    LapackMKLStubs.dposv_(&&arg_uplo,&&arg_n,&&arg_nrhs,arg_a.Ptr,&&arg_lda,arg_b.Ptr,&&arg_ldb,&&arg_info)
  finally
    NativeUtilities.freeM arg_a
    NativeUtilities.freeM arg_b
  // INFO
  match arg_info with
   | -1  -> invalid_arg "dposv_: uplo (argument 1)"
   | -2  -> invalid_arg "dposv_: n (argument 2)"
   | -3  -> invalid_arg "dposv_: nrhs (argument 3)"
   | -4  -> invalid_arg "dposv_: a (argument 4)"
   | -5  -> invalid_arg "dposv_: lda (argument 5)"
   | -6  -> invalid_arg "dposv_: b (argument 6)"
   | -7  -> invalid_arg "dposv_: ldb (argument 7)"
   | -8  -> invalid_arg "dposv_: info (argument 8)"
   | 0   -> ()
   | n   -> failwith (sprintf "dposv_ : returned %d. The computation failed." n)
  // fixups
  let a = Matrix.transpose a
  let b = Matrix.transpose b
  // result tuple
  a,b


///Solve Upper
 member this.dgels_((a:matrix),(b:matrix)) = 
  // input copies
  let a = Matrix.copy a
  let b = Matrix.copy b
  // dimensions
  let m = NativeUtilities.matrixDim1 a in
  let n = NativeUtilities.matrixDim2 a in
  NativeUtilities.assertDimensions "dgels_" ("(max m n)","Dim1(b)") ((max m n),NativeUtilities.matrixDim1 b);
  let nrhs = NativeUtilities.matrixDim2 b in
  // allocate results
  let work = Array.zeroCreate  (1)
  // transpose
  let a = Matrix.transpose a
  let b = Matrix.transpose b
  // setup actuals
  let mutable arg_transa = 'n'
  let mutable arg_m = m
  let mutable arg_n = n
  let mutable arg_nrhs = nrhs
  let arg_a = NativeUtilities.pinM a
  let mutable arg_lda = max 1 m
  let arg_b = NativeUtilities.pinM b
  let mutable arg_ldb = max m (max 1 n)
  let arg_work = NativeUtilities.pinA work
  let mutable arg_lwork = -1
  let mutable arg_info = 0
  // ask for work array size
  try
    LapackMKLStubs.dgels_(&&arg_transa,&&arg_m,&&arg_n,&&arg_nrhs,arg_a.Ptr,&&arg_lda,arg_b.Ptr,&&arg_ldb,arg_work.Ptr,&&arg_lwork,&&arg_info)
  finally
    NativeUtilities.freeA arg_work
  if arg_info = 0   || arg_info=(-10) then
    arg_lwork <- int32 work.[0]
  else assert(false)
  let arg_work = NativeUtilities.pinA (Array.zeroCreate arg_lwork : float[])
  // call function
  try
    LapackMKLStubs.dgels_(&&arg_transa,&&arg_m,&&arg_n,&&arg_nrhs,arg_a.Ptr,&&arg_lda,arg_b.Ptr,&&arg_ldb,arg_work.Ptr,&&arg_lwork,&&arg_info)
  finally
    NativeUtilities.freeM arg_a
    NativeUtilities.freeM arg_b
    NativeUtilities.freeA arg_work
  // INFO
  match arg_info with
   | -1  -> invalid_arg "dgels_: transa (argument 1)"
   | -2  -> invalid_arg "dgels_: m (argument 2)"
   | -3  -> invalid_arg "dgels_: n (argument 3)"
   | -4  -> invalid_arg "dgels_: nrhs (argument 4)"
   | -5  -> invalid_arg "dgels_: a (argument 5)"
   | -6  -> invalid_arg "dgels_: lda (argument 6)"
   | -7  -> invalid_arg "dgels_: b (argument 7)"
   | -8  -> invalid_arg "dgels_: ldb (argument 8)"
   | -9  -> invalid_arg "dgels_: work (argument 9)"
   | -10 -> invalid_arg "dgels_: lwork (argument 10)"
   | -11 -> invalid_arg "dgels_: info (argument 11)"
   | 0   -> ()
   | n   -> failwith (sprintf "dgels_ : returned %d. The computation failed." n)
  // fixups
  let a = Matrix.transpose a
  let b = Matrix.transpose b
  // result tuple
  a,b


///Eigen Value of Symetric Matrix
 member this.dsyev_((jobz),(uplo),(a:matrix)) = 
  // input copies
  let a = Matrix.copy a
  // dimensions
  let n = NativeUtilities.matrixDim1 a in
  NativeUtilities.assertDimensions "dsyev_" ("n","Dim2(a)") (n,NativeUtilities.matrixDim2 a);
  // allocate results
  let w = Array.zeroCreate  (n)
  let work = Array.zeroCreate  (1)
  // transpose
  let a = Matrix.transpose a
  // setup actuals
  let mutable arg_jobz = jobz
  let mutable arg_uplo = uplo
  let mutable arg_n = n
  let arg_a = NativeUtilities.pinM a
  let mutable arg_lda = max 1 n
  let arg_w = NativeUtilities.pinA w
  let arg_work = NativeUtilities.pinA work
  let mutable arg_lwork = -1
  let mutable arg_info = 0
  // ask for work array size
  try
    LapackMKLStubs.dsyev_(&&arg_jobz,&&arg_uplo,&&arg_n,arg_a.Ptr,&&arg_lda,arg_w.Ptr,arg_work.Ptr,&&arg_lwork,&&arg_info)
  finally
    NativeUtilities.freeA arg_work
  if arg_info = 0   || arg_info=(-8) then
    arg_lwork <- int32 work.[0]
  else assert(false)
  let arg_work = NativeUtilities.pinA (Array.zeroCreate arg_lwork : float[])
  // call function
  try
    LapackMKLStubs.dsyev_(&&arg_jobz,&&arg_uplo,&&arg_n,arg_a.Ptr,&&arg_lda,arg_w.Ptr,arg_work.Ptr,&&arg_lwork,&&arg_info)
  finally
    NativeUtilities.freeM arg_a
    NativeUtilities.freeA arg_w
    NativeUtilities.freeA arg_work
  // INFO
  match arg_info with
   | -1  -> invalid_arg "dsyev_: jobz (argument 1)"
   | -2  -> invalid_arg "dsyev_: uplo (argument 2)"
   | -3  -> invalid_arg "dsyev_: n (argument 3)"
   | -4  -> invalid_arg "dsyev_: a (argument 4)"
   | -5  -> invalid_arg "dsyev_: lda (argument 5)"
   | -6  -> invalid_arg "dsyev_: w (argument 6)"
   | -7  -> invalid_arg "dsyev_: work (argument 7)"
   | -8  -> invalid_arg "dsyev_: lwork (argument 8)"
   | -9  -> invalid_arg "dsyev_: info (argument 9)"
   | 0   -> ()
   | n   -> failwith (sprintf "dsyev_ : returned %d. The computation failed." n)
  // fixups
  let a = Matrix.transpose a
  // result tuple
  a,w


///Eigen Value of Symetric Matrix - Divide and Conquer
 member this.dsyevd_((jobz),(uplo),(a:matrix)) = 
  // input copies
  let a = Matrix.copy a
  // dimensions
  let n = NativeUtilities.matrixDim1 a in
  NativeUtilities.assertDimensions "dsyevd_" ("n","Dim2(a)") (n,NativeUtilities.matrixDim2 a);
  // allocate results
  let w = Array.zeroCreate  (n)
  let work = Array.zeroCreate  (1)
  let iwork = Array.zeroCreate  (1)
  // transpose
  let a = Matrix.transpose a
  // setup actuals
  let mutable arg_jobz = jobz
  let mutable arg_uplo = uplo
  let mutable arg_n = n
  let arg_a = NativeUtilities.pinM a
  let mutable arg_lda = max 1 n
  let arg_w = NativeUtilities.pinA w
  let arg_work = NativeUtilities.pinA work
  let mutable arg_lwork = -1
  let arg_iwork = NativeUtilities.pinA iwork
  let mutable arg_liwork = -1
  let mutable arg_info = 0
  // ask for work array size
  try
    LapackMKLStubs.dsyevd_(&&arg_jobz,&&arg_uplo,&&arg_n,arg_a.Ptr,&&arg_lda,arg_w.Ptr,arg_work.Ptr,&&arg_lwork,arg_iwork.Ptr,&&arg_liwork,&&arg_info)
  finally
    NativeUtilities.freeA arg_work
    NativeUtilities.freeA arg_iwork
  if arg_info = 0   || arg_info=(-8)  || arg_info=(-10) then
    arg_lwork <- int32 work.[0]
    arg_liwork <-  iwork.[0]
  else assert(false)
  let arg_work = NativeUtilities.pinA (Array.zeroCreate arg_lwork : float[])
  let arg_iwork = NativeUtilities.pinA (Array.zeroCreate arg_liwork : int[])
  // call function
  try
    LapackMKLStubs.dsyevd_(&&arg_jobz,&&arg_uplo,&&arg_n,arg_a.Ptr,&&arg_lda,arg_w.Ptr,arg_work.Ptr,&&arg_lwork,arg_iwork.Ptr,&&arg_liwork,&&arg_info)
  finally
    NativeUtilities.freeM arg_a
    NativeUtilities.freeA arg_w
    NativeUtilities.freeA arg_work
    NativeUtilities.freeA arg_iwork
  // INFO
  match arg_info with
   | -1  -> invalid_arg "dsyevd_: jobz (argument 1)"
   | -2  -> invalid_arg "dsyevd_: uplo (argument 2)"
   | -3  -> invalid_arg "dsyevd_: n (argument 3)"
   | -4  -> invalid_arg "dsyevd_: a (argument 4)"
   | -5  -> invalid_arg "dsyevd_: lda (argument 5)"
   | -6  -> invalid_arg "dsyevd_: w (argument 6)"
   | -7  -> invalid_arg "dsyevd_: work (argument 7)"
   | -8  -> invalid_arg "dsyevd_: lwork (argument 8)"
   | -9  -> invalid_arg "dsyevd_: iwork (argument 9)"
   | -10 -> invalid_arg "dsyevd_: liwork (argument 10)"
   | -11 -> invalid_arg "dsyevd_: info (argument 11)"
   | 0   -> ()
   | n   -> failwith (sprintf "dsyevd_ : returned %d. The computation failed." n)
  // fixups
  let a = Matrix.transpose a
  // result tuple
  a,w


///Singular Value Decomposition
 member this.dgesvd_((a:matrix)) = 
  // input copies
  let a = Matrix.copy a
  // dimensions
  let m = NativeUtilities.matrixDim1 a in
  let n = NativeUtilities.matrixDim2 a in
  // allocate results
  let s = Array.zeroCreate  (min m n)
  let u = Matrix.zero (m) (min m n)
  let vt = Matrix.zero (n) (n)
  let work = Array.zeroCreate  (1)
  // transpose
  let a = Matrix.transpose a
  let u = Matrix.transpose u
  let vt = Matrix.transpose vt
  // setup actuals
  let mutable arg_jobu = 'A'
  let mutable arg_jobvt = 'A'
  let mutable arg_m = m
  let mutable arg_n = n
  let arg_a = NativeUtilities.pinM a
  let mutable arg_lda = max 1 m
  let arg_s = NativeUtilities.pinA s
  let arg_u = NativeUtilities.pinM u
  let mutable arg_ldu = m
  let arg_vt = NativeUtilities.pinM vt
  let mutable arg_ldvt = n
  let arg_work = NativeUtilities.pinA work
  let mutable arg_lwork = -1
  let mutable arg_info = 0
  // ask for work array size
  try
    LapackMKLStubs.dgesvd_(&&arg_jobu,&&arg_jobvt,&&arg_m,&&arg_n,arg_a.Ptr,&&arg_lda,arg_s.Ptr,arg_u.Ptr,&&arg_ldu,arg_vt.Ptr,&&arg_ldvt,arg_work.Ptr,&&arg_lwork,&&arg_info)
  finally
    NativeUtilities.freeA arg_work
  if arg_info = 0   || arg_info=(-13) then
    arg_lwork <- int32 work.[0]
  else assert(false)
  let arg_work = NativeUtilities.pinA (Array.zeroCreate arg_lwork : float[])
  // call function
  try
    LapackMKLStubs.dgesvd_(&&arg_jobu,&&arg_jobvt,&&arg_m,&&arg_n,arg_a.Ptr,&&arg_lda,arg_s.Ptr,arg_u.Ptr,&&arg_ldu,arg_vt.Ptr,&&arg_ldvt,arg_work.Ptr,&&arg_lwork,&&arg_info)
  finally
    NativeUtilities.freeM arg_a
    NativeUtilities.freeA arg_s
    NativeUtilities.freeM arg_u
    NativeUtilities.freeM arg_vt
    NativeUtilities.freeA arg_work
  // INFO
  match arg_info with
   | -1  -> invalid_arg "dgesvd_: jobu (argument 1)"
   | -2  -> invalid_arg "dgesvd_: jobvt (argument 2)"
   | -3  -> invalid_arg "dgesvd_: m (argument 3)"
   | -4  -> invalid_arg "dgesvd_: n (argument 4)"
   | -5  -> invalid_arg "dgesvd_: a (argument 5)"
   | -6  -> invalid_arg "dgesvd_: lda (argument 6)"
   | -7  -> invalid_arg "dgesvd_: s (argument 7)"
   | -8  -> invalid_arg "dgesvd_: u (argument 8)"
   | -9  -> invalid_arg "dgesvd_: ldu (argument 9)"
   | -10 -> invalid_arg "dgesvd_: vt (argument 10)"
   | -11 -> invalid_arg "dgesvd_: ldvt (argument 11)"
   | -12 -> invalid_arg "dgesvd_: work (argument 12)"
   | -13 -> invalid_arg "dgesvd_: lwork (argument 13)"
   | -14 -> invalid_arg "dgesvd_: info (argument 14)"
   | 0   -> ()
   | n   -> failwith (sprintf "dgesvd_ : returned %d. The computation failed." n)
  // fixups
  let u = Matrix.transpose u
  let vt = Matrix.transpose vt
  // result tuple
  s,u,vt


///Singular Value Decomposition Divide- Conquer
 member this.dgesdd_((a:matrix)) = 
  // input copies
  let a = Matrix.copy a
  // dimensions
  let m = NativeUtilities.matrixDim1 a in
  let n = NativeUtilities.matrixDim2 a in
  // allocate results
  let s = Array.zeroCreate  (min m n)
  let u = Matrix.zero (m) (m)
  let vt = Matrix.zero (n) (n)
  let work = Array.zeroCreate  (1)
  let iwork = Array.zeroCreate  (8*(min m n))
  // transpose
  let a = Matrix.transpose a
  let u = Matrix.transpose u
  let vt = Matrix.transpose vt
  // setup actuals
  let mutable arg_JOBZ = 'A'
  let mutable arg_m = m
  let mutable arg_n = n
  let arg_a = NativeUtilities.pinM a
  let mutable arg_lda = max 1 m
  let arg_s = NativeUtilities.pinA s
  let arg_u = NativeUtilities.pinM u
  let mutable arg_ldu = m
  let arg_vt = NativeUtilities.pinM vt
  let mutable arg_ldvt = n
  let arg_work = NativeUtilities.pinA work
  let mutable arg_lwork = -1
  let arg_iwork = NativeUtilities.pinA iwork
  let mutable arg_info = 0
  // ask for work array size
  try
    LapackMKLStubs.dgesdd_(&&arg_JOBZ,&&arg_m,&&arg_n,arg_a.Ptr,&&arg_lda,arg_s.Ptr,arg_u.Ptr,&&arg_ldu,arg_vt.Ptr,&&arg_ldvt,arg_work.Ptr,&&arg_lwork,arg_iwork.Ptr,&&arg_info)
  finally
    NativeUtilities.freeA arg_work
  if arg_info = 0   || arg_info=(-12) then
    arg_lwork <- int32 work.[0]
  else assert(false)
  let arg_work = NativeUtilities.pinA (Array.zeroCreate arg_lwork : float[])
  // call function
  try
    LapackMKLStubs.dgesdd_(&&arg_JOBZ,&&arg_m,&&arg_n,arg_a.Ptr,&&arg_lda,arg_s.Ptr,arg_u.Ptr,&&arg_ldu,arg_vt.Ptr,&&arg_ldvt,arg_work.Ptr,&&arg_lwork,arg_iwork.Ptr,&&arg_info)
  finally
    NativeUtilities.freeM arg_a
    NativeUtilities.freeA arg_s
    NativeUtilities.freeM arg_u
    NativeUtilities.freeM arg_vt
    NativeUtilities.freeA arg_work
    NativeUtilities.freeA arg_iwork
  // INFO
  match arg_info with
   | -1  -> invalid_arg "dgesdd_: JOBZ (argument 1)"
   | -2  -> invalid_arg "dgesdd_: m (argument 2)"
   | -3  -> invalid_arg "dgesdd_: n (argument 3)"
   | -4  -> invalid_arg "dgesdd_: a (argument 4)"
   | -5  -> invalid_arg "dgesdd_: lda (argument 5)"
   | -6  -> invalid_arg "dgesdd_: s (argument 6)"
   | -7  -> invalid_arg "dgesdd_: u (argument 7)"
   | -8  -> invalid_arg "dgesdd_: ldu (argument 8)"
   | -9  -> invalid_arg "dgesdd_: vt (argument 9)"
   | -10 -> invalid_arg "dgesdd_: ldvt (argument 10)"
   | -11 -> invalid_arg "dgesdd_: work (argument 11)"
   | -12 -> invalid_arg "dgesdd_: lwork (argument 12)"
   | -13 -> invalid_arg "dgesdd_: iwork (argument 13)"
   | -14 -> invalid_arg "dgesdd_: info (argument 14)"
   | 0   -> ()
   | n   -> failwith (sprintf "dgesdd_ : returned %d. The computation failed." n)
  // fixups
  let u = Matrix.transpose u
  let vt = Matrix.transpose vt
  // result tuple
  s,u,vt


///Single Value Decomposition for Symetric Matrices
 member this.dsygv_((a:matrix),(b:matrix)) = 
  // input copies
  let a = Matrix.copy a
  let b = Matrix.copy b
  // dimensions
  let n = NativeUtilities.matrixDim1 a in
  NativeUtilities.assertDimensions "dsygv_" ("n","Dim2(a)") (n,NativeUtilities.matrixDim2 a);
  NativeUtilities.assertDimensions "dsygv_" ("n","Dim1(b)") (n,NativeUtilities.matrixDim1 b);
  NativeUtilities.assertDimensions "dsygv_" ("n","Dim2(b)") (n,NativeUtilities.matrixDim2 b);
  // allocate results
  let w = Array.zeroCreate  (n)
  let work = Array.zeroCreate  (1)
  // transpose
  let a = Matrix.transpose a
  let b = Matrix.transpose b
  // setup actuals
  let mutable arg_itype = 1
  let mutable arg_JOBZ = 'V'
  let mutable arg_uplo = 'U'
  let mutable arg_n = n
  let arg_a = NativeUtilities.pinM a
  let mutable arg_lda = max 1 n
  let arg_b = NativeUtilities.pinM b
  let mutable arg_ldb = max 1 n
  let arg_w = NativeUtilities.pinA w
  let arg_work = NativeUtilities.pinA work
  let mutable arg_lwork = -1
  let mutable arg_info = 0
  // ask for work array size
  try
    LapackMKLStubs.dsygv_(&&arg_itype,&&arg_JOBZ,&&arg_uplo,&&arg_n,arg_a.Ptr,&&arg_lda,arg_b.Ptr,&&arg_ldb,arg_w.Ptr,arg_work.Ptr,&&arg_lwork,&&arg_info)
  finally
    NativeUtilities.freeA arg_work
  if arg_info = 0   || arg_info=(-11) then
    arg_lwork <- int32 work.[0]
  else assert(false)
  let arg_work = NativeUtilities.pinA (Array.zeroCreate arg_lwork : float[])
  // call function
  try
    LapackMKLStubs.dsygv_(&&arg_itype,&&arg_JOBZ,&&arg_uplo,&&arg_n,arg_a.Ptr,&&arg_lda,arg_b.Ptr,&&arg_ldb,arg_w.Ptr,arg_work.Ptr,&&arg_lwork,&&arg_info)
  finally
    NativeUtilities.freeM arg_a
    NativeUtilities.freeM arg_b
    NativeUtilities.freeA arg_w
    NativeUtilities.freeA arg_work
  // INFO
  match arg_info with
   | -1  -> invalid_arg "dsygv_: itype (argument 1)"
   | -2  -> invalid_arg "dsygv_: JOBZ (argument 2)"
   | -3  -> invalid_arg "dsygv_: uplo (argument 3)"
   | -4  -> invalid_arg "dsygv_: n (argument 4)"
   | -5  -> invalid_arg "dsygv_: a (argument 5)"
   | -6  -> invalid_arg "dsygv_: lda (argument 6)"
   | -7  -> invalid_arg "dsygv_: b (argument 7)"
   | -8  -> invalid_arg "dsygv_: ldb (argument 8)"
   | -9  -> invalid_arg "dsygv_: w (argument 9)"
   | -10 -> invalid_arg "dsygv_: work (argument 10)"
   | -11 -> invalid_arg "dsygv_: lwork (argument 11)"
   | -12 -> invalid_arg "dsygv_: info (argument 12)"
   | 0   -> ()
   | n   -> failwith (sprintf "dsygv_ : returned %d. The computation failed." n)
  // fixups
  let a = Matrix.transpose a
  let b = Matrix.transpose b
  // result tuple
  a,b,w


///Single Value Decomposition for Symetric Matrices Divide and Conquer
 member this.dsygvd_((a:matrix),(b:matrix)) = 
  // input copies
  let a = Matrix.copy a
  let b = Matrix.copy b
  // dimensions
  let n = NativeUtilities.matrixDim1 a in
  NativeUtilities.assertDimensions "dsygvd_" ("n","Dim2(a)") (n,NativeUtilities.matrixDim2 a);
  NativeUtilities.assertDimensions "dsygvd_" ("n","Dim1(b)") (n,NativeUtilities.matrixDim1 b);
  NativeUtilities.assertDimensions "dsygvd_" ("n","Dim2(b)") (n,NativeUtilities.matrixDim2 b);
  // allocate results
  let w = Array.zeroCreate  (n)
  let work = Array.zeroCreate  (1)
  let iwork = Array.zeroCreate  (1)
  // transpose
  let a = Matrix.transpose a
  let b = Matrix.transpose b
  // setup actuals
  let mutable arg_itype = 1
  let mutable arg_JOBZ = 'V'
  let mutable arg_uplo = 'U'
  let mutable arg_n = n
  let arg_a = NativeUtilities.pinM a
  let mutable arg_lda = max 1 n
  let arg_b = NativeUtilities.pinM b
  let mutable arg_ldb = max 1 n
  let arg_w = NativeUtilities.pinA w
  let arg_work = NativeUtilities.pinA work
  let mutable arg_lwork = -1
  let arg_iwork = NativeUtilities.pinA iwork
  let mutable arg_liwork = -1
  let mutable arg_info = 0
  // ask for work array size
  try
    LapackMKLStubs.dsygvd_(&&arg_itype,&&arg_JOBZ,&&arg_uplo,&&arg_n,arg_a.Ptr,&&arg_lda,arg_b.Ptr,&&arg_ldb,arg_w.Ptr,arg_work.Ptr,&&arg_lwork,arg_iwork.Ptr,&&arg_liwork,&&arg_info)
  finally
    NativeUtilities.freeA arg_work
    NativeUtilities.freeA arg_iwork
  if arg_info = 0   || arg_info=(-11)  || arg_info=(-13) then
    arg_lwork <- int32 work.[0]
    arg_liwork <-  iwork.[0]
  else assert(false)
  let arg_work = NativeUtilities.pinA (Array.zeroCreate arg_lwork : float[])
  let arg_iwork = NativeUtilities.pinA (Array.zeroCreate arg_liwork : int[])
  // call function
  try
    LapackMKLStubs.dsygvd_(&&arg_itype,&&arg_JOBZ,&&arg_uplo,&&arg_n,arg_a.Ptr,&&arg_lda,arg_b.Ptr,&&arg_ldb,arg_w.Ptr,arg_work.Ptr,&&arg_lwork,arg_iwork.Ptr,&&arg_liwork,&&arg_info)
  finally
    NativeUtilities.freeM arg_a
    NativeUtilities.freeM arg_b
    NativeUtilities.freeA arg_w
    NativeUtilities.freeA arg_work
    NativeUtilities.freeA arg_iwork
  // INFO
  match arg_info with
   | -1  -> invalid_arg "dsygvd_: itype (argument 1)"
   | -2  -> invalid_arg "dsygvd_: JOBZ (argument 2)"
   | -3  -> invalid_arg "dsygvd_: uplo (argument 3)"
   | -4  -> invalid_arg "dsygvd_: n (argument 4)"
   | -5  -> invalid_arg "dsygvd_: a (argument 5)"
   | -6  -> invalid_arg "dsygvd_: lda (argument 6)"
   | -7  -> invalid_arg "dsygvd_: b (argument 7)"
   | -8  -> invalid_arg "dsygvd_: ldb (argument 8)"
   | -9  -> invalid_arg "dsygvd_: w (argument 9)"
   | -10 -> invalid_arg "dsygvd_: work (argument 10)"
   | -11 -> invalid_arg "dsygvd_: lwork (argument 11)"
   | -12 -> invalid_arg "dsygvd_: iwork (argument 12)"
   | -13 -> invalid_arg "dsygvd_: liwork (argument 13)"
   | -14 -> invalid_arg "dsygvd_: info (argument 14)"
   | 0   -> ()
   | n   -> failwith (sprintf "dsygvd_ : returned %d. The computation failed." n)
  // fixups
  let a = Matrix.transpose a
  let b = Matrix.transpose b
  // result tuple
  a,b,w


///LU factorization to compute the solution to a real  system of linear equations 
 member this.dgesvx_((a:matrix),(b:matrix)) = 
  // input copies
  let a = Matrix.copy a
  let b = Matrix.copy b
  // dimensions
  let n = NativeUtilities.matrixDim1 a in
  NativeUtilities.assertDimensions "dgesvx_" ("n","Dim2(a)") (n,NativeUtilities.matrixDim2 a);
  NativeUtilities.assertDimensions "dgesvx_" ("n","Dim1(b)") (n,NativeUtilities.matrixDim1 b);
  let nrhs = NativeUtilities.matrixDim2 b in
  // allocate results
  let af = Matrix.zero (n) (n)
  let ipiv = Array.zeroCreate  (n)
  let r = Array.zeroCreate  (n)
  let c = Array.zeroCreate  (n)
  let x = Matrix.zero (n) (nrhs)
  let ferr = Array.zeroCreate  (nrhs)
  let berr = Array.zeroCreate  (nrhs)
  let work = Array.zeroCreate  (4*n)
  let iwork = Array.zeroCreate  (n)
  // transpose
  let a = Matrix.transpose a
  let af = Matrix.transpose af
  let b = Matrix.transpose b
  let x = Matrix.transpose x
  // setup actuals
  let mutable arg_fact = 'E'
  let mutable arg_transx = 'n'
  let mutable arg_n = n
  let mutable arg_nrhs = nrhs
  let arg_a = NativeUtilities.pinM a
  let mutable arg_lda = max 1 n
  let arg_af = NativeUtilities.pinM af
  let mutable arg_ldaf = max 1 n
  let arg_ipiv = NativeUtilities.pinA ipiv
  let mutable arg_equed = 'n'
  let arg_r = NativeUtilities.pinA r
  let arg_c = NativeUtilities.pinA c
  let arg_b = NativeUtilities.pinM b
  let mutable arg_ldb = max 1 n
  let arg_x = NativeUtilities.pinM x
  let mutable arg_ldx = max 1 n
  let mutable arg_rcond = 0.0
  let arg_ferr = NativeUtilities.pinA ferr
  let arg_berr = NativeUtilities.pinA berr
  let arg_work = NativeUtilities.pinA work
  let arg_iwork = NativeUtilities.pinA iwork
  let mutable arg_info = 0
  // call function
  try
    LapackMKLStubs.dgesvx_(&&arg_fact,&&arg_transx,&&arg_n,&&arg_nrhs,arg_a.Ptr,&&arg_lda,arg_af.Ptr,&&arg_ldaf,arg_ipiv.Ptr,&&arg_equed,arg_r.Ptr,arg_c.Ptr,arg_b.Ptr,&&arg_ldb,arg_x.Ptr,&&arg_ldx,&&arg_rcond,arg_ferr.Ptr,arg_berr.Ptr,arg_work.Ptr,arg_iwork.Ptr,&&arg_info)
  finally
    NativeUtilities.freeM arg_a
    NativeUtilities.freeM arg_af
    NativeUtilities.freeA arg_ipiv
    NativeUtilities.freeA arg_r
    NativeUtilities.freeA arg_c
    NativeUtilities.freeM arg_b
    NativeUtilities.freeM arg_x
    NativeUtilities.freeA arg_ferr
    NativeUtilities.freeA arg_berr
    NativeUtilities.freeA arg_work
    NativeUtilities.freeA arg_iwork
  // INFO
  match arg_info with
   | -1  -> invalid_arg "dgesvx_: fact (argument 1)"
   | -2  -> invalid_arg "dgesvx_: transx (argument 2)"
   | -3  -> invalid_arg "dgesvx_: n (argument 3)"
   | -4  -> invalid_arg "dgesvx_: nrhs (argument 4)"
   | -5  -> invalid_arg "dgesvx_: a (argument 5)"
   | -6  -> invalid_arg "dgesvx_: lda (argument 6)"
   | -7  -> invalid_arg "dgesvx_: af (argument 7)"
   | -8  -> invalid_arg "dgesvx_: ldaf (argument 8)"
   | -9  -> invalid_arg "dgesvx_: ipiv (argument 9)"
   | -10 -> invalid_arg "dgesvx_: equed (argument 10)"
   | -11 -> invalid_arg "dgesvx_: r (argument 11)"
   | -12 -> invalid_arg "dgesvx_: c (argument 12)"
   | -13 -> invalid_arg "dgesvx_: b (argument 13)"
   | -14 -> invalid_arg "dgesvx_: ldb (argument 14)"
   | -15 -> invalid_arg "dgesvx_: x (argument 15)"
   | -16 -> invalid_arg "dgesvx_: ldx (argument 16)"
   | -17 -> invalid_arg "dgesvx_: rcond (argument 17)"
   | -18 -> invalid_arg "dgesvx_: ferr (argument 18)"
   | -19 -> invalid_arg "dgesvx_: berr (argument 19)"
   | -20 -> invalid_arg "dgesvx_: work (argument 20)"
   | -21 -> invalid_arg "dgesvx_: iwork (argument 21)"
   | -22 -> invalid_arg "dgesvx_: info (argument 22)"
   | 0   -> ()
   | n   -> failwith (sprintf "dgesvx_ : returned %d. The computation failed." n)
  // fixups
  let a = Matrix.transpose a
  let af = Matrix.transpose af
  let b = Matrix.transpose b
  let x = Matrix.transpose x
  // result tuple
  a,af,ipiv,arg_equed,r,c,b,x,arg_rcond,ferr,berr


///Cholesky Factorisation - Expert
 member this.dposvx_((a:matrix),(b:matrix)) = 
  // input copies
  let a = Matrix.copy a
  let b = Matrix.copy b
  // dimensions
  let n = NativeUtilities.matrixDim1 a in
  NativeUtilities.assertDimensions "dposvx_" ("n","Dim2(a)") (n,NativeUtilities.matrixDim2 a);
  NativeUtilities.assertDimensions "dposvx_" ("n","Dim1(b)") (n,NativeUtilities.matrixDim1 b);
  let nrhs = NativeUtilities.matrixDim2 b in
  // allocate results
  let af = Matrix.zero (n) (n)
  let s = Array.zeroCreate  (n)
  let x = Matrix.zero (n) (nrhs)
  let ferr = Array.zeroCreate  (nrhs)
  let berr = Array.zeroCreate  (nrhs)
  let work = Array.zeroCreate  (3*n)
  let iwork = Array.zeroCreate  (n)
  // transpose
  let a = Matrix.transpose a
  let af = Matrix.transpose af
  let b = Matrix.transpose b
  let x = Matrix.transpose x
  // setup actuals
  let mutable arg_fact = 'E'
  let mutable arg_uplo = 'U'
  let mutable arg_n = n
  let mutable arg_nrhs = nrhs
  let arg_a = NativeUtilities.pinM a
  let mutable arg_lda = max 1 n
  let arg_af = NativeUtilities.pinM af
  let mutable arg_ldaf = max 1 n
  let mutable arg_equed = 'n'
  let arg_s = NativeUtilities.pinA s
  let arg_b = NativeUtilities.pinM b
  let mutable arg_ldb = max 1 n
  let arg_x = NativeUtilities.pinM x
  let mutable arg_ldx = max 1 n
  let mutable arg_rcond = 0.0
  let arg_ferr = NativeUtilities.pinA ferr
  let arg_berr = NativeUtilities.pinA berr
  let arg_work = NativeUtilities.pinA work
  let arg_iwork = NativeUtilities.pinA iwork
  let mutable arg_info = 0
  // call function
  try
    LapackMKLStubs.dposvx_(&&arg_fact,&&arg_uplo,&&arg_n,&&arg_nrhs,arg_a.Ptr,&&arg_lda,arg_af.Ptr,&&arg_ldaf,&&arg_equed,arg_s.Ptr,arg_b.Ptr,&&arg_ldb,arg_x.Ptr,&&arg_ldx,&&arg_rcond,arg_ferr.Ptr,arg_berr.Ptr,arg_work.Ptr,arg_iwork.Ptr,&&arg_info)
  finally
    NativeUtilities.freeM arg_a
    NativeUtilities.freeM arg_af
    NativeUtilities.freeA arg_s
    NativeUtilities.freeM arg_b
    NativeUtilities.freeM arg_x
    NativeUtilities.freeA arg_ferr
    NativeUtilities.freeA arg_berr
    NativeUtilities.freeA arg_work
    NativeUtilities.freeA arg_iwork
  // INFO
  match arg_info with
   | -1  -> invalid_arg "dposvx_: fact (argument 1)"
   | -2  -> invalid_arg "dposvx_: uplo (argument 2)"
   | -3  -> invalid_arg "dposvx_: n (argument 3)"
   | -4  -> invalid_arg "dposvx_: nrhs (argument 4)"
   | -5  -> invalid_arg "dposvx_: a (argument 5)"
   | -6  -> invalid_arg "dposvx_: lda (argument 6)"
   | -7  -> invalid_arg "dposvx_: af (argument 7)"
   | -8  -> invalid_arg "dposvx_: ldaf (argument 8)"
   | -9  -> invalid_arg "dposvx_: equed (argument 9)"
   | -10 -> invalid_arg "dposvx_: s (argument 10)"
   | -11 -> invalid_arg "dposvx_: b (argument 11)"
   | -12 -> invalid_arg "dposvx_: ldb (argument 12)"
   | -13 -> invalid_arg "dposvx_: x (argument 13)"
   | -14 -> invalid_arg "dposvx_: ldx (argument 14)"
   | -15 -> invalid_arg "dposvx_: rcond (argument 15)"
   | -16 -> invalid_arg "dposvx_: ferr (argument 16)"
   | -17 -> invalid_arg "dposvx_: berr (argument 17)"
   | -18 -> invalid_arg "dposvx_: work (argument 18)"
   | -19 -> invalid_arg "dposvx_: iwork (argument 19)"
   | -20 -> invalid_arg "dposvx_: info (argument 20)"
   | 0   -> ()
   | n   -> failwith (sprintf "dposvx_ : returned %d. The computation failed." n)
  // fixups
  let a = Matrix.transpose a
  let af = Matrix.transpose af
  let b = Matrix.transpose b
  let x = Matrix.transpose x
  // result tuple
  a,af,arg_equed,s,b,x,arg_rcond,ferr,berr


///Cholesky factorisation of a real symmetric positive definite matrix
 member this.dpotrf_((uplo),(a:matrix)) = 
  // input copies
  let a = Matrix.copy a
  // dimensions
  let n = NativeUtilities.matrixDim1 a in
  NativeUtilities.assertDimensions "dpotrf_" ("n","Dim2(a)") (n,NativeUtilities.matrixDim2 a);
  // allocate results
  // transpose
  let a = Matrix.transpose a
  // setup actuals
  let mutable arg_uplo = uplo
  let mutable arg_n = n
  let arg_a = NativeUtilities.pinM a
  let mutable arg_lda = max 1 n
  let mutable arg_info = 0
  // call function
  try
    LapackMKLStubs.dpotrf_(&&arg_uplo,&&arg_n,arg_a.Ptr,&&arg_lda,&&arg_info)
  finally
    NativeUtilities.freeM arg_a
  // INFO
  match arg_info with
   | -1  -> invalid_arg "dpotrf_: uplo (argument 1)"
   | -2  -> invalid_arg "dpotrf_: n (argument 2)"
   | -3  -> invalid_arg "dpotrf_: a (argument 3)"
   | -4  -> invalid_arg "dpotrf_: lda (argument 4)"
   | -5  -> invalid_arg "dpotrf_: info (argument 5)"
   | 0   -> ()
   | n   -> failwith (sprintf "dpotrf_ : returned %d. The computation failed." n)
  // fixups
  let a = Matrix.transpose a
  // result tuple
  a


///LU factorisation of general matrix using partial pivoting and row interchanges
 member this.dgetrf_((a:matrix)) = 
  // input copies
  let a = Matrix.copy a
  // dimensions
  let m = NativeUtilities.matrixDim1 a in
  let n = NativeUtilities.matrixDim2 a in
  // allocate results
  let ipiv = Array.zeroCreate  (min m n)
  // transpose
  let a = Matrix.transpose a
  // setup actuals
  let mutable arg_m = m
  let mutable arg_n = n
  let arg_a = NativeUtilities.pinM a
  let mutable arg_lda = max 1 m
  let arg_ipiv = NativeUtilities.pinA ipiv
  let mutable arg_info = 0
  // call function
  try
    LapackMKLStubs.dgetrf_(&&arg_m,&&arg_n,arg_a.Ptr,&&arg_lda,arg_ipiv.Ptr,&&arg_info)
  finally
    NativeUtilities.freeM arg_a
    NativeUtilities.freeA arg_ipiv
  // INFO
  match arg_info with
   | -1  -> invalid_arg "dgetrf_: m (argument 1)"
   | -2  -> invalid_arg "dgetrf_: n (argument 2)"
   | -3  -> invalid_arg "dgetrf_: a (argument 3)"
   | -4  -> invalid_arg "dgetrf_: lda (argument 4)"
   | -5  -> invalid_arg "dgetrf_: ipiv (argument 5)"
   | -6  -> invalid_arg "dgetrf_: info (argument 6)"
   | 0   -> ()
   | n   -> failwith (sprintf "dgetrf_ : returned %d. The computation failed." n)
  // fixups
  let a = Matrix.transpose a
  // result tuple
  a,ipiv


///QR Factorisation
 member this.dgeqrf_((a:matrix)) = 
  // input copies
  let a = Matrix.copy a
  // dimensions
  let m = NativeUtilities.matrixDim1 a in
  let n = NativeUtilities.matrixDim2 a in
  // allocate results
  let tau = Array.zeroCreate  (min m n)
  let work = Array.zeroCreate  (1)
  // transpose
  let a = Matrix.transpose a
  // setup actuals
  let mutable arg_m = m
  let mutable arg_n = n
  let arg_a = NativeUtilities.pinM a
  let mutable arg_lda = max 1 m
  let arg_tau = NativeUtilities.pinA tau
  let arg_work = NativeUtilities.pinA work
  let mutable arg_lwork = -1
  let mutable arg_info = 0
  // ask for work array size
  try
    LapackMKLStubs.dgeqrf_(&&arg_m,&&arg_n,arg_a.Ptr,&&arg_lda,arg_tau.Ptr,arg_work.Ptr,&&arg_lwork,&&arg_info)
  finally
    NativeUtilities.freeA arg_work
  if arg_info = 0   || arg_info=(-7) then
    arg_lwork <- int32 work.[0]
  else assert(false)
  let arg_work = NativeUtilities.pinA (Array.zeroCreate arg_lwork : float[])
  // call function
  try
    LapackMKLStubs.dgeqrf_(&&arg_m,&&arg_n,arg_a.Ptr,&&arg_lda,arg_tau.Ptr,arg_work.Ptr,&&arg_lwork,&&arg_info)
  finally
    NativeUtilities.freeM arg_a
    NativeUtilities.freeA arg_tau
    NativeUtilities.freeA arg_work
  // INFO
  match arg_info with
   | -1  -> invalid_arg "dgeqrf_: m (argument 1)"
   | -2  -> invalid_arg "dgeqrf_: n (argument 2)"
   | -3  -> invalid_arg "dgeqrf_: a (argument 3)"
   | -4  -> invalid_arg "dgeqrf_: lda (argument 4)"
   | -5  -> invalid_arg "dgeqrf_: tau (argument 5)"
   | -6  -> invalid_arg "dgeqrf_: work (argument 6)"
   | -7  -> invalid_arg "dgeqrf_: lwork (argument 7)"
   | -8  -> invalid_arg "dgeqrf_: info (argument 8)"
   | 0   -> ()
   | n   -> failwith (sprintf "dgeqrf_ : returned %d. The computation failed." n)
  // fixups
  let a = Matrix.transpose a
  // result tuple
  a,tau


///Matrix Vector Multiplication
 member this.dgemv_((a:matrix),(x:vector)) = 
  // input copies
  let a = Matrix.copy a
  let x = Vector.copy x
  // dimensions
  let m = NativeUtilities.matrixDim1 a in
  let n = NativeUtilities.matrixDim2 a in
  NativeUtilities.assertDimensions "dgemv_" ("n","Dim(x)") (n,NativeUtilities.vectorDim  x);
  // allocate results
  let y = Vector.zero (m)
  // transpose
  let a = Matrix.transpose a
  // setup actuals
  let mutable arg_trans = 'n'
  let mutable arg_m = m
  let mutable arg_n = n
  let mutable arg_alpha = 1.0
  let arg_a = NativeUtilities.pinM a
  let mutable arg_lda = max 1 m
  let arg_x = NativeUtilities.pinV x
  let mutable arg_incx = 1
  let mutable arg_beta = 1.0
  let arg_y = NativeUtilities.pinV y
  let mutable arg_incx = 1
  // call function
  try
    LapackMKLStubs.dgemv_(&&arg_trans,&&arg_m,&&arg_n,&&arg_alpha,arg_a.Ptr,&&arg_lda,arg_x.Ptr,&&arg_incx,&&arg_beta,arg_y.Ptr,&&arg_incx)
  finally
    NativeUtilities.freeM arg_a
    NativeUtilities.freeV arg_x
    NativeUtilities.freeV arg_y
  // INFO
  // fixups
  // result tuple
  y


///EigenValues and Eigen Vectors for nonsymetruc matrices
 member this.dggev_((a:matrix),(b:matrix)) = 
  // input copies
  let a = Matrix.copy a
  let b = Matrix.copy b
  // dimensions
  let n = NativeUtilities.matrixDim1 a in
  NativeUtilities.assertDimensions "dggev_" ("n","Dim2(a)") (n,NativeUtilities.matrixDim2 a);
  NativeUtilities.assertDimensions "dggev_" ("n","Dim1(b)") (n,NativeUtilities.matrixDim1 b);
  NativeUtilities.assertDimensions "dggev_" ("n","Dim2(b)") (n,NativeUtilities.matrixDim2 b);
  // allocate results
  let alphar = Array.zeroCreate  (n)
  let alphai = Array.zeroCreate  (n)
  let beta = Array.zeroCreate  (n)
  let vl = Array2D.zeroCreate (n) (n)
  let vr = Array2D.zeroCreate (n) (n)
  let work = Array.zeroCreate  ((8*n))
  // transpose
  let a = Matrix.transpose a
  let b = Matrix.transpose b
  // setup actuals
  let mutable arg_jobvl = 'N'
  let mutable arg_jobvr = 'V'
  let mutable arg_n = n
  let arg_a = NativeUtilities.pinM a
  let mutable arg_lda = max 1 n
  let arg_b = NativeUtilities.pinM b
  let mutable arg_ldb = max 1 n
  let arg_alphar = NativeUtilities.pinA alphar
  let arg_alphai = NativeUtilities.pinA alphai
  let arg_beta = NativeUtilities.pinA beta
  let arg_vl = NativeUtilities.pinA2 vl
  let mutable arg_ldvl = n
  let arg_vr = NativeUtilities.pinA2 vr
  let mutable arg_ldvr = n
  let arg_work = NativeUtilities.pinA work
  let mutable arg_lwork = (8*n)
  let mutable arg_info = 0
  // call function
  try
    LapackMKLStubs.dggev_(&&arg_jobvl,&&arg_jobvr,&&arg_n,arg_a.Ptr,&&arg_lda,arg_b.Ptr,&&arg_ldb,arg_alphar.Ptr,arg_alphai.Ptr,arg_beta.Ptr,arg_vl.Ptr,&&arg_ldvl,arg_vr.Ptr,&&arg_ldvr,arg_work.Ptr,&&arg_lwork,&&arg_info)
  finally
    NativeUtilities.freeM arg_a
    NativeUtilities.freeM arg_b
    NativeUtilities.freeA arg_alphar
    NativeUtilities.freeA arg_alphai
    NativeUtilities.freeA arg_beta
    NativeUtilities.freeA2 arg_vl
    NativeUtilities.freeA2 arg_vr
    NativeUtilities.freeA arg_work
  // INFO
  match arg_info with
   | -1  -> invalid_arg "dggev_: jobvl (argument 1)"
   | -2  -> invalid_arg "dggev_: jobvr (argument 2)"
   | -3  -> invalid_arg "dggev_: n (argument 3)"
   | -4  -> invalid_arg "dggev_: a (argument 4)"
   | -5  -> invalid_arg "dggev_: lda (argument 5)"
   | -6  -> invalid_arg "dggev_: b (argument 6)"
   | -7  -> invalid_arg "dggev_: ldb (argument 7)"
   | -8  -> invalid_arg "dggev_: alphar (argument 8)"
   | -9  -> invalid_arg "dggev_: alphai (argument 9)"
   | -10 -> invalid_arg "dggev_: beta (argument 10)"
   | -11 -> invalid_arg "dggev_: vl (argument 11)"
   | -12 -> invalid_arg "dggev_: ldvl (argument 12)"
   | -13 -> invalid_arg "dggev_: vr (argument 13)"
   | -14 -> invalid_arg "dggev_: ldvr (argument 14)"
   | -15 -> invalid_arg "dggev_: work (argument 15)"
   | -16 -> invalid_arg "dggev_: lwork (argument 16)"
   | -17 -> invalid_arg "dggev_: info (argument 17)"
   | 0   -> ()
   | n   -> failwith (sprintf "dggev_ : returned %d. The computation failed." n)
  // fixups
  let a = Matrix.transpose a
  let b = Matrix.transpose b
  // result tuple
  a,b,alphar,alphai,beta,vr

 end
end
module LapackMKL = begin
 let MKLProvider = new Microsoft.FSharp.Math.Experimental.Provider<_>("MKL",[|"mkl_def.dll";"mkl_lapack.dll"|],fun () -> new LapackMKLService() :> ILapack)
end
