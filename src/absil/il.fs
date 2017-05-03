// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module internal Microsoft.FSharp.Compiler.AbstractIL.IL

#nowarn "49"
#nowarn "343" // The type 'ILAssemblyRef' implements 'System.IComparable' explicitly but provides no corresponding override for 'Object.Equals'.
#nowarn "346" // The struct, record or union type 'IlxExtensionType' has an explicit implementation of 'Object.Equals'. ...


open Internal.Utilities
open Microsoft.FSharp.Compiler.AbstractIL
open Microsoft.FSharp.Compiler.AbstractIL.Diagnostics
open Microsoft.FSharp.Compiler.AbstractIL.Internal
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library
open System.Collections
open System.Collections.Generic
open System.Collections.Concurrent
open System.Runtime.CompilerServices
 
let logging = false 

let runningOnMono = 
#if ENABLE_MONO_SUPPORT
// Officially supported way to detect if we are running on Mono.
// See http://www.mono-project.com/FAQ:_Technical
// "How can I detect if am running in Mono?" section
    try
        System.Type.GetType("Mono.Runtime") <> null
    with e-> 
        // Must be robust in the case that someone else has installed a handler into System.AppDomain.OnTypeResolveEvent
        // that is not reliable.
        // This is related to bug 5506--the issue is actually a bug in VSTypeResolutionService.EnsurePopulated which is  
        // called by OnTypeResolveEvent. The function throws a NullReferenceException. I'm working with that team to get 
        // their issue fixed but we need to be robust here anyway.
        false  
#else
    false
#endif

let _ = if logging then dprintn "* warning: Il.logging is on"

let int_order = LanguagePrimitives.FastGenericComparer<int>

let notlazy v = Lazy.CreateFromValue v

/// A little ugly, but the idea is that if a data structure does not 
/// contain lazy values then we don't add laziness.  So if the thing to map  
/// is already evaluated then immediately apply the function.  
let lazyMap f (x:Lazy<_>) =  
      if x.IsValueCreated then notlazy (f (x.Force())) else lazy (f (x.Force()))

[<RequireQualifiedAccess>]
type PrimaryAssembly = 
    | Mscorlib
    | DotNetCore   

    member this.Name = 
        match this with
        | Mscorlib -> "mscorlib"
        | DotNetCore -> "System.Runtime"
    static member IsSomePrimaryAssembly n = 
      n = PrimaryAssembly.Mscorlib.Name 
      || n = PrimaryAssembly.DotNetCore.Name  

// -------------------------------------------------------------------- 
// Utilities: type names
// -------------------------------------------------------------------- 

let splitNameAt (nm:string) idx = 
    if idx < 0 then failwith "splitNameAt: idx < 0";
    let last = nm.Length - 1 
    if idx > last then failwith "splitNameAt: idx > last";
    (nm.Substring(0,idx)),
    (if idx < last then nm.Substring (idx+1,last - idx) else "")

let rec splitNamespaceAux (nm:string) = 
    match nm.IndexOf '.' with 
    | -1 -> [nm]
    | idx -> 
        let s1,s2 = splitNameAt nm idx 
        s1::splitNamespaceAux s2 

/// Global State. All namespace splits ever seen
// ++GLOBAL MUTABLE STATE (concurrency-safe)
let memoizeNamespaceTable = new ConcurrentDictionary<string,string list>()

//  ++GLOBAL MUTABLE STATE (concurrency-safe)
let memoizeNamespaceRightTable = new ConcurrentDictionary<string,string option * string>()


let splitNamespace nm =
    memoizeNamespaceTable.GetOrAdd(nm, splitNamespaceAux)

let splitNamespaceMemoized nm = splitNamespace nm

// ++GLOBAL MUTABLE STATE (concurrency-safe)
let memoizeNamespaceArrayTable = 
    Concurrent.ConcurrentDictionary<string,string[]>()

let splitNamespaceToArray nm =
    memoizeNamespaceArrayTable.GetOrAdd(nm, fun nm -> 
        let x = Array.ofList (splitNamespace nm)
        x)

let splitILTypeName (nm:string) = 
    match nm.LastIndexOf '.' with
    | -1 -> [],nm
    | idx -> 
        let s1,s2 = splitNameAt nm idx
        splitNamespace s1,s2

let emptyStringArray = ([| |] : string[])

// Duplicate of comment in import.fs:
//   The type names that flow to the point include the "mangled" type names used for static parameters for provided types.
//   For example, 
//       Foo.Bar,"1.0"
//   This is because the ImportSystemType code goes via Abstract IL type references. Ultimately this probably isn't 
//   the best way to do things.
let splitILTypeNameWithPossibleStaticArguments (nm:string) = 
    let nm,suffix = 
        match nm.IndexOf ',' with
        | -1 -> nm, None
        | idx -> let s1, s2 = splitNameAt nm idx in s1, Some s2

    let nsp,nm = 
        match nm.LastIndexOf '.' with
        | -1 -> emptyStringArray,nm
        | idx -> 
            let s1,s2 = splitNameAt nm idx
            splitNamespaceToArray s1,s2
    nsp, (match suffix with None -> nm | Some s -> nm + "," + s)

(*
splitILTypeNameWithPossibleStaticArguments "Foo" = ([| |], "Foo")
splitILTypeNameWithPossibleStaticArguments "Foo.Bar" = ([| "Foo" |], "Bar")
splitILTypeNameWithPossibleStaticArguments "Foo.Bar,3" = ([| "Foo" |], "Bar,3")
splitILTypeNameWithPossibleStaticArguments "Foo.Bar," = ([| "Foo" |], "Bar,")
splitILTypeNameWithPossibleStaticArguments "Foo.Bar,\"1.0\"" = ([| "Foo" |], "Bar,\"1.0\"")
splitILTypeNameWithPossibleStaticArguments "Foo.Bar.Bar,\"1.0\"" = ([| "Foo"; "Bar" |], "Bar,\"1.0\"")
*)

let unsplitTypeName (ns,n) = 
    match ns with 
    | [] -> String.concat "." ns + "." + n 
    | _ -> n 

let splitTypeNameRightAux nm = 
    if String.contains nm '.' then 
      let idx = String.rindex nm '.'
      let s1,s2 = splitNameAt nm idx
      Some s1,s2 
    else None, nm

let splitTypeNameRight nm =
    memoizeNamespaceRightTable.GetOrAdd(nm, splitTypeNameRightAux)

// -------------------------------------------------------------------- 
// Ordered lists with a lookup table
// --------------------------------------------------------------------

/// This is used to store event, property and field maps.
type LazyOrderedMultiMap<'Key,'Data when 'Key : equality>(keyf : 'Data -> 'Key, lazyItems : Lazy<'Data list>) = 

    let quickMap= 
        lazyItems |> lazyMap (fun entries -> 
            let t = new Dictionary<_,_>(entries.Length, HashIdentity.Structural)
            do entries |> List.iter (fun y -> let key = keyf y in t.[key] <- y :: (if t.ContainsKey(key) then t.[key] else [])) 
            t)

    member self.Entries() = lazyItems.Force()

    member self.Add(y) = new LazyOrderedMultiMap<'Key,'Data>(keyf, lazyItems |> lazyMap (fun x -> y :: x))
    
    member self.Filter(f) = new LazyOrderedMultiMap<'Key,'Data>(keyf, lazyItems |> lazyMap (List.filter f))

    member self.Item with get(x) = let t = quickMap.Force() in if t.ContainsKey x then t.[x] else []


//---------------------------------------------------------------------
// SHA1 hash-signing algorithm.  Used to get the public key token from
// the public key.
//---------------------------------------------------------------------


let b0 n =  (n &&& 0xFF)
let b1 n =  ((n >>> 8) &&& 0xFF)
let b2 n =  ((n >>> 16) &&& 0xFF)
let b3 n =  ((n >>> 24) &&& 0xFF)


module SHA1 = 
    let inline (>>>&)  (x:int) (y:int)  = int32 (uint32 x >>> y)
    let f(t,b,c,d) = 
        if t < 20 then (b &&& c) ||| ((~~~b) &&& d)
        elif t < 40 then b ^^^ c ^^^ d
        elif t < 60 then (b &&& c) ||| (b &&& d) ||| (c &&& d)
        else b ^^^ c ^^^ d

    let [<Literal>] k0to19 = 0x5A827999
    let [<Literal>] k20to39 = 0x6ED9EBA1
    let [<Literal>] k40to59 = 0x8F1BBCDC
    let [<Literal>] k60to79 = 0xCA62C1D6

    let k t = 
        if t < 20 then k0to19 
        elif t < 40 then k20to39 
        elif t < 60 then k40to59 
        else k60to79 


    type SHAStream = 
        { stream: byte[];
          mutable pos: int;
          mutable eof:  bool; }

    let rotLeft32 x n =  (x <<< n) ||| (x >>>& (32-n))

    
    // padding and length (in bits!) recorded at end 
    let shaAfterEof sha  = 
        let n = sha.pos
        let len = sha.stream.Length
        if n = len then 0x80
        else 
          let padded_len = (((len + 9 + 63) / 64) * 64) - 8
          if n < padded_len - 8  then 0x0  
          elif (n &&& 63) = 56 then int32 ((int64 len * int64 8) >>> 56) &&& 0xff
          elif (n &&& 63) = 57 then int32 ((int64 len * int64 8) >>> 48) &&& 0xff
          elif (n &&& 63) = 58 then int32 ((int64 len * int64 8) >>> 40) &&& 0xff
          elif (n &&& 63) = 59 then int32 ((int64 len * int64 8) >>> 32) &&& 0xff
          elif (n &&& 63) = 60 then int32 ((int64 len * int64 8) >>> 24) &&& 0xff
          elif (n &&& 63) = 61 then int32 ((int64 len * int64 8) >>> 16) &&& 0xff
          elif (n &&& 63) = 62 then int32 ((int64 len * int64 8) >>> 8) &&& 0xff
          elif (n &&& 63) = 63 then (sha.eof <- true; int32 (int64 len * int64 8) &&& 0xff)
          else 0x0

    let shaRead8 sha = 
        let s = sha.stream 
        let b = if sha.pos >= s.Length then shaAfterEof sha else int32 s.[sha.pos]
        sha.pos <- sha.pos + 1
        b
        
    let shaRead32 sha  = 
        let b0 = shaRead8 sha
        let b1 = shaRead8 sha
        let b2 = shaRead8 sha
        let b3 = shaRead8 sha
        let res = (b0 <<< 24) ||| (b1 <<< 16) ||| (b2 <<< 8) ||| b3
        res

    let sha1Hash sha = 
        let mutable h0 = 0x67452301
        let mutable h1 = 0xEFCDAB89
        let mutable h2 = 0x98BADCFE
        let mutable h3 = 0x10325476
        let mutable h4 = 0xC3D2E1F0
        let mutable a = 0
        let mutable b = 0
        let mutable c = 0
        let mutable d = 0
        let mutable e = 0
        let w = Array.create 80 0x00
        while (not sha.eof) do
            for i = 0 to 15 do
                w.[i] <- shaRead32 sha
            for t = 16 to 79 do
                w.[t] <- rotLeft32 (w.[t-3] ^^^ w.[t-8] ^^^ w.[t-14] ^^^ w.[t-16]) 1
            a <- h0 
            b <- h1
            c <- h2
            d <- h3
            e <- h4
            for t = 0 to 79 do
                let temp = (rotLeft32 a 5) + f(t,b,c,d) + e + w.[t] + k(t)
                e <- d
                d <- c
                c <- rotLeft32 b 30
                b <- a
                a <- temp
            h0 <- h0 + a
            h1 <- h1 + b
            h2 <- h2 + c
            h3 <- h3 + d
            h4 <- h4 + e
        h0,h1,h2,h3,h4

    let sha1HashBytes s = 
        let (_h0,_h1,_h2,h3,h4) = sha1Hash { stream = s; pos = 0; eof = false }   // the result of the SHA algorithm is stored in registers 3 and 4
        Array.map byte [|  b0 h4; b1 h4; b2 h4; b3 h4; b0 h3; b1 h3; b2 h3; b3 h3; |]


let sha1HashBytes s = SHA1.sha1HashBytes s

// --------------------------------------------------------------------
// 
// -------------------------------------------------------------------- 

type ILVersionInfo = uint16 * uint16 * uint16 * uint16

type Locale = string

[<StructuralEquality; StructuralComparison>]
type PublicKey =
    | PublicKey of byte[]
    | PublicKeyToken of byte[]
    member x.IsKey=match x with PublicKey _ -> true | _ -> false
    member x.IsKeyToken=match x with PublicKeyToken _ -> true | _ -> false
    member x.Key=match x with PublicKey b -> b | _ -> invalidOp "not a key"
    member x.KeyToken=match x with PublicKeyToken b -> b | _ -> invalidOp"not a key token"

    member x.ToToken() = 
        match x with 
        | PublicKey bytes -> SHA1.sha1HashBytes bytes
        | PublicKeyToken token -> token
    static member KeyAsToken(k) = PublicKeyToken(PublicKey(k).ToToken())

[<StructuralEquality; StructuralComparison>]
type AssemblyRefData =
    { assemRefName: string;
      assemRefHash: byte[] option;
      assemRefPublicKeyInfo: PublicKey option;
      assemRefRetargetable: bool;
      assemRefVersion: ILVersionInfo option;
      assemRefLocale: Locale option; } 

/// Global state: table of all assembly references keyed by AssemblyRefData.
let AssemblyRefUniqueStampGenerator = new UniqueStampGenerator<AssemblyRefData>()

let compareVersions x y =
    match x,y with 
    | None, None -> 0
    | Some _, None -> 1
    | None, Some _ -> -1
    | Some(x1,x2,x3,x4), Some(y1,y2,y3,y4) ->
        if y1>x1 then 1 
        elif y1<x1 then -1
        elif y2>x2 then 1 
        elif y2<x1 then -1
        elif y3>x3 then 1 
        elif y3<x1 then -1
        elif y4>x4 then 1 
        elif y4<x1 then -1
        else 0

let isMscorlib data =
    if System.String.Compare(data.assemRefName, "mscorlib") = 0 then true 
    else false

let GetReferenceUnifiedVersion data = 
    let mutable highest = data.assemRefVersion
    if not (isMscorlib data) then 
        for ref in AssemblyRefUniqueStampGenerator.Table do
            if System.String.Compare(ref.assemRefName, data.assemRefName) = 0 && highest < ref.assemRefVersion then 
                highest <- ref.assemRefVersion
    highest

[<Sealed>]
type ILAssemblyRef(data)  =  
    let uniqueStamp = AssemblyRefUniqueStampGenerator.Encode(data)
    member x.Name=data.assemRefName
    member x.Hash=data.assemRefHash
    member x.PublicKey=data.assemRefPublicKeyInfo
    member x.Retargetable=data.assemRefRetargetable  
    member x.Version=GetReferenceUnifiedVersion data
    member x.Locale=data.assemRefLocale
    member x.UniqueStamp=uniqueStamp
    override x.GetHashCode() = uniqueStamp
    override x.Equals(yobj) = ((yobj :?> ILAssemblyRef).UniqueStamp = uniqueStamp)
    interface System.IComparable with
        override x.CompareTo(yobj) = compare (yobj :?> ILAssemblyRef).UniqueStamp uniqueStamp
    static member Create(name,hash,publicKey,retargetable,version,locale) =
        ILAssemblyRef
            { assemRefName=name;
              assemRefHash=hash;
              assemRefPublicKeyInfo=publicKey;
              assemRefRetargetable=retargetable;
              assemRefVersion=version;
              assemRefLocale=locale; }
              
    static member FromAssemblyName (aname:System.Reflection.AssemblyName) =
        let locale = None
        //match aname.CultureInfo with 
        //   | null -> None 
        //   | x -> Some x.Name
        let publicKey = 
           match aname.GetPublicKey()  with 
           | null | [| |] -> 
               match aname.GetPublicKeyToken()  with 
               | null | [| |] -> None
               | bytes -> Some (PublicKeyToken bytes)
           | bytes -> 
               Some (PublicKey bytes)
        
        let version = 
           match aname.Version with 
           | null -> None
           | v -> Some (uint16 v.Major,uint16 v.Minor,uint16 v.Build,uint16 v.Revision)
           
        let retargetable = aname.Flags = System.Reflection.AssemblyNameFlags.Retargetable

        ILAssemblyRef.Create(aname.Name,None,publicKey,retargetable,version,locale)
 


    member aref.QualifiedName = 
        let b = new System.Text.StringBuilder(100)
        let add (s:string) = (b.Append(s) |> ignore)
        let addC (s:char) = (b.Append(s) |> ignore)
        add(aref.Name);
        match aref.Version with 
        | None -> ()
        | Some (a,b,c,d) -> 
            add ", Version=";
            add (string (int a))
            add ".";
            add (string (int b))
            add ".";
            add (string (int c))
            add ".";
            add (string (int d))
            add ", Culture="
            match aref.Locale with 
            | None -> add "neutral"
            | Some b -> add b
            add ", PublicKeyToken="
            match aref.PublicKey with 
            | None -> add "null"
            | Some pki -> 
                  let pkt = pki.ToToken()
                  let convDigit(digit) = 
                      let digitc = 
                          if digit < 10 
                          then  System.Convert.ToInt32 '0' + digit 
                          else System.Convert.ToInt32 'a' + (digit - 10) 
                      System.Convert.ToChar(digitc)
                  for i = 0 to pkt.Length-1 do
                      let v = pkt.[i]
                      addC (convDigit(System.Convert.ToInt32(v)/16))
                      addC (convDigit(System.Convert.ToInt32(v)%16))
            // retargetable can be true only for system assemblies that definitely have Version
            if aref.Retargetable then
                add ", Retargetable=Yes" 
        b.ToString()


[<StructuralEquality; StructuralComparison>]
type ILModuleRef = 
    { name: string;
      hasMetadata: bool; 
      hash: byte[] option; }
    static member Create(name,hasMetadata,hash) = 
        { name=name;
          hasMetadata= hasMetadata;
          hash=hash }
    
    member x.Name=x.name
    member x.HasMetadata=x.hasMetadata
    member x.Hash=x.hash 

[<StructuralEquality; StructuralComparison>]
[<RequireQualifiedAccess>]
type ILScopeRef = 
    | Local
    | Module of ILModuleRef 
    | Assembly of ILAssemblyRef
    member x.IsLocalRef   = match x with ILScopeRef.Local      -> true | _ -> false
    member x.IsModuleRef  = match x with ILScopeRef.Module _   -> true | _ -> false
    member x.IsAssemblyRef= match x with ILScopeRef.Assembly _ -> true | _ -> false
    member x.ModuleRef    = match x with ILScopeRef.Module x   -> x | _ -> failwith "not a module reference"
    member x.AssemblyRef  = match x with ILScopeRef.Assembly x -> x | _ -> failwith "not an assembly reference"

    member scoref.QualifiedName = 
        match scoref with 
        | ILScopeRef.Local -> ""
        | ILScopeRef.Module mref -> "module "^mref.Name
        | ILScopeRef.Assembly aref when aref.Name = "mscorlib" -> ""
        | ILScopeRef.Assembly aref -> aref.QualifiedName

    member scoref.QualifiedNameWithNoShortPrimaryAssembly = 
        match scoref with 
        | ILScopeRef.Local -> ""
        | ILScopeRef.Module mref -> "module "+mref.Name
        | ILScopeRef.Assembly aref -> aref.QualifiedName

type ILArrayBound = int32 option 
type ILArrayBounds = ILArrayBound * ILArrayBound

[<StructuralEquality; StructuralComparison>]
type ILArrayShape = 
    | ILArrayShape of ILArrayBounds list (* lobound/size pairs *)
    member x.Rank = (let (ILArrayShape l) = x in l.Length)
    static member SingleDimensional = ILArrayShapeStatics.SingleDimensional
    static member FromRank n = if n = 1 then ILArrayShape.SingleDimensional else ILArrayShape(List.replicate n (Some 0,None))


