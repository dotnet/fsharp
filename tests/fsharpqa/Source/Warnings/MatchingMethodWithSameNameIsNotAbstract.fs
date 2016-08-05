// #Warnings
//<Expects status="Error" span="(11,16)" id="FS0767">The type Foo contains the member 'MyX' but it is not a virtual or abstract method that is available to override or implement.</Expects>
//<Expects>ToString</Expects>

type Foo(x : int) =
  member v.MyX() = x
  
let foo =  
    { new Foo(3) 
        with
        member v.MyX() = 4 }


exit 0