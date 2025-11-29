ImplFile
  (ParsedImplFileInput
     ("/root/Type/Module After Inherit 01.fs", false, QualifiedNameOfFile Module,
      [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Base],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,5--4,9)),
                  ObjectModel
                    (Class,
                     [ImplicitCtor
                        (None, [], Const (Unit, (4,9--4,11)), None,
                         PreXmlDoc ((4,9), FSharp.Compiler.Xml.XmlDocCollector),
                         (4,5--4,9), { AsKeyword = None })], (4,14--4,23)), [],
                  Some
                    (ImplicitCtor
                       (None, [], Const (Unit, (4,9--4,11)), None,
                        PreXmlDoc ((4,9), FSharp.Compiler.Xml.XmlDocCollector),
                        (4,5--4,9), { AsKeyword = None })), (4,5--4,23),
                  { LeadingKeyword = Type (4,0--4,4)
                    EqualsRange = Some (4,12--4,13)
                    WithKeyword = None })], (4,0--4,23));
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [Derived],
                     PreXmlDoc ((6,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (6,5--6,12)),
                  ObjectModel
                    (Unspecified,
                     [ImplicitCtor
                        (None, [], Const (Unit, (6,12--6,14)), None,
                         PreXmlDoc ((6,12), FSharp.Compiler.Xml.XmlDocCollector),
                         (6,5--6,12), { AsKeyword = None });
                      ImplicitInherit
                        (LongIdent (SynLongIdent ([Base], [], [None])),
                         Const (Unit, (7,16--7,18)), None, (7,4--7,18),
                         { InheritKeyword = (7,4--7,11) })], (7,4--7,18)), [],
                  Some
                    (ImplicitCtor
                       (None, [], Const (Unit, (6,12--6,14)), None,
                        PreXmlDoc ((6,12), FSharp.Compiler.Xml.XmlDocCollector),
                        (6,5--6,12), { AsKeyword = None })), (6,5--7,18),
                  { LeadingKeyword = Type (6,0--6,4)
                    EqualsRange = Some (6,15--6,16)
                    WithKeyword = None })], (6,0--7,18));
           NestedModule
             (SynComponentInfo
                ([], None, [], [InvalidModule],
                 PreXmlDoc ((9,4), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (9,4--9,24)), false,
              [Let
                 (false,
                  [SynBinding
                     (None, Normal, false, false, [],
                      PreXmlDoc ((10,8), FSharp.Compiler.Xml.XmlDocCollector),
                      SynValData
                        (None, SynValInfo ([], SynArgInfo ([], false, None)),
                         None),
                      Named (SynIdent (x, None), false, None, (10,12--10,13)),
                      None, Const (Int32 2, (10,16--10,17)), (10,12--10,13),
                      Yes (10,8--10,17), { LeadingKeyword = Let (10,8--10,11)
                                           InlineKeyword = None
                                           EqualsRange = Some (10,14--10,15) })],
                  (10,8--10,17), { InKeyword = None })], false, (9,4--10,17),
              { ModuleKeyword = Some (9,4--9,10)
                EqualsRange = Some (9,25--9,26) })],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--10,17), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,32)] }, set []))

(9,4)-(9,10) parse error Modules cannot be nested inside types. Define modules at module or namespace level.
