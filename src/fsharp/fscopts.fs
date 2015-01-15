// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// # FSComp.SR.opts

module internal Microsoft.FSharp.Compiler.Fscopts

open Internal.Utilities
open System
open System.Collections.Generic
open Microsoft.FSharp.Compiler 
open Microsoft.FSharp.Compiler.AbstractIL 
open Microsoft.FSharp.Compiler.AbstractIL.IL
open Microsoft.FSharp.Compiler.AbstractIL.Internal 
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library 
open Microsoft.FSharp.Compiler.AbstractIL.Extensions.ILX
open Microsoft.FSharp.Compiler.AbstractIL.Diagnostics
open Microsoft.FSharp.Compiler.Build
open Microsoft.FSharp.Compiler.Env
open Microsoft.FSharp.Compiler.TypeChecker
open Microsoft.FSharp.Compiler.Tast
open Microsoft.FSharp.Compiler.Tastops 
open Microsoft.FSharp.Compiler.Tastops.DebugPrint 
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.ErrorLogger
open Microsoft.FSharp.Compiler.AbstractIL.IL
open Microsoft.FSharp.Compiler.Lib
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.Lexhelp
#if NO_COMPILER_BACKEND
#else
open Microsoft.FSharp.Compiler.Ilxgen
#endif


module Attributes = 
    open System.Runtime.CompilerServices

    //[<assembly: System.Security.SecurityTransparent>]
    [<Dependency("FSharp.Core",LoadHint.Always)>] 
    do()

let lexFilterVerbose = false
let mutable enableConsoleColoring = true // global state

let setFlag r n = 
    match n with 
    | 0 -> r false
    | 1 -> r true
    | _ -> raise (Failure "expected 0/1")

let SetOptimizeOff(tcConfigB : TcConfigBuilder) = 
    tcConfigB.optSettings <- { tcConfigB.optSettings with jitOptUser = Some false }
    tcConfigB.optSettings <- { tcConfigB.optSettings with localOptUser = Some false }
    tcConfigB.optSettings <- { tcConfigB.optSettings with crossModuleOptUser = Some false }
    tcConfigB.optSettings <- { tcConfigB.optSettings with lambdaInlineThreshold = 0 }
    tcConfigB.ignoreSymbolStoreSequencePoints <- false;
    tcConfigB.doDetuple <- false; 
    tcConfigB.doTLR <- false;
    tcConfigB.doFinalSimplify <- false;

let SetOptimizeOn(tcConfigB : TcConfigBuilder) =    
    tcConfigB.optSettings <- { tcConfigB.optSettings with jitOptUser = Some true }
    tcConfigB.optSettings <- { tcConfigB.optSettings with localOptUser = Some true }
    tcConfigB.optSettings <- { tcConfigB.optSettings with crossModuleOptUser = Some true }
    tcConfigB.optSettings <- { tcConfigB.optSettings with lambdaInlineThreshold = 6 }

    tcConfigB.ignoreSymbolStoreSequencePoints <- true;
    tcConfigB.doDetuple <- true;  
    tcConfigB.doTLR <- true;
    tcConfigB.doFinalSimplify <- true;

let SetOptimizeSwitch (tcConfigB : TcConfigBuilder) switch = 
    if (switch = On) then SetOptimizeOn(tcConfigB) else SetOptimizeOff(tcConfigB)
        
let SetTailcallSwitch (tcConfigB : TcConfigBuilder) switch =
    tcConfigB.emitTailcalls <- (switch = On)
        
let jitoptimizeSwitch (tcConfigB : TcConfigBuilder) switch =
    tcConfigB.optSettings <- { tcConfigB.optSettings with jitOptUser = Some (switch = On) }
    
let localoptimizeSwitch (tcConfigB : TcConfigBuilder) switch =
    tcConfigB.optSettings <- { tcConfigB.optSettings with localOptUser = Some (switch = On) }
    
let crossOptimizeSwitch (tcConfigB : TcConfigBuilder) switch =
    tcConfigB.optSettings <- { tcConfigB.optSettings with crossModuleOptUser = Some (switch = On) }

let splittingSwitch (tcConfigB : TcConfigBuilder) switch =
    tcConfigB.optSettings <- { tcConfigB.optSettings with abstractBigTargets = switch = On }

let callVirtSwitch (tcConfigB : TcConfigBuilder) switch =
    tcConfigB.alwaysCallVirt <- switch = On    

let useHighEntropyVASwitch (tcConfigB : TcConfigBuilder) switch = 
    tcConfigB.useHighEntropyVA <- switch = On

let subSystemVersionSwitch (tcConfigB : TcConfigBuilder) (text : string) = 
    let fail() = error(Error(FSComp.SR.optsInvalidSubSystemVersion(text), rangeCmdArgs))

    // per spec for 357994: Validate input string, should be two positive integers x.y when x>=4 and y>=0 and both <= 65535
    if System.String.IsNullOrEmpty(text) then fail()
    else
    match text.Split('.') with
    | [| majorStr; minorStr|] ->
        match (Int32.TryParse majorStr), (Int32.TryParse minorStr) with
        | (true, major), (true, minor) when major >= 4 && major <=65535 && minor >=0 && minor <= 65535 -> tcConfigB.subsystemVersion <- (major, minor)
        | _ -> fail()
    | _ -> fail()

let (++) x s = x @ [s]

let SetTarget (tcConfigB : TcConfigBuilder)(s : string) =
    match s.ToLowerInvariant() with
    | "exe"     ->  tcConfigB.target <- ConsoleExe
    | "winexe"  ->  tcConfigB.target <- WinExe
    | "library" ->  tcConfigB.target <- Dll
    | "module"  ->  tcConfigB.target <- Module
    | _         ->  error(Error(FSComp.SR.optsUnrecognizedTarget(s),rangeCmdArgs))

let SetDebugSwitch (tcConfigB : TcConfigBuilder) (dtype : string option) (s : OptionSwitch) =
    match dtype with
    | Some(s) ->
       match s with 
       | "pdbonly" -> tcConfigB.jitTracking <- false
       | "full" -> tcConfigB.jitTracking <- true 
       | _ -> error(Error(FSComp.SR.optsUnrecognizedDebugType(s), rangeCmdArgs))
    | None -> tcConfigB.jitTracking <- s = On 
    tcConfigB.debuginfo <- s = On ;

let setOutFileName tcConfigB s = 
    tcConfigB.outputFile <- Some s

let setSignatureFile tcConfigB s = 
    tcConfigB.printSignature <- true ; 
    tcConfigB.printSignatureFile <- s

// option tags
let tagString = "<string>"
let tagExe = "exe"
let tagWinExe = "winexe"
let tagLibrary = "library"
let tagModule = "module"
let tagFile = "<file>"
let tagFileList = "<file;...>"
let tagDirList = "<dir;...>"
let tagPathList = "<path;...>"
let tagResInfo = "<resinfo>"
let tagFullPDBOnly = "{full|pdbonly}"
let tagWarnList = "<warn;...>"
let tagSymbolList = "<symbol;...>"
let tagAddress = "<address>"
let tagInt = "<n>"
let tagNone = ""


// PrintOptionInfo
//----------------

/// Print internal "option state" information for diagnostics and regression tests.  
let PrintOptionInfo (tcConfigB:TcConfigBuilder) =
    printfn "  jitOptUser . . . . . . : %+A" tcConfigB.optSettings.jitOptUser
    printfn "  localOptUser . . . . . : %+A" tcConfigB.optSettings.localOptUser
    printfn "  crossModuleOptUser . . : %+A" tcConfigB.optSettings.crossModuleOptUser
    printfn "  lambdaInlineThreshold  : %+A" tcConfigB.optSettings.lambdaInlineThreshold
    printfn "  ignoreSymStoreSeqPts . : %+A" tcConfigB.ignoreSymbolStoreSequencePoints
    printfn "  doDetuple  . . . . . . : %+A" tcConfigB.doDetuple
    printfn "  doTLR  . . . . . . . . : %+A" tcConfigB.doTLR
    printfn "  doFinalSimplify. . . . : %+A" tcConfigB.doFinalSimplify
    printfn "  jitTracking  . . . . . : %+A" tcConfigB.jitTracking
    printfn "  debuginfo  . . . . . . : %+A" tcConfigB.debuginfo
    printfn "  resolutionEnvironment  : %+A" tcConfigB.resolutionEnvironment
    printfn "  product  . . . . . . . : %+A" tcConfigB.productNameForBannerText
    tcConfigB.includes |> List.sort
                       |> List.iter (printfn "  include  . . . . . . . : %A")
  

