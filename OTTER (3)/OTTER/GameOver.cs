using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace OTTER
{
    public partial class GameOver : Form
    {
        public GameOver()
        {
            InitializeComponent();
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            BGL.spriteCount = 0;
            BGL.allSprites.Clear();
            GC.Collect();
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Okruzenje poz = new Okruzenje();
            poz.frmpozadina = this;
            poz.ShowDialog();
            BGL.allSprites.Clear();
            this.Invoke((Action)delegate { this.Hide(); });
            
            
        }
        public Form frmKraj;
        private int odabir;

        private void Gameovercs_Load(object sender, EventArgs e)
        {
            using (StreamWriter sw = File.AppendText("datoteka.txt"))
            {
                sw.WriteLine(Rang);
            }
            using (StreamReader sr = File.OpenText("datoteka.txt"))
            {
                string linija;
                while((linija=sr.ReadLine())!=null)
                {
                    listBox1.Items.Add(linija);
                }

            }
        }
        
        private string rang;

        public string Rang { get => rang; set => rang = value; }
        public int Odabir { get => odabir; set => odabir = value; }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
