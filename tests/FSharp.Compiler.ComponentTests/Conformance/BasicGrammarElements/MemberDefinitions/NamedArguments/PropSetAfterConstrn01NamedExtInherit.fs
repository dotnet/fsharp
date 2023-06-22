// #Regression #Conformance #DeclarationElements #MemberDefinitions #NamedArguments 
#light

module GenericInheritedClass2 =
    type R =
        class
           val mutable w : System.Collections.Generic.List<int>
           member obj.W with set(v) = obj.w <- v
           new() = { w = new System.Collections.Generic.List<int>()}
        end

    type S =        
        class
           inherit R
           val mutable x : int
           val mutable y : string
           member obj.X with set(v) = obj.x <- v
           member obj.Y with set(v) = obj.y <- v
           new(a,b) = { inherit R(); x=a; y=b }
        end
module GenericInheritedClassExt2 =
    type GenericInheritedClass2.S with
        member x.A with set v = x.X  <- v + 1
        member x.B with set v = x.Y  <- v + "1"
    type GenericInheritedClass2.R with
        member x.C with set v =  v |> Seq.iter x.w.Add 

    // Standard construction
    let x1 = GenericInheritedClass2.S(1,"1", A = 2, B = "2",C = [ 3] )
    if x1.x <> 3   then failwith "Failed: 1"
    if x1.y <> "21" then failwith "Failed: 2"
    if x1.w.Count <> 1 then failwith "Failed: 1"
