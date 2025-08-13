ImplFile
  (ParsedImplFileInput
     ("/root/Type/Do Binding With Invalid Constructs 01.fs", false,
      QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [ClassWithDoBinding],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,5--4,23)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [], Const (Unit, (4,23--4,25)), None,
                         PreXmlDoc ((4,23), FSharp.Compiler.Xml.XmlDocCollector),
                         (4,5--4,23), { AsKeyword = None });
                      LetBindings
                        ([SynBinding
                            (None, Normal, false, true, [],
                             PreXmlDoc ((5,4), FSharp.Compiler.Xml.XmlDocCollector),
                             SynValData
                               (None,
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                None),
                             Named
                               (SynIdent (initialized, None), false, None,
                                (5,16--5,27)), None,
                             Const (Bool false, (5,30--5,35)), (5,16--5,27),
                             Yes (5,4--5,35),
                             { LeadingKeyword = Let (5,4--5,7)
                               InlineKeyword = None
                               EqualsRange = Some (5,28--5,29) })], false, false,
                         (5,4--5,35));
                      LetBindings
                        ([SynBinding
                            (None, Do, false, false, [], PreXmlDocEmpty,
                             SynValData
                               (None,
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                None), Const (Unit, (7,4--9,27)), None,
                             Sequential
                               (SuppressNeither, true,
                                App
                                  (NonAtomic, false, Ident printfn,
                                   Const
                                     (String
                                        ("Starting initialization", Regular,
                                         (8,16--8,41)), (8,16--8,41)),
                                   (8,8--8,41)),
                                LongIdentSet
                                  (SynLongIdent ([initialized], [], [None]),
                                   Const (Bool true, (9,23--9,27)), (9,8--9,27)),
                                (8,8--9,27), { SeparatorRange = None }),
                             (7,4--9,27), NoneAtDo,
                             { LeadingKeyword = Do (7,4--7,6)
                               InlineKeyword = None
                               EqualsRange = None })], false, false, (7,4--9,27))],
                     (5,4--9,27)), [],
                  Some
                    (ImplicitCtor
                       (None, [], Const (Unit, (4,23--4,25)), None,
                        PreXmlDoc ((4,23), FSharp.Compiler.Xml.XmlDocCollector),
                        (4,5--4,23), { AsKeyword = None })), (4,5--9,27),
                  { LeadingKeyword = Type (4,0--4,4)
                    EqualsRange = Some (4,26--4,27)
                    WithKeyword = None })], (4,0--9,27));
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [InternalType],
                     PreXmlDoc ((11,4), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (11,9--11,21)),
                  Simple
                    (TypeAbbrev
                       (Ok, LongIdent (SynLongIdent ([int], [], [None])),
                        (11,24--11,27)), (11,24--11,27)), [], None,
                  (11,9--11,27), { LeadingKeyword = Type (11,4--11,8)
                                   EqualsRange = Some (11,22--11,23)
                                   WithKeyword = None })], (11,4--11,27))],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--11,27), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,57)] }, set []))

(11,4)-(11,8) parse warning Nested type definitions are not allowed. Types must be defined at module or namespace level.
(13,4)-(13,6) parse error Unexpected keyword 'do' in definition. Expected incomplete structured construct at or before this point or other token.
(16,4)-(16,10) parse warning Modules cannot be nested inside types. Define modules at module or namespace level.
(19,4)-(19,8) parse warning 'open' declarations must appear at module level, not inside types.
