// (c) Microsoft Corporation 2005-2007.

module internal FsLexYacc.FsYacc.AST

#nowarn "62" // This construct is for ML compatibility.


open System
open System.Collections.Generic
open Printf
open Microsoft.FSharp.Collections
open Internal.Utilities
open Internal.Utilities.Text.Lexing

/// An active pattern that should be in the F# standard library
let (|KeyValue|) (kvp:KeyValuePair<_,_>) = kvp.Key,kvp.Value


type Identifier = string
type Code = string * Position

type ParserSpec= 
    { Header         : Code;
      Tokens         : (Identifier * string option) list;
      Types          : (Identifier * string) list;
      Associativities: (Identifier * Associativity) list list;
      StartSymbols   : Identifier list;
      Rules          : (Identifier * Rule list) list }
      
and Rule = Rule of Identifier list * Identifier option * Code option
and Associativity = LeftAssoc | RightAssoc | NonAssoc

type Terminal = string
type NonTerminal = string
type Symbol = Terminal of Terminal | NonTerminal of NonTerminal
type Symbols = Symbol list


//---------------------------------------------------------------------
// Output Raw Parser Spec AST

let StringOfSym sym = match sym with Terminal s -> "'" ^ s ^ "'" | NonTerminal s -> s

let OutputSym os sym = fprintf os "%s" (StringOfSym sym)

let OutputSyms os syms =
    fprintf os "%s" (String.Join(" ",Array.map StringOfSym syms))

let OutputTerminalSet os (tset:string seq)  =
    fprintf os "%s" (String.Join(";", tset |> Seq.toArray))

let OutputAssoc os p = 
    match p with 
    | LeftAssoc -> fprintf os "left"
    | RightAssoc -> fprintf os "right"
    | NonAssoc -> fprintf os "nonassoc"


//---------------------------------------------------------------------
// PreProcess Raw Parser Spec AST

type PrecedenceInfo = 
    | ExplicitPrec of Associativity * int 
    | NoPrecedence
      
type Production = Production of NonTerminal * PrecedenceInfo * Symbols * Code option

type ProcessedParserSpec = 
    { Terminals: (Terminal * PrecedenceInfo) list;
      NonTerminals: NonTerminal list;
      Productions: Production list;
      StartSymbols: NonTerminal list }


let ProcessParserSpecAst (spec: ParserSpec) = 
    let explicitPrecInfo = 
        spec.Associativities 
        |> List.mapi (fun n precSpecs -> precSpecs |> List.map (fun (precSym, assoc) -> precSym,ExplicitPrec (assoc, 10000 - n)))
        |> List.concat
    
    for (key,_) in explicitPrecInfo |> Seq.countBy fst |> Seq.filter (fun (_,n) -> n > 1)  do
        failwithf "%s is given two associativities" key
    
    let explicitPrecInfo = 
        explicitPrecInfo |> Map.ofList

    let implicitSymPrecInfo = NoPrecedence
    let terminals = List.map fst spec.Tokens @ ["error"]in 
    let terminalSet = Set.ofList terminals
    let IsTerminal z = terminalSet.Contains(z)
    let prec_of_terminal sym implicitPrecInfo = 
       if explicitPrecInfo.ContainsKey(sym) then explicitPrecInfo.[sym]
       else match implicitPrecInfo with Some x -> x | None -> implicitSymPrecInfo
       
    let mkSym s = if IsTerminal s then Terminal s else NonTerminal s
    let prods =  
        spec.Rules |> List.mapi (fun i (nonterm,rules) -> 
            rules |> List.mapi (fun j (Rule(syms,precsym,code)) -> 
                let precInfo = 
                    let precsym = List.foldBack (fun x acc -> match acc with Some _ -> acc | None -> match x with z when IsTerminal z -> Some z | _ -> acc) syms precsym
                    let implicitPrecInfo = NoPrecedence
                    match precsym with 
                    | None -> implicitPrecInfo 
                    | Some sym -> if explicitPrecInfo.ContainsKey(sym) then explicitPrecInfo.[sym] else implicitPrecInfo
                Production(nonterm, precInfo, List.map mkSym syms, code)))
         |> List.concat
    let nonTerminals = List.map fst spec.Rules
    let nonTerminalSet = Set.ofList nonTerminals
    let checkNonTerminal nt =  
        if nt <> "error" && not (nonTerminalSet.Contains(nt)) then 
            failwith (sprintf "NonTerminal '%s' has no productions" nt)

    for (Production(nt,_,syms,_)) in prods do
        for sym in syms do 
           match sym with 
           | NonTerminal nt -> 
               checkNonTerminal nt 
           | Terminal t ->  
               if not (IsTerminal t) then failwith (sprintf "token %s is not declared" t)
           
    if spec.StartSymbols= [] then (failwith "at least one %start declaration is required");

    for (nt,_) in spec.Types do 
        checkNonTerminal nt;

    let terminals = terminals |> List.map (fun t -> (t,prec_of_terminal t None)) 

    { Terminals=terminals;
      NonTerminals=nonTerminals;
      Productions=prods;
      StartSymbols=spec.StartSymbols }


//-------------------------------------------------
// Process LALR(1) grammars to tables

type ProductionIndex = int
type ProdictionDotIndex = int

/// Represent (ProductionIndex,ProdictionDotIndex) as one integer 
type Item0 = uint32  

let mkItem0 (prodIdx,dotIdx) : Item0 = (uint32 prodIdx <<< 16) ||| uint32 dotIdx
let prodIdx_of_item0 (item0:Item0) = int32 (item0 >>> 16)
let dotIdx_of_item0 (item0:Item0) = int32 (item0 &&& 0xFFFFu)

/// Part of the output of CompilerLalrParserSpec
type Action = 
  | Shift of int
  | Reduce of ProductionIndex
  | Accept
  | Error
    
let outputPrecInfo os p = 
    match p with 
    | ExplicitPrec (assoc,n) -> fprintf os "explicit %a %d" OutputAssoc assoc n
    | NoPrecedence  -> fprintf os "noprec"


/// LR(0) kernels
type Kernel = Set<Item0>

