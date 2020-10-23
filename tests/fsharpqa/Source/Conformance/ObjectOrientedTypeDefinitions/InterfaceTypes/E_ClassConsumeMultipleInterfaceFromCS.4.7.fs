// #Conformance #ObjectOrientedTypes #InterfacesAndImplementations #ReqNOMT 
// <Expects id="FS3350" status="error" span="(8,6-8,7)">Feature 'interfaces with multiple generic instantiation' is not available in F# 4.7. Please use language version 'preview' or greater.</Expects>
#light

let mutable res = true

// Check we can't implement an interface inheriting from multiple instantiations of an interface when defining an object expression inheriting from a C# class type
type D() = 
    inherit T()
    interface I_003<int> with
       member xxx.Home(i) = i

if (D() :> I_003<int>).Home(5) <> 5 then
            System.Console.WriteLine("D.Home failed")
            res <- false


if (res = true) then
    exit 0

exit 1