and ILArrayShapeStatics() = 
    static let singleDimensional = ILArrayShape [(Some 0, None)]    
    static member SingleDimensional = singleDimensional

/// Calling conventions. These are used in method pointer types.
[<StructuralEquality; StructuralComparison; RequireQualifiedAccess>]
type ILArgConvention = 
    | Default
    | CDecl 
    | StdCall 
    | ThisCall 
    | FastCall 
    | VarArg
      
[<StructuralEquality; StructuralComparison; RequireQualifiedAccess>]
type ILThisConvention =
    | Instance
    | InstanceExplicit
    | Static

[<StructuralEquality; StructuralComparison>]
type ILCallingConv =
    | Callconv of ILThisConvention * ILArgConvention
    member x.ThisConv           = let (Callconv(a,_b)) = x in a
    member x.BasicConv          = let (Callconv(_a,b)) = x in b
    member x.IsInstance         = match x.ThisConv with ILThisConvention.Instance -> true | _ -> false
    member x.IsInstanceExplicit = match x.ThisConv with ILThisConvention.InstanceExplicit -> true | _ -> false
    member x.IsStatic           = match x.ThisConv with ILThisConvention.Static -> true | _ -> false

    static member Instance = ILCallingConvStatics.Instance
    static member Static = ILCallingConvStatics.Static

/// Static storage to amortize the allocation of <c>ILCallingConv.Instance</c> and <c>ILCallingConv.Static</c>.
and ILCallingConvStatics() = 
    static let instanceCallConv = Callconv(ILThisConvention.Instance,ILArgConvention.Default)
    static let staticCallConv =  Callconv(ILThisConvention.Static,ILArgConvention.Default)
    static member Instance = instanceCallConv
    static member Static = staticCallConv

type ILBoxity = 
  | AsObject 
  | AsValue


// IL type references have a pre-computed hash code to enable quick lookup tables during binary generation.
[<CustomEquality; CustomComparison>]
type ILTypeRef = 
    { trefScope: ILScopeRef;
      trefEnclosing: string list;
      trefName: string; 
      hashCode : int 
      mutable asBoxedType: ILType }
      
    static member Create(scope,enclosing,name) = 
        let hashCode = hash scope * 17 ^^^ (hash enclosing * 101 <<< 1) ^^^ (hash name * 47 <<< 2)
        { trefScope=scope;
          trefEnclosing= enclosing;
          trefName=name;
          hashCode=hashCode;
          asBoxedType = Unchecked.defaultof<_> }
          
    member x.Scope = x.trefScope
    member x.Enclosing = x.trefEnclosing
    member x.Name = x.trefName
    member x.ApproxId = x.hashCode

    member x.AsBoxedType (tspec:ILTypeSpec) = 
        if isNil tspec.tspecInst then
            let v = x.asBoxedType
            match box v with 
            | null -> 
               let r = ILType.Boxed tspec
               x.asBoxedType <- r
               r
            | _ -> v
        else 
            ILType.Boxed tspec

    override x.GetHashCode() = x.hashCode
    override x.Equals(yobj) = 
         let y = (yobj :?> ILTypeRef) 
         (x.ApproxId = y.ApproxId) && 
         (x.Scope = y.Scope) && 
         (x.Name = y.Name) && 
         (x.Enclosing = y.Enclosing)
    interface System.IComparable with
        override x.CompareTo(yobj) = 
            let y = (yobj :?> ILTypeRef) 
            let c = compare x.ApproxId y.ApproxId
            if c <> 0 then c else
            let c = compare x.Scope y.Scope
            if c <> 0 then c else
            let c = compare x.Name y.Name 
            if c <> 0 then c else
            compare x.Enclosing y.Enclosing
        
    member tref.FullName = String.concat "." (tref.Enclosing @ [tref.Name])
        
    member tref.BasicQualifiedName =
        (String.concat "+" (tref.Enclosing @ [ tref.Name ] )).Replace(",", @"\,")

    member tref.AddQualifiedNameExtensionWithNoShortPrimaryAssembly(basic) = 
        let sco = tref.Scope.QualifiedNameWithNoShortPrimaryAssembly
        if sco = "" then basic else String.concat ", " [basic;sco]

    member tref.QualifiedNameWithNoShortPrimaryAssembly = 
        tref.AddQualifiedNameExtensionWithNoShortPrimaryAssembly(tref.BasicQualifiedName)

    member tref.QualifiedName = 
        let basic = tref.BasicQualifiedName
        let sco = tref.Scope.QualifiedName
        if sco = "" then basic else String.concat ", " [basic;sco]


    override x.ToString() = x.FullName

        
and
    [<StructuralEquality; StructuralComparison>]
    ILTypeSpec = 
    { tspecTypeRef: ILTypeRef;    
      /// The type instantiation if the type is generic.
      tspecInst: ILGenericArgs }    
    member x.TypeRef=x.tspecTypeRef
    member x.Scope=x.TypeRef.Scope
    member x.Enclosing=x.TypeRef.Enclosing
    member x.Name=x.TypeRef.Name
    member x.GenericArgs=x.tspecInst
    static member Create(tref,inst) = { tspecTypeRef =tref; tspecInst=inst }
    override x.ToString() = x.TypeRef.ToString() + if isNil x.GenericArgs then "" else "<...>"
    member x.BasicQualifiedName = 
        let tc = x.TypeRef.BasicQualifiedName
        if isNil x.GenericArgs then
            tc
        else 
            tc + "[" + String.concat "," (x.GenericArgs |> List.map (fun arg -> "[" + arg.QualifiedNameWithNoShortPrimaryAssembly + "]")) + "]"

    member x.AddQualifiedNameExtensionWithNoShortPrimaryAssembly(basic) = 
        x.TypeRef.AddQualifiedNameExtensionWithNoShortPrimaryAssembly(basic)

    member x.FullName=x.TypeRef.FullName

and [<RequireQualifiedAccess; StructuralEquality; StructuralComparison>]
    ILType =
    | Void                   
    | Array    of ILArrayShape * ILType 
    | Value    of ILTypeSpec      
    | Boxed    of ILTypeSpec      
    | Ptr      of ILType             
    | Byref    of ILType           
    | FunctionPointer     of ILCallingSignature 
    | TypeVar    of uint16              
    | Modified of bool * ILTypeRef * ILType

    member x.BasicQualifiedName = 
        match x with 
        | ILType.TypeVar n -> "!" + string n
        | ILType.Modified(_,_ty1,ty2) -> ty2.BasicQualifiedName
        | ILType.Array (ILArrayShape(s),ty) -> ty.BasicQualifiedName + "[" + System.String(',',s.Length-1) + "]"
        | ILType.Value tr | ILType.Boxed tr -> tr.BasicQualifiedName
        | ILType.Void -> "void"
        | ILType.Ptr _ty -> failwith "unexpected pointer type"
        | ILType.Byref _ty -> failwith "unexpected byref type"
        | ILType.FunctionPointer _mref -> failwith "unexpected function pointer type"

    member x.AddQualifiedNameExtensionWithNoShortPrimaryAssembly(basic) = 
        match x with 
        | ILType.TypeVar _n -> basic
        | ILType.Modified(_,_ty1,ty2) -> ty2.AddQualifiedNameExtensionWithNoShortPrimaryAssembly(basic)
        | ILType.Array (ILArrayShape(_s),ty) -> ty.AddQualifiedNameExtensionWithNoShortPrimaryAssembly(basic)
        | ILType.Value tr | ILType.Boxed tr -> tr.AddQualifiedNameExtensionWithNoShortPrimaryAssembly(basic)
        | ILType.Void -> failwith "void"
        | ILType.Ptr _ty -> failwith "unexpected pointer type"
        | ILType.Byref _ty -> failwith "unexpected byref type"
        | ILType.FunctionPointer _mref -> failwith "unexpected function pointer type"
        
    member x.QualifiedNameWithNoShortPrimaryAssembly = 
        x.AddQualifiedNameExtensionWithNoShortPrimaryAssembly(x.BasicQualifiedName)
    member x.TypeSpec =
      match x with 
      | ILType.Boxed tr | ILType.Value tr -> tr
      | _ -> invalidOp "not a nominal type"
    member x.Boxity =
      match x with 
      | ILType.Boxed _ -> AsObject
      | ILType.Value _ -> AsValue
      | _ -> invalidOp "not a nominal type"
    member x.TypeRef = 
      match x with 
      | ILType.Boxed tspec | ILType.Value tspec -> tspec.TypeRef
      | _ -> invalidOp "not a nominal type"
    member x.IsNominal = 
      match x with 
      | ILType.Boxed _ | ILType.Value _ -> true
      | _ -> false
    member x.GenericArgs =
      match x with 
      | ILType.Boxed tspec | ILType.Value tspec -> tspec.GenericArgs
      | _ -> []
    member x.IsTyvar =
      match x with 
      | ILType.TypeVar _ -> true | _ -> false

and [<StructuralEquality; StructuralComparison>]
    ILCallingSignature = 
    { CallingConv: ILCallingConv;
      ArgTypes: ILTypes;
      ReturnType: ILType }

and ILGenericArgs = list<ILType>
and ILTypes = list<ILType>


let mkILCallSig (cc,args,ret) = { ArgTypes=args; CallingConv=cc; ReturnType=ret}
let mkILBoxedType (tspec:ILTypeSpec) = tspec.TypeRef.AsBoxedType tspec

type ILMethodRef =
    { mrefParent: ILTypeRef;
      mrefCallconv: ILCallingConv;
      mrefGenericArity: int; 
      mrefName: string;
      mrefArgs: ILTypes;
      mrefReturn: ILType }
    member x.EnclosingTypeRef = x.mrefParent
    member x.CallingConv = x.mrefCallconv
    member x.Name = x.mrefName
    member x.GenericArity = x.mrefGenericArity
    member x.ArgCount = List.length x.mrefArgs
    member x.ArgTypes = x.mrefArgs
    member x.ReturnType = x.mrefReturn

    member x.CallingSignature = mkILCallSig (x.CallingConv,x.ArgTypes,x.ReturnType)
    static member Create(a,b,c,d,e,f) = 
        { mrefParent= a;mrefCallconv=b;mrefName=c;mrefGenericArity=d; mrefArgs=e;mrefReturn=f }
    override x.ToString() = x.EnclosingTypeRef.ToString() + "::" + x.Name + "(...)"


[<StructuralEquality; StructuralComparison>]
type ILFieldRef = 
    { EnclosingTypeRef: ILTypeRef;
      Name: string;
      Type: ILType }
    override x.ToString() = x.EnclosingTypeRef.ToString() + "::" + x.Name

[<StructuralEquality; StructuralComparison>]
type ILMethodSpec = 
    { mspecMethodRef: ILMethodRef;
      mspecEnclosingType: ILType;          
      mspecMethodInst: ILGenericArgs; }     
    static member Create(a,b,c) = { mspecEnclosingType=a; mspecMethodRef =b; mspecMethodInst=c }
    member x.MethodRef = x.mspecMethodRef
    member x.EnclosingType=x.mspecEnclosingType
    member x.GenericArgs=x.mspecMethodInst
    member x.Name=x.MethodRef.Name
    member x.CallingConv=x.MethodRef.CallingConv
    member x.GenericArity = x.MethodRef.GenericArity
    member x.FormalArgTypes = x.MethodRef.ArgTypes
    member x.FormalReturnType = x.MethodRef.ReturnType
    override x.ToString() = x.MethodRef.ToString() + "(...)"


type ILFieldSpec =
    { FieldRef: ILFieldRef;
      EnclosingType: ILType }         
    member x.FormalType       = x.FieldRef.Type
    member x.Name             = x.FieldRef.Name
    member x.EnclosingTypeRef = x.FieldRef.EnclosingTypeRef
    override x.ToString() = x.FieldRef.ToString()


// --------------------------------------------------------------------
// Debug info.                                                     
// -------------------------------------------------------------------- 

type Guid =  byte[]

type ILPlatform = 
    | X86
    | AMD64
    | IA64

type ILSourceDocument = 
    { sourceLanguage: Guid option; 
      sourceVendor: Guid option;
      sourceDocType: Guid option;
      sourceFile: string; }
    static member Create(language,vendor,docType,file) =
        { sourceLanguage=language; 
          sourceVendor=vendor;
          sourceDocType=docType;
          sourceFile=file; }
    member x.Language=x.sourceLanguage
    member x.Vendor=x.sourceVendor
    member x.DocumentType=x.sourceDocType
    member x.File=x.sourceFile

type ILSourceMarker =
    { sourceDocument: ILSourceDocument;
      sourceLine: int;
      sourceColumn: int;
      sourceEndLine: int;
      sourceEndColumn: int }
    static member Create(document, line, column, endLine, endColumn) = 
        { sourceDocument=document;
          sourceLine=line;
          sourceColumn=column;
          sourceEndLine=endLine;
          sourceEndColumn=endColumn }
    member x.Document=x.sourceDocument
    member x.Line=x.sourceLine
    member x.Column=x.sourceColumn
    member x.EndLine=x.sourceEndLine
    member x.EndColumn=x.sourceEndColumn
    override x.ToString() = sprintf "(%d,%d)-(%d,%d)" x.Line x.Column x.EndLine x.EndColumn

// --------------------------------------------------------------------
// Custom attributes                                                     
// -------------------------------------------------------------------- 

type ILAttribElem =  
  | String of string  option
  | Bool of bool
  | Char of char
  | SByte of int8
  | Int16 of int16
  | Int32 of int32
  | Int64 of int64
  | Byte of uint8
  | UInt16 of uint16
  | UInt32 of uint32
  | UInt64 of uint64
  | Single of single
  | Double of double
  | Null 
  | Type of ILType option
  | TypeRef of ILTypeRef option
  | Array of ILType * ILAttribElem list

type ILAttributeNamedArg =  (string * ILType * bool * ILAttribElem)
type ILAttribute = 
    { Method: ILMethodSpec;
      Data: byte[] }

[<NoEquality; NoComparison; Sealed>]
type ILAttributes(f: unit -> ILAttribute[]) = 
   let mutable array = InlineDelayInit<_>(f)
   member x.AsArray = array.Value
   member x.AsList = x.AsArray |> Array.toList

type ILCodeLabel = int

// --------------------------------------------------------------------
// Instruction set.                                                     
// -------------------------------------------------------------------- 

type ILBasicType =
  | DT_R
  | DT_I1
  | DT_U1
  | DT_I2
  | DT_U2
  | DT_I4
  | DT_U4
  | DT_I8
  | DT_U8
  | DT_R4
  | DT_R8
  | DT_I
  | DT_U
  | DT_REF

[<StructuralEquality; StructuralComparison; RequireQualifiedAccess>]
type ILToken = 
  | ILType of ILType 
  | ILMethod of ILMethodSpec 
  | ILField of ILFieldSpec

[<StructuralEquality; StructuralComparison; RequireQualifiedAccess>]
type ILConst = 
  | I4 of int32
  | I8 of int64
  | R4 of single
  | R8 of double

type ILTailcall = 
  | Tailcall
  | Normalcall

type ILAlignment =  
  | Aligned
  | Unaligned1
  | Unaligned2
  | Unaligned4

type ILVolatility =  
  | Volatile
  | Nonvolatile

type ILReadonly =  
  | ReadonlyAddress
  | NormalAddress

type ILVarArgs = ILTypes option

[<StructuralEquality; StructuralComparison>]
type ILComparisonInstr = 
  | BI_beq        
  | BI_bge        
  | BI_bge_un     
  | BI_bgt        
  | BI_bgt_un        
  | BI_ble        
  | BI_ble_un        
  | BI_blt        
  | BI_blt_un 
  | BI_bne_un 
  | BI_brfalse 
  | BI_brtrue 


[<StructuralEquality; NoComparison>]
type ILInstr = 
  | AI_add    
  | AI_add_ovf
  | AI_add_ovf_un
  | AI_and    
  | AI_div   
  | AI_div_un
  | AI_ceq      
  | AI_cgt      
  | AI_cgt_un   
  | AI_clt     
  | AI_clt_un  
  | AI_conv      of ILBasicType
  | AI_conv_ovf  of ILBasicType
  | AI_conv_ovf_un  of ILBasicType
  | AI_mul       
  | AI_mul_ovf   
  | AI_mul_ovf_un
  | AI_rem       
  | AI_rem_un       
  | AI_shl       
  | AI_shr       
  | AI_shr_un
  | AI_sub       
  | AI_sub_ovf   
  | AI_sub_ovf_un   
  | AI_xor       
  | AI_or        
  | AI_neg       
  | AI_not       
  | AI_ldnull    
  | AI_dup       
  | AI_pop
  | AI_ckfinite 
  | AI_nop
  | AI_ldc of ILBasicType * ILConst 
  | I_ldarg     of uint16
  | I_ldarga    of uint16
  | I_ldind     of ILAlignment * ILVolatility * ILBasicType
  | I_ldloc     of uint16
  | I_ldloca    of uint16
  | I_starg     of uint16
  | I_stind     of  ILAlignment * ILVolatility * ILBasicType
  | I_stloc     of uint16

  | I_br    of  ILCodeLabel
  | I_jmp   of ILMethodSpec
  | I_brcmp of ILComparisonInstr * ILCodeLabel 
  | I_switch    of ILCodeLabel list  
  | I_ret 

  | I_call     of ILTailcall * ILMethodSpec * ILVarArgs
  | I_callvirt of ILTailcall * ILMethodSpec * ILVarArgs
  | I_callconstraint of ILTailcall * ILType * ILMethodSpec * ILVarArgs
  | I_calli    of ILTailcall * ILCallingSignature * ILVarArgs
  | I_ldftn    of ILMethodSpec
  | I_newobj  of ILMethodSpec  * ILVarArgs
  
  | I_throw
  | I_endfinally
  | I_endfilter
  | I_leave     of  ILCodeLabel
  | I_rethrow

  | I_ldsfld      of ILVolatility * ILFieldSpec
  | I_ldfld       of ILAlignment * ILVolatility * ILFieldSpec
  | I_ldsflda     of ILFieldSpec
  | I_ldflda      of ILFieldSpec 
  | I_stsfld      of ILVolatility  *  ILFieldSpec
  | I_stfld       of ILAlignment * ILVolatility * ILFieldSpec
  | I_ldstr       of string
  | I_isinst      of ILType
  | I_castclass   of ILType
  | I_ldtoken     of ILToken
  | I_ldvirtftn   of ILMethodSpec

  | I_cpobj       of ILType
  | I_initobj     of ILType
  | I_ldobj       of ILAlignment * ILVolatility * ILType
  | I_stobj       of ILAlignment * ILVolatility * ILType
  | I_box         of ILType
  | I_unbox       of ILType
  | I_unbox_any   of ILType
  | I_sizeof      of ILType

  | I_ldelem      of ILBasicType
  | I_stelem      of ILBasicType
  | I_ldelema     of ILReadonly * bool * ILArrayShape * ILType
  | I_ldelem_any  of ILArrayShape * ILType
  | I_stelem_any  of ILArrayShape * ILType
  | I_newarr      of ILArrayShape * ILType 
  | I_ldlen

  | I_mkrefany    of ILType
  | I_refanytype  
  | I_refanyval   of ILType

  | I_break 
  | I_seqpoint of ILSourceMarker

  | I_arglist  

  | I_localloc
  | I_cpblk of ILAlignment * ILVolatility
  | I_initblk of ILAlignment  * ILVolatility

  (* FOR EXTENSIONS, e.g. MS-ILX *)  
  | EI_ilzero of ILType
  | EI_ldlen_multi      of int32 * int32



[<RequireQualifiedAccess>]
type ILExceptionClause = 
    | Finally of (ILCodeLabel * ILCodeLabel)
    | Fault  of (ILCodeLabel * ILCodeLabel)
    | FilterCatch of (ILCodeLabel * ILCodeLabel) * (ILCodeLabel * ILCodeLabel)
    | TypeCatch of ILType * (ILCodeLabel * ILCodeLabel)

