(* (c) Microsoft Corporation 2005-2008.  *)

module internal FsLexYacc.FsLex.AST

open System.Collections.Generic
open FSharp.Text
open Microsoft.FSharp.Collections
open Internal.Utilities
open Internal.Utilities.Text.Lexing

let (|KeyValue|) (kvp:KeyValuePair<_,_>) = kvp.Key,kvp.Value

type Ident = string
type Code = string * Position

type Alphabet = uint32

let Eof : Alphabet = 0xFFFFFFFEu
let Epsilon : Alphabet = 0xFFFFFFFFu


let unicode = ref false

let unicodeCategories = 
 dict 
  [| "Pe", System.Globalization.UnicodeCategory.ClosePunctuation; // (Pe)
    "Pc", System.Globalization.UnicodeCategory.ConnectorPunctuation; // (Pc)
    "Cc", System.Globalization.UnicodeCategory.Control; // (Cc)
    "Sc", System.Globalization.UnicodeCategory.CurrencySymbol; // (Sc)
    "Pd", System.Globalization.UnicodeCategory.DashPunctuation; // (Pd)
    "Nd", System.Globalization.UnicodeCategory.DecimalDigitNumber; // (Nd)
    "Me", System.Globalization.UnicodeCategory.EnclosingMark; // (Me)
    "Pf", System.Globalization.UnicodeCategory.FinalQuotePunctuation; // (Pf)
    "Cf", enum 15; //System.Globalization.UnicodeCategory.Format; // (Cf)
    "Pi", System.Globalization.UnicodeCategory.InitialQuotePunctuation; // (Pi)
    "Nl", System.Globalization.UnicodeCategory.LetterNumber; // (Nl)
    "Zl", System.Globalization.UnicodeCategory.LineSeparator; // (Zl)
    "Ll", System.Globalization.UnicodeCategory.LowercaseLetter; // (Ll)
    "Sm", System.Globalization.UnicodeCategory.MathSymbol; // (Sm)
    "Lm", System.Globalization.UnicodeCategory.ModifierLetter; // (Lm)
    "Sk", System.Globalization.UnicodeCategory.ModifierSymbol; // (Sk)
    "Mn", System.Globalization.UnicodeCategory.NonSpacingMark; // (Mn)
    "Ps", System.Globalization.UnicodeCategory.OpenPunctuation; // (Ps)
    "Lo", System.Globalization.UnicodeCategory.OtherLetter; // (Lo)
    "Cn", System.Globalization.UnicodeCategory.OtherNotAssigned; // (Cn)
    "No", System.Globalization.UnicodeCategory.OtherNumber; // (No)
    "Po", System.Globalization.UnicodeCategory.OtherPunctuation; // (Po)
    "So", System.Globalization.UnicodeCategory.OtherSymbol; // (So)
    "Zp", System.Globalization.UnicodeCategory.ParagraphSeparator; // (Zp)
    "Co", System.Globalization.UnicodeCategory.PrivateUse; // (Co)
    "Zs", System.Globalization.UnicodeCategory.SpaceSeparator; // (Zs)
    "Mc", System.Globalization.UnicodeCategory.SpacingCombiningMark; // (Mc)
    "Cs", System.Globalization.UnicodeCategory.Surrogate; // (Cs)
    "Lt", System.Globalization.UnicodeCategory.TitlecaseLetter; // (Lt)
    "Lu", System.Globalization.UnicodeCategory.UppercaseLetter; // (Lu)
  |]

let NumUnicodeCategories = unicodeCategories.Count
let _ = assert (NumUnicodeCategories = 30) // see table interpreter
let encodedUnicodeCategoryBase = 0xFFFFFF00u
let EncodeUnicodeCategoryIndex(idx:int) = encodedUnicodeCategoryBase + uint32 idx
let EncodeUnicodeCategory(s:string) = 
    if not (!unicode) then 
         failwith "unicode category classes may only be used if --unicode is specified";
    if unicodeCategories.ContainsKey(s) then 
        EncodeUnicodeCategoryIndex (int32 unicodeCategories.[s])
    else
        failwithf "invalid Unicode category: '%s'" s

let IsUnicodeCategory(x:Alphabet) = (encodedUnicodeCategoryBase <= x) && (x < encodedUnicodeCategoryBase + uint32 NumUnicodeCategories)
let UnicodeCategoryIndex(x:Alphabet) = (x - encodedUnicodeCategoryBase)

