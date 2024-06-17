ImplFile
  (ParsedImplFileInput
     ("/root/TypeParameters/MethodDefAndUse.fs", false,
      QualifiedNameOfFile MethodDefAndUse, [], [],
      [SynModuleOrNamespace
         ([MethodDefAndUse], false, AnonModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, true, false, [],
                  PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None,
                     SynValInfo
                       ([[SynArgInfo ([], false, Some arg)]],
                        SynArgInfo ([], false, None)), None),
                  LongIdent
                    (SynLongIdent ([myMethod], [], [None]), None,
                     Some
                       (SynValTyparDecls
                          (Some
                             (PostfixList
                                ([SynTyparDecl
                                    ([], SynTypar (S, HeadType, false), [],
                                     { AmpersandRanges = [] });
                                  SynTyparDecl
                                    ([], SynTypar (a, None, false), [],
                                     { AmpersandRanges = [] })], [],
                                 (1,19--1,27))), false)),
                     Pats
                       [Paren
                          (Typed
                             (Named
                                (SynIdent (arg, None), false, None, (1,29--1,32)),
                              Var (SynTypar (a, None, false), (1,35--1,37)),
                              (1,29--1,37)), (1,28--1,38))], None, (1,11--1,38)),
                  None, Ident arg, (1,11--1,38), NoneAtLet,
                  { LeadingKeyword = Let (1,0--1,3)
                    InlineKeyword = Some (1,4--1,10)
                    EqualsRange = Some (1,39--1,40) })], (1,0--1,44));
           Expr
             (App
                (NonAtomic, false,
                 TypeApp
                   (Ident myMethod, (3,8--3,9),
                    [Var (SynTypar (T, HeadType, false), (3,9--3,11))], [],
                    Some (3,11--3,12), (3,8--3,12), (3,0--3,12)),
                 Const (Int32 2, (3,13--3,14)), (3,0--3,14)), (3,0--3,14))],
          PreXmlDocEmpty, [], None, (1,0--3,14), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
