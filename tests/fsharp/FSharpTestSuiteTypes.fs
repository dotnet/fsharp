module FSharpTestSuiteTypes

open PlatformHelpers

type ProcessorArchitecture = 
    | X86
    | IA64
    | AMD64
    | Unknown of string
    override this.ToString() = 
        match this with
        | X86 -> "x86"
        | IA64 -> "IA64"
        | AMD64 -> "AMD64"
        | Unknown arc -> arc

type RunError = 
    | GenericError of string
    | ProcessExecError of (string * int * string)
    | Skipped of string

type Permutation = 
    | FSI_FILE
    | FSI_STDIN
    | FSI_STDIN_OPT
    | FSI_STDIN_GUI
    | FSC_CORECLR
    | FSC_BASIC
    | FSC_BASIC_64
    | GENERATED_SIGNATURE
    | FSC_OPT_MINUS_DEBUG
    | FSC_OPT_PLUS_DEBUG
    | AS_DLL
    override this.ToString() = (sprintf "%A" this)

type TestConfig = 
    { EnvironmentVariables : Map<string, string>
      CORDIR : string
      CORSDK : string
      CSC : string
      csc_flags : string
      BUILD_CONFIG : string
      FSC : string
      fsc_flags : string
      FSCBinPath : string
      FSCOREDLLPATH : string
      FSDIFF : string
      FSI : string
      fsi_flags : string
      ILDASM : string
      SN : string
      NGEN : string
      PEVERIFY : string
      Directory: string }

