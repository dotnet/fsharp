ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Binary - Plus 04.fs", false, QualifiedNameOfFile Module,
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
                         ([op_PipeRight], [], [Some (OriginalNotation "|>")]),
                       None, (4,0--4,2)),
                    App
                      (NonAtomic, false,
                       App
                         (NonAtomic, true,
                          LongIdent
                            (false,
                             SynLongIdent
                               ([op_Addition], [], [Some (OriginalNotation "+")]),
                             None, (3,2--3,3)), Ident a, (3,0--3,3)),
                       ArbitraryAfterError
                         ("declExprInfixPlusMinus", (3,3--3,3)), (3,0--3,3)),
                    (3,0--4,2)), Const (Unit, (4,3--4,5)), (3,0--4,5)),
              (3,0--4,5))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,5), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,0)-(4,2) parse error Unexpected infix operator in expression
(3,2)-(3,3) parse error Unexpected token '+' or incomplete expression
