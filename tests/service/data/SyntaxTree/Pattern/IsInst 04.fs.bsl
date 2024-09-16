ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/IsInst 04.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Match
                (Yes (3,0--3,13), Const (Unit, (3,6--3,8)),
                 [SynMatchClause
                    (IsInst (FromParseError (4,4--4,4), (4,2--4,4)), None,
                     ArbitraryAfterError ("patternClauses2", (4,4--4,4)),
                     (4,2--4,4), Yes, { ArrowRange = None
                                        BarRange = Some (4,0--4,1) })],
                 (3,0--4,4), { MatchKeyword = (3,0--3,5)
                               WithKeyword = (3,9--3,13) }), (3,0--4,4));
           Expr (Const (Unit, (6,0--6,2)), (6,0--6,2))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,2), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,2)-(4,4) parse error Expecting type
(6,0)-(6,1) parse error Incomplete structured construct at or before this point in pattern matching. Expected '->' or other token.
