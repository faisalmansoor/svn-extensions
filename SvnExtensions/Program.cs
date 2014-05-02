using System;
using System.IO;
using SharpSvn;

namespace SvnExtensions
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("USAGE: SvnExtensions.exe Path");
                Console.WriteLine("DESCRIPTION:");
                Console.WriteLine("\t If Path is a local directory, all the svn roots under PATH will be updated");
                Console.WriteLine("\t If path is a svn repo url, all the trunks under the path will be checked out to current directory.");
                return;
            }

            string path = args[0];

            if (Directory.Exists(path))
            {
                UpdateRecursively(path);
                return;
            }

            var svnVisitor = new SvnRepoVisitor(ProcessSvnPath);
            svnVisitor.Visit(path);
        }

        #region Update

        private static void UpdateRecursively(string path)
        {
            Console.WriteLine("Searching svn roots ...");
            var roots = Directory.GetDirectories(path, "*.svn", SearchOption.AllDirectories);
            Console.WriteLine("Updating {0} svn roots under {1}.", roots.Length, path);
            foreach (var root in roots)
            {
                Update(Path.GetDirectoryName(root));
            }
            return;
        }

        private static void Update(string path)
        {
            if (!Directory.Exists(Path.Combine(path, ".svn")))
            {
                return;
            }

            try
            {
                Console.WriteLine("Updating: {0}", path);
                var client = new SvnClient();
                SvnUpdateResult result;
                if (!client.Update(path, out result))
                {
                    Console.WriteLine(result.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed: {0}", ex.Message);
            }
        }

        #endregion

        #region Checkout

        private static bool ProcessSvnPath(Uri uri, SvnClient client)
        {
            if (EndWith(uri, "branches") || EndWith(uri, "tags"))
            {
                return false;
            }

            if (EndWith(uri, "trunk"))
            {
                Checkout(client, uri);
                return false;
            }

            return true;
        }

        private static void Checkout(SvnClient client, Uri svnuri)
        {
            Console.WriteLine("Checking out {0}", svnuri);
            string localPath = Path.Combine(Environment.CurrentDirectory, svnuri.LocalPath.TrimStart(new[] { '/' }));
            client.CheckOut(svnuri, localPath);
        }

        private static bool EndWith(Uri svnuri, string target)
        {
            string dirName = svnuri.AbsolutePath.TrimEnd(new char[] { '/' });
            return dirName.EndsWith(target, StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }
}