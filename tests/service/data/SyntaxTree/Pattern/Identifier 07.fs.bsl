ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Identifier 07.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Do
                (Sequential
                   (SuppressNeither, true,
                    Match
                      (Yes (4,4--4,16), Ident e,
                       [SynMatchClause
                          (LongIdent
                             (SynLongIdent ([E], [(5,7--5,8)], [None]), None,
                              None, Pats [], None, (5,6--5,8)), None,
                           ArbitraryAfterError ("patternClauses2", (5,8--5,8)),
                           (5,6--5,8), Yes, { ArrowRange = None
                                              BarRange = Some (5,4--5,5) })],
                       (4,4--5,8), { MatchKeyword = (4,4--4,9)
                                     WithKeyword = (4,12--4,16) }),
                    Const (Unit, (7,4--7,6)), (4,4--7,6),
                    { SeparatorRange = None }), (3,0--7,6)), (3,0--7,6))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,6), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(5,8)-(5,8) parse error Identifier expected
(7,4)-(7,5) parse error Incomplete structured construct at or before this point in pattern matching. Expected '->' or other token.
