// #Regression #Conformance #SignatureFiles 
// Regression test for FSharp1.0:5628
// Title: incorrect signature check: implementation file missing implementation of method declared in signature file

//<Expects status="error" span="(7,19-7,30)" id="FS0034">Module 'Test' contains.    static member C1\.op_Explicit: x: C1 -> float    .but its signature specifies.    static member C1\.op_Explicit: x: C1 -> int    .The types differ$</Expects>



module Test
[<SealedAttribute ()>]
type C1 =
  class
    new : unit -> C1
    static member op_Explicit : x:C1 -> int
    static member op_Explicit : x:C1 -> float
  end

