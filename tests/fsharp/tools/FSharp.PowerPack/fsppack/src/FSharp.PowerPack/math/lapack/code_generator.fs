// CONTENTS-INDEX-REGEXP = FROM>^.?.?! <TO
//----------------------------------------------------------------------------
//CONTENTS-START-LINE: HERE=2 SEP=2
// 24.    argument specification
// 48.    generator
// 335.   specifications
// 457.   specifications list
// 482.   generate netlib and MKL bindings
//CONTENTS-END-LINE:
//----------------------------------------------------------------------------

#nowarn "51"

open System
open System.Runtime.InteropServices
open Microsoft.FSharp.NativeInterop
open Microsoft.FSharp.Math

// Developed by Can Erten.

//----------------------------------------------------------------------------
//! argument specification
//----------------------------------------------------------------------------

type Arg =
  // The first string is always the name of the argument.
  | Dim    of string                            // "m"
  | Matrix of string * string * string          // "a","m","k"
  | Vector of string * string                   // "v","k"
  | HW     of string * string                   // "lda","k"          -- HW meaning HardWired
  | Array  of string * string                   // ipiv,n             -- also local workspace
  | Array2 of string * string * string          // worko, "m", "n"    -- 2 dimension array with length m and n
  | Out    of Arg
  | In     of Arg
  | InOut  of Arg
  | Info   of string                            // "info"
  | UserDefined of string                       // "joleft" -> it will have its own data type
  | Trans  of string * string option            // "transId", (None, 'n' or (Some "a", 't' and no need to fix up matrix "a"))
  | Work   of  Arg * string        // "var NAME   ARRAY - SIZE....  // TYPE!!!!"
  // double *work, int *lwork,

let mapLeafArg f = function Out arg -> f arg | InOut arg -> f arg | arg -> f arg


//----------------------------------------------------------------------------
//! generator
//----------------------------------------------------------------------------

open System.Collections.Generic
open System.IO

let mutable sw = null : StreamWriter
let printf fmt = fprintf (sw :> TextWriter) fmt
let start path = sw <- new StreamWriter(path:string); Printf.printf "Writing %s\n" path
let stop()  = sw.Close()

// let printf fmt = Printf.printf fmt

let genFormal arg = 
  let inputMatcher p = 
    match p with 
             | UserDefined(a) -> sprintf "(%s)"  a
             | Matrix (a,m,n) -> sprintf "(%s:matrix)" a
             | Vector (v,m)   -> sprintf "(%s:vector)" v
             | Array (a,b)    -> sprintf "(%s)" a
             | _ -> assert (false)  
  match arg with
    | In p1    -> Some (inputMatcher p1)
    | InOut p1 -> Some (inputMatcher p1)
    | _        -> None
  //////////////////////////////////////////////
  
// This is overkill:
let genInputCopies arg = 
    let fixInput arg = 
        match arg with 
        | Matrix (a,_,_)  -> printf "  let %s = Matrix.copy %s\n" a a
        | Vector(a,_)     -> printf "  let %s = Vector.copy %s\n" a a
        | Array(a,_)      -> printf "  let %s = Array.copy %s\n" a a
        | Array2(a,_,_)   -> printf "  let %s = Array2D.Copy %s\n" a a
        | UserDefined (_) -> ()
        | _ -> assert false
    match arg with
    | In a  | InOut a -> fixInput a
    | _ -> () 
  
