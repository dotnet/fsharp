module Test

module T1 = 
    type Enum1 = 
        | E = 1
        with
        member this.Foo() = 1 // not ok

module T2 =    
    type Enum2 = 
        | E2 = 1
    type Enum2
        with
        member this.Foo() = 1 // not ok
