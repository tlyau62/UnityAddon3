using UnityAddon.Core.Attributes;
using UnityAddon.Ef;
using UnityAddon.Ef.Transaction;
using UnityAddon.Ef.TransactionInterceptor;

namespace UnityAddon.EfTest.Transaction.TransactionInterceptorsException
{
    [Configuration]
    public class TransactionInterceptorsTestsConfig<T1, T2> : UnityAddonEfCustomConfig
        where T1 : ITransactionInterceptor
        where T2 : ITransactionInterceptor
    {
        [Bean]
        public override TransactionInterceptorOption TransactionInterceptorOption()
        {
            var option = new TransactionInterceptorOption();

            option.AddTransactionInterceptor<T1>();
            option.AddTransactionInterceptor<T2>();

            return option;
        }
    }
}
