// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// This file is compiled 3(!) times in the codebase
//    - as the internal implementation of printf '%A' formatting 
//           defines: RUNTIME
//    - as the internal implementation of structured formatting in the FSharp.Compiler-proto.dll 
//           defines: COMPILER + BUILDING_WITH_LKG
//    - as the internal implementation of structured formatting in FSharp.Compiler.dll 
//           defines: COMPILER 
//           NOTE: this implementation is used by fsi.exe. This is very important.
//
// The one implementation file is used because we very much want to keep the implementations of
// structured formatting the same for fsi.exe and '%A' printing. However fsi.exe may have
// a richer feature set.
//
// Note no layout objects are ever transferred between the above implementations, and in 
// all 4 cases the layout types are really different types.

#nowarn "52" // The value has been copied to ensure the original is not mutated by this operation

#if COMPILER
// FSharp.Compiler-proto.dll:
// FSharp.Compiler.dll:
namespace Internal.Utilities.StructuredFormat
#else
#if RUNTIME 
// FSharp.Core.dll:
namespace Microsoft.FSharp.Text.StructuredPrintfImpl
#else
// Powerpack: 
namespace Microsoft.FSharp.Text.StructuredFormat
#endif
#endif

    // Breakable block layout implementation.
    // This is a fresh implementation of pre-existing ideas.

    open System
    open System.Diagnostics
    open System.Text
    open System.IO
    open System.Reflection
    open System.Globalization
    open System.Collections.Generic
    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
    open Microsoft.FSharp.Core.Operators
    open Microsoft.FSharp.Reflection
    open Microsoft.FSharp.Collections
    open Microsoft.FSharp.Primitives.Basics

#if FX_RESHAPED_REFLECTION
    open PrimReflectionAdapters
    open ReflectionAdapters
#endif

    /// A joint, between 2 layouts, is either:
    ///  - unbreakable, or
    ///  - breakable, and if broken the second block has a given indentation.
    [<StructuralEquality; NoComparison>]
#if COMPILER
    type internal Joint =
#else
    type Joint =
#endif
     | Unbreakable
     | Breakable of int
     | Broken of int

    /// Leaf juxt,data,juxt
    /// Node juxt,left,juxt,right,juxt and joint
    ///
    /// If either juxt flag is true, then no space between words.
    [<NoEquality; NoComparison>]
#if COMPILER
    type internal Layout =
#else
    type Layout =
#endif
     | Leaf of bool * obj * bool
     | Node of bool * layout * bool * layout * bool * joint
     | Attr of string * (string * string) list * layout

#if COMPILER
    and internal layout = Layout
#else
    and layout = Layout
#endif

#if COMPILER
    and internal joint = Joint
#else
    and joint = Joint
#endif

    [<NoEquality; NoComparison>]
#if COMPILER
    type internal IEnvironment = 
#else
    type IEnvironment = 
#endif
        abstract GetLayout : obj -> layout
        abstract MaxColumns : int
        abstract MaxRows : int
     
#if COMPILER
    module internal LayoutOps = 
#else
    module LayoutOps = 
