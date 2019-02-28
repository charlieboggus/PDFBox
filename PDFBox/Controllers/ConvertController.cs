using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using PDFBox.Models;

namespace PDFBox.Controllers
{
    [Route("Convert")]
    public class ConvertController : Controller
    {
        [Route("")]
        [Route("index")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("word")]
        public IActionResult Word()
        {
            return View();
        }

        [Route("powerpoint")]
        public IActionResult PowerPoint()
        {
            return View();
        }

        [Route("excel")]
        public IActionResult Excel()
        {
            return View();
        }
    }
}
