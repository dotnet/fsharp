// #Regression #XMLDoc
// Verify that XmlDoc names are generated, but no empty members are generated re: issue #148
//<Expects status=success></Expects>

#light 

namespace MyRather.MyDeep.MyNamespace 
open System.Xml

/// class1
type Class1() = 
    /// x
    member this.X = "X"

type Class2() =
    member this.Y = "Y"
    
///testModule
module MyModule =

    let check (xml:XmlDocument) name xmlDoc =
        let foundDoc = ((xml.SelectSingleNode ("/doc/members/member[@name='" + name + "']")).SelectSingleNode "summary").InnerText.Trim()
        if xmlDoc <> foundDoc then
            printfn "%s: generated xmlDoc <%s> differs from excpected <%s>" name foundDoc xmlDoc
        xmlDoc = foundDoc
    
    let hasEmptyMembers (xml:XmlDocument) =
        let node = xml.SelectSingleNode ("/doc/members/member[@name='']")
        if node <> null then
            printfn "Empty member name entries found." 
        node <> null
        
    let test =
        let myname = System.Reflection.Assembly.GetExecutingAssembly().Location
        let xmlname = System.IO.Path.ChangeExtension(myname, "xml")


#if WITHXMLVERIFICATION
        let xml = new XmlDocument()
        xml.Load xmlname
        if    check xml "P:MyRather.MyDeep.MyNamespace.Class1.X" "x"
           && check xml "T:MyRather.MyDeep.MyNamespace.Class1" "class1"
           && not(hasEmptyMembers xml)
        then 0 else 1
#else
        0
#endif

    test |> exit
