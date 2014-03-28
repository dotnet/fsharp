// #Conformance #ObjectOrientedTypes #InterfacesAndImplementations #ReqNOMT 
// <Expects id="FS0443" status="error" span="(19,6-23,8)">This type implements or inherits the same interface at different generic instantiations 'I_002\<string\>' and 'I_002\<char\>'\. This is not permitted in this version of F#\.</Expects>
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
  
if (res = true) then
    exit 0
    
  
exit 1

