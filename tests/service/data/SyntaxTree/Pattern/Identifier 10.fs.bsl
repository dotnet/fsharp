ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Identifier 10.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Match
                (Yes (3,0--3,12), Ident e,
                 [SynMatchClause
                    (LongIdent
                       (SynLongIdent ([E], [(4,3--4,4)], [None]), None, None,
                        Pats [], None, (4,2--4,4)), None,
                     ArbitraryAfterError ("patternClauses2", (4,4--4,4)),
                     (4,2--4,4), Yes, { ArrowRange = None
                                        BarRange = Some (4,0--4,1) })],
                 (3,0--4,4), { MatchKeyword = (3,0--3,5)
                               WithKeyword = (3,8--3,12) }), (3,0--4,4))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,4), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(4,4)-(4,4) parse error Identifier expected
(5,0)-(5,0) parse error Incomplete structured construct at or before this point in pattern matching. Expected '->' or other token.
