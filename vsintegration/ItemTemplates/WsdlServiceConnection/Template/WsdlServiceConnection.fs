// The WsdlService type provider allows you to write code to consume a web service. 
// For more information, please go to 
//    http://go.microsoft.com/fwlink/?LinkId=229211

module $safeitemrootname$

#if INTERACTIVE
#r "System.Runtime.Serialization"
#r "System.ServiceModel"
#r "FSharp.Data.TypeProviders"
#endif

open System.Runtime.Serialization
open System.ServiceModel
open Microsoft.FSharp.Data.TypeProviders

// You can sign up for a Bing service developer account at http://msdn.microsoft.com/en-us/library/gg605201.aspx
let BING_APP_ID = "<your Bing Maps Developer Key>"

// Using Bing Map API routing service to calculate the driving distance between two Geolocations. http://www.microsoft.com/maps/developers/mapapps.aspx
type RouteService = Microsoft.FSharp.Data.TypeProviders.WsdlService<ServiceUri = "http://dev.virtualearth.net/webservices/v1/routeservice/routeservice.svc?wsdl">
type RouteCommon = RouteService.ServiceTypes.dev.virtualearth.net.webservices.v1.common
type Route = RouteService.ServiceTypes.dev.virtualearth.net.webservices.v1.route

let startPoint = Route.Waypoint(Location = RouteCommon.Location(Latitude = 47.64012046, Longitude = -122.1297104)) 
let endPoint = Route.Waypoint(Location = RouteCommon.Location(Latitude = 47.62049103, Longitude = -122.3492355))
let routReq1 = new Route.RouteRequest(Waypoints = [|startPoint; endPoint|])
routReq1.Credentials <- new RouteCommon.Credentials(ApplicationId = BING_APP_ID)
RouteService.GetBasicHttpBinding_IRouteService().CalculateRoute(routReq1).Result.Summary.Distance |> printfn "Driving Distance = %A (miles)" 