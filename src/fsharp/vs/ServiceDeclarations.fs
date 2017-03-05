// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

//----------------------------------------------------------------------------
// Open up the compiler as an incremental service for parsing,
// type checking and intellisense-like environment-reporting.
//--------------------------------------------------------------------------

namespace Microsoft.FSharp.Compiler.SourceCodeServices

open System

open Microsoft.FSharp.Compiler 
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library  
open Microsoft.FSharp.Compiler.AbstractIL.Diagnostics 

open Microsoft.FSharp.Compiler.Layout
open Microsoft.FSharp.Compiler.Layout.TaggedTextOps
open Microsoft.FSharp.Compiler.Lib
open Microsoft.FSharp.Compiler.PrettyNaming
open Microsoft.FSharp.Compiler.Tast
open Microsoft.FSharp.Compiler.Tastops
open Microsoft.FSharp.Compiler.Infos
open Microsoft.FSharp.Compiler.NameResolution
open Microsoft.FSharp.Compiler.InfoReader

/// An intellisense declaration
[<Sealed>]
type FSharpDeclarationListItem(name: string, nameInCode: string, glyph: FSharpGlyph, info, isAttribute: bool, accessibility: FSharpAccessibility option) =
    let mutable descriptionTextHolder:FSharpToolTipText<_> option = None
    let mutable task = null

    member decl.Name = name
    member decl.NameInCode = nameInCode

    member decl.StructuredDescriptionTextAsync = 
            match info with
            | Choice1Of2 (items, infoReader, m, denv, reactor:IReactorOperations, checkAlive) -> 
                    // reactor causes the lambda to execute on the background compiler thread, through the Reactor
                    reactor.EnqueueAndAwaitOpAsync ("StructuredDescriptionTextAsync", fun ctok -> 
                         RequireCompilationThread ctok
                          // This is where we do some work which may touch TAST data structures owned by the IncrementalBuilder - infoReader, item etc. 
                          // It is written to be robust to a disposal of an IncrementalBuilder, in which case it will just return the empty string. 
                          // It is best to think of this as a "weak reference" to the IncrementalBuilder, i.e. this code is written to be robust to its
                          // disposal. Yes, you are right to scratch your head here, but this is ok.
                         cancellable.Return(
                              if checkAlive() then FSharpToolTipText(items |> Seq.toList |> List.map (ItemDescriptionsImpl.FormatStructuredDescriptionOfItem true infoReader m denv))
                              else FSharpToolTipText [ FSharpStructuredToolTipElement.Single(wordL (tagText (FSComp.SR.descriptionUnavailable())), FSharpXmlDoc.None) ]))
            | Choice2Of2 result -> 
                async.Return result

    member decl.DescriptionTextAsync = 
        decl.StructuredDescriptionTextAsync
        |> Tooltips.Map Tooltips.ToFSharpToolTipText

    member decl.StructuredDescriptionText = 
        match descriptionTextHolder with
        | Some descriptionText -> descriptionText
        | None ->
            match info with
            | Choice1Of2 _ -> 

                // The dataTipSpinWaitTime limits how long we block the UI thread while a tooltip pops up next to a selected item in an IntelliSense completion list.
                // This time appears to be somewhat amortized by the time it takes the VS completion UI to actually bring up the tooltip after selecting an item in the first place.
                if isNull task then
                    // kick off the actual (non-cooperative) work
                    task <- System.Threading.Tasks.Task.Factory.StartNew(fun() -> 
                        let text = decl.StructuredDescriptionTextAsync |> Async.RunSynchronously
                        descriptionTextHolder <- Some text) 

                // The dataTipSpinWaitTime limits how long we block the UI thread while a tooltip pops up next to a selected item in an IntelliSense completion list.
                // This time appears to be somewhat amortized by the time it takes the VS completion UI to actually bring up the tooltip after selecting an item in the first place.
                task.Wait EnvMisc2.dataTipSpinWaitTime  |> ignore
                match descriptionTextHolder with 
                | Some text -> text
                | None -> FSharpToolTipText [ FSharpStructuredToolTipElement.Single(wordL (tagText (FSComp.SR.loadingDescription())), FSharpXmlDoc.None) ]

            | Choice2Of2 result -> 
                result

    member decl.DescriptionText = decl.StructuredDescriptionText |> Tooltips.ToFSharpToolTipText
    member decl.Glyph = glyph 
    member decl.IsAttribute = isAttribute
    member decl.Accessibility = accessibility
      
