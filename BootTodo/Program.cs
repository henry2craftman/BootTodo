using System.Text;

namespace BootTodo
{
    // 클래스 이름을 더 명확하게 변경했습니다.
    internal class BootTodo
    {
        // GoogleDocsManager 객체를 한 번만 생성합니다.
        private static readonly GoogleDocsManager docsManager = new GoogleDocsManager();

        // enum의 시작 값을 명시적으로 1로 지정하는 것이 좋습니다.
        public enum Option
        {
            WriteDiary = 1,
            SearchDiary,
            Exit
        }

        // 파일 경로는 const 대신 readonly static으로 선언하여 유연성을 높일 수 있습니다.
        private static readonly string FilePath = "Diary.txt";

        // 추가: 단축키로 입력될 문자열을 상수로 정의
        private static readonly string DONE_MARK = "[o] ";
        private static readonly string NOT_DONE_MARK = "[x] ";

        static async Task Main()
        {
            // 프로그램 시작 시 파일이 없으면 생성할지 한 번만 묻습니다.
            EnsureFileExists();

            while (true) // 더 간단하고 명확한 무한 루프
            {
                Console.Clear();
                var choice = DisplayMenuAndGetChoice();

                switch (choice)
                {
                    case Option.WriteDiary:
                        await HandleWriteDiary();
                        break;

                    case Option.SearchDiary:
                        HandleSearchDiary();
                        break;

                    case Option.Exit:
                        Console.WriteLine("프로그램을 종료합니다.");
                        return; // return으로 while 루프와 Main 메서드를 완전히 종료

                    default:
                        Console.WriteLine("잘못된 선택입니다. 1, 2, 3 중에서 선택해주세요.");
                        break;
                }

                // 각 작업이 끝난 후 사용자가 내용을 확인할 시간을 줍니다.
                Console.WriteLine("\n아무 키나 눌러 메뉴로 돌아가세요.");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// 메뉴를 표시하고 사용자의 선택을 받아 Option enum으로 반환합니다.
        /// </summary>
        private static Option DisplayMenuAndGetChoice()
        {
            Console.WriteLine("옵션을 선택 후 Enter키를 눌러주세요.");
            Console.WriteLine("1. 다이어리 쓰기");
            Console.WriteLine("2. 다이어리 검색");
            Console.WriteLine("3. 종료");
            Console.Write("입력: ");
            string inputStr = Console.ReadLine();

            if (int.TryParse(inputStr, out int choice) && Enum.IsDefined(typeof(Option), choice))
            {
                return (Option)choice;
            }

            return 0; // 잘못된 입력을 나타내는 값 (default 케이스로 처리됨)
        }


        /// <summary>
        /// 일기 쓰기 로직을 처리합니다. 단축키 기능이 포함된 버전입니다.
        /// </summary>
        private static async Task HandleWriteDiary()
        {
            Console.Clear();
            string todayDate = DateTime.Now.ToString("yyyy.MM.dd");
            Console.WriteLine($"오늘 날짜: {todayDate}");

            try
            {
                if (File.ReadAllLines(FilePath).Any(line => line.StartsWith(todayDate)))
                {
                    Console.WriteLine("오늘 날짜의 일기는 이미 존재합니다. 수정은 텍스트 파일을 직접 열어주세요.");
                    return;
                }

                // 사용자에게 단축키 안내
                Console.WriteLine("일기 내용을 입력하세요. (작성을 끝내려면 'quit' 입력 후 엔터)");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("단축키: F1 = [o] , F2 = [x]");
                Console.ResetColor();

                var contentLines = new List<string>();

                while (true)
                {
                    // ReadLine() 대신 직접 만든 ReadLineWithHotkeys() 메서드를 호출
                    string input = ReadLineWithHotkeys();

                    if (input.Equals("quit", StringComparison.OrdinalIgnoreCase))
                    {
                        if (contentLines.Count == 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("오늘의 시작은 반드시 다이어리를 쓰고 시작해야 합니다. 무엇이든 적어주세요.");
                            Console.ResetColor();
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }

                    contentLines.Add(input);
                }

                using (StreamWriter sw = File.AppendText(FilePath))
                {
                    if (new FileInfo(FilePath).Length > 0) sw.WriteLine();
                    sw.WriteLine(todayDate);
                    foreach (string line in contentLines) sw.WriteLine(line);
                    sw.WriteLine("----");
                }
                Console.WriteLine("일기가 성공적으로 저장되었습니다.");

                // ★★★ Google Docs에 동기화하는 부분 ★★★
                // 로컬에 저장된 일기 내용을 하나의 문자열로 합칩니다.
                string diaryContentForGoogle = $"{todayDate}\n" +
                                               string.Join("\n", contentLines) +
                                               "\n----\n";

                // Google Docs에 전송합니다.
                await docsManager.AppendText(diaryContentForGoogle);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"오류 발생: {ex.Message}");
            }
        }

        /// <summary>
        /// 단축키를 포함하여 콘솔에서 한 줄을 읽는 새로운 메서드.
        /// </summary>
        /// <returns>사용자가 입력한 한 줄의 문자열</returns>
        private static string ReadLineWithHotkeys()
        {
            var currentLine = new StringBuilder();

            while (true)
            {
                // Console.ReadKey(true)는 키를 누르는 즉시 감지하고, 화면에는 자동으로 출력하지 않습니다.
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                switch (keyInfo.Key)
                {
                    // Enter 키: 한 줄 입력 완료
                    case ConsoleKey.Enter:
                        Console.WriteLine(); // 줄바꿈
                        return currentLine.ToString();

                    // F1 단축키: [o] 입력
                    case ConsoleKey.F1:
                        currentLine.Append(DONE_MARK);
                        Console.Write(DONE_MARK);
                        break;

                    // F2 단축키: [x] 입력
                    case ConsoleKey.F2:
                        currentLine.Append(NOT_DONE_MARK);
                        Console.Write(NOT_DONE_MARK);
                        break;

                    // Backspace 키: 마지막 글자 지우기
                    case ConsoleKey.Backspace:
                        if (currentLine.Length > 0)
                        {
                            currentLine.Remove(currentLine.Length - 1, 1);
                            // 화면에서 마지막 글자를 지우는 트릭
                            Console.Write("\b \b");
                        }
                        break;

                    // 그 외 모든 일반 키
                    default:
                        // 쉬프트, 컨트롤 같은 제어 키는 무시하고 일반 문자만 처리
                        if (!char.IsControl(keyInfo.KeyChar))
                        {
                            currentLine.Append(keyInfo.KeyChar);
                            Console.Write(keyInfo.KeyChar);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 일기 검색 로직을 처리합니다.
        /// </summary>
        private static void HandleSearchDiary()
        {
            Console.Clear();
            Console.WriteLine("검색할 날짜를 'yyyy.MM.dd' 형식으로 입력하세요.");
            Console.Write("날짜: ");
            string dateInput = Console.ReadLine();

            // 입력된 날짜 형식이 유효한지 간단히 확인
            if (!DateTime.TryParseExact(dateInput, "yyyy.MM.dd", null, System.Globalization.DateTimeStyles.None, out _))
            {
                Console.WriteLine("잘못된 날짜 형식입니다.");
                return;
            }

            try
            {
                var entryLines = new List<string>();
                bool isDateFound = false;

                // using으로 안전하게 파일을 엽니다.
                using (StreamReader sr = new StreamReader(FilePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (isDateFound)
                        {
                            // 다음 날짜가 나오거나, 구분선이 나오면 검색을 멈춥니다.
                            if (IsDateLine(line) || line.StartsWith("----"))
                            {
                                break;
                            }
                            entryLines.Add(line);
                        }
                        else if (line.StartsWith(dateInput))
                        {
                            isDateFound = true;
                            // 날짜 제목도 결과에 포함시킬 수 있습니다.
                            // entryLines.Add(line);
                        }
                    }
                }

                if (isDateFound)
                {
                    Console.WriteLine($"\n--- {dateInput}의 일기 ---");
                    Console.WriteLine(string.Join(Environment.NewLine, entryLines));
                    Console.WriteLine("----------------------");
                }
                else
                {
                    Console.WriteLine("해당 날짜의 일기를 찾을 수 없습니다.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"오류 발생: {ex.Message}");
            }
        }

        /// <summary>
        /// 문자열이 'yyyy.MM.dd' 형식의 날짜인지 확인하는 도우미 메서드
        /// </summary>
        private static bool IsDateLine(string line)
        {
            return DateTime.TryParseExact(line, "yyyy.MM.dd", null, System.Globalization.DateTimeStyles.None, out _);
        }

        /// <summary>
        /// 프로그램 시작 시 Diary.txt 파일이 존재하는지 확인하고, 없으면 생성 여부를 묻습니다.
        /// </summary>
        private static void EnsureFileExists()
        {
            if (!File.Exists(FilePath))
            {
                Console.WriteLine($"파일이 없습니다. '{FilePath}' 파일을 생성하시겠습니까? (Y/N)");
                string answer = Console.ReadLine();
                if (answer.Contains("Y", StringComparison.OrdinalIgnoreCase))
                {
                    File.Create(FilePath).Close(); // 파일을 만들고 즉시 닫아줍니다.
                    Console.WriteLine("파일이 생성되었습니다.");
                }
                else
                {
                    Console.WriteLine("프로그램을 종료합니다.");
                    Environment.Exit(0); // 파일 없이 진행할 수 없으므로 프로그램 종료
                }
            }
        }
    }
}