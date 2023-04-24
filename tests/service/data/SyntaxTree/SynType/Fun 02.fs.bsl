ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Fun 02.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Paren
                (Typed
                   (Const (Unit, (3,1--3,3)),
                    Fun
                      (LongIdent (SynLongIdent ([a], [], [None])),
                       Fun
                         (LongIdent (SynLongIdent ([b], [], [None])),
                          LongIdent (SynLongIdent ([c], [], [None])),
                          (3,10--3,16), { ArrowRange = (3,12--3,14) }),
                       (3,5--3,16), { ArrowRange = (3,7--3,9) }), (3,1--3,16)),
                 (3,0--3,1), Some (3,16--3,17), (3,0--3,17)), (3,0--3,17))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,17), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
