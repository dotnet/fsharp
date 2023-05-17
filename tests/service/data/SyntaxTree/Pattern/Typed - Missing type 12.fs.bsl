ImplFile
  (ParsedImplFileInput
     ("/root/Pattern/Typed - Missing type 12.fs", false,
      QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (Do
                (LetOrUse
                   (false, false,
                    [SynBinding
                       (None, Normal, false, false, [],
                        PreXmlDoc ((4,4), FSharp.Compiler.Xml.XmlDocCollector),
                        SynValData
                          (None,
                           SynValInfo
                             ([[SynArgInfo ([], false, Some i);
                                SynArgInfo ([], false, Some j)]],
                              SynArgInfo ([], false, None)), None),
                        LongIdent
                          (SynLongIdent ([f], [], [None]), None, None,
                           Pats
                             [Paren
                                (Tuple
                                   (false,
                                    [Typed
                                       (Named
                                          (SynIdent (i, None), false, None,
                                           (4,11--4,12)),
                                        FromParseError (4,13--4,13),
                                        (4,11--4,13));
                                     Named
                                       (SynIdent (j, None), false, None,
                                        (4,15--4,16))], [(4,13--4,14)],
                                    (4,11--4,16)), (4,10--4,17))], None,
                           (4,8--4,17)), None, Const (Int32 1, (4,20--4,21)),
                        (4,8--4,17), NoneAtLet,
                        { LeadingKeyword = Let (4,4--4,7)
                          InlineKeyword = None
                          EqualsRange = Some (4,18--4,19) })],
                    Const (Unit, (6,4--6,6)), (4,4--6,6), { InKeyword = None }),
                 (3,0--6,6)), (3,0--6,6))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,6), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(4,13)-(4,14) parse error Unexpected symbol ',' in pattern
