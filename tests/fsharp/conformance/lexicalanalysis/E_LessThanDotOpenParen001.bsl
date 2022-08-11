
E_LessThanDotOpenParen001.fsx(23,12,23,15): typecheck error FS0043: No overloads match for method 'op_PlusPlusPlus'.

Known return type: ^a

Known type parameters: < (string -> int) , TestType<int,string> >

Available overloads:
 - static member TestType.(+++) : a: 'T * b: TestType<'T,'S> -> 'T // Argument 'a' doesn't match
 - static member TestType.(+++) : a: TestType<'T,'S> * b: 'T -> 'T // Argument 'a' doesn't match
 - static member TestType.(+++) : a: TestType<'T,'S> * b: ('T -> 'S) -> 'T // Argument 'a' doesn't match
 - static member TestType.(+++) : a: TestType<'T,'S> * b: TestType<'T,'S> -> 'T // Argument 'a' doesn't match

E_LessThanDotOpenParen001.fsx(25,10,25,45): typecheck error FS0041: No overloads match for method 'op_PlusPlusPlus'.

Known types of arguments: (string -> int) * TestType<int,string>

Available overloads:
 - static member TestType.(+++) : a: 'T * b: TestType<'T,'S> -> 'T // Argument 'a' doesn't match
 - static member TestType.(+++) : a: TestType<'T,'S> * b: 'T -> 'T // Argument 'a' doesn't match
 - static member TestType.(+++) : a: TestType<'T,'S> * b: ('T -> 'S) -> 'T // Argument 'a' doesn't match
 - static member TestType.(+++) : a: TestType<'T,'S> * b: TestType<'T,'S> -> 'T // Argument 'a' doesn't match

E_LessThanDotOpenParen001.fsx(26,10,26,68): typecheck error FS0041: No overloads match for method 'op_PlusPlusPlus'.

Known types of arguments: (string -> int) * TestType<int,string>

Available overloads:
 - static member TestType.(+++) : a: 'T * b: TestType<'T,'S> -> 'T // Argument 'a' doesn't match
 - static member TestType.(+++) : a: TestType<'T,'S> * b: 'T -> 'T // Argument 'a' doesn't match
 - static member TestType.(+++) : a: TestType<'T,'S> * b: ('T -> 'S) -> 'T // Argument 'a' doesn't match
 - static member TestType.(+++) : a: TestType<'T,'S> * b: TestType<'T,'S> -> 'T // Argument 'a' doesn't match
