using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;


namespace WindowsFormsApplication3
{
   

    public partial class Form1 : Form
    {
       private string[] recipients;
        private string[] liquides;
        private string[] feux;
        int recipnumb;
        int liqpnumb;
        int feunumb;
        private Metier experience;

        static class Constants
        {
            public const Char ExpActif = '0';
            public const Char Comparaison = '1';
            public const Char feuAllumer = '2';
            public const Char chaufRecip = '3';
            public const Char ebulliton = '4';
            public const Char evapliq = '5';
            public const Char ExpTerm = '6';
        }

        class DAO
        {
            private string[] tabLiquide = new string[80]; // Création d'un tableau de string qui receptionnera les liquides
            private string[] tabRecipient = new string[80]; // Création d'un tableau de string qui receptionnera les recipients
            private string[] tabFeu = new string[80]; // Création d'un tableau de string qui receptionnera les feux
            private string temp; // Création d'un string temporaire pour effectuer le transfert entre la DB et les tableaux 
            private int index = 0; // Création d'un index permettant de naviguer dans les tableaux afin de les remplir 
            private string connexion_string = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\SampledB.mdf;Integrated Security=True"; // Création d'un string permettant d'ouvrir la dB avec des parametres prédéfinis 
            private string tempName; // Création d'un string permettant de trim le nom avant de le stocker dans le tableau 
            private string tempMatiere; // Création d'un string permettant de trim la matiere avant de la stocker dans le tableaus  
            private string tempCoefEbu; // Création d'un string permettant de trim le coefficient de chauffe d'un liquide avant de le stocker dans le tableau 
            public DAO()
        {
            try
            {
                SqlConnection MyConnection = new SqlConnection(connexion_string); // Ouverture d'une connexion à la dB avec la connexion_string en parametres
                MyConnection.Open(); // Activation de la connexion

                SqlCommand cmdLiquide = new SqlCommand("Select * from Liquide"); // Création de la requete SQL permettant de récuperer tous les liquides stockés dans la dB
                cmdLiquide.Connection = MyConnection; // Metier de la requete
                SqlDataReader readerL = cmdLiquide.ExecuteReader();  // Récupération des résultats de la requete dans un Reader 

                while (readerL.Read()) // Tant que le reader voit quelque chose 
                {
                    tempName = String.Format("{0}", readerL[1]); // tempName recupere le nom dans le resultat de la requete
                    tempName = tempName.Trim(); // trim de tempName permettant de supprimer les espaces inutiles
                    tempCoefEbu = String.Format("{0}", readerL[4]);
                    tempCoefEbu = tempCoefEbu.Trim();

                    temp = String.Format("{1} {2} {3} {4}", readerL[0], tempName, readerL[2], readerL[3], tempCoefEbu); // temp recupere le resultat de la requete
                    tabLiquide[index] = temp; // remplissage du tableau final avec temp 
                    index++; // Incrémentation de l'index permettant de naviguer dans le tableau
                }
                index = 0; // Remise a 0 de l'index afin de pouvoir le réutiliser 
                readerL.Close(); // Fermeture du Reader et donc de la requete 


                SqlCommand cmdRecipient = new SqlCommand("Select * from Recipient"); // Création de la requete SQL permettant de récuperer tous les recipients stockés dans la dB
                cmdRecipient.Connection = MyConnection; // Metier de la requete
                SqlDataReader readerR = cmdRecipient.ExecuteReader();  // Récupération des résultats de la requete dans un Reader 

                while (readerR.Read()) // Tant que le reader voit quelque chose 
                {
                    tempName = String.Format("{0}", readerR[1]); // tempName recupere le nom dans le resultat de la requete
                    tempName = tempName.Trim(); // trim de tempName permettant de supprimer les espaces inutiles

                    temp = String.Format("{1} {2} {3}", readerR[0], tempName, readerR[2], readerR[3]); // temp recupere le resultat de la requete
                    tabRecipient[index] = temp; // remplissage du tableau final avec temp 
                    index++; // Incrémentation de l'index permettant de naviguer dans le tableau
                }
                index = 0; // Remise a 0 de l'index afin de pouvoir le réutiliser 
                readerR.Close(); // Fermeture du Reader et donc de la requete 


                SqlCommand cmdFeu = new SqlCommand("Select * from Feu"); // Création de la requete SQL permettant de récuperer tous les feux stockés dans la dB
                cmdFeu.Connection = MyConnection; // Metier de la requete
                SqlDataReader readerF = cmdFeu.ExecuteReader();  // Récupération des résultats de la requete dans un Reader 

                while (readerF.Read()) // Tant que le reader voit quelque chose 
                {
                    tempName = String.Format("{0}", readerF[1]); // tempName recupere le nom dans le resultat de la requete
                    tempName = tempName.Trim(); // trim de tempName permettant de supprimer les espaces inutiles
                    tempMatiere = String.Format("{0}", readerF[2]); // tempMatiere recupere le nom dans le resultat de la requete
                    tempMatiere = tempMatiere.Trim(); // trim de tempMatiere permettant de supprimer les espaces inutiles

                    temp = String.Format("{1} {2} {3} {4}", readerF[0], tempName, tempMatiere, readerF[3], readerF[4]); // temp recupere le resultat de la requete
                    tabFeu[index] = temp; // remplissage du tableau final avec temp 
                    index++; // Incrémentation de l'index permettant de naviguer dans le tableau
                }
                index = 0; // Remise a 0 de l'index afin de pouvoir le réutiliser 
                readerF.Close(); // Fermeture du Reader et donc de la requete 
            }
            catch (Exception e) // Si l'ouverture de la dB est impossible 
            {
                Console.WriteLine(e.Message); // Afficher le message d'erreur renvoyé 
                RecupViaTxt(); // Lancer la récupération via les fichiers locaux 
            }
        }
            public void RecupViaTxt()
            {
                string[] p_path = new string[3]; // Création d'un tableau qui contiendra les chemins d'accès aux fichiers
                p_path[0] = "Liquides.txt"; // Configuration du tableau 
                p_path[1] = "Recipients.txt"; // Configuration du tableau 
                p_path[2] = "Feux.txt"; // Configuration du tableau 

                Console.WriteLine("\n TENTATIVE DE RECUPERATION VIA LES FICHIERS LOCAUX\n");

                try
                {
                    StreamReader dao = new StreamReader(p_path[0]); // Ouverture du fichier via un StreamReader que l'on nomme dao

                    if (dao != null) // Si le fichier contient quelque chose
                    {
                        temp = dao.ReadLine(); // temp prend la valeur de la première ligne

                        while (temp != null) // Tant que temp contient quelque chose
                        {
                            tabLiquide[index] = temp; // Remplissage du tableau de liquide avec le contenu de Temp 
                            temp = dao.ReadLine(); // temp prend la valeur de la ligne suivante 
                            index++; // Incrémentation de l'index afin de pointer sur la case suivante du tableau 
                        }
                    }
                    dao.Close(); // Fermeture du fichier
                }
                catch (Exception e) // Si le fichier ne s'ouvre pas / mal 
                {
                    Console.WriteLine(e.Message); // Affichage du message d'erreur 
                }


                try
                {
                    StreamReader dao = new StreamReader(p_path[1]); // Ouverture du fichier via un StreamReader que l'on nomme dao

                    if (dao != null) // Si le fichier contient quelque chose
                    {
                        temp = dao.ReadLine(); // temp prend la valeur de la premiere ligne 

                        while (temp != null) // Tant que temp contient quelque chose 
                        {
                            tabRecipient[index] = temp; // Remplissage du contenu de Recipient avec le contenu de Temp 
                            temp = dao.ReadLine(); // temp prend la valeur de la ligne suivante 
                            index++; // Incrémentation de l'index afin de pointer sur la case suivante du tableau 
                        }
                    }
                    dao.Close(); // Fermeture du fichier 
                }
                catch (Exception e) // Si le fichier ne s'ouvre pas / mal 
                {
                    Console.WriteLine(e.Message); // Affichage du message d'erreur 
                }

                try
                {
                    StreamReader dao = new StreamReader(p_path[2]); // Ouverture du fichier via un StreamReader que l'on nomme dao

                    if (dao != null) // Si le fichier contient quelque chose
                    {
                        temp = dao.ReadLine(); // temp prend la valeur de la premiere ligne 

                        while (temp != null) // Tant que temp contient quelque chose 
                        {
                            tabFeu[index] = temp; // Remplissage du contenu de Recipient avec le contenu de Temp 
                            temp = dao.ReadLine(); // temp prend la valeur de la ligne suivante 
                            index++; // Incrémentation de l'index afin de pointer sur la case suivante du tableau 
                        }
                    }
                    dao.Close(); // Fermeture du fichier 
                }
                catch (Exception e) // Si le fichier ne s'ouvre pas / mal 
                {
                    Console.WriteLine(e.Message); // Affichage du message d'erreur 
                }
            }