// OptionBlock: Input files
//-------------------------

let inputFileFlagsBoth (tcConfigB : TcConfigBuilder) =
    [   CompilerOption("reference", tagFile, OptionString (fun s -> tcConfigB.AddReferencedAssemblyByPath (rangeStartup,s)), None,
                            Some (FSComp.SR.optsReference()) );
    ]

let referenceFlagAbbrev (tcConfigB : TcConfigBuilder) = 
        CompilerOption("r", tagFile, OptionString (fun s -> tcConfigB.AddReferencedAssemblyByPath (rangeStartup,s)), None,
                            Some(FSComp.SR.optsShortFormOf("--reference")) )
      
let inputFileFlagsFsi tcConfigB = inputFileFlagsBoth tcConfigB
let inputFileFlagsFsc tcConfigB = inputFileFlagsBoth tcConfigB


// OptionBlock: Errors and warnings
//---------------------------------

let errorsAndWarningsFlags (tcConfigB : TcConfigBuilder) = 
    [
        CompilerOption("warnaserror", tagNone, OptionSwitch(fun switch   -> tcConfigB.globalWarnAsError <- switch <> Off), None,
                            Some (FSComp.SR.optsWarnaserrorPM())); 

        CompilerOption("warnaserror", tagWarnList, OptionIntListSwitch (fun n switch -> 
                                                                    if switch = Off then 
                                                                        tcConfigB.specificWarnAsError <- ListSet.remove (=) n tcConfigB.specificWarnAsError ;
                                                                        tcConfigB.specificWarnAsWarn  <- ListSet.insert (=) n tcConfigB.specificWarnAsWarn
                                                                    else 
                                                                        tcConfigB.specificWarnAsWarn  <- ListSet.remove (=) n tcConfigB.specificWarnAsWarn ;
                                                                        tcConfigB.specificWarnAsError <- ListSet.insert (=) n tcConfigB.specificWarnAsError), None,
                            Some (FSComp.SR.optsWarnaserror()));
           
        CompilerOption("warn", tagInt, OptionInt (fun n -> 
                                                     tcConfigB.globalWarnLevel <- 
                                                     if (n >= 0 && n <= 5) then n 
                                                     else error(Error(FSComp.SR.optsInvalidWarningLevel(n),rangeCmdArgs))), None,
                            Some (FSComp.SR.optsWarn()));
           
        CompilerOption("nowarn", tagWarnList, OptionStringList (fun n -> tcConfigB.TurnWarningOff(rangeCmdArgs,n)), None,
                            Some (FSComp.SR.optsNowarn())); 
                            
        CompilerOption("warnon", tagWarnList, OptionStringList (fun n -> tcConfigB.TurnWarningOn(rangeCmdArgs,n)), None,
                            Some(FSComp.SR.optsWarnOn()));                             
        
        CompilerOption("consolecolors", tagNone, OptionSwitch (fun switch -> enableConsoleColoring <- switch=On), None, 
                            Some (FSComp.SR.optsConsoleColors()))
    ]


// OptionBlock: Output files
//--------------------------
          
let outputFileFlagsFsi (_tcConfigB : TcConfigBuilder) = []
let outputFileFlagsFsc (tcConfigB : TcConfigBuilder) =
    [
        CompilerOption("out", tagFile, OptionString (setOutFileName tcConfigB), None,
                            Some (FSComp.SR.optsNameOfOutputFile()) ); 

        CompilerOption("target",  tagExe, OptionString (SetTarget tcConfigB), None,
                            Some (FSComp.SR.optsBuildConsole()));
                           
        CompilerOption("target", tagWinExe, OptionString (SetTarget tcConfigB), None,
                            Some (FSComp.SR.optsBuildWindows()));

        CompilerOption("target", tagLibrary, OptionString (SetTarget tcConfigB), None,
                            Some (FSComp.SR.optsBuildLibrary()));

        CompilerOption("target", tagModule, OptionString (SetTarget tcConfigB), None,
                            Some (FSComp.SR.optsBuildModule()));

        CompilerOption("delaysign", tagNone, OptionSwitch (fun s -> tcConfigB.delaysign <- (s = On)), None,
                            Some (FSComp.SR.optsDelaySign()));

        CompilerOption("doc", tagFile, OptionString (fun s -> tcConfigB.xmlDocOutputFile <- Some s), None,
                            Some (FSComp.SR.optsWriteXml()));

        CompilerOption("keyfile", tagFile, OptionString (fun s -> tcConfigB.signer <- Some(s)),  None,
                           Some (FSComp.SR.optsStrongKeyFile()));

        CompilerOption("keycontainer", tagString, OptionString(fun s -> tcConfigB.container <- Some(s)),None,
                           Some(FSComp.SR.optsStrongKeyContainer()));

        CompilerOption("platform", tagString, OptionString (fun s -> tcConfigB.platform <- match s with | "x86" -> Some X86 | "x64" -> Some AMD64 | "Itanium" -> Some IA64 | "anycpu32bitpreferred" -> (tcConfigB.prefer32Bit <- true; None) | "anycpu" -> None | _ -> error(Error(FSComp.SR.optsUnknownPlatform(s),rangeCmdArgs))), None,
                            Some(FSComp.SR.optsPlatform())) ;

        CompilerOption("nooptimizationdata", tagNone, OptionUnit (fun () -> tcConfigB.onlyEssentialOptimizationData <- true), None,
                           Some (FSComp.SR.optsNoOpt()));

        CompilerOption("nointerfacedata", tagNone, OptionUnit (fun () -> tcConfigB.noSignatureData <- true), None,
                           Some (FSComp.SR.optsNoInterface()));

        CompilerOption("sig", tagFile, OptionString (setSignatureFile tcConfigB), None,
                           Some (FSComp.SR.optsSig()));    
    ]


// OptionBlock: Resources
//-----------------------

let resourcesFlagsFsi (_tcConfigB : TcConfigBuilder) = []
let resourcesFlagsFsc (tcConfigB : TcConfigBuilder) =
    [
        CompilerOption("win32res", tagFile, OptionString (fun s -> tcConfigB.win32res <- s), None,
                           Some (FSComp.SR.optsWin32res()));
        
        CompilerOption("win32manifest", tagFile, OptionString (fun s -> tcConfigB.win32manifest <- s), None,
                           Some (FSComp.SR.optsWin32manifest()));
        
        CompilerOption("nowin32manifest", tagNone, OptionUnit (fun () -> tcConfigB.includewin32manifest <- false), None,
                           Some (FSComp.SR.optsNowin32manifest()));

        CompilerOption("resource", tagResInfo, OptionString (fun s -> tcConfigB.AddEmbeddedResource s), None,
                           Some (FSComp.SR.optsResource()));

        CompilerOption("linkresource", tagResInfo, OptionString (fun s -> tcConfigB.linkResources <- tcConfigB.linkResources ++ s), None,
                           Some (FSComp.SR.optsLinkresource()));
    ]


// OptionBlock: Code generation
//-----------------------------
      
let codeGenerationFlags (tcConfigB : TcConfigBuilder) =
    [
        CompilerOption("debug", tagNone, OptionSwitch (SetDebugSwitch tcConfigB None), None,
                           Some (FSComp.SR.optsDebugPM()));
        
        CompilerOption("debug", tagFullPDBOnly, OptionString (fun s -> SetDebugSwitch tcConfigB (Some(s)) On), None,
                           Some (FSComp.SR.optsDebug()));

        CompilerOption("optimize", tagNone, OptionSwitch (SetOptimizeSwitch tcConfigB) , None,
                           Some (FSComp.SR.optsOptimize()));

        CompilerOption("tailcalls", tagNone, OptionSwitch (SetTailcallSwitch tcConfigB), None,
                           Some (FSComp.SR.optsTailcalls()));
                           
        CompilerOption("crossoptimize", tagNone, OptionSwitch (crossOptimizeSwitch tcConfigB), None,
                           Some (FSComp.SR.optsCrossoptimize()));
        
    ]
 

// OptionBlock: Language
//----------------------

