﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;

namespace ReactiveUI
{
    /// <summary>
    /// Extension methods associated with the ReactiveCommand class.
    /// </summary>
    public static class ReactiveCommandMixins
    {
        /// <summary>
        /// A utility method that will pipe an Observable to an ICommand (i.e.
        /// it will first call its CanExecute with the provided value, then if
        /// the command can be executed, Execute() will be called).
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="this">The source observable to pipe into the command.</param>
        /// <param name="command">The command to be executed.</param>
        /// <returns>An object that when disposes, disconnects the Observable
        /// from the command.</returns>
        public static IDisposable InvokeCommand<T>(this IObservable<T> @this, ICommand command)
        {
            var canExecuteChanged = Observable
                .FromEventPattern(h => command.CanExecuteChanged += h, h => command.CanExecuteChanged -= h)
                .Select(_ => Unit.Default)
                .StartWith(Unit.Default);

            return WithLatestFromFixed(@this, canExecuteChanged, (value, _) => InvokeCommandInfo.From(command, command.CanExecute(value), value))
                .Where(ii => ii.CanExecute)
                .Do(ii => command.Execute(ii.Value))
                .Subscribe();
        }

        /// <summary>
        /// A utility method that will pipe an Observable to an ICommand (i.e.
        /// it will first call its CanExecute with the provided value, then if
        /// the command can be executed, Execute() will be called).
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="this">The source observable to pipe into the command.</param>
        /// <param name="command">The command to be executed.</param>
        /// <returns>An object that when disposes, disconnects the Observable
        /// from the command.</returns>
        public static IDisposable InvokeCommand<T, TResult>(this IObservable<T> @this, ReactiveCommandBase<T, TResult> command)
        {
            return WithLatestFromFixed(@this, command.CanExecute, (value, canExecute) => InvokeCommandInfo.From(command, canExecute, value))
                .Where(ii => ii.CanExecute)
                .SelectMany(ii => command.Execute(ii.Value).Catch(Observable<TResult>.Empty))
                .Subscribe();
        }

        /// <summary>
        /// A utility method that will pipe an Observable to an ICommand (i.e.
        /// it will first call its CanExecute with the provided value, then if
        /// the command can be executed, Execute() will be called).
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <typeparam name="TTarget">The target type.</typeparam>
        /// <param name="this">The source observable to pipe into the command.</param>
        /// <param name="target">The root object which has the Command.</param>
        /// <param name="commandProperty">The expression to reference the Command.</param>
        /// <returns>An object that when disposes, disconnects the Observable
        /// from the command.</returns>
        public static IDisposable InvokeCommand<T, TTarget>(this IObservable<T> @this, TTarget target, Expression<Func<TTarget, ICommand>> commandProperty)
            where TTarget : class
        {
            var command = target.WhenAnyValue(commandProperty);
            var commandCanExecuteChanged = command
                .Select(c => c == null ? Observable<ICommand>.Empty : Observable
                    .FromEventPattern(h => c.CanExecuteChanged += h, h => c.CanExecuteChanged -= h)
                    .Select(_ => c)
                    .StartWith(c))
                .Switch();

            return WithLatestFromFixed(@this, commandCanExecuteChanged, (value, cmd) => InvokeCommandInfo.From(cmd, cmd.CanExecute(value), value))
                .Where(ii => ii.CanExecute)
                .Do(ii => ii.Command.Execute(ii.Value))
                .Subscribe();
        }

        /// <summary>
        /// A utility method that will pipe an Observable to an ICommand (i.e.
        /// it will first call its CanExecute with the provided value, then if
        /// the command can be executed, Execute() will be called).
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <typeparam name="TTarget">The target type.</typeparam>
        /// <param name="this">The source observable to pipe into the command.</param>
        /// <param name="target">The root object which has the Command.</param>
        /// <param name="commandProperty">The expression to reference the Command.</param>
        /// <returns>An object that when disposes, disconnects the Observable
        /// from the command.</returns>
        public static IDisposable InvokeCommand<T, TResult, TTarget>(this IObservable<T> @this, TTarget target, Expression<Func<TTarget, ReactiveCommandBase<T, TResult>>> commandProperty)
            where TTarget : class
        {
            var command = target.WhenAnyValue(commandProperty);
            var invocationInfo = command
                .Select(cmd => cmd == null ? Observable<InvokeCommandInfo<ReactiveCommandBase<T, TResult>, T>>.Empty : cmd
                    .CanExecute
                    .Select(canExecute => InvokeCommandInfo.From(cmd, canExecute, default(T))))
                .Switch();

            return WithLatestFromFixed(@this, invocationInfo, (value, ii) => ii.WithValue(value))
                .Where(ii => ii.CanExecute)
                .SelectMany(ii => ii.Command.Execute(ii.Value).Catch(Observable<TResult>.Empty))
                .Subscribe();
        }

        // See https://github.com/Reactive-Extensions/Rx.NET/issues/444
        private static IObservable<TResult> WithLatestFromFixed<TLeft, TRight, TResult>(
            IObservable<TLeft> @this,
            IObservable<TRight> other,
            Func<TLeft, TRight, TResult> resultSelector) =>
            @this
                .Publish(
                    os =>
                            other
                                .Select(
                                    a =>
                                        os
                                            .Select(b => resultSelector(b, a)))
                                .Switch());

        private struct InvokeCommandInfo<TCommand, TValue>
        {
            private readonly TCommand _command;
            private readonly bool _canExecute;
            private readonly TValue _value;

            public InvokeCommandInfo(TCommand command, bool canExecute)
                : this(command, canExecute, default(TValue))
            {
            }

            public InvokeCommandInfo(TCommand command, bool canExecute, TValue value)
            {
                _command = command;
                _canExecute = canExecute;
                _value = value;
            }

            public TCommand Command => _command;

            public bool CanExecute => _canExecute;

            public TValue Value => _value;

            public InvokeCommandInfo<TCommand, TValue> WithValue(TValue value) =>
                new InvokeCommandInfo<TCommand, TValue>(_command, _canExecute, value);
        }

        private static class InvokeCommandInfo
        {
            public static InvokeCommandInfo<TCommand, TValue> From<TCommand, TValue>(TCommand command, bool canExecute, TValue value) =>
                new InvokeCommandInfo<TCommand, TValue>(command, canExecute, value);
        }
    }
}
