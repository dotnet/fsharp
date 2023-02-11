ImplFile
  (ParsedImplFileInput
     ("/root/NestedModule/IncompleteNestedModuleShouldBePresent.fs", false,
      QualifiedNameOfFile A.B, [], [],
      [SynModuleOrNamespace
         ([A; B], false, NamedModule,
          [NestedModule
             (SynComponentInfo
                ([], None, [], [C],
                 PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None,
                 /root/NestedModule/IncompleteNestedModuleShouldBePresent.fs (3,0--3,8)),
              false, [], false,
              /root/NestedModule/IncompleteNestedModuleShouldBePresent.fs (3,0--3,8),
              { ModuleKeyword =
                 Some
                   /root/NestedModule/IncompleteNestedModuleShouldBePresent.fs (3,0--3,6)
                EqualsRange = None });
           Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((5,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named
                    (SynIdent (a, None), false, None,
                     /root/NestedModule/IncompleteNestedModuleShouldBePresent.fs (5,4--5,5)),
                  None,
                  Const
                    (Unit,
                     /root/NestedModule/IncompleteNestedModuleShouldBePresent.fs (5,8--5,10)),
                  /root/NestedModule/IncompleteNestedModuleShouldBePresent.fs (5,4--5,5),
                  Yes
                    /root/NestedModule/IncompleteNestedModuleShouldBePresent.fs (5,0--5,10),
                  { LeadingKeyword =
                     Let
                       /root/NestedModule/IncompleteNestedModuleShouldBePresent.fs (5,0--5,3)
                    InlineKeyword = None
                    EqualsRange =
                     Some
                       /root/NestedModule/IncompleteNestedModuleShouldBePresent.fs (5,6--5,7) })],
              /root/NestedModule/IncompleteNestedModuleShouldBePresent.fs (5,0--5,10))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          /root/NestedModule/IncompleteNestedModuleShouldBePresent.fs (1,0--5,10),
          { LeadingKeyword =
             Module
               /root/NestedModule/IncompleteNestedModuleShouldBePresent.fs (1,0--1,6) })],
      (true, false), { ConditionalDirectives = []
                       CodeComments = [] }, set []))
