// Regression test for DevDiv:257720 - Improve diagnostics for TPs

open Microsoft.FSharp.Data.TypeProviders

type T1 = ODataService< ServiceUri = "http://www.bing.com/" > 

//<Expects status="error" span="(5,11)" id="FS3033">The type provider 'Microsoft\.FSharp\.Data\.TypeProviders\.DesignTime\.DataProviders' reported an error: Error reading schema\. The remote server returned an error: \(404\) Not Found\.$</Expects>