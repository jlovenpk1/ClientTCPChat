using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ServerTCPChat.Client
{
    internal class Client
    {
        public int _port { get; private set; }
        public string _ipAdress { get; private set; }
        public string _userName { get; set; }
        private string _message = string.Empty;
        private const string exit = "пока";
        private string _responceMessage = string.Empty;
        private byte[] _data;
        private const int _offset = 0;
        private const int _index = 0;
        private int _bytes = 0;
        private TcpClient _client;
        private NetworkStream _stream;
        private StringBuilder _builder;

        /// <summary>
        /// Подключиться к серверу
        /// </summary>
        public void Start()
        {
            Configuration();
            Console.WriteLine("-----------------------------");
            Console.WriteLine("Введите Ваше Имя");
            _userName = Console.ReadLine();
            _builder = new StringBuilder();
            try
            {
                _client = new TcpClient(_ipAdress, _port);
                _stream = _client.GetStream();
                _data = Encoding.Unicode.GetBytes(_userName);
                SendUserName();
                Сonnect();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine("Не удалось подключиться........");
                Start();
            }
            finally
            {
                if(_client != null)
                _client.Close();
            }
        }

        /// <summary>
        /// Отправить имя пользователя
        /// </summary>
        private void SendUserName()
        {
            _data = Encoding.Unicode.GetBytes(_userName);
            _stream.Write(_data, _offset, _data.Length);
            _responceMessage = ResponceMessage();
            Console.WriteLine($"Сервер: {_responceMessage}");
        }

        /// <summary>
        /// Подключение к серверу
        /// </summary>
        private void Сonnect()
        {
            while (true)
            {
                Console.WriteLine($"{_userName}: ");
                _message = Console.ReadLine();
                if(_message == "")
                {
                    Console.WriteLine("Введите команду!");
                    Сonnect();
                }
                _data = Encoding.Unicode.GetBytes(_message);
                _stream.Write(_data, _offset, _data.Length);
                _responceMessage = ResponceMessage();
                Console.WriteLine($"Сервер: {_responceMessage}");
                if (_message.ToLower() == "пока")
                {
                    _client.Close();
                    _stream.Close();
                    Environment.Exit(0);
                }
            }
        }

        /// <summary>
        /// Метод конфигурации. Вызывается первым при запуске метода Start
        /// </summary>
        private void Configuration()
        {
            Console.WriteLine("Введите подключаемый порт");
            var _portCheck = Console.ReadLine();
            if (isNumberPort(_portCheck))
            {
                _port = int.Parse(_portCheck);
            } // проверяем на правильность введения данных
            else
            {
                Configuration();
            }
            Console.WriteLine("Введите IP адрес. Например: 127.0.0.1");
            var _ipCheck = Console.ReadLine();
            if (isIPAdress(_ipCheck))
            {
                _ipAdress = _ipCheck;
            } // проверяем на правильность введения данных
            else
            {
                Configuration();
            }

        }

        /// <summary>
        /// Проверка на ввод числа
        /// </summary>
        /// <param name="line">Строка, где должно быть число</param>
        /// <returns></returns>
        private bool isNumberPort(string line)
        {
            try
            {
                if (line.Length > 4) // если длина порта больше 5-и символов то отправляем false
                {
                    return false;
                }
                var check = int.Parse(line);
                return true;
            }
            catch
            {
                return false;
            }

        }

        /// <summary>
        /// Проверка на правильность IP адреса
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private bool isIPAdress(string line)
        {
            var array = line.Split(new char[] { '.' });
            if (array.Length == 4)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    if (!isNumberPort(array[i]))
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Получить ответ от сервера
        /// </summary>
        /// <returns></returns>
        private string ResponceMessage()
        {
            _data = new byte[64];
            _builder.Clear();
            do
            {
                _bytes = _stream.Read(_data, _offset, _data.Length);
                _builder.Append(Encoding.Unicode.GetString(_data, _index, _bytes));

            } while (_stream.DataAvailable);
            return _builder.ToString();
        }
    }
}
