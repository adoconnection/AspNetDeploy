using System;
using AspNetDeploy.Notifications.Model;

namespace AspNetDeploy.Notifications
{
    public class EventsHub
    {
        public static EventHandler<AppResponse> TransmitApp;
    }
}