/// Indexes of LR(0) kernels in the KernelTable
type KernelIdx = int

/// Indexes in the TerminalTable and NonTerminalTable
type TerminalIndex = int
type NonTerminalIndex = int

/// Representation of Symbols.
/// Ideally would be declared as 
///    type SymbolIndex = PTerminal of TerminalIndex | PNonTerminal of NonTerminalIndex
/// but for performance reasons we embed as a simple integer (saves ~10%)
///
/// We use an active pattern to reverse the embedding.
type SymbolIndex = int
let PTerminal(i:TerminalIndex) : SymbolIndex = -i-1
let PNonTerminal(i:NonTerminalIndex) : SymbolIndex = i
let (|PTerminal|PNonTerminal|) x = if x < 0 then PTerminal (-(x+1)) else PNonTerminal x

type SymbolIndexes = SymbolIndex list

/// Indexes in the LookaheadTable, SpontaneousTable, PropagateTable
/// Embed in a single integer, since these are faster
/// keys for the dictionary hash tables
///
/// Logically:
///
///   type KernelItemIndex = KernelItemIdx of KernelIdx * Item0
type KernelItemIndex = int64
let KernelItemIdx (i1,i2) = ((int64 i1) <<< 32) ||| int64 i2


/// Indexes into the memoizing table for the Goto computations
/// Embed in a single integer, since these are faster
/// keys for the dictionary hash tables
///
/// Logically:
///
///   type GotoItemIndex = GotoItemIdx of KernelIdx * SymbolIndex
type GotoItemIndex = uint64
let GotoItemIdx (i1:KernelIdx,i2:SymbolIndex) = (uint64 (uint32 i1) <<< 32) ||| uint64 (uint32 i2)
let (|GotoItemIdx|) (i64:uint64) = int32 ((i64 >>> 32) &&& 0xFFFFFFFFUL), int32 (i64 &&& 0xFFFFFFFFUL)

