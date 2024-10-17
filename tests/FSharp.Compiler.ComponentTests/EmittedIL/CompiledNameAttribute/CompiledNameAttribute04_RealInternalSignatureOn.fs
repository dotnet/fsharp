// #Regression #NoMono #NoMT #CodeGen #EmittedIL #Attributes   
// Regression test for FSharp1.0:4740
// Title: Expose currying information in F# compiled form

// Regression test for FSharp1.0:4661
// Title: PreserveSigAttribute pseudo-custom attribute on COM interop interfaces does not compile correctly

// Regression test for FSharp1.0:5684
// Title: We should align generation of IL code for pseudo-custom attributes like PreserveSigAttribute for all language constructs (currently, it is incorrect for object expressions)
module Program

open System
open System.Runtime.InteropServices


let f1 x y = x + y
let f2 x = x 

[<AbstractClass>]
type C() = 
  member this.P = 1
  member this.M1 x y = x + y 
  [<PreserveSigAttribute>]
  member this.M2 x = x 

  abstract A1 : int -> int -> int
  abstract A2 : int -> int 


type IInterface =
    interface
        [<PreserveSigAttribute>]
        abstract SomeMethod : int -> int
    end


type S = 
  struct
    [<PreserveSigAttribute>]
    member this.M1 x = x
  end


type ITestInterface =
    interface
        abstract M : int -> int
    end

let a = { new ITestInterface with
              [<PreserveSigAttribute>]
              member this.M x = x + 1 }
