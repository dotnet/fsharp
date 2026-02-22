// #Regression #NoMT #CompilerOptions 
// Regression test for FSHARP1.0:3508
// --standalone option can't include .EXE
//<Expects id="FS2007" status="error">Static linking may not include a \.EXE</Expects>
module M

type y() = class
              inherit M.x()
           end

let v = new y()

exit 0
