using System;
using System.Collections.Generic;
using System.Linq;
using AspNetDeploy.Notifications;
using AspNetDeploy.Notifications.Model;
using AspNetDeploy.WebUI.Hubs;
using Microsoft.AspNet.SignalR;

namespace AspNetDeploy.WebUI
{
    public class EventsConfig
    {
        public static void Subscribe()
        {
            EventsHub.TransmitApp += (sender, response) => TransmitApp(response);
        }

        private static void TransmitApp(AppResponse response)
        {
            if (response is AppConnectionResponse)
            {
                TransmitApp(response as AppConnectionResponse);
            }
            else if (response is AppUsersResponse)
            {
                TransmitApp(response as AppUsersResponse);
            }
        }

        private static void TransmitApp(AppConnectionResponse response)
        {
            IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<AppHub>();

            hubContext.Clients.Clients(new List<string> { response.ConnectionId }).receive(new
            {
                name = response.Name,
                data = response.Data
            });
        }

        private static void TransmitApp(AppUsersResponse response)
        {
            IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<AppHub>();

            foreach (Guid guid in response.UserGuids)
            {
                HashSet<string> connections = AppHub.ConnectedUsers.GetOrAdd(guid.ToString(), _ => new HashSet<string>());

                hubContext.Clients.Clients(connections.ToList()).receive(new
                {
                    name = response.Name,
                    data = response.Data
                });
            }
        }
    }
}