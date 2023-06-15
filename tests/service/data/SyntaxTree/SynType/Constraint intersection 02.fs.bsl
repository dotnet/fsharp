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
                                    (LongIdent
                                       (SynLongIdent ([IDisposable], [], [None])),
                                     (3,20--3,32));
                                  HashConstraint
                                    (LongIdent (SynLongIdent ([I], [], [None])),
                                     (3,15--3,17))], (3,10--3,32)), (3,7--3,32)),
                           (3,6--3,33))], None, (3,4--3,33)), None,
                  Const (Unit, (3,36--3,38)), (3,4--3,33), NoneAtLet,
                  { LeadingKeyword = Let (3,0--3,3)
                    InlineKeyword = None
                    EqualsRange = Some (3,34--3,35) })], (3,0--3,38))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--3,38), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
