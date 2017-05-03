// #Warnings
//<Expects status="Error" id="FS0039">The value, constructor, namespace or type 'DateTie' is not defined</Expects>
//<Expects>Maybe you want one of the following:\s+DateTime,?\s+DateTimeKind,?\s+DateTimeOffset</Expects>

let x = System.DateTie.MaxValue

exit 0