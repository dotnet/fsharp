// Regression test for DevDiv:378936
// DesignTime and Runtime  assemblies are both there, but the DesignTime helper (i.e. an assembly referenced by the DesignTime one) is missing
// We expect a graceful error (as opposed to a crash)
//<Expects status="error" id="FS3031">The type provider '.+MyTPRuntime.dll' reported an error: Assembly attribute 'TypeProviderAssemblyAttribute' refers to a designer assembly 'MyTPDesignTime' which cannot be loaded or doesn't exist.+</Expects>

exit 0