[<RequireQualifiedAccess; NoEquality; NoComparison>]
type ILExceptionSpec = 
    { Range: (ILCodeLabel * ILCodeLabel);
      Clause: ILExceptionClause }

/// Indicates that a particular local variable has a particular source 
/// language name within a given set of ranges. This does not effect local 
/// variable numbering, which is global over the whole method. 
[<RequireQualifiedAccess; NoEquality; NoComparison>]
type ILLocalDebugMapping =
    { LocalIndex: int;
      LocalName: string; }

[<RequireQualifiedAccess; NoEquality; NoComparison>]
type ILLocalDebugInfo = 
    { Range: (ILCodeLabel * ILCodeLabel);
      DebugMappings: ILLocalDebugMapping list }

[<RequireQualifiedAccess; NoEquality; NoComparison>]
type ILCode = 
    { Labels: Dictionary<ILCodeLabel,int> 
      Instrs:ILInstr[] 
      Exceptions: ILExceptionSpec list 
      Locals: ILLocalDebugInfo list }

[<RequireQualifiedAccess; NoComparison; NoEquality>]
type ILLocal = 
    { Type: ILType;
      IsPinned: bool;
      DebugInfo: (string * int * int) option }
      
type ILLocals = list<ILLocal>

[<RequireQualifiedAccess; NoEquality; NoComparison>]
type ILMethodBody = 
    { IsZeroInit: bool;
      MaxStack: int32;
      NoInlining: bool;
      Locals: ILLocals;
      Code:  ILCode;
      SourceMarker: ILSourceMarker option }

[<RequireQualifiedAccess>]
type ILMemberAccess = 
    | Assembly
    | CompilerControlled
    | FamilyAndAssembly
    | FamilyOrAssembly
    | Family
    | Private 
    | Public 

[<RequireQualifiedAccess; StructuralEquality; StructuralComparison>]
type ILFieldInit = 
    | String of string
    | Bool of bool
    | Char of uint16
    | Int8 of int8
    | Int16 of int16
    | Int32 of int32
    | Int64 of int64
    | UInt8 of uint8
    | UInt16 of uint16
    | UInt32 of uint32
    | UInt64 of uint64
    | Single of single
    | Double of double
    | Null
  
// -------------------------------------------------------------------- 
// Native Types, for marshalling to the native C interface.
// These are taken directly from the ILASM syntax, and don't really
// correspond yet to the ECMA Spec (Partition II, 7.4).  
// -------------------------------------------------------------------- 

[<RequireQualifiedAccess; StructuralEquality; StructuralComparison>]
type ILNativeType = 
    | Empty
    | Custom of Guid * string * string * byte[] (* guid,nativeTypeName,custMarshallerName,cookieString *)
    | FixedSysString of int32
    | FixedArray of int32
    | Currency
    | LPSTR
    | LPWSTR
    | LPTSTR
    | ByValStr
    | TBSTR
    | LPSTRUCT
    | Struct
    | Void
    | Bool
    | Int8
    | Int16
    | Int32
    | Int64
    | Single
    | Double
    | Byte
    | UInt16
    | UInt32
    | UInt64
    | Array of ILNativeType option * (int32 * int32 option) option (* optional idx of parameter giving size plus optional additive i.e. num elems *)
    | Int
    | UInt
    | Method
    | AsAny
    | BSTR
    | IUnknown
    | IDispatch
    | Interface
    | Error               
    | SafeArray of ILNativeVariant * string option 
    | ANSIBSTR
    | VariantBool


and 
  [<RequireQualifiedAccess; StructuralEquality; StructuralComparison>]
  ILNativeVariant = 
    | Empty
    | Null
    | Variant
    | Currency
    | Decimal               
    | Date               
    | BSTR               
    | LPSTR               
    | LPWSTR               
    | IUnknown               
    | IDispatch               
    | SafeArray               
    | Error               
    | HRESULT               
    | CArray               
    | UserDefined               
    | Record               
    | FileTime
    | Blob               
    | Stream               
    | Storage               
    | StreamedObject               
    | StoredObject               
    | BlobObject               
    | CF                
    | CLSID
    | Void 
    | Bool
    | Int8
    | Int16                
    | Int32                
    | Int64                
    | Single                
    | Double                
    | UInt8                
    | UInt16                
    | UInt32                
    | UInt64                
    | PTR                
    | Array of ILNativeVariant                
    | Vector of ILNativeVariant                
    | Byref of ILNativeVariant                
    | Int                
    | UInt                

type 
  [<RequireQualifiedAccess; StructuralEquality; StructuralComparison>]
  ILSecurityAction = 
    | Request 
    | Demand
    | Assert
    | Deny
    | PermitOnly
    | LinkCheck 
    | InheritCheck
    | ReqMin
    | ReqOpt
    | ReqRefuse
    | PreJitGrant
    | PreJitDeny
    | NonCasDemand
    | NonCasLinkDemand
    | NonCasInheritance
    | LinkDemandChoice
    | InheritanceDemandChoice
    | DemandChoice

[<RequireQualifiedAccess; StructuralEquality; StructuralComparison>]
type ILPermission = 
    | PermissionSet of ILSecurityAction * byte[]

[<RequireQualifiedAccess; NoEquality; NoComparison>]
type ILPermissions =
    | SecurityDecls of ILPermission list
    | SecurityDeclsLazy of Lazy<ILPermission list>
    member x.AsList = match x with SecurityDecls m -> m | SecurityDeclsLazy m -> m.Force()

[<RequireQualifiedAccess>]
type PInvokeCharBestFit  = 
    | UseAssembly
    | Enabled
    | Disabled

[<RequireQualifiedAccess>]
type PInvokeThrowOnUnmappableChar =
    | UseAssembly
    | Enabled
    | Disabled

[<RequireQualifiedAccess>]
type PInvokeCallingConvention =
    | None
    | Cdecl
    | Stdcall
    | Thiscall
    | Fastcall
    | WinApi

[<RequireQualifiedAccess>]
type PInvokeCharEncoding =
    | None
    | Ansi
    | Unicode
    | Auto

[<RequireQualifiedAccess; NoComparison; NoEquality>]
type PInvokeMethod =
    { Where: ILModuleRef;
      Name: string;
      CallingConv: PInvokeCallingConvention;
      CharEncoding: PInvokeCharEncoding;
      NoMangle: bool;
      LastError: bool;
      ThrowOnUnmappableChar: PInvokeThrowOnUnmappableChar;
      CharBestFit: PInvokeCharBestFit }

[<RequireQualifiedAccess; NoEquality; NoComparison>]
type ILParameter =
    { Name: string option;
      Type: ILType;
      Default: ILFieldInit option;  
      Marshal: ILNativeType option; 
      IsIn: bool;
      IsOut: bool;
      IsOptional: bool;
      CustomAttrs: ILAttributes }

type ILParameters = list<ILParameter>

[<RequireQualifiedAccess; NoEquality; NoComparison>]
type ILReturn = 
    { Marshal: ILNativeType option;
      Type: ILType; 
      CustomAttrs: ILAttributes }

type ILOverridesSpec = 
    | OverridesSpec of ILMethodRef * ILType
    member x.MethodRef = let (OverridesSpec(mr,_ty)) = x in mr
    member x.EnclosingType = let (OverridesSpec(_mr,ty)) = x in ty

type ILMethodVirtualInfo = 
    { IsFinal: bool
      IsNewSlot: bool 
      IsCheckAccessOnOverride: bool
      IsAbstract: bool }

type MethodKind =
    | Static 
    | Cctor 
    | Ctor 
    | NonVirtual 
    | Virtual of ILMethodVirtualInfo

[<RequireQualifiedAccess>]
type MethodBody =
    | IL of ILMethodBody
    | PInvoke of PInvokeMethod       (* platform invoke to native  *)
    | Abstract
    | Native

type ILLazyMethodBody = 
    | ILLazyMethodBody of Lazy<MethodBody >
    member x.Contents = let (ILLazyMethodBody mb) = x in mb.Force()

[<RequireQualifiedAccess>]
type MethodCodeKind =
    | IL
    | Native
    | Runtime

let mkMethBodyAux mb = ILLazyMethodBody (Lazy.CreateFromValue mb)
let mkMethBodyLazyAux mb = ILLazyMethodBody mb

let typesOfILParams (ps:ILParameters) : ILTypes = ps |> List.map (fun p -> p.Type)

[<StructuralEquality; StructuralComparison>]
type ILGenericVariance =
    | NonVariant
    | CoVariant
    | ContraVariant

type ILGenericParameterDef =
    { Name: string;
      Constraints: ILTypes;
      Variance: ILGenericVariance; 
      HasReferenceTypeConstraint: bool;
      CustomAttrs : ILAttributes;
      HasNotNullableValueTypeConstraint: bool;
      HasDefaultConstructorConstraint: bool; }

    override x.ToString() = x.Name 

type ILGenericParameterDefs = ILGenericParameterDef list

[<NoComparison; NoEquality>]
type ILMethodDef = 
    { Name: string;
      mdKind: MethodKind;
      CallingConv: ILCallingConv;
      Parameters: ILParameters;
      Return: ILReturn;
      Access: ILMemberAccess;
      mdBody: ILLazyMethodBody;   
      mdCodeKind: MethodCodeKind;   
      IsInternalCall: bool;
      IsManaged: bool;
      IsForwardRef: bool;
      SecurityDecls: ILPermissions;
      HasSecurity: bool;
      IsEntryPoint:bool;
      IsReqSecObj: bool;
      IsHideBySig: bool;
      IsSpecialName: bool;
      IsUnmanagedExport: bool;
      IsSynchronized: bool;
      IsPreserveSig: bool;
      IsMustRun: bool;
      IsNoInline: bool;
      GenericParams: ILGenericParameterDefs;
      CustomAttrs: ILAttributes; }
    member x.ParameterTypes = typesOfILParams x.Parameters
    // Whidbey feature: SafeHandle finalizer must be run 
    member md.Code = 
          match md.mdBody.Contents with 
          | MethodBody.IL il-> Some il.Code
          | _ -> None
    member x.IsIL = match x.mdBody.Contents with | MethodBody.IL _ -> true | _ -> false
    member x.Locals = match x.mdBody.Contents with | MethodBody.IL il -> il.Locals | _ -> []

    member x.MethodBody = match x.mdBody.Contents with MethodBody.IL il -> il | _ -> failwith "not IL"

    member x.SourceMarker = x.MethodBody.SourceMarker
    member x.MaxStack     = x.MethodBody.MaxStack  
    member x.IsZeroInit   = x.MethodBody.IsZeroInit

    member x.IsClassInitializer   = match x.mdKind with | MethodKind.Cctor      -> true | _ -> false
    member x.IsConstructor        = match x.mdKind with | MethodKind.Ctor       -> true | _ -> false
    member x.IsStatic             = match x.mdKind with | MethodKind.Static     -> true | _ -> false
    member x.IsNonVirtualInstance = match x.mdKind with | MethodKind.NonVirtual -> true | _ -> false
    member x.IsVirtual            = match x.mdKind with | MethodKind.Virtual _  -> true | _ -> false

    member x.IsFinal                = match x.mdKind with | MethodKind.Virtual v -> v.IsFinal    | _ -> invalidOp "not virtual"
    member x.IsNewSlot              = match x.mdKind with | MethodKind.Virtual v -> v.IsNewSlot  | _ -> invalidOp "not virtual"
    member x.IsCheckAccessOnOverride= match x.mdKind with | MethodKind.Virtual v -> v.IsCheckAccessOnOverride   | _ -> invalidOp "not virtual"
    member x.IsAbstract             = match x.mdKind with | MethodKind.Virtual v -> v.IsAbstract | _ -> invalidOp "not virtual"

    member md.CallingSignature =  mkILCallSig (md.CallingConv,md.ParameterTypes,md.Return.Type)


/// Index table by name and arity. 
type MethodDefMap = Map<string, ILMethodDef list>

[<Sealed>]
type ILMethodDefs(f : (unit -> ILMethodDef[])) = 

    let mutable array = InlineDelayInit<_>(f)
    let mutable dict = InlineDelayInit<_>(fun () -> 
            let arr = array.Value
            let t = Dictionary<_,_>()
            for i = arr.Length - 1 downto 0 do 
                let y = arr.[i]
                let key = y.Name
                if t.ContainsKey key then 
                    t.[key] <- y :: t.[key]
                else
                    t.[key] <- [ y ]
            t)

    interface IEnumerable with 
        member x.GetEnumerator() = ((x :> IEnumerable<ILMethodDef>).GetEnumerator() :> IEnumerator)

    interface IEnumerable<ILMethodDef> with 
        member x.GetEnumerator() = (array.Value :> IEnumerable<ILMethodDef>).GetEnumerator()

    member x.AsArray = array.Value
    member x.AsList = array.Value|> Array.toList
    member x.FindByName nm  =  if dict.Value.ContainsKey nm then dict.Value.[nm] else []
    member x.FindByNameAndArity (nm,arity) = x.FindByName nm |> List.filter (fun x -> List.length x.Parameters = arity)

[<NoComparison; NoEquality>]
type ILEventDef =
    { Type: ILType option; 
      Name: string;
      IsRTSpecialName: bool;
      IsSpecialName: bool;
      AddMethod: ILMethodRef; 
      RemoveMethod: ILMethodRef;
      FireMethod: ILMethodRef option;
      OtherMethods: ILMethodRef list;
      CustomAttrs: ILAttributes; }

(* Index table by name. *)
[<NoEquality; NoComparison>]
type ILEventDefs = 
    | Events of LazyOrderedMultiMap<string, ILEventDef>
    member x.AsList = let (Events t) = x in t.Entries()
    member x.LookupByName s = let (Events t) = x in t.[s]

[<NoComparison; NoEquality>]
type ILPropertyDef = 
    { Name: string;
      IsRTSpecialName: bool;
      IsSpecialName: bool;
      SetMethod: ILMethodRef option;
      GetMethod: ILMethodRef option;
      CallingConv: ILThisConvention;
      Type: ILType;
      Init: ILFieldInit option;
      Args: ILTypes;
      CustomAttrs: ILAttributes; }
    
// Index table by name.
[<NoEquality; NoComparison>]
type ILPropertyDefs = 
    | Properties of LazyOrderedMultiMap<string, ILPropertyDef>
    member x.AsList = let (Properties t) = x in t.Entries()
    member x.LookupByName s = let (Properties t) = x in t.[s]

[<NoComparison; NoEquality>]
type ILFieldDef = 
    { Name: string;
      Type: ILType;
      IsStatic: bool;
      Access: ILMemberAccess;
      Data:  byte[] option;
      LiteralValue:  ILFieldInit option;
      Offset:  int32 option; 
      IsSpecialName: bool;
      Marshal: ILNativeType option; 
      NotSerialized: bool;
      IsLiteral: bool ;
      IsInitOnly: bool;
      CustomAttrs: ILAttributes; }


// Index table by name.  Keep a canonical list to make sure field order is not disturbed for binary manipulation.
type ILFieldDefs = 
    | Fields of LazyOrderedMultiMap<string, ILFieldDef>
    member x.AsList = let (Fields t) = x in t.Entries()
    member x.LookupByName s = let (Fields t) = x in t.[s]

type ILMethodImplDef =
    { Overrides: ILOverridesSpec;
      OverrideBy: ILMethodSpec }

// Index table by name and arity. 
type ILMethodImplDefs = 
    | MethodImpls of Lazy<MethodImplsMap>
    member x.AsList = let (MethodImpls ltab) = x in Map.foldBack (fun _x y r -> y@r) (ltab.Force()) []

and MethodImplsMap = Map<string * int, ILMethodImplDef list>

[<RequireQualifiedAccess>]
type ILTypeDefLayout =
    | Auto
    | Sequential of ILTypeDefLayoutInfo
    | Explicit of ILTypeDefLayoutInfo (* REVIEW: add field info here *)

and ILTypeDefLayoutInfo =
    { Size: int32 option;
      Pack: uint16 option } 

[<RequireQualifiedAccess>]
type ILTypeInit =
    | BeforeField
    | OnAny

[<RequireQualifiedAccess>]
type ILDefaultPInvokeEncoding =
    | Ansi
    | Auto
    | Unicode

type ILTypeDefAccess =
    | Public 
    | Private
    | Nested of ILMemberAccess 

[<RequireQualifiedAccess>]
type ILTypeDefKind =
    | Class
    | ValueType
    | Interface
    | Enum 
    | Delegate


[<NoComparison; NoEquality>]
type ILTypeDef =  
    { tdKind: ILTypeDefKind;
      Name: string;  
      GenericParams: ILGenericParameterDefs;   (* class is generic *)
      Access: ILTypeDefAccess;  
      IsAbstract: bool;
      IsSealed: bool; 
      IsSerializable: bool; 
      IsComInterop: bool; (* Class or interface generated for COM interop *) 
      Layout: ILTypeDefLayout;
      IsSpecialName: bool;
      Encoding: ILDefaultPInvokeEncoding;
      NestedTypes: ILTypeDefs;
      Implements: ILTypes;  
      Extends: ILType option; 
      Methods: ILMethodDefs;
      SecurityDecls: ILPermissions;
      HasSecurity: bool;
      Fields: ILFieldDefs;
      MethodImpls: ILMethodImplDefs;
      InitSemantics: ILTypeInit;
      Events: ILEventDefs;
      Properties: ILPropertyDefs;
      CustomAttrs: ILAttributes; }
    member x.IsClass =     (match x.tdKind with ILTypeDefKind.Class -> true | _ -> false)
    member x.IsInterface = (match x.tdKind with ILTypeDefKind.Interface -> true | _ -> false)
    member x.IsEnum =      (match x.tdKind with ILTypeDefKind.Enum -> true | _ -> false)
    member x.IsDelegate =  (match x.tdKind with ILTypeDefKind.Delegate -> true | _ -> false)

    member tdef.IsStructOrEnum = 
        match tdef.tdKind with
        | ILTypeDefKind.ValueType | ILTypeDefKind.Enum -> true
        | _ -> false


and [<Sealed>] ILTypeDefs(f : unit -> (string list * string * ILAttributes * Lazy<ILTypeDef>)[]) =

    let mutable array = InlineDelayInit<_>(f)
    let mutable dict = InlineDelayInit<_>(fun () -> 
            let arr = array.Value
            let t = Dictionary<_,_>(HashIdentity.Structural)
            for (nsp, nm, _attr, ltd) in arr do 
                let key = nsp, nm
                t.[key] <- ltd
            t)

    member x.AsArray = [| for (_,_,_,ltd) in array.Value -> ltd.Force() |]
    member x.AsList = x.AsArray |> Array.toList

    interface IEnumerable with 
        member x.GetEnumerator() = ((x :> IEnumerable<ILTypeDef>).GetEnumerator() :> IEnumerator)

    interface IEnumerable<ILTypeDef> with 
        member x.GetEnumerator() = 
            (seq { for (_,_,_,ltd) in array.Value -> ltd.Force() }).GetEnumerator()
    
    member x.AsArrayOfLazyTypeDefs = array.Value

    member x.FindByName nm  = 
        let ns,n = splitILTypeName nm
        dict.Value.[(ns,n)].Force()

type ILNestedExportedType =
    { Name: string;
      Access: ILMemberAccess;
      Nested: ILNestedExportedTypes;
      CustomAttrs: ILAttributes } 

and ILNestedExportedTypes = 
    | ILNestedExportedTypes of Lazy<Map<string,ILNestedExportedType>>
    member x.AsList = let (ILNestedExportedTypes ltab) = x in Map.foldBack (fun _x y r -> y::r) (ltab.Force()) []

and [<NoComparison; NoEquality>]
    ILExportedTypeOrForwarder =
    { ScopeRef: ILScopeRef;
      Name: string;
      IsForwarder: bool;
      Access: ILTypeDefAccess;
      Nested: ILNestedExportedTypes;
      CustomAttrs: ILAttributes } 

and ILExportedTypesAndForwarders = 
    | ILExportedTypesAndForwarders of Lazy<Map<string,ILExportedTypeOrForwarder>>
    member x.AsList = let (ILExportedTypesAndForwarders ltab) = x in Map.foldBack (fun _x y r -> y::r) (ltab.Force()) []

