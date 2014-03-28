// #Regression #Conformance #ObjectOrientedTypes #Structs 
#light

// FSB 1281, Indexed Item property on structs with get and set gives System.AccessViolationException in fsi

open System

[<Struct>]
type Dodad =
    val mutable m_intArray    : int[]
    val mutable m_stringArray : string[]
    
    new (startingInts, startingStrings) = {m_intArray = startingInts; 
                                           m_stringArray = startingStrings}

    member this.Item with get (idx : int)         = this.m_intArray.[idx]
                     and  set (idx : int) (x:int) = this.m_intArray.[idx] <- x
                    
    member this.StrItem with get (idx : string)            = this.m_stringArray.[Int32.Parse(idx)]
                        and  set (idx : string) (x:string) = this.m_stringArray.[Int32.Parse(idx)] <- x

let mutable test = new Dodad([|128; 256|], [|"a";"b"|])

// Test construction
if test.[0] <> 128 then exit 1
if test.[1] <> 256 then exit 1
if test.StrItem("0") <> "a" then exit 1
if test.StrItem("1") <> "b" then exit 1

// Test indexer's setter and getter
test.[0] <- -1 
test.[1] <- -2 
if test.[0] <> -1  then exit 1
if test.[1] <> -2  then exit 1

// Text indexed property's setter and getter
test.StrItem("0") <- "zero"
test.StrItem("1") <- "one"
if test.StrItem("0") <> "zero" then exit 1
if test.StrItem("1") <> "one" then exit 1

exit 0
