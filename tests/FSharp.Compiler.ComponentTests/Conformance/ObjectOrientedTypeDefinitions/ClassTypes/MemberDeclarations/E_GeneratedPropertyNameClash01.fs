// #Regression #Conformance #ObjectOrientedTypes #Classes #MethodsAndProperties #MemberDefinitions 
// Regression for 858162, confirming we get a good error message here
//<Expects status="error" id="FS0023" span="(6,19-6,24)">The member 'Item1' cannot be defined because the name 'Item1' clashes with the generated property 'Item1' in this type or module</Expects>

type tree = Node of tree * int
    with member x.Item1 = x

exit 1