let numLowUnicodeChars = 128
let _ = assert (numLowUnicodeChars = 128) // see table interpreter
let specificUnicodeChars = new Dictionary<_,_>()
let specificUnicodeCharsDecode = new Dictionary<_,_>()
let EncodeChar(c:char) = 
     let x = System.Convert.ToUInt32 c
     if !unicode then 
         if x < uint32 numLowUnicodeChars then x 
         else 
             if not(specificUnicodeChars.ContainsKey(c)) then
                 let idx = uint32 numLowUnicodeChars + uint32 specificUnicodeChars.Count  
                 specificUnicodeChars.[c] <- idx
                 specificUnicodeCharsDecode.[idx] <- c
             specificUnicodeChars.[c]
     else         
         if x >= 256u then failwithf "the Unicode character '%c' may not be used unless --unicode is specified" c;
         x
let DecodeChar(x:Alphabet) = 
     if !unicode then 
         if x < uint32 numLowUnicodeChars then System.Convert.ToChar x
         else specificUnicodeCharsDecode.[x]
     else
         if x >= 256u then failwithf "the Unicode character '%x' may not be used unless --unicode is specified" x;
         System.Convert.ToChar x
         
         

let NumSpecificUnicodeChars() = specificUnicodeChars.Count
let GetSpecificUnicodeChars() = 
    specificUnicodeChars 
        |> Seq.sortBy (fun (KeyValue(k,v)) -> v) 
        |> Seq.map (fun (KeyValue(k,v)) -> k) 

let GetSingleCharAlphabet() = 
    if !unicode 
    then Set.ofList [ for c in 0..numLowUnicodeChars-1 do yield (char c)
                      for c in GetSpecificUnicodeChars() do yield c ]
    else Set.ofList [ for x in 0..255 ->  (char x) ]
         
let GetAlphabet() = 
    if !unicode 
    then Set.ofList [ for c in GetSingleCharAlphabet() do yield EncodeChar c
                      for uc in 0 .. NumUnicodeCategories-1 do yield EncodeUnicodeCategoryIndex uc ]
    else Set.ofList [ for c in GetSingleCharAlphabet() do yield EncodeChar c ]

         
//let DecodeAlphabet (x:Alphabet) = System.Convert.ToChar(x)

(*
for i in 0 .. 65535 do 
    let c = char i
    if System.Char.GetUnicodeCategory c = System.Globalization.UnicodeCategory.PrivateUse then 
        printfn "i = %x" i
*)

type Spec = 
    { TopCode: Code;
      Macros: (Ident * Regexp) list;
      Rules: (Ident * Ident list * Clause list) list;
      BottomCode: Code }
and Clause = Regexp * Code
and Regexp = 
  | Alt of Regexp list
  | Seq of Regexp list
  | Inp of Input
  | Star of Regexp
  | Macro of Ident
and Input =
  | Alphabet of Alphabet
  | UnicodeCategory of string 
  | Any 
  | NotCharSet of Set<Alphabet>

type NodeId = int   

type NfaNode = 
    { Id: NodeId;
      Name: string;
      Transitions: Dictionary<Alphabet, NfaNode list>;
      Accepted: (int * int) list }

type DfaNode = 
    { Id: int;
      Name: string;
      mutable Transitions: (Alphabet * DfaNode) list;
      Accepted: (int * int) list }

type MultiMap<'a,'b> = Dictionary<'a,'b list>
let LookupMultiMap (trDict:MultiMap<_,_>) a  =
    if trDict.ContainsKey(a) then trDict.[a] else []

let AddToMultiMap (trDict:MultiMap<_,_>) a b =
    let prev = LookupMultiMap trDict a
    trDict.[a] <- b :: prev

type NfaNodeMap() = 
    let map = new Dictionary<int,NfaNode>(100)
    member x.Item with get(nid) = map.[nid]
    member x.Count = map.Count

    member x.NewNfaNode(trs,ac) = 
        let nodeId = map.Count+1 // ID zero is reserved
        let trDict = new Dictionary<_,_>(List.length trs)
        for (a,b) in trs do
           AddToMultiMap trDict a b
           
        let node : NfaNode = {Id=nodeId; Name=string nodeId; Transitions=trDict; Accepted=ac}
        map.[nodeId] <-node;
        node

