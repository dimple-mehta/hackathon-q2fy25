using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace core_web_ai_accounting_api.Plugins
{
    public class GoProposal_Plugins
    {
        private readonly List<string> _taxObligations = new List<string>();

        public GoProposal_Plugins()
        {
            for (int i = 0; i < 50; i++)
            {
                _taxObligations.Add("Hello" + i);
            }
        }

        [KernelFunction("get_tax_obligations")]
        [Description("Gets a list of income tax obligations for the end of period statement")]
        [return: Description("An array of tax obligations")]
        public Task<List<string>> GetTaxObligationsAsync()
        {
            return Task.FromResult(_taxObligations);
        }
    }
}
