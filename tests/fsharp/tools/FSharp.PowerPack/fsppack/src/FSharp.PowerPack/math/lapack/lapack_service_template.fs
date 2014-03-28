[NOTICE]
module [MODULENAME] = begin
  [<System.Runtime.InteropServices.DllImport(@"[BLASDLL]",EntryPoint="dgemm[_]")>]
  extern void dgemm_(char *transa, char *transb, int *m, int *n, int *k, double *alpha, double *a, int *lda, double *b, int *ldb, double *beta, double *c, int *ldc);

  [<System.Runtime.InteropServices.DllImport(@"[BLASDLL]",EntryPoint="dtrsv[_]")>]
  extern void dtrsv_(char *uplo,char *trans,char *diag,int *n,double *a,int *lda,double *x,int *incx);

  [<System.Runtime.InteropServices.DllImport(@"[BLASDLL]",EntryPoint="dtrsm[_]")>]
  extern void dtrsm_(char *side,char *uplo,char *trans,char *diag,int *m,int *n,double *alpha,double *a,int *lda,double *b,int *ldb);   
   
  [<System.Runtime.InteropServices.DllImport(@"[LAPACKDLL]",EntryPoint="dgesv[_]")>]
  extern void dgesv_(int *n, int *nrhs, double *a, int *lda, int *ipiv, double *b, int *ldb, int *info);

  [<System.Runtime.InteropServices.DllImport(@"[LAPACKDLL]",EntryPoint="dgeev[_]")>]
  extern void dgeev_(char *jobvl, char *jobvr, int *n, double *a, int *lda, double *wr, double *wi, double *vl, int *ldvl, double *vr, int *ldvr, double *work, int *lwork, int *info);
  [<System.Runtime.InteropServices.DllImport(@"[LAPACKDLL]",EntryPoint="dposv[_]")>]
  extern void dposv_(char *uplo, int *n, int *nrhs, double *a, int *lda, double *b, int *ldb, int *info);
  
  [<System.Runtime.InteropServices.DllImport(@"[LAPACKDLL]",EntryPoint="dgels[_]")>]
  extern void dgels_(char *trans, int *m,int *n, int *nrhs, double *a, int *lda, double *b, int *ldb, double *work, int *lwork, int *info);
  
  [<System.Runtime.InteropServices.DllImport(@"[LAPACKDLL]",EntryPoint="dgglse[_]")>]
  extern void dgglse_(int *m, int *n, int *p, double *a, int *lda, double *b, int *ldb, double *c, double *d, double *x, double *work, int *lwork, int *info);
  
  [<System.Runtime.InteropServices.DllImport(@"[LAPACKDLL]",EntryPoint="dsyev[_]")>]
  extern void dsyev_(char *jobz, char *uplo, int *n, double *a,int *lda, double *w, double *work, int *lwork, int *info);
  
  [<System.Runtime.InteropServices.DllImport(@"[LAPACKDLL]",EntryPoint="dsyevd[_]")>]
  extern void dsyevd_(char *jobz, char *uplo, int *n, double *a, int *lda, double *w, double *work, int *lwork, int *iwork, int *liwork, int *info);
  
  [<System.Runtime.InteropServices.DllImport(@"[LAPACKDLL]",EntryPoint="dgesvd[_]")>]
  extern void dgesvd_(char *jobu, char *jobvt, int  *m, int *n, double *a, int *lda, double *s, double *u, int *ldu, double *vt, int *ldvt, double *work, int *lwork, int *info);
  
  [<System.Runtime.InteropServices.DllImport(@"[LAPACKDLL]",EntryPoint="dgesdd[_]")>]
  extern void dgesdd_(char *JOBZ, int  *m, int *n, double *a, int *lda, double *s, double *u, int *ldu, double *vt, int *ldvt, double *work, int *lwork,int *iwork, int *info);
  
  [<System.Runtime.InteropServices.DllImport(@"[LAPACKDLL]",EntryPoint="dsygv[_]")>]
  extern void dsygv_(int *itype, char *jobz, char *uplo, int *n, double *a, int *lda, double *b, int *ldb, double *w, double *work, int *lwork, int *info);
  
  [<System.Runtime.InteropServices.DllImport(@"[LAPACKDLL]",EntryPoint="dsygvd[_]")>]     
  extern void dsygvd_(int *itype, char *jobz, char *uplo, int *n, double *a, int *lda, double *b, int *ldb, double *w, double *work, int *lwork,int *iwork, int *liwork, int *info);
  
  [<System.Runtime.InteropServices.DllImport(@"[LAPACKDLL]",EntryPoint="dggev[_]")>]     
  extern void dggev_( char *jobvl, char *jobvr, int *n, double *a, int *lda, double *b, int *ldb, double *alphar, double *alphai,double *beta,double *vl,int *ldvl,double *vr,int *ldvr,double *work, int *lwork,int *info);
  
  [<System.Runtime.InteropServices.DllImport(@"[LAPACKDLL]",EntryPoint="dgesvx[_]")>]     
  extern void dgesvx_(char *fact, char *trans, int *n, int *nrhs, double *a, int *lda, double *af, int *ldaf, int *ipiv, char *equed, double *r, double *c, double *b, int *ldb, double *x, int *ldx, double *rcond, double *ferr, double *berr, double *work, int *iwork, int *info);
  
  [<System.Runtime.InteropServices.DllImport(@"[LAPACKDLL]",EntryPoint="dposvx[_]")>]     
  extern void  dposvx_(char *fact, char *uplo, int *n, int *nrhs, double *a, int *lda, double *af, int *ldaf, char *equed, double *s, double *b, int *ldb, double *x, int *ldx, double *rcond, double  *ferr, double *berr, double *work, int *iwork, int *info);

  [<System.Runtime.InteropServices.DllImport(@"[LAPACKDLL]",EntryPoint="dpotrf[_]")>]     
  extern void  dpotrf_(char *uplo, int *n, double *a, int *lda, int *info);
  
  [<System.Runtime.InteropServices.DllImport(@"[LAPACKDLL]",EntryPoint="dgetrf[_]")>]     
  extern void  dgetrf_(int *m, int *n, double *a, int *lda, int *ipiv, int *info);
  
  [<System.Runtime.InteropServices.DllImport(@"[LAPACKDLL]",EntryPoint="dgeqrf[_]")>]     
  extern void dgeqrf_(int  *m, int *n, double *a, int *lda, double *tau, double *work, int *lwork, int *info);
  
  [<System.Runtime.InteropServices.DllImport(@"[BLASDLL]",EntryPoint="dgemv[_]")>]
  extern void dgemv_(char* trans, int* m, int* n,double* alpha, double* A, int* lda,double* x, int* incx, double* beta,double* y, int* incy);
  
end
