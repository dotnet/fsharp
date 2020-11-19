// #Regression #Conformance 
// Regression test for FSHARP1.0:3330
// 
#light


let mutable times_x_was_computed = 0
let mutable times_y_was_computed = 0
let mutable times_c_was_computed = 0

//Simple class to store a single variable
type AClass ( initialValue:int) = 
    let variable = initialValue
    
    member x.Variable = variable

//function which returns a function which returns a function
let f a = 

    //pre-computation of x
    let x =
        times_x_was_computed <- times_x_was_computed + 1
        a * a
        
    //function which returns a function
    fun (b:AClass) ->
        //pre-computation of y - this should be done only once!
        let y =
            times_y_was_computed <- times_y_was_computed + 1
            b.Variable + x
            
        fun c ->
            times_c_was_computed <- times_c_was_computed + 1
            c + y
            
do
    let g = f 1
    let innerFunction = g (AClass(2))
    
    {1 .. 5}
    |> Seq.iter (fun i ->
        innerFunction i |> ignore
    )

if times_x_was_computed <> 1 then 
                                   printfn "value x computed %d time(s); expected 1 time" times_x_was_computed 
                                   exit 1

if times_y_was_computed <> 1 then 
                                   printfn "value y computed %d time(s); expected 1 time" times_y_was_computed 
                                   exit 1

if times_c_was_computed <> 5 then 
                                   printfn "value c computed %d time(s); expected 5 times" times_c_was_computed 
                                   exit 1

exit 0
