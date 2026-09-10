ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Identifier 03.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Match
                (Yes (3,0--3,12), Ident e,
                 [SynMatchClause
                    (LongIdent
                       (SynLongIdent ([E], [], [None]), None, None, Pats [],
                        None, (4,2--4,3)), None,
                     ArbitraryAfterError ("patternClauses2", (4,3--4,3)),
                     (4,2--4,3), Yes, { ArrowRange = None
                                        BarRange = Some (4,0--4,1) })],
                 (3,0--4,3), { MatchKeyword = (3,0--3,5)
                               WithKeyword = (3,8--3,12) }), (3,0--4,3))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,3), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(4,0)-(4,3) parse error Incomplete structured construct at or before this point in pattern matching. Expected '->' or other token.
