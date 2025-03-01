using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;

namespace PSI_Rendu1
{
    /// <summary>
    /// Représente un nœud dans un graphe avec un identifiant unique
    /// </summary>
    public class Noeud
    {
        /// <summary>
        /// Identifiant numérique du nœud
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Constructeur pour créer un nouveau nœud
        /// </summary>
        /// <param name="id">Identifiant unique à attribuer au nœud</param>
        public Noeud(int id) { Id = id; }
    }

    /// <summary>
    /// Représente une connexion entre deux nœuds dans un graphe
    /// </summary>
    public class Lien
    {
        /// <summary>
        /// Premier nœud connecté par la liaison
        /// </summary>
        public Noeud Noeud1 { get; set; }

        /// <summary>
        /// Second nœud connecté par la liaison
        /// </summary>
        public Noeud Noeud2 { get; set; }

        /// <summary>
        /// Constructeur pour créer une nouvelle liaison
        /// </summary>
        /// <param name="n1">Premier nœud de la liaison</param>
        /// <param name="n2">Second nœud de la liaison</param>
        public Lien(Noeud n1, Noeud n2)
        {
            Noeud1 = n1;
            Noeud2 = n2;
        }
    }

    /// <summary>
    /// Classe principale représentant un graphe et ses opérations
    /// </summary>
    public class Graphe
    {
        /// <summary>
        /// Dictionnaire des nœuds du graphe indexés par leur ID
        /// </summary>
        public Dictionary<int, Noeud> Noeuds { get; set; }

        /// <summary>
        /// Liste de toutes les liaisons du graphe
        /// </summary>
        public List<Lien> Liens { get; set; }

        /// <summary>
        /// Représentation du graphe sous forme de liste d'adjacence
        /// </summary>
        public Dictionary<int, List<int>> ListeAdjacence { get; set; }

        /// <summary>
        /// Matrice d'adjacence booléenne représentant les connexions
        /// </summary>
        public bool[,] MatriceAdjacence { get; set; }

        /// <summary>
        /// Nombre total de nœuds dans le graphe
        /// </summary>
        public int NombreNoeuds { get; private set; }

        /// <summary>
        /// Constructeur pour initialiser un graphe avec un nombre spécifique de nœuds
        /// </summary>
        /// <param name="nombreNoeuds">Nombre initial de nœuds dans le graphe</param>
        public Graphe(int nombreNoeuds)
        {
            NombreNoeuds = nombreNoeuds;
            Noeuds = new Dictionary<int, Noeud>();
            Liens = new List<Lien>();
            ListeAdjacence = new Dictionary<int, List<int>>();
            MatriceAdjacence = new bool[nombreNoeuds, nombreNoeuds];

            for (int i = 0; i < NombreNoeuds; i++)
            {
                Noeuds[i] = new Noeud(i);
                ListeAdjacence[i] = new List<int>();
            }
        }

        /// <summary>
        /// Ajoute une liaison bidirectionnelle entre deux nœuds
        /// </summary>
        /// <param name="noeud1">ID du premier nœud</param>
        /// <param name="noeud2">ID du second nœud</param>
        public void AjouterLien(int noeud1, int noeud2)
        {
            if (!ListeAdjacence[noeud1].Contains(noeud2))
            {
                ListeAdjacence[noeud1].Add(noeud2);
                ListeAdjacence[noeud2].Add(noeud1);
                MatriceAdjacence[noeud1, noeud2] = true;
                MatriceAdjacence[noeud2, noeud1] = true;
                Liens.Add(new Lien(Noeuds[noeud1], Noeuds[noeud2]));
            }
        }