[<RequireQualifiedAccess>]
type ILResourceAccess = 
    | Public 
    | Private 

[<RequireQualifiedAccess>]
type ILResourceLocation =
    | Local of (unit -> byte[])
    | File of ILModuleRef * int32
    | Assembly of ILAssemblyRef

type ILResource =
    { Name: string;
      Location: ILResourceLocation;
      Access: ILResourceAccess;
      CustomAttrs: ILAttributes }
    /// Read the bytes from a resource local to an assembly
    member r.Bytes = 
        match r.Location with 
        | ILResourceLocation.Local b -> b()
        | _ -> failwith "Bytes"

type ILResources = 
    | ILResources of Lazy<ILResource list>
    member x.AsList = let (ILResources ltab) = x in (ltab.Force())

// -------------------------------------------------------------------- 
// One module in the "current" assembly
// -------------------------------------------------------------------- 

[<RequireQualifiedAccess>]
type ILAssemblyLongevity =
    | Unspecified
    | Library
    | PlatformAppDomain
    | PlatformProcess
    | PlatformSystem


type ILAssemblyManifest = 
    { Name: string;
      AuxModuleHashAlgorithm: int32;
      SecurityDecls: ILPermissions;
      PublicKey: byte[] option;
      Version: ILVersionInfo option;
      Locale: Locale option;
      CustomAttrs: ILAttributes;

      AssemblyLongevity: ILAssemblyLongevity; 
      DisableJitOptimizations: bool;
      JitTracking: bool;
      IgnoreSymbolStoreSequencePoints: bool;
      Retargetable: bool;

      /// Records the types implemented by other modules. 
      ExportedTypes: ILExportedTypesAndForwarders;
      /// Records whether the entrypoint resides in another module. 
      EntrypointElsewhere: ILModuleRef option; 
    } 

type ILModuleDef = 
    { Manifest: ILAssemblyManifest option;
      CustomAttrs: ILAttributes;
      Name: string;
      TypeDefs: ILTypeDefs;
      SubsystemVersion : int * int
      UseHighEntropyVA : bool
      (* Random bits of relatively uninteresting data *)
      SubSystemFlags: int32;
      IsDLL: bool;
      IsILOnly: bool;
      Platform: ILPlatform option; 
      StackReserveSize: int32 option;
      Is32Bit: bool;
      Is32BitPreferred: bool;
      Is64Bit: bool;
      VirtualAlignment: int32;
      PhysicalAlignment: int32;
      ImageBase: int32;
      MetadataVersion: string;
      Resources: ILResources;
      NativeResources: list<Lazy<byte[]>>; (* e.g. win32 resources *)
    }
    member x.ManifestOfAssembly = 
        match x.Manifest with 
        | Some m -> m
        | None -> failwith "no manifest.  It is possible you are using an auxiliary module of an assembly in a context where the main module of an assembly is expected.  Typically the main module of an assembly must be specified first within a list of the modules in an assembly."

    member m.HasManifest = 
        match m.Manifest with None -> false | _ -> true


// -------------------------------------------------------------------- 
// Add fields and types to tables, with decent error messages
// when clashes occur...
// -------------------------------------------------------------------- 

let mkILEmptyGenericParams = ([]: ILGenericParameterDefs)
let emptyILGenericArgsList = ([ ]: ILType list)




// --------------------------------------------------------------------
// Make ILTypeRefs etc.
// -------------------------------------------------------------------- 

let mkILNestedTyRef (scope,l,nm) =  ILTypeRef.Create(scope,l,nm)
let mkILTyRef (scope,nm) =  mkILNestedTyRef (scope,[],nm)

type ILGenericArgsList = ILType list

let mkILTySpec (tref,inst) =  ILTypeSpec.Create(tref, inst)

let mkILNonGenericTySpec tref =  mkILTySpec (tref,[])

let mkILTyRefInTyRef (tref:ILTypeRef,nm) = 
    mkILNestedTyRef (tref.Scope,tref.Enclosing@[tref.Name],nm)

let mkILTy boxed tspec = 
  match boxed with AsObject -> mkILBoxedType tspec | _ -> ILType.Value tspec

let mkILNamedTy vc tref tinst = mkILTy vc (ILTypeSpec.Create(tref, tinst))

let mkILValueTy tref tinst = mkILNamedTy AsValue tref tinst
let mkILBoxedTy tref tinst = mkILNamedTy AsObject tref tinst

let mkILNonGenericValueTy tref = mkILNamedTy AsValue tref []
let mkILNonGenericBoxedTy tref = mkILNamedTy AsObject tref []

// -------------------------------------------------------------------- 
// Making assembly, module and file references
// -------------------------------------------------------------------- 

let mkSimpleAssRef n = 
  ILAssemblyRef.Create(n, None, None, false, None, None)

let mkSimpleModRef n = 
    ILModuleRef.Create(n, true, None)

// --------------------------------------------------------------------
// The toplevel class of a module is called "<Module>"
//
// REVIEW: the  following comments from the ECMA Spec (Partition II, Section 9.8)
//
// "For an ordinary type, if the metadata merges two definitions 
// of the same type, it simply discards one definition on the 
// assumption they are equivalent and that any anomaly will be 
// discovered when the type is used.  For the special class that 
// holds global members, however, members are unioned across all 
// modules at merge time. If the same name appears to be defined 
// for cross-module use in multiple modules then there is an 
// error.  In detail:
//  - If no member of the same kind (field or method), name, and 
//    signature exists, then add this member to the output class.
//  - If there are duplicates and no more than one has an 
//    accessibility other than compilercontrolled, then add them 
//    all in the output class.
//  - If there are duplicates and two or more have an accessibility 
//    other than compilercontrolled an error has occurred."
// -------------------------------------------------------------------- 

let typeNameForGlobalFunctions = "<Module>"

let mkILTypeForGlobalFunctions scoref = mkILBoxedType (mkILNonGenericTySpec (ILTypeRef.Create(scoref,[],typeNameForGlobalFunctions)))

let isTypeNameForGlobalFunctions d = (d = typeNameForGlobalFunctions)


let mkILMethRef (tref,callconv,nm,gparams,args,rty) =
    { mrefParent=tref; 
      mrefCallconv=callconv;
      mrefGenericArity=gparams;
      mrefName=nm;
      mrefArgs=args;
      mrefReturn=rty}

let mkILMethSpecForMethRefInTy (mref,typ,minst) = 
    { mspecMethodRef=mref;
      mspecEnclosingType=typ;
      mspecMethodInst=minst }

let mkILMethSpec (mref, vc, tinst, minst) = mkILMethSpecForMethRefInTy (mref,mkILNamedTy vc mref.EnclosingTypeRef tinst, minst)

let mk_mspec_in_tref (tref,vc,cc,nm,args,rty,tinst,minst) =
  mkILMethSpec (mkILMethRef ( tref,cc,nm,List.length minst,args,rty),vc,tinst,minst)

let mkILMethSpecInTy (typ:ILType, cc, nm, args, rty, minst:ILGenericArgs) =
  mkILMethSpecForMethRefInTy (mkILMethRef (typ.TypeRef,cc,nm,minst.Length,args,rty),typ,minst)

let mkILNonGenericMethSpecInTy (typ,cc,nm,args,rty) = 
  mkILMethSpecInTy (typ,cc,nm,args,rty,[])

let mkILInstanceMethSpecInTy (typ:ILType,nm,args,rty,minst) =
  mkILMethSpecInTy (typ, ILCallingConv.Instance, nm, args, rty, minst)

let mkILNonGenericInstanceMethSpecInTy (typ:ILType,nm,args,rty) =
  mkILInstanceMethSpecInTy (typ,nm,args,rty,[])

let mkILStaticMethSpecInTy (typ,nm,args,rty,minst) =
  mkILMethSpecInTy (typ,ILCallingConv.Static,nm,args,rty,minst)

let mkILNonGenericStaticMethSpecInTy (typ,nm,args,rty) =
  mkILStaticMethSpecInTy (typ,nm,args,rty,[])

let mkILCtorMethSpec (tref,args,cinst) = 
  mk_mspec_in_tref(tref,AsObject,ILCallingConv.Instance,".ctor",args,ILType.Void,cinst, [])

let mkILCtorMethSpecForTy (ty,args) = 
  mkILMethSpecInTy(ty,ILCallingConv.Instance,".ctor",args,ILType.Void, [])

let mkILNonGenericCtorMethSpec (tref,args) = 
  mkILCtorMethSpec (tref,args,[])

// --------------------------------------------------------------------
// Make references to fields
// -------------------------------------------------------------------- 

let mkILFieldRef(tref,nm,ty) = { EnclosingTypeRef=tref; Name=nm; Type=ty}

let mkILFieldSpec (tref,ty) = { FieldRef= tref; EnclosingType=ty }

let mkILFieldSpecInTy (typ:ILType,nm,fty) = 
    mkILFieldSpec (mkILFieldRef (typ.TypeRef,nm,fty), typ)
    
let emptyILCustomAttrs = ILAttributes (fun () -> [| |])

let mkILCustomAttrsFromArray (l: ILAttribute[]) = if l.Length = 0 then emptyILCustomAttrs else ILAttributes (fun () -> l)
let mkILCustomAttrs l = l |> List.toArray |> mkILCustomAttrsFromArray
let mkILComputedCustomAttrs f = ILAttributes f

let andTailness x y = 
  match x with Tailcall when y -> Tailcall | _ -> Normalcall

// -------------------------------------------------------------------- 
// Basic operations on code.
// -------------------------------------------------------------------- 

let formatCodeLabel (x:int) = "L"+string x

//  ++GLOBAL MUTABLE STATE (concurrency safe)
let codeLabelCount = ref 0
let generateCodeLabel() = System.Threading.Interlocked.Increment(codeLabelCount)

let instrIsRet i = 
    match i with 
    | I_ret ->  true
    | _ -> false

let nonBranchingInstrsToCode instrs : ILCode = 
    let instrs = Array.ofList instrs
    let instrs = 
        if instrs.Length <> 0 && instrIsRet (Array.last instrs) then instrs
        else Array.append instrs  [| I_ret |]

    { Labels = Dictionary()
      Instrs = instrs
      Exceptions = []
      Locals = [] }


// -------------------------------------------------------------------- 
// 
// -------------------------------------------------------------------- 

let emptyILSecurityDecls = ILPermissions.SecurityDecls []
let mkILSecurityDecls l = match l with [] -> emptyILSecurityDecls | _ -> ILPermissions.SecurityDecls l
let mkILLazySecurityDecls l = ILPermissions.SecurityDeclsLazy l

let mkILTyvarTy tv = ILType.TypeVar tv

let mkILSimpleTypar nm =
   { Name=nm;
     Constraints = []
     Variance=NonVariant;
     HasReferenceTypeConstraint=false;
     HasNotNullableValueTypeConstraint=false;
     HasDefaultConstructorConstraint=false; 
     CustomAttrs = emptyILCustomAttrs }

let gparam_of_gactual (_ga:ILType) = mkILSimpleTypar "T"

let mkILFormalTypars (x: ILGenericArgsList) = List.map gparam_of_gactual x

let mkILFormalGenericArgs numtypars (gparams:ILGenericParameterDefs)  =
    List.mapi (fun n _gf -> mkILTyvarTy (uint16 (numtypars + n))) gparams
 
let mkILFormalBoxedTy tref gparams = mkILBoxedTy tref (mkILFormalGenericArgs 0 gparams)

let mkILFormalNamedTy bx tref gparams = mkILNamedTy bx tref (mkILFormalGenericArgs 0 gparams)

// -------------------------------------------------------------------- 
// Operations on class etc. defs.
// -------------------------------------------------------------------- 

let mkRefForNestedILTypeDef scope (enc:ILTypeDef list,td:ILTypeDef)  = 
    mkILNestedTyRef(scope, (enc |> List.map (fun etd -> etd.Name)), td.Name)

// -------------------------------------------------------------------- 
// Operations on type tables.
// -------------------------------------------------------------------- 

let getName (ltd: Lazy<ILTypeDef>) = 
    let td = Lazy.force ltd
    let ns,n = splitILTypeName td.Name
    (ns,n,td.CustomAttrs,ltd)

let addILTypeDef td (tdefs: ILTypeDefs) = ILTypeDefs (fun () -> [| yield getName (notlazy td); yield! tdefs.AsArrayOfLazyTypeDefs |])
let mkILTypeDefsFromArray l =  ILTypeDefs (fun () -> Array.map (notlazy >> getName) l)
let mkILTypeDefs l =  mkILTypeDefsFromArray (Array.ofList l)
let mkILTypeDefsComputed f = ILTypeDefs f
let emptyILTypeDefs = mkILTypeDefsFromArray [| |]

// -------------------------------------------------------------------- 
// Operations on method tables.
// -------------------------------------------------------------------- 

let mkILMethodsFromArray xs =  ILMethodDefs (fun () -> xs)
let mkILMethods xs =  xs |> Array.ofList |> mkILMethodsFromArray
let mkILMethodsComputed f =  ILMethodDefs f
let emptyILMethods = mkILMethodsFromArray [| |]

let filterILMethodDefs f (mdefs: ILMethodDefs) = 
    ILMethodDefs (fun () -> mdefs.AsArray |> Array.filter f)

// -------------------------------------------------------------------- 
// Operations and defaults for modules, assemblies etc.
// -------------------------------------------------------------------- 

let defaultSubSystem = 3 (* this is what comes out of ILDASM on 30/04/2001 *)
let defaultPhysAlignment = 512 (* this is what comes out of ILDASM on 30/04/2001 *)
let defaultVirtAlignment = 0x2000 (* this is what comes out of ILDASM on 30/04/2001 *)
let defaultImageBase = 0x034f0000 (* this is what comes out of ILDASM on 30/04/2001 *)

// -------------------------------------------------------------------- 
// Array types
// -------------------------------------------------------------------- 

let mkILArrTy (ty, shape) = ILType.Array(shape,ty)
let mkILArr1DTy ty = mkILArrTy (ty,ILArrayShape.SingleDimensional)
let isILArrTy ty = match ty with ILType.Array _ -> true| _ -> false
let destILArrTy ty = match ty with ILType.Array(shape,ty) -> (shape,ty) | _ -> failwith "destILArrTy"

// -------------------------------------------------------------------- 
// Sigs of special types built-in
// -------------------------------------------------------------------- 

[<Literal>]
let tname_Object = "System.Object"
[<Literal>]
let tname_String = "System.String"
[<Literal>]
let tname_Array = "System.Array"
[<Literal>]
let tname_Type = "System.Type"
[<Literal>]
let tname_Int64 = "System.Int64"
[<Literal>]
let tname_UInt64 = "System.UInt64"
[<Literal>]
let tname_Int32 = "System.Int32"
[<Literal>]
let tname_UInt32 = "System.UInt32"
[<Literal>]
let tname_Int16 = "System.Int16"
[<Literal>]
let tname_UInt16 = "System.UInt16"
[<Literal>]
let tname_SByte = "System.SByte"
[<Literal>]
let tname_Byte = "System.Byte"
[<Literal>]
let tname_Single = "System.Single"
[<Literal>]
let tname_Double = "System.Double"
[<Literal>]
let tname_Bool = "System.Boolean"
[<Literal>]
let tname_Char = "System.Char"
[<Literal>]
let tname_IntPtr = "System.IntPtr"
[<Literal>]
let tname_UIntPtr = "System.UIntPtr"

[<NoEquality; NoComparison>]
// This data structure needs an entirely delayed implementation
type ILGlobals(primaryScopeRef) = 
    
    let m_mkSysILTypeRef nm = mkILTyRef(primaryScopeRef, nm)

    let m_typ_Object = mkILBoxedType (mkILNonGenericTySpec (m_mkSysILTypeRef tname_Object))
    let m_typ_String = mkILBoxedType (mkILNonGenericTySpec (m_mkSysILTypeRef tname_String))
    let m_typ_Array = mkILBoxedType (mkILNonGenericTySpec (m_mkSysILTypeRef tname_Array))
    let m_typ_Type = mkILBoxedType (mkILNonGenericTySpec (m_mkSysILTypeRef tname_Type))
    let m_typ_SByte = ILType.Value (mkILNonGenericTySpec (m_mkSysILTypeRef tname_SByte))
    let m_typ_Int16 = ILType.Value (mkILNonGenericTySpec (m_mkSysILTypeRef tname_Int16))
    let m_typ_Int32 = ILType.Value (mkILNonGenericTySpec (m_mkSysILTypeRef tname_Int32))
    let m_typ_Int64 = ILType.Value (mkILNonGenericTySpec (m_mkSysILTypeRef tname_Int64))
    let m_typ_Byte = ILType.Value (mkILNonGenericTySpec (m_mkSysILTypeRef tname_Byte))
    let m_typ_UInt16 = ILType.Value (mkILNonGenericTySpec (m_mkSysILTypeRef tname_UInt16))
    let m_typ_UInt32 = ILType.Value (mkILNonGenericTySpec (m_mkSysILTypeRef tname_UInt32))
    let m_typ_UInt64 = ILType.Value (mkILNonGenericTySpec (m_mkSysILTypeRef tname_UInt64))
    let m_typ_Single = ILType.Value (mkILNonGenericTySpec (m_mkSysILTypeRef tname_Single))
    let m_typ_Double = ILType.Value (mkILNonGenericTySpec (m_mkSysILTypeRef tname_Double))
    let m_typ_Bool = ILType.Value (mkILNonGenericTySpec (m_mkSysILTypeRef tname_Bool))
    let m_typ_Char = ILType.Value (mkILNonGenericTySpec (m_mkSysILTypeRef tname_Char))
    let m_typ_IntPtr = ILType.Value (mkILNonGenericTySpec (m_mkSysILTypeRef tname_IntPtr))
    let m_typ_UIntPtr = ILType.Value (mkILNonGenericTySpec (m_mkSysILTypeRef tname_UIntPtr))

    member x.primaryAssemblyScopeRef    = m_typ_Object.TypeRef.Scope
    member x.primaryAssemblyName        = m_typ_Object.TypeRef.Scope.AssemblyRef.Name
    member x.typ_Object                 = m_typ_Object
    member x.typ_String                 = m_typ_String
    member x.typ_Array                  = m_typ_Array
    member x.typ_Type                   = m_typ_Type
    member x.typ_IntPtr                 = m_typ_IntPtr                    
    member x.typ_UIntPtr                = m_typ_UIntPtr
    member x.typ_Byte                   = m_typ_Byte                 
    member x.typ_Int16                  = m_typ_Int16
    member x.typ_Int32                  = m_typ_Int32
    member x.typ_Int64                  = m_typ_Int64
    member x.typ_SByte                  = m_typ_SByte
    member x.typ_UInt16                 = m_typ_UInt16
    member x.typ_UInt32                 = m_typ_UInt32
    member x.typ_UInt64                 = m_typ_UInt64
    member x.typ_Single                 = m_typ_Single
    member x.typ_Double                 = m_typ_Double
    member x.typ_Bool                   = m_typ_Bool
    member x.typ_Char                   = m_typ_Char
    override x.ToString() = "<ILGlobals>"

let mkILGlobals primaryScopeRef = ILGlobals primaryScopeRef

let mkNormalCall mspec = I_call (Normalcall, mspec, None)
let mkNormalCallvirt mspec = I_callvirt (Normalcall, mspec, None)
let mkNormalCallconstraint (ty,mspec) = I_callconstraint (Normalcall, ty, mspec, None)
let mkNormalNewobj mspec =  I_newobj (mspec, None)

/// Comment on common object cache sizes:
/// mkLdArg - I can't imagine any IL method we generate needing more than this
/// mkLdLoc - I tried 256, and there were LdLoc allocations left, so I upped it o 512. I didn't check again.
/// mkStLoc - it should be the same as LdLoc  (where there's a LdLoc there must be a StLoc)
/// mkLdcInt32 - just a guess

