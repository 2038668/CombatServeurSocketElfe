using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatServeurSocketElfe.Classes
{
    public class Nain:Combattant
    {
        public string Arme { get; set; }
        string[] tArme = {"hache","pioche","lance" };
        public Nain(int vie, int force, int noArme)
        {
            Vie = vie;
            Force = force;
            Arme = tArme[noArme];
            Avatar = Image.FromFile("nain.jpg");
        }
        public void Frapper(Elfe elfe)
        {
            elfe.Vie = elfe.Vie - Force;
        }
    }
}
