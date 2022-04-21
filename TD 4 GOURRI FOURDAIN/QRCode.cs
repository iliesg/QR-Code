using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TD_4_GOURRI_FOURDAIN
{
    class QRCode : MyImage
    {
        string chain;
        string qrcodechain_global;
        string version;

        public QRCode(string chain, string version)
        {
            this.chain = chain;
            this.version = version;
            if (this.version == "1") 
            { 
                this.qrcodechain_global = QRFinalV1(EcritureChaine()); 
            }
            if (this.version == "2") 
            { 
                this.qrcodechain_global = QRFinalV2(EcritureChaine()); 
            }

        }

        #region Accès
        public string Chaîne
        {
            get { return this.chain; }
        }
        public string QRChainWithErr
        {
            get { return this.qrcodechain_global; }
        }
        #endregion


        #region Conversion

        /// <summary>
        /// Passe d'un nombre entier à un nombre binaire.
        /// </summary>
        /// <param name="octet">Nombre binaire à convertir</param>
        /// <param name="longueur">Longueur de la chaine de bits du nombre binaire</param>
        /// <returns></returns>
        string IntToBinary(int octet, int longueur)
        {  
            int division = 0;
            int reste = 0;
            string result = "";

            do
            {
                division = octet / 2;
                reste = octet % 2;
                result = reste + result;
                octet = division;
            } while (octet > 1);

            result = octet + result;

            int tailleBinaire = result.Length;
            int aCombler = longueur - tailleBinaire;
            int i = 0;
            // On comble avec des 0 au début de la chaine de bits afin que le nombre binaire ait la longueur désirée
            while (i < aCombler) 
            {
                result = "0" + result;
                i++;
            }
            return result;
        }

        /// <summary>
        /// Passe d'un caractère en un entier alphanumérique correspondant
        /// </summary>
        /// <param name="caractere">Caractère à convertir</param>
        /// <returns></returns>
        private int ConvertLetterToQRInt(char caractere)
        {
            int caractereInt = 0;
            int convEnChar = Convert.ToInt32(caractere);

            if (convEnChar >= 65 && convEnChar <= 90)
            {
                caractereInt = convEnChar - 55;
            }
            if (convEnChar >= 38 && convEnChar < 39)
            {
                caractereInt = convEnChar - 1;
            }

            if (caractere == ' ') { caractereInt = 36; }
            if (caractere == '$') { caractereInt = 37; }
            if (caractere == '%') { caractereInt = 38; }
            if (caractere == '*') { caractereInt = 39; }
            if (caractere == '+') { caractereInt = 40; }
            if (caractere == '-') { caractereInt = 41; }
            if (caractere == '.') { caractereInt = 42; }
            if (caractere == '/') { caractereInt = 43; }
            if (caractere == ':') { caractereInt = 44; }

            return caractereInt;
        }

        /// <summary>
        /// Encode une chaine de caractères en une chaine binaire.
        /// </summary>
        /// <param name="chain">Chaine de caractères à encoder.</param>
        /// <returns></returns>
        private string Convert_Chain_To_Bits()
        {
            int longueurChaine = chain.Length;
            int indexPaires = 0; //le nombre de découpages fait pour avoir un maximum de paires
            char car1 = ' ';
            char car2 = ' ';
            string chaineBin = "";
            bool a = longueurChaine % 2 == 0;

            if (a == true)
            {
                indexPaires = longueurChaine - 1;
            }
            else
            {
                indexPaires = longueurChaine - 2;
            }

            for (var i = 0; i < indexPaires; i += 2)
            {
                car1 = chain[i];
                car2 = chain[i + 1];
                int int1 = ConvertLetterToQRInt(car1);
                int int2 = ConvertLetterToQRInt(car2);

                double paire = Math.Pow(45, 1) * int1 + Math.Pow(45, 0) * int2;
                string paireBin = IntToBinary(Convert.ToInt32(paire), 11);
                chaineBin += paireBin;
            }

            if (a == false) //pour le dernier caractère 
            {
                car1 = chain[longueurChaine - 1];
                int int1 = ConvertLetterToQRInt(car1);
                double dernier = Math.Pow(45, 0) * int1;
                string dernierBin = IntToBinary(Convert.ToInt32(dernier), 6);
                chaineBin += dernierBin;
            }

            return chaineBin;
        }

        /// <summary>
        /// Convertit une chaine de string composée en une chaine binaire en un tableau d'octets
        /// </summary>
        /// <param name="chaineBin"> à convertir en tableau d'octets </param>
        /// <returns></returns>
        public byte[] Convert_String_To_Tab_Byte(string chaineBin)
        {
            int nbOctets = chaineBin.Length / 8; //un octet est composé de 8 bits
            string[] tabOctetString = new string[nbOctets];
            int debut = 0;
            int longueur = 0;

            for (int i = 0; i < nbOctets; i++)
            {
                string octet = chaineBin.Substring(debut, 8);
                tabOctetString[i] = octet;
                longueur += octet.Length;
                debut += 8;
            }

            byte[] tabOctet = new byte[tabOctetString.Length];

            for (int j = 0; j < tabOctetString.Length; j++)
            {
                byte b = Convert.ToByte(tabOctetString[j], 2);

                tabOctet[j] = b;
            }
            return tabOctet;
        }

        /// <summary>
        /// Retourne la chaine binaire correspondant au mode, la taille de la chaine, les données encodées de la chaine, la terminaison, et les bits pour compléter l'octet.
        /// </summary>
        /// <returns></returns>
        private string EcritureChaineInter()
        {
            int longueurChaine = chain.Length;
            string mode = "0010"; // indicateur du mode alphanumérique sur 4 bits 
            string nbCarBin = "";
            string chaineBin = "";
            string total = "";
            int tailleQr = 0;

            // version 1 : taille de 152 bits
            if (version == "1") 
            {
                tailleQr = 152; 
            }

            //version 2 : taille de 272 bits
            if (version == "2") 
            { 
                tailleQr = 272; 
            }

            nbCarBin = IntToBinary(longueurChaine, 9); // indicateur de longueur sur 9 bits
            chaineBin = Convert_Chain_To_Bits();

            // total correspond à l'enchainements des bits, du nombre de caractères, et de la chaine chaine convertie en bits (avant terminaison)
            total += mode;
            total += nbCarBin;
            total += chaineBin;

            int taille = total.Length;
            int dif = tailleQr - taille; 

            if (dif >= 4)
            {
                total += "0000";
            }
            else
            {
                for (var i = 0; i < dif; i++)
                {
                    total += "0";
                }
            }

            int tailleFinale = total.Length;
            bool div8 = tailleFinale % 8 == 0;

            do
            {
                total += "0";
                tailleFinale = total.Length;

                if (tailleFinale % 8 == 0) 
                { 
                    div8 = true; 
                }

            } while (div8 == false);

            return total;
        }

        /// <summary>
        /// Retourne la chaine inégrale de bits de la taille max de bits de la version du code QR (avant l'ajout de l'erreur)
        /// </summary>
        /// <returns></returns>
        public string EcritureChaine()
        {
            string chaineInter = EcritureChaineInter();
            string oct1 = "11101100";
            string oct2 = "00010001";
            int tailleQR = 0;

            if (version == "1") 
            { 
                tailleQR = 152; 
            }
            if (version == "2") 
            { 
                tailleQR = 272; 
            }

            int difference = tailleQR - chaineInter.Length;
            int octAjouter = difference / 8; //détermine le nombre d'octets à rajouter

            if (octAjouter % 2 == 0)
            {
                for (var i = 0; i < octAjouter / 2; i++)
                {
                    chaineInter += oct1;
                    chaineInter += oct2;
                }
            }
            else
            {
                var i = 0;
                while (i < octAjouter - 1)
                {
                    chaineInter += oct1;
                    i++;
                    chaineInter += oct2;
                    i++;
                }
                chaineInter += oct1;
            }
            return chaineInter;
        }

        /// <summary>
        /// Retourne la chaine de correction d'erreur en octets pour la version 1
        /// </summary>
        /// <param name="bits1"> 152 bits</param>
        /// <returns></returns>
        public byte[] ErreurV1(byte[] bits1)
        {
            byte[] result = ReedSolomonAlgorithm.Encode(bits1, 7, ErrorCorrectionCodeType.QRCode);
            return result;
        }

        /// <summary>
        /// etourne la chaine de correction d'erreur en octets pour la version 2
        /// </summary>
        /// <param name="bits2"> 272 bit</param>
        /// <returns></returns>
        public byte[] ErreurV2(byte[] bits2)
        {
            byte[] result = ReedSolomonAlgorithm.Encode(bits2, 10, ErrorCorrectionCodeType.QRCode);
            return result;
        }

        /// <summary>
        /// Donne le QR code à placer ensuite dans la matrice
        /// </summary>
        /// <param name="chaineInter"> chaine du QR code avant l'ajout de la correction d'erreurs</param>
        /// <returns></returns>
        public string QRFinalV1(string chaineInter)
        {
            byte[] bits = Convert_String_To_Tab_Byte(chaineInter);
            byte[] correction = ErreurV1(bits);
            string correctionBin = "";

            foreach (byte b in correction)
            {
                string binaire = IntToBinary(Convert.ToInt32(b), 8);
                correctionBin += binaire;
            }
            return chaineInter + correctionBin;
        }
        public string QRFinalV2(string chaineInter)
        {
            byte[] bits = Convert_String_To_Tab_Byte(chaineInter);
            byte[] correction = ErreurV2(bits);
            string correctionBin = "";

            foreach (byte b in correction)
            {
                string binaire = IntToBinary(Convert.ToInt32(b), 8);
                correctionBin += binaire;
            }
            return chaineInter + correctionBin;
        }

        #endregion

        #region Écriture de l'image
        

        /// <summary>
        /// Ajoute les pixels de manière horizontale
        /// </summary>
        /// <param name="Qrcode">Matrice contenant le Qr code </param>
        /// <param name="pixelh">Coordonnée horizontale </param>
        /// <param name="pixelv">Coordonnée verticale </param>
        /// <param name="longueurH"> où ajouter les pixels honrizontalement </param>
        /// <param name="r"></param>
        /// <param name="v"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private byte[,,] RemplissageHorizontal(byte[,,] Qrcode, int pixelh, int pixelv, int longueurH, byte r, byte v, byte b)
        {
            for (int i = 0; i < longueurH; i++)
            {
                int x = pixelh + i;
                Qrcode = ColorationNouB(Qrcode, x, pixelv, r, v, b);
            }
            return Qrcode;
        }

        /// <summary>
        /// Ajoute les pixels de manière verticale
        /// </summary>
        /// <param name="Qrcode">Matrice contenant le Qr code </param>
        /// <param name="pixelh">Coordonnée horizontale </param>
        /// <param name="pixelv">Coordonnée verticale </param>
        /// <param name="longueurV">où ajouter les pixels honrizontalement verticalement </param>
        /// <param name="r"></param>
        /// <param name="v"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private byte[,,] RemplissageVertical(byte[,,] Qrcode, int pixelh, int pixelv, int longueurV, byte r, byte v, byte b)
        {
            for (int i = 0; i < longueurV; i++)
            {
                int y = pixelv + i;
                Qrcode = ColorationNouB(Qrcode, pixelh, y, r, v, b);
            }
            return Qrcode;
        }

        /// <summary>
        /// Ajoute les pixels pour les motifs de recherches (carrées), de manière horizontale puis verticale
        /// </summary>
        /// <param name="Qrcode"> Matrice contenant le Qr code </param>
        /// <param name="x"> coordonnée horizontale </param>
        /// <param name="y"> coordonnée verticale </param>
        /// <param name="longueur"> où ajouter les pixels </param>
        /// <returns></returns>
        private byte[,,] RemplissageCarre(byte[,,] Qrcode, int x, int y, int longueur, byte r, byte v, byte b)
        {
            Qrcode = RemplissageHorizontal(Qrcode, x, y, longueur, r, b, b);
            Qrcode = RemplissageHorizontal(Qrcode, x, y + longueur - 1, longueur, r, v, b);

            Qrcode = RemplissageVertical(Qrcode, x, y, longueur, r, v, b);
            Qrcode = RemplissageVertical(Qrcode, x + longueur - 1, y, longueur, r, v, b);

            return Qrcode;
        }

        /// <summary>
        /// Place les motifs de recherche à leur emplacemenent
        /// </summary>
        /// <param name="Qrcode"> Matrice contenant le Qr code </param>
        /// <param name="largeur"> largeur de la matrice </param>
        /// <returns></returns>
        private byte[,,] PlacerMotifsRe(byte[,,] Qrcode, int largeur)
        {
            // Haut Gauche : carré noir extérieur 7x7, carré blanc 5x5, carré noir 3x3, carré noir de 1x1
            Qrcode = RemplissageCarre(Qrcode, 0, 0, 7, 0, 0, 0);
            Qrcode = RemplissageCarre(Qrcode, 1, 1, 5, 255, 255, 255);
            Qrcode = RemplissageCarre(Qrcode, 2, 2, 3, 0, 0, 0);
            Qrcode = RemplissageCarre(Qrcode, 3, 3, 1, 0, 0, 0);

            //Haut Droit 
            Qrcode = RemplissageCarre(Qrcode, largeur - 7, 0, 7, 0, 0, 0);
            Qrcode = RemplissageCarre(Qrcode, largeur - 7 + 1, 1, 5, 255, 255, 255);
            Qrcode = RemplissageCarre(Qrcode, largeur - 7 + 2, 2, 3, 0, 0, 0);
            Qrcode = RemplissageCarre(Qrcode, largeur - 7 + 3, 3, 1, 0, 0, 0);

            //Bas Gauche
            Qrcode = RemplissageCarre(Qrcode, 0, largeur - 7 + 0, 7, 0, 0, 0);
            Qrcode = RemplissageCarre(Qrcode, 1, largeur - 7 + 1, 5, 255, 255, 255);
            Qrcode = RemplissageCarre(Qrcode, 2, largeur - 7 + 2, 3, 0, 0, 0);
            Qrcode = RemplissageCarre(Qrcode, 3, largeur - 7 + 3, 1, 0, 0, 0);

            return Qrcode;
        }

        /// <summary>
        /// Place le module sombre en fonction de la version
        /// </summary>
        /// <param name="Qrcode"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        private byte[,,] PlacerModSomb(byte[,,] Qrcode, string version)
        {
            int v = Convert.ToInt32(version);
            Qrcode = ColorationNouB(Qrcode, 8, (4 * v) + 9, 0, 0, 0);
            return Qrcode;
        }

        private byte[,,] PlacerMotifsSynchro(byte[,,] Qrcode)
        {
            int pixelv = 6;
            int pixelh = 6;
            byte noirOuBlanc = 0;

            for (int i = 0; i < 5; i++)
            {
                int x = 8 + i;
                Qrcode = ColorationNouB(Qrcode, x, pixelv, noirOuBlanc, noirOuBlanc, noirOuBlanc);

                if (noirOuBlanc == 0)
                {
                    noirOuBlanc = 255;
                }
                else
                {
                    noirOuBlanc = 0;
                }
            }

            noirOuBlanc = 0;
            for (int i = 0; i < 5; i++)
            {
                int y = 8 + i;
                Qrcode = ColorationNouB(Qrcode, pixelh, y, noirOuBlanc, noirOuBlanc, noirOuBlanc);
                if (noirOuBlanc == 0)
                {
                    noirOuBlanc= 255;
                }
                else
                {
                    noirOuBlanc = 0;
                }
            }
            return Qrcode;
        }

        private byte[,,] PlacerSeparateur(byte[,,] Qrcode, int largeur)
        {
            // Haut gauche
            Qrcode = RemplissageHorizontal(Qrcode, 0, 7, 8, 255, 255, 255);
            Qrcode = RemplissageVertical(Qrcode, 7, 0, 8, 255, 255, 255);

            // Haut droite
            Qrcode = RemplissageHorizontal(Qrcode, largeur - 8, 7, 8, 255, 255, 255);
            Qrcode = RemplissageVertical(Qrcode, largeur - 8, 0, 8, 255, 255, 255);

            // Bas gauche
            Qrcode = RemplissageHorizontal(Qrcode, 0, largeur - 8, 8, 255, 255, 255);
            Qrcode = RemplissageVertical(Qrcode, 7, largeur - 8, 8, 255, 255, 255);

            return Qrcode;
        }

        private byte[,,] BitsColoration(byte[,,] Qrcode, char bit, int x, int y)
        {
            if (bit == '0')
            {
                Qrcode = ColorationNouB(Qrcode, x, y, 255, 255, 255);
            }
            else
            {
                Qrcode = ColorationNouB(Qrcode, x, y, 0, 0, 0);
            }

            return Qrcode;
        }
        private byte[,,] ColorationNouB(byte[,,] Qrcode, int x, int y, byte r, byte v, byte b)
        {
            Qrcode[y, x, 0] = r;
            Qrcode[y, x, 1] = v;
            Qrcode[y, x, 2] = b;
            return Qrcode;
        }


        private byte[,,] RemplissageMontee(byte[,,] Qrcode, string chainebit, int h, int v)
        {
            int index = 0;

            while (index < chainebit.Length)
            {
                char char1 = chainebit[index];
                char char2 = chainebit[index + 1];

                char1 = AppliquerMasque(char1, h, v);
                char2 = AppliquerMasque(char2, h - 1, v);
                Qrcode = BitsColoration(Qrcode, char1, h, v);
                Qrcode = BitsColoration(Qrcode, char2, h - 1, v);

                v -= 1;
                index += 2;
            }

            return Qrcode;
        }
        private byte[,,] RemplissageDescente(byte[,,] Qrcode, string chainebit, int h, int v)
        {
            int index = 0;
            while (index < chainebit.Length)
            {
                char char1 = chainebit[index];
                char char2 = chainebit[index + 1];

                char1 = AppliquerMasque(char1, h, v);
                char2 = AppliquerMasque(char2, h - 1, v);

                Qrcode = BitsColoration(Qrcode, char1, h, v);
                Qrcode = BitsColoration(Qrcode, char2, h - 1, v);
                v += 1;
                index += 2;
            }
            return Qrcode;
        }

        private byte[,,] RemplissagesOctetcsV1(byte[,,] Qrcode, string chainebit)
        {
            string octets = chainebit.Substring(0, 24); // 24 = 12x2
            Qrcode = RemplissageMontee(Qrcode, octets, 20, 20);

            octets = chainebit.Substring(24, 24);
            Qrcode = RemplissageDescente(Qrcode, octets, 18, 9);

            octets = chainebit.Substring(48, 24);
            Qrcode = RemplissageMontee(Qrcode, octets, 16, 20);

            octets = chainebit.Substring(72, 24);
            Qrcode = RemplissageDescente(Qrcode, octets, 14, 9);

            octets = chainebit.Substring(96, 28);
            Qrcode = RemplissageMontee(Qrcode, octets, 12, 20);

            octets = chainebit.Substring(124, 12);
            Qrcode = RemplissageMontee(Qrcode, octets, 12, 5);

            octets = chainebit.Substring(136, 12);
            Qrcode = RemplissageDescente(Qrcode, octets, 10, 0);

            octets = chainebit.Substring(148, 28);
            Qrcode = RemplissageDescente(Qrcode, octets, 10, 7);

            octets = chainebit.Substring(176, 8);
            Qrcode = RemplissageMontee(Qrcode, octets, 8, 12);

            octets = chainebit.Substring(184, 8);
            Qrcode = RemplissageDescente(Qrcode, octets, 5, 9);

            octets = chainebit.Substring(192, 8);
            Qrcode = RemplissageMontee(Qrcode, octets, 3, 12);

            octets = chainebit.Substring(200, 8);
            Qrcode = RemplissageDescente(Qrcode, octets, 1, 9);

            return Qrcode;
        }
        private byte[,,] RemplissagesOctetcsV2(byte[,,] Qrcode, string chainebit)
        {
            string octets = chainebit.Substring(0, 32); // 32 = 16x2
            Qrcode = RemplissageMontee(Qrcode, octets, 24, 24);

            octets = chainebit.Substring(32, 32);
            Qrcode = RemplissageDescente(Qrcode, octets, 22, 9);

            octets = chainebit.Substring(64, 8);
            Qrcode = RemplissageMontee(Qrcode, octets, 20, 24);

            octets = chainebit.Substring(72, 14);
            Qrcode = RemplissageMontee(Qrcode, octets, 20, 15);

            octets = chainebit.Substring(86, 14);
            Qrcode = RemplissageDescente(Qrcode, octets, 18, 9);

            octets = chainebit.Substring(100, 8);
            Qrcode = RemplissageDescente(Qrcode, octets, 18, 21);

            octets = chainebit.Substring(108, 8);
            Qrcode = RemplissageMontee(Qrcode, octets, 16, 24);

            /// On remplit manuellement les pixels impactés par le motif d'alignement,
            /// car RemplissageDescente remplit 2 colonnes à la fois
            char char1 = AppliquerMasque(chainebit[116], 15, 20);
            BitsColoration(Qrcode, char1, 15, 20);
            char1 = AppliquerMasque(chainebit[117], 15, 19);
            BitsColoration(Qrcode, char1, 15, 19);
            char1 = AppliquerMasque(chainebit[118], 15, 18);
            BitsColoration(Qrcode, char1, 15, 18);
            char1 = AppliquerMasque(chainebit[119], 15, 17);
            BitsColoration(Qrcode, char1, 15, 17);
            char1 = AppliquerMasque(chainebit[120], 15, 16);
            BitsColoration(Qrcode, char1, 15, 16);

            octets = chainebit.Substring(121, 18);
            Qrcode = RemplissageMontee(Qrcode, octets, 16, 15);

            // Esquive des motifs de sychronisation
            octets = chainebit.Substring(139, 12);
            Qrcode = RemplissageMontee(Qrcode, octets, 16, 5);

            octets = chainebit.Substring(151, 12);
            Qrcode = RemplissageDescente(Qrcode, octets, 14, 0);

            // Esquive des motifs de sychronisation
            octets = chainebit.Substring(163, 36);
            Qrcode = RemplissageDescente(Qrcode, octets, 14, 7);

            octets = chainebit.Substring(199, 36);
            Qrcode = RemplissageMontee(Qrcode, octets, 12, 24);

            // Esquive des motifs de sychronisation
            octets = chainebit.Substring(235, 12);
            Qrcode = RemplissageMontee(Qrcode, octets, 12, 5);

            octets = chainebit.Substring(247, 12);
            Qrcode = RemplissageDescente(Qrcode, octets, 10, 0);

            // Esquive des motifs de sychronisation
            octets = chainebit.Substring(259, 36);
            Qrcode = RemplissageDescente(Qrcode, octets, 10, 7);

            octets = chainebit.Substring(295, 16);
            Qrcode = RemplissageMontee(Qrcode, octets, 8, 16);

            octets = chainebit.Substring(311, 16);
            Qrcode = RemplissageDescente(Qrcode, octets, 5, 9);

            octets = chainebit.Substring(327, 16);
            Qrcode = RemplissageMontee(Qrcode, octets, 3, 16);

            octets = chainebit.Substring(343, 8);
            Qrcode = RemplissageDescente(Qrcode, octets, 1, 9);

            //bits résiduels
            char1 = AppliquerMasque(chainebit[151], 1, 13);
            BitsColoration(Qrcode, char1, 1, 13);
            char1 = AppliquerMasque('0', 0, 13);
            BitsColoration(Qrcode, char1, 0, 13);

            //bits résiduels
            octets = "000000";
            Qrcode = RemplissageDescente(Qrcode, octets, 1, 14);

            return Qrcode;
        }


        /// <summary>
        /// Place le motif d'alignement de la version 2
        /// </summary>
        /// <param name="Qrcode"></param>
        /// <param name="largeur"></param>
        /// <returns></returns>
        private byte[,,] PlaceMotAlign(byte[,,] Qrcode, int largeur)
        {
            //Carré noir : 5x5
            Qrcode = RemplissageCarre(Qrcode, largeur - 9, largeur - 9, 5, 0, 0, 0);

            //Carré blanc : 3x3
            Qrcode = RemplissageCarre(Qrcode, largeur - 8, largeur - 8, 3, 255, 255, 255);

            //Carré central noir
            Qrcode = ColorationNouB(Qrcode, largeur - 7, largeur - 7, 0, 0, 0);
            return Qrcode;
        }


        public byte[,,] ImageQrCode(string chaineBit)
        {
            byte[,,] Qrcode = null;

            if (version == "1")
            {
                Qrcode = new byte[21, 21, 3];
                Qrcode = PlacerMotifsRe(Qrcode, 21);
                Qrcode = PlacerSeparateur(Qrcode, 21);
                Qrcode = PlacerMotifsSynchro(Qrcode);
                Qrcode = PlacerModSomb(Qrcode, "1");
                Qrcode = PlacerMasqV1(Qrcode);
                Qrcode = RemplissagesOctetcsV1(Qrcode, chaineBit);
            }
            if (version == "2")
            {
                Qrcode = new byte[25, 25, 3];
                Qrcode = PlacerMotifsRe(Qrcode, 25);
                Qrcode = PlacerSeparateur(Qrcode, 25);
                Qrcode = PlaceMotAlign(Qrcode, 25);
                Qrcode = PlacerMotifsSynchro(Qrcode);
                Qrcode = PlacerModSomb(Qrcode, "2");
                Qrcode = PlacerMasqV2(Qrcode);
                Qrcode = RemplissagesOctetcsV2(Qrcode, chaineBit);
            }

            return Qrcode;
        }

        private char AppliquerMasque(char bit, int h, int v)
        {
            if ((h + v) % 2 == 0)
            {
                if (bit != '0') 
                { 
                    bit = '0'; 
                }
                else 
                { 
                    bit = '1'; 
                }
            }
            return bit;
        }

        private byte[,,] PlacerMasqV1(byte[,,] Qrcode)
        {
            //Masque: 1110111 11000100

            Qrcode = ColorationNouB(Qrcode, 0, 8, 0, 0, 0); //1
            Qrcode = ColorationNouB(Qrcode, 1, 8, 0, 0, 0); //1
            Qrcode = ColorationNouB(Qrcode, 2, 8, 0, 0, 0); //1
            Qrcode = ColorationNouB(Qrcode, 3, 8, 255, 255, 255); //0
            Qrcode = ColorationNouB(Qrcode, 4, 8, 0, 0, 0); //1
            Qrcode = ColorationNouB(Qrcode, 5, 8, 0, 0, 0); //1
            Qrcode = ColorationNouB(Qrcode, 7, 8, 0, 0, 0); //1

            Qrcode = ColorationNouB(Qrcode, 8, 8, 0, 0, 0); //1
            Qrcode = ColorationNouB(Qrcode, 8, 7, 0, 0, 0); //1
            Qrcode = ColorationNouB(Qrcode, 8, 5, 255, 255, 255); //0
            Qrcode = ColorationNouB(Qrcode, 8, 4, 255, 255, 255); //0
            Qrcode = ColorationNouB(Qrcode, 8, 3, 255, 255, 255); //0
            Qrcode = ColorationNouB(Qrcode, 8, 2, 0, 0, 0); //1
            Qrcode = ColorationNouB(Qrcode, 8, 1, 255, 255, 255); //0
            Qrcode = ColorationNouB(Qrcode, 8, 0, 255, 255, 255); //0

            Qrcode = ColorationNouB(Qrcode, 8, 20, 0, 0, 0); //1
            Qrcode = ColorationNouB(Qrcode, 8, 19, 0, 0, 0); //1
            Qrcode = ColorationNouB(Qrcode, 8, 18, 0, 0, 0); //1
            Qrcode = ColorationNouB(Qrcode, 8, 17, 255, 255, 255); //0
            Qrcode = ColorationNouB(Qrcode, 8, 16, 0, 0, 0); //1
            Qrcode = ColorationNouB(Qrcode, 8, 15, 0, 0, 0); //1
            Qrcode = ColorationNouB(Qrcode, 8, 14, 0, 0, 0); //1


            Qrcode = ColorationNouB(Qrcode, 13, 8, 0, 0, 0); //1
            Qrcode = ColorationNouB(Qrcode, 14, 8, 0, 0, 0); //1
            Qrcode = ColorationNouB(Qrcode, 15, 8, 255, 255, 255); //0
            Qrcode = ColorationNouB(Qrcode, 16, 8, 255, 255, 255); //0
            Qrcode = ColorationNouB(Qrcode, 17, 8, 255, 255, 255); //0
            Qrcode = ColorationNouB(Qrcode, 18, 8, 0, 0, 0); //1
            Qrcode = ColorationNouB(Qrcode, 19, 8, 255, 255, 255); //0
            Qrcode = ColorationNouB(Qrcode, 20, 8, 255, 255, 255); //0

            return Qrcode;
        }

        private byte[,,] PlacerMasqV2(byte[,,] Qrcode)
        {
            //Masque : 1110111 11000100 
            Qrcode = ColorationNouB(Qrcode, 0, 8, 0, 0, 0); //1
            Qrcode = ColorationNouB(Qrcode, 1, 8, 0, 0, 0); //1
            Qrcode = ColorationNouB(Qrcode, 2, 8, 0, 0, 0); //1
            Qrcode = ColorationNouB(Qrcode, 3, 8, 255, 255, 255); //0
            Qrcode = ColorationNouB(Qrcode, 4, 8, 0, 0, 0); //1
            Qrcode = ColorationNouB(Qrcode, 5, 8, 0, 0, 0); //1
            Qrcode = ColorationNouB(Qrcode, 7, 8, 0, 0, 0); //1

            Qrcode = ColorationNouB(Qrcode, 8, 8, 0, 0, 0); //1
            Qrcode = ColorationNouB(Qrcode, 8, 7, 0, 0, 0); //1
            Qrcode = ColorationNouB(Qrcode, 8, 5, 255, 255, 255); //0
            Qrcode = ColorationNouB(Qrcode, 8, 4, 255, 255, 255); //0
            Qrcode = ColorationNouB(Qrcode, 8, 3, 255, 255, 255); //0
            Qrcode = ColorationNouB(Qrcode, 8, 2, 0, 0, 0); //1
            Qrcode = ColorationNouB(Qrcode, 8, 1, 255, 255, 255); //0
            Qrcode = ColorationNouB(Qrcode, 8, 0, 255, 255, 255); //0

            Qrcode = ColorationNouB(Qrcode, 8, 24, 0, 0, 0); //1
            Qrcode = ColorationNouB(Qrcode, 8, 23, 0, 0, 0); //1
            Qrcode = ColorationNouB(Qrcode, 8, 22, 0, 0, 0); //1
            Qrcode = ColorationNouB(Qrcode, 8, 21, 255, 255, 255); //0
            Qrcode = ColorationNouB(Qrcode, 8, 20, 0, 0, 0); //1
            Qrcode = ColorationNouB(Qrcode, 8, 19, 0, 0, 0); //1
            Qrcode = ColorationNouB(Qrcode, 8, 18, 0, 0, 0); //1


            Qrcode = ColorationNouB(Qrcode, 17, 8, 0, 0, 0); //1
            Qrcode = ColorationNouB(Qrcode, 18, 8, 0, 0, 0); //1
            Qrcode = ColorationNouB(Qrcode, 19, 8, 255, 255, 255); //0
            Qrcode = ColorationNouB(Qrcode, 20, 8, 255, 255, 255); //0
            Qrcode = ColorationNouB(Qrcode, 21, 8, 255, 255, 255); //0
            Qrcode = ColorationNouB(Qrcode, 22, 8, 0, 0, 0); //1
            Qrcode = ColorationNouB(Qrcode, 23, 8, 255, 255, 255); //0
            Qrcode = ColorationNouB(Qrcode, 24, 8, 255, 255, 255); //0

            return Qrcode;
        }

        #endregion

    }
}
