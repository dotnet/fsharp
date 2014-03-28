namespace Microsoft.FSharp.Math

module GlobalAssociations =

    open Microsoft.FSharp.Math
    open Microsoft.FSharp.Math.Instances
    open System
    open System.Numerics

    let ComplexNumerics = 
      { new IFractional<_> with 
          member __.Zero = Microsoft.FSharp.Math.Complex.Zero
          member __.One = Microsoft.FSharp.Math.Complex.One
          member __.Add(a,b) = a + b
          member __.Subtract(a,b) = a - b
          member __.Multiply(a,b) = a * b
          member __.Equals(a,b) = (a = b)
          member __.Compare(a,b) = compare a b
          member __.Divide(a,b) = a / b
          member __.Negate(a) = -a
          member __.Abs(a)  = a // not signed
          member __.Sign(a) = 1 // not signed
          member __.Reciprocal(a) =  Microsoft.FSharp.Math.Complex.One / a 
          member __.ToString((x:Microsoft.FSharp.Math.Complex),fmt,fmtprovider) = x.ToString(fmt,fmtprovider)
          member __.Parse(s,numstyle,fmtprovider) = Microsoft.FSharp.Math.Complex.mkRect (System.Double.Parse(s,numstyle,fmtprovider),0.0) }

    let ht = 
        let ht = new System.Collections.Generic.Dictionary<Type,obj>() 
        let optab =
            [ typeof<float>,   (Some(FloatNumerics    :> INumeric<float>) :> obj);
              typeof<int32>,   (Some(Int32Numerics    :> INumeric<int32>) :> obj);
              typeof<int64>,   (Some(Int64Numerics    :> INumeric<int64>) :> obj);
              typeof<BigInteger>,  (Some(BigIntNumerics   :> INumeric<BigInteger>) :> obj);
              typeof<float32>, (Some(Float32Numerics  :> INumeric<float32>) :> obj);
              typeof<Microsoft.FSharp.Math.Complex>, (Some(ComplexNumerics :> INumeric<Microsoft.FSharp.Math.Complex>) :> obj);
              typeof<bignum>,  (Some(BigNumNumerics   :> INumeric<bignum>) :> obj); ]
           
        List.iter (fun (ty,ops) -> ht.Add(ty,ops)) optab;
        ht
        
    let Put (ty: System.Type, d : obj)  =
        lock ht (fun () -> 
            if ht.ContainsKey(ty) then invalidArg "ty" ("the type "+ty.Name+" already has a registered numeric association");
            ht.Add(ty, d))
      
    let TryGetNumericAssociation<'a>() = 
        lock ht (fun () -> 
            let ty = typeof<'a>  
            if ht.ContainsKey(ty) then
                match ht.[ty] with
                | :? (INumeric<'a> option) as r -> r
                | _ -> invalidArg "ty" ("The type "+ty.Name+" has a numeric association but it was not of the correct type")
            else
                None)

    let GetNumericAssociation() = (TryGetNumericAssociation()).Value
    let RegisterNumericAssociation (d : INumeric<'a>)  = Put(typeof<'a>, box(Some d))


