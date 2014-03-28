// #Conformance #SignatureFiles 
#light

// Check FSI signature files for properties

namespace Properties01

type PropertiesTest() =

    member this.GetProperty = 1
    member this.SetProperty with set (x : string) = ()

    member this.GetSetProperty with get ()           = "foo"
                               and  set (x : string) = ()


module Test =

    let t = new PropertiesTest()
    if t.GetProperty <> 1 then exit 1
    t.SetProperty <- "foo"

    if t.GetSetProperty <> "foo" then exit 1
    t.GetSetProperty <- "bar"

    exit 0
