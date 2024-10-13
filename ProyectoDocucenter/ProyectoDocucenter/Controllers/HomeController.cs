using ProyectoDocucenter.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Cryptography;

namespace DocucenterBFA.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        //[HttpPost]
        //public IActionResult UploadPdf(IFormFile pdfFile)
        //{
        //    if (pdfFile != null && pdfFile.Length > 0)
        //    {
        //        using (var memoryStream = new MemoryStream())
        //        {
        //            pdfFile.CopyTo(memoryStream);
        //            var pdfBytes = memoryStream.ToArray();

        //            // Convertir a base64
        //            string pdfBase64 = Convert.ToBase64String(pdfBytes);

        //            // Calcular el hash
        //            using (var sha256 = SHA256.Create())
        //            {
        //                byte[] hashBytes = sha256.ComputeHash(pdfBytes);
        //                string hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

        //                // Pasar datos a la vista
        //                ViewBag.PdfBase64 = pdfBase64;
        //                ViewBag.Hash = hash;
        //                ViewBag.Date = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        //            }
        //        }
        //    }

        //    return View("Index");
        //}

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
    }
}
