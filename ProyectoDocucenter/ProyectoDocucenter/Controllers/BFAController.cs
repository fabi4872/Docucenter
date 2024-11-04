using Microsoft.AspNetCore.Mvc;
using Nethereum.Contracts;
using Nethereum.Web3.Accounts;
using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;

namespace DocucenterBFA.Controllers
{
    public class BFAController : Controller
    {
        // Logger
        private readonly ILogger<BFAController> _logger;

        // URL del nodo de prueba
        private const string UrlNodoPrueba = "http://127.0.0.1:7545";

        // Chain ID (Network ID) del nodo de prueba
        private const int ChainID = 5777;

        // Key privada (Signature)
        private const string PrivateKey = "0x4dc681e132aaecb26a118729cea8c754b5e29faf25ca796cbf3d56a373775205";

        // Dirección del contrato
        private const string ContractAddress = "0x0071C2215a4DFE116611c6403335b363F0f663Cb";

        // ABI del contrato
        private const string ABI = @"[
    {
      ""inputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""constructor""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": """",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""stamplist"",
      ""outputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": ""object"",
          ""type"": ""uint256""
        },
        {
          ""internalType"": ""address"",
          ""name"": ""stamper"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""blockno"",
          ""type"": ""uint256""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function"",
      ""constant"": true
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""uint256[]"",
          ""name"": ""objectlist"",
          ""type"": ""uint256[]""
        }
      ],
      ""name"": ""put"",
      ""outputs"": [],
      ""stateMutability"": ""nonpayable"",
      ""type"": ""function""
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": ""pos"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""getStamplistPos"",
      ""outputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": """",
          ""type"": ""uint256""
        },
        {
          ""internalType"": ""address"",
          ""name"": """",
          ""type"": ""address""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": """",
          ""type"": ""uint256""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function"",
      ""constant"": true
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": ""object"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""getObjectCount"",
      ""outputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": """",
          ""type"": ""uint256""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function"",
      ""constant"": true
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": ""object"",
          ""type"": ""uint256""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""pos"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""getObjectPos"",
      ""outputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": """",
          ""type"": ""uint256""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function"",
      ""constant"": true
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""stamper"",
          ""type"": ""address""
        }
      ],
      ""name"": ""getStamperCount"",
      ""outputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": """",
          ""type"": ""uint256""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function"",
      ""constant"": true
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""address"",
          ""name"": ""stamper"",
          ""type"": ""address""
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""pos"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""getStamperPos"",
      ""outputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": """",
          ""type"": ""uint256""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function"",
      ""constant"": true
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": ""h"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""checkHash"",
      ""outputs"": [
        {
          ""internalType"": ""bool"",
          ""name"": """",
          ""type"": ""bool""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function"",
      ""constant"": true
    },
    {
      ""inputs"": [
        {
          ""internalType"": ""uint256"",
          ""name"": ""h"",
          ""type"": ""uint256""
        }
      ],
      ""name"": ""getHashData"",
      ""outputs"": [
        {
          ""internalType"": ""uint256[]"",
          ""name"": """",
          ""type"": ""uint256[]""
        },
        {
          ""internalType"": ""address[]"",
          ""name"": """",
          ""type"": ""address[]""
        },
        {
          ""internalType"": ""uint256[]"",
          ""name"": """",
          ""type"": ""uint256[]""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function"",
      ""constant"": true
    },
    {
      ""inputs"": [],
      ""name"": ""getAllHashes"",
      ""outputs"": [
        {
          ""internalType"": ""uint256[]"",
          ""name"": """",
          ""type"": ""uint256[]""
        }
      ],
      ""stateMutability"": ""view"",
      ""type"": ""function"",
      ""constant"": true
    }
]";

        public BFAController(ILogger<BFAController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetHashes()
        {
            try
            {
                var account = new Account(PrivateKey, ChainID);
                var web3 = new Web3(account, UrlNodoPrueba);

                // Activar transacciones de tipo legacy
                web3.TransactionManager.UseLegacyAsDefault = true;

                // Cargar el contrato en la dirección especificada
                var contract = web3.Eth.GetContract(ABI, ContractAddress);

                // Llamar a la función "getAllHashes" del contrato
                var getAllHashesFunction = contract.GetFunction("getAllHashes");
                var hashes = await getAllHashesFunction.CallAsync<List<BigInteger>>();

                // Convertir cada BigInteger en una cadena hexadecimal
                var hashStrings = hashes.Select(h => "0x" + h.ToString("X").ToLower()).ToList();

                // Retornar la lista de hashes en formato JSON
                return Json(new { success = true, data = hashStrings });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al interactuar con el contrato: {ex.Message}" });
            }
        }

        // Acción para almacenar un nuevo hash
        [HttpPost]
        public async Task<JsonResult> StoreHash([FromBody] HashInput input)
        {
            try
            {
                if (string.IsNullOrEmpty(input.Hash))
                {
                    return Json(new { success = false, message = "Hash no puede ser nulo o vacío." });
                }

                var account = new Account(PrivateKey);
                var web3 = new Web3(account, UrlNodoPrueba);
                web3.TransactionManager.UseLegacyAsDefault = true;

                var contract = web3.Eth.GetContract(ABI, ContractAddress);
                var putFunction = contract.GetFunction("put");

                // Convertir el hash en formato string a BigInteger
                BigInteger hashValue = BigInteger.Parse(input.Hash, System.Globalization.NumberStyles.HexNumber);

                // Crear una lista con el hash convertido
                var objectList = new List<BigInteger> { hashValue };

                var transactionHash = await putFunction.SendTransactionAsync(account.Address, new Nethereum.Hex.HexTypes.HexBigInteger(300000), null, objectList);

                return Json(new { success = true, transactionHash });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al almacenar el hash: {ex.Message}" });
            }
        }

        // Acción para consultar la información de un hash
        [HttpGet]
        public async Task<JsonResult> GetHashData([FromQuery] string hash)
        {
            try
            {
                // Validar si el hash es nulo o vacío
                if (string.IsNullOrEmpty(hash))
                    return Json(new { success = false, message = "El hash proporcionado no es válido." });

                // Eliminar el prefijo '0x' si está presente y convertir a minúsculas
                if (hash.StartsWith("0x"))
                    hash = hash.Substring(2);

                hash = hash.ToLower();

                // Convertir el hash de cadena hexadecimal a BigInteger
                BigInteger hashValue = BigInteger.Parse(hash, System.Globalization.NumberStyles.HexNumber);

                var account = new Account(PrivateKey, ChainID);
                var web3 = new Web3(account, UrlNodoPrueba);
                web3.TransactionManager.UseLegacyAsDefault = true;

                var contract = web3.Eth.GetContract(ABI, ContractAddress);
                var getHashDataFunction = contract.GetFunction("getHashData");

                // Llamar a la función "getHashData" para obtener los detalles del hash
                var result = await getHashDataFunction.CallDeserializingToObjectAsync<HashDataDto>(hashValue);

                // Verificar si BlockNumbers es nulo o está vacío
                if (result.BlockNumbers == null || result.BlockNumbers.Count == 0)
                {
                    // Devolver success como true pero data como null
                    return Json(new { success = true, data = (object)null });
                }

                // Extraer el número de bloque (usamos el primer elemento)
                BigInteger blockNumber = result.BlockNumbers[0];
                var block = await web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(new Nethereum.Hex.HexTypes.HexBigInteger(blockNumber));

                // Convertir la marca de tiempo a una fecha y hora en formato dd/MM/yyyy HH:mm:ss
                DateTimeOffset timeStamp = DateTimeOffset.FromUnixTimeSeconds((long)block.Timestamp.Value);
                DateTime argentinaTime = timeStamp.ToOffset(TimeSpan.FromHours(-3)).DateTime;
                string formattedTimeStamp = argentinaTime.ToString("dd/MM/yyyy HH:mm:ss");

                // Crear el objeto de resultado con el formato requerido
                var responseData = new
                {
                    numeroBloque = blockNumber.ToString(),
                    fechaYHoraStamp = formattedTimeStamp,
                    hash = "0x" + hash  // Asegurarse de incluir el prefijo hexadecimal
                };

                return Json(new { success = true, data = responseData });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al consultar el hash: {ex.Message}" });
            }
        }

        // DTO para mapear la respuesta de la función getHashData
        [FunctionOutput]
        public class HashDataDto
        {
            [Parameter("uint256[]", "objects", 1)]
            public List<BigInteger>? Objects { get; set; }

            [Parameter("address[]", "stampers", 2)]
            public List<string>? Stampers { get; set; }

            [Parameter("uint256[]", "blocknos", 3)]
            public List<BigInteger>? BlockNumbers { get; set; }
        }
    }

    public class StandardTokenDeployment : ContractDeploymentMessage
    {
        public static string BYTECODE = "0x608060405234801561001057600080fd5b5060006040518060600160405280600081526020013373ffffffffffffffffffffffffffffffffffffffff1681526020014381525090806001815401808255809150506001900390600052602060002090600302016000909190919091506000820151816000015560208201518160010160006101000a81548173ffffffffffffffffffffffffffffffffffffffff021916908373ffffffffffffffffffffffffffffffffffffffff16021790555060408201518160020155505061103a806100da6000396000f3fe608060405234801561001057600080fd5b506004361061009e5760003560e01c80639d192428116100665780639d19242814610181578063a08d1a4f146101b3578063aceaf4a0146101e3578063c15ed49114610215578063fe99f742146102455761009e565b80630500d7d0146100a35780633d07f5c1146100d55780633d76b7a3146100f15780634d48061b146101215780637e56bd5914610151575b600080fd5b6100bd60048036038101906100b891906109c0565b610263565b6040516100cc93929190610a3d565b60405180910390f35b6100ef60048036038101906100ea9190610bcd565b6102bd565b005b61010b600480360381019061010691906109c0565b610476565b6040516101189190610c31565b60405180910390f35b61013b600480360381019061013691906109c0565b610498565b6040516101489190610c4c565b60405180910390f35b61016b60048036038101906101669190610c67565b6104b8565b6040516101789190610c4c565b60405180910390f35b61019b600480360381019061019691906109c0565b6104f2565b6040516101aa93929190610a3d565b60405180910390f35b6101cd60048036038101906101c89190610cd3565b610595565b6040516101da9190610c4c565b60405180910390f35b6101fd60048036038101906101f891906109c0565b6105fb565b60405161020c93929190610e8f565b60405180910390f35b61022f600480360381019061022a9190610edb565b61084a565b60405161023c9190610c4c565b60405180910390f35b61024d610896565b60405161025a9190610f08565b60405180910390f35b6000818154811061027357600080fd5b90600052602060002090600302016000915090508060000154908060010160009054906101000a900473ffffffffffffffffffffffffffffffffffffffff16908060020154905083565b60008151905060005b818110156104715760008382815181106102e3576102e2610f2a565b5b60200260200101519050600060405180606001604052808381526020013373ffffffffffffffffffffffffffffffffffffffff1681526020014381525090806001815401808255809150506001900390600052602060002090600302016000909190919091506000820151816000015560208201518160010160006101000a81548173ffffffffffffffffffffffffffffffffffffffff021916908373ffffffffffffffffffffffffffffffffffffffff160217905550604082015181600201555050600060016000805490506103ba9190610f88565b905060016000838152602001908152602001600020819080600181540180825580915050600190039060005260206000200160009091909190915055600260003373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000208190806001815401808255809150506001900390600052602060002001600090919091909150555050808061046990610fbc565b9150506102c6565b505050565b6000806001600084815260200190815260200160002080549050119050919050565b600060016000838152602001908152602001600020805490509050919050565b60006001600084815260200190815260200160002082815481106104df576104de610f2a565b5b9060005260206000200154905092915050565b600080600080848154811061050a57610509610f2a565b5b906000526020600020906003020160000154600085815481106105305761052f610f2a565b5b906000526020600020906003020160010160009054906101000a900473ffffffffffffffffffffffffffffffffffffffff166000868154811061057657610575610f2a565b5b9060005260206000209060030201600201549250925092509193909250565b6000600260008473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002082815481106105e8576105e7610f2a565b5b9060005260206000200154905092915050565b606080606060006001600086815260200190815260200160002080549050905060008167ffffffffffffffff81111561063757610636610a8a565b5b6040519080825280602002602001820160405280156106655781602001602082028036833780820191505090505b50905060008267ffffffffffffffff81111561068457610683610a8a565b5b6040519080825280602002602001820160405280156106b25781602001602082028036833780820191505090505b50905060008367ffffffffffffffff8111156106d1576106d0610a8a565b5b6040519080825280602002602001820160405280156106ff5781602001602082028036833780820191505090505b50905060005b84811015610835576000600160008b8152602001908152602001600020828154811061073457610733610f2a565b5b90600052602060002001549050600080828154811061075657610755610f2a565b5b90600052602060002090600302019050806000015486848151811061077e5761077d610f2a565b5b6020026020010181815250508060010160009054906101000a900473ffffffffffffffffffffffffffffffffffffffff168584815181106107c2576107c1610f2a565b5b602002602001019073ffffffffffffffffffffffffffffffffffffffff16908173ffffffffffffffffffffffffffffffffffffffff1681525050806002015484848151811061081457610813610f2a565b5b6020026020010181815250505050808061082d90610fbc565b915050610705565b50828282965096509650505050509193909250565b6000600260008373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff168152602001908152602001600020805490509050919050565b6060600060016000805490506108ac9190610f88565b905060008167ffffffffffffffff8111156108ca576108c9610a8a565b5b6040519080825280602002602001820160405280156108f85781602001602082028036833780820191505090505b5090506000600190505b82811161096d576000818154811061091d5761091c610f2a565b5b9060005260206000209060030201600001548260018361093d9190610f88565b8151811061094e5761094d610f2a565b5b602002602001018181525050808061096590610fbc565b915050610902565b50809250505090565b6000604051905090565b600080fd5b600080fd5b6000819050919050565b61099d8161098a565b81146109a857600080fd5b50565b6000813590506109ba81610994565b92915050565b6000602082840312156109d6576109d5610980565b5b60006109e4848285016109ab565b91505092915050565b6109f68161098a565b82525050565b600073ffffffffffffffffffffffffffffffffffffffff82169050919050565b6000610a27826109fc565b9050919050565b610a3781610a1c565b82525050565b6000606082019050610a5260008301866109ed565b610a5f6020830185610a2e565b610a6c60408301846109ed565b949350505050565b600080fd5b6000601f19601f8301169050919050565b7f4e487b7100000000000000000000000000000000000000000000000000000000600052604160045260246000fd5b610ac282610a79565b810181811067ffffffffffffffff82111715610ae157610ae0610a8a565b5b80604052505050565b6000610af4610976565b9050610b008282610ab9565b919050565b600067ffffffffffffffff821115610b2057610b1f610a8a565b5b602082029050602081019050919050565b600080fd5b6000610b49610b4484610b05565b610aea565b90508083825260208201905060208402830185811115610b6c57610b6b610b31565b5b835b81811015610b955780610b8188826109ab565b845260208401935050602081019050610b6e565b5050509392505050565b600082601f830112610bb457610bb3610a74565b5b8135610bc4848260208601610b36565b91505092915050565b600060208284031215610be357610be2610980565b5b600082013567ffffffffffffffff811115610c0157610c00610985565b5b610c0d84828501610b9f565b91505092915050565b60008115159050919050565b610c2b81610c16565b82525050565b6000602082019050610c466000830184610c22565b92915050565b6000602082019050610c6160008301846109ed565b92915050565b60008060408385031215610c7e57610c7d610980565b5b6000610c8c858286016109ab565b9250506020610c9d858286016109ab565b9150509250929050565b610cb081610a1c565b8114610cbb57600080fd5b50565b600081359050610ccd81610ca7565b92915050565b60008060408385031215610cea57610ce9610980565b5b6000610cf885828601610cbe565b9250506020610d09858286016109ab565b9150509250929050565b600081519050919050565b600082825260208201905092915050565b6000819050602082019050919050565b610d488161098a565b82525050565b6000610d5a8383610d3f565b60208301905092915050565b6000602082019050919050565b6000610d7e82610d13565b610d888185610d1e565b9350610d9383610d2f565b8060005b83811015610dc4578151610dab8882610d4e565b9750610db683610d66565b925050600181019050610d97565b5085935050505092915050565b600081519050919050565b600082825260208201905092915050565b6000819050602082019050919050565b610e0681610a1c565b82525050565b6000610e188383610dfd565b60208301905092915050565b6000602082019050919050565b6000610e3c82610dd1565b610e468185610ddc565b9350610e5183610ded565b8060005b83811015610e82578151610e698882610e0c565b9750610e7483610e24565b925050600181019050610e55565b5085935050505092915050565b60006060820190508181036000830152610ea98186610d73565b90508181036020830152610ebd8185610e31565b90508181036040830152610ed18184610d73565b9050949350505050565b600060208284031215610ef157610ef0610980565b5b6000610eff84828501610cbe565b91505092915050565b60006020820190508181036000830152610f228184610d73565b905092915050565b7f4e487b7100000000000000000000000000000000000000000000000000000000600052603260045260246000fd5b7f4e487b7100000000000000000000000000000000000000000000000000000000600052601160045260246000fd5b6000610f938261098a565b9150610f9e8361098a565b9250828203905081811115610fb657610fb5610f59565b5b92915050565b6000610fc78261098a565b91507fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff8203610ff957610ff8610f59565b5b60018201905091905056fea2646970667358221220264cbb811b45754080ac56f121f7c6ce5de7c5a2fb557e9936df607d3ffe039c64736f6c63430008130033";

        public StandardTokenDeployment() : base(BYTECODE)
        {
        }

        [Parameter("uint256", "totalSupply")]
        public BigInteger TotalSupply { get; set; }
    }

    public class HashInput
    {
        public string Hash { get; set; }
    }
}
