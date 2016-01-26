/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the Apache License, Version 2.0, please send an email to 
 * vspython@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Apache License, Version 2.0.
 *
 * You must not remove this notice, or any other, from this software.
 *
 * ***************************************************************************/

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.VisualStudio;

namespace TestUtilities {
    public class WpfProxy : IDisposable {
        private readonly Thread _thread;
        private readonly Dispatcher _dispatcher;
        private bool _isDisposed;

        public WpfProxy() {
            using (var ready = new AutoResetEvent(false)) {
                _thread = new Thread(ControllerThread);
                _thread.SetApartmentState(ApartmentState.STA);
                _thread.Name = "Wpf Proxy Thread";
                _thread.Start(ready);
                ready.WaitOne();
            }
            for (int retries = 10; (_dispatcher = Dispatcher.FromThread(_thread)) == null && retries > 0; --retries) {
                Thread.Sleep(10);
                Console.WriteLine("Retry {0}", retries);
            }
            if (_dispatcher == null) {
                _thread.Abort();
                throw new InvalidOperationException("Unable to get dispatcher");
            }

            // Allow plenty of time for the dispatcher to start responding
            for (int retries = 50; retries > 0; --retries) {
                try {
                    _dispatcher.Invoke(() => { });
                    break;
                } catch (OperationCanceledException) {
                    Thread.Sleep(100);
                }
            }
        }

        public static WpfProxy FromObject(object obj) {
            var proxy = obj as WpfObjectProxy;
            if (proxy != null) {
                return proxy._provider;
            }
            return null;
        }

        private void ControllerThread(object obj) {
            var dispatcher = Dispatcher.FromThread(Thread.CurrentThread);
            ((AutoResetEvent)obj).Set();
            Dispatcher.Run();
        }

        public dynamic Create<T>(Func<T> creator) where T : DependencyObject {
            return new WpfObjectProxy<T>(this, InvokeWithRetry(creator));
        }

        public void Dispose() {
            Dispose(true);
        }
        
        protected virtual void Dispose(bool disposing) {
            if (_isDisposed) {
                return;
            }
            _isDisposed = true;

            if (disposing) {
                GC.SuppressFinalize(this);
                if (_dispatcher != null) {
                    _dispatcher.BeginInvokeShutdown(DispatcherPriority.Send);
                }
            }
            if (_thread != null && !_thread.Join(1000)) {
                try {
                    _thread.Abort();
                } catch (ThreadStateException) {
                }
            }
        }

        ~WpfProxy() {
            Dispose(false);
        }

        public bool IsDisposed {
            get { return _isDisposed; }
        }

        public void InvokeWithRetry(Action action, DispatcherPriority priority = DispatcherPriority.Normal) {
            for (int retries = 100; retries > 0; --retries) {
                try {
                    _dispatcher.Invoke(action, priority);
                    return;
                } catch (OperationCanceledException) {
                    Thread.Sleep(10);
                }
            }
            throw new OperationCanceledException();
        }

        public T InvokeWithRetry<T>(Func<T> action, DispatcherPriority priority = DispatcherPriority.Normal) {
            for (int retries = 100; retries > 0; --retries) {
                try {
                    return _dispatcher.Invoke(action, priority);
                } catch (OperationCanceledException) {
                    Thread.Sleep(10);
                }
            }
            throw new OperationCanceledException();
        }


        public void Invoke(Action action, DispatcherPriority priority = DispatcherPriority.Normal) {
            _dispatcher.Invoke(action, priority);
        }

        public void Invoke(Action action, DispatcherPriority priority, TimeSpan timeout) {
            _dispatcher.Invoke(action, priority, CancellationToken.None, timeout);
        }

        public T Invoke<T>(Func<T> action, DispatcherPriority priority = DispatcherPriority.Normal) {
            return _dispatcher.Invoke(action, priority);
        }

        public T Invoke<T>(Func<T> action, DispatcherPriority priority, TimeSpan timeout) {
            return _dispatcher.Invoke(action, priority, CancellationToken.None, timeout);
        }

        public async Task InvokeAsync(Action action, DispatcherPriority priority = DispatcherPriority.Normal) {
            await _dispatcher.InvokeAsync(action, priority);
        }

        public async Task<T> InvokeAsync<T>(Func<T> action, DispatcherPriority priority = DispatcherPriority.Normal) {
            return await _dispatcher.InvokeAsync(action, priority);
        }