let ldargs = [| for i in 0 .. 128 -> I_ldarg (uint16 i) |]
let mkLdarg i = if 0us < i && i < uint16 ldargs.Length then ldargs.[int i] else I_ldarg i
let mkLdarg0 = mkLdarg 0us

let ldlocs = [| for i in 0 .. 512 -> I_ldloc (uint16 i) |]
let mkLdloc i = if 0us < i && i < uint16 ldlocs.Length then ldlocs.[int i] else I_ldloc i

let stlocs = [| for i in 0 .. 512 -> I_stloc (uint16 i) |]
let mkStloc i = if 0us < i && i < uint16 stlocs.Length then stlocs.[int i] else I_stloc i

let ldi32s = [| for i in 0 .. 256 -> AI_ldc (DT_I4,ILConst.I4 i) |]
let mkLdcInt32 i = if 0 < i && i < ldi32s.Length then ldi32s.[i] else AI_ldc (DT_I4,ILConst.I4 i)

let tname_CompilerGeneratedAttribute = "System.Runtime.CompilerServices.CompilerGeneratedAttribute"
let tname_DebuggableAttribute = "System.Diagnostics.DebuggableAttribute"



        
(* NOTE: ecma_ prefix refers to the standard "mscorlib" *)
let ecmaPublicKey = PublicKeyToken (Bytes.ofInt32Array [|0xde; 0xad; 0xbe; 0xef; 0xca; 0xfe; 0xfa; 0xce |]) 
   
let isILBoxedTy = function ILType.Boxed _ -> true | _ -> false
let isILValueTy = function ILType.Value _ -> true | _ -> false


let isPrimaryAssemblyTySpec (tspec:ILTypeSpec) n = 
  let tref = tspec.TypeRef
  let scoref = tref.Scope
  (tref.Name = n) &&
  match scoref with
  | ILScopeRef.Assembly n -> PrimaryAssembly.IsSomePrimaryAssembly n.Name
  | ILScopeRef.Module _ -> false
  | ILScopeRef.Local -> true

let isILBoxedPrimaryAssemblyTy (ty:ILType) n = 
  isILBoxedTy ty && isPrimaryAssemblyTySpec ty.TypeSpec n

let isILValuePrimaryAssemblyTy (ty:ILType) n = 
  isILValueTy ty && isPrimaryAssemblyTySpec ty.TypeSpec n
      
let isILObjectTy            ty = isILBoxedPrimaryAssemblyTy ty tname_Object
let isILStringTy            ty = isILBoxedPrimaryAssemblyTy ty tname_String
let isILTypedReferenceTy    ty = isILValuePrimaryAssemblyTy ty "System.TypedReference"
let isILSByteTy        ty = isILValuePrimaryAssemblyTy ty tname_SByte
let isILByteTy         ty = isILValuePrimaryAssemblyTy ty tname_Byte
let isILInt16Ty        ty = isILValuePrimaryAssemblyTy ty tname_Int16
let isILUInt16Ty       ty = isILValuePrimaryAssemblyTy ty tname_UInt16
let isILInt32Ty        ty = isILValuePrimaryAssemblyTy ty tname_Int32
let isILUInt32Ty       ty = isILValuePrimaryAssemblyTy ty tname_UInt32
let isILInt64Ty        ty = isILValuePrimaryAssemblyTy ty tname_Int64
let isILUInt64Ty       ty = isILValuePrimaryAssemblyTy ty tname_UInt64
let isILIntPtrTy       ty = isILValuePrimaryAssemblyTy ty tname_IntPtr
let isILUIntPtrTy      ty = isILValuePrimaryAssemblyTy ty tname_UIntPtr
let isILBoolTy         ty = isILValuePrimaryAssemblyTy ty tname_Bool
let isILCharTy         ty = isILValuePrimaryAssemblyTy ty tname_Char
let isILSingleTy       ty = isILValuePrimaryAssemblyTy ty tname_Single
let isILDoubleTy       ty = isILValuePrimaryAssemblyTy ty tname_Double

// -------------------------------------------------------------------- 
// Rescoping
// -------------------------------------------------------------------- 

let rescopeILScopeRef scoref scoref1 = 
    match scoref,scoref1 with 
    | _,ILScopeRef.Local -> scoref 
    | ILScopeRef.Local,_ -> scoref1
    | _,ILScopeRef.Module _ -> scoref
    | ILScopeRef.Module _,_ -> scoref1
    | _ -> scoref1

let rescopeILTypeRef scoref (tref1:ILTypeRef) = 
    let scoref1 = tref1.Scope 
    let scoref2 = rescopeILScopeRef scoref scoref1
    if scoref1 === scoref2 then tref1
    else ILTypeRef.Create(scoref2,tref1.Enclosing,tref1.Name)

// ORIGINAL IMPLEMENTATION (too many allocations
//         { tspecTypeRef=rescopeILTypeRef scoref tref;
//           tspecInst=rescopeILTypes scoref tinst } 
let rec rescopeILTypeSpec scoref (tspec1:ILTypeSpec) = 
    let tref1 = tspec1.TypeRef
    let tinst1 = tspec1.GenericArgs
    let tref2 = rescopeILTypeRef scoref tref1

    // avoid reallocation in the common case 
    if tref1 === tref2 then 
        if isNil tinst1 then tspec1 else
        let tinst2 = rescopeILTypes scoref tinst1
        if tinst1 === tinst2 then tspec1 else 
        ILTypeSpec.Create (tref2, tinst2)
    else
        let tinst2 = rescopeILTypes scoref tinst1
        ILTypeSpec.Create (tref2, tinst2)

and rescopeILType scoref typ = 
    match typ with 
    | ILType.Ptr t -> ILType.Ptr (rescopeILType scoref t)
    | ILType.FunctionPointer t -> ILType.FunctionPointer (rescopeILCallSig scoref t)
    | ILType.Byref t -> ILType.Byref (rescopeILType scoref t)
    | ILType.Boxed cr1 -> 
        let cr2 = rescopeILTypeSpec scoref cr1
        if cr1 === cr2 then typ else 
        mkILBoxedType cr2
    | ILType.Array (s,ety1) -> 
        let ety2 = rescopeILType scoref ety1
        if ety1 === ety2 then typ else 
        ILType.Array (s,ety2)
    | ILType.Value cr1 -> 
        let cr2 = rescopeILTypeSpec scoref cr1 
        if cr1 === cr2 then typ else 
        ILType.Value cr2
    | ILType.Modified(b,tref,ty) -> ILType.Modified(b,rescopeILTypeRef scoref tref, rescopeILType scoref ty)
    | x -> x

and rescopeILTypes scoref i = 
    if isNil i then i
    else List.mapq (rescopeILType scoref) i

and rescopeILCallSig scoref  csig = 
    mkILCallSig (csig.CallingConv,rescopeILTypes scoref csig.ArgTypes,rescopeILType scoref csig.ReturnType)

let rescopeILMethodRef scoref (x:ILMethodRef) =
    { mrefParent = rescopeILTypeRef scoref x.EnclosingTypeRef;
      mrefCallconv = x.mrefCallconv;
      mrefGenericArity=x.mrefGenericArity;
      mrefName=x.mrefName;
      mrefArgs = rescopeILTypes scoref x.mrefArgs;
      mrefReturn= rescopeILType scoref x.mrefReturn }

let rescopeILFieldRef scoref x = 
    { EnclosingTypeRef = rescopeILTypeRef scoref x.EnclosingTypeRef;
      Name= x.Name;
      Type= rescopeILType scoref x.Type }

// -------------------------------------------------------------------- 
// Instantiate polymorphism in types
// -------------------------------------------------------------------- 

let rec instILTypeSpecAux numFree inst (tspec:ILTypeSpec) = 
    ILTypeSpec.Create(tspec.TypeRef,instILGenericArgsAux numFree inst tspec.GenericArgs) 
  
and instILTypeAux numFree (inst:ILGenericArgs) typ = 
    match typ with 
    | ILType.Ptr t       -> ILType.Ptr (instILTypeAux numFree inst t)
    | ILType.FunctionPointer t      -> ILType.FunctionPointer (instILCallSigAux numFree inst t)
    | ILType.Array (a,t) -> ILType.Array (a,instILTypeAux numFree inst t)
    | ILType.Byref t     -> ILType.Byref (instILTypeAux numFree inst t)
    | ILType.Boxed cr    -> mkILBoxedType (instILTypeSpecAux numFree inst cr)
    | ILType.Value cr    -> ILType.Value (instILTypeSpecAux numFree inst cr)
    | ILType.TypeVar  v -> 
        let v = int v
        let top = inst.Length
        if v < numFree then typ else
        if v - numFree >= top then 
            ILType.TypeVar (uint16 (v - top)) 
        else 
            List.item (v - numFree) inst
    | x -> x
    
and instILGenericArgsAux numFree inst i = List.map (instILTypeAux numFree inst) i

and instILCallSigAux numFree inst  csig = 
  mkILCallSig  (csig.CallingConv,List.map (instILTypeAux numFree inst) csig.ArgTypes,instILTypeAux numFree inst csig.ReturnType)

let instILType     i t = instILTypeAux 0 i t

// --------------------------------------------------------------------
// MS-IL: Parameters, Return types and Locals
// -------------------------------------------------------------------- 

let mkILParam (name,ty) : ILParameter =
    { Name=name;
      Default=None;
      Marshal=None;
      IsIn=false;
      IsOut=false;
      IsOptional=false;
      Type=ty;
      CustomAttrs=emptyILCustomAttrs }
let mkILParamNamed (s,ty) = mkILParam (Some s,ty)
let mkILParamAnon ty = mkILParam (None,ty)

let mkILReturn ty : ILReturn = 
    { Marshal=None;
      Type=ty;
      CustomAttrs=emptyILCustomAttrs  }

let mkILLocal ty dbgInfo : ILLocal = 
    { IsPinned=false;
      Type=ty;
      DebugInfo=dbgInfo }

type ILFieldSpec with
  member fr.ActualType = 
      let env = fr.EnclosingType.GenericArgs
      instILType env fr.FormalType

// -------------------------------------------------------------------- 
// Make a method mbody
// -------------------------------------------------------------------- 

let mkILMethodBody (zeroinit,locals,maxstack,code,tag) : ILMethodBody = 
  { IsZeroInit=zeroinit
    MaxStack=maxstack
    NoInlining=false
    Locals= locals 
    Code= code
    SourceMarker=tag }

let mkMethodBody (zeroinit,locals,maxstack,code,tag) = MethodBody.IL (mkILMethodBody (zeroinit,locals,maxstack,code,tag))

// -------------------------------------------------------------------- 
// Make a constructor
// -------------------------------------------------------------------- 

let mkILVoidReturn = mkILReturn ILType.Void


let mkILCtor (access,args,impl) = 
    { Name=".ctor";
      mdKind=MethodKind.Ctor;
      CallingConv=ILCallingConv.Instance;
      Parameters = args
      Return= mkILVoidReturn;
      Access=access;
      mdBody= mkMethBodyAux impl;
      mdCodeKind=MethodCodeKind.IL;
      IsInternalCall=false;
      IsManaged=true;
      IsForwardRef=false;
      SecurityDecls=emptyILSecurityDecls;
      HasSecurity=false;
      IsEntryPoint=false;
      GenericParams=mkILEmptyGenericParams;
      IsReqSecObj=false;
      IsHideBySig=false;
      IsSpecialName=true;
      IsUnmanagedExport=false;
      IsSynchronized=false;
      IsNoInline=false;
      IsMustRun=false;
      IsPreserveSig=false;
      CustomAttrs = emptyILCustomAttrs; }

// -------------------------------------------------------------------- 
// Do-nothing ctor, just pass on to monomorphic superclass
// -------------------------------------------------------------------- 

let mkCallBaseConstructor (typ,args: ILType list) =
    [ mkLdarg0; ] @
    List.mapi (fun i _ -> mkLdarg (uint16 (i+1))) args @
    [ mkNormalCall (mkILCtorMethSpecForTy (typ,[])) ]

let mkNormalStfld fspec = I_stfld (Aligned,Nonvolatile,fspec)
let mkNormalStsfld fspec = I_stsfld (Nonvolatile,fspec)
let mkNormalLdsfld fspec = I_ldsfld (Nonvolatile,fspec)
let mkNormalLdfld fspec = I_ldfld (Aligned,Nonvolatile,fspec)
let mkNormalLdflda fspec = I_ldflda fspec
let mkNormalLdobj dt = I_ldobj(Aligned,Nonvolatile,dt)
let mkNormalStobj dt = I_stobj(Aligned,Nonvolatile,dt)

let mkILNonGenericEmptyCtor tag superTy = 
    let ctor = mkCallBaseConstructor (superTy,[])
    mkILCtor(ILMemberAccess.Public,[],mkMethodBody(false,[],8, nonBranchingInstrsToCode ctor,tag))

// -------------------------------------------------------------------- 
// Make a static, top level monomophic method - very useful for
// creating helper ILMethodDefs for internal use.
// -------------------------------------------------------------------- 

let mkILStaticMethod (genparams,nm,access,args,ret,impl) = 
    { GenericParams=genparams;
      Name=nm;
      CallingConv = ILCallingConv.Static;
      mdKind=MethodKind.Static;
      Parameters = args
      Return= ret;
      Access=access;
      HasSecurity=false;
      SecurityDecls=emptyILSecurityDecls;
      IsEntryPoint=false;
      CustomAttrs = emptyILCustomAttrs;
      mdBody= mkMethBodyAux impl;
      mdCodeKind=MethodCodeKind.IL;
      IsInternalCall=false;
      IsManaged=true;
      IsForwardRef=false;
      IsReqSecObj=false;
      IsHideBySig=false;
      IsSpecialName=false;
      IsUnmanagedExport=false;
      IsSynchronized=false;
      IsNoInline=false;
      IsMustRun=false;
      IsPreserveSig=false; }

let mkILNonGenericStaticMethod (nm,access,args,ret,impl) = 
    mkILStaticMethod (mkILEmptyGenericParams,nm,access,args,ret,impl)

let mkILClassCtor impl = 
    { Name=".cctor";
      CallingConv=ILCallingConv.Static;
      GenericParams=mkILEmptyGenericParams;
      mdKind=MethodKind.Cctor;
      Parameters = []
      Return=mkILVoidReturn;
      Access=ILMemberAccess.Private; 
      IsEntryPoint=false;
      HasSecurity=false;
      SecurityDecls=emptyILSecurityDecls;
      CustomAttrs = emptyILCustomAttrs;
      mdBody= mkMethBodyAux impl; 
      mdCodeKind=MethodCodeKind.IL;
      IsInternalCall=false;
      IsManaged=true;
      IsForwardRef=false;
      IsReqSecObj=false;
      IsHideBySig=false;
      IsSpecialName=true;
      IsUnmanagedExport=false; 
      IsSynchronized=false;
      IsNoInline=false;
      IsMustRun=false;
      IsPreserveSig=false;  } 

// -------------------------------------------------------------------- 
// Make a virtual method, where the overriding is simply the default
// (i.e. overrides by name/signature)
// -------------------------------------------------------------------- 

let mk_ospec (typ:ILType,callconv,nm,genparams,formal_args,formal_ret) =
  OverridesSpec (mkILMethRef (typ.TypeRef, callconv, nm, genparams, formal_args,formal_ret), typ)

let mkILGenericVirtualMethod (nm,access,genparams,actual_args,actual_ret,impl) = 
  { Name=nm;
    GenericParams=genparams;
    CallingConv=ILCallingConv.Instance;
    mdKind=
      MethodKind.Virtual 
        { IsFinal=false; 
          // REVIEW: We'll need to start setting this eventually
          IsNewSlot = false;
          IsCheckAccessOnOverride=true;
          IsAbstract=(match impl with MethodBody.Abstract -> true | _ -> false) ; };
    Parameters=actual_args;
    Return=actual_ret;
    Access=access;
    IsEntryPoint=false;
    HasSecurity=false;
    SecurityDecls=emptyILSecurityDecls;
    CustomAttrs = emptyILCustomAttrs;
    mdBody= mkMethBodyAux impl;
    mdCodeKind=MethodCodeKind.IL;
    IsInternalCall=false;
    IsManaged=true;
    IsForwardRef=false;
    IsReqSecObj=false;
    IsHideBySig=false;
    IsSpecialName=false;
    IsUnmanagedExport=false; 
    IsSynchronized=false;
    IsNoInline=false;
    IsMustRun=false;
    IsPreserveSig=false; }
    
let mkILNonGenericVirtualMethod (nm,access,args,ret,impl) =  
  mkILGenericVirtualMethod (nm,access,mkILEmptyGenericParams,args,ret,impl)

let mkILGenericNonVirtualMethod (nm,access,genparams, actual_args,actual_ret, impl) = 
  { Name=nm;
    GenericParams=genparams;
    CallingConv=ILCallingConv.Instance;
    mdKind=MethodKind.NonVirtual;
    Parameters=actual_args;
    Return=actual_ret;
    Access=access;
    IsEntryPoint=false;
    HasSecurity=false;
    SecurityDecls=emptyILSecurityDecls;
    CustomAttrs = emptyILCustomAttrs;
    mdBody= mkMethBodyAux impl;
    mdCodeKind=MethodCodeKind.IL;
    IsInternalCall=false;
    IsManaged=true;
    IsForwardRef=false;
    IsReqSecObj=false;
    IsHideBySig=true; // see Bug343136: missing HideBySig attribute makes it problematic for C# to consume F# method overloads.
    IsSpecialName=false;
    IsUnmanagedExport=false; 
    IsSynchronized=false;
    IsNoInline=false;
    IsMustRun=false;
    IsPreserveSig=false; }
    
let mkILNonGenericInstanceMethod (nm,access,args,ret,impl) =  
  mkILGenericNonVirtualMethod (nm,access,mkILEmptyGenericParams,args,ret,impl)


// -------------------------------------------------------------------- 
// Add some code to the end of the .cctor for a type.  Create a .cctor
// if one doesn't exist already.
// -------------------------------------------------------------------- 

let ilmbody_code2code f (il: ILMethodBody)  =
  {il with Code = f il.Code}

let mdef_code2code f md  =
    let il = 
        match md.mdBody.Contents with 
        | MethodBody.IL il-> il 
        | _ -> failwith "mdef_code2code - method not IL"
    let b = MethodBody.IL (ilmbody_code2code f il)
    {md with mdBody= mkMethBodyAux b }  

let prependInstrsToCode (instrs: ILInstr list) (c2: ILCode) = 
    let instrs = Array.ofList instrs
    let n = instrs.Length
    match c2.Instrs.[0] with 
    // If there is a sequence point as the first instruction then keep it at the front
    | I_seqpoint _ as i0 ->
        let labels = 
            let dict = Dictionary.newWithSize c2.Labels.Count
            for kvp in c2.Labels do dict.Add(kvp.Key, if kvp.Value = 0 then 0 else kvp.Value + n)
            dict
        { c2 with Labels = labels
                  Instrs = Array.concat [| [|i0|] ; instrs ; c2.Instrs.[1..] |] }
    | _ ->
        let labels =
            let dict = Dictionary.newWithSize c2.Labels.Count
            for kvp in c2.Labels do dict.Add(kvp.Key, kvp.Value + n)
            dict
        { c2 with Labels = labels
                  Instrs = Array.append instrs c2.Instrs }

let prependInstrsToMethod new_code md  = 
    mdef_code2code (prependInstrsToCode new_code) md

// Creates cctor if needed 
let cdef_cctorCode2CodeOrCreate tag f cd = 
    let mdefs = cd.Methods
    let cctor = 
        match mdefs.FindByName ".cctor" with 
        | [mdef] -> mdef
        | [] -> mkILClassCtor (mkMethodBody (false,[],1,nonBranchingInstrsToCode [ ],tag))
        | _ -> failwith "bad method table: more than one .cctor found"
        
    let methods = ILMethodDefs (fun () -> [| yield f cctor; for md in mdefs do if md.Name <> ".cctor" then yield md |])
    {cd with Methods = methods}


