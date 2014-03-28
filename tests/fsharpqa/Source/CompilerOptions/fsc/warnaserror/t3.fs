// #Regression #NoMT #CompilerOptions 
// Regression test for FSHARP1.0:5381

//<Expects status="warning" id="FS0025"></Expects>
//<Expects status="error" id="FS0026"></Expects>
type DU = | A
          | B

let f x = match x with
          | B -> 0
          | B -> 0
