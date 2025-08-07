ImplFile
  (ParsedImplFileInput
     ("/root/Expression/Yield 04.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (YieldOrReturn
                ((true, false),
                 Paren
                   (Typed
                      (ArrayOrListComputed
                         (false,
                          IndexRange
                            (Some (Const (Int32 1, (3,9--3,10))), (3,11--3,13),
                             Some (Const (Int32 10, (3,14--3,16))), (3,9--3,10),
                             (3,14--3,16), (3,9--3,16)), (3,7--3,18)),
                       App
                         (LongIdent (SynLongIdent ([list], [], [None])), None,
                          [LongIdent (SynLongIdent ([int], [], [None]))], [],
                          None, true, (3,20--3,28)), (3,7--3,28)), (3,6--3,7),
                    Some (3,28--3,29), (3,6--3,29)), (3,0--3,29),
                 { YieldOrReturnKeyword = (3,0--3,5) }), (3,0--3,29))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,29), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