let defineSymbol tcConfigB s = tcConfigB.conditionalCompilationDefines <- s :: tcConfigB.conditionalCompilationDefines
      
let mlCompatibilityFlag (tcConfigB : TcConfigBuilder) = 
        CompilerOption("mlcompatibility", tagNone, OptionUnit   (fun () -> tcConfigB.mlCompatibility<-true; tcConfigB.TurnWarningOff(rangeCmdArgs,"62")),  None,
                           Some (FSComp.SR.optsMlcompatibility()))
let languageFlags tcConfigB =
    [
        CompilerOption("checked", tagNone, OptionSwitch (fun switch -> tcConfigB.checkOverflow <- (switch = On)),  None,
                           Some (FSComp.SR.optsChecked()));
        CompilerOption("define", tagString, OptionString (defineSymbol tcConfigB),  None,
                           Some (FSComp.SR.optsDefine()));
        mlCompatibilityFlag tcConfigB
    ]
    

// OptionBlock: Advanced user options
//-----------------------------------

let libFlag (tcConfigB : TcConfigBuilder) = 
        CompilerOption("lib", tagDirList, OptionStringList (fun s -> tcConfigB.AddIncludePath (rangeStartup,s,tcConfigB.implicitIncludeDir)), None,
                           Some (FSComp.SR.optsLib()))

let libFlagAbbrev (tcConfigB : TcConfigBuilder) = 
        CompilerOption("I", tagDirList, OptionStringList (fun s -> tcConfigB.AddIncludePath (rangeStartup,s,tcConfigB.implicitIncludeDir)), None,
                           Some (FSComp.SR.optsShortFormOf("--lib")))
      
let codePageFlag (tcConfigB : TcConfigBuilder) = 
        CompilerOption("codepage", tagInt, OptionInt (fun n -> 
                     try 
                         System.Text.Encoding.GetEncodingShim(n) |> ignore
                     with :? System.ArgumentException as err -> 
                         error(Error(FSComp.SR.optsProblemWithCodepage(n,err.Message),rangeCmdArgs))

                     tcConfigB.inputCodePage <- Some(n)), None,
                           Some (FSComp.SR.optsCodepage()))

let utf8OutputFlag (tcConfigB: TcConfigBuilder) = 
        CompilerOption("utf8output", tagNone, OptionUnit (fun () -> tcConfigB.utf8output <- true), None,
                           Some (FSComp.SR.optsUtf8output()))

let fullPathsFlag  (tcConfigB : TcConfigBuilder)  = 
        CompilerOption("fullpaths", tagNone, OptionUnit (fun () -> tcConfigB.showFullPaths <- true), None,
                           Some (FSComp.SR.optsFullpaths()))

let cliRootFlag (_tcConfigB : TcConfigBuilder) = 
        CompilerOption("cliroot", tagString, OptionString (fun _  -> ()), Some(DeprecatedCommandLineOptionFull(FSComp.SR.optsClirootDeprecatedMsg(), rangeCmdArgs)),
                           Some(FSComp.SR.optsClirootDescription()))
          
let advancedFlagsBoth tcConfigB =
    [
        codePageFlag tcConfigB;
        utf8OutputFlag tcConfigB;
        fullPathsFlag tcConfigB;
        libFlag tcConfigB;
    ]

let noFrameworkFlag isFsc tcConfigB = 
    CompilerOption("noframework", tagNone, OptionUnit (fun () -> 
                                               tcConfigB.framework <- false; 
                                               if isFsc then 
                                                   tcConfigB.implicitlyResolveAssemblies <- false), None,
                           Some (FSComp.SR.optsNoframework()))

let advancedFlagsFsi tcConfigB = advancedFlagsBoth tcConfigB  @ [noFrameworkFlag false tcConfigB]
let setTargetProfile tcConfigB v = 
    tcConfigB.primaryAssembly <- 
        match v with
        | "mscorlib" -> PrimaryAssembly.Mscorlib
        | "netcore"  -> PrimaryAssembly.DotNetCore
        | _ -> error(Error(FSComp.SR.optsInvalidTargetProfile(v), rangeCmdArgs))

let advancedFlagsFsc tcConfigB =
    advancedFlagsBoth tcConfigB @
    [
        yield CompilerOption("baseaddress", tagAddress, OptionString (fun s -> tcConfigB.baseAddress <- Some(int32 s)), None, Some (FSComp.SR.optsBaseaddress()));
        yield noFrameworkFlag true tcConfigB;

        yield CompilerOption("standalone", tagNone, OptionUnit (fun _ -> 
                                             tcConfigB.openDebugInformationForLaterStaticLinking <- true; 
                                             tcConfigB.standalone <- true;
                                             tcConfigB.implicitlyResolveAssemblies <- true), None,
                             Some (FSComp.SR.optsStandalone()));

        yield CompilerOption("staticlink", tagFile, OptionString (fun s -> tcConfigB.extraStaticLinkRoots <- tcConfigB.extraStaticLinkRoots @ [s]), None,
                             Some (FSComp.SR.optsStaticlink()));

        if runningOnMono then 
            yield CompilerOption("resident", tagFile, OptionUnit (fun () -> ()), None,
                                 Some (FSComp.SR.optsResident()));

        yield CompilerOption("pdb", tagString, OptionString (fun s -> tcConfigB.debugSymbolFile <- Some s), None,
                             Some (FSComp.SR.optsPdb()));
        yield CompilerOption("simpleresolution", tagNone, OptionUnit (fun () -> tcConfigB.useMonoResolution<-true), None,
                             Some (FSComp.SR.optsSimpleresolution()));

        yield CompilerOption("highentropyva", tagNone, OptionSwitch (useHighEntropyVASwitch tcConfigB), None, Some (FSComp.SR.optsUseHighEntropyVA()))
        yield CompilerOption("subsystemversion", tagString, OptionString (subSystemVersionSwitch tcConfigB), None, Some (FSComp.SR.optsSubSystemVersion()))
        yield CompilerOption("targetprofile", tagString, OptionString (setTargetProfile tcConfigB), None, Some(FSComp.SR.optsTargetProfile()))
        yield CompilerOption("quotations-debug", tagNone, OptionSwitch(fun switch -> tcConfigB.emitDebugInfoInQuotations <- switch = On), None, Some(FSComp.SR.optsEmitDebugInfoInQuotations()))
    ]

// OptionBlock: Internal options (internal use only)
//--------------------------------------------------

let testFlag tcConfigB = 
        CompilerOption("test", tagString, OptionString (fun s -> 
                                            match s with
                                            | "ErrorRanges"      -> tcConfigB.errorStyle <- ErrorStyle.TestErrors
                                            | "MemberBodyRanges" -> PostTypecheckSemanticChecks.testFlagMemberBody := true
                                            | "Tracking"         -> Lib.tracking := true (* general purpose on/off diagnostics flag *)
                                            | "NoNeedToTailcall" -> tcConfigB.optSettings <- { tcConfigB.optSettings with reportNoNeedToTailcall = true }
                                            | "FunctionSizes"    -> tcConfigB.optSettings <- { tcConfigB.optSettings with reportFunctionSizes = true }
                                            | "TotalSizes"       -> tcConfigB.optSettings <- { tcConfigB.optSettings with reportTotalSizes = true }
                                            | "HasEffect"        -> tcConfigB.optSettings <- { tcConfigB.optSettings with reportHasEffect = true }
                                            | "NoErrorText"      -> FSComp.SR.SwallowResourceText <- true
                                            | "EmitFeeFeeAs100001" -> tcConfigB.testFlagEmitFeeFeeAs100001 <- true
                                            | "DumpDebugInfo"    -> tcConfigB.dumpDebugInfo <- true
                                            | "ShowLoadedAssemblies" -> tcConfigB.showLoadedAssemblies <- true
                                            | "ContinueAfterParseFailure" -> tcConfigB.continueAfterParseFailure <- true
                                            | str                -> warning(Error(FSComp.SR.optsUnknownArgumentToTheTestSwitch(str),rangeCmdArgs))), None,
                           None)

