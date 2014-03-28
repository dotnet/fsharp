// #Regression #XMLDoc #OCaml
// This used to be regression test for FSHARP1.0:1561
// Now that the OCaml style XmlDoc is gone, (** ... *) should be just a normal comment.
// We want to verify that no xml with a "summary" is generate.
// See also FSHARP1.0:1654
//<Expects status=success></Expects>

#light 

(** I'm an xml comment! *)
type e = 
  | A
  | B

let x = e.A

let test =
    let myname = System.Reflection.Assembly.GetExecutingAssembly().Location
    let xmlname = System.IO.Path.ChangeExtension(myname, "xml")

#if WITHXMLVERIFICATION
    let xml = new System.Xml.XmlDocument()
    xml.Load(xmlname);
    // System.Console.WriteLine("{0}", xml.GetElementsByTagName("summary").Item(0).FirstChild.Value)
    if xml.GetElementsByTagName("summary").Count = 0 then 0 else 1
#else
    0 
#endif

test |> exit
