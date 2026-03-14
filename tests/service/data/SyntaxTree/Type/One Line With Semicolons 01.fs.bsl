ImplFile
  (ParsedImplFileInput
     ("/root/Type/One Line With Semicolons 01.fs", false,
      QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [A],
                     PreXmlDoc ((4,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,5--4,6)),
                  Simple
                    (TypeAbbrev
                       (Ok, LongIdent (SynLongIdent ([A], [], [None])),
                        (4,9--4,10)), (4,9--4,10)), [], None, (4,5--4,10),
                  { LeadingKeyword = Type (4,0--4,4)
                    EqualsRange = Some (4,7--4,8)
                    WithKeyword = None })], (4,0--4,10));
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [B],
                     PreXmlDoc ((4,12), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (4,17--4,18)),
                  Simple
                    (TypeAbbrev
                       (Ok, LongIdent (SynLongIdent ([A], [], [None])),
                        (4,21--4,22)), (4,21--4,22)), [], None, (4,17--4,22),
                  { LeadingKeyword = Type (4,12--4,16)
                    EqualsRange = Some (4,19--4,20)
                    WithKeyword = None })], (4,12--4,22));
           NestedModule
             (SynComponentInfo
                ([], None, [], [C],
                 PreXmlDoc ((4,24), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (4,24--4,32)), false,
              [Expr (Const (Unit, (4,35--4,37)), (4,35--4,37))], false,
              (4,24--4,37), { ModuleKeyword = Some (4,24--4,30)
                              EqualsRange = Some (4,33--4,34) });
           Exception
             (SynExceptionDefn
                (SynExceptionDefnRepr
                   ([],
                    SynUnionCase
                      ([], SynIdent (D, None), Fields [], PreXmlDocEmpty, None,
                       (4,49--4,50), { BarRange = None }), None,
                    PreXmlDoc ((4,39), FSharp.Compiler.Xml.XmlDocCollector),
                    None, (4,39--4,50)), None, [], (4,39--4,50)), (4,39--4,50));
           ModuleAbbrev (E, [C], (4,52--4,64));
           Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((4,66), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([[]], SynArgInfo ([], false, None)), None),
                  LongIdent
                    (SynLongIdent ([f], [], [None]), None, None,
                     Pats [Paren (Const (Unit, (4,72--4,74)), (4,72--4,74))],
                     None, (4,70--4,74)), None, Const (Unit, (4,77--4,79)),
                  (4,70--4,74), NoneAtLet, { LeadingKeyword = Let (4,66--4,69)
                                             InlineKeyword = None
                                             EqualsRange = Some (4,75--4,76) })],
              (4,66--4,79), { InKeyword = None });
           Open
             (ModuleOrNamespace
                (SynLongIdent ([System], [], [None]), (4,86--4,92)),
              (4,81--4,92));
           NestedModule
             (SynComponentInfo
                ([], None, [], [G],
                 PreXmlDoc ((4,94), FSharp.Compiler.Xml.XmlDocCollector), false,
                 None, (4,94--4,102)), false,
              [ModuleAbbrev (H, [E], (4,105--4,117))], false, (4,94--4,117),
              { ModuleKeyword = Some (4,94--4,100)
                EqualsRange = Some (4,103--4,104) })],
          PreXmlDoc ((2,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (2,0--4,117), { LeadingKeyword = Module (2,0--2,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [LineComment (1,0--1,50)] }, set []))
