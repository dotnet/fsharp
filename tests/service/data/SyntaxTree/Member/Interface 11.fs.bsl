ImplFile
  (ParsedImplFileInput
     ("/root/Member/Interface 11.fs", false, QualifiedNameOfFile Module, [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [T],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,6)),
                  ObjectModel
                    (Unspecified,
                     [Interface
                        (LongIdent (SynLongIdent ([I], [], [None])),
                         Some (4,16--4,20), Some [], (4,4--4,20))], (4,4--4,20)),
                  [], None, (3,5--4,20), { LeadingKeyword = Type (3,0--3,4)
                                           EqualsRange = Some (3,7--3,8)
                                           WithKeyword = None })], (3,0--4,20));
           Let
             (false,
              [SynBinding
                 (None, Normal, false, false, [],
                  PreXmlDoc ((6,0), FSharp.Compiler.Xml.XmlDocCollector),
                  SynValData
                    (None, SynValInfo ([[]], SynArgInfo ([], false, None)), None),
                  LongIdent
                    (SynLongIdent ([f], [], [None]), None, None,
                     Pats [Paren (Const (Unit, (6,6--6,8)), (6,6--6,8))], None,
                     (6,4--6,8)), None, Const (Unit, (6,11--6,13)), (6,4--6,8),
                  NoneAtLet, { LeadingKeyword = Let (6,0--6,3)
                               InlineKeyword = None
                               EqualsRange = Some (6,9--6,10) })], (6,0--6,13),
              { InKeyword = None })],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--6,13), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        WarnDirectives = []
        CodeComments = [] }, set []))

(4,21)-(4,23) parse error Unexpected symbol '[<' in member definition
(6,0)-(6,3) parse error Incomplete structured construct at or before this point in member definition
