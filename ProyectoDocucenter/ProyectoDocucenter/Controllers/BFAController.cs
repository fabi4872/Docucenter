using Microsoft.AspNetCore.Mvc;
using Nethereum.Contracts;
using Nethereum.Web3.Accounts;
using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;
using Nethereum.Hex.HexTypes;
using System;

namespace DocucenterBFA.Controllers
{
    public class BFAController : Controller
    {
        private readonly ILogger<BFAController> _logger;

        public BFAController(ILogger<BFAController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> DeployAndGetContract()
        {
            try
            {
                //DIRECCION DEL CONTRATO
                //0x6850be85c8c264ef1562ebae547fd7086c281774
                //0x06a2dabf7fec27d27f9283cb2de1cd328685510c

                var abi = @"[
	{
		""inputs"": [],
		""name"": ""getSaludo"",
		""outputs"": [
			{
				""internalType"": ""string"",
				""name"": """",
				""type"": ""string""
			}
		],
		""stateMutability"": ""view"",
		""type"": ""function""
	},
	{
		""inputs"": [],
		""name"": ""saludo"",
		""outputs"": [
			{
				""internalType"": ""string"",
				""name"": """",
				""type"": ""string""
			}
		],
		""stateMutability"": ""view"",
		""type"": ""function""
	},
	{
		""inputs"": [
			{
				""internalType"": ""string"",
				""name"": ""_newSaludo"",
				""type"": ""string""
			}
		],
		""name"": ""setSaludo"",
		""outputs"": [],
		""stateMutability"": ""nonpayable"",
		""type"": ""function""
	}
]";
                //var privateKey = "0x7580e7fb49df1c861f0050fae31c2224c6aba908e116b8da44ee8cd927b990b0";
                //var url = "http://testchain.nethereum.com:8545";
                //var chainId = 444444444500;
                var privateKey = "0x0692ca035d4cd4d4667f4bf5e7647f3aae414d14025290b0b2bc12a8ffd823c8";
                var url = "HTTP://127.0.0.1:7545";
                var chainId = 1337;
                
                var account = new Account(privateKey, chainId);
                var web3 = new Web3(account, url);

                // Activar transacciones de tipo legacy
                web3.TransactionManager.UseLegacyAsDefault = true;

                // Crear un mensaje de despliegue
                var deploymentMessage = new StandardTokenDeployment
                {
                    TotalSupply = 100000
                };

                var saldo = await web3.Eth.GetBalance.SendRequestAsync(account.Address);
                var code = await web3.Eth.GetCode.SendRequestAsync("0xA7775A3b8FD543f01f4CDEeE371140eA78d25E25");

                // Enviar la transacción de despliegue
                var transactionHash = await web3.Eth.DeployContract.SendRequestAsync(deploymentMessage.ByteCode, account.Address, new HexBigInteger(6721975));

                // Obtener el recibo de la transacción de despliegue
                var receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);

                // Esperar hasta que la transacción sea minada
                while (receipt == null)
                {
                    await Task.Delay(5000); // Esperar 5 segundos antes de reintentar
                    receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
                }

                // Obtener la dirección del contrato desplegado
                var contractAddress = receipt.ContractAddress;

                // Crear una instancia del contrato usando el ABI y la dirección del contrato desplegado
                var contract = web3.Eth.GetContract(abi, contractAddress);

                // Mostrar la dirección del contrato
                ViewBag.Message = $"Contrato desplegado exitosamente en la dirección: {contractAddress}";
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error al desplegar el contrato: {ex.Message}";
            }

            return View("Index");
        }

