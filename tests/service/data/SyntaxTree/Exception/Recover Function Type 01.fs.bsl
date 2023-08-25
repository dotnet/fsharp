ImplFile
  (ParsedImplFileInput
     ("/root/Exception/Recover Function Type 01.fs", false,
      QualifiedNameOfFile Recover Function Type 01, [], [],
      [SynModuleOrNamespace
         ([Foo], false, DeclaredNamespace,
          [Exception
             (SynExceptionDefn
                (SynExceptionDefnRepr
                   ([],
                    SynUnionCase
                      ([], SynIdent (Bar, None),
                       Fields
                         [SynField
                            ([], false, None,
                             Fun
                               (LongIdent (SynLongIdent ([int], [], [None])),
                                LongIdent (SynLongIdent ([int], [], [None])),
                                (3,17--3,27), { ArrowRange = (3,21--3,23) }),
                             false,
                             PreXmlDoc ((3,17), FSharp.Compiler.Xml.XmlDocCollector),
                             None, (3,17--3,27), { LeadingKeyword = None })],
                       PreXmlDocEmpty, None, (3,10--3,27), { BarRange = None }),
                    None, PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                    None, (3,0--3,27)), None, [], (3,0--3,27)), (3,0--3,27));
           Exception
             (SynExceptionDefn
                (SynExceptionDefnRepr
                   ([],
                    SynUnionCase
                      ([], SynIdent (Other, None),
                       Fields
                         [SynField
                            ([], false, None,
                             LongIdent (SynLongIdent ([int], [], [None])), false,
                             PreXmlDoc ((4,19), FSharp.Compiler.Xml.XmlDocCollector),
                             None, (4,19--4,22), { LeadingKeyword = None })],
                       PreXmlDocEmpty, None, (4,10--4,22), { BarRange = None }),
                    None, PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                    None, (4,0--4,22)), None, [], (4,0--4,22)), (4,0--4,22))],
          PreXmlDocEmpty, [], None, (1,0--4,22),
          { LeadingKeyword = Namespace (1,0--1,9) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,17)-(3,27) parse error Unexpected function type in union case field definition. If you intend the field to be a function, consider wrapping the function signature with parens, e.g. | Case of a -> b into | Case of (a -> b).
