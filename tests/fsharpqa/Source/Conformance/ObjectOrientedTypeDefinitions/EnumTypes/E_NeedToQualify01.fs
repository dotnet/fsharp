// #Regression #Conformance #ObjectOrientedTypes #Enums 
#light

// FSB 1552, Internal error: badly formed Item_ctor_group.
//<Expects id="FS0039" span="(21,26-21,31)" status="error">The value or constructor 'Black' is not defined</Expects>
//<Expects id="FS0039" span="(22,26-22,31)" status="error">The value or constructor 'White' is not defined</Expects>
//<Expects id="FS0039" span="(23,26-23,31)" status="error">The value or constructor 'Empty' is not defined</Expects>

// Unlike discriminated unions, Enums need to be fully qualified. 

open System

[<FlagsAttribute>]
type Stone =
    | Black     = 0b0001
    | White     = 0b0010
    | Empty     = 0b0100
    | OffBoard  = 0b1000

let blackStoneList = [
                        [Black]; 
                        [White]; 
                        [Empty]
                     ]

// Shouldn't compile
exit 1
