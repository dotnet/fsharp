// #XMLDoc
// Regression test for Dev11:390683, 388264
//<Expects status="success"></Expects>

open Microsoft.FSharp.Data.UnitSystems.SI.UnitNames

/// <summary>This is A</summary>
/// <param name="p1">This is pA</param>
let A (pA : float<ampere>) = pA / 1.<ampere>

/// <summary>This is B</summary>
/// <param name="p1">This is pB</param>
let B (pB : single) = 1.f<ampere> * pB

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

    let a3 = xml.GetElementsByTagName("member").Item(1).Attributes.Item(0).Value = "M:UnitOfMeasure01.B(System.Single)"
    if not a3 then printfn "a3: expected: M:UnitOfMeasure01.B(System.Single)"; exit 1

    let a4 = xml.GetElementsByTagName("member").Item(2).Attributes.Item(0).Value = "M:UnitOfMeasure01.A(System.Double)"
    if not a4 then printfn "a4: expected: M:UnitOfMeasure01.A(System.Double)"; exit 1
#endif
    0

test |> exit
