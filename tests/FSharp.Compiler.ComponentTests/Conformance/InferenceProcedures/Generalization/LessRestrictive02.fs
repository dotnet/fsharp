// #Regression #Conformance #TypeInference 
// Regression test for FSharp1.0:3187
// Title: better inference for mutually recursive generic classes
// Descr: Verify types are inferred correctly for generic classes

module Example_Chris = 
    open System

    // Type to represent a stateful function. A function which takes an input state and
    // returns a result combined with an updated state.
    type StatefulFunc<'state, 'result> = StatefulFunc of ('state -> 'result * 'state)

    // Run our stateful function
    let Run (StatefulFunc f) initialState = f initialState

    type StateBuilder() = 

        member this.Bind(
                            result : StatefulFunc<'state, 'a>, 
                            restOfComputation : 'a -> StatefulFunc<'state, 'b>
                        ) = 
                        
            StatefulFunc(fun initialState ->
                let result, updatedState = Run result initialState
                Run (restOfComputation result) updatedState
            )

        member this.Using(
                            x : 'a when 'a :> IDisposable, 
                            restOfComputation : 'a -> StatefulFunc<'state, 'b>
                        ) = 
                            
            StatefulFunc(fun initialState ->
                try
                    Run (restOfComputation x) initialState
                finally
                    x.Dispose()
            )
        
        member this.Combine(
                             partOne : StatefulFunc<'state, unit>, 
                             partTwo : StatefulFunc<'state, 'a>
                            ) = 
                            
            StatefulFunc(fun initialState ->
                let (), updatedState = Run partOne initialState
                Run partTwo updatedState
            )
        
        member this.Delay(
                            restOfComputation : unit -> StatefulFunc<'state, 'a>
                        ) = 

            StatefulFunc (fun initialState -> 
                Run ( restOfComputation() ) initialState
            )
        
        member this.For(elements : seq<'a>, forBody : ('a -> StatefulFunc<'state, unit>)) = 
            
            StatefulFunc(fun initialState ->
                let mutable state = initialState
                
                for e in elements do
                    let (), updatedState = Run (forBody e) (state)
                    state <- updatedState
            
                // Return unit * finalState
                (), state
            )
            
        member this.Return(x : 'a) = 
            StatefulFunc(fun initialState -> x, initialState)
        
        member this.TryFinally(
                                tryBlock : StatefulFunc<'state, 'a>, 
                                finallyBlock : unit -> unit
                            ) = 
                            
            StatefulFunc(fun initialState ->
                try
                    Run tryBlock initialState
                finally
                    finallyBlock()
            )
                
        
        member this.TryWith(
                            tryBlock : StatefulFunc<'state, 'a>, 
                            exnHandler : exn -> StatefulFunc<'state, 'a>
                        ) = 

            StatefulFunc(fun initialState ->
                try
                    Run tryBlock initialState
                with
                | e -> 
                    Run (exnHandler e) initialState
            )       

        member this.While(
                            predicate : unit -> bool, 
                            body : StatefulFunc<'state, unit>
                        ) = 
                    
            StatefulFunc(fun initialState ->    

                let state = ref initialState
                while predicate() = true do
                    let (), updatedState = Run body (!state)
                    state := updatedState
                    
                // Return unit * finalState
                (), !state
            )
        
        member this.Zero() = 
            StatefulFunc(fun initialState -> (), initialState)

    // Declare the state workflow builder
    let state = StateBuilder()

    // Primitive functions for getting and setting state
    let GetState          = StatefulFunc (fun state -> state, state)
    let SetState newState = StatefulFunc (fun prevState -> (), newState)

    let Add x =
        state { 
            let! currentTotal, history = GetState
            do! SetState (currentTotal + x, (sprintf "Added %d" x) :: history) 
        }
        
    let Subtract x =
        state {
            let! currentTotal, history = GetState
            do! SetState (currentTotal - x, (sprintf "Subtracted %d" x) :: history) 
        }
        
    let Multiply x =
        state { 
            let! currentTotal, history = GetState
            do! SetState (currentTotal * x, (sprintf "Multiplied by %d" x) :: history) 
        }

    let calculatorActions =
        state {
            do! Add 2
            do! Multiply 10
            do! Subtract 3
            return "Finished" 
        }
