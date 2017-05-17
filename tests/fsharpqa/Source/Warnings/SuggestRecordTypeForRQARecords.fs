// #Warnings
//<Expects status="Error" id="FS0039">The record label 'Field1' is not defined.</Expects>
//<Expects>Maybe you want one of the following:\s+MyRecord.Field1</Expects>

[<RequireQualifiedAccess>]
type MyRecord = {
    Field1: string
    Field2: int
}

let r = { Field1 = "hallo"; Field2 = 1 }
    
exit 0