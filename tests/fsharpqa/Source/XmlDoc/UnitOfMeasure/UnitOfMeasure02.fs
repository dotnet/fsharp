// #XMLDoc
// Regression test for Dev11:390683, 388264
//<Expects status="success"></Expects>

// Similar to UnitOfMeasure01, but with 2 params

open Microsoft.FSharp.Data.UnitSystems.SI.UnitNames

/// <summary>This is A</summary>
/// <param name="pA">This is pA</param>
let A (pA : float<ampere/ohm>) = pA / 1.<ampere>

/// <summary>This is B</summary>
/// <param name="pB1">This is pB1</param>
/// <param name="pB2">This is pB2</param>
let B (pB1 : int<meter>, pB2 : int<meter^2>) = pB1 * pB2

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
    
    let a3 = xml.GetElementsByTagName("member").Item(1).Attributes.Item(0).Value = "M:UnitOfMeasure02.B(System.Int32,System.Int32)"
    if not a3 then printfn "a3: expected: M:UnitOfMeasure02.B(System.Int32,System.Int32)"; exit 1

    let a4 = xml.GetElementsByTagName("member").Item(2).Attributes.Item(0).Value = "M:UnitOfMeasure02.A(System.Double)"
    if not a4 then printfn "a4: expected: M:UnitOfMeasure02.A(System.Double)"; exit 1
#endif
    0

test |> exit
