ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Binary - Eq 03.fs", false, QualifiedNameOfFile Module,
      [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (App
                (Atomic, false, Ident M,
                 Paren
                   (App
                      (NonAtomic, false,
                       App
                         (NonAtomic, true,
                          LongIdent
                            (false,
                             SynLongIdent
                               ([op_Equality], [], [Some (OriginalNotation "=")]),
                             None, (3,4--3,5)), Ident a, (3,2--3,5)),
                       ArbitraryAfterError ("declExprInfixEquals", (3,5--3,5)),
                       (3,2--3,5)), (3,1--3,2), Some (3,6--3,7), (3,1--3,7)),
                 (3,0--3,7)), (3,0--3,7))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,7), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,4)-(3,5) parse error Unexpected token '=' or incomplete expression
