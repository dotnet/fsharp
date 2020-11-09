// #Regression #Diagnostics 
//<Expects status="warning" span="(4,11-4,19)" id="FS3189">Redundant arguments are being ignored in function 'failwith'\. Expected 1 but got 2 arguments\.$</Expects>
module M
let f() = failwith "Used" "Ignored"
let g() = failwith "Used"