using HopfieldNeuralNetwork;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HNN
{
    public partial class Form1 : Form
    {
        private int noOfNeurons = 100;
        private int m_iSize = 20;
        Bitmap image = null;

        private NeuralNetwork NN;
        private List<Neuron> pattern;
        private List<Neuron> initialState;
        private List<Neuron> graphic;

        private int numPat = 0;
        private int selected = 0;

        private List<String> fileName;

       

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pattern = new List<Neuron>(noOfNeurons);
            initialState = new List<Neuron>(noOfNeurons);
            NN = new NeuralNetwork(noOfNeurons);
            fileName = new List<String>();
            setInitialStatePattern();
            graphic = initialState;
        }

        private void setInitialStatePattern()
        {
            //...some initialState forming code goes
            Random rnd = new Random();
            for (int i = 0; i < noOfNeurons; i++)
            {
                Neuron neuron = new Neuron();
                initialState.Add(neuron);

                int bit;
                bit = rnd.Next(2);
                if (bit == 0) initialState[i].State = NeuronStates.AlongField;
                else
                if (bit == 1) initialState[i].State = NeuronStates.AgainstField;
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics myGraphics = e.Graphics;
            if(graphic != null)
            {
                for (int i = 0; i < graphic.Count; i++)
                {
                   if(graphic[i].State == -1)
                    {
                        myGraphics.FillRectangle(new SolidBrush(Color.Black), (i/10) * m_iSize, (i%10) * m_iSize, m_iSize, m_iSize);
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (numPat == 6) MessageBox.Show("Only up to 6 patterns are to be stored in Jimz Red's Neural Network.");
            else openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            image = new Bitmap(openFileDialog1.FileName);
            bool flag = true;
            for (int i = 0; i < fileName.Count; i++)
            {
                if (fileName[i] == openFileDialog1.FileName)
                {
                    DialogResult dr = MessageBox.Show("The pattern you chose is already in the Jimz Red's Neural Network. Confirming this will only give more bias to this pattern. Do you want to continue or not?", "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (dr == DialogResult.No) flag = false;
                    break;
                }
            }

            if(flag == true)
            {
                numPat++;
                label1.Text = "Number of Patterns: " + numPat;
                fileName.Add(openFileDialog1.FileName);

                //...some pattern forming codes
                correctPattern();
                NN.AddPattern(pattern);

                //display loaded image in picture box
                displayImage(numPat);
                this.Refresh();
            }
        }

        private void correctPattern()
        {
            for (int i = 0; i < noOfNeurons; i++)
            {
                Neuron neuron = new Neuron();
                pattern.Add(neuron);
                int bit = 0;
                if (image.GetPixel(i / image.Width, i % image.Width).GetBrightness() < 0.02)
                    bit = 1;

                if (bit == 0) pattern[i].State = NeuronStates.AlongField;
                else
                    if (bit == 1) pattern[i].State = NeuronStates.AgainstField;
            }
        }

        private void distortPattern()
        {
            int num = trackBar1.Value * 2;
            Random r = new Random();
            int p = 0;

            for (int i=0; i<num; i++)
            {
                p = r.Next(noOfNeurons);
                if(initialState[p].State == 1)
                    initialState[p].State = NeuronStates.AgainstField;
                else
                    initialState[p].State = NeuronStates.AlongField;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(numPat == 0)
            {
                MessageBox.Show("Add patterns first to the Jimz Red's Neural Network.");
            }
            else if(selected == 0){
                MessageBox.Show("Select a pattern first to run.");
            }
            else
            {
                NN.Run(initialState);
                timer1.Enabled = true;
                trackBar1.Value = 0;
                label2.Text = "Distort Level: " + trackBar1.Value * 10 + "%";
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            graphic = NN.Neurons;
            this.Refresh();
            timer1.Enabled = false;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if(numPat == 0) MessageBox.Show("Add patterns first to the Jimz Red's Neural Network.");
            else if(selected == 0) MessageBox.Show("Select a pattern to distort.");
            if (trackBar1.Value!=0) distortPattern();
            label2.Text = "Distort Level: " + trackBar1.Value * 10 + "%";
            graphic = initialState;
            this.Refresh();
        }

        private void displayImage(int num)
        {
            if (image != null)
            {
                Bitmap bmp = image;
                Bitmap zoomed;
                int width = 1, height = 1;

                switch (num)
                {
                    case 1: width = pictureBox2.Width; height = pictureBox2.Height; break;
                    case 2: width = pictureBox3.Width; height = pictureBox3.Height; break;
                    case 3: width = pictureBox4.Width; height = pictureBox4.Height; break;
                    case 4: width = pictureBox5.Width; height = pictureBox5.Height; break;
                    case 5: width = pictureBox6.Width; height = pictureBox6.Height; break;
                    case 6: width = pictureBox7.Width; height = pictureBox7.Height; break;
                }

                zoomed = new Bitmap((int)(width), (int)(height));

                using (Graphics g = Graphics.FromImage(zoomed))
                {
                    g.InterpolationMode = InterpolationMode.NearestNeighbor;
                    g.DrawImage(bmp, new Rectangle(Point.Empty, zoomed.Size));
                }

                switch (num)
                {
                    case 1: pictureBox2.Image = zoomed;break;
                    case 2: pictureBox3.Image = zoomed; break;
                    case 3: pictureBox4.Image = zoomed; break;
                    case 4: pictureBox5.Image = zoomed; break;
                    case 5: pictureBox6.Image = zoomed; break;
                    case 6: pictureBox7.Image = zoomed; break;
                }
            }
        }
               
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            selected = 1;
            image = new Bitmap(fileName[0]);
            pictureboxHelper();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            selected = 1;
            image = new Bitmap(fileName[1]);
            pictureboxHelper();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            selected = 1;
            image = new Bitmap(fileName[2]);
            pictureboxHelper();
        }

        
        private void pictureBox5_Click(object sender, EventArgs e)
        {
            selected = 1;
            image = new Bitmap(fileName[3]);
            pictureboxHelper();
        }
        private void pictureBox6_Click(object sender, EventArgs e)
        {
            selected = 1;
            image = new Bitmap(fileName[4]);
            pictureboxHelper();
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            selected = 1;
            image = new Bitmap(fileName[5]);
            pictureboxHelper();
        }

        private void pictureboxHelper()
        {
            correctPattern();
            initialState = pattern;
            graphic = initialState;
            this.Refresh();
            trackBar1.Value = 0;
        }


        private void clearBtn_Click(object sender, EventArgs e)
        {
            selected = 1;
            NN.FreeMatrix();
            pattern.Clear();
            fileName.Clear();
            pictureBox2.Image = null;
            pictureBox3.Image = null;
            pictureBox4.Image = null;
            pictureBox5.Image = null;
            pictureBox6.Image = null;
            pictureBox7.Image = null;
            trackBar1.Value = 0;
            label2.Text = "Distort Level: 0 %";
            numPat = 0;
            label1.Text = "Number of Patterns: 0";
            setInitialStatePattern();
            graphic = initialState;
            this.Refresh();
        }
    }
}
