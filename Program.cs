using System.Text;
public class MainApp
{
    static List<int> RandomizeLottery(Random random) //arpoo listan joka sisältää 7 uniikkia kokonaislukua väliltä 1-40
    {
        List<int> randomNumbers = new List<int>();
        int min = 1;
        int max = 40;
       
        for(int i = 0; i < 7; i++)
        {
            while (true) //Jos numero löytyy jo listasta arvotaan uusi niin pitkään kunnes se on uniikki
            {
                int randomNumber = random.Next(min, max + 1); //arpoo satunnaisluvun väliltä 1-40

                if (!randomNumbers.Contains(randomNumber))
                {
                    randomNumbers.Add(randomNumber);
                    break;
                }
            }   
        }
        return randomNumbers;
    }

  
    static bool ValidateNumbers(List<int> numbers) 
    {
        List <int> uniqueNumbers = new List<int>();

        if (numbers.Count != 7) //Tarkista onko käyttäjä syöttänyt tasan 7 numeroa
        {
            Console.WriteLine("You must input 7 numbers");
            return false;
        }
        
        foreach(int number in numbers)
        {
            if (number > 40 || number < 1)
            {
                Console.WriteLine("Inputted numbers must be between 1-40");
                return false;
            }

            if (uniqueNumbers.Contains(number)) //Tarkista onko jokainen numero uniikki
            {
                Console.WriteLine("Inputted numbers must be unique");
                return false;
            }
            uniqueNumbers.Add(number);
        }
        return true; //palautetaan true jos kaikki validoinnit meni läpi
    }

    static List<int> AskNumbers() //Kysyy käyttäjältä numerot millä pelataan ja palauttaa ne listana
    {
        bool isValid = false;
        List<int> validatedList = new List<int>();

        while (!isValid) {
        Console.Write("Enter 7 unique numbers between 1 and 40, separated by commas: ");
        string userInput = Console.ReadLine() ?? string.Empty;
        bool isNumber = true;

        string[] parts = userInput.Split(',');

        List<int> userInputtedList = new List<int>();

        foreach(string part in parts)
        {
            if(int.TryParse(part, out int n))
            {
                userInputtedList.Add(n);
            }
            else
            {
                    Console.WriteLine("All inputs must be valid numbers");
                    isNumber = false; //Mikäli käyttäjä syöttää muuta, kuin numeroita asetetaan isnumber falseksi
            }
        }
            isValid = ValidateNumbers(userInputtedList);
            if (isValid && isNumber)
            {
                validatedList = userInputtedList;
                Console.WriteLine();
                Console.WriteLine($"Your chosen numbers: [{string.Join(", ", validatedList)}]");
            }
        }
       
        return validatedList;
    }

    static void PrintEndScreen(int totalRounds, int totalSpent, int totalWinnings, int result) 
    {
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine("=== Final Summary ===");
        Console.WriteLine($"Total rounds: {totalRounds}");
        Console.WriteLine($"Total spent: {totalSpent},00 €");
        Console.WriteLine($"Total winnings: {totalWinnings},00 €");
        Console.WriteLine($"Net result: {result},00 €");
        Console.WriteLine("Thanks for playing!");
    }

