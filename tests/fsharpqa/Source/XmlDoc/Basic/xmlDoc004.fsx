// #Regression #XMLDoc
// This used to be regression test for FSHARP1.0:850
// Verify that XmlDoc name is correctly generated
//<Expects status=success></Expects>

#light 

///mynamespace
namespace MyRather.MyDeep.MyNamespace 
///testRecord
type TestRecord = { 
    /// Record string
    MyProperpty : string 
} 

///test enum
type TestEnum = VALUE1 
                ///enumValue2
                | VALUE2

///my module comment
module myModule =
    ///integer value
    let myVariable = 4;

    ///my function
    let testAnd x y =
        ///inner match
        match x, y with
            | true, true -> true
            | _ -> false;;