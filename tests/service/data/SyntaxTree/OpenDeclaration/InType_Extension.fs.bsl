ImplFile
  (ParsedImplFileInput
     ("/root/OpenDeclaration/InType_Extension.fs", false,
      QualifiedNameOfFile InType_Extension, [],
      [SynModuleOrNamespace
         ([InType_Extension], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [System; Int32],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (1,5--1,17)),
                  ObjectModel (Augmentation (1,18--1,22), [], (1,5--3,34)),
                  [Open
                     (Type
                        (LongIdent
                           (SynLongIdent
                              ([System; Math], [(2,20--2,21)], [None; None])),
                         (2,14--2,25)), (2,4--2,25));
                   Member
                     (SynBinding
                        (None, Normal, false, false, [],
                         PreXmlDoc ((3,4), FSharp.Compiler.Xml.XmlDocCollector),
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
                              ([this; Abs111], [(3,15--3,16)], [None; None]),
                            None, None, Pats [], None, (3,11--3,22)), None,
                         App
                           (Atomic, false, Ident Abs,
                            Paren
                              (Ident this, (3,28--3,29), Some (3,33--3,34),
                               (3,28--3,34)), (3,25--3,34)), (3,11--3,22),
                         NoneAtInvisible, { LeadingKeyword = Member (3,4--3,10)
                                            InlineKeyword = None
                                            EqualsRange = Some (3,23--3,24) }),
                      (3,4--3,34))], None, (1,5--3,34),
                  { LeadingKeyword = Type (1,0--1,4)
                    EqualsRange = None
                    WithKeyword = None })], (1,0--3,34))], PreXmlDocEmpty, [],
          None, (1,0--3,34), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
