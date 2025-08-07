ImplFile
  (ParsedImplFileInput
     ("/root/OpenDeclaration/InExp_type_member3.fs", false,
      QualifiedNameOfFile InExp_type_member3, [],
      [SynModuleOrNamespace
         ([InExp_type_member3], false, AnonModule,
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
                      GetSetMember
                        (Some
                           (SynBinding
                              (None, Normal, false, false, [],
                               PreXmlMerge
  (PreXmlDoc ((2,4), FSharp.Compiler.Xml.XmlDocCollector), PreXmlDocEmpty),
                               SynValData
                                 (Some
                                    { IsInstance = true
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = false
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = PropertyGet },
                                  SynValInfo
                                    ([[SynArgInfo ([], false, None)]; []],
                                     SynArgInfo ([], false, None)), None),
                               LongIdent
                                 (SynLongIdent
                                    ([_; MinValue], [(2,12--2,13)], [None; None]),
                                  Some get, None,
                                  Pats
                                    [Paren
                                       (Const (Unit, (2,30--2,32)), (2,30--2,32))],
                                  None, (2,27--2,32)), None,
                               Open
                                 (Type
                                    (LongIdent
                                       (SynLongIdent
                                          ([System; Int32], [(2,51--2,52)],
                                           [None; None])), (2,45--2,57)),
                                  (2,35--2,57), (2,35--2,67), Ident MinValue),
                               (2,27--2,32), NoneAtInvisible,
                               { LeadingKeyword = Member (2,4--2,10)
                                 InlineKeyword = None
                                 EqualsRange = Some (2,33--2,34) })),
                         Some
                           (SynBinding
                              (None, Normal, false, false, [],
                               PreXmlMerge
  (PreXmlDoc ((2,4), FSharp.Compiler.Xml.XmlDocCollector), PreXmlDocEmpty),
                               SynValData
                                 (Some
                                    { IsInstance = true
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = false
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = PropertySet },
                                  SynValInfo
                                    ([[SynArgInfo ([], false, None)];
                                      [SynArgInfo ([], false, None)]],
                                     SynArgInfo ([], false, None)), None),
                               LongIdent
                                 (SynLongIdent
                                    ([_; MinValue], [(2,12--2,13)], [None; None]),
                                  Some set, None,
                                  Pats
                                    [Paren
                                       (Typed
                                          (Wild (2,77--2,78),
                                           LongIdent
                                             (SynLongIdent ([int], [], [None])),
                                           (2,77--2,83)), (2,76--2,84))], None,
                                  (2,72--2,84)), None,
                               Const (Unit, (2,87--2,89)), (2,72--2,84),
                               NoneAtInvisible,
                               { LeadingKeyword = Member (2,4--2,10)
                                 InlineKeyword = None
                                 EqualsRange = Some (2,85--2,86) })),
                         (2,4--2,89), { InlineKeyword = None
                                        WithKeyword = (2,22--2,26)
                                        GetKeyword = Some (2,27--2,30)
                                        AndKeyword = Some (2,68--2,71)
                                        SetKeyword = Some (2,72--2,75) })],
                     (2,4--2,89)), [],
                  Some
                    (ImplicitCtor
                       (None, [], Const (Unit, (1,8--1,10)), None,
                        PreXmlDoc ((1,8), FSharp.Compiler.Xml.XmlDocCollector),
                        (1,5--1,8), { AsKeyword = None })), (1,5--2,89),
                  { LeadingKeyword = Type (1,0--1,4)
                    EqualsRange = Some (1,11--1,12)
                    WithKeyword = None })], (1,0--2,89))], PreXmlDocEmpty, [],
          None, (1,0--3,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
