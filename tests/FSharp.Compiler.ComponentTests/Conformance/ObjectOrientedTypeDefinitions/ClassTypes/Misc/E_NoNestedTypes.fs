// #Regression #Conformance #ObjectOrientedTypes #Classes 
// Verify error when attempting to declare a nested type
//<Expects status="error" span="(6,6-6,11)" id="FS0547">A type definition requires one or more members or other declarations\. If you intend to define an empty class, struct or interface, then use 'type \.\.\. = class end', 'interface end' or 'struct end'\.$</Expects>
//<Expects status="error" span="(9,5-9,8)" id="FS0010">Unexpected keyword 'end' in implementation file$</Expects>

type Bar =                 
    type IFoo = interface
                end
    end
