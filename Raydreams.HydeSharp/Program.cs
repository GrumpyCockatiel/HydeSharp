using System;
using System.IO;
using System.Linq;
using Markdig;
using Raydreams.Common.IO;

namespace Raydreams.HydeSharp
{
    /// <summary></summary>
    public class Hyde
    {
        /// <summary></summary>
        private string _basePath;

        /// <summary></summary>
        static void Main(string[] args)
        {
            Hyde app = new Hyde($"{IOHelpers.DesktopPath}/website/");

            string template = app.GetTemplate();
            string[] posts = app.GetFilePaths();
            app.WritePosts(template, posts);

            Console.WriteLine("Done!");
        }

        /// <summary></summary>
        public Hyde(string basePath)
        {
            this._basePath = basePath;
        }

        /// <summary></summary>
        public string BasePath => this._basePath;

        /// <summary></summary>
        public void WritePosts(string template, string[] posts)
        {
            MarkdownPipeline pipe = new MarkdownPipelineBuilder().UseSoftlineBreakAsHardlineBreak().Build();

            foreach (string file in posts)
            {
                FileInfo info = new FileInfo(file);

                if (!info.Exists)
                    continue;

                // get the markdown
                string original = File.ReadAllText(file);
                string html = Markdown.ToHtml(original, pipe);

                string post = template.Replace(@"{{ content }}", html);

                string baseName = Path.GetFileNameWithoutExtension(file);

                File.WriteAllText($"{this.BasePath}site/{baseName}.html", post);
            }
        }

        /// <summary></summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string[] GetFilePaths()
        {
            string[] files = Directory.EnumerateFiles(this.BasePath + "posts", "*.md", SearchOption.TopDirectoryOnly).ToArray();

            return files;
        }

        /// <summary></summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string GetTemplate()
        {
            return File.ReadAllText(this.BasePath + "_template.html");
        }

    }
}
