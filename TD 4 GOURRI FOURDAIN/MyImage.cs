using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using static System.Math;


namespace TD_4_GOURRI_FOURDAIN
{
    class MyImage
    {
        
        /// Attributs

        private string nom;
        private string typeImage;

        private int tailleFichier;
        private int tailleImage;
        private int tailleOffset;
        private int largeur;
        private int hauteur;
        private int nb_Bit_Couleur;
        private int offset;

        private byte[] header = new byte[54];
        private byte[] myfile; 
        private byte[,,] pixelsRvb; // Matrice 3 dimensions, la 3e dimension représente les pixels rouge, vert et bleu
        private byte[] imageByte;
        private byte[] headerinfo; // infos liées à l'image
        private byte[] pixelsOctets; 

        private Pixel[,] image;

        /// Accesseurs

        public string Nom
        {
            get { return nom; }
        }
        public string TypeImage
        {
            get { return typeImage; }
        }
        public int TailleFichier
        {
            get { return tailleFichier; }
        }

        public int TailleOffset
        {
            get { return tailleOffset; }
        }

        public int Largeur
        {
            get { return largeur; }
        }

        public int Hauteur
        {
            get { return hauteur; }
        }

        public int NbBitCouleurs
        {
            get { return nb_Bit_Couleur; }
        }
        public int GetTailleImage
        {
            get { return this.tailleImage; }
        }

        public Pixel[,] Image
        {
            get { return image; }
        }


        /// Constructeurs
        
        public MyImage(string fichier)
        {
            From_Image_To_File(fichier);
            nom = fichier;
            typeImage = "BM";

            for (int i = 0; i < 54; i++) // Header
            {
                header[i] = myfile[i];
            }

            // tailleFichier correspond à i allant de 2 à 5
            offset = 2;
            tailleFichier = Convertir_Endian_To_Int(4);

            /// INFORMATIONS SUR L'IMAGE

            // taille offset correspond à i allant de 14 à 17
            offset = 14;  
            tailleOffset = Convertir_Endian_To_Int(4);

            // largeur correspond à i allant de 18 et 21
            offset = 18; 
            largeur = Convertir_Endian_To_Int(4);

            // hauteur correspond à i allant de 22 et 25
            offset = 22; 
            hauteur = Convertir_Endian_To_Int(4);

            // nb_Bit_Couleur correspond à i allant de 28 et 29
            offset = 28; 
            nb_Bit_Couleur = Convertir_Endian_To_Int(2);

            offset = 54;
            image = new Pixel[hauteur, largeur];

            //Transformation de l'image en matrice de pixels
            for (int i = 0; i < hauteur; i++) 
            {
                for (int j = 0; j < largeur; j++)
                {
                    image[i, j] = new Pixel(myfile[offset], myfile[offset + 1], myfile[offset + 2]);
                    offset += 3;
                }
            }
        }

        public MyImage(){ }

        /// Méthodes de conversion 

        /// <summary>
        /// Construction de la matrice à 3 dimensions
        /// </summary>
        /// <param name="fichier"></param>
        /// <returns></returns>
        public byte[,,] FichierDansMatrice(string fichier)
        {
            byte[] nbLignes = new byte[4]; 
            byte[] nbColonnes = new byte[4];
            this.header = new byte[14];
            this.headerinfo = new byte[40];

            this.imageByte = File.ReadAllBytes(fichier); //l'image deviens un tableau d'octets
            this.pixelsOctets = new byte[this.imageByte.Length - 54];

            for (int h = 0; h <= 13; h++)
            {
                this.header[h] = this.imageByte[h];
            }

            for (int h = 14; h <= 53; h++)
            {
                this.headerinfo[h - 14] = this.imageByte[h];
            }

            for (int h = 54; h < imageByte.Length; h++)
            {
                this.pixelsOctets[h - 54] = this.imageByte[h];
            }

            for (int h = 8; h <= 11; h++) // informations sur le nombre de lignes
            {
                nbLignes[h - 8] = this.headerinfo[h];
            }

            for (int h = 4; h <= 7; h++) // informations sur le nombre de colonnes 
            {
                nbColonnes[h - 4] = this.headerinfo[h];
            }

            this.hauteur = Convertir_Endian_To_Int(nbLignes);
            this.largeur = Convertir_Endian_To_Int(nbColonnes);

            this.pixelsRvb = new byte[this.hauteur, this.largeur, 3]; // Création de la matrice à 3 dimensions
            int j = 0; //index

            for (int i = 0; i < this.hauteur; i++)
            {
                for (int k = 0; k < this.largeur; k++)
                {
                    for (int h = 0; h < 3; h++)
                    {
                        this.pixelsRvb[i, k, h] = this.pixelsOctets[j];
                        j++;
                    }
                }
            }
            return this.pixelsRvb;
        }

