// #NoMT #Import 
#light
namespace NS

// Compiled using [fsc -a] to produce a class library

     type LibGen<'a>() =
      class
        [<DefaultValue(false)>]
        val mutable instanceField : 'a 
     end
