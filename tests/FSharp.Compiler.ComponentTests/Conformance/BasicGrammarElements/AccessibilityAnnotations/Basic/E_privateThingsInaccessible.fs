// #Regression #Conformance #DeclarationElements #Accessibility 
//<Expects status="error" span="(18,17)" id="FS1094">The value 'somePrivateField' is not accessible from this code location$</Expects>
//<Expects status="error" span="(19,17)" id="FS1094">The value 'somePrivateMethod' is not accessible from this code location$</Expects>
//<Expects status="error" span="(23,17)" id="FS0491">The member or object constructor 'PrivateMethod' is not accessible\. Private members may only be accessed from within the declaring type\. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions\.$</Expects>

module Module1 =
    
    let private somePrivateField = true
    let private somePrivateMethod x y = x ** y
    
    type Foo() =
        member this.PublicMethod() = true
        member private this.PrivateMethod() = false
        
module Module2 =

    // Private in public module   
    let test1 = Module1.somePrivateField
    let test2 = Module1.somePrivateMethod 2.0 8.0
 
    // Private methods   
    let x = new Module1.Foo()
    let test3 = x.PrivateMethod()

