ImplFile
  (ParsedImplFileInput
     ("/root/RangeOfAttributeShouldBeIncludedInSynModuleDeclLet.fs", false,
      QualifiedNameOfFile RangeOfAttributeShouldBeIncludedInSynModuleDeclLet, [],
      [],
      [SynModuleOrNamespace
         ([RangeOfAttributeShouldBeIncludedInSynModuleDeclLet], false,
          AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false,
                  [{ Attributes =
                      [{ TypeName = SynLongIdent ([Foo], [], [None])
                         ArgExpr =
                          Const
                            (Unit,
                             /root/RangeOfAttributeShouldBeIncludedInSynModuleDeclLet.fs (1,2--1,5))
                         Target = None
                         AppliesToGetterAndSetter = false
                         Range =
                          /root/RangeOfAttributeShouldBeIncludedInSynModuleDeclLet.fs (1,2--1,5) }]
                     Range =
                      /root/RangeOfAttributeShouldBeIncludedInSynModuleDeclLet.fs (1,0--1,7) }],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named
                    (SynIdent (a, None), false, None,
                     /root/RangeOfAttributeShouldBeIncludedInSynModuleDeclLet.fs (2,4--2,5)),
                  None,
                  Const
                    (Int32 0,
                     /root/RangeOfAttributeShouldBeIncludedInSynModuleDeclLet.fs (2,8--2,9)),
                  /root/RangeOfAttributeShouldBeIncludedInSynModuleDeclLet.fs (1,0--2,5),
                  Yes
                    /root/RangeOfAttributeShouldBeIncludedInSynModuleDeclLet.fs (1,0--2,9),
                  { LeadingKeyword =
                     Let
                       /root/RangeOfAttributeShouldBeIncludedInSynModuleDeclLet.fs (2,0--2,3)
                    InlineKeyword = None
                    EqualsRange =
                     Some
                       /root/RangeOfAttributeShouldBeIncludedInSynModuleDeclLet.fs (2,6--2,7) })],
              /root/RangeOfAttributeShouldBeIncludedInSynModuleDeclLet.fs (1,0--2,9))],
          PreXmlDocEmpty, [], None,
          /root/RangeOfAttributeShouldBeIncludedInSynModuleDeclLet.fs (1,0--2,9),
          { LeadingKeyword = None })], (true, false),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))