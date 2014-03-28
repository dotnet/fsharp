// Regression test for DevDiv#124017
// "TypeProvider: poor error message for malformed type providers that expose events with no Add/Remove methods"
// Case3: both GetAddMethod returns non-null, GetReturnMethod return null => error GetRemoveMethod
//<Expects status="error" span="(5,9-5,18)" id="FS3030">Event 'Event1' on provided type 'N\.T' has no value from GetRemoveMethod\(\)$</Expects>
let _ = new N.T()
