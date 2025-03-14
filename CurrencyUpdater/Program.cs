using HttpDataGetter.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyUpdater
{
    internal class Program
    {
        private static readonly string sqlConnection = @"Data Source=LAPTOP-AKT294RC;Initial Catalog = CurrencyExchanger;Integrated Security=True";

        private static SqlCommand command;

        private static HttpClient httpClient = new HttpClient();
        private static string urlRate = "https://api.nbrb.by/exrates/rates";

        static async Task Main(string[] args)
        {
            var connection = new SqlConnection(sqlConnection);
            connection.Open();

            SqlTransaction transaction = connection.BeginTransaction();
            command = connection.CreateCommand();
            command.Transaction = transaction;

            List<int> currenciesIds = GetAllCurrencyIds();

            string updateCommand = "UPDATE Currency SET Cur_OfficialRate=";
            for (int i = 0; i < currenciesIds.Count; i++)
            {
                using (var response = await httpClient.GetAsync(urlRate + "/" + currenciesIds[i].ToString()))
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Rate rate = await response.Content.ReadFromJsonAsync<Rate>();

                        string correctedRate = RateCorrector(rate.Cur_OfficialRate.ToString());

                        string value = $"{correctedRate} WHERE Cur_ID={currenciesIds[i]}";

                        string resultCommand = updateCommand + value;
                        command.CommandText = resultCommand;
                        command.ExecuteNonQuery();

                        Console.WriteLine($"Курс валюты с Id = {currenciesIds[i]} обновлён.");
                    }
                    else
                    {
                        Console.WriteLine($"Ошибка в поле запроса с валютой Id = {currenciesIds[i]}.");
                    }
                }
            }
            transaction.Commit();
            connection.Close();
        }

        private static List<int> GetAllCurrencyIds()
        {
            List<int> result = new List<int>();

            string selectCommand = "SELECT Cur_ID FROM Currency";
            command.CommandText = selectCommand;
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(reader.GetInt32(0));
                }
            }
            return result;
        }

        public static string RateCorrector(string ratecur)
        {
            string res = "";

            if (ratecur.Length == 0)
            {
                res = "NULL";
                return res;
            }

            for (int i = 0; i < ratecur.Length; i++)
            {
                if (ratecur[i] == ',') res += '.';
                else res += ratecur[i];
            }

            return res;
        }
    }
}
