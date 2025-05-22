ImplFile
  (ParsedImplFileInput
     ("/root/Expression/YieldBang 02.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (YieldOrReturnFrom
                ((true, false),
                 Paren
                   (Typed
                      (ArrayOrListComputed
                         (false,
                          IndexRange
                            (Some (Const (Int32 1, (3,10--3,11))), (3,12--3,14),
                             Some (Const (Int32 10, (3,15--3,17))), (3,10--3,11),
                             (3,15--3,17), (3,10--3,17)), (3,8--3,19)),
                       App
                         (LongIdent (SynLongIdent ([list], [], [None])), None,
                          [LongIdent (SynLongIdent ([int], [], [None]))], [],
                          None, true, (3,21--3,29)), (3,8--3,29)), (3,7--3,8),
                    Some (3,29--3,30), (3,7--3,30)), (3,0--3,30),
                 { YieldOrReturnFromKeyword = (3,0--3,6) }), (3,0--3,30))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,30), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
