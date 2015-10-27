// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Compiler
#nowarn "57"
open Internal.Utilities.Debug
open Internal.Utilities.FileSystem
open System
open System.IO
open System.Reflection             
open System.Diagnostics
open System.Collections.Generic
open System

open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.CompileOps
open Microsoft.FSharp.Compiler.CompileOptions
open Microsoft.FSharp.Compiler.Tastops
open Microsoft.FSharp.Compiler.TcGlobals
open Microsoft.FSharp.Compiler.ErrorLogger
open Microsoft.FSharp.Compiler.Lib
open Microsoft.FSharp.Compiler.AbstractIL
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library 

module internal IncrementalBuild =

    /// A particular node in the Expr language. Use an int for keys instead of the entire Expr to avoid extra hashing.
    type Id = 
        | Id of int
        static member toInt (Id id) = id
        override id.ToString() = match id with Id(n) ->sprintf "Id(%d)" n
            
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
        | ScalarMap of Id * string * ScalarBuildRule * (obj->obj)

        /// Get the Id for the given ScalarBuildRule.
        static member GetId = function
            | ScalarInput(id,_) ->id
            | ScalarDemultiplex(id,_,_,_) ->id
            | ScalarMap(id,_,_,_) ->id
        /// Get the Name for the givenScalarExpr.
        static member GetName = function
            | ScalarInput(_,n) ->n                
            | ScalarDemultiplex(_,n,_,_) ->n
            | ScalarMap(_,n,_,_) ->n                
        override ve.ToString() = 
            match ve with 
            | ScalarInput(Id id,name) ->sprintf "InputScalar(%d,%s)" id name
            | ScalarDemultiplex(Id id,name,_,_) ->sprintf "ScalarDemultiplex(%d,%s)" id name
            | ScalarMap(Id id,name,_,_) ->sprintf "ScalarMap(%d,%s)" id name

    /// A build rule with a vector of outputs
    and VectorBuildRule = 
        /// VectorInput (uniqueRuleId, outputName)
        ///
        /// A build rule representing the transformation of a single input to a single output
        | VectorInput of Id * string 

        /// VectorInput (uniqueRuleId, outputName, initialAccumulator, inputs, taskFunction)
        ///
        /// A build rule representing the scan-left combinining a single scalar accumulator input with a vector of inputs
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
        static member GetId = function
            | VectorInput(id,_) ->id
            | VectorScanLeft(id,_,_,_,_) ->id
            | VectorMap(id,_,_,_) ->id
            | VectorStamp(id,_,_,_) ->id
            | VectorMultiplex(id,_,_,_) ->id
        /// Get the Name for the given VectorBuildRule.
        static member GetName = function
            | VectorInput(_,n) ->n
            | VectorScanLeft(_,n,_,_,_) ->n
            | VectorMap(_,n,_,_) ->n
            | VectorStamp(_,n,_,_) ->n
            | VectorMultiplex(_,n,_,_) ->n
        override ve.ToString() = 
            match ve with 
            | VectorInput(Id id,name) ->sprintf "VectorInput(%d,%s)" id name
            | VectorScanLeft(Id id,name,_,_,_) ->sprintf "VectorScanLeft(%d,%s)" id name
            | VectorMap(Id id,name,_,_) ->sprintf "VectorMap(%d,%s)" id name
            | VectorStamp(Id id,name,_,_) ->sprintf "VectorStamp(%d,%s)" id name
            | VectorMultiplex(Id id,name,_,_) ->sprintf "VectorMultiplex(%d,%s)" id name
        
    [<NoEquality; NoComparison>]
    type BuildRuleExpr =
        | ScalarBuildRule of ScalarBuildRule
        | VectorBuildRule of VectorBuildRule      
        /// Get the Id for the given Expr.
        static member GetId = function
            | ScalarBuildRule se ->ScalarBuildRule.GetId se
            | VectorBuildRule ve ->VectorBuildRule.GetId ve      
        /// Get the Name for the given Expr.
        static member GetName= function
            | ScalarBuildRule se ->ScalarBuildRule.GetName se
            | VectorBuildRule ve ->VectorBuildRule.GetName ve      
        override e.ToString() = 
            match e with 
            | ScalarBuildRule _ -> sprintf "ScalarBuildRule se" 
            | VectorBuildRule _ -> sprintf "VectorBuildRule ve"

    // Ids of exprs            
    let nextid = ref 999 // Number ids starting with 1000 to discern them
    let NextId() =
        nextid:=!nextid+1
        Id(!nextid)                    
        
    type IScalar = 
        abstract GetScalarExpr : unit -> ScalarBuildRule
    type IVector =
        abstract GetVectorExpr : unit-> VectorBuildRule
            
    type Scalar<'T> =  interface  end

    type Vector<'T> = interface end
    
    /// The outputs of a build        
    [<NoEquality; NoComparison>]
    type NamedOutput = 
        | NamedVectorOutput of string * IVector
        | NamedScalarOutput of string * IScalar

    type BuildRules = { RuleList : (string * BuildRuleExpr) list }

    /// Visit each task and call op with the given accumulator.
    let FoldOverBuildRules(rules:BuildRules, op, acc)=
        let rec VisitVector (ve:VectorBuildRule) acc = 
            match ve with
            | VectorInput _ ->op (VectorBuildRule ve) acc
            | VectorScanLeft(_,_,a,i,_) ->op (VectorBuildRule ve) (VisitVector i (VisitScalar a acc))
            | VectorMap(_,_,i,_)
            | VectorStamp(_,_,i,_) ->op (VectorBuildRule ve) (VisitVector i acc)
            | VectorMultiplex(_,_,i,_) ->op (VectorBuildRule ve) (VisitScalar i acc)
        and VisitScalar (se:ScalarBuildRule) acc = 
            match se with
            | ScalarInput _ ->op (ScalarBuildRule se) acc
            | ScalarDemultiplex(_,_,i,_) ->op (ScalarBuildRule se) (VisitVector i acc)
            | ScalarMap(_,_,i,_) ->op (ScalarBuildRule se) (VisitScalar i acc)
        let rec VisitRule (expr:BuildRuleExpr) acc =  
            match expr with
            | ScalarBuildRule se ->VisitScalar se acc
            | VectorBuildRule ve ->VisitVector ve acc
        List.foldBack VisitRule (rules.RuleList |> List.map snd) acc            
    
    /// Convert from interfaces into discriminated union.
    let ToBuild (names:NamedOutput list) : BuildRules = 

        // Create the rules.
        let CreateRules() = 
           { RuleList = names |> List.map(function NamedVectorOutput(n,v) -> n,VectorBuildRule(v.GetVectorExpr())
                                                 | NamedScalarOutput(n,s) -> n,ScalarBuildRule(s.GetScalarExpr())) }
        
        // Ensure that all names are unique.
        let EnsureUniqueNames (expr:BuildRuleExpr) (acc:Map<string,Id>) = 
            let AddUniqueIdToNameMapping(id,name)=
                match acc.TryFind name with
                 | Some(priorId) -> 
                    if id<>priorId then failwith (sprintf "Two build expressions had the same name: %s" name)
                    else acc
                 | None-> Map.add name id acc
            let id = BuildRuleExpr.GetId(expr)
            let name = BuildRuleExpr.GetName(expr)
            AddUniqueIdToNameMapping(id,name)
        
        // Validate the rule tree
        let ValidateRules (rules:BuildRules) =
            FoldOverBuildRules(rules,EnsureUniqueNames,Map.empty) |> ignore
        
        // Convert and validate
        let rules = CreateRules()
        ValidateRules rules
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
        member is.IsEvaluated() = 
        
            let rec IsEvaluated(is) =
                match is with
                | UnevaluatedInput -> false
                | SingleMappedVectorInput iss -> iss |> Array.forall IsEvaluated
                | _ -> true
            IsEvaluated(is)
        override is.ToString() = sprintf "%A" is
            
    
    /// A slot for holding a single result.
    type Result =
        | NotAvailable
        | InProgress of (unit -> Eventually<obj>) * DateTime 
        | Available of obj * DateTime * InputSignature
        /// Get the available result. Throw an exception if not available.
        static member GetAvailable = function Available(o,_,_) ->o  | _->failwith "No available result"
        /// Get the time stamp if available. Otheriwse MaxValue.        
        static member Timestamp = function Available(_,ts,_) ->ts | InProgress(_,ts) -> ts | _-> DateTime.MaxValue
        /// Get the time stamp if available. Otheriwse MaxValue.        
        static member InputSignature = function Available(_,_,signature) ->signature | _-> UnevaluatedInput
        
        member x.ResultIsInProgress =  match x with | InProgress _ -> true | _ -> false
        member x.GetInProgressContinuation() =  match x with | InProgress (f,_) -> f() | _ -> failwith "not in progress"
        member x.TryGetAvailable() =  match x with | InProgress _ | NotAvailable -> None | Available(obj,dt,i) -> Some(obj,dt,i)

        override r.ToString() = 
            match r with 
            | NotAvailable -> "NotAvailable"
            | InProgress _ -> "InProgress"
            | Available(o, ts, _) -> sprintf "Available('%s' as of %A)" (o.ToString()) ts
            
    /// An immutable sparse vector of results.                
    type ResultVector(size,zeroElementTimestamp,map) =
        let get slot = 
            match Map.tryFind slot map with
            | Some(result) ->result
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
//            use t = Trace.Call("IncrementalBuildVerbose", "MaxTimestamp",  fun _->sprintf "vector of size=%d" size)
            let Maximize (lasttimestamp:DateTime) (_,result) = 
                let thistimestamp = Result.Timestamp result
                let m = max lasttimestamp thistimestamp
