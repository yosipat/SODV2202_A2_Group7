using System.Data;
using System.Security.Permissions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Group7_Assignment2_NHL
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            BuildDBFromFile();
        }

        public class NHLData
        {
            public string Name;
            public string Team;
            public string Pos;
            public int GP;
            public int G;
            public int A;
            public int P;
            public int Diff; //+/-
            public int PIM;
            public double PGP;
            public int PPG;
            public int PPP;
            public int SHG;
            public int SHP;
            public int GWG;
            public int OTG;
            public int S;
            public double SPercent;
            public string TOI;
            public double ShiftsGP;
            public double FOW;

            public NHLData(string[] data)
            {

                Name = data[0];
                Team = data[1];
                Pos = data[2];
                GP = Convert.ToInt32(data[3]);
                G = Convert.ToInt32(data[4]);
                A = Convert.ToInt32(data[5]);
                P = Convert.ToInt32(data[6]);
                Diff = Convert.ToInt32(data[7]); //+/-
                PIM = Convert.ToInt32(data[8]);
                double.TryParse(data[9], out PGP);
                PPG = Convert.ToInt32(data[10]);
                PPP = Convert.ToInt32(data[11]);
                SHG = Convert.ToInt32(data[12]);
                SHP = Convert.ToInt32(data[13]);
                GWG = Convert.ToInt32(data[14]);
                OTG = Convert.ToInt32(data[15]);
                S = Convert.ToInt32(data[16]);
                double.TryParse(data[17], out SPercent);
                TOI = data[18];
                double.TryParse(data[19], out ShiftsGP);
                double.TryParse(data[20], out FOW);
            }
        }


        List<NHLData> Datas = new List<NHLData>();
        public void BuildDBFromFile()
        {
            using (var reader = File.OpenText("NHL Player Stats 2017-18.csv"))
            {
                string input = reader.ReadLine();
                string[] column = input.Split(",");
                foreach (string s in column)
                {
                    if (s != "")
                    {
                        dataGridView1.Columns.Add(s, s);
                    }
                }

                while ((input = reader.ReadLine()) != null)
                {
                    string[] csv = input.Split(',');
                    NHLData data = new NHLData(csv);
                    Datas.Add(data);
                }

                ShowTable(Datas);
            }
        }

        public void ShowTable(List<NHLData> data)
        {
            foreach (NHLData d in data)
            {
                dataGridView1.Rows.Add(d.Name, d.Team, d.Pos, d.GP, d.G, d.A, d.P, d.Diff, d.PIM, d.PGP, d.PPG, d.PPP, d.SHG, d.SHP, d.GWG, d.OTG, d.S, d.SPercent, d.TOI, d.ShiftsGP, d.FOW);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string condition=txtSearch.Text;

            if (!string.IsNullOrEmpty(condition))
            {
                // check condition

            }
            else
            {
                ShowTable(Datas);
            }
        }
    }
}