let rec genDimension f (knownDims:Dictionary<string,int>) arg =
  let isExprNotId (str:string) = str.Contains(" ") // e.g. dgels function has a "max m n" dimension
  let setDimension d (expr,exprDescription) =
    if isExprNotId d then
      // Some dimensions are related to other (prior) dims by formula. Assert it.
      printf "  NativeUtilities.assertDimensions \"%s\" (\"%s\",\"%s\") (%s,%s);\n" f d exprDescription d expr
      //printf "  assert(%s = %s);\n" d expr
    else
      if knownDims.ContainsKey(d) then
        printf "  NativeUtilities.assertDimensions \"%s\" (\"%s\",\"%s\") (%s,%s);\n" f d exprDescription d expr
        //printf "  assert(%s = %s);\n" d expr
      else
        printf "  let %s = %s in\n" d expr
        knownDims.[d] <- 1
  match arg with
  | Dim n                      -> ()
  | Matrix (a,m,n)             -> setDimension m (sprintf "NativeUtilities.matrixDim1 %s" a,sprintf "Dim1(%s)" a);
                                  setDimension n (sprintf "NativeUtilities.matrixDim2 %s" a,sprintf "Dim2(%s)" a);
  | Vector (v,m)               -> setDimension m (sprintf "NativeUtilities.vectorDim  %s" v,sprintf "Dim(%s)"  v);
  | HW _                       -> ()
  | Trans _                    -> ()
  | Out _                      -> ()
  | InOut p                    -> genDimension f knownDims p
  | In p                       -> genDimension f knownDims p
  | _ -> ()
  
let rec genAllocResult arg = 
  match arg with
  | Matrix (a,m,n)      -> printf "  let %s = Matrix.zero (%s) (%s)\n" a m n
  | Array (ip, n)       -> printf "  let %s = Array.zeroCreate  (%s)\n" ip n
  | Array2(work, ld, n) -> printf "  let %s = Array2D.zeroCreate (%s) (%s)\n" work ld n
  | Vector (v,m)        -> printf "  let %s = Vector.zero (%s)\n"    v m
  | Work (p,_)              -> genAllocResult p
  | Out p -> genAllocResult p
  | _ -> ()
 
let preFixups args = 
  // Matrices that must be transposed prior to calling.
  let selectTransArgs  = function Trans (tv,Some a) -> Some a | _ -> None
  let selectMatrixArgs = mapLeafArg (function Matrix (a,m,n) | In(Matrix (a,m,n)) | InOut(Matrix (a,m,n)) -> Some a |  _ -> None)
  let matrices  = List.choose  selectMatrixArgs args |> Set.ofList
  let transArgs = List.choose selectTransArgs  args |> Set.ofList
  Set.diff matrices transArgs

let postFixups args = 
  // Matrices that must be transposed prior to calling.
  let selectTransArgs = function Trans (tv,Some a) -> Some a | _ -> None
  let selectMatrixOut = function Out (Matrix (a,m,n)) -> Some a | InOut (Matrix (a,m,n)) -> Some a | _ -> None
  let matricesOut = List.choose selectMatrixOut args |> Set.ofList
  let transArgs    = List.choose selectTransArgs args |> Set.ofList
  Set.diff matricesOut transArgs

let rec genTransposeFixup fixups arg = 
  match arg with
    | Matrix(a,m,n) when Set.contains a fixups -> printf "  let %s = Matrix.transpose %s\n" a a
    | Out arg -> genTransposeFixup fixups arg
    | InOut arg -> genTransposeFixup fixups arg
    | In arg -> genTransposeFixup fixups arg
    | _ -> ()

let rec genActualDef arg = 
  match arg with
  | Dim n                  -> printf "  let mutable arg_%s = %s\n" n n
  | Matrix (a,m,n)         -> printf "  let arg_%s = NativeUtilities.pinM %s\n" a a
  | Vector (v,m)           -> printf "  let arg_%s = NativeUtilities.pinV %s\n" v v
  | Array(a,i)             -> printf "  let arg_%s = NativeUtilities.pinA %s\n" a a
  | Array2(a,m,n)          -> printf "  let arg_%s = NativeUtilities.pinA2 %s\n" a a
  | Trans (tv,opt)         -> printf "  let mutable arg_%s = '%s'\n" tv (if opt.IsSome then "t" else "n")
  | UserDefined(s)         -> printf "  let mutable arg_%s = %s\n" s s
  | HW (n,x)               -> printf "  let mutable arg_%s = %s\n" n x
  | Info(n)                -> printf "  let mutable arg_%s = 0\n" n
  
  | Work (p,_)             -> genActualDef p  
  | Out p                  -> genActualDef p
  | InOut p                -> genActualDef p
  | In p                   -> genActualDef p
  
