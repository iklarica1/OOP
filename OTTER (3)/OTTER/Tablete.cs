using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OTTER
{
    class Tablete : Sprite
    {
        public bool Aktivan { get; set; }
        private int bodoviVrijednost;
        public int Bodovivrijednost
        {
            get { return bodoviVrijednost; }
            set {
                bodoviVrijednost = value;
                
                    
            }
        }
        private string rub;
        public string Rub
        {
            get { return rub; }
            set
            {
                rub = value;
            }
        }
        public override int X
        {
            get { return base.X; }
            set
            {
                if (value + this.Width > GameOptions.RightEdge)
                {
                    base.x = GameOptions.RightEdge - this.Width;
                    this.Rub = "desno";

                }
                else if (value < 0)
                {
                    base.x = 0;
                    this.Rub = "lijevo";
                }
                else
                {
                    base.x = value;
                    this.Rub = "";
                }
            }
        }
        public override int Y
        {
            get { return base.Y; }
            set
            {
                if(value+this.Heigth>GameOptions.DownEdge)
                {
                    base.y = GameOptions.DownEdge - this.Heigth;
                    this.Rub = "dolje";
                }
                else if(value<0)
                {
                    this.Y = 0;

                    this.Rub = "gore";
                }
                else
                {
                    base.y = value;
                    this.Rub = "";
                }
            }
        }
        private int brzina;
        public int Brzina
        {
            get { return brzina; }
            set { brzina = value; }
        }
        public Tablete(string pic,int x,int y):base(pic,x,y)
        {
            rub = "";
            Aktivan = false;
            
        }
        public bool TouchingSprite(Tablete n)
        {
            Sprite s = n;
            if(this.TouchingSprite(s))
            {
                n.Aktivan = false;
                this.Bodovivrijednost += n.Bodovivrijednost;
                return true;
            }
            return false;
        }
        
    }
    class Magnezij : Tablete
    {
        public Magnezij(string pic,int x,int y):base(pic,x,y)
        {
            this.Bodovivrijednost = 6;
            this.Brzina = 25;
        }
    }
    class Kalcij : Tablete
    {
        public Kalcij(string pic, int x, int y) : base(pic, x, y)
        {
            this.Bodovivrijednost = 4;
            this.Brzina = 15;
        }
    }
    class VitaminC : Tablete
    {
        public VitaminC(string pic,int x,int y):base(pic,x,y)
        {
            this.Bodovivrijednost = 2;
            this.Brzina = 10;
        }
    }
    
 }
     

