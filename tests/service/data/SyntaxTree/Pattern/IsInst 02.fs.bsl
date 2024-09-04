ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/IsInst 02.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Match
                (Yes (3,0--3,13), Const (Unit, (3,6--3,8)),
                 [SynMatchClause
                    (IsInst
                       (LongIdent (SynLongIdent ([T], [], [None])), (4,2--4,6)),
                     None, ArbitraryAfterError ("patternClauses2", (4,6--4,6)),
                     (4,2--4,6), Yes, { ArrowRange = None
                                        BarRange = Some (4,0--4,1) })],
                 (3,0--4,6), { MatchKeyword = (3,0--3,5)
                               WithKeyword = (3,9--3,13) }), (3,0--4,6));
           Expr (Const (Unit, (6,0--6,2)), (6,0--6,2))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,2), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(6,0)-(6,1) parse error Incomplete structured construct at or before this point in pattern matching. Expected '->' or other token.