let code_of_mdef (md:ILMethodDef) = 
    match md.Code with 
    | Some x -> x
    | None -> failwith "code_of_mdef: not IL" 

let mkRefToILMethod (tref, md: ILMethodDef) =
    mkILMethRef (tref, md.CallingConv, md.Name, md.GenericParams.Length, md.ParameterTypes, md.Return.Type)

let mkRefToILField (tref,fdef:ILFieldDef) =   mkILFieldRef (tref, fdef.Name, fdef.Type)

let mkRefForILMethod scope (tdefs,tdef) mdef = mkRefToILMethod (mkRefForNestedILTypeDef scope (tdefs,tdef), mdef)
let mkRefForILField scope (tdefs,tdef) (fdef:ILFieldDef) = mkILFieldRef (mkRefForNestedILTypeDef scope (tdefs,tdef), fdef.Name, fdef.Type)


(* Creates cctor if needed *)
let prependInstrsToClassCtor instrs tag cd = 
    cdef_cctorCode2CodeOrCreate tag (prependInstrsToMethod instrs) cd
    

let mkILField (isStatic,nm,ty,init,at,access,isLiteral) =
   { Name=nm;
     Type=ty;
     IsStatic = isStatic; 
     LiteralValue = init;
     Data=at;
     Offset=None;
     IsSpecialName = false;
     Marshal=None; 
     NotSerialized=false;
     IsInitOnly = false;
     IsLiteral = isLiteral; 
     Access = access; 
     CustomAttrs=emptyILCustomAttrs }

let mkILInstanceField (nm,ty,init,access) = mkILField (false,nm,ty,init,None,access,false)
let mkILStaticField (nm,ty,init,at,access) = mkILField (true,nm,ty,init,at,access,false)
let mkILLiteralField (nm,ty,init,at,access) = mkILField (true, nm, ty, Some init, at, access, true)

// -------------------------------------------------------------------- 
// Scopes for allocating new temporary variables.
// -------------------------------------------------------------------- 

type ILLocalsAllocator(numPrealloc:int) = 
    let newLocals = ResizeArray<ILLocal>()
    member tmps.AllocLocal loc = 
        let locn = uint16(numPrealloc + newLocals.Count)
        newLocals.Add loc;
        locn

    member tmps.Close() = ResizeArray.toList newLocals


let mkILFieldsLazy l =  Fields (LazyOrderedMultiMap((fun (f:ILFieldDef) -> f.Name),l))
let mkILFields l =  mkILFieldsLazy (notlazy l)
let emptyILFields = mkILFields []

let mkILEventsLazy l =  Events (LazyOrderedMultiMap((fun (e: ILEventDef) -> e.Name),l))
let mkILEvents l =  mkILEventsLazy (notlazy l)
let emptyILEvents =  mkILEvents []

let mkILPropertiesLazy l =  Properties (LazyOrderedMultiMap((fun (p: ILPropertyDef) -> p.Name),l) )
let mkILProperties l =  mkILPropertiesLazy (notlazy l)
let emptyILProperties =  mkILProperties []

let addExportedTypeToTable (y: ILExportedTypeOrForwarder) tab = Map.add y.Name y tab
let mkILExportedTypes l =  ILExportedTypesAndForwarders (notlazy (List.foldBack addExportedTypeToTable l Map.empty))
let mkILExportedTypesLazy (l:Lazy<_>) =   ILExportedTypesAndForwarders (lazy (List.foldBack addExportedTypeToTable (l.Force()) Map.empty))

let addNestedExportedTypeToTable (y: ILNestedExportedType) tab =
    Map.add y.Name y tab

let mkILNestedExportedTypes l =  
    ILNestedExportedTypes (notlazy (List.foldBack addNestedExportedTypeToTable l Map.empty))

let mkILNestedExportedTypesLazy (l:Lazy<_>) =  
    ILNestedExportedTypes (lazy (List.foldBack addNestedExportedTypeToTable (l.Force()) Map.empty))

let mkILResources l =  ILResources (notlazy l)
let mkILResourcesLazy l =  ILResources l

let addMethodImplToTable y tab =
    let key = (y.Overrides.MethodRef.Name,y.Overrides.MethodRef.ArgTypes.Length)
    let prev = Map.tryFindMulti key tab
    Map.add key (y::prev) tab

let mkILMethodImpls l =  MethodImpls (notlazy (List.foldBack addMethodImplToTable l Map.empty))
let mkILMethodImplsLazy l =  MethodImpls (lazy (List.foldBack addMethodImplToTable (Lazy.force l) Map.empty))
let emptyILMethodImpls =  mkILMethodImpls []


// -------------------------------------------------------------------- 
// Make a constructor that simply takes its arguments and stuffs
// them in fields.  preblock is how to call the superclass constructor....
// -------------------------------------------------------------------- 

let mkILStorageCtorWithParamNames(tag,preblock,typ,extraParams,flds,access) = 
    mkILCtor(access,
            (flds |> List.map (fun (pnm,_,ty) -> mkILParamNamed (pnm,ty))) @ extraParams,
            mkMethodBody
              (false,[],2,
               nonBranchingInstrsToCode
                 begin 
                   (match tag with Some x -> [I_seqpoint x] | None -> []) @ 
                   preblock @
                   List.concat (List.mapi (fun n (_pnm,nm,ty) -> 
                     [ mkLdarg0;
                       mkLdarg (uint16 (n+1));
                       mkNormalStfld (mkILFieldSpecInTy (typ,nm,ty));
                     ])  flds)
                 end,tag))
    
let mkILSimpleStorageCtorWithParamNames(tag,base_tspec,typ,extraParams,flds,access) = 
    let preblock = 
      match base_tspec with 
        None -> []
      | Some tspec -> 
          ([ mkLdarg0; 
             mkNormalCall (mkILCtorMethSpecForTy (mkILBoxedType tspec,[])) ])
    mkILStorageCtorWithParamNames(tag,preblock,typ,extraParams,flds,access)

let addParamNames flds = 
    flds |> List.map (fun (nm,ty) -> (nm,nm,ty))

let mkILSimpleStorageCtor(tag,base_tspec,typ,extraParams,flds,access) = 
    mkILSimpleStorageCtorWithParamNames(tag,base_tspec,typ, extraParams, addParamNames flds, access)

let mkILStorageCtor(tag,preblock,typ,flds,access) = mkILStorageCtorWithParamNames(tag, preblock, typ, [], addParamNames flds, access)


let mkILGenericClass (nm, access, genparams, extends, impl, methods, fields, nestedTypes, props, events, attrs, init) =
  { tdKind=ILTypeDefKind.Class;
    Name=nm;
    GenericParams= genparams;
    Access = access;
    Implements = impl
    IsAbstract = false;
    IsSealed = false;
    IsSerializable = false;
    IsComInterop=false;
    IsSpecialName=false;
    Layout=ILTypeDefLayout.Auto;
    Encoding=ILDefaultPInvokeEncoding.Ansi;
    InitSemantics=init;
    Extends = Some extends;
    Methods= methods; 
    Fields= fields;
    NestedTypes=nestedTypes;
    CustomAttrs=attrs;
    MethodImpls=emptyILMethodImpls;
    Properties=props;
    Events=events;
    SecurityDecls=emptyILSecurityDecls; 
    HasSecurity=false;
} 
    
let mkRawDataValueTypeDef (iltyp_ValueType: ILType) (nm,size,pack) =
  { tdKind=ILTypeDefKind.ValueType;
    Name = nm;
    GenericParams= [];
    Access = ILTypeDefAccess.Private;
    Implements = []
    IsAbstract = false;
    IsSealed = true;
    Extends = Some iltyp_ValueType;
    IsComInterop=false;    
    IsSerializable = false;
    IsSpecialName=false;
    Layout=ILTypeDefLayout.Explicit { Size=Some size; Pack=Some pack };
    Encoding=ILDefaultPInvokeEncoding.Ansi;
    InitSemantics=ILTypeInit.BeforeField;
    Methods= emptyILMethods; 
    Fields= emptyILFields;
    NestedTypes=emptyILTypeDefs;
    CustomAttrs=emptyILCustomAttrs;
    MethodImpls=emptyILMethodImpls;
    Properties=emptyILProperties;
    Events=emptyILEvents;
    SecurityDecls=emptyILSecurityDecls; 
    HasSecurity=false;  }


let mkILSimpleClass (ilg: ILGlobals) (nm, access, methods, fields, nestedTypes, props, events, attrs, init) =
  mkILGenericClass (nm,access, mkILEmptyGenericParams, ilg.typ_Object, [], methods, fields, nestedTypes, props, events, attrs, init)

let mkILTypeDefForGlobalFunctions ilg (methods,fields) = mkILSimpleClass ilg (typeNameForGlobalFunctions, ILTypeDefAccess.Public, methods, fields, emptyILTypeDefs, emptyILProperties, emptyILEvents, emptyILCustomAttrs,ILTypeInit.BeforeField)

let destTypeDefsWithGlobalFunctionsFirst ilg (tdefs: ILTypeDefs) = 
  let l = tdefs.AsList
  let top,nontop = l |> List.partition (fun td -> td.Name = typeNameForGlobalFunctions)
  let top2 = if isNil top then [ mkILTypeDefForGlobalFunctions ilg (emptyILMethods, emptyILFields) ] else top
  top2@nontop

let mkILSimpleModule assname modname dll subsystemVersion useHighEntropyVA tdefs hashalg locale flags exportedTypes metadataVersion = 
    { Manifest= 
        Some { Name=assname;
               AuxModuleHashAlgorithm= match hashalg with | Some(alg) -> alg | _ -> 0x8004; // SHA1
               SecurityDecls=emptyILSecurityDecls;
               PublicKey= None;
               Version= None;
               Locale=locale
               CustomAttrs=emptyILCustomAttrs;
               AssemblyLongevity=ILAssemblyLongevity.Unspecified;
               DisableJitOptimizations = 0 <> (flags &&& 0x4000);
               JitTracking = (0 <> (flags &&& 0x8000)); // always turn these on
               IgnoreSymbolStoreSequencePoints = (0 <> (flags &&& 0x2000));
               Retargetable = (0 <> (flags &&& 0x100));
               ExportedTypes=exportedTypes;
               EntrypointElsewhere=None
             };
      CustomAttrs=emptyILCustomAttrs;
      Name=modname;
      NativeResources=[];
      TypeDefs=tdefs;
      SubsystemVersion = subsystemVersion
      UseHighEntropyVA = useHighEntropyVA
      SubSystemFlags=defaultSubSystem;
      IsDLL=dll;
      IsILOnly=true;
      Platform=None;
      StackReserveSize=None;
      Is32Bit=false;
      Is32BitPreferred=false;
      Is64Bit=false;
      PhysicalAlignment=defaultPhysAlignment;
      VirtualAlignment=defaultVirtAlignment;
      ImageBase=defaultImageBase;
      MetadataVersion=metadataVersion;
      Resources=mkILResources [];
    }


//-----------------------------------------------------------------------
// [instructions_to_code] makes the basic block structure of code from
// a primitive array of instructions.  We
// do this be iterating over the instructions, pushing new basic blocks 
// everytime we encounter an address that has been recorded
// [bbstartToCodeLabelMap].
//-----------------------------------------------------------------------

            
// REVIEW: this function shows up on performance traces. If we eliminated the last ILX->IL rewrites from the
// F# compiler we could get rid of this structured code representation from Abstract IL altogether and 
// never convert F# code into this form.
let buildILCode (_methName:string) lab2pc instrs tryspecs localspecs : ILCode =
    { Labels = lab2pc
      Instrs = instrs
      Exceptions = tryspecs
      Locals = localspecs }


// -------------------------------------------------------------------- 
// Detecting Delegates
// -------------------------------------------------------------------- 

let mkILDelegateMethods (ilg: ILGlobals) (iltyp_AsyncCallback, iltyp_IAsyncResult) (parms,rtv:ILReturn) = 
    let rty = rtv.Type
    let one nm args ret =
        let mdef = mkILNonGenericVirtualMethod (nm,ILMemberAccess.Public,args,mkILReturn ret,MethodBody.Abstract)
        {mdef with 
                   mdKind=
                      match mdef.mdKind with 
                      | MethodKind.Virtual vinfo -> MethodKind.Virtual {vinfo with IsAbstract=false; } 
                      | k -> k 
                   mdCodeKind=MethodCodeKind.Runtime;
                   IsHideBySig=true; }
    let ctor = mkILCtor(ILMemberAccess.Public, [ mkILParamNamed("object",ilg.typ_Object); mkILParamNamed("method",ilg.typ_IntPtr) ], MethodBody.Abstract)
    let ctor = { ctor with  mdCodeKind=MethodCodeKind.Runtime; IsHideBySig=true }
    [ ctor;
      one "Invoke" parms rty;
      one "BeginInvoke" (parms @ [mkILParamNamed("callback",iltyp_AsyncCallback); mkILParamNamed("objects",ilg.typ_Object) ] ) iltyp_IAsyncResult;
      one "EndInvoke" [mkILParamNamed("result",iltyp_IAsyncResult)] rty; ]
    

let mkCtorMethSpecForDelegate (ilg: ILGlobals) (typ:ILType,useUIntPtr) =
    let scoref = typ.TypeRef.Scope 
    mkILInstanceMethSpecInTy (typ,".ctor",[rescopeILType scoref ilg.typ_Object; rescopeILType scoref (if useUIntPtr then ilg.typ_UIntPtr else ilg.typ_IntPtr)],ILType.Void,emptyILGenericArgsList)

type ILEnumInfo =
    { enumValues: (string * ILFieldInit) list;  
      enumType: ILType }

let getTyOfILEnumInfo info = info.enumType

let computeILEnumInfo (mdName,mdFields: ILFieldDefs) = 
    match (List.partition (fun fd -> fd.IsStatic) mdFields.AsList) with 
    | staticFields,[vfd] -> 
        { enumType = vfd.Type; 
          enumValues = staticFields |> List.map (fun fd -> (fd.Name, match fd.LiteralValue with Some i -> i | None -> failwith ("info_of_enum_tdef: badly formed enum "+mdName+": static field does not have an default value")))  }
    | _,[] -> failwith ("info_of_enum_tdef: badly formed enum "+mdName+": no non-static field found")
    | _,_ -> failwith ("info_of_enum_tdef: badly formed enum "+mdName+": more than one non-static field found")

 

//---------------------------------------------------------------------
// Primitives to help read signatures.  These do not use the file cursor, but
// pass around an int index
//---------------------------------------------------------------------

let sigptr_get_byte bytes sigptr = 
    Bytes.get bytes sigptr, sigptr + 1

let sigptr_get_bool bytes sigptr = 
    let b0,sigptr = sigptr_get_byte bytes sigptr
    (b0 = 0x01) ,sigptr

let sigptr_get_u8 bytes sigptr = 
    let b0,sigptr = sigptr_get_byte bytes sigptr
    byte b0,sigptr

let sigptr_get_i8 bytes sigptr = 
    let i,sigptr = sigptr_get_u8 bytes sigptr
    sbyte i,sigptr

let sigptr_get_u16 bytes sigptr = 
    let b0,sigptr = sigptr_get_byte bytes sigptr
    let b1,sigptr = sigptr_get_byte bytes sigptr
    uint16 (b0 ||| (b1 <<< 8)),sigptr

let sigptr_get_i16 bytes sigptr = 
    let u,sigptr = sigptr_get_u16 bytes sigptr
    int16 u,sigptr

let sigptr_get_i32 bytes sigptr = 
    let b0,sigptr = sigptr_get_byte bytes sigptr
    let b1,sigptr = sigptr_get_byte bytes sigptr
    let b2,sigptr = sigptr_get_byte bytes sigptr
    let b3,sigptr = sigptr_get_byte bytes sigptr
    b0 ||| (b1 <<< 8) ||| (b2 <<< 16) ||| (b3 <<< 24),sigptr

let sigptr_get_u32 bytes sigptr = 
    let u,sigptr = sigptr_get_i32 bytes sigptr
    uint32 u,sigptr

let sigptr_get_i64 bytes sigptr = 
    let b0,sigptr = sigptr_get_byte bytes sigptr
    let b1,sigptr = sigptr_get_byte bytes sigptr
    let b2,sigptr = sigptr_get_byte bytes sigptr
    let b3,sigptr = sigptr_get_byte bytes sigptr
    let b4,sigptr = sigptr_get_byte bytes sigptr
    let b5,sigptr = sigptr_get_byte bytes sigptr
    let b6,sigptr = sigptr_get_byte bytes sigptr
    let b7,sigptr = sigptr_get_byte bytes sigptr
    int64 b0 ||| (int64 b1 <<< 8) ||| (int64 b2 <<< 16) ||| (int64 b3 <<< 24) |||
    (int64 b4 <<< 32) ||| (int64 b5 <<< 40) ||| (int64 b6 <<< 48) ||| (int64 b7 <<< 56),
    sigptr

let sigptr_get_u64 bytes sigptr = 
    let u,sigptr = sigptr_get_i64 bytes sigptr
    uint64 u,sigptr

let float32_of_bits (x:int32) = System.BitConverter.ToSingle(System.BitConverter.GetBytes(x),0)
let float_of_bits (x:int64) = System.BitConverter.Int64BitsToDouble(x)

let sigptr_get_ieee32 bytes sigptr = 
    let u,sigptr = sigptr_get_i32 bytes sigptr
    float32_of_bits u,sigptr

let sigptr_get_ieee64 bytes sigptr = 
    let u,sigptr = sigptr_get_i64 bytes sigptr
    float_of_bits u,sigptr

let sigptr_get_intarray n (bytes:byte[]) sigptr = 
    let res = Bytes.zeroCreate n
    for i = 0 to n - 1 do 
        res.[i] <- bytes.[sigptr + i]
    res, sigptr + n

let sigptr_get_string n bytes sigptr = 
    let intarray,sigptr = sigptr_get_intarray n bytes sigptr
    System.Text.Encoding.UTF8.GetString(intarray , 0, intarray.Length), sigptr
   
let sigptr_get_z_i32 bytes sigptr = 
    let b0,sigptr = sigptr_get_byte bytes sigptr
    if b0 <= 0x7F then b0, sigptr
    elif b0 <= 0xbf then 
        let b0 = b0 &&& 0x7f
        let b1,sigptr = sigptr_get_byte bytes sigptr
        (b0 <<< 8) ||| b1, sigptr
    else 
        let b0 = b0 &&& 0x3f
        let b1,sigptr = sigptr_get_byte bytes sigptr
        let b2,sigptr = sigptr_get_byte bytes sigptr
        let b3,sigptr = sigptr_get_byte bytes sigptr
        (b0 <<< 24) ||| (b1 <<< 16) ||| (b2 <<< 8) ||| b3, sigptr

let sigptr_get_serstring  bytes sigptr = 
    let len,sigptr = sigptr_get_z_i32 bytes sigptr 
    sigptr_get_string ( len) bytes sigptr 
  
let sigptr_get_serstring_possibly_null  bytes sigptr = 
    let b0,new_sigptr = sigptr_get_byte bytes sigptr  
    if b0 = 0xFF then // null case
        None,new_sigptr
    else  // throw away  new_sigptr, getting length & text advance
        let len,sigptr = sigptr_get_z_i32 bytes sigptr 
        let s, sigptr = sigptr_get_string len bytes sigptr
        Some(s),sigptr
  
//---------------------------------------------------------------------
// Get the public key token from the public key.
//---------------------------------------------------------------------

let mkRefToILAssembly (m: ILAssemblyManifest) = 
    ILAssemblyRef.Create(m.Name, None, (match m.PublicKey with Some k -> Some (PublicKey.KeyAsToken(k)) | None -> None), m.Retargetable, m.Version, m.Locale)

let z_unsigned_int_size n = 
    if n <= 0x7F then 1
    elif n <= 0x3FFF then 2
    else 3

