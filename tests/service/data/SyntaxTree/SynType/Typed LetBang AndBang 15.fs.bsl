ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Typed LetBang AndBang 15.fs", false,
      QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (App
                (NonAtomic, false, Ident async,
                 ComputationExpr
                   (false,
                    Tuple
                      (false,
                       [LetOrUse
                          (false, false, true, true,
                           [SynBinding
                              (None, Normal, false, false, [], PreXmlDocEmpty,
                               SynValData
                                 (None,
                                  SynValInfo ([], SynArgInfo ([], false, None)),
                                  None),
                               Typed
                                 (Paren
                                    (Tuple
                                       (false,
                                        [Named
                                           (SynIdent (x, None), false, None,
                                            (4,10--4,11));
                                         Named
                                           (SynIdent (y, None), false, None,
                                            (4,13--4,14))], [(4,11--4,12)],
                                        (4,10--4,14)), (4,9--4,15)),
                                  Tuple
                                    (false,
                                     [Type
                                        (LongIdent
                                           (SynLongIdent ([int], [], [None])));
                                      Star (4,21--4,22);
                                      Type
                                        (LongIdent
                                           (SynLongIdent ([int], [], [None])))],
                                     (4,17--4,26)), (4,9--4,26)), None,
                               App
                                 (Atomic, false, Ident asyncInt,
                                  Const (Unit, (4,37--4,39)), (4,29--4,39)),
                               (4,4--5,16), Yes (4,4--4,39),
                               { LeadingKeyword = Let (4,4--4,8)
                                 InlineKeyword = None
                                 EqualsRange = Some (4,27--4,28) })],
                           ImplicitZero (4,39--4,39), (4,4--5,16),
                           { LetOrUseKeyword = (4,4--4,8)
                             InKeyword = None
                             EqualsRange = Some (4,27--4,28) }); Ident y],
                       [(5,15--5,16)], (4,4--5,18)), (3,6--7,1)), (3,0--7,1)),
              (3,0--7,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(5,15)-(5,16) parse error Unexpected symbol ',' in expression. Expected '=' or other token.