            public string[] getTabLiquide() // Permet de récuperer le tableau de Liquides
            {
                return tabLiquide;
            }

            public string[] getTabRecipient() // Permet de récuperer le tableau de Recipients 
            {
                return tabRecipient;
            }

            public string[] getTabFeu() // Permet de récuperer le tableau de Feux 
            {
                return tabFeu;
            }
        }

        class Liquide

        {



            //*** attributs de liquide ***//

            private string nomLiquide;

            private int degreEbullition;

            private double coefficiantEbulition;

            private double temperatureLiquide;

            private double coefficientChauffe;





            //*** constructeur de liquide ***//

            public Liquide() { }  // Constructeur par défaut



            public Liquide(string l_nomLiquide, int l_degreEbullition, double l_coefficiantEbulition, double l_coefficientChauffe)

            {

                nomLiquide = l_nomLiquide;

                degreEbullition = l_degreEbullition;

                coefficiantEbulition = l_coefficiantEbulition;

                coefficientChauffe = l_coefficientChauffe;

                temperatureLiquide = 0;

            }



            //*** liste des get de liquide ***//



            public double get_temperatureLiquide()

            {

                return temperatureLiquide;

            }



            public string get_nomLiquide()

            {

                return nomLiquide;

            }



            public double get_coefficientEbulition()

