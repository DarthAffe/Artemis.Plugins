using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Artemis.Plugins.LayerBrushes.ShaderBrush.OpenGL;

internal class OpenGLThread : IDisposable
{
    #region Properties & Fields

    private readonly Thread _thread;

    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly CancellationToken _cancellationToken;

    private readonly BlockingCollection<(Action action, ManualResetEventSlim waitHandle)> _actions = new();

    #endregion

    #region Constructors

    public OpenGLThread()
    {
        this._cancellationToken = _cancellationTokenSource.Token;

        _thread = new Thread(UpdateLoop);
        _thread.Start();
    }

    #endregion

    #region Methods

    public void Invoke(Action action)
    {
        ManualResetEventSlim waitHandle = new(false);
        _actions.Add((action, waitHandle), _cancellationToken);

        waitHandle.Wait(_cancellationToken);
        waitHandle.Dispose();
    }

    private void UpdateLoop()
    {
        while (!_cancellationToken.IsCancellationRequested)
        {
            try
            {
                if (_actions.TryTake(out (Action action, ManualResetEventSlim waitHandle) data, 500))
                {
                    try
                    {
                        data.action();
                    }
                    finally
                    {
                        data.waitHandle.Set();
                    }
                }
            }
            catch { /**/ }
        }
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _thread.Join(1000);
    }

    #endregion
}