ImplFile
  (ParsedImplFileInput
     ("/root/Expression/If 06.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Do
                (IfThenElse
                   (Const (Bool true, (4,7--4,11)),
                    ArbitraryAfterError
                      ("typedSequentialExprBlock1", (4,16--4,16)), None,
                    Yes (4,4--4,16), false, (4,4--4,16),
                    { IfKeyword = (4,4--4,6)
                      IsElif = false
                      ThenKeyword = (4,12--4,16)
                      ElseKeyword = None
                      IfToThenRange = (4,4--4,16) }), (3,0--4,16)), (3,0--4,16));
           Open
             (ModuleOrNamespace
                (SynLongIdent
                   ([`global`], [], [Some (OriginalNotation "global")]),
                 (6,5--6,11)), (6,0--6,11))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,11), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(6,0)-(6,4) parse error Possible incorrect indentation: this token is offside of context started at position (4:5). Try indenting this token further or using standard formatting conventions.
(6,0)-(6,4) parse error Expecting expression
