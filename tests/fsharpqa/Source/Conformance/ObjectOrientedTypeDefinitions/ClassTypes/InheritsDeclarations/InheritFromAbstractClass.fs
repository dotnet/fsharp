// #Regression #Conformance #ObjectOrientedTypes #Classes #Inheritance 
// Regression test for FSHARP1.0:3929
// Impossible to subclass certain C# classes
//<Expects status="success"></Expects>


type Bottom() =
    inherit Mid()
    override x.PubF() = ()
