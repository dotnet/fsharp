ImplFile
  (ParsedImplFileInput
     ("/root/OpenDeclaration/InExp_fun.fs", false, QualifiedNameOfFile InExp_fun,
      [],
      [SynModuleOrNamespace
         ([InExp_fun], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None,
                     SynValInfo
                       ([[SynArgInfo ([], false, Some x)]],
                        SynArgInfo ([], false, None)), None),
                  Named (SynIdent (fun1, None), false, None, (1,4--1,8)), None,
                  Lambda
                    (false, false,
                     SimplePats
                       ([Id (x, None, false, false, false, (1,15--1,16))], [],
                        (1,15--1,16)),
                     Open
                       (ModuleOrNamespace
                          (SynLongIdent ([System], [], [None]), (1,25--1,31)),
                        (1,20--1,31), (1,20--1,38),
                        App
                          (NonAtomic, false,
                           App
                             (NonAtomic, true,
                              LongIdent
                                (false,
                                 SynLongIdent
                                   ([op_Addition], [],
                                    [Some (OriginalNotation "+")]), None,
                                 (1,35--1,36)), Ident x, (1,33--1,36)),
                           Const (Int32 1, (1,37--1,38)), (1,33--1,38))),
                     Some
                       ([Named (SynIdent (x, None), false, None, (1,15--1,16))],
                        Open
                          (ModuleOrNamespace
                             (SynLongIdent ([System], [], [None]), (1,25--1,31)),
                           (1,20--1,31), (1,20--1,38),
                           App
                             (NonAtomic, false,
                              App
                                (NonAtomic, true,
                                 LongIdent
                                   (false,
                                    SynLongIdent
                                      ([op_Addition], [],
                                       [Some (OriginalNotation "+")]), None,
                                    (1,35--1,36)), Ident x, (1,33--1,36)),
                              Const (Int32 1, (1,37--1,38)), (1,33--1,38)))),
                     (1,11--1,38), { ArrowRange = Some (1,17--1,19) }),
                  (1,4--1,8), NoneAtLet, { LeadingKeyword = Let (1,0--1,3)
                                           InlineKeyword = None
                                           EqualsRange = Some (1,9--1,10) })],
              (1,0--1,38))], PreXmlDocEmpty, [], None, (1,0--1,38),
          { LeadingKeyword = None })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))
