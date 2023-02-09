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
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None,
                     /root/UnionCaseFieldsCanHaveComments.fs (1,5--1,8)),
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
                                  PreXmlDoc ((5,2), FSharp.Compiler.Xml.XmlDocCollector),
                                  None,
                                  /root/UnionCaseFieldsCanHaveComments.fs (4,2--5,15),
                                  { LeadingKeyword = None });
                               SynField
                                 ([], false, None,
                                  LongIdent (SynLongIdent ([bool], [], [None])),
                                  false,
                                  PreXmlDoc ((7,2), FSharp.Compiler.Xml.XmlDocCollector),
                                  None,
                                  /root/UnionCaseFieldsCanHaveComments.fs (6,2--7,6),
                                  { LeadingKeyword = None })],
                            PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                            None,
                            /root/UnionCaseFieldsCanHaveComments.fs (2,0--7,6),
                            { BarRange =
                               Some
                                 /root/UnionCaseFieldsCanHaveComments.fs (3,0--3,1) })],
                        /root/UnionCaseFieldsCanHaveComments.fs (2,0--7,6)),
                     /root/UnionCaseFieldsCanHaveComments.fs (2,0--7,6)), [],
                  None, /root/UnionCaseFieldsCanHaveComments.fs (1,5--7,6),
                  { LeadingKeyword =
                     Type /root/UnionCaseFieldsCanHaveComments.fs (1,0--1,4)
                    EqualsRange =
                     Some /root/UnionCaseFieldsCanHaveComments.fs (1,9--1,10)
                    WithKeyword = None })],
              /root/UnionCaseFieldsCanHaveComments.fs (1,0--7,6))],
          PreXmlDocEmpty, [], None,
          /root/UnionCaseFieldsCanHaveComments.fs (1,0--7,6),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))