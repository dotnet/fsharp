// #Regression #XMLDoc
// This used to be regression test for FSHARP1.0:850
// Verify that XmlDoc name is correctly generated
//<Expects status=success></Expects>

#light 

open System.Xml
open MyRather.MyDeep.MyNamespace 

let check (xml:XmlDocument) name xmlDoc =
    xmlDoc = ((xml.SelectSingleNode ("/doc/members/member[@name='" + name + "']")).SelectSingleNode "summary").InnerText.Trim()
let test =
    let myname = System.Reflection.Assembly.GetExecutingAssembly().Location
    let xmlname = System.IO.Path.ChangeExtension(myname, "xml")


#if WITHXMLVERIFICATION
    let xml = new XmlDocument()
    xml.Load xmlname
    if check xml "T:MyRather.MyDeep.MyNamespace.TestEnum.VALUE2" "enumValue2"
       && check xml "T:MyRather.MyDeep.MyNamespace.TestEnum" "test enum"
       && check xml "F:MyRather.MyDeep.MyNamespace.TestRecord.MyProperpty" "Record string"
       && check xml "T:MyRather.MyDeep.MyNamespace.TestRecord" "testRecord"
       && check xml "M:MyRather.MyDeep.MyNamespace.myModule.testAnd(System.Boolean,System.Boolean)" "my function"
       && check xml "P:MyRather.MyDeep.MyNamespace.myModule.myVariable" "integer value"
       && check xml "T:MyRather.MyDeep.MyNamespace.myModule" "my module comment"
    then 0 else 1
#else
    0 
#endif

test |> exit
