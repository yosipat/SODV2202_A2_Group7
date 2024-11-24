using System;
using System.Data;
using System.Reflection;


namespace Group7_Assignment2_NHL
{

   
    public partial class Form1 : Form
    {
        public class NHLData
        {
            public string Name { get; set; }
            public string Team { get; set; }
            public string Pos { get; set; }
            public int GP { get; set; }
            public int G { get; set; }
            public int A { get; set; }
            public int P { get; set; }
            public int Diff { get; set; } //+/-
            public int PIM { get; set; }
            public double PGP { get; set; }
            public int PPG { get; set; }
            public int PPP { get; set; }
            public int SHG { get; set; }
            public int SHP { get; set; }
            public int GWG { get; set; }
            public int OTG { get; set; }
            public int S { get; set; }
            public double SPercent { get; set; }
            public string TOI { get; set; }
            public double ShiftsGP { get; set; }
            public double FOW { get; set; }

            public NHLData(string[] data)
            {
                double d;
                Name = data[0];
                Team = data[1];
                Pos = data[2];
                GP = Convert.ToInt32(data[3]);
                G = Convert.ToInt32(data[4]);
                A = Convert.ToInt32(data[5]);
                P = Convert.ToInt32(data[6]);
                Diff = Convert.ToInt32(data[7]); //+/-
                PIM = Convert.ToInt32(data[8]);
                PGP = double.TryParse(data[9], out d) ? d : 0;
                PPG = Convert.ToInt32(data[10]);
                PPP = Convert.ToInt32(data[11]);
                SHG = Convert.ToInt32(data[12]);
                SHP = Convert.ToInt32(data[13]);
                GWG = Convert.ToInt32(data[14]);
                OTG = Convert.ToInt32(data[15]);
                S = Convert.ToInt32(data[16]);
                SPercent = double.TryParse(data[17], out d) ? d : 0;
                TOI = data[18];
                ShiftsGP = double.TryParse(data[19], out d) ? d : 0;
                FOW = double.TryParse(data[20], out d) ? d : 0;
            }
        }
        public Form1()
        {
            InitializeComponent();
            BuildDBFromFile();
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
            dataGridView1.Rows.Clear();
            foreach (NHLData d in data)
            {
                dataGridView1.Rows.Add(d.Name, d.Team, d.Pos, d.GP, d.G, d.A, d.P, d.Diff, d.PIM, d.PGP, d.PPG, d.PPP, d.SHG, d.SHP, d.GWG, d.OTG, d.S, d.SPercent, d.TOI, d.ShiftsGP, d.FOW);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string condition = txtSearch.Text;

            if (!string.IsNullOrEmpty(condition))
            {
                // check condition
                var filteredData = ApplyFilters(Datas, condition); // filter
                filteredData = ApplySorting(filteredData, condition); // sort
                ShowTable(filteredData);
            }
            else
            {
                ShowTable(Datas);
            }
        }

        public List<NHLData> ApplyFilters(List<NHLData> players, string filterExpression)
        {
            var filters = filterExpression.Split(',');
            IEnumerable<NHLData> query = players;

            try
            {
                foreach (var filter in filters)
                {
                    var parts = filter.Trim().Split(' ');

                    if (parts.Length == 3)
                    {
                        var property = parts[0];
                        var operation = parts[1];
                        var value = parts[2];

                        property = ConvertProperty(property);
                        var propertyInfo = typeof(NHLData).GetProperty(property);
                        if (propertyInfo == null)
                            throw new ArgumentException($"Invalid column name: {property}");

                        if(operation != "<" && operation != "<=" && operation != ">" && operation != ">=" && operation != "==")
                        {
                            throw new ArgumentException($"Invalid operation : {operation}\nPlease use <, <=, >, >=, or ==");
                        }

                            query = query.Where(player => ApplyCondition(player, propertyInfo, operation, value));
                    }
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
            return query.ToList();
        }

        public string ConvertProperty(string value) {

            var property = value;

            if (property == "+/-") property = "Diff";
            if (property == "P/GP") property = "PGP";
            if (property == "S%") property = "SPercent";
            if (property == "TOI/GP") property = "TOI";
            if (property == "Shifts/GP") property = "ShiftsGP";
            if (property == "FOW%") property = "FOW";

            return property;
        }

        public bool ApplyCondition(NHLData player, PropertyInfo propertyInfo, string operation, string value)
        {
                var playerValue = propertyInfo.GetValue(player);

            if (propertyInfo.PropertyType == typeof(string))
            {
                switch (operation)
                {
                    case "==":
                        return playerValue.ToString().Equals(value, StringComparison.OrdinalIgnoreCase);
                    default:
                        throw new ArgumentException($"Invalid operator: {operation}");
                }
            }
            else if (propertyInfo.PropertyType == typeof(int) || propertyInfo.PropertyType == typeof(double))
            {
                double numericValue;
                double.TryParse(value, out numericValue);

                switch (operation)
                {
                    case ">":
                        return Convert.ToDouble(playerValue) > numericValue;
                    case ">=":
                        return Convert.ToDouble(playerValue) >= numericValue;
                    case "<":
                        return Convert.ToDouble(playerValue) < numericValue;
                    case "<=":
                        return Convert.ToDouble(playerValue) <= numericValue;
                    case "==":
                        return Convert.ToDouble(playerValue) == numericValue;
                    default:
                        throw new ArgumentException($"Invalid operator: {operation}");
                }
            }

            else
            {
                return true;
            }
        }



        private List<NHLData> ApplySorting(List<NHLData> players, string sortExpression)
        {
            var sorts = sortExpression.Split(',');
            IOrderedEnumerable<NHLData> query = null;

            try
            {
                foreach (var sort in sorts)
                {
                var parts = sort.Trim().Split(' ');

                if (parts.Length == 2)
                {
                    var property = parts[0];
                    var order = parts[1].ToLower();

                    property = ConvertProperty(property);

                    var propertyInfo = typeof(NHLData).GetProperty(property);
                    if (propertyInfo == null)
                        throw new ArgumentException($"Invalid property name: {property}");

                    if (order == "asc")
                    {
                        query = (query == null) ? players.OrderBy(p => propertyInfo.GetValue(p)) : query.ThenBy(p => propertyInfo.GetValue(p));
                    }
                    else if (order == "des")
                    {
                        query = (query == null) ? players.OrderByDescending(p => propertyInfo.GetValue(p)) : query.ThenByDescending(p => propertyInfo.GetValue(p));
                    }
                    else
                    {
                        throw new ArgumentException($"Invalid order: {order}");
                    }
                }

            }
        }
         catch (Exception ex)
            {
                MessageBox.Show(ex.Message+"\nPlease use asc or des.");
            }

            return (query == null)? players:query.ToList();
           
        }
    }
}
