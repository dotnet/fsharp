// #Conformance #DeclarationElements #Fields #MemberDefinitions 
// Verify the ability to create public, static fields


type ClassType =
//    [<DefaultValue>]
//    static val mutable public  FieldPub : string
    [<DefaultValue>]
    static val mutable private FieldPri : string

    override this.ToString() = "ClassType"

[<Struct>]
type StructType =
//    [<DefaultValue>]
//    static val mutable public  FieldPub : string 
    [<DefaultValue>]
    static val mutable private FieldPri : string 

    override this.ToString() = "StructType"    


//ClassType.FieldPub <- "a class"
//if ClassType.FieldPub  <> "a class" then exit 1
//StructType.FieldPub <- "a struct"
//if StructType.FieldPub <> "a struct" then exit 1

exit 0