// not shown in fsc.exe help, no warning on use, motiviation is for use from VS
let vsSpecificFlags (tcConfigB: TcConfigBuilder) = 
  [ CompilerOption("vserrors", tagNone, OptionUnit (fun () -> tcConfigB.errorStyle <- ErrorStyle.VSErrors), None, None);
    CompilerOption("validate-type-providers", tagNone, OptionUnit (fun () -> tcConfigB.validateTypeProviders <- true), None, None);
    CompilerOption("LCID", tagInt, OptionInt (fun n -> tcConfigB.lcid <- Some(n)), None, None);
    CompilerOption("flaterrors", tagNone, OptionUnit (fun () -> tcConfigB.flatErrors <- true), None, None); 
    CompilerOption("sqmsessionguid", tagNone, OptionString (fun s -> tcConfigB.sqmSessionGuid <- try System.Guid(s) |> Some  with e -> None), None, None);
    CompilerOption("maxerrors", tagInt, OptionInt (fun n -> tcConfigB.maxErrors <- n), None, None); ]


let internalFlags (tcConfigB:TcConfigBuilder) =
  [
    CompilerOption("use-incremental-build", tagNone, OptionUnit (fun () -> tcConfigB.useIncrementalBuilder <- true), None, None)
    CompilerOption("stamps", tagNone, OptionUnit (fun () -> 
#if DEBUG
        Tast.verboseStamps := true
#else 
        ()
#endif
    ), Some(InternalCommandLineOption("--stamps", rangeCmdArgs)), None);
    CompilerOption("ranges", tagNone, OptionSet Tastops.DebugPrint.layoutRanges, Some(InternalCommandLineOption("--ranges", rangeCmdArgs)), None);  
    CompilerOption("terms" , tagNone, OptionUnit (fun () -> tcConfigB.showTerms <- true), Some(InternalCommandLineOption("--terms", rangeCmdArgs)), None);
    CompilerOption("termsfile" , tagNone, OptionUnit (fun () -> tcConfigB.writeTermsToFiles <- true), Some(InternalCommandLineOption("--termsfile", rangeCmdArgs)), None);
    CompilerOption("use-incremental-build", tagNone, OptionUnit (fun () -> tcConfigB.useIncrementalBuilder <- true), None, None)
#if DEBUG
    CompilerOption("debug-parse", tagNone, OptionUnit (fun () -> Internal.Utilities.Text.Parsing.Flags.debug <- true), Some(InternalCommandLineOption("--debug-parse", rangeCmdArgs)), None);
    CompilerOption("ilfiles", tagNone, OptionUnit (fun () -> tcConfigB.writeGeneratedILFiles <- true), Some(InternalCommandLineOption("--ilfiles", rangeCmdArgs)), None);
#endif
    CompilerOption("pause", tagNone, OptionUnit (fun () -> tcConfigB.pause <- true), Some(InternalCommandLineOption("--pause", rangeCmdArgs)), None);
    CompilerOption("detuple", tagNone, OptionInt (setFlag (fun v -> tcConfigB.doDetuple <- v)), Some(InternalCommandLineOption("--detuple", rangeCmdArgs)), None);
    CompilerOption("simulateException", tagNone, OptionString (fun s -> tcConfigB.simulateException <- Some(s)), Some(InternalCommandLineOption("--simulateException", rangeCmdArgs)), Some "Simulate an exception from some part of the compiler");    
    CompilerOption("stackReserveSize", tagNone, OptionString (fun s -> tcConfigB.stackReserveSize <- Some(int32 s)), Some(InternalCommandLineOption("--stackReserveSize", rangeCmdArgs)), Some ("for an exe, set stack reserve size"));
    CompilerOption("tlr", tagInt, OptionInt (setFlag (fun v -> tcConfigB.doTLR <- v)), Some(InternalCommandLineOption("--tlr", rangeCmdArgs)), None);
    CompilerOption("finalSimplify", tagInt, OptionInt (setFlag (fun v -> tcConfigB.doFinalSimplify <- v)), Some(InternalCommandLineOption("--finalSimplify", rangeCmdArgs)), None);
#if TLR_LIFT
    CompilerOption("tlrlift", tagNone, OptionInt (setFlag  (fun v -> Tlr.liftTLR := v)), Some(InternalCommandLineOption("--tlrlift", rangeCmdArgs)), None);
#endif
    CompilerOption("parseonly", tagNone, OptionUnit (fun () -> tcConfigB.parseOnly <- true), Some(InternalCommandLineOption("--parseonly", rangeCmdArgs)), None);
    CompilerOption("typecheckonly", tagNone, OptionUnit (fun () -> tcConfigB.typeCheckOnly <- true), Some(InternalCommandLineOption("--typecheckonly", rangeCmdArgs)), None);
    CompilerOption("ast", tagNone, OptionUnit (fun () -> tcConfigB.printAst <- true), Some(InternalCommandLineOption("--ast", rangeCmdArgs)), None);
    CompilerOption("tokenize", tagNone, OptionUnit (fun () -> tcConfigB.tokenizeOnly <- true), Some(InternalCommandLineOption("--tokenize", rangeCmdArgs)), None);
    CompilerOption("testInteractionParser", tagNone, OptionUnit (fun () -> tcConfigB.testInteractionParser <- true), Some(InternalCommandLineOption("--testInteractionParser", rangeCmdArgs)), None);
    CompilerOption("testparsererrorrecovery", tagNone, OptionUnit (fun () -> tcConfigB.reportNumDecls <- true), Some(InternalCommandLineOption("--testparsererrorrecovery", rangeCmdArgs)), None);
    CompilerOption("inlinethreshold", tagInt, OptionInt (fun n -> tcConfigB.optSettings <- { tcConfigB.optSettings with lambdaInlineThreshold = n }), Some(InternalCommandLineOption("--inlinethreshold", rangeCmdArgs)), None);
    CompilerOption("extraoptimizationloops", tagNone, OptionInt (fun n -> tcConfigB.extraOptimizationIterations <- n), Some(InternalCommandLineOption("--extraoptimizationloops", rangeCmdArgs)), None);
    CompilerOption("abortonerror", tagNone, OptionUnit (fun () -> tcConfigB.abortOnError <- true), Some(InternalCommandLineOption("--abortonerror", rangeCmdArgs)), None);
    CompilerOption("implicitresolution", tagNone, OptionUnit (fun _ -> tcConfigB.implicitlyResolveAssemblies <- true), Some(InternalCommandLineOption("--implicitresolution", rangeCmdArgs)), None);

    CompilerOption("resolutions", tagNone, OptionUnit (fun () -> tcConfigB.showReferenceResolutions <- true), Some(InternalCommandLineOption("", rangeCmdArgs)), None); // "Display assembly reference resolution information") ;
    CompilerOption("resolutionframeworkregistrybase", tagString, OptionString (fun s -> tcConfigB.resolutionFrameworkRegistryBase<-s), Some(InternalCommandLineOption("", rangeCmdArgs)), None); // "The base registry key to use for assembly resolution. This part in brackets here: HKEY_LOCAL_MACHINE\[SOFTWARE\Microsoft\.NETFramework]\v2.0.50727\AssemblyFoldersEx");
    CompilerOption("resolutionassemblyfoldersuffix", tagString, OptionString (fun s -> tcConfigB.resolutionAssemblyFoldersSuffix<-s), Some(InternalCommandLineOption("resolutionassemblyfoldersuffix", rangeCmdArgs)), None); // "The base registry key to use for assembly resolution. This part in brackets here: HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\.NETFramework\v2.0.50727\[AssemblyFoldersEx]");
    CompilerOption("resolutionassemblyfoldersconditions", tagString, OptionString (fun s -> tcConfigB.resolutionAssemblyFoldersConditions <- ","^s), Some(InternalCommandLineOption("resolutionassemblyfoldersconditions", rangeCmdArgs)), None); // "Additional reference resolution conditions. For example \"OSVersion=5.1.2600.0,PlatformID=id");
    CompilerOption("msbuildresolution", tagNone, OptionUnit (fun () -> tcConfigB.useMonoResolution<-false), Some(InternalCommandLineOption("msbuildresolution", rangeCmdArgs)), None); // "Resolve assembly references using MSBuild resolution rules rather than directory based (Default=true except when running fsc.exe under mono)");
    CompilerOption("indirectcallarraymethods", tagNone, OptionUnit (fun () -> tcConfigB.indirectCallArrayMethods<-true), Some(InternalCommandLineOption("--indirectCallArrayMethods", rangeCmdArgs)), None);
    CompilerOption("alwayscallvirt",tagNone,OptionSwitch(callVirtSwitch tcConfigB),Some(InternalCommandLineOption("alwayscallvirt",rangeCmdArgs)), None);
    CompilerOption("nodebugdata",tagNone, OptionUnit (fun () -> tcConfigB.noDebugData<-true),Some(InternalCommandLineOption("--nodebugdata",rangeCmdArgs)), None);
    testFlag tcConfigB  ] @
  vsSpecificFlags tcConfigB @
  [ CompilerOption("jit", tagNone, OptionSwitch (jitoptimizeSwitch tcConfigB), Some(InternalCommandLineOption("jit", rangeCmdArgs)), None);
    CompilerOption("localoptimize", tagNone, OptionSwitch(localoptimizeSwitch tcConfigB),Some(InternalCommandLineOption("localoptimize", rangeCmdArgs)), None);
    CompilerOption("splitting", tagNone, OptionSwitch(splittingSwitch tcConfigB),Some(InternalCommandLineOption("splitting", rangeCmdArgs)), None);
    CompilerOption("versionfile", tagString, OptionString (fun s -> tcConfigB.version <- VersionFile s), Some(InternalCommandLineOption("versionfile", rangeCmdArgs)), None);
    CompilerOption("times" , tagNone, OptionUnit  (fun () -> tcConfigB.showTimes <- true), Some(InternalCommandLineOption("times", rangeCmdArgs)), None); // "Display timing profiles for compilation");
#if EXTENSIONTYPING
    CompilerOption("showextensionresolution" , tagNone, OptionUnit  (fun () -> tcConfigB.showExtensionTypeMessages <- true), Some(InternalCommandLineOption("showextensionresolution", rangeCmdArgs)), None); // "Display information about extension type resolution");
#endif
    (* BEGIN: Consider as public Retail option? *)
    // Some System.Console do not have operational colors, make this available in Retail?    
    CompilerOption("metadataversion", tagString, OptionString (fun s -> tcConfigB.metadataVersion <- Some(s)), Some(InternalCommandLineOption("metadataversion", rangeCmdArgs)), None);
  ]

  
