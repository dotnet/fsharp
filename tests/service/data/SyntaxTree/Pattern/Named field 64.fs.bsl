ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Named field 64.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Match
                (Yes (3,0--3,12), Const (Int32 1, (3,6--3,7)),
                 [SynMatchClause
                    (LongIdent
                       (SynLongIdent ([A], [], [None]), None, None,
                        NamePatPairs
                          ([NamePatPairField
                              (SynLongIdent ([a], [], [None]), Some (4,6--4,7),
                               (4,4--4,15),
                               QuoteExpr
                                 (Quote
                                    (Ident op_Quotation, false,
                                     Const (Int32 1, (4,11--4,12)), false,
                                     (4,8--4,15)), (4,8--4,15)),
                               Some (Comma ((4,15--4,16), Some (4,16))));
                            NamePatPairField
                              (SynLongIdent ([b], [], [None]), Some (4,19--4,20),
                               (4,17--4,30),
                               QuoteExpr
                                 (Quote
                                    (Ident op_QuotationUntyped, true, Ident A,
                                     false, (4,21--4,30)), (4,21--4,30)), None)],
                           (4,4--4,31), { ParenRange = (4,3--4,31) }), None,
                        (4,2--4,31)), None, Const (Unit, (4,35--4,37)),
                     (4,2--4,37), Yes, { ArrowRange = Some (4,32--4,34)
                                         BarRange = Some (4,0--4,1) })],
                 (3,0--4,37), { MatchKeyword = (3,0--3,5)
                                WithKeyword = (3,8--3,12) }), (3,0--4,37))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,37), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
