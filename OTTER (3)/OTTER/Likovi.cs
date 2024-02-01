using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace OTTER
{

    public class Likovi:Sprite
    {
        private string rub;
        public string Rub
        {
            get { return rub; }
            set
            {
                rub = value;
            }
        }
        public delegate void EventHandler();
        public event EventHandler KrajIgre;
        private int bodovi;
        public int Bodovi
        {
            get { return bodovi; }
            set
            {
                bodovi = value;
            }
        }
        public override int X
        {
            get { return base.X; }
            set
            {
                if(value+this.Width>GameOptions.RightEdge)
                {
                    base.x = GameOptions.RightEdge - this.Width;
                    this.Rub = "desno";

                }
                else if(value<0)
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
                base.Y = value;
                if (value + this.Heigth > GameOptions.DownEdge)
                    base.y = GameOptions.DownEdge - this.Heigth;
                else if (value < 0)
                    base.y = 0;
                else
                    base.y = value;
            }
        }
        
        public void Move(int x)
        {
            this.X += x;
        }
        
        public Likovi(string pic,int x,int y):base(pic,x,y)
        {
            Rub = "";
            bodovi = 0;
            
        }
        
        
        
    }
}
