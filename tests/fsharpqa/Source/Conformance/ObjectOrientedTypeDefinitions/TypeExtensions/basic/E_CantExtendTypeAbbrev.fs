// #Regression #Conformance #ObjectOrientedTypes #TypeExtensions 
// Verify you can't add type extensions to a type abbreviation
//<Expects span="(6,6-6,12)" status="error" id="FS0964">Type abbreviations cannot have augmentations$</Expects>
//<Expects span="(7,5-7,33)" status="error" id="FS0895">Type abbreviations cannot have members$</Expects>

type string with
    member this.ReturnFive() = 5
