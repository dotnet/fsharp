// #Conformance #ObjectOrientedTypes #InterfacesAndImplementations #ReqNOMT 
// <Expects id="FS0443" status="error" span="(8,6-8,7)">This type implements the same interface at different generic instantiations 'I_002\<string\>' and 'I_002\<char\>'\. This is not permitted in this version of F#\.</Expects>
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

