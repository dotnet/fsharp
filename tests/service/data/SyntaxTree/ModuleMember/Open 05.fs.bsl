ImplFile
  (ParsedImplFileInput
     ("/root/ModuleMember/Open 05.fs", false, QualifiedNameOfFile Module, [], [],
      [SynModuleOrNamespace
         ([Module], false, NamedModule,
          [Open
             (ModuleOrNamespace (SynLongIdent ([], [], []), (3,4--3,4)),
              (3,0--3,4));
           Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [T],
                     PreXmlDoc ((5,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (5,5--5,6)),
                  Simple
                    (TypeAbbrev
                       (Ok, LongIdent (SynLongIdent ([int], [], [None])),
                        (5,9--5,12)), (5,9--5,12)), [], None, (5,5--5,12),
                  { LeadingKeyword = Type (5,0--5,4)
                    EqualsRange = Some (5,7--5,8)
                    WithKeyword = None })], (5,0--5,12))],
          PreXmlDoc ((1,0), FSharp.Compiler.Xml.XmlDocCollector), [], None,
          (1,0--5,12), { LeadingKeyword = Module (1,0--1,6) })], (true, true),
      { ConditionalDirectives = []
        CodeComments = [] }, set []))

(3,5)-(5,0) parse error Incomplete structured construct at or before this point in open declaration. Expected identifier, 'global', 'type' or other token.
