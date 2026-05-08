using System;
using System.Collections.Generic;

namespace StreetFighter.Core
{
    /// <summary>
    /// Central event dispatcher for decoupled runtime systems.
    /// </summary>
    public class EventBus : IEventBus
    {
        private readonly Dictionary<Type, List<Delegate>> eventHandlers = new();

        /// <inheritdoc />
        public void Subscribe<TEvent>(Action<TEvent> listener)
        {
            if (listener == null)
            {
                throw new ArgumentNullException(nameof(listener));
            }

            var eventType = typeof(TEvent);
            if (!eventHandlers.TryGetValue(eventType, out var handlers))
            {
                handlers = new List<Delegate>();
                eventHandlers[eventType] = handlers;
            }

            handlers.Add(listener);
        }

        /// <inheritdoc />
        public void Unsubscribe<TEvent>(Action<TEvent> listener)
        {
            if (listener == null)
            {
                return;
            }

            var eventType = typeof(TEvent);
            if (!eventHandlers.TryGetValue(eventType, out var handlers))
            {
                return;
            }

            handlers.Remove(listener);
        }

        /// <inheritdoc />
        public void Publish<TEvent>(TEvent eventData)
        {
            if (eventHandlers.TryGetValue(typeof(TEvent), out var handlers))
            {
                foreach (Action<TEvent> callback in handlers.ToArray())
                {
                    callback?.Invoke(eventData);
                }
            }
        }
    }

    /// <summary>
    /// Interface for event bus implementations.
    /// </summary>
    public interface IEventBus
    {
        void Subscribe<TEvent>(Action<TEvent> listener);
        void Unsubscribe<TEvent>(Action<TEvent> listener);
        void Publish<TEvent>(TEvent eventData);
    }
}
