using System;

namespace EventHandlers
{
    public static class EventHandlerExtender
    {
        public static void InvokeSafe<T>(this EventHandler<T> eventHandler, T message)
        {
            if (eventHandler == null)
            {
                return;
            }

            eventHandler.Invoke(null, message);
        }
    }
}