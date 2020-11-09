// #Regression #Conformance #DeclarationElements #Fields #MemberDefinitions 


// Verify that static fields must be marked mutable.
//<Expects status="error" span="(11,20)" id="FS0880">Uninitialized 'val' fields must be mutable and marked with the '\[<DefaultValue>\]' attribute\. Consider using a 'let' binding instead of a 'val' field\.$</Expects>
//<Expects status="error" span="(11,20)" id="FS0881">Static 'val' fields in types must be mutable, private and marked with the '\[<DefaultValue>\]' attribute\. They are initialized to the 'null' or 'zero' value for their type\. Consider also using a 'static let mutable' binding in a class type\.$</Expects>

type Foo() =
    class
        [<DefaultValue>]
        static val thingey : int
    end

