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

