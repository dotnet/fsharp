ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Binary - Eq 07.fs", false, QualifiedNameOfFile Module,
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
                        ArbitraryAfterError ("tupleExpr8", (3,8--3,8));
                        App
                          (NonAtomic, false,
                           App
                             (NonAtomic, true,
                              LongIdent
                                (false,
                                 SynLongIdent
                                   ([op_Equality], [],
                                    [Some (OriginalNotation "=")]), None,
                                 (3,13--3,14)), Ident c, (3,11--3,14)),
                           Const (Int32 3, (3,15--3,16)), (3,11--3,16))],
                       [(3,7--3,8); (3,9--3,10)], (3,2--3,16)), (3,1--3,2),
                    Some (3,16--3,17), (3,1--3,17)), (3,0--3,17)), (3,0--3,17))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,17), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,9)-(3,10) parse error Expecting expression
