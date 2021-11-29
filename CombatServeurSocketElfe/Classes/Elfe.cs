using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CombatServeurSocketElfe.Classes
{
    public class Elfe:Combattant
    {
        public int  Sort{ get; set; }

        public Elfe(int vie, int force, int sort)
        {
            Vie = vie;
            Force = force;
            Sort = sort;
            Avatar = Image.FromFile("elfe.jpg");
        }
        public void LancerSort(Nain nain)
        {
            nain.Vie = nain.Vie - Sort;
            if (Sort > 1)                
                Sort--;
            if(nain.Force > 1)
                nain.Force--;            
        }
    }
}
