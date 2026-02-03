// #Regression #Diagnostics 
//<Expects status="warning" span="(6,5-6,10)" id="FS3189">Redundant arguments are being ignored in function 'raise'\. Expected 1 but got 2 arguments\.$</Expects>
module M
type T() =
  member __.M1() =
    raise (exn()) "Ignored"
  member __.M2() =
    raise (exn())