// #Regression #TypeInference 
// Regression for FSB 4772, asymmetric warning on user supplied alpha instantiation
// Verify we give appropriate warnings when a generic value is constrained
//<Expects id="FS0064" span="(10,22-10,24)"   status="warning">This construct causes code to be less generic than indicated by the type annotations.</Expects>
//<Expects id="FS0064" span="(11,21-11,23)" status="warning">This construct causes code to be less generic than indicated by the type annotations.</Expects>
//<Expects id="FS0064" span="(12,30-12,23)" status="warning">This construct causes code to be less generic than indicated by the type annotations.</Expects>
//<Expects id="FS0064" span="(13,22-13,23)" status="warning">This construct causes code to be less generic than indicated by the type annotations.</Expects>
//<Expects id="FS0064" span="(14,23-14,24)" status="warning">This construct causes code to be less generic than indicated by the type annotations.</Expects>

let ffA (x:'a) = x = ""             // FAIL: no warning on 'a = string
let ffB (x:'a) = [x;""]             // FAIL: no warning on 'a = string
let ffC (x:'a) = printf "%s" x      // OK:   warns on 'a = string
let ffD (x:'a) = ["";x]             // OK:   warns on 'a = string
let ffE (x:'a) = "" = x             // OK:   warns on 'a = string

exit 0
