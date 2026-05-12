// #NoMT #CompilerOptions 
// --codepage 0
// It does not mean much to me...
//<Expects status="success"></Expects>


let x = "a"
let y = 'a'.ToString();

if x = y then () else failwith "Failed: 1"