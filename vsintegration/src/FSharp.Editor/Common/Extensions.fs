// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
[<AutoOpen>]
/// Type and Module Extensions
module internal Microsoft.VisualStudio.FSharp.Editor.Extensions

open System
open System.IO
open System.Runtime.CompilerServices
open Microsoft.CodeAnalysis
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio


type Path with
    static member GetFullPathSafe path =
        try Path.GetFullPath path
        with _ -> path

    static member GetFileNameSafe path =
        try Path.GetFileName path
        with _ -> path

type System.IServiceProvider with
    member self.GetService<'T>() = self.GetService(typeof<'T>) :?> 'T
    member self.GetService<'S, 'T>() = self.GetService(typeof<'S>) :?> 'T



type [<Extension>] SRTPExtension () =
    /// SRTP Extension method that is added to any type that implements a TryGetValue method
    /// returns an option instead
    [<Extension>] static member inline TryGet (collection, key) = tryGet key collection

type FSharpNavigationDeclarationItem with
    member self.RoslynGlyph : Glyph =
        match self.Glyph with
        | FSharpGlyph.Class
        | FSharpGlyph.Typedef
        | FSharpGlyph.Type
        | FSharpGlyph.Exception ->
            match self.Access with
            | Some SynAccess.Private -> Glyph.ClassPrivate
            | Some SynAccess.Internal -> Glyph.ClassInternal
            | _ -> Glyph.ClassPublic
        | FSharpGlyph.Constant -> 
            match self.Access with
            | Some SynAccess.Private -> Glyph.ConstantPrivate
            | Some SynAccess.Internal -> Glyph.ConstantInternal
            | _ -> Glyph.ConstantPublic
        | FSharpGlyph.Delegate -> 
            match self.Access with
            | Some SynAccess.Private -> Glyph.DelegatePrivate
            | Some SynAccess.Internal -> Glyph.DelegateInternal
            | _ -> Glyph.DelegatePublic
        | FSharpGlyph.Union
        | FSharpGlyph.Enum -> 
            match self.Access with
            | Some SynAccess.Private -> Glyph.EnumPrivate
            | Some SynAccess.Internal -> Glyph.EnumInternal
            | _ -> Glyph.EnumPublic
        | FSharpGlyph.EnumMember
        | FSharpGlyph.Variable
        | FSharpGlyph.Field -> 
            match self.Access with
            | Some SynAccess.Private -> Glyph.FieldPrivate
            | Some SynAccess.Internal -> Glyph.FieldInternal
            | _ -> Glyph.FieldPublic
        | FSharpGlyph.Event -> 
            match self.Access with
            | Some SynAccess.Private -> Glyph.EventPrivate
            | Some SynAccess.Internal -> Glyph.EventInternal
            | _ -> Glyph.EventPublic
        | FSharpGlyph.Interface -> 
            match self.Access with
            | Some SynAccess.Private -> Glyph.InterfacePrivate
            | Some SynAccess.Internal -> Glyph.InterfaceInternal
            | _ -> Glyph.InterfacePublic
        | FSharpGlyph.Method
        | FSharpGlyph.OverridenMethod -> 
            match self.Access with
            | Some SynAccess.Private -> Glyph.MethodPrivate
            | Some SynAccess.Internal -> Glyph.MethodInternal
            | _ -> Glyph.MethodPublic
        | FSharpGlyph.Module -> 
            match self.Access with
            | Some SynAccess.Private -> Glyph.ModulePrivate
            | Some SynAccess.Internal -> Glyph.ModuleInternal
            | _ -> Glyph.ModulePublic
        | FSharpGlyph.NameSpace -> Glyph.Namespace
        | FSharpGlyph.Property -> 
            match self.Access with
            | Some SynAccess.Private -> Glyph.PropertyPrivate
            | Some SynAccess.Internal -> Glyph.PropertyInternal
            | _ -> Glyph.PropertyPublic
        | FSharpGlyph.Struct -> 
            match self.Access with
            | Some SynAccess.Private -> Glyph.StructurePrivate
            | Some SynAccess.Internal -> Glyph.StructureInternal
            | _ -> Glyph.StructurePublic
        | FSharpGlyph.ExtensionMethod ->
            match self.Access with
            | Some SynAccess.Private -> Glyph.ExtensionMethodPrivate
            | Some SynAccess.Internal -> Glyph.ExtensionMethodInternal
            | _ -> Glyph.ExtensionMethodPublic
        | FSharpGlyph.Error -> Glyph.Error




type IVsHierarchy with

    member self.TryGetItemProperty<'t> (itemId:uint32,propertyId:int) : 't option =
        match self.GetProperty (itemId, propertyId) with
        | _, (:? 't as property) -> Some property | _ -> None


    member self.TryGetItemProperty<'t> (itemId:uint32,propertyId:__VSHPROPID) : 't option =
        self.TryGetItemProperty<'t> (itemId, int propertyId)


    member self.TryGetProperty<'t> (propertyId:__VSHPROPID) : 't option =
        self.TryGetProperty<'t> propertyId 


    member self.TryGetProperty<'t> (propertyId:int) =
        self.TryGetItemProperty (VSConstants.VSITEMID_ROOT, propertyId ) : 't option


    member self.TryGetGuidProperty (propertyId:int) : Guid option =
        match self.GetGuidProperty (VSConstants.VSITEMID_ROOT, propertyId) with
        | VSConstants.S_OK, guid -> Some guid | _ -> None
    
    
    member self.TryGetGuidProperty (propertyId:__VSHPROPID) : Guid option =
       match  self.GetGuidProperty (VSConstants.VSITEMID_ROOT, int propertyId) with 
       | VSConstants.S_OK, guid -> Some guid | _ -> None
    
    
    member self.TryGetTypeGuid () : Guid option =
        self.TryGetGuidProperty __VSHPROPID.VSHPROPID_TypeGuid
 
 
    member self.TryGetProjectGuid () : Guid option =
        self.TryGetGuidProperty __VSHPROPID.VSHPROPID_ProjectIDGuid


    member self.TryGetProject () : EnvDTE.Project option =
        self.TryGetProperty<EnvDTE.Project> __VSHPROPID.VSHPROPID_ExtObject


    member self.TryGetProjectDir () : string option =
        self.TryGetProperty __VSHPROPID.VSHPROPID_ProjectDir
    

    member self.TryGetName () : string option =
        self.TryGetProperty __VSHPROPID.VSHPROPID_Name
    

    member self.TryGetItemName (itemId:uint32) : string option =
        self.TryGetItemProperty (itemId, __VSHPROPID.VSHPROPID_Name)


    member self.TryGetCanonicaltName (itemId:uint32) : string option =
        match self.GetCanonicalName itemId with
        | VSConstants.S_OK, name -> Some name | _ -> None
    

    member self.TryGetParentHierarchy () : IVsHierarchy option =
        self.TryGetProperty __VSHPROPID.VSHPROPID_ParentHierarchy
        

    member self.TryGetOutputAssemblyPath () = maybe {
        let! project = self.TryGetProject ()         
        let getProperty tag = 
            try Some (project.Properties.[tag].Value.ToString ()) 
            with _ -> None        
        let! fullPath = getProperty "FullPath"
        let! outputPath = 
            try Some (project.ConfigurationManager.ActiveConfiguration.Properties.["OutputPath"].Value.ToString()) 
            with _ -> None
        let! outputFileName = getProperty "OutputFileName" 
        return Path.Combine (fullPath, outputPath, outputFileName) |> Path.GetFullPath
    }


    member self.GetFilesOnDisk () =
        self.TryGetProject () |> Option.map (fun project -> 
            [ for pi in project.ProjectItems -> pi.Document.Path ]
        ) |> Option.defaultValue []


    member self.TryGetFilePath () =
        self.TryGetProject () |> Option.map (fun project -> project.FileName)


    member self.GetReferencedProjects () = 
        maybe {
            let! (project:EnvDTE.Project) = self.TryGetProperty<EnvDTE.Project> __VSHPROPID.VSHPROPID_ExtObject
            let references = (project :?> VSLangProj.VSProject).References |> Seq.cast<VSLangProj.Reference>
            return references |> Seq.choose (Option.ofObj >> Option.bind (fun r -> try Option.ofObj r.SourceProject with _ -> None))
        } |> Option.defaultValue Seq.empty
    

    //member self.GetReferencedProjectIds () = 
    //    self.GetReferencedProjects () |> Seq.map (fun projRef -> projRef.)



[<RequireQualifiedAccess>]
module String =   

    let getLines (str: string) =
        use reader = new StringReader(str)
        [|  let mutable line = reader.ReadLine()
            while not (isNull line) do
                yield line
                line <- reader.ReadLine()
            if str.EndsWith("\n") then
            // last trailing space not returned
            // http://stackoverflow.com/questions/19365404/stringreader-omits-trailing-linebreak
                yield String.Empty
        |]


[<RequireQualifiedAccess>]
module Option =

    let guard (x: bool) : Option<unit> =
        if x then Some() else None

    let attempt (f: unit -> 'T) = try Some <| f() with _ -> None

    let inline ofNull value =
        if obj.ReferenceEquals(value, null) then None else Some value

    /// Gets the option if Some x, otherwise try to get another value
    let inline orTry f =
        function
        | Some x -> Some x
        | None -> f()

    /// Gets the value if Some x, otherwise try to get another value by calling a function
    let inline getOrTry f =
        function
        | Some x -> x
        | None -> f()


[<RequireQualifiedAccess>]
module List =
    let foldi (folder : 'State -> int -> 'T -> 'State) (state : 'State) (xs : 'T list) =
        let mutable state = state
        let mutable i = 0
        for x in xs do
            state <- folder state i x
            i <- i + 1
        state


[<RequireQualifiedAccess>]
module Seq =
    open System.Collections.Immutable

    let toImmutableArray (xs: seq<'a>) : ImmutableArray<'a> = xs.ToImmutableArray()


[<RequireQualifiedAccess>]
module Array =
    /// Optimized arrays equality. ~100x faster than `array1 = array2` on strings.
    /// ~2x faster for floats
    /// ~0.8x slower for ints
    let areEqual (xs: 'T []) (ys: 'T []) =
        match xs, ys with
        | null, null -> true
        | [||], [||] -> true
        | null, _ | _, null -> false
        | _ when xs.Length <> ys.Length -> false
        | _ ->
            let mutable break' = false
            let mutable i = 0
            let mutable result = true
            while i < xs.Length && not break' do
                if xs.[i] <> ys.[i] then 
                    break' <- true
                    result <- false
                i <- i + 1
            result
    
    /// check if subArray is found in the wholeArray starting 
    /// at the provided index
    let isSubArray (subArray: 'T []) (wholeArray:'T []) index = 
        if isNull subArray || isNull wholeArray then false
        elif subArray.Length = 0 then true
        elif subArray.Length > wholeArray.Length then false
        elif subArray.Length = wholeArray.Length then areEqual subArray wholeArray else
        let rec loop subidx idx =
            if subidx = subArray.Length then true 
            elif subArray.[subidx] = wholeArray.[idx] then loop (subidx+1) (idx+1) 
            else false
        loop 0 index
        
    /// Returns true if one array has another as its subset from index 0.
    let startsWith (prefix: _ []) (whole: _ []) =
        isSubArray prefix whole 0
        
    /// Returns true if one array has trailing elements equal to another's.
    let endsWith (suffix: _ []) (whole: _ []) =
        isSubArray suffix whole (whole.Length-suffix.Length)