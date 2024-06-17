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
           Expr
             (App
                (Atomic, false,
                 TypeApp
                   (Ident AGoodType, (4,9--4,10),
                    [Var (SynTypar (P, None, false), (4,10--4,12));
                     LongIdent (SynLongIdent ([int], [], [None]))],
                    [(4,12--4,13)], Some (4,17--4,18), (4,9--4,18), (4,0--4,18)),
                 Const (Unit, (4,18--4,20)), (4,0--4,20)), (4,0--4,20))],
          PreXmlDocEmpty, [], None, (1,0--4,20), { LeadingKeyword = None })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))
