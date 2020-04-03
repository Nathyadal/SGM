using SMGService.Contracts;
using SMGService.Models;
using SMGService.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMGService.Services
{
    public class XMLProcessService : IXMLProcess
    {
        #region Private methods to internal validation
        /// <summary>
        /// This method converts to valid datetime the born day contained in the xml
        /// </summary>
        /// <param name="tss_loan_request"></param>
        /// <returns>Datetime object with the born day</returns>
        private DateTime GetDate(Tss_loan_request tss_loan_request)
        {
            string dateTime = "";
            foreach (var item in tss_loan_request.Data)
            {
                switch (item.Name)
                {
                    case XMLConstant.Day: dateTime += item.Text + "/"; break;
                    case XMLConstant.Month: dateTime += item.Text + "/"; break;
                    case XMLConstant.Year: dateTime += item.Text; break;
                    default: break;
                }
            }
            return DateTime.Parse(dateTime);
        }
        /// <summary>
        /// Get the salary value contained in the xml and return the value
        /// </summary>
        /// <param name="tss_loan_request"></param>
        /// <returns>float with the salary value</returns>
        private float GetSalary(Tss_loan_request tss_loan_request)
        {
            foreach (var item in tss_loan_request.Data)
            {
                if (XMLConstant.Income_Ammount == item.Name.Trim())
                {
                    return float.Parse(item.Text);
                }
            }
            return float.Parse("0");
        }
        /// <summary>
        /// This method compare if the salary is within the established range
        /// </summary>
        /// <param name="Frequency">how many times recive the salary specified in the xml</param>
        /// <param name="Salary">the salary recived</param>
        /// <returns>bool(is true is the salary is approve or false is not)</returns>
        private bool GetAvgSalary(int Frequency, float Salary)
        {
            float RealSalary = Frequency * Salary;
            if (RealSalary >= (2000 * 0.85) && RealSalary <= (2000 * 1.15)) return true;
            return false;
        }

        #endregion

        /// <summary>
        /// Get the real age
        /// </summary>
        /// <param name="tss_loan_request"></param>
        /// <returns>bool(true>18 || false<18)</returns>
        public bool GetAge(Tss_loan_request tss_loan_request)
        {
            double AgeDate = (DateTime.Today - GetDate(tss_loan_request)).TotalDays / 365;
            if ((int)AgeDate < 18) return false;
            return true;
        }
        /// <summary>
        /// Compare how many months the employer have work for the company
        /// </summary>
        /// <param name="tss_loan_request"></param>
        /// <returns>bool (true>12 || false<12)</returns>
        public bool GetEmployerMonth(Tss_loan_request tss_loan_request)
        {
            foreach (var item in tss_loan_request.Data)
            {
                if (XMLConstant.Employer_Lenght_Month == item.Name)
                {
                    if (Convert.ToInt32(item.Text) < 12) return false;
                    else return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Get yhe amount desired specified in the xml and compare if is within the range
        /// </summary>
        /// <param name="tss_loan_request"></param>
        /// <returns>bool (true if is within the range,false is not)</returns>
        public bool GetLoanAmountDesired(Tss_loan_request tss_loan_request)
        {
            foreach (var item in tss_loan_request.Data)
            {
                if (XMLConstant.Loan_Amount_Desired == item.Name)
                {
                    if (Convert.ToInt32(item.Text) >= 200  && Convert.ToInt32(item.Text) <= 500) return true;
                    else return false;
                }
            }
            return false;
        }
        /// <summary>
        /// Methos to validate the home state
        /// </summary>
        /// <param name="tss_loan_request"></param>
        /// <returns>string message with the response</returns>
        public string GetHomeState(Tss_loan_request tss_loan_request)
        {
            foreach (var item in tss_loan_request.Data)
            {
                if (XMLConstant.Home_State == item.Name)
                {
                    if (!(item.Text.Length == 2)) return "El valor de Home_State no puede ser menor ni mayor a los dos caracteres";
                    switch (item.Text)
                    {
                        //CA, FL, NY, ND, GA, NC
                        case "CA": return "Fromato CA no es valido";
                        case "FL": return "Fromato FL no es valido";
                        case "NY": return "Fromato NY no es valido";
                        case "ND": return "Fromato ND no es valido";
                        case "GA": return "Fromato GA no es valido";
                        case "NC": return "Fromato NC no es valido";
                        default: break;
                    }
                }
            }
            return "";
        }
        /// <summary>
        /// Get the real real income amount and make the validation specified
        /// </summary>
        /// <param name="tss_loan_request"></param>
        /// <returns>bool(true if is in the range or false if is without the range)</returns>
        public bool GetIncomeAmount(Tss_loan_request tss_loan_request)
        {
            float getSalary = GetSalary(tss_loan_request);
            foreach (var item in tss_loan_request.Data)
            {
                if (XMLConstant.Income_Frequency.Contains(item.Name))
                {
                    switch (item.Text.ToLower())
                    {
                        case "weekly":
                            return GetAvgSalary(4, getSalary);
                        case "biweekly":
                            return GetAvgSalary(2, getSalary);
                        case "monthly":
                            return GetAvgSalary(1, getSalary);
                        default: break;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// This method create the success message
        /// </summary>
        /// <param name="tss_loan_request"></param>
        /// <returns>string with the message</returns>
        public string MessageSuccess(Tss_loan_request tss_loan_request)
        {
            string Message = "El XML ID:";
            foreach (var item in tss_loan_request.Data)
            {
                if (XMLConstant.Customer_Id == item.Name.Trim())
                {
                    Message += item.Text;
                }
                if (XMLConstant.First_Name == item.Name.Trim())
                {
                    Message += " "+item.Text+ ",fue aceptado con el nombre "+item.Text;
                }
                if (XMLConstant.Last_Name == item.Name.Trim())
                {
                    Message += " " + item.Text;
                }
            }
            return Message;
        }

    }
}
