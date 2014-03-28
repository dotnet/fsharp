// #Conformance #ObjectOrientedTypes #TypeExtensions  
#light
namespace NS
    type Lib() =
      class
        [<DefaultValue>]
        val mutable private instanceField : int
        [<DefaultValue>]
        static val mutable private staticField : int
        member x.Name () = "Lib"
        member x.DefProp = 1
     end
 
    type LibGen<'a>() =
      class
        [<DefaultValue(false)>]
        val mutable private instanceField : 'a 
        [<DefaultValue(false)>]
        static val mutable private staticField : 'a
        member x.Name ()  = "LibGen" 
        member x.DefProp = Unchecked.defaultof<'a>
     end
