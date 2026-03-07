// #Regression #Conformance #ObjectOrientedTypes #InterfacesAndImplementations 
// Regression for 6457, this used to throw an internal error
// <Expects status="error" id="FS0039" span="(7,17-7,19)">The type parameter 'T is not defined</Expects>
// <Expects status="error" id="FS0715" span="(10,17-10,25)">Anonymous type variables are not permitted in this declaration</Expects>

type I2 =
   abstract v : 'T

type I2<'T> =
   abstract v : #seq<'T>

exit 1