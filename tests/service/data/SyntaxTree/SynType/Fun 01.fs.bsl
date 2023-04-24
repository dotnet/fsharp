ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Fun 01.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Paren
                (Typed
                   (Const (Unit, (3,1--3,3)),
                    Fun
                      (LongIdent (SynLongIdent ([a], [], [None])),
                       LongIdent (SynLongIdent ([b], [], [None])), (3,5--3,11),
                       { ArrowRange = (3,7--3,9) }), (3,1--3,11)), (3,0--3,1),
                 Some (3,11--3,12), (3,0--3,12)), (3,0--3,12))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,12), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
