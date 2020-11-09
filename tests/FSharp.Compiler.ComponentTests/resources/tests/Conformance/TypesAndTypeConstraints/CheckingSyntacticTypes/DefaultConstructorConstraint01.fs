// #Regression #Conformance #TypeConstraints 
#light
// FSB 1742, Wrong ctor signature (in ctor constraint) is not reported as an error
//<Expects id="FS0700" span="(13,15-13,40)" status="error">'new'.+constraint</Expects>
// DefaultConstructorConstraint01.fs(12,15-12,40): error FS0191: 'new' constraints must take one argument of type 'unit' and return the constructed type.

(*
5.1.5.4	.NET Default  Constructor Constraints
A constraint of the form typar : (new : unit -> 'a) is an explicit .NET default constructor constraint. 
During constraint solving, the constraint type : (new : unit -> 'a) is met if type has a parameterless object constructor.
*)

let g<'a when 'a : (new : string -> 'a)> () = new 'a()
