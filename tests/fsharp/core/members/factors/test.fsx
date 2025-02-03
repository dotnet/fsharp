// #Regression #Conformance #MemberDefinitions #ObjectOrientedTypes #Classes 
//---------------------------------------------------------------
// lists.fs		F# source file of generic data types
//
// 2006 written by Ralf Herbrich
// Microsoft Research Ltd.
//---------------------------------------------------------------

#if TESTS_AS_APP
module Core_members_factors
#endif

let failures = ref []

let report_failure (s : string) = 
    stderr.Write" NO: "
    stderr.WriteLine s
    failures := !failures @ [s]

let test (s : string) b = 
    stderr.Write(s)
    if b then stderr.WriteLine " OK"
    else report_failure (s)

let check s b1 b2 = test s (b1 = b2)

open System.Collections.Generic

type Matrix = M of int
  with 
   member x.NoRows = 0 
   member x.NoColumns = 0 
   member x.Item with get((i:int),(j:int)) = 0.0
  end
  
module Types = begin
  /// Shorthand notation for .NET 2.0 Lists
  type ResizeArray<'a> = List<'a>
end

open Types
module ResizeArray = begin
  /// Iterates the function f for every element of the ResizeArray.
  let iter f (r:ResizeArray<_>) = r.ForEach (fun x -> f x)

  /// Creates a ResizeArray from a List.
  let of_list l :ResizeArray<_> = new ResizeArray<_>(List.toArray l)
end

//---------------------------------------------------------------
// distribution.fs		F# source file of the abstract distribution class
//
// 2006 written by Ralf Herbrich
// Microsoft Research Ltd.
//---------------------------------------------------------------


//***************************************************
// Abstract distribution base class 
//***************************************************

/// An abstract class for probability distributions in the exponential family.
type IDistribution = 
  interface 
        /// Gets a sample from the distribution.
        abstract member Sample : int -> System.Random -> Matrix
        /// Computes the probability density value at a particular point.
        abstract member Density : Matrix -> float
        /// Computes the absolute change between two distributions.
        abstract member AbsoluteDifference : IDistribution -> float
  end
  
//***************************************************
// Abstract distribution operation class 
//***************************************************

/// The list of distribution operations (at the moment, this is just the constant distribution).
type DistributionOps<'Distribution> =
  interface
    /// The constant function distribution.
    abstract One : 'Distribution
  end

//---------------------------------------------------------------
// distributions.fs		F# source file of the distributions library
//
// 2006 written by Ralf Herbrich
// Microsoft Research Ltd.
//---------------------------------------------------------------

   
//***************************************************
// 1D Gaussian
//***************************************************

/// The 1D Gaussian class for probability distribution.
type Gaussian1D =
  class 
    /// The precision mean of the Gaussian
    val tau : float
    /// The precision of the Gaussian
    val pi : float
    
    /// The standard constructor.
    new () = 
      { tau = 0.0; pi = 0.0; }
    /// The parameterized constructor.
    new (precisionMean, precision) = 
      { tau = precisionMean; pi = precision; }
    
    /// Precision of the Gaussian.
    member x.Precision with get() = x.pi
    /// Precision times mean of the Gaussian.
    member x.PrecisionMean with get() = x.tau
    /// Mean of the Gaussian (Mu).
    member x.Mean with get () = x.tau / x.pi
    /// Variance of the Gaussian (Sigma^2).
    member x.Variance with get () = 1.0 / x.pi

    member x.Density (point:Matrix) = 
        if (point.NoRows > 1 || point.NoColumns > 1) then
          failwith "This is a 1D distribution which cannot have a density of multidimensional points."
        else
          let diff = point.Item(1,1) - x.Mean in 
          sqrt (x.Precision / (2.0 * System.Math.PI)) * exp (-diff * x.Precision * diff / 2.0)
          
      /// Absolute difference between two Gaussians 
    member x.AbsoluteDifference (y: Gaussian1D) = 
      max (abs (x.PrecisionMean - y.PrecisionMean)) (abs (x.Precision - y.Precision)) 
        
      /// Samples a 1D Gaussian
    member x.Sample (numberOfSamples:int) random = M 1
    
    interface IDistribution with 
      override x.Density point =  x.Density (point)
      override x.AbsoluteDifference distribution = 
          match distribution with
          | :? Gaussian1D as gaussian1D -> x.AbsoluteDifference (gaussian1D)
          | _ -> failwith "Wrong distribution"

      override x.Sample numberOfSamples random = x.Sample numberOfSamples random
    end
    
    /// String representation of a 1D Gaussian
    override x.ToString() = "[" + x.Mean.ToString () + "," + (sqrt (x.Variance)).ToString () + "]"
  end

