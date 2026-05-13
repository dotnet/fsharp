ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Match 03.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Do
                (Match
                   (Yes (4,4--4,16), Const (Int32 1, (4,10--4,11)),
                    [SynMatchClause
                       (Const (Int32 2, (6,4--6,5)), None,
                        ArbitraryAfterError ("patternClauses2", (6,5--6,5)),
                        (6,4--6,5), Yes, { ArrowRange = None
                                           BarRange = None })], (4,4--6,5),
                    { MatchKeyword = (4,4--4,9)
                      WithKeyword = (4,12--4,16) }), (3,0--6,5)), (3,0--6,5))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,5), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(7,0)-(7,0) parse error Incomplete structured construct at or before this point in pattern matching. Expected '->' or other token.