        [HttpGet]
        public async Task<ActionResult> HolaMundoContract()
        {
            try
            {
                // ABI del contrato ya desplegado
                var abi = @"[{""inputs"":[],""name"":""getSaludo"",""outputs"":[{""internalType"":""string"",""name"":"""",""type"":""string""}],""stateMutability"":""view"",""type"":""function""},{""inputs"":[],""name"":""saludo"",""outputs"":[{""internalType"":""string"",""name"":"""",""type"":""string""}],""stateMutability"":""view"",""type"":""function""},{""inputs"":[{""internalType"":""string"",""name"":""_newSaludo"",""type"":""string""}],""name"":""setSaludo"",""outputs"":[],""stateMutability"":""nonpayable"",""type"":""function""}]";

                // Clave privada de la cuenta
                var privateKey = "0x7580e7fb49df1c861f0050fae31c2224c6aba908e116b8da44ee8cd927b990b0";
                var chainId = 444444444500;
                var account = new Account(privateKey, chainId);
                var web3 = new Web3(account, "http://testchain.nethereum.com:8545");

                // Activar transacciones de tipo legacy
                web3.TransactionManager.UseLegacyAsDefault = true;

                var blockNumber = await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
                

                // Dirección del contrato ya desplegado
                var contractAddress = "0x06a2dabf7fec27d27f9283cb2de1cd328685510c"; // Reemplaza con la dirección de tu contrato

                // Crear una instancia del contrato usando el ABI y la dirección del contrato desplegado
                var contract = web3.Eth.GetContract(abi, contractAddress);
                var code = await web3.Eth.GetCode.SendRequestAsync(contractAddress);

                // Llamar a la función "getSaludo"
                var getSaludoFunction = contract.GetFunction("getSaludo");
                string saludo = await getSaludoFunction.CallAsync<string>();

                // Mostrar el saludo obtenido del contrato
                ViewBag.Message = $"El saludo del contrato en la dirección {contractAddress} es: {saludo}";
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error al interactuar con el contrato: {ex.Message}";
            }

            return View("Index");
        }
    }

    public class StandardTokenDeployment : ContractDeploymentMessage
    {
        public static string BYTECODE = "0x60806040526040518060400160405280600a81526020017f486f6c61204d756e646f000000000000000000000000000000000000000000008152505f90816100479190610293565b50348015610053575f80fd5b50610362565b5f81519050919050565b7f4e487b71000000000000000000000000000000000000000000000000000000005f52604160045260245ffd5b7f4e487b71000000000000000000000000000000000000000000000000000000005f52602260045260245ffd5b5f60028204905060018216806100d457607f821691505b6020821081036100e7576100e6610090565b5b50919050565b5f819050815f5260205f209050919050565b5f6020601f8301049050919050565b5f82821b905092915050565b5f600883026101497fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff8261010e565b610153868361010e565b95508019841693508086168417925050509392505050565b5f819050919050565b5f819050919050565b5f61019761019261018d8461016b565b610174565b61016b565b9050919050565b5f819050919050565b6101b08361017d565b6101c46101bc8261019e565b84845461011a565b825550505050565b5f90565b6101d86101cc565b6101e38184846101a7565b505050565b5b81811015610206576101fb5f826101d0565b6001810190506101e9565b5050565b601f82111561024b5761021c816100ed565b610225846100ff565b81016020851015610234578190505b610248610240856100ff565b8301826101e8565b50505b505050565b5f82821c905092915050565b5f61026b5f1984600802610250565b1980831691505092915050565b5f610283838361025c565b9150826002028217905092915050565b61029c82610059565b67ffffffffffffffff8111156102b5576102b4610063565b5b6102bf82546100bd565b6102ca82828561020a565b5f60209050601f8311600181146102fb575f84156102e9578287015190505b6102f38582610278565b86555061035a565b601f198416610309866100ed565b5f5b828110156103305784890151825560018201915060208501945060208101905061030b565b8683101561034d5784890151610349601f89168261025c565b8355505b6001600288020188555050505b505050505050565b6106e38061036f5f395ff3fe608060405234801561000f575f80fd5b506004361061003f575f3560e01c80630a3a9d6314610043578063b703ec5914610061578063db0702611461007f575b5f80fd5b61004b61009b565b6040516100589190610237565b60405180910390f35b610069610126565b6040516100769190610237565b60405180910390f35b61009960048036038101906100949190610394565b6101b5565b005b5f80546100a790610408565b80601f01602080910402602001604051908101604052809291908181526020018280546100d390610408565b801561011e5780601f106100f55761010080835404028352916020019161011e565b820191905f5260205f20905b81548152906001019060200180831161010157829003601f168201915b505050505081565b60605f805461013490610408565b80601f016020809104026020016040519081016040528092919081815260200182805461016090610408565b80156101ab5780601f10610182576101008083540402835291602001916101ab565b820191905f5260205f20905b81548152906001019060200180831161018e57829003601f168201915b5050505050905090565b805f90816101c391906105de565b5050565b5f81519050919050565b5f82825260208201905092915050565b8281835e5f83830152505050565b5f601f19601f8301169050919050565b5f610209826101c7565b61021381856101d1565b93506102238185602086016101e1565b61022c816101ef565b840191505092915050565b5f6020820190508181035f83015261024f81846101ff565b905092915050565b5f604051905090565b5f80fd5b5f80fd5b5f80fd5b5f80fd5b7f4e487b71000000000000000000000000000000000000000000000000000000005f52604160045260245ffd5b6102a6826101ef565b810181811067ffffffffffffffff821117156102c5576102c4610270565b5b80604052505050565b5f6102d7610257565b90506102e3828261029d565b919050565b5f67ffffffffffffffff82111561030257610301610270565b5b61030b826101ef565b9050602081019050919050565b828183375f83830152505050565b5f610338610333846102e8565b6102ce565b9050828152602081018484840111156103545761035361026c565b5b61035f848285610318565b509392505050565b5f82601f83011261037b5761037a610268565b5b813561038b848260208601610326565b91505092915050565b5f602082840312156103a9576103a8610260565b5b5f82013567ffffffffffffffff8111156103c6576103c5610264565b5b6103d284828501610367565b91505092915050565b7f4e487b71000000000000000000000000000000000000000000000000000000005f52602260045260245ffd5b5f600282049050600182168061041f57607f821691505b602082108103610432576104316103db565b5b50919050565b5f819050815f5260205f209050919050565b5f6020601f8301049050919050565b5f82821b905092915050565b5f600883026104947fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff82610459565b61049e8683610459565b95508019841693508086168417925050509392505050565b5f819050919050565b5f819050919050565b5f6104e26104dd6104d8846104b6565b6104bf565b6104b6565b9050919050565b5f819050919050565b6104fb836104c8565b61050f610507826104e9565b848454610465565b825550505050565b5f90565b610523610517565b61052e8184846104f2565b505050565b5b81811015610551576105465f8261051b565b600181019050610534565b5050565b601f8211156105965761056781610438565b6105708461044a565b8101602085101561057f578190505b61059361058b8561044a565b830182610533565b50505b505050565b5f82821c905092915050565b5f6105b65f198460080261059b565b1980831691505092915050565b5f6105ce83836105a7565b9150826002028217905092915050565b6105e7826101c7565b67ffffffffffffffff811115610600576105ff610270565b5b61060a8254610408565b610615828285610555565b5f60209050601f831160018114610646575f8415610634578287015190505b61063e85826105c3565b8655506106a5565b601f19841661065486610438565b5f5b8281101561067b57848901518255600182019150602085019450602081019050610656565b868310156106985784890151610694601f8916826105a7565b8355505b6001600288020188555050505b50505050505056fea264697066735822122033aa2c9d5a8f0f4e00216fe0b378b5ab144e8980dbe96056c874a40eb132d7f464736f6c634300081a0033";

        public StandardTokenDeployment() : base(BYTECODE)
        {
        }

        [Parameter("uint256", "totalSupply")]
        public BigInteger TotalSupply { get; set; }
    }
}
