ImplFile
  (ParsedImplFileInput
     ("/root/Exception/SynExceptionDefnReprShouldContainTheRangeOfTheExceptionKeyword.fs",
      false, QualifiedNameOfFile X, [],
      [SynModuleOrNamespace
         ([X], false, NamedModule,
          [Exception
             (SynExceptionDefn
                (SynExceptionDefnRepr
                   ([],
                    SynUnionCase
                      ([], SynIdent (Foo, None),
                       Fields
                         [SynField
                            ([], false, None,
                             LongIdent (SynLongIdent ([string], [], [None])),
                             false,
                             PreXmlDoc ((5,17), FSharp.Compiler.Xml.XmlDocCollector),
                             None, (5,17--5,23), { LeadingKeyword = None
                                                   MutableKeyword = None })],
                       PreXmlDocEmpty, None, (5,10--5,23), { BarRange = None }),
                    None, PreXmlDoc ((5,0), FSharp.Compiler.Xml.XmlDocCollector),
                    None, (3,0--5,23), { ExceptionKeyword = (5,0--5,9) }), None,
                 [], (3,0--5,23)), (3,0--5,23))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,23), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (4,0--4,20)] }, set []))
