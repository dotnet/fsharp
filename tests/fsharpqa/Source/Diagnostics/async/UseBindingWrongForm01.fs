// #Regression #Diagnostics #Async 
// Regression tests for DEV10:916135
//<Expects status="error" span="(8,20-8,22)" id="FS0001">The type 'int \[\]' is not compatible with the type 'System\.IDisposable'$</Expects>
//<Expects status="error" span="(8,15-8,78)" id="FS0001">The type 'int \[\]' is not compatible with the type 'System\.IDisposable'$</Expects>
//<Expects status="error" span="(14,16-14,28)" id="FS1228">'use!' bindings must be of the form 'use! <var> = <expr>'$</Expects>

let ax = async {
              use! r1 = Async.Parallel [| async.Return(1); async.Return(2) |]
              let y = 4        
              return r1         
         }     

let a = async {        
          use! [| r1; r2 |] = Async.Parallel [| async.Return(1); async.Return(2) |]
          let y = 4
          return r1,r2
        }
