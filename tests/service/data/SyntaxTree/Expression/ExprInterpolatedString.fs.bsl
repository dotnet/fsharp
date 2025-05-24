ImplFile
  (ParsedImplFileInput
     ("/root/Expression/ExprInterpolatedString.fs", false,
      QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (x, None), false, None, (3,4--3,5)), None,
                  App
                    (Atomic, false, Ident C,
                     Paren
                       (App
                          (NonAtomic, false,
                           App
                             (NonAtomic, true,
                              LongIdent
                                (false,
                                 SynLongIdent
                                   ([op_Equality], [],
                                    [Some (OriginalNotation "=")]), None,
                                 (3,14--3,15)), Ident Name, (3,10--3,15)),
                           Const
                             (String ("123", Regular, (3,15--3,20)),
                              (3,15--3,20)), (3,10--3,20)), (3,9--3,10),
                        Some (3,20--3,21), (3,9--3,21)), (3,8--3,21)),
                  (3,4--3,5), Yes (3,0--3,21), { LeadingKeyword = Let (3,0--3,3)
                                                 InlineKeyword = None
                                                 EqualsRange = Some (3,6--3,7) })],
              (3,0--3,21));
           Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (y, None), false, None, (4,4--4,5)), None,
                  App
                    (Atomic, false, Ident C,
                     Paren
                       (App
                          (NonAtomic, false,
                           App
                             (NonAtomic, true,
                              LongIdent
                                (false,
                                 SynLongIdent
                                   ([op_EqualsDollar], [],
                                    [Some (OriginalNotation "=$")]), None,
                                 (4,14--4,16)), Ident Name, (4,10--4,16)),
                           Const
                             (String ("123", Regular, (4,16--4,21)),
                              (4,16--4,21)), (4,10--4,21)), (4,9--4,10),
                        Some (4,21--4,22), (4,9--4,22)), (4,8--4,22)),
                  (4,4--4,5), Yes (4,0--4,22), { LeadingKeyword = Let (4,0--4,3)
                                                 InlineKeyword = None
                                                 EqualsRange = Some (4,6--4,7) })],
              (4,0--4,22));
           Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((5,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (z, None), false, None, (5,4--5,5)), None,
                  App
                    (Atomic, false, Ident C,
                     Paren
                       (App
                          (NonAtomic, false,
                           App
                             (NonAtomic, true,
                              LongIdent
                                (false,
                                 SynLongIdent
                                   ([op_Equality], [],
                                    [Some (OriginalNotation "=")]), None,
                                 (5,14--5,15)), Ident Name, (5,10--5,15)),
                           InterpolatedString
                             ([String ("123", (5,16--5,22))], Regular,
                              (5,16--5,22)), (5,10--5,22)), (5,9--5,10),
                        Some (5,22--5,23), (5,9--5,23)), (5,8--5,23)),
                  (5,4--5,5), Yes (5,0--5,23), { LeadingKeyword = Let (5,0--5,3)
                                                 InlineKeyword = None
                                                 EqualsRange = Some (5,6--5,7) })],
              (5,0--5,23));
           Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((6,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (a, None), false, None, (6,4--6,5)), None,
                  ArbitraryAfterError ("localBinding2", (6,5--6,5)), (6,4--6,5),
                  Yes (6,0--6,5), { LeadingKeyword = Let (6,0--6,3)
                                    InlineKeyword = None
                                    EqualsRange = None })], (6,0--6,5));
           Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((7,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([], SynArgInfo ([], false, None)), None),
                  Named (SynIdent (b, None), false, None, (7,4--7,5)), None,
                  InterpolatedString
                    ([String ("123", (7,8--7,14))], Regular, (7,8--7,14)),
                  (7,4--7,5), Yes (7,0--7,14), { LeadingKeyword = Let (7,0--7,3)
                                                 InlineKeyword = None
                                                 EqualsRange = Some (7,6--7,7) })],
              (7,0--7,14))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--7,14), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(6,6)-(6,8) parse error Unexpected infix operator in binding. Expected '=' or other token.