        public bool CanExecute(RoutedCommand command, object target, object parameter) {
            target = WpfObjectProxy.UnwrapIfProxy(target);
            
            if (!(target is IInputElement)) {
                return false;
            }

            parameter = WpfObjectProxy.UnwrapIfProxy(parameter);
            return Invoke(
                () => command.CanExecute(parameter, (IInputElement)target),
                DispatcherPriority.SystemIdle
            );
        }

        public Task Execute(RoutedCommand command, object target, object parameter) {
            target = WpfObjectProxy.UnwrapIfProxy(target);

            if (!(target is IInputElement)) {
                throw new InvalidOperationException("Cannot execute command on " + ToString());
            }

            parameter = WpfObjectProxy.UnwrapIfProxy(parameter);
            return InvokeAsync(() => command.Execute(parameter, target as IInputElement));
        }
    }

    public class WpfObjectProxy : DynamicObject {
        protected internal readonly WpfProxy _provider;
        protected internal readonly DependencyObject _object;

        internal static object UnwrapIfProxy(object value) {
            var proxy = value as WpfObjectProxy;
            return (proxy != null) ? proxy._object : value;
        }

        protected internal WpfObjectProxy(WpfProxy provider, DependencyObject obj) {
            _provider = provider;
            _object = obj;
        }

        public override string ToString() {
            return string.Format("{0}<{1}>", GetType().Name, _object.GetType().Name);
        }

        public Dispatcher Dispatcher {
            get { return _object.Dispatcher; }
        }

        public U GetValue<U>(DependencyProperty property) {
            if (Dispatcher.CheckAccess()) {
                return (U)_object.GetValue(property);
            } else {
                return Dispatcher.Invoke(
                    () => (U)_object.GetValue(property),
                    DispatcherPriority.SystemIdle
                );
            }
        }

        public void SetValue<U>(DependencyProperty property, U value) {
            Dispatcher.Invoke(() => _object.SetCurrentValue(property, value));
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result) {
            if (base.TryInvokeMember(binder, args, out result)) {
                return true;
            }

            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            if (binder.IgnoreCase) {
                flags |= BindingFlags.IgnoreCase;
            }

            var method = _object.GetType().GetMethod(binder.Name, flags);
            if (method != null) {
                result = method.Invoke(_object, args);
                return true;
            }

            return false;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result) {
            if (base.TryGetMember(binder, out result)) {
                return true;
            }

            var flags = BindingFlags.Static | BindingFlags.Public;
            if (binder.IgnoreCase) {
                flags |= BindingFlags.IgnoreCase;
            }

            var field = _object.GetType().GetField(binder.Name + "Property", flags);
            if (field != null) {
                var dp = field.GetValue(_object) as DependencyProperty;
                if (dp != null) {
                    result = GetValue<object>(dp);
                    return true;
                }
            }

            flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            if (binder.IgnoreCase) {
                flags |= BindingFlags.IgnoreCase;
            }

            field = _object.GetType().GetField(binder.Name, flags);
            if (field != null) {
                result = field.GetValue(_object);
                return true;
            }

            var prop = _object.GetType().GetProperty(binder.Name, flags);
            if (prop != null && prop.CanRead) {
                result = prop.GetValue(_object);
                return true;
            }

            return false;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value) {
            if (base.TrySetMember(binder, value)) {
                return true;
            }

            var flags = BindingFlags.Static | BindingFlags.Public;
            if (binder.IgnoreCase) {
                flags |= BindingFlags.IgnoreCase;
            }

            var field = _object.GetType().GetField(binder.Name + "Property", flags);
            if (field != null) {
                var dp = field.GetValue(_object) as DependencyProperty;
                if (dp != null) {
                    SetValue(dp, value);
                    return true;
                }
            }

            flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            if (binder.IgnoreCase) {
                flags |= BindingFlags.IgnoreCase;
            }

            field = _object.GetType().GetField(binder.Name, flags);
            if (field != null && !field.IsInitOnly) {
                field.SetValue(_object, value);
                return true;
            }

            var prop = _object.GetType().GetProperty(binder.Name, flags);
            if (prop != null && prop.CanWrite) {
                prop.SetValue(_object, value);
                return true;
            }

            return false;
        }
    }

    public class WpfObjectProxy<T> : WpfObjectProxy where T : DependencyObject {
        private static DependencyObject Unwrap(dynamic obj) {
            return (T)WpfObjectProxy.UnwrapIfProxy(obj);
        }
        
        public WpfObjectProxy(WpfProxy provider, object obj)
            : base(provider, Unwrap(obj)) {
        }

        public T Object {
            get { return (T)_object; }
        }
    }

}
