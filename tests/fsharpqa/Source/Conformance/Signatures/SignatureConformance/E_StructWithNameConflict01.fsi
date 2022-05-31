// #Conformance #Signatures #Structs #Regression
// Regression for Dev11:137942, structs used to not give errors on when member names conflicted with interface members
//<Expects status="error" span="(14,21-14,34)" id="FS0034">Module 'M' contains.    override Foo\.GetEnumerator: unit -> IEnumerator<'T>    .but its signature specifies.    member Foo\.GetEnumerator: unit -> IEnumerator<'T>    .The compiled names differ</Expects>
//<Expects status="notin" span="(18,13-18,26)" id="FS0039">The field, constructor or member 'GetEnumerator' is not defined$</Expects>
module M

open System.Collections.Generic

[<Struct>]
type Foo<'T> = 
    new : 'T -> 'T Foo
    member public GetEnumerator: unit -> IEnumerator<'T> 
    interface IEnumerable<'T>