        /// <summary>
        /// Retrouve le fichier en partant de son nom 
        /// </summary>
        /// <param name="fichier">Nom du fichier</param>
        /// <returns></returns>
        private void From_Image_To_File(string fichier)
        {
            myfile = File.ReadAllBytes(fichier);
        }

        /// <summary>
        /// Passe d'un tableau d'octets en Little Endian à int
        /// </summary>
        /// <param name="tableau"></param>
        /// <returns></returns>
        public int Convertir_Endian_To_Int(byte[] tableau)
        {
            int k = 0;
            int result = 0;

            for (int i = 0; i < 4; i++)
            {
                result += tableau[i] * (int)Math.Pow(256, k); 
                k++;
            }
            return result;
        }
        /// <summary>
        /// Passe des octets au format Little Endian à int, pour cela on fait une itération
        /// </summary>
        /// <param name="octet"> à convertir</param>
        /// <returns></returns>
        private int Convertir_Endian_To_Int(int octet, int fin = 0, int result = 0)
        {
            if (fin == octet)
            {
                return result;
            }
            else
            {
                result = result + Convert.ToInt32(myfile[offset + fin] * Math.Pow(256, fin));
                fin++;
                return Convertir_Endian_To_Int(octet, fin, result);
            }
        }

        /// <summary>
        /// Passe d'un int à un litlle endian
        /// </summary>
        /// <param name="taille"></param>
        /// <returns></returns>
        static byte[] Convertir_Int_To_Endian(int taille)
        {
            byte[] end = new byte[4]; //suppose qu'il est initialisé à 0

            if (taille < Math.Pow(2, 32))
            {
                int d = Math.DivRem(taille,(int)(Math.Pow(2, 24)), out int r);
                int c = Math.DivRem(r,(int)(Math.Pow(2, 16)), out int r2);
                int b = Math.DivRem(r2,(int)(Math.Pow(2, 8)), out int a);

                byte poidsFort = (byte)d;
                byte deuxieme = (byte)c;
                byte troisieme = (byte)b;
                byte poidsFaible = (byte)a;

                end[0] = poidsFaible;
                end[1] = troisieme;
                end[2] = deuxieme;
                end[3] = poidsFort;
            }

            return end;
        }
        


        /// Méthodes de traitement d'images

