ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Typed LetBang AndBang 04.fs", false,
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
                           Typed
                             (Record
                                ([(([], Name), Some (4,16--4,17),
                                   Named
                                     (SynIdent (name, None), false, None,
                                      (4,18--4,22)));
                                  (([], Age), Some (4,28--4,29),
                                   Named
                                     (SynIdent (age, None), false, None,
                                      (4,30--4,33)))], (4,9--4,35)),
                              LongIdent (SynLongIdent ([Person], [], [None])),
                              (4,9--4,43)), None,
                           App
                             (Atomic, false, Ident asyncPerson,
                              Const (Unit, (4,57--4,59)), (4,46--4,59)),
                           (4,4--6,15), Yes (4,4--4,59),
                           { LeadingKeyword = Let (4,4--4,8)
                             InlineKeyword = None
                             EqualsRange = Some (4,44--4,45) });
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
                         EqualsRange = Some (4,44--4,45) }), (3,6--7,1)),
                 (3,0--7,1)), (3,0--7,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
