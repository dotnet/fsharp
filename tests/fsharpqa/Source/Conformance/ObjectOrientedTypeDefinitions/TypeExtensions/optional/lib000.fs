// #Conformance #ObjectOrientedTypes #TypeExtensions  
#light
namespace NS
    type Lib() =
      class
        [<DefaultValue>]
        val mutable instanceField : int
        //[<DefaultValue>]
        // static val mutable staticField : int
        static let mutable staticField = 0
        member x.Name () = "Lib"
        member x.DefProp = 1
     end
 
    type LibGen<'a>() =
      class
        [<DefaultValue(false)>]
        val mutable instanceField : 'a 
        //[<DefaultValue(false)>]
        //static val mutable staticField : 'a
        static let mutable staticField = Unchecked.defaultof<'a>
        member x.Name ()  = "LibGen" 
        member x.DefProp = Unchecked.defaultof<'a>
     end
