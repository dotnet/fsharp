// #Conformance #DeclarationElements #Fields #MemberDefinitions 
// Sanity check static mutable fields on classes. (Should have default value / null)

type Foo() =
    [<DefaultValue>]
    static val mutable private defaultStr : string
    [<DefaultValue>]
    static val mutable private defaultInt : int

    static member Check1() = Foo.defaultInt <> 0
    static member Check2() = Foo.defaultStr <> null
    

if Foo.Check1() then failwith "Failed: 1"
if Foo.Check2() then failwith "Failed: 2"
