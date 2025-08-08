ImplFile
  (ParsedImplFileInput
     ("/root/OpenDeclaration/InType_Extension2.fs", false,
      QualifiedNameOfFile InType_Extension2, [],
      [SynModuleOrNamespace
         ([InType_Extension2], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [System; Int32],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (1,5--1,17)),
                  ObjectModel (Augmentation (1,18--1,22), [], (1,5--3,25)),
                  [Member
                     (SynBinding
                        (None, Normal, false, false, [],
                         PreXmlDoc ((2,4), FSharp.Compiler.Xml.XmlDocCollector),
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
                              ([this; Abs111], [(2,15--2,16)], [None; None]),
                            None, None, Pats [], None, (2,11--2,22)), None,
                         App
                           (Atomic, false, Ident Abs,
                            Paren
                              (Ident this, (2,28--2,29), Some (2,33--2,34),
                               (2,28--2,34)), (2,25--2,34)), (2,11--2,22),
                         NoneAtInvisible, { LeadingKeyword = Member (2,4--2,10)
                                            InlineKeyword = None
                                            EqualsRange = Some (2,23--2,24) }),
                      (2,4--2,34));
                   Open
                     (Type
                        (LongIdent
                           (SynLongIdent
                              ([System; Math], [(3,20--3,21)], [None; None])),
                         (3,14--3,25)), (3,4--3,25))], None, (1,5--3,25),
                  { LeadingKeyword = Type (1,0--1,4)
                    EqualsRange = None
                    WithKeyword = None })], (1,0--3,25))], PreXmlDocEmpty, [],
          None, (1,0--3,25), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