// OptionBlock: Deprecated flags (fsc, service only)
//--------------------------------------------------
    
let compilingFsLibFlag (tcConfigB : TcConfigBuilder) = 
        CompilerOption("compiling-fslib", tagNone, OptionUnit (fun () -> tcConfigB.compilingFslib <- true; 
                                                                         tcConfigB.TurnWarningOff(rangeStartup,"42"); 
                                                                         ErrorLogger.reportLibraryOnlyFeatures <- false;
                                                                         IlxSettings.ilxCompilingFSharpCoreLib := true), Some(InternalCommandLineOption("--compiling-fslib", rangeCmdArgs)), None)
let compilingFsLib20Flag (tcConfigB : TcConfigBuilder) = 
        CompilerOption("compiling-fslib-20", tagNone, OptionString (fun s -> tcConfigB.compilingFslib20 <- Some s; ), Some(InternalCommandLineOption("--compiling-fslib-20", rangeCmdArgs)), None)
let compilingFsLib40Flag (tcConfigB : TcConfigBuilder) = 
        CompilerOption("compiling-fslib-40", tagNone, OptionUnit (fun () -> tcConfigB.compilingFslib40 <- true; ), Some(InternalCommandLineOption("--compiling-fslib-40", rangeCmdArgs)), None)
let mlKeywordsFlag = 
        CompilerOption("ml-keywords", tagNone, OptionUnit (fun () -> Lexhelp.Keywords.permitFsharpKeywords <- false), Some(DeprecatedCommandLineOptionNoDescription("--ml-keywords", rangeCmdArgs)), None)

let gnuStyleErrorsFlag tcConfigB = 
        CompilerOption("gnu-style-errors", tagNone, OptionUnit (fun () -> tcConfigB.errorStyle <- ErrorStyle.EmacsErrors), Some(DeprecatedCommandLineOptionNoDescription("--gnu-style-errors", rangeCmdArgs)), None)

let deprecatedFlagsBoth tcConfigB =
    [ 
      CompilerOption("light", tagNone, OptionUnit (fun () -> tcConfigB.light <- Some(true)), Some(DeprecatedCommandLineOptionNoDescription("--light", rangeCmdArgs)), None);
      CompilerOption("indentation-syntax", tagNone, OptionUnit (fun () -> tcConfigB.light <- Some(true)), Some(DeprecatedCommandLineOptionNoDescription("--indentation-syntax", rangeCmdArgs)), None);
      CompilerOption("no-indentation-syntax", tagNone, OptionUnit (fun () -> tcConfigB.light <- Some(false)), Some(DeprecatedCommandLineOptionNoDescription("--no-indentation-syntax", rangeCmdArgs)), None); 
    ]
          
let deprecatedFlagsFsi tcConfigB = deprecatedFlagsBoth tcConfigB
let deprecatedFlagsFsc tcConfigB =
    deprecatedFlagsBoth tcConfigB @
    [
    cliRootFlag tcConfigB;
    CompilerOption("jit-optimize", tagNone, OptionUnit (fun _ -> tcConfigB.optSettings <- { tcConfigB.optSettings with jitOptUser = Some true }), Some(DeprecatedCommandLineOptionNoDescription("--jit-optimize", rangeCmdArgs)), None);
    CompilerOption("no-jit-optimize", tagNone, OptionUnit (fun _ -> tcConfigB.optSettings <- { tcConfigB.optSettings with jitOptUser = Some false }), Some(DeprecatedCommandLineOptionNoDescription("--no-jit-optimize", rangeCmdArgs)), None);
    CompilerOption("jit-tracking", tagNone, OptionUnit (fun _ -> tcConfigB.jitTracking <- true), Some(DeprecatedCommandLineOptionNoDescription("--jit-tracking", rangeCmdArgs)), None);
    CompilerOption("no-jit-tracking", tagNone, OptionUnit (fun _ -> tcConfigB.jitTracking <- false), Some(DeprecatedCommandLineOptionNoDescription("--no-jit-tracking", rangeCmdArgs)), None);
    CompilerOption("progress", tagNone, OptionUnit (fun () -> progress := true), Some(DeprecatedCommandLineOptionNoDescription("--progress", rangeCmdArgs)), None);
    (compilingFsLibFlag tcConfigB) ;
    (compilingFsLib20Flag tcConfigB) ;
    (compilingFsLib40Flag tcConfigB) ;
    CompilerOption("version", tagString, OptionString (fun s -> tcConfigB.version <- VersionString s), Some(DeprecatedCommandLineOptionNoDescription("--version", rangeCmdArgs)), None);
//  "--clr-mscorlib", OptionString (fun s -> warning(Some(DeprecatedCommandLineOptionNoDescription("--clr-mscorlib", rangeCmdArgs))) ;   tcConfigB.Build.mscorlib_assembly_name <- s), "\n\tThe name of mscorlib on the target CLR"; 
    CompilerOption("local-optimize", tagNone, OptionUnit (fun _ -> tcConfigB.optSettings <- { tcConfigB.optSettings with localOptUser = Some true }), Some(DeprecatedCommandLineOptionNoDescription("--local-optimize", rangeCmdArgs)), None);
    CompilerOption("no-local-optimize", tagNone, OptionUnit (fun _ -> tcConfigB.optSettings <- { tcConfigB.optSettings with localOptUser = Some false }), Some(DeprecatedCommandLineOptionNoDescription("--no-local-optimize", rangeCmdArgs)), None);
    CompilerOption("cross-optimize", tagNone, OptionUnit (fun _ -> tcConfigB.optSettings <- { tcConfigB.optSettings with crossModuleOptUser = Some true }), Some(DeprecatedCommandLineOptionNoDescription("--cross-optimize", rangeCmdArgs)), None);
    CompilerOption("no-cross-optimize", tagNone, OptionUnit (fun _ -> tcConfigB.optSettings <- { tcConfigB.optSettings with crossModuleOptUser = Some false }), Some(DeprecatedCommandLineOptionNoDescription("--no-cross-optimize", rangeCmdArgs)), None);
    CompilerOption("no-string-interning", tagNone, OptionUnit (fun () -> tcConfigB.internConstantStrings <- false), Some(DeprecatedCommandLineOptionNoDescription("--no-string-interning", rangeCmdArgs)), None);
    CompilerOption("statistics", tagNone, OptionUnit (fun () -> tcConfigB.stats <- true), Some(DeprecatedCommandLineOptionNoDescription("--statistics", rangeCmdArgs)), None);
    CompilerOption("generate-filter-blocks", tagNone, OptionUnit (fun () -> tcConfigB.generateFilterBlocks <- true), Some(DeprecatedCommandLineOptionNoDescription("--generate-filter-blocks", rangeCmdArgs)), None); 
    //CompilerOption("no-generate-filter-blocks", tagNone, OptionUnit (fun () -> tcConfigB.generateFilterBlocks <- false), Some(DeprecatedCommandLineOptionNoDescription("--generate-filter-blocks", rangeCmdArgs)), None); 
    CompilerOption("max-errors", tagInt, OptionInt (fun n -> tcConfigB.maxErrors <- n), Some(DeprecatedCommandLineOptionSuggestAlternative("--max-errors", "--maxerrors", rangeCmdArgs)),None);
    CompilerOption("debug-file", tagNone, OptionString (fun s -> tcConfigB.debugSymbolFile <- Some s), Some(DeprecatedCommandLineOptionSuggestAlternative("--debug-file", "--pdb", rangeCmdArgs)), None);
    CompilerOption("no-debug-file", tagNone,  OptionUnit (fun () -> tcConfigB.debuginfo <- false), Some(DeprecatedCommandLineOptionSuggestAlternative("--no-debug-file", "--debug-", rangeCmdArgs)), None);
    CompilerOption("Ooff", tagNone, OptionUnit (fun () -> SetOptimizeOff(tcConfigB)), Some(DeprecatedCommandLineOptionSuggestAlternative("-Ooff", "--optimize-", rangeCmdArgs)), None);
    mlKeywordsFlag ;
    gnuStyleErrorsFlag tcConfigB;
    ]