let z_unsigned_int n = 
    if n >= 0 &&  n <= 0x7F then [| byte n |] 
    elif n >= 0x80 && n <= 0x3FFF then [| byte (0x80 ||| (n >>>& 8)); byte (n &&& 0xFF) |] 
    else [| byte (0xc0 ||| (n >>>& 24)); 
            byte ((n >>>& 16) &&& 0xFF); 
            byte ((n >>>& 8) &&& 0xFF); 
            byte (n &&& 0xFF) |]

let string_as_utf8_bytes (s:string) = System.Text.Encoding.UTF8.GetBytes s

(* Little-endian encoding of int64 *)
let dw7 n = byte ((n >>> 56) &&& 0xFFL)
let dw6 n = byte ((n >>> 48) &&& 0xFFL)
let dw5 n = byte ((n >>> 40) &&& 0xFFL)
let dw4 n = byte ((n >>> 32) &&& 0xFFL)
let dw3 n = byte ((n >>> 24) &&& 0xFFL)
let dw2 n = byte ((n >>> 16) &&& 0xFFL)
let dw1 n = byte ((n >>> 8)  &&& 0xFFL)
let dw0 n = byte (n          &&& 0xFFL)

let u8AsBytes (i:byte) = [| i |]
let u16AsBytes x =  let n = (int x) in [| byte (b0 n); byte (b1 n) |]
let i32AsBytes i = [| byte (b0 i); byte (b1 i); byte (b2 i); byte (b3 i) |]
let i64AsBytes i = [| dw0 i; dw1 i; dw2 i; dw3 i; dw4 i; dw5 i; dw6 i; dw7 i |]

let i8AsBytes (i:sbyte) = u8AsBytes (byte i)
let i16AsBytes (i:int16) = u16AsBytes (uint16 i)
let u32AsBytes (i:uint32) = i32AsBytes (int32 i)
let u64AsBytes (i:uint64) = i64AsBytes (int64 i)

let bits_of_float32 (x:float32) = System.BitConverter.ToInt32(System.BitConverter.GetBytes(x),0)
let bits_of_float (x:float) = System.BitConverter.DoubleToInt64Bits(x)

let ieee32AsBytes i = i32AsBytes (bits_of_float32 i)
let ieee64AsBytes i = i64AsBytes (bits_of_float i)

let et_END = 0x00uy
let et_VOID = 0x01uy
let et_BOOLEAN = 0x02uy
let et_CHAR = 0x03uy
let et_I1 = 0x04uy
let et_U1 = 0x05uy
let et_I2 = 0x06uy
let et_U2 = 0x07uy
let et_I4 = 0x08uy
let et_U4 = 0x09uy
let et_I8 = 0x0Auy
let et_U8 = 0x0Buy
let et_R4 = 0x0Cuy
let et_R8 = 0x0Duy
let et_STRING = 0x0Euy
let et_PTR = 0x0Fuy
let et_BYREF = 0x10uy
let et_VALUETYPE      = 0x11uy
let et_CLASS          = 0x12uy
let et_VAR            = 0x13uy
let et_ARRAY          = 0x14uy
let et_WITH           = 0x15uy
let et_TYPEDBYREF     = 0x16uy
let et_I              = 0x18uy
let et_U              = 0x19uy
let et_FNPTR          = 0x1Buy
let et_OBJECT         = 0x1Cuy
let et_SZARRAY        = 0x1Duy
let et_MVAR           = 0x1Euy
let et_CMOD_REQD      = 0x1Fuy
let et_CMOD_OPT       = 0x20uy

let formatILVersion ((a,b,c,d):ILVersionInfo) = sprintf "%d.%d.%d.%d" (int a) (int b) (int c) (int d)

let encodeCustomAttrString s = 
    let arr = string_as_utf8_bytes s
    Array.concat [ z_unsigned_int arr.Length; arr ]      

let rec encodeCustomAttrElemType x = 
    match x with
    | ILType.Value tspec when tspec.Name = tname_SByte ->  [| et_I1 |]
    | ILType.Value tspec when tspec.Name = tname_Byte ->  [| et_U1 |]
    | ILType.Value tspec when tspec.Name = tname_Int16 ->  [| et_I2 |]
    | ILType.Value tspec when tspec.Name = tname_UInt16 ->  [| et_U2 |]
    | ILType.Value tspec when tspec.Name = tname_Int32 ->  [| et_I4 |]
    | ILType.Value tspec when tspec.Name = tname_UInt32 ->  [| et_U4 |]
    | ILType.Value tspec when tspec.Name = tname_Int64 ->  [| et_I8 |]
    | ILType.Value tspec when tspec.Name = tname_UInt64 ->  [| et_U8 |]
    | ILType.Value tspec when tspec.Name = tname_Double ->  [| et_R8 |]
    | ILType.Value tspec when tspec.Name = tname_Single ->  [| et_R4 |]
    | ILType.Value tspec when tspec.Name = tname_Char ->  [| et_CHAR |]
    | ILType.Value tspec when tspec.Name = tname_Bool ->  [| et_BOOLEAN |]
    | ILType.Boxed tspec when tspec.Name = tname_String ->  [| et_STRING |]
    | ILType.Boxed tspec when tspec.Name = tname_Object ->  [| 0x51uy |] 
    | ILType.Boxed tspec when tspec.Name = tname_Type ->  [| 0x50uy |]
    | ILType.Value tspec ->  Array.append [| 0x55uy |] (encodeCustomAttrString tspec.TypeRef.QualifiedNameWithNoShortPrimaryAssembly)
    | ILType.Array (shape, elemType) when shape = ILArrayShape.SingleDimensional -> 
          Array.append [| et_SZARRAY |] (encodeCustomAttrElemType elemType)
    | _ ->  failwith "encodeCustomAttrElemType: unrecognized custom element type"

/// Given a custom attribute element, work out the type of the .NET argument for that element.
let rec encodeCustomAttrElemTypeForObject x = 
    match x with
    | ILAttribElem.String _  -> [| et_STRING |]
    | ILAttribElem.Bool _    -> [| et_BOOLEAN |]
    | ILAttribElem.Char _    -> [| et_CHAR |]
    | ILAttribElem.SByte _   -> [| et_I1 |]
    | ILAttribElem.Int16 _   -> [| et_I2 |]
    | ILAttribElem.Int32 _   -> [| et_I4 |]
    | ILAttribElem.Int64 _   -> [| et_I8 |]
    | ILAttribElem.Byte _    -> [| et_U1 |]
    | ILAttribElem.UInt16 _  -> [| et_U2 |]
    | ILAttribElem.UInt32 _  -> [| et_U4 |]
    | ILAttribElem.UInt64 _  -> [| et_U8 |]
    | ILAttribElem.Type _    -> [| 0x50uy |]
    | ILAttribElem.TypeRef _ -> [| 0x50uy |]
    | ILAttribElem.Null _    -> [| et_STRING  |]// yes, the 0xe prefix is used when passing a "null" to a property or argument of type "object" here
    | ILAttribElem.Single _  -> [| et_R4 |]
    | ILAttribElem.Double _  -> [| et_R8 |]
    | ILAttribElem.Array (elemTy,_) -> [| yield et_SZARRAY; yield! encodeCustomAttrElemType elemTy |]


let rec decodeCustomAttrElemType (ilg: ILGlobals) bytes sigptr x = 
    match x with
    | x when x =  et_I1 -> ilg.typ_SByte, sigptr
    | x when x = et_U1 -> ilg.typ_Byte, sigptr
    | x when x =  et_I2 -> ilg.typ_Int16, sigptr
    | x when x =  et_U2 -> ilg.typ_UInt16, sigptr
    | x when x =  et_I4 -> ilg.typ_Int32, sigptr
    | x when x =  et_U4 -> ilg.typ_UInt32, sigptr
    | x when x =  et_I8 -> ilg.typ_Int64, sigptr
    | x when x =  et_U8 -> ilg.typ_UInt64, sigptr
    | x when x =  et_R8 -> ilg.typ_Double, sigptr
    | x when x =  et_R4 -> ilg.typ_Single, sigptr
    | x when x = et_CHAR -> ilg.typ_Char, sigptr
    | x when x =  et_BOOLEAN -> ilg.typ_Bool, sigptr
    | x when x =  et_STRING -> ilg.typ_String, sigptr
    | x when x =  et_OBJECT -> ilg.typ_Object, sigptr
    | x when x =  et_SZARRAY -> 
         let et,sigptr = sigptr_get_u8 bytes sigptr 
         let elemTy,sigptr = decodeCustomAttrElemType ilg bytes sigptr et
         mkILArr1DTy elemTy, sigptr
    | x when x = 0x50uy -> ilg.typ_Type, sigptr
    | _ ->  failwithf "decodeCustomAttrElemType ilg: unrecognized custom element type: %A" x


/// Given a custom attribute element, encode it to a binary representation according to the rules in Ecma 335 Partition II.
let rec encodeCustomAttrPrimValue ilg c = 
    match c with 
    | ILAttribElem.Bool b -> [| (if b then 0x01uy else 0x00uy) |]
    | ILAttribElem.String None 
    | ILAttribElem.Type None 
    | ILAttribElem.TypeRef None
    | ILAttribElem.Null -> [| 0xFFuy |]
    | ILAttribElem.String (Some s) -> encodeCustomAttrString s
    | ILAttribElem.Char x -> u16AsBytes (uint16 x)
    | ILAttribElem.SByte x -> i8AsBytes x
    | ILAttribElem.Int16 x -> i16AsBytes x
    | ILAttribElem.Int32 x -> i32AsBytes x
    | ILAttribElem.Int64 x -> i64AsBytes x
    | ILAttribElem.Byte x -> u8AsBytes x
    | ILAttribElem.UInt16 x -> u16AsBytes x
    | ILAttribElem.UInt32 x -> u32AsBytes x
    | ILAttribElem.UInt64 x -> u64AsBytes x
    | ILAttribElem.Single x -> ieee32AsBytes x
    | ILAttribElem.Double x -> ieee64AsBytes x
    | ILAttribElem.Type (Some ty) -> encodeCustomAttrString ty.QualifiedNameWithNoShortPrimaryAssembly 
    | ILAttribElem.TypeRef (Some tref) -> encodeCustomAttrString tref.QualifiedNameWithNoShortPrimaryAssembly
    | ILAttribElem.Array (_,elems) ->  
         [| yield! i32AsBytes elems.Length; for elem in elems do yield! encodeCustomAttrPrimValue ilg elem |]

and encodeCustomAttrValue ilg ty c = 
    match ty, c with 
    | ILType.Boxed tspec, _ when tspec.Name = tname_Object ->  
       [| yield! encodeCustomAttrElemTypeForObject c; yield! encodeCustomAttrPrimValue ilg c |]
    | ILType.Array (shape, _), ILAttribElem.Null when shape = ILArrayShape.SingleDimensional ->  
       [| yield! i32AsBytes 0xFFFFFFFF |]
    | ILType.Array (shape, elemType), ILAttribElem.Array (_,elems) when shape = ILArrayShape.SingleDimensional ->  
       [| yield! i32AsBytes elems.Length; for elem in elems do yield! encodeCustomAttrValue ilg elemType elem |]
    | _ -> 
       encodeCustomAttrPrimValue ilg c

let encodeCustomAttrNamedArg ilg (nm, ty, prop, elem) = 
   [| yield (if prop then 0x54uy else 0x53uy) 
      yield! encodeCustomAttrElemType ty;
      yield! encodeCustomAttrString nm;
      yield! encodeCustomAttrValue ilg ty elem |]

let mkILCustomAttribMethRef (ilg: ILGlobals) (mspec:ILMethodSpec, fixedArgs: list<_>, namedArgs: list<_>) = 
    let argtys = mspec.MethodRef.ArgTypes
    let args = 
      [| yield! [| 0x01uy; 0x00uy; |]
         for (argty,fixedArg) in Seq.zip argtys fixedArgs do
            yield! encodeCustomAttrValue ilg argty fixedArg
         yield! u16AsBytes (uint16 namedArgs.Length) 
         for namedArg in namedArgs do 
             yield! encodeCustomAttrNamedArg ilg namedArg |]

    { Method = mspec;
      Data = args }

let mkILCustomAttribute ilg (tref,argtys,argvs,propvs) = 
    mkILCustomAttribMethRef ilg (mkILNonGenericCtorMethSpec (tref,argtys),argvs,propvs)

let MscorlibScopeRef = ILScopeRef.Assembly (ILAssemblyRef.Create("mscorlib", None, Some ecmaPublicKey, true, None, None))
let EcmaMscorlibILGlobals = mkILGlobals MscorlibScopeRef


// PermissionSet is a 'blob' having the following format:
// - A byte containing a period (.).
// - A compressed int32 containing the number of attributes encoded in the blob.
// - An array of attributes each containing the following:
// - A String, which is the fully-qualified type name of the attribute. (Strings are encoded
//      as a compressed int to indicate the size followed by an array of UTF8 characters.)
// - A set of properties, encoded as the named arguments to a custom attribute would be (as
//      in §23.3, beginning with NumNamed).
let mkPermissionSet (ilg: ILGlobals) (action,attributes: list<(ILTypeRef * (string * ILType * ILAttribElem) list)>) = 
    let bytes = 
        [| yield (byte '.');
           yield! z_unsigned_int attributes.Length;
           for (tref:ILTypeRef,props) in attributes do 
              yield! encodeCustomAttrString tref.QualifiedNameWithNoShortPrimaryAssembly
              let bytes = 
                  [| yield! z_unsigned_int props.Length;
                      for (nm,typ,value) in props do 
                          yield! encodeCustomAttrNamedArg ilg (nm,typ,true,value)|]
              yield! z_unsigned_int bytes.Length;
              yield! bytes |]
              
    ILPermission.PermissionSet(action,bytes)


// Parse an IL type signature argument within a custom attribute blob
type ILTypeSigParser(tstring : string) =

    let mutable startPos = 0
    let mutable currentPos = 0

    let reset() = startPos <- 0 ; currentPos <- 0
    let nil = '\r' // cannot appear in a type sig

    // take a look at the next value, but don't advance
    let peek() = if currentPos < (tstring.Length-1) then tstring.[currentPos+1] else nil
    let peekN(skip) = if currentPos < (tstring.Length - skip) then tstring.[currentPos+skip] else nil
    // take a look at the current value, but don't advance
    let here() = if currentPos < tstring.Length then tstring.[currentPos] else nil
    // move on to the next character
    let step() = currentPos <- currentPos+1
    // ignore the current lexeme
    let skip() = startPos <- currentPos
    // ignore the current lexeme, advance
    let drop() = skip() ; step() ; skip()
    // return the current lexeme, advance
    let take() = 
        let s = if currentPos < tstring.Length then tstring.[startPos..currentPos] else ""
        drop()
        s

    // The format we accept is
    // "<type name>{`<arity>[<type>,+]}{<array rank>}{<scope>}"  E.g.,
    //
    // System.Collections.Generic.Dictionary
    //     `2[
    //         [System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],
    //         dev.virtualearth.net.webservices.v1.search.CategorySpecificPropertySet], 
    // mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
    //
    // Note that 
    //   Since we're only reading valid IL, we assume that the signature is properly formed
    //   For type parameters, if the type is non-local, it will be wrapped in brackets ([])
    //   Still needs testing with jagged arrays and byref parameters
    member private x.ParseType() =

        // Does the type name start with a leading '['?  If so, ignore it
        // (if the specialization type is in another module, it will be wrapped in bracket)
        if here() = '[' then drop()
    
        // 1. Iterate over beginning of type, grabbing the type name and determining if it's generic or an array
        let typeName = 
            while (peek() <> '`') && (peek() <> '[') && (peek() <> ']') && (peek() <> ',') && (peek() <> nil) do step()
            take()
    
        // 2. Classify the type

        // Is the type generic?
        let typeName, specializations = 
            if here() = '`' then
                drop() // step to the number
                // fetch the arity
                let arity = 
                    while (int(here()) >= (int('0'))) && (int(here()) <= ((int('9')))) && (int(peek()) >= (int('0'))) && (int(peek()) <= ((int('9')))) do step()
                    System.Int32.Parse(take())
                // skip the '['
                drop()
                // get the specializations
                typeName+"`"+(arity.ToString()), Some(([for _i in 0..arity-1 do yield x.ParseType()]))
            else
                typeName, None

        // Is the type an array?
        let rank = 
            if here() = '[' then
                let mutable rank = 0

                while here() <> ']' do
                    rank <- rank + 1
                    step()
                drop()

                Some(ILArrayShape(List.replicate rank (Some 0, None)))
            else
                None

        // Is there a scope?
        let scope = 
            if (here() = ',' || here() = ' ') && (peek() <> '[' && peekN(2) <> '[') then
                let grabScopeComponent() =
                    if here() = ',' then drop() // ditch the ','
                    if here() = ' ' then drop() // ditch the ' '

                    while (peek() <> ',' && peek() <> ']' && peek() <> nil) do step()
                    take()

                let scope =
                    [ yield grabScopeComponent() // assembly
                      yield grabScopeComponent() // version
                      yield grabScopeComponent() // culture
                      yield grabScopeComponent() // public key token
                    ] |> String.concat ","
                ILScopeRef.Assembly(ILAssemblyRef.FromAssemblyName(System.Reflection.AssemblyName(scope)))        
            else
                ILScopeRef.Local

        // strip any extraneous trailing brackets or commas
        if (here() = ']')  then drop()
        if (here() = ',') then drop()

        // build the IL type
        let tref = mkILTyRef(scope, typeName)
        let genericArgs = 
            match specializations with
            | None -> []
            | Some(genericArgs) -> genericArgs
        let tspec = ILTypeSpec.Create(tref,genericArgs)
        let ilty = 
            match tspec.Name with
            | "System.SByte"
            | "System.Byte"
            | "System.Int16"
            | "System.UInt16"
            | "System.Int32"
            | "System.UInt32"
            | "System.Int64"
            | "System.UInt64"
            | "System.Char"
            | "System.Double"
            | "System.Single"
            | "System.Boolean" -> ILType.Value(tspec)
            | _ -> ILType.Boxed(tspec)

        // if it's an array, wrap it - otherwise, just return the IL type
        match rank with
        | Some(r) -> ILType.Array(r,ilty)
        | _ -> ilty
    
    member x.ParseTypeSpec() =
        reset()
        let ilty = x.ParseType()
        ILAttribElem.Type(Some(ilty))

