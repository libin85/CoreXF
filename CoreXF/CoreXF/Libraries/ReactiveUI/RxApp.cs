
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System;
using System.Reactive;
using System.Reactive.Concurrency;
using CoreXF;

namespace ReactiveUI
{
    public class RxApp
    {
        public static List<ICreatesObservableForProperty> ICreatesObservableForProperty = new List<ICreatesObservableForProperty> { new INPCObservableForProperty() };

        /// <summary>
        /// The size of a small cache of items. Often used for the MemoizingMRUCache class.
        /// </summary>
        public const int SmallCacheLimit = 32;

        /// <summary>
        /// The size of a large cache of items. Often used for the MemoizingMRUCache class.
        /// </summary>
        public const int BigCacheLimit = 64;

        /// <summary>
        /// This Observer is signalled whenever an object that has a
        /// ThrownExceptions property doesn't Subscribe to that Observable. Use
        /// Observer.Create to set up what will happen - the default is to crash
        /// the application with an error message.
        /// </summary>
        public static IObserver<Exception> DefaultExceptionHandler
        {
            get => _defaultExceptionHandler;
            set => _defaultExceptionHandler = value;
        }
        private static IObserver<Exception> _defaultExceptionHandler;

        [MethodImpl(MethodImplOptions.NoOptimization)]
        internal static void EnsureInitialized()
        {
            // NB: This method only exists to invoke the static constructor
        }

        /// <summary>
        /// MainThreadScheduler is the scheduler used to schedule work items that
        /// should be run "on the UI thread". In normal mode, this will be
        /// DispatcherScheduler, and in Unit Test mode this will be Immediate,
        /// to simplify writing common unit tests.
        /// </summary>
        public static IScheduler MainThreadScheduler
        {
            get
            {
               return  _mainThreadScheduler;
            }
            set
            {
                _mainThreadScheduler = value;
            }
        }
        private static IScheduler _mainThreadScheduler;

        static RxApp()
        {
            DefaultExceptionHandler = Observer.Create<Exception>(ex =>
            {
                MainThreadScheduler.Schedule(() =>
                {
                    ExceptionManager.SendError(ex, "FromRX", deduplicate: true);
                    return;

#pragma warning disable CA1065 // Avoid exceptions in constructors -- In scheduler.
                    throw new UnhandledErrorException(
                        "An object implementing IHandleObservableErrors (often a ReactiveCommand or ObservableAsPropertyHelper) has errored, thereby breaking its observable pipeline. To prevent this, ensure the pipeline does not error, or Subscribe to the ThrownExceptions property of the object in question to handle the erroneous case.",
                        ex);
#pragma warning restore CA1065
                });
            });

            if (_mainThreadScheduler == null)
            {
                _mainThreadScheduler = DefaultScheduler.Instance;
            }


        }

    }
}
