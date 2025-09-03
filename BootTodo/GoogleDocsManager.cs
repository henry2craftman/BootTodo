using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace BootTodo
{
    internal class GoogleDocsManager
    {
        // ★★★ 2단계에서 복사한 Apps Script 웹 앱 URL을 여기에 붙여넣으세요! ★★★
        private static readonly string AppScriptUrl = "Paste Google URL Here";

        // HttpClient는 한 번만 생성해서 재사용하는 것이 좋습니다.
        private static readonly HttpClient client = new HttpClient();

        public async Task AppendText(string textToAppend)
        {
            // Apps Script로 보낼 데이터를 준비합니다. 
            // 'text'는 Apps Script의 e.parameter.text 부분과 이름이 같아야 합니다.
            var values = new Dictionary<string, string>
        {
            { "text", textToAppend }
        };

            var content = new FormUrlEncodedContent(values);

            try
            {
                // 준비된 데이터를 POST 방식으로 Apps Script URL에 보냅니다.
                var response = await client.PostAsync(AppScriptUrl, content);

                // Apps Script로부터 받은 응답을 읽습니다.
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode && responseString.Equals("Success"))
                {
                    Console.WriteLine("Google Docs에 성공적으로 동기화되었습니다.");
                }
                else
                {
                    // 응답이 'Success'가 아니면 오류로 간주합니다.
                    throw new Exception(responseString);
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Google Docs 업데이트 중 오류 발생: {e.Message}");
                Console.ResetColor();
            }
        }
    }
}
