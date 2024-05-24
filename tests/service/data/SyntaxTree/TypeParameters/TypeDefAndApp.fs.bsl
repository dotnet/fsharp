ImplFile
  (ParsedImplFileInput
     ("/root/TypeParameters/TypeDefAndApp.fs", false,
      QualifiedNameOfFile TypeDefAndApp, [], [],
      [SynModuleOrNamespace
         ([TypeDefAndApp], false, AnonModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([],
                     Some
                       (PostfixList
                          ([SynTyparDecl
                              ([], SynTypar (T, HeadType, false), [],
                               { AmpersandRanges = [] });
                            SynTyparDecl
                              ([], SynTypar (S, HeadType, false), [],
                               { AmpersandRanges = [] })], [], (1,14--1,22))),
                     [], [AGoodType],
                     PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector),
                     true, None, (1,5--1,14)),
                  ObjectModel
                    (Class,
                     [ImplicitCtor
                        (None, [], Const (Unit, (1,22--1,24)), None,
                         PreXmlDoc ((1,22), FSharp.Compiler.Xml.XmlDocCollector),
                         (1,5--1,14), { AsKeyword = None })], (2,4--2,13)), [],
                  Some
                    (ImplicitCtor
                       (None, [], Const (Unit, (1,22--1,24)), None,
                        PreXmlDoc ((1,22), FSharp.Compiler.Xml.XmlDocCollector),
                        (1,5--1,14), { AsKeyword = None })), (1,5--2,13),
                  { LeadingKeyword = Type (1,0--1,4)
                    EqualsRange = Some (1,24--1,25)
                    WithKeyword = None })], (1,0--2,13));
           Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (myInstance, None), false, None, (4,4--4,14)),
                  None,
                  App
                    (Atomic, false,
                     TypeApp
                       (Ident AGoodType, (4,26--4,27),
                        [Var (SynTypar (P, None, false), (4,27--4,29));
                         LongIdent (SynLongIdent ([int], [], [None]))],
                        [(4,29--4,30)], Some (4,34--4,35), (4,26--4,35),
                        (4,17--4,35)), Const (Unit, (4,35--4,37)), (4,17--4,37)),
                  (4,4--4,14), Yes (4,0--4,37),
                  { LeadingKeyword = Let (4,0--4,3)
                    InlineKeyword = None
                    EqualsRange = Some (4,15--4,16) })], (4,0--4,37))],
          PreXmlDocEmpty, [], None, (1,0--4,37), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
