// Verify the DataServiceCollection static param
// DataServiceCollection = true

type T = Microsoft.FSharp.Data.TypeProviders.ODataService< 
                                                ServiceUri = "http://services.odata.org/V2/OData/OData.svc/", 
                                                LocalSchemaFile = "", 
                                                ForceUpdate = true, 
                                                DataServiceCollection = true >

let t = T.GetDataContext()

let q = 
    query {
        for l in t.Suppliers do
        head 
    }

let w = 
    match q.Products with
    | :? System.Data.Services.Client.DataServiceCollection<T.ServiceTypes.Product> -> 0
    | _ -> (printf "FAIL: type is a System.Data.Services.Client.DataServiceCollection<T.ServiceTypes.Product>"; 1)

exit w
