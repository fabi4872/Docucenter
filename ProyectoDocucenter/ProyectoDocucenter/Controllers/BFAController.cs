using Microsoft.AspNetCore.Mvc;
using Nethereum.Contracts;
using Nethereum.Web3.Accounts;
using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;
using ProyectoDocucenter.ModelsDB;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Microsoft.EntityFrameworkCore;
using Nethereum.Hex.HexConvertors.Extensions;
using System.Text;
using System.Security.Cryptography;
using System.Security.Policy;
using Nethereum.BlockchainProcessing.BlockStorage.Entities;

namespace DocucenterBFA.Controllers
{
    public class BFAController : Controller
    {
        // DB
        private readonly BFAContext _context;

        // Logger
        private readonly ILogger<BFAController> _logger;

        // URL del nodo de prueba
        private const string UrlNodoPrueba = "http://127.0.0.1:7545";

        // Chain ID (Network ID) del nodo de prueba
        private const int ChainID = 5777;

        private const string Tabla = "TRANSACCION";

        // Key privada (Signature)
        private const string PrivateKey = "0x4dc681e132aaecb26a118729cea8c754b5e29faf25ca796cbf3d56a373775205";
        private const string PrivateKeyV2 = "0xeeb38bc44e3c3cfae7318dee9dd1c14d51a359ad601c0f590b3079e692abeeff";

        // Dirección del contrato
        private const string ContractAddress = "0x0071C2215a4DFE116611c6403335b363F0f663Cb";
        private const string ContractAddressV2 = "0xe194054d31C96987B6bED1e4cD5096aD35994d91";

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

