namespace RuntimeAsync.Library

open System
open System.Threading.Tasks

module Api =
    let addFromTaskAndValueTask (left: Task<int>) (right: ValueTask<int>) : Task<int> =
        runtimeTask {
            let! l = left
            let! r = right
            return l + r
        }

    let bindUnitTaskAndUnitValueTask () : Task<string> =
        runtimeTask {
            do! Task.CompletedTask
            do! ValueTask.CompletedTask
            return "completed"
        }

    let safeDivide (x: int) (y: int) : Task<int> =
        runtimeTask {
            try
                if y = 0 then
                    failwith "division by zero"
                return x / y
            with _ ->
                return 0
        }

    let nestedRuntimeTask () : Task<int> =
        runtimeTask {
            let! x = addFromTaskAndValueTask (Task.FromResult 20) (ValueTask<int>(22))
            return x + 2
        }
