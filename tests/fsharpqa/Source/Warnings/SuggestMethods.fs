// #Warnings
//<Expects status="Error" id="FS0039">The field, constructor or member '.ctor' is not defined.</Expects>
//<Expects>Maybe you want one of the following:</Expects>
//<Expects>Method1</Expects>

module Test2 = 
    type D() = 

       static let x = 1

       member x.Method1() = 10


    D.``.ctor``()
    
exit 0