ImplFile
  (ParsedImplFileInput
     ("/root/UnionCaseFieldsCanHaveComments.fs", false,
      QualifiedNameOfFile UnionCaseFieldsCanHaveComments, [], [],
      [SynModuleOrNamespace
         ([UnionCaseFieldsCanHaveComments], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Foo],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None,
                     /root/UnionCaseFieldsCanHaveComments.fs (2,5--2,8)),
                  Simple
                    (Union
                       (None,
                        [SynUnionCase
                           ([], SynIdent (Thing, None),
                            Fields
                              [SynField
                                 ([], false, Some first,
                                  LongIdent
                                    (SynLongIdent ([string], [], [None])), false,
                                  PreXmlDoc ((6,2), FSharp.Compiler.Xml.XmlDocCollector),
                                  None,
                                  /root/UnionCaseFieldsCanHaveComments.fs (5,2--6,15),
                                  { LeadingKeyword = None });
                               SynField
                                 ([], false, None,
                                  LongIdent (SynLongIdent ([bool], [], [None])),
                                  false,
                                  PreXmlDoc ((8,2), FSharp.Compiler.Xml.XmlDocCollector),
                                  None,
                                  /root/UnionCaseFieldsCanHaveComments.fs (7,2--8,6),
                                  { LeadingKeyword = None })],
                            PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                            None,
                            /root/UnionCaseFieldsCanHaveComments.fs (3,0--8,6),
                            { BarRange =
                               Some
                                 /root/UnionCaseFieldsCanHaveComments.fs (4,0--4,1) })],
                        /root/UnionCaseFieldsCanHaveComments.fs (3,0--8,6)),
                     /root/UnionCaseFieldsCanHaveComments.fs (3,0--8,6)), [],
                  None, /root/UnionCaseFieldsCanHaveComments.fs (2,5--8,6),
                  { LeadingKeyword =
                     Type /root/UnionCaseFieldsCanHaveComments.fs (2,0--2,4)
                    EqualsRange =
                     Some /root/UnionCaseFieldsCanHaveComments.fs (2,9--2,10)
                    WithKeyword = None })],
              /root/UnionCaseFieldsCanHaveComments.fs (2,0--8,6))],
          PreXmlDocEmpty, [], None,
          /root/UnionCaseFieldsCanHaveComments.fs (2,0--9,0),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))