SigFile
  (ParsedSigFileInput
     ("/root/SynType/SynTypeTupleDoesIncludeLeadingParameterAttributes.fsi",
      QualifiedNameOfFile SynTypeTupleDoesIncludeLeadingParameterAttributes, [],
      [],
      [SynModuleOrNamespaceSig
         ([SynTypeTupleDoesIncludeLeadingParameterAttributes], false, AnonModule,
          [Types
             ([SynTypeDefnSig
                 (SynComponentInfo
                    ([], None, [], [T],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (2,5--2,6)),
                  ObjectModel
                    (Unspecified,
                     [Member
                        (SynValSig
                           ([], SynIdent (M, None),
                            SynValTyparDecls (None, true),
                            Fun
                              (Tuple
                                 (false,
                                  [Type
                                     (SignatureParameter
                                        ([{ Attributes =
                                             [{ TypeName =
                                                 SynLongIdent
                                                   ([SomeAttribute], [], [None])
                                                ArgExpr =
                                                 Const (Unit, (3,16--3,29))
                                                Target = None
                                                AppliesToGetterAndSetter = false
                                                Range = (3,16--3,29) }]
                                            Range = (3,14--3,31) }], false, None,
                                         LongIdent
                                           (SynLongIdent ([a], [], [None])),
                                         (3,14--3,33))); Star (3,34--3,35);
                                   Type
                                     (SignatureParameter
                                        ([{ Attributes =
                                             [{ TypeName =
                                                 SynLongIdent
                                                   ([OtherAttribute], [], [None])
                                                ArgExpr =
                                                 Const (Unit, (3,38--3,52))
                                                Target = None
                                                AppliesToGetterAndSetter = false
                                                Range = (3,38--3,52) }]
                                            Range = (3,36--3,54) }], false, None,
                                         LongIdent
                                           (SynLongIdent ([b], [], [None])),
                                         (3,36--3,56)))], (3,14--3,56)),
                               LongIdent (SynLongIdent ([int], [], [None])),
                               (3,14--3,63), { ArrowRange = (3,57--3,59) }),
                            SynValInfo
                              ([[SynArgInfo
                                   ([{ Attributes =
                                        [{ TypeName =
                                            SynLongIdent
                                              ([SomeAttribute], [], [None])
                                           ArgExpr = Const (Unit, (3,16--3,29))
                                           Target = None
                                           AppliesToGetterAndSetter = false
                                           Range = (3,16--3,29) }]
                                       Range = (3,14--3,31) }], false, None);
                                 SynArgInfo
                                   ([{ Attributes =
                                        [{ TypeName =
                                            SynLongIdent
                                              ([OtherAttribute], [], [None])
                                           ArgExpr = Const (Unit, (3,38--3,52))
                                           Target = None
                                           AppliesToGetterAndSetter = false
                                           Range = (3,38--3,52) }]
                                       Range = (3,36--3,54) }], false, None)]],
                               SynArgInfo ([], false, None)), false, false,
                            PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
                            None, None, (3,4--3,63),
                            { LeadingKeyword = Member (3,4--3,10)
                              InlineKeyword = None
                              WithKeyword = None
                              EqualsRange = None }),
                         { IsInstance = true
                           IsDispatchSlot = false
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = Member }, (3,4--3,63),
                         { GetSetKeywords = None })], (3,4--3,63)), [],
                  (2,5--3,63), { LeadingKeyword = Type (2,0--2,4)
                                 EqualsRange = Some (2,7--2,8)
                                 WithKeyword = None })], (2,0--3,63))],
          PreXmlDocEmpty, [], None, (2,0--4,0), { LeadingKeyword = None })],
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
