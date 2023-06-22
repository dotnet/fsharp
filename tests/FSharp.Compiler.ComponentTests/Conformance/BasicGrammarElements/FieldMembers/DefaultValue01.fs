// #Conformance #DeclarationElements #Fields #MemberDefinitions 
// Declare a mutable class field via default value
// You can't set the field in an explicit ctor using the record initialization
// syntax, so you have to resort to using a 'then' expression afterwards. :(
module DefaultValue01

type Point =
    [<DefaultValue>]
    val mutable public m_x : float
  
    val mutable public m_y : float

    new(x, y) as this = { m_y = y } then this.m_x <- x


let t1 = new Point(1.0, 2.0)
if t1.m_x <> 1.0 then failwith "Failed: 1"
if t1.m_y <> 2.0 then failwith "Failed: 2"
