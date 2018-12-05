using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ArchiveMe
{
    public class OPPost : Post {
        public string sub { get; set; }
        public int images { get; set; }
        public int replies { get; set; }

        public OPPost(JToken OPpost, string thread_id, string board)
        {
            dynamic post = JsonConvert.DeserializeObject(OPpost.ToString());
            sub = TrySet(post, "sub");
            images = post.images;
            replies = post.replies;
            SetPostData(OPpost, thread_id, board);

        }

        public OPPost(string sub,int images,int replies)
        {
            this.sub = sub;
            this.images = images;
            this.replies = replies;
        }

    }

    public class Post
    {
        private string thread_id {get;set;}
        private string board { get; set; }

        public UInt64 no { get; set; }
        public Int64 time { get; set; }
        public string name { get; set; }
        public string id { get; set; }
        public string country { get; set; }       //only one of this will be set
        public string troll_country { get;set; }
        public string country_name { get; set; }
        public long tim { get; set; } //filename 
        public string ext { get; set; }
        public string com { get; set; }

        public string image { get; set; }
        public string thumbnail { get; set; }

        public Post()
        {
        }

        public Post(UInt64 no, Int64 time, string name, string id,string country, string troll_country, string country_name, string com, string image, string thumbnail)
        {
            this.no = no;
            this.time = time;
            this.name = name;

            this.id = (id != "*") ? " ID: "+id : null;
            this.country = (country != "*") ? country : null;
            this.troll_country = (troll_country != "*") ? troll_country : null;
            this.country_name = (country_name != "*") ? country_name : null;
            this.com = (com != "*") ? com : null;
            this.thumbnail = (thumbnail != "*") ? thumbnail : null;
            this.image = (image != "*") ? image : null;
        }

        public UInt64 SetPostData(JToken raw_post, string thread_id, string board)
        {
            this.thread_id = thread_id;
            this.board = board;
            dynamic post = JsonConvert.DeserializeObject(raw_post.ToString());

            string text = post.ext;
            no = post.no;
            time = post.time;
            name = post.name;

            id = TrySet(post, "id");
            country = TrySet(post,"country");
            troll_country = TrySet(post, "troll_country");
            country_name = TrySet(post, "country_name");
            ext = TrySet(post, "ext");
            com = TrySet(post, "com");

            if((TrySet(post, "tim") != null)){
                tim = (TrySet(post, "tim"));
            }
            else{
                tim = 0;
            }

            if (ext != null)
            {
                if (DowloadImages(thread_id,board,tim.ToString(),ext))
                {
                    image = "images\\" + board + "\\" + thread_id + "\\" + tim + ext;
                    thumbnail = "images\\" + board + "\\" + thread_id + "\\" + tim + "s.jpg";
                }
            }

            return no;
        }

        private bool DowloadImages(string thread_id,string board, string tim, string ext)
        {
            string image_save_path = System.AppDomain.CurrentDomain.BaseDirectory + "images\\" + board + "\\" + thread_id + "\\" + tim  + ext;
            string thumbnail_save_path = System.AppDomain.CurrentDomain.BaseDirectory +"images\\" + board + "\\" + thread_id + "\\" + tim + "s.jpg";

            string image_url = "https://i.4cdn.org/"+board+"/"+tim+ext;
            string thumbnail_url = "https://i.4cdn.org/" + board + "/" + tim + "s.jpg";

            using (var client = new System.Net.WebClient())
            {
                try
                {
                    System.IO.Directory.CreateDirectory(System.AppDomain.CurrentDomain.BaseDirectory+"images\\" + board + "\\" + thread_id + "\\");
                    client.DownloadFile(image_url,image_save_path);
                    client.DownloadFile(thumbnail_url, thumbnail_save_path);
                }
                catch (Exception)
                {
                    return false;
                    throw;
                }
            }

            return true;
        }

        public static dynamic TrySet(dynamic obj,string post_propery)
        {
            dynamic property;
            try
            {
                 property = obj[post_propery].Value;
            }
            catch
            {
                property = null;
            }
            return property;
        }

        public UInt64 GetNo()
        {
            return no;
        }



    }

}
