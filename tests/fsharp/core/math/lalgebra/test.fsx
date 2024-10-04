// #Conformance #Regression 
#if TESTS_AS_APP
module Core_math_lalgebra
#endif

#light


let failures = ref []

let report_failure (s : string) = 
    stderr.Write" NO: "
    stderr.WriteLine s
    failures := !failures @ [s]

let test (s : string) b = 
    stderr.Write(s)
    if b then stderr.WriteLine " OK"
    else report_failure (s)

#nowarn "0049";;

#r "FSharp.Math.Providers.dll"

open System
open Math
open Math.Notation
open Microsoft.FSharp.Math.Experimental.LinearAlgebra


//! TESTS HERE......
//------------------

// These are some helper routines to "roughly" test for equality.

/// This function returns a float number that is the next larger number than abs(x)
/// when represented as a float.
let eps(x) = 2.0**(-52.0 + (log (abs x))/(log 2.0))
/// This function tests for equality using relative errors.
let eq (x:float) (y:float) = if x = 0.0 && y < 1e-5 then true
                             elif y = 0.0 && x < 1e-5 then true
                             elif (abs (x-y)) <= 1e-5*abs(x) then true
                             elif (abs (x-y)) <= 1e-5*abs(y) then true
                             elif x = y then true
                             else false

/// This function tests two vectors for equality.
let eqv (x:vector) (y:vector) = Vector.foldi (fun i acc a -> acc && (eq a y.[i])) true x

/// This function tests two matrices for equality.
let eqm (x:matrix) (y:matrix) = Matrix.foldi (fun i j acc a -> acc && (eq a y.[i,j])) true x

/// Checks whether matrix A is lower triangular.
let IsLowerTriangular A = A |> Matrix.foralli (fun i j aij -> i >= j || aij = 0.0)



