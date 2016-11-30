using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CsharpHttpServerSample
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                FileServer();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
        // 常に「HELLO」を返す HTTP サーバ
        static void SimpleServer(string[] args)
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:1000/");
            listener.Start();
            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                HttpListenerResponse res = context.Response;
                res.StatusCode = 200;
                byte[] content = Encoding.UTF8.GetBytes("HELLO");
                res.OutputStream.Write(content, 0, content.Length);
                res.Close();
            }
        }
        // ちゃんとファイル内容を返す HTTP サーバ
        static void FileServer()
        {
            // ドキュメントルート (プロジェクトディレクトリ内 wwwroot)
            // 例: "C:\Projects\CsharpHttpServerSample\wwwroot"
            string wwwroot = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\wwwroot");

            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:1000/");
            listener.Start();

            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest req = context.Request;
                HttpListenerResponse res = context.Response;

                // URL (ここには "/" とか "/index.html" 等が入ってくる)
                string urlPath = req.RawUrl;
                Console.WriteLine(urlPath);
                if (urlPath == "/") urlPath = "/index.html";

                // 実際のローカルファイルパス
                string path = wwwroot + urlPath.Replace("/", "\\");

                // ファイル内容を出力
                try
                {
                    res.StatusCode = 200;
                    byte[] content = File.ReadAllBytes(path);
                    res.OutputStream.Write(content, 0, content.Length);
                }
                catch (Exception ex)
                {
                    res.StatusCode = 500; // 404 でも良いのだがここは雑に 500 にまとめておく
                    byte[] content = Encoding.UTF8.GetBytes(ex.Message);
                    res.OutputStream.Write(content, 0, content.Length);
                }
                res.Close();
            }
        }
    }
}
/*
 * 
                // 特定URLに対する応答
                if (req.RawUrl.StartsWith("/receive"))
                {
                    res.StatusCode = 200;
                    res.ContentType = "application/json";
                    // 応答
                    byte[] content = Encoding.UTF8.GetBytes(@"{""hello"": 0, ""count"": 10}");
                    try
                    {
                        res.OutputStream.Write(content, 0, content.Length);
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("Error4: " + ex.Message);
                    }
                }
*/
