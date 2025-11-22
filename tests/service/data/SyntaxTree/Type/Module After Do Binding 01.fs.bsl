ImplFile
  (ParsedImplFileInput
     ("/root/Type/Module After Do Binding 01.fs", false,
      QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [MyClass],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,5--4,12)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [], Const (Unit, (4,12--4,14)), None,
                         PreXmlDoc ((4,12), FSharp.Compiler.Xml.XmlDocCollector),
                         (4,5--4,12), { AsKeyword = None });
                      LetBindings
                        ([SynBinding
                            (None, Do, false, false, [], PreXmlDocEmpty,
                             SynValData
                               (None,
                                SynValInfo ([], SynArgInfo ([], false, None)),
                                None), Const (Unit, (5,4--5,29)), None,
                             App
                               (NonAtomic, false, Ident printfn,
                                Const
                                  (String
                                     ("Initializing", Regular, (5,15--5,29)),
                                   (5,15--5,29)), (5,7--5,29)), (5,4--5,29),
                             NoneAtDo, { LeadingKeyword = Do (5,4--5,6)
                                         InlineKeyword = None
                                         EqualsRange = None })], false, false,
                         (5,4--5,29), { InKeyword = None })], (5,4--5,29)), [],
                  Some
                    (ImplicitCtor
                       (None, [], Const (Unit, (4,12--4,14)), None,
                        PreXmlDoc ((4,12), FSharp.Compiler.Xml.XmlDocCollector),
                        (4,5--4,12), { AsKeyword = None })), (4,5--5,29),
                  { LeadingKeyword = Type (4,0--4,4)
                    EqualsRange = Some (4,15--4,16)
                    WithKeyword = None })], (4,0--5,29));
           NestedModule
             (SynComponentInfo
                ([], None, [], [M],
                 PreXmlDoc ((6,4), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (6,4--6,12)), false,
              [Let
                 (false,
                  [SynBinding
                     (None, Normal, false, false, [],
                      PreXmlDoc ((7,8), FSharp.Compiler.Xml.XmlDocCollector),
                      SynValData
                        (None, SynValInfo ([], SynArgInfo ([], false, None)),
                         None),
                      Named (SynIdent (helper, None), false, None, (7,12--7,18)),
                      None, Const (Int32 42, (7,21--7,23)), (7,12--7,18),
                      Yes (7,8--7,23), { LeadingKeyword = Let (7,8--7,11)
                                         InlineKeyword = None
                                         EqualsRange = Some (7,19--7,20) })],
                  (7,8--7,23), { InKeyword = None })], false, (6,4--7,23),
              { ModuleKeyword = Some (6,4--6,10)
                EqualsRange = Some (6,13--6,14) })],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--7,23), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,48)] }, set []))

(6,4)-(6,10) parse error Modules cannot be nested inside types. Define modules at module or namespace level.
