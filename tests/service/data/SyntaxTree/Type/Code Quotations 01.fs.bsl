ImplFile
  (ParsedImplFileInput
     ("/root/Type/Code Quotations 01.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Open
             (ModuleOrNamespace
                (SynLongIdent
                   ([FSharp; Quotations], [(4,11--4,12)], [None; None]),
                 (4,5--4,22)), (4,0--4,22));
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [MyClass],
                     PreXmlDoc ((6,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (6,5--6,12)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [], Const (Unit, (6,12--6,14)), None,
                         PreXmlDoc ((6,12), FSharp.Compiler.Xml.XmlDocCollector),
                         (6,5--6,12), { AsKeyword = None });
                      Member
                        (SynBinding
                           (None, Normal, false, false, [],
                            PreXmlDoc ((7,4), FSharp.Compiler.Xml.XmlDocCollector),
                            SynValData
                              (Some { IsInstance = true
                                      IsDispatchSlot = false
                                      IsOverrideOrExplicitImpl = false
                                      IsFinal = false
                                      GetterOrSetterIsCompilerGenerated = false
                                      MemberKind = Member },
                               SynValInfo
                                 ([[SynArgInfo ([], false, None)]; []],
                                  SynArgInfo ([], false, None)), None),
                            LongIdent
                              (SynLongIdent
                                 ([_; GetQuotation], [(7,12--7,13)],
                                  [None; None]), None, None,
                               Pats
                                 [Paren
                                    (Const (Unit, (7,25--7,27)), (7,25--7,27))],
                               None, (7,11--7,27)), None,
                            FromParseError
                              (Quote
                                 (Ident op_Quotation, false,
                                  ArbitraryAfterError
                                    ("quoteExpr2", (8,10--8,10)), false,
                                  (8,8--8,10)), (8,8--8,10)), (7,11--7,27),
                            NoneAtInvisible,
                            { LeadingKeyword = Member (7,4--7,10)
                              InlineKeyword = None
                              EqualsRange = Some (7,28--7,29) }), (7,4--8,10))],
                     (7,4--8,10)), [],
                  Some
                    (ImplicitCtor
                       (None, [], Const (Unit, (6,12--6,14)), None,
                        PreXmlDoc ((6,12), FSharp.Compiler.Xml.XmlDocCollector),
                        (6,5--6,12), { AsKeyword = None })), (6,5--8,10),
                  { LeadingKeyword = Type (6,0--6,4)
                    EqualsRange = Some (6,15--6,16)
                    WithKeyword = None })], (6,0--8,10));
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [T],
                     PreXmlDoc ((9,12), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (9,17--9,18)),
                  Simple
                    (TypeAbbrev
                       (Ok, LongIdent (SynLongIdent ([int], [], [None])),
                        (9,21--9,24)), (9,21--9,24)), [], None, (9,17--9,24),
                  { LeadingKeyword = Type (9,12--9,16)
                    EqualsRange = Some (9,19--9,20)
                    WithKeyword = None })], (9,12--9,24));
           NestedModule
             (SynComponentInfo
                ([], None, [], [M],
                 PreXmlDoc ((10,12), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (10,12--10,20)), false,
              [Let
                 (false,
                  [SynBinding
                     (None, Normal, false, false, [],
                      PreXmlDoc ((11,16), FSharp.Compiler.Xml.XmlDocCollector),
                      SynValData
                        (None, SynValInfo ([], SynArgInfo ([], false, None)),
                         None),
                      Named (SynIdent (x, None), false, None, (11,20--11,21)),
                      None, Const (Int32 1, (11,24--11,25)), (11,20--11,21),
                      Yes (11,16--11,25), { LeadingKeyword = Let (11,16--11,19)
                                            InlineKeyword = None
                                            EqualsRange = Some (11,22--11,23) })],
                  (11,16--11,25))], false, (10,12--11,25),
              { ModuleKeyword = Some (10,12--10,18)
                EqualsRange = Some (10,21--10,22) })],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--11,25), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,59)] }, set []))

(9,12)-(9,16) parse error Incomplete structured construct at or before this point in quotation literal
(8,8)-(8,10) parse error Unmatched '<@ @>'
(9,12)-(9,16) parse error Unexpected keyword 'type' in type definition. Expected incomplete structured construct at or before this point or other token.
(10,12)-(10,18) parse warning Modules cannot be nested inside types. Define modules at module or namespace level.
(12,8)-(12,10) parse error Unexpected end of quotation in definition. Expected incomplete structured construct at or before this point or other token.
