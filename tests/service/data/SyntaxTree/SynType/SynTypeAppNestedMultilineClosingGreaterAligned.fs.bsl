ImplFile
  (ParsedImplFileInput
     ("/root/SynType/SynTypeAppNestedMultilineClosingGreaterAligned.fs", false,
      QualifiedNameOfFile SynTypeAppNestedMultilineClosingGreaterAligned, [],
      [SynModuleOrNamespace
         ([SynTypeAppNestedMultilineClosingGreaterAligned], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([],
                     Some
                       (PostfixList
                          ([SynTyparDecl
                              ([], SynTypar (T, None, false), [],
                               { AmpersandRanges = [] })], [], (1,8--1,12))), [],
                     [Foo],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     true, None, (1,5--1,8)),
                  ObjectModel (Class, [], (1,15--1,24)), [], None, (1,5--1,24),
                  { LeadingKeyword = Type (1,0--1,4)
                    EqualsRange = Some (1,13--1,14)
                    WithKeyword = None })], (1,0--1,24));
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Terminal],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,13)),
                  ObjectModel
                    (Unspecified,
                     [AbstractSlot
                        (SynValSig
                           ([], SynIdent (onKey, None),
                            SynValTyparDecls (None, true),
                            App
                              (LongIdent (SynLongIdent ([Foo], [], [None])),
                               Some (5,11--5,12),
                               [App
                                  (LongIdent (SynLongIdent ([Foo], [], [None])),
                                   Some (6,15--6,16),
                                   [LongIdent (SynLongIdent ([int], [], [None]))],
                                   [], Some (8,12--8,13), false, (6,12--8,13))],
                               [], Some (9,8--9,9), false, (5,8--9,9)),
                            SynValInfo ([], SynArgInfo ([], false, None)), false,
                            false,
                            PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                            Single None, None, (4,4--9,23),
                            { LeadingKeyword = Abstract (4,4--4,12)
                              InlineKeyword = None
                              WithKeyword = Some (9,10--9,14)
                              EqualsRange = None }),
                         { IsInstance = true
                           IsDispatchSlot = true
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = PropertyGetSet }, (4,4--9,23),
                         { GetSetKeywords =
                            Some (GetSet ((9,15--9,18), (9,20--9,23))) })],
                     (4,4--9,23)), [], None, (3,5--9,23),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,14--3,15)
                    WithKeyword = None })], (3,0--9,23))], PreXmlDocEmpty, [],
          None, (1,0--10,0), { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
