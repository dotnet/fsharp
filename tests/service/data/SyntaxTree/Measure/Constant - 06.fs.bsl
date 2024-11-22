ImplFile
  (ParsedImplFileInput
     ("/root/Measure/Constant - 06.fs", false, QualifiedNameOfFile M, [],
      [SynModuleOrNamespace
         ([M], false, NamedModule,
          [Expr
             (Const
                (Measure
                   (Int32 42, (3,0--3,2),
                    Seq
                      ([Power
                          (Named ([m], (3,3--3,4)), (3,4--3,5),
                           Integer (12345, (3,5--3,10)), (3,3--3,10))],
                       (3,3--3,10)), { LessRange = (3,2--3,3)
                                       GreaterRange = (3,10--3,11) }),
                 (3,0--3,11)), (3,0--3,11))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,11), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
