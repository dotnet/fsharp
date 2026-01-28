// #Regression #NoMT #CompilerOptions 
// Test that using [--flaterrors] does not make an impact on regular single-line error messages
//<Expects id="FS0039" status="error">The value or constructor 'b' is not defined</Expects>

let a = b
