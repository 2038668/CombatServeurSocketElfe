using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CombatServeurSocketElfe.Classes;

namespace CombatServeurSocketElfe
{
    public partial class frmServeurSocketElfe : Form
    {
        Random m_r;
        Nain m_nain;
        Elfe m_elfe;
        TcpListener m_ServerListener;
        Socket m_client;
        Thread m_thCombat;

        public frmServeurSocketElfe()
        {
            InitializeComponent();
            m_r = new Random();
            Reset();
            btnReset.Enabled = false;
            //Démarre un serveur de socket (TcpListener)
            m_ServerListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8888);
            m_ServerListener.Start();
            lstReception.Items.Add("Serveur démarré !");
            lstReception.Items.Add("PRESSER : << attendre un client >>");
            lstReception.Update();
            Control.CheckForIllegalCrossThreadCalls = false;
        }
        void Reset()
        {
            m_nain = new Nain(1, 0, 0);
            picNain.Image = m_nain.Avatar;
            AfficheStatNain();

            m_elfe = new Elfe(m_r.Next(10, 20), m_r.Next(2, 6), m_r.Next(2, 6));
            picElfe.Image = m_elfe.Avatar;
            AfficheStatElfe();
 
            lstReception.Items.Clear();
        }

        void AfficheStatNain()
        {
            lblVieNain.Text = "Vie: " + m_nain.Vie.ToString(); ;
            lblForceNain.Text = "Force: " + m_nain.Force.ToString();
            lblArmeNain.Text = "Arme: " + m_nain.Arme;

            this.Update(); // pour s'assurer de l'affichage via le thread
        }
        void AfficheStatElfe()
        {
            lblVieElfe.Text = "Vie: " + m_elfe.Vie.ToString();
            lblForceElfe.Text = "Force: " + m_elfe.Force.ToString();
            lblSortElfe.Text = "Sort: " + m_elfe.Sort.ToString();

            this.Update(); // pour s'assurer de l'affichage via le thread
        }
        private void btnReset_Click(object sender, EventArgs e)
        {

            Reset();
        }

        private void btnAttente_Click(object sender, EventArgs e)
        {
            // Combat par un thread
            btnReset.Enabled = true;
            ThreadStart code = new ThreadStart(Combat);
            Thread combat = new Thread(code);
            combat.Start();
        }
            
        
            
        public void Combat() 
        {
            // déclarations de variables locales 
            string reponseServeur = "aucune";
            string receptionClient = "rien";
            string[] tReception;
            int nbOctetsRec;
            byte[] tByteReception = new byte[50];
            ASCIIEncoding textByte = new ASCIIEncoding();
            byte[] tByteEnvoie;

            try
            {
                while (m_nain.Vie > 0 && m_elfe.Vie > 0)
                {
                    //initialisation d'un client (bloquant) 
                    m_client = m_ServerListener.AcceptSocket();
                    lstReception.Items.Add("Client branché !");
                    lstReception.Update();
                    Thread.Sleep(500);
                    //recoit donnees cliente
                    nbOctetsRec = m_client.Receive(tByteReception);
                    receptionClient = Encoding.ASCII.GetString(tByteReception);
                    lstReception.Items.Add("du client :" + receptionClient);
                    lstReception.Update();
                    //split pour pouvoir lire les donnees necessaires
                    tReception = receptionClient.Split(';');
                    m_nain.Vie = Convert.ToInt32(tReception[0]);
                    m_nain.Force = Convert.ToInt32(tReception[1]);
                    m_nain.Arme = tReception[2].ToString();
                    AfficheStatNain();
                    //execute frapper
                    MessageBox.Show("Serveur: Frapper l'elfe ");
                    while (m_nain.Vie > 0 && m_elfe.Vie > 0)
                    {
                        m_nain.Frapper(m_elfe);
                        AfficheStatElfe();
                        //execute LancerSort
                        m_elfe.LancerSort(m_nain);
                        AfficheStatNain();
                        AfficheStatElfe();
                    }
                        //envoi des données au client
                    reponseServeur = m_nain.Vie + ";" + m_nain.Force + ";" + m_nain.Arme + ";" + m_elfe.Vie + ";" + m_elfe.Force + ";" + m_elfe.Sort;
                    lstReception.Items.Add(reponseServeur);
                    lstReception.Update();
                    tByteEnvoie = textByte.GetBytes(reponseServeur);
                    m_client.Send(tByteEnvoie);
                    Thread.Sleep(500);
                    // Fermeture du client
                    m_client.Close();
                    //verification si il y a un gagnant

                }
            }
            catch (Exception ex)
            {
                lstReception.Items.Add("Server not not ready! CATCH exception");
                lstReception.Items.Add(ex.Message);
                lstReception.Update();
            }
            Thread.Sleep(500);
            lstReception.Items.Add("PRESSER : << attendre un client >>");
            lstReception.Update();
            btnFermer.Enabled = true;

        
        }

        private void btnFermer_Click(object sender, EventArgs e)
        {
            // il faut avoir un objet elfe et un objet nain instanciés
            //m_elfe.Vie = 0;
            //m_nain.Vie = 0;
            try
            {
                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message);
            }
        }

        private void frmServeurSocketElfe_FormClosing(object sender, FormClosingEventArgs e)
        {
            btnFermer_Click(sender,e);
            try
            {
                // il faut avoir un objet TCPListener existant
                m_ServerListener.Stop();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message);
            }
        }

        private void frmServeurSocketElfe_Load(object sender, EventArgs e)
        {

        }
    }
}