/// A table of declarations for Intellisense completion 
[<Sealed>]
type FSharpDeclarationListInfo(declarations: FSharpDeclarationListItem[]) = 
    member self.Items = declarations

    // Make a 'Declarations' object for a set of selected items
    static member Create(infoReader:InfoReader, m, denv, ccu, tcImports, items, reactor, checkAlive) = 
        let g = infoReader.g
        let items = items |> ItemDescriptionsImpl.RemoveExplicitlySuppressed g
        
        // Sort by name. For things with the same name, 
        //     - show types with fewer generic parameters first
        //     - show types before over other related items - they usually have very useful XmlDocs 
        let items = 
            items |> List.sortBy (fun x -> 
                let name = 
                    match x with  
                    | Item.Types (_,(TType_app(tcref,_) :: _)) -> 1 + tcref.TyparsNoRange.Length
                    // Put delegate ctors after types, sorted by #typars. RemoveDuplicateItems will remove FakeInterfaceCtor and DelegateCtor if an earlier type is also reported with this name
                    | Item.FakeInterfaceCtor (TType_app(tcref,_)) 
                    | Item.DelegateCtor (TType_app(tcref,_)) -> 1000 + tcref.TyparsNoRange.Length
                    // Put type ctors after types, sorted by #typars. RemoveDuplicateItems will remove DefaultStructCtors if a type is also reported with this name
                    | Item.CtorGroup (_, (cinfo :: _)) -> 1000 + 10 * (tcrefOfAppTy g cinfo.EnclosingType).TyparsNoRange.Length 
                    | _ -> 0
                x.DisplayName, name)

        // Remove all duplicates. We've put the types first, so this removes the DelegateCtor and DefaultStructCtor's.
        let items = items |> ItemDescriptionsImpl.RemoveDuplicateItems g

        if verbose then dprintf "service.ml: mkDecls: %d found groups after filtering\n" (List.length items); 

        // Group by display name
        let items = items |> List.groupBy (fun x -> x.DisplayName) 

        // Filter out operators (and list)
        let items = 
            // Check whether this item looks like an operator.
            let isOperatorItem(name, item) = 
                match item with 
                | [Item.Value _]
                | [Item.MethodGroup _ ] -> IsOperatorName name
                | [Item.UnionCase _] -> IsOperatorName name
                | _ -> false              

            let isFSharpList name = (name = "[]") // list shows up as a Type and a UnionCase, only such entity with a symbolic name, but want to filter out of intellisense

            items |> List.filter (fun (name, items) -> not (isOperatorItem(name, items)) && not (isFSharpList name)) 
            
        let decls = 
            // Filter out duplicate names
            items |> List.map (fun (nm,itemsWithSameName) -> 
                match itemsWithSameName with
                | [] -> failwith "Unexpected empty bag"
                | items -> 
                    let glyph = ItemDescriptionsImpl.GlyphOfItem(denv, items.Head)
                    let name, nameInCode =
                        if nm.StartsWith "( " && nm.EndsWith " )" then
                            let cleanName = nm.[2..nm.Length - 3]
                            cleanName, 
                            if IsOperatorName nm then cleanName else "``" + cleanName + "``"
                        else nm, nm

                    FSharpDeclarationListItem(
                        name, nameInCode, glyph, Choice1Of2 (items, infoReader, m, denv, reactor, checkAlive), 
                        ItemDescriptionsImpl.IsAttribute infoReader items.Head, FSharpSymbol.Create(g, ccu, tcImports, items.Head).Accessibility))

        new FSharpDeclarationListInfo(Array.ofList decls)
    
    static member Error msg = 
        new FSharpDeclarationListInfo(
                [| FSharpDeclarationListItem("<Note>", "<Note>", FSharpGlyph.Error, Choice2Of2 (FSharpToolTipText [FSharpStructuredToolTipElement.CompositionError msg]), false, None) |])
    
    static member Empty = FSharpDeclarationListInfo([| |])
