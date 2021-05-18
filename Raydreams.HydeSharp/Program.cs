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

        /// <summary>The config file should be specified on the CL but we'll set a default</summary>
        private string _configFile = "_config.yml";

        /// <summary>The Markdown pipeline</summary>
        private MarkdownPipeline _pipe = null;

        /// <summary></summary>
        /// <param name="args">Path to website source</param>
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

            this._pipe = new MarkdownPipelineBuilder().UseSoftlineBreakAsHardlineBreak().Build();
        }

        /// <summary>Lets do it</summary>
        public bool Process()
        {
            try
            {
                string template = this.GetTemplate();
                this.TransformPages( template );
                this.TransformPosts(template);
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

        /// <summary></summary>
        public MarkdownPipeline MDPipeline => this._pipe;

        /// <summary></summary>
        public void TransformPages(string template)
        {
            string[] pages = Directory.EnumerateFiles( $"{this.BasePath}/", "*.md", SearchOption.TopDirectoryOnly ).ToArray();

            foreach ( string file in pages )
            {
                FileInfo info = new FileInfo( file );

                if ( !info.Exists )
                    continue;

                // get the markdown
                string original = File.ReadAllText( file );
                string html = Markdown.ToHtml( original, this.MDPipeline );

                string post = template.Replace( @"{{ content }}", html );

                string baseName = Path.GetFileNameWithoutExtension( file );

                File.WriteAllText( $"{this.BasePath}/{SiteFolder}/{baseName}.html", post );
            }
        }

        /// <summary>Transforms all posts to HTML</summary>
        public void TransformPosts(string template)
        {
            string[] posts = Directory.EnumerateFiles( $"{this.BasePath}/{PostsFolder}", "*.md", SearchOption.TopDirectoryOnly ).ToArray();

            foreach (string file in posts)
            {
                FileInfo info = new FileInfo(file);

                if (!info.Exists)
                    continue;

                Post meta = file.TryParseFilename();

                if ( meta == null )
                    continue;

                // get the markdown
                string original = File.ReadAllText(file);
                string html = Markdown.ToHtml(original, this.MDPipeline);

                string post = template.Replace( @"{{ content }}", html);

                //string baseName = Path.GetFileNameWithoutExtension(file);

                File.WriteAllText( $"{this.BasePath}/{SiteFolder}/{meta.Name}.html", post);
            }
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
