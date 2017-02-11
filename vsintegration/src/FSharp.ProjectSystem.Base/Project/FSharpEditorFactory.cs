using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Designer.Interfaces;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudioTools.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace Microsoft.VisualStudio.FSharp.ProjectSystem
{
    [Guid(Constants.FSharpEditorFactoryIdString)]
    [ProvideEditorFactory(typeof(FSharpEditorFactory), 101)]
    [ProvideEditorExtension(typeof(FSharpEditorFactory), ".fs", 32)]
    [ProvideEditorExtension(typeof(FSharpEditorFactory), ".fsi", 32)]
    [ProvideEditorExtension(typeof(FSharpEditorFactory), ".fsscript", 32)]
    [ProvideEditorExtension(typeof(FSharpEditorFactory), ".fsx", 32)]
    public class FSharpEditorFactory : CommonEditorFactory
    {
        public FSharpEditorFactory(Package package) : base(package) { }
    }
}