let rec genActualArg arg = 
  match arg with
  | Dim n                  -> sprintf "&&arg_%s" n
  | Matrix (a,m,n)         -> sprintf "arg_%s.Ptr" a
  | Vector (v,m)           -> sprintf "arg_%s.Ptr" v
  | Array (a,i)            -> sprintf "arg_%s.Ptr" a
  | Array2 (a,i,p)         -> sprintf "arg_%s.Ptr" a
  | HW (n,x)               -> sprintf "&&arg_%s" n
  | Trans (tv,x)           -> sprintf "&&arg_%s" tv
  | Info (m)               -> sprintf "&&arg_%s" m
  | UserDefined(s)         -> sprintf "&&arg_%s" s
  
  | Work (p,_) -> genActualArg p
  | Out p -> genActualArg p
  | InOut p ->  genActualArg p
  | In  p   ->  genActualArg p

let genWorkWorkaround f spec moduleName=
    //find work args  Array(a,i)     ->      printf    "    NativeUtilities.freeA arg_%s\n" a
    let selectWorkArrayName  = function Work (Array (aName,_),typ) -> Some(aName,typ) | _ -> None
    let selectWorkHWSizeName = function Work (HW (sizeName,_),_) -> Some(sizeName) | _ -> None
    
    
    let woNameList = List.choose selectWorkArrayName spec
    let woSizeList = List.choose selectWorkHWSizeName spec
    
  
    assert (woNameList.Length = woSizeList.Length)
    
    if woNameList.Length >0 then
        // filteri
 
        // Note: not tail recursive
        let findall_i f xs =
            let rec aux i xs = match xs with [] -> [] | y::ys -> if f i y then i :: aux (i+1) ys else aux (i+1) ys  
            aux 0 xs
                
        let sizeIndexList = findall_i (fun i x -> match x with | Work((HW _),_) -> true | _ -> false) spec
        printf "  // ask for work array size\n"
        printf "  try\n";
        printf "    %s.%s(%s)\n" moduleName f (String.Join(",", Array.ofList (List.map genActualArg spec)));
        printf "  finally\n";
        
        // freeing all!!! start from here
        
        List.iter (fun x ->  printf "    NativeUtilities.freeA arg_%s\n" (fst x)) woNameList

        printf "  if arg_info = 0  "
        // this is returns +1 value be careful!!! it#s modified..
        List.iter (fun (x : int) -> printf " or arg_info=(-%s) " ((x+1).ToString())) sizeIndexList
        printf "then\n"
        
        List.iter2
           (fun s n -> 
                match (snd n) with
                | "f" -> printf "    arg_%s <- int32 %s.[0]\n" s (fst n)
                | "i" -> printf "    arg_%s <-  %s.[0]\n" s (fst n)
                | _ -> assert(false) ) woSizeList woNameList
        
        printf "  else assert(false)\n"
        
        
        List.iter2
           (fun n s -> 
                match (snd n) with
                | "f" -> printf "  let arg_%s = NativeUtilities.pinA (Array.zeroCreate arg_%s : float[])\n" (fst n) s
                | "i" -> printf "  let arg_%s = NativeUtilities.pinA (Array.zeroCreate arg_%s : int[])\n" (fst n) s
                | _ -> assert(false) )   woNameList woSizeList 

