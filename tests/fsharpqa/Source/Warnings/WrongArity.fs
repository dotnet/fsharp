// #Warnings
//<Expects status="Error"  id="FS0503">A member or object constructor 'MyMember' taking 3</Expects>

type MyType() = 
   static member MyMember(arg1, arg2:int ) = () 
   static member MyMember(arg1, arg2:byte) = ()

 
MyType.MyMember("", 0, 0)
    
exit 0