// #XMLDoc
// Regression test for Dev11:40070
//<Expects status="success"></Expects>

/// Doc comment for Test.
type Test = 
  /// Doc comment for Field1
  | Field1 = 0
  /// Doc comment for Field2
  | Field2 = 1
 
let test =
    let myname = System.Reflection.Assembly.GetExecutingAssembly().Location
    let xmlname = System.IO.Path.ChangeExtension(myname, "xml")

#if WITHXMLVERIFICATION
    let xml = new System.Xml.XmlDocument()
    xml.Load(xmlname);
    if System.String.Compare(xml.GetElementsByTagName("summary").Item(2).FirstChild.Value, System.Environment.NewLine + " Doc comment for Test." + System.Environment.NewLine) = 0 then 0 else 1
    if System.String.Compare(xml.GetElementsByTagName("summary").Item(1).FirstChild.Value, System.Environment.NewLine + " Doc comment for Field1" + System.Environment.NewLine) = 0 then 0 else 1
    if System.String.Compare(xml.GetElementsByTagName("summary").Item(0).FirstChild.Value, System.Environment.NewLine + " Doc comment for Field2" + System.Environment.NewLine) = 0 then 0 else 1
#else
    0 
#endif

test |> exit