        /// <summary>
        /// Lit un graphe à partir d'un fichier au format Matrix Market (.mtx)
        /// </summary>
        /// <param name="chemin">Chemin d'accès au fichier</param>
        /// <returns>Instance de Graphe initialisée</returns>
        public static Graphe LireFichierMTX(string chemin)
        {
            using (StreamReader lecteur = new StreamReader(chemin))
            {
                string ligne;
                while ((ligne = lecteur.ReadLine()) != null && ligne.StartsWith("%")) { }

                string[] dimensions = ligne.Split();
                int nombreNoeuds = int.Parse(dimensions[0]);

                Graphe graphe = new Graphe(nombreNoeuds);

                while ((ligne = lecteur.ReadLine()) != null)
                {
                    string[] parties = ligne.Split();
                    int noeud1 = int.Parse(parties[0]) - 1;
                    int noeud2 = int.Parse(parties[1]) - 1;
                    graphe.AjouterLien(noeud1, noeud2);
                }
                return graphe;
            }
        }

        /// <summary>
        /// Génère une visualisation graphique du graphe et l'enregistre en PNG
        /// </summary>
        public void AfficherGraphe()
        {
            int largeur = 1500;
            int hauteur = 1500;
            Bitmap bitmap = new Bitmap(largeur, hauteur);
            Graphics g = Graphics.FromImage(bitmap);
            g.Clear(Color.White);
            Random rand = new Random();
            Dictionary<int, Point> positions = new Dictionary<int, Point>();
            int marge = 200;
            int espaceMin = 100;

            foreach (var noeud in Noeuds.Values)
            {
                Point nouvellePosition;
                bool collision;
                int tentatives = 0;
                do
                {
                    collision = false;
                    nouvellePosition = new Point(
                        rand.Next(marge, largeur - marge),
                        rand.Next(marge, hauteur - marge));

                    foreach (var pos in positions.Values)
                    {
                        if (Distance(nouvellePosition, pos) < espaceMin)
                        {
                            collision = true;
                            break;
                        }
                    }
                    tentatives++;
                } while (collision && tentatives < 100);

                positions[noeud.Id] = nouvellePosition;
            }

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Pen pen = new Pen(Color.FromArgb(150, Color.Gray), 1.5f);

            foreach (var lien in Liens)
            {
                Point p1 = positions[lien.Noeud1.Id];
                Point p2 = positions[lien.Noeud2.Id];
                g.DrawLine(pen, p1, p2);
            }

            foreach (var noeud in Noeuds.Values)
            {
                Point p = positions[noeud.Id];
                int diametre = 40;

                g.FillEllipse(Brushes.DarkGray,
                    p.X - diametre / 2 + 3,
                    p.Y - diametre / 2 + 3,
                    diametre,
                    diametre);
                g.FillEllipse(Brushes.SteelBlue,
                    p.X - diametre / 2,
                    p.Y - diametre / 2,
                    diametre,
                    diametre);
                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;

                g.DrawString(noeud.Id.ToString(),
                    new Font("Arial", 12, FontStyle.Bold),
                    Brushes.White,
                    new RectangleF(p.X - diametre / 2, p.Y - diametre / 2, diametre, diametre),
                    format);
            }

            bitmap.Save("graphe.png", ImageFormat.Png);
            Console.WriteLine("Graphe enregistré sous 'graphe.png'");
        }

        /// <summary>
        /// Calcule la distance euclidienne entre deux points
        /// </summary>
        private double Distance(Point a, Point b)
        {
            return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }

        /// <summary>
        /// Effectue un parcours en largeur (BFS) à partir d'un nœud donné
        /// </summary>
        /// <param name="debut">ID du nœud de départ</param>
        /// <returns>Ordre de visite des nœuds</returns>
        public List<int> ParcoursLargeur(int debut)
        {
            var visite = new bool[NombreNoeuds];
            var file = new Queue<int>();
            var ordreVisite = new List<int>();

            file.Enqueue(debut);
            visite[debut] = true;

            while (file.Count > 0)
            {
                int noeud = file.Dequeue();
                ordreVisite.Add(noeud);

                foreach (var voisin in ListeAdjacence[noeud])
                {
                    if (!visite[voisin])
                    {
                        visite[voisin] = true;
                        file.Enqueue(voisin);
                    }
                }
            }
            return ordreVisite;
        }

