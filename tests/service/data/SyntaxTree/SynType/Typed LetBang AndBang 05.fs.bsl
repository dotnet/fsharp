ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Typed LetBang AndBang 05.fs", false,
      QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (App
                (NonAtomic, false, Ident async,
                 ComputationExpr
                   (false,
                    LetOrUse
                      (false, false, true, true,
                       [SynBinding
                          (None, Normal, false, false, [], PreXmlDocEmpty,
                           SynValData
                             (None,
                              SynValInfo ([], SynArgInfo ([], false, None)),
                              None),
                           Paren
                             (Typed
                                (Record
                                   ([(([], Name), Some (4,17--4,18),
                                      Named
                                        (SynIdent (name, None), false, None,
                                         (4,19--4,23)));
                                     (([], Age), Some (4,29--4,30),
                                      Named
                                        (SynIdent (age, None), false, None,
                                         (4,31--4,34)))], (4,10--4,36)),
                                 LongIdent (SynLongIdent ([Person], [], [None])),
                                 (4,10--4,44)), (4,9--4,45)), None,
                           App
                             (Atomic, false, Ident asyncPerson,
                              Const (Unit, (4,59--4,61)), (4,48--4,61)),
                           (4,4--6,15), Yes (4,4--4,61),
                           { LeadingKeyword = Let (4,4--4,8)
                             InlineKeyword = None
                             EqualsRange = Some (4,46--4,47) });
                        SynBinding
                          (None, Normal, false, false, [], PreXmlDocEmpty,
                           SynValData
                             (None,
                              SynValInfo ([], SynArgInfo ([], false, None)),
                              None),
                           Typed
                             (Record
                                ([(([], Id), Some (5,14--5,15),
                                   Named
                                     (SynIdent (id, None), false, None,
                                      (5,16--5,18)))], (5,9--5,20)),
                              LongIdent (SynLongIdent ([User], [], [None])),
                              (5,9--5,26)), None,
                           App
                             (Atomic, false, Ident asyncUser,
                              Const (Unit, (5,38--5,40)), (5,29--5,40)),
                           (5,4--5,40), Yes (5,4--5,40),
                           { LeadingKeyword = And (5,4--5,8)
                             InlineKeyword = None
                             EqualsRange = Some (5,27--5,28) })],
                       YieldOrReturn
                         ((false, true), Ident name, (6,4--6,15),
                          { YieldOrReturnKeyword = (6,4--6,10) }), (4,4--6,15),
                       { LetOrUseKeyword = (4,4--4,8)
                         InKeyword = None
                         EqualsRange = Some (4,46--4,47) }), (3,6--7,1)),
                 (3,0--7,1)), (3,0--7,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