        private const string ABIV2 = @"[
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
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""idTabla"",
          ""type"": ""uint256""
        },
        {
          ""internalType"": ""string"",
          ""name"": ""nombreTabla"",
          ""type"": ""string""
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
        },
        {
          ""internalType"": ""uint256"",
          ""name"": ""idTabla"",
          ""type"": ""uint256""
        },
        {
          ""internalType"": ""string"",
          ""name"": ""nombreTabla"",
          ""type"": ""string""
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
        },
        {
          ""internalType"": ""uint256"",
          ""name"": """",
          ""type"": ""uint256""
        },
        {
          ""internalType"": ""string"",
          ""name"": """",
          ""type"": ""string""
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
        },
        {
          ""internalType"": ""uint256[]"",
          ""name"": """",
          ""type"": ""uint256[]""
        },
        {
          ""internalType"": ""string[]"",
          ""name"": """",
          ""type"": ""string[]""
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

        public BFAController(ILogger<BFAController> logger, BFAContext context)
        {
            _logger = logger;
            _context = context;
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
                var account = new Account(PrivateKeyV2, ChainID);
                var web3 = new Web3(account, UrlNodoPrueba);

                // Activar transacciones de tipo legacy
                web3.TransactionManager.UseLegacyAsDefault = true;

                // Cargar el contrato en la dirección especificada
                var contract = web3.Eth.GetContract(ABIV2, ContractAddressV2);

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

                var account = new Account(PrivateKeyV2);
                var web3 = new Web3(account, UrlNodoPrueba);
                web3.TransactionManager.UseLegacyAsDefault = true;
                Transaccion? transaccion = null;

                var contract = web3.Eth.GetContract(ABIV2, ContractAddressV2);
                var putFunction = contract.GetFunction("put");

                // Convertir el hash a BigInteger sin signo usando HexToBigInteger
                BigInteger hashValue = input.Hash.HexToBigInteger(false);

                // Convertir de vuelta a hexadecimal sin ceros iniciales
                string hashHex = "0x" + hashValue.ToString("X");

                // Guardar en la base de datos el hash en formato hexadecimal sin ceros iniciales
                bool exito = await this.GuardarTransaccionEnDB(input.Base64, hashHex);
                if (exito)
                {
                    transaccion = await this.ObtenerTransaccionEnDB(hashHex);

                    if (transaccion != null)
                    {
                        // Crear una lista con el hash convertido para la blockchain
                        var objectList = new List<BigInteger> { hashValue };

                        // Llamada al método 'put' del contrato, incluyendo idTabla y nombreTabla
                        var transactionHash = await putFunction.SendTransactionAsync(
                            account.Address,
                            new Nethereum.Hex.HexTypes.HexBigInteger(300000),
                            null,
                            objectList,
                            transaccion.Id,
                            Tabla
                        );

                        return Json(new { success = true, transactionHash });
                    }
                }

                return Json(new { success = false, message = $"Error al almacenar el hash" });
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

                // Asegurarse de que el hash tenga el prefijo '0x' y esté en minúsculas
                if (!hash.StartsWith("0x"))
                    hash = "0x" + hash;
                hash = hash.ToLower();

                // Convertir el hash a BigInteger sin signo usando HexToBigInteger
                BigInteger hashValue = hash.HexToBigInteger(false);

                var account = new Account(PrivateKeyV2, ChainID);
                var web3 = new Web3(account, UrlNodoPrueba);
                web3.TransactionManager.UseLegacyAsDefault = true;

                var contract = web3.Eth.GetContract(ABIV2, ContractAddressV2);
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
                Transaccion? tr = await this.ObtenerTransaccionEnDB(hash);
                string hashRecuperado = result.Objects != null && result.Objects.Count > 0 ? "0x" + result.Objects[0].ToString("X") : string.Empty;
                var responseData = new
                {
                    numeroBloque = blockNumber.ToString(),
                    fechaYHoraStamp = formattedTimeStamp,
                    hash = hashRecuperado,
                    base64 = tr != null ? tr.Base64 : null,
                    idTabla = result.IdTablas != null && result.IdTablas.Count > 0 ? result.IdTablas[0].ToString() : string.Empty, // Extrae el primer valor de idTabla
                    nombreTabla = result.NombreTablas != null && result.NombreTablas.Count > 0 ? result.NombreTablas[0] : string.Empty // Extrae el primer valor de nombreTabla
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

            [Parameter("uint256[]", "idTablas", 4)]
            public List<BigInteger>? IdTablas { get; set; }

            [Parameter("string[]", "nombreTablas", 5)]
            public List<string>? NombreTablas { get; set; }
        }

        private async Task<bool> GuardarTransaccionEnDB(string base64, string hash)
        {
            try
            {
                Transaccion transaccion = new Transaccion() 
                { 
                    Base64 = base64,
                    Hash = hash 
                };

                _context.Transaccions.Add(transaccion);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return false; 
            }

            return true;
        }

        private async Task<Transaccion?> ObtenerTransaccionEnDB(string hash)
        {
            try
            {
                return await _context.Transaccions.FirstOrDefaultAsync(x => x.Hash == hash);
            }
            catch (Exception)
            {
            }

            return null;
        }
    }

    public class StandardTokenDeployment : ContractDeploymentMessage
    {
        //public static string BYTECODE = "0x608060405234801561001057600080fd5b5060006040518060600160405280600081526020013373ffffffffffffffffffffffffffffffffffffffff1681526020014381525090806001815401808255809150506001900390600052602060002090600302016000909190919091506000820151816000015560208201518160010160006101000a81548173ffffffffffffffffffffffffffffffffffffffff021916908373ffffffffffffffffffffffffffffffffffffffff16021790555060408201518160020155505061103a806100da6000396000f3fe608060405234801561001057600080fd5b506004361061009e5760003560e01c80639d192428116100665780639d19242814610181578063a08d1a4f146101b3578063aceaf4a0146101e3578063c15ed49114610215578063fe99f742146102455761009e565b80630500d7d0146100a35780633d07f5c1146100d55780633d76b7a3146100f15780634d48061b146101215780637e56bd5914610151575b600080fd5b6100bd60048036038101906100b891906109c0565b610263565b6040516100cc93929190610a3d565b60405180910390f35b6100ef60048036038101906100ea9190610bcd565b6102bd565b005b61010b600480360381019061010691906109c0565b610476565b6040516101189190610c31565b60405180910390f35b61013b600480360381019061013691906109c0565b610498565b6040516101489190610c4c565b60405180910390f35b61016b60048036038101906101669190610c67565b6104b8565b6040516101789190610c4c565b60405180910390f35b61019b600480360381019061019691906109c0565b6104f2565b6040516101aa93929190610a3d565b60405180910390f35b6101cd60048036038101906101c89190610cd3565b610595565b6040516101da9190610c4c565b60405180910390f35b6101fd60048036038101906101f891906109c0565b6105fb565b60405161020c93929190610e8f565b60405180910390f35b61022f600480360381019061022a9190610edb565b61084a565b60405161023c9190610c4c565b60405180910390f35b61024d610896565b60405161025a9190610f08565b60405180910390f35b6000818154811061027357600080fd5b90600052602060002090600302016000915090508060000154908060010160009054906101000a900473ffffffffffffffffffffffffffffffffffffffff16908060020154905083565b60008151905060005b818110156104715760008382815181106102e3576102e2610f2a565b5b60200260200101519050600060405180606001604052808381526020013373ffffffffffffffffffffffffffffffffffffffff1681526020014381525090806001815401808255809150506001900390600052602060002090600302016000909190919091506000820151816000015560208201518160010160006101000a81548173ffffffffffffffffffffffffffffffffffffffff021916908373ffffffffffffffffffffffffffffffffffffffff160217905550604082015181600201555050600060016000805490506103ba9190610f88565b905060016000838152602001908152602001600020819080600181540180825580915050600190039060005260206000200160009091909190915055600260003373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000208190806001815401808255809150506001900390600052602060002001600090919091909150555050808061046990610fbc565b9150506102c6565b505050565b6000806001600084815260200190815260200160002080549050119050919050565b600060016000838152602001908152602001600020805490509050919050565b60006001600084815260200190815260200160002082815481106104df576104de610f2a565b5b9060005260206000200154905092915050565b600080600080848154811061050a57610509610f2a565b5b906000526020600020906003020160000154600085815481106105305761052f610f2a565b5b906000526020600020906003020160010160009054906101000a900473ffffffffffffffffffffffffffffffffffffffff166000868154811061057657610575610f2a565b5b9060005260206000209060030201600201549250925092509193909250565b6000600260008473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002082815481106105e8576105e7610f2a565b5b9060005260206000200154905092915050565b606080606060006001600086815260200190815260200160002080549050905060008167ffffffffffffffff81111561063757610636610a8a565b5b6040519080825280602002602001820160405280156106655781602001602082028036833780820191505090505b50905060008267ffffffffffffffff81111561068457610683610a8a565b5b6040519080825280602002602001820160405280156106b25781602001602082028036833780820191505090505b50905060008367ffffffffffffffff8111156106d1576106d0610a8a565b5b6040519080825280602002602001820160405280156106ff5781602001602082028036833780820191505090505b50905060005b84811015610835576000600160008b8152602001908152602001600020828154811061073457610733610f2a565b5b90600052602060002001549050600080828154811061075657610755610f2a565b5b90600052602060002090600302019050806000015486848151811061077e5761077d610f2a565b5b6020026020010181815250508060010160009054906101000a900473ffffffffffffffffffffffffffffffffffffffff168584815181106107c2576107c1610f2a565b5b602002602001019073ffffffffffffffffffffffffffffffffffffffff16908173ffffffffffffffffffffffffffffffffffffffff1681525050806002015484848151811061081457610813610f2a565b5b6020026020010181815250505050808061082d90610fbc565b915050610705565b50828282965096509650505050509193909250565b6000600260008373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff168152602001908152602001600020805490509050919050565b6060600060016000805490506108ac9190610f88565b905060008167ffffffffffffffff8111156108ca576108c9610a8a565b5b6040519080825280602002602001820160405280156108f85781602001602082028036833780820191505090505b5090506000600190505b82811161096d576000818154811061091d5761091c610f2a565b5b9060005260206000209060030201600001548260018361093d9190610f88565b8151811061094e5761094d610f2a565b5b602002602001018181525050808061096590610fbc565b915050610902565b50809250505090565b6000604051905090565b600080fd5b600080fd5b6000819050919050565b61099d8161098a565b81146109a857600080fd5b50565b6000813590506109ba81610994565b92915050565b6000602082840312156109d6576109d5610980565b5b60006109e4848285016109ab565b91505092915050565b6109f68161098a565b82525050565b600073ffffffffffffffffffffffffffffffffffffffff82169050919050565b6000610a27826109fc565b9050919050565b610a3781610a1c565b82525050565b6000606082019050610a5260008301866109ed565b610a5f6020830185610a2e565b610a6c60408301846109ed565b949350505050565b600080fd5b6000601f19601f8301169050919050565b7f4e487b7100000000000000000000000000000000000000000000000000000000600052604160045260246000fd5b610ac282610a79565b810181811067ffffffffffffffff82111715610ae157610ae0610a8a565b5b80604052505050565b6000610af4610976565b9050610b008282610ab9565b919050565b600067ffffffffffffffff821115610b2057610b1f610a8a565b5b602082029050602081019050919050565b600080fd5b6000610b49610b4484610b05565b610aea565b90508083825260208201905060208402830185811115610b6c57610b6b610b31565b5b835b81811015610b955780610b8188826109ab565b845260208401935050602081019050610b6e565b5050509392505050565b600082601f830112610bb457610bb3610a74565b5b8135610bc4848260208601610b36565b91505092915050565b600060208284031215610be357610be2610980565b5b600082013567ffffffffffffffff811115610c0157610c00610985565b5b610c0d84828501610b9f565b91505092915050565b60008115159050919050565b610c2b81610c16565b82525050565b6000602082019050610c466000830184610c22565b92915050565b6000602082019050610c6160008301846109ed565b92915050565b60008060408385031215610c7e57610c7d610980565b5b6000610c8c858286016109ab565b9250506020610c9d858286016109ab565b9150509250929050565b610cb081610a1c565b8114610cbb57600080fd5b50565b600081359050610ccd81610ca7565b92915050565b60008060408385031215610cea57610ce9610980565b5b6000610cf885828601610cbe565b9250506020610d09858286016109ab565b9150509250929050565b600081519050919050565b600082825260208201905092915050565b6000819050602082019050919050565b610d488161098a565b82525050565b6000610d5a8383610d3f565b60208301905092915050565b6000602082019050919050565b6000610d7e82610d13565b610d888185610d1e565b9350610d9383610d2f565b8060005b83811015610dc4578151610dab8882610d4e565b9750610db683610d66565b925050600181019050610d97565b5085935050505092915050565b600081519050919050565b600082825260208201905092915050565b6000819050602082019050919050565b610e0681610a1c565b82525050565b6000610e188383610dfd565b60208301905092915050565b6000602082019050919050565b6000610e3c82610dd1565b610e468185610ddc565b9350610e5183610ded565b8060005b83811015610e82578151610e698882610e0c565b9750610e7483610e24565b925050600181019050610e55565b5085935050505092915050565b60006060820190508181036000830152610ea98186610d73565b90508181036020830152610ebd8185610e31565b90508181036040830152610ed18184610d73565b9050949350505050565b600060208284031215610ef157610ef0610980565b5b6000610eff84828501610cbe565b91505092915050565b60006020820190508181036000830152610f228184610d73565b905092915050565b7f4e487b7100000000000000000000000000000000000000000000000000000000600052603260045260246000fd5b7f4e487b7100000000000000000000000000000000000000000000000000000000600052601160045260246000fd5b6000610f938261098a565b9150610f9e8361098a565b9250828203905081811115610fb657610fb5610f59565b5b92915050565b6000610fc78261098a565b91507fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff8203610ff957610ff8610f59565b5b60018201905091905056fea2646970667358221220264cbb811b45754080ac56f121f7c6ce5de7c5a2fb557e9936df607d3ffe039c64736f6c63430008130033";
        public static string BYTECODE = "0x60806040523480156200001157600080fd5b5060006040518060a00160405280600081526020013373ffffffffffffffffffffffffffffffffffffffff168152602001438152602001600081526020016040518060200160405280600081525081525090806001815401808255809150506001900390600052602060002090600502016000909190919091506000820151816000015560208201518160010160006101000a81548173ffffffffffffffffffffffffffffffffffffffff021916908373ffffffffffffffffffffffffffffffffffffffff160217905550604082015181600201556060820151816003015560808201518160040190816200010791906200038a565b50505062000471565b600081519050919050565b7f4e487b7100000000000000000000000000000000000000000000000000000000600052604160045260246000fd5b7f4e487b7100000000000000000000000000000000000000000000000000000000600052602260045260246000fd5b600060028204905060018216806200019257607f821691505b602082108103620001a857620001a76200014a565b5b50919050565b60008190508160005260206000209050919050565b60006020601f8301049050919050565b600082821b905092915050565b600060088302620002127fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff82620001d3565b6200021e8683620001d3565b95508019841693508086168417925050509392505050565b6000819050919050565b6000819050919050565b60006200026b620002656200025f8462000236565b62000240565b62000236565b9050919050565b6000819050919050565b62000287836200024a565b6200029f620002968262000272565b848454620001e0565b825550505050565b600090565b620002b6620002a7565b620002c38184846200027c565b505050565b5b81811015620002eb57620002df600082620002ac565b600181019050620002c9565b5050565b601f8211156200033a576200030481620001ae565b6200030f84620001c3565b810160208510156200031f578190505b620003376200032e85620001c3565b830182620002c8565b50505b505050565b600082821c905092915050565b60006200035f600019846008026200033f565b1980831691505092915050565b60006200037a83836200034c565b9150826002028217905092915050565b620003958262000110565b67ffffffffffffffff811115620003b157620003b06200011b565b5b620003bd825462000179565b620003ca828285620002ef565b600060209050601f831160018114620004025760008415620003ed578287015190505b620003f985826200036c565b86555062000469565b601f1984166200041286620001ae565b60005b828110156200043c5784890151825560018201915060208501945060208101905062000415565b868310156200045c578489015162000458601f8916826200034c565b8355505b6001600288020188555050505b505050505050565b61188c80620004816000396000f3fe608060405234801561001057600080fd5b506004361061009e5760003560e01c80639d192428116100665780639d19242814610183578063a08d1a4f146101b7578063aceaf4a0146101e7578063c15ed4911461021b578063fe99f7421461024b5761009e565b80630500d7d0146100a35780633d76b7a3146100d75780634d48061b146101075780637a0c4fcf146101375780637e56bd5914610153575b600080fd5b6100bd60048036038101906100b89190610c65565b610269565b6040516100ce959493929190610d72565b60405180910390f35b6100f160048036038101906100ec9190610c65565b610357565b6040516100fe9190610de7565b60405180910390f35b610121600480360381019061011c9190610c65565b610379565b60405161012e9190610e02565b60405180910390f35b610151600480360381019061014c919061101a565b610399565b005b61016d600480360381019061016891906110a5565b610580565b60405161017a9190610e02565b60405180910390f35b61019d60048036038101906101989190610c65565b6105ba565b6040516101ae959493929190610d72565b60405180910390f35b6101d160048036038101906101cc9190611111565b6106bf565b6040516101de9190610e02565b60405180910390f35b61020160048036038101906101fc9190610c65565b610725565b6040516102129594939291906113d9565b60405180910390f35b6102356004803603810190610230919061144f565b610aef565b6040516102429190610e02565b60405180910390f35b610253610b3b565b604051610260919061147c565b60405180910390f35b6000818154811061027957600080fd5b90600052602060002090600502016000915090508060000154908060010160009054906101000a900473ffffffffffffffffffffffffffffffffffffffff16908060020154908060030154908060040180546102d4906114cd565b80601f0160208091040260200160405190810160405280929190818152602001828054610300906114cd565b801561034d5780601f106103225761010080835404028352916020019161034d565b820191906000526020600020905b81548152906001019060200180831161033057829003601f168201915b5050505050905085565b6000806001600084815260200190815260200160002080549050119050919050565b600060016000838152602001908152602001600020805490509050919050565b60008351905060005b818110156105795760008582815181106103bf576103be6114fe565b5b6020026020010151905060006040518060a001604052808381526020013373ffffffffffffffffffffffffffffffffffffffff1681526020014381526020018781526020018681525090806001815401808255809150506001900390600052602060002090600502016000909190919091506000820151816000015560208201518160010160006101000a81548173ffffffffffffffffffffffffffffffffffffffff021916908373ffffffffffffffffffffffffffffffffffffffff160217905550604082015181600201556060820151816003015560808201518160040190816104ab91906116d9565b505050600060016000805490506104c291906117da565b905060016000838152602001908152602001600020819080600181540180825580915050600190039060005260206000200160009091909190915055600260003373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff168152602001908152602001600020819080600181540180825580915050600190039060005260206000200160009091909190915055505080806105719061180e565b9150506103a2565b5050505050565b60006001600084815260200190815260200160002082815481106105a7576105a66114fe565b5b9060005260206000200154905092915050565b600080600080606060008087815481106105d7576105d66114fe565b5b9060005260206000209060050201905080600001548160010160009054906101000a900473ffffffffffffffffffffffffffffffffffffffff16826002015483600301548460040180805461062b906114cd565b80601f0160208091040260200160405190810160405280929190818152602001828054610657906114cd565b80156106a45780601f10610679576101008083540402835291602001916106a4565b820191906000526020600020905b81548152906001019060200180831161068757829003601f168201915b50505050509050955095509550955095505091939590929450565b6000600260008473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000208281548110610712576107116114fe565b5b9060005260206000200154905092915050565b606080606080606060006001600088815260200190815260200160002080549050905060008167ffffffffffffffff81111561076457610763610e22565b5b6040519080825280602002602001820160405280156107925781602001602082028036833780820191505090505b50905060008267ffffffffffffffff8111156107b1576107b0610e22565b5b6040519080825280602002602001820160405280156107df5781602001602082028036833780820191505090505b50905060008367ffffffffffffffff8111156107fe576107fd610e22565b5b60405190808252806020026020018201604052801561082c5781602001602082028036833780820191505090505b50905060008467ffffffffffffffff81111561084b5761084a610e22565b5b6040519080825280602002602001820160405280156108795781602001602082028036833780820191505090505b50905060008567ffffffffffffffff81111561089857610897610e22565b5b6040519080825280602002602001820160405280156108cb57816020015b60608152602001906001900390816108b65790505b50905060005b86811015610ad0576000600160008f81526020019081526020016000208281548110610900576108ff6114fe565b5b906000526020600020015490506000808281548110610922576109216114fe565b5b90600052602060002090600502019050806000015488848151811061094a576109496114fe565b5b6020026020010181815250508060010160009054906101000a900473ffffffffffffffffffffffffffffffffffffffff1687848151811061098e5761098d6114fe565b5b602002602001019073ffffffffffffffffffffffffffffffffffffffff16908173ffffffffffffffffffffffffffffffffffffffff168152505080600201548684815181106109e0576109df6114fe565b5b6020026020010181815250508060030154858481518110610a0457610a036114fe565b5b602002602001018181525050806004018054610a1f906114cd565b80601f0160208091040260200160405190810160405280929190818152602001828054610a4b906114cd565b8015610a985780601f10610a6d57610100808354040283529160200191610a98565b820191906000526020600020905b815481529060010190602001808311610a7b57829003601f168201915b5050505050848481518110610ab057610aaf6114fe565b5b602002602001018190525050508080610ac89061180e565b9150506108d1565b5084848484849a509a509a509a509a5050505050505091939590929450565b6000600260008373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff168152602001908152602001600020805490509050919050565b606060006001600080549050610b5191906117da565b905060008167ffffffffffffffff811115610b6f57610b6e610e22565b5b604051908082528060200260200182016040528015610b9d5781602001602082028036833780820191505090505b5090506000600190505b828111610c125760008181548110610bc257610bc16114fe565b5b90600052602060002090600502016000015482600183610be291906117da565b81518110610bf357610bf26114fe565b5b6020026020010181815250508080610c0a9061180e565b915050610ba7565b50809250505090565b6000604051905090565b600080fd5b600080fd5b6000819050919050565b610c4281610c2f565b8114610c4d57600080fd5b50565b600081359050610c5f81610c39565b92915050565b600060208284031215610c7b57610c7a610c25565b5b6000610c8984828501610c50565b91505092915050565b610c9b81610c2f565b82525050565b600073ffffffffffffffffffffffffffffffffffffffff82169050919050565b6000610ccc82610ca1565b9050919050565b610cdc81610cc1565b82525050565b600081519050919050565b600082825260208201905092915050565b60005b83811015610d1c578082015181840152602081019050610d01565b60008484015250505050565b6000601f19601f8301169050919050565b6000610d4482610ce2565b610d4e8185610ced565b9350610d5e818560208601610cfe565b610d6781610d28565b840191505092915050565b600060a082019050610d876000830188610c92565b610d946020830187610cd3565b610da16040830186610c92565b610dae6060830185610c92565b8181036080830152610dc08184610d39565b90509695505050505050565b60008115159050919050565b610de181610dcc565b82525050565b6000602082019050610dfc6000830184610dd8565b92915050565b6000602082019050610e176000830184610c92565b92915050565b600080fd5b7f4e487b7100000000000000000000000000000000000000000000000000000000600052604160045260246000fd5b610e5a82610d28565b810181811067ffffffffffffffff82111715610e7957610e78610e22565b5b80604052505050565b6000610e8c610c1b565b9050610e988282610e51565b919050565b600067ffffffffffffffff821115610eb857610eb7610e22565b5b602082029050602081019050919050565b600080fd5b6000610ee1610edc84610e9d565b610e82565b90508083825260208201905060208402830185811115610f0457610f03610ec9565b5b835b81811015610f2d5780610f198882610c50565b845260208401935050602081019050610f06565b5050509392505050565b600082601f830112610f4c57610f4b610e1d565b5b8135610f5c848260208601610ece565b91505092915050565b600080fd5b600067ffffffffffffffff821115610f8557610f84610e22565b5b610f8e82610d28565b9050602081019050919050565b82818337600083830152505050565b6000610fbd610fb884610f6a565b610e82565b905082815260208101848484011115610fd957610fd8610f65565b5b610fe4848285610f9b565b509392505050565b600082601f83011261100157611000610e1d565b5b8135611011848260208601610faa565b91505092915050565b60008060006060848603121561103357611032610c25565b5b600084013567ffffffffffffffff81111561105157611050610c2a565b5b61105d86828701610f37565b935050602061106e86828701610c50565b925050604084013567ffffffffffffffff81111561108f5761108e610c2a565b5b61109b86828701610fec565b9150509250925092565b600080604083850312156110bc576110bb610c25565b5b60006110ca85828601610c50565b92505060206110db85828601610c50565b9150509250929050565b6110ee81610cc1565b81146110f957600080fd5b50565b60008135905061110b816110e5565b92915050565b6000806040838503121561112857611127610c25565b5b6000611136858286016110fc565b925050602061114785828601610c50565b9150509250929050565b600081519050919050565b600082825260208201905092915050565b6000819050602082019050919050565b61118681610c2f565b82525050565b6000611198838361117d565b60208301905092915050565b6000602082019050919050565b60006111bc82611151565b6111c6818561115c565b93506111d18361116d565b8060005b838110156112025781516111e9888261118c565b97506111f4836111a4565b9250506001810190506111d5565b5085935050505092915050565b600081519050919050565b600082825260208201905092915050565b6000819050602082019050919050565b61124481610cc1565b82525050565b6000611256838361123b565b60208301905092915050565b6000602082019050919050565b600061127a8261120f565b611284818561121a565b935061128f8361122b565b8060005b838110156112c05781516112a7888261124a565b97506112b283611262565b925050600181019050611293565b5085935050505092915050565b600081519050919050565b600082825260208201905092915050565b6000819050602082019050919050565b600082825260208201905092915050565b600061131582610ce2565b61131f81856112f9565b935061132f818560208601610cfe565b61133881610d28565b840191505092915050565b600061134f838361130a565b905092915050565b6000602082019050919050565b600061136f826112cd565b61137981856112d8565b93508360208202850161138b856112e9565b8060005b858110156113c757848403895281516113a88582611343565b94506113b383611357565b925060208a0199505060018101905061138f565b50829750879550505050505092915050565b600060a08201905081810360008301526113f381886111b1565b90508181036020830152611407818761126f565b9050818103604083015261141b81866111b1565b9050818103606083015261142f81856111b1565b905081810360808301526114438184611364565b90509695505050505050565b60006020828403121561146557611464610c25565b5b6000611473848285016110fc565b91505092915050565b6000602082019050818103600083015261149681846111b1565b905092915050565b7f4e487b7100000000000000000000000000000000000000000000000000000000600052602260045260246000fd5b600060028204905060018216806114e557607f821691505b6020821081036114f8576114f761149e565b5b50919050565b7f4e487b7100000000000000000000000000000000000000000000000000000000600052603260045260246000fd5b60008190508160005260206000209050919050565b60006020601f8301049050919050565b600082821b905092915050565b60006008830261158f7fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff82611552565b6115998683611552565b95508019841693508086168417925050509392505050565b6000819050919050565b60006115d66115d16115cc84610c2f565b6115b1565b610c2f565b9050919050565b6000819050919050565b6115f0836115bb565b6116046115fc826115dd565b84845461155f565b825550505050565b600090565b61161961160c565b6116248184846115e7565b505050565b5b818110156116485761163d600082611611565b60018101905061162a565b5050565b601f82111561168d5761165e8161152d565b61166784611542565b81016020851015611676578190505b61168a61168285611542565b830182611629565b50505b505050565b600082821c905092915050565b60006116b060001984600802611692565b1980831691505092915050565b60006116c9838361169f565b9150826002028217905092915050565b6116e282610ce2565b67ffffffffffffffff8111156116fb576116fa610e22565b5b61170582546114cd565b61171082828561164c565b600060209050601f8311600181146117435760008415611731578287015190505b61173b85826116bd565b8655506117a3565b601f1984166117518661152d565b60005b8281101561177957848901518255600182019150602085019450602081019050611754565b868310156117965784890151611792601f89168261169f565b8355505b6001600288020188555050505b505050505050565b7f4e487b7100000000000000000000000000000000000000000000000000000000600052601160045260246000fd5b60006117e582610c2f565b91506117f083610c2f565b9250828203905081811115611808576118076117ab565b5b92915050565b600061181982610c2f565b91507fffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff820361184b5761184a6117ab565b5b60018201905091905056fea26469706673582212201e70a80fe80dabfb85e158b5b04d88cd53c89c133cfde82b71a3045bd16ffae764736f6c63430008130033";

        public StandardTokenDeployment() : base(BYTECODE)
        {
        }

        [Parameter("uint256", "totalSupply")]
        public BigInteger TotalSupply { get; set; }
    }

    public class HashInput
    {
        public string Hash { get; set; }
        public string Base64 { get; set; }
    }
}
