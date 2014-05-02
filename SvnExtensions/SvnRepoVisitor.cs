using System;
using System.Collections.ObjectModel;
using System.Linq;
using SharpSvn;

namespace SvnExtensions
{
    public class SvnRepoVisitor
    {
        private readonly Func<Uri, SvnClient, bool> _process;
        private readonly SvnClient _svnClient;
        public SvnRepoVisitor(Func<Uri, SvnClient, bool> process)
        {
            _process = process;
            _svnClient = new SvnClient();
        }

        public void Visit(string svnuri)
        {
            Visit(new Uri(svnuri));
        }

        public void Visit(Uri svnuri)
        {
            if (string.IsNullOrWhiteSpace(svnuri.AbsolutePath))
            {
                return;
            }

            if (!_process(svnuri, _svnClient))
            {
                return;
            }

            Collection<SvnListEventArgs> list;
            if (!_svnClient.GetList(svnuri, out list))
            {
                Console.WriteLine("ERROR: Failed to GetList for {0}", svnuri);
            }

            var children = list.Where(c => !string.IsNullOrWhiteSpace(c.Path));

            foreach (SvnListEventArgs child in children)
            {
                Visit(child.Uri);
            }
        }
    }
}