// OptionBlock: Miscellaneous options
//-----------------------------------

let DisplayBannerText tcConfigB =
    if tcConfigB.showBanner then (
        printfn "%s" tcConfigB.productNameForBannerText
        printfn "%s" (FSComp.SR.optsCopyright())
    )

/// FSC only help. (FSI has it's own help function).
let displayHelpFsc tcConfigB (blocks:CompilerOptionBlock list) =
    DisplayBannerText tcConfigB;
    printCompilerOptionBlocks blocks
    exit 0
      
let miscFlagsBoth tcConfigB = 
    [   CompilerOption("nologo", tagNone, OptionUnit (fun () -> tcConfigB.showBanner <- false), None, Some (FSComp.SR.optsNologo()));
    ]
      
let miscFlagsFsc tcConfigB =
    miscFlagsBoth tcConfigB @
    [   CompilerOption("help", tagNone, OptionHelp (fun blocks -> displayHelpFsc tcConfigB blocks), None, Some (FSComp.SR.optsHelp()))
    ]
let miscFlagsFsi tcConfigB = miscFlagsBoth tcConfigB


// OptionBlock: Abbreviations of existing options
//-----------------------------------------------
      
let abbreviatedFlagsBoth tcConfigB =
    [
        CompilerOption("d", tagString, OptionString (defineSymbol tcConfigB), None, Some(FSComp.SR.optsShortFormOf("--define")));
        CompilerOption("O", tagNone, OptionSwitch (SetOptimizeSwitch tcConfigB) , None, Some(FSComp.SR.optsShortFormOf("--optimize[+|-]")));
        CompilerOption("g", tagNone, OptionSwitch (SetDebugSwitch tcConfigB None), None, Some(FSComp.SR.optsShortFormOf("--debug")));
        CompilerOption("i", tagString, OptionUnit (fun () -> tcConfigB.printSignature <- true), None, Some(FSComp.SR.optsShortFormOf("--sig")));
        referenceFlagAbbrev tcConfigB; (* -r <dll> *)
        libFlagAbbrev tcConfigB;       (* -I <dir> *)
    ]