#endif
        let rec juxtLeft = function
          | Leaf (jl,_,_)         -> jl
          | Node (jl,_,_,_,_,_) -> jl
          | Attr (_,_,l)        -> juxtLeft l

        let rec juxtRight = function
          | Leaf (_,_,jr)         -> jr
          | Node (_,_,_,_,jr,_) -> jr
          | Attr (_,_,l)        -> juxtRight l

        let mkNode l r joint =
           let jl = juxtLeft  l 
           let jm = juxtRight l || juxtLeft r 
           let jr = juxtRight r 
           Node(jl,l,jm,r,jr,joint)


        // constructors


        let objL   (obj:obj) = Leaf (false,obj,false)
        let sLeaf  (l,(str:string),r) = Leaf (l,(str:>obj),r)
        let wordL  str = sLeaf (false,str,false)
        let sepL   str = sLeaf (true ,str,true)   
        let rightL str = sLeaf (true ,str,false)   
        let leftL  str = sLeaf (false,str,true)
        let emptyL = sLeaf (true,"",true)
        let isEmptyL = function 
         | Leaf(true,s,true) -> 
            match s with 
            | :? string as s -> s = "" 
            | _ -> false
         | _ -> false
         

        let aboveL  l r = mkNode l r (Broken 0)

        let tagAttrL tag attrs l = Attr(tag,attrs,l)

        let apply2 f l r = if isEmptyL l then r else
                           if isEmptyL r then l else f l r

        let (^^)  l r  = mkNode l r (Unbreakable)
        let (++)  l r  = mkNode l r (Breakable 0)
        let (--)  l r  = mkNode l r (Breakable 1)
        let (---) l r  = mkNode l r (Breakable 2)
        let (@@)   l r = apply2 (fun l r -> mkNode l r (Broken 0)) l r
        let (@@-)  l r = apply2 (fun l r -> mkNode l r (Broken 1)) l r
        let (@@--) l r = apply2 (fun l r -> mkNode l r (Broken 2)) l r
        let tagListL tagger = function
            | []    -> emptyL
            | [x]   -> x
            | x::xs ->
                let rec process' prefixL = function
                  | []    -> prefixL
                  | y::ys -> process' ((tagger prefixL) ++ y) ys
                process' x xs
            
        let commaListL x = tagListL (fun prefixL -> prefixL ^^ rightL ",") x
        let semiListL x  = tagListL (fun prefixL -> prefixL ^^ rightL ";") x
        let spaceListL x = tagListL (fun prefixL -> prefixL) x
        let sepListL x y = tagListL (fun prefixL -> prefixL ^^ x) y
        let bracketL l = leftL "(" ^^ l ^^ rightL ")"
        let tupleL xs = bracketL (sepListL (sepL ",") xs)
        let aboveListL = function
          | []    -> emptyL
          | [x]   -> x
          | x::ys -> List.fold (fun pre y -> pre @@ y) x ys

        let optionL xL = function
          | None   -> wordL "None"
          | Some x -> wordL "Some" -- (xL x)

        let listL xL xs = leftL "[" ^^ sepListL (sepL ";") (List.map xL xs) ^^ rightL "]"

        let squareBracketL x = leftL "[" ^^ x ^^ rightL "]"    

        let braceL         x = leftL "{" ^^ x ^^ rightL "}"

        let boundedUnfoldL
                    (itemL     : 'a -> layout)
                    (project   : 'z -> ('a * 'z) option)
                    (stopShort : 'z -> bool)
                    (z : 'z)
                    maxLength =
          let rec consume n z =
            if stopShort z then [wordL "..."] else
            match project z with
              | None       -> []  // exhaused input 
              | Some (x,z) -> if n<=0 then [wordL "..."]               // hit print_length limit 
                                      else itemL x :: consume (n-1) z  // cons recursive... 
          consume maxLength z  

        let unfoldL itemL project z maxLength = boundedUnfoldL  itemL project (fun _ -> false) z maxLength
          
    /// These are a typical set of options used to control structured formatting.
    [<NoEquality; NoComparison>]
#if COMPILER
    type internal FormatOptions = 
#else
    type FormatOptions = 
#endif
        { FloatingPointFormat: string;
          AttributeProcessor: (string -> (string * string) list -> bool -> unit);
#if RUNTIME
#else
#if COMPILER    // FSharp.Compiler.dll: This is the PrintIntercepts extensibility point currently revealed by fsi.exe's AddPrinter
          PrintIntercepts: (IEnvironment -> obj -> Layout option) list;
          StringLimit : int;
#endif
#endif
          FormatProvider: System.IFormatProvider;
#if FX_RESHAPED_REFLECTION
          ShowNonPublic : bool
#else
          BindingFlags: System.Reflection.BindingFlags
#endif
          PrintWidth : int; 
          PrintDepth : int; 
          PrintLength : int;
          PrintSize : int;        
          ShowProperties : bool;
          ShowIEnumerable: bool; }
        static member Default =
            { FormatProvider = (System.Globalization.CultureInfo.InvariantCulture :> System.IFormatProvider);
#if RUNTIME
#else
#if COMPILER    // FSharp.Compiler.dll: This is the PrintIntercepts extensibility point currently revealed by fsi.exe's AddPrinter
              PrintIntercepts = [];
              StringLimit = System.Int32.MaxValue;
#endif
#endif
              AttributeProcessor= (fun _ _ _ -> ());
#if FX_RESHAPED_REFLECTION
              ShowNonPublic = false
#else
              BindingFlags = System.Reflection.BindingFlags.Public;
#endif
              FloatingPointFormat = "g10";
              PrintWidth = 80 ; 
              PrintDepth = 100 ; 
              PrintLength = 100;
              PrintSize = 10000;
              ShowProperties = false;
              ShowIEnumerable = true; }



#if COMPILER
    module internal ReflectUtils = 
#else
    module ReflectUtils = 
#endif
        open System
        open System.Reflection

#if FX_RESHAPED_REFLECTION
        open PrimReflectionAdapters
        open Microsoft.FSharp.Core.ReflectionAdapters
#endif

        [<NoEquality; NoComparison>]
        type TypeInfo =
          | TupleType of Type list
          | FunctionType of Type * Type
          | RecordType of (string * Type) list
          | SumType of (string * (string * Type) list) list
          | UnitType
          | ObjectType of Type

        let isNamedType(typ:Type) = not (typ.IsArray || typ.IsByRef || typ.IsPointer)
        let equivHeadTypes (ty1:Type) (ty2:Type) = 
            isNamedType(ty1) &&
            if ty1.IsGenericType then 
              ty2.IsGenericType && (ty1.GetGenericTypeDefinition()).Equals(ty2.GetGenericTypeDefinition())
            else 
              ty1.Equals(ty2)

        let option = typedefof<obj option>
        let func = typedefof<(obj -> obj)>

        let isOptionTy typ = equivHeadTypes typ (typeof<int option>)
        let isUnitType typ = equivHeadTypes typ (typeof<unit>)
        let isListType typ = 
            FSharpType.IsUnion typ && 
            (let cases = FSharpType.GetUnionCases typ 
             cases.Length > 0 && equivHeadTypes (typedefof<list<_>>) cases.[0].DeclaringType)

        [<NoEquality; NoComparison>]
        type ValueInfo =
          | TupleValue of obj list
          | FunctionClosureValue of System.Type 
          | RecordValue of (string * obj) list
          | ConstructorValue of string * (string * obj) list
          | ExceptionValue of System.Type * (string * obj) list
          | UnitValue
          | ObjectValue of obj

        module Value = 

            // Analyze an object to see if it the representation
            // of an F# value.
            let GetValueInfoOfObject (bindingFlags:BindingFlags) (obj : obj) = 
#if FX_RESHAPED_REFLECTION
              let showNonPublic = isNonPublicFlag bindingFlags
#endif
              match obj with 
              | null -> ObjectValue(obj)
              | _ -> 
                let reprty = obj.GetType() 

                // First a bunch of special rules for tuples
                // Because of the way F# currently compiles tuple values 
                // of size > 7 we can only reliably reflect on sizes up
                // to 7.

                if FSharpType.IsTuple reprty then 
                    TupleValue (FSharpValue.GetTupleFields obj |> Array.toList)
                elif FSharpType.IsFunction reprty then 
                    FunctionClosureValue reprty
                    
                // It must be exception, abstract, record or union.
                // Either way we assume the only properties defined on
                // the type are the actual fields of the type.  Again,
                // we should be reading attributes here that indicate the
                // true structure of the type, e.g. the order of the fields.   
#if FX_RESHAPED_REFLECTION
                elif FSharpType.IsUnion(reprty, showNonPublic) then 
                    let tag,vals = FSharpValue.GetUnionFields (obj,reprty, showNonPublic) 
#else
                elif FSharpType.IsUnion(reprty,bindingFlags) then 
                    let tag,vals = FSharpValue.GetUnionFields (obj,reprty,bindingFlags) 
#endif
                    let props = tag.GetFields()
                    let pvals = (props,vals) ||> Array.map2 (fun prop v -> prop.Name,v)
                    ConstructorValue(tag.Name, Array.toList pvals)
#if FX_RESHAPED_REFLECTION
                elif FSharpType.IsExceptionRepresentation(reprty, showNonPublic) then 
                    let props = FSharpType.GetExceptionFields(reprty, showNonPublic) 
                    let vals = FSharpValue.GetExceptionFields(obj, showNonPublic)
#else
                elif FSharpType.IsExceptionRepresentation(reprty,bindingFlags) then 
                    let props = FSharpType.GetExceptionFields(reprty,bindingFlags) 
                    let vals = FSharpValue.GetExceptionFields(obj,bindingFlags) 
#endif
                    let pvals = (props,vals) ||> Array.map2 (fun prop v -> prop.Name,v)
                    ExceptionValue(reprty, pvals |> Array.toList)
#if FX_RESHAPED_REFLECTION
                elif FSharpType.IsRecord(reprty, showNonPublic) then 
                    let props = FSharpType.GetRecordFields(reprty, showNonPublic) 
#else
                elif FSharpType.IsRecord(reprty,bindingFlags) then 
                    let props = FSharpType.GetRecordFields(reprty,bindingFlags) 
#endif
                    RecordValue(props |> Array.map (fun prop -> prop.Name, prop.GetValue(obj,null)) |> Array.toList)
                else
                    ObjectValue(obj)

            // This one is like the above but can make use of additional
            // statically-known type information to aid in the
            // analysis of null values. 

            let GetValueInfo bindingFlags (x : 'a)  (* x could be null *) = 
                let obj = (box x)
                match obj with 
                | null -> 
                   let typ = typeof<'a>
                   if isOptionTy typ then  ConstructorValue("None", [])
                   elif isUnitType typ then  UnitValue
                   else ObjectValue(obj)
                | _ -> 
                  GetValueInfoOfObject bindingFlags (obj) 


            let GetInfo bindingFlags (v:'a) = GetValueInfo bindingFlags (v:'a)

#if COMPILER
    module internal Display = 
#else
    module Display = 
#endif

        open ReflectUtils
        open LayoutOps
        let string_of_int (i:int) = i.ToString()

        let typeUsesSystemObjectToString (typ:System.Type) =
#if FX_ATLEAST_PORTABLE
            try 
#if FX_RESHAPED_REFLECTION
                let methInfo = typ.GetRuntimeMethod("ToString",[| |])
                methInfo.DeclaringType = typeof<System.Object>
#else
                let methInfo = typ.GetMethod("ToString",[| |])
                methInfo.DeclaringType = typeof<System.Object>
#endif
            with e -> false
#else        
            try let methInfo = typ.GetMethod("ToString",BindingFlags.Public ||| BindingFlags.Instance,null,[| |],null)
                methInfo.DeclaringType = typeof<System.Object>
            with e -> false
#endif
        /// If "str" ends with "ending" then remove it from "str", otherwise no change.
        let trimEnding (ending:string) (str:string) =
#if FX_NO_CULTURE_INFO_ARGS
          if str.EndsWith(ending) then 
#else
          if str.EndsWith(ending,StringComparison.Ordinal) then 
#endif
              str.Substring(0,str.Length - ending.Length) 
          else str

        let catchExn f = try Choice1Of2 (f ()) with e -> Choice2Of2 e

        // An implementation of break stack.
        // Uses mutable state, relying on linear threading of the state.

        [<NoEquality; NoComparison>]
        type Breaks = 
            Breaks of
                int *     // pos of next free slot 
                int *     // pos of next possible "outer" break - OR - outer=next if none possible 
                int array // stack of savings, -ve means it has been broken   

        // next  is next slot to push into - aka size of current occupied stack.  
        // outer counts up from 0, and is next slot to break if break forced.
        // - if all breaks forced, then outer=next.
        // - popping under these conditions needs to reduce outer and next.
        

        //let dumpBreaks prefix (Breaks(next,outer,stack)) = ()
        //   printf "%s: next=%d outer=%d stack.Length=%d\n" prefix next outer stack.Length;
        //   stdout.Flush() 
             
        let chunkN = 400      
        let breaks0 () = Breaks(0,0,Array.create chunkN 0)

        let pushBreak saving (Breaks(next,outer,stack)) =
            //dumpBreaks "pushBreak" (next,outer,stack);
            let stack = 
                if next = stack.Length then
                  Array.init (next + chunkN) (fun i -> if i < next then stack.[i] else 0) // expand if full 
                else
                  stack
           
            stack.[next] <- saving;
            Breaks(next+1,outer,stack)

        let popBreak (Breaks(next,outer,stack)) =
            //dumpBreaks "popBreak" (next,outer,stack);
            if next=0 then raise (Failure "popBreak: underflow");
            let topBroke = stack.[next-1] < 0
            let outer = if outer=next then outer-1 else outer  // if all broken, unwind 
            let next  = next - 1
            Breaks(next,outer,stack),topBroke

        let forceBreak (Breaks(next,outer,stack)) =
            //dumpBreaks "forceBreak" (next,outer,stack);
            if outer=next then
              // all broken 
                None
            else
                let saving = stack.[outer]
                stack.[outer] <- -stack.[outer];    
                let outer = outer+1
                Some (Breaks(next,outer,stack),saving)

        // -------------------------------------------------------------------------
        // fitting
        // ------------------------------------------------------------------------
          
        let squashTo (maxWidth,leafFormatter) layout =
            if maxWidth <= 0 then layout else 
            let rec fit breaks (pos,layout) =
                // breaks = break context, can force to get indentation savings.
                // pos    = current position in line
                // layout = to fit
                //------
                // returns:
                // breaks
                // layout - with breaks put in to fit it.
                // pos    - current pos in line = rightmost position of last line of block.
                // offset - width of last line of block
                // NOTE: offset <= pos -- depending on tabbing of last block
               
                let breaks,layout,pos,offset =
                    match layout with
                    | Attr (tag,attrs,l) ->
                        let breaks,layout,pos,offset = fit breaks (pos,l) 
                        let layout = Attr (tag,attrs,layout) 
                        breaks,layout,pos,offset
                    | Leaf (jl,obj,jr) ->
                        let text:string = leafFormatter obj 
                        // save the formatted text from the squash
                        let layout = Leaf(jl,(text :> obj),jr) 
                        let textWidth = text.Length
                        let rec fitLeaf breaks pos =
                          if pos + textWidth <= maxWidth then
                              breaks,layout,pos + textWidth,textWidth // great, it fits 
                          else
                              match forceBreak breaks with
                              | None                 -> 
                                  breaks,layout,pos + textWidth,textWidth // tough, no more breaks 
                              | Some (breaks,saving) -> 
                                  let pos = pos - saving 
                                  fitLeaf breaks pos
                       
                        fitLeaf breaks pos
                    | Node (jl,l,jm,r,jr,joint) ->
                        let mid = if jm then 0 else 1
                        match joint with
                        | Unbreakable    ->
                            let breaks,l,pos,offsetl = fit breaks (pos,l)    // fit left 
                            let pos = pos + mid                              // fit space if juxt says so 
                            let breaks,r,pos,offsetr = fit breaks (pos,r)    // fit right 
                            breaks,Node (jl,l,jm,r,jr,Unbreakable),pos,offsetl + mid + offsetr
                        | Broken indent ->
                            let breaks,l,pos,offsetl = fit breaks (pos,l)    // fit left 
                            let pos = pos - offsetl + indent                 // broken so - offset left + ident 
                            let breaks,r,pos,offsetr = fit breaks (pos,r)    // fit right 
                            breaks,Node (jl,l,jm,r,jr,Broken indent),pos,indent + offsetr
                        | Breakable indent ->
                            let breaks,l,pos,offsetl = fit breaks (pos,l)    // fit left 
                            // have a break possibility, with saving 
                            let saving = offsetl + mid - indent
                            let pos = pos + mid
                            if saving>0 then
                                let breaks = pushBreak saving breaks
                                let breaks,r,pos,offsetr = fit breaks (pos,r)
                                let breaks,broken = popBreak breaks
                                if broken then
                                    breaks,Node (jl,l,jm,r,jr,Broken indent)   ,pos,indent + offsetr
                                else
                                    breaks,Node (jl,l,jm,r,jr,Breakable indent),pos,offsetl + mid + offsetr
                            else
                                // actually no saving so no break 
                                let breaks,r,pos,offsetr = fit breaks (pos,r)
                                breaks,Node (jl,l,jm,r,jr,Breakable indent)  ,pos,offsetl + mid + offsetr
               
               //printf "\nDone:     pos=%d offset=%d" pos offset;
                breaks,layout,pos,offset
           
            let breaks = breaks0 ()
            let pos = 0
            let _,layout,_,_ = fit breaks (pos,layout)
            layout

        // -------------------------------------------------------------------------
        // showL
        // ------------------------------------------------------------------------

        let combine strs = System.String.Concat(Array.ofList(strs) : string[])
        let showL opts leafFormatter layout =
            let push x rstrs = x::rstrs
            let z0 = [],0
            let addText (rstrs,i) (text:string) = push text rstrs,i + text.Length
            let index   (_,i)               = i
            let extract rstrs = combine(List.rev rstrs) 
            let newLine (rstrs,_) n     = // \n then spaces... 
                let indent = new System.String(' ', n)
                let rstrs = push "\n"   rstrs
                let rstrs = push indent rstrs
                rstrs,n

            // addL: pos is tab level 
            let rec addL z pos layout = 
                match layout with 
                | Leaf (_,obj,_)                 -> 
                    let text = leafFormatter obj 
                    addText z text
                | Node (_,l,_,r,_,Broken indent) 
                     // Print width = 0 implies 1D layout, no squash
                     when not (opts.PrintWidth = 0)  -> 
                    let z = addL z pos l
                    let z = newLine z (pos+indent)
                    let z = addL z (pos+indent) r
                    z
                | Node (_,l,jm,r,_,_)             -> 
                    let z = addL z pos l
                    let z = if jm then z else addText z " "
                    let pos = index z
                    let z = addL z pos r
                    z
                | Attr (_,_,l) ->
                    addL z pos l
           
            let rstrs,_ = addL z0 0 layout
            extract rstrs


        // -------------------------------------------------------------------------
        // outL
        // ------------------------------------------------------------------------

        let outL outAttribute leafFormatter (chan : TextWriter) layout =
            // write layout to output chan directly 
            let write (s:string) = chan.Write(s)
            // z is just current indent 
            let z0 = 0
            let index i = i
            let addText z text  = write text;  (z + text.Length)
            let newLine _ n     = // \n then spaces... 
                let indent = new System.String(' ',n)
                chan.WriteLine();
                write indent;
                n
                
            // addL: pos is tab level 
            let rec addL z pos layout = 
                match layout with 
                | Leaf (_,obj,_)                 -> 
                    let text = leafFormatter obj 
                    addText z text
                | Node (_,l,_,r,_,Broken indent) -> 
                    let z = addL z pos l
                    let z = newLine z (pos+indent)
                    let z = addL z (pos+indent) r
                    z
                | Node (_,l,jm,r,_,_)             -> 
                    let z = addL z pos l
                    let z = if jm then z else addText z " "
                    let pos = index z
                    let z = addL z pos r
                    z 
                | Attr (tag,attrs,l) ->
                let _ = outAttribute tag attrs true
                let z = addL z pos l
                let _ = outAttribute tag attrs false
                z
           
            let _ = addL z0 0 layout
            ()

        // --------------------------------------------------------------------
        // pprinter: using general-purpose reflection...
        // -------------------------------------------------------------------- 
          
        let getValueInfo bindingFlags (x:'a) = Value.GetInfo bindingFlags (x:'a)

        let unpackCons recd =
            match recd with 
            | [(_,h);(_,t)] -> (h,t)
            | _             -> failwith "unpackCons"

        let getListValueInfo bindingFlags (x:obj) =
            match x with 
            | null -> None 
            | _ -> 
                match getValueInfo bindingFlags x with
                | ConstructorValue ("Cons",recd) -> Some (unpackCons recd)
                | ConstructorValue ("Empty",[]) -> None
                | _ -> failwith "List value had unexpected ValueInfo"

        let compactCommaListL xs = sepListL (sepL ",") xs // compact, no spaces around "," 
        let nullL = wordL "null"
        let measureL = wordL "()"
          
        // --------------------------------------------------------------------
        // pprinter: attributes
        // -------------------------------------------------------------------- 

        let makeRecordVerticalL nameXs =
            let itemL (name,xL) = let labelL = wordL name in ((labelL ^^ wordL "=")) -- (xL  ^^ (rightL ";"))
            let braceL xs = (leftL "{") ^^ xs ^^ (rightL "}")
            braceL (aboveListL (List.map itemL nameXs))

        // This is a more compact rendering of records - and is more like tuples 
        let makeRecordHorizontalL nameXs = 
            let itemL (name,xL) = let labelL = wordL name in ((labelL ^^ wordL "=")) -- xL
            let braceL xs = (leftL "{") ^^ xs ^^ (rightL "}")
            braceL (sepListL (rightL ";")  (List.map itemL nameXs))

        let makeRecordL nameXs = makeRecordVerticalL nameXs 

        let makePropertiesL nameXs =
            let itemL (name,v) = 
               let labelL = wordL name 
               (labelL ^^ wordL "=")
               ^^ (match v with 
                   | None -> wordL "?" 
                   | Some xL -> xL)
               ^^ (rightL ";")
            let braceL xs = (leftL "{") ^^ xs ^^ (rightL "}")
            braceL (aboveListL (List.map itemL nameXs))

        let makeListL itemLs =
            (leftL "[") 
            ^^ sepListL (rightL ";") itemLs 
            ^^ (rightL "]")

        let makeArrayL xs =
            (leftL "[|") 
            ^^ sepListL (rightL ";") xs 
            ^^ (rightL "|]")

        let makeArray2L xs = leftL "[" ^^ aboveListL xs ^^ rightL "]"  

        // --------------------------------------------------------------------
        // pprinter: anyL - support functions
        // -------------------------------------------------------------------- 

        let getProperty (obj: obj) name =
            let ty = obj.GetType()
#if FX_ATLEAST_PORTABLE
            let prop = ty.GetProperty(name, (BindingFlags.Instance ||| BindingFlags.Public ||| BindingFlags.NonPublic))
            if prop <> null then prop.GetValue(obj,[||])
#if FX_NO_MISSINGMETHODEXCEPTION
            // Profile 7, 47, 78 and 259 raise MissingMemberException
            else 
                let msg = System.String.Concat([| "Method '"; ty.FullName; "."; name; "' not found." |])
                raise (System.MissingMemberException(msg))
#else
            // Others raise MissingMethodException
            else 
                let msg = System.String.Concat([| "Method '"; ty.FullName; "."; name; "' not found." |])
                raise (System.MissingMethodException(msg))
#endif
#else
#if FX_NO_CULTURE_INFO_ARGS
            ty.InvokeMember(name, (BindingFlags.GetProperty ||| BindingFlags.Instance ||| BindingFlags.Public ||| BindingFlags.NonPublic), null, obj, [| |])
#else
            ty.InvokeMember(name, (BindingFlags.GetProperty ||| BindingFlags.Instance ||| BindingFlags.Public ||| BindingFlags.NonPublic), null, obj, [| |],CultureInfo.InvariantCulture)
#endif
#endif
        let formatChar isChar c = 
            match c with 
            | '\'' when isChar -> "\\\'"
            | '\"' when not isChar -> "\\\""
            //| '\n' -> "\\n"
            //| '\r' -> "\\r"
            //| '\t' -> "\\t"
            | '\\' -> "\\\\"
            | '\b' -> "\\b"
            | _ when System.Char.IsControl(c) -> 
                 let d1 = (int c / 100) % 10 
                 let d2 = (int c / 10) % 10 
                 let d3 = int c % 10 
                 "\\" + d1.ToString() + d2.ToString() + d3.ToString()
            | _ -> c.ToString()
            
        let formatString (s:string) =
            let rec check i = i < s.Length && not (System.Char.IsControl(s,i)) && s.[i] <> '\"' && check (i+1) 
            let rec conv i acc = if i = s.Length then combine (List.rev acc) else conv (i+1) (formatChar false s.[i] :: acc)  
            "\"" + s + "\""
            // REVIEW: should we check for the common case of no control characters? Reinstate the following?
            //"\"" + (if check 0 then s else conv 0 []) + "\""

        let formatStringInWidth (width:int) (str:string) =
            // Return a truncated version of the string, e.g.
            //   "This is the initial text, which has been truncated"+[12 chars]
            //
            // Note: The layout code forces breaks based on leaf size and possible break points.
            //       It does not force leaf size based on width.
            //       So long leaf-string width can not depend on their printing context...
            //
            // The suffix like "+[dd chars]" is 11 chars.
            //                  12345678901
            let suffixLength    = 11 // turning point suffix length
            let prefixMinLength = 12 // arbitrary. If print width is reduced, want to print a minimum of information on strings...
            let prefixLength = max (width - 2 (*quotes*) - suffixLength) prefixMinLength
            "\"" + (str.Substring(0,prefixLength)) + "\"" + "+[" + (str.Length - prefixLength).ToString() + " chars]"

        // --------------------------------------------------------------------
        // pprinter: anyL
        // -------------------------------------------------------------------- 
                           
        type Precedence = 
            | BracketIfTupleOrNotAtomic = 2
            | BracketIfTuple = 3
            | NeverBracket = 4

        // In fsi.exe, certain objects are not printed for top-level bindings.
        [<StructuralEquality; NoComparison>]
        type ShowMode = 
            | ShowAll 
            | ShowTopLevelBinding

        // polymorphic and inner recursion limitations prevent us defining polyL in the recursive loop 
        let polyL bindingFlags (objL: ShowMode -> int -> Precedence -> ValueInfo -> obj -> Layout) showMode i prec  (x:'a) (* x could be null *) =
            objL showMode i prec (getValueInfo bindingFlags (x:'a))  (box x) 

        let anyL showMode bindingFlags (opts:FormatOptions) (x:'a) =
            // showMode = ShowTopLevelBinding on the outermost expression when called from fsi.exe,
            // This allows certain outputs, e.g. objects that would print as <seq> to be suppressed, etc. See 4343.
            // Calls to layout proper sub-objects should pass showMode = ShowAll.

            // Precedences to ensure we add brackets in the right places   
            
            // Keep a record of objects encountered along the way
            let path = Dictionary<obj,int>(10,HashIdentity.Reference)

            // Roughly count the "nodes" printed, e.g. leaf items and inner nodes, but not every bracket and comma.
            let size = ref opts.PrintSize
            let exceededPrintSize() = !size<=0
            let countNodes n = if !size > 0 then size := !size - n else () // no need to keep decrementing (and avoid wrap around) 
            let stopShort _ = exceededPrintSize() // for unfoldL

            // Recursive descent
            let rec objL depthLim prec (x:obj) = polyL bindingFlags objWithReprL ShowAll  depthLim prec x // showMode for inner expr 
            and sameObjL depthLim prec (x:obj) = polyL bindingFlags objWithReprL showMode depthLim prec x // showMode preserved 

            and objWithReprL showMode depthLim prec (info:ValueInfo) (x:obj) (* x could be null *) =
                try
                  if depthLim<=0 || exceededPrintSize() then wordL "..." else
                  match x with 
                  | null -> 
                    reprL showMode (depthLim-1) prec info x
                  | _    ->
                    if (path.ContainsKey(x)) then 
                       wordL "..."
                    else 
                        path.Add(x,0);
                        let res = 
                          // Lazy<T> values. VS2008 used StructuredFormatDisplayAttribute to show via ToString. Dev10 (no attr) needs a special case.
                          let ty = x.GetType()
                          if ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<Lazy<_>> then
                            Some (wordL (x.ToString()))
                          else
                            // Try the StructuredFormatDisplayAttribute extensibility attribute
                            match x.GetType().GetCustomAttributes (typeof<StructuredFormatDisplayAttribute>, true) with
                            | null | [| |] -> None
                            | res -> 
                               let attr = (res.[0] :?> StructuredFormatDisplayAttribute) 
                               let txt = attr.Value
                               if txt = null || txt.Length <= 1 then  
                                   None
                               else
                                  let messageRegexPattern = @"^(?<pre>.*?)(?<!\\){(?<prop>.*?)(?<!\\)}(?<post>.*)$"
                                  let illFormedBracketPattern = @"(?<!\\){|(?<!\\)}"

                                  let rec buildObjMessageL (txt:string) (layouts:Layout list) =
                                    
                                    let replaceEscapedBrackets (txt:string) =
                                      txt.Replace("\{", "{").Replace("\}", "}")
                                      
                                    // to simplify support for escaped brackets, switch to using a Regex to simply parse the text as the following regex groups:
                                    //  1) Everything up to the first opening bracket not preceded by a "\", lazily
                                    //  2) Everything between that opening bracket and a closing bracket not preceded by a "\", lazily
                                    //  3) Everything after that closing bracket
                                    let m = System.Text.RegularExpressions.Regex.Match(txt, messageRegexPattern)
                                    match m.Success with
                                      | false ->  
                                        // there isn't a match on the regex looking for a property, so now let's make sure we don't have an ill-formed format string (i.e. mismatched/stray brackets)
                                        let illFormedMatch = System.Text.RegularExpressions.Regex.IsMatch(txt, illFormedBracketPattern)
                                        match illFormedMatch with
                                        | true -> None // there are mismatched brackets, bail out
                                        | false when layouts.Length > 1 -> Some (spaceListL (List.rev ((wordL (replaceEscapedBrackets(txt))::layouts))))
                                        | false -> Some (wordL (replaceEscapedBrackets(txt)))
                                      | true ->
                                        // we have a hit on a property reference
                                        let preText = replaceEscapedBrackets(m.Groups.["pre"].Value) // everything before the first opening bracket
                                        let postText = m.Groups.["post"].Value // Everything after the closing bracket
                                        let prop = replaceEscapedBrackets(m.Groups.["prop"].Value) // Unescape everything between the opening and closing brackets

                                        match catchExn (fun () -> getProperty x prop) with
                                          | Choice2Of2 e -> Some (wordL ("<StructuredFormatDisplay exception: " + e.Message + ">"))
                                          | Choice1Of2 alternativeObj ->
                                              try 
                                                  let alternativeObjL = 
                                                    match alternativeObj with 
                                                        // A particular rule is that if the alternative property
                                                        // returns a string, we turn off auto-quoting and escaping of
                                                        // the string, i.e. just treat the string as display text.
                                                        // This allows simple implementations of 
                                                        // such as
                                                        //
                                                        //    [<StructuredFormatDisplay("{StructuredDisplayString}I")>]
                                                        //    type BigInt(signInt:int, v : BigNat) =
                                                        //        member x.StructuredDisplayString = x.ToString()
                                                        //
                                                        | :? string as s -> sepL s
                                                        | _ -> 
                                                          // recursing like this can be expensive, so let's throttle it severely
                                                          sameObjL (depthLim/10) Precedence.BracketIfTuple alternativeObj
                                                  countNodes 0 // 0 means we do not count the preText and postText 

                                                  let postTextMatch = System.Text.RegularExpressions.Regex.Match(postText, messageRegexPattern)
                                                  // the postText for this node will be everything up to the next occurrence of an opening brace, if one exists
                                                  let currentPostText =
                                                    match postTextMatch.Success with
                                                      | false -> postText 
                                                      | true -> postTextMatch.Groups.["pre"].Value

                                                  let newLayouts = (sepL preText ^^ alternativeObjL ^^ sepL currentPostText)::layouts
                                                  match postText with
                                                    | "" ->
                                                      //We are done, build a space-delimited layout from the collection of layouts we've accumulated
                                                      Some (spaceListL (List.rev newLayouts))
                                                    | remainingPropertyText when postTextMatch.Success ->
                                                      
                                                      // look for stray brackets in the text before the next opening bracket
                                                      let strayClosingMatch = System.Text.RegularExpressions.Regex.IsMatch(postTextMatch.Groups.["pre"].Value, illFormedBracketPattern)
                                                      match strayClosingMatch with
                                                      | true -> None
                                                      | false -> 
                                                        // More to process, keep going, using the postText starting at the next instance of a '{'
                                                        let openingBracketIndex = postTextMatch.Groups.["prop"].Index-1
                                                        buildObjMessageL remainingPropertyText.[openingBracketIndex..] newLayouts
                                                    | remaingPropertyText ->
                                                      // make sure we don't have any stray brackets
                                                      let strayClosingMatch = System.Text.RegularExpressions.Regex.IsMatch(remaingPropertyText, illFormedBracketPattern)
                                                      match strayClosingMatch with
                                                      | true -> None
                                                      | false ->
                                                        // We are done, there's more text but it doesn't contain any more properties, we need to remove escaped brackets now though
                                                        // since that wasn't done when creating currentPostText
                                                        Some (spaceListL (List.rev ((sepL preText ^^ alternativeObjL ^^ sepL (replaceEscapedBrackets(remaingPropertyText)))::layouts)))
                                              with _ -> 
                                                None
                                  // Seed with an empty layout with a space to the left for formatting purposes
                                  buildObjMessageL txt [leftL ""] 
#if RUNTIME
#else
#if COMPILER    // FSharp.Compiler.dll: This is the PrintIntercepts extensibility point currently revealed by fsi.exe's AddPrinter
                        let res = 
                            match res with 
                            | Some _ -> res
                            | None -> 
                                let env = { new IEnvironment with
                                                member env.GetLayout(y) = objL (depthLim-1) Precedence.BracketIfTuple y 
                                                member env.MaxColumns = opts.PrintLength
                                                member env.MaxRows = opts.PrintLength }
                                opts.PrintIntercepts |> List.tryPick (fun intercept -> intercept env x)
#endif
#endif
                        let res = 
                            match res with 
                            | Some res -> res
                            | None     -> reprL showMode (depthLim-1) prec info x
                        path .Remove(x) |> ignore;
                        res
                with
                  e ->
                    countNodes 1
                    wordL ("Error: " + e.Message)

            and recdAtomicTupleL depthLim recd =
                // tuples up args to UnionConstruction or ExceptionConstructor. no node count.
                match recd with 
                | [(_,x)] -> objL depthLim Precedence.BracketIfTupleOrNotAtomic x 
                | txs     -> leftL "(" ^^ compactCommaListL (List.map (snd >> objL depthLim Precedence.BracketIfTuple) txs) ^^ rightL ")" 

            and bracketIfL b basicL =
                if b then (leftL "(") ^^ basicL ^^ (rightL ")") else basicL

            and reprL showMode depthLim prec repr x (* x could be null *) =
                let showModeFilter lay = match showMode with ShowAll -> lay | ShowTopLevelBinding -> emptyL                                                             
                match repr with 
                | TupleValue vals -> 
                    let basicL = sepListL (rightL ",") (List.map (objL depthLim Precedence.BracketIfTuple ) vals)
                    bracketIfL (prec <= Precedence.BracketIfTuple) basicL 

                | RecordValue items -> 
                    let itemL (name,x) =
                      countNodes 1 // record labels are counted as nodes. [REVIEW: discussion under 4090].
                      (name,objL depthLim Precedence.BracketIfTuple x)
                    makeRecordL (List.map itemL items)

                | ConstructorValue (constr,recd) when // x is List<T>. Note: "null" is never a valid list value. 
                                                      x<>null && isListType (x.GetType()) ->
                    match constr with 
                    | "Cons" -> 
                        let (x,xs) = unpackCons recd
                        let project xs = getListValueInfo bindingFlags xs
                        let itemLs = objL depthLim Precedence.BracketIfTuple x :: boundedUnfoldL (objL depthLim Precedence.BracketIfTuple) project stopShort xs (opts.PrintLength - 1)
                        makeListL itemLs
                    | _ ->
                        countNodes 1
                        wordL "[]" 

                | ConstructorValue(nm,[])   ->
                    countNodes 1
                    (wordL nm)

                | ConstructorValue(nm,recd) ->
                    countNodes 1 // e.g. Some (Some (Some (Some 2))) should count for 5 
                    (wordL nm --- recdAtomicTupleL depthLim recd) |> bracketIfL (prec <= Precedence.BracketIfTupleOrNotAtomic)

                | ExceptionValue(ty,recd) ->
                    countNodes 1
                    let name = ty.Name 
                    match recd with
                      | []   -> (wordL name)
                      | recd -> (wordL name --- recdAtomicTupleL depthLim recd) |> bracketIfL (prec <= Precedence.BracketIfTupleOrNotAtomic)

                | FunctionClosureValue ty ->
                    // Q: should function printing include the ty.Name? It does not convey much useful info to most users, e.g. "clo@0_123".    
                    countNodes 1
                    wordL ("<fun:"+ty.Name+">") |> showModeFilter

                | ObjectValue(obj)  ->
                    match obj with 
                    | null -> (countNodes 1; nullL)
                    | _ -> 
                    let ty = obj.GetType()
                    match obj with 
                    | :? string as s ->
                        countNodes 1
#if COMPILER  
                        if s.Length + 2(*quotes*) <= opts.StringLimit then
                           // With the quotes, it fits within the limit.
                           wordL (formatString s)
                        else
                           // When a string is considered too long to print, there is a choice: what to print?
                           // a) <string>            -- follows <fun:typename>
                           // b) <string:length>     -- follows <fun:typename> and gives just the length
                           // c) "abcdefg"+[n chars] -- gives a prefix and the remaining chars
                           wordL (formatStringInWidth opts.StringLimit s)
#else
                        wordL (formatString s)  
#endif                        
                    | :? System.Array as arr -> 
                        match arr.Rank with
                        | 1 -> 
                             let n = arr.Length
                             let b1 = arr.GetLowerBound(0) 
                             let project depthLim = if depthLim=(b1+n) then None else Some (box (arr.GetValue(depthLim)),depthLim+1)
                             let itemLs = boundedUnfoldL (objL depthLim Precedence.BracketIfTuple) project stopShort b1 opts.PrintLength
                             makeArrayL (if b1 = 0 then itemLs else wordL("bound1="+string_of_int b1)::itemLs)
                        | 2 -> 
                             let n1 = arr.GetLength(0)
                             let n2 = arr.GetLength(1)
                             let b1 = arr.GetLowerBound(0) 
                             let b2 = arr.GetLowerBound(1) 
                             let project2 x y =
                               if x>=(b1+n1) || y>=(b2+n2) then None
                               else Some (box (arr.GetValue(x,y)),y+1)
                             let rowL x = boundedUnfoldL (objL depthLim Precedence.BracketIfTuple) (project2 x) stopShort b2 opts.PrintLength |> makeListL
                             let project1 x = if x>=(b1+n1) then None else Some (x,x+1)
                             let rowsL  = boundedUnfoldL rowL project1 stopShort b1 opts.PrintLength
                             makeArray2L (if b1=0 && b2 = 0 then rowsL else wordL("bound1=" + string_of_int b1)::wordL("bound2=" + string_of_int b2)::rowsL)
                          | n -> 
                             makeArrayL [wordL("rank=" + string_of_int n)]
                        
                    // Format 'set' and 'map' nicely
                    | _ when  
                          (let ty = obj.GetType()
                           ty.IsGenericType && (ty.GetGenericTypeDefinition() = typedefof<Map<int,int>> 
                                                || ty.GetGenericTypeDefinition() = typedefof<Set<int>>) ) ->
                         let ty = obj.GetType()
                         let word = if ty.GetGenericTypeDefinition() = typedefof<Map<int,int>> then "map" else "set"
                         let possibleKeyValueL v = 
                             if word = "map" &&
                                (match v with null -> false | _ -> true) && 
                                v.GetType().IsGenericType && 
                                v.GetType().GetGenericTypeDefinition() = typedefof<KeyValuePair<int,int>> then
                                  objL depthLim Precedence.BracketIfTuple (v.GetType().GetProperty("Key").GetValue(v, [| |]), 
                                                                           v.GetType().GetProperty("Value").GetValue(v, [| |]))
                             else
                                  objL depthLim Precedence.BracketIfTuple v
                         let it = (obj :?>  System.Collections.IEnumerable).GetEnumerator() 
                         try 
                           let itemLs = boundedUnfoldL possibleKeyValueL (fun () -> if it.MoveNext() then Some(it.Current,()) else None) stopShort () (1+opts.PrintLength/12)
                           (wordL word --- makeListL itemLs) |> bracketIfL (prec <= Precedence.BracketIfTupleOrNotAtomic)
                         finally 
                            match it with 
                            | :? System.IDisposable as e -> e.Dispose()
                            | _ -> ()

                    | :? System.Collections.IEnumerable as ie ->
                         let showContent = 
                            // do not display content of IQueryable since its execution may take significant time
                            opts.ShowIEnumerable && (ie.GetType().GetInterfaces() |> Array.exists(fun ty -> ty.FullName = "System.Linq.IQueryable") |> not)

                         if showContent then
                           let word = "seq"
                           let it = ie.GetEnumerator() 
                           try 
                             let itemLs = boundedUnfoldL (objL depthLim Precedence.BracketIfTuple) (fun () -> if it.MoveNext() then Some(it.Current,()) else None) stopShort () (1+opts.PrintLength/30)
                             (wordL word --- makeListL itemLs) |> bracketIfL (prec <= Precedence.BracketIfTupleOrNotAtomic)
                           finally 
                              match it with 
                              | :? System.IDisposable as e -> e.Dispose()
                              | _ -> ()
                             
                         else
                           // Sequence printing is turned off for declared-values, and maybe be disabled to users.
                           // There is choice here, what to print? <seq> or ... or ?
                           // Also, in the declared values case, if the sequence is actually a known non-lazy type (list, array etc etc) we could print it.  
                           wordL "<seq>" |> showModeFilter
                    | _ ->
                         if showMode = ShowTopLevelBinding && typeUsesSystemObjectToString (obj.GetType()) then
                           emptyL
                         else
                           countNodes 1
                           let basicL = LayoutOps.objL obj  // This buries an obj in the layout, rendered at squash time via a leafFormatter.
                                                            // If the leafFormatter was directly here, then layout leaves could store strings.
                           match obj with 
                           | _ when opts.ShowProperties ->
#if FX_ATLEAST_PORTABLE
                              let props = ty.GetProperties(BindingFlags.Instance ||| BindingFlags.Public)
#else                           
                              let props = ty.GetProperties(BindingFlags.GetField ||| BindingFlags.Instance ||| BindingFlags.Public)
#endif                              
                              let props = 
                                props |> Array.filter (fun pi ->
                                    // check if property is annotated with System.Diagnostics.DebuggerBrowsable(Never). 
                                    // Its evaluation may have unexpected side effects and\or block printing.
                                    match Seq.toArray (pi.GetCustomAttributes(typeof<System.Diagnostics.DebuggerBrowsableAttribute>, false)) with
                                    | [|:? System.Diagnostics.DebuggerBrowsableAttribute as attr |] -> attr.State <> System.Diagnostics.DebuggerBrowsableState.Never
                                    | _ -> true
                                )

                              // massively reign in deep printing of properties 
                              let nDepth = depthLim/10
#if FX_ATLEAST_PORTABLE
                              System.Array.Sort((props),{ new System.Collections.Generic.IComparer<PropertyInfo> with member this.Compare(p1,p2) = compare (p1.Name) (p2.Name) } );
#else                              
                              System.Array.Sort((props:>System.Array),{ new System.Collections.IComparer with member this.Compare(p1,p2) = compare ((p1 :?> PropertyInfo).Name) ((p2 :?> PropertyInfo).Name) } );
#endif                        

                              if props.Length = 0 || (nDepth <= 0) then basicL 
                              else basicL --- 
                                     (props 
                                      |> Array.toList 
                                      |> List.map (fun p -> (p.Name,(try Some (objL nDepth Precedence.BracketIfTuple (getProperty obj p.Name)) 
                                                                     with _ -> None)))
                                      |> makePropertiesL)
                           | _ -> basicL 
                | UnitValue -> countNodes 1; measureL

            polyL bindingFlags objWithReprL showMode opts.PrintDepth Precedence.BracketIfTuple x

        // --------------------------------------------------------------------
        // pprinter: leafFormatter
        // --------------------------------------------------------------------

        let leafFormatter (opts:FormatOptions) (obj :obj) =
            match obj with 
            | null -> "null"
            | :? double as d -> 
                let s = d.ToString(opts.FloatingPointFormat,opts.FormatProvider)
                if System.Double.IsNaN(d) then "nan"
                elif System.Double.IsNegativeInfinity(d) then "-infinity"
                elif System.Double.IsPositiveInfinity(d) then "infinity"
                elif opts.FloatingPointFormat.[0] = 'g'  && String.forall(fun c -> System.Char.IsDigit(c) || c = '-')  s
                then s + ".0" 
                else s
            | :? single as d -> 
                (if System.Single.IsNaN(d) then "nan"
                 elif System.Single.IsNegativeInfinity(d) then "-infinity"
                 elif System.Single.IsPositiveInfinity(d) then "infinity"
                 elif opts.FloatingPointFormat.Length >= 1 && opts.FloatingPointFormat.[0] = 'g' 
                  && float32(System.Int32.MinValue) < d && d < float32(System.Int32.MaxValue) 
                  && float32(int32(d)) = d 
                 then (System.Convert.ToInt32 d).ToString(opts.FormatProvider) + ".0"
                 else d.ToString(opts.FloatingPointFormat,opts.FormatProvider)) 
                + "f"
            | :? System.Decimal as d -> d.ToString("g",opts.FormatProvider) + "M"
            | :? uint64 as d -> d.ToString(opts.FormatProvider) + "UL"
            | :? int64  as d -> d.ToString(opts.FormatProvider) + "L"
            | :? int32  as d -> d.ToString(opts.FormatProvider)
            | :? uint32 as d -> d.ToString(opts.FormatProvider) + "u"
            | :? int16  as d -> d.ToString(opts.FormatProvider) + "s"
            | :? uint16 as d -> d.ToString(opts.FormatProvider) + "us"
            | :? sbyte  as d -> d.ToString(opts.FormatProvider) + "y"
            | :? byte   as d -> d.ToString(opts.FormatProvider) + "uy"
            | :? nativeint as d -> d.ToString() + "n"
            | :? unativeint  as d -> d.ToString() + "un"
            | :? bool   as b -> (if b then "true" else "false")
            | :? char   as c -> "\'" + formatChar true c + "\'"
            | _ -> try  let text = obj.ToString()
                        if text = null then "" else text
                   with e ->
                     // If a .ToString() call throws an exception, catch it and use the message as the result.
                     // This may be informative, e.g. division by zero etc...
                     "<ToString exception: " + e.Message + ">" 

        let any_to_layout opts x = anyL ShowAll BindingFlags.Public opts x

        let squash_layout opts l = 
            // Print width = 0 implies 1D layout, no squash
            if opts.PrintWidth = 0 then 
                l 
            else 
                l |> squashTo (opts.PrintWidth,leafFormatter opts)

        let output_layout opts oc l = 
            l |> squash_layout opts 
              |> outL opts.AttributeProcessor (leafFormatter opts) oc

        let layout_to_string opts l = 
            l |> squash_layout opts 
              |> showL opts (leafFormatter opts) 

        let output_any_ex opts oc x = x |> any_to_layout opts |> output_layout opts oc

        let output_any oc x = output_any_ex FormatOptions.Default oc x

        let layout_as_string opts x = x |> any_to_layout opts |> layout_to_string opts

        let any_to_string x = layout_as_string FormatOptions.Default x

#if RUNTIME
#if FX_RESHAPED_REFLECTION
        let internal anyToStringForPrintf opts (showNonPublicMembers : bool) x = 
            let bindingFlags = ReflectionUtils.toBindingFlags showNonPublicMembers
#else
        let internal anyToStringForPrintf opts (bindingFlags:BindingFlags) x = 
#endif
            x |> anyL ShowAll bindingFlags opts |> layout_to_string opts
#endif

#if COMPILER
        /// Called 
        let fsi_any_to_layout opts x = anyL ShowTopLevelBinding BindingFlags.Public opts x 
#endif  
