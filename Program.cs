using System.Text;
public class MainApp
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8; // Tämä mahdollistaa euro merkkien tulostuksen konsoliin
        int selectedMode = 0; //valittu gamemode (1 = kiinteä kierrosmäärä, 2 = loputon peli, 3 = lopeta pelaaminen 4 = statsit)
        int numberOfRounds = 0;
        List<int> playerNumbers = new();
        int totalRounds = 0;
        int moneySpent = 0;
        int totalWon = 0;
        int netResult = 0;
        string savePath = "Results.txt";
        string breakLine = "--------------------------------------"; //Erotus viiva tallennetujen pelejen välissä
        bool keepPlaying = true;

        while (keepPlaying)
        {
            IntroText();
            selectedMode = AskChoice("Choice (1/2/3/4): ", 4);

            switch (selectedMode)
            {
                case 1: //Peli 1
                    playerNumbers = AskLottoNumbers();
                    AskRoundNumber(out numberOfRounds);
                    PlayGame(playerNumbers, selectedMode, numberOfRounds, out totalRounds, out moneySpent, out totalWon, out netResult);
                    SaveResults(savePath, totalRounds, moneySpent, totalWon, netResult, playerNumbers, breakLine);
                    break;

                case 2: //Peli 2
                    playerNumbers = AskLottoNumbers();
                    PlayGame(playerNumbers, selectedMode, 0, out totalRounds, out moneySpent, out totalWon, out netResult);
                    SaveResults(savePath, totalRounds, moneySpent, totalWon, netResult, playerNumbers, breakLine);
                    break;

                case 3: //Lopeta pelaaminen
                    keepPlaying = false;
                    break;

                case 4: //Statsit
                    ShowStats(savePath, breakLine);
                    break;
            }
        }
    }

    /// <summary>
    ///Tallentaa pelatun kierroksen tiedot Results.txt tiedostoon
    /// </summary>
    /// <param name="totalRounds"></param>
    /// <param name="moneySpent"></param>
    /// <param name="totalWon"></param>
    /// <param name="netResult"></param>
    /// <param name="playerNumbers"></param>
    static void SaveResults(string path, int totalRounds, int moneySpent, int totalWon, int netResult, List<int> playerNumbers, string breakLine)
    {
        string gameStats = $"Player Numbers: [{string.Join(", ", playerNumbers)}]\nRounds played: {totalRounds}\nMoney spent: {moneySpent},00€\nMoney won: {totalWon},00€\nNet result: {netResult},00€";
        string gameNumber = $"Game: {CountGames(path, breakLine) + 1}";
        string currentTime = $"({DateTime.Now.ToString("dd.MM.yyyy, HH':'mm':'ss")})";

        string saveText = $"{gameNumber} {currentTime}\n{gameStats}\n{breakLine}";

        File.AppendAllText(path, saveText + Environment.NewLine);
    }

    /// <summary>
    /// Tulostaa kaikki tallennetut pelit
    /// </summary>
    /// <param name="path"></param>
    static void PrintResults(string path)
    {
        if (File.Exists(path))
        {
            Console.WriteLine("\nGames that have been played so far:\n");
            foreach (string line in File.ReadAllLines(path))
            {
                Console.WriteLine(line);
            }
        }
    }

    /// <summary>
    /// Tulostaa yksittäisen pelin numeron perusteella
    /// </summary>
    /// <param name="path"></param>
    /// <param name="index"></param>
    static void PrintResult(string path, int index)
    {
        bool correctGame = false;
        if (File.Exists(path))
        {
            Console.WriteLine();
            foreach (string line in File.ReadAllLines(path))
            {
                if (line == ($"Game: {index}")) //Olettaa, että pelit on merkitty muodossa "Game: 1"
                {
                    correctGame = true;
                }else if(line.Contains("Game")) //jos sana Game löytyy mutta indexi väärä
                {
                    correctGame = false;
                }

                if(correctGame == true)
                {
                    Console.WriteLine(line);
                }
            }
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Kysyy käyttäjältä minkä pelin hän haluaa nähdä
    /// </summary>
    /// <param name="path"></param>
    /// <param name="breakLine"></param>
    static void AskGameToPrint(string path, string breakLine)
    {
        int gameAmount = CountGames(path, breakLine);
        string choiceText = $"Choice (1-{gameAmount}): ";

        int choice = AskChoice(choiceText, gameAmount);
        PrintResult(path, choice);
    }

    /// <summary>
    /// Kysyy käyttäjältä valintaa, halutulla tekstillä ja parametreissa ilmoitetun määrän mukaan
    /// </summary>
    /// <param name="textToDisplay"></param>
    /// <param name="choicesAmount"></param>
    /// <returns></returns>
    static int AskChoice(string textToDisplay, int choicesAmount)
    {
        while (true)
        {
            Console.Write(textToDisplay);
            string userChoiceInput = Console.ReadLine() ?? string.Empty;
            int choice;
            bool isNumber = int.TryParse(userChoiceInput, out choice);

            if (!isNumber)
            {
                Console.WriteLine("Invalid Choice");
            }
            else if (choice <= choicesAmount && choice > 0)
            {
                return choice;
            }
            else
            {
                Console.WriteLine("Invalid Choice");
            }
        }
    }

    /// <summary>
    /// Näyttää käyttäjän valinnan mukaan, joko yhden pelin tai kaikki pelit
    /// </summary>
    /// <param name="path"></param>
    /// <param name="breakLine"></param>
    static void ShowStats(string path, string breakLine)
    {
        int gameCount = CountGames(path, breakLine);
        if (gameCount < 1)
        {
            Console.WriteLine("There are no saved games yet\n");
        }
        else
        {
            Console.WriteLine();
            Console.WriteLine($"Number of games saved: {gameCount}");
            Console.WriteLine("1) Show stats of a specific game");
            Console.WriteLine("2) Show stats of all games");

            int choice = AskChoice("Choice (1/2): ", 2);

            switch (choice)
            {
                case 1:
                    AskGameToPrint(path, breakLine);
                    break;

                case 2:
                    PrintResults(path);
                    break;
            }
        }
    }

    /// <summary>
    /// Laskee montako peliä on tallennettuna
    /// </summary>
    /// <param name="path"></param>
    /// <param name="breakLine"></param>
    /// <returns></returns>
    static int CountGames(string path, string breakLine)
    {
        int amountOfGames = 0;
        if (File.Exists(path))
        {
            foreach(string line in File.ReadAllLines(path))
            {
                if(line == breakLine)
                {
                    amountOfGames++;
                }
            }
            return amountOfGames;
        }
        return 0; //If no games
    }

    /// <summary>
    /// Kysyy käyttäjältä montako kierrosta pelataan
    /// </summary>
    /// <param name="userChoiceInput"></param>
    /// <param name="selectedMode"></param>
    /// <param name="playerNumbers"></param>
    
    private static void AskRoundNumber(out int numberOfRounds)
    {
        Console.WriteLine();
        numberOfRounds = 0;
        string userInput = "";
        while (true)
        {
            Console.Write("How many rounds to play? (1-1,000,000): ");
            userInput = Console.ReadLine() ?? string.Empty;

            bool isNumber = int.TryParse(userInput, out numberOfRounds);

            if (!isNumber)
            {
                Console.WriteLine("Input must be a number between 1-1,000,000");
            }
            else if (numberOfRounds > 0 && numberOfRounds < 1000001)
            {
                break; //Käyttäjän syötteen ollessa numero ja oikealta väliltä se hyväksytään
            }
        }
    }

    private static void IntroText()
    {
        Console.WriteLine("Selected play mode:");
        Console.WriteLine("1) Fixed number of rounds (prints each round)");
        Console.WriteLine("2) Continuous auto-play (live stats; press SPACE to stop)");
        Console.WriteLine("3) Exit");
        Console.WriteLine("4) Stats");
    }

    /// <summary>
    /// arpoo listan joka sisältää 7 uniikkia kokonaislukua väliltä 1-40
    /// </summary>
    /// <param name="random"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Validoi, että käyttäjän syöttämä lista on halutun lainen
    /// </summary>
    /// <param name="numbers"></param>
    /// <returns></returns>
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
        return true; 
    }

    /// <summary>
    /// Kysyy käyttäjältä numerot millä pelataan ja palauttaa ne listana
    /// </summary>
    /// <returns></returns>
    static List<int> AskLottoNumbers()
    {
        bool isValidNumbers = false;
        bool isNumber = true;
        List<int> lottoNumbers = new();
        List<int> defaultNumbers = new([1, 2, 3, 4, 5, 6, 7]);

        while (!isValidNumbers || !isNumber) 
        {
            Console.Write("Enter 7 unique numbers between 1 and 40, separated by commas: ");
            string userInput = Console.ReadLine() ?? string.Empty;
            lottoNumbers = [];

            if (userInput == string.Empty) //tyhjällä inputilla default numerot 1-7
            {
                lottoNumbers = defaultNumbers;
                Console.WriteLine();
                Console.WriteLine($"Your chosen numbers: [{string.Join(", ", lottoNumbers)}]");
                break;
            }

            string[] parts = userInput.Split(',');

            

            foreach(string part in parts)
            {
                if(int.TryParse(part, out int n))
                {
                    lottoNumbers.Add(n);
                    isNumber = true;
                }
                else
                {
                    Console.WriteLine("All inputs must be valid numbers");
                    isNumber = false; //Mikäli käyttäjä syöttää muuta, kuin numeroita asetetaan isnumber falseksi
                    break;
                }
            }

            if (!isNumber)
            {
                continue; //Aloittaa while loopin alusta mikäli syöte ei ole numero
            }

            isValidNumbers = ValidateNumbers(lottoNumbers);  //testaa onko lista validi ja palauttaa true tai false sen mukaan
            if (isValidNumbers)
            {
                Console.WriteLine();
                Console.WriteLine($"Your chosen numbers: [{string.Join(", ", lottoNumbers)}]");
            }
        }
        return lottoNumbers;
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
        Console.WriteLine();
    }


    static void PrintAfterRound(int totalRounds, int totalSpent, int totalWinnings)
    { 
        Console.Write($"\rRounds: {totalRounds} | Spent: {totalSpent},00 € | Won: {totalWinnings},00 €");
    }

   
    static void PlayGame(List<int> playerNumbers, int selectedMode, int roundNumber, out int totalRounds, out int moneySpent, out int totalWon, out int netResult)
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
            Console.WriteLine();
            for (int i = 0; i < roundNumber; i++)
            {
                PlayOneRound();
                Thread.Sleep(10); //"nukuttaa" threadin joka kierroksen välissä siksi, ettei peli kuormita prosessoria
            }
        }
        else if (selectedMode == 2) //Peli 2
        {
            PrintAutoPlayText();
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

        //Out valuet
        totalRounds = roundsPlayed;
        moneySpent = totalSpent;
        totalWon = totalWinnings;
        netResult = profit;

        PrintEndScreen(totalRounds, moneySpent, totalWon, netResult);
    }

    static void PrintAutoPlayText()
    {
        Console.WriteLine();
        Console.WriteLine("Auto-play running... Press 'SPACE' to stop.");
        Console.WriteLine();
    }

   /// <summary>
   /// Palauttaa osumien mukaan rahasumman
   /// </summary>
   /// <param name="matches"></param>
   /// <returns></returns>
    static int GetPayout(int matches)
    {
        switch (matches)
        {
            case 3:
                return 1;

            case 4:
                return 10;

            case 5:
                return 100;

            case 6:
                return 10000;

            case 7:
                return 1000000;

            default:
                return 0;
        }
    }
    /// <summary>
    /// Vertailee kahta listaa numeroita ja palauttaa osumien määrän, olettaa että listat ei sisällä duplikaatteja
    /// </summary>
    /// <param name="playerNumbers"></param>
    /// <param name="randomNumbers"></param>
    /// <returns></returns>
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
}