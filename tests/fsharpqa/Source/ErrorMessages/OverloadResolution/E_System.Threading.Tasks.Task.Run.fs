open System
open System.Threading.Tasks
let makeTask () = new Task<_>(fun () -> 1)
let task = makeTask()
let rt1 = Task.Run(fun () -> task)
let rt2 = Task.Run(Func<_>(fun () -> task))
let rt3 = Task.Run<_>(Func<_>(fun () -> task))
let rt4 : Task<_> = Task.Run<_>(Func<_>(fun () -> task))
let rt5 = Task.Run(Func<Task<_>>(fun () -> task))
let rt6 = Task.Run(Func<Task<int>>(fun () -> task))
let rt7 = Task.Run<int>(Func<Task<int>>(fun () -> task))
let rt8 = Task.Run<int>(Func<Task<_>>(fun () -> task))
let rt9 = Task.Run<int>(Func<_>(fun () -> task))
let rt10 = Task.Run<int>(fun () -> task)

//<Expects id="FS0039" status="error" span="(5,11-5,35)">A unique overload for method 'Run' could not be determined based on type information prior to this program point. A type annotation may be needed.</Expects>
//<Expects id="FS0039" status="error" span="(5,11-5,35)">Argument given: unit -> Task<int></Expects>
//<Expects id="FS0039" status="error" span="(5,11-5,35)">Candidates:</Expects>
//<Expects id="FS0039" status="error" span="(5,11-5,35)"> - Task.Run<'TResult>(function: Func<'TResult>) : Task<'TResult></Expects>
//<Expects id="FS0039" status="error" span="(5,11-5,35)"> - Task.Run<'TResult>(function: Func<Task<'TResult>>) : Task<'TResult></Expects>