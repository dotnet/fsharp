ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/QuoteExprInPattern.fs", false,
      QualifiedNameOfFile QuoteExprInPattern, [],
      [SynModuleOrNamespace
         ([QuoteExprInPattern], false, NamedModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None,
                     SynValInfo
                       ([[SynArgInfo ([], false, Some x)]],
                        SynArgInfo ([], false, None)), None),
                  LongIdent
                    (SynLongIdent ([f], [], [None]), None, None,
                     Pats [Named (SynIdent (x, None), false, None, (3,6--3,7))],
                     None, (3,4--3,7)), None,
                  Match
                    (Yes (4,4--4,16), Ident x,
                     [SynMatchClause
                        (QuoteExpr
                           (Quote
                              (Ident op_Quotation, false,
                               App
                                 (NonAtomic, false,
                                  App
                                    (NonAtomic, true,
                                     LongIdent
                                       (false,
                                        SynLongIdent
                                          ([op_Addition], [],
                                           [Some (OriginalNotation "+")]), None,
                                        (5,11--5,12)),
                                     Const (Int32 1, (5,9--5,10)), (5,9--5,12)),
                                  Const (Int32 2, (5,13--5,14)), (5,9--5,14)),
                               false, (5,6--5,17)), (5,6--5,17)), None,
                         Const (Unit, (5,21--5,23)), (5,6--5,23), Yes,
                         { ArrowRange = Some (5,18--5,20)
                           BarRange = Some (5,4--5,5) })], (4,4--5,23),
                     { MatchKeyword = (4,4--4,9)
                       WithKeyword = (4,12--4,16) }), (3,4--3,7), NoneAtLet,
                  { LeadingKeyword = Let (3,0--3,3)
                    InlineKeyword = None
                    EqualsRange = Some (3,8--3,9) })], (3,0--5,23))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,23), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
