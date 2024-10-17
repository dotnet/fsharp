    // #Regression #Conformance #SignatureFiles #Classes #ObjectConstructors #ObjectOrientedTypes #Fields #MemberDefinitions #MethodsAndProperties #Unions #InterfacesAndImplementations #Events #Overloading #Recursion #Regression 



module rec Tests

open System
open System.Collections
open System.Collections.Generic
open System.Diagnostics
open System.Windows.Forms

let failures = ref []
let report_failure s = 
  stderr.WriteLine " NO"; failures := s :: failures.Value
let test s b = stderr.Write(s:string);  if b then stderr.WriteLine " OK" else report_failure s
let check s v1 v2 = test s (v1 = v2)

//--------------------------------------------------------------
// Test defining a record using object-expression syntax

module StaticInitializerTest3 =

    let x = ref 2
    do x := 3

    type C() = 
        static let mutable v = x.Value + 1
        static do v <- 3
        
        member x.P = v
        static member P2 = v+x.Value

    check "lwnohivq16" (new C()).P 3
    check "lwnohivq48" C.P2 6

let _ = 
  if not failures.Value.IsEmpty then (eprintfn "Test Failed, failures = %A" failures.Value; exit 1) 
  else (stdout.WriteLine "Test Passed"; 
        printf "TEST PASSED OK"; 
        exit 0)

