// Regression test for DevDiv:123580 ("TypeProvider: malformed type-provider that exposes a generic type with >0 generic arguments causes an internal error")
// We just want to verify that no assert happens + the error message is meaningful.
// The code below is supposed to fail because a provided type can't be generic.

//<Expects status="error" span="(7,9-7,18)" id="FS3126">Invalid number of generic arguments to type 'Tuple' in provided type\. Expected '0' arguments, given '3'\.$</Expects>

let b = new N.T()
