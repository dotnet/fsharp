// Regression test for DevDiv:122834
// ("TypeProvider: Incorrect error message from compiler when TP exposes a Property that has no getter")
//
//<Expects status="error" span="(7,10-7,24)" id="FS3016">Property 'StaticProp' on provided type 'T' has CanRead=true but there was no value from GetGetMethod\(\)$</Expects>
//

let _ =  N.T.StaticProp
