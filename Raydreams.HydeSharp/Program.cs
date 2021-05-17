using System;
using System.IO;
using System.Linq;
using Markdig;
using Raydreams.Common.IO;

namespace Raydreams.HydeSharp
{
    /// <summary>Main EXE class</summary>
    public class Hyde
    {
        public static readonly string SiteFolder = "_site";
        public static readonly string PostsFolder = "_posts";
        public static readonly string LayoutsFolder = "_layouts";

        /// <summary>Base path of the project</summary>
        private string _basePath;

        /// <summary></summary>
        static void Main(string[] args)
        {
            Hyde app = new Hyde( $"{IOHelpers.DesktopPath}/website/" );
            bool res = app.Process();

            Console.WriteLine( $"Sucess = {res}" );
        }

        /// <summary></summary>
        public Hyde(string basePath)
        {
            this._basePath = basePath;
        }

        /// <summary>Lets do it</summary>
        public bool Process()
        {
            try
            {
                string template = this.GetTemplate();
                string[] posts = this.GetFilePaths();
                this.TransformPosts(template, posts);
            }
            catch (System.Exception exp)
            {
                Console.WriteLine(exp.Message);
                return false;
            }

            return true;
        }

        /// <summary></summary>
        public string BasePath => this._basePath;

        public void TransformPages(string template, string[] posts)
        {

        }

        /// <summary>Transforms all posts to HTML</summary>
        public void TransformPosts(string template, string[] posts)
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

                string post = template.Replace( @"{{ content }}", html);

                string baseName = Path.GetFileNameWithoutExtension(file);

                File.WriteAllText( $"{this.BasePath}/{SiteFolder}/{baseName}.html", post);
            }
        }

        /// <summary>Gets all the posts file paths</summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string[] GetFilePaths()
        {
            string[] files = Directory.EnumerateFiles( $"{this.BasePath}/{PostsFolder}", "*.md", SearchOption.TopDirectoryOnly).ToArray();

            return files;
        }

        /// <summary>Gets a template</summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string GetTemplate(string name = "default")
        {
            if (String.IsNullOrWhiteSpace(name))
                name = "default";

            return File.ReadAllText( $"{this.BasePath}/{LayoutsFolder}/{name.Trim()}.html" );
        }

    }
}
