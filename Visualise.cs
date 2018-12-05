using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Resources;

namespace ArchiveMe
{
    class Visualise
    {
        public Visualise(string board, string thread_id)
        {
            Database database = new Database(thread_id, board);
            OPPost op_p = database.GetOPPOst();
            List<Post> posts = database.GetPosts();
            string template;

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ArchiveMe.web.template.html"))
            {
                TextReader tr = new StreamReader(stream);
                template = tr.ReadToEnd();
            }

            SetOPPost(op_p,ref template, posts[0]);
            SetPosts(ref template, posts);
            database.Close();
            SaveAndOpen(template);
        }

        private void SetOPPost(OPPost op_post,ref string template, Post op_post_)
        {
            string html_OPpost;

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ArchiveMe.web.OpPost.html"))
            {
                TextReader tr = new StreamReader(stream);
                html_OPpost = tr.ReadToEnd();
            }
            DateTimeOffset dateTime = DateTimeOffset.FromUnixTimeSeconds(op_post_.time);

            html_OPpost = html_OPpost.Replace("&subject&", op_post.sub);
            html_OPpost = html_OPpost.Replace("&no&", op_post_.no.ToString());
            html_OPpost = html_OPpost.Replace("&time&", dateTime.DateTime.ToString());
            html_OPpost = html_OPpost.Replace("&name&", op_post_.name);
            html_OPpost = html_OPpost.Replace("&id&", op_post_.id);
            html_OPpost = html_OPpost.Replace("&image&", op_post_.image);
            html_OPpost = html_OPpost.Replace("&thumbnails&", op_post_.thumbnail);
            html_OPpost = html_OPpost.Replace("&text&", op_post_.com);
            html_OPpost = html_OPpost.Replace("&country&", op_post_.country_name);

            int copy_index = template.IndexOf("<body>")+7;
            template = template.Insert(copy_index, html_OPpost);
        }

        private void SetPosts(ref string template, List<Post> posts)
        {

            for (int i = 1; i < posts.Count; i++)
            {
                string html_post;

                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ArchiveMe.web.post.html"))
                {
                    TextReader tr = new StreamReader(stream);
                    html_post = tr.ReadToEnd();
                }
                DateTimeOffset dateTime = DateTimeOffset.FromUnixTimeSeconds(posts[i].time);
                html_post = html_post.Replace("&no&", posts[i].no.ToString());
                html_post = html_post.Replace("&time&", dateTime.DateTime.ToString());
                html_post = html_post.Replace("&name&", posts[i].name);
                html_post = html_post.Replace("&id&", posts[i].id);
                html_post = html_post.Replace("&image&", posts[i].image);
                html_post = html_post.Replace("&thumbnails&", posts[i].thumbnail);
                html_post = html_post.Replace("&text&", posts[i].com);
                html_post = html_post.Replace("&country&", posts[i].country_name);


                int copy_index = template.IndexOf("</body>") - 1;
                template = template.Insert(copy_index, html_post);
            }
        }

        private void SaveAndOpen(string template)
        {
            File.WriteAllText("web.html",template);
            var p = new System.Diagnostics.Process();
            p.StartInfo = new System.Diagnostics.ProcessStartInfo("web.html")
            {
                UseShellExecute = true
            };
            p.Start();
        }
    }
}