let abbreviatedFlagsFsi tcConfigB = abbreviatedFlagsBoth tcConfigB
let abbreviatedFlagsFsc tcConfigB =
    abbreviatedFlagsBoth tcConfigB @
    [   (* FSC only abbreviated options *)
        CompilerOption("o", tagString, OptionString (setOutFileName tcConfigB), None, Some(FSComp.SR.optsShortFormOf("--out")));
        CompilerOption("a", tagString, OptionUnit (fun () -> tcConfigB.target <- Dll), None, Some(FSComp.SR.optsShortFormOf("--target library")));
        (* FSC help abbreviations. FSI has it's own help options... *)
        CompilerOption("?"        , tagNone, OptionHelp (fun blocks -> displayHelpFsc tcConfigB blocks), None, Some(FSComp.SR.optsShortFormOf("--help")));
        CompilerOption("help"     , tagNone, OptionHelp (fun blocks -> displayHelpFsc tcConfigB blocks), None, Some(FSComp.SR.optsShortFormOf("--help")));
        CompilerOption("full-help", tagNone, OptionHelp (fun blocks -> displayHelpFsc tcConfigB blocks), None, Some(FSComp.SR.optsShortFormOf("--help")))
    ]
    
let abbrevFlagSet tcConfigB isFsc =
    let mutable argList : string list = []
    for c in ((if isFsc then abbreviatedFlagsFsc else abbreviatedFlagsFsi) tcConfigB) do
        match c with
        | CompilerOption(arg,_,OptionString _,_,_)
        | CompilerOption(arg,_,OptionStringList _,_,_) -> argList <- argList @ ["-"^arg;"/"^arg]
        | _ -> ()
    Set.ofList argList
    
// check for abbreviated options that accept spaces instead of colons, and replace the spaces
// with colons when necessary
let PostProcessCompilerArgs (abbrevArgs : string Set) (args : string []) =
    let mutable i = 0
    let mutable idx = 0
    let len = args.Length
    let mutable arga : string[] = Array.create len ""
    
    while i < len do
        if not(abbrevArgs.Contains(args.[i])) || i = (len - 1)  then
            arga.[idx] <- args.[i] ;
            i <- i+1
        else
            arga.[idx] <- args.[i] ^ ":" ^ args.[i+1]
            i <- i + 2
        idx <- idx + 1
    Array.toList arga.[0 .. (idx - 1)]

// OptionBlock: QA options
//------------------------
      
let testingAndQAFlags _tcConfigB =
  [
    CompilerOption("dumpAllCommandLineOptions", tagNone, OptionHelp(fun blocks -> dumpCompilerOptionBlocks blocks), None, None) // "Command line options")
  ]


// Core compiler options, overview
//--------------------------------
      
(*  The "core" compiler options are "the ones defined here".
    Currently, fsi.exe has some additional options, defined in fsi.fs.
    
    The compiler options are put into blocks, named as <block>Flags.
    Some block options differ between fsc and fsi, in this case they split as <block>FlagsFsc and <block>FlagsFsi.
    
    The "service.fs" (language service) flags are the same as the fsc flags (except help options are removed).
    REVIEW: is this correct? what about fsx files in VS and fsi options?
  
    Block                      | notes
    ---------------------------|--------------------
    outputFileFlags            |
    inputFileFlags             |
    resourcesFlags             |
    codeGenerationFlags        |
    errorsAndWarningsFlags     |
    languageFlags              |
    miscFlags                  |
    advancedFlags              |
    internalFlags              |
    abbreviatedFlags           |
    deprecatedFlags            | REVIEW: some of these may have been valid for fsi.exe?
    fsiSpecificFlags           | These are defined later, in fsi.fs
    ---------------------------|--------------------
*)

// Core compiler options exported to fsc.fs, service.fs and fsi.fs
//----------------------------------------------------------------

/// The core/common options used by fsc.exe. [not currently extended by fsc.fs].
let GetCoreFscCompilerOptions (tcConfigB: TcConfigBuilder) = 
  [ PublicOptions(FSComp.SR.optsHelpBannerOutputFiles(), outputFileFlagsFsc      tcConfigB); 
    PublicOptions(FSComp.SR.optsHelpBannerInputFiles(), inputFileFlagsFsc       tcConfigB);
    PublicOptions(FSComp.SR.optsHelpBannerResources(), resourcesFlagsFsc       tcConfigB);
    PublicOptions(FSComp.SR.optsHelpBannerCodeGen(), codeGenerationFlags     tcConfigB);
    PublicOptions(FSComp.SR.optsHelpBannerErrsAndWarns(), errorsAndWarningsFlags  tcConfigB);
    PublicOptions(FSComp.SR.optsHelpBannerLanguage(), languageFlags           tcConfigB);
    PublicOptions(FSComp.SR.optsHelpBannerMisc(), miscFlagsFsc            tcConfigB);
    PublicOptions(FSComp.SR.optsHelpBannerAdvanced(), advancedFlagsFsc        tcConfigB);
    PrivateOptions(List.concat              [ internalFlags           tcConfigB;
                                              abbreviatedFlagsFsc     tcConfigB;
                                              deprecatedFlagsFsc      tcConfigB;
                                              testingAndQAFlags       tcConfigB])
  ]

/// The core/common options used by the F# VS Language Service.
/// Filter out OptionHelp which does printing then exit. This is not wanted in the context of VS!!
let GetCoreServiceCompilerOptions (tcConfigB:TcConfigBuilder) =
  let isHelpOption = function CompilerOption(_,_,OptionHelp _,_,_) -> true | _ -> false
  List.map (filterCompilerOptionBlock (isHelpOption >> not)) (GetCoreFscCompilerOptions tcConfigB)

/// The core/common options used by fsi.exe. [note, some additional options are added in fsi.fs].
let GetCoreFsiCompilerOptions (tcConfigB: TcConfigBuilder) =
  [ PublicOptions(FSComp.SR.optsHelpBannerOutputFiles()        , outputFileFlagsFsi      tcConfigB);
    PublicOptions(FSComp.SR.optsHelpBannerInputFiles()         , inputFileFlagsFsi       tcConfigB);
    PublicOptions(FSComp.SR.optsHelpBannerResources()          , resourcesFlagsFsi       tcConfigB);
    PublicOptions(FSComp.SR.optsHelpBannerCodeGen()            , codeGenerationFlags     tcConfigB);
    PublicOptions(FSComp.SR.optsHelpBannerErrsAndWarns()       , errorsAndWarningsFlags  tcConfigB);
    PublicOptions(FSComp.SR.optsHelpBannerLanguage()           , languageFlags           tcConfigB);
    // Note: no HTML block for fsi.exe
    PublicOptions(FSComp.SR.optsHelpBannerMisc()       , miscFlagsFsi            tcConfigB);
    PublicOptions(FSComp.SR.optsHelpBannerAdvanced()   , advancedFlagsFsi        tcConfigB);
    PrivateOptions(List.concat              [ internalFlags           tcConfigB;
                                              abbreviatedFlagsFsi     tcConfigB;
                                              deprecatedFlagsFsi      tcConfigB;
                                              testingAndQAFlags       tcConfigB])
  ]




//----------------------------------------------------------------------------
// PrintWholeAssemblyImplementation
//----------------------------------------------------------------------------

let showTermFileCount = ref 0    
let PrintWholeAssemblyImplementation (tcConfig:TcConfig) outfile header expr =
    if tcConfig.showTerms then
        if tcConfig.writeTermsToFiles then 
            let filename = outfile ^ ".terms"
            let n = !showTermFileCount
            showTermFileCount := n+1;
            use f = System.IO.File.CreateText (filename ^ "-" ^ string n ^ "-" ^ header)
            Layout.outL f (Layout.squashTo 192 (DebugPrint.assemblyL expr));
        else 
            dprintf "\n------------------\nshowTerm: %s:\n" header;
            Layout.outL stderr (Layout.squashTo 192 (DebugPrint.assemblyL expr));
            dprintf "\n------------------\n";

//----------------------------------------------------------------------------
// ReportTime 
//----------------------------------------------------------------------------

let tPrev = ref None
let nPrev = ref None
let ReportTime (tcConfig:TcConfig) descr =
    
    match !nPrev with
    | None -> ()
    | Some prevDescr ->
        if tcConfig.pause then 
            dprintf "[done '%s', entering '%s'] press any key... " prevDescr descr;
            System.Console.ReadLine() |> ignore;
        // Intentionally putting this right after the pause so a debugger can be attached.
        match tcConfig.simulateException with
        | Some("fsc-oom") -> raise(System.OutOfMemoryException())
        | Some("fsc-an") -> raise(System.ArgumentNullException("simulated"))
        | Some("fsc-invop") -> raise(System.InvalidOperationException())
        | Some("fsc-av") -> raise(System.AccessViolationException())
        | Some("fsc-aor") -> raise(System.ArgumentOutOfRangeException())
        | Some("fsc-dv0") -> raise(System.DivideByZeroException())
        | Some("fsc-nfn") -> raise(System.NotFiniteNumberException())
        | Some("fsc-oe") -> raise(System.OverflowException())
        | Some("fsc-atmm") -> raise(System.ArrayTypeMismatchException())
        | Some("fsc-bif") -> raise(System.BadImageFormatException())
        | Some("fsc-knf") -> raise(System.Collections.Generic.KeyNotFoundException())
        | Some("fsc-ior") -> raise(System.IndexOutOfRangeException())
        | Some("fsc-ic") -> raise(System.InvalidCastException())
        | Some("fsc-ip") -> raise(System.InvalidProgramException())
        | Some("fsc-ma") -> raise(System.MemberAccessException())
        | Some("fsc-ni") -> raise(System.NotImplementedException())
        | Some("fsc-nr") -> raise(System.NullReferenceException())
        | Some("fsc-oc") -> raise(System.OperationCanceledException())
        | Some("fsc-fail") -> failwith "simulated"
        | _ -> ()




    if (tcConfig.showTimes || verbose) then 
        // Note that timing calls are relatively expensive on the startup path so we don't
        // make this call unless showTimes has been turned on.
        let timeNow = System.Diagnostics.Process.GetCurrentProcess().UserProcessorTime.TotalSeconds
        let maxGen = System.GC.MaxGeneration
        let gcNow = [| for i in 0 .. maxGen -> System.GC.CollectionCount(i) |]
        let ptime = System.Diagnostics.Process.GetCurrentProcess()
        let wsNow = ptime.WorkingSet64/1000000L

        match !tPrev, !nPrev with
        | Some (timePrev,gcPrev:int []),Some prevDescr ->
            let spanGC = [| for i in 0 .. maxGen -> System.GC.CollectionCount(i) - gcPrev.[i] |]
            dprintf "TIME: %4.1f Delta: %4.1f Mem: %3d" 
                timeNow (timeNow - timePrev) 
                wsNow;
            dprintf " G0: %3d G1: %2d G2: %2d [%s]\n" 
                spanGC.[Operators.min 0 maxGen] spanGC.[Operators.min 1 maxGen] spanGC.[Operators.min 2 maxGen]
                prevDescr

        | _ -> ()
        tPrev := Some (timeNow,gcNow)

    nPrev := Some descr

#if NO_COMPILER_BACKEND
#else  
//----------------------------------------------------------------------------
// OPTIMIZATION - support - addDllToOptEnv
//----------------------------------------------------------------------------

let AddExternalCcuToOpimizationEnv tcGlobals optEnv ccuinfo =
    match ccuinfo.FSharpOptimizationData.Force() with 
    | None -> optEnv
    | Some(data) -> Opt.BindCcu ccuinfo.FSharpViewOfMetadata data optEnv tcGlobals

//----------------------------------------------------------------------------
// OPTIMIZATION - support - optimize
//----------------------------------------------------------------------------

let InitialOptimizationEnv (tcImports:TcImports) (tcGlobals:TcGlobals) =
    let ccuinfos = tcImports.GetImportedAssemblies()
    let optEnv = Opt.IncrementalOptimizationEnv.Empty
    let optEnv = List.fold (AddExternalCcuToOpimizationEnv tcGlobals) optEnv ccuinfos 
    optEnv
   
let ApplyAllOptimizations (tcConfig:TcConfig, tcGlobals, tcVal, outfile, importMap, isIncrementalFragment, optEnv, ccu:CcuThunk, tassembly:TypedAssembly) =
    // NOTE: optEnv - threads through 
    //
    // Always optimize once - the results of this step give the x-module optimization 
    // info.  Subsequent optimization steps choose representations etc. which we don't 
    // want to save in the x-module info (i.e. x-module info is currently "high level"). 
    PrintWholeAssemblyImplementation tcConfig outfile "pass-start" tassembly;
#if DEBUG
    if tcConfig.showOptimizationData then dprintf "Expression prior to optimization:\n%s\n" (Layout.showL (Layout.squashTo 192 (DebugPrint.assemblyL tassembly)));
    if tcConfig.showOptimizationData then dprintf "CCU prior to optimization:\n%s\n" (Layout.showL (Layout.squashTo 192 (DebugPrint.entityL ccu.Contents)));
#endif

    let optEnv0 = optEnv
    let (TAssembly(implFiles)) = tassembly
    ReportTime tcConfig ("Optimizations");
    let results,(optEnvFirstLoop,_,_) = 
        ((optEnv0,optEnv0,optEnv0),implFiles) ||> List.mapFold (fun (optEnvFirstLoop,optEnvExtraLoop,optEnvFinalSimplify) implFile -> 

            // Only do abstract_big_targets on the first pass!  Only do it when TLR is on!  
            let optSettings = tcConfig.optSettings 
            let optSettings = { optSettings with abstractBigTargets = tcConfig.doTLR }
            let optSettings = { optSettings with reportingPhase = true }
            
            //ReportTime tcConfig ("Initial simplify");
            let optEnvFirstLoop,implFile,implFileOptData = 
                Opt.OptimizeImplFile(optSettings,ccu,tcGlobals,tcVal, importMap,optEnvFirstLoop,isIncrementalFragment,tcConfig.emitTailcalls,implFile)

            let implFile = AutoBox.TransformImplFile tcGlobals importMap implFile 
                            
            // Only do this on the first pass!
            let optSettings = { optSettings with abstractBigTargets = false }
            let optSettings = { optSettings with reportingPhase = false }
#if DEBUG
            if tcConfig.showOptimizationData then dprintf "Optimization implFileOptData:\n%s\n" (Layout.showL (Layout.squashTo 192 (Opt.moduleInfoL tcGlobals implFileOptData)));
#endif

            let implFile,optEnvExtraLoop = 
                if tcConfig.extraOptimizationIterations > 0 then 
                    //ReportTime tcConfig ("Extra simplification loop");
                    let optEnvExtraLoop,implFile, _ = Opt.OptimizeImplFile(optSettings,ccu,tcGlobals,tcVal, importMap,optEnvExtraLoop,isIncrementalFragment,tcConfig.emitTailcalls,implFile)
                    //PrintWholeAssemblyImplementation tcConfig outfile (sprintf "extra-loop-%d" n) implFile;
                    implFile,optEnvExtraLoop
                else
                    implFile,optEnvExtraLoop

            let implFile = 
                if tcConfig.doDetuple then 
                    //ReportTime tcConfig ("Detupled optimization");
                    let implFile = implFile |> Detuple.DetupleImplFile ccu tcGlobals 
                    //PrintWholeAssemblyImplementation tcConfig outfile "post-detuple" implFile;
                    implFile 
                else implFile 

            let implFile = 
                if tcConfig.doTLR then 
                    implFile |> Tlr.MakeTLRDecisions ccu tcGlobals 
                else implFile 

            let implFile = 
                Lowertop.LowerImplFile tcGlobals implFile

            let implFile,optEnvFinalSimplify =
                if tcConfig.doFinalSimplify then 
                    //ReportTime tcConfig ("Final simplify pass");
                    let optEnvFinalSimplify,implFile, _ = Opt.OptimizeImplFile(optSettings,ccu,tcGlobals,tcVal, importMap,optEnvFinalSimplify,isIncrementalFragment,tcConfig.emitTailcalls,implFile)
                    //PrintWholeAssemblyImplementation tcConfig outfile "post-rec-opt" implFile;
                    implFile,optEnvFinalSimplify 
                else 
                    implFile,optEnvFinalSimplify 
            (implFile,implFileOptData),(optEnvFirstLoop,optEnvExtraLoop,optEnvFinalSimplify))

    let implFiles,implFileOptDatas = List.unzip results
    let assemblyOptData = Opt.UnionModuleInfos implFileOptDatas
    let tassembly = TAssembly(implFiles)
    PrintWholeAssemblyImplementation tcConfig outfile "pass-end" tassembly;
    ReportTime tcConfig ("Ending Optimizations");

    tassembly, assemblyOptData,optEnvFirstLoop


//----------------------------------------------------------------------------
// ILX generation 
//----------------------------------------------------------------------------

let CreateIlxAssemblyGenerator (_tcConfig:TcConfig,tcImports:TcImports,tcGlobals, tcVal, generatedCcu) = 
    let ilxGenerator = new Ilxgen.IlxAssemblyGenerator (tcImports.GetImportMap(), tcGlobals, tcVal, generatedCcu)
    let ccus = tcImports.GetCcusInDeclOrder()
    ilxGenerator.AddExternalCcus ccus
    ilxGenerator

let GenerateIlxCode (ilxBackend, isInteractiveItExpr, isInteractiveOnMono, tcConfig:TcConfig, topAttrs, optimizedImpls, fragName, netFxHasSerializableAttribute, ilxGenerator : IlxAssemblyGenerator) =
    if !progress then dprintf "Generating ILX code...\n";
    let ilxGenOpts : IlxGenOptions = 
        { generateFilterBlocks = tcConfig.generateFilterBlocks;
          emitConstantArraysUsingStaticDataBlobs = not isInteractiveOnMono;
          workAroundReflectionEmitBugs=tcConfig.isInteractive; // REVIEW: is this still required? 
          generateDebugSymbols= tcConfig.debuginfo;
          fragName = fragName;
          localOptimizationsAreOn= tcConfig.optSettings.localOpt ();
          testFlagEmitFeeFeeAs100001 = tcConfig.testFlagEmitFeeFeeAs100001;
          mainMethodInfo= (if (tcConfig.target = Dll || tcConfig.target = Module) then None else Some topAttrs.mainMethodAttrs);
          ilxBackend = ilxBackend;
          isInteractive = tcConfig.isInteractive;
          isInteractiveItExpr = isInteractiveItExpr;
          netFxHasSerializableAttribute = netFxHasSerializableAttribute;
          alwaysCallVirt = tcConfig.alwaysCallVirt }

    ilxGenerator.GenerateCode (ilxGenOpts, optimizedImpls, topAttrs.assemblyAttrs,topAttrs.netModuleAttrs) 
#endif // !NO_COMPILER_BACKEND

//----------------------------------------------------------------------------
// Assembly ref normalization: make sure all assemblies are referred to
// by the same references. Only used for static linking.
//----------------------------------------------------------------------------

let NormalizeAssemblyRefs (tcImports:TcImports) scoref =
    match scoref with 
    | ILScopeRef.Local 
    | ILScopeRef.Module _ -> scoref
    | ILScopeRef.Assembly aref -> 
        match tcImports.TryFindDllInfo (Range.rangeStartup,aref.Name,lookupOnly=false) with 
        | Some dllInfo -> dllInfo.ILScopeRef
        | None -> scoref

let fsharpModuleName (t:CompilerTarget) (s:string) = 
    // return the name of the file as a module name
    let ext = match t with | Dll -> "dll" | Module -> "netmodule" | _ -> "exe"
    s + "." + ext


let ignoreFailureOnMono1_1_16 f = try f() with _ -> ()

let DoWithErrorColor isWarn f =
    if not enableConsoleColoring then
        f()
    else
        let foreBackColor =
            try
                let c = Console.ForegroundColor // may fail, perhaps on Mac, and maybe ForegroundColor is Black
                let b = Console.BackgroundColor // may fail, perhaps on Mac, and maybe BackgroundColor is White
                Some (c,b)
            with
                e -> None
        match foreBackColor with
          | None -> f() (* could not get console colours, so no attempt to change colours, can not set them back *)
          | Some (c,_) ->
              try
                let warnColor  = if Console.BackgroundColor = ConsoleColor.White then ConsoleColor.DarkBlue else ConsoleColor.Cyan
                let errorColor = ConsoleColor.Red
                ignoreFailureOnMono1_1_16 (fun () -> Console.ForegroundColor <- (if isWarn then warnColor else errorColor));
                f();
              finally
                ignoreFailureOnMono1_1_16 (fun () -> Console.ForegroundColor <- c)


          

        