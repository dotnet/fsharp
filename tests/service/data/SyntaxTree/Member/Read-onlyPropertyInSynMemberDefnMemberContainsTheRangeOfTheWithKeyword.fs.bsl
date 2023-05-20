ImplFile
  (ParsedImplFileInput
     ("/root/Member/Read-onlyPropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword.fs",
      false,
      QualifiedNameOfFile
        Read-onlyPropertyInSynMemberDefnMemberContainsTheRangeOfTheWithKeyword,
      [], [],
      [SynModuleOrNamespace
         ([Foo], false, DeclaredNamespace,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Foo],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,8)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [], SimplePats ([], [], (3,8--3,10)), None,
                         PreXmlDoc ((3,8), FSharp.Compiler.Xml.XmlDocCollector),
                         (3,5--3,8), { AsKeyword = None });
                      GetSetMember
                        (Some
                           (SynBinding
                              (None, Normal, false, false, [],
                               PreXmlMerge
  (PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector), PreXmlDocEmpty),
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
                                    ([this; MyReadProperty], [(5,15--5,16)],
                                     [None; None]), Some get, None,
                                  Pats
                                    [Paren
                                       (Const (Unit, (5,40--5,42)), (5,40--5,42))],
                                  None, (5,36--5,42)), None,
                               Ident myInternalValue, (5,36--5,42),
                               NoneAtInvisible,
                               { LeadingKeyword = Member (5,4--5,10)
                                 InlineKeyword = None
                                 EqualsRange = Some (5,43--5,44) })), None,
                         (5,4--5,60), { InlineKeyword = None
                                        WithKeyword = (5,31--5,35)
                                        GetKeyword = Some (5,36--5,39)
                                        AndKeyword = None
                                        SetKeyword = None })], (5,4--5,60)), [],
                  Some
                    (ImplicitCtor
                       (None, [], SimplePats ([], [], (3,8--3,10)), None,
                        PreXmlDoc ((3,8), FSharp.Compiler.Xml.XmlDocCollector),
                        (3,5--3,8), { AsKeyword = None })), (3,5--5,60),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,11--3,12)
                    WithKeyword = None })], (3,0--5,60))], PreXmlDocEmpty, [],
          None, (1,0--5,60), { LeadingKeyword = Namespace (1,0--1,9) })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [LineComment (4,4--4,28)] }, set []))
