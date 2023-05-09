ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Try with - Missing expr 02.fs", false,
      QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (TryWith
                (ArbitraryAfterError ("typedSequentialExprBlockR", (3,0--3,3)),
                 [SynMatchClause
                    (Wild (4,5--4,6), None,
                     ArbitraryAfterError
                       ("typedSequentialExprBlockR", (4,7--4,9)), (4,5--4,9),
                     Yes, { ArrowRange = Some (4,7--4,9)
                            BarRange = None })], (3,0--4,9), Yes (3,0--3,3),
                 Yes (4,0--4,4), { TryKeyword = (3,0--3,3)
                                   TryToWithRange = (3,0--4,4)
                                   WithKeyword = (4,0--4,4)
                                   WithToEndRange = (4,0--4,9) }), (3,0--4,9))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,9), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,0)-(4,4) parse error Incomplete structured construct at or before this point in expression
(5,0)-(5,0) parse warning Possible incorrect indentation: this token is offside of context started at position (3:1). Try indenting this token further or using standard formatting conventions.
(5,0)-(5,0) parse error Incomplete structured construct at or before this point in pattern matching