let  genInfo f spec = 
    let rec genInfoHandle i arg = 
        let argPrint i arg = printf "   | -%-2d -> invalid_arg \"%s: %s (argument %d)\"\n" (i+1) f arg (i+1)
        match arg with
        | Dim n               -> argPrint i n
        | Matrix (a,_,_)      -> argPrint i a
        | Vector (v,_)        -> argPrint i v
        | Array (a,_)         -> argPrint i a
        | Array2 (a,_,_)      -> argPrint i a
        | HW (n,x)            -> argPrint i n
        | Trans (tv,x)        -> argPrint i tv
        | Info (m)            -> argPrint i m
        | UserDefined(s)      -> argPrint i s
        
        | Work (p,_)          -> genInfoHandle i p
        | Out p               -> genInfoHandle i p
        | InOut p             -> genInfoHandle i p
        | In p                -> genInfoHandle i p
      // if info exist
    //let info = try Some (List.find(fun x -> match x with  | Info _ -> true;  | _ -> false ) spec)
      //         with Not_found -> None
               
    //let info2 = List.tryFind (fun x -> match x with | Info _ -> true | _ -> false) spec
    let info = List.tryFind (function Info _ -> true | _ -> false) spec
      
    if (info.IsSome) then     
      let name = match info.Value with | Info(s) -> s | _ -> assert(false)
      printf "  match arg_%s with\n" name
      List.iteri genInfoHandle spec
      printf "   | 0   -> ()\n" 
      printf "   | n   -> failwith (sprintf \"%s : returned %%d. The computation failed.\" n)\n" f
    ()

let rec genActualDispose arg = 
  match arg with
  | Dim n                -> ()
  | Matrix (a,m,n)       -> printf "    NativeUtilities.freeM arg_%s\n" a
  | Vector (v,m)         -> printf "    NativeUtilities.freeV arg_%s\n" v
  | Array(a,i)           -> printf "    NativeUtilities.freeA arg_%s\n" a
  | Array2(a,i,j)        -> printf "    NativeUtilities.freeA2 arg_%s\n" a
  | HW (n,x)             -> ()
  | Trans (tv,x)         -> ()
  | Info _ -> ()
  | UserDefined(s) -> ()
  | Work (p,_) -> genActualDispose p
  | InOut p -> genActualDispose p
  | Out p -> genActualDispose p
  | In p   -> genActualDispose p
  
let genResult arg = 
  match arg with
  | Out (Matrix (a,m,n)) -> Some (sprintf "%s" a)
  | Out (Vector (v,m))   -> Some (sprintf "%s" v)
  | Out (Array (a,_))    -> Some (sprintf "%s" a)
  | Out (Array2 (a,_,_)) -> Some (sprintf "%s" a)
  | Out (HW(a,_))        -> Some (sprintf "arg_%s" a)
  | InOut (Matrix (a,m,n)) -> Some (sprintf "%s" a)
  | InOut (Vector (v,m))   -> Some (sprintf "%s" v)
  | InOut (Array (a,_))    -> Some (sprintf "%s" a)
  | InOut (Array2 (a,_,_)) -> Some (sprintf "%s" a)
  | Out _ -> assert false
  | InOut _ -> assert false
  | _ -> None

let genFunction (f,spec) moduleName= 
  printf "\n";
  let args = List.choose genFormal spec
  printf " member this.%s(%s) = \n" f (String.Join(",",Array.ofList args))
  
  printf "  // input copies\n";
  List.iter genInputCopies spec
  
  printf "  // dimensions\n";
  let knownDims = new System.Collections.Generic.Dictionary<_,_>()
  List.iter (genDimension f knownDims) spec;
  printf "  // allocate results\n";
  List.iter genAllocResult spec;
  printf "  // transpose\n";
  List.iter (genTransposeFixup (preFixups spec)) spec
  printf "  // setup actuals\n";
  List.iter genActualDef spec;
  
   // work call if work exist
  genWorkWorkaround f spec moduleName
  /////////////////
  printf "  // call function\n"
  printf "  try\n";
  printf "    %s.%s(%s)\n" moduleName f (String.Join(",", Array.ofList (List.map genActualArg spec)));
  printf "  finally\n";
  List.iter genActualDispose spec;

  printf "  // INFO\n"
  genInfo f spec
  
  printf "  // fixups\n";
  List.iter (genTransposeFixup (postFixups spec)) spec   
  printf "  // result tuple\n";
  printf "  %s\n" (String.Join(",", Array.ofList (List.choose genResult spec)));
  printf "\n";


//----------------------------------------------------------------------------
//! specifications
//----------------------------------------------------------------------------
    
