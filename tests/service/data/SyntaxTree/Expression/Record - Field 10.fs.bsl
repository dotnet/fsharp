ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Record - Field 10.fs", false, QualifiedNameOfFile Foo,
      [], [],
      [SynModuleOrNamespace
         ([Foo], false, NamedModule,
          [Expr
             (ComputationExpr
                (false,
                 Sequential
                   (SuppressNeither, true, Ident A,
                    App
                      (NonAtomic, false,
                       App
                         (NonAtomic, true,
                          LongIdent
                            (false,
                             SynLongIdent
                               ([op_Equality], [], [Some (OriginalNotation "=")]),
                             None, (4,4--4,5)), Ident B, (4,2--4,5)),
                       Const (Int32 1, (4,6--4,7)), (4,2--4,7)), (3,2--4,7)),
                 (3,0--4,9)), (3,0--4,9))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,9), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
