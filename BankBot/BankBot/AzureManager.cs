using BankBot.Models;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankBot.Models
{
    public class AzureManager
    {

        private static AzureManager instance;
        private MobileServiceClient client;
        private IMobileServiceTable<Bankdetails> bankTable;

        private AzureManager()
        {
            this.client = new MobileServiceClient("http://banktables.azurewebsites.net/");
            this.bankTable = this.client.GetTable<Bankdetails>();
        }

        public MobileServiceClient AzureClient
        {
            get { return client; }
        }

        public static AzureManager AzureManagerInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AzureManager();
                }

                return instance;
            }
        }

        public async Task addBank(Bankdetails bankdetails)
        {
            await this.bankTable.InsertAsync(bankdetails);
        }

        public async Task<List<Bankdetails>> GetBanks()
        {
            return await this.bankTable.ToListAsync();
        }

        public async Task UpdateBank(Bankdetails bankdetails)
        {
            await this.bankTable.UpdateAsync(bankdetails);
        }

        public async Task DeleteBank(Bankdetails bankdetails)
        {
            await this.bankTable.DeleteAsync(bankdetails);
        }

    }
}