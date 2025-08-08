type AAA =
  interface System.IDisposable with
    open System
    member this.Dispose (): unit = 
      raise (NotImplementedException())
