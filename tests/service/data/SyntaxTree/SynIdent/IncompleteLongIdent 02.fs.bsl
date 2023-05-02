ImplFile
  (ParsedImplFileInput
     ("/root/SynIdent/IncompleteLongIdent 02.fs", false,
      QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (DiscardAfterMissingQualificationAfterDot
                (LongIdent
                   (false, SynLongIdent ([A; B], [(4,1--4,2)], [None; None]),
                    None, (4,0--4,3)), (4,3--4,4), (4,0--4,4)), (4,0--4,4))],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--4,4), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,3)-(4,4) parse error Missing qualification after '.'
