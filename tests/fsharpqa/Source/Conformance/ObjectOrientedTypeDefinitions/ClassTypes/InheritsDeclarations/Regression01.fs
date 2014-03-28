// #Regression #Conformance #ObjectOrientedTypes #Classes #Inheritance 
// Regression of FSB 4205, Unable to inherit from a C#
// class where a virtual method becomes abstract in a subclass

type MyType() =
    inherit Bug4205_CSDerived()
    override x.Prop = 42

exit 0
