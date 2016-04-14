// Regression test for DevDiv:257720 - Improve diagnostics for TPs

open Microsoft.FSharp.Data.TypeProviders
type T1 = ODataService< ServiceUri = "" > 

//<Expects status="error" span="(4,11)" id="FS3033">The type provider 'Microsoft\.FSharp\.Data\.TypeProviders\.DesignTime\.DataProviders' reported an error: Error reading schema\. Could not find file '.+\$metadata'\.$</Expects>
