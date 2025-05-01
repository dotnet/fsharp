ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/As 05.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Match
                (Yes (3,0--3,17),
                 App
                   (NonAtomic, false, Ident Some, Const (Int32 1, (3,11--3,12)),
                    (3,6--3,12)),
                 [SynMatchClause
                    (As (Wild (4,2--4,3), Wild (4,6--4,6), (4,2--4,6)), None,
                     ArbitraryAfterError ("patternClauses2", (4,6--4,6)),
                     (4,2--4,6), Yes, { ArrowRange = None
                                        BarRange = Some (4,0--4,1) })],
                 (3,0--4,6), { MatchKeyword = (3,0--3,5)
                               WithKeyword = (3,13--3,17) }), (3,0--4,6))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,6), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,4)-(4,6) parse error Expecting pattern
(5,0)-(5,0) parse error Incomplete structured construct at or before this point in pattern matching. Expected '->' or other token.
