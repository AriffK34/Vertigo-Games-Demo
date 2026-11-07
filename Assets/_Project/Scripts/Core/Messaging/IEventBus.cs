using System;

namespace VertigoDemo.Messaging
{
    public interface IEventBus
    {
        #region Public Methods

        void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : struct;

        void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : struct;

        void Publish<TEvent>(TEvent message) where TEvent : struct;

        #endregion
    }
}
