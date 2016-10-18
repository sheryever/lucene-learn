using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Directory = Lucene.Net.Store.Directory;
using Version = Lucene.Net.Util.Version;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public List<Book> Books { get; set; }
        public Directory Index { get; set; }
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Books = GetBooks();

            lblBookCount.Text = "Total books: " + Books.Count;

        }

        private List<Book> GetBooks()
        {
            var result = new List<Book>();

            for (int i = 0; i < 50; i++)
            {
                var book = new Book {Id = i, Title = "Book " + RandomString(5)};
                var pageCount = random.Next(5, 20);
                book.Pages = new List<Page>();
                for (int j = 0; j < pageCount; j++)
                {
                    book.Pages.Add(new Page {No = j, Text = RandomString(4)});
                }
                result.Add(book);
            }
            

            return result;
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StandardAnalyzer analyzer = new StandardAnalyzer(Version.LUCENE_30);
            Index = new RAMDirectory();

            IndexWriter writer = new IndexWriter(Index, analyzer, new IndexWriter.MaxFieldLength(15));

            foreach (var book in Books)
            {
                var doc = new Document();
                var bookId = new NumericField("Id", Field.Store.YES, false);
                bookId.SetIntValue(book.Id);
                var bookTitle = new Field("Title", book.Title, Field.Store.YES, Field.Index.ANALYZED);
                doc.Add(bookId);
                doc.Add(bookTitle);

                foreach (var page in book.Pages)
                {
                    var pageNo = new NumericField("no", Field.Store.YES, false);
                    pageNo.SetIntValue(page.No);
                    doc.Add(pageNo);
                    doc.Add(new Field("Text", page.Text, Field.Store.YES, Field.Index.ANALYZED));
                }
                writer.AddDocument(doc);
            }
            writer.Dispose();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
                return;
            var analyer = new StandardAnalyzer(Version.LUCENE_30);
            var query = new QueryParser(Version.LUCENE_30, "Text", analyer);

            int hitPerPage = 10;

            var reader = DirectoryReader.Open(Index, true);
            var searcher = new IndexSearcher(reader);
            var topDoc = searcher.Search(query.Parse(txtSearch.Text), hitPerPage);

            var hits = topDoc.ScoreDocs;

            foreach (var hit in hits)
            {
                int docId = hit.Doc;
                Document d = searcher.Doc(docId);
                var bookId = Convert.ToInt32(d.GetValues("Id")[0]);
                lbSearchResult.Items.Add(Books[bookId].Title);
            }

        }
    }
}
