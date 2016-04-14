// Regression test for DevDiv:257720 - Improve diagnostics for TPs

#if INTERACTIVE
#r "System.ServiceModel"
#r "FSharp.Data.TypeProviders"
#endif

open Microsoft.FSharp.Data.TypeProviders
type zillowMortgageRate = ODataService< @"https://api.datamarket.azure.com/Zillow/MortgageInformationAPIs/", ForceUpdate=true >

//<Expects status="error" span="(9,27)" id="FS3033">The type provider 'Microsoft\.FSharp\.Data\.TypeProviders\.DesignTime\.DataProviders' reported an error: The provided ServiceUri is for a data service that supports fixed queries\. The OData type provider does not support such services\.$</Expects>
