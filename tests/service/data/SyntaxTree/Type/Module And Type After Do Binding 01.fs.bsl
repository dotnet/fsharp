ImplFile
  (ParsedImplFileInput
     ("/root/Type/Module And Type After Do Binding 01.fs", false,
      QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [ClassWithDo],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,5--4,16)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [], Const (Unit, (4,16--4,18)), None,
                         PreXmlDoc ((4,16), FSharp.Compiler.Xml.XmlDocCollector),
                         (4,5--4,16), { AsKeyword = None });
                      LetBindings
                        ([SynBinding
                            (None, Do, false, false, [], PreXmlDocEmpty,
                             SynValData
                               (None,
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                None), Const (Unit, (5,4--6,22)), None,
                             App
                               (NonAtomic, false, Ident printfn,
                                Const
                                  (String ("init", Regular, (6,16--6,22)),
                                   (6,16--6,22)), (6,8--6,22)), (5,4--6,22),
                             NoneAtDo, { LeadingKeyword = Do (5,4--5,6)
                                         InlineKeyword = None
                                         EqualsRange = None })], false, false,
                         (5,4--6,22), { InKeyword = None })], (5,4--6,22)), [],
                  Some
                    (ImplicitCtor
                       (None, [], Const (Unit, (4,16--4,18)), None,
                        PreXmlDoc ((4,16), FSharp.Compiler.Xml.XmlDocCollector),
                        (4,5--4,16), { AsKeyword = None })), (4,5--6,22),
                  { LeadingKeyword = Type (4,0--4,4)
                    EqualsRange = Some (4,19--4,20)
                    WithKeyword = None })], (4,0--6,22));
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [InternalType],
                     PreXmlDoc ((8,4), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (8,9--8,21)),
                  Simple
                    (TypeAbbrev
                       (Ok, LongIdent (SynLongIdent ([int], [], [None])),
                        (8,24--8,27)), (8,24--8,27)), [], None, (8,9--8,27),
                  { LeadingKeyword = Type (8,4--8,8)
                    EqualsRange = Some (8,22--8,23)
                    WithKeyword = None })], (8,4--8,27));
           NestedModule
             (SynComponentInfo
                ([], None, [], [InternalModule],
                 PreXmlDoc ((10,4), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (10,4--10,25)), false,
              [Let
                 (false,
                  [SynBinding
                     (None, Normal, false, false, [],
                      PreXmlDoc ((11,8), FSharp.Compiler.Xml.XmlDocCollector),
                      SynValData
                        (None, SynValInfo ([], SynArgInfo ([], false, None)),
                         None),
                      Named (SynIdent (x, None), false, None, (11,12--11,13)),
                      None, Const (Int32 1, (11,16--11,17)), (11,12--11,13),
                      Yes (11,8--11,17), { LeadingKeyword = Let (11,8--11,11)
                                           InlineKeyword = None
                                           EqualsRange = Some (11,14--11,15) })],
                  (11,8--11,17), { InKeyword = None })], false, (10,4--11,17),
              { ModuleKeyword = Some (10,4--10,10)
                EqualsRange = Some (10,26--10,27) });
           Open
             (ModuleOrNamespace
                (SynLongIdent
                   ([System; Collections], [(13,15--13,16)], [None; None]),
                 (13,9--13,27)), (13,4--13,27))],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--13,27), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,47)] }, set []))

(8,4)-(8,8) parse error Nested type definitions are not allowed. Types must be defined at module or namespace level.
(10,4)-(10,10) parse error Modules cannot be nested inside types. Define modules at module or namespace level.
(13,4)-(13,8) parse error 'open' declarations must appear at module level, not inside types.