let sl = "Lapack"
let sb = "Blas"

let fspec_dgemm =
  "Matrix-Matrix Multiplication", sb, "dgemm_",
  [Trans ("transa",Some "a");Trans ("transb",Some "b");Dim "m";Dim "n";Dim "k";
   HW ("alpha","1.0");In (Matrix("a","m","k"));HW ("ldk","k");In (Matrix("b","k","n"));
   HW ("ldn","n");HW ("beta","1.0");Out (Matrix("c","m","n"));HW ("ldm","m")]

let fspec_dgesv =
  "Solve", sl ,"dgesv_" ,
  [Dim "n"; Dim "NRHS"; InOut (Matrix("a","n","n")); HW("lda","max 1 n");Out (Array("ipiv", "n")); 
   InOut (Matrix("b", "n", "NRHS")); HW("ldb","max 1 n"); Info "info"  ]       

let fspec_dtrsv =
  "Solve dtrsv", sb ,"dtrsv_" ,
  [In (UserDefined("uplo")); 
   Trans ("transa",Some "a");
   HW ("diag","'N'");
   Dim "n";
   In (Matrix("a","n","n")); HW("lda","max 1 n");   
   InOut (Vector("x", "n")); HW("incx","1"); ]       

let fspec_dtrsm =
  "Solve dtrsm", sb ,"dtrsm_" ,
  [HW ("side","'L'");
   In (UserDefined("uplo")); 
   Trans ("transa",Some "a");
   HW ("diag","'N'");
   Dim "m";
   Dim "n";
   HW ("alpha","1.0");   
   In    (Matrix("a","m","k")); HW("lda","m"); // assumes side=L
   InOut (Matrix("b","m","n")); HW("ldb","m"); // assumes side=L
  ]       

// work is problematic with lapack, intel's OK
let fspec_dgglse =
  "Solve LSE using GRQ", sl, "dgglse_",
  [Dim "m"; Dim "n"; Dim "p"; InOut (Matrix("a","m","n")); HW("lda","max 1 m"); In (Matrix("b","p","n"));
   HW("ldb","max 1 p"); InOut (Vector("c","m")); In(Vector("d","p")); Out (Array("x","n"));
   Work (Array("work","1"),"f"); Work (HW("lwork","-1"),"i"); Info "info" ]
             
let fspec_dgeev =
  "EigenValue Non-Symetrix" , sl , "dgeev_",
  [HW("jobvl","'N'");In(UserDefined("jobvr")); //In (HW("jobvl"));In (UserDefined("jobvr"))  ;
   Dim "n";  In (Matrix("a","n","n")) ;     HW("lda","n"); 
   Out (Array("wr","n"));Out (Array ("wi","n"));
   Array2("vl","n","n");    (HW("ldvl","n")); 
   Out (Array2("vr","n","n"));   (HW("ldvr","n")); 
   Array("work","(4*n)"); HW("lwork","(4*n)");
   Info("info")]
                            
let fspec_dposv =
  "Solve Cholesky", sl,  "dposv_",
  [HW("uplo","'U'"); Dim "n"; Dim "nrhs"; InOut (Matrix("a","n","n")); HW("lda","max 1 n"); InOut(Matrix("b","n","nrhs"));
   HW("ldb","max 1 n");Info "info" ]     
                        
let fspec_dgels =
  "Solve Upper" , sl, "dgels_",
  [Trans ("transa",None) ; Dim "m"; Dim "n"; Dim "nrhs";
   InOut  (Matrix("a","m","n"));               HW("lda","max 1 m") ;InOut  (Matrix ("b","(max m n)", "nrhs"));
   HW("ldb","max m (max 1 n)") ; Work (Array("work","1"),"f"); Work (HW("lwork","-1"),"i");Info("info")]
   
       