let LexerStateToNfa (macros: Map<string,_>) (clauses: Clause list) = 

    /// Table allocating node ids 
    let nfaNodeMap = new NfaNodeMap()
    
    /// Compile a regular expression into the NFA
    let rec CompileRegexp re dest = 
        match re with 
        | Alt res -> 
            let trs = res |> List.map (fun re -> (Epsilon,CompileRegexp re dest)) 
            nfaNodeMap.NewNfaNode(trs,[])
        | Seq res -> 
            List.foldBack (CompileRegexp) res dest 
        | Inp (Alphabet c) -> 
            nfaNodeMap.NewNfaNode([(c, dest)],[])
            
        | Star re -> 
            let nfaNode = nfaNodeMap.NewNfaNode([(Epsilon, dest)],[])
            let sre = CompileRegexp re nfaNode
            AddToMultiMap nfaNode.Transitions Epsilon sre
            nfaNodeMap.NewNfaNode([(Epsilon,sre); (Epsilon,dest)],[])
        | Macro m -> 
            if not (macros.ContainsKey(m)) then failwith ("The macro "+m+" is not defined");
            CompileRegexp (macros.[m]) dest 

        // These cases unwind the difficult cases in the syntax that rely on knowing the
        // entire alphabet.
        //
        // Note we've delayed the expension of these until we've worked out all the 'special' Unicode characters
        // mentioned in the entire lexer spec, i.e. we wait until GetAlphabet returns a reliable and stable answer.
        | Inp (UnicodeCategory uc) -> 
            let re = Alt([ yield Inp(Alphabet(EncodeUnicodeCategory uc))
                           // Also include any specific characters in this category
                           for c in GetSingleCharAlphabet() do 
                               if System.Char.GetUnicodeCategory(c) = unicodeCategories.[uc] then 
                                    yield Inp(Alphabet(EncodeChar(c))) ])
            CompileRegexp re dest

        | Inp Any -> 
            let re = Alt([ for n in GetAlphabet() do yield Inp(Alphabet(n)) ])
            CompileRegexp re dest

        | Inp (NotCharSet chars) -> 
            let re = Alt [ // Include any characters from those in the alphabet besides those that are not immediately excluded
                           for c in GetSingleCharAlphabet() do 
                               let ec = EncodeChar c
                               if not (chars.Contains(ec)) then 
                                   yield Inp(Alphabet(ec))

                           // Include all unicode categories 
                           // That is, negations _only_ exclude precisely the given set of characters. You can't
                           // exclude whole classes of characters as yet
                           if !unicode then 
                               let ucs = chars |> Set.map(DecodeChar >> System.Char.GetUnicodeCategory)  
                               for KeyValue(nm,uc) in unicodeCategories do
                                   //if ucs.Contains(uc) then 
                                   //    do printfn "warning: the unicode category '\\%s' ('%s') is automatically excluded by this character set negation. Consider adding this to the negation." nm  (uc.ToString())
                                   //    yield! []
                                   //else
                                       yield Inp(Alphabet(EncodeUnicodeCategory nm)) 
                         ]
            CompileRegexp re dest

    let actions = new System.Collections.Generic.List<_>()
    
    /// Compile an acceptance of a regular expression into the NFA
    let sTrans macros nodeId (regexp,code) = 
        let actionId = actions.Count
        actions.Add(code)
        let sAccept = nfaNodeMap.NewNfaNode([],[(nodeId,actionId)])
        CompileRegexp regexp sAccept 

    let trs = clauses |> List.mapi (fun n x -> (Epsilon,sTrans macros n x)) 
    let nfaStartNode = nfaNodeMap.NewNfaNode(trs,[])
    nfaStartNode,(actions |> Seq.readonly), nfaNodeMap

// TODO: consider a better representation here.
type internal NfaNodeIdSetBuilder = HashSet<NodeId>

type internal NfaNodeIdSet(nodes: NfaNodeIdSetBuilder) = 
    // BEWARE: the next line is performance critical
    let s = nodes |> Seq.toArray |> (fun arr -> Array.sortInPlaceWith compare arr; arr) // 19

    // These are all surprisingly slower:
    //let s = nodes |> Seq.toArray |> Array.sort 
    //let s = nodes |> Seq.toArray |> Array.sortWith compare // 76
    //let s = nodes |> Seq.toArray |> (fun arr -> Array.sortInPlace arr; arr) // 76

    member x.Representation = s
    member x.Elements = s 
    member x.Fold f z = Array.fold f z s
    interface System.IComparable with 
        member x.CompareTo(y:obj) = 
            let y = (y :?> NfaNodeIdSet)
            let xr = x.Representation
            let yr = y.Representation
            let c = compare xr.Length yr.Length
            if c <> 0 then c else 
            let n = yr.Length
            let rec go i = 
                if i >= n then 0 else
                let c = compare xr.[i] yr.[i]
                if c <> 0 then c else
                go (i+1) 
            go 0

    override x.Equals(y:obj) = 
        match y with 
        | :? NfaNodeIdSet as y -> 
            let xr = x.Representation
            let yr = y.Representation
            let n = yr.Length
            xr.Length = n && 
            (let rec go i = (i < n) && xr.[i] = yr.[i] && go (i+1) 
             go 0)
        | _ -> false

    override x.GetHashCode() = hash s

    member x.IsEmpty = (s.Length = 0)
    member x.Iterate f = s |> Array.iter f

