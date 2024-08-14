ImplFile
  (ParsedImplFileInput
     ("/root/SimplePats/SimplePats 02.fs", false, QualifiedNameOfFile SimplePats,
      [], [],
      [SynModuleOrNamespace
         ([SimplePats], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [X],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,6)),
                  ObjectModel
                    (Class,
                     [ImplicitCtor
                        (None, [],
                         SimplePats
                           ([Typed
                               (Id (i, None, false, false, false, (3,7--3,8)),
                                LongIdent (SynLongIdent ([int], [], [None])),
                                (3,7--3,13))], [], (3,6--3,14)), None,
                         PreXmlDoc ((3,6), FSharp.Compiler.Xml.XmlDocCollector),
                         (3,5--3,6), { AsKeyword = None })], (3,17--3,26)), [],
                  Some
                    (ImplicitCtor
                       (None, [],
                        SimplePats
                          ([Typed
                              (Id (i, None, false, false, false, (3,7--3,8)),
                               LongIdent (SynLongIdent ([int], [], [None])),
                               (3,7--3,13))], [], (3,6--3,14)), None,
                        PreXmlDoc ((3,6), FSharp.Compiler.Xml.XmlDocCollector),
                        (3,5--3,6), { AsKeyword = None })), (3,5--3,26),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,15--3,16)
                    WithKeyword = None })], (3,0--3,26));
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Y],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,5--4,6)),
                  ObjectModel
                    (Class,
                     [ImplicitCtor
                        (None, [],
                         SimplePats
                           ([Id (a, None, false, false, false, (4,7--4,8));
                             Id (b, None, false, false, false, (4,9--4,10))],
                            [(4,8--4,9)], (4,6--4,11)), None,
                         PreXmlDoc ((4,6), FSharp.Compiler.Xml.XmlDocCollector),
                         (4,5--4,6), { AsKeyword = None })], (4,14--4,23)), [],
                  Some
                    (ImplicitCtor
                       (None, [],
                        SimplePats
                          ([Id (a, None, false, false, false, (4,7--4,8));
                            Id (b, None, false, false, false, (4,9--4,10))],
                           [(4,8--4,9)], (4,6--4,11)), None,
                        PreXmlDoc ((4,6), FSharp.Compiler.Xml.XmlDocCollector),
                        (4,5--4,6), { AsKeyword = None })), (4,5--4,23),
                  { LeadingKeyword = Type (4,0--4,4)
                    EqualsRange = Some (4,12--4,13)
                    WithKeyword = None })], (4,0--4,23));
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Z],
                     PreXmlDoc ((5,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (5,5--5,6)),
                  ObjectModel
                    (Class,
                     [ImplicitCtor
                        (None, [],
                         SimplePats
                           ([Attrib
                               (Id
                                  (bar, None, false, false, false, (5,15--5,18)),
                                [{ Attributes =
                                    [{ TypeName =
                                        SynLongIdent ([Foo], [], [None])
                                       ArgExpr = Const (Unit, (5,9--5,12))
                                       Target = None
                                       AppliesToGetterAndSetter = false
                                       Range = (5,9--5,12) }]
                                   Range = (5,7--5,14) }], (5,7--5,18));
                             Attrib
                               (Typed
                                  (Id
                                     (v, None, false, false, false, (5,28--5,29)),
                                   LongIdent (SynLongIdent ([V], [], [None])),
                                   (5,28--5,32)),
                                [{ Attributes =
                                    [{ TypeName =
                                        SynLongIdent ([Foo], [], [None])
                                       ArgExpr = Const (Unit, (5,22--5,25))
                                       Target = None
                                       AppliesToGetterAndSetter = false
                                       Range = (5,22--5,25) }]
                                   Range = (5,20--5,27) }], (5,20--5,32))],
                            [(5,18--5,19)], (5,6--5,33)), None,
                         PreXmlDoc ((5,6), FSharp.Compiler.Xml.XmlDocCollector),
                         (5,5--5,6), { AsKeyword = None })], (5,36--5,45)), [],
                  Some
                    (ImplicitCtor
                       (None, [],
                        SimplePats
                          ([Attrib
                              (Id (bar, None, false, false, false, (5,15--5,18)),
                               [{ Attributes =
                                   [{ TypeName =
                                       SynLongIdent ([Foo], [], [None])
                                      ArgExpr = Const (Unit, (5,9--5,12))
                                      Target = None
                                      AppliesToGetterAndSetter = false
                                      Range = (5,9--5,12) }]
                                  Range = (5,7--5,14) }], (5,7--5,18));
                            Attrib
                              (Typed
                                 (Id
                                    (v, None, false, false, false, (5,28--5,29)),
                                  LongIdent (SynLongIdent ([V], [], [None])),
                                  (5,28--5,32)),
                               [{ Attributes =
                                   [{ TypeName =
                                       SynLongIdent ([Foo], [], [None])
                                      ArgExpr = Const (Unit, (5,22--5,25))
                                      Target = None
                                      AppliesToGetterAndSetter = false
                                      Range = (5,22--5,25) }]
                                  Range = (5,20--5,27) }], (5,20--5,32))],
                           [(5,18--5,19)], (5,6--5,33)), None,
                        PreXmlDoc ((5,6), FSharp.Compiler.Xml.XmlDocCollector),
                        (5,5--5,6), { AsKeyword = None })), (5,5--5,45),
                  { LeadingKeyword = Type (5,0--5,4)
                    EqualsRange = Some (5,34--5,35)
                    WithKeyword = None })], (5,0--5,45));
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Unit],
                     PreXmlDoc ((6,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (6,5--6,9)),
                  ObjectModel
                    (Class,
                     [ImplicitCtor
                        (None, [], SimplePats ([], [], (6,9--6,11)), None,
                         PreXmlDoc ((6,9), FSharp.Compiler.Xml.XmlDocCollector),
                         (6,5--6,9), { AsKeyword = None })], (6,14--6,23)), [],
                  Some
                    (ImplicitCtor
                       (None, [], SimplePats ([], [], (6,9--6,11)), None,
                        PreXmlDoc ((6,9), FSharp.Compiler.Xml.XmlDocCollector),
                        (6,5--6,9), { AsKeyword = None })), (6,5--6,23),
                  { LeadingKeyword = Type (6,0--6,4)
                    EqualsRange = Some (6,12--6,13)
                    WithKeyword = None })], (6,0--6,23))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,23), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))
