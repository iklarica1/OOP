using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace OTTER
{
    /// <summary>
    /// -
    /// </summary>
    public partial class BGL : Form
    {
        public Form frmIzbornik;
       
        private string player;
        public string Player
        {
            get { return player; }
            set
            {
                if (value == "")
                    player = "Nepoznat";
                else
                    player = value;
            }
        }
        
        /* ------------------- */
        #region Environment Variables

        List<Func<int>> GreenFlagScripts = new List<Func<int>>();

        /// <summary>
        /// Uvjet izvršavanja igre. Ako je <c>START == true</c> igra će se izvršavati.
        /// </summary>
        /// <example><c>START</c> se često koristi za beskonačnu petkalu. Primjer metode/skripte:
        /// <code>
        /// private int MojaMetoda()
        /// {
        ///     while(START)
        ///     {
        ///       //ovdje ide kod
        ///     }
        ///     return 0;
        /// }</code>
        /// </example>
        public static bool START = true;

        //sprites
        /// <summary>
        /// Broj likova.
        /// </summary>
        public static int spriteCount = 0, soundCount = 0;

        /// <summary>
        /// Lista svih likova.
        /// </summary>
        //public static List<Sprite> allSprites = new List<Sprite>();
        public static SpriteList<Sprite> allSprites = new SpriteList<Sprite>();

        //sensing
        int mouseX, mouseY;
        Sensing sensing = new Sensing();

        //background
        List<string> backgroundImages = new List<string>();
        int backgroundImageIndex = 0;
        string ISPIS = "";

        SoundPlayer[] sounds = new SoundPlayer[1000];
        TextReader[] readFiles = new StreamReader[1000];
        TextWriter[] writeFiles = new StreamWriter[1000];
        bool showSync = false;
        int loopcount;
        DateTime dt = new DateTime();
        String time;
        double lastTime, thisTime, diff;

        #endregion
        /* ------------------- */
        #region Events

        private void Draw(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            try
            {                
                foreach (Sprite sprite in allSprites)
                {                    
                    if (sprite != null)
                        if (sprite.Show == true)
                        {
                            g.DrawImage(sprite.CurrentCostume, new Rectangle(sprite.X, sprite.Y, sprite.Width, sprite.Heigth));
                        }
                    if (allSprites.Change)
                        break;
                }
                if (allSprites.Change)
                    allSprites.Change = false;
            }
            catch
            {
                //ako se doda sprite dok crta onda se mijenja allSprites
                MessageBox.Show("Greška!");
            }
        }

        private void startTimer(object sender, EventArgs e)
        {
            timer1.Start();
            timer2.Start();
            Init();
        }

        private void updateFrameRate(object sender, EventArgs e)
        {
            updateSyncRate();
        }

        /// <summary>
        /// Crta tekst po pozornici.
        /// </summary>
        /// <param name="sender">-</param>
        /// <param name="e">-</param>
        public void DrawTextOnScreen(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            var brush = new SolidBrush(Color.WhiteSmoke);
            string text = ISPIS;

            SizeF stringSize = new SizeF();
            Font stringFont = new Font("Arial", 14);
            stringSize = e.Graphics.MeasureString(text, stringFont);

            using (Font font1 = stringFont)
            {
                RectangleF rectF1 = new RectangleF(0, 0, stringSize.Width, stringSize.Height);
                e.Graphics.FillRectangle(brush, Rectangle.Round(rectF1));
                e.Graphics.DrawString(text, font1, Brushes.Black, rectF1);
            }
        }

        private void mouseClicked(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = true;
            sensing.MouseDown = true;
        }

        private void mouseDown(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = true;
            sensing.MouseDown = true;            
        }

        private void mouseUp(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = false;
            sensing.MouseDown = false;
        }

        private void mouseMove(object sender, MouseEventArgs e)
        {
            mouseX = e.X;
            mouseY = e.Y;

            //sensing.MouseX = e.X;
            //sensing.MouseY = e.Y;
            //Sensing.Mouse.x = e.X;
            //Sensing.Mouse.y = e.Y;
            sensing.Mouse.X = e.X;
            sensing.Mouse.Y = e.Y;

        }

        private void keyDown(object sender, KeyEventArgs e)
        {
            sensing.Key = e.KeyCode.ToString();
            sensing.KeyPressedTest = true;
        }

        private void keyUp(object sender, KeyEventArgs e)
        {
            sensing.Key = "";
            sensing.KeyPressedTest = false;
        }

        private void Update(object sender, EventArgs e)
        {
            if (sensing.KeyPressed(Keys.Escape))
            {
                START = false;
            }

            if (START)
            {
                this.Refresh();
            }
        }

        #endregion
        /* ------------------- */
        #region Start of Game Methods

        //my
        #region my

        //private void StartScriptAndWait(Func<int> scriptName)
        //{
        //    Task t = Task.Factory.StartNew(scriptName);
        //    t.Wait();
        //}

        //private void StartScript(Func<int> scriptName)
        //{
        //    Task t;
        //    t = Task.Factory.StartNew(scriptName);
        //}

        private int AnimateBackground(int intervalMS)
        {
            while (START)
            {
                setBackgroundPicture(backgroundImages[backgroundImageIndex]);
                Game.WaitMS(intervalMS);
                backgroundImageIndex++;
                if (backgroundImageIndex == 3)
                    backgroundImageIndex = 0;
            }
            return 0;
        }

        private void KlikNaZastavicu()
        {
            foreach (Func<int> f in GreenFlagScripts)
            {
                Task.Factory.StartNew(f);
            }
        }

        #endregion

        /// <summary>
        /// BGL
        /// </summary>
        public BGL()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Pričekaj (pauza) u sekundama.
        /// </summary>
        /// <example>Pričekaj pola sekunde: <code>Wait(0.5);</code></example>
        /// <param name="sekunde">Realan broj.</param>
        public void Wait(double sekunde)
        {
            int ms = (int)(sekunde * 1000);
            Thread.Sleep(ms);
        }

        //private int SlucajanBroj(int min, int max)
        //{
        //    Random r = new Random();
        //    int br = r.Next(min, max + 1);
        //    return br;
        //}

        /// <summary>
        /// -
        /// </summary>
        public void Init()
        {
            if (dt == null) time = dt.TimeOfDay.ToString();
            loopcount++;
            //Load resources and level here
            this.Paint += new PaintEventHandler(DrawTextOnScreen);
            SetupGame();
        }

        /// <summary>
        /// -
        /// </summary>
        /// <param name="val">-</param>
        public void showSyncRate(bool val)
        {
            showSync = val;
            if (val == true) syncRate.Show();
            if (val == false) syncRate.Hide();
        }

        /// <summary>
        /// -
        /// </summary>
        public void updateSyncRate()
        {
            if (showSync == true)
            {
                thisTime = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
                diff = thisTime - lastTime;
                lastTime = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                double fr = (1000 / diff) / 1000;

                int fr2 = Convert.ToInt32(fr);

                syncRate.Text = fr2.ToString();
            }

        }

        //stage
        #region Stage

        /// <summary>
        /// Postavi naslov pozornice.
        /// </summary>
        /// <param name="title">tekst koji će se ispisati na vrhu (naslovnoj traci).</param>
        public void SetStageTitle(string title)
        {
            this.Text = title;
        }

        /// <summary>
        /// Postavi boju pozadine.
        /// </summary>
        /// <param name="r">r</param>
        /// <param name="g">g</param>
        /// <param name="b">b</param>
        public void setBackgroundColor(int r, int g, int b)
        {
            this.BackColor = Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Postavi boju pozornice. <c>Color</c> je ugrađeni tip.
        /// </summary>
        /// <param name="color"></param>
        public void setBackgroundColor(Color color)
        {
            this.BackColor = color;
        }

        /// <summary>
        /// Postavi sliku pozornice.
        /// </summary>
        /// <param name="backgroundImage">Naziv (putanja) slike.</param>
        public void setBackgroundPicture(string backgroundImage)
        {
            this.BackgroundImage = new Bitmap(backgroundImage);
        }

        /// <summary>
        /// Izgled slike.
        /// </summary>
        /// <param name="layout">none, tile, stretch, center, zoom</param>
        public void setPictureLayout(string layout)
        {
            if (layout.ToLower() == "none") this.BackgroundImageLayout = ImageLayout.None;
            if (layout.ToLower() == "tile") this.BackgroundImageLayout = ImageLayout.Tile;
            if (layout.ToLower() == "stretch") this.BackgroundImageLayout = ImageLayout.Stretch;
            if (layout.ToLower() == "center") this.BackgroundImageLayout = ImageLayout.Center;
            if (layout.ToLower() == "zoom") this.BackgroundImageLayout = ImageLayout.Zoom;
        }

        #endregion

        //sound
        #region sound methods

        /// <summary>
        /// Učitaj zvuk.
        /// </summary>
        /// <param name="soundNum">-</param>
        /// <param name="file">-</param>
        public void loadSound(int soundNum, string file)
        {
            soundCount++;
            sounds[soundNum] = new SoundPlayer(file);
        }

        /// <summary>
        /// Sviraj zvuk.
        /// </summary>
        /// <param name="soundNum">-</param>
        public void playSound(int soundNum)
        {
            sounds[soundNum].Play();
        }

        /// <summary>
        /// loopSound
        /// </summary>
        /// <param name="soundNum">-</param>
        public void loopSound(int soundNum)
        {
            sounds[soundNum].PlayLooping();
        }

        /// <summary>
        /// Zaustavi zvuk.
        /// </summary>
        /// <param name="soundNum">broj</param>
        public void stopSound(int soundNum)
        {
            sounds[soundNum].Stop();
        }

        #endregion

        //file
        #region file methods

        /// <summary>
        /// Otvori datoteku za čitanje.
        /// </summary>
        /// <param name="fileName">naziv datoteke</param>
        /// <param name="fileNum">broj</param>
        public void openFileToRead(string fileName, int fileNum)
        {
            readFiles[fileNum] = new StreamReader(fileName);
        }

        /// <summary>
        /// Zatvori datoteku.
        /// </summary>
        /// <param name="fileNum">broj</param>
        public void closeFileToRead(int fileNum)
        {
            readFiles[fileNum].Close();
        }

        /// <summary>
        /// Otvori datoteku za pisanje.
        /// </summary>
        /// <param name="fileName">naziv datoteke</param>
        /// <param name="fileNum">broj</param>
        public void openFileToWrite(string fileName, int fileNum)
        {
            writeFiles[fileNum] = new StreamWriter(fileName);
        }

        /// <summary>
        /// Zatvori datoteku.
        /// </summary>
        /// <param name="fileNum">broj</param>
        public void closeFileToWrite(int fileNum)
        {
            writeFiles[fileNum].Close();
        }

        /// <summary>
        /// Zapiši liniju u datoteku.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <param name="line">linija</param>
        public void writeLine(int fileNum, string line)
        {
            writeFiles[fileNum].WriteLine(line);
        }

        /// <summary>
        /// Pročitaj liniju iz datoteke.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <returns>vraća pročitanu liniju</returns>
        public string readLine(int fileNum)
        {
            return readFiles[fileNum].ReadLine();
        }

        /// <summary>
        /// Čita sadržaj datoteke.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <returns>vraća sadržaj</returns>
        public string readFile(int fileNum)
        {
            return readFiles[fileNum].ReadToEnd();
        }

        #endregion

        //mouse & keys
        #region mouse methods

        /// <summary>
        /// Sakrij strelicu miša.
        /// </summary>
        public void hideMouse()
        {
            Cursor.Hide();
        }

        /// <summary>
        /// Pokaži strelicu miša.
        /// </summary>
        public void showMouse()
        {
            Cursor.Show();
        }

        /// <summary>
        /// Provjerava je li miš pritisnut.
        /// </summary>
        /// <returns>true/false</returns>
        public bool isMousePressed()
        {
            //return sensing.MouseDown;
            return sensing.MouseDown;
        }

        /// <summary>
        /// Provjerava je li tipka pritisnuta.
        /// </summary>
        /// <param name="key">naziv tipke</param>
        /// <returns></returns>
        public bool isKeyPressed(string key)
        {
            if (sensing.Key == key)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Provjerava je li tipka pritisnuta.
        /// </summary>
        /// <param name="key">tipka</param>
        /// <returns>true/false</returns>
        public bool isKeyPressed(Keys key)
        {
            if (sensing.Key == key.ToString())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #endregion
        /* ------------------- */

        /* ------------ GAME CODE START ------------ */

        /* Game variables */

        Likovi jagoda, candy;
        Tablete mag, kal, vitC;
        JunkFood cola, fries, burger;

        Random rand = new Random();



        public delegate void DodirDelegata();
        public static event DodirDelegata dodir;
        /* Initialization */
       

        private void SetupGame()
        {
            //1. setup stage
            SetStageTitle("Igrač: "+this.Player);
            //setBackgroundColor(Color.WhiteSmoke);            
            setBackgroundPicture("backgrounds\\WoodTable.jpg");
            //none, tile, stretch, center, zoom
            //setPictureLayout("stretch");


            //2. add sprites

            jagoda = new Likovi("sprites\\strawberry.jpg", 0, 400);
            jagoda.SetSize(35);
            jagoda.SetTransparentColor(Color.White);
            Game.AddSprite(jagoda);

            candy = new Likovi("sprites\\candy.png",400, 350);
            candy.SetSize(75);
           
            Game.AddSprite(candy);
            vitC = new VitaminC("sprites\\YellowTablet.jpg", 0, 0);
            vitC.SetSize(20);
            //vitC.SetVisible(false);
            
            Game.AddSprite(vitC);

            kal = new Kalcij("sprites\\BlueTablet.jpg",0,0);
            kal.SetSize(20);
            kal.SetTransparentColor(Color.WhiteSmoke);
            //kal.SetVisible(false);
            Game.AddSprite(kal);
            mag = new Magnezij("sprites\\GrayTablet.jpg",0, 0);
            mag.SetSize(20);
            //mag.SetVisible(false);
            Game.AddSprite(mag);
      
            cola = new JunkFood("sprites\\Cola.png",150,350);
            cola.SetSize(15);
            Game.AddSprite(cola);
            fries = new JunkFood("sprites\\Fries.png", 350, 250);
            fries.SetSize(20);
            Game.AddSprite(fries);
            burger = new JunkFood("sprites\\Burger.png", 450, 100);
            burger.SetSize(20);
            Game.AddSprite(burger);
            
            jagoda.KrajIgre += OsobaKraj;
            dodir += DodirTableta;
            

            //3. scripts that start
            Game.StartScript(Jagoda);
            Game.StartScript(Stvori);
            Game.StartScript(EnemyKretanje);
            Game.StartScript(Poraz);
            Game.StartScript(Pobjeda);
           
        }
        
        

        /* Scripts */
        
        private void OsobaKraj()
        {
            ISPIS = "Kraj igre: "+ this.Player+"-" + jagoda.Bodovi + " bodova";
            Wait(0.1);
            START = false;
        }

        private void DodirTableta()
        {
            if (jagoda.TouchingSprite(vitC))
            {
                vitC.Y = 0;
                vitC.X = rand.Next(50, 600);
            }
            else if (jagoda.TouchingSprite(kal))
            {
                kal.Y = 0;
                kal.X = rand.Next(50, 600);
            }
            else if (jagoda.TouchingSprite(mag))
            {
                mag.Y = 0;
                mag.X = rand.Next(50, 600);
            }
             
        }
        private int Stvori()
        {
            vitC.X = 100;
            kal.X = 250;
            mag.X = 450;
            while(START)
            {
                ISPIS = "Broj bodova: " + jagoda.Bodovi;
                vitC.Y += vitC.Brzina;
                Wait(0.2);
                kal.Y += kal.Brzina;
                Wait(0.2);
                mag.Y += mag.Brzina;
                Wait(0.2);
                if(vitC.TouchingEdge())
                {
                    jagoda.Bodovi -= 1;
                    vitC.Y = 0;
                    
                }
                if(kal.TouchingEdge())
                {
                    jagoda.Bodovi -= 1;
                    kal.Y = 0;
                    
                }
                if(mag.TouchingEdge())
                {
                    jagoda.Bodovi -= 2;
                    
                    mag.Y = 0;
                }
                if (jagoda.Bodovi <= -1)
                {
                    ISPIS = "KRAJ IGRE!!";
                }

            }
            return 0;

        }
        
        private int Jagoda()
        {
           
            while (START)
            {
                
                int pozx = jagoda.X;
                int pozy = jagoda.Y;
                if (sensing.KeyPressed(Keys.Right))
                    jagoda.X += 15;
                if (sensing.KeyPressed(Keys.Left))
                    jagoda.X -= 15;
                if (sensing.KeyPressed(Keys.Up))
                {
                    jagoda.Y -= 15;
                }
                if(sensing.KeyPressed(Keys.Down))
                {
                    jagoda.Y += 15;
                }
                Wait(0.01);
                while(jagoda.TouchingSprite(cola)||jagoda.TouchingSprite(fries)||jagoda.TouchingSprite(burger))
                {
                    jagoda.X = pozx;
                    jagoda.Y = pozy;
                    jagoda.Bodovi -= 3;
                    Wait(0.5);
                }
                 
              
                if (jagoda.TouchingSprite(vitC))
                {
                    Wait(0.2);
                    dodir.Invoke();
                    jagoda.Bodovi += vitC.Bodovivrijednost;
                    ISPIS = "Broj bodova: " + jagoda.Bodovi;

                }
                else if (jagoda.TouchingSprite(kal))
                {
                    Wait(0.2);
                    dodir.Invoke();
                    jagoda.Bodovi += kal.Bodovivrijednost;
                    ISPIS = "Broj bodova: " + jagoda.Bodovi;

                }
                else if (jagoda.TouchingSprite(mag))
                {
                    Wait(0.2);
                    dodir.Invoke();
                    jagoda.Bodovi += mag.Bodovivrijednost;
                    ISPIS = "Broj bodova: " + jagoda.Bodovi;
                }

                if (jagoda.Bodovi >= 30)
                {
                    ISPIS = "LEVEL 2!! Bodovi: " + jagoda.Bodovi;
                    LevelUp();
                }

                if (jagoda.Bodovi >= 60 )
                    ISPIS = "POBJEDA!! Bodovi: " + jagoda.Bodovi;
            }
            return 0;
        }
        public int Poraz()
        {
            while(START)
            {
                if (ISPIS.Contains("KRAJ"))
                    {
                    START = false;
                    this.Invoke((Action)delegate { this.Hide(); });
                    GameOver k = new GameOver();
                    k.Rang = "Bodovi: " + Podatak.ToString() + "Igrač: " + Player;
                    k.ShowDialog();
                }
            }
            return 0;
        }
        public void LevelUp()
        {
            jagoda.CurrentCostume = new Bitmap("sprites\\Bike.png");
            jagoda.Width = jagoda.CurrentCostume.Width;
            jagoda.Heigth = jagoda.CurrentCostume.Height;
            jagoda.SetSize(15);
            this.setBackgroundPicture("backgrounds\\PlasticTable.jpg");

            burger.X = 200;
            fries.X = 450;
            cola.X = 300;
            
        }
        public int Pobjeda()
        {
            while(START)
            {
                if(ISPIS.Contains("POBJEDA"))
                {
                    START = false;
                    this.Invoke((Action)delegate { this.Hide(); });
                    GameOver k = new GameOver();
                    k.Rang = "POBJEDA!! Bodovi: " + jagoda.Bodovi + "Igrač: " + Player;
                    k.ShowDialog();
                }
            }
            return 0;
        }
        
     public int EnemyKretanje()
        {
            candy.SetHeading(Sprite.DirectionsType.right);
            int brdodira = 0;
            while (START)
            {
                candy.PointToSprite(jagoda);
                candy.MoveSteps(2);
                if (candy.TouchingSprite(jagoda))
                {
                    jagoda.Bodovi -= 1;
                    Wait(1);
                    brdodira++;
                    
                }
                else if (candy.Rub == "desno")
                    candy.SetHeading(Sprite.DirectionsType.left);
                else if (candy.Rub == "lijevo")
                    candy.SetHeading(Sprite.DirectionsType.right);
                Wait(0.02);
                if (brdodira == 5)
                {
                    ISPIS = "KRAJ IGRE!! Bodovi: " + jagoda.Bodovi;
                   
                }
            }
            return 0;
        }
        
        private int pozadina;
        public int Pozadina { get => pozadina; set => pozadina = value; }
        public int Podatak { get => jagoda.Bodovi; set => jagoda.Bodovi = value; }
        
        /* ------------ GAME CODE END ------------ */


    }
}