(*printf @"
//----------------------------------------------------------------------------
//
// FLab.LinearAlgebra
//
//  These tests generate random matrices but do not yet check whether they
//  are singular or not. Hence, there is an almost zero probability that the
//  some procedures crash because of this reason.
//
//----------------------------------------------------------------------------
"*)
let rnd = new System.Random()

let RegressionTest provider maxDimension =
    let mutable acc = true
    
    do
(*        printf @"
          SolveTriangularLinearSystem
        "*)
        acc <- true
        for n=1 to maxDimension do
            // Generate random lower triangular matrices and solutions.
            for i=1 to 10 do
                let A = Matrix.init n n (fun i j -> if i>j then rnd.NextDouble() elif i=j then 1.0 else 0.0)
                let x = Vector.init n (fun i -> rnd.NextDouble())
                let v = A*x
                let y = SolveTriangularLinearSystem A v true
                //assert(eqv x y)
                acc <- acc && (eqv x y)
            
            // Generate random upper triangular matrices and solutions.
            for i=1 to 10 do
                let A = Matrix.init n n (fun i j -> if i<j then rnd.NextDouble() elif i=j then 1.0 else 0.0)
                let x = Vector.init n (fun i -> rnd.NextDouble())
                let v = A*x
                let y = SolveTriangularLinearSystem A v false
                //assert(eqv x y)
                acc <- acc && (eqv x y)
        test (provider ^ ": SolveTriangularLinearSystem") acc

    do
(*        printf @"
          SolveTriangularLinearSystems
        "*)
        acc <- true
        for n=1 to maxDimension do
            // Generate random lower triangular matrices and solutions.
            for i=1 to 10 do
                let A = Matrix.init n n (fun i j -> if i>j then rnd.NextDouble() elif i=j then 1.0 else 0.0)
                let X = Matrix.init n n (fun i j -> rnd.NextDouble())
                let B = A*X
                let Y = SolveTriangularLinearSystems A B true
                //assert(eqm X Y)
                acc <- acc && (eqm X Y)
            
            // Generate random upper triangular matrices and solutions.
            for i=1 to 10 do
                let A = Matrix.init n n (fun i j -> if i<j then rnd.NextDouble() elif i=j then 1.0 else 0.0)
                let X = Matrix.init n n (fun i j -> rnd.NextDouble())
                let B = A*X
                let Y = SolveTriangularLinearSystems A B false
                //assert(eqm X Y)
                acc <- acc && (eqm X Y)
        test (provider ^ ": SolveTriangularLinearSystems") acc
    
    do
(*        printf @"
          SolveLinearSystem
        "*)
        acc <- true
        for n=1 to maxDimension do
            // Generate random matrices and solutions.
            for i=1 to 10 do
                let A = Matrix.init n n (fun _ _ -> rnd.NextDouble())
                let x = Vector.init n (fun i -> rnd.NextDouble())
                let v = A*x
                let y = SolveLinearSystem A v
                //assert(eqv x y)
                acc <- acc && (eqv x y)
            
            // Generate random matrices and solutions.
            for i=1 to 10 do
                let A = Matrix.init n n (fun _ _ -> rnd.NextDouble())
                let x = Vector.init n (fun i -> rnd.NextDouble())
                let v = A*x
                let y = SolveLinearSystem A v
                //assert(eqv x y)
                acc <- acc && (eqv x y)
        test (provider ^ ": SolveTriangularLinearSystem") acc

    do
(*        printf @"
          SolveLinearSystems
        "*)
        acc <- true
        for n=1 to maxDimension do
            // Generate random matrices and solutions.
            for i=1 to 10 do
                let A = Matrix.init n n (fun _ _ -> rnd.NextDouble())
                let X = Matrix.init n n (fun i j -> rnd.NextDouble())
                let B = A*X
                let Y = SolveLinearSystems A B
                //assert(eqm X Y)
                acc <- acc && (eqm X Y)
            
            // Generate random matrices and solutions.
            for i=1 to 10 do
                let A = Matrix.init n n (fun _ _ -> rnd.NextDouble())
                let X = Matrix.init n n (fun i j -> rnd.NextDouble())
                let B = A*X
                let Y = SolveLinearSystems A B
                //assert(eqm X Y)
                acc <- acc && (eqm X Y)
        test (provider ^ ": SolveTriangularLinearSystems") acc

    do
(*        printf @"
          LU
        "*)
        acc <- true
        for n=1 to maxDimension do
            // Generate random square matrices and compute their LU factorization.
            for i=1 to 10 do
                let A = Matrix.init n n (fun i j -> rnd.NextDouble())
                let P,L,U = LU A
                //assert(eqm (A.PermuteRows P) (L*U) && IsLowerTriangular L && IsLowerTriangular U.T)
                acc <- acc && (eqm (A.PermuteRows P) (L*U) && IsLowerTriangular L && IsLowerTriangular U.Transpose)
        test (provider ^ ": LU") acc

    do
(*        printf @"
          Determinant
        "*)
        acc <- true
        for n=1 to maxDimension do
            // Generate random square matrices and multiply them and check determinant.
            for i=1 to 10 do
                let A = Matrix.init n n (fun i j -> rnd.NextDouble())
                let B = Matrix.init n n (fun i j -> rnd.NextDouble())
                let dA = Determinant A
                let dB = Determinant B
                let dAB = Determinant (A*B)
                //assert(eq (dA * dB) dAB)
                acc <- acc && (eq (dA * dB) dAB)
        test (provider ^ ": Determinant") acc

    do
(*        printf @"
          Inverse
        "*)
        acc <- true
        for n=1 to maxDimension do
            // Generate a random square matrix and compute its inverse, there is an almost zero probability that it is singular.
            for i=1 to 10 do
                let A = Matrix.init n n (fun i j -> rnd.NextDouble())
                let Ainv = Inverse A
                let I = Matrix.identity n
                //assert(eqm (A * Ainv) I)
                acc <- acc && (eqm (A * Ainv) I)
        test (provider ^ ": Inverse") acc

    do
(*        printf @"
          QR
        "*)
        acc <- true
        for n=1 to maxDimension do
            // Generate a random square matrix and compute its QR decomposition.
            for i=1 to 10 do
                let A = Matrix.init n n (fun i j -> rnd.NextDouble())
                let Q,R = QR A
                //assert(eqm (Q * R) A)
                acc <- acc && (eqm (Q * R) A)
        test (provider ^ ": QR") acc

    do
(*        printf @"
          Cholesky
        "*)
        acc <- true
        for n=1 to maxDimension do
            // Generate a random square matrix, and compute QR to get a random orthogonal matrix Q. Then
            // generate a diagonal matrix with positive entries V and compute QVQ' to get a random positive
            // definite matrix.
            for i=1 to 10 do
                let A = Matrix.init n n (fun i j -> rnd.NextDouble())
                let Q,_ = QR A
                let B = Q * (Matrix.init n n (fun i j -> if i = j then rnd.NextDouble() else 0.0)) * Q.Transpose
                let C = Cholesky B
                //assert(eqm (C.T * C) B)
                acc <- acc && (eqm (C.Transpose * C) B)
        test (provider ^ ": Cholesky") acc

    do
(*        printf @"
          Hessenberg
        "*)
      if provider = "Managed" then (* JAMES! *)      
        acc <- true
        for n=1 to maxDimension do
            for i=1 to 10 do
                let A = Matrix.init n n (fun i j -> rnd.NextDouble())
                let Q,H = Hessenberg A
                let isHessenberg A = Matrix.foldi (fun i j acc aij -> if i > j+1 then (acc && (abs aij) < 1e-5) else acc) true A
                let QH = Q * H
                //assert(eqm (Q * H) A)
                acc <- acc && (eqm (Q * H) A)
                //assert(isHessenberg H)
                acc <- acc && (isHessenberg H)
        test (provider ^ ": Hessenberg") acc
    ()

    do
(*        printf @"
          LeastSquares
        "*)
      if provider <> "Managed" then (* JAMES! *)
        acc <- true
        for n=10 to maxDimension do
            for i=1 to 10 do
                let A = Matrix.init n (n+rnd.Next(10)-5) (fun i j -> rnd.NextDouble())
                let b = Vector.init n (fun i -> rnd.NextDouble())
                let x = LeastSquares A b
                //assert(eqv (A.T * (A * x)) (A.T * b))
                acc <- acc && (eqv (A.Transpose * (A * x)) (A.Transpose * b))
        test (provider ^ ": LeastSquares") acc
    ()

let maxDimension = 50

// Test the managed code
Lapack.Stop()
printf "\nService Status: %s\n\n" (Lapack.Status())
RegressionTest "Managed" maxDimension

// Test the Intel MKL code
Lapack.StartWith(MKLProvider)
printf "\nService Status: %s\n\n" (Lapack.Status())
RegressionTest "Intel MKL" maxDimension

// Test the netlib code
Lapack.StartWith(NetlibProvider)
printf "\nService Status: %s\n\n" (Lapack.Status())
RegressionTest "Netlib" maxDimension
  

//! Finish
//--------  


#if TESTS_AS_APP
let RUN() = !failures
#else
let aa =
  match !failures with 
  | [] -> 
      stdout.WriteLine "Test Passed"
      printf "TEST PASSED OK" ;
      exit 0
  | _ -> 
      stdout.WriteLine "Test Failed"
      exit 1
#endif


