// Regression test for DevDiv:378936
// Design and Runtime assemblies are both there.
// We expect just an error because the TP is not fully implemented
//<Expects status="error" id="FS3033">The type provider 'MyTPDesignTime\.HelloWorldProvider' reported an error: not impl</Expects>

exit 0
