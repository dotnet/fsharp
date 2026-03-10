// #Conformance #DeclarationElements #ObjectConstructors 
// Verify ability to pass parameters in obj ctor to constructor of base class.

type s1 = string
type s2 = string
type s3 = string


type Grandfather (name : s1) =
    let m_name = name
    member this.GrandfatherName = m_name
    member this.GetName() = m_name

type Father (name : s2, gfName : s1) =
    inherit Grandfather(gfName)
    let m_name = name
    member this.FatherName = m_name
    member this.GetName() = sprintf "%s son of %s" m_name ((this :> Grandfather).GetName())
    
type Son (name : s3, fName : s2, gfName : s1) =
    inherit Father(fName, gfName)
    let m_name = name
    member this.SonName = m_name
    member this.GetName() = sprintf "%s son of %s" m_name ((this :> Father).GetName())

let test1 = new Son("bart", "homer", "abraham")
if test1.GetName() <> "bart son of homer son of abraham" then exit 1

exit 0