let decodeILAttribData (ilg: ILGlobals) (ca: ILAttribute) = 
    let bytes = ca.Data
    let sigptr = 0
    let bb0,sigptr = sigptr_get_byte bytes sigptr
    let bb1,sigptr = sigptr_get_byte bytes sigptr
    if not (bb0 = 0x01 && bb1 = 0x00) then failwith "decodeILAttribData: invalid data";

    let rec parseVal argty sigptr = 
      match argty with 
      | ILType.Value tspec when tspec.Name = "System.SByte" ->  
          let n,sigptr = sigptr_get_i8 bytes sigptr
          ILAttribElem.SByte n, sigptr
      | ILType.Value tspec when tspec.Name = "System.Byte" ->  
          let n,sigptr = sigptr_get_u8 bytes sigptr
          ILAttribElem.Byte n, sigptr
      | ILType.Value tspec when tspec.Name = "System.Int16" ->  
          let n,sigptr = sigptr_get_i16 bytes sigptr
          ILAttribElem.Int16 n, sigptr
      | ILType.Value tspec when tspec.Name = "System.UInt16" ->  
          let n,sigptr = sigptr_get_u16 bytes sigptr
          ILAttribElem.UInt16 n, sigptr
      | ILType.Value tspec when tspec.Name = "System.Int32" ->  
          let n,sigptr = sigptr_get_i32 bytes sigptr
          ILAttribElem.Int32 n, sigptr
      | ILType.Value tspec when tspec.Name = "System.UInt32" ->  
          let n,sigptr = sigptr_get_u32 bytes sigptr
          ILAttribElem.UInt32 n, sigptr
      | ILType.Value tspec when tspec.Name = "System.Int64" ->  
          let n,sigptr = sigptr_get_i64 bytes sigptr
          ILAttribElem.Int64 n, sigptr
      | ILType.Value tspec when tspec.Name = "System.UInt64" ->  
          let n,sigptr = sigptr_get_u64 bytes sigptr
          ILAttribElem.UInt64 n, sigptr
      | ILType.Value tspec when tspec.Name = "System.Double" ->  
          let n,sigptr = sigptr_get_ieee64 bytes sigptr
          ILAttribElem.Double n, sigptr
      | ILType.Value tspec when tspec.Name = "System.Single" ->  
          let n,sigptr = sigptr_get_ieee32 bytes sigptr
          ILAttribElem.Single n, sigptr
      | ILType.Value tspec when tspec.Name = "System.Char" ->  
          let n,sigptr = sigptr_get_u16 bytes sigptr
          ILAttribElem.Char (char (int32 n)), sigptr
      | ILType.Value tspec when tspec.Name = "System.Boolean" ->  
          let n,sigptr = sigptr_get_byte bytes sigptr
          ILAttribElem.Bool (not (n = 0)), sigptr
      | ILType.Boxed tspec when tspec.Name = "System.String" ->  
          let n,sigptr = sigptr_get_serstring_possibly_null bytes sigptr
          ILAttribElem.String n, sigptr
      | ILType.Boxed tspec when tspec.Name = "System.Type" ->  
          let nOpt,sigptr = sigptr_get_serstring_possibly_null bytes sigptr
          match nOpt with
          | None -> ILAttribElem.TypeRef(None), sigptr
          | Some n -> 
            try
                let parser = ILTypeSigParser(n)
                parser.ParseTypeSpec(),sigptr
            with e ->
                failwith (sprintf "decodeILAttribData: error parsing type in custom attribute blob: %s" e.Message)
      | ILType.Boxed tspec when tspec.Name = "System.Object" ->  
          let et,sigptr = sigptr_get_u8 bytes sigptr
          if et = 0xFFuy then 
              ILAttribElem.Null, sigptr
          else
              let ty,sigptr = decodeCustomAttrElemType ilg bytes sigptr et 
              parseVal ty sigptr 
      | ILType.Array(shape,elemTy) when shape = ILArrayShape.SingleDimensional ->  
          let n,sigptr = sigptr_get_i32 bytes sigptr
          if n = 0xFFFFFFFF then ILAttribElem.Null,sigptr else
          let rec parseElems acc n sigptr = 
            if n = 0 then List.rev acc else
            let v,sigptr = parseVal elemTy sigptr
            parseElems (v ::acc) (n-1) sigptr
          let elems = parseElems [] n sigptr
          ILAttribElem.Array(elemTy,elems), sigptr
      | ILType.Value _ ->  (* assume it is an enumeration *)
          let n,sigptr = sigptr_get_i32 bytes sigptr
          ILAttribElem.Int32 n, sigptr
      | _ ->  failwith "decodeILAttribData: attribute data involves an enum or System.Type value"
    let rec parseFixed argtys sigptr = 
      match argtys with 
        [] -> [],sigptr
      | h::t -> 
          let nh,sigptr = parseVal h sigptr
          let nt,sigptr = parseFixed t sigptr
          nh ::nt, sigptr
    let fixedArgs,sigptr = parseFixed ca.Method.FormalArgTypes sigptr
    let nnamed,sigptr = sigptr_get_u16 bytes sigptr
    let rec parseNamed acc n sigptr = 
      if n = 0 then List.rev acc else
      let isPropByte,sigptr = sigptr_get_u8 bytes sigptr
      let isProp = (int isPropByte = 0x54)
      let et,sigptr = sigptr_get_u8 bytes sigptr
      // We have a named value 
      let ty,sigptr = 
        if (0x50 = (int et) || 0x55 = (int et)) then
            let qualified_tname,sigptr = sigptr_get_serstring bytes sigptr
            let unqualified_tname, rest = 
                let pieces = qualified_tname.Split(',')
                if pieces.Length > 1 then 
                    pieces.[0], Some (String.concat "," pieces.[1..])
                else 
                    pieces.[0], None
            let scoref = 
                match rest with 
                | Some aname -> ILScopeRef.Assembly(ILAssemblyRef.FromAssemblyName(System.Reflection.AssemblyName(aname)))        
                | None -> ilg.primaryAssemblyScopeRef

            let tref = mkILTyRef (scoref,unqualified_tname)
            let tspec = mkILNonGenericTySpec tref
            ILType.Value(tspec),sigptr            
        else
            decodeCustomAttrElemType ilg bytes sigptr et
      let nm,sigptr = sigptr_get_serstring bytes sigptr
      let v,sigptr = parseVal ty sigptr
      parseNamed ((nm,ty,isProp,v) :: acc) (n-1) sigptr
    let named = parseNamed [] (int nnamed) sigptr
    fixedArgs,named     


// -------------------------------------------------------------------- 
// Functions to collect up all the references in a full module or
// assembly manifest.  The process also allocates
// a unique name to each unique internal assembly reference.
// -------------------------------------------------------------------- 

type ILReferences = 
    { AssemblyReferences: ILAssemblyRef list; 
      ModuleReferences: ILModuleRef list; }

type ILReferencesAccumulator = 
    { refsA: HashSet<ILAssemblyRef>; 
      refsM: HashSet<ILModuleRef>; }

let emptyILRefs = 
  { AssemblyReferences=[];
    ModuleReferences = []; }

(* Now find references. *)
let refs_of_assref (s:ILReferencesAccumulator) x = s.refsA.Add x |> ignore
let refs_of_modref (s:ILReferencesAccumulator) x = s.refsM.Add x |> ignore
    
let refs_of_scoref s x = 
    match x with 
    | ILScopeRef.Local -> () 
    | ILScopeRef.Assembly assref -> refs_of_assref s assref
    | ILScopeRef.Module modref -> refs_of_modref s modref  

let refs_of_tref s (x:ILTypeRef) = refs_of_scoref s x.Scope
  
let rec refs_of_typ s x = 
    match x with
    | ILType.Void |  ILType.TypeVar _ -> ()
    | ILType.Modified(_,ty1,ty2) -> refs_of_tref s ty1; refs_of_typ s ty2
    | ILType.Array (_,ty)
    | ILType.Ptr ty | ILType.Byref ty -> refs_of_typ s ty 
    | ILType.Value tr | ILType.Boxed tr -> refs_of_tspec s tr
    | ILType.FunctionPointer mref -> refs_of_callsig s mref 

and refs_of_inst s i = refs_of_typs s i
and refs_of_tspec s (x:ILTypeSpec) = refs_of_tref s x.TypeRef;  refs_of_inst s x.GenericArgs
and refs_of_callsig s csig  = refs_of_typs s csig.ArgTypes; refs_of_typ s csig.ReturnType
and refs_of_genparam s x = refs_of_typs s x.Constraints
and refs_of_genparams s b = List.iter (refs_of_genparam s) b
    
and refs_of_dloc s ts = refs_of_tref s ts
   
and refs_of_mref s (x:ILMethodRef) = 
    refs_of_dloc s x.EnclosingTypeRef  ;
    refs_of_typs s x.mrefArgs;
    refs_of_typ s x.mrefReturn
    
and refs_of_fref s x = refs_of_tref s x.EnclosingTypeRef; refs_of_typ s x.Type
and refs_of_ospec s (OverridesSpec(mref,ty)) = refs_of_mref s mref; refs_of_typ s ty 
and refs_of_mspec s (x: ILMethodSpec) = 
    refs_of_mref s x.MethodRef;
    refs_of_typ s x.EnclosingType;
    refs_of_inst s x.GenericArgs

and refs_of_fspec s x =
    refs_of_fref s x.FieldRef;
    refs_of_typ s x.EnclosingType

and refs_of_typs s l = List.iter (refs_of_typ s) l
  
and refs_of_token s x = 
    match x with
    | ILToken.ILType ty -> refs_of_typ s ty
    | ILToken.ILMethod mr -> refs_of_mspec s mr
    | ILToken.ILField fr -> refs_of_fspec s fr

and refs_of_custom_attr s x = refs_of_mspec s x.Method
    
and refs_of_custom_attrs s (cas : ILAttributes) = List.iter (refs_of_custom_attr s) cas.AsList
and refs_of_varargs s tyso = Option.iter (refs_of_typs s) tyso 
and refs_of_instr s x = 
    match x with
    | I_call (_,mr,varargs) | I_newobj (mr,varargs) | I_callvirt (_,mr,varargs) ->
        refs_of_mspec s mr;
        refs_of_varargs s varargs
    | I_callconstraint (_,tr,mr,varargs) -> 
        refs_of_typ s tr;
        refs_of_mspec s mr;
        refs_of_varargs s varargs
    | I_calli (_,callsig,varargs) ->  
        refs_of_callsig s callsig;  refs_of_varargs s varargs 
    | I_jmp mr | I_ldftn mr | I_ldvirtftn mr -> 
        refs_of_mspec s mr
    | I_ldsfld (_,fr) | I_ldfld (_,_,fr) | I_ldsflda fr | I_ldflda fr | I_stsfld (_,fr) | I_stfld (_,_,fr) -> 
        refs_of_fspec s fr
    | I_isinst ty | I_castclass ty | I_cpobj ty | I_initobj ty | I_ldobj (_,_,ty) 
    | I_stobj (_,_,ty) | I_box ty |I_unbox ty | I_unbox_any ty | I_sizeof ty
    | I_ldelem_any (_,ty) | I_ldelema (_,_,_,ty) |I_stelem_any (_,ty) | I_newarr (_,ty)
    | I_mkrefany ty | I_refanyval ty 
    | EI_ilzero ty ->   refs_of_typ s ty 
    | I_ldtoken token -> refs_of_token s token 
    | I_stelem _|I_ldelem _|I_ldstr _|I_switch _|I_stloc _|I_stind _
    | I_starg _|I_ldloca _|I_ldloc _|I_ldind _
    | I_ldarga _|I_ldarg _|I_leave _|I_br _
    | I_brcmp _|I_rethrow|I_refanytype|I_ldlen|I_throw|I_initblk _ |I_cpblk _ 
    | I_localloc|I_ret |I_endfilter|I_endfinally|I_arglist
    | I_break
    | AI_add    | AI_add_ovf | AI_add_ovf_un | AI_and    | AI_div    | AI_div_un | AI_ceq      | AI_cgt      | AI_cgt_un   | AI_clt     
    | AI_clt_un  | AI_conv      _ | AI_conv_ovf  _ | AI_conv_ovf_un  _ | AI_mul       | AI_mul_ovf    | AI_mul_ovf_un | AI_rem       | AI_rem_un       
    | AI_shl       | AI_shr       | AI_shr_un | AI_sub       | AI_sub_ovf   | AI_sub_ovf_un   | AI_xor       | AI_or        | AI_neg       | AI_not       
    | AI_ldnull    | AI_dup       | AI_pop | AI_ckfinite | AI_nop | AI_ldc       _
    | I_seqpoint _ | EI_ldlen_multi _ ->  ()
      
  
and refs_of_il_code s (c: ILCode)  = 
    c.Instrs |> Array.iter (refs_of_instr s) 
    c.Exceptions |> List.iter (fun e -> e.Clause |> (function 
        | ILExceptionClause.TypeCatch (ilty, _) -> refs_of_typ s ilty
        | _ -> ()))

and refs_of_ilmbody s (il: ILMethodBody) = 
    List.iter (refs_of_local s) il.Locals
    refs_of_il_code s il.Code 
    
and refs_of_local s loc = refs_of_typ s loc.Type
    
and refs_of_mbody s x = 
    match x with 
    | MethodBody.IL il -> refs_of_ilmbody s il
    | MethodBody.PInvoke (attr) -> refs_of_modref s attr.Where
    | _ -> ()

and refs_of_mdef s md = 
    List.iter (refs_of_param s) md.Parameters;
    refs_of_return s md.Return;
    refs_of_mbody s  md.mdBody.Contents;
    refs_of_custom_attrs s  md.CustomAttrs;
    refs_of_genparams s  md.GenericParams
    
and refs_of_param s p = refs_of_typ s p.Type 
and refs_of_return s (rt:ILReturn) = refs_of_typ s rt.Type
and refs_of_mdefs s x =  Seq.iter (refs_of_mdef s) x
    
and refs_of_event_def s (ed: ILEventDef) = 
    Option.iter (refs_of_typ s)  ed.Type ;
    refs_of_mref  s ed.AddMethod ;
    refs_of_mref  s ed.RemoveMethod;
    Option.iter (refs_of_mref s) ed.FireMethod ;
    List.iter (refs_of_mref s)  ed.OtherMethods ;
    refs_of_custom_attrs  s ed.CustomAttrs
    
and refs_of_events s (x: ILEventDefs) =  List.iter (refs_of_event_def s) x.AsList
    
and refs_of_property_def s pd = 
    Option.iter (refs_of_mref s)  pd.SetMethod ;
    Option.iter (refs_of_mref s)  pd.GetMethod ;
    refs_of_typ  s pd.Type ;
    refs_of_typs  s pd.Args ;
    refs_of_custom_attrs  s pd.CustomAttrs
    
and refs_of_properties s (x: ILPropertyDefs) = List.iter (refs_of_property_def s) x.AsList
    
and refs_of_fdef s fd = 
    refs_of_typ  s fd.Type;
    refs_of_custom_attrs  s fd.CustomAttrs

and refs_of_fields s fields = List.iter (refs_of_fdef s) fields
    
and refs_of_method_impls s mimpls =  List.iter (refs_of_method_impl s) mimpls
    
and refs_of_method_impl s m = 
    refs_of_ospec s m.Overrides;
    refs_of_mspec s m.OverrideBy

and refs_of_tdef_kind _s _k =  ()
  
and refs_of_tdef s (td : ILTypeDef)  =  
    refs_of_types s td.NestedTypes;
    refs_of_genparams s  td.GenericParams;
    refs_of_typs  s td.Implements;
    Option.iter (refs_of_typ s) td.Extends;
    refs_of_mdefs        s td.Methods;
    refs_of_fields       s td.Fields.AsList;
    refs_of_method_impls s td.MethodImpls.AsList;
    refs_of_events       s td.Events;
    refs_of_tdef_kind    s td.tdKind;
    refs_of_custom_attrs s td.CustomAttrs;
    refs_of_properties   s td.Properties

and refs_of_string _s _ = ()
and refs_of_types s (types: ILTypeDefs) = Seq.iter  (refs_of_tdef s) types
    
and refs_of_exported_type s (c: ILExportedTypeOrForwarder) = 
    refs_of_custom_attrs s c.CustomAttrs
    
and refs_of_exported_types s (tab: ILExportedTypesAndForwarders) = List.iter (refs_of_exported_type s) tab.AsList
    
and refs_of_resource_where s x = 
    match x with 
    | ILResourceLocation.Local _ -> ()
    | ILResourceLocation.File (mref,_) -> refs_of_modref s mref
    | ILResourceLocation.Assembly aref -> refs_of_assref s aref

and refs_of_resource s x = 
    refs_of_resource_where s x.Location;
    refs_of_custom_attrs s x.CustomAttrs
    
and refs_of_resources s (tab: ILResources) = List.iter (refs_of_resource s) tab.AsList
    
and refs_of_modul s m = 
    refs_of_types s m.TypeDefs;
    refs_of_resources s m.Resources;
    Option.iter (refs_of_manifest s) m.Manifest
    
and refs_of_manifest s m = 
    refs_of_custom_attrs s m.CustomAttrs;
    refs_of_exported_types s m.ExportedTypes

let computeILRefs modul = 
    let s = 
      { refsA = HashSet<_>(HashIdentity.Structural) 
        refsM = HashSet<_>(HashIdentity.Structural) }

    refs_of_modul s modul;
    { AssemblyReferences = Seq.fold (fun acc x -> x::acc) [] s.refsA
      ModuleReferences =  Seq.fold (fun acc x -> x::acc) [] s.refsM }

let tspan = System.TimeSpan(System.DateTime.Now.Ticks - System.DateTime(2000,1,1).Ticks)

let parseILVersion (vstr : string) = 
    // matches "v1.2.3.4" or "1.2.3.4". Note, if numbers are missing, returns -1 (not 0).
    let mutable vstr = vstr.TrimStart [|'v'|] 
    // if the version string contains wildcards, replace them
    let versionComponents = vstr.Split([|'.'|])
    
    // account for wildcards
    if versionComponents.Length > 2 then
      let defaultBuild = (uint16)tspan.Days % System.UInt16.MaxValue - 1us
      let defaultRevision = (uint16)(System.DateTime.Now.TimeOfDay.TotalSeconds / 2.0) % System.UInt16.MaxValue - 1us
      if versionComponents.[2] = "*" then
        if versionComponents.Length > 3 then
          failwith "Invalid version format"
        else
          // set the build number to the number of days since Jan 1, 2000
          versionComponents.[2] <- defaultBuild.ToString() ;
          // Set the revision number to number of seconds today / 2
          vstr <- System.String.Join(".",versionComponents) + "." + defaultRevision.ToString() ;
      elif versionComponents.Length > 3 && versionComponents.[3] = "*" then
        // Set the revision number to number of seconds today / 2
        versionComponents.[3] <- defaultRevision.ToString() ;
        vstr <- System.String.Join(".",versionComponents) ;
        
    let version = System.Version(vstr)
    let zero32 n = if n < 0 then 0us else uint16(n)
    // since the minor revision will be -1 if none is specified, we need to truncate to 0 to not break existing code
    let minorRevision = if version.Revision = -1 then 0us else uint16(version.MinorRevision)   
    (zero32 version.Major, zero32 version.Minor, zero32 version.Build, minorRevision);;


let compareILVersions (a1,a2,a3,a4) ((b1,b2,b3,b4) : ILVersionInfo) = 
    let c = compare a1 b1
    if c <> 0 then c else
    let c = compare a2 b2
    if c <> 0 then c else
    let c = compare a3 b3
    if c <> 0 then c else
    let c = compare a4 b4
    if c <> 0 then c else
    0


let resolveILMethodRefWithRescope r td (mref:ILMethodRef) = 
    let args = mref.ArgTypes
    let nargs = args.Length
    let nm = mref.Name
    let possibles = td.Methods.FindByNameAndArity (nm,nargs)
    if isNil possibles then failwith ("no method named " + nm + " found in type " + td.Name)
    match 
      possibles |> List.filter (fun md -> 
          mref.CallingConv = md.CallingConv &&
          // REVIEW: this uses equality on ILType.  For CMOD_OPTIONAL this is not going to be correct
          (md.Parameters,mref.ArgTypes) ||> List.lengthsEqAndForall2 (fun p1 p2 -> r p1.Type = p2) &&
          // REVIEW: this uses equality on ILType.  For CMOD_OPTIONAL this is not going to be correct 
          r md.Return.Type = mref.ReturnType) with 
    | [] -> failwith ("no method named "+nm+" with appropriate argument types found in type "+td.Name)
    | [mdef] ->  mdef
    | _ -> failwith ("multiple methods named "+nm+" appear with identical argument types in type "+td.Name)
        
let resolveILMethodRef td mref = resolveILMethodRefWithRescope id td mref

let mkRefToILModule m =
  ILModuleRef.Create(m.Name, true, None)


let ungenericizeTypeName n = 
  let sym = '`'
  if 
    String.contains n sym && 
      (* check what comes after the symbol is a number *)
    (let m = String.rindex n sym
     let res = ref (m < n.Length - 1)
     for i = m + 1 to n.Length - 1 do
       res := !res && n.[i] >= '0' && n.[i] <= '9';
     !res)
  then 
      let pos = String.rindex n sym
      String.sub n 0 pos
  else n


type ILEventRef =
    { erA: ILTypeRef; erB: string }
    static member Create(a,b) = {erA=a;erB=b}
    member x.EnclosingTypeRef = x.erA
    member x.Name = x.erB

type ILPropertyRef =
    { prA: ILTypeRef; prB: string }
    static member Create (a,b) = {prA=a;prB=b}
    member x.EnclosingTypeRef = x.prA
    member x.Name = x.prB
