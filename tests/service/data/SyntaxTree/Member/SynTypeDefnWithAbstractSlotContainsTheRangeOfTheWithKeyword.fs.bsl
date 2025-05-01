ImplFile
  (ParsedImplFileInput
     ("/root/Member/SynTypeDefnWithAbstractSlotContainsTheRangeOfTheWithKeyword.fs",
      false,
      QualifiedNameOfFile
        SynTypeDefnWithAbstractSlotContainsTheRangeOfTheWithKeyword, [], [],
      [SynModuleOrNamespace
         ([SynTypeDefnWithAbstractSlotContainsTheRangeOfTheWithKeyword], false,
          AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Foo],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (2,5--2,8)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [], Const (Unit, (2,8--2,10)), None,
                         PreXmlDoc ((2,8), FSharp.Compiler.Xml.XmlDocCollector),
                         (2,5--2,8), { AsKeyword = None });
                      AbstractSlot
                        (SynValSig
                           ([], SynIdent (Bar, None),
                            SynValTyparDecls (None, true),
                            LongIdent (SynLongIdent ([int], [], [None])),
                            SynValInfo ([], SynArgInfo ([], false, None)), false,
                            false,
                            PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
                            Single None, None, (3,4--3,42),
                            { LeadingKeyword =
                               AbstractMember ((3,4--3,12), (3,13--3,19))
                              InlineKeyword = None
                              WithKeyword = Some (3,30--3,34)
                              EqualsRange = None }),
                         { IsInstance = true
                           IsDispatchSlot = true
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = PropertyGetSet }, (3,4--3,42),
                         { GetSetKeywords =
                            Some (GetSet ((3,35--3,38), (3,39--3,42))) })],
                     (3,4--3,42)), [],
                  Some
                    (ImplicitCtor
                       (None, [], Const (Unit, (2,8--2,10)), None,
                        PreXmlDoc ((2,8), FSharp.Compiler.Xml.XmlDocCollector),
                        (2,5--2,8), { AsKeyword = None })), (2,5--3,42),
                  { LeadingKeyword = Type (2,0--2,4)
                    EqualsRange = Some (2,11--2,12)
                    WithKeyword = None })], (2,0--3,42))], PreXmlDocEmpty, [],
          None, (2,0--4,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