type NodeSetSet = Set<NfaNodeIdSet>

let newDfaNodeId = 
    let i = ref 0 
    fun () -> let res = !i in incr i; res
   
let NfaToDfa (nfaNodeMap:NfaNodeMap) nfaStartNode = 
    let numNfaNodes = nfaNodeMap.Count
    let rec EClosure1 (acc:NfaNodeIdSetBuilder) (n:NfaNode) = 
        if not (acc.Contains n.Id) then 
            acc.Add n.Id |> ignore;
            if n.Transitions.ContainsKey(Epsilon) then
                match n.Transitions.[Epsilon] with 
                | [] -> () // this Clause is an optimization - the list is normally empty
                | tr -> 
                    //printfn "n.Id = %A, #Epsilon = %d" n.Id tr.Length
                    tr |> List.iter (EClosure1 acc) 

    let EClosure (moves:list<NodeId>) = 
        let acc = new NfaNodeIdSetBuilder(HashIdentity.Structural)
        for i in moves do
            EClosure1 acc nfaNodeMap.[i];
        new NfaNodeIdSet(acc)

    // Compute all the immediate one-step moves for a set of NFA states, as a dictionary
    // mapping inputs to destination lists
    let ComputeMoves (nset:NfaNodeIdSet) = 
        let moves = new MultiMap<_,_>()
        nset.Iterate(fun nodeId -> 
            for (KeyValue(inp,dests)) in nfaNodeMap.[nodeId].Transitions do
                if inp <> Epsilon then 
                    match dests with 
                    | [] -> ()  // this Clause is an optimization - the list is normally empty
                    | tr -> tr |> List.iter(fun dest -> AddToMultiMap moves inp dest.Id))
        moves

    let acc = new NfaNodeIdSetBuilder(HashIdentity.Structural)
    EClosure1 acc nfaStartNode;
    let nfaSet0 = new NfaNodeIdSet(acc)

    let dfaNodes = ref (Map.empty<NfaNodeIdSet,DfaNode>)

    let GetDfaNode nfaSet = 
        if (!dfaNodes).ContainsKey(nfaSet) then 
            (!dfaNodes).[nfaSet]
        else 
            let dfaNode =
                { Id= newDfaNodeId(); 
                  Name = nfaSet.Fold (fun s nid -> nfaNodeMap.[nid].Name+"-"+s) ""; 
                  Transitions=[];
                  Accepted= nfaSet.Elements 
                            |> Seq.map (fun nid -> nfaNodeMap.[nid].Accepted)
                            |> List.concat }
            //Printf.printfn "id = %d" dfaNode.Id;

            dfaNodes := (!dfaNodes).Add(nfaSet,dfaNode); 
            dfaNode
            
    let workList = ref [nfaSet0]
    let doneSet = ref Set.empty

    //let count = ref 0 
    let rec Loop () = 
        match !workList with 
        | [] -> ()
        | nfaSet :: t -> 
            workList := t;
            if (!doneSet).Contains(nfaSet) then 
                Loop () 
            else
                let moves = ComputeMoves nfaSet
                for (KeyValue(inp,movesForInput)) in moves do
                    assert (inp <> Epsilon);
                    let moveSet = EClosure movesForInput;
                    if not moveSet.IsEmpty then 
                        //incr count
                        let dfaNode = GetDfaNode nfaSet
                        dfaNode.Transitions <- (inp, GetDfaNode moveSet) :: dfaNode.Transitions;
                        (* Printf.printf "%d (%s) : %s --> %d (%s)\n" dfaNode.Id dfaNode.Name (match inp with EncodeChar c -> String.make 1 c | LEof -> "eof") moveSetDfaNode.Id moveSetDfaNode.Name;*)
                        workList := moveSet :: !workList;

                doneSet := (!doneSet).Add(nfaSet);


                Loop()
    Loop();
    //Printf.printfn "count = %d" !count;
    let ruleStartNode = GetDfaNode nfaSet0
    let ruleNodes = 
        (!dfaNodes) 
        |> Seq.map (fun kvp -> kvp.Value) 
        |> Seq.toList
        |> List.sortBy (fun s -> s.Id)
    ruleStartNode,ruleNodes

let Compile spec = 
    List.foldBack
        (fun (name,args,clauses) (perRuleData,dfaNodes) -> 
            let nfa, actions, nfaNodeMap = LexerStateToNfa (Map.ofList spec.Macros) clauses
            let ruleStartNode, ruleNodes = NfaToDfa nfaNodeMap nfa
            //Printf.printfn "name = %s, ruleStartNode = %O" name ruleStartNode.Id;
            (ruleStartNode,actions) :: perRuleData, ruleNodes @ dfaNodes)
        spec.Rules
        ([],[])

