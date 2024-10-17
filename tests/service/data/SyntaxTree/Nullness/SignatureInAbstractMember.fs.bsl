ImplFile
  (ParsedImplFileInput
     ("/root/Nullness/SignatureInAbstractMember.fs", false,
      QualifiedNameOfFile SignatureInAbstractMember, [], [],
      [SynModuleOrNamespace
         ([SignatureInAbstractMember], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [MyClassBase1],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (1,5--1,17)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [], Const (Unit, (1,17--1,19)), None,
                         PreXmlDoc ((1,17), FSharp.Compiler.Xml.XmlDocCollector),
                         (1,5--1,17), { AsKeyword = None });
                      AbstractSlot
                        (SynValSig
                           ([], SynIdent (function1, None),
                            SynValTyparDecls (None, true),
                            Fun
                              (WithNull
                                 (LongIdent
                                    (SynLongIdent ([string], [], [None])), false,
                                  (2,31--2,44), { BarRange = (2,38--2,39) }),
                               WithNull
                                 (LongIdent
                                    (SynLongIdent ([string], [], [None])), false,
                                  (2,48--2,61), { BarRange = (2,55--2,56) }),
                               (2,31--2,61), { ArrowRange = (2,45--2,47) }),
                            SynValInfo
                              ([[SynArgInfo ([], false, None)]],
                               SynArgInfo ([], false, None)), false, false,
                            PreXmlDoc ((2,3), FSharp.Compiler.Xml.XmlDocCollector),
                            Single None, None, (2,3--2,61),
                            { LeadingKeyword =
                               AbstractMember ((2,3--2,11), (2,12--2,18))
                              InlineKeyword = None
                              WithKeyword = None
                              EqualsRange = None }),
                         { IsInstance = true
                           IsDispatchSlot = true
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = Member }, (2,3--2,61),
                         { GetSetKeywords = None })], (2,3--2,61)), [],
                  Some
                    (ImplicitCtor
                       (None, [], Const (Unit, (1,17--1,19)), None,
                        PreXmlDoc ((1,17), FSharp.Compiler.Xml.XmlDocCollector),
                        (1,5--1,17), { AsKeyword = None })), (1,5--2,61),
                  { LeadingKeyword = Type (1,0--1,4)
                    EqualsRange = Some (1,20--1,21)
                    WithKeyword = None })], (1,0--2,61))], PreXmlDocEmpty, [],
          None, (1,0--3,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
