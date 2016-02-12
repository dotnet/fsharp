// #Regression #Diagnostics 
//<Expects status="warning" span="(6,5-6,15)" id="FS3189">Redundant arguments are being ignored in function 'invalidArg'\. Expected 2 but got 3 arguments\.$</Expects>
module M
type T() =
  member __.M1 source =
    invalidArg source "Used" "Ignored"
  member __.M2 source =
    invalidArg source "Used"