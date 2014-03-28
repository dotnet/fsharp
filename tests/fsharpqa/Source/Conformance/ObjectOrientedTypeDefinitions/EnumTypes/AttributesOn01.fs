// #Conformance #ObjectOrientedTypes #Enums 
#light

// Verify ability to put attributes on Enum tags

open System
open System.Reflection
open System.Xml.Serialization

type EnumType = 
    |[<XmlEnum>] A = 1
    |[<XmlEnum>] B = 2




[<STAThread; EntryPoint>]
let main (args: string[]) = 
   
    let anenum = new EnumType()
    let tp = anenum.GetType()
   
    let memberinfoa = Attribute.GetCustomAttribute(tp.GetMember("A").[0], typeof<XmlEnumAttribute>)
    let memberinfob = Attribute.GetCustomAttribute(tp.GetMember("B").[0], typeof<XmlEnumAttribute>)
    
    if memberinfoa = null then exit 1
    if memberinfob = null then exit 1
    
    0
