ImplFile
  (ParsedImplFileInput
     ("/root/SynTyparDecl/Constraint intersection 01.fs", false,
      QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([],
                     Some
                       (PostfixList
                          ([SynTyparDecl
                              ([], SynTypar (t, None, false),
                               [HashConstraint
                                  (App
                                     (LongIdent
                                        (SynLongIdent ([seq], [], [None])),
                                      Some (3,16--3,17),
                                      [LongIdent
                                         (SynLongIdent ([int], [], [None]))], [],
                                      Some (3,20--3,21), false, (3,13--3,21)),
                                   (3,12--3,21));
                                HashConstraint
                                  (LongIdent
                                     (SynLongIdent ([IDisposable], [], [None])),
                                   (3,24--3,36));
                                HashConstraint
                                  (LongIdent (SynLongIdent ([I], [], [None])),
                                   (3,39--3,41))],
                               { AmpersandRanges =
                                  [(3,10--3,11); (3,22--3,23); (3,37--3,38)] });
                            SynTyparDecl
                              ([], SynTypar (y, None, false),
                               [HashConstraint
                                  (App
                                     (LongIdent
                                        (SynLongIdent ([seq], [], [None])),
                                      Some (3,52--3,53),
                                      [Var
                                         (SynTypar (t, None, false),
                                          (3,53--3,55))], [], Some (3,55--3,56),
                                      false, (3,49--3,56)), (3,48--3,56))],
                               { AmpersandRanges = [(3,46--3,47)] })], [],
                           (3,6--3,57))), [], [C],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     true, None, (3,5--3,6)),
                  ObjectModel
                    (Unspecified,
                     [Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                            SynValData
                              (Some { IsInstance = true
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = false
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = Member },
                               SynValInfo
                                 ([[SynArgInfo ([], false, None)];
                                   [SynArgInfo ([], false, Some x);
                                    SynArgInfo ([], false, Some y)]],
                                  SynArgInfo ([], false, None)), None, None),
                            LongIdent
                              (SynLongIdent
                                 ([_; G], [(4,12--4,13)], [None; None]), None,
                               None,
                               Pats
                                 [Paren
                                    (Tuple
                                       (false,
                                        [Typed
                                           (Named
                                              (SynIdent (x, None), false, None,
                                               (4,16--4,17)),
                                            Var
                                              (SynTypar (t, None, false),
                                               (4,19--4,21)), (4,16--4,21));
                                         Typed
                                           (Named
                                              (SynIdent (y, None), false, None,
                                               (4,23--4,24)),
                                            Var
                                              (SynTypar (y, None, false),
                                               (4,26--4,28)), (4,23--4,28))],
                                        [(4,21--4,22)], (4,16--4,28)),
                                     (4,15--4,29))], None, (4,11--4,29)), None,
                            Const (Unit, (4,32--4,34)), (4,11--4,29),
                            NoneAtInvisible,
                            { LeadingKeyword = Member (4,4--4,10)
                              InlineKeyword = None
                              EqualsRange = Some (4,30--4,31) }), (4,4--4,34))],
                     (4,4--4,34)), [], None, (3,5--4,34),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,58--3,59)
                    WithKeyword = None })], (3,0--4,34))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--4,34), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