//                use t = Trace.Call("IncrementalBuildVerbose", "Maximize",  fun _->sprintf "last=%s this=%s max=%s" (lasttimestamp.ToString()) (thistimestamp.ToString()) (m.ToString()))
                m
            List.fold Maximize zeroElementTimestamp (asList.Force())
        member rv.Signature() =
            let l = asList.Force()
            let l = l |> List.map(fun (_,result) ->Result.InputSignature result)
            SingleMappedVectorInput (l|>List.toArray)
                                  
        member rv.FoldLeft f s : 'a = List.fold f s (asList.Force())
        override rv.ToString() = asList.ToString()   // NOTE: Force()ing this inside ToString() leads to StackOverflowException and very undesirable debugging behavior for all of F#
                
    /// A result of performing build actions
    [<NoEquality; NoComparison>]
    type ResultSet =
        | ScalarResult of Result
        | VectorResult of ResultVector
        override rs.ToString() = 
            match rs with
            | ScalarResult(sr) ->sprintf "ScalarResult(%s)" (sr.ToString())
            | VectorResult(rs) ->sprintf "VectorResult(%s)" (rs.ToString())
                            
    /// Action timing
    module Time =     
        let sw = new Stopwatch()
        let Action<'T> taskname slot func : 'T= 
            if Trace.ShouldLog("IncrementalBuildWorkUnits") then 
                let slotMessage = 
                    if slot= -1 then sprintf "%s" taskname
                    else sprintf "%s over slot %d" taskname slot
                // Timings and memory
                let maxGen = System.GC.MaxGeneration
                let ptime = System.Diagnostics.Process.GetCurrentProcess()
                let timePrev = ptime.UserProcessorTime.TotalSeconds
                let gcPrev = [| for i in 0 .. maxGen -> System.GC.CollectionCount i |]
                let pbPrev = ptime.PrivateMemorySize64 in                

                // Call the function
                let result = func()
                
                // Report.
                let timeNow = ptime.UserProcessorTime.TotalSeconds
                let pbNow = ptime.PrivateMemorySize64
                let spanGC = [| for i in 0 .. maxGen -> System.GC.CollectionCount i - gcPrev.[i] |]
                
                Trace.PrintLine("IncrementalBuildWorkUnits", fun _ ->
                                                        sprintf "%s TIME: %4.3f MEM: %3d (delta) G0: %3d G1: %2d G2: %2d" 
                                                            slotMessage
                                                            (timeNow - timePrev) 
                                                            (pbNow - pbPrev)
                                                            spanGC.[min 0 maxGen] 
                                                            spanGC.[min 1 maxGen] 
                                                            spanGC.[min 2 maxGen])
                result
            else func()            
        
    /// Result of a particular action over the bound build tree
    [<NoEquality; NoComparison>]
    type ActionResult = 
        | IndexedResult of Id * int * (*slotcount*) int * Eventually<obj> * DateTime 
        | ScalarValuedResult of Id * obj * DateTime * InputSignature
        | VectorValuedResult of Id * obj[] * DateTime * InputSignature
        | ResizeResult of Id * (*slotcount*) int
        override ar.ToString() = 
            match ar with
            | IndexedResult(id,slot,slotcount,_,dt) ->sprintf "IndexedResult(%d,%d,%d,obj,%A)" (Id.toInt id) slot slotcount dt
            | ScalarValuedResult(id,_,dt,inputsig) ->sprintf "ScalarValuedResult(%d,obj,%A,%A)" (Id.toInt id) dt inputsig
            | VectorValuedResult(id,_,dt,inputsig) ->sprintf "VectorValuedResult(%d,obj[],%A,%A)" (Id.toInt id) dt inputsig
            | ResizeResult(id,slotcount) ->sprintf "ResizeResult(%d,%d)" (Id.toInt id) slotcount
        
        
    /// A pending action over the bound build tree
    [<NoEquality; NoComparison>]
    type Action = 
        | IndexedAction of Id * (*taskname*)string * int * (*slotcount*) int * DateTime * (unit->Eventually<obj>)
        | ScalarAction of Id * (*taskname*)string * DateTime * InputSignature * (unit->obj)
        | VectorAction of Id * (*taskname*)string * DateTime * InputSignature *  (unit->obj[])
        | ResizeResultAction of Id * (*slotcount*) int 
        /// Execute one action and return a corresponding result.
        static member Execute action = 
            match action with
            | IndexedAction(id,taskname,slot,slotcount,timestamp,func) -> IndexedResult(id,slot,slotcount,Time.Action taskname slot func,timestamp)
            | ScalarAction(id,taskname,timestamp,inputsig,func) -> ScalarValuedResult(id,Time.Action taskname (-1) func,timestamp,inputsig)
            | VectorAction(id,taskname,timestamp,inputsig,func) -> VectorValuedResult(id,Time.Action taskname (-1) func,timestamp,inputsig)
            | ResizeResultAction(id,slotcount) -> ResizeResult(id,slotcount)
     
    /// String helper functions for when there's no %A
    type String = 
        static member OfList2 l =
            " ["^String.Join(",\n ", List.toArray (l|>List.map (fun (v1,v2) ->((box v1).ToString()) + ";" + ((box v2).ToString())))) + " ]"
            
    /// A set of build rules and the corresponding, possibly partial, results from building.
    [<Sealed>]
    type PartialBuild(rules:BuildRules, results:Map<Id,ResultSet>) = 
        member bt.Rules = rules
        member bt.Results = results
        override bt.ToString() = 
            let sb = new System.Text.StringBuilder()
            results |> Map.iter(fun id result->
                                    let id = Id.toInt id
                                    let s = sprintf "\n    {Id=%d,ResultSet=%s}" id (result.ToString())
                                    let _ = sb.Append(s)
                                    ())
            sprintf "{Rules={%s}\n Results={%s}}" (String.OfList2 rules.RuleList) (sb.ToString())
   
    /// Given an expression, find the expected width.
    let rec GetVectorWidthByExpr(bt:PartialBuild,ve:VectorBuildRule) = 
        let KnownValue ve = 
            match bt.Results.TryFind(VectorBuildRule.GetId ve) with 
            | Some(resultSet) ->
                match resultSet with
                | VectorResult rv ->Some(rv.Size)
                | _ -> failwith "Expected vector to have vector result."
            | None-> None
        match ve with
        | VectorScanLeft(_,_,_,i,_)
        | VectorMap(_,_,i,_)
        | VectorStamp(_,_,i,_) ->
            match GetVectorWidthByExpr(bt,i) with
            | Some _ as r -> r
            | None->KnownValue ve  
        | VectorInput _
        | VectorMultiplex _ -> KnownValue ve  
        
    /// Given an expression name, get the corresponding expression.    
    let GetTopLevelExprByName(bt:PartialBuild, seek:string) =
        bt.Rules.RuleList |> List.filter(fun(name,_) ->name=seek) |> List.map(fun(_,root) ->root) |> List.head
    
    /// Get an expression matching the given name.
    let GetExprByName(bt:PartialBuild, seek:string) : BuildRuleExpr = 
        let MatchName (expr:BuildRuleExpr) (acc:BuildRuleExpr option) : BuildRuleExpr option =
            let name = BuildRuleExpr.GetName(expr)
            if name = seek then Some(expr) else acc
        let matchOption = FoldOverBuildRules(bt.Rules,MatchName,None)
        Option.get matchOption

    // Given an Id, find the corresponding expression.
    let GetExprById(bt:PartialBuild, seek:Id) : BuildRuleExpr= 
        let rec VectorExprOfId ve =
            match ve with
            | VectorInput(id,_) ->if seek=id then Some(VectorBuildRule ve) else None
            | VectorScanLeft(id,_,a,i,_) ->
                if seek=id then Some(VectorBuildRule ve) else
                    let result = ScalarExprOfId(a) 
                    match result with Some _ -> result | None->VectorExprOfId i
            | VectorMap(id,_,i,_) ->if seek=id then Some(VectorBuildRule ve) else VectorExprOfId i
            | VectorStamp(id,_,i,_) ->if seek=id then Some(VectorBuildRule ve) else VectorExprOfId i
            | VectorMultiplex(id,_,i,_) ->if seek=id then Some(VectorBuildRule ve) else ScalarExprOfId i
        and ScalarExprOfId se =
            match se with
            | ScalarInput(id,_) ->if seek=id then Some(ScalarBuildRule se) else None
            | ScalarDemultiplex(id,_,i,_) ->if seek=id then Some(ScalarBuildRule se) else VectorExprOfId i
            | ScalarMap(id,_,i,_) ->if seek=id then Some(ScalarBuildRule se) else ScalarExprOfId i
        let ExprOfId(expr:BuildRuleExpr) = 
            match expr with
            | ScalarBuildRule se ->ScalarExprOfId se
            | VectorBuildRule ve ->VectorExprOfId ve
        let exprs = bt.Rules.RuleList |> List.map(fun(_,root) ->ExprOfId(root)) |> List.filter Option.isSome
        match exprs with
        | Some(expr)::_ -> expr
        | _ -> failwith (sprintf "GetExprById did not find an expression for Id %d" (Id.toInt seek))

    let GetVectorWidthById (bt:PartialBuild) seek = 
        match GetExprById(bt,seek) with 
        | ScalarBuildRule _ ->failwith "Attempt to get width of scalar." 
        | VectorBuildRule ve ->Option.get (GetVectorWidthByExpr(bt,ve))

    let GetScalarExprResult(bt:PartialBuild, se:ScalarBuildRule) =
        match bt.Results.TryFind(ScalarBuildRule.GetId se) with 
        | Some(resultSet) ->
            match se,resultSet with
            | ScalarInput _,ScalarResult(r)
            | ScalarMap _,ScalarResult(r)
            | ScalarDemultiplex _,ScalarResult(r) ->r
            | se,result->failwith (sprintf "GetScalarExprResult had no match for %A,%A" se result) 
        | None->NotAvailable

    let GetVectorExprResultVector(bt:PartialBuild, ve:VectorBuildRule) =
        match bt.Results.TryFind(VectorBuildRule.GetId ve) with 
        | Some(resultSet) ->
            match ve,resultSet with
            | VectorScanLeft _,VectorResult rv
            | VectorMap _,VectorResult rv
            | VectorInput _,VectorResult rv
            | VectorStamp _,VectorResult rv
            | VectorMultiplex _,VectorResult rv -> Some rv
            | ve,result->failwith (sprintf "GetVectorExprResultVector had no match for %A,%A" ve result) 
        | None->None

    let GetVectorExprResult(bt:PartialBuild, ve:VectorBuildRule, slot) =
        match bt.Results.TryFind(VectorBuildRule.GetId ve) with 
        | Some(resultSet) ->
            match ve,resultSet with
            | VectorScanLeft _,VectorResult rv
            | VectorMap _,VectorResult rv
            | VectorInput _,VectorResult rv
            | VectorStamp _,VectorResult rv -> rv.Get slot
            | VectorMultiplex _,VectorResult rv -> rv.Get slot
            | ve,result->failwith (sprintf "GetVectorExprResult had no match for %A,%A" ve result) 
        | None->NotAvailable

    /// Get the maximum build stamp for an output.
    let MaxTimestamp(bt:PartialBuild,id,_inputstamp) = 
        match bt.Results.TryFind(id) with
        | Some(resultset) -> 
            match resultset with 
            | ScalarResult(rs) -> Result.Timestamp rs
            | VectorResult rv -> rv.MaxTimestamp()
        | None -> DateTime.MaxValue
        
    let Signature(bt:PartialBuild,id) =
        match bt.Results.TryFind(id) with
        | Some(resultset) -> 
            match resultset with 
            | ScalarResult(rs) -> Result.InputSignature rs
            | VectorResult rv -> rv.Signature()
        | None -> UnevaluatedInput               
     
    /// Get all the results for the given expr.
    let AllResultsOfExpr extractor (bt:PartialBuild) expr = 
        let GetAvailable (rv:ResultVector) = 
            let Extract acc (_, result) = (extractor result)::acc
            List.rev (rv.FoldLeft Extract [])
        let GetVectorResultById id = 
            match bt.Results.TryFind(id) with
            | Some(found) ->
                match found with
                | VectorResult rv ->GetAvailable rv
                | _ -> failwith "wrong result type"
            | None -> []
            
        GetVectorResultById(VectorBuildRule.GetId(expr))


   
        
    let AvailableAllResultsOfExpr bt expr = 
        let msg = "Expected all results to be available"
        AllResultsOfExpr (function Available(o,_,_) -> o | _ -> failwith msg) bt expr
        
    /// Bind a set of build rules to a set of input values.
    let ToBound(buildRules:BuildRules, vectorinputs, scalarinputs) = 
        let now = DateTime.Now
        let rec ApplyScalarExpr(se,results) =
            match se with
            | ScalarInput(id,n) -> 
                let matches = scalarinputs 
                                |> List.filter (fun (inputname,_) ->inputname=n) 
                                |> List.map (fun (_,inputvalue:obj) -> ScalarResult(Available(inputvalue,now,BoundInputScalar)))
                List.foldBack (Map.add id) matches results
            | ScalarMap(_,_,se,_) ->ApplyScalarExpr(se,results)
            | ScalarDemultiplex(_,_,ve,_) ->ApplyVectorExpr(ve,results)
        and ApplyVectorExpr(ve,results) =
            match ve with
            | VectorInput(id,n) ->
                let matches = vectorinputs 
                                |> List.filter (fun (inputname,_,_) ->inputname=n) 
                                |> List.map (fun (_,size,inputvalues:obj list) ->
                                                        let results = inputvalues|>List.mapi(fun i value->i,Available(value,now,BoundInputVector))
                                                        VectorResult(ResultVector(size,DateTime.MinValue,results|>Map.ofList))
                                                        )
                List.foldBack (Map.add id) matches results
            | VectorScanLeft(_,_,a,i,_) ->ApplyVectorExpr(i,ApplyScalarExpr(a,results))
            | VectorMap(_,_,i,_)
            | VectorStamp(_,_,i,_) ->ApplyVectorExpr(i,results)
            | VectorMultiplex(_,_,i,_) ->ApplyScalarExpr(i,results)
        let ApplyExpr expr results =
            match expr with
            | ScalarBuildRule se ->ApplyScalarExpr(se,results)
            | VectorBuildRule ve ->ApplyVectorExpr(ve,results)
                                                                             
        // Place vector inputs into results map.
        let results = List.foldBack ApplyExpr (buildRules.RuleList |> List.map snd) Map.empty
        PartialBuild(buildRules,results)
        
        
    /// Visit each executable action and call actionFunc with the given accumulator.
    let ForeachAction output bt (actionFunc:Action->'acc->'acc) (acc:'acc) =
        use t = Trace.Call("IncrementalBuildVerbose", "ForeachAction",  fun _->sprintf "name=%s" output)
        let seen = Dictionary<_,_>()
        let Seen(id) = 
            if seen.ContainsKey(id) then true
            else seen.[id]<-true
                 false
                 
        let HasChanged(inputtimestamp,outputtimestamp) =
           if inputtimestamp<>outputtimestamp then
               Trace.PrintLine("IncrementalBuildVerbose", fun _ -> sprintf "Input timestamp is %A. Output timestamp is %A." inputtimestamp outputtimestamp)
               true
           else false
           
           
        let ShouldEvaluate(bt,currentsig:InputSignature,id) =
            let isAvailable = currentsig.IsEvaluated()
            if isAvailable then 
                let priorsig = Signature(bt,id)
                currentsig<>priorsig
            else false
            
        /// Make sure the result vector saved matches the size of expr
        let ResizeVectorExpr(ve:VectorBuildRule,acc)  = 
            let id = VectorBuildRule.GetId ve
            match GetVectorWidthByExpr(bt,ve) with
            | Some(expectedWidth) ->
                match bt.Results.TryFind(id) with
                | Some(found) ->
                    match found with
                    | VectorResult rv ->
                        if rv.Size<> expectedWidth then 
                            actionFunc (ResizeResultAction(id,expectedWidth)) acc
                        else acc
                    | _ -> acc
                | None -> acc        
            | None -> acc           
        
        let rec VisitVector ve acc =
        
            if Seen(VectorBuildRule.GetId ve) then acc
            else
                Trace.PrintLine("IncrementalBuildVerbose", fun _ -> sprintf "In ForeachAction at vector expression %s" (ve.ToString()))
                let acc = ResizeVectorExpr(ve,acc)        
                match ve with
                | VectorInput _ ->acc
                | VectorScanLeft(id,taskname,accumulatorExpr,inputExpr,func) ->
                    let acc =
                        match GetVectorWidthByExpr(bt,ve) with
                        | Some(cardinality) ->                    
                            let GetInputAccumulator slot =
                                if slot=0 then GetScalarExprResult(bt,accumulatorExpr) 
                                else GetVectorExprResult(bt,ve,slot-1)
                        
                            let Scan slot =
                                let accumulatorResult = GetInputAccumulator slot
                                let inputResult = GetVectorExprResult(bt,inputExpr,slot)
                                match accumulatorResult,inputResult with 
                                | Available(accumulator,accumulatortimesamp,_accumulatorInputSig),Available(input,inputtimestamp,_inputSig) ->
                                    let inputtimestamp = max inputtimestamp accumulatortimesamp
                                    let prevoutput = GetVectorExprResult(bt,ve,slot)
                                    let outputtimestamp = Result.Timestamp prevoutput
                                    let scanOp = 
                                        if HasChanged(inputtimestamp,outputtimestamp) then
                                            Some (fun () -> func accumulator input)
                                        elif prevoutput.ResultIsInProgress then
                                            Some prevoutput.GetInProgressContinuation
                                        else 
                                            // up-to-date and complete, no work required
                                            None
                                    match scanOp with 
                                    | Some scanOp -> Some(actionFunc (IndexedAction(id,taskname,slot,cardinality,inputtimestamp,scanOp)) acc)
                                    | None -> None
                                | _ -> None                            
                                
                            match ([0..cardinality-1]|>List.tryPick Scan) with Some(acc) ->acc | None->acc
                        | None -> acc
                    
                    // Check each slot for an action that may be performed.
                    VisitVector inputExpr (VisitScalar accumulatorExpr acc)
                | VectorMap(id, taskname, inputExpr, func) ->
                    let acc =
                        match GetVectorWidthByExpr(bt,ve) with
                        | Some(cardinality) ->       
                            if cardinality=0 then
                                // For vector length zero, just propagate the prior timestamp.
                                let inputtimestamp = MaxTimestamp(bt,VectorBuildRule.GetId(inputExpr),DateTime.MinValue)
                                let outputtimestamp = MaxTimestamp(bt,id,DateTime.MinValue)
                                if HasChanged(inputtimestamp,outputtimestamp) then
                                    Trace.PrintLine("IncrementalBuildVerbose", fun _ -> sprintf "Vector Map with cardinality zero setting output timestamp to %A." inputtimestamp)
                                    actionFunc (VectorAction(id,taskname,inputtimestamp,EmptyTimeStampedInput inputtimestamp, fun _ ->[||])) acc
                                else acc
                            else                                                
                                let MapResults acc slot =
                                    let inputtimestamp = Result.Timestamp (GetVectorExprResult(bt,inputExpr,slot))
                                    let outputtimestamp = Result.Timestamp (GetVectorExprResult(bt,ve,slot))
                                    if HasChanged(inputtimestamp,outputtimestamp) then
                                        let OneToOneOp() =
                                            Eventually.Done (func (Result.GetAvailable (GetVectorExprResult(bt,inputExpr,slot))))
                                        actionFunc (IndexedAction(id,taskname,slot,cardinality,inputtimestamp,OneToOneOp)) acc
                                    else acc
                                [0..cardinality-1] |> List.fold MapResults acc                         
                        | None -> acc
                    VisitVector inputExpr acc
                | VectorStamp(id, taskname, inputExpr, func) -> 
               
                    // For every result that is available, check time stamps.
                    let acc =
                        match GetVectorWidthByExpr(bt,ve) with
                        | Some(cardinality) ->    
                            if cardinality=0 then
                                // For vector length zero, just propagate the prior timestamp.
                                let inputtimestamp = MaxTimestamp(bt,VectorBuildRule.GetId(inputExpr),DateTime.MinValue)
                                let outputtimestamp = MaxTimestamp(bt,id,DateTime.MinValue)
                                if HasChanged(inputtimestamp,outputtimestamp) then
                                    Trace.PrintLine("IncrementalBuildVerbose", fun _ -> sprintf "Vector Stamp with cardinality zero setting output timestamp to %A." inputtimestamp)
                                    actionFunc (VectorAction(id,taskname,inputtimestamp,EmptyTimeStampedInput inputtimestamp,fun _ ->[||])) acc
                                else acc
                            else                 
                                let CheckStamp acc slot = 
                                    let inputresult = GetVectorExprResult(bt,inputExpr,slot)
                                    match inputresult with
                                    | Available(ires,_,_) ->
                                        let oldtimestamp = Result.Timestamp (GetVectorExprResult(bt,ve,slot))
                                        let newtimestamp = func ires
                                        if newtimestamp<>oldtimestamp then 
                                            Trace.PrintLine("IncrementalBuildVerbose", fun _ -> sprintf "Old timestamp was %A. New timestamp is %A." oldtimestamp newtimestamp)
                                            actionFunc (IndexedAction(id,taskname,slot,cardinality,newtimestamp, fun _ -> Eventually.Done ires)) acc
                                        else acc
                                    | _ -> acc
                                [0..cardinality-1] |> List.fold CheckStamp acc
                        | None -> acc
                    VisitVector inputExpr acc
                | VectorMultiplex(id, taskname, inputExpr, func) -> 
                    VisitScalar inputExpr
                        (match GetScalarExprResult(bt,inputExpr) with
                         | Available(inp,inputtimestamp,inputsig) ->
                           let outputtimestamp = MaxTimestamp(bt,id,inputtimestamp)
                           if HasChanged(inputtimestamp,outputtimestamp) then
                               let MultiplexOp() = func inp
                               actionFunc (VectorAction(id,taskname,inputtimestamp,inputsig,MultiplexOp)) acc
                           else acc
                         | _->acc)                
        and VisitScalar se acc =
            if Seen(ScalarBuildRule.GetId se) then acc
            else
                Trace.PrintLine("IncrementalBuildVerbose", fun _ -> sprintf "In ForeachAction at scalar expression %s" (se.ToString()))
                match se with
                | ScalarInput _ ->acc
                | ScalarDemultiplex(id,taskname,inputExpr,func) ->
                    VisitVector inputExpr 
                            (
                                match GetVectorExprResultVector(bt,inputExpr) with
                                | Some(inputresult) ->   
                                    let currentsig = inputresult.Signature()
                                    if ShouldEvaluate(bt,currentsig,id) then
                                        let inputtimestamp = MaxTimestamp(bt, VectorBuildRule.GetId(inputExpr), DateTime.MaxValue) 
                                        let DemultiplexOp() = 
                                            let input = AvailableAllResultsOfExpr bt inputExpr |> List.toArray
                                            func input
                                        actionFunc (ScalarAction(id,taskname,inputtimestamp,currentsig,DemultiplexOp)) acc
                                    else acc
                                | None -> acc
                            )
                | ScalarMap(id,taskname,inputExpr,func) ->
                    VisitScalar inputExpr
                        (match GetScalarExprResult(bt,inputExpr) with
                         | Available(inp,inputtimestamp,inputsig) ->
                           let outputtimestamp = MaxTimestamp(bt, id, inputtimestamp)
                           if HasChanged(inputtimestamp,outputtimestamp) then
                               let MapOp() = func inp
                               actionFunc (ScalarAction(id,taskname,inputtimestamp,inputsig,MapOp)) acc
                           else acc
                         | _->acc)
                         
        let Visit expr acc = 
            match expr with
            | ScalarBuildRule se ->VisitScalar se acc
            | VectorBuildRule ve ->VisitVector ve acc                    
                    
        let filtered = bt.Rules.RuleList |> List.filter (fun (s,_) -> s = output) |> List.map snd
        List.foldBack Visit filtered acc
    
    /// Given the result of a single action, apply that action to the Build
    let ApplyResult(actionResult:ActionResult,bt:PartialBuild) = 
        use t = Trace.Call("IncrementalBuildVerbose", "ApplyResult", fun _ -> "")
        let result = 
            match actionResult with 
            | ResizeResult(id,slotcount) ->
                match bt.Results.TryFind(id) with
                | Some(resultSet) ->
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
                let results = [0..values.Length-1]|>List.fold Append (Map.empty)
                let results = VectorResult(ResultVector(values.Length,timestamp,results))
                let bt = PartialBuild(bt.Rules, Map.add id results bt.Results)
                bt
                
            | IndexedResult(id,index,slotcount,value,timestamp) ->
                let width = (GetVectorWidthById bt id)
                let priorResults = bt.Results.TryFind(id) 
                let prior =
                    match priorResults with
                    | Some(prior) ->prior
                    | None->VectorResult(ResultVector.OfSize width)
                match prior with
                | VectorResult rv ->                                
                    let result = 
                        match value with 
                        | Eventually.Done res -> 
                            Trace.PrintLine("FSharpBackgroundBuildVerbose", fun _ -> "Eventually.Done...")
                            Available(res,timestamp, IndexedValueElement timestamp)
                        | Eventually.NotYetDone f -> 
                            Trace.PrintLine("FSharpBackgroundBuildVerbose", fun _ -> "Eventually.NotYetDone...")
                            InProgress (f,timestamp)
                    let results = rv.Resize(slotcount).Set(index,result)
                    PartialBuild(bt.Rules, Map.add id (VectorResult(results)) bt.Results)
                | _->failwith "Unexpected"
        result
        
    /// Evaluate the result of a single output
    let EvalLeafsFirst output bt =
        use t = Trace.Call("IncrementalBuildVerbose", "EvalLeafsFirst", fun _->sprintf "name=%s" output)

        let ExecuteApply action bt = 
            let actionResult = Action.Execute(action)
            ApplyResult(actionResult,bt)
        let rec Eval(bt,gen) =
            Trace.PrintLine("FSharpBackgroundBuildVerbose", fun _ -> sprintf "---- Build generation %d ----" gen)
            #if DEBUG
            // This can happen, for example, if there is a task whose timestamp never stops increasing.
            // Possibly could detect this case directly.
            if gen>5000 then failwith "Infinite loop in incremental builder?"
            #endif
            let newBt = ForeachAction output bt ExecuteApply bt
            if newBt=bt then bt else Eval(newBt,gen+1)
        Eval(bt,0)
        
    let Step output (bt:PartialBuild) = 
        use t = Trace.Call("IncrementalBuildVerbose", "Step", fun _->sprintf "name=%s" output)
        
        let BuildActionList() = 
            use t = Trace.Call("IncrementalBuildVerbose", "BuildActionList", fun _->sprintf "name=%s" output)
            let Cons action list =  action :: list  
            // Hey look, we're building up the whole list, executing one thing and then throwing
            // the list away. What about saving the list inside the Build instance?
            ForeachAction output bt Cons []
            
        let ExecuteOneAction(worklist) = 
            use t = Trace.Call("IncrementalBuildVerbose", "ExecuteOneAction", fun _->sprintf "name=%s" output)
            match worklist with 
            | action::_ ->
                let actionResult = Action.Execute(action)
                Some(ApplyResult(actionResult,bt))
            | _->None
            
        ExecuteOneAction(BuildActionList())                
        
    /// Eval by calling step over and over until done.
    let rec EvalStepwise output bt = 
        use t = Trace.Call("IncrementalBuildVerbose", "EvalStepwise", fun _->sprintf "name=%s" output)
        let rec Evaluate(output,bt)= 
            let newBt = Step output bt
            match newBt with
            | Some(newBt) -> Evaluate(output,newBt)
            | None->bt
        Evaluate(output,bt)
        
  /// Evaluate a build.
    let Eval output bt = EvalLeafsFirst output bt

  /// Get a scalar vector. Result must be available
    let GetScalarResult<'T>(name,bt) : ('T*DateTime) option = 
        use t = Trace.Call("IncrementalBuildVerbose", "GetScalarResult", fun _->sprintf "name=%s" name)
        match GetTopLevelExprByName(bt,name) with 
        | ScalarBuildRule se ->
            let id = ScalarBuildRule.GetId se
            match bt.Results.TryFind(id) with
            | Some(result) ->
                match result with 
                | ScalarResult(sr) ->
                    match sr.TryGetAvailable() with                     
                    | Some(r,timestamp,_) -> Some(downcast r, timestamp)
                    | None -> None
                | _ ->failwith "Expected a scalar result."
            | None->None
        | VectorBuildRule _ -> failwith "Expected scalar."
    
  /// Get a result vector. All results must be available or thrown an exception.
    let GetVectorResult<'T>(name,bt) : 'T[] = 
        match GetTopLevelExprByName(bt,name) with 
        | ScalarBuildRule _ -> failwith "Expected vector."
        | VectorBuildRule ve -> AvailableAllResultsOfExpr bt ve |> List.map(unbox) |> Array.ofList
        
  /// Get an element of vector result or None if there were no results.
    let GetVectorResultBySlot<'T>(name,slot,bt) : ('T*DateTime) option = 
        match GetTopLevelExprByName(bt,name) with 
        | ScalarBuildRule _ -> failwith "Expected vector expression"
        | VectorBuildRule ve ->
            match GetVectorExprResult(bt,ve,slot).TryGetAvailable() with
            | Some(o,timestamp,_) -> Some(downcast o,timestamp)
            | None->None

    /// Given an input value, find the corresponding slot.        
    let GetSlotByInput<'T>(name:string,input:'T,build:PartialBuild,equals:'T->'T->bool) : int = 
        let expr = GetExprByName(build,name)
        let id = BuildRuleExpr.GetId(expr)
        let resultSet = Option.get ( build.Results.TryFind(id))
        match resultSet with 
        | VectorResult rv ->
            let MatchNames acc (slot,result) = 
                match result with
                | Available(o,_,_) ->
                    let o = o :?> 'T
                    if equals o input then Some slot else acc
                | _ -> acc
            let slotOption = rv.FoldLeft MatchNames None
            match slotOption with 
            | Some slot -> slot
            | _ -> failwith (sprintf "Could not find requested input '%A' named '%s' in set %+A" input name rv)
        | _ -> failwith (sprintf "Could not find requested input: %A" input)

    
    // Redeclare functions in the incremental build scope-----------------------------------------------------------------------

    // Methods for declaring inputs and outputs            

    /// Declares a vector build input.
    let InputVector<'T> name = 
        let expr = VectorInput(NextId(),name) 
        { new Vector<'T>
          interface IVector with
               override pe.GetVectorExpr() = expr }

    /// Declares a scalar build input.
    let InputScalar<'T> name = 
        let expr = ScalarInput(NextId(),name)
        { new Scalar<'T>
          interface IScalar with
               override pe.GetScalarExpr() = expr }
    
    module Scalar =
    
        let Map (taskname:string) (task:'I->'O) (input:Scalar<'I>) : Scalar<'O> =
            let BoxingMap i = box(task(unbox(i)))
            let input = (input:?>IScalar).GetScalarExpr()
            let expr = ScalarMap(NextId(),taskname,input,BoxingMap)
            { new Scalar<'O>
              interface IScalar with
                   override pe.GetScalarExpr() = expr}
                   
        let Multiplex (taskname:string) (task:'I -> 'O array) (input:Scalar<'I>) : Vector<'O> =      
            let BoxingMultiplex i = Array.map box (task(unbox(i)))
            let input = (input:?>IScalar).GetScalarExpr()
            let expr = VectorMultiplex(NextId(),taskname,input,BoxingMultiplex) 
            { new Vector<'O>
              interface IVector with
                   override pe.GetVectorExpr() = expr}    
            
    module Vector =
        /// Maps one vector to another using the given function.    
        let Map (taskname:string) (task:'I ->'O) (input:Vector<'I>) : Vector<'O> = 
            let BoxingMapVector i =
                box(task(unbox i))
            let input = (input:?>IVector).GetVectorExpr()
            let expr = VectorMap(NextId(),taskname,input,BoxingMapVector) 
            { new Vector<'O>
              interface IVector with
                   override pe.GetVectorExpr() = expr }            
            
        
        /// Apply a function to each element of the vector, threading an accumulator argument
        /// through the computation. Returns intermediate results in a vector.
        let ScanLeft (taskname:string) (task:'A -> 'I -> Eventually<'A>) (acc:Scalar<'A>) (input:Vector<'I>) : Vector<'A> =
            let BoxingScanLeft a i =
                Eventually.box(task (unbox a) (unbox i))
            let acc = (acc:?>IScalar).GetScalarExpr()
            let input = (input:?>IVector).GetVectorExpr()
            let expr = VectorScanLeft(NextId(),taskname,acc,input,BoxingScanLeft) 
            { new Vector<'A>
              interface IVector with
                   override pe.GetVectorExpr() = expr }    
            
        /// Apply a function to a vector to get a scalar value.
        let Demultiplex (taskname:string) (task:'I[] -> 'O) (input:Vector<'I>) : Scalar<'O> =
            let BoxingDemultiplex i =
                box(task (Array.map unbox i) )
            let input = (input:?>IVector).GetVectorExpr()
            let expr = ScalarDemultiplex(NextId(),taskname,input,BoxingDemultiplex)
            { new Scalar<'O>
              interface IScalar with
                   override pe.GetScalarExpr() = expr }                
            
        /// Creates a new vector with the same items but with 
        /// timestamp specified by the passed-in function.  
        let Stamp (taskname:string) (task:'I -> DateTime) (input:Vector<'I>) : Vector<'I> =
            let BoxingTouch i =
                task(unbox i)
            let input = (input:?>IVector).GetVectorExpr()
            let expr = VectorStamp(NextId(),taskname,input,BoxingTouch) 
            { new Vector<'I>
              interface IVector with
                   override pe.GetVectorExpr() = expr }    

        let AsScalar (taskname:string) (input:Vector<'I>) : Scalar<'I array> = 
            Demultiplex taskname (fun v->v) input
                  
    /// Declare build outputs and bind them to real values.
    type BuildDescriptionScope() =
        let mutable outputs = []
        /// Declare a named scalar output.
        member b.DeclareScalarOutput(name,output:Scalar<'t>)=
            let output:IScalar = output:?>IScalar
            outputs <- NamedScalarOutput(name,output) :: outputs
        /// Declare a named vector output.
        member b.DeclareVectorOutput(name,output:Vector<'t>)=
            let output:IVector = output:?>IVector
            outputs <- NamedVectorOutput(name,output) :: outputs
        /// Set the conrete inputs for this build
        member b.GetInitialPartialBuild(vectorinputs,scalarinputs) =
            ToBound(ToBuild outputs,vectorinputs,scalarinputs)   


[<RequireQualifiedAccess>]
type Severity = 
    | Warning 
    | Error

type ErrorInfo = {
    FileName:string
    StartLine:int
    EndLine:int
    StartColumn:int
    EndColumn:int
    Severity:Severity
    Message:string 
    Subcategory:string } with 
    override e.ToString()=
        sprintf "%s (%d,%d)-(%d,%d) %s %s %s" 
            e.FileName
            e.StartLine e.StartColumn e.EndLine e.EndColumn
            e.Subcategory
            (if e.Severity=Severity.Warning then "warning" else "error") 
            e.Message    
            
    /// Decompose a warning or error into parts: position, severity, message
    static member internal CreateFromExceptionAndAdjustEof(exn,warn,trim:bool,fallbackRange:range, (linesCount:int, lastLength:int)) = 
        let r = ErrorInfo.CreateFromException(exn,warn,trim,fallbackRange)
                
        // Adjust to make sure that errors reported at Eof are shown at the linesCount        
        let startline, schange = min (r.StartLine, false) (linesCount, true)
        let endline,   echange = min (r.EndLine, false)   (linesCount, true)
        
        if not (schange || echange) then r
        else
            let r = if schange then { r with StartLine = startline; StartColumn = lastLength } else r
            if echange then { r with EndLine = endline; EndColumn = 1 + lastLength } else r

    /// Decompose a warning or error into parts: position, severity, message
    static member internal CreateFromException(exn,warn,trim:bool,fallbackRange:range) = 
        let m = match GetRangeOfError exn with Some m -> m | None -> fallbackRange 
        let (s1:int),(s2:int) = Pos.toVS m.Start
        let (s3:int),(s4:int) = Pos.toVS (if trim then m.Start else m.End)
        let msg = bufs (fun buf -> OutputPhasedError buf exn false)
        {FileName=m.FileName; StartLine=s1; StartColumn=s2; EndLine=s3; EndColumn=s4; Severity=(if warn then Severity.Warning else Severity.Error); Subcategory=exn.Subcategory(); Message=msg}
        
    
/// Use to reset error and warning handlers            
[<Sealed>]
type ErrorScope()  = 
    let mutable errors = [] 
    static let mutable mostRecentError = None
    let unwindBP = PushThreadBuildPhaseUntilUnwind (BuildPhase.TypeCheck)    
    let unwindEL =        
        PushErrorLoggerPhaseUntilUnwind (fun _oldLogger -> 
            { new ErrorLogger("ErrorScope") with 
                member x.WarnSinkImpl(exn) = 
                      errors <- ErrorInfo.CreateFromException(exn,true,false,range.Zero):: errors
                member x.ErrorSinkImpl(exn) = 
                      let err = ErrorInfo.CreateFromException(exn,false,false,range.Zero)
                      errors <- err :: errors
                      mostRecentError <- Some(err)
                member x.ErrorCount = errors.Length })
        
    member x.Errors = errors |> List.filter (fun error -> error.Severity = Severity.Error)
    member x.Warnings = errors |> List.filter (fun error -> error.Severity = Severity.Warning)
    member x.ErrorsAndWarnings = errors
    member x.TryGetFirstErrorText() =
        match x.Errors with 
        | error :: _ -> Some(error.Message)
        | [] -> None
    
    interface IDisposable with
          member d.Dispose() = 
              unwindEL.Dispose() (* unwind pushes when ErrorScope disposes *)
              unwindBP.Dispose()

    static member MostRecentError = mostRecentError
    
    static member Protect<'a> (m:range) (f:unit->'a) (err:string->'a) : 'a = 
        use errorScope = new ErrorScope()
        let res = 
            try 
                Some(f())
            with e -> errorRecovery e m; None
        match res with 
        | Some(res) ->res
        | None -> 
            match errorScope.TryGetFirstErrorText() with 
            | Some text -> err text
            | None -> err ""

    static member ProtectWithDefault m f dflt = 
        ErrorScope.Protect m f (fun _ -> dflt)

    static member ProtectAndDiscard m f = 
        ErrorScope.Protect m f (fun _ -> ())
      
// ------------------------------------------------------------------------------------------
// The incremental build definition for parsing and typechecking F#
// ------------------------------------------------------------------------------------------
module internal IncrementalFSharpBuild =

    open Internal.Utilities
    open Internal.Utilities.Collections

    open IncrementalBuild
    open Microsoft.FSharp.Compiler.CompileOps
    open Microsoft.FSharp.Compiler.CompileOptions
    open Microsoft.FSharp.Compiler.Ast
    open Microsoft.FSharp.Compiler.ErrorLogger
    open Microsoft.FSharp.Compiler.TcGlobals
    open Microsoft.FSharp.Compiler.TypeChecker
    open Microsoft.FSharp.Compiler.Tast 
    open Microsoft.FSharp.Compiler.Range
    open Microsoft.FSharp.Compiler
    open Microsoft.FSharp.Compiler.AbstractIL.Internal

    module Tc = Microsoft.FSharp.Compiler.TypeChecker

    open Microsoft.FSharp.Compiler.AbstractIL.Diagnostics 
    open Internal.Utilities.Debug

    // This type is designed to be a lightweight way to instrument the most recent filenames that the
    // IncrementalBuilder did a parse/typecheck of, so we can more easily unittest/debug the 
    // 'incremental' behavior of the product.
    type internal FixedLengthMRU<'T>() =
        let MAX = 40   // Length of the MRU.  For our current unit tests, 40 is enough.
        let data : ('T option)[] = Array.create MAX None
        let mutable curIndex = 0
        let mutable numAdds = 0
        // called by the product, to note when a parse/typecheck happens for a file
        member this.Add(filename:'T) =
            numAdds <- numAdds + 1
            data.[curIndex] <- Some filename
            curIndex <- curIndex + 1
            if curIndex = MAX then
                curIndex <- 0
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
        | IBEDeleted

    let IncrementalBuilderEventsMRU = new FixedLengthMRU<IBEvent>()  
    let GetMostRecentIncrementalBuildEvents(n) = IncrementalBuilderEventsMRU.MostRecentList(n)
    let GetCurrentIncrementalBuildEventNum() = IncrementalBuilderEventsMRU.CurrentEventNum 

    type FileDependency = {
            // Name of the file
            Filename : string
            // If true, then deletion or creation of this file should trigger an entirely fresh build
            ExistenceDependency : bool
            // If true, then changing this file should trigger and call to incrementally build
            IncrementalBuildDependency : bool } with
        override this.ToString() =
            sprintf "FileDependency(%s,existence=%A,incremental=%A)" this.Filename this.ExistenceDependency this.IncrementalBuildDependency 

    /// Accumulated results of type checking.
    [<NoEquality; NoComparison>]
    type TypeCheckAccumulator =
        { tcState: TcState
          tcImports:TcImports
          tcGlobals:TcGlobals
          tcConfig:TcConfig
          tcEnv: TcEnv
          topAttribs:TopAttribs option
          typedImplFiles:TypedImplFile list
          errors:(PhasedError * bool) list } // errors=true, warnings=false

    /// Maximum time share for a piece of background work before it should (cooperatively) yield
    /// to enable other requests to be serviced. Yielding means returning a continuation function
    /// (via an Eventually<_> value of case NotYetDone) that can be called as the next piece of work. 
    let maxTimeShareMilliseconds = 
        match System.Environment.GetEnvironmentVariable("mFSharp_MaxTimeShare") with 
        | null | "" -> 50L
        | s -> int64 s
      
    /// Global service state
    type FrameworkImportsCacheKey = (*resolvedpath*)string list * string * (*ClrRoot*)string list* (*fsharpBinaries*)string
    let private frameworkTcImportsCache = AgedLookup<FrameworkImportsCacheKey,(TcGlobals * TcImports)>(8, areSame=(fun (x,y) -> x = y)) 

    /// This function strips the "System" assemblies from the tcConfig and returns a age-cached TcImports for them.
    let GetFrameworkTcImports(tcConfig:TcConfig) =
        // Split into installed and not installed.
        let frameworkDLLs,nonFrameworkResolutions,unresolved = TcAssemblyResolutions.SplitNonFoundationalResolutions(tcConfig)
        let frameworkDLLsKey = 
            frameworkDLLs 
            |> List.map(fun ar->ar.resolvedPath) // The cache key. Just the minimal data.
            |> List.sort  // Sort to promote cache hits.
        let tcGlobals,frameworkTcImports = 
            // Prepare the frameworkTcImportsCache
            //
            // The data elements in this key are very important. There should be nothing else in the TcConfig that logically affects
            // the import of a set of framework DLLs into F# CCUs. That is, the F# CCUs that result from a set of DLLs (including
            // FSharp.Core.dll andb mscorlib.dll) must be logically invariant of all the other compiler configuration parameters.
            let key = (frameworkDLLsKey,
                       tcConfig.primaryAssembly.Name, 
                       tcConfig.ClrRoot,
                       tcConfig.fsharpBinariesDir)
            match frameworkTcImportsCache.TryGet key with 
            | Some res -> res
            | None -> 
                let tcConfigP = TcConfigProvider.Constant(tcConfig)
                let ((tcGlobals,tcImports) as res) = TcImports.BuildFrameworkTcImports (tcConfigP, frameworkDLLs, nonFrameworkResolutions)
                frameworkTcImportsCache.Put(key,res)
                tcGlobals,tcImports
        tcGlobals,frameworkTcImports,nonFrameworkResolutions,unresolved


    /// An error logger that captures errors and eventually sends a single error or warning for all the errors and warning in a file
    type CompilationErrorLogger (debugName:string, tcConfig:TcConfig, errorLogger:ErrorLogger) = 
        inherit ErrorLogger("CompilationErrorLogger("+debugName+")")
            
        let warningsSeenInScope = new ResizeArray<_>()
        let errorsSeenInScope = new ResizeArray<_>()
            
        let warningOrError warn exn = 
            let warn = warn && not (ReportWarningAsError (tcConfig.globalWarnLevel, tcConfig.specificWarnOff, tcConfig.specificWarnOn, tcConfig.specificWarnAsError, tcConfig.specificWarnAsWarn, tcConfig.globalWarnAsError) exn)                
            if not warn then
                errorsSeenInScope.Add(exn)
                errorLogger.ErrorSink(exn)                
            else if ReportWarning (tcConfig.globalWarnLevel, tcConfig.specificWarnOff, tcConfig.specificWarnOn) exn then 
                warningsSeenInScope.Add(exn)
                errorLogger.WarnSink(exn)                    

        override x.WarnSinkImpl(exn) = warningOrError true exn
        override x.ErrorSinkImpl(exn) = warningOrError false exn
        override x.ErrorCount = errorLogger.ErrorCount 

        member x.GetErrors() = 
            let errorsAndWarnings = (errorsSeenInScope |> ResizeArray.toList |> List.map(fun e->e,true)) @ (warningsSeenInScope |> ResizeArray.toList |> List.map(fun e->e,false))
            errorsAndWarnings


    /// This represents the global state established as each task function runs as part of the build
    ///
    /// Use to reset error and warning handlers            
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

    type IncrementalBuilder(tcConfig : TcConfig, projectDirectory : string, assemblyName, niceNameGen : Ast.NiceNameGenerator, lexResourceManager,
                            sourceFiles:string list, ensureReactive, errorLogger:ErrorLogger, 
                            keepGeneratedTypedAssembly:bool)
               =
        //use t = Trace.Call("IncrementalBuildVerbose", "Create", fun _ -> sprintf " tcConfig.includes = %A" tcConfig.includes)
        
        let tcConfigP = TcConfigProvider.Constant(tcConfig)
        let importsInvalidated = new Event<string>()
        let beforeTypeCheckFile = new Event<_>()

        // Resolve assemblies and create the framework TcImports. This is done when constructing the
        // builder itself, rather than as an incremental task. This caches a level of "system" references. No type providers are 
        // included in these references. 
        let (tcGlobals,frameworkTcImports,nonFrameworkResolutions,unresolvedReferences) = GetFrameworkTcImports tcConfig
        
        // Check for the existence of loaded sources and prepend them to the sources list if present.
        let sourceFiles = tcConfig.GetAvailableLoadedSources() @ (sourceFiles|>List.map(fun s -> rangeStartup,s))

        // Mark up the source files with an indicator flag indicating if they are the last source file in the project
        let sourceFiles = 
            let flags = tcConfig.ComputeCanContainEntryPoint(sourceFiles |> List.map snd)
            (sourceFiles,flags) ||> List.map2 (fun (m,nm) flag -> (m,nm,flag))
        
        // Get the original referenced assembly names
        // do System.Diagnostics.Debug.Assert(not((sprintf "%A" nonFrameworkResolutions).Contains("System.dll")),sprintf "Did not expect a system import here. %A" nonFrameworkResolutions)

        // Get the names and time stamps of all the non-framework referenced assemblies, which will act 
        // as inputs to one of the nodes in the build. 
        //
        // This operation is done when constructing the builder itself, rather than as an incremental task. 
        let nonFrameworkAssemblyInputs = 
            // Note we are not calling errorLogger.GetErrors() anywhere for this task. 
            // REVIEW: Consider if this is ok. I believe so, because this is a background build and we aren't currently reporting errors from the background build. 
            let errorLogger = CompilationErrorLogger("nonFrameworkAssemblyInputs", tcConfig, errorLogger)
            // Return the disposable object that cleans up
            use _holder = new CompilationGlobalsScope(errorLogger,BuildPhase.Parameter, projectDirectory) 

            [ for r in nonFrameworkResolutions do
                let originalTimeStamp = 
                    try 
                        if FileSystem.SafeExists(r.resolvedPath) then
                            let result = FileSystem.GetLastWriteTimeShim(r.resolvedPath)
                            Trace.Print("FSharpBackgroundBuildVerbose", fun _ -> sprintf "Found referenced assembly '%s'.\n" r.resolvedPath)
                            result
                        else
                            Trace.Print("FSharpBackgroundBuildVerbose", fun _ -> sprintf "Did not find referenced assembly '%s' on disk.\n" r.resolvedPath)
                            DateTime.Now                               
                    with e -> 
                        Trace.Print("FSharpBackgroundBuildVerbose", fun _ -> sprintf "Did not find referenced assembly '%s' due to exception.\n" r.resolvedPath)
                        // Note we are not calling errorLogger.GetErrors() anywhere for this task. This warning will not be reported...
                        // REVIEW: Consider if this is ok. I believe so, because this is a background build and we aren't currently reporting errors from the background build. 
                        errorLogger.Warning(e)
                        DateTime.Now                               
                yield (r.originalReference.Range,r.resolvedPath,originalTimeStamp)  ]
            
        // The IncrementalBuilder needs to hold up to one item that needs to be disposed, which is the tcImports for the incremental
        // build. 
        let mutable cleanupItem = None : TcImports option
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

        ///----------------------------------------------------
        /// START OF BUILD TASK FUNCTIONS 
                
        /// This is a build task function that gets placed into the build rules as the computation for a VectorStamp
        ///
        /// Get the timestamp of the given file name.
        let StampFileNameTask (_m:range, filename:string, _isLastCompiland:bool) =
            assertNotDisposed()
            FileSystem.GetLastWriteTimeShim(filename)
                            
        /// This is a build task function that gets placed into the build rules as the computation for a VectorMap
        ///
        /// Parse the given files and return the given inputs. This function is expected to be
        /// able to be called with a subset of sourceFiles and return the corresponding subset of
        /// parsed inputs. 
        let ParseTask (sourceRange:range,filename:string,isLastCompiland) =
            assertNotDisposed()
            let errorLogger = CompilationErrorLogger("ParseTask", tcConfig, errorLogger)
            // Return the disposable object that cleans up
            use _holder = new CompilationGlobalsScope(errorLogger, BuildPhase.Parse, projectDirectory)

            Trace.Print("FSharpBackgroundBuild", fun _ -> sprintf "Parsing %s..." filename)
            
            try  
                IncrementalBuilderEventsMRU.Add(IBEParsed filename)
                let result = ParseOneInputFile(tcConfig,lexResourceManager, [], filename ,isLastCompiland,errorLogger,(*retryLocked*)true)
                Trace.PrintLine("FSharpBackgroundBuildVerbose", fun _ -> sprintf "done.")
                result,sourceRange,filename,errorLogger.GetErrors ()
            with exn -> 
                System.Diagnostics.Debug.Assert(false, sprintf "unexpected failure in IncrementalFSharpBuild.Parse\nerror = %s" (exn.ToString()))
                failwith "last chance failure"  
                
        
        /// This is a build task function that gets placed into the build rules as the computation for a Vector.Stamp
        ///
        /// Timestamps of referenced assemblies are taken from the file's timestamp.
        let TimestampReferencedAssemblyTask (_range, filename, originalTimeStamp) =
            assertNotDisposed()
            // Note: we are not calling errorLogger.GetErrors() anywhere. Is this a problem?
            let errorLogger = CompilationErrorLogger("TimestampReferencedAssemblyTask", tcConfig, errorLogger)
            // Return the disposable object that cleans up
            use _holder = new CompilationGlobalsScope(errorLogger, BuildPhase.Parameter, projectDirectory) // Parameter because -r reference

            let timestamp = 
                try
                    if FileSystem.SafeExists(filename) then
                        let ts = FileSystem.GetLastWriteTimeShim(filename)
                        if ts<>originalTimeStamp then 
                            Trace.PrintLine("FSharpBackgroundBuildVerbose", fun _ -> sprintf "Noticing change in timestamp of file %s from %A to %A" filename originalTimeStamp ts)
                        else    
                            Trace.PrintLine("FSharpBackgroundBuildVerbose", fun _ -> sprintf "Noticing no change in timestamp of file %s (still %A)" filename originalTimeStamp)
                        ts
                    else
                        Trace.PrintLine("FSharpBackgroundBuildVerbose", fun _ -> sprintf "Noticing that file %s was deleted, but ignoring that for timestamp checking" filename)
                        originalTimeStamp
                with exn -> 
                    // For example, malformed filename
                    Trace.PrintLine("FSharpBackgroundBuildVerbose", fun _ -> sprintf "Exception when checking stamp of file %s, using old stamp %A" filename originalTimeStamp)
                    // Note we are not calling errorLogger.GetErrors() anywhere for this task. This warning will not be reported...
                    // REVIEW: Consider if this is ok. I believe so, because this is a background build and we aren't currently reporting errors from the background build. 
                    errorLogger.Warning exn
                    originalTimeStamp                      
            timestamp
                
         
        /// This is a build task function that gets placed into the build rules as the computation for a Vector.Demultiplex
        ///
        // Link all the assemblies together and produce the input typecheck accumulator               
        let CombineImportedAssembliesTask _ : TypeCheckAccumulator =
            assertNotDisposed()
            let errorLogger = CompilationErrorLogger("CombineImportedAssembliesTask", tcConfig, errorLogger)
            // Return the disposable object that cleans up
            use _holder = new CompilationGlobalsScope(errorLogger, BuildPhase.Parameter, projectDirectory)

            let tcImports = 
                try
                    // We dispose any previous tcImports, for the case where a dependency changed which caused this part
                    // of the partial build to be re-evaluated.
                    disposeCleanupItem()

                    Trace.PrintLine("FSharpBackgroundBuild", fun _ -> "About to (re)create tcImports")
                    let tcImports = TcImports.BuildNonFrameworkTcImports(
#if TYPE_PROVIDER_SECURITY
                                                                         None,
#endif
                                                                         tcConfigP,tcGlobals,frameworkTcImports,nonFrameworkResolutions,unresolvedReferences)  
#if EXTENSIONTYPING
                    for ccu in tcImports.GetCcusExcludingBase() do
                        // When a CCU reports an invalidation, merge them together and just report a 
                        // general "imports invalidated". This triggers a rebuild.
                        ccu.Deref.InvalidateEvent.Add(fun msg -> importsInvalidated.Trigger msg)
#endif
                    
                    Trace.PrintLine("FSharpBackgroundBuild", fun _ -> "(Re)created tcImports")
                    
                    // The tcImports must be cleaned up if this builder ever gets disposed. We also dispose any previous
                    // tcImports should we be re-creating an entry because a dependency changed which caused this part
                    // of the partial build to be re-evaluated.
                    setCleanupItem tcImports

                    tcImports
                with e -> 
                    System.Diagnostics.Debug.Assert(false, sprintf "Could not BuildAllReferencedDllTcImports %A" e)
                    Trace.PrintLine("FSharpBackgroundBuild", fun _ -> "Failed to recreate tcImports\n  %A")
                    errorLogger.Warning(e)
                    frameworkTcImports           

            let tcEnv0 = GetInitialTcEnv (Some assemblyName, rangeStartup, tcConfig, tcImports, tcGlobals)
            let tcState0 = GetInitialTcState (rangeStartup, assemblyName, tcConfig, tcGlobals, tcImports, niceNameGen, tcEnv0)
            let tcAcc = 
                { tcGlobals=tcGlobals
                  tcImports=tcImports
                  tcState=tcState0
                  tcConfig=tcConfig
                  tcEnv=tcEnv0
                  topAttribs=None
                  typedImplFiles=[]
                  errors=errorLogger.GetErrors() }   
            tcAcc
                
        /// This is a build task function that gets placed into the build rules as the computation for a Vector.ScanLeft
        ///
        /// Type check all files.     
        let TypeCheckTask (tcAcc:TypeCheckAccumulator) input : Eventually<TypeCheckAccumulator> =    
            assertNotDisposed()
            match input with 
            | Some input, _sourceRange, filename, parseErrors->
                IncrementalBuilderEventsMRU.Add(IBETypechecked filename)
                let capturingErrorLogger = CompilationErrorLogger("TypeCheckTask", tcConfig, errorLogger)
                let errorLogger = GetErrorLoggerFilteringByScopedPragmas(false,GetScopedPragmasForInput(input),capturingErrorLogger)
                let tcAcc = {tcAcc with errors = tcAcc.errors @ parseErrors}
                let fullComputation = 
                    eventually {
                        Trace.PrintLine("FSharpBackgroundBuild", fun _ -> sprintf "Typechecking %s..." filename)                
                        beforeTypeCheckFile.Trigger filename
                        let! (tcEnv,topAttribs,typedImplFiles),tcState = 
                            TypeCheckOneInputEventually ((fun () -> errorLogger.ErrorCount > 0),
                                                         tcConfig,tcAcc.tcImports,
                                                         tcAcc.tcGlobals,
                                                         None,
                                                         NameResolution.TcResultsSink.NoSink,
                                                         tcAcc.tcState,input)
                        
                        /// Only keep the typed interface files when doing a "full" build for fsc.exe, otherwise just throw them away
                        let typedImplFiles = if keepGeneratedTypedAssembly then typedImplFiles else []
                        Trace.PrintLine("FSharpBackgroundBuild", fun _ -> sprintf "done.")
                        return {tcAcc with tcState=tcState 
                                           tcEnv=tcEnv
                                           topAttribs=Some topAttribs
                                           typedImplFiles=typedImplFiles
                                           errors = tcAcc.errors @ capturingErrorLogger.GetErrors() } 
                    }
                    
                // Run part of the Eventually<_> computation until a timeout is reached. If not complete, 
                // return a new Eventually<_> computation which recursively runs more of the computation.
                //   - When the whole thing is finished commit the error results sent through the errorLogger.
                //   - Each time we do real work we reinstall the CompilationGlobalsScope
                if ensureReactive then 
                    let timeSlicedComputation = 
                        fullComputation |> 
                           Eventually.repeatedlyProgressUntilDoneOrTimeShareOver 
                              maxTimeShareMilliseconds
                              (fun f -> 
                                  // Reinstall the compilation globals each time we start or restart
                                  use unwind = new CompilationGlobalsScope (errorLogger, BuildPhase.TypeCheck, projectDirectory) 
                                  Trace.Print("FSharpBackgroundBuildVerbose", fun _ -> sprintf "continuing %s.\n" filename)
                                  f()
                                  (* unwind dispose *)
                              )
                               
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
            Trace.PrintLine("FSharpBackgroundBuildVerbose", fun _ -> sprintf "Finalizing Type Check" )
            let finalAcc = tcStates.[tcStates.Length-1]
            let results = tcStates |> List.ofArray |> List.map (fun acc-> acc.tcEnv, (Option.get acc.topAttribs), acc.typedImplFiles)
            let (tcEnvAtEndOfLastFile,topAttrs,mimpls),tcState = TypeCheckMultipleInputsFinish (results,finalAcc.tcState)
            let tcState,tassembly = TypeCheckClosedInputSetFinish (mimpls,tcState)
            tcState, topAttrs, tassembly, tcEnvAtEndOfLastFile, finalAcc.tcImports, finalAcc.tcGlobals, finalAcc.tcConfig

        // END OF BUILD TASK FUNCTIONS
        // ---------------------------------------------------------------------------------------------            

        // ---------------------------------------------------------------------------------------------            
        // START OF BUILD DESCRIPTION

        let buildDescription            = new BuildDescriptionScope ()

        // Inputs
        let filenames                   = InputVector<range*string*bool> "FileNames"
        let referencedAssemblies        = InputVector<range*string*DateTime> "ReferencedAssemblies"
        
        // Build
        let stampedFileNames            = Vector.Stamp "SourceFileTimeStamps" StampFileNameTask filenames
        let parseTrees                  = Vector.Map "Parse" ParseTask stampedFileNames
        let stampedReferencedAssemblies = Vector.Stamp "TimestampReferencedAssembly" TimestampReferencedAssemblyTask referencedAssemblies
        let initialTcAcc                = Vector.Demultiplex "CombineImportedAssemblies" CombineImportedAssembliesTask stampedReferencedAssemblies
        let tcStates                    = Vector.ScanLeft "TypeCheck" TypeCheckTask initialTcAcc parseTrees
        let finalizedTypeCheck          = Vector.Demultiplex "FinalizeTypeCheck" FinalizeTypeCheckTask tcStates

        // Outputs
        do buildDescription.DeclareVectorOutput ("ParseTrees", parseTrees)
        do buildDescription.DeclareVectorOutput ("TypeCheckingStates",tcStates)
        do buildDescription.DeclareScalarOutput ("InitialTcAcc", initialTcAcc)
        do buildDescription.DeclareScalarOutput ("FinalizeTypeCheck", finalizedTypeCheck)

        // END OF BUILD DESCRIPTION
        // ---------------------------------------------------------------------------------------------            


        let fileDependencies = 
            let unresolvedFileDependencies = 
                unresolvedReferences
                |> List.map (function Microsoft.FSharp.Compiler.CompileOps.UnresolvedAssemblyReference(referenceText, _) -> referenceText)
                |> List.filter(fun referenceText->not(Path.IsInvalidPath(referenceText))) // Exclude things that are definitely not a file name
                |> List.map(fun referenceText -> if FileSystem.IsPathRootedShim(referenceText) then referenceText else System.IO.Path.Combine(projectDirectory,referenceText))
                |> List.map (fun file->{Filename =  file; ExistenceDependency = true; IncrementalBuildDependency = true })
            let resolvedFileDependencies = 
                nonFrameworkResolutions |> List.map (fun r -> {Filename =  r.resolvedPath ; ExistenceDependency = true; IncrementalBuildDependency = true })
#if DEBUG
            do resolvedFileDependencies |> List.iter (fun x -> System.Diagnostics.Debug.Assert(FileSystem.IsPathRootedShim(x.Filename), sprintf "file dependency should be absolute path: '%s'" x.Filename))
#endif        
            let sourceFileDependencies = 
                sourceFiles  |> List.map (fun (_,f,_) -> {Filename =  f ; ExistenceDependency = true; IncrementalBuildDependency = true })               
            List.concat [unresolvedFileDependencies;resolvedFileDependencies;sourceFileDependencies]

#if TRACK_DOWN_EXTRA_BACKSLASHES        
        do fileDependencies |> List.iter(fun dep ->
              Debug.Assert(not(dep.Filename.Contains(@"\\")), "IncrementalBuild.Create results in a non-canonical filename with extra backslashes: "^dep.Filename)
              )
#endif        

        do IncrementalBuilderEventsMRU.Add(IBEDeleted)
        let buildInputs = ["FileNames", sourceFiles.Length, sourceFiles |> List.map box
                           "ReferencedAssemblies", nonFrameworkAssemblyInputs.Length, nonFrameworkAssemblyInputs |> List.map box ]

        // This is the intial representation of progress through the build, i.e. we have made no progress.
        let mutable partialBuild = buildDescription.GetInitialPartialBuild (buildInputs, [])

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
        member __.BeforeTypeCheckFile = beforeTypeCheckFile.Publish
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

        member __.Step () =  
            match IncrementalBuild.Step "TypeCheckingStates" partialBuild with 
            | None -> 
                false
            | Some newPartialBuild -> 
                partialBuild <- newPartialBuild
                true
    
        member __.GetAntecedentTypeCheckResultsBySlot slotOfFile = 
            let result = 
                match slotOfFile with
                | (*first file*) 0 -> GetScalarResult<TypeCheckAccumulator>("InitialTcAcc",partialBuild)
                | _ -> GetVectorResultBySlot<TypeCheckAccumulator>("TypeCheckingStates",slotOfFile-1,partialBuild)  
        
            match result with
            | Some({tcState=tcState; tcGlobals=tcGlobals; tcConfig=tcConfig; tcImports=tcImports; errors=errors},timestamp) ->
                Some(tcState,tcImports,tcGlobals,tcConfig,errors,timestamp)
            | _->None
        
        member __.TypeCheck() = 
            let newPartialBuild = IncrementalBuild.Eval "FinalizeTypeCheck" partialBuild
            partialBuild <- newPartialBuild
            match GetScalarResult<TcState * TypeChecker.TopAttribs * Tast.TypedAssembly * TypeChecker.TcEnv * TcImports * TcGlobals * TcConfig>("FinalizeTypeCheck",partialBuild) with
            | Some((tcState,topAttribs,typedAssembly,tcEnv,tcImports,tcGlobals,tcConfig),_) -> tcState,topAttribs,typedAssembly,tcEnv,tcImports,tcGlobals,tcConfig
            | None -> failwith "Build was not evaluated."
        
        member __.GetSlotOfFileName(filename:string) =
            // Get the slot of the given file and force it to build.
            let CompareFileNames (_,f1,_) (_,f2,_) = 
                let result = 
                       System.String.Compare(f1,f2,StringComparison.CurrentCultureIgnoreCase)=0
                    || System.String.Compare(FileSystem.GetFullPathShim(f1),FileSystem.GetFullPathShim(f2),StringComparison.CurrentCultureIgnoreCase)=0
                result
            GetSlotByInput("FileNames",(rangeStartup,filename,false),partialBuild,CompareFileNames)
        
#if NO_QUICK_SEARCH_HELPERS // only used in QuickSearch prototype
#else
        member __.GetSlotsCount () =
          let expr = GetExprByName(partialBuild,"FileNames")
          let id = BuildRuleExpr.GetId(expr)
          match partialBuild.Results.TryFind(id) with
          | Some(VectorResult vr) -> vr.Size
          | _ -> failwith "Cannot know sizes"
      
        member this.GetParseResultsBySlot slot =
          let result = GetVectorResultBySlot<Ast.ParsedInput option * Range.range * string>("ParseTrees",slot,partialBuild)  
          match result with
          | Some ((inputOpt,range,fileName), _) -> inputOpt, range, fileName
          | None -> 
                let newPartialBuild = IncrementalBuild.Eval "ParseTrees" partialBuild
                partialBuild <- newPartialBuild
                this.GetParseResultsBySlot slot
        
#endif // 

            //------------------------------------------------------------------------------------
            // CreateIncrementalBuilder (for background type checking). Note that fsc.fs also
            // creates an incremental builder used by the command line compiler.
            //-----------------------------------------------------------------------------------
        static member CreateBackgroundBuilderForProjectOptions (scriptClosureOptions:LoadClosure option, sourceFiles:string list, commandLineArgs:string list, projectDirectory, useScriptResolutionRules, isIncompleteTypeCheckEnvironment) =
    
            // Trap and report warnings and errors from creation.
            use errorScope = new ErrorScope()

            // Create the builder.         
            // Share intern'd strings across all lexing/parsing
            let resourceManager = new Lexhelp.LexResourceManager() 

            /// Create a type-check configuration
            let tcConfigB = 
                let defaultFSharpBinariesDir = Internal.Utilities.FSharpEnvironment.BinFolderOfDefaultFSharpCompiler.Value
                    
                // see also fsc.fs:ProcessCommandLineArgsAndImportAssemblies(), as there are many similarities to where the PS creates a tcConfigB
                let tcConfigB = 
                    TcConfigBuilder.CreateNew(defaultFSharpBinariesDir, implicitIncludeDir=projectDirectory, 
                                              optimizeForMemory=true, isInteractive=false, isInvalidationSupported=true) 
                // The following uses more memory but means we don't take read-exclusions on the DLLs we reference 
                // Could detect well-known assemblies--ie System.dll--and open them with read-locks 
                tcConfigB.openBinariesInMemory <- true
                tcConfigB.resolutionEnvironment 
                    <- if useScriptResolutionRules 
                        then MSBuildResolver.DesigntimeLike  
                        else MSBuildResolver.CompileTimeLike
                
                tcConfigB.conditionalCompilationDefines <- 
                    let define = if useScriptResolutionRules then "INTERACTIVE" else "COMPILED"
                    define::tcConfigB.conditionalCompilationDefines

                // Apply command-line arguments.
                try
                    ParseCompilerOptions ((fun _sourceOrDll -> () ), CompileOptions.GetCoreServiceCompilerOptions tcConfigB, commandLineArgs)
                with e -> errorRecovery e range0


                // Never open PDB files for the language service, even if --standalone is specified
                tcConfigB.openDebugInformationForLaterStaticLinking <- false
        
                if tcConfigB.framework then
                    // ~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-
                    // If you see a failure here running unittests consider whether it it caused by 
                    // a mismatched version of Microsoft.Build.Framework. Run unittests under a debugger. If
                    // you see an old version of Microsoft.Build.*.dll getting loaded it it is likely caused by
                    // using an old ITask or ITaskItem from some tasks assembly.
                    // I solved this problem by adding a Unittests.config.dll which has a binding redirect to 
                    // the current (right now, 4.0.0.0) version of the tasks assembly.
                    // ~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-~-
                    System.Diagnostics.Debug.Assert(false, "Language service requires --noframework flag")
                    tcConfigB.framework<-false
                tcConfigB 
            match scriptClosureOptions with
            | Some closure -> 
                let dllReferences = 
                    [for reference in tcConfigB.referencedDLLs do
                        // If there's (one or more) resolutions of closure references then yield them all
                        match closure.References  |> List.tryFind (fun (resolved,_)->resolved=reference.Text) with
                        | Some(resolved,closureReferences) -> 
                            for closureReference in closureReferences do
                                yield AssemblyReference(closureReference.originalReference.Range, resolved)
                        | None -> yield reference]
                tcConfigB.referencedDLLs<-[]
                // Add one by one to remove duplicates
                for dllReference in dllReferences do
                    tcConfigB.AddReferencedAssemblyByPath(dllReference.Range,dllReference.Text)
                tcConfigB.knownUnresolvedReferences<-closure.UnresolvedReferences
            | None -> ()
            // Make sure System.Numerics is referenced for out-of-project .fs files
            if isIncompleteTypeCheckEnvironment then 
                tcConfigB.addVersionSpecificFrameworkReferences <- true 

            let _, _, assemblyName = tcConfigB.DecideNames sourceFiles
        
            let tcConfig = TcConfig.Create(tcConfigB,validate=true)

            let niceNameGen = NiceNameGenerator()
        
            // Sink internal errors and warnings.
            // Q: Why is it ok to ignore these?
            // These are errors from the background build of files the user doesn't see. Squiggles will appear in the editted file via the foreground parse\typecheck
            let warnSink (exn:PhasedError) = Trace.PrintLine("IncrementalBuild", (exn.ToString >> sprintf "Background warning: %s"))
            let errorSink (exn:PhasedError) = Trace.PrintLine("IncrementalBuild", (exn.ToString >> sprintf "Background error: %s"))

            let errorLogger =
                { new ErrorLogger("CreateIncrementalBuilder") with 
                      member x.ErrorCount=0
                      member x.WarnSinkImpl e = warnSink e
                      member x.ErrorSinkImpl e = errorSink e }

            let builder = 
                new IncrementalBuilder 
                        (tcConfig, projectDirectory, assemblyName, niceNameGen,
                        resourceManager, sourceFiles, true, // stay reactive
                        errorLogger, false // please discard implementation results
                        )
                                 
            Trace.PrintLine("IncrementalBuild", fun () -> sprintf "CreateIncrementalBuilder: %A" builder.Dependencies)
    #if DEBUG
            builder.Dependencies|> List.iter (fun df -> System.Diagnostics.Debug.Assert(FileSystem.IsPathRootedShim(df.Filename), sprintf "dependency file was not absolute: '%s'" df.Filename))
    #endif

            (builder, errorScope.ErrorsAndWarnings)

