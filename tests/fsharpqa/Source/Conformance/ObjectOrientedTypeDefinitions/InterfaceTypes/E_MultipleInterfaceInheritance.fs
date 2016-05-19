// #Regression #Conformance #ObjectOrientedTypes #InterfacesAndImplementations 
// Verify that the same generic interface can NOT be inherited multiple times with different type parameters.
//<Expects id="FS0039" status="error" span="(48,7-48,9)">The field, constructor or member 'Me' is not defined</Expects>
//<Expects id="FS0039" status="error" span="(52,7-52,9)">The field, constructor or member 'Me' is not defined</Expects>
//<Expects id="FS0039" status="error" span="(56,7-56,11)">The field, constructor or member 'Home' is not defined</Expects>
//<Expects id="FS0366" status="error" span="(60,6-62,8)">No implementation was given for 'abstract member I_002\.Me : 'a -> int'\. Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e\.g\. 'interface \.\.\. with member \.\.\.'\.</Expects>
//<Expects id="FS0366" status="error" span="(60,6-62,8)">No implementation was given for 'abstract member I_002\.Me : 'a -> int'\. Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e\.g\. 'interface \.\.\. with member \.\.\.'\.</Expects>
//<Expects id="FS0001" status="error" span="(64,24-64,32)">This expression was expected to have type.    'int'    .but here has type.    'string'</Expects>
//<Expects id="FS0001" status="error" span="(65,24-65,30)">This expression was expected to have type.    'int'    .but here has type.    'char'</Expects>

//Interface - empty
type I_000<'a> =
 interface
 end 

//Interface with inherits-decl 
type I_001 =
 interface
  inherit I_000<char>
  inherit I_000<string>
 end

//Interface with type-defn-members 
type I_002<'a> =
 interface
  abstract Me: 'a -> int
 end 
 
//Interface with inherits-decl & type-defn-members 
type I_003<'a> =
 interface
  inherit I_002<string>
  inherit I_002<char>
  abstract Home: 'a -> 'a
 end 

type T () =
  class
    interface I_003<int> with
      member x.Home i = i
      member x.Me (s:string) = 2
      member x.Me (c:char) = 1
  end
  
  
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
        member x.Home(i) = i 
      }.Home (
        {new I_002<int> with 
          member x.Me (s:string)  = 0
          member x.Me (c:char) = 1
        }.Me(5) )  <> 0 ) then
            System.Console.WriteLine("I_003.Home failed")         
            res <- false
