ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Binary 02.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (App
                (NonAtomic, false,
                 App
                   (NonAtomic, true,
                    LongIdent
                      (false,
                       SynLongIdent
                         ([op_Addition], [], [Some (OriginalNotation "+")]),
                       None, (3,6--3,7)),
                    App
                      (NonAtomic, false,
                       App
                         (NonAtomic, true,
                          LongIdent
                            (false,
                             SynLongIdent
                               ([op_Addition], [], [Some (OriginalNotation "+")]),
                             None, (3,2--3,3)), Const (Int32 1, (3,0--3,1)),
                          (3,0--3,3)), Const (Int32 2, (3,4--3,5)), (3,0--3,5)),
                    (3,0--3,7)), Const (Int32 3, (3,8--3,9)), (3,0--3,9)),
              (3,0--3,9))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,9), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
