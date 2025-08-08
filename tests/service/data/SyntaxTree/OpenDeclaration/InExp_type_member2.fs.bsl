ImplFile
  (ParsedImplFileInput
     ("/root/OpenDeclaration/InExp_type_member2.fs", false,
      QualifiedNameOfFile InExp_type_member2, [],
      [SynModuleOrNamespace
         ([InExp_type_member2], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Foo],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (1,5--1,8)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [], Const (Unit, (1,8--1,10)), None,
                         PreXmlDoc ((1,8), FSharp.Compiler.Xml.XmlDocCollector),
                         (1,5--1,8), { AsKeyword = None });
                      AutoProperty
                        ([], false, MinValue,
                         Some (LongIdent (SynLongIdent ([int], [], [None]))),
                         PropertyGetSet,
                         { IsInstance = true
                           IsDispatchSlot = false
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = Member },
                         { IsInstance = true
                           IsDispatchSlot = false
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = PropertySet },
                         PreXmlDoc ((2,4), FSharp.Compiler.Xml.XmlDocCollector),
                         GetSet (None, None, None),
                         Paren
                           (Open
                              (Type
                                 (LongIdent
                                    (SynLongIdent
                                       ([System; Int32], [(2,48--2,49)],
                                        [None; None])), (2,42--2,54)),
                               (2,32--2,54), (2,32--2,64), Ident MinValue),
                            (2,31--2,32), Some (2,64--2,65), (2,31--2,65)),
                         (2,4--2,79),
                         { LeadingKeyword =
                            MemberVal ((2,4--2,10), (2,11--2,14))
                           WithKeyword = Some (2,66--2,70)
                           EqualsRange = Some (2,29--2,30)
                           GetSetKeywords =
                            Some (GetSet ((2,71--2,74), (2,76--2,79))) })],
                     (2,4--2,79)), [],
                  Some
                    (ImplicitCtor
                       (None, [], Const (Unit, (1,8--1,10)), None,
                        PreXmlDoc ((1,8), FSharp.Compiler.Xml.XmlDocCollector),
                        (1,5--1,8), { AsKeyword = None })), (1,5--2,79),
                  { LeadingKeyword = Type (1,0--1,4)
                    EqualsRange = Some (1,11--1,12)
                    WithKeyword = None })], (1,0--2,79))], PreXmlDocEmpty, [],
          None, (1,0--3,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
