open System

open SQLite.Net
open SQLite.Net.Attributes
open SQLite.Net.Platform.Generic

type Site (url:string) =
  let mutable id = new int()
  let mutable BD = ""
  let mutable site = url
  let mutable shown = false
  let mutable exemplarcontributor = false
  [<PrimaryKey>] [<AutoIncrement>]
  member x.ID with get() = id
              and set v = id <- v
  member x.ExemplarContributor with get() = exemplarcontributor
                                  and set v = exemplarcontributor <- v
  member x.Shown with get() = shown
                      and set v = shown <- v
  member x.BreakDown with get() = BD
                      and set v = BD <- v
  [<Indexed>]
  member x.Site with get() = site
                 and set v = site <- v
  member x.Url = url
  new() = Site "www.google.com"

[<CLIMutable>]
type Site2 =
  { id : int
    visited : string
    comment : string }

type Database (path) =
  inherit SQLiteConnection(new SQLitePlatformGeneric(), path)
  member x.Setup() =
    base.CreateTable<Site>() |> ignore
    base.CreateTable<Site2>() |> ignore

[<EntryPoint>]
let main argv = 
  let D = new Database("test.sqlitedb")
  D.Setup() |> ignore

  let s = new Site "www.google.com"
  D.Insert(s) |> ignore
  D.Commit|>ignore
  0