let fspec_dsyev =
  "Eigen Value of Symetric Matrix",sl, "dsyev_",
  [In (UserDefined("jobz")); In (UserDefined("uplo")); Dim "n"; InOut (Matrix("a","n","n")); HW("lda","max 1 n");
   Out (Array("w","n")); Work (Array("work","1"),"f"); Work (HW("lwork", "-1"),"i"); Info ("info")]

let fspec_dsyevd =
  "Eigen Value of Symetric Matrix - Divide and Conquer",sl, "dsyevd_",
  [In (UserDefined("jobz")); In (UserDefined("uplo")); Dim "n"; InOut (Matrix("a","n","n")); HW("lda","max 1 n");
   Out (Array("w","n")); Work (Array("work","1"),"f");    Work (HW("lwork", "-1"),"i");
   Work (Array("iwork","1"),"i"); Work (HW("liwork", "-1"),"i");Info ("info") ]

let fspec_dgesvd =
  "Singular Value Decomposition", sl,"dgesvd_",
  [HW("jobu","'A'"); HW("jobvt","'A'"); Dim "m"; Dim "n";
   In (Matrix ("a","m","n")); HW("lda","max 1 m");
   Out (Array("s","min m n"));
   Out (Matrix("u","m","min m n"));  HW("ldu","m"); 
   Out (Matrix("vt","n","n"));       HW("ldvt","n");
   Work (Array("work","1"),"f"); 
   Work (HW("lwork", "-1"),"i");
   Info ("info") ]

let fspec_dgesdd =
  "Singular Value Decomposition Divide- Conquer", sl, "dgesdd_",
  [HW("JOBZ","'A'");  Dim "m"; Dim "n"; 
   In (Matrix ("a","m","n")); HW("lda","max 1 m");
   Out (Array("s","min m n"));
   Out (Matrix("u" ,"m","m")); HW("ldu" ,"m");
   Out (Matrix("vt","n","n")); HW("ldvt","n");
   Work (Array("work","1"),"f");
   Work (HW("lwork", "-1"),"i");
   Array("iwork","8*(min m n)");  
   Info ("info") ]

let fspec_dsygv_ =
  "Single Value Decomposition for Symetric Matrices", sl, "dsygv_",
  [HW("itype","1");HW("JOBZ","'V'");HW("uplo","'U'");Dim "n"; InOut (Matrix ("a","n","n"));HW("lda","max 1 n");InOut (Matrix ("b","n","n"));
   HW("ldb","max 1 n"); Out(Array("w","n")); Work (Array("work","1"),"f"); Work (HW("lwork", "-1"),"i");Info ("info") ]
 
let fspec_dsygvd_ =
  "Single Value Decomposition for Symetric Matrices Divide and Conquer", sl,"dsygvd_",
  [HW("itype","1");HW("JOBZ","'V'");HW("uplo","'U'");Dim "n"; InOut (Matrix ("a","n","n"));HW("lda","max 1 n");InOut (Matrix ("b","n","n"));
   HW("ldb","max 1 n"); Out(Array("w","n")); Work (Array("work","1"),"f"); Work (HW("lwork", "-1"),"i"); Work (Array("iwork","1"),"i"); Work (HW("liwork", "-1"),"i");Info ("info") ]
 
let fspec_dgesvx_ =
  "LU factorization to compute the solution to a real  system of linear equations ", sl, "dgesvx_",
  [HW("fact","'E'");Trans ("transx",None);Dim "n";Dim "nrhs";InOut (Matrix ("a","n","n"));HW("lda","max 1 n");
   Out (Matrix("af","n","n"));HW("ldaf","max 1 n");Out (Array("ipiv", "n")); Out (HW("equed","'n'")); Out (Array("r","n"));
   Out (Array("c","n")); InOut (Matrix ("b","n","nrhs"));HW("ldb","max 1 n"); Out( Matrix("x","n","nrhs")); HW("ldx","max 1 n"); 
   Out (HW("rcond","0.0")); Out (Array("ferr","nrhs")); Out (Array("berr","nrhs")); Array ("work", "4*n"); Array("iwork","n");Info ("info")]

