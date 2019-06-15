// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.LanguageServer.Client;
using Microsoft.VisualStudio.Threading;

namespace Microsoft.VisualStudio.FSharp.Editor.Helpers
{
    /// <summary>
    /// Exists as an abstract implementor of <see cref="ILanguageClient" /> purely to manage the non-standard async
    /// event handlers.
    /// </summary>
    public abstract class LanguageClient : ILanguageClient
    {
        public abstract string Name { get; }

        public abstract IEnumerable<string> ConfigurationSections { get; }

        public abstract object InitializationOptions { get; }

        public abstract IEnumerable<string> FilesToWatch { get; }

        public event AsyncEventHandler<EventArgs> StartAsync;

#pragma warning disable 67 // The event 'LanguageClient.StopAsync' is never used
        public event AsyncEventHandler<EventArgs> StopAsync;
#pragma warning restore 67

        public abstract Task<Connection> ActivateAsync(CancellationToken token);

        protected abstract Task DoLoadAsync();

        public async Task OnLoadedAsync()
        {
            await DoLoadAsync();
            await StartAsync.InvokeAsync(this, EventArgs.Empty);
        }

        public abstract Task OnServerInitializeFailedAsync(Exception e);

        public abstract Task OnServerInitializedAsync();
    }
}
