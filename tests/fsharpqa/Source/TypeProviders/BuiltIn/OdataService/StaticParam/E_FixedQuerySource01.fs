//Dev11:255207, make sure we give a good error for AzureDataMarket sources that use fixed queries
//<Expects status="error" span="(4,10-4,135)" id="FS3033">The type provider 'Microsoft\.FSharp\.Data\.TypeProviders\.DesignTime\.DataProviders' reported an error: The provided ServiceUri is for a data service that supports fixed queries\. The OData type provider does not support such services\.$</Expects>

type T = Microsoft.FSharp.Data.TypeProviders.ODataService<"https://api.datamarket.azure.com/data.ashx/Zillow/MortgageInformationAPIs">

exit 1