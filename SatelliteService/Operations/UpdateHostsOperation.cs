using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using SatelliteService.Contracts;

namespace SatelliteService.Operations
{
    public class UpdateHostsOperation : Operation
    {
        private Regex lineParseRegex = new Regex(@"^(?<IP>[^\s]+)\s+(?<Domain>.+)", RegexOptions.Compiled | RegexOptions.Singleline);

        private dynamic configuration;
        private Guid? backupFileGuid = null;

        public UpdateHostsOperation(IBackupRepository backupRepository) : base(backupRepository)
        {
        }

        public void Configure(dynamic configuration, IDictionary<string, object> variables)
        {
            this.configuration = configuration;
            base.Configure(variables);
        }

        public override void Run()
        {
            const string hostsPath = @"C:\Windows\System32\drivers\etc\hosts";

            this.backupFileGuid = this.BackupRepository.StoreFile(hostsPath);

            IList<string> lines = File.ReadLines(hostsPath).ToList();
            IList<string> appendLines = new List<string>();

            if (this.configuration.add != null)
            {
                foreach (dynamic line in this.configuration.add)
                {
                    this.AppendLineIfRequred(lines, appendLines, (string) line.ip, (string) line.domain);
                }
            }

            if (appendLines.Count > 0)
            {
                using (FileStream fileStream = new FileStream(hostsPath, FileMode.Append, FileAccess.Write))
                {
                    using (StreamWriter streamWriter = new StreamWriter(fileStream))
                    {
                        streamWriter.WriteLine("");
                        streamWriter.WriteLine("###############################");
                        streamWriter.WriteLine("");

                        foreach (string line in appendLines)
                        {
                            streamWriter.WriteLine(line);
                        }

                        streamWriter.Flush();
                    }
                }
            }
        }

        public override void Rollback()
        {
            if (this.backupFileGuid.HasValue)
            {
                this.BackupRepository.RestoreFile(this.backupFileGuid.Value);
            }
        }

        private void AppendLineIfRequred(IEnumerable<string> lines, IList<string> appendLines, string ip, string domain)
        {
            if (lines.All(line => this.ParseHostsLine(line).Value.ToLower() != domain))
            {
                appendLines.Add(ip + "	" + domain);
            }
        }

        private KeyValuePair<string, string> ParseHostsLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                return new KeyValuePair<string, string>(string.Empty, string.Empty);
            }

            Match match = lineParseRegex.Match(line);

            if (!match.Success)
            {
                return new KeyValuePair<string, string>(string.Empty, string.Empty);
            }

            return new KeyValuePair<string, string>(match.Groups["IP"].Value, match.Groups["Domain"].Value);
        }
    }
}