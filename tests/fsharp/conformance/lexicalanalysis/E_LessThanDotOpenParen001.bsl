
E_LessThanDotOpenParen001.fsx(18,71,18,75): typecheck error FS0193: A type parameter is missing a constraint 'when ( ^T or  ^?8045) : (static member ( + ) :  ^T *  ^?8045 ->  ^?8046)'

E_LessThanDotOpenParen001.fsx(18,73,18,75): typecheck error FS0064: This construct causes code to be less generic than indicated by the type annotations. The type variable 'S has been constrained to be type 'int'.

E_LessThanDotOpenParen001.fsx(18,27,18,30): typecheck error FS1198: The generic member '( +++ )' has been used at a non-uniform instantiation prior to this program point. Consider reordering the members so this member occurs first. Alternatively, specify the full type of the member explicitly, including argument types, return type and any additional generic parameters and constraints.

E_LessThanDotOpenParen001.fsx(18,19,18,67): typecheck error FS0670: This code is not sufficiently generic. The type variable  ^T when  ^T : (static member ( + ) :  ^T *  ^T ->  ^a) could not be generalized because it would escape its scope.

E_LessThanDotOpenParen001.fsx(31,12,31,15): typecheck error FS0043: No overloads match for method 'op_PlusPlusPlus'.



Available overloads:
 - static member TestType.( +++ ) : a:'T * b:TestType<'T,'S> -> 'T

 - static member TestType.( +++ ) : a:(int -> 'T) * b:TestType<'T,int> ->  ^a when  ^T1 : (static member ( + ) :  ^T1 *  ^T1 ->  ^a)

 - static member TestType.( +++ ) : a:TestType<'T,'S> * b:'T -> 'T

 - static member TestType.( +++ ) : a:TestType<'T,'S> * b:('T -> 'S) -> 'T

 - static member TestType.( +++ ) : a:TestType<'T,'S> * b:TestType<'T,'S> -> 'T

E_LessThanDotOpenParen001.fsx(33,10,33,45): typecheck error FS0041: No overloads match for method 'op_PlusPlusPlus'.



Arguments given:
 - (string -> int)

 - TestType<int,string>



Available overloads:
 - static member TestType.( +++ ) : a:'T * b:TestType<'T,'S> -> 'T

 - static member TestType.( +++ ) : a:(int -> 'T) * b:TestType<'T,int> ->  ^a when  ^T1 : (static member ( + ) :  ^T1 *  ^T1 ->  ^a)

 - static member TestType.( +++ ) : a:TestType<'T,'S> * b:'T -> 'T

 - static member TestType.( +++ ) : a:TestType<'T,'S> * b:('T -> 'S) -> 'T

 - static member TestType.( +++ ) : a:TestType<'T,'S> * b:TestType<'T,'S> -> 'T

E_LessThanDotOpenParen001.fsx(34,44,34,54): typecheck error FS0001: This expression was expected to have type
    'int'    
but here has type
    'string'    

E_LessThanDotOpenParen001.fsx(34,10,34,68): typecheck error FS0041: No overloads match for method 'op_PlusPlusPlus'.



Arguments given:
 - (int -> 'a)

 - TestType<int,string>



Available overloads:
 - static member TestType.( +++ ) : a:'T * b:TestType<'T,'S> -> 'T

 - static member TestType.( +++ ) : a:(int -> 'T) * b:TestType<'T,int> ->  ^a when  ^T1 : (static member ( + ) :  ^T1 *  ^T1 ->  ^a)

 - static member TestType.( +++ ) : a:TestType<'T,'S> * b:'T -> 'T

 - static member TestType.( +++ ) : a:TestType<'T,'S> * b:('T -> 'S) -> 'T

 - static member TestType.( +++ ) : a:TestType<'T,'S> * b:TestType<'T,'S> -> 'T
