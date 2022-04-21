using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TD_4_GOURRI_FOURDAIN
{
    public class Complexe // Pour la méthode Fractale
    {
        double reelle;
        double imag;
        public Complexe(double reelle, double imag)
        {
            this.reelle = reelle;
            this.imag = imag;
        }
        public void AuCarre()
        {
            double temp = (reelle * reelle - (imag * imag));
            imag = 2.0 * reelle * imag;
            reelle = temp;
        }
        public double Module()
        {
            double module = Math.Sqrt((reelle * reelle) + (imag * imag));
            return module;
        }
        public void Ajouter(Complexe z)
        {
            reelle = reelle + z.reelle;
            imag = imag + z.imag;
        }
    }
}
