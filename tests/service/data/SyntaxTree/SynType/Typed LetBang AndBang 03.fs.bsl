ImplFile
  (ParsedImplFileInput
     ("/root/SynType/Typed LetBang AndBang 03.fs", false,
      QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Expr
             (App
                (NonAtomic, false, Ident async,
                 ComputationExpr
                   (false,
                    LetOrUseBang
                      (Yes (4,4--4,61), false, true,
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
                             (4,10--4,44)), (4,9--4,45)),
                       App
                         (Atomic, false, Ident asyncPerson,
                          Const (Unit, (4,59--4,61)), (4,48--4,61)),
                       [SynExprAndBang
                          (Yes (5,4--5,42), false, true,
                           Paren
                             (Typed
                                (Record
                                   ([(([], Id), Some (5,15--5,16),
                                      Named
                                        (SynIdent (id, None), false, None,
                                         (5,17--5,19)))], (5,10--5,21)),
                                 LongIdent (SynLongIdent ([User], [], [None])),
                                 (5,10--5,27)), (5,9--5,28)),
                           App
                             (Atomic, false, Ident asyncUser,
                              Const (Unit, (5,40--5,42)), (5,31--5,42)),
                           (5,4--5,42), { AndBangKeyword = (5,4--5,8)
                                          EqualsRange = (5,29--5,30)
                                          InKeyword = None })],
                       YieldOrReturn
                         ((false, true), Ident name, (6,4--6,15),
                          { YieldOrReturnKeyword = (6,4--6,10) }), (4,4--6,15),
                       { LetOrUseBangKeyword = (4,4--4,8)
                         EqualsRange = Some (4,46--4,47) }), (3,6--7,1)),
                 (3,0--7,1)), (3,0--7,1))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,1), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
