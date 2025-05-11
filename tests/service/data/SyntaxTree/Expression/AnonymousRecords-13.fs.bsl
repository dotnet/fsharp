ImplFile
  (ParsedImplFileInput
     ("/root/Expression/AnonymousRecords-13.fs", false,
      QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (AnonRecd
                (false, None,
                 [(SynLongIdent ([a], [], [None]), Some (3,4--3,5),
                   Quote
                     (Ident op_Quotation, false, Const (Int32 3, (3,9--3,10)),
                      false, (3,6--3,13)))], (3,0--3,16),
                 { OpeningBraceRange = (3,0--3,2) }), (3,0--3,16));
           Expr
             (AnonRecd
                (false, None,
                 [(SynLongIdent ([a], [], [None]), Some (5,4--5,5),
                   FromParseError
                     (Quote
                        (Ident op_Quotation, false,
                         App
                           (NonAtomic, false,
                            App
                              (NonAtomic, true,
                               LongIdent
                                 (false,
                                  SynLongIdent
                                    ([op_AtGreaterBar], [],
                                     [Some (OriginalNotation "@>|")]), None,
                                  (5,11--5,14)), Const (Int32 3, (5,9--5,10)),
                               (5,9--5,14)),
                            ArbitraryAfterError ("declExprInfix", (5,14--5,14)),
                            (5,9--5,14)), false, (5,6--5,15)), (5,6--5,15)))],
                 (5,0--5,15), { OpeningBraceRange = (5,0--5,2) }), (5,0--5,15))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,15), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(5,11)-(5,14) parse error Unexpected token '@>|' or incomplete expression
(5,14)-(5,15) parse error Unexpected symbol '}' in quotation literal. Expected end of quotation or other token.
(5,6)-(5,8) parse error Unmatched '<@ @>'
(5,0)-(5,2) parse error Unmatched '{|'
