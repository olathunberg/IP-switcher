using System;
using System.Collections.Generic;
using System.Reflection;

namespace TTech.IP_Switcher.Helpers
{
    public class SimpleMessenger
    {
        private static readonly object creationLock = new();
        private static SimpleMessenger defaultInstance;
        readonly MessageToActionsMap _messageToActionsMap = new();
        private readonly object registerLock = new();

        public static SimpleMessenger Default
        {
            get
            {
                if (defaultInstance == null)
                {
                    lock (creationLock)
                    {
                        if (defaultInstance == null)
                            defaultInstance = new SimpleMessenger();
                    }
                }

                return defaultInstance;
            }
        }

        public void Register(string message, Action callback) => Register(message, callback, null);

        public void Register<T>(string message, Action<T> callback) => Register(message, callback, typeof(T));

        /// <summary>
        /// Notifies all registered parties that a message is being broadcasted.
        /// </summary>
        /// <param name="message">The message to broadcast.</param>
        /// <param name="parameter">The parameter to pass together with the message.</param>
        public void SendMessage(string message, object parameter)
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentException("'message' cannot be null or empty.");
            if (_messageToActionsMap.TryGetParameterType(message, out Type registeredParameterType) && registeredParameterType == null)
                throw new TargetParameterCountException(string.Format("Cannot pass a parameter with message '{0}'. Registered action(s) expect no parameter.", message));

            var actions = _messageToActionsMap.GetActions(message);
            if (actions != null)
                actions.ForEach(action => action.DynamicInvoke(parameter));
        }

        public void SendMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentException("'message' cannot be null or empty.");

            if (_messageToActionsMap.TryGetParameterType(message, out Type registeredParameterType) && registeredParameterType != null)
                throw new TargetParameterCountException(string.Format("Must pass a parameter of type {0} with this message. Registered action(s) expect it.", registeredParameterType.FullName));

            var actions = _messageToActionsMap.GetActions(message);
            if (actions != null)
                actions.ForEach(action => action.DynamicInvoke());
        }

        private void Register(string message, Delegate callback, Type parameterType)
        {
            lock (registerLock)
            {
                if (string.IsNullOrEmpty(message))
                    throw new ArgumentException("'message' cannot be null or empty.");

                if (callback == null)
                    throw new ArgumentNullException(nameof(callback));

                this.VerifyParameterType(message, parameterType);

                _messageToActionsMap.AddAction(message, callback.Target, callback.Method, parameterType);
            }
        }

        private void VerifyParameterType(string message, Type parameterType)
        {
            if (_messageToActionsMap.TryGetParameterType(message, out Type previouslyRegisteredParameterType))
            {
                if (previouslyRegisteredParameterType != null && parameterType != null)
                {
                    if (!previouslyRegisteredParameterType.Equals(parameterType))
                        throw new InvalidOperationException(string.Format(
                            "The registered action's parameter type is inconsistent with the previously registered actions for message '{0}'.\nExpected: {1}\nAdding: {2}",
                            message,
                            previouslyRegisteredParameterType.FullName,
                            parameterType.FullName));
                }
                else
                {
                    // One, or both, of previouslyRegisteredParameterType or callbackParameterType are null.
                    if (previouslyRegisteredParameterType != parameterType)   // not both null?
                    {
                        throw new TargetParameterCountException(string.Format(
                            "The registered action has a number of parameters inconsistent with the previously registered actions for message \"{0}\".\nExpected: {1}\nAdding: {2}",
                            message,
                            previouslyRegisteredParameterType == null ? 0 : 1,
                            parameterType == null ? 0 : 1));
                    }
                }
            }
        }
        private sealed class MessageToActionsMap
        {
            // Stores a hash where the key is the message and the value is the list of callbacks to invoke.
            readonly Dictionary<string, List<WeakAction>> _map = [];

            internal void AddAction(string message, object target, MethodInfo method, Type actionType)
            {
                if (message == null)
                    throw new ArgumentNullException(nameof(message));

                if (method == null)
                    throw new ArgumentNullException(nameof(method));

                lock (_map)
                {
                    if (!_map.ContainsKey(message))
                        _map[message] = [];

                    _map[message].Add(new WeakAction(target, method, actionType));
                }
            }

            internal List<Delegate> GetActions(string message)
            {
                if (message == null)
                    throw new ArgumentNullException(nameof(message));

                List<Delegate> actions;
                lock (_map)
                {
                    if (!_map.TryGetValue(message, out List<WeakAction> weakActions))
                        return [];

                    actions = new List<Delegate>(weakActions.Count);
                    for (int i = weakActions.Count - 1; i > -1; --i)
                    {
                        WeakAction weakAction = weakActions[i];
                        if (weakAction == null)
                            continue;

                        var action = weakAction.CreateAction();
                        if (action != null)
                        {
                            actions.Add(action);
                        }
                        else
                        {
                            // The target object is dead, so get rid of the weak action.
                            weakActions.Remove(weakAction);
                        }
                    }

                    // Delete the list from the map if it is now empty.
                    if (weakActions.Count == 0)
                        _map.Remove(message);
                }

                // Reverse the list to ensure the callbacks are invoked in the order they were registered.
                actions.Reverse();

                return actions;
            }

            internal bool TryGetParameterType(string message, out Type parameterType)
            {
                if (message == null)
                    throw new ArgumentNullException(nameof(message));

                parameterType = null;
                List<WeakAction> weakActions;
                lock (_map)
                {
                    if (!_map.TryGetValue(message, out weakActions) || weakActions.Count == 0)
                        return false;
                }
                parameterType = weakActions[0].ParameterType;
                return true;
            }
        }

        private sealed class WeakAction
        {
            internal readonly Type ParameterType;

            readonly Type _delegateType;

            readonly MethodInfo _method;

            readonly WeakReference _targetRef;

            internal WeakAction(object target, MethodInfo method, Type parameterType)
            {
                if (target == null)
                {
                    _targetRef = null;
                }
                else
                {
                    _targetRef = new WeakReference(target);
                }

                _method = method;

                this.ParameterType = parameterType;

                if (parameterType == null)
                {
                    _delegateType = typeof(Action);
                }
                else
                {
                    _delegateType = typeof(Action<>).MakeGenericType(parameterType);
                }
            }

            internal Delegate CreateAction()
            {
                // Rehydrate into a real Action object, so that the method can be invoked.
                if (_targetRef == null)
                {
                    return Delegate.CreateDelegate(_delegateType, _method);
                }
                else
                {
                    try
                    {
                        object target = _targetRef.Target;
                        if (target != null)
                            return Delegate.CreateDelegate(_delegateType, target, _method);
                    }
                    catch
                    {
                        // Intentionally left blank
                    }
                }

                return null;
            }
        }
    }
}