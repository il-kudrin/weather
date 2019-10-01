using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Weather.Client.WeatherWcfReference;

namespace Weather.Client
{
    public partial class Form1 : Form
    {
        ServiceClient _client;
        public Form1(ServiceClient client)
        {
            _client = client;
            InitializeComponent();
            InitializeCombobox(_client);
        }

        private void InitializeCombobox(ServiceClient client)
        {
            string[] cities = new string[0];
            try
            {
                var cityResponse = client.GetCitiesForTomowow();
                if (string.IsNullOrEmpty(cityResponse.ErrorDescription))
                {
                    if (cityResponse.Cities != null && cityResponse.Cities.Length > 0)
                    {
                        comboBox1.Items.AddRange(cityResponse.Cities);
                    }
                    else
                    {
                        textBox1.Text = "Нет городов в списке на завтра :( запустите парсер";
                    }
                }
                else
                {
                    textBox1.Text = "Не удалось получить список городов";
                }
            }
            catch (Exception ex)
            {
                textBox1.Text = "Не удалось получить список городов";
            }
            this.comboBox1.SelectedIndexChanged +=
            new System.EventHandler(comboBox1_SelectedIndexChanged);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedCity = (string)comboBox1.SelectedItem;
            try
            {
                var cityResponse = _client.GetWeather(new GetWeaterRequest() { CityName = selectedCity });
                if (string.IsNullOrEmpty(cityResponse.ErrorDescription))
                {
                    textBox1.Text = $"Завтра({cityResponse.Date.ToString("dd-MM-yyyy")}) в городе {cityResponse.CityName} {cityResponse.Description}. Температура от {cityResponse.MinT} до {cityResponse.MaxT}";
                }
                else
                {
                    textBox1.Text = $"Не удалось получить погоду на завтра для {selectedCity}. Попробуйте другой город.";
                }

            }
            catch (Exception ex)
            {
                textBox1.Text = $"Не удалось получить погоду на завтра для {selectedCity}. Попробуйте другой город.";
            }

        }
    }
}
