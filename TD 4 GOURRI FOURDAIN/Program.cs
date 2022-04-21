using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace TD_4_GOURRI_FOURDAIN
{
    class Program
    {
        static string Selection()
        {
            Console.WriteLine(
                "\n================ Traitement d'image =================\n " +
                "\nChoisissez une image parmi celles-ci : " +
                "\n\n1. Lena" +
                "\n2. Coco" +
                "\n3. Lac en montagne" +
                "\n\n================ Autres options ===================== " +
                "\n\n4. Qrcode d'une chaîne de caractères" +
                "\n\n5. Ouvrir le rapport\n\n");

            int numeroImage = -1;
            do 
            {
                try
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    numeroImage = Convert.ToInt32(Console.ReadLine());
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Veuillez saisir un nombre entier\n");
                }

            } while (numeroImage != 1 && numeroImage != 2 && numeroImage != 3 && numeroImage != 4 && numeroImage != 5);

            string filename = null;

            switch (numeroImage)
            {
                case 1:
                    Console.Clear();
                    //Console.Beep(1500, 150);
                    filename = "lena.bmp";
                    break;
                case 2:
                    Console.Clear();
                    //Console.Beep(1500, 150);
                    filename = "coco.bmp";
                    break;
                case 3:
                    Console.Clear();
                    //Console.Beep(1500, 150);
                    filename = "lac.bmp";
                    break;
                case 4:
                    Console.Clear();
                    //Console.Beep(1500, 150);
                    QRCode();
                    break;
                case 5: 
                    Console.Clear();
                    //Console.Beep(1500, 150);
                    Console.WriteLine("Ouverture du rapport en .pdf");
                    Process.Start("Rapport Gourri Fourdain TDH.pdf");
                    break;
            }

            if (numeroImage != 4 || numeroImage != 5)
            {
                Console.WriteLine("Ouvrir l'image ?\n1. Oui\n2. Non\n");
                int ouvrir = -1;
                do
                {
                    try
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        ouvrir = Convert.ToInt32(Console.ReadLine());
                    }
                    catch
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("Entrez 1 pour oui ou 2 pour non\n");
                    }
                } while (ouvrir != 1 && ouvrir != 2);

                if (ouvrir == 1)
                {
                    Console.Clear();
                    Process.Start(filename);
                }
                Console.Clear();
            }         
            return filename;
        }
        static void Traitement(MyImage image)
        {
            int numeroImage = -1;
            Console.WriteLine("Choisissez un effet pour " + image.Nom + " :\n" +
                "\n\n================================== CLASSIQUE ===============================" +
                "\n\n1. Nuance de gris" +
                "\n2. Noir et blanc " +
                "\n3. Négatif" +
                "\n4. Sépia" +
                "\n5. Image miroir" +
                "\n6. Rotation" +
                "\n7. Agrandir" +
                "\n8. Rétrécir" +
                "\n\n================================== AUTRES ==================================" +
                "\n\n9. Appliquer un filtre (Convolution)" +
                "\n10. Fractale de Mandelbrot" +
                "\n11. Afficher l'histogramme de l'image" +
                "\n12. Cacher une image dans une autre" +
                "\n13. Décrypter une image" +
                "\n\n0. Sortir\n");
            try
            {        
                numeroImage = Convert.ToInt32(Console.ReadLine());
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch
            {     
                Console.Write("Veuillez entrer un entier entre 1 et 13\n");
                Console.ForegroundColor = ConsoleColor.Red;
            }

            switch (numeroImage)
            {
                case 0:
                    Environment.Exit(0); 
                    break;
                case 1:
                    Console.Clear();
                    //Console.Beep(1700, 150);
                    image.EnGris();
                    break;
                case 2:
                    Console.Clear();
                    //Console.Beep(1700, 150);
                    image.NoirEtBlanc();
                    break;
                case 3:
                    Console.Clear();
                    //Console.Beep(1700, 150);
                    image.Negatif();
                    break;
                case 4:
                    Console.Clear();
                    //Console.Beep(1700, 150);
                    image.Sepia();
                    break;
                case 5:
                    Console.Clear();
                    //Console.Beep(1700, 150);
                    image.Miroir();
                    break;
                case 6:
                    Console.Clear();
                    //Console.Beep(1700, 150);
                    image.Rotation();
                    break;
                case 7:
                    Console.Clear();
                    //Console.Beep(1700, 150);
                    image.Agrandir();
                    break;
                case 8:
                    Console.Clear();
                    //Console.Beep(1700, 150);
                    image.Retrecir();
                    break;
                case 9:
                    Console.Clear();
                    //Console.Beep(1700, 150);
                    image.FiltreMatConv();
                    break;
                case 10:
                    Console.Clear();
                    //Console.Beep(1700, 150);
                    image.Fractale();
                    break;
                case 11: 
                    Console.Clear();
                    //Console.Beep(1700, 150);
                    image.Histogramme();
                    break;
                case 12:
                    Console.Clear();
                    //Console.Beep(1700, 150);
                    Console.WriteLine("\nQuelle image voulez vous cacher dans " + image.Nom
                                 + "\n\n1. Lena"
                                 + "\n2. Coco"
                                 + "\n3. Lac en montagne\n\n");

                    string fichier2 = "";

                    int choix = 0;
                    int.TryParse(Console.ReadLine(), out choix);

                    if (choix == 1) { fichier2 = "lena.bmp"; }
                    if (choix == 2) { fichier2 = "coco.bmp"; }
                    if (choix == 3) { fichier2 = "lac.bmp"; }

                    MyImage image2 = new MyImage(fichier2);
                    image.AfficherImage(image.ImageDansImage(image2.Image), image.Nom);
                    break;
                case 13:
                    Console.Clear();
                    image.AfficherImage(image.DecrypteImage(), image.Nom);
                    break;
            }
        }

        static void QRCode()
        {
            Console.Clear();
            QRCode QrCode;
            string version = "";
            string chaineDeCar = "";
            bool test = true;

            while (test)
            {
                Console.WriteLine("==================================  QRCODE  ==================================");
                Console.WriteLine("\nSaisissez un mot ou une phrase (de moins de 47 caractères)\n\n");
                chaineDeCar = Console.ReadLine().ToUpper();

                if (chaineDeCar.Length <= 47) 
                {
                    test = false; 
                }
                else
                {
                    Console.WriteLine("\n\nVotre phrase est trop longue, il faut saisir moins de 47 caractères");
                }
            }

            if (chaineDeCar.Length > 25 && chaineDeCar.Length <= 47)
            {
                Console.WriteLine("\n\nLa version du QRcode sera : 2");
                QrCode = new QRCode(chaineDeCar, "2");
            }

            else
            {
                bool test2 = true;
                while (test2)
                {
                    Console.WriteLine("\n\nChoisissez la version de QRcode : 1 ou 2 ? ");
                    version = Console.ReadLine();

                    if (version == "1" || version == "2") 
                    { 
                        Console.ForegroundColor = ConsoleColor.White; 
                        test2 = false; 
                    }
                    else 
                    {
                        Console.ForegroundColor = ConsoleColor.Red; 
                        Console.WriteLine("Veuillez saisir 1 ou 2"); 
                    }
                }
                Console.WriteLine("\n\nVous avez choisi la version : " + version);
                QrCode = new QRCode(chaineDeCar, version);
            }

            ///Pour afficher la chaine de bits
            Console.WriteLine("\n\n\n" + QrCode.QRChainWithErr);
            Console.WriteLine("\nQR code bits chain length :\n" + QrCode.QRChainWithErr.Length);

            MyImage ImageQr = new MyImage();
            ImageQr.FichierDansMatrice("lena.bmp"); //Pour ne pas réécrire le header d'une image carré

            byte[,,] matQR = QrCode.ImageQrCode(QrCode.QRChainWithErr);

            ImageQr.QRCodeImage(matQR);
            
        }
        static void Main(string[] args)
        {
            Console.WriteLine("==================================  BONJOUR !  ==================================\n\n\n");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("♥ ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Bienvenue dans le projet de traitement d'images de Héloïse FOURDAIN et Ilies GOURRI!");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(" ♥\n");

            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            MyImage image = new MyImage(Selection());
            int fini = 0;

            while (fini != 666)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Traitement(image);
                Console.WriteLine("Si vous ne voulez pas continuer de modifier l'image tapez 666 sinon 999");
                fini = int.Parse(Console.ReadLine());
                Console.Clear();
            }
            Console.ReadKey();
        }
    }
}
