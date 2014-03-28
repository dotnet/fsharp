// check that a decent error is given when a constructor is too polymorphic
module Test

type Gaussian1D =
   class 
      member x.Foot with get() = failwith ""
   end


module AccessingProtectedMembersFromUnderLambdas = begin
    type DS() = class
      inherit System.Data.DataSet()
      member t.Foo () =
        let a = new DS() in
        (fun () -> a.GetSchemaSerializable()) |> ignore;   //should give error
        let f () = a.GetSchemaSerializable() in           //should give error
        let x = { new System.Object() with GetHashCode() = (a.GetSchemaSerializable() |> ignore); 3 } in // should give error
        let h = new System.EventHandler(fun _ _ -> (a.GetSchemaSerializable() |> ignore)) in // should give error
        // check that protected members from the surrounding context can be used
        // in the arguments to an object constructor.
        let obj = { new System.Collections.ArrayList( (a.GetSchemaSerializable() |> ignore; 3)) with 
                       member x.ToString() = "" 
                    end } in
        ()
    end
end


[<Struct>]
type C(x) =
   member y.Z = x

module ImplicitClassCOnstructionMayNotUseExplicitCOnstruction_Bug_1341_FSharp_1_0 = begin
    type s = class 
     new () = {}
    end

    type t (x:s) = class
     new () = { }
    end

    type t2 (x:s) = class
     let x = 1
     new () = { }
    end

    type t3 () = class
     new (x:int) = { }
    end

end
