ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Binary - Eq 06.fs", false, QualifiedNameOfFile Module,
      [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (App
                (Atomic, false, Ident M,
                 Paren
                   (Tuple
                      (false,
                       [App
                          (NonAtomic, false,
                           App
                             (NonAtomic, true,
                              LongIdent
                                (false,
                                 SynLongIdent
                                   ([op_Equality], [],
                                    [Some (OriginalNotation "=")]), None,
                                 (3,4--3,5)), Ident a, (3,2--3,5)),
                           Const (Int32 1, (3,6--3,7)), (3,2--3,7));
                        App
                          (NonAtomic, false,
                           App
                             (NonAtomic, true,
                              LongIdent
                                (false,
                                 SynLongIdent
                                   ([op_Equality], [],
                                    [Some (OriginalNotation "=")]), None,
                                 (3,11--3,12)), Ident b, (3,9--3,12)),
                           ArbitraryAfterError
                             ("declExprInfixEquals", (3,12--3,12)), (3,9--3,12));
                        App
                          (NonAtomic, false,
                           App
                             (NonAtomic, true,
                              LongIdent
                                (false,
                                 SynLongIdent
                                   ([op_Equality], [],
                                    [Some (OriginalNotation "=")]), None,
                                 (3,16--3,17)), Ident c, (3,14--3,17)),
                           Const (Int32 3, (3,18--3,19)), (3,14--3,19))],
                       [(3,7--3,8); (3,12--3,13)], (3,2--3,19)), (3,1--3,2),
                    Some (3,19--3,20), (3,1--3,20)), (3,0--3,20)), (3,0--3,20))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,20), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,12)-(3,13) parse error Unexpected symbol ',' in expression
(3,11)-(3,12) parse error Unexpected token '=' or incomplete expression