            {

                return coefficiantEbulition;

            }



            public int get_degreEbullition()

            {

                return degreEbullition;

            }

            public double get_coefficientChauffe()
            {
                return coefficientChauffe;
            }


            //*** liste des set de liquide ***//



            public void set_temperatureDuLiquide(double l_temperature)

            {

                temperatureLiquide = l_temperature;

            }



            public void set_nom_liquide(string nom)

            {

                nomLiquide = nom;

            }



            //*** afficheur de liquide ***//



            public void afficheur_liquide()

            {

                Console.WriteLine(" Nom du liquide : " + nomLiquide + " qui bouille  a : " + degreEbullition.ToString() + " degré(s)\n");

            }



        };





        class Recipient

        {



            //*** attributs de Recipient ***//



            private string nomRecipient;

            private int capaciteMax;

            private string matiere;

            private double volumeActuel;

            private Liquide liquideContenu;



            //*** constructeurs de Recipient ***//



            public Recipient() { }  // Constructeur par défaut.



            public Recipient(string r_nomRecipient, int r_capaciteMax, string r_matiere)

            {

                matiere = r_matiere;

                nomRecipient = r_nomRecipient;

                capaciteMax = r_capaciteMax;

                volumeActuel = 0;

                liquideContenu = null;

            }



            //*** liste des get de recipient ***//



            public double get_volumeActuel()

            {

                return volumeActuel;

            }



            public int get_capaciteMax()

            {

                return capaciteMax;

            }



            public string get_matiereRecipient()

            {

                return matiere;

            }



            public string get_nomRecipient()

            {

                return nomRecipient;

            }



            //* liste des get du liquideContenu *//



            public double get_temperatureLiquideContenu()

            {

                return liquideContenu.get_temperatureLiquide();

            }



            public string get_nomLiquideContenu()

            {

                return liquideContenu.get_nomLiquide();

            }



            public double get_coefficientLiquideContenut()

            {

                return liquideContenu.get_coefficientEbulition();

            }



            public int get_temperaturEbulitionLiquide()

            {

                return liquideContenu.get_degreEbullition();

            }



            //*** liste des set de recipient ***//



            public void set_volumeActuel(double r_volumeActuel)

            {

                volumeActuel = r_volumeActuel;

            }



            public void set_temperatureLiquideContenu(double temperature)

            {

                liquideContenu.set_temperatureDuLiquide(temperature);

            }



            //* set remplir un recipient d'un liquide *//



            public void set_remplir(Liquide liquide, double r_quantite)

            {

                liquideContenu = liquide;

                volumeActuel = r_quantite;

            }



            //* set en cas de debordement *//



            public void set_volumeActuelDeborder()

            {

                volumeActuel = capaciteMax;

            }



            //*** afficheur de recipient ***//



            public void afficheur_recipient()

