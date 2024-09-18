using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

class Program
{
    // URL base da API
    static string URLInitial = "http://localhost:5120/";

    static async Task Main(string[] args)
    {
        int opc;
        string ObjUrl;

        do
        {
            Console.WriteLine("Bem vindo ao sistema de consulta de veículos");
            Console.WriteLine("1 - Listar todos os Veiculos");
            Console.WriteLine("2 - Cadastrar Veiculo");
            Console.WriteLine("0 - Sair\n");

            Console.Write("Informe a opção desejada:");

            opc = Convert.ToInt32(Console.ReadLine());
            switch (opc)
            {
                case 1:
                    Console.Clear();
                    Console.WriteLine("Informe o número da página de listagem:");
                    int numeroPagina = Convert.ToInt32(Console.ReadLine());

                    Console.Clear();

                    ObjUrl = "Veiculos/";
                    await ListarVeiculos(numeroPagina, ObjUrl);
                    Console.WriteLine("\nPressione qualquer tecla para voltar ao menu...");
                    Console.ReadKey();

                    Console.Clear();
                    break;
                case 2:
                    Console.Clear();
                    Console.Write("Informe o nome do Veiculo cadastrado:");
                    string? Nome = Console.ReadLine();

                    Console.Write($"Informe a marca do {Nome}: ");
                    string? Marca = Console.ReadLine();

                    Console.Write($"Infome o ano do {Nome}:");
                    int Ano = Convert.ToInt32(Console.ReadLine());

                    Console.Clear();
                    ObjUrl = "Veiculos/";
                    await CadVeiculo(Nome,Marca,Ano,ObjUrl);

                    Console.WriteLine("\nPressione qualquer tecla para voltar ao menu...");
                    Console.ReadKey();
                    Console.Clear();

                    break;
            }
        } while (opc != 0);

        Console.WriteLine("Programa finalizado");
    }

    // Método para listar veículos chamando uma API
    static async Task ListarVeiculos(int numeroPagina, string ObjUrl)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                // Monta a URL completa
                string url = URLInitial + ObjUrl + "Listar?pagina=" + numeroPagina;

                // Faz a requisição GET para a API
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                // Leitura da resposta como string
                string respostaApi = await response.Content.ReadAsStringAsync();

                var jsonElement = JsonSerializer.Deserialize<JsonElement>(respostaApi); // Deserializa a string JSON

                // Formata o JSON com indentação para facilitar a leitura
                string respostaFormatada = JsonSerializer.Serialize(jsonElement, new JsonSerializerOptions { WriteIndented = true });

                Console.WriteLine("Veículos listados pela API:");
                Console.WriteLine(respostaFormatada);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("Erro ao chamar a API: " + e.Message);
            }
        }
    }
    static async Task CadVeiculo(string? Nome, string? Marca, int Ano,string ObjUrl)
    {
        if (Nome != null && Marca != null && Ano >= 1950)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string url = URLInitial + ObjUrl + "Cadastrar";

                    var veiculo = new
                    {
                        Nome = Nome,
                        Marca = Marca,
                        Ano = Ano
                    };

                    var jsonContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(veiculo), System.Text.Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(url, jsonContent);
                    response.EnsureSuccessStatusCode();

                    string respostaApi = await response.Content.ReadAsStringAsync();

                    var jsonElement = JsonSerializer.Deserialize<JsonElement>(respostaApi); // Deserializa a string JSON

                    // Formata o JSON com indentação para facilitar a leitura
                    string respostaFormatada = JsonSerializer.Serialize(jsonElement, new JsonSerializerOptions { WriteIndented = true });

                    Console.WriteLine("Devolução da API:");
                    Console.Write(respostaFormatada);
                }
                catch (HttpRequestException e)
                {

                    Console.WriteLine($"Erro ao chamar a API: {e.Message}");

                }
            }
        }   
    }
}
