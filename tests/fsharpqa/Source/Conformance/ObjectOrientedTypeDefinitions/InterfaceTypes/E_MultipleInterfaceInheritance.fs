// #Regression #Conformance #ObjectOrientedTypes #InterfacesAndImplementations 
// Verify that the same generic interface can NOT be inherited multiple times with different type parameters.

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

//<Expects id="FS0039" status="error" span="(41,7-41,9)">The type 'T' does not define the field, constructor or member 'Me'</Expects>
//<Expects id="FS0039" status="error" span="(45,7-45,9)">The type 'T' does not define the field, constructor or member 'Me'</Expects>
//<Expects id="FS0039" status="error" span="(49,7-49,11)">The type 'T' does not define the field, constructor or member 'Home'</Expects>
//<Expects id="FS0366" status="error" span="(53,6-55,8)">No implementation was given for those members: </Expects>
//<Expects span="(53,6-55,8)">'abstract I_002\.Me: 'a -> int'\.</Expects>
//<Expects span="(53,6-55,8)">'abstract I_002\.Me: 'a -> int'\.</Expects>
//<Expects span="(53,6-53,8)">Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e\.g\. 'interface \.\.\. with member \.\.\.'\.</Expects>
//<Expects id="FS0001" status="error" span="(57,24-57,32)">This expression was expected to have type.    'int'    .but here has type.    'string'</Expects>
//<Expects id="FS0001" status="error" span="(58,24-58,30)">This expression was expected to have type.    'int'    .but here has type.    'char'</Expects>
