ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/As 12.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Match
                (Yes (3,0--3,12), Ident a,
                 [SynMatchClause
                    (As
                       (IsInst
                          (LongIdent (SynLongIdent ([T], [], [None])),
                           (4,2--4,6)), Wild (4,9--4,9), (4,2--4,9)), None,
                     ArbitraryAfterError ("patternClauses2", (4,9--4,9)),
                     (4,2--4,9), Yes, { ArrowRange = None
                                        BarRange = Some (4,0--4,1) })],
                 (3,0--4,9), { MatchKeyword = (3,0--3,5)
                               WithKeyword = (3,8--3,12) }), (3,0--4,9));
           Expr (Const (Unit, (6,0--6,2)), (6,0--6,2))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,2), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,7)-(4,9) parse error Expecting pattern
(6,0)-(6,1) parse error Incomplete structured construct at or before this point in pattern matching. Expected '->' or other token.
