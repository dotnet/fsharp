// The SqlDataConnection (LINQ to SQL) TypeProvider allows you to write code that uses 
// a live connection to a database. For more information, please go to 
//    http://go.microsoft.com/fwlink/?LinkId=229209

module $safeitemrootname$

#if INTERACTIVE
#r "System.Data"
#r "System.Data.Linq"
#r "FSharp.Data.TypeProviders"
#endif

open System.Data
open System.Data.Linq
open Microsoft.FSharp.Data.TypeProviders

// You can use Server Explorer to build your ConnectionString. 
type SqlConnection = Microsoft.FSharp.Data.TypeProviders.SqlDataConnection<ConnectionString = @"Data Source=(LocalDB)\v11.0;Initial Catalog=tempdb;Integrated Security=True">
let db = SqlConnection.GetDataContext()

//let table = query {
//    for r in db.SomeTable do
//    select r
//    }
//
//for p in table do
//    printfn "%s" p.SomeProperty
