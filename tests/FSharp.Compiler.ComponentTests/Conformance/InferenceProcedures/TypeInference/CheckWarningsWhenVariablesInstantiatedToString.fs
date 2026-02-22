// #Regression #TypeInference 
// Regression test for FSHARP1.0:4758
// Type Inference
// Check Warnings When Variables Instantiated To String
//<Expects id="FS0064" span="(14,22-14,24)" status="warning">.+'a.+'string'</Expects>
//<Expects id="FS0064" span="(15,21-15,23)" status="warning">.+'a.+'string'</Expects>
//<Expects id="FS0064" span="(16,30-16,31)" status="warning">.+'a.+'string'</Expects>
//<Expects id="FS0064" span="(17,22-17,23)" status="warning">.+'a.+'string'</Expects>
//<Expects id="FS0064" span="(18,23-18,24)" status="warning">.+'a.+'string'</Expects>
//<Expects id="FS0064" span="(19,22-19,23)" status="warning">.+'a.+'string'</Expects>
module M
let forceString (x:string) = ()
 
let ffA (x:'a) = x = ""             // expect: warns 'a = string
let ffB (x:'a) = [x;""]             // expect: warns 'a = string
let ffC (x:'a) = forceString x      // expect: warns 'a = string
let ffD (x:'a) = ["";x]             // expect: warns 'a = string
let ffE (x:'a) = "" = x             // expect: warns 'a = string
let ffF (x:'a) = ffE(x)             // expect: warns 'a = string