            {

                Console.WriteLine("C'est un recipient " + nomRecipient + " d\' une capacité de " + capaciteMax.ToString() + " ,il est fais en " + matiere + "\n");



                if (volumeActuel == 0)

                {

                    Console.WriteLine("Le recipient est vide");

                }

                else

                {

                    Console.WriteLine("Le recipient contient: " + volumeActuel.ToString() + "cl\n");

                    liquideContenu.afficheur_liquide();



                }

            }



        };





        class Feu

        {



            //*** attributs de Feu ***//

            private string marque;
            private string matiere;
            private string modele;
            private int vitesseChauffe;
            private int degreCourant;
            private bool etat;
            private int chrono_debut;
            private int chrono_actuelle;
            private int compteur;
            private Recipient recipient;

            //*** constructeur de Feu ***//

            public Feu() { }


            public Feu(string f_marque, string f_matiere, string f_modele, int r_vitesseChauffe)
            {
                marque = f_marque;
                matiere = f_matiere;
                modele = f_modele;
                vitesseChauffe = r_vitesseChauffe;
                degreCourant = 0;
                etat = false;
                chrono_debut = Environment.TickCount;
                chrono_actuelle = 0;
                recipient = null;
            }


            //*** liste des get de Feu ***//

            public int get_chrono_debut()
            {
                return 0; //  chrono_debut;

            }

            public int get_chrono_actuelle()
            {
                return 0; // chrono_actuelle;
            }



            public int get_degreCourant()
            {
                return degreCourant;
            }



            public string get_matiereFeu()
            {
                return matiere;
            }

            //*** liste des set de Feu ***//

            public void set_degreCourant(int tempFeu)
            {
                degreCourant = degreCourant - tempFeu;
            }

            public void set_volumeAcutuelFeu(double f_volume)
            {
                recipient.set_volumeActuel(f_volume);
            }





            public void set_chronoActuelle()

            {
                compteur = Environment.TickCount;
                chrono_actuelle = compteur - chrono_debut;
            }


            //* set mettre un recipient sur le feu *//
            
            public void set_PutOnFire(Recipient r_recipient)

            {
                recipient = r_recipient;
                alumage_feu();
            }

            //* allumage du feu *//

            public void alumage_feu()

            {

                compteur = Environment.TickCount;
                chrono_debut = compteur;
                etat = true;

            }



            //* augmentation de la temperature de la plaque *//



            public void chauffe_feu(int temperature_souhaiter)

            {

                compteur = Environment.TickCount;

                chrono_actuelle = compteur;

                int calcul = chrono_actuelle - chrono_debut;
                degreCourant = vitesseChauffe * calcul;
                chrono_actuelle = compteur - chrono_debut;

            }

            //*** maintien de la temperature jusqua ebulition ***//

            public void maintenirFeu(double temperature)

            {
                //mise en ebulition du liquide

                temperature += recipient.get_coefficientLiquideContenut() / recipient.get_volumeActuel();
                recipient.set_temperatureLiquideContenu(temperature);
                //pour eviter que la temperature depace celle dï¿½bulition
                if (recipient.get_temperatureLiquideContenu() > recipient.get_temperaturEbulitionLiquide())
                {
                    temperature = recipient.get_temperaturEbulitionLiquide();
                    recipient.set_temperatureLiquideContenu(temperature);
                }

                // compteur = time(&compteur);

                System.Threading.Thread.Sleep(1000);

            }



            //*** afficheur de feu ***/



            public void affiche_feu()
                {

                Console.WriteLine("Le feu {0}-{0}", marque, modele);
                Console.WriteLine("\nCe feu est fait en {0} ", matiere);
                Console.WriteLine("\nCe feu a une vitesse de chauffe de {0} \n", vitesseChauffe);
                //Console.WriteLine("\nLe chrono est lance depuis %g secondes\n", chrono_actuelle - chrono_debut);
                Console.WriteLine("\nLe feu a une temperature actuelle de :{0} \n", degreCourant);

                if (etat)

                {
                    Console.WriteLine("Le feu est allume");
                }
                else
                {
                    Console.WriteLine("Le feu est eteint \n");
                }
                if (recipient != null)
                {
                    Console.WriteLine("Un recipient est sur le feu \n");
                    recipient.afficheur_recipient();
                }
                else
                {
                    Console.WriteLine("Le feu est vide \n");
                }
            }
        };


        class Metier
        {
            private Feu a_feu;
            private Liquide a_liquide;
            private Recipient a_recipient;
            private string[] tabLiquide;
            private string[] tabRecipient;
            private string[] tabFeu;
            private string message;
            private int statut;

