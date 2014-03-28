// #NoMT #CompilerOptions
//<Expects status="error" span="(9,14)" id="FS0010">Unexpected keyword 'end' in implementation file$</Expects>
//<Expects status="error" span="(7,10)" id="FS0010">Unexpected character '\?' in type name$</Expects>
namespace N

module M =
    type á = class
               static member M() = 11
             end
