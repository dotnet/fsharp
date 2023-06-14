ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Binary - Plus 02.fs", false, QualifiedNameOfFile Module,
      [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (App
                (NonAtomic, false,
                 App
                   (NonAtomic, true,
                    LongIdent
                      (false,
                       SynLongIdent
                         ([op_Addition], [], [Some (OriginalNotation "+")]),
                       None, (3,2--3,3)), Ident a, (3,0--3,3)),
                 ArbitraryAfterError ("declExprInfixPlusMinus", (3,3--3,3)),
                 (3,0--3,3)), (3,0--3,3))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,3), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,0)-(4,0) parse error Possible incorrect indentation: this token is offside of context started at position (3:1). Try indenting this token further or using standard formatting conventions.
(3,2)-(3,3) parse error Unexpected token '+' or incomplete expression
