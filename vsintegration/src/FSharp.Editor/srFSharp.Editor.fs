// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace Microsoft.VisualStudio.FSharp.Editor

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
    let ImplementInterface = lazy ( GetString "ImplementInterface" ) 
    let ImplementInterfaceWithoutTypeAnnotation = lazy ( GetString "ImplementInterfaceWithoutTypeAnnotation" ) 
    let SimplifyName = lazy ( GetString "SimplifyName")
    let NameCanBeSimplified = lazy ( GetString "NameCanBeSimplified")
    let FSharpFunctionsOrMethodsClassificationType = lazy (GetString "FSharpFunctionsOrMethodsClassificationType")
    let FSharpMutableVarsClassificationType = lazy (GetString "FSharpMutableVarsClassificationType")
    let FSharpPrintfFormatClassificationType = lazy (GetString "FSharpPrintfFormatClassificationType")
    let FSharpPropertiesClassificationType = lazy (GetString "FSharpPropertiesClassificationType")
    let FSharpDisposablesClassificationType = lazy (GetString "FSharpDisposablesClassificationType")
    let RemoveUnusedOpens = lazy (GetString "RemoveUnusedOpens")
    let UnusedOpens = lazy (GetString "UnusedOpens")