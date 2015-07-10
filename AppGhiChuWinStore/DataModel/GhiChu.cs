using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppGhiChuWinStore.DataModel
{
    public class GhiChu
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Time { get; set; }
        public bool Remind { get; set; }
        public bool Complete { get; set; }
        public GhiChu()
        {
            //empty constructor  
        }
        public GhiChu(string title, string content, string time, bool remind, bool complete)
        {
            Title = title;
            Content = content;
            Time = time;
            Remind = remind;
            Complete = complete;
        }
    }
}