let fspec_dposvx_ =
  "Cholesky Factorisation - Expert", sl , "dposvx_",
  [HW("fact","'E'"); HW("uplo","'U'");Dim "n";Dim "nrhs";InOut (Matrix ("a","n","n"));HW("lda","max 1 n"); 
   Out (Matrix("af","n","n"));HW("ldaf","max 1 n");Out (HW("equed","'n'"));  Out (Array("s","n"));
   InOut (Matrix ("b","n","nrhs"));HW("ldb","max 1 n"); Out( Matrix("x","n","nrhs")); HW("ldx","max 1 n"); 
   Out (HW("rcond","0.0")); Out (Array("ferr","nrhs")); Out (Array("berr","nrhs")); Array ("work", "3*n"); Array("iwork","n");Info ("info")]

let fspec_dpotrf =
  "Cholesky factorisation of a real symmetric positive definite matrix" , sl, "dpotrf_",
  [In(UserDefined("uplo")); Dim "n"; InOut (Matrix("a", "n","n")); HW ("lda", "max 1 n"); Info ("info")]                                            

let fspec_dgetrf =
  "LU factorisation of general matrix using partial pivoting and row interchanges" , sl, "dgetrf_",
  [ Dim "m"; Dim "n"; InOut (Matrix("a", "m","n")); HW ("lda", "max 1 m"); Out (Array("ipiv","min m n")); Info ("info")]                                            

let fspec_dgeqrf_=
  "QR Factorisation", sl, "dgeqrf_",
  [ Dim "m"; Dim "n" ;InOut (Matrix ("a", "m","n")) ;HW ("lda", "max 1 m");Out (Array("tau", "min m n"));
    Work (Array("work","1"),"f"); Work (HW("lwork", "-1"),"i"); Info ("info")]

/// Do not use trans argument here.           
let fspec_dgemv_ =
  "Matrix Vector Multiplication", sb,"dgemv_" ,
  [Trans("trans",None);Dim "m";Dim "n";HW ("alpha","1.0");In (Matrix("a", "m","n"));HW("lda","max 1 m");
   In (Vector("x", "n"));
   HW("incx","1"); HW ("beta","1.0"); Out (Vector("y", "m")); HW("incx","1")] 

// dggev_( char *jobvl, char *jobvr, int *n, double *a, int *lda, double *b, int *ldb, 
//double *alphar, double *alphai,double *beta,double *vl,int *ldvl,double *vr,int *ldvr,double *work, int *lwork,int *info);                            
let fspec_dggev =
  "EigenValues and Eigen Vectors for nonsymetruc matrices", sl, "dggev_", 
  [HW("jobvl","'N'"); HW("jobvr", "'V'");  Dim "n" ;InOut (Matrix ("a", "n","n")); HW ("lda", "max 1 n");InOut (Matrix ("b", "n","n"));
   HW ("ldb", "max 1 n"); Out (Array("alphar","n"));Out (Array("alphai","n")); Out (Array("beta","n"));
   Array2("vl","n","n");    (HW("ldvl","n")); 
   Out (Array2("vr","n","n"));   (HW("ldvr","n")); 
   Array("work","(8*n)"); HW("lwork","(8*n)");
   Info("info")]


//----------------------------------------------------------------------------
//! specifications list
//----------------------------------------------------------------------------

let fspecs = [fspec_dgemm;
              fspec_dgesv;
              fspec_dtrsv;
              fspec_dtrsm;
              fspec_dgglse;
              fspec_dgeev;
              fspec_dposv;
              fspec_dgels;
              fspec_dsyev;
              fspec_dsyevd;
              fspec_dgesvd;
              fspec_dgesdd;
              fspec_dsygv_;
              fspec_dsygvd_;
              fspec_dgesvx_;
              fspec_dposvx_;
              fspec_dpotrf;
              fspec_dgetrf;
              fspec_dgeqrf_;
              fspec_dgemv_;
              fspec_dggev;
             ]


//----------------------------------------------------------------------------
//! generate netlib and MKL bindings
//----------------------------------------------------------------------------

