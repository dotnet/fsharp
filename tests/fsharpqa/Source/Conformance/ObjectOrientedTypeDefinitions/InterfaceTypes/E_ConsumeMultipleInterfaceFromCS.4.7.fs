// #Conformance #ObjectOrientedTypes #InterfacesAndImplementations #ReqNOMT 
// <Expects id="FS3350" status="error" span="(22,6-26,8)">Feature 'interfaces with multiple generic instantiation' is not available in F# 4.7. Please use language version 'preview' or greater.</Expects>
// <Expects id="FS3350" status="error" span="(34,6-40,8)">Feature 'interfaces with multiple generic instantiation' is not available in F# 4.7. Please use language version 'preview' or greater.</Expects>
// <Expects id="FS3350" status="error" span="(54,6-54,7)">Feature 'interfaces with multiple generic instantiation' is not available in F# 4.7. Please use language version 'preview' or greater.</Expects>
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

// Check we can't implement an interface inheriting from multiple instantiations of an interface when defining an object expression implementing a C# interface type 
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
  
// Check we can't implement an interface inheriting from multiple instantiations of an interface when defining an object expression inheriting from a C# class type
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

