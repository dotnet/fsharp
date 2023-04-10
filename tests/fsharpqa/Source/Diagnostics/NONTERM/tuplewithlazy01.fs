// #Regression #Diagnostics 
// Regression test for DevDiv:64339
// Note that the bug still repros in CHK/DBG bits - we will knownfail it
//<Expects status="error" span="(10,25-10,29)" id="FS0010">Unexpected keyword 'lazy' in type definition$</Expects>
//<Expects status="error" span="(13,29-13,33)" id="FS0010">Unexpected keyword 'lazy'$</Expects>

// 5 elements -> ok
type Ok(a, b, c, d, e : lazy<int>) = class end

// 6 elements -> weird diagnostics (in non RET builds)
type Bad(a, b, c, d, e, f : lazy<int>) = class end

