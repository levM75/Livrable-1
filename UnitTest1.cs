using Xunit;
using System.Linq;
using System.Collections.Generic;

namespace PSI_Rendu1.Tests
{
    public class GrapheTests
    {
        /// <summary>
        /// Teste la création initiale d'un graphe
        /// </summary>
        [Fact]
        public void Constructeur_DoitInitialiserGrapheVide()
        {
            var graphe = new Graphe(5);

            Assert.Equal(5, graphe.NombreNoeuds);
            Assert.Empty(graphe.Liens);
            Assert.All(graphe.ListeAdjacence.Values, l => Assert.Empty(l));
        }

        /// <summary>
        /// Teste l'ajout de liaisons entre nœuds
        /// </summary>
        [Theory]
        [InlineData(0, 1)]
        [InlineData(2, 3)]
        public void AjouterLien_DoitMettreAJourStructures(int n1, int n2)
        {
            var graphe = new Graphe(4);

            graphe.AjouterLien(n1, n2);

            Assert.Contains(n2, graphe.ListeAdjacence[n1]);
            Assert.Contains(n1, graphe.ListeAdjacence[n2]);
            Assert.True(graphe.MatriceAdjacence[n1, n2]);
            Assert.Single(graphe.Liens);
        }

        /// <summary>
        /// Teste la lecture depuis un fichier MTX
        /// </summary>
        [Fact]
        public void LireFichierMTX_DoitParserCorrectement()
        {
            const string contenuFichier = @"%%MatrixMarket matrix coordinate pattern symmetric
3 3 3
1 2
2 3
3 1";

            File.WriteAllText("test.mtx", contenuFichier);
            var graphe = Graphe.LireFichierMTX("test.mtx");

            Assert.Equal(3, graphe.NombreNoeuds);
            Assert.Equal(3, graphe.Liens.Count);
            Assert.Contains(1, graphe.ListeAdjacence[0]);
        }

        /// <summary>
        /// Teste la détection de connexité
        /// </summary>
        [Fact]
        public void EstConnexe_DoitDetecterCorrectement()
        {
            var grapheConnexe = new Graphe(3);
            grapheConnexe.AjouterLien(0, 1);
            grapheConnexe.AjouterLien(1, 2);

            var grapheNonConnexe = new Graphe(3);
            grapheNonConnexe.AjouterLien(0, 1);

            Assert.True(grapheConnexe.EstConnexe());
            Assert.False(grapheNonConnexe.EstConnexe());
        }

        /// <summary>
        /// Teste la détection de cycles
        /// </summary>
        [Theory]
        [InlineData(true, 0, 1, 1, 2, 2, 0)] // Triangle
        [InlineData(false, 0, 1, 1, 2)] // Chaîne
        public void ContientCycle_DoitRetournerValeurCorrecte(bool attendu, params int[] liens)
        {
            var graphe = new Graphe(3);
            for (int i = 0; i < liens.Length; i += 2)
            {
                graphe.AjouterLien(liens[i], liens[i + 1]);
            }

            Assert.Equal(attendu, graphe.ContientCycle());
        }

        /// <summary>
        /// Teste l'analyse des propriétés du graphe
        /// </summary>
        [Fact]
        public void AnalyserProprietes_DoitAfficherCaracteristiques()
        {
            var graphe = new Graphe(3);
            graphe.AjouterLien(0, 1);
            graphe.AjouterLien(1, 2);

            var output = new StringWriter();
            Console.SetOut(output);

            graphe.AnalyserProprietes();

            Assert.Contains("non orienté", output.ToString());
            Assert.Contains("non pondéré", output.ToString());
        }

        /// <summary>
        /// Teste le parcours en largeur (BFS)
        /// </summary>
        [Fact]
        public void ParcoursLargeur_DoitRespecterOrdre()
        {
            var graphe = new Graphe(4);
            graphe.AjouterLien(0, 1);
            graphe.AjouterLien(0, 2);
            graphe.AjouterLien(1, 3);

            var resultat = graphe.ParcoursLargeur(0);

            Assert.Equal(new List<int> { 0, 1, 2, 3 }, resultat);
        }

        /// <summary>
        /// Teste le parcours en profondeur (DFS)
        /// </summary>
        [Fact]
        public void ParcoursProfondeur_DoitRespecterOrdre()
        {
            var graphe = new Graphe(4);
            graphe.AjouterLien(0, 1);
            graphe.AjouterLien(0, 2);
            graphe.AjouterLien(1, 3);

            var resultat = graphe.ParcoursProfondeur(0);

            Assert.Equal(new List<int> { 0, 1, 3, 2 }, resultat);
        }
    }
}