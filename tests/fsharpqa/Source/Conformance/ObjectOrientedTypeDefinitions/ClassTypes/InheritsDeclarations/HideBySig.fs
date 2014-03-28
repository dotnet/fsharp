// #Regression #Conformance #ObjectOrientedTypes #Classes #Inheritance 
// Regression of Bug 343136, Unable to consume F# members from a C#
// where F# does not generate HideBySig IL attribute

module Main

type Base() =
    abstract member Test : int -> int
    default this.Test x = x + 1

type Descendant() =
    inherit Base()
    member this.Test (x,y) = x - y
