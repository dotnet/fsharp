ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Constraint intersection 02.fs", false,
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
                                (Some (SynTypar (t, None, false)),
                                 [HashConstraint
                                    (LongIdent (SynLongIdent ([I], [], [None])),
                                     (3,15--3,17));
                                  HashConstraint
                                    (LongIdent
                                       (SynLongIdent ([IDisposable], [], [None])),
                                     (3,20--3,32));
                                  HashConstraint
                                    (App
                                       (LongIdent
                                          (SynLongIdent ([seq], [], [None])),
                                        Some (3,39--3,40),
                                        [LongIdent
                                           (SynLongIdent ([int], [], [None]))],
                                        [], Some (3,43--3,44), false,
                                        (3,36--3,44)), (3,35--3,44));
                                  HashConstraint
                                    (LongIdent (SynLongIdent ([I2], [], [None])),
                                     (3,47--3,50))], (3,10--3,50),
                                 { AmpersandRanges =
                                    [(3,13--3,14); (3,18--3,19); (3,33--3,34);
                                     (3,45--3,46)] }), (3,7--3,50)), (3,6--3,51))],
                     None, (3,4--3,51)), None, Const (Unit, (3,54--3,56)),
                  (3,4--3,51), NoneAtLet, { LeadingKeyword = Let (3,0--3,3)
                                            InlineKeyword = None
                                            EqualsRange = Some (3,52--3,53) })],
              (3,0--3,56))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,56), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
