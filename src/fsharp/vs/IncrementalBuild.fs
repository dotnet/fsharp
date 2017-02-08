// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Compiler


open System
open System.IO
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks
open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.NameResolution
open Microsoft.FSharp.Compiler.Tastops
open Microsoft.FSharp.Compiler.Lib
open Microsoft.FSharp.Compiler.AbstractIL
open Microsoft.FSharp.Compiler.AbstractIL.IL
open Microsoft.FSharp.Compiler.AbstractIL.Diagnostics 
open Microsoft.FSharp.Compiler.AbstractIL.Internal
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library 
open Microsoft.FSharp.Compiler.CompileOps
open Microsoft.FSharp.Compiler.CompileOptions
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.ErrorLogger
open Microsoft.FSharp.Compiler.TcGlobals
open Microsoft.FSharp.Compiler.TypeChecker
open Microsoft.FSharp.Compiler.Tast 
open Microsoft.FSharp.Compiler.Range
open Internal.Utilities
open Internal.Utilities.Collections


[<AutoOpen>]
module internal IncrementalBuild =

    /// A particular node in the Expr language. Use an int for keys instead of the entire Expr to avoid extra hashing.
    type Id = Id of int
            
    [<NoEquality; NoComparison>]
    /// A build rule representing a single output
    type ScalarBuildRule = 
        /// ScalarInput (uniqueRuleId, outputName)
        ///
        /// A build rule representing a single input, producing the input as its single scalar result
        | ScalarInput of Id * string

        /// ScalarDemultiplex (uniqueRuleId, outputName, input, taskFunction)
        ///
        /// A build rule representing the merge of a set of inputs to a single output
        | ScalarDemultiplex of Id * string * VectorBuildRule * (obj[] -> obj)

        /// ScalarMap (uniqueRuleId, outputName, input, taskFunction)
        ///
        /// A build rule representing the transformation of a single input to a single output
        /// THIS CASE IS CURRENTLY UNUSED
        | ScalarMap of Id * string * ScalarBuildRule * (obj->obj)

        /// Get the Id for the given ScalarBuildRule.
        member  x.Id = 
            match x with
            | ScalarInput(id,_) ->id
            | ScalarDemultiplex(id,_,_,_) ->id
            | ScalarMap(id,_,_,_) ->id
        /// Get the Name for the givenScalarExpr.
        member x.Name = 
            match x with 
            | ScalarInput(_,n) ->n                
            | ScalarDemultiplex(_,n,_,_) ->n
            | ScalarMap(_,n,_,_) ->n                

    /// A build rule with a vector of outputs
    and VectorBuildRule = 
        /// VectorInput (uniqueRuleId, outputName)
        ///
        /// A build rule representing the transformation of a single input to a single output
        | VectorInput of Id * string 

        /// VectorInput (uniqueRuleId, outputName, initialAccumulator, inputs, taskFunction)
        ///
        /// A build rule representing the scan-left combining a single scalar accumulator input with a vector of inputs
        | VectorScanLeft of Id * string * ScalarBuildRule * VectorBuildRule * (obj->obj->Eventually<obj>)

        /// VectorMap (uniqueRuleId, outputName, inputs, taskFunction)
        ///
        /// A build rule representing the parallel map of the inputs to outputs
        | VectorMap of Id * string * VectorBuildRule * (obj->obj) 

        /// VectorStamp (uniqueRuleId, outputName, inputs, stampFunction)
        ///
        /// A build rule representing pairing the inputs with a timestamp specified by the given function.  
        | VectorStamp of Id * string * VectorBuildRule * (obj->DateTime)

        /// VectorMultiplex (uniqueRuleId, outputName, input, taskFunction)
        ///
        /// A build rule representing taking a single input and transforming it to a vector of outputs
        | VectorMultiplex of Id * string * ScalarBuildRule * (obj->obj[])

        /// Get the Id for the given VectorBuildRule.
        member x.Id = 
            match x with 
            | VectorInput(id,_) ->id
            | VectorScanLeft(id,_,_,_,_) ->id
            | VectorMap(id,_,_,_) ->id
            | VectorStamp (id,_,_,_) ->id
            | VectorMultiplex(id,_,_,_) ->id
        /// Get the Name for the given VectorBuildRule.
        member x.Name = 
            match x with 
            | VectorInput(_,n) ->n
            | VectorScanLeft(_,n,_,_,_) ->n
            | VectorMap(_,n,_,_) ->n
            | VectorStamp (_,n,_,_) ->n
            | VectorMultiplex(_,n,_,_) ->n
        
    [<NoEquality; NoComparison>]
    type BuildRuleExpr =
        | ScalarBuildRule of ScalarBuildRule
        | VectorBuildRule of VectorBuildRule      
        /// Get the Id for the given Expr.
        member x.Id = 
            match x with 
            | ScalarBuildRule se -> se.Id
            | VectorBuildRule ve -> ve.Id      
        /// Get the Name for the given Expr.
        member x.Name = 
            match x with 
            | ScalarBuildRule se -> se.Name
            | VectorBuildRule ve -> ve.Name    

    // Ids of exprs            
    let nextid = ref 999 // Number ids starting with 1000 to discern them
    let NextId() =
        nextid:=!nextid+1
        Id(!nextid)                    
        
    type INode = 
        abstract Name: string

    type IScalar = 
        inherit INode
        abstract Expr: ScalarBuildRule

    type IVector =
        inherit INode
        abstract Expr: VectorBuildRule
            
    type Scalar<'T> =  interface inherit IScalar  end

    type Vector<'T> = interface inherit IVector end
    
    /// The outputs of a build        
    [<NoEquality; NoComparison>]
    type NamedOutput = 
        | NamedVectorOutput of IVector
        | NamedScalarOutput of IScalar

    type BuildRules = { RuleList: (string * BuildRuleExpr) list }

    /// Visit each task and call op with the given accumulator.
    let FoldOverBuildRules(rules:BuildRules, op, acc)=
        let rec visitVector (ve:VectorBuildRule) acc = 
            match ve with
            | VectorInput _ ->op (VectorBuildRule ve) acc
            | VectorScanLeft(_,_,a,i,_) ->op (VectorBuildRule ve) (visitVector i (visitScalar a acc))
            | VectorMap(_,_,i,_)
            | VectorStamp (_,_,i,_) ->op (VectorBuildRule ve) (visitVector i acc)
            | VectorMultiplex(_,_,i,_) ->op (VectorBuildRule ve) (visitScalar i acc)

        and visitScalar (se:ScalarBuildRule) acc = 
            match se with
            | ScalarInput _ ->op (ScalarBuildRule se) acc
            | ScalarDemultiplex(_,_,i,_) ->op (ScalarBuildRule se) (visitVector i acc)
            | ScalarMap(_,_,i,_) ->op (ScalarBuildRule se) (visitScalar i acc)

        let visitRule (expr:BuildRuleExpr) acc =  
            match expr with
            | ScalarBuildRule se ->visitScalar se acc
            | VectorBuildRule ve ->visitVector ve acc

        List.foldBack visitRule (rules.RuleList |> List.map snd) acc            
    
    /// Convert from interfaces into discriminated union.
    let ToBuild (names:NamedOutput list): BuildRules = 

        // Create the rules.
        let createRules() = 
           { RuleList = names |> List.map (function NamedVectorOutput(v) -> v.Name,VectorBuildRule(v.Expr)
                                                  | NamedScalarOutput(s) -> s.Name,ScalarBuildRule(s.Expr)) }
        
        // Ensure that all names are unique.
        let ensureUniqueNames (expr:BuildRuleExpr) (acc:Map<string,Id>) = 
            let AddUniqueIdToNameMapping(id,name)=
                match acc.TryFind name with
                 | Some priorId -> 
                    if id<>priorId then failwith (sprintf "Two build expressions had the same name: %s" name)
                    else acc
                 | None-> Map.add name id acc
            let id = expr.Id
            let name = expr.Name
            AddUniqueIdToNameMapping(id,name)
        
        // Validate the rule tree
        let validateRules (rules:BuildRules) =
            FoldOverBuildRules(rules,ensureUniqueNames,Map.empty) |> ignore
        
        // Convert and validate
        let rules = createRules()
        validateRules rules
        rules

    /// These describe the input conditions for a result. If conditions change then the result is invalid.
    type InputSignature =
        | SingleMappedVectorInput of InputSignature[]
        | EmptyTimeStampedInput of DateTime
        | BoundInputScalar // An external input into the build
        | BoundInputVector // An external input into the build
        | IndexedValueElement of DateTime
        | UnevaluatedInput

        /// Return true if the result is fully evaluated
        member is.IsEvaluated = 
            match is with
            | UnevaluatedInput -> false
            | SingleMappedVectorInput iss -> iss |> Array.forall (fun is -> is.IsEvaluated)
            | _ -> true
            
    
    /// A slot for holding a single result.
    type Result =
        | NotAvailable
        | InProgress of (unit -> Eventually<obj>) * DateTime 
        | Available of obj * DateTime * InputSignature

        /// Get the available result. Throw an exception if not available.
        member x.GetAvailable() = match x with Available(o,_,_) ->o  | _ -> failwith "No available result"

        /// Get the time stamp if available. Otherwise MaxValue.        
        member x.Timestamp = match x with Available(_,ts,_) -> ts | InProgress(_,ts) -> ts | _ -> DateTime.MaxValue

        /// Get the time stamp if available. Otheriwse MaxValue.        
        member x.InputSignature = match x with Available(_,_,signature) -> signature | _ -> UnevaluatedInput
        
        member x.ResultIsInProgress =  match x with | InProgress _ -> true | _ -> false
        member x.GetInProgressContinuation() =  match x with | InProgress (f,_) -> f() | _ -> failwith "not in progress"
        member x.TryGetAvailable() =  match x with | InProgress _ | NotAvailable -> None | Available(obj,dt,i) -> Some (obj,dt,i)

    /// An immutable sparse vector of results.                
    type ResultVector(size,zeroElementTimestamp,map) =
        let get slot = 
            match Map.tryFind slot map with
            | Some result ->result
            | None->NotAvailable                   
        let asList = lazy List.map (fun i->i,get i) [0..size-1]

        static member OfSize(size) = ResultVector(size,DateTime.MinValue,Map.empty)
        member rv.Size = size
        member rv.Get slot = get slot
        member rv.Resize(newsize) = 
            if size<>newsize then 
                ResultVector(newsize, zeroElementTimestamp, map |> Map.filter(fun s _ -> s < newsize))
            else rv

        member rv.Set(slot,value) = 
#if DEBUG
            if slot<0 then failwith "ResultVector slot less than zero"
            if slot>=size then failwith "ResultVector slot too big"
