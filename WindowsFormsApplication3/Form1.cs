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
        private Lancement experience;

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
                   

                    temp = String.Format("{1} {2} {3}", readerL[0], tempName, readerL[2], readerL[3]); // temp recupere le resultat de la requete
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






            //*** constructeur de liquide ***//

            public Liquide() { }  // Constructeur par défaut



            public Liquide(string l_nomLiquide, int l_degreEbullition, double l_coefficiantEbulition)

            {

                nomLiquide = l_nomLiquide;

                degreEbullition = l_degreEbullition;

                coefficiantEbulition = l_coefficiantEbulition;
            

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

            public int get_vitesseChauffe()
            {
                return vitesseChauffe;
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


        class Lancement
        {
            private Feu a_feu;
            private Liquide a_liquide;
            private Recipient a_recipient;
            private string[] tabLiquide;
            private string[] tabRecipient;
            private string[] tabFeu;
            private string message;
            private char statut;
            private int chrono_debut;
            private int chrono_actuelle;

            //*** Pas dattributs ***//
            //*** Constructeur vide ***//
            //public Lancement() { }
            //*** Constructeur surchargé ***//

            public Lancement()
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

                nom = tabSLiquide[0];
                degre = int.Parse(tabSLiquide[1]);
                coefficientEbulition = double.Parse(tabSLiquide[2]);

                Liquide liquide = new Liquide(nom, degre, coefficientEbulition);
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
            public string get_message()
            {
                return message;
            }
            public char get_statut()
            {
                return statut;
            }

            public Feu get_feu()
            {
                return a_feu;
            }
            public Liquide get_liquide()
            {
                return a_liquide;
            }
            public Recipient get_recipient()
            {
                return a_recipient;
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

            // getTabRecipient -> camelCase
            // get_tab_recipient

            public string[] get_tabRecipient()
            {
                return tabRecipient;
            }

            public void affectationChoix(int p_choixRecipient, int p_choixLiquide, int p_choixFeu, int p_quantite)
            {
                recupereRecipient(p_choixRecipient);
                recupereLiquide(p_choixLiquide);
                recupereFeu(p_choixFeu);
                versement(p_quantite);
            }
            private void versement(int quantite)
            {
                a_recipient.set_remplir(a_liquide, quantite);
                comparaison();
            }

            public void lancementChauffe()

            {
                message = "Incompatibilité détectée.";
                statut = Constants.Comparaison;

                int difference;

                int condition = 0;

                //recipient sur feu et allumage du feu
                a_feu.set_PutOnFire(a_recipient);
                chrono_debut = Environment.TickCount;
                a_feu.affiche_feu();
                //augmentation de la temperature de la plaque jusqua temperature d'ï¿½bulition du liquideContenu


            }
            public void statutActuelle()
            {
                int timeNow = Environment.TickCount;
                chrono_actuelle = timeNow - chrono_debut;
                int temp1 = 0;
                if (a_feu.get_degreCourant() < a_liquide.get_degreEbullition())
                {
                    temp1 = a_feu.get_vitesseChauffe() * (chrono_actuelle / 1000);
                    a_feu.set_degreCourant(temp1);

                    if (a_feu.get_degreCourant() > a_liquide.get_degreEbullition())
                    {
                        a_feu.set_degreCourant(a_liquide.get_degreEbullition());
                    }

                    message = "la température de la plaque monte à température d'ébullition, sa température est de " + a_feu.get_degreCourant() + " °C";
                    statut = '3';

                }
                else if (a_liquide.get_temperatureLiquide() < a_liquide.get_degreEbullition())
                {
                    a_feu.maintenirFeu(a_liquide.get_temperatureLiquide());
                    message = "Le liquide est en cours de chauffe, sa température est de " + a_liquide.get_temperatureLiquide() + " °C";
                    statut = '4';
                }
                else
                {
                    double diminution = a_recipient.get_volumeActuel();
                    diminution = a_recipient.get_volumeActuel() - (a_recipient.get_coefficientLiquideContenut() / (a_recipient.get_coefficientLiquideContenut() - 1));
                    a_recipient.set_volumeActuel(diminution);
                    //pour eviter que la temperature depace celle dï¿½bulition
                    message = "Le liquide est en cours d'évaporation, son volume est de " + a_recipient.get_volumeActuel() + " Cl";
                    statut = '5';
                }
                if (a_recipient.get_volumeActuel() == 0)
                {
                    message = "il ne reste plus de liquide";
                    statut = '6';
                }
            }
        }
        public Form1()
        {
            InitializeComponent();
            experience = new Lancement(); //instanciation de "Metier"
            liquides = experience.get_tabLiquide(); // récuperation des liquides par la l'instanciation de metier
            recipients = experience.get_tabRecipient(); // récuperation des recipients par la l'instanciation de metier
            feux = experience.get_tabFeu(); // récuperation des feux par la l'instanciation de metier

            // for (int i = 0; i < liquides.Count(); i++)
            for (int i = 0; i < 3; i++)
            {
                listBox1.Items.Add(liquides[i]); // ajout des liquides dans la listsbox1
                                                 // MessageBox.Show(liquides[i]);  
                listBox2.Items.Add(recipients[i]); //ajout des recipients dans la listbox2
                listBox3.Items.Add(feux[i]);    // ajout des feux dans la listbox3
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            liqpnumb = listBox1.SelectedIndex; // liqpnumb récupere l'index selectionner
            // MessageBox.Show(liqpnumb.ToString());


        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            recipnumb = listBox2.SelectedIndex;  // récupere l'index selectionner
                                                 // MessageBox.Show(recipnumb.ToString());
        }

        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            feunumb = listBox3.SelectedIndex; // récupere l'index selectionner
                                              // MessageBox.Show(feunumb.ToString());
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e) // bouton RUN 
        {
            // int tmpsCourant,tmpsMax; // déclaration du temps courant et du temps max
            experience.affectationChoix(recipnumb, liqpnumb, feunumb, 50); // temps max calcul le temps que affectation choix possede (affection choix recupere tous les index selct)
            experience.lancementChauffe();
            char Locmess;
            //  tmpsCourant = 0;
            while (experience.get_statut() != 6) // boucle tant que temps courant inférieur au temps max
            {
                /*   switch (experience.get_statut()) // si experiance.get_statut possede une information
                       {
                       case Constants.ExpActif : // dans le cas ou exp.get_statut est à ExpActif il envoie un message pour dire le staut de l'exp
                           textBox1.Text = experience.get_message();
                           break; // arrete la boucle 
                       case Constants.feuAllumer: // dans le cas ou exp.get_statut est à feuAllumer il envoie un message pour dire le staut de l'exp
                           textBox1.Text = experience.get_message();
                           break; // arrete la boucle 
                       case Constants.chaufRecip: //dans le cas ou exp.get_statut est à cahufRecip il envoie un message pour dire le staut de l'exp
                           textBox1.Text = experience.get_message();
                           break; // arrete la boucle 
                       case Constants.ebulliton: // dans le cas ou exp.get_statut est à ebulliton il envoie un message pour dire le staut de l'exp
                           textBox1.Text = experience.get_message();
                           break; // arrete la boucle 
                       case Constants.evapliq: // dans le cas ou exp.get_statut est à evapliq il envoie un message pour dire le staut de l'exp
                           textBox1.Text = experience.get_message();
                           break; // arrete la boucle 
                       case Constants.Comparaison: // dans le cas ou exp.get_statut est à Comparaison il envoie un message pour dire le staut de l'exp 
                           textBox1.Text = experience.get_message();
                           break; // arrete la boucle 
                       default: // par default 
                       break; // arrete la boucle 
                       }*/

                /* textBox1.Text = experience.get_message();
                 MessageBox.Show(textBox1.Text);
                 System.Threading.Thread.Sleep(5000);
                 // tmpsCourant = tmpsCourant + 50; */

                Locmess = experience.get_statut();
                textBox1.Text = Locmess.ToString();
                System.Threading.Thread.Sleep(1000);
               // MessageBox.Show(Locmess);

            }




        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
