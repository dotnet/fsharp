// #Regression #XMLDoc
// This used to be regression test for FSHARP1.0:850
// Now, we simple verify that OCaml style xml-doc are gone (i.e. treated as regular comments)
// Verify that //// is not xmldoc (after an ocaml-style xml-comment)
//<Expects status="success"></Expects>

#light 

(** I'm an xml comment! *)
//// I am NOT an xml comment
type e = 
  | A
  | B
 
let test =
    let myname = System.Reflection.Assembly.GetExecutingAssembly().Location
    let xmlname = System.IO.Path.ChangeExtension(myname, "xml")

#if WITHXMLVERIFICATION
    let xml = new System.Xml.XmlDocument()
    xml.Load(xmlname);
    if xml.GetElementsByTagName("summary").Count = 0 then 0 else 1
#else
    0 
#endif

test |> exit
