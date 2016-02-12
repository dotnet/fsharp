// #Regression #Diagnostics 
//<Expects status="warning" span="(4,11-4,20)" id="FS3189">Redundant arguments are being ignored in function 'failwithf'\. Expected 3 but got 4 arguments\.$</Expects>
module M
let f() = failwithf "Used %A %s" "this" "but not" "this"
let g() = failwith "Used %A" "this"
let h() =
  let failwithf arg = Printf.ksprintf failwith arg
  failwithf "Used" "Ignored"