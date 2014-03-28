// #Conformance #ObjectOrientedTypes #TypeExtensions #Regression
// Regression for 909525, previously there was no error here
// <Expects status="error" id="FS0909" span="(5,15-5,33)">All implemented interfaces should be declared on the initial declaration of the type</Expects>
type System.Random with
    interface System.IComparable 
    static member Factory() = 1

exit 1
