// Regression test for DevDiv:122834
// ("TypeProvider: Incorrect error message from compiler when TP exposes a Property that has no getter")
//
//<Expects status="error" span="(7,1-7,16)" id="FS3018">Property 'StaticProp2' on provided type 'T' has CanWrite=true but there was no value from GetSetMethod\(\)$</Expects>
//

N.T.StaticProp2 <- true
