using Microsoft.Data.Sqlite;
using Microsoft.SemanticKernel;
using System.Text;
using System.Text.Json;


namespace IMPOLAssistant.SemanticKernel
{
    public class TabelaLeguraKernelService : ITabelaLeguraKernelService
    {
        private readonly Kernel kernel;
        string prompt = @"
                Imam sledeću tabelu u SQLITE bazi podataka. 
                U njoj su navedene legure, profili i odgovarajuće mašine na kojima se šaržira,
                a u zavisnosti od legure biraju se komponente čije količine su date u kolonama k_1 do k_14 a vrste sirovine u kolonama vm_1 do vm_14.
                
               
                CREATE TABLE [podaci] (
                   [legura] TEXT, -- naziv legure
                   [kolicina] TEXT, -- kolicina legure
                   [kom] TEXT, -- broj komada
                   [masina] TEXT, -- masina na kojoj se stvara legura
                   [duzina] TEXT, -- duzina legure
                   [profil] TEXT, -- profil legure
                   [k_1] TEXT, --  k1 kolicina prve sirovine
                   [k_2] TEXT, --  k2 kolicina druge sirovine
                   [k_3] TEXT, --  k3 kolicina trece sirovine
                   [k_4] TEXT, --  k4 kolicina cetvrte sirovine
                   [k_5] TEXT, --  k5 kolicina pete sirovine
                   [k_6] TEXT, --  k5 kolicina seste sirovine
                   [k_7] TEXT, --  k7 kolicina sedme sirovine
                   [k_8] TEXT, --  k8 kolicina osme sirovine
                   [k_9] TEXT, --  k9 kolicina devete sirovine
                   [k_10] TEXT, --  k10 kolicina desete sirovine
                   [k_11] TEXT, --  k11 kolicina jedanaeste sirovine
                   [k_12] TEXT, --  k12 kolicina dvanaeste sirovine
                   [k_13] TEXT, --  k13 kolicina trinaeste sirovine
                   [k_14] TEXT, --  k14 kolicina cetrnaste sirovine
                   [vm_1] TEXT, --  vm1 - vrsta materijala / naziv prve sirovine
                   [vm_2] TEXT, --  vm2 - vrsta materijala / naziv druge sirovine
                   [vm_3] TEXT, --  vm3 - vrsta materijala / naziv trece sirovine
                   [vm_4] TEXT, --  vm4 - vrsta materijala / naziv cetvrte sirovine
                   [vm_5] TEXT, --  vm5 - vrsta materijala / naziv pete sirovine
                   [vm_6] TEXT, --  vm6 - vrsta materijala / naziv seste sirovine
                   [vm_7] TEXT, --  vm7 - vrsta materijala / naziv sedme sirovine
                   [vm_8] TEXT, --  vm8 - vrsta materijala / naziv osme sirovine
                   [vm_9] TEXT, --  vm9 - vrsta materijala / naziv devete sirovine
                   [vm_10] TEXT, --  vm10 - vrsta materijala / naziv desete sirovine
                   [vm_11] TEXT, --  vm11 - vrsta materijala / naziv jedanaeste sirovine
                   [vm_12] TEXT, --  vm12 - vrsta materijala / naziv dvanaeste sirovine
                   [vm_13] TEXT, --  vm13 - vrsta materijala / naziv trinaeste sirovine
                   [vm_14] TEXT  --  vm14 - vrsta materijala / naziv cetrnaeste sirovine
                )

                Napiši SQL upit koji odgovara na pitanje:
                {0}


                Odgovori samo sa SQL kodom bez dodatnog formatiranja, taj kod će direktno biti pozvan nad bazom i mora biti tačan:
                ";

        string prompt_pitanje = @"
                Korisnik je postavio sledeće pitanje:
                {0}
                Imamo sledeći upit koji je izvršen nad tabelom da bi se dobili relevantni podaci:
                {1}
                Dobijeni su sledeći podaci:
                {2}
                Sastavi odgvor na korisnikovo pitanje, izbegavajući prazna tekstualna polja i nulte vrednosti.
                Odgovor ce čitati lice koji nije upoznato sa tehnologijama, sastaviti odgovor koristeći jednostavne termine.
                Uvek objavi sve podatke koji su dobijeni.
                Ukoliko nije moguće odgovoriti na korisnikovo pitanje, napiši ""Odgovor nije pronađen"".
                ";
        public TabelaLeguraKernelService(Kernel kernel)
        {
            this.kernel = kernel;
        }

        public async Task<string> ProcessUserQueryAsync(string query)
        {
            var result = await kernel.InvokePromptAsync(string.Format(this.prompt, query));
            var value = result.GetValue<string>();
            if (!string.IsNullOrWhiteSpace(value))
            {
                var json = ExecuteQuery(value);
                var prompt_format = string.Format(this.prompt_pitanje, query, value, json);
                var odgovor = await kernel.InvokePromptAsync(prompt_format);
                return odgovor.GetValue<string>() ?? "Odgovor nije pronađen.";
            }
            
            return value != null ? value : string.Empty;
        }

        private string ExecuteQuery(string sqlQuery)
        {

            string connectionString = "Data Source=podaci.db";
            StringBuilder result = new StringBuilder();

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var selectCmd = connection.CreateCommand();
                selectCmd.CommandText = sqlQuery;

                var rows = new List<Dictionary<string, string>>();

                using (var reader = selectCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var row = new Dictionary<string, string>();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {

                            string columnName = reader.GetName(i);
                            string columnValue = reader[i].ToString();
                            row.Add(columnName, columnValue);
                        }

                        rows.Add(row);
                    }
                }

                return JsonSerializer.Serialize(rows, new JsonSerializerOptions { WriteIndented = true });

            }

        }
    }
}