        /// <summary>
        /// Effectue un parcours en profondeur (DFS) à partir d'un nœud donné
        /// </summary>
        /// <param name="debut">ID du nœud de départ</param>
        /// <returns>Ordre de visite des nœuds</returns>
        public List<int> ParcoursProfondeur(int debut)
        {
            var visite = new bool[NombreNoeuds];
            var ordreVisite = new List<int>();
            ParcoursProfondeurUtil(debut, visite, ordreVisite);
            return ordreVisite;
        }

        private void ParcoursProfondeurUtil(int noeud, bool[] visite, List<int> ordreVisite)
        {
            visite[noeud] = true;
            ordreVisite.Add(noeud);
            foreach (var voisin in ListeAdjacence[noeud])
            {
                if (!visite[voisin])
                {
                    ParcoursProfondeurUtil(voisin, visite, ordreVisite);
                }
            }
        }

        /// <summary>
        /// Vérifie si le graphe est connexe
        /// </summary>
        /// <returns>True si tous les nœuds sont connectés, sinon False</returns>
        public bool EstConnexe()
        {
            return ParcoursLargeur(0).Count == NombreNoeuds;
        }

        /// <summary>
        /// Affiche les propriétés fondamentales du graphe
        /// </summary>
        public void AnalyserProprietes()
        {
            Console.WriteLine($"Ordre du graphe : {NombreNoeuds}");
            Console.WriteLine($"Taille du graphe : {Liens.Count}");
            Console.WriteLine($"Le graphe est {(EstConnexe() ? "connexe" : "non connexe")}");

            bool estNonOriente = true;
            foreach (var lien in Liens)
            {
                if (!Liens.Any(l => l.Noeud1.Id == lien.Noeud2.Id && l.Noeud2.Id == lien.Noeud1.Id))
                {
                    estNonOriente = false;
                    break;
                }
            }
            Console.WriteLine($"Le graphe est {(estNonOriente ? "non orienté" : "orienté")}");

            bool estPondere = Liens.Any(lien => lien is LienPondere);
            Console.WriteLine($"Le graphe est {(estPondere ? "pondéré" : "non pondéré")}");
        }

        /// <summary>
        /// Représente une liaison pondérée entre deux nœuds
        /// </summary>
        public class LienPondere : Lien
        {
            /// <summary>
            /// Poids associé à la liaison
            /// </summary>
            public double Poids { get; set; }

            /// <summary>
            /// Constructeur pour une liaison pondérée
            /// </summary>
            public LienPondere(Noeud n1, Noeud n2, double poids) : base(n1, n2)
            {
                Poids = poids;
            }
        }

        /// <summary>
        /// Détecte la présence d'au moins un cycle dans le graphe
        /// </summary>
        /// <returns>True si un cycle est détecté, sinon False</returns>
        public bool ContientCycle()
        {
            if (EstOriente()) // À implémenter (vérifie si le graphe est orienté)
                return ContientCycleOriente();
            else
                return ContientCycleNonOriente();
        }
        /// <summary>
        /// Vérifie si le graphe est orienté en testant la symétrie de la matrice d'adjacence
        /// </summary>
        /// <returns>True si le graphe est orienté, sinon False</returns>
        public bool EstOriente()
        {
            for (int i = 0; i < NombreNoeuds; i++)
            {
                for (int j = 0; j < NombreNoeuds; j++)
                {
                    if (MatriceAdjacence[i, j] != MatriceAdjacence[j, i])
                        return true;
                }
            }
            return false;
        }
        private bool ContientCycleNonOriente()
        {
            bool[] visite = new bool[NombreNoeuds];
            for (int i = 0; i < NombreNoeuds; i++)
                if (!visite[i] && ContientCycleNonOrienteUtil(i, -1, visite))
                    return true;
            return false;
        }

