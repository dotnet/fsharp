ImplFile
  (ParsedImplFileInput
     ("/root/Type/Module Inside Delegate 01.fs", false,
      QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [MyDelegate],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,5--4,15)),
                  ObjectModel
                    (Delegate
                       (Fun
                          (Tuple
                             (false,
                              [Type
                                 (LongIdent (SynLongIdent ([int], [], [None])));
                               Star (4,34--4,35);
                               Type
                                 (LongIdent (SynLongIdent ([int], [], [None])))],
                              (4,30--4,39)),
                           LongIdent (SynLongIdent ([int], [], [None])),
                           (4,30--4,46), { ArrowRange = (4,40--4,42) }),
                        SynValInfo
                          ([[SynArgInfo ([], false, None);
                             SynArgInfo ([], false, None)]],
                           SynArgInfo ([], false, None))),
                     [AbstractSlot
                        (SynValSig
                           ([], SynIdent (Invoke, None),
                            SynValTyparDecls (None, true),
                            Fun
                              (Tuple
                                 (false,
                                  [Type
                                     (LongIdent
                                        (SynLongIdent ([int], [], [None])));
                                   Star (4,34--4,35);
                                   Type
                                     (LongIdent
                                        (SynLongIdent ([int], [], [None])))],
                                  (4,30--4,39)),
                               LongIdent (SynLongIdent ([int], [], [None])),
                               (4,30--4,46), { ArrowRange = (4,40--4,42) }),
                            SynValInfo
                              ([[SynArgInfo ([], false, None);
                                 SynArgInfo ([], false, None)]],
                               SynArgInfo ([], false, None)), false, false,
                            PreXmlDocEmpty, Single None, None, (4,18--4,46),
                            { LeadingKeyword = Synthetic
                              InlineKeyword = None
                              WithKeyword = None
                              EqualsRange = None }),
                         { IsInstance = true
                           IsDispatchSlot = true
                           IsOverrideOrExplicitImpl = false
                           IsFinal = false
                           GetterOrSetterIsCompilerGenerated = false
                           MemberKind = Member }, (4,18--4,46),
                         { GetSetKeywords = None })], (4,18--4,46)), [], None,
                  (4,5--4,46), { LeadingKeyword = Type (4,0--4,4)
                                 EqualsRange = Some (4,16--4,17)
                                 WithKeyword = None })], (4,0--4,46));
           NestedModule
             (SynComponentInfo
                ([], None, [], [InvalidModule],
                 PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (5,4--5,24)), false,
              [Let
                 (false,
                  [SynBinding
                     (None, Normal, false, false, [],
                      PreXmlDoc ((6,8), FSharp.Compiler.Xml.XmlDocCollector),
                      SynValData
                        (None, SynValInfo ([], SynArgInfo ([], false, None)),
                         None),
                      Named (SynIdent (x, None), false, None, (6,12--6,13)),
                      None, Const (Int32 1, (6,16--6,17)), (6,12--6,13),
                      Yes (6,8--6,17), { LeadingKeyword = Let (6,8--6,11)
                                         InlineKeyword = None
                                         EqualsRange = Some (6,14--6,15) })],
                  (6,8--6,17), { InKeyword = None })], false, (5,4--6,17),
              { ModuleKeyword = Some (5,4--5,10)
                EqualsRange = Some (5,25--5,26) })],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--6,17), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,46)] }, set []))

(5,4)-(5,10) parse error Modules cannot be nested inside types. Define modules at module or namespace level.