            //*** Pas dattributs ***//
            //*** Constructeur vide ***//
            //public Metier() { }
            //*** Constructeur surchargé ***//

            public Metier()
            {
                DAO dao = new DAO();

                tabRecipient = dao.getTabRecipient();
              
                tabLiquide = dao.getTabLiquide();
               
                tabFeu = dao.getTabFeu();
             
            }

       
            public void Activation(int p_indiceRecipient, int p_indiceLiquide, int p_indiceFeu)  // Ajout du JL du 28/05/2018
            {
                recupereLiquide(p_indiceLiquide);
                recupereFeu(p_indiceFeu);
                recupereRecipient(p_indiceRecipient);
                comparaison();
            }
            //*** comparaison de la matiere du recipient et de celui du feu ***//



            private void comparaison()

            {
                string matiereFeu = a_feu.get_matiereFeu();
                string matiereRecipient = a_recipient.get_matiereRecipient().Trim();

                if (matiereFeu == matiereRecipient)
                {
                    lancementChauffe();
                }

                else
                {
                    message = "Incompatibilité détectée.";
                    statut = Constants.Comparaison;
                }
            }

            //*** tout le processus de chauffe apres la comparaison des materiaux ***//
            private void recupereLiquide(int choixLiquide)
            {
                int i = 0;
                string nom;
                char delimiteur = ';';

                string[] tabSLiquide = tabLiquide[choixLiquide].Split(delimiteur);
                int degre;
                double coefficientEbulition;
                double coefficientChauffe;

                nom = tabSLiquide[0];
                degre = int.Parse(tabSLiquide[1]);
                coefficientEbulition = double.Parse(tabSLiquide[2]);
                coefficientChauffe = int.Parse(tabSLiquide[3]);

                Liquide liquide = new Liquide(nom, degre, coefficientEbulition,coefficientChauffe);
                i++;
                a_liquide = liquide;
            }

            public string[] get_tabLiquide()
            {
                return tabLiquide;
            }

            private void recupereFeu(int choixFeu)
            {
                int i = 0;
                int vitesse_chauffe;
                string marque;
                string matiereFeu;
                string modele;
                char delimiteur = ';';

                string[] tabSFeu = tabFeu[choixFeu].Split(delimiteur);

                marque = tabSFeu[0];
                matiereFeu = tabSFeu[1];
                modele = tabSFeu[2];
                vitesse_chauffe = int.Parse(tabSFeu[3]);

                Feu feu = new Feu(marque, matiereFeu, modele, vitesse_chauffe);
                i++;

                a_feu = feu;
            }
            public string[] get_tabFeu()
            {
                return tabFeu;
            }

            private void recupereRecipient(int choixRecipient)
            {
                int i = 0;
                string nom;
                int capaciteMax;
                string matiereRecipient;
                char delimiteur = ';';

                string[] tabSRecipient = tabRecipient[choixRecipient].Split(delimiteur);

                nom = tabSRecipient[0];
                capaciteMax = int.Parse(tabSRecipient[1]);
                matiereRecipient = tabSRecipient[2];

                Recipient recipient = new Recipient(nom, capaciteMax, matiereRecipient);
                i++;

                a_recipient = recipient;
            }

            public string[] get_tabRecipient()
            {
                return tabRecipient;
            }

            public int affectationChoix(int p_choixRecipient, int p_choixLiquide, int p_choixFeu, int p_quantite)
            {
                recupereRecipient(p_choixRecipient);
                recupereLiquide(p_choixLiquide);
                recupereFeu(p_choixFeu);
                versement(p_quantite);
                return 60;  // Pour essai
            }
            private void versement(int quantite)
            {
                a_recipient.set_remplir(a_liquide, quantite);
                comparaison();
            }

            public void lancementChauffe()

