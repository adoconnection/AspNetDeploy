using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvnHelper
{
    public class Class1
    {
        /*
 
         IList<SourceControlInfo> sourceControlInfos = new List<SourceControlInfo>();

                    foreach (SourceControl sourceControl in sourceControls)
                    {
                        using (SvnClient client = new SvnClient())
                        {
                            NetworkCredential credential = new NetworkCredential(
                                sourceControl.GetStringProperty("Login"),
                                sourceControl.GetStringProperty("Password"));

                            client.Authentication.DefaultCredentials = credential;

                            string path = @"H:\AspNetDeployWorkingFolder\Sources\" + sourceControl.Id;

                            if (!Directory.Exists(path))
                            {
                                client.CheckOut(new Uri(sourceControl.GetStringProperty("URL") + "/trunk"), path);    
                            }
                            else
                            {
                                client.Update(path);
                            }
                    
                            VisualStudioSolutionParser parser = new VisualStudioSolutionParser();

                            string[] solutions = Directory.GetFiles(path, "*.sln");

                            IList<VisualStudioSolution> visualStudioSolutions = solutions.Select(parser.Parse).ToList();

                            sourceControlInfos.Add(new SourceControlInfo()
                            {
                                SourceControl = sourceControl,
                                Solutions = visualStudioSolutions
                            });

                        }
                    }

         */
    }
}
