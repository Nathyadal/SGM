using SMGService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMGService.Contracts
{
    public interface IXMLProcess
    {
        bool GetAge(Tss_loan_request tss_loan_request);
        bool GetEmployerMonth(Tss_loan_request tss_loan_request);
        bool GetLoanAmountDesired(Tss_loan_request tss_loan_request);
        string GetHomeState(Tss_loan_request tss_loan_request);
        bool GetIncomeAmount(Tss_loan_request tss_loan_request);
        string MessageSuccess(Tss_loan_request tss_loan_request);
    }
}