let produceCode moduleName (comments,(library),funcName,spec) = 
    printf "\n///%s" comments
    genFunction (funcName,spec) moduleName
    
    
let generateFile filename providerId blasDLLName lapackDLLName hyphen notices =  
    let hyphenS = "[_]"
    let hyphenReplace = if hyphen then "_" else ""
    let quoted s = "\"" ^ s ^ "\""

    let className  = sprintf "Lapack%sService" providerId
    let moduleName = sprintf "Lapack%sStubs"   providerId
    
    let blasdllX    = "[BLASDLL]"
    let lapackdllX  = "[LAPACKDLL]"
    let moduleNameX = "[MODULENAME]"
    let noticeX     = "[NOTICE]"

    let notice = String.Join("\n", Array.ofList (List.map (fun s -> "/// " ^ s) notices))
    
    start (filename)
    printfn "namespace Microsoft.FSharp.Math.Bindings.Internals"
    printfn "#nowarn \"51\""
    printfn "open Microsoft.FSharp.Math"
    printfn "open Microsoft.FSharp.Math.Bindings.Internals"
    
    let templateFile = File.ReadAllText("lapack_service_template.fs")
    printf "%s" (templateFile.Replace(blasdllX,blasDLLName)
                             .Replace(lapackdllX,lapackDLLName)
                             .Replace(moduleNameX,moduleName)
                             .Replace(hyphenS,hyphenReplace)
                             .Replace(noticeX,notice)
                )
    printfn "/// Internal provider of Lapack functionality, not for direct user usage."
    printfn "type %s() = class" className
    printfn " interface ILapack with "
    List.iter (produceCode moduleName) fspecs
    printfn " end"
    printfn "end"
    printfn "module Lapack%s = begin" providerId
    printfn " let %sProvider = new Microsoft.FSharp.Math.Experimental.Provider<_>(%s,[|%s;%s|],fun () -> new %s() :> ILapack)"
      providerId
      (quoted providerId)
      (quoted blasDLLName) (quoted lapackDLLName)
      className
    printfn "end"
    stop()
    start (filename ^ "i") 
    printfn "namespace Microsoft.FSharp.Math.Bindings.Internals"       
    printfn "module Lapack%s =" providerId
    printfn "  val %sProvider : Microsoft.FSharp.Math.Experimental.Provider<Microsoft.FSharp.Math.Bindings.Internals.ILapack>" providerId
    printfn "module %s = begin end" moduleName    
    stop()

let noticesMLK = ["Warning:";
                  "IMPORTANT WARNING NOTICE:";
                  "INTEL MATH KERNEL LIBRARY 9.1 FOR WINDOWS DOES NOT BELONG TO MICROSOFT - IT IS THIRD PARTY TECHNOLOGY.";
                  "IT IS CLEARED ONLY FOR USE BY A SPECIFIC MSR RESEARCH TEAM.";
                  "DO NOT USE IT UNTIL YOU HAVE CLEARED ITS USE FOR YOUR PROJECT WITH YOUR LEGAL CONTACT.";
                  "";
                  "The following stubs bind directly to Intel MKL functionality.";
                  "You should not use them without:";
                  "a) Intel MKL developer licenses.";
                  "b) Seeking local legal approval.";
                 ]    

do
  let filename   = "lapack_service_netlib.fs"
  let providerId = "Netlib"
  let blasdll    = "blas.dll"
  let lapackdll  = "lapack.dll"
  let typeHyphen = true  // true = underscore suffix 
  let notices    = ["Notice: This file generates bindings for netlib.org BLAS/LAPACK DLLs"] 
  generateFile filename providerId blasdll lapackdll typeHyphen notices

do
  let filename   = "lapack_service_mkl.fs"
  let providerId = "MKL"
  let blasdll    = "mkl_def.dll"
  let lapackdll  = "mkl_lapack.dll"
  let typeHyphen = false // false = no underscore suffix
  let notices    = noticesMLK
  generateFile filename providerId blasdll lapackdll typeHyphen notices


(* scratch *)
