using Braintree;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoneShop_Utility.BrainTree
{
    public class BrainTreeGate : IBrainTreeGate
    {
        private IBraintreeGateway _braintreeGateway { get; set; }
        public BrainTreeSettings _options { get; set; }

        public BrainTreeGate(IOptions<BrainTreeSettings> options)
        {
            _options = options.Value;
        }

        public IBraintreeGateway CreateGateway()
        {
            return new BraintreeGateway(_options.Enviroment, _options.MerchantId, _options.PublicKey, _options.PrivateKey);
        }

        public IBraintreeGateway GetGateway()
        {
            if(_braintreeGateway == null)
            {
                _braintreeGateway = CreateGateway();
            }
            return _braintreeGateway;
        }
    }
}
