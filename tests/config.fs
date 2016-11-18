module TestConfig

open System
open System.IO
open System.Collections.Generic
open Microsoft.Win32

open PlatformHelpers
open FSharpTestSuiteTypes

let private fileExists = Commands.fileExists __SOURCE_DIRECTORY__ >> Option.isSome

type private FSLibPaths = 
    { FSCOREDLLPATH : string }


let requireFile nm = 
    if fileExists nm then nm else failwith (sprintf "couldn't find %s" nm)

let config configurationName envVars =

    let SCRIPT_ROOT = __SOURCE_DIRECTORY__ 
    let FSCBinPath = SCRIPT_ROOT/".."/configurationName/"net40"/"bin"
    let FSDIFF = SCRIPT_ROOT/"fsharpqa"/"testenv"/"bin"/"diff.exe"

    let csc_flags = "/nologo" 
    let fsc_flags = "-r:System.Core.dll --nowarn:20 --define:COMPILED" 
    let fsi_flags = "-r:System.Core.dll --nowarn:20  --define:INTERACTIVE --maxerrors:1 --abortonerror" 

    let OSARCH, CORDIR, CORSDK = WindowsPlatform.clrPaths envVars
         
    let fsiroot = 
        match OSARCH with
        | X86 -> "fsi"
        | _ -> "fsiAnyCpu"

    let CSC = requireFile (CORDIR/"csc.exe")
    let NGEN = requireFile (CORDIR/"ngen.exe")
    let ILDASM = requireFile (CORSDK/"ildasm.exe")
    let SN = requireFile (CORSDK/"sn.exe") 
    let PEVERIFY = requireFile (CORSDK/"peverify.exe")
    let FSC = requireFile (FSCBinPath/"fsc.exe")
    let FSI = requireFile (FSCBinPath/(fsiroot+".exe"))
    let FSCOREDLLPATH = requireFile (FSCBinPath/"FSharp.Core.dll") 

    { EnvironmentVariables = envVars
      CORDIR = CORDIR |> Commands.pathAddBackslash
      CORSDK = CORSDK |> Commands.pathAddBackslash
      FSCBinPath = FSCBinPath |> Commands.pathAddBackslash
      FSCOREDLLPATH = FSCOREDLLPATH
      FSDIFF = FSDIFF
      ILDASM = ILDASM
      SN = SN
      NGEN = NGEN 
      PEVERIFY = PEVERIFY
      CSC = CSC 
      BUILD_CONFIG = configurationName
      FSC = FSC
      FSI = FSI
      csc_flags = csc_flags
      fsc_flags = fsc_flags 
      fsi_flags = fsi_flags 
      Directory="" }
    
let logConfig (cfg: TestConfig) =
    log "---------------------------------------------------------------"
    log "Executables"
    log ""
    log "CORDIR              =%s" cfg.CORDIR
    log "CORSDK              =%s" cfg.CORSDK
    log "CSC                 =%s" cfg.CSC
    log "BUILD_CONFIG        =%s" cfg.BUILD_CONFIG
    log "csc_flags           =%s" cfg.csc_flags
    log "FSC                 =%s" cfg.FSC
    log "fsc_flags           =%s" cfg.fsc_flags
    log "FSCBINPATH          =%s" cfg.FSCBinPath
    log "FSCOREDLLPATH       =%s" cfg.FSCOREDLLPATH
    log "FSDIFF              =%s" cfg.FSDIFF
    log "FSI                 =%s" cfg.FSI
    log "fsi_flags           =%s" cfg.fsi_flags
    log "ILDASM              =%s" cfg.ILDASM
    log "NGEN                =%s" cfg.NGEN
    log "PEVERIFY            =%s" cfg.PEVERIFY
    log "---------------------------------------------------------------"
