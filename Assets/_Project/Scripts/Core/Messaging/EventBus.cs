using System;
using System.Collections.Generic;

namespace VertigoDemo.Messaging
{
    public sealed class EventBus : IEventBus
    {
        #region Fields

        private readonly Dictionary<Type, Delegate> _handlers = new Dictionary<Type, Delegate>();

        #endregion

        #region Public Methods

        public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : struct
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            Type eventType = typeof(TEvent);
            _handlers.TryGetValue(eventType, out Delegate existingHandlers);
            _handlers[eventType] = Delegate.Combine(existingHandlers, handler);
        }

        public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : struct
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            Type eventType = typeof(TEvent);

            if (!_handlers.TryGetValue(eventType, out Delegate existingHandlers))
            {
                return;
            }

            Delegate remainingHandlers = Delegate.Remove(existingHandlers, handler);
            if (remainingHandlers == null)
            {
                _handlers.Remove(eventType);
                return;
            }

            _handlers[eventType] = remainingHandlers;
        }

        public void Publish<TEvent>(TEvent message) where TEvent : struct
        {
            if (!_handlers.TryGetValue(typeof(TEvent), out Delegate handlers))
            {
                return;
            }

            Action<TEvent> typedHandlers = (Action<TEvent>)handlers;
            typedHandlers.Invoke(message);
        }

        #endregion
    }
}
