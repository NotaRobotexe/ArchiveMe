using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using System.IO;

namespace ArchiveMe
{
    class Database
    {
        SQLiteCommand sql_com;
        SQLiteConnection sql_dat;
        string thread_id;
        string board;


        //prefix th... in front of thread id for post and op... in front of OP post
        public Database(string thread_id,string board)
        {
            this.thread_id = thread_id;
            this.board = board;
            System.IO.Directory.CreateDirectory("Database");

            sql_dat = new SQLiteConnection("Data Source=Database\\"+board.ToString()+".db");
            sql_dat.Open();

            string sql = "CREATE TABLE IF NOT EXISTS th"+thread_id
                +"(post_no INT,  post_time BIGINT, name VARCHAR(25),id VARCHAR(25), country VARCHAR(2),troll_country VARCHAR(2)," +
                "country_name VARCHAR(25),com TEXT,image VARCHAR(255),thumbnail VARCHAR(255)); CREATE TABLE IF NOT EXISTS op" + thread_id.ToString()
                + "(sub TEXT, images INT, replies INT);";
            sql_com = new SQLiteCommand(sql, sql_dat);
            sql_com.ExecuteNonQuery();
        }
        
        public OPPost GetOPPOst()
        {
            string sql = "SELECT * from op" + thread_id;
            sql_com = new SQLiteCommand(sql, sql_dat);

            OPPost op = null;
            using (SQLiteDataReader reader = sql_com.ExecuteReader())
            {
                while (reader.Read())
                {
                    op = new OPPost(reader.GetString(0),reader.GetInt16(1), reader.GetInt16(2));
                }
            }

            return op;
        }

        public List<Post> GetPosts()
        {
            string sql = "SELECT * from th" + thread_id + " ORDER BY post_no ASC";
            sql_com = new SQLiteCommand(sql, sql_dat);
            List<Post> posts = new List<Post>();

            using (SQLiteDataReader reader = sql_com.ExecuteReader())
            {
                while (reader.Read())
                {
                    Post post = new Post(Convert.ToUInt64(reader.GetInt64(0)), reader.GetInt64(1),reader.GetString(2), reader.GetString(3), reader.GetString(4), 
                        reader.GetString(5), reader.GetString(6), reader.GetString(7), reader.GetString(8), reader.GetString(9));
                    posts.Add(post);
                }
            }
            return posts;
        }

        public void SaveOpPost(OPPost oppost)
        {
            string sub = (oppost.sub != null) ? oppost.sub.ToString() : "*";

            string sql = "INSERT INTO op" + thread_id + " VALUES('"+sub+"','"+oppost.images.ToString()+ "','" + oppost.replies.ToString() +"');";
            sql_com = new SQLiteCommand(sql, sql_dat);
            sql_com.ExecuteNonQuery();
        }

        public void SavePosts(Post post)
        {
            string id = (post.id != null) ? post.id.ToString() : "*";
            string country = (post.country != null) ? post.country.ToString() : "*";
            string troll_country = (post.troll_country != null) ? post.troll_country.ToString() : "*";
            string country_name = (post.country_name != null) ? post.country_name.ToString() : "*";
            string com = (post.com != null) ? post.com.ToString() : "*";
            string thumbnail = (post.thumbnail != null) ? post.thumbnail.ToString() : "*";
            string image = (post.image != null) ? post.image.ToString() : "*";

            string sql = "INSERT INTO th" + thread_id + " VALUES('" + post.no.ToString() + "','" + post.time.ToString() + "','" + post.name.ToString() + "','" +
                 id + "','" + country + "','" + troll_country + "','" + country_name + "','" + com + "','" + image + "','" + thumbnail + "');";

            sql_com = new SQLiteCommand(sql, sql_dat);
            sql_com.ExecuteNonQuery();
        }

        public void SavePosts(Post[] posts)
        {
            foreach (var post in posts)
            {
                SavePosts(post);
            }
        }

        public void Close()
        {
            sql_dat.Close();
        }

        public static void GetAllDatabasesAndTables()
        {
            DirectoryInfo dir = new DirectoryInfo("Database");
            FileInfo[] files = dir.GetFiles("*.db");

            Console.WriteLine("\nDatabases:");
            SQLiteCommand com;
            SQLiteConnection dat;
            foreach (var file in files)
            {
                Console.WriteLine("/"+file.Name.Substring(0,file.Name.IndexOf('.'))+"/");
                dat = new SQLiteConnection("Data Source=Database\\" + file.Name);
                dat.Open();
                string sql = "SELECT name FROM sqlite_master WHERE name LIKE 'th%'";
                com = new SQLiteCommand(sql, dat);
                using (SQLiteDataReader reader = com.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine("  "+reader.GetString(0).Substring(2));
                    }
                }

                dat.Close();
            }

        }
    }
}
