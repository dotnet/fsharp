ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Yield 01.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (YieldOrReturn
                ((true, false),
                 Typed
                   (ArrayOrListComputed
                      (false,
                       IndexRange
                         (Some (Const (Int32 1, (3,8--3,9))), (3,10--3,12),
                          Some (Const (Int32 10, (3,13--3,15))), (3,8--3,9),
                          (3,13--3,15), (3,8--3,15)), (3,6--3,17)),
                    App
                      (LongIdent (SynLongIdent ([list], [], [None])), None,
                       [LongIdent (SynLongIdent ([int], [], [None]))], [], None,
                       true, (3,19--3,27)), (3,6--3,27)), (3,0--3,27),
                 { YieldOrReturnKeyword = (3,0--3,5) }), (3,0--3,27))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,27), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
