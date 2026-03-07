// #Regression #Conformance #TypeConstraints 

// FSB 1742, Wrong ctor signature (in ctor constraint) is not reported as an error
//<Expects id="FS0071" span="(18,11-18,15)" status="error">Type constraint mismatch when applying the default type 'C' for a type inference variable</Expects>
// DefaultConstructorConstraint02.fs(17,11-17,15): error FS0071: Type constraint mismatch when applying the default type 'C' for a type inference variable
(*
5.1.5.4	.NET Default  Constructor Constraints
A constraint of the form typar : (new : unit -> 'a) is an explicit .NET default constructor constraint. 
During constraint solving, the constraint type : (new : unit -> 'a) is met if type has a parameterless object constructor.
*)

type C(i:int)=
    class
    end

let g<'a when 'a : (new : unit -> 'a)> () = new 'a()

let a:C = g ()
