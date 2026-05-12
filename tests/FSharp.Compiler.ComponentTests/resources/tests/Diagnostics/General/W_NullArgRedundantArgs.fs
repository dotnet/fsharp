// #Regression #Diagnostics 
//<Expects status="warning" span="(6,17-6,24)" id="FS3189">Redundant arguments are being ignored in function 'nullArg'\. Expected 1 but got 2 arguments\.$</Expects>
namespace M0
module M1 =
  module M2 =
    let f arg = nullArg "arg" "Ignored"
    let g arg = nullArg "arg"