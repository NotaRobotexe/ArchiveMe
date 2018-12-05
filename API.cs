using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ArchiveMe
{
    class API
    {
        public bool stop = false;
        int refres_time { get; set; } = 5;
        const string json_url = "https://a.4cdn.org/";
        UInt64 last_post_id;
        Database database;
        int new_posts = 0;

        public void Thread_scrapper(string board,string thread_id)
        {
            GetAndSaveOldPosts(board, thread_id);

            Console.WriteLine("\nType 'quit' to quit");
            Console.WriteLine("Checking for new posts...");
            System.Threading.Thread thread = new System.Threading.Thread(() => GetNewPosts(board, thread_id) );
            thread.Start();
        }

        public void Quit()
        {
            Console.WriteLine("Quit");
            stop = true;
            database.Close();
        }


        private void GetAndSaveOldPosts(string board, string thread_id)
        {
            string url = json_url + board + "/thread/" + thread_id + ".json";
            var client = new WebClient();
            var json = client.DownloadString(url);
            var posts_raw = (JArray)JObject.Parse(json)["posts"];


            OPPost op_post = new OPPost(posts_raw[0],thread_id,board);
            Post []posts = new Post[posts_raw.Count];


            last_post_id = op_post.SetPostData(posts_raw[0],thread_id,board);
            for (int i = 0; i < posts_raw.Count; i++)
            {
                posts[i] = new Post();
                last_post_id = posts[i].SetPostData(posts_raw[i], thread_id, board);
                Console.Write("\r{0}   ",i.ToString() + " posts");
            }

            database = new Database(thread_id, board);
            database.SaveOpPost(op_post);
            database.SavePosts(posts);
            
        }

        private void GetNewPosts(string board, string thread_id)
        {
            while (stop == false)
            {
                System.Threading.Thread.Sleep(refres_time * 1000);
                string url = json_url + board + "/thread/" + thread_id + ".json";
                var client = new WebClient();
                var json = client.DownloadString(url);
                var posts_raw = (JArray)JObject.Parse(json)["posts"];

                if (CheckIfIsActive((JObject)posts_raw[0]))
                {
                    ParseNewPosts(board, thread_id, posts_raw);
                }
                else
                {
                    Quit();
                }
            }
        }

        private void ParseNewPosts(string board, string thread_id, JArray posts_raw)
        {
            UInt64 lattest_post = GetPostNo(posts_raw, 1);

            if (lattest_post > last_post_id)
            {
                int count_down = 1;
                while (lattest_post > last_post_id)
                {
                    Post pst = new Post();
                    pst.SetPostData(posts_raw[posts_raw.Count - count_down], thread_id, board);
                    database.SavePosts(pst);

                    new_posts++;
                    Console.Write("\r{0}   ", new_posts.ToString() + " new posts");

                    count_down++;
                    lattest_post = GetPostNo(posts_raw, count_down);
                }

                last_post_id = GetPostNo(posts_raw, 1);
            }
        }

        private UInt64 GetPostNo(JArray posts_raw,int minus)
        {
            try
            {
                var no_pos = posts_raw[posts_raw.Count - minus].ToString().Substring(posts_raw[posts_raw.Count - minus].ToString().IndexOf(": ") + 2, 9);
                UInt64 lattest_post = Convert.ToUInt64(no_pos);
                return lattest_post;
            }
            catch
            {
                var no_pos = posts_raw[posts_raw.Count - minus].ToString().Substring(posts_raw[posts_raw.Count - minus].ToString().IndexOf(": ") + 2, 8);
                UInt64 lattest_post = Convert.ToUInt64(no_pos);
                return lattest_post;
            }
        }

        private bool CheckIfIsActive(JObject first_post)
        {
            dynamic post = JsonConvert.DeserializeObject(first_post.ToString());

            var closed = Post.TrySet(post, "closed");
            var archived = Post.TrySet(post, "archived");
            if (closed == 1 || archived == 1)
            {
                Console.WriteLine("Thread closed or archived.");
                return false;
            }
            return true;
        }
    }
}
