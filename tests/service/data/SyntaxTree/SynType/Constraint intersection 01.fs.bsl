ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Constraint intersection 01.fs", false,
      QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None,
                     SynValInfo
                       ([[SynArgInfo ([], false, Some f)]],
                        SynArgInfo ([], false, None)), None),
                  LongIdent
                    (SynLongIdent ([y], [], [None]), None, None,
                     Pats
                       [Paren
                          (Typed
                             (Named
                                (SynIdent (f, None), false, None, (3,7--3,8)),
                              Intersection
                                (None,
                                 [HashConstraint
                                    (LongIdent (SynLongIdent ([I], [], [None])),
                                     (3,10--3,12));
                                  HashConstraint
                                    (App
                                       (LongIdent
                                          (SynLongIdent ([Task], [], [None])),
                                        Some (3,20--3,21),
                                        [LongIdent
                                           (SynLongIdent ([int], [], [None]))],
                                        [], Some (3,24--3,25), false,
                                        (3,16--3,25)), (3,15--3,25));
                                  HashConstraint
                                    (App
                                       (LongIdent
                                          (SynLongIdent ([seq], [], [None])),
                                        Some (3,32--3,33),
                                        [LongIdent
                                           (SynLongIdent ([string], [], [None]))],
                                        [], Some (3,39--3,40), false,
                                        (3,29--3,40)), (3,28--3,40))],
                                 (3,10--3,40),
                                 { AmpersandRanges =
                                    [(3,13--3,14); (3,26--3,27)] }), (3,7--3,40)),
                           (3,6--3,41))], None, (3,4--3,41)), None,
                  Const (Unit, (3,44--3,46)), (3,4--3,41), NoneAtLet,
                  { LeadingKeyword = Let (3,0--3,3)
                    InlineKeyword = None
                    EqualsRange = Some (3,42--3,43) })], (3,0--3,46))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,46), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
