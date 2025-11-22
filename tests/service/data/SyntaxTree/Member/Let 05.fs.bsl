ImplFile
  (ParsedImplFileInput
     ("/root/Member/Let 05.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Foo],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,8)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [], Const (Unit, (3,8--3,10)), None,
                         PreXmlDoc ((3,8), FSharp.Compiler.Xml.XmlDocCollector),
                         (3,5--3,8), { AsKeyword = None });
                      LetBindings
                        ([SynBinding
                            (None, Normal, false, false, [],
                             PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                             SynValData
                               (None,
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                None),
                             Named (SynIdent (a, None), false, None, (4,8--4,9)),
                             None, Const (Int32 0, (4,12--4,13)), (4,8--4,9),
                             Yes (4,4--4,13),
                             { LeadingKeyword = Let (4,4--4,7)
                               InlineKeyword = None
                               EqualsRange = Some (4,10--4,11) })], false, false,
                         (4,4--4,16), { InKeyword = Some (4,14--4,16) });
                      LetBindings
                        ([SynBinding
                            (None, Normal, false, false, [],
                             PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                             SynValData
                               (None,
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                None),
                             Named (SynIdent (y, None), false, None, (5,8--5,9)),
                             None, Const (Int32 1, (5,12--5,13)), (5,8--5,9),
                             Yes (5,4--5,13),
                             { LeadingKeyword = Let (5,4--5,7)
                               InlineKeyword = None
                               EqualsRange = Some (5,10--5,11) })], false, false,
                         (5,4--5,16), { InKeyword = Some (5,14--5,16) });
                      LetBindings
                        ([SynBinding
                            (None, Normal, false, false, [],
                             PreXmlDoc ((6,4), FSharp.Compiler.Xml.XmlDocCollector),
                             SynValData
                               (None,
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                None),
                             Named (SynIdent (x, None), false, None, (6,8--6,9)),
                             None, Const (Int32 0, (6,12--6,13)), (6,8--6,9),
                             Yes (6,4--6,13),
                             { LeadingKeyword = Let (6,4--6,7)
                               InlineKeyword = None
                               EqualsRange = Some (6,10--6,11) })], false, false,
                         (6,4--6,16), { InKeyword = Some (6,14--6,16) });
                      Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((7,4), FSharp.Compiler.Xml.XmlDocCollector),
                            SynValData
                              (Some { IsInstance = true
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = false
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = Member },
                               SynValInfo
                                 ([[SynArgInfo ([], false, None)]; []],
                                  SynArgInfo ([], false, None)), None),
                            LongIdent
                              (SynLongIdent
                                 ([this; A], [(7,15--7,16)], [None; None]), None,
                               None, Pats [], None, (7,11--7,17)), None, Ident a,
                            (7,11--7,17), NoneAtInvisible,
                            { LeadingKeyword = Member (7,4--7,10)
                              InlineKeyword = None
                              EqualsRange = Some (7,18--7,19) }), (7,4--7,21))],
                     (4,4--7,21)), [],
                  Some
                    (ImplicitCtor
                       (None, [], Const (Unit, (3,8--3,10)), None,
                        PreXmlDoc ((3,8), FSharp.Compiler.Xml.XmlDocCollector),
                        (3,5--3,8), { AsKeyword = None })), (3,5--7,21),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,11--3,12)
                    WithKeyword = None })], (3,0--7,21))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,21), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
