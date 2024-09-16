ImplFile
  (ParsedImplFileInput
     ("/root/LeadingKeyword/StaticAbstractKeyword.fs", false,
      QualifiedNameOfFile StaticAbstractKeyword, [], [],
      [SynModuleOrNamespace
         ([StaticAbstractKeyword], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [X],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (2,5--2,6)),
                  ObjectModel
                    (Unspecified,
                     [AbstractSlot
                        (SynValSig
                           ([], SynIdent (Y, None),
                            SynValTyparDecls (None, true),
                            Fun
                              (LongIdent (SynLongIdent ([int], [], [None])),
                               LongIdent (SynLongIdent ([int], [], [None])),
                               (3,24--3,34), { ArrowRange = (3,28--3,30) }),
                            SynValInfo
                              ([[SynArgInfo ([], false, None)]],
                               SynArgInfo ([], false, None)), false, false,
                            PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
                            Single None, None, (3,4--3,34),
                            { LeadingKeyword =
                               StaticAbstract ((3,4--3,10), (3,11--3,19))
                              InlineKeyword = None
                              WithKeyword = None
                              EqualsRange = None }),
                         { IsInstance = false
                           IsDispatchSlot = true
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = Member }, (3,4--3,34),
                         { GetSetKeywords = None })], (3,4--3,34)), [], None,
                  (2,5--3,34), { LeadingKeyword = Type (2,0--2,4)
                                 EqualsRange = Some (2,7--2,8)
                                 WithKeyword = None })], (2,0--3,34))],
          PreXmlDocEmpty, [], None, (2,0--4,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))

(3,4)-(3,19) parse warning Declaring "interfaces with static abstract methods" is an advanced feature. See https://aka.ms/fsharp-iwsams for guidance. You can disable this warning by using '#nowarn "3535"' or '--nowarn:3535'.
