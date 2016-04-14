// #Quotations
#nowarn "57"
#r "System.Data.Services.Client.dll"
#r "FSharp.Data.TypeProviders.dll"

open Microsoft.FSharp.Linq
open Microsoft.FSharp.Data.TypeProviders
open System
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.ExprShape
open Microsoft.FSharp.Linq.RuntimeHelpers

[<AutoOpen>]
module Infrastructure =
    let mutable failures = []
    let reportFailure s = 
        stderr.WriteLine " NO"; failures <- s :: failures

    let argv = System.Environment.GetCommandLineArgs() 
    let SetCulture() = 
        if argv.Length > 2 && argv.[1] = "--culture" then  
            let cultureString = argv.[2] 
            let culture = new System.Globalization.CultureInfo(cultureString) 
            stdout.WriteLine ("Running under culture "+culture.ToString()+"...");
            System.Threading.Thread.CurrentThread.CurrentCulture <-  culture
  
    do SetCulture()    

    let check  s v1 v2 = 
       if v1 = v2 then 
           printfn "test %s...passed " s 
       else 
           failures <- failures @ [(s, box v1, box v2)]
           printfn "test %s...failed, expected \n\t%A\ngot\n\t%A" s v2 v1

    let test s b = check s b true
    let qmap f (x:System.Linq.IQueryable<_>) = x |> Seq.map f |> System.Linq.Queryable.AsQueryable

    let checkCommuteSeq s (q1: System.Linq.IQueryable<'T>) q2 =
        check s (q1 |> Seq.toList) (q2 |> Seq.toList)

    let checkCommuteVal s q1 q2 =
        check s q1 q2

 
type Northwest = ODataService< "http://services.odata.org/V2/Northwind/Northwind.svc/",LocalSchemaFile="schema1.csdl",ForceUpdate=false  >

module Test = begin end

module DuplicateTypes = 
     
    type Northwest = ODataService< "http://services.odata.org/V2/Northwind/Northwind.svc/",LocalSchemaFile="schema2.csdl",ForceUpdate=false >


 
type Northwest2 = ODataService< "http://services.odata.org/V2/Northwind/Northwind.svc/",LocalSchemaFile="schema3.csdl",ForceUpdate=false  >
 

open System
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.ExprShape
open Microsoft.FSharp.Linq
open Microsoft.FSharp.Linq.RuntimeHelpers

module Queryable = 

    let db = Northwest.GetDataContext()
    let urlCapture = new System.Text.StringBuilder()
    db.DataContext.SendingRequest.Add (fun x -> printfn "yurl = %A" x.Request.RequestUri; urlCapture.Append x.Request.RequestUri |> ignore)

    let checkODataUrlTextForValue s (q1: unit -> 'T) (text:string) =
        urlCapture.Clear() |> ignore
        q1() |> ignore
        let url = urlCapture.ToString()
        check s url text



    let allCustomersQuery = 
        checkODataUrlTextForValue "vwe4yuwe09uu"
            (fun () -> query { for c in db.Customers do select c } |> Seq.length)
            "http://services.odata.org/V2/Northwind/Northwind.svc/Customers()"
        

    let allEmployeesQuery = 
        checkODataUrlTextForValue "vwe4yuwe09uu"
            (fun () -> query { for c in db.Employees do select c } |> Seq.length)
            "http://services.odata.org/V2/Northwind/Northwind.svc/Employees()"

    let allProductsQuery = 
        checkODataUrlTextForValue "vwe4yuwe09uu"
            (fun () -> query { for c in db.Products do select c } |> Seq.length)
            "http://services.odata.org/V2/Northwind/Northwind.svc/Products()"
        

    let allOrdersQuery = 
        checkODataUrlTextForValue "vwe4yuwe09uu"
            (fun () -> query { for c in db.Orders do select c } |> Seq.length)
            "http://services.odata.org/V2/Northwind/Northwind.svc/Orders()"
        

    let firstFiveOrders = 
        checkODataUrlTextForValue "vwe4yuwe09uu"
            (fun () -> query { for c in db.Orders do take 5; select c } |> Seq.length)
            "http://services.odata.org/V2/Northwind/Northwind.svc/Orders()?$top=5"
        


    let ordersSortedByShipDateLatestFirst = 
        checkODataUrlTextForValue "vwe4yuwe09uu"
            (fun () -> query { for o in db.Orders do sortByNullableDescending o.ShippedDate; select (o.OrderID, o.ShippedDate) } |> Seq.length)
            "http://services.odata.org/V2/Northwind/Northwind.svc/Orders()?$orderby=ShippedDate desc&$select=OrderID,ShippedDate"
        

    let ordersSortedByShipDateEarliestFirst = 
        checkODataUrlTextForValue "vwe4yuwe09uu"
            (fun () -> query { for o in db.Orders do sortByNullable o.ShippedDate; select (o.OrderID, o.ShippedDate) } |> Seq.length)
            "http://services.odata.org/V2/Northwind/Northwind.svc/Orders()?$orderby=ShippedDate&$select=OrderID,ShippedDate"
        

    let ordersSortedByCustomerIDAndShipDateLatestFirst= 
        checkODataUrlTextForValue "vwe4yuwe09uu"
            (fun () -> query { for o in db.Orders do sortBy o.CustomerID; thenByNullableDescending o.ShippedDate; select (o.CustomerID, o.OrderID, o.ShippedDate) } |> Seq.length)
            "http://services.odata.org/V2/Northwind/Northwind.svc/Orders()?$orderby=CustomerID,ShippedDate desc&$select=CustomerID,OrderID,ShippedDate"

        


    let ordersSortedByCustomerIDAndShipDateEarliestFirst= 
        checkODataUrlTextForValue "vwe4yuwe09uu"
            (fun () -> query { for o in db.Orders do sortBy o.CustomerID; thenByNullable o.ShippedDate; select (o.CustomerID, o.OrderID, o.ShippedDate) } |> Seq.length)
            "http://services.odata.org/V2/Northwind/Northwind.svc/Orders()?$orderby=CustomerID,ShippedDate&$select=CustomerID,OrderID,ShippedDate"
    
#if COMPILED
    [<System.STAThread>]
    do()
#endif

let _ = 
  if not failures.IsEmpty then (printfn "Test Failed, failures = %A" failures; exit 1) 
  else (stdout.WriteLine "Test Passed"; 
        System.IO.File.WriteAllText("test.ok","ok"); 
        exit 0)


