// #Regression #Conformance #ObjectOrientedTypes #Structs 
// Regression test for FSHARP1.0:3143
//<Expects span="(7,6-7,7)" status="error" id="FS0954">This type definition involves an immediate cyclic reference through a struct field or inheritance relation$</Expects>
//<Expects span="(7,6-7,7)" status="error" id="FS0912">This declaration element is not permitted in an augmentation$</Expects>
//<Expects span="(10,10-10,11)" status="error" id="FS0039">The value or constructor 'S' is not defined</Expects>
//<Expects span="(11,10-11,11)" status="error" id="FS0039">The value or constructor 'S' is not defined</Expects>
type S(x:S) = struct
              end

let s1 = S()
let s2 = S(s1)
