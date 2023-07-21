module Test

type R =
    {
        F1: int
        F2: int
    }
  
    static let createRecord v1  r = { F1 = v1; F2 = v1 }
    static member Q = <@ createRecord 42  @>


printfn "%A" R.Q