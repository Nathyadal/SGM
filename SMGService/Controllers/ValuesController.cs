using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SMGService.Contracts;
using SMGService.Models;

namespace SMGService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IXMLProcess _xMLProcess;
        public ValuesController(IXMLProcess xMLProcess)
        {
            _xMLProcess = xMLProcess;
        }
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "SERVICE:", "IS RUNNING" };
        }

        /// <summary>
        /// This method recive the xml and makes de validation to acept or reject the xml
        /// </summary>
        /// <param name="tss_loan_request">XML Object</param>
        /// <returns>Json message with the result</returns>
        [HttpPost]
        [Route("ValidateXml")]
        public JsonResult ValidateXml([FromBody]Tss_loan_request tss_loan_request)
        {
            try
            {
                string HomeState = _xMLProcess.GetHomeState(tss_loan_request);
                if (!_xMLProcess.GetAge(tss_loan_request)) return MessageResult("El XML no cumple con el formato válido: " + "No cumple con la edad requerida");
                if (!_xMLProcess.GetEmployerMonth(tss_loan_request)) return MessageResult("El XML no cumple con el formato válido: " + "Debe tener al menos 12 meses de estar trabajando ");
                if (!_xMLProcess.GetLoanAmountDesired(tss_loan_request)) return MessageResult("El XML no cumple con el formato válido: " + "No cumple con el monto del prestamo deseado");
                if (HomeState != "") return MessageResult("El XML no cumple con el formato válido: " + HomeState);
                if (!_xMLProcess.GetIncomeAmount(tss_loan_request)) return MessageResult("El XML no cumple con el formato válido: " + "El salario se encuentra 15% por debajo de 2000 ó 15% por arriba");
                return MessageResult(_xMLProcess.MessageSuccess(tss_loan_request));
            }
            catch (Exception e)
            {
                return MessageResult(e.Message);
            }
        }

        /// <summary>
        /// This method recive the xml in string format  and makes de validation to acept or reject the xml
        /// </summary>
        /// <param name="tss_loan_request">XML Object</param>
        /// <returns>Json message with the result</returns>
        [HttpPost]
        [Route("ValidateXmlString")]
        public JsonResult ValidateXmlString([FromBody]string tss_loan_request)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Tss_loan_request));
                using (StringReader reader = new StringReader(tss_loan_request))
                {
                    Tss_loan_request data = (Tss_loan_request)(serializer.Deserialize(reader));

                    string HomeState = _xMLProcess.GetHomeState(data);
                    if (!_xMLProcess.GetAge(data)) return MessageResult("El XML no cumple con el formato válido: " + "No cumple con la edad requerida");
                    if (!_xMLProcess.GetEmployerMonth(data)) return MessageResult("El XML no cumple con el formato válido: " + "Debe tener al menos 12 meses de estar trabajando ");
                    if (!_xMLProcess.GetLoanAmountDesired(data)) return MessageResult("El XML no cumple con el formato válido: " + "No cumple con el monto del prestamo deseado");
                    if (HomeState != "") return MessageResult("El XML no cumple con el formato válido: " + HomeState);
                    if (!_xMLProcess.GetIncomeAmount(data)) return MessageResult("El XML no cumple con el formato válido: " + "El salario se encuentra 15% por debajo de 2000 ó 15% por arriba");
                    return MessageResult(_xMLProcess.MessageSuccess(data));
                }
            }
            catch (Exception e)
            {
                return MessageResult(e.Message);
            }
        }

        /// <summary>
        /// Get the string message and serialize teh string to json result to return a json message
        /// </summary>
        /// <param name="Message"></param>
        /// <returns>JSONRESULT with teh message</returns>
        private JsonResult MessageResult(string Message)
        {
            var resuser = JsonConvert.SerializeObject(new
            {
                message = Message,
            });
            return new JsonResult(resuser);
        }
    }
}
