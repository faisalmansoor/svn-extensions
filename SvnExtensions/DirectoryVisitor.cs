using System;
using System.IO;

namespace SvnExtensions
{
    public class DirectoryVisitor
    {
        private readonly Func<string, bool> _process;
        public DirectoryVisitor(Func<string, bool> process)
        {
            _process = process;
        }

        public void Visit(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            if (!_process(path))
            {
                return;
            }
            
            var children = Directory.EnumerateDirectories(path);

            foreach (string child in children)
            {
                Visit(child);
            }
        }
    }
}
