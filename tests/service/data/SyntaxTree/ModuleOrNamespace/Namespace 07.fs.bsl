ImplFile
  (ParsedImplFileInput
     ("/root/ModuleOrNamespace/Namespace 07.fs", false,
      QualifiedNameOfFile Namespace 07, [], [],
      [SynModuleOrNamespace
         ([], false, DeclaredNamespace,
          [Types
             ([SynTypeDefn
                 (SynComponentInfo
                    ([], None, [], [T],
                     PreXmlDoc ((3,0), FSharp.Compiler.Xml.XmlDocCollector),
                     false, None, (3,5--3,6)),
                  Simple
                    (TypeAbbrev
                       (Ok, LongIdent (SynLongIdent ([int], [], [None])),
                        (3,9--3,12)), (3,9--3,12)), [], None, (3,5--3,12),
                  { LeadingKeyword = Type (3,0--3,4)
                    EqualsRange = Some (3,7--3,8)
                    WithKeyword = None })], (3,0--3,12))], PreXmlDocEmpty, [],
          None, (1,0--3,12), { LeadingKeyword = Namespace (1,0--1,9) })],
      (true, true), { ConditionalDirectives = []
                      CodeComments = [] }, set []))

(3,0)-(3,4) parse error Unexpected start of structured construct in implementation file. Expected identifier, 'global' or other token.
