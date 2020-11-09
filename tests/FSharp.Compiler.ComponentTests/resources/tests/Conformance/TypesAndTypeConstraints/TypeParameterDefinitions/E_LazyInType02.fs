// #Conformance  #Regression #TypeAnnotations
// Dev11:14899, keyword lazy in type annotation no longer allowed
//<Expects status="error" span="(7,20-7,24)" id="FS0010">Unexpected keyword 'lazy' in pattern\. Expected '\)' or other token\.$</Expects>
//<Expects status="error" span="(7,12-7,13)" id="FS0583">Unmatched '\('$</Expects>
let f<'T> (x : Lazy<'T>) = () // this version works

let f1<'T> (x : 'T lazy) = () // Compiler doesn’t allow this notation  

exit 1