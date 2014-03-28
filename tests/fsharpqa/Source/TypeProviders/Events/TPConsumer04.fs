// Regression test for DevDiv#124017
// "TypeProvider: poor error message for malformed type providers that expose events with no Add/Remove methods"
// Case4: both GetAddMethod and GetReturnMethod return non-null => all is fine, no errors
//<Expects status="success"></Expects>
let _ = new N.T()
