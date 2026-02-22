// #Regression #Diagnostics 
//<Expects status="warning" span="(6,20-6,29)" id="FS3189">Redundant arguments are being ignored in function 'invalidOp'\. Expected 1 but got 2 arguments\.$</Expects>
namespace M0
module M1 =
  module M2 =
    let f source = invalidOp source "Ignored"
    let g source = invalidOp source