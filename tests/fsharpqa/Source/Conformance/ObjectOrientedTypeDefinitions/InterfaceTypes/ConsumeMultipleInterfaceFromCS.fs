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

// Check we can use an object expression implementing a C# interface type inheriting multiple instantiations of an interface
if ( {new I_003<int> with 
               member xxx.Home(i) = i
               member xxx.Me(c:char) = 0
               member xxx.Me(s:string) = 0
      }.Home (
        {new I_002<int> with 
          member x.Me (s) = s
        }.Me(0) )  <> 0 ) then
            System.Console.WriteLine("I_003.Home failed")         
            res <- false
  
// Check we can use an object expression inheriting from a C# type implementing multiple instantiations of an interface
if (({new T() with 
               member x.ToString() = "a"
      interface I_003<int> with 
               member xxx.Home(i) = i
               member xxx.Me(c:char) = 0
               member xxx.Me(s:string) = 0
      } :> I_003<int>).Home (
        {new I_002<int> with 
          member x.Me (s) = s
        }.Me(0) )  <> 0 ) then
            System.Console.WriteLine("T.Home obj expr failed")         
            res <- false
  
// Check we can create an object of a C# type implementing multiple instantiations of an interface
if T().Home(4) <> 0 then
            System.Console.WriteLine("T.Home failed")         
            res <- false


// Check we can inherit from a C# type implementing multiple instantiations of an interface
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

