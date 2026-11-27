namespace Microsoft.CommonLanguageServerProtocol.Framework;

using System.Text.Json;
using StreamJsonRpc;

internal abstract class CancellableSystemTextJsonLanguageServer<TRequestContext>(
    JsonRpc jsonRpc, JsonSerializerOptions options)
    : SystemTextJsonLanguageServer<TRequestContext>(jsonRpc, options)
{
    protected override IRequestExecutionQueue<TRequestContext> ConstructRequestExecutionQueue()
    {
        var queue = new CancellableRequestExecutionQueue(this, HandlerProvider);
        queue.Start();
        return queue;
    }

    private sealed class CancellableRequestExecutionQueue(
        AbstractLanguageServer<TRequestContext> server, AbstractHandlerProvider handlers)
        : RequestExecutionQueue<TRequestContext>(server, handlers)
    {
        protected override bool CancelInProgressWorkUponMutatingRequest => true;
    }
}
