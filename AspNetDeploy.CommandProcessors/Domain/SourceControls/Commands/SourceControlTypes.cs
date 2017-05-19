using System;
using System.Collections.Generic;
using AspNetDeploy.Notifications;
using AspNetDeploy.Notifications.Model;
using EventHandlers;

namespace AspNetDeploy.CommandProcessors.Domain.SourceControls.Commands
{
    public class SourceControlTypes : IAppCommandProcessor
    {
        public string CommandName
        {
            get
            {
                return "App/SourceControls/Types";
            }
        }

        public void Process(AppCommand message)
        {
            Guid userGuid = message.UserGuid;

            EventsHub.TransmitApp.InvokeSafe(new AppConnectionResponse()
            {
                ConnectionId = message.ConnectionId,
                Name = "App/SourceControls/Types",
                Data = new List<object>()
                {
                    new
                    {
                        type = "svn",
                        display = "SVN",
                        fields = new List<object>()
                        {
                            new
                            {
                                type = "string",
                                key = "url",
                                display = "URL",
                                hint = "Enter the link to the root of your project repository"
                            },
                            new
                            {
                                type = "string",
                                key = "login",
                                display = "Login"
                            },
                            new
                            {
                                type = "string",
                                key = "password",
                                display = "Password"
                            }
                        }
                    },
                    new
                    {
                        type = "fileSystem",
                        display = "File System",
                        fields = new List<object>()
                        {
                            new
                            {
                                type = "string",
                                key = "path",
                                display = "Path",
                            },
                            new
                            {
                                type = "select",
                                key = "mode",
                                display = "Mode",
                                options = new List<object>()
                                {
                                    new
                                    {
                                        key = "relative",
                                        diplsay = "Relative"
                                    },
                                    new
                                    {
                                        key = "absolute",
                                        diplsay = "Absolute"
                                    }
                                }
                            }
                        }
                    }
                }
            });
        }
    }
}