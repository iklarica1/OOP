using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OTTER
{
    public partial class Okruzenje : Form
    {
        BGL igra = new BGL();
        public Okruzenje()
        {
            InitializeComponent();
        }
        public Form frmpozadina;
        Pozadina poz = new Pozadina();
        private int odabir;

        public int Odabir { get => odabir; set => odabir = value; }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            BGL igra = new BGL();
            igra.frmIzbornik = this;
            igra.Pozadina = 1;
            igra.Player = textBox1.Text;
            igra.ShowDialog();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
