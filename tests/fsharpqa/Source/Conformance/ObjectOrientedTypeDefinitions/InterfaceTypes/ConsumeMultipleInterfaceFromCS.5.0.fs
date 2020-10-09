// #Conformance #ObjectOrientedTypes #InterfacesAndImplementations #ReqNOMT 
#light	

let mutable res = true
let t = new T()
if (t.Me("F#") <> 2) then
  System.Console.WriteLine("t.Me(string) failed")
  res <- false
  
if (t.Me('a') <> 1) then
  System.Console.WriteLine("t.Me(char) failed")
  res <- false

if (t.Home(0) <> 0) then
  System.Console.WriteLine("t.Home failed")
  res <- false

// Check we can use an object expression inheriting from a C# type implementing multiple instantiations of an interface
if (({new T() with 
               member x.ToString() = "a"
      } :> I_003<int>).Home (4)  <> 0 ) then
            System.Console.WriteLine("T.Home obj expr failed")         
            res <- false
  
// Check we can create an object of a C# type implementing multiple instantiations of an interface
if T().Home(4) <> 0 then
            System.Console.WriteLine("T.Home failed")         
            res <- false


// Check we can inherit from a C# type implementing multiple instantiations of an interface
type D() = 
    inherit T()

if (D() :> I_003<int>).Home(5) <> 0 then
            System.Console.WriteLine("D.Home failed")         
            res <- false


if (res = true) then
    exit 0
    
  
exit 1