#endif
            ResultVector(size, zeroElementTimestamp, Map.add slot value map)

        member rv.MaxTimestamp() =
            let maximize (lasttimestamp:DateTime) (_,result:Result) =  max lasttimestamp result.Timestamp
            List.fold maximize zeroElementTimestamp (asList.Force())

        member rv.Signature() =
            let l = asList.Force()
            let l = l |> List.map (fun (_,result) -> result.InputSignature)
            SingleMappedVectorInput (l|>List.toArray)
                                  
        member rv.FoldLeft f s: 'a = List.fold f s (asList.Force())
                
    /// A result of performing build actions
    [<NoEquality; NoComparison>]
    type ResultSet =
        | ScalarResult of Result
        | VectorResult of ResultVector
                            
    /// Result of a particular action over the bound build tree
    [<NoEquality; NoComparison>]
    type ActionResult = 
        | IndexedResult of Id * int * (*slotcount*) int * Eventually<obj> * DateTime 
        | ScalarValuedResult of Id * obj * DateTime * InputSignature
        | VectorValuedResult of Id * obj[] * DateTime * InputSignature
        | ResizeResult of Id * (*slotcount*) int
        
        
    /// A pending action over the bound build tree
    [<NoEquality; NoComparison>]
    type Action = 
        | IndexedAction of Id * (*taskname*)string * int * (*slotcount*) int * DateTime * (unit->Eventually<obj>)
        | ScalarAction of Id * (*taskname*)string * DateTime * InputSignature * (unit->obj)
        | VectorAction of Id * (*taskname*)string * DateTime * InputSignature *  (unit->obj[])
        | ResizeResultAction of Id * (*slotcount*) int 
        /// Execute one action and return a corresponding result.
        member action.Execute() = 
            match action with
            | IndexedAction(id,_taskname,slot,slotcount,timestamp,func) -> IndexedResult(id,slot,slotcount,func(),timestamp)
            | ScalarAction(id,_taskname,timestamp,inputsig,func) -> ScalarValuedResult(id,func(),timestamp,inputsig)
            | VectorAction(id,_taskname,timestamp,inputsig,func) -> VectorValuedResult(id,func(),timestamp,inputsig)
            | ResizeResultAction(id,slotcount) -> ResizeResult(id,slotcount)
     
    /// A set of build rules and the corresponding, possibly partial, results from building.
    [<Sealed>]
    type PartialBuild(rules:BuildRules, results:Map<Id,ResultSet>) = 
        member bt.Rules = rules
        member bt.Results = results
   
    /// Given an expression, find the expected width.
    let rec GetVectorWidthByExpr(bt:PartialBuild,ve:VectorBuildRule) = 
        let id = ve.Id
        let KnownValue() = 
            match bt.Results.TryFind id with 
            | Some resultSet ->
                match resultSet with
                | VectorResult rv ->Some rv.Size
                | _ -> failwith "Expected vector to have vector result."
            | None-> None
        match ve with
        | VectorScanLeft(_,_,_,i,_)
        | VectorMap(_,_,i,_)
        | VectorStamp (_,_,i,_) ->
            match GetVectorWidthByExpr(bt,i) with
            | Some _ as r -> r
            | None -> KnownValue()
        | VectorInput _
        | VectorMultiplex _ -> KnownValue()
        
    /// Given an expression name, get the corresponding expression.    
    let GetTopLevelExprByName(bt:PartialBuild, seek:string) =
        bt.Rules.RuleList |> List.filter(fun(name,_) ->name=seek) |> List.map (fun(_,root) ->root) |> List.head
    
    /// Get an expression matching the given name.
    let GetExprByName(bt:PartialBuild, node:INode): BuildRuleExpr = 
        let matchName (expr:BuildRuleExpr) (acc:BuildRuleExpr option): BuildRuleExpr option =
            if expr.Name = node.Name then Some expr else acc
        let matchOption = FoldOverBuildRules(bt.Rules,matchName,None)
        Option.get matchOption

    // Given an Id, find the corresponding expression.
    let GetExprById(bt:PartialBuild, seek:Id): BuildRuleExpr= 
        let rec vectorExprOfId ve =
            match ve with
            | VectorInput(id,_) ->if seek=id then Some (VectorBuildRule ve) else None
            | VectorScanLeft(id,_,a,i,_) ->
                if seek=id then Some (VectorBuildRule ve) else
                    let result = scalarExprOfId(a) 
                    match result with Some _ -> result | None->vectorExprOfId i
            | VectorMap(id,_,i,_) ->if seek=id then Some (VectorBuildRule ve) else vectorExprOfId i
            | VectorStamp (id,_,i,_) ->if seek=id then Some (VectorBuildRule ve) else vectorExprOfId i
            | VectorMultiplex(id,_,i,_) ->if seek=id then Some (VectorBuildRule ve) else scalarExprOfId i

        and scalarExprOfId se =
            match se with
            | ScalarInput(id,_) ->if seek=id then Some (ScalarBuildRule se) else None
            | ScalarDemultiplex(id,_,i,_) ->if seek=id then Some (ScalarBuildRule se) else vectorExprOfId i
            | ScalarMap(id,_,i,_) ->if seek=id then Some (ScalarBuildRule se) else scalarExprOfId i

        let exprOfId(expr:BuildRuleExpr) = 
            match expr with
            | ScalarBuildRule se ->scalarExprOfId se
            | VectorBuildRule ve ->vectorExprOfId ve

        let exprs = bt.Rules.RuleList |> List.map (fun(_,root) ->exprOfId(root)) |> List.filter Option.isSome
        match exprs with
        | Some expr :: _ -> expr
        | _ -> failwith (sprintf "GetExprById did not find an expression for Id")

    let GetVectorWidthById (bt:PartialBuild) seek = 
        match GetExprById(bt,seek) with 
        | ScalarBuildRule _ ->failwith "Attempt to get width of scalar." 
        | VectorBuildRule ve -> Option.get (GetVectorWidthByExpr(bt,ve))

    let GetScalarExprResult (bt:PartialBuild, se:ScalarBuildRule) =
        match bt.Results.TryFind (se.Id) with 
        | Some resultSet ->
            match se,resultSet with
            | ScalarInput _,ScalarResult r
            | ScalarMap _,ScalarResult r
            | ScalarDemultiplex _,ScalarResult r ->r
            | _ ->failwith "GetScalarExprResult had no match"
        | None->NotAvailable

    let GetVectorExprResultVector (bt:PartialBuild, ve:VectorBuildRule) =
        match bt.Results.TryFind (ve.Id) with 
        | Some resultSet ->
            match ve,resultSet with
            | VectorScanLeft _,VectorResult rv
            | VectorMap _,VectorResult rv
            | VectorInput _,VectorResult rv
            | VectorStamp _,VectorResult rv
            | VectorMultiplex _,VectorResult rv -> Some rv
            | _ -> failwith "GetVectorExprResultVector had no match"
        | None->None

    let GetVectorExprResult (bt:PartialBuild, ve:VectorBuildRule, slot) =
        match bt.Results.TryFind ve.Id with 
        | Some resultSet ->
            match ve,resultSet with
            | VectorScanLeft _,VectorResult rv
            | VectorMap _,VectorResult rv
            | VectorInput _,VectorResult rv
            | VectorStamp _,VectorResult rv -> rv.Get slot
            | VectorMultiplex _,VectorResult rv -> rv.Get slot
            | _ -> failwith "GetVectorExprResult had no match" 
        | None->NotAvailable

    /// Get the maximum build stamp for an output.
    let MaxTimestamp(bt:PartialBuild,id) = 
        match bt.Results.TryFind id with
        | Some resultset -> 
            match resultset with 
            | ScalarResult(rs) -> rs.Timestamp
            | VectorResult rv -> rv.MaxTimestamp()
        | None -> DateTime.MaxValue
        
    let Signature(bt:PartialBuild,id) =
        match bt.Results.TryFind id with
        | Some resultset -> 
            match resultset with 
            | ScalarResult(rs) -> rs.InputSignature
            | VectorResult rv -> rv.Signature()
        | None -> UnevaluatedInput               
     
    /// Get all the results for the given expr.
    let AllResultsOfExpr extractor (bt:PartialBuild) (expr: VectorBuildRule) = 
        let GetAvailable (rv:ResultVector) = 
            let Extract acc (_, result) = (extractor result)::acc
            List.rev (rv.FoldLeft Extract [])
        let GetVectorResultById id = 
            match bt.Results.TryFind id with
            | Some found ->
                match found with
                | VectorResult rv ->GetAvailable rv
                | _ -> failwith "wrong result type"
            | None -> []
            
        GetVectorResultById(expr.Id)


   
    [<RequireQualifiedAccess>]
    type BuildInput =
        | Vector of INode * obj list
        | Scalar of INode * obj

        /// Declare a named scalar output.
        static member ScalarInput (node:Scalar<'T>,value: 'T) = BuildInput.Scalar(node,box value)
        static member VectorInput(node:Vector<'T>,values: 'T list) = BuildInput.Vector(node,List.map box values)

        
    let AvailableAllResultsOfExpr bt expr = 
        let msg = "Expected all results to be available"
        AllResultsOfExpr (function Available(o,_,_) -> o | _ -> failwith msg) bt expr
        
    /// Bind a set of build rules to a set of input values.
    let ToBound(buildRules:BuildRules, inputs: BuildInput list) = 
        let now = DateTime.Now
        let rec applyScalarExpr(se,results) =
            match se with
            | ScalarInput(id,n) -> 
                let matches = 
                   [ for input in inputs  do
                       match input with 
                       | BuildInput.Scalar (node, value) ->
                         if node.Name = n then 
                             yield ScalarResult(Available(value,now,BoundInputScalar))
                       | _ -> () ]
                List.foldBack (Map.add id) matches results
            | ScalarMap(_,_,se,_) ->applyScalarExpr(se,results)
            | ScalarDemultiplex(_,_,ve,_) ->ApplyVectorExpr(ve,results)
        and ApplyVectorExpr(ve,results) =
            match ve with
            | VectorInput(id,n) ->
                let matches = 
                   [ for input in inputs  do
                       match input with 
                       | BuildInput.Scalar _ -> ()
                       | BuildInput.Vector (node, values) ->
                         if node.Name = n then 
                            let results = values|>List.mapi(fun i value->i,Available(value,now,BoundInputVector))
                            yield VectorResult(ResultVector(values.Length,DateTime.MinValue,results|>Map.ofList)) ]
                List.foldBack (Map.add id) matches results
            | VectorScanLeft(_,_,a,i,_) ->ApplyVectorExpr(i,applyScalarExpr(a,results))
            | VectorMap(_,_,i,_)
            | VectorStamp (_,_,i,_) ->ApplyVectorExpr(i,results)
            | VectorMultiplex(_,_,i,_) ->applyScalarExpr(i,results)

        let applyExpr expr results =
            match expr with
            | ScalarBuildRule se ->applyScalarExpr(se,results)
            | VectorBuildRule ve ->ApplyVectorExpr(ve,results)
                                                                             
        // Place vector inputs into results map.
        let results = List.foldBack applyExpr (buildRules.RuleList |> List.map snd) Map.empty
        PartialBuild(buildRules,results)
        
    type Target = Target of INode * int option

    /// Visit each executable action necessary to evaluate the given output (with an optional slot in a
    /// vector output). Call actionFunc with the given accumulator.
    let ForeachAction (Target(output, optSlot)) bt (actionFunc:Action->'acc->'acc) (acc:'acc) =
        let seen = Dictionary<Id,bool>()
        let isSeen id = 
            if seen.ContainsKey id then true
            else 
                seen.[id] <- true
                false
                 
        let shouldEvaluate(bt,currentsig:InputSignature,id) =
            if currentsig.IsEvaluated then 
                currentsig <> Signature(bt,id)
            else false
            
        /// Make sure the result vector saved matches the size of expr
        let resizeVectorExpr(ve:VectorBuildRule,acc)  = 
            match GetVectorWidthByExpr(bt,ve) with
            | Some expectedWidth ->
                match bt.Results.TryFind ve.Id with
                | Some found ->
                    match found with
                    | VectorResult rv ->
                        if rv.Size<> expectedWidth then 
                            actionFunc (ResizeResultAction(ve.Id ,expectedWidth)) acc
                        else acc
                    | _ -> acc
                | None -> acc        
            | None -> acc           
        
        let rec visitVector optSlot (ve: VectorBuildRule) acc =
        
            if isSeen ve.Id then acc
            else
                let acc = resizeVectorExpr(ve,acc)        
                match ve with
                | VectorInput _ ->acc
                | VectorScanLeft(id,taskname,accumulatorExpr,inputExpr,func) ->
                    let acc =
                        match GetVectorWidthByExpr(bt,ve) with
                        | Some cardinality ->                    
                            let limit = match optSlot with None -> cardinality | Some slot -> (slot+1)
                        
                            let Scan slot =
                                let accumulatorResult = 
                                    if slot=0 then GetScalarExprResult (bt,accumulatorExpr) 
                                    else GetVectorExprResult (bt,ve,slot-1)

                                let inputResult = GetVectorExprResult (bt,inputExpr,slot)
                                match accumulatorResult,inputResult with 
                                | Available(accumulator,accumulatortimesamp,_accumulatorInputSig),Available(input,inputtimestamp,_inputSig) ->
                                    let inputtimestamp = max inputtimestamp accumulatortimesamp
                                    let prevoutput = GetVectorExprResult (bt,ve,slot)
                                    let outputtimestamp = prevoutput.Timestamp
                                    let scanOp = 
                                        if inputtimestamp <> outputtimestamp then
                                            Some (fun () -> func accumulator input)
                                        elif prevoutput.ResultIsInProgress then
                                            Some prevoutput.GetInProgressContinuation
                                        else 
                                            // up-to-date and complete, no work required
                                            None
                                    match scanOp with 
                                    | Some scanOp -> Some (actionFunc (IndexedAction(id,taskname,slot,cardinality,inputtimestamp,scanOp)) acc)
                                    | None -> None
                                | _ -> None                            
                                
                            match ([0..limit-1]|>List.tryPick Scan) with Some (acc) ->acc | None->acc
                        | None -> acc
                    
                    // Check each slot for an action that may be performed.
                    visitVector None inputExpr (visitScalar accumulatorExpr acc)

                | VectorMap(id, taskname, inputExpr, func) ->
                    let acc =
                        match GetVectorWidthByExpr(bt,ve) with
                        | Some cardinality ->       
                            if cardinality=0 then
                                // For vector length zero, just propagate the prior timestamp.
                                let inputtimestamp = MaxTimestamp(bt,inputExpr.Id)
                                let outputtimestamp = MaxTimestamp(bt,id)
                                if inputtimestamp <> outputtimestamp then
                                    actionFunc (VectorAction(id,taskname,inputtimestamp,EmptyTimeStampedInput inputtimestamp, fun _ ->[||])) acc
                                else acc
                            else                                                
                                let MapResults acc slot =
                                    let inputtimestamp = GetVectorExprResult(bt,inputExpr,slot).Timestamp
                                    let outputtimestamp = GetVectorExprResult(bt,ve,slot).Timestamp
                                    if inputtimestamp <> outputtimestamp then
                                        let OneToOneOp() =
                                            Eventually.Done (func (GetVectorExprResult(bt,inputExpr,slot).GetAvailable()))
                                        actionFunc (IndexedAction(id,taskname,slot,cardinality,inputtimestamp,OneToOneOp)) acc
                                    else acc
                                match optSlot with 
                                | None ->
                                    [0..cardinality-1] |> List.fold MapResults acc                         
                                | Some slot -> 
                                    MapResults acc slot
                        | None -> acc

                    visitVector optSlot inputExpr acc

                | VectorStamp (id, taskname, inputExpr, func) -> 
               
                    // For every result that is available, check time stamps.
                    let acc =
                        match GetVectorWidthByExpr(bt,ve) with
                        | Some cardinality ->    
                            if cardinality=0 then
                                // For vector length zero, just propagate the prior timestamp.
                                let inputtimestamp = MaxTimestamp(bt,inputExpr.Id)
                                let outputtimestamp = MaxTimestamp(bt,id)
                                if inputtimestamp <> outputtimestamp then
                                    actionFunc (VectorAction(id,taskname,inputtimestamp,EmptyTimeStampedInput inputtimestamp,fun _ ->[||])) acc
                                else acc
                            else                 
                                let checkStamp acc slot = 
                                    let inputresult = GetVectorExprResult (bt,inputExpr,slot)
                                    match inputresult with
                                    | Available(ires,_,_) ->
                                        let oldtimestamp = GetVectorExprResult(bt,ve,slot).Timestamp
                                        let newtimestamp = func ires
                                        if newtimestamp <> oldtimestamp then 
                                            actionFunc (IndexedAction(id,taskname,slot,cardinality,newtimestamp, fun _ -> Eventually.Done ires)) acc
                                        else acc
                                    | _ -> acc
                                match optSlot with 
                                | None ->
                                    [0..cardinality-1] |> List.fold checkStamp acc
                                | Some slot -> 
                                    checkStamp acc slot
                        | None -> acc
                    visitVector optSlot inputExpr acc

                | VectorMultiplex(id, taskname, inputExpr, func) -> 
                    let acc = 
                        match GetScalarExprResult (bt,inputExpr) with
                         | Available(inp,inputtimestamp,inputsig) ->
                           let outputtimestamp = MaxTimestamp(bt,id)
                           if inputtimestamp <> outputtimestamp then
                               let MultiplexOp() = func inp
                               actionFunc (VectorAction(id,taskname,inputtimestamp,inputsig,MultiplexOp)) acc
                           else acc
                         | _ -> acc
                    visitScalar inputExpr acc

        and visitScalar (se:ScalarBuildRule) acc =
            if isSeen se.Id then acc
            else
                match se with
                | ScalarInput _ -> acc
                | ScalarDemultiplex (id,taskname,inputExpr,func) ->
                    let acc = 
                        match GetVectorExprResultVector (bt,inputExpr) with
                        | Some inputresult ->   
                            let currentsig = inputresult.Signature()
                            if shouldEvaluate(bt,currentsig,id) then
                                let inputtimestamp = MaxTimestamp(bt, inputExpr.Id)
                                let DemultiplexOp() = 
                                    let input = AvailableAllResultsOfExpr bt inputExpr |> List.toArray
                                    func input
                                actionFunc (ScalarAction(id,taskname,inputtimestamp,currentsig,DemultiplexOp)) acc
                            else acc
                        | None -> acc

                    visitVector None inputExpr acc

                | ScalarMap (id,taskname,inputExpr,func) ->
                    let acc = 
                        match GetScalarExprResult (bt,inputExpr) with
                        | Available(inp,inputtimestamp,inputsig) ->
                           let outputtimestamp = MaxTimestamp(bt, id)
                           if inputtimestamp <> outputtimestamp then
                               let MapOp() = func inp
                               actionFunc (ScalarAction(id,taskname,inputtimestamp,inputsig,MapOp)) acc
                           else acc
                        | _ -> acc
                    
                    visitScalar inputExpr acc
                         
                    
        let expr = bt.Rules.RuleList |> List.find (fun (s,_) -> s = output.Name) |> snd
        match expr with
        | ScalarBuildRule se -> visitScalar se acc
        | VectorBuildRule ve -> visitVector optSlot ve acc                    
    
    /// Compute the max timestamp on all available inputs
    let ComputeMaxTimeStamp output (bt: PartialBuild) acc =
        let expr = bt.Rules.RuleList |> List.find (fun (s,_) -> s = output) |> snd
        match expr with 
        | VectorBuildRule  (VectorStamp (_id, _taskname, inputExpr, func) as ve) -> 
                match GetVectorWidthByExpr(bt,ve) with
                | Some cardinality ->    
                    let CheckStamp acc slot = 
                        match GetVectorExprResult (bt,inputExpr,slot) with
                        | Available(ires,_,_) -> max acc (func ires)
                        | _ -> acc
                    [0..cardinality-1] |> List.fold CheckStamp acc
                | None -> acc

        | _ -> failwith "expected a VectorStamp"
    

    /// Given the result of a single action, apply that action to the Build
    let ApplyResult(actionResult:ActionResult,bt:PartialBuild) = 
        match actionResult with 
        | ResizeResult(id,slotcount) ->
            match bt.Results.TryFind id with
            | Some resultSet ->
                match resultSet with 
                | VectorResult rv -> 
                    let rv = rv.Resize(slotcount)
                    let results = Map.add id (VectorResult rv) bt.Results
                    PartialBuild(bt.Rules,results)
                | _ -> failwith "Unexpected"                
            | None -> failwith "Unexpected"
        | ScalarValuedResult(id,value,timestamp,inputsig) ->
            PartialBuild(bt.Rules, Map.add id (ScalarResult(Available(value,timestamp,inputsig))) bt.Results)
        | VectorValuedResult(id,values,timestamp,inputsig) ->
            let Append acc slot = 
                Map.add slot (Available(values.[slot],timestamp,inputsig)) acc
            let results = [0..values.Length-1]|>List.fold Append Map.empty
            let results = VectorResult(ResultVector(values.Length,timestamp,results))
            let bt = PartialBuild(bt.Rules, Map.add id results bt.Results)
            bt
                
        | IndexedResult(id,index,slotcount,value,timestamp) ->
            let width = GetVectorWidthById bt id
            let priorResults = bt.Results.TryFind id 
            let prior =
                match priorResults with
                | Some prior ->prior
                | None->VectorResult(ResultVector.OfSize width)
            match prior with
            | VectorResult rv ->                                
                let result = 
                    match value with 
                    | Eventually.Done res -> 
                        Available(res,timestamp, IndexedValueElement timestamp)
                    | Eventually.NotYetDone f -> 
                        InProgress (f,timestamp)
                let results = rv.Resize(slotcount).Set(index,result)
                PartialBuild(bt.Rules, Map.add id (VectorResult(results)) bt.Results)
            | _ -> failwith "Unexpected"
        
    let mutable injectCancellationFault = false
    let LocallyInjectCancellationFault() = 
        injectCancellationFault <- true
        { new IDisposable with member __.Dispose() =  injectCancellationFault <- false }

    /// Apply the result, and call the 'save' function to update the build.  
    ///
    /// Will throw OperationCanceledException if the cancellation token has been set.
    let ExecuteApply save (ct: CancellationToken) (action:Action) bt = 
        ct.ThrowIfCancellationRequested()
        if (injectCancellationFault) then raise (OperationCanceledException("injected fault"))
        let actionResult = action.Execute()
        let newBt = ApplyResult(actionResult,bt)
        save newBt
        newBt

    /// Evaluate the result of a single output
    ///
    /// Will throw OperationCanceledException if the cancellation token has been set.
    let EvalLeafsFirst save (ct: CancellationToken) target bt =

        let rec eval(bt,gen) =
            #if DEBUG
            // This can happen, for example, if there is a task whose timestamp never stops increasing.
            // Possibly could detect this case directly.
            if gen>5000 then failwith "Infinite loop in incremental builder?"
            #endif
            let newBt = ForeachAction target bt (ExecuteApply save ct) bt
            if newBt=bt then  bt else eval(newBt,gen+1)
        eval(bt,0)
        
    /// Evaluate one step of the build.  Call the 'save' function to save the intermediate result.
    ///
    /// Will throw OperationCanceledException if the cancellation token has been set.
    let Step save ct target (bt:PartialBuild) = 
        
        // Hey look, we're building up the whole list, executing one thing and then throwing
        // the list away. What about saving the list inside the Build instance?
        let worklist = ForeachAction target bt (fun a l -> a :: l) []
            
        match worklist with 
        | action::_ -> Some (ExecuteApply save ct action bt)
        | _ -> None
            
    /// Evaluate an output of the build.
    ///
    /// Will throw OperationCanceledException if the cancellation token has been set.  Intermediate
    /// progrewss along the way may be saved through the use of the 'save' function.
    let Eval save ct node bt = EvalLeafsFirst save ct (Target(node,None)) bt

    /// Evaluate an output of the build.
    ///
    /// Will throw OperationCanceledException if the cancellation token has been set.  Intermediate
    /// progrewss along the way may be saved through the use of the 'save' function.
    let EvalUpTo save ct (node, n) bt = EvalLeafsFirst save ct (Target(node, Some n)) bt

    /// Check if an output is up-to-date and ready
    let IsReady target bt = 
        let worklist = ForeachAction target bt (fun a l -> a :: l) []
        worklist.IsEmpty
        
    /// Check if an output is up-to-date and ready
    let MaxTimeStampInDependencies target bt = 
        ComputeMaxTimeStamp target bt DateTime.MinValue 

    /// Get a scalar vector. Result must be available
    let GetScalarResult<'T>(node:Scalar<'T>,bt): ('T*DateTime) option = 
        match GetTopLevelExprByName(bt,node.Name) with 
        | ScalarBuildRule se ->
            let id = se.Id
            match bt.Results.TryFind id with
            | Some result ->
                match result with 
                | ScalarResult(sr) ->
                    match sr.TryGetAvailable() with                     
                    | Some (r,timestamp,_) -> Some (downcast r, timestamp)
                    | None -> None
                | _ ->failwith "Expected a scalar result."
            | None->None
        | VectorBuildRule _ -> failwith "Expected scalar."
    
    /// Get a result vector. All results must be available or thrown an exception.
    let GetVectorResult<'T>(node:Vector<'T>,bt): 'T[] = 
        match GetTopLevelExprByName(bt,node.Name) with 
        | ScalarBuildRule _ -> failwith "Expected vector."
        | VectorBuildRule ve -> AvailableAllResultsOfExpr bt ve |> List.map (unbox) |> Array.ofList
        
    /// Get an element of vector result or None if there were no results.
    let GetVectorResultBySlot<'T>(node:Vector<'T>,slot,bt): ('T*DateTime) option = 
        match GetTopLevelExprByName(bt,node.Name) with 
        | ScalarBuildRule _ -> failwith "Expected vector expression"
        | VectorBuildRule ve ->
            match GetVectorExprResult(bt,ve,slot).TryGetAvailable() with
            | Some (o,timestamp,_) -> Some (downcast o,timestamp)
            | None->None

    /// Given an input value, find the corresponding slot.        
    let TryGetSlotByInput<'T>(node:Vector<'T>,input:'T,build:PartialBuild,equals:'T->'T->bool): int option = 
        let expr = GetExprByName(build,node)
        let id = expr.Id
        match build.Results.TryFind id with 
        | None -> None
        | Some resultSet ->
        match resultSet with 
        | VectorResult rv ->
            let MatchNames acc (slot,result) = 
                match result with
                | Available(o,_,_) ->
                    let o = o :?> 'T
                    if equals o input then Some slot else acc
                | _ -> acc
            let slotOption = rv.FoldLeft MatchNames None
            slotOption 
            // failwith (sprintf "Could not find requested input '%A' named '%s' in set %+A" input name rv)
        | _ -> None // failwith (sprintf "Could not find requested input: %A" input)

    
    // Redeclare functions in the incremental build scope-----------------------------------------------------------------------

    // Methods for declaring inputs and outputs            

    /// Declares a vector build input.
    let InputVector<'T> name = 
        let expr = VectorInput(NextId(),name) 
        { new Vector<'T>
          interface IVector with
               override __.Name = name
               override pe.Expr = expr }

    /// Declares a scalar build input.
    let InputScalar<'T> name = 
        let expr = ScalarInput(NextId(),name)
        { new Scalar<'T>
          interface IScalar with
               override __.Name = name
               override pe.Expr = expr }
    
            
    module Vector =
        /// Maps one vector to another using the given function.    
        let Map (taskname:string) (task:'I ->'O) (input:Vector<'I>): Vector<'O> = 
            let input = input.Expr
            let expr = VectorMap(NextId(),taskname,input,unbox >> task >> box) 
            { new Vector<'O>
              interface IVector with
                   override __.Name = taskname
                   override pe.Expr = expr }            
            
        
        /// Apply a function to each element of the vector, threading an accumulator argument
        /// through the computation. Returns intermediate results in a vector.
        let ScanLeft (taskname:string) (task:'A -> 'I -> Eventually<'A>) (acc:Scalar<'A>) (input:Vector<'I>): Vector<'A> =
            let BoxingScanLeft a i = Eventually.box(task (unbox a) (unbox i))
            let acc = acc.Expr
            let input = input.Expr
            let expr = VectorScanLeft(NextId(),taskname,acc,input,BoxingScanLeft) 
            { new Vector<'A>
              interface IVector with
                   override __.Name = taskname
                   override pe.Expr = expr }    
            
        /// Apply a function to a vector to get a scalar value.
        let Demultiplex (taskname:string) (task:'I[] -> 'O) (input:Vector<'I>): Scalar<'O> =
            let BoxingDemultiplex i =
                box(task (Array.map unbox i) )
            let input = input.Expr
            let expr = ScalarDemultiplex(NextId(),taskname,input,BoxingDemultiplex)
            { new Scalar<'O>
              interface IScalar with
                   override __.Name = taskname
                   override pe.Expr = expr }                
            
        /// Creates a new vector with the same items but with 
        /// timestamp specified by the passed-in function.  
        let Stamp (taskname:string) (task:'I -> DateTime) (input:Vector<'I>): Vector<'I> =
            let input = input.Expr
            let expr = VectorStamp (NextId(),taskname,input,unbox >> task) 
            { new Vector<'I>
              interface IVector with
                   override __.Name = taskname
                   override pe.Expr = expr }    

        let AsScalar (taskname:string) (input:Vector<'I>): Scalar<'I array> = 
            Demultiplex taskname (fun v->v) input
                  
    /// Declare build outputs and bind them to real values.
    type BuildDescriptionScope() =
        let mutable outputs = []
        /// Declare a named scalar output.
        member b.DeclareScalarOutput(output:Scalar<'T>)=
            outputs <- NamedScalarOutput(output) :: outputs
        /// Declare a named vector output.
        member b.DeclareVectorOutput(output:Vector<'T>)=
            outputs <- NamedVectorOutput(output) :: outputs
        /// Set the concrete inputs for this build
        member b.GetInitialPartialBuild(inputs:BuildInput list) =
            ToBound(ToBuild outputs,inputs)   


[<RequireQualifiedAccess>]
type FSharpErrorSeverity = 
    | Warning 
    | Error

type FSharpErrorInfo(fileName, s:pos, e:pos, severity: FSharpErrorSeverity, message: string, subcategory: string, errorNum: int) = 
    member __.StartLine = Line.toZ s.Line
    member __.StartLineAlternate = s.Line
    member __.EndLine = Line.toZ e.Line
    member __.EndLineAlternate = e.Line
    member __.StartColumn = s.Column
    member __.EndColumn = e.Column
    member __.Severity = severity
    member __.Message = message
    member __.Subcategory = subcategory
    member __.FileName = fileName
    member __.ErrorNumber = errorNum
    member __.WithStart(newStart) = FSharpErrorInfo(fileName, newStart, e, severity, message, subcategory, errorNum)
    member __.WithEnd(newEnd) = FSharpErrorInfo(fileName, s, newEnd, severity, message, subcategory, errorNum)
    override __.ToString()= sprintf "%s (%d,%d)-(%d,%d) %s %s %s" fileName (int s.Line) (s.Column + 1) (int e.Line) (e.Column + 1) subcategory (if severity=FSharpErrorSeverity.Warning then "warning" else "error")  message
            
    /// Decompose a warning or error into parts: position, severity, message, error number
    static member (*internal*) CreateFromException(exn, isError, trim:bool, fallbackRange:range) = 
        let m = match GetRangeOfDiagnostic exn with Some m -> m | None -> fallbackRange 
        let e = if trim then m.Start else m.End
        let msg = bufs (fun buf -> OutputPhasedDiagnostic ErrorLogger.ErrorStyle.DefaultErrors buf exn false)
        let errorNum = GetDiagnosticNumber exn
        FSharpErrorInfo(m.FileName, m.Start, e, (if isError then FSharpErrorSeverity.Error else FSharpErrorSeverity.Warning), msg, exn.Subcategory(), errorNum)
        
    /// Decompose a warning or error into parts: position, severity, message, error number
    static member internal CreateFromExceptionAndAdjustEof(exn, isError, trim:bool, fallbackRange:range, (linesCount:int, lastLength:int)) = 
        let r = FSharpErrorInfo.CreateFromException(exn,isError,trim,fallbackRange)
                
        // Adjust to make sure that errors reported at Eof are shown at the linesCount        
        let startline, schange = min (r.StartLineAlternate, false) (linesCount, true)
        let endline,   echange = min (r.EndLineAlternate, false)   (linesCount, true)
        
        if not (schange || echange) then r
        else
            let r = if schange then r.WithStart(mkPos startline lastLength) else r
            if echange then r.WithEnd(mkPos  endline (1 + lastLength)) else r

    
/// Use to reset error and warning handlers            
[<Sealed>]
type ErrorScope()  = 
    let mutable errors = [] 
    static let mutable mostRecentError = None
    let unwindBP = PushThreadBuildPhaseUntilUnwind (BuildPhase.TypeCheck)    
    let unwindEL =        
        PushErrorLoggerPhaseUntilUnwind (fun _oldLogger -> 
            { new ErrorLogger("ErrorScope") with 
                member x.DiagnosticSink(exn, isError) = 
                      let err = FSharpErrorInfo.CreateFromException(exn,isError,false,range.Zero)
                      errors <- err :: errors
                      if isError then 
                          mostRecentError <- Some err
                member x.ErrorCount = errors.Length })
        
    member x.Errors = errors |> List.filter (fun error -> error.Severity = FSharpErrorSeverity.Error)
    member x.Warnings = errors |> List.filter (fun error -> error.Severity = FSharpErrorSeverity.Warning)
    member x.Diagnostics = errors
    member x.TryGetFirstErrorText() =
        match x.Errors with 
        | error :: _ -> Some error.Message
        | [] -> None
    
    interface IDisposable with
          member d.Dispose() = 
              unwindEL.Dispose() (* unwind pushes when ErrorScope disposes *)
              unwindBP.Dispose()

    static member MostRecentError = mostRecentError
    
    static member Protect<'a> (m:range) (f:unit->'a) (err:string->'a): 'a = 
        use errorScope = new ErrorScope()
        let res = 
            try 
                Some (f())
            with e -> errorRecovery e m; None
        match res with 
        | Some res ->res
        | None -> 
            match errorScope.TryGetFirstErrorText() with 
            | Some text -> err text
            | None -> err ""

    static member ProtectWithDefault m f dflt = 
        ErrorScope.Protect m f (fun _ -> dflt)

    static member ProtectAndDiscard m f = 
        ErrorScope.Protect m f (fun _ -> ())
      

        

// Record the most recent IncrementalBuilder events, so we can more easily unittest/debug the 
// 'incremental' behavior of the product.
module IncrementalBuilderEventTesting = 

    type internal FixedLengthMRU<'T>() =
        let MAX = 400   // Length of the MRU.  For our current unit tests, 400 is enough.
        let data = Array.create MAX None
        let mutable curIndex = 0
        let mutable numAdds = 0
        // called by the product, to note when a parse/typecheck happens for a file
        member this.Add(filename:'T) =
            numAdds <- numAdds + 1
            data.[curIndex] <- Some filename
            curIndex <- (curIndex + 1) % MAX
        member this.CurrentEventNum = numAdds
        // called by unit tests, returns 'n' most recent additions.
        member this.MostRecentList(n:int) : list<'T> =
            if n < 0 || n > MAX then
                raise <| new System.ArgumentOutOfRangeException("n", sprintf "n must be between 0 and %d, inclusive, but got %d" MAX n)
            let mutable remaining = n
            let mutable s = []
            let mutable i = curIndex - 1
            while remaining <> 0 do
                if i < 0 then
                    i <- MAX - 1
                match data.[i] with
                | None -> ()
                | Some x -> s <- x :: s
                i <- i - 1
                remaining <- remaining - 1
            List.rev s

    type IBEvent =
        | IBEParsed of string // filename
        | IBETypechecked of string // filename
        | IBECreated

    // ++GLOBAL MUTBALE STATE FOR TESTING++
    let MRU = new FixedLengthMRU<IBEvent>()  
    let GetMostRecentIncrementalBuildEvents(n) = MRU.MostRecentList(n)
    let GetCurrentIncrementalBuildEventNum() = MRU.CurrentEventNum 

module Tc = Microsoft.FSharp.Compiler.TypeChecker


/// Accumulated results of type checking.
[<NoEquality; NoComparison>]
type TypeCheckAccumulator =
    { tcState: TcState
      tcImports:TcImports
      tcGlobals:TcGlobals
      tcConfig:TcConfig
      tcEnvAtEndOfFile: TcEnv
      tcResolutions: TcResolutions list
      tcSymbolUses: TcSymbolUses list
      topAttribs:TopAttribs option
      typedImplFiles:TypedImplFile list
      tcErrors:(PhasedDiagnostic * FSharpErrorSeverity) list } // errors=true, warnings=false

      
/// Global service state
type FrameworkImportsCacheKey = (*resolvedpath*)string list * string * (*TargetFrameworkDirectories*)string list* (*fsharpBinaries*)string

type FrameworkImportsCache(keepStrongly) = 
    let frameworkTcImportsCache = AgedLookup<FrameworkImportsCacheKey,(TcGlobals * TcImports)>(keepStrongly, areSame=(fun (x,y) -> x = y)) 
    member __.Downsize() = frameworkTcImportsCache.Resize(keepStrongly=0)
    member __.Clear() = frameworkTcImportsCache.Clear()

    /// This function strips the "System" assemblies from the tcConfig and returns a age-cached TcImports for them.
    member __.Get(tcConfig:TcConfig) =
        // Split into installed and not installed.
        let frameworkDLLs,nonFrameworkResolutions,unresolved = TcAssemblyResolutions.SplitNonFoundationalResolutions(tcConfig)
        let frameworkDLLsKey = 
            frameworkDLLs 
            |> List.map (fun ar->ar.resolvedPath) // The cache key. Just the minimal data.
            |> List.sort  // Sort to promote cache hits.

        let tcGlobals,frameworkTcImports = 
            // Prepare the frameworkTcImportsCache
            //
            // The data elements in this key are very important. There should be nothing else in the TcConfig that logically affects
            // the import of a set of framework DLLs into F# CCUs. That is, the F# CCUs that result from a set of DLLs (including
            // FSharp.Core.dll and mscorlib.dll) must be logically invariant of all the other compiler configuration parameters.
            let key = (frameworkDLLsKey,
                        tcConfig.primaryAssembly.Name, 
                        tcConfig.TargetFrameworkDirectories,
                        tcConfig.fsharpBinariesDir)
            match frameworkTcImportsCache.TryGet key with 
            | Some res -> res
            | None -> 
                let tcConfigP = TcConfigProvider.Constant(tcConfig)
                let ((tcGlobals,tcImports) as res) = TcImports.BuildFrameworkTcImports (tcConfigP, frameworkDLLs, nonFrameworkResolutions)
                frameworkTcImportsCache.Put(key,res)
                tcGlobals,tcImports
        tcGlobals,frameworkTcImports,nonFrameworkResolutions,unresolved


/// An error logger that capture errors, filtering them according to warning levels etc.
type internal CompilationErrorLogger (debugName:string, tcConfig:TcConfig) = 
    inherit ErrorLogger("CompilationErrorLogger("+debugName+")")
            
    let mutable errorCount = 0
    let diagnostics = new ResizeArray<_>()

    override x.DiagnosticSink(exn, isError) = 
        if isError || ReportWarningAsError (tcConfig.globalWarnLevel, tcConfig.specificWarnOff, tcConfig.specificWarnOn, tcConfig.specificWarnAsError, tcConfig.specificWarnAsWarn, tcConfig.globalWarnAsError) exn then
            diagnostics.Add(exn, isError)
            errorCount <- errorCount + 1
        else if ReportWarning (tcConfig.globalWarnLevel, tcConfig.specificWarnOff, tcConfig.specificWarnOn) exn then 
            diagnostics.Add(exn, isError)

    override x.ErrorCount = errorCount

    member x.GetErrors() = 
        [ for (e,isError) in diagnostics -> e, (if isError then FSharpErrorSeverity.Error else FSharpErrorSeverity.Warning) ]


/// This represents the global state established as each task function runs as part of the build.
///
/// Use to reset error and warning handlers.
type CompilationGlobalsScope(errorLogger:ErrorLogger,phase,projectDirectory) = 
    do ignore projectDirectory
    let unwindEL = PushErrorLoggerPhaseUntilUnwind(fun _ -> errorLogger)
    let unwindBP = PushThreadBuildPhaseUntilUnwind (phase)
    // Return the disposable object that cleans up
    interface IDisposable with
        member d.Dispose() =
            unwindBP.Dispose()         
            unwindEL.Dispose()
                            

//------------------------------------------------------------------------------------
// Rules for reactive building.
//
// This phrases the compile as a series of vector functions and vector manipulations.
// Rules written in this language are then transformed into a plan to execute the 
// various steps of the process.
//-----------------------------------------------------------------------------------

type PartialCheckResults = 
    { TcState: TcState 
      TcImports: TcImports 
      TcGlobals: TcGlobals 
      TcConfig: TcConfig 
      TcEnvAtEnd: TcEnv 
      Errors: (PhasedDiagnostic * FSharpErrorSeverity) list 
      TcResolutions: TcResolutions list 
      TcSymbolUses: TcSymbolUses list 
      TopAttribs: TopAttribs option
      TimeStamp: System.DateTime }

    static member Create (tcAcc: TypeCheckAccumulator, timestamp) = 
        { TcState = tcAcc.tcState
          TcImports = tcAcc.tcImports
          TcGlobals = tcAcc.tcGlobals
          TcConfig = tcAcc.tcConfig
          TcEnvAtEnd = tcAcc.tcEnvAtEndOfFile
          Errors = tcAcc.tcErrors
          TcResolutions = tcAcc.tcResolutions
          TcSymbolUses = tcAcc.tcSymbolUses
          TopAttribs = tcAcc.topAttribs
          TimeStamp = timestamp }


[<AutoOpen>]
module Utilities = 
    let TryFindStringAttribute tcGlobals attribSpec attribs =
        match TryFindFSharpAttribute tcGlobals attribSpec attribs with
        | Some (Attrib(_,_,[ AttribStringArg(s) ],_,_,_,_))  -> Some s
        | _ -> None

/// The implementation of the information needed by TcImports in CompileOps.fs for an F# assembly reference.
//
/// Constructs the build data (IRawFSharpAssemblyData) representing the assembly when used 
/// as a cross-assembly reference.  Note the assembly has not been generated on disk, so this is
/// a virtualized view of the assembly contents as computed by background checking.
type RawFSharpAssemblyDataBackedByLanguageService (tcConfig,tcGlobals,tcState:TcState,outfile,topAttrs,assemblyName,ilAssemRef) = 

    /// Try to find an attribute that takes a string argument

    let generatedCcu = tcState.Ccu
    let exportRemapping = MakeExportRemapping generatedCcu generatedCcu.Contents
                      
    let sigData = 
        let _sigDataAttributes,sigDataResources = Driver.EncodeInterfaceData(tcConfig,tcGlobals,exportRemapping,generatedCcu,outfile,true)
        [ for r in sigDataResources  do
            let ccuName = GetSignatureDataResourceName r
            let bytes = 
                match r.Location with 
                | ILResourceLocation.Local b -> b()
                | _ -> assert false; failwith "unreachable"
            yield (ccuName, bytes) ]

    let autoOpenAttrs = topAttrs.assemblyAttrs |> List.choose (List.singleton >> TryFindStringAttribute tcGlobals tcGlobals.attrib_AutoOpenAttribute)
    let ivtAttrs = topAttrs.assemblyAttrs |> List.choose (List.singleton >> TryFindStringAttribute tcGlobals tcGlobals.attrib_InternalsVisibleToAttribute)
    interface IRawFSharpAssemblyData with 
        member __.GetAutoOpenAttributes(_ilg) = autoOpenAttrs
        member __.GetInternalsVisibleToAttributes(_ilg) =  ivtAttrs
        member __.TryGetRawILModule() = None
        member __.GetRawFSharpSignatureData(_m,_ilShortAssemName,_filename) = sigData
        member __.GetRawFSharpOptimizationData(_m,_ilShortAssemName,_filename) = [ ]
        member __.GetRawTypeForwarders() = mkILExportedTypes []  // TODO: cross-project references with type forwarders
        member __.ShortAssemblyName = assemblyName
        member __.ILScopeRef = IL.ILScopeRef.Assembly ilAssemRef
        member __.ILAssemblyRefs = [] // These are not significant for service scenarios
        member __.HasAnyFSharpSignatureDataAttribute =  true
        member __.HasMatchingFSharpSignatureDataAttribute _ilg = true


/// Manages an incremental build graph for the build of a single F# project
type IncrementalBuilder(frameworkTcImportsCache: FrameworkImportsCache, tcConfig: TcConfig, projectDirectory, outfile, 
                        assemblyName, niceNameGen: Ast.NiceNameGenerator, lexResourceManager,
                        sourceFiles, projectReferences: IProjectReference list, loadClosureOpt: LoadClosure option, ensureReactive, 
                        keepAssemblyContents, keepAllBackgroundResolutions) =

    /// Maximum time share for a piece of background work before it should (cooperatively) yield
    /// to enable other requests to be serviced. Yielding means returning a continuation function
    /// (via an Eventually<_> value of case NotYetDone) that can be called as the next piece of work. 
    let maxTimeShareMilliseconds = 
        match System.Environment.GetEnvironmentVariable("FCS_MaxTimeShare") with 
        | null | "" -> 50L
        | s -> int64 s

    let tcConfigP = TcConfigProvider.Constant(tcConfig)
    let importsInvalidated = new Event<string>()
    let fileParsed = new Event<string>()
    let beforeFileChecked = new Event<string>()
    let fileChecked = new Event<string>()
    let projectChecked = new Event<unit>()

    // Resolve assemblies and create the framework TcImports. This is done when constructing the
    // builder itself, rather than as an incremental task. This caches a level of "system" references. No type providers are 
    // included in these references. 
    let (tcGlobals,frameworkTcImports,nonFrameworkResolutions,unresolvedReferences) = frameworkTcImportsCache.Get tcConfig
        
    // Check for the existence of loaded sources and prepend them to the sources list if present.
    let sourceFiles = tcConfig.GetAvailableLoadedSources() @ (sourceFiles |>List.map (fun s -> rangeStartup,s))

    // Mark up the source files with an indicator flag indicating if they are the last source file in the project
    let sourceFiles = 
        let flags, isExe = tcConfig.ComputeCanContainEntryPoint(sourceFiles |> List.map snd)
        ((sourceFiles,flags) ||> List.map2 (fun (m,nm) flag -> (m,nm,(flag, isExe))))

    // Get the names and time stamps of all the non-framework referenced assemblies, which will act 
    // as inputs to one of the nodes in the build. 
    //
    // This operation is done when constructing the builder itself, rather than as an incremental task. 
    let nonFrameworkAssemblyInputs = 
        // Note we are not calling errorLogger.GetErrors() anywhere for this task. 
        // This is ok because not much can actually go wrong here.
        let errorLogger = CompilationErrorLogger("nonFrameworkAssemblyInputs", tcConfig)
        // Return the disposable object that cleans up
        use _holder = new CompilationGlobalsScope(errorLogger,BuildPhase.Parameter, projectDirectory) 

        [ for r in nonFrameworkResolutions do
            let originalTimeStamp = 
                try 
                    if FileSystem.SafeExists(r.resolvedPath) then
                        let result = FileSystem.GetLastWriteTimeShim(r.resolvedPath)
                        result
                    else
                        DateTime.Now                               
                with e -> 
                    // Note we are not calling errorLogger.GetErrors() anywhere for this task. This warning will not be reported...
                    errorLogger.Warning(e)
                    DateTime.Now                               
            yield (Choice1Of2 r.resolvedPath,originalTimeStamp)  
          for pr in projectReferences  do
            yield Choice2Of2 pr, defaultArg (pr.GetLogicalTimeStamp()) DateTime.Now]
            
    // The IncrementalBuilder needs to hold up to one item that needs to be disposed, which is the tcImports for the incremental
    // build. 
    let mutable cleanupItem = None: TcImports option
    let disposeCleanupItem() =
            match cleanupItem with 
            | None -> ()
            | Some item -> 
                cleanupItem <- None
                dispose item 

    let setCleanupItem x = 
            assert cleanupItem.IsNone
            cleanupItem <- Some x

    let mutable disposed = false
    let assertNotDisposed() =
        if disposed then  
            System.Diagnostics.Debug.Assert(false, "IncrementalBuild object has already been disposed!")
    let mutable referenceCount = 0

    //----------------------------------------------------
    // START OF BUILD TASK FUNCTIONS 
                
    /// This is a build task function that gets placed into the build rules as the computation for a VectorStamp
    ///
    /// Get the timestamp of the given file name.
    let StampFileNameTask (_m:range, filename:string, _isLastCompiland) =
        assertNotDisposed()
        FileSystem.GetLastWriteTimeShim(filename)
                            
    /// This is a build task function that gets placed into the build rules as the computation for a VectorMap
    ///
    /// Parse the given files and return the given inputs. This function is expected to be
    /// able to be called with a subset of sourceFiles and return the corresponding subset of
    /// parsed inputs. 
    let ParseTask (sourceRange:range,filename:string,isLastCompiland) =
        assertNotDisposed()
        let errorLogger = CompilationErrorLogger("ParseTask", tcConfig)
        // Return the disposable object that cleans up
        use _holder = new CompilationGlobalsScope(errorLogger, BuildPhase.Parse, projectDirectory)

        try  
            IncrementalBuilderEventTesting.MRU.Add(IncrementalBuilderEventTesting.IBEParsed filename)
            let result = ParseOneInputFile(tcConfig,lexResourceManager, [], filename ,isLastCompiland,errorLogger,(*retryLocked*)true)
            fileParsed.Trigger (filename)
            result,sourceRange,filename,errorLogger.GetErrors ()
        with exn -> 
            System.Diagnostics.Debug.Assert(false, sprintf "unexpected failure in IncrementalFSharpBuild.Parse\nerror = %s" (exn.ToString()))
            failwith "last chance failure"  
                
        
    /// This is a build task function that gets placed into the build rules as the computation for a Vector.Stamp
    ///
    /// Timestamps of referenced assemblies are taken from the file's timestamp.
    let TimestampReferencedAssemblyTask (ref, originalTimeStamp) =
        assertNotDisposed()
        // Note: we are not calling errorLogger.GetErrors() anywhere. Not a problem because timestamping can't really fail
        let errorLogger = CompilationErrorLogger("TimestampReferencedAssemblyTask", tcConfig)
        // Return the disposable object that cleans up
        use _holder = new CompilationGlobalsScope(errorLogger, BuildPhase.Parameter, projectDirectory) // Parameter because -r reference

        let timestamp = 
            try
                match ref with 
                | Choice1Of2 (filename) -> 
                    if FileSystem.SafeExists(filename) then
                        FileSystem.GetLastWriteTimeShim(filename)
                    else
                        originalTimeStamp
                | Choice2Of2 (pr:IProjectReference) ->
                    defaultArg (pr.GetLogicalTimeStamp()) originalTimeStamp
            with exn -> 
                // Note we are not calling errorLogger.GetErrors() anywhere for this task. This warning will not be reported...
                errorLogger.Warning exn
                originalTimeStamp                      
        timestamp
                
         
    /// This is a build task function that gets placed into the build rules as the computation for a Vector.Demultiplex
    ///
    // Link all the assemblies together and produce the input typecheck accumulator               
    let CombineImportedAssembliesTask _: TypeCheckAccumulator =
        assertNotDisposed()
        let errorLogger = CompilationErrorLogger("CombineImportedAssembliesTask", tcConfig)
        // Return the disposable object that cleans up
        use _holder = new CompilationGlobalsScope(errorLogger, BuildPhase.Parameter, projectDirectory)

        let tcImports = 
            try
                // We dispose any previous tcImports, for the case where a dependency changed which caused this part
                // of the partial build to be re-evaluated.
                disposeCleanupItem()

                let tcImports = TcImports.BuildNonFrameworkTcImports(tcConfigP,tcGlobals,frameworkTcImports,nonFrameworkResolutions,unresolvedReferences)  
#if EXTENSIONTYPING
                for ccu in tcImports.GetCcusExcludingBase() do
                    // When a CCU reports an invalidation, merge them together and just report a 
                    // general "imports invalidated". This triggers a rebuild.
                    ccu.Deref.InvalidateEvent.Add(fun msg -> importsInvalidated.Trigger msg)
#endif
                    
                    
                // The tcImports must be cleaned up if this builder ever gets disposed. We also dispose any previous
                // tcImports should we be re-creating an entry because a dependency changed which caused this part
                // of the partial build to be re-evaluated.
                setCleanupItem tcImports

                tcImports
            with e -> 
                System.Diagnostics.Debug.Assert(false, sprintf "Could not BuildAllReferencedDllTcImports %A" e)
                errorLogger.Warning(e)
                frameworkTcImports           

        let tcInitial = GetInitialTcEnv (assemblyName, rangeStartup, tcConfig, tcImports, tcGlobals)
        let tcState = GetInitialTcState (rangeStartup, assemblyName, tcConfig, tcGlobals, tcImports, niceNameGen, tcInitial)
        let loadClosureErrors = 
           [ match loadClosureOpt with 
             | None -> ()
             | Some loadClosure -> 
                for inp in loadClosure.Inputs do
                    for (err, isError) in inp.MetaCommandDiagnostics do 
                        yield err,(if isError then FSharpErrorSeverity.Error else FSharpErrorSeverity.Warning) ]

        let tcAcc = 
            { tcGlobals=tcGlobals
              tcImports=tcImports
              tcState=tcState
              tcConfig=tcConfig
              tcEnvAtEndOfFile=tcInitial
              tcResolutions=[]
              tcSymbolUses=[]
              topAttribs=None
              typedImplFiles=[]
              tcErrors = loadClosureErrors @ errorLogger.GetErrors() }   
        tcAcc
                
    /// This is a build task function that gets placed into the build rules as the computation for a Vector.ScanLeft
    ///
    /// Type check all files.     
    let TypeCheckTask (tcAcc:TypeCheckAccumulator) input: Eventually<TypeCheckAccumulator> =    
        assertNotDisposed()
        match input with 
        | Some input, _sourceRange, filename, parseErrors->
            IncrementalBuilderEventTesting.MRU.Add(IncrementalBuilderEventTesting.IBETypechecked filename)
            let capturingErrorLogger = CompilationErrorLogger("TypeCheckTask", tcConfig)
            let errorLogger = GetErrorLoggerFilteringByScopedPragmas(false,GetScopedPragmasForInput(input),capturingErrorLogger)
            let fullComputation = 
                eventually {
                    beforeFileChecked.Trigger (filename)

                    ApplyMetaCommandsFromInputToTcConfig tcConfig (input, Path.GetDirectoryName filename) |> ignore
                    let sink = TcResultsSinkImpl(tcAcc.tcGlobals)
                    let hadParseErrors = not (List.isEmpty parseErrors)

                    let! (tcEnvAtEndOfFile,topAttribs,typedImplFiles),tcState = 
                        TypeCheckOneInputEventually ((fun () -> hadParseErrors || errorLogger.ErrorCount > 0),
                                                        tcConfig,tcAcc.tcImports,
                                                        tcAcc.tcGlobals,
                                                        None,
                                                        TcResultsSink.WithSink sink,
                                                        tcAcc.tcState,input)
                        
                    /// Only keep the typed interface files when doing a "full" build for fsc.exe, otherwise just throw them away
                    let typedImplFiles = if keepAssemblyContents then typedImplFiles else []
                    let tcResolutions = if keepAllBackgroundResolutions then sink.GetResolutions() else TcResolutions.Empty
                    let tcEnvAtEndOfFile = (if keepAllBackgroundResolutions then tcEnvAtEndOfFile else tcState.TcEnvFromImpls)
                    let tcSymbolUses = sink.GetSymbolUses()  
                    fileChecked.Trigger (filename)
                    return {tcAcc with tcState=tcState 
                                       tcEnvAtEndOfFile=tcEnvAtEndOfFile
                                       topAttribs=Some topAttribs
                                       typedImplFiles=typedImplFiles
                                       tcResolutions=tcAcc.tcResolutions @ [tcResolutions]
                                       tcSymbolUses=tcAcc.tcSymbolUses @ [tcSymbolUses]
                                       tcErrors = tcAcc.tcErrors @ parseErrors @ capturingErrorLogger.GetErrors() } 
                }
                    
            // Run part of the Eventually<_> computation until a timeout is reached. If not complete, 
            // return a new Eventually<_> computation which recursively runs more of the computation.
            //   - When the whole thing is finished commit the error results sent through the errorLogger.
            //   - Each time we do real work we reinstall the CompilationGlobalsScope
            if ensureReactive then 
                let timeSlicedComputation = 
                    fullComputation |> 
                        Eventually.repeatedlyProgressUntilDoneOrTimeShareOverOrCanceled
                            maxTimeShareMilliseconds
                            CancellationToken.None
                            (fun f -> 
                                // Reinstall the compilation globals each time we start or restart
                                use unwind = new CompilationGlobalsScope (errorLogger, BuildPhase.TypeCheck, projectDirectory) 
                                f())
                               
                timeSlicedComputation
            else 
                use unwind = new CompilationGlobalsScope (errorLogger, BuildPhase.TypeCheck, projectDirectory) 
                fullComputation |> Eventually.force |> Eventually.Done 
        | _ -> 
            Eventually.Done tcAcc


    /// This is a build task function that gets placed into the build rules as the computation for a Vector.Demultiplex
    ///
    /// Finish up the typechecking to produce outputs for the rest of the compilation process
    let FinalizeTypeCheckTask (tcStates:TypeCheckAccumulator[]) = 
        assertNotDisposed()
        let errorLogger = CompilationErrorLogger("CombineImportedAssembliesTask", tcConfig)
        use _holder = new CompilationGlobalsScope(errorLogger, BuildPhase.TypeCheck, projectDirectory)

        // Get the state at the end of the type-checking of the last file
        let finalAcc = tcStates.[tcStates.Length-1]

        // Finish the checking
        let (_tcEnvAtEndOfLastFile,topAttrs,mimpls),tcState = 
            let results = tcStates |> List.ofArray |> List.map (fun acc-> acc.tcEnvAtEndOfFile, defaultArg acc.topAttribs EmptyTopAttrs, acc.typedImplFiles)
            TypeCheckMultipleInputsFinish (results,finalAcc.tcState)

  
        let ilAssemRef, tcAssemblyDataOpt, tcAssemblyExprOpt = 
          try
            // TypeCheckClosedInputSetFinish fills in tcState.Ccu but in incrfemental scenarios we don't want this,
            // so we make this temporary here
            let oldContents = tcState.Ccu.Deref.Contents
            try
            let tcState,tcAssemblyExpr = TypeCheckClosedInputSetFinish (mimpls,tcState)

            // Compute the identity of the generated assembly based on attributes, options etc.
            // Some of this is duplicated from fsc.fs
            let ilAssemRef = 
                let publicKey = 
                    try 
                        let signingInfo = Driver.ValidateKeySigningAttributes (tcConfig, tcGlobals, topAttrs)
                        match Driver.GetStrongNameSigner signingInfo with 
                        | None -> None
                        | Some s -> Some (PublicKey.KeyAsToken(s.PublicKey))
                    with e -> 
                        errorRecoveryNoRange e
                        None
                let locale = TryFindStringAttribute tcGlobals (tcGlobals.FindSysAttrib  "System.Reflection.AssemblyCultureAttribute") topAttrs.assemblyAttrs
                let assemVerFromAttrib = 
                    TryFindStringAttribute tcGlobals (tcGlobals.FindSysAttrib "System.Reflection.AssemblyVersionAttribute") topAttrs.assemblyAttrs 
                    |> Option.bind  (fun v -> try Some (parseILVersion v) with _ -> None)
                let ver = 
                    match assemVerFromAttrib with 
                    | None -> tcConfig.version.GetVersionInfo(tcConfig.implicitIncludeDir)
                    | Some v -> v
                ILAssemblyRef.Create(assemblyName, None, publicKey, false, Some ver, locale)
                
            let tcAssemblyDataOpt = 
                try
                  // Assemblies containing type provider components can not successfully be used via cross-assembly references.
                  // We return 'None' for the assembly portion of the cross-assembly reference 
                  let hasTypeProviderAssemblyAttrib = 
                      topAttrs.assemblyAttrs |> List.exists (fun (Attrib(tcref,_,_,_,_,_,_)) -> tcref.CompiledRepresentationForNamedType.BasicQualifiedName = typeof<Microsoft.FSharp.Core.CompilerServices.TypeProviderAssemblyAttribute>.FullName)
                  if hasTypeProviderAssemblyAttrib then
                    None
                  else
                    Some  (RawFSharpAssemblyDataBackedByLanguageService (tcConfig,tcGlobals,tcState,outfile,topAttrs,assemblyName,ilAssemRef) :> IRawFSharpAssemblyData)

                with e -> 
                    errorRecoveryNoRange e
                    None
            ilAssemRef, tcAssemblyDataOpt, Some tcAssemblyExpr
            finally 
                tcState.Ccu.Deref.Contents <- oldContents
          with e -> 
            errorRecoveryNoRange e
            mkSimpleAssRef assemblyName, None, None

        let finalAccWithErrors = 
            { finalAcc with 
                tcErrors = finalAcc.tcErrors @ errorLogger.GetErrors() 
                topAttribs = Some topAttrs
            }
        ilAssemRef, tcAssemblyDataOpt, tcAssemblyExprOpt, finalAccWithErrors

    // END OF BUILD TASK FUNCTIONS
    // ---------------------------------------------------------------------------------------------            

    // ---------------------------------------------------------------------------------------------            
    // START OF BUILD DESCRIPTION

    // Inputs
    let fileNamesNode               = InputVector<range*string*(bool*bool)> "FileNames"
    let referencedAssembliesNode    = InputVector<Choice<string,IProjectReference>*DateTime> "ReferencedAssemblies"
        
    // Build
    let stampedFileNamesNode        = Vector.Stamp "SourceFileTimeStamps" StampFileNameTask fileNamesNode
    let stampedReferencedAssembliesNode = Vector.Stamp "TimestampReferencedAssembly" TimestampReferencedAssemblyTask referencedAssembliesNode
    let initialTcAccNode            = Vector.Demultiplex "CombineImportedAssemblies" CombineImportedAssembliesTask stampedReferencedAssembliesNode
#if FCS_RETAIN_BACKGROUND_PARSE_RESULTS
    let parseTreesNode              = Vector.Map "ParseTrees" ParseTask stampedFileNamesNode
    let tcStatesNode                = Vector.ScanLeft "TypeCheckingStates" TypeCheckTask initialTcAccNode stampedFileNamesNode
#else
    let tcStatesNode                = Vector.ScanLeft "TypeCheckingStates" (fun tcAcc n -> TypeCheckTask tcAcc (ParseTask n)) initialTcAccNode stampedFileNamesNode
#endif
    let finalizedTypeCheckNode      = Vector.Demultiplex "FinalizeTypeCheck" FinalizeTypeCheckTask tcStatesNode

    // Outputs
    let buildDescription            = new BuildDescriptionScope ()

    do buildDescription.DeclareVectorOutput stampedFileNamesNode
    do buildDescription.DeclareVectorOutput stampedReferencedAssembliesNode
#if FCS_RETAIN_BACKGROUND_PARSE_RESULTS
    do buildDescription.DeclareVectorOutput parseTreesNode
#endif
    do buildDescription.DeclareVectorOutput tcStatesNode
    do buildDescription.DeclareScalarOutput initialTcAccNode
    do buildDescription.DeclareScalarOutput finalizedTypeCheckNode

    // END OF BUILD DESCRIPTION
    // ---------------------------------------------------------------------------------------------            


    let fileDependencies = 
        [ for (UnresolvedAssemblyReference(referenceText, _))  in unresolvedReferences do
            // Exclude things that are definitely not a file name
            if not(FileSystem.IsInvalidPathShim(referenceText)) then 
                let file = if FileSystem.IsPathRootedShim(referenceText) then referenceText else Path.Combine(projectDirectory,referenceText) 
                yield file 

          for r in nonFrameworkResolutions do 
                yield  r.resolvedPath 

          for (_,f,_) in sourceFiles do
                yield f 
        ]

    do IncrementalBuilderEventTesting.MRU.Add(IncrementalBuilderEventTesting.IBECreated)
    let buildInputs = [ BuildInput.VectorInput (fileNamesNode, sourceFiles)
                        BuildInput.VectorInput (referencedAssembliesNode, nonFrameworkAssemblyInputs) ]

    // This is the initial representation of progress through the build, i.e. we have made no progress.
    let mutable partialBuild = buildDescription.GetInitialPartialBuild buildInputs

    let SavePartialBuild b = partialBuild <- b

    let MaxTimeStampInDependencies (output:INode) = 
        IncrementalBuild.MaxTimeStampInDependencies output.Name partialBuild 

    member this.IncrementUsageCount() = 
        assertNotDisposed() 
        referenceCount  <- referenceCount  + 1
        { new System.IDisposable with member x.Dispose() = this.DecrementUsageCount() }

    member this.DecrementUsageCount() = 
        assertNotDisposed()
        referenceCount  <- referenceCount  - 1
        if referenceCount = 0 then 
                disposed <- true
                disposeCleanupItem()

    member __.IsAlive = referenceCount > 0

    member __.TcConfig = tcConfig
    member __.FileParsed = fileParsed.Publish
    member __.BeforeFileChecked = beforeFileChecked.Publish
    member __.FileChecked = fileChecked.Publish
    member __.ProjectChecked = projectChecked.Publish
    member __.ImportedCcusInvalidated = importsInvalidated.Publish
    member __.Dependencies = fileDependencies 
#if EXTENSIONTYPING
    member __.ThereAreLiveTypeProviders = 
        let liveTPs =
            match cleanupItem with 
            | None -> []
            | Some tcImports -> [for ia in tcImports.GetImportedAssemblies() do yield! ia.TypeProviders]
        match liveTPs with
        | [] -> false
        | _ -> true                
#endif

    member __.Step (ct) =  
        match IncrementalBuild.Step SavePartialBuild ct (Target(tcStatesNode, None)) partialBuild with 
        | None -> 
            projectChecked.Trigger()
            false
        | Some _ -> 
            true
    
    member ib.GetCheckResultsBeforeFileInProjectIfReady (filename): PartialCheckResults option  = 
        let slotOfFile = ib.GetSlotOfFileName filename
        let result = 
            match slotOfFile with
            | (*first file*) 0 -> GetScalarResult(initialTcAccNode,partialBuild)
            | _ -> GetVectorResultBySlot(tcStatesNode,slotOfFile-1,partialBuild)  
        
        match result with
        | Some (tcAcc,timestamp) -> Some (PartialCheckResults.Create (tcAcc,timestamp))
        | _ -> None
        
    
    member ib.AreCheckResultsBeforeFileInProjectReady filename = 
        let slotOfFile = ib.GetSlotOfFileName filename
        match slotOfFile with
        | (*first file*) 0 -> IncrementalBuild.IsReady (Target(initialTcAccNode, None)) partialBuild 
        | _ -> IncrementalBuild.IsReady (Target(tcStatesNode, Some (slotOfFile-1))) partialBuild  
        
    member ib.GetCheckResultsBeforeFileInProject (filename, ct) = 
        let slotOfFile = ib.GetSlotOfFileName filename
        ib.GetTypeCheckResultsBeforeSlotInProject (slotOfFile, ct)

    member ib.GetCheckResultsAfterFileInProject (filename, ct) = 
        let slotOfFile = ib.GetSlotOfFileName filename + 1
        ib.GetTypeCheckResultsBeforeSlotInProject (slotOfFile, ct)

    member ib.GetTypeCheckResultsBeforeSlotInProject (slotOfFile, ct) = 
        let result = 
            match slotOfFile with
            | (*first file*) 0 -> 
                let build = IncrementalBuild.Eval SavePartialBuild ct initialTcAccNode partialBuild
                GetScalarResult(initialTcAccNode,build)
            | _ -> 
                let build = IncrementalBuild.EvalUpTo SavePartialBuild ct (tcStatesNode, (slotOfFile-1)) partialBuild
                GetVectorResultBySlot(tcStatesNode,slotOfFile-1,build)  
        
        match result with
        | Some (tcAcc,timestamp) -> PartialCheckResults.Create (tcAcc,timestamp)
        | None -> failwith "Build was not evaluated, expected the results to be ready after 'Eval'."

    member b.GetCheckResultsAfterLastFileInProject (ct) = 
        b.GetTypeCheckResultsBeforeSlotInProject(b.GetSlotsCount(), ct) 

    member __.GetCheckResultsAndImplementationsForProject(ct) = 
        let build = IncrementalBuild.Eval SavePartialBuild ct finalizedTypeCheckNode partialBuild
        match GetScalarResult(finalizedTypeCheckNode,build) with
        | Some ((ilAssemRef, tcAssemblyDataOpt, tcAssemblyExprOpt, tcAcc), timestamp) -> 
            PartialCheckResults.Create (tcAcc,timestamp), ilAssemRef, tcAssemblyDataOpt, tcAssemblyExprOpt
        | None -> failwith "Build was not evaluated, expcted the results to be ready after 'Eval'."
        
    member __.GetLogicalTimeStampForProject() = 
        let t1 = MaxTimeStampInDependencies stampedFileNamesNode 
        let t2 = MaxTimeStampInDependencies stampedReferencedAssembliesNode 
        max t1 t2
        
    member __.GetSlotOfFileName(filename:string) =
        // Get the slot of the given file and force it to build.
        let CompareFileNames (_,f1,_) (_,f2,_) = 
            let result = 
                   System.String.Compare(f1,f2,StringComparison.CurrentCultureIgnoreCase)=0
                || System.String.Compare(FileSystem.GetFullPathShim(f1),FileSystem.GetFullPathShim(f2),StringComparison.CurrentCultureIgnoreCase)=0
            result
        match TryGetSlotByInput(fileNamesNode,(rangeStartup,filename,(false,false)),partialBuild,CompareFileNames) with
        | Some slot -> slot
        | None -> failwith (sprintf "The file '%s' was not part of the project. Did you call InvalidateConfiguration when the list of files in the project changed?" filename)
        
    member __.GetSlotsCount () =
        let expr = GetExprByName(partialBuild,fileNamesNode)
        match partialBuild.Results.TryFind (expr.Id) with
        | Some (VectorResult vr) -> vr.Size
        | _ -> failwith "Failed to find sizes"
      
    member ib.GetParseResultsForFile (filename, ct) =
        let slotOfFile = ib.GetSlotOfFileName filename
#if FCS_RETAIN_BACKGROUND_PARSE_RESULTS
        match GetVectorResultBySlot(parseTreesNode,slotOfFile,partialBuild) with
        | Some (results, _) -> results
        | None -> 
            let build = IncrementalBuild.EvalUpTo SavePartialBuild ct (parseTreesNode, slotOfFile) partialBuild  
            match GetVectorResultBySlot(parseTreesNode,slotOfFile,build) with
            | Some (results, _) -> results
            | None -> failwith "Build was not evaluated, expcted the results to be ready after 'Eval'."
#else
        let results = 
            match GetVectorResultBySlot(stampedFileNamesNode,slotOfFile,partialBuild) with
            | Some (results, _) ->  results
            | None -> 
                let build = IncrementalBuild.EvalUpTo SavePartialBuild ct (stampedFileNamesNode, slotOfFile) partialBuild  
                match GetVectorResultBySlot(stampedFileNamesNode,slotOfFile,build) with
                | Some (results, _) -> results
                | None -> failwith "Build was not evaluated, expcted the results to be ready after 'Eval'."
        // re-parse on demand instead of retaining
        ParseTask results
#endif

    member __.ProjectFileNames  = sourceFiles  |> List.map (fun (_,f,_) -> f)

    /// CreateIncrementalBuilder (for background type checking). Note that fsc.fs also
    /// creates an incremental builder used by the command line compiler.
    static member TryCreateBackgroundBuilderForProjectOptions (referenceResolver, frameworkTcImportsCache, loadClosureOpt:LoadClosure option, sourceFiles:string list, commandLineArgs:string list, projectReferences, projectDirectory, useScriptResolutionRules, keepAssemblyContents, keepAllBackgroundResolutions) =
    
        // Trap and report warnings and errors from creation.
        use errorScope = new ErrorScope()
        let builderOpt = 
            try

            // Create the builder.         
            // Share intern'd strings across all lexing/parsing
            let resourceManager = new Lexhelp.LexResourceManager() 

            /// Create a type-check configuration
            let tcConfigB, sourceFilesNew = 
                let defaultFSharpBinariesDir = Internal.Utilities.FSharpEnvironment.BinFolderOfDefaultFSharpCompiler.Value
                    
                // see also fsc.fs:runFromCommandLineToImportingAssemblies(), as there are many similarities to where the PS creates a tcConfigB
                let tcConfigB = 
                    TcConfigBuilder.CreateNew(referenceResolver, defaultFSharpBinariesDir, implicitIncludeDir=projectDirectory, 
                                                optimizeForMemory=true, isInteractive=false, isInvalidationSupported=true) 
                // The following uses more memory but means we don'T take read-exclusions on the DLLs we reference 
                // Could detect well-known assemblies--ie System.dll--and open them with read-locks 
                tcConfigB.openBinariesInMemory <- true
                tcConfigB.resolutionEnvironment 
                    <- if useScriptResolutionRules 
                        then ReferenceResolver.DesignTimeLike  
                        else ReferenceResolver.CompileTimeLike
                
                tcConfigB.conditionalCompilationDefines <- 
                    let define = if useScriptResolutionRules then "INTERACTIVE" else "COMPILED"
                    define::tcConfigB.conditionalCompilationDefines

                tcConfigB.projectReferences <- projectReferences

                // Apply command-line arguments and collect more source files if they are in the arguments
                let sourceFilesNew = 
                    try
                        let sourceFilesAcc = ResizeArray(sourceFiles)
                        let collect name = if not (Filename.isDll name) then sourceFilesAcc.Add name
                        ParseCompilerOptions (collect, GetCoreServiceCompilerOptions tcConfigB, commandLineArgs)
                        sourceFilesAcc |> ResizeArray.toList
                    with e ->
                        errorRecovery e range0
                        sourceFiles

                // Never open PDB files for the language service, even if --standalone is specified
                tcConfigB.openDebugInformationForLaterStaticLinking <- false
        
                tcConfigB, sourceFilesNew

            match loadClosureOpt with
            | Some loadClosure -> 
                let dllReferences = 
                    [for reference in tcConfigB.referencedDLLs do
                        // If there's (one or more) resolutions of closure references then yield them all
                        match loadClosure.References  |> List.tryFind (fun (resolved,_)->resolved=reference.Text) with
                        | Some (resolved,closureReferences) -> 
                            for closureReference in closureReferences do
                                yield AssemblyReference(closureReference.originalReference.Range, resolved, None)
                        | None -> yield reference]
                tcConfigB.referencedDLLs <- []
                // Add one by one to remove duplicates
                for dllReference in dllReferences do
                    tcConfigB.AddReferencedAssemblyByPath(dllReference.Range,dllReference.Text)
                tcConfigB.knownUnresolvedReferences <- loadClosure.UnresolvedReferences
            | None -> ()

            let tcConfig = TcConfig.Create(tcConfigB,validate=true)

            let niceNameGen = NiceNameGenerator()
        
            let outfile, _, assemblyName = tcConfigB.DecideNames sourceFilesNew
        
            let builder = 
                new IncrementalBuilder(frameworkTcImportsCache,
                                        tcConfig, projectDirectory, outfile, assemblyName, niceNameGen,
                                        resourceManager, sourceFilesNew, projectReferences, loadClosureOpt, ensureReactive=true, 
                                        keepAssemblyContents=keepAssemblyContents, 
                                        keepAllBackgroundResolutions=keepAllBackgroundResolutions)
            Some builder
            with e -> 
            errorRecoveryNoRange e
            None

        builderOpt, errorScope.Diagnostics

    static member KeepBuilderAlive (builderOpt: IncrementalBuilder option) = 
        match builderOpt with 
        | Some builder -> builder.IncrementUsageCount() 
        | None -> { new System.IDisposable with member __.Dispose() = () }

    member b.IsBeingKeptAliveApartFromCacheEntry = (referenceCount >= 2)
