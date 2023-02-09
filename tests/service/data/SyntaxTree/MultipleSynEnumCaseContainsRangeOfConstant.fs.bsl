ImplFile
  (ParsedImplFileInput
     ("/root/MultipleSynEnumCaseContainsRangeOfConstant.fs", false,
      QualifiedNameOfFile MultipleSynEnumCaseContainsRangeOfConstant, [], [],
      [SynModuleOrNamespace
         ([MultipleSynEnumCaseContainsRangeOfConstant], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Foo],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None,
                     /root/MultipleSynEnumCaseContainsRangeOfConstant.fs (1,5--1,8)),
                  Simple
                    (Enum
                       ([SynEnumCase
                           ([], SynIdent (One, None),
                            Const
                              (Int32 1,
                               /root/MultipleSynEnumCaseContainsRangeOfConstant.fs (2,13--2,23)),
                            PreXmlDoc ((2,4), FSharp.Compiler.Xml.XmlDocCollector),
                            /root/MultipleSynEnumCaseContainsRangeOfConstant.fs (2,6--2,23),
                            { BarRange =
                               Some
                                 /root/MultipleSynEnumCaseContainsRangeOfConstant.fs (2,4--2,5)
                              EqualsRange =
                               /root/MultipleSynEnumCaseContainsRangeOfConstant.fs (2,10--2,11) });
                         SynEnumCase
                           ([], SynIdent (Two, None),
                            Const
                              (Int32 2,
                               /root/MultipleSynEnumCaseContainsRangeOfConstant.fs (3,12--3,13)),
                            PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
                            /root/MultipleSynEnumCaseContainsRangeOfConstant.fs (3,6--3,13),
                            { BarRange =
                               Some
                                 /root/MultipleSynEnumCaseContainsRangeOfConstant.fs (3,4--3,5)
                              EqualsRange =
                               /root/MultipleSynEnumCaseContainsRangeOfConstant.fs (3,10--3,11) })],
                        /root/MultipleSynEnumCaseContainsRangeOfConstant.fs (2,4--3,13)),
                     /root/MultipleSynEnumCaseContainsRangeOfConstant.fs (2,4--3,13)),
                  [], None,
                  /root/MultipleSynEnumCaseContainsRangeOfConstant.fs (1,5--3,13),
                  { LeadingKeyword =
                     Type
                       /root/MultipleSynEnumCaseContainsRangeOfConstant.fs (1,0--1,4)
                    EqualsRange =
                     Some
                       /root/MultipleSynEnumCaseContainsRangeOfConstant.fs (1,9--1,10)
                    WithKeyword = None })],
              /root/MultipleSynEnumCaseContainsRangeOfConstant.fs (1,0--3,13))],
          PreXmlDocEmpty, [], None,
          /root/MultipleSynEnumCaseContainsRangeOfConstant.fs (1,0--3,13),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))