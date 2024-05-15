module FSharp.Compiler.Interactive.FsiHelp

module Parser =

    type Help =
        { Summary: string
          Remarks: string option
          Parameters: (string * string) list
          Returns: string option
          Exceptions: (string * string) list
          Examples: (string * string) list
          FullName: string
          Assembly: string }

        member ToDisplayString: unit -> string

module Logic =

    module Quoted =

        val tryGetDocumentation: expr: Quotations.Expr -> Parser.Help option