            {
                int difference;
                int condition = 0;
                
                a_feu.set_PutOnFire(a_recipient);
                a_feu.affiche_feu();
                //augmentation de la temperature de la plaque jusqua temperature d'ï¿½bulition du liquideContenu
                while (a_feu.get_degreCourant() < a_recipient.get_temperaturEbulitionLiquide())
                {
                    a_feu.chauffe_feu(a_liquide.get_degreEbullition());
                    Console.WriteLine("La temperature actuelle de la plaque est de: {0}\n", condition);
                    System.Threading.Thread.Sleep(1000);
                    if (condition < a_feu.get_degreCourant())
                    {
                        condition = a_feu.get_degreCourant();
                        Console.WriteLine("La temperature actuelle de la plaque est de: {0}\n", condition);
                    }
                }

                //diminution de la temperature de la plaque si elle est superieur a celle demander
                if (a_feu.get_degreCourant() > a_recipient.get_temperaturEbulitionLiquide())
                {
                    difference = a_feu.get_degreCourant() - a_recipient.get_temperaturEbulitionLiquide();
                    Console.WriteLine("La temperature va baisser de: {0} \n", difference);
                    a_feu.set_degreCourant(difference);

                }

                a_feu.affiche_feu();
                //mise en ebulition du liquide mais  non fonctionnel
                // time_t compteur;
                // srand(time(null));
                // compteur = time(&compteur);

                double temperature = a_recipient.get_temperatureLiquideContenu();
                //augmente la temperature du liquide jusqua ebulition
                while (a_recipient.get_temperatureLiquideContenu() < a_recipient.get_temperaturEbulitionLiquide())
                {
                    a_feu.maintenirFeu(temperature);
                    temperature = a_recipient.get_temperatureLiquideContenu();
                    Console.WriteLine("La temperature du liquide est de {0} degrés \n", a_recipient.get_temperatureLiquideContenu());
                }


                //evaporation du liquide
                double diminution = a_recipient.get_volumeActuel();
                while (a_recipient.get_volumeActuel() > 0)
                {
                    //le liquide perdra (coefficient / (coefficient -1) cl par seconde
                    diminution = a_recipient.get_volumeActuel() - (a_recipient.get_coefficientLiquideContenut() / (a_recipient.get_coefficientLiquideContenut() - 1));
                    a_recipient.set_volumeActuel(diminution);
                    //pour eviter que la temperature depace celle dï¿½bulition
                    if (a_recipient.get_volumeActuel() < 0)
                    {
                        a_recipient.set_volumeActuel(0);
                    }

                    //compteur = time(&compteur);
                    System.Threading.Thread.Sleep(1000);
                    message = "Le recipient contient actuellement" + a_recipient.get_volumeActuel() + "\n";
                    
                }
                Console.ReadLine();

            }
            public string get_message()
            {
                return message;
            }

            public int get_statut()
            {
                return statut;
            }
        };
       
        public Form1()
        {
           InitializeComponent();
           experience = new Metier();
           liquides = experience.get_tabLiquide();
           recipients = experience.get_tabRecipient();
           feux = experience.get_tabFeu();

                for (int i = 0; i < 4; i++)
                {
                    listBox1.Items.Add(liquides[i]);
                    listBox2.Items.Add(recipients[i]);
                    listBox3.Items.Add(feux[i]);             
                }

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            liqpnumb = listBox1.SelectedIndex;
            // MessageBox.Show(liqpnumb.ToString());


        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            recipnumb = listBox2.SelectedIndex;
             // MessageBox.Show(recipnumb.ToString());
        }

        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            feunumb = listBox3.SelectedIndex;
           // MessageBox.Show(feunumb.ToString());
        }

        private void label1_Click(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int tmpsCourant,tmpsMax;
            tmpsMax= experience.affectationChoix(recipnumb, liqpnumb, feunumb, int.Parse(textBox1.Text));
            tmpsCourant = 0;
            while(tmpsCourant < tmpsMax)
                {
                switch (experience.get_statut())
                    {
                    case Constants.ExpActif :
                        textBox1.Text = experience.get_message();
                        break;
                    case Constants.feuAllumer :
                        textBox1.Text = experience.get_message();
                        break;
                    case Constants.chaufRecip:
                        textBox1.Text = experience.get_message();
                        break;
                    case Constants.ebulliton:
                        textBox1.Text = experience.get_message();
                        break;
                    case Constants.evapliq:
                        textBox1.Text = experience.get_message();
                        break;
                    case Constants.Comparaison:
                        textBox1.Text = experience.get_message();
                        break;
                    default:
                    break;
                   }
                experience.get_message();
                System.Threading.Thread.Sleep(5000);
                tmpsCourant = tmpsCourant + 50;
            }
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)  // ajout Loan qui remplassera les Listbox
        {

            liqpnumb = listBox1.SelectedIndex; //Je n'arrive pas encore à ajouter les liquides et les mettres dans le menus déroulant (ce que j'aimerai voir avec vous)
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) //remplasserment des ListBox à voir ensemble j'arrive pas à mettre les "nom" dans la liste déroulante
        {
            //comboBox1.Items.Add();
            recipnumb = listBox2.SelectedIndex;
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
