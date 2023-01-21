using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_club_console_application
{
    class Program
    {
        static void Main(string[] args)
        {
            GameClub gameClub = new GameClub(8);
            gameClub.Work();
        }
    }

    class GameClub
    {
        private int _money = 0;

        private List<SlotMachine> _slotMachine = new List<SlotMachine>();
        private Queue<Client> _clients = new Queue<Client>();

        public GameClub(int slotMachineCount)
        {
            Random random = new Random();

            for(int i = 0; i < slotMachineCount; i++)
            {
                _slotMachine.Add(new SlotMachine(random.Next(5, 20)));
            }

            CreateNewClient(25, random);
        }

        public void CreateNewClient(int count, Random random)
        {
            for (int i = 0; i < count; i++)
            {
                _clients.Enqueue(new Client(random.Next(100, 300), random));
            }
        }

        public void Work()
        {
            while(_clients.Count > 0)
            {
                Client newClient = _clients.Dequeue();
                Console.WriteLine($"Баланс клуба {_money} рублей. Ждем нового клиента!");
                Console.WriteLine($"У вас новый клиент, и он хочет купить {newClient.DesiredMinutes} минут.");
                ShowAllSlotMashineState();

                Console.Write("\nВы предлагаете ему игровой автомат под номером: ");
                string userInput = Console.ReadLine();

                if(int.TryParse(userInput, out int sloteMashineNumber))
                {
                    sloteMashineNumber -= 1;

                    if(sloteMashineNumber >= 0 && sloteMashineNumber < _slotMachine.Count)
                    {
                        if(_slotMachine[sloteMashineNumber].IsTaken)
                        {
                            Console.WriteLine("Вы пытаетесь посадить клиента за игровой автомат, который занят. Клиент не хочет ждать и ушел!");
                        }
                        else
                        {
                            if(newClient.CheckSolvency(_slotMachine[sloteMashineNumber]))
                            {
                                Console.WriteLine("Клиент оплатил время и пошел играть за игровой автомат " + (sloteMashineNumber + 1) + ".");
                                _money += newClient.Pay();
                                _slotMachine[sloteMashineNumber].BecomeTaken(newClient);
                            }
                            else
                            {
                                Console.WriteLine("У клиента не хватило денег, пошел копить дальше! :(");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Вы не смогли посадить клиента за игровой автомат. Он ушел!");
                    }
                }
                else
                {
                    CreateNewClient(1, new Random());
                    Console.WriteLine("Неверный ввод! Повторите снова.");
                }

                Console.WriteLine("Чтобы перейти к следующему клиенту, нажмите любую клавишу.");

                Console.ReadKey();
                Console.Clear();
                SpendOneMinute();
            }
        }

        private void ShowAllSlotMashineState()
        {
            Console.WriteLine("\nСписок всех игровых автоматов: ");
            for (int i = 0; i < _slotMachine.Count; i++)
            {
                Console.Write(i + 1 + " - ");
                _slotMachine[i].ShowState();
            }
        }

        private void SpendOneMinute()
        {
            foreach (var sloteMashine in _slotMachine)
            {
                sloteMashine.SpendOneMinute();
            }
        }
    }

    class SlotMachine
    {
        private Client _client;
        private int _minutesRemaining;

        public bool IsTaken
        {
            get
            {
                return _minutesRemaining > 0;
            }
        }

        public int PricePerMinute { get; private set; }

        public SlotMachine(int pricePerMinute)
        {
            PricePerMinute = pricePerMinute;
        }

        public void BecomeTaken(Client client)
        {
            _client = client;
            _minutesRemaining = _client.DesiredMinutes;
        }

        public void SpendOneMinute()
        {
            _minutesRemaining--;
        }

        public void ShowState()
        {
            if(IsTaken)
                Console.WriteLine($"Игровой автомат занят, осталось минут: {_minutesRemaining}.");
            else
                Console.WriteLine($"Игровой автомат свободен, цена за минуту: {PricePerMinute}.");
        }
    }

    class Client
    {
        private int _money;
        private int _moneyToPay;

        public int DesiredMinutes { get; private set; }
        
        public Client (int money, Random random)
        {
            _money = money;
            DesiredMinutes = random.Next(10, 30);
        }

        public bool CheckSolvency(SlotMachine sloteMachine)
        {
            _moneyToPay = DesiredMinutes * sloteMachine.PricePerMinute;

            if (_money >= _moneyToPay)
                return true;
            else
            {
                _moneyToPay = 0;
                return false;
            }
        }

        public int Pay()
        {
            _money -= _moneyToPay;
            return _moneyToPay;
        }
    }
}
