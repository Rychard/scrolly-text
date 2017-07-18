using System;
using System.IO;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        if(args.Length < 3)
        {
            Console.WriteLine("Usage: scrolly-text <source-directory> <filter> <line-time>");
            return;
        }

        String sourceDirectory = args[0];
        String filter = args[1];
        String lineTimeRaw = args[2];
        Int32 lineTime;
        Boolean success = Int32.TryParse(lineTimeRaw, out lineTime);
        if(!success) { lineTime = 100; }

        Boolean running = true;
        while(running)
        {
            var files = Directory.EnumerateFiles(sourceDirectory, filter, SearchOption.AllDirectories);
            foreach (var file in files)
            {
                using(var stream = File.OpenRead(file))
                using(var sr = new StreamReader(stream))
                {
                    Int32 lineNumber = 0;

                    while(!sr.EndOfStream)
                    {
                        String line = sr.ReadLine();

                        if(Console.IsOutputRedirected)
                        {
                            Console.WriteLine(line);
                        }
                        else
                        {
                            lineNumber++;
                            Console.CursorVisible = false;

                            // These must be in the inner-most loop to avoid issues when the console is resized.
                            Int32 bufferWidth = Console.IsOutputRedirected ? 80 : Console.BufferWidth;
                            String blankLine = new String(' ', bufferWidth);

                            String relativeFilename = $"{file.Substring(sourceDirectory.Length)}:{lineNumber}";
                            Console.WriteLine(blankLine);

                            Int32 left = (Console.WindowWidth - relativeFilename.Length);
                            Int32 top = Console.CursorTop - 1;
                            Console.SetCursorPosition(left, top);
                            Console.Write(relativeFilename);

                            Int32 availableSpace = (Console.BufferWidth - line.Length);
                            availableSpace = Math.Max(availableSpace, 1);

                            String a = $"{line}{new String(' ', availableSpace)}";
                            Console.SetCursorPosition(0, top - 1);
                            Console.Write(a);
                        }

                        Thread.Sleep(lineTime);
                    }
                }
            }
        }
    }
}