        /// <summary>
        /// Applique à chaque pixel une nuance de gris
        /// </summary>
        public void EnGris()
        {
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    image[i, j].NuanceGris();
                }
            }
            ImageFinale(0);
        }

        /// <summary>
        /// Applique à chaque pixel noir ou blanc
        /// </summary>
        public void NoirEtBlanc()
        {
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    image[i, j].NoirBlanc();
                }
            }
            ImageFinale(0); 
        }

        /// <summary>
        /// Symétrique de l'image
        /// </summary>
        public void Miroir()
        {
            Pixel[,] miroir = new Pixel[hauteur,largeur];

            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    miroir[i, miroir.GetLength(1) - 1 - j] = image[i, j];
                }
            }

            image = miroir;
            ImageFinale(0);
        }

        /// <summary>
        ///Agrandit une image en fonction du multiplicateur voulu
        /// </summary>
        public void Agrandir()
        {
            int multiplicateur = 0;
            bool entier = true;

            do
            {
                Console.Write("Veuillez saisir le multiplicateur pour l'agrandissement\n");
                try
                {
                    multiplicateur = Convert.ToInt32(Console.ReadLine());
                }
                catch
                {
                    Console.Write("\nSaisir un entier\n");
                    entier = false;
                }

            } while (entier == false);

            Pixel[,] agrandie = new Pixel[hauteur * multiplicateur, largeur * multiplicateur];

            for (int i = 0; i < agrandie.GetLength(0); i++)
            {
                for (int j = 0; j < agrandie.GetLength(1); j++)
                {
                    agrandie[i, j] = image[i / multiplicateur, j / multiplicateur];
                }
            }
            image = agrandie;

            ImageFinale(1, multiplicateur);
        }

        /// <summary>
        ///Agrandit une image en fonction du multiplicateur voulu
        /// </summary>
        public void AgrandirQRcode()
        {
            byte[] fichier = null;
            byte[] tab = new byte[4];

            Pixel[,] agrandie = new Pixel[hauteur * 5, largeur * 5];

            for (int i = 0; i < agrandie.GetLength(0); i++)
            {
                for (int j = 0; j < agrandie.GetLength(1); j++)
                {
                    agrandie[i, j] = image[i / 5, j / 5];
                }
            }
            image = agrandie;


            fichier = new byte[((largeur * 5) * (hauteur * 5) * 3) + 54];  // *3 car 3 couleurs
            tab = Convertir_Int_To_Endian(largeur * 5);

            for (int i = 0; i < 4; i++)
            {
                header[18 + i] = tab[i];
            }

            tab = Convertir_Int_To_Endian(hauteur * 5);

            for (int i = 0; i < 4; i++)
            {
                header[22 + i] = tab[i];
            }


            for (int i = 0; i <= 53; i++)
            {
                fichier[i] = header[i];
            }

            int index2 = 0;

            for (int i = 0; i < image.GetLength(0); i++)
            {
                for (int j = 0; j < image.GetLength(1); j++)
                {
                    fichier[56 + index2] = Convert.ToByte(image[i, j].Red);
                    fichier[55 + index2] = Convert.ToByte(image[i, j].Green);
                    fichier[54 + index2] = Convert.ToByte(image[i, j].Blue);
                    index2 += 3;
                }
            }

            try
            {
                File.WriteAllBytes("./ImageQRcode.bmp", fichier);
                Process.Start("ImageQRcode.bmp");
            }
            catch (IOException) { Console.WriteLine("erreur"); }
        }

        /// <summary>
        /// Retrécit une image en fonction du diviseur voulu
        /// </summary>
        public void Retrecir()
        {
            int diviseur = 0;
            bool entier;

            do
            {
                entier = true;
                Console.Write("Veuillez saisir le diviseur pour le rétrécisement\n");
                try
                {
                    diviseur = Convert.ToInt32(Console.ReadLine());
                }
                catch
                {
                    Console.Write("\nSaisir un entier\n"); 
                    entier = false;
                }

            } while (entier == false || diviseur <= 0); 

            Pixel[,] retrecie = new Pixel[hauteur / diviseur, largeur / diviseur];

            for (int i = 0; i < retrecie.GetLength(0); i++)
            {
                for (int j = 0; j < retrecie.GetLength(1); j++)
                {
                    retrecie[i, j] = image[i * diviseur, j * diviseur];
                }
            }

            image = retrecie;
            ImageFinale(2, diviseur); 
        }

        /// <summary>
        /// Redéfinition des hauteur et largeur puis remplissage
        /// </summary>
        public void Rotation()
        {
            int degre = 0;
            bool entier = true;

            do
            {
                Console.WriteLine("Saisir l'angle voulu \n");
                try
                {
                    degre = int.Parse(Console.ReadLine());
                }
                catch
                {
                    Console.WriteLine("Saisir un entier");
                    entier = false;
                }

            } while (entier == false);

            degre %= 360;
            
            double degRad = PI / 180 * degre; // En radian

            int largeurTemp = (int)(largeur * Abs(Sin(PI / 2 - degRad)) + hauteur * Abs(Sin(degRad)));
            int hauteurTemp = (int)(hauteur * Abs(Cos(degRad)) + largeur * Abs(Cos(PI / 2 - degRad)));

            while (largeurTemp % 4 != 0)
            {
                largeurTemp++;
            }

            while (hauteurTemp % 4 != 0)
            {
                hauteurTemp++;
            }

            Pixel[,] temporaire = new Pixel[hauteurTemp, largeurTemp];

            int indexI = 0;
            int indexJ = 0;
            if (degre == 90 || degre == 180 || degre == 270) //Cas simples
            {
                if (degre == 90)
                {
                    for (int i = 0; i < hauteur; i++)
                    {
                        for (int j = 0; j < largeur; j++)
                        {
                            temporaire[largeur - j - 1, i] = image[i, j];
                        }
                    }

                }
                if (degre == 180)
                {
                    for (int i = 0; i < hauteur; i++)
                    {
                        for (int j = 0; j < largeur; j++)
                        {
                            temporaire[hauteur - i - 1, largeur - 1 - j] = image[i, j];
                        }
                    }

                }
                if (degre == 270)
                {
                    for (int i = 0; i < hauteur; i++)
                    {
                        for (int j = 0; j < largeur; j++)
                        {
                            temporaire[j, hauteur - 1 - i] = image[i, j];
                        }
                    }
                }
            }
            else
            {
                if (degre > 0 && degre < 90)
                {
                    indexI = (int)((largeur - 1) * Abs((Cos(PI / 2 - degRad))));
                    indexJ = 0;
                }
                if (degre > 90 && degre < 180)
                {
                    indexI = hauteurTemp - 1;
                    indexJ = (int)((largeur - 1) * Cos(PI - degRad));
                }
                if (degre > 180 && degre < 270)
                {
                    indexI = (int)((hauteur - 1) * Abs(Cos(degRad - PI)));
                    indexJ = largeurTemp - 1;
                }
                if (degre > 270)
                {
                    indexI = 0;
                    indexJ = (int)((hauteur - 1) * Abs(Cos(degRad - (3 * PI / 2))));
                }

                // Remplissage

                for (int i = 0; i < hauteur; i++)
                {
                    for (int j = 0; j < largeur; j++)
                    {
                        temporaire[indexI + (int)(Cos(degRad) * i - Sin(degRad) * j), indexJ + (int)(Sin(degRad) * i + Cos(degRad) * j)] = image[i, j];
                    }
                }

                // Bords blancs

                for (int i = 0; i < hauteurTemp; i++)
                {
                    for (int j = 0; j < largeurTemp; j++)
                    {
                        if (temporaire[i, j] == null)
                        {
                            temporaire[i, j] = new Pixel(255, 255, 255);
                        }
                    }
                }
                
            }

            image = temporaire;
            largeur = largeurTemp;
            hauteur = hauteurTemp;

            ImageFinale(3);
        }

        /// <summary>
        /// Permets l'application de la matrice de convolution
        /// </summary>
        /// <param name="convolution">matrice de convolution</param>
        /// <param name="flou">applique (ou non) le flou</param>
        /// <returns></returns>
        private void Convolution(int[,] convolution, bool flou)
        {
            Pixel[,] imageTemporaire = new Pixel[hauteur, largeur];

            for (int i = 1; i < hauteur - 1; i++)
            {
                for (int j = 1; j < largeur - 1; j++)
                {

                    int valueBlue = image[i - 1, j - 1].Blue * convolution[0, 0] + image[i - 1, j].Blue * convolution[0, 1] + image[i - 1, j + 1].Blue * convolution[0, 2] +
                                    image[i, j - 1].Blue * convolution[1, 0] + image[i, j].Blue * convolution[1, 1] + image[i, j + 1].Blue * convolution[1, 2] +
                                    image[i + 1, j - 1].Blue * convolution[2, 0] + image[i + 1, j].Blue * convolution[2, 1] + image[i + 1, j + 1].Blue * convolution[2, 2];


                    int valueGreen = image[i - 1, j - 1].Green * convolution[0, 0] + image[i - 1, j].Green * convolution[0, 1] + image[i - 1, j + 1].Green * convolution[0, 2] +
                                     image[i, j - 1].Green * convolution[1, 0] + image[i, j].Green * convolution[1, 1] + image[i, j + 1].Green * convolution[1, 2] +
                                     image[i + 1, j - 1].Green * convolution[2, 0] + image[i + 1, j].Green * convolution[2, 1] + image[i + 1, j + 1].Green * convolution[2, 2];


                    int valueRed = image[i - 1, j - 1].Red * convolution[0, 0] + image[i - 1, j].Red * convolution[0, 1] + image[i - 1, j + 1].Red * convolution[0, 2] +
                                    image[i, j - 1].Red * convolution[1, 0] + image[i, j].Red * convolution[1, 1] + image[i, j + 1].Red * convolution[1, 2] +
                                    image[i + 1, j - 1].Red * convolution[2, 0] + image[i + 1, j].Red * convolution[2, 1] + image[i + 1, j + 1].Red * convolution[2, 2];

                    if (flou)
                    {
                        valueBlue = valueBlue / 9;
                        valueGreen = valueGreen / 9;
                        valueRed = valueRed / 9;
                    }

                    if (valueBlue < 0)
                    {
                        valueBlue = 0;
                    }

                    if (valueBlue > 255)
                    {
                        valueBlue = 255;
                    }

                    if (valueGreen < 0)
                    {
                        valueGreen = 0;
                    }

                    if (valueGreen > 255)
                    {
                        valueGreen = 255;
                    }

                    if (valueRed < 0)
                    {
                        valueRed = 0;
                    }
                    if (valueRed > 255)
                    {
                        valueRed = 255;
                    }

                    imageTemporaire[i, j] = new Pixel(valueBlue, valueGreen, valueRed);
                }
            }

            imageTemporaire[0, 0] = imageTemporaire[1, 1]; // coin supérieur gauche
            imageTemporaire[hauteur - 1, 0] = imageTemporaire[hauteur - 2, 1]; // coin inférieur gauche
            imageTemporaire[0, largeur - 1] = imageTemporaire[1, largeur - 2]; // coin supérieur droit
            imageTemporaire[hauteur - 1, largeur - 1] = imageTemporaire[hauteur - 2, largeur - 2]; // coin inférieur droit

            for (int ligne = 1; ligne < hauteur - 1; ligne++)
            {
                imageTemporaire[ligne, largeur - 1] = imageTemporaire[ligne, largeur - 2]; // bord droit
                imageTemporaire[ligne, 0] = imageTemporaire[ligne, 1]; // bord gauche
            }

            for (int colonne = 1; colonne < largeur - 1; colonne++)
            {
                imageTemporaire[hauteur - 1, colonne] = imageTemporaire[hauteur - 2, colonne]; // inférieur
                imageTemporaire[0, colonne] = imageTemporaire[1, colonne]; // bord supérieur
            }

            image = imageTemporaire;
            ImageFinale(0); /// Pas besoin de modifier le header
        }

        /// <summary>
        /// Applique les filtres grâce à la matrice de convolution
        /// </summary>
        public void FiltreMatConv()
        {
            int numeroImage = -1;
            int[,] convolution = null;
            bool estFlou = false;

            Console.WriteLine("Choisissez le filtre que vous souhaitez appliquer.\n" +
            "\n1. Détection de contour" +
            "\n2. Renforcement des bords" +
            "\n3. Flou" +
            "\n4. Repoussage" +
            "\n\n0. Sortir\n");
            do
            {
                Console.Write("> ");
                try
                {
                    numeroImage = Convert.ToInt32(Console.ReadLine());
                }
                catch
                {
                    Console.Write("Entrez un numéro valide\n");
                }

            } while (numeroImage != 0 && numeroImage != 1 && numeroImage != 2 && numeroImage != 3 && numeroImage != 4);

            switch (numeroImage)
            {
                case 1:
                    int[,] contour3 = { { -1, -1, -1 }, { -1, 8, -1 }, { -1, -1, -1 } };
                    convolution = contour3;
                    break;
                case 2:
                    int[,] renforcement = { { 0, -1, 0 }, { -1, 5, -1 }, { 0, -1, 0 } };
                    convolution = renforcement;
                    break;
                case 3:
                    int[,] flou = { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };
                    convolution = flou;
                    estFlou = true;
                    break;
                case 4:
                    int[,] repoussage = { { -2, -1, 0 }, { -1, 1, 1 }, { 0, 1, 2 } };
                    convolution = repoussage;
                    break;
            }

            Convolution(convolution, estFlou);
        }

        /// <summary>
        /// Création d'une fractale 
        /// </summary>
        public void Fractale()
        {
            for (int x = 0; x < image.GetLength(0); x++)
            {
                for (int y = 0; y < image.GetLength(1); y++)
                {

                    double reel = Convert.ToDouble((x - (image.GetLength(0) / 2)) / Convert.ToDouble(image.GetLength(0) / 4));
                    double imagi = Convert.ToDouble((y - (image.GetLength(1) / 2)) / Convert.ToDouble(image.GetLength(1) / 4));
                    int i = 0;
                    Complexe z = new Complexe(0, 0);

                    Complexe c = new Complexe(reel, imagi);

                    while (i < 100)
                    {
                        z.AuCarre();
                        z.Ajouter(c);

                        if (z.Module() > 2)
                           break; 
                        i++;
                    }

                    if (i < 100)
                    {
                        image[x, y] = new Pixel(255 * i / 100, 200 * i / 100, 203 * i / 100);
                    }
                    else
                    {
                        image[x, y] = new Pixel(0, 0,0);
                    }
                }
            }
            ImageFinale(4);
        }
       
        /// <summary>
        /// Génère l'histogramme de l'image choisie
        /// </summary>
        public void Histogramme()
        {
            Pixel[,] histograme = new Pixel[image.GetLength(0), image.GetLength(1)];

            for (int i = 0; i < image.GetLength(0); i++)
            {
                int[] addition = { 0, 0, 0 };

                for (int j = 0; j < image.GetLength(1); j++)
                {
                    addition[0] = addition[0] + image[i, j].Red;
                    addition[1] = addition[1] + image[i, j].Green;
                    addition[2] = addition[2] + image[i, j].Blue;
                }
                int[] hCouleurs = { addition[0] / 255, addition[1] / 255, addition[2] / 255 };

                for (int k = 0; k < image.GetLength(1); k++)
                {
                    int rouge = 0;
                    int bleu = 0;
                    int vert = 0;

                    if (k <= hCouleurs[0])
                    {
                        rouge = 255;
                    }

                    if (k <= hCouleurs[1])
                    {
                        vert = 255;
                    }

                    if (k <= hCouleurs[2])
                    {
                        bleu = 255;
                    }

                    histograme[i, k] = new Pixel(rouge, vert, bleu);
                }
            }
            image = histograme;
            ImageFinale(4);
        }

        /// <summary>
        /// Code une image dans une autre
        /// </summary>
        /// <param name="image2"></param>
        /// <returns></returns>
        public Pixel[,] ImageDansImage(Pixel[,] image2)
        {

            Pixel[,] fusionImage = new Pixel[hauteur, largeur];           
            int r1 = 0;
            int r2 = 0;
            int v1 = 0;
            int v2 = 0;
            int b1 = 0;
            int b2 = 0;

            for (int i = 0; i < image.GetLength(0); i++)
            {
                for (int j = 0; j < image.GetLength(1); j++)
                {
                    r1 = image[i, j].Red ;
                    v1 = image[i, j].Green ;
                    b1 = image[i, j].Blue ;


                    if (i < image2.GetLength(0) && j < image2.GetLength(1))
                    {
                        r2 = image2[i, j].Red / 16;
                        v2 = image2[i, j].Green / 16;
                        b2 = image2[i, j].Blue / 16;
                        r2 *= 16;
                        v2 *= 16;
                        b2 *= 16;

                        int[] nouveauPix = { r1 + r2, v1 + v2, b1 + b2 };
                        fusionImage[i, j] = new Pixel(nouveauPix);
                    }
                    else
                    {
                        int[] nouveauPix = { r1, v1, b1 }; // Si sortie de la 2ème image
                        fusionImage[i, j] = new Pixel(nouveauPix);
                    }
                }
            }
            return fusionImage;
        }

        /// <summary>
        /// Décrypte une image (problème)
        /// </summary>
        /// <returns></returns>
        public Pixel[,] DecrypteImage()
        {
            Pixel[,] imageDecrypt = new Pixel[hauteur, largeur];
            for (int i = 0; i < image.GetLength(0); i++)
            {
                for (int j = 0; j < image.GetLength(1); j++)
                {
                    Math.DivRem(image[i, j].Red, 16, out int rougeC);
                    Math.DivRem(image[i, j].Green, 16, out int vertC);
                    Math.DivRem(image[i, j].Blue, 16, out int bleuC);

                    rougeC *= 16;
                    vertC *= 16;
                    bleuC *= 16;
                    int[] pixels = { rougeC, vertC, bleuC };
                    imageDecrypt[i, j] = new Pixel(pixels);
                }
            }
            return imageDecrypt;
        }

        ///INNOVATIONS

        /// <summary>
        /// Rend une image Négative
        /// </summary>
        public void Negatif()
        {
            for (int i = 0; i < this.Hauteur; i++)
            {
                for (int j = 0; j < this.Largeur; j++)
                {
                    this.image[i, j].Red = (byte)(255 - this.image[i, j].Red);
                    this.image[i, j].Green = (byte)(255 - this.image[i, j].Green);
                    this.image[i, j].Blue = (byte)(255 - this.image[i, j].Blue);
                }
            }
            ImageFinale(0);
        }

        /// <summary>
        /// Rend une image en Sépia (en teintes brunes)
        /// </summary>
        public void Sepia()
        {
            int valeurGris = 0;

            for (int i = 0; i < this.Hauteur; i++)
            {
                for (int j = 0; j < this.Largeur; j++)
                {
                    valeurGris = (this.image[i, j].Red + this.image[i, j].Green + this.image[i, j].Blue) / 3;
                    this.image[i, j].Red = (byte)((valeurGris + 162) / 2); // r = 162, v =  128 et b = 101 correspondent aux valeurs Sépia traditionnelles 
                    this.image[i, j].Green = (byte)((valeurGris + 128) / 2);
                    this.image[i, j].Blue = (byte)((valeurGris + 101) / 2);
                }
            }
            ImageFinale(0);
        }

        /// <summary>
        /// Retourne le QRcode
        /// </summary>
        /// <param name="Qrcode"> matrice contenant le QR code (version 1 ou 2) générée par la classe QRCode </param>
        /// <returns></returns>
        public void QRCodeImage(byte[,,] Qrcode) 
        {
            pixelsRvb = Qrcode;
            int nb0FinLigne = 0;
            int QrH = Qrcode.GetLength(0);
            int QrL = Qrcode.GetLength(1);

            //On ajoute 1 octet jusqu'à ce que l'image bitmap soit égale à 4
            int OctLigne = QrL * 3;
            bool div4 = false;

            do
            {
                nb0FinLigne++;
                OctLigne += nb0FinLigne;

                if (OctLigne % 4 == 0) 
                {
                    div4 = true; 
                }

            } while (div4 == false);

            int tailleQr = OctLigne * QrH; 
            
            int tailleFichier = tailleQr + 54;
            imageByte = new byte[tailleFichier];


            for (int i = 0; i < 14; i++)
            {
                imageByte[i] = this.header[i];
            }

            for (int i = 14; i < 54; i++)
            {
                imageByte[i] = this.headerinfo[i - 14];
            }

            byte[] tabFic = new byte[4];
            tabFic = Convertir_Int_To_Endian(tailleFichier);

            byte[] tabLignes = new byte[4];
            tabLignes = Convertir_Int_To_Endian(QrH);

            byte[] tabColonnes = new byte[4];
            tabColonnes = Convertir_Int_To_Endian(QrL);

            byte[] tabImageTaille = new byte[4];
            tabImageTaille = Convertir_Int_To_Endian(tailleQr);

            // Remplissage header et headerinfo

            for (int i = 2; i < 6; i++) // Taille fichier
            {
                imageByte[i] = tabFic[i - 2];
            }
            for (int i = 18; i < 22; i++) // Largeur image
            {
                imageByte[i] = tabColonnes[i - 18];
            }
            for (int i = 22; i < 26; i++) // Hauteur image
            {
                imageByte[i] = tabLignes[i - 22];
            }
            for (int i = 34; i < 38; i++) // Taille image
            {
                imageByte[i] = tabImageTaille[i - 34];
            }

            int indexoctet = 54;

            for (int i = Qrcode.GetLength(0) - 1; i >= 0; i--)
            {
                for (int j = 0; j < Qrcode.GetLength(1); j++)
                {
                    imageByte[indexoctet] = Qrcode[i, j, 0];
                    indexoctet++;
                    imageByte[indexoctet] = Qrcode[i, j, 1];
                    indexoctet++;
                    imageByte[indexoctet] = Qrcode[i, j, 2];
                    indexoctet++;
                }

                for (int zeros = 0; zeros < nb0FinLigne; zeros++)
                {
                    imageByte[indexoctet] = 0;
                    indexoctet++;
                }
            }

            try
            {
                File.WriteAllBytes("./ImageQRcode.bmp", imageByte);  
                Process.Start("ImageQRcode.bmp");
            }
            catch (IOException) { Console.WriteLine("erreur"); }
        }

        /// Méthoode qui créée la nouvelle image

        /// <summary>
        /// Modifications du header, écrit l'image et l'affiche
        /// </summary>
        /// <param name="operation"> </param>
        /// <param name="index"> pour agrandissement et retrecissement </param>
        private void ImageFinale(int fonction, int index = 0)
        {
            byte[] fichier = null; 
            byte[] tab = new byte[4];

            switch (fonction)
            {
                case 0: // Gris, Noir et blanc
                    fichier = new byte[tailleFichier];
                    break;

                case 1: // Agrandir
                    fichier = new byte[((largeur * index) * (hauteur * index) * 3) + 54];  // *3 car 3 couleurs
                    tab = Convertir_Int_To_Endian(largeur * index);

                    for (int i = 0; i < 4; i++)
                    {
                        header[18 + i] = tab[i];
                    }

                    tab = Convertir_Int_To_Endian(hauteur * index);

                    for (int i = 0; i < 4; i++)
                    {
                        header[22 + i] = tab[i];
                    }
                    break;

                case 2: // Rétrécir
                    fichier = new byte[((largeur / index) * (hauteur / index) * 3) + 54]; 

                    tab = Convertir_Int_To_Endian(largeur / index);

                    for (int i = 0; i < 4; i++)
                    {
                        header[18 + i] = tab[i];
                    }

                    tab = Convertir_Int_To_Endian(hauteur / index);

                    for (int i = 0; i < 4; i++)
                    {
                        header[22 + i] = tab[i];
                    }
                    break;

                case 3: // Rotation                  
                    tailleImage = largeur * hauteur * 5;
                    tailleFichier = tailleImage + TailleOffset;
                    fichier = new byte[tailleFichier];

                    tab = Convertir_Int_To_Endian(largeur);
                    
                    for (int i = 0; i < 4; i++)
                    {
                        header[18 + i] = tab[i];
                    }

                    tab = Convertir_Int_To_Endian(hauteur);
                    
                    for (int i = 0; i < 4; i++)
                    {
                        header[22 + i] = tab[i];
                    }
                    break;

                case 4: // Fractale et Histogramme
                    tailleImage = largeur * hauteur * 5;
                    tailleFichier = tailleImage + TailleOffset;
                    fichier = new byte[tailleFichier];
                    tab = Convertir_Int_To_Endian(largeur);

                    for (int i = 0; i < 4; i++)
                    {
                        header[18 + i] = tab[i];
                    }

                    tab = Convertir_Int_To_Endian(hauteur);

                    for (int i = 0; i < 4; i++)
                    {
                        header[22 + i] = tab[i];
                    }
                    break;
            }
           

            for (int i = 0; i <= 53; i++)
            {
                fichier[i] = header[i];
            }

            int index2 = 0;

            for (int i = 0; i < image.GetLength(0); i++)
            {
                for (int j = 0; j < image.GetLength(1); j++)
                {
                    fichier[56 + index2] = Convert.ToByte(image[i, j].Red);
                    fichier[55 + index2] = Convert.ToByte(image[i, j].Green);
                    fichier[54 + index2] = Convert.ToByte(image[i, j].Blue);
                    index2 += 3;
                }
            }

            File.WriteAllBytes("imageAvecEffet.bmp", fichier);
            Process.Start("imageAvecEffet.bmp");
        }

        /// <summary>
        /// Affiche l'image de la même manière que ImageFinale() mais pour les fonctions de cryptage
        /// </summary>
        /// <param name="imageF"></param>
        /// <param name="fichier"></param>
        public void AfficherImage(Pixel[,] imageF, string fichier)
        {
            int hauteurF = imageF.GetLength(0); 
            int largeurF = imageF.GetLength(1); 
            int index = 0;

            int tailleF = hauteurF * largeurF * 3 + 54;
            byte[] image1 = File.ReadAllBytes(fichier);
            byte[] image2 = new byte[tailleF];

            for (int i = 0; i <= 53; i++) 
            {
                image2[i] = image1[i]; 
            }

            for (int i = 2; i <= 5; i++) //taille
            {
                image1[i] = Convertir_Int_To_Endian(tailleF)[index];
                index++; 
            } 

            index = 0;
            for (int k = 18; k <= 21; k++) // largeur
            {
                image2[k] = Convertir_Int_To_Endian(largeurF)[index];
                index++; 
            }

            index = 0;
            for (int k = 22; k <= 25; k++) // hauteur
            {
                image2[k] = Convertir_Int_To_Endian(hauteurF)[index];
                index++; 
            }

            index = 54;
            for (int i = 0; i < hauteurF; i++)
            {
                for (int j = 0; j < largeurF; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        byte bytepixel = (byte)imageF[i, j].Pixels[k];
                        image2[index] = bytepixel; index++;
                    }
                }
            }
            File.WriteAllBytes("imageAvecEffet.bmp", image2);
            Process.Start("imageAvecEffet.bmp");
        }
    }
}

