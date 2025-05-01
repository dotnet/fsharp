// #Regression #Conformance #DeclarationElements #MemberDefinitions #Overloading #NoMono #ReqNOMT 
// Regression test for 4075 overload resolution confused by C<> and C<T> when C.GenericMethod<T> is given explicit instantiation

let reproB1 : int = MyClass.M(box 12)           // typechecks ok
let reproB2       = MyClass.M<int>(box 12)      // reports field/member M undefined
let reproB3       = MyClass< >.M<int>(box 12)   // typechecks ok
let reproB4 :int  = MyClass< >.M<_ >(box 12)    // typechecks ok