        private bool ContientCycleNonOrienteUtil(int noeud, int parent, bool[] visite)
        {
            visite[noeud] = true;
            foreach (int voisin in ListeAdjacence[noeud])
            {
                if (!visite[voisin])
                {
                    if (ContientCycleNonOrienteUtil(voisin, noeud, visite))
                        return true;
                }
                else if (voisin != parent)
                    return true;
            }
            return false;
        }
        private bool ContientCycleOriente()
        {
            int[] couleur = new int[NombreNoeuds];
            for (int i = 0; i < NombreNoeuds; i++)
                if (couleur[i] == 0 && ContientCycleOrienteUtil(i, couleur))
                    return true;
            return false;
        }

        private bool ContientCycleOrienteUtil(int noeud, int[] couleur)
        {
            couleur[noeud] = 1;
            foreach (int voisin in ListeAdjacence[noeud])
            {
                if (couleur[voisin] == 1)
                    return true;
                if (couleur[voisin] == 0 && ContientCycleOrienteUtil(voisin, couleur))
                    return true;
            }
            couleur[noeud] = 2;
            return false;
        }

        /// <summary>
        /// Classe principale contenant le point d'entrée du programme
        /// </summary>
        class Programme
        {
            static void Main(string[] args)
            {
                string chemin = "C:\\Users\\vimot\\TD Statistiques\\soc-karate.mtx";
                Graphe graphe = Graphe.LireFichierMTX(chemin);

                Console.WriteLine("=== Propriétés du graphe ===");
                Console.WriteLine($"Nombre de nœuds : {graphe.NombreNoeuds}");
                Console.WriteLine($"Nombre de liens : {graphe.Liens.Count}");
                Console.WriteLine($"Le graphe est {(graphe.EstConnexe() ? "connexe" : "non connexe")}");
                Console.WriteLine($"Le graphe est {(graphe.EstOriente() ? "orienté" : "non orienté")}");
                Console.WriteLine($"Le graphe {(graphe.ContientCycle() ? "contient" : "ne contient pas")} un cycle");

                Console.WriteLine("\n=== Matrice d'adjacence ===");
                for (int i = 0; i < graphe.NombreNoeuds; i++)
                {
                    for (int j = 0; j < graphe.NombreNoeuds; j++)
                    {
                        Console.Write(graphe.MatriceAdjacence[i, j] ? "1 " : "0 ");
                    }
                    Console.WriteLine();
                }

                Console.WriteLine("\n=== Liste d'adjacence ===");
                foreach (var noeud in graphe.ListeAdjacence)
                {
                    Console.WriteLine($"Nœud {noeud.Key} : {string.Join(", ", noeud.Value)}");
                }

                Console.WriteLine("\n=== Parcours en largeur (BFS) ===");
                var parcoursLargeur = graphe.ParcoursLargeur(0);
                Console.WriteLine(string.Join(" -> ", parcoursLargeur));

                Console.WriteLine("\n=== Parcours en profondeur (DFS) ===");
                var parcoursProfondeur = graphe.ParcoursProfondeur(0);
                Console.WriteLine(string.Join(" -> ", parcoursProfondeur));

                Console.WriteLine("\n=== Détection de cycles ===");
                if (graphe.ContientCycle())
                {
                    Console.WriteLine("Le graphe contient au moins un cycle.");
                }
                else
                {
                    Console.WriteLine("Aucun cycle détecté dans le graphe.");
                }

                Console.WriteLine("\n=== Génération de la visualisation ===");
                graphe.AfficherGraphe();
                Console.WriteLine("Le graphe a été enregistré sous 'graphe.png'.");

                Console.WriteLine("\nAppuyez sur une touche pour quitter...");
                Console.ReadKey();
            }
        }
    }
}