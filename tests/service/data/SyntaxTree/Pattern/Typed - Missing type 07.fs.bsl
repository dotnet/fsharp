ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Typed - Missing type 07.fs", false,
      QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Match
                (Yes (3,0--3,13), Const (Unit, (3,6--3,8)),
                 [SynMatchClause
                    (Typed
                       (Named (SynIdent (i, None), false, None, (4,2--4,3)),
                        FromParseError (4,4--4,4), (4,2--4,4)), None,
                     ArbitraryAfterError ("patternClauses2", (4,4--4,4)),
                     (4,2--4,4), Yes, { ArrowRange = None
                                        BarRange = Some (4,0--4,1) })],
                 (3,0--4,4), { MatchKeyword = (3,0--3,5)
                               WithKeyword = (3,9--3,13) }), (3,0--4,4))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,4), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(5,0)-(5,0) parse error Incomplete structured construct at or before this point in pattern
(3,9)-(3,13) parse error Unexpected end of input in 'match' or 'try' expression
