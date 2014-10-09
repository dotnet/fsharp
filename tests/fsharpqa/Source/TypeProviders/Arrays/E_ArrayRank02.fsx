// #Regression #TypeProvider #Arrays
// This is regression test for DevDiv:213995
//<Expects status="error" span="(5,10-5,34)" id="FS3033">System\.String\[,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,\].*has too many dimensions</Expects>

type X = N.T<"System.String", 35>

let _ = typeof<X>

exit 0