    static void PrintAfterRound(int totalRounds, int totalSpent, int totalWinnings) { //Tulostus kierrosten välissä
        Console.Write($"\rRounds: {totalRounds} | Spent: {totalSpent},00 € | Won: {totalWinnings},00 €");
    }
    static void GameLogic(List<int> playerNumbers, int selectedMode, int roundNumber)
    {
        int roundPrice = 1; //hinta, jonka 1 kierros maksaa
        int matchesPerGame;
        int totalWinnings = 0;
        int totalSpent = 0;
        int profit = 0;
        int roundsPlayed = 0;
        Random random = new(); //Random numero generaattori, jota käytetään arpoessa lottonumeroita


        void PlayOneRound()
        {
            roundsPlayed++;
            totalSpent += roundPrice; 
            matchesPerGame = CompareNumbers(playerNumbers, RandomizeLottery(random)); //Laskee montako oikein pelaaja sai
            totalWinnings += GetPayout(matchesPerGame);
            PrintAfterRound(roundsPlayed, totalSpent, totalWinnings);
        }

        if (selectedMode == 1) //Peli 1
        {
            for (int i = 0; i < roundNumber; i++)
            {
                PlayOneRound();
                Thread.Sleep(10); //"nukuttaa" threadin joka kierroksen välissä siksi, ettei peli kuormita prosessoria
            }
        }

        if (selectedMode == 2) //Peli 2
        {
            Console.WriteLine();
            Console.WriteLine("Auto-play running... Press 'SPACE' to stop.");
            Console.WriteLine();
            while (true)
            {
                PlayOneRound();

                if (Console.KeyAvailable) //Jos käyttäjä painaa jotain näppäimistössä, tämä mahdollistaa sen, että luuppi pyörii kunnes käyttäjä painaa jotain
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true); //lukee käyttäjän painaman näppäimen, mutta ei näytä sitä consolessa

                    if (keyInfo.Key == ConsoleKey.Spacebar) //Tarkistetaan onko se space
                    {
                        break;
                    }
                }
                Thread.Sleep(10); //"nukuttaa" threadin joka kierroksen välissä siksi, ettei peli kuormita prosessoria
            }
        }

        profit = totalWinnings - totalSpent; //Laskee paljonko käyttäjä on voitolla
        PrintEndScreen(roundsPlayed, totalSpent, totalWinnings, profit); //Pelin päättyessä tulostetaan loppunäkymä
    }


    static int GetPayout(int matches)
    {
        Dictionary<int, int> payouts = new Dictionary<int, int>(); //osumien määrä, raha voitettu

        payouts.Add(1, 0);
        payouts.Add(2, 0);
        payouts.Add(3, 1);
        payouts.Add(4, 10);
        payouts.Add(5, 100);
        payouts.Add(6, 10000);
        payouts.Add(7, 100000);

        if (payouts.ContainsKey(matches)) //turha if check? matches pitäisi aina osua 0-7 välille
        {
            return payouts[matches]; //palauttaa oikean rahasumman osumien mukaan
        }

        return 0;
    }

    static int CompareNumbers(List<int> playerNumbers, List<int> randomNumbers) //Vertaa käyttäjän numeroita, lottonumeroihin ja palauttaa osumien määrän
    {
        int amountOfMatches = 0;

        foreach(int number in playerNumbers)
        {
            if (randomNumbers.Contains(number))
            {
                amountOfMatches++;
            }
        }
        return amountOfMatches;
    }

   

    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8; // Tämä mahdollistaa euro merkkien tulostuksen konsoliin
        string userChoiceInput = "";
        int selectedMode = 0; //valittu gamemode (1 = kiinteä kierrosmäärä, 2 = loputon peli)
        List<int> playerNumbers = new List<int>();

        Console.WriteLine("Selected play mode:");
        Console.WriteLine("1) Fixed number of rounds (prints each round");
        Console.WriteLine("2) Continuous auto-play (live stats; press SPACE to stop)");


        while (true) //Kysyy niin pitkää gamemodea kunnes käyttäjän syöte on validi (1 tai 2)
        {
            Console.Write("Choice (1/2): ");
            userChoiceInput = Console.ReadLine() ?? string.Empty;
            int choice;
            bool isNumber = int.TryParse(userChoiceInput, out choice); //Kokeilee muuttaa käyttäjän syötteen kokonaisluvuksi, jos syöte on jotain muuta palauttaa false

            if (!isNumber) //Tarkistaa onko käyttäjän syöte ylipäätään numero
            {
                Console.WriteLine("Invalid Choice");
            }
            else if (choice != 1 && choice != 2) //Mikäli käyttäjän syöte on 1 tai 2, se hyväksytään
            {
                Console.WriteLine($"Invalid choice");
            }
            else
            {
                selectedMode = choice;
                playerNumbers = AskNumbers(); //Tallennetaan käyttäjän syöttämät numerot playerNumbers listaan
                break;                       
            }
        }

        if(selectedMode == 1) //Peli 1
        {
            Console.WriteLine();
            int roundNumber = 0;
            string userInput = "";
            while (true)
            {
                Console.Write("How many rounds to play? (1-1,000,000): ");
                userInput = Console.ReadLine() ?? string.Empty;

                bool isNumber = int.TryParse(userInput, out roundNumber);
                 
                if (!isNumber)
                {
                    Console.WriteLine("Input must be a number between 1-1,000,000");
                }
                else if (roundNumber > 0 && roundNumber < 1000001)
                {
                    break; //Käyttäjän syötteen ollessa numero ja oikealta väliltä se hyväksytään
                }
            }
            GameLogic(playerNumbers, selectedMode, roundNumber);
        } else if(selectedMode == 2) //Peli 2
        {
            GameLogic(playerNumbers, selectedMode, 0);
        }

    }
}