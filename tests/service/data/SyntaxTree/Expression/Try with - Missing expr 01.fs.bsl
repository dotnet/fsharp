ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Try with - Missing expr 01.fs", false,
      QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (TryWith
                (ArbitraryAfterError ("try2", (3,3--3,3)),
                 [SynMatchClause
                    (Wild (4,5--4,6), None, Const (Unit, (4,10--4,12)),
                     (4,5--4,12), Yes, { ArrowRange = Some (4,7--4,9)
                                         BarRange = None })], (3,0--4,12),
                 Yes (3,0--3,3), Yes (4,0--4,4),
                 { TryKeyword = (3,0--3,3)
                   TryToWithRange = (3,0--4,4)
                   WithKeyword = (4,0--4,4)
                   WithToEndRange = (4,0--4,12) }), (3,0--4,12))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,12), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,0)-(4,4) parse error Expecting expression
