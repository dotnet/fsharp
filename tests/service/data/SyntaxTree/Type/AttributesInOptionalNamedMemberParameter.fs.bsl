ImplFile
  (ParsedImplFileInput
     ("/root/Type/AttributesInOptionalNamedMemberParameter.fs", false,
      QualifiedNameOfFile AttributesInOptionalNamedMemberParameter, [], [],
      [SynModuleOrNamespace
         ([AttributesInOptionalNamedMemberParameter], false, AnonModule,
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
                              (SignatureParameter
                                 ([{ Attributes =
                                      [{ TypeName =
                                          SynLongIdent ([Foo], [], [None])
                                         ArgExpr = Const (Unit, (3,25--3,28))
                                         Target = None
                                         AppliesToGetterAndSetter = false
                                         Range = (3,25--3,28) };
                                       { TypeName =
                                          SynLongIdent ([Bar], [], [None])
                                         ArgExpr = Const (Unit, (3,30--3,33))
                                         Target = None
                                         AppliesToGetterAndSetter = false
                                         Range = (3,30--3,33) }]
                                     Range = (3,23--3,35) }], true, Some a,
                                  LongIdent (SynLongIdent ([A], [], [None])),
                                  (3,23--3,41)),
                               LongIdent (SynLongIdent ([B], [], [None])),
                               (3,23--3,46), { ArrowRange = (3,42--3,44) }),
                            SynValInfo
                              ([[SynArgInfo
                                   ([{ Attributes =
                                        [{ TypeName =
                                            SynLongIdent ([Foo], [], [None])
                                           ArgExpr = Const (Unit, (3,25--3,28))
                                           Target = None
                                           AppliesToGetterAndSetter = false
                                           Range = (3,25--3,28) };
                                         { TypeName =
                                            SynLongIdent ([Bar], [], [None])
                                           ArgExpr = Const (Unit, (3,30--3,33))
                                           Target = None
                                           AppliesToGetterAndSetter = false
                                           Range = (3,30--3,33) }]
                                       Range = (3,23--3,35) }], true, Some a)]],
                               SynArgInfo ([], false, None)), false, false,
                            PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, None, (3,4--3,46),
                            { LeadingKeyword =
                               AbstractMember ((3,4--3,12), (3,13--3,19))
                              InlineKeyword = None
                              WithKeyword = None
                              EqualsRange = None }),
                         { IsInstance = true
                           IsDispatchSlot = true
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = Member }, (3,4--3,46),
                         { GetSetKeywords = None })], (3,4--3,46)), [], None,
                  (2,5--3,46), { LeadingKeyword = Type (2,0--2,4)
                                 EqualsRange = Some (2,7--2,8)
                                 WithKeyword = None })], (2,0--3,46))],
          PreXmlDocEmpty, [], None, (2,0--4,0), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
