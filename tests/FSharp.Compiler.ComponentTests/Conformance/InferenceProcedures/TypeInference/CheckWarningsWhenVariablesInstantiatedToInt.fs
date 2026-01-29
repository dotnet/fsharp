// #Regression #TypeInference 
// Regression test for FSHARP1.0:4758
// Type Inference
// Check Warnings When Variables Instantiated To Int
//<Expects id="FS0064" span="(14,22-14,23)" status="warning">.+'a.+'int'</Expects>
//<Expects id="FS0064" span="(15,21-15,22)" status="warning">.+'a.+'int'</Expects>
//<Expects id="FS0064" span="(16,27-16,28)" status="warning">.+'a.+'int'</Expects>
//<Expects id="FS0064" span="(17,21-17,22)" status="warning">.+'a.+'int'</Expects>
//<Expects id="FS0064" span="(18,22-18,23)" status="warning">.+'a.+'int'</Expects>
//<Expects id="FS0064" span="(19,22-19,23)" status="warning">.+'a.+'int'</Expects>
module M
let forceInt (x:int) = ()

let ffF (x:'a) = x = 1             // warning FS0064: This construct causes code to be less generic than indicated by the type annotations. The type variable 'a has been constrained to be type 'int'.
let ffG (x:'a) = [x;1]             // warning FS0064: This construct causes code to be less generic than indicated by the type annotations. The type variable 'a has been constrained to be type 'int'.
let ffH (x:'a) = forceInt x        // warning FS0064: This construct causes code to be less generic than indicated by the type annotations. The type variable 'a has been constrained to be type 'int'.
let ffI (x:'a) = [1;x]             // warning FS0064: This construct causes code to be less generic than indicated by the type annotations. The type variable 'a has been constrained to be type 'int'.
let ffJ (x:'a) = 1 = x             // warning FS0064: This construct causes code to be less generic than indicated by the type annotations. The type variable 'a has been constrained to be type 'int'.
let ffK (x:'a) = ffJ(x)            // warning FS0064: This construct causes code to be less generic than indicated by the type annotations. The type variable 'a has been constrained to be type 'int'.
