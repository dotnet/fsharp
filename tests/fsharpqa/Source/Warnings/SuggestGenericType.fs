// #Warnings
//<Expects status="Error" id="FS0039">The type 'int11' is not defined</Expects>
//<Expects>Maybe you want one of the following:</Expects>
//<Expects>int16</Expects>
//<Expects>int</Expects>

type T = System.Collections.Generic.Dictionary<int11,int>

exit 0