// #XMLDoc
// Regression test for Dev11:390683, 388264
//<Expects status="success"></Expects>

// Similar to UnitOfMeasure03, but with the UoM in a DU

open Microsoft.FSharp.Data.UnitSystems.SI.UnitNames

type DU =
    /// <summary>This is A</summary>
    | A of float<ampere/ohm>

    /// <summary>This is B</summary>
    | B of (int<meter> * int64<meter^2>)

let test =
    let myname = System.Reflection.Assembly.GetExecutingAssembly().Location
    let xmlname = System.IO.Path.ChangeExtension(myname, "xml")

#if WITHXMLVERIFICATION
    let xml = new System.Xml.XmlDocument()
    xml.Load(xmlname)

    let a1 = xml.GetElementsByTagName("summary").Item(0).FirstChild.Value = "This is B"
    if not a1 then printfn "a1: expected: This is B"; exit 1

    let a2 = xml.GetElementsByTagName("summary").Item(1).FirstChild.Value = "This is A"
    if not a2 then printfn "a2: expected: This is A"; exit 1
    
    let a3 = xml.GetElementsByTagName("member").Item(0).Attributes.Item(0).Value = "T:UnitOfMeasure04.DU.B"
    if not a3 then printfn "a3: expected: T:UnitOfMeasure04.DU.B"; exit 1

    let a4 = xml.GetElementsByTagName("member").Item(1).Attributes.Item(0).Value = "T:UnitOfMeasure04.DU.A"
    if not a4 then printfn "a4: expected: T:UnitOfMeasure04.DU.A"; exit 1
#endif
    0

test |> exit


