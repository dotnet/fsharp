ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Record - Field 12.fs", false, QualifiedNameOfFile Foo,
      [],
      [SynModuleOrNamespace
         ([Foo], false, NamedModule,
          [Expr
             (Record
                (None, None,
                 [SynExprRecordField
                    ((SynLongIdent ([F1], [], [None]), true), Some (3,5--3,6),
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
                                 (4,5--4,6)), Ident F2, (4,2--4,6)),
                           Const (Int32 2, (4,7--4,8)), (4,2--4,8))), (3,2--4,8),
                     None)], (3,0--4,10)), (3,0--4,10))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,10), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
