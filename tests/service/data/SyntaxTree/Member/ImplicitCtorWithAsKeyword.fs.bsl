ImplFile
  (ParsedImplFileInput
     ("/root/Member/ImplicitCtorWithAsKeyword.fs", false,
      QualifiedNameOfFile ImplicitCtorWithAsKeyword, [], [],
      [SynModuleOrNamespace
         ([ImplicitCtorWithAsKeyword], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [CompilerStateCache],
                     PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, Some (Internal (2,5--2,13)), (2,14--2,32)),
                  ObjectModel
                    (Class,
                     [ImplicitCtor
                        (None, [],
                         Paren
                           (Tuple
                              (false,
                               [Typed
                                  (Named
                                     (SynIdent (readAllBytes, None), false, None,
                                      (2,33--2,45)),
                                   Fun
                                     (LongIdent
                                        (SynLongIdent ([string], [], [None])),
                                      Array
                                        (1,
                                         LongIdent
                                           (SynLongIdent ([byte], [], [None])),
                                         (2,57--2,63)), (2,47--2,63),
                                      { ArrowRange = (2,54--2,56) }),
                                   (2,33--2,63));
                                Typed
                                  (Named
                                     (SynIdent (projectOptions, None), false,
                                      None, (2,65--2,79)),
                                   LongIdent
                                     (SynLongIdent
                                        ([FSharpProjectOptions], [], [None])),
                                   (2,65--2,101))], [(2,63--2,64)],
                               (2,33--2,101)), (2,32--2,102)), Some this,
                         PreXmlDoc ((2,32), FSharp.Compiler.Xml.XmlDocCollector),
                         (2,14--2,32), { AsKeyword = Some (4,4--4,6) })],
                     (8,4--8,13)), [],
                  Some
                    (ImplicitCtor
                       (None, [],
                        Paren
                          (Tuple
                             (false,
                              [Typed
                                 (Named
                                    (SynIdent (readAllBytes, None), false, None,
                                     (2,33--2,45)),
                                  Fun
                                    (LongIdent
                                       (SynLongIdent ([string], [], [None])),
                                     Array
                                       (1,
                                        LongIdent
                                          (SynLongIdent ([byte], [], [None])),
                                        (2,57--2,63)), (2,47--2,63),
                                     { ArrowRange = (2,54--2,56) }),
                                  (2,33--2,63));
                               Typed
                                 (Named
                                    (SynIdent (projectOptions, None), false,
                                     None, (2,65--2,79)),
                                  LongIdent
                                    (SynLongIdent
                                       ([FSharpProjectOptions], [], [None])),
                                  (2,65--2,101))], [(2,63--2,64)], (2,33--2,101)),
                           (2,32--2,102)), Some this,
                        PreXmlDoc ((2,32), FSharp.Compiler.Xml.XmlDocCollector),
                        (2,14--2,32), { AsKeyword = Some (4,4--4,6) })),
                  (2,5--8,13), { LeadingKeyword = Type (2,0--2,4)
                                 EqualsRange = Some (4,12--4,13)
                                 WithKeyword = None })], (2,0--8,13))],
          PreXmlDocEmpty, [], None, (2,0--9,0), { LeadingKeyword = None })],
      (true, true),
      { ConditionalDirectives = []
        CodeComments =
         [LineComment (3,0--3,23); LineComment (5,0--5,7);
          LineComment (6,0--6,8); LineComment (7,0--7,9)] }, set []))
