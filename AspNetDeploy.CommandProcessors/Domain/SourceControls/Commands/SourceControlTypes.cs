using System.Collections.Generic;

namespace AspNetDeploy.CommandProcessors.Domain.SourceControls.Commands
{
    public class SourceControlTypes : AppCommandProcessor
    {
        public override string CommandName
        {
            get
            {
                return "App/SourceControls/Types";
            }
        }

        public override void Process()
        {
            this.TransmitConnection(
                "App/SourceControls/Types",
                new List<object>()
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
                });
        }
    }
}