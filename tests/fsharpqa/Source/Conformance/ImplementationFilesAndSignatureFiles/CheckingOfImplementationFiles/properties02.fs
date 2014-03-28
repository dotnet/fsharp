// #Conformance #SignatureFiles 
#light

// Check FSI signature files for properties

namespace Properties01

type AbstractPropertiesTest =

    abstract GetProperty : int
    abstract SetProperty : string with set
    abstract GetSetProperty : string
    abstract GetSetProperty : string with set

type Implementation() =
    interface AbstractPropertiesTest with
        override this.GetProperty = 1
        override this.SetProperty with set x = ()
        override this.GetSetProperty with get () = "things"
                                     and  set x  = ()

module Test = 

    let t = new Implementation() :> AbstractPropertiesTest

    if t.GetProperty <> 1 then exit 1

    t.SetProperty <- "stuff"

    if t.GetSetProperty <> "things" then exit 1
    t.GetSetProperty <- "stuff"

    exit 0
