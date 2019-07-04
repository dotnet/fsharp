// #Regression #Conformance #TypesAndModules #Records 
// Verify appropriate error with ambiguous record inference
// Same as 01, but with generic types
// Regression test for FSHARP1.0:2780
//<Expects id="FS0764" span="(13,20-13,30)" status="error">No assignment given for field 'Y' of type 'N\.M\.Blue+</Expects>
namespace N
module M =

    type GRed<'a>  = { GA : 'a }
    type Blue<'a> = { GX : 'a; Y : int }

    let gaBlue   = { GX = 0; Y = 1 }
    let gunknown = { GX = 0 }
