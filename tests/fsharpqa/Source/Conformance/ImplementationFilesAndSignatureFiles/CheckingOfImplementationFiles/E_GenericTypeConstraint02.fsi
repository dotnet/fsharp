// #Regression #Conformance #SignatureFiles 
module Test

// Regression test for FSharp1.0:5834 - Generic constraints on explictly specified type parameters work differently in fsi and fs
//<Expects status="error" span="(9,19)" id="FS0001">A type parameter is missing a constraint 'when 'a: struct'</Expects>
//<Expects status="notin" span="(9,7)" id="FS0340">The signature and implementation are not compatible because the declaration of the type parameter 'a' requires a constraint of the form 'a: struct</Expects>

type G<'a when 'a : struct>
val h<'a> : 'a -> G<'a>
