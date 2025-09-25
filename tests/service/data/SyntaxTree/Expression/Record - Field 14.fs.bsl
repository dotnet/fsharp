ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Record - Field 14.fs", false, QualifiedNameOfFile Foo,
      [],
      [SynModuleOrNamespace
         ([Foo], false, NamedModule,
          [Expr
             (Record
                (None, None,
                 [SynExprRecordField
                    ((SynLongIdent ([F1], [], [None]), true), Some (3,5--3,6),
                     Some (Const (Int32 1, (3,7--3,8))), (3,2--3,8), None);
                  SynExprRecordField
                    ((SynLongIdent ([F2], [], [None]), true), Some (4,5--4,6),
                     Some
                       (App
                          (NonAtomic, false,
                           App
                             (NonAtomic, true,
                              LongIdent
                                (false,
                                 SynLongIdent
                                   ([op_Equality], [],
                                    [Some (OriginalNotation "=")]), None,
                                 (5,5--5,6)), Ident F3, (5,2--5,6)),
                           Const (Int32 3, (5,7--5,8)), (5,2--5,8))), (4,2--5,8),
                     None)], (3,0--5,10), { OpeningBraceRange = (3,0--3,1)
                                            WithKeyword = None }), (3,0--5,10))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,10), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
