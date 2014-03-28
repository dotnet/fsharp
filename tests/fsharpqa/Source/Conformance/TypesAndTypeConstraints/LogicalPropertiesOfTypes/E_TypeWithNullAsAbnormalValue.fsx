// #Regression #Conformance #TypeConstraints 
// Type With Null As Abnormal Value.fsx
#light
// F# List
let x1 : int list = null

// F# record
type R = { x : int }
let r : R = null

// F# tuple
type T = int * int
let t : T = null

// F# function
type F = int -> int
let f : F = null

// F# class
type C = class
         end
let c : C = null

// F# interface
type I = interface
         end
let i : I = null

// F# union
type U = | U1
let u : U = null
 
//<Expects id="FS0043" span="(5,21-5,25)" status="error">The type 'int list' does not have 'null' as a proper value</Expects>
//<Expects id="FS0043" span="(9,13-9,17)" status="error">The type 'R' does not have 'null' as a proper value</Expects>
//<Expects id="FS0043" span="(13,13-13,17)" status="error">The type 'T' does not have 'null' as a proper value</Expects>
//<Expects id="FS0043" span="(17,13-17,17)" status="error">The type 'F' does not have 'null' as a proper value</Expects>
//<Expects id="FS0043" span="(22,13-22,17)" status="error">The type 'C' does not have 'null' as a proper value</Expects>
//<Expects id="FS0043" span="(27,13-27,17)" status="error">The type 'I' does not have 'null' as a proper value</Expects>
//<Expects id="FS0043" span="(31,13-31,17)" status="error">The type 'U' does not have 'null' as a proper value</Expects>
