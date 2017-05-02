// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.ComponentModel
open System.Diagnostics

[<RequireQualifiedAccess>]
module SR =
    let private resources = lazy (new System.Resources.ResourceManager("FSharp.Editor", System.Reflection.Assembly.GetExecutingAssembly()))

    let GetString(name:string) =        
        let s = resources.Value.GetString(name, System.Globalization.CultureInfo.CurrentUICulture)
#if DEBUG
        if null = s then System.Diagnostics.Debug.Assert(false, sprintf "**RESOURCE ERROR**: Resource token %s does not exist!" name)
#endif
        s.Replace(@"\n", System.Environment.NewLine)

    //    Sigh!!!!  We do this because at the moment we don't have a tool to generate the SR from a resx file
    let AddNewKeyword = lazy ( GetString "AddNewKeyword" )                                   // "Add 'new' keyword"
    let PrefixValueNameWithUnderscore = lazy ( GetString "PrefixValueNameWithUnderscore" )   // "Prefix value name with underscore"
    let RenameValueToUnderscore = lazy ( GetString "RenameValueToUnderscore" )               // "Rename value to '_'"
    let RenameValueToDoubleUnderscore = lazy ( GetString "RenameValueToDoubleUnderscore" )               // "Rename value to '__'"
    let ImplementInterface = lazy ( GetString "ImplementInterface" ) 
    let ImplementInterfaceWithoutTypeAnnotation = lazy ( GetString "ImplementInterfaceWithoutTypeAnnotation" ) 
    let SimplifyName = lazy ( GetString "SimplifyName")
    let NameCanBeSimplified = lazy ( GetString "NameCanBeSimplified")
    let FSharpFunctionsOrMethodsClassificationType = lazy (GetString "FSharpFunctionsOrMethodsClassificationType")
    let FSharpMutableVarsClassificationType = lazy (GetString "FSharpMutableVarsClassificationType")
    let FSharpPrintfFormatClassificationType = lazy (GetString "FSharpPrintfFormatClassificationType")
    let FSharpPropertiesClassificationType = lazy (GetString "FSharpPropertiesClassificationType")
    let FSharpDisposablesClassificationType = lazy (GetString "FSharpDisposablesClassificationType")
    let TheValueIsUnused = lazy (GetString "TheValueIsUnused")
    let RemoveUnusedOpens = lazy (GetString "RemoveUnusedOpens")
    let UnusedOpens = lazy (GetString "UnusedOpens")
    let AddProjectReference = lazy (GetString "AddProjectReference")
    let AddAssemblyReference = lazy (GetString "AddAssemblyReference")
    let NavigatingTo = lazy (GetString "NavigatingTo")
    let CannotDetermineSymbol = lazy (GetString "CannotDetermineSymbol")
    let CannotNavigateUnknown = lazy (GetString "CannotNavigateUnknown")
    let LocatingSymbol = lazy (GetString "LocatingSymbol")
    let NavigateToFailed = lazy (GetString "NavigateToFailed")
    let ExceptionsLabel = lazy (GetString "ExceptionsHeader")
    let GenericParametersLabel = lazy (GetString "GenericParametersHeader")
    
    //--------------------------------------------------------------------------------------
    // Attributes used to mark up editable properties 
    
    [<AttributeUsage(AttributeTargets.All)>]
    type internal SRDescriptionAttribute(description:string) =
        inherit DescriptionAttribute(description)
        let mutable replaced = false
    
        override x.Description = 
            if not (replaced) then 
                replaced <- true
                x.DescriptionValue <- GetString(base.Description)
            base.Description
    
    [<AttributeUsage(AttributeTargets.All)>]
    type internal SRCategoryAttribute(category:string) =
        inherit CategoryAttribute(category)
        override x.GetLocalizedString(value:string) =
            GetString(value)
    
    [<AttributeUsage(AttributeTargets.Class ||| AttributeTargets.Property ||| AttributeTargets.Field, Inherited = false, AllowMultiple = false)>]
    type internal SRDisplayNameAttribute(name:string) =  
        inherit DisplayNameAttribute()
    
        override x.DisplayName = 
          match GetString(name) with 
          | null -> 
              Debug.Assert(false, "String resource '" + name + "' is missing")
              name
          | result -> result
