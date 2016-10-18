using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public List<Page> Pages { get; set; }
    }

    public class Page
    {
        public int No { get; set; }
        public string Text { get; set; }
    }
}
