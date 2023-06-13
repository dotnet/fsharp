// #Regression #Conformance #DeclarationElements #Fields #MemberDefinitions 
// Verify we disallow the creation of public, static fields
//<Expects span="(7,32)" status="error" id="FS0881">Static 'val' fields in types must be mutable, private and marked with the '\[<DefaultValue>\]' attribute\. They are initialized to the 'null' or 'zero' value for their type\. Consider also using a 'static let mutable' binding in a class type\.$</Expects>
//<Expects span="(16,32)" status="error" id="FS0881">Static 'val' fields in types must be mutable, private and marked with the '\[<DefaultValue>\]' attribute\. They are initialized to the 'null' or 'zero' value for their type\. Consider also using a 'static let mutable' binding in a class type\.$</Expects>
type ClassType =
    [<DefaultValue>]
    static val mutable public  FieldPub : string
    [<DefaultValue>]
    static val mutable private FieldPri : string

    override this.ToString() = "ClassType"

[<Struct>]
type StructType =
    [<DefaultValue>]
    static val mutable public  FieldPub : string 
    [<DefaultValue>]
    static val mutable private FieldPri : string 

    override this.ToString() = "StructType"    


ClassType.FieldPub <- "a class"
if ClassType.FieldPub  <> "a class" then failwith "Failed: 1"

StructType.FieldPub <- "a struct"
if StructType.FieldPub <> "a struct" then failwith "Failed: 2"
