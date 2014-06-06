// #Regression #XMLDoc
// Verify that XmlDoc name is correctly generated
//<Expects status=success></Expects>

#light 

namespace MyRather.MyDeep.MyNamespace 
open System.Xml

///shape
type Shape =
    | Rectangle of width : float * length : float
    ///circle
    | Circle of radius : float
    | Prism of width : float * float * height : float

///freeRecord
type Enum = 
    ///xxx
    | XXX = 3
    ///zzz
    | ZZZ = 11

///testModule
module MyModule =
    ///testRecord
    type TestRecord = { 
        /// Record string
        MyProperpty : string 
    } 

    ///point3D
    type Point3D =
       struct 
          ///point.x
          val x: float
          ///point.y
          val y: float
          val z: float
       end 

    ///test enum
    type TestEnum = 
        ///enumValue1
        | VALUE1 = 1
        ///enumValue2
        | VALUE2 = 2

    ///test union
    type TestUnion = 
        ///union - enum
        | TestEnum
        ///union - record
        | TestRecord

    ///testClass
    type MyGenericClass<'a> (x: 'a) = 
        ///testClass value
        let data = x
        
        do printfn "%A" x
        ///testClass member
        member this.TestMethod() =
            printfn "test method"
    
    ///nested module
    module MyNestedModule = 
        ///testRecord nested
        type TestRecord = { 
            /// Record string nested
            MyProperpty : string 
        } 

        ///test enum nested
        type TestEnum = 
            ///enumValue1 nested
            | VALUE1 = 1
            ///enumValue2 nested
            | VALUE2 = 2

        ///test union nested
        type TestUnion = 
            ///union - enum nested
            | TestEnum
            ///union - record nested
            | TestRecord

        ///testClass nested
        type MyGenericClass<'a> (x: 'a) = 
            ///testClass value nested
            let mutable data = x
            
            do printfn "%A" x
            ///testClass member nested
            member this.TestMethod() =
                printfn "test method"
            /// A read-write property. 
            member this.MyReadWriteProperty
                with get () = data
                and set (value) = data <- value                

    let check (xml:XmlDocument) name xmlDoc =
        let foundDoc = ((xml.SelectSingleNode ("/doc/members/member[@name='" + name + "']")).SelectSingleNode "summary").InnerText.Trim()
        if xmlDoc <> foundDoc then
            printfn "%s: generated xmlDoc <%s> differs from excpected <%s>" name foundDoc xmlDoc
        xmlDoc = foundDoc

    let test =
        let myname = System.Reflection.Assembly.GetExecutingAssembly().Location
        let xmlname = System.IO.Path.ChangeExtension(myname, "xml")


#if WITHXMLVERIFICATION
        let xml = new XmlDocument()
        xml.Load xmlname
        if check xml "F:MyRather.MyDeep.MyNamespace.Enum.ZZZ" "zzz"
           && check xml "F:MyRather.MyDeep.MyNamespace.Enum.XXX" "xxx"
           && check xml "T:MyRather.MyDeep.MyNamespace.Enum" "freeRecord"
           && check xml "T:MyRather.MyDeep.MyNamespace.Shape.Circle" "circle"
           && check xml "T:MyRather.MyDeep.MyNamespace.Shape" "shape"
           && check xml "M:MyRather.MyDeep.MyNamespace.MyModule.MyGenericClass`1.TestMethod" "testClass member"
           && check xml "T:MyRather.MyDeep.MyNamespace.MyModule.MyGenericClass`1" "testClass"
           && check xml "T:MyRather.MyDeep.MyNamespace.MyModule.TestUnion.TestRecord" "union - record"
           && check xml "T:MyRather.MyDeep.MyNamespace.MyModule.TestUnion.TestEnum" "union - enum"
           && check xml "T:MyRather.MyDeep.MyNamespace.MyModule.TestUnion" "test union"
           && check xml "F:MyRather.MyDeep.MyNamespace.MyModule.TestEnum.VALUE2" "enumValue2"
           && check xml "F:MyRather.MyDeep.MyNamespace.MyModule.TestEnum.VALUE1" "enumValue1"
           && check xml "T:MyRather.MyDeep.MyNamespace.MyModule.TestEnum" "test enum"
           && check xml "F:MyRather.MyDeep.MyNamespace.MyModule.TestRecord.MyProperpty" "Record string"
           && check xml "T:MyRather.MyDeep.MyNamespace.MyModule.TestRecord" "testRecord"
           && check xml "M:MyRather.MyDeep.MyNamespace.MyModule.MyNestedModule.MyGenericClass`1.TestMethod" "testClass member nested"
           && check xml "T:MyRather.MyDeep.MyNamespace.MyModule.MyNestedModule.MyGenericClass`1" "testClass nested"
           && check xml "T:MyRather.MyDeep.MyNamespace.MyModule.MyNestedModule.TestUnion.TestRecord" "union - record nested"
           && check xml "T:MyRather.MyDeep.MyNamespace.MyModule.MyNestedModule.TestUnion.TestEnum" "union - enum nested"
           && check xml "T:MyRather.MyDeep.MyNamespace.MyModule.MyNestedModule.TestUnion" "test union nested"
           && check xml "F:MyRather.MyDeep.MyNamespace.MyModule.MyNestedModule.TestEnum.VALUE2" "enumValue2 nested"
           && check xml "F:MyRather.MyDeep.MyNamespace.MyModule.MyNestedModule.TestEnum.VALUE1" "enumValue1 nested"
           && check xml "T:MyRather.MyDeep.MyNamespace.MyModule.MyNestedModule.TestEnum" "test enum nested"
           && check xml "F:MyRather.MyDeep.MyNamespace.MyModule.MyNestedModule.TestRecord.MyProperpty" "Record string nested"
           && check xml "T:MyRather.MyDeep.MyNamespace.MyModule.MyNestedModule.TestRecord" "testRecord nested"
           && check xml "T:MyRather.MyDeep.MyNamespace.MyModule.MyNestedModule" "nested module"
           && check xml "T:MyRather.MyDeep.MyNamespace.MyModule" "testModule"
           && check xml "T:MyRather.MyDeep.MyNamespace.MyModule.Point3D" "point3D"
           && check xml "F:MyRather.MyDeep.MyNamespace.MyModule.Point3D.x" "point.x"
           && check xml "F:MyRather.MyDeep.MyNamespace.MyModule.Point3D.y" "point.y"
           && check xml "P:MyRather.MyDeep.MyNamespace.MyModule.MyNestedModule.MyGenericClass`1.MyReadWriteProperty(`0)" "A read-write property."
           && check xml "P:MyRather.MyDeep.MyNamespace.MyModule.MyNestedModule.MyGenericClass`1.MyReadWriteProperty" "A read-write property."
        then 0 else 1
#else
        0
#endif

    test |> exit
