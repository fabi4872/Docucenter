using ProyectoDocucenter.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Cryptography;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Hex.HexTypes;

namespace DocucenterBFA.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;        
        // URL del nodo de prueba de la BFA
        private const string BfaTestnetUrl = "http://public.test2.bfa.ar:8545";
        // Dirección del contrato
        private const string ContractAddress = "0xFc0f01A88bD08b988173A2354952087C9492d947";
        // ABI
        private const string ContractAbi = "[{\"inputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"constructor\",\"signature\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"name\":\"object\",\"type\":\"bytes32\"},{\"indexed\":false,\"name\":\"blockNo\",\"type\":\"uint256\"}],\"name\":\"Stamped\",\"type\":\"event\",\"signature\":\"0x3a0b5d7066b49e6f19e8199e84c7a296092240386482cc1b23a7c4e3c98a79d5\"},{\"constant\":false,\"inputs\":[{\"name\":\"objectList\",\"type\":\"bytes32[]\"}],\"name\":\"put\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\",\"signature\":\"0x3a00faae\"},{\"constant\":true,\"inputs\":[{\"name\":\"pos\",\"type\":\"uint256\"}],\"name\":\"getStamplistPos\",\"outputs\":[{\"name\":\"\",\"type\":\"bytes32\"},{\"name\":\"\",\"type\":\"address\"},{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\",\"signature\":\"0x9d192428\"},{\"constant\":true,\"inputs\":[{\"name\":\"object\",\"type\":\"bytes32\"}],\"name\":\"getObjectCount\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\",\"signature\":\"0xc2433cd3\"},{\"constant\":true,\"inputs\":[{\"name\":\"object\",\"type\":\"bytes32\"},{\"name\":\"pos\",\"type\":\"uint256\"}],\"name\":\"getObjectPos\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\",\"signature\":\"0x789bbb41\"},{\"constant\":true,\"inputs\":[{\"name\":\"object\",\"type\":\"bytes32\"},{\"name\":\"stamper\",\"type\":\"address\"}],\"name\":\"getBlockNo\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\",\"signature\":\"0xbbc48b7c\"},{\"constant\":true,\"inputs\":[{\"name\":\"stamper\",\"type\":\"address\"}],\"name\":\"getStamperCount\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\",\"signature\":\"0xc15ed491\"},{\"constant\":true,\"inputs\":[{\"name\":\"stamper\",\"type\":\"address\"},{\"name\":\"pos\",\"type\":\"uint256\"}],\"name\":\"getStamperPos\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\",\"signature\":\"0xa08d1a4f\"}]";
        // Dirección de cuenta en la BFA con ethers gratuitos
        private const string AccountAddress = "0x3fE92Fd12f2B8260e070EA7BB1F9DC126384Da6E";
        // Web3
        private Web3 web3;


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            
            // Reemplazar con clave privada de la cuenta (SIGNATURE)
            string privateKey = "0x94cd6d51dd2b18b4e4b9a9b01ac96d10bdd1b75560b15e2d2d5168231a99438b25baf2b21e06b00ad7a2fd4814d906d5e29fcb8b72a5b628306a69ab73c3360f1b";
            var account = new Account(privateKey);

            // Inicializar Web3 con el nodo de la BFA y la cuenta
            web3 = new Web3(account, BfaTestnetUrl);
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UploadPdf(IFormFile pdfFile)
        {
            if (pdfFile != null && pdfFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    pdfFile.CopyTo(memoryStream);
                    var pdfBytes = memoryStream.ToArray();

                    // Convertir a base64
                    string pdfBase64 = Convert.ToBase64String(pdfBytes);

                    // Calcular el hash
                    using (var sha256 = SHA256.Create())
                    {
                        byte[] hashBytes = sha256.ComputeHash(pdfBytes);
                        string hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                        // Retornar los datos como JSON
                        return Json(new
                        {
                            success = true,
                            code = Guid.NewGuid(),
                            date = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                            hash,
                            pdfBase64,
                        });
                    }
                }
            }

            return Json(new { success = false, message = "El archivo PDF no es válido." });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // POST: BFA/ConsultarHash
        [HttpPost]
        public async Task<ActionResult> ConsultarHash(string hashBase64)
        {
            try
            {
                // Decodificar base64 a un array de bytes
                byte[] hashBytes = Convert.FromBase64String(hashBase64);

                // Ajustar el array de bytes a 32 bytes (bytes32)
                byte[] hashBytes32 = new byte[32];
                Array.Copy(hashBytes, hashBytes32, Math.Min(hashBytes.Length, 32));

                // Cargar el contrato y la función getObjectCount
                var contract = web3.Eth.GetContract(ContractAbi, ContractAddress);
                var getObjectCountFunction = contract.GetFunction("getObjectCount");

                // Llamar a la función de consulta con el hash ajustado
                var result = await getObjectCountFunction.CallAsync<uint>(hashBytes32);

                // Mostrar el resultado en la vista
                ViewBag.HashCount = result;
                ViewBag.Message = $"El hash {hashBase64} ha sido encontrado {result} veces en la BFA.";
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error al consultar el hash: {ex.Message}";
            }

            return View("Index");
        }

        // POST: BFA/GuardarHash
        [HttpPost]
        public async Task<ActionResult> GuardarHash(string hashBase64)
        {
            try
            {
                // Decodificar base64 a un array de bytes
                byte[] hashBytes = Convert.FromBase64String(hashBase64);

                // Ajustar el array de bytes a 32 bytes (bytes32)
                byte[] hashBytes32 = new byte[32];
                Array.Copy(hashBytes, hashBytes32, Math.Min(hashBytes.Length, 32));

                // Preparar el array para la función put
                var hashList = new[] { hashBytes32 };

                // Cargar el contrato y la función put
                var contract = web3.Eth.GetContract(ContractAbi, ContractAddress);
                var putFunction = contract.GetFunction("put");

                // Enviar la transacción desde la cuenta por defecto configurada en Web3
                string transactionHash = await putFunction.SendTransactionAsync(
                    web3.TransactionManager.Account.Address, // Dirección de la cuenta
                    new HexBigInteger(210000), // Gas Limit ajustado
                    new HexBigInteger(0), // Gas Price
                    hashList
                );

                ViewBag.Message = $"Hash guardado exitosamente. Transacción: {transactionHash}";
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error al guardar el hash: {ex.Message}";
            }

            return View("Index");
        }
    }
}
