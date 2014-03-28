// Regression test for DevDiv#124017
// "TypeProvider: poor error message for malformed type providers that expose events with no Add/Remove methods"
//<Expects status="error" span="(5,9-5,18)" id="FS3029">Event 'Event1' on provided type 'N\.T' has no value from GetAddMethod\(\)$</Expects>
// Case1: both GetAddMethod returns null, GetReturnMethod returns null => error on GetAddMethod
let _ = new N.T()
