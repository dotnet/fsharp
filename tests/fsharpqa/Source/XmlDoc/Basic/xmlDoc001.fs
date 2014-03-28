// #XMLDoc
// Regression test for FSHARP1.0:850
// Simple /// is xmldoc
//<Expects status=success></Expects>

#light 

/// I'm an xml comment!
type e = 
  | A
  | B
 
let test =
    let myname = System.Reflection.Assembly.GetExecutingAssembly().Location
    let xmlname = System.IO.Path.ChangeExtension(myname, "xml")

#if WITHXMLVERIFICATION
    let xml = new System.Xml.XmlDocument()
    xml.Load(xmlname);
    if System.String.Compare(xml.GetElementsByTagName("summary").Item(0).FirstChild.Value, System.Environment.NewLine ^ " I'm an xml comment!" ^ System.Environment.NewLine) = 0 then 0 else 1
#else
    0 
#endif

test |> exit