/// Create a work list and loop until it is exhausted, calling a worker function for
/// each element. Pass a function to queue additional work on the work list 
/// to the worker function
let ProcessWorkList start f =
    let work = ref (start : 'a list)
    let queueWork = (fun x -> work := x :: !work)
    let rec loop() = 
        match !work with 
        | [] -> ()
        | x :: t -> 
            work := t; 
            f queueWork x;
            loop()
    loop()

/// A standard utility to compute a least fixed point of a set under a generative computation
let LeastFixedPoint f set = 
    let acc = ref set
    ProcessWorkList (Set.toList set) (fun queueWork item ->
          f(item) |> List.iter (fun i2 -> if not (Set.contains i2 !acc) then (acc := Set.add i2 !acc; queueWork i2)) )
    !acc

/// A general standard memoization utility. Be sure to apply to only one (function) argument to build the
/// residue function!
let Memoize f = 
    let t = new Dictionary<_,_>(1000)
    fun x -> 
        let ok,v = t.TryGetValue(x) 
        if ok then v else let res = f x in t.[x] <- res; res 

/// A standard utility to create a dictionary from a list of pairs
let CreateDictionary xs = 
    let dict = new Dictionary<_,_>()
    for x,y in xs do dict.Add(x,y)
    dict

/// Allocate indexes for each non-terminal
type NonTerminalTable(nonTerminals:NonTerminal list) = 
    let nonterminalsWithIdxs = List.mapi (fun (i:NonTerminalIndex) n -> (i,n)) nonTerminals
    let nonterminalIdxs = List.map fst nonterminalsWithIdxs
    let a = Array.ofList nonTerminals
    let b = CreateDictionary [ for i,x in nonterminalsWithIdxs -> x,i ];
    member table.OfIndex(i) = a.[i]
    member table.ToIndex(i) = b.[i]
    member table.Indexes = nonterminalIdxs

/// Allocate indexes for each terminal
type TerminalTable(terminals:(Terminal * PrecedenceInfo) list) = 
    let terminalsWithIdxs = List.mapi (fun i (t,_) -> (i,t)) terminals
    let terminalIdxs = List.map fst terminalsWithIdxs
    let a = Array.ofList (List.map fst terminals)
    let b = Array.ofList (List.map snd terminals)
    let c = CreateDictionary [ for i,x in terminalsWithIdxs -> x,i ]

    member table.OfIndex(i) = a.[i]
    member table.PrecInfoOfIndex(i) = b.[i]
    member table.ToIndex(i) = c.[i]
    member table.Indexes = terminalIdxs

/// Allocate indexes for each production
type ProductionTable(ntTab:NonTerminalTable, termTab:TerminalTable, nonTerminals:string list, prods: Production list) =
    let prodsWithIdxs = List.mapi (fun i n -> (i,n)) prods
    let a =  
        prodsWithIdxs
        |> List.map(fun (_,Production(_,_,syms,_)) -> 
              syms 
              |> Array.ofList  
              |> Array.map (function 
                            | Terminal t -> PTerminal (termTab.ToIndex t) 
                            | NonTerminal nt -> PNonTerminal (ntTab.ToIndex nt )) )
        |> Array.ofList
    let b = Array.ofList (List.map (fun (_,Production(nt,_,_,_)) -> ntTab.ToIndex nt) prodsWithIdxs)
    let c = Array.ofList (List.map (fun (_,Production(_,prec,_,_)) -> prec) prodsWithIdxs)
    let productions = 
        nonTerminals
        |> List.map(fun nt -> (ntTab.ToIndex nt, List.choose (fun (i,Production(nt2,prec,syms,_)) -> if nt2=nt then Some i else None) prodsWithIdxs))
        |> CreateDictionary

    member prodTab.Symbols(i) = a.[i]
    member prodTab.NonTerminal(i) = b.[i]
    member prodTab.Precedence(i) = c.[i]
    member prodTab.Symbol i n = 
        let syms = prodTab.Symbols i
        if n >= syms.Length then None else Some (syms.[n])
    member prodTab.Productions = productions

/// A mutable table maping kernels to sets of lookahead tokens
type LookaheadTable() = 
    let t = new Dictionary<KernelItemIndex,Set<TerminalIndex>>()
    member table.Add(x,y) = 
        let prev = if t.ContainsKey(x) then t.[x] else Set.empty 
        t.[x] <- prev.Add(y)
    member table.Contains(x,y) = t.ContainsKey(x) && t.[x].Contains(y)
    member table.GetLookaheads(idx:KernelItemIndex) = 
        let ok,v = t.TryGetValue(idx)  
        if ok then v else Set.empty
    member table.Count = t |> Seq.fold(fun acc (KeyValue(_,v)) -> v.Count+acc) 0

/// A mutable table giving an index to each LR(0) kernel. Kernels are referred to only by index.
type KernelTable(kernels) =
    // Give an index to each LR(0) kernel, and from now on refer to them only by index 
    // Also develop "kernelItemIdx" to refer to individual items within a kernel 
    let kernelsAndIdxs = List.mapi (fun i x -> (i,x)) kernels
    let kernelIdxs = List.map fst kernelsAndIdxs
    let toIdxMap = Map.ofList [ for i,x in kernelsAndIdxs -> x,i ]
    let ofIdxMap = Array.ofList kernels
    member t.Indexes = kernelIdxs
    member t.Index(kernel) = toIdxMap.[kernel]
    member t.Kernel(i) = ofIdxMap.[i]

/// Hold the results of cpmuting the LALR(1) closure of an LR(0) kernel
type Closure1Table() = 
    let t = new Dictionary<Item0,HashSet<TerminalIndex>>()
    member table.Add(a,b) = 
        if not (t.ContainsKey(a)) then t.[a] <- new HashSet<_>(HashIdentity.Structural)
        t.[a].Add(b)
    member table.Count  = t.Count
    member table.IEnumerable = (t :> seq<_>)
    member table.Contains(a,b) = t.ContainsKey(a) && t.[a].Contains(b)

/// A mutable table giving a lookahead set Set<Terminal> for each kernel. The terminals represent the
/// "spontaneous" items for the kernel. TODO: document this more w.r.t. the Dragon book.
type SpontaneousTable() = 
    let t = new Dictionary<KernelItemIndex,HashSet<TerminalIndex>>()
    member table.Add(a,b) = 
        if not (t.ContainsKey(a)) then t.[a] <- new HashSet<_>(HashIdentity.Structural)
        t.[a].Add(b)
    member table.Count  = t.Count
    member table.IEnumerable = (t :> seq<_>)

/// A mutable table giving a Set<KernelItemIndex> for each kernel. The kernels represent the
/// "propagate" items for the kernel. TODO: document this more w.r.t. the Dragon book.
type PropagateTable() = 
    let t = new Dictionary<KernelItemIndex,HashSet<KernelItemIndex>>()
    member table.Add(a,b) = 
        if not (t.ContainsKey(a)) then t.[a] <- new HashSet<KernelItemIndex>(HashIdentity.Structural)
        t.[a].Add(b)
    member table.Item 
      with get(a) = 
        let ok,v = t.TryGetValue(a) 
        if ok then v :> seq<_> else Seq.empty
    member table.Count  = t.Count


/// Compile a pre-processed LALR parser spec to tables following the Dragon book algorithm
let CompilerLalrParserSpec logf (spec : ProcessedParserSpec) =
    let stopWatch = new System.Diagnostics.Stopwatch()
    let reportTime() = printfn "        time: %A" stopWatch.Elapsed; stopWatch.Reset(); stopWatch.Start()
    stopWatch.Start()

    // Augment the grammar 
    let fakeStartNonTerminals = spec.StartSymbols |> List.map(fun nt -> "_start"^nt) 
    let nonTerminals = fakeStartNonTerminals@spec.NonTerminals
    let endOfInputTerminal = "$$"
    let dummyLookahead = "#"
    let dummyPrec = NoPrecedence
    let terminals = spec.Terminals @ [(dummyLookahead,dummyPrec); (endOfInputTerminal,dummyPrec)]
    let prods = List.map2 (fun a b -> Production(a, dummyPrec,[NonTerminal b],None)) fakeStartNonTerminals spec.StartSymbols @ spec.Productions
    let startNonTerminalIdx_to_prodIdx (i:int) = i

    // Build indexed tables 
    let ntTab = NonTerminalTable(nonTerminals)
    let termTab = TerminalTable(terminals)
    let prodTab = ProductionTable(ntTab,termTab,nonTerminals,prods)
    let dummyLookaheadIdx = termTab.ToIndex dummyLookahead
    let endOfInputTerminalIdx = termTab.ToIndex endOfInputTerminal

    let errorTerminalIdx = termTab.ToIndex "error"

    // Compute the FIRST function
    printf  "computing first function..."; stdout.Flush();

    let computedFirstTable = 
        let seed = 
            Map.ofList
             [ for term in termTab.Indexes do yield (PTerminal(term),Set.singleton (Some term))
               for nonTerm in ntTab.Indexes do 
                  yield 
                    (PNonTerminal nonTerm, 
                     List.foldBack 
                       (fun prodIdx acc -> match prodTab.Symbol prodIdx 0 with None -> Set.add None acc | Some _ -> acc) 
                       prodTab.Productions.[nonTerm] 
                       Set.empty) ]
                 
        let add changed ss (x,y) = 
            let s = Map.find x ss
            if Set.contains y s then ss 
            else (changed := true; Map.add x (Set.add y s) ss)

        let oneRound (ss:Map<_,_>) = 
            let changed = ref false
            let frontier = 
                let res = ref []
                for nonTermX in ntTab.Indexes do 
                    for prodIdx in prodTab.Productions.[nonTermX] do
                        let rhs = Array.toList (prodTab.Symbols prodIdx)
                        let rec place l =
                            match l with
                            | (yi :: t) -> 
                                res := 
                                   List.choose 
                                     (function None -> None | Some a -> Some (PNonTerminal nonTermX,Some a)) 
                                     (Set.toList ss.[yi]) 
                                   @ !res;
                                if ss.[yi].Contains(None) then place t;
                            | [] -> 
                                res := (PNonTerminal nonTermX,None) :: !res
                        place rhs
                !res
            let ss' = List.fold (add changed) ss frontier
            !changed, ss'

        let rec loop ss = 
            let changed, ss' = oneRound ss
            if changed then loop ss' else ss'
        loop seed 
            
      
    /// Compute the first set of the given sequence of non-terminals. If any of the non-terminals
    /// have an empty token in the first set then we have to iterate through those. 
    let ComputeFirstSetOfTokenList =
        Memoize (fun (str,term) -> 
            let acc = new System.Collections.Generic.List<_>()
            let rec add l = 
                match l with 
                | [] -> acc.Add(term)
                | sym :: moreSyms -> 
                    let firstSetOfSym = computedFirstTable.[sym]
                    firstSetOfSym |> Set.iter (function None -> () | Some v -> acc.Add(v)) 
                    if firstSetOfSym.Contains(None) then add moreSyms 
            add str;
            Set.ofSeq acc)
    
    // (int,int) representation of LR(0) items 
    let prodIdx_to_item0 idx = mkItem0(idx,0) 
    let prec_of_item0 item0 = prodTab.Precedence (prodIdx_of_item0 item0)
    let ntIdx_of_item0 item0 = prodTab.NonTerminal (prodIdx_of_item0 item0)

    let lsyms_of_item0 item0 = 
        let prodIdx = prodIdx_of_item0 item0
        let dotIdx = dotIdx_of_item0 item0
        let syms = prodTab.Symbols prodIdx
        if dotIdx <= 0 then [||] else syms.[..dotIdx-1]

    let rsyms_of_item0 item0 = 
        let prodIdx = prodIdx_of_item0 item0
        let dotIdx = dotIdx_of_item0 item0
        let syms = prodTab.Symbols prodIdx
        syms.[dotIdx..]

    let rsym_of_item0 item0 = 
        let prodIdx = prodIdx_of_item0 item0
        let dotIdx = dotIdx_of_item0 item0
        prodTab.Symbol prodIdx dotIdx

    let advance_of_item0 item0 = 
        let prodIdx = prodIdx_of_item0 item0
        let dotIdx = dotIdx_of_item0 item0
        mkItem0(prodIdx,dotIdx+1)
    let fakeStartNonTerminalsSet = Set.ofList (fakeStartNonTerminals |> List.map ntTab.ToIndex)

    let IsStartItem item0 = fakeStartNonTerminalsSet.Contains(ntIdx_of_item0 item0)
    let IsKernelItem item0 = (IsStartItem item0 || dotIdx_of_item0 item0 <> 0)

    let StringOfSym sym = match sym with PTerminal s -> "'" ^ termTab.OfIndex s ^ "'" | PNonTerminal s -> ntTab.OfIndex s

    let OutputSym os sym = fprintf os "%s" (StringOfSym sym)

    let OutputSyms os syms =
        fprintf os "%s" (String.Join(" ",Array.map StringOfSym syms))

    // Print items and other stuff 
    let OutputItem0 os item0 =
        fprintf os "    %s -> %a . %a" (ntTab.OfIndex (ntIdx_of_item0 item0)) (* outputPrecInfo precInfo *) OutputSyms (lsyms_of_item0 item0) OutputSyms (rsyms_of_item0 item0) 
        
    let OutputItem0Set os s = 
        Set.iter (fun item -> fprintfn os "%a" OutputItem0 item) s

    let OutputFirstSet os m = 
        Set.iter (function None ->  fprintf os "<empty>" | Some x -> fprintfn os "  term %s" x) m

    let OutputFirstMap os m = 
        Map.iter (fun x y -> fprintf os "first '%a' = " OutputSym x; fprintfn os "%a" OutputFirstSet y) m

    let OutputAction os m = 
        match m with 
        | Shift n -> fprintf os "  shift %d" n 
        | Reduce prodIdx ->  fprintf os "  reduce %s --> %a" (ntTab.OfIndex (prodTab.NonTerminal prodIdx)) OutputSyms (prodTab.Symbols prodIdx)
        | Error ->  fprintf os "  error"
        | Accept -> fprintf os "  accept" 
    
    let OutputActions os m = 
        Array.iteri (fun i (prec,action) -> let term = termTab.OfIndex i in fprintfn os "    action '%s' (%a): %a" term outputPrecInfo prec OutputAction action) m

    let OutputActionTable os m = 
        Array.iteri (fun i n -> fprintfn os "state %d:" i; fprintfn os "%a" OutputActions n) m

    let OutputImmediateActions os m = 
        match m with 
        | None -> fprintf os "<none>"
        | Some a -> OutputAction os a
    
    let OutputGotos os m = 
        Array.iteri (fun ntIdx s -> let nonterm = ntTab.OfIndex ntIdx in match s with Some st -> fprintfn os "    goto %s: %d" nonterm st | None -> ()) m
    
    let OutputCombined os m = 
        Array.iteri (fun i (a,b,c,d) -> 
            fprintf os "state %d:" i
            fprintf os "  items:"
            fprintf os "%a" OutputItem0Set a 
            fprintf os "  actions:"
            fprintf os "%a" OutputActions b 
            fprintf os "  immediate action: "
            fprintf os "%a" OutputImmediateActions c 
            fprintf os "  gotos:"
            fprintf os "%a" OutputGotos d) m
    
    let OutputLalrTables os (prods,states, startStates,actionTable,immediateActionTable,gotoTable,endOfInputTerminalIdx,errorTerminalIdx) = 
        let combined = Array.ofList (List.map2 (fun x (y,(z,w)) -> x,y,z,w) (Array.toList states) (List.zip (Array.toList actionTable) (List.zip (Array.toList immediateActionTable) (Array.toList gotoTable))))
        fprintfn os "------------------------";
        fprintfn os "states = ";
        fprintfn os "%a" OutputCombined combined;
        fprintfn os "startStates = %s" (String.Join(";",Array.ofList (List.map string startStates)));
        fprintfn os "------------------------"


    // Closure of LR(0) nonTerminals, items etc 
    let ComputeClosure0NonTerminal = 
        Memoize (fun nt -> 
            let seed = (List.foldBack (prodIdx_to_item0 >> Set.add) prodTab.Productions.[nt] Set.empty)
            LeastFixedPoint 
                (fun item0 -> 
                   match rsym_of_item0 item0 with
                   | None -> []
                   | Some(PNonTerminal ntB) ->  List.map prodIdx_to_item0 prodTab.Productions.[ntB]
                   | Some(PTerminal _) -> [])
                seed)

    // Close a symbol under epsilon moves
    let ComputeClosure0Symbol rsym acc = 
        match rsym with
        | Some (PNonTerminal nt) -> Set.union (ComputeClosure0NonTerminal nt) acc
        | _ -> acc

    // Close a set under epsilon moves
    let ComputeClosure0 iset = 
        Set.fold (fun acc x -> ComputeClosure0Symbol (rsym_of_item0 x) acc) iset iset 

    // Right symbols after closing under epsilon moves
    let RelevantSymbolsOfKernel kernel =
        let kernelClosure0 = ComputeClosure0 kernel
        Set.fold (fun acc x -> Option.fold (fun acc x -> Set.add x acc) acc (rsym_of_item0 x)) Set.empty kernelClosure0 

    // Goto set of a kernel of LR(0) nonTerminals, items etc 
    // Input is kernel, output is kernel
    let ComputeGotosOfKernel iset sym = 
        let isetClosure = ComputeClosure0 iset
        let acc = new System.Collections.Generic.List<_>(10)
        isetClosure |> Set.iter (fun item0 -> 
              match rsym_of_item0 item0 with 
              | Some sym2 when sym = sym2 -> acc.Add(advance_of_item0 item0) 
              | _ -> ()) 
        Set.ofSeq acc
    
    // Build the full set of LR(0) kernels 
    reportTime(); printf "building kernels..."; stdout.Flush();
    let startItems = List.mapi (fun i _ -> prodIdx_to_item0 (startNonTerminalIdx_to_prodIdx i)) fakeStartNonTerminals
    let startKernels = List.map Set.singleton startItems
    let kernels = 

        /// We use a set-of-sets here. F# sets support structural comparison but at the time of writing
        /// did not structural hashing. 
        let acc = ref Set.empty
        ProcessWorkList startKernels (fun addToWorkList kernel -> 
            if not ((!acc).Contains(kernel)) then
                acc := (!acc).Add(kernel);
                for csym in RelevantSymbolsOfKernel kernel do 
                    let gotoKernel = ComputeGotosOfKernel kernel csym 
                    assert (gotoKernel.Count > 0)
                    addToWorkList gotoKernel )
                    
        !acc |> Seq.toList |> List.map (Set.filter IsKernelItem)
    
    reportTime(); printf "building kernel table..."; stdout.Flush();
    // Give an index to each LR(0) kernel, and from now on refer to them only by index 
    let kernelTab = new KernelTable(kernels)
    let startKernelIdxs = List.map kernelTab.Index startKernels
    let startKernelItemIdxs = List.map2 (fun a b -> KernelItemIdx(a,b)) startKernelIdxs startItems

    let outputKernelItemIdx os (kernelIdx,item0)  =
        fprintf os "kernel %d, item %a" kernelIdx OutputItem0 item0

    /// A cached version of the "goto" computation on LR(0) kernels 
    let gotoKernel = 
        Memoize (fun (GotoItemIdx(kernelIdx,sym)) -> 
            let gset = ComputeGotosOfKernel (kernelTab.Kernel kernelIdx) sym
            if gset.IsEmpty then None else Some (kernelTab.Index gset))

    /// Iterate (iset,sym) pairs such that (gotoKernel kernelIdx sym) is not empty
    let IterateGotosOfKernel kernelIdx f =
        for sym in RelevantSymbolsOfKernel (kernelTab.Kernel kernelIdx) do 
            match gotoKernel (GotoItemIdx(kernelIdx,sym)) with 
            | None -> ()
            | Some k -> f sym k
    

    // This is used to compute the closure of an LALR(1) kernel 
    //
    // For each item [A --> X.BY, a] in I
    //   For each production B -> g in G'
    //     For each terminal b in FIRST(Ya)
    //        such that [B --> .g, b] is not in I do
    //            add [B --> .g, b] to I
    
    let ComputeClosure1 iset = 
        let acc = new Closure1Table()
        ProcessWorkList iset (fun addToWorkList (item0,pretokens:Set<TerminalIndex>) ->
            pretokens |> Set.iter (fun pretoken -> 
                if not (acc.Contains(item0,pretoken)) then
                    acc.Add(item0,pretoken) |> ignore
                    let rsyms = rsyms_of_item0 item0 
                    if rsyms.Length > 0 then 
                        match rsyms.[0] with 
                        | (PNonTerminal ntB) -> 
                             let firstSet = ComputeFirstSetOfTokenList (Array.toList rsyms.[1..],pretoken)
                             for prodIdx in prodTab.Productions.[ntB] do
                                 addToWorkList (prodIdx_to_item0 prodIdx,firstSet)
                        | PTerminal _ -> ()))
        acc

    // Compute the "spontaneous" and "propagate" maps for each LR(0) kernelItem 
    //
    // Input: The kernal K of a set of LR(0) items I and a grammar symbol X
    //
    // Output: The lookaheads generated spontaneously by items in I for kernel items 
    // in goto(I,X) and the items I from which lookaheads are propagated to kernel
    // items in goto(I,X)
    //
    // Method
    //   1. Construct LR(0) kernel items (done - above)
    //   2. 
    // TODO: this is very, very slow. 
    //
    // PLAN TO OPTIMIZE THIS;
    //   - Clarify and comment what's going on here
    //   - verify if we really have to do these enormouos closure computations
    //   - assess if it's possible to use the symbol we're looking for to help trim the jset
    
    reportTime(); printf "computing lookahead relations..."; stdout.Flush();

        
    let spontaneous, propagate  =
        let closure1OfItem0WithDummy = 
            Memoize (fun item0 -> ComputeClosure1 [(item0,Set.ofList [dummyLookaheadIdx])])

        let spontaneous = new SpontaneousTable()
        let propagate = new PropagateTable()
        let count = ref 0 

        for kernelIdx in kernelTab.Indexes do
            printf  "."; stdout.Flush();
            //printf  "kernelIdx = %d\n" kernelIdx; stdout.Flush();
            let kernel = kernelTab.Kernel(kernelIdx)
            for item0 in kernel do  
                let item0Idx = KernelItemIdx(kernelIdx,item0)
                let jset = closure1OfItem0WithDummy item0
                //printf  "#jset = %d\n" jset.Count; stdout.Flush();
                for (KeyValue(closureItem0, lookaheadTokens)) in jset.IEnumerable do
                    incr count
                    match rsym_of_item0 closureItem0 with 
                    | None -> ()
                    | Some rsym ->
                         match gotoKernel (GotoItemIdx(kernelIdx,rsym)) with 
                         | None -> ()
                         | Some gotoKernelIdx ->
                              let gotoItem = advance_of_item0 closureItem0
                              let gotoItemIdx = KernelItemIdx(gotoKernelIdx,gotoItem)
                              for lookaheadToken in lookaheadTokens do
                                  if lookaheadToken = dummyLookaheadIdx 
                                  then propagate.Add(item0Idx, gotoItemIdx) |> ignore
                                  else spontaneous.Add(gotoItemIdx, lookaheadToken) |> ignore


        //printfn "#kernelIdxs = %d, count = %d" kernelTab.Indexes.Length !count
        spontaneous,
        propagate
   
    //printfn "#spontaneous = %d, #propagate = %d" spontaneous.Count propagate.Count; stdout.Flush();
   
    //exit 0;
    // Repeatedly use the "spontaneous" and "propagate" maps to build the full set 
    // of lookaheads for each LR(0) kernelItem.   
    reportTime(); printf  "building lookahead table..."; stdout.Flush();
    let lookaheadTable = 

        // Seed the table with the startKernelItems and the spontaneous info
        let initialWork =
            [ for idx in startKernelItemIdxs do
                  yield (idx,endOfInputTerminalIdx)
              for (KeyValue(kernelItemIdx,lookaheads)) in spontaneous.IEnumerable do
                  for lookahead in lookaheads do
                      yield (kernelItemIdx,lookahead) ]

        let acc = new LookaheadTable()
        // Compute the closure
        ProcessWorkList 
            initialWork
            (fun queueWork (kernelItemIdx,lookahead) ->
                acc.Add(kernelItemIdx,lookahead)
                for gotoKernelIdx in propagate.[kernelItemIdx] do
                    if not (acc.Contains(gotoKernelIdx,lookahead)) then 
                        queueWork(gotoKernelIdx,lookahead))
        acc

    //printf  "built lookahead table, #lookaheads = %d\n" lookaheadTable.Count; stdout.Flush();

    reportTime(); printf "building action table..."; stdout.Flush();
    let shiftReduceConflicts = ref 0
    let reduceReduceConflicts = ref 0
    let actionTable, immediateActionTable = 

        // Now build the action tables. First a utility to merge the given action  
        // into the table, taking into account precedences etc. and reporting errors. 
        let addResolvingPrecedence (arr: _[]) kernelIdx termIdx (precNew, actionNew) = 
            // printf "DEBUG: state %d: adding action for %s, precNew = %a, actionNew = %a\n" kernelIdx (termTab.OfIndex termIdx) outputPrec precNew OutputAction actionNew; 
            // We add in order of precedence - however the precedences may be the same, and we give warnings when rpecedence resolution is based on implicit file orderings 

            let (precSoFar, actionSoFar) as itemSoFar = arr.[termIdx]

            // printf "DEBUG: state %d: adding action for %s, precNew = %a, precSoFar = %a, actionSoFar = %a\n" kernelIdx (termTab.OfIndex termIdx) outputPrec precNew outputPrec precSoFar OutputAction actionSoFar; 
            // if compare_prec precSoFar precNew = -1 then failwith "addResolvingPrecedence"; 

            let itemNew = (precNew, actionNew) 
            let winner = 
                let reportConflict x1 x2 reason =
                    let reportAction (p, a) =
                        let an, astr = 
                            match a with
                            | Shift x -> "shift", sprintf "shift(%d)" x
                            | Reduce x ->
                                let nt = prodTab.NonTerminal x
                                "reduce", prodTab.Symbols x
                                |> Array.map StringOfSym
                                |> String.concat " "
                                |> sprintf "reduce(%s:%s)" (ntTab.OfIndex nt)
                            | _ -> "", ""
                        let pstr = 
                            match p with 
                            | ExplicitPrec (assoc,n) -> 
                                let astr = 
                                    match assoc with 
                                    | LeftAssoc -> "left"
                                    | RightAssoc -> "right"
                                    | NonAssoc -> "nonassoc"
                                sprintf "[explicit %s %d]" astr n
                            | NoPrecedence  -> 
                                "noprec"
                        an, "{" + pstr + " " + astr + "}"
                    let a1n, astr1 = reportAction x1
                    let a2n, astr2 = reportAction x2
                    printfn "        %s/%s error at state %d on terminal %s between %s and %s - assuming the former because %s" a1n a2n kernelIdx (termTab.OfIndex termIdx) astr1 astr2 reason
                match itemSoFar,itemNew with 
                | (_,Shift s1),(_, Shift s2) -> 
                   if actionSoFar <> actionNew then 
                      reportConflict itemSoFar itemNew "internal error"
                   itemSoFar

                | (((precShift,Shift sIdx) as shiftItem), 
                   ((precReduce,Reduce prodIdx) as reduceItem))
                | (((precReduce,Reduce prodIdx) as reduceItem), 
                   ((precShift,Shift sIdx) as shiftItem)) -> 
                    match precReduce, precShift with 
                    | (ExplicitPrec (_,p1), ExplicitPrec(assocNew,p2)) -> 
                      if p1 < p2 then shiftItem
                      elif p1 > p2 then reduceItem
                      else
                        match assocNew with 
                        | LeftAssoc ->  reduceItem
                        | RightAssoc -> shiftItem
                        | NonAssoc ->
                           reportConflict shiftItem reduceItem "we prefer shift on equal precedences"
                           incr shiftReduceConflicts;
                           shiftItem
                    | _ ->
                       reportConflict shiftItem reduceItem "we prefer shift when unable to compare precedences"
                       incr shiftReduceConflicts;
                       shiftItem
                | ((_,Reduce prodIdx1),(_, Reduce prodIdx2)) -> 
                   "we prefer the rule earlier in the file"
                   |> if prodIdx1 < prodIdx2 then reportConflict itemSoFar itemNew else reportConflict itemNew itemSoFar
                   incr reduceReduceConflicts;
                   if prodIdx1 < prodIdx2 then itemSoFar else itemNew
                | _ -> itemNew 
            arr.[termIdx] <- winner

          
        // This build the action table for one state. 
        let ComputeActions kernelIdx = 
            let kernel = kernelTab.Kernel kernelIdx
            let arr = Array.create terminals.Length (NoPrecedence,Error)

            //printf  "building lookahead table LR(1) items for kernelIdx %d\n" kernelIdx; stdout.Flush();

            // Compute the LR(1) items based on lookaheads
            let items = 
                 [ for item0 in kernel do
                     let kernelItemIdx = KernelItemIdx(kernelIdx,item0)
                     let lookaheads = lookaheadTable.GetLookaheads(kernelItemIdx)
                     yield (item0,lookaheads) ]
                 |> ComputeClosure1

            for (KeyValue(item0,lookaheads)) in items.IEnumerable do

                let nonTermA = ntIdx_of_item0 item0
                match rsym_of_item0 item0 with 
                | Some (PTerminal termIdx) -> 
                    let action =
                      match gotoKernel (GotoItemIdx(kernelIdx,PTerminal termIdx)) with 
                      | None -> failwith "action on terminal should have found a non-empty goto state"
                      | Some gkernelItemIdx -> Shift gkernelItemIdx
                    let prec = termTab.PrecInfoOfIndex termIdx
                    addResolvingPrecedence arr kernelIdx termIdx (prec, action) 
                | None ->
                    for lookahead in lookaheads do
                        if not (IsStartItem(item0)) then
                            let prodIdx = prodIdx_of_item0 item0
                            let prec = prec_of_item0 item0
                            let action = (prec, Reduce prodIdx)
                            addResolvingPrecedence arr kernelIdx lookahead action 
                        elif lookahead = endOfInputTerminalIdx then
                            let prec = prec_of_item0 item0
                            let action = (prec,Accept)
                            addResolvingPrecedence arr kernelIdx lookahead action 
                        else ()
                | _ -> ()

            // If there is a single item A -> B C . and no Shift or Accept actions (i.e. only Error or Reduce, so the choice of terminal 
            // cannot affect what we do) then we emit an immediate reduce action for the rule corresponding to that item 
            // Also do the same for Accept rules. 
            let closure = (ComputeClosure0 kernel)

            let immediateAction =
                match Set.toList closure with
                | [item0] ->
                    match (rsym_of_item0 item0) with 
                    | None when (let reduceOrErrorAction = function Error | Reduce _ -> true | Shift _ | Accept -> false
                                 termTab.Indexes |> List.forall(fun terminalIdx -> reduceOrErrorAction (snd(arr.[terminalIdx]))))
                        -> Some (Reduce (prodIdx_of_item0 item0))

                    | None when (let acceptOrErrorAction = function Error | Accept -> true | Shift _ | Reduce _ -> false
                                 List.forall (fun terminalIdx -> acceptOrErrorAction (snd(arr.[terminalIdx]))) termTab.Indexes)
                        -> Some Accept

                    | _ -> None
                | _ -> None

            // A -> B C . rules give rise to reductions in favour of errors 
            for item0 in ComputeClosure0 kernel do
                let prec = prec_of_item0 item0
                match rsym_of_item0 item0 with 
                | None ->
                    for terminalIdx in termTab.Indexes do 
                        if snd(arr.[terminalIdx]) = Error then 
                            let prodIdx = prodIdx_of_item0 item0
                            let action = (prec, (if IsStartItem(item0) then Accept else Reduce prodIdx))
                            addResolvingPrecedence arr kernelIdx terminalIdx action
                | _  -> ()

            arr,immediateAction

        let actionInfo = List.map ComputeActions kernelTab.Indexes
        Array.ofList (List.map fst actionInfo),
        Array.ofList (List.map snd actionInfo)

    // The goto table is much simpler - it is based on LR(0) kernels alone. 

    reportTime(); printf  "        building goto table..."; stdout.Flush();
    let gotoTable = 
         let gotos kernelIdx = Array.ofList (List.map (fun nt -> gotoKernel (GotoItemIdx(kernelIdx,PNonTerminal nt))) ntTab.Indexes)
         Array.ofList (List.map gotos kernelTab.Indexes)

    reportTime(); printfn  "        returning tables."; stdout.Flush();
    if !shiftReduceConflicts > 0 then printfn  "        %d shift/reduce conflicts" !shiftReduceConflicts; stdout.Flush();
    if !reduceReduceConflicts > 0 then printfn  "        %d reduce/reduce conflicts" !reduceReduceConflicts; stdout.Flush();
    if !shiftReduceConflicts > 0 || !reduceReduceConflicts > 0 then printfn  "        consider setting precedences explicitly using %%left %%right and %%nonassoc on terminals and/or setting explicit precedence on rules using %%prec"

    /// The final results
    let states = kernels |> Array.ofList 
    let prods = Array.ofList (List.map (fun (Production(nt,prec,syms,code)) -> (nt, ntTab.ToIndex nt, syms,code)) prods)

    logf (fun logStream -> 
        printfn  "writing tables to log"; stdout.Flush();
        OutputLalrTables logStream     (prods, states, startKernelIdxs, actionTable, immediateActionTable, gotoTable, (termTab.ToIndex endOfInputTerminal), errorTerminalIdx));

    let states = states |> Array.map (Set.toList >> List.map prodIdx_of_item0)
    (prods, states, startKernelIdxs, 
     actionTable, immediateActionTable, gotoTable, 
     (termTab.ToIndex endOfInputTerminal), 
     errorTerminalIdx, nonTerminals)

  
(* Some examples for testing *)  

(*

let example1 = 
  let e = "E" 
  let t = "Terminal"
  let plus = "+"
  let mul = "*"
  let f = "F"
  let lparen = "("
  let rparen = ")"
  let id = "id"
  
  let terminals = [plus; mul; lparen; rparen; id]
  let nonTerminals = [e; t; f]
  
  let p2 = e, (NonAssoc, ExplicitPrec 1), [NonTerminal e; Terminal plus; NonTerminal t], None
  let p3 = e, (NonAssoc, ExplicitPrec 2), [NonTerminal t], None in  
  let p4 = t, (NonAssoc, ExplicitPrec 3), [NonTerminal t; Terminal mul; NonTerminal f], None
  let p5 = t, (NonAssoc, ExplicitPrec 4), [NonTerminal f], None
  let p6 = f, (NonAssoc, ExplicitPrec  5), [Terminal lparen; NonTerminal e; Terminal rparen], None
  let p7 = f, (NonAssoc, ExplicitPrec 6), [Terminal id], None

  let prods = [p2;p3;p4;p5;p6;p7]
  Spec(terminals,nonTerminals,prods, [e])

let example2 = 
  let prods = [ "S", (NonAssoc, ExplicitPrec 1), [NonTerminal "C";NonTerminal "C"], None; 
                "C", (NonAssoc, ExplicitPrec 2), [Terminal "c";NonTerminal "C"], None ;
                "C", (NonAssoc, ExplicitPrec 3), [Terminal "d"] , None  ]in
  Spec(["c";"d"],["S";"C"],prods, ["S"])

let example3 = 
  let terminals = ["+"; "*"; "("; ")"; "id"]
  let nonTerminals = ["E"; "Terminal"; "E'"; "F"; "Terminal'"]
  let prods = [ "E", (NonAssoc, ExplicitPrec 1), [ NonTerminal "Terminal"; NonTerminal "E'" ], None;
                "E'", (NonAssoc, ExplicitPrec 2), [ Terminal "+"; NonTerminal "Terminal"; NonTerminal "E'"], None;
                "E'", (NonAssoc, ExplicitPrec 3), [ ], None;
                "Terminal", (NonAssoc, ExplicitPrec 4), [ NonTerminal "F"; NonTerminal "Terminal'" ], None;
                "Terminal'", (NonAssoc, ExplicitPrec 5), [ Terminal "*"; NonTerminal "F"; NonTerminal "Terminal'"], None;
                "Terminal'", (NonAssoc, ExplicitPrec 6), [ ], None;
                "F", (NonAssoc, ExplicitPrec 7), [ Terminal "("; NonTerminal "E"; Terminal ")"], None;
                "F", (NonAssoc, ExplicitPrec 8), [ Terminal "id"], None ]
  Spec(terminals,nonTerminals,prods, ["E"])

let example4 = 
  let terminals = ["+"; "*"; "("; ")"; "id"]
  let nonTerminals = ["E"]
  let prods = [ "E", (NonAssoc, ExplicitPrec 1), [ NonTerminal "E"; Terminal "+"; NonTerminal "E" ], None;
                "E", (NonAssoc, ExplicitPrec 2), [ NonTerminal "E"; Terminal "*"; NonTerminal "E" ], None;
                "E", (NonAssoc, ExplicitPrec 3), [ Terminal "("; NonTerminal "E"; Terminal ")"], None;
                "E", (NonAssoc, ExplicitPrec 8), [ Terminal "id"],  None ]
  Spec(terminals,nonTerminals,prods, ["E"])

let example5 = 
  let terminals = ["+"; "*"; "("; ")"; "id"]
  let nonTerminals = ["E"]
  let prods = [ "E", (NonAssoc, ExplicitPrec 1), [ NonTerminal "E"; Terminal "+"; NonTerminal "E" ], None;
                "E", (NonAssoc, ExplicitPrec 2), [ NonTerminal "E"; Terminal "*"; NonTerminal "E" ], None;
                "E", (NonAssoc, ExplicitPrec 3), [ Terminal "("; NonTerminal "E"; Terminal ")"], None;
                "E", (NonAssoc, ExplicitPrec 8), [ Terminal "id"], None ]
  Spec(terminals,nonTerminals,prods, ["E"])

let example6 = 
  let terminals = ["+"; "*"; "("; ")"; "id"; "-"]
  let nonTerminals = ["E"]
  let prods = [ "E", (RightAssoc, ExplicitPrec 1), [ NonTerminal "E"; Terminal "-"; NonTerminal "E" ], None;
                "E", (LeftAssoc, ExplicitPrec 1), [ NonTerminal "E"; Terminal "+"; NonTerminal "E" ], None;
                "E", (LeftAssoc, ExplicitPrec 2), [ NonTerminal "E"; Terminal "*"; NonTerminal "E" ], None;
                "E", (NonAssoc, ExplicitPrec 3), [ Terminal "("; NonTerminal "E"; Terminal ")"], None;
                "E", (NonAssoc, ExplicitPrec 8), [ Terminal "id"], None ]
  Spec(terminals,nonTerminals,prods, ["E"])


let example7 = 
  let prods = [ "S", (NonAssoc, ExplicitPrec 1), [NonTerminal "L";Terminal "="; NonTerminal "R"], None; 
                "S", (NonAssoc, ExplicitPrec 2), [NonTerminal "R"], None ;
                "L", (NonAssoc, ExplicitPrec 3), [Terminal "*"; NonTerminal "R"], None;
                "L", (NonAssoc, ExplicitPrec 3), [Terminal "id"], None; 
                "R", (NonAssoc, ExplicitPrec 3), [NonTerminal "L"], None; ]
  Spec(["*";"=";"id"],["S";"L";"R"],prods, ["S"])



let test ex = CompilerLalrParserSpec stdout ex

(* let _ = test example2*)
(* let _ = exit 1*)
(* let _ = test example3 
let _ = test example1  
let _ = test example4
let _ = test example5
let _ = test example6 *)
*)