//***************************************************
// The distribution operations of a 1D Gaussian
//***************************************************

let GaussianDistributionOps = { new DistributionOps<Gaussian1D>  with
                                    member __.One = new Gaussian1D (0.0 , 0.0) }



//---------------------------------------------------------------
// factorgraph.fs	F# source file of the factor graph library
//
// 2006 written by Ralf Herbrich
// Microsoft Research Ltd.
//---------------------------------------------------------------

open System.Collections.Generic

//***************************************************
// The variable node interface
//***************************************************

/// A single variable node in a factor graph. This is the non-mutable interface.
type IVariableNode =
  interface
    /// The marginal distribution of the variable.
    abstract Distribution : IDistribution
  end

//***************************************************
// The specific variable node class
//***************************************************

/// A single variable in a factor graph.
type VariableNode<'Distribution> when 'Distribution :> IDistribution =
  class
    interface IVariableNode with
      /// Just return the distribution
      member x.Distribution = x.distribution :> IDistribution
    end
    
    /// The marginal distribution of the variable.
    val mutable distribution : 'Distribution
    
    /// Sets up a new variable node
    new (dOps : DistributionOps<_>) = { distribution = dOps.One; }
  end

//***************************************************
// The factor node interface
//***************************************************

/// The computation nodes (i.e. factor nodes) of a factor graph.
type IFactorNode =
  interface
    /// The list of all variables that this factor "talks" to.
    abstract VariableNodes : IEnumerable< IVariableNode >
    /// The list of messages from the factor to all its variables.
    abstract Messages : IEnumerable< IDistribution >
    /// Abstract update (computation) mechanism
    abstract UpdateMessage : int -> float 
  end

(*
/// A factor graph node.
type FactorGraphNode = 
  {
    /// The internal ID of a factor graphs node.
    ID		: int;
    /// The X coordinate of the factor graph node.
    X		: float;
    /// The Y coordinate of the factor graph node.
    Y		: float;
    /// The name of the node.
    Name	: string;
  }*)

//---------------------------------------------------------------
// factornodes.fs	F# source file of several factor nodes
//
// 2006 written by Ralf Herbrich
// Microsoft Research Ltd.
//---------------------------------------------------------------

open System

(*
let Gaussian1DPriorFactorNode ((var: VariableNode<Gaussian1D>), mean, variance) =
  //let message = ref  GaussianDistributionOps.One in 
  { new IFactorNode 
  with UpdateMessage i =
      if i > 0 then 
        raise (new ArgumentOutOfRangeException ("iVariableIndex", "This factor only points to one variable."));
      let oldMarginal = var.distribution in
      let newMarginal = new Gaussian1D (mean / variance + oldMarginal.PrecisionMean, 1.0 / variance + oldMarginal.Precision) in
      var.distribution <- newMarginal;
      oldMarginal.AbsoluteDifference (newMarginal)
  and get_Messages() = Seq.ofList [ ] //(!message :> IDistribution) ]
  and get_VariableNodes() = Seq.ofList [ (var :> IVariableNode) ] }
*)

let OneVariableNode((var: VariableNode<_>),f) =
 // let message = ref  (new Gaussian1D(0.0,0.0) ) in 
  { new IFactorNode  with
      member __.UpdateMessage i =
          if i > 0 then 
            raise (new ArgumentOutOfRangeException ("iVariableIndex", "This factor only points to one variable."));
          let oldMarginal = var.distribution in
          let newMarginal = f oldMarginal in
          var.distribution <- newMarginal;
          (oldMarginal :> IDistribution).AbsoluteDifference (newMarginal)
      member __.Messages = Seq.ofList [ (* (!message :> IDistribution) *) ]
      member __.VariableNodes = Seq.ofList [ (var :> IVariableNode) ] }

let Gaussian1DPriorFactorNode((var: VariableNode<Gaussian1D>), mean, variance) =
  let update (oldMarginal : Gaussian1D) = new Gaussian1D (mean / variance + oldMarginal.PrecisionMean, 1.0 / variance + oldMarginal.Precision) in
  OneVariableNode(var, update)
  

//---------------------------------------------------------------------
// Finish up



#if TESTS_AS_APP
let RUN() = !failures
#else
let aa =
  match !failures with 
  | [] -> 
      stdout.WriteLine "Test Passed"
      printf "TEST PASSED OK" ;
      exit 0
  | _ -> 
      stdout.WriteLine "Test Failed"
      exit 1
#endif

