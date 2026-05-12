// #Regression #Diagnostics 
// Regression test for FSHARP1.0:3240
//<Expects id="FS0001" span="(15,22-15,26)" status="error">This expression was expected to have type.    'M\.M1\.Typ1'    .but here has type.    'M\.M2\.Typ1'</Expects>

module M
 module M1 = 
   type Typ1 = A

 module M2 = 
   type Typ1 = A
   
 open M1
 open M2
   
 let error = (M1.A = M2.A)

