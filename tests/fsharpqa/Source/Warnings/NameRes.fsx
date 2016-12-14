// #Warnings
//<Expects status="success"></Expects>

#load "Processes.fs"
#load "Supervisor.fs"

open Foo

let _ : DeadStatus = DoesNotExist
 
exit 0