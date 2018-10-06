using System;
using System.Collections.Generic;

namespace edcs
{
    public interface IModel
    {
         UInt64 LineCount {get;}
        void InsertLine(UInt64 lineNumber, string line);

        void RemoveLine(UInt64 lineNumber);
        
        void AppendLine(string line);

        IEnumerable<string> List(string regex);
        
        IEnumerable<string> List();
       
        IEnumerable<string> List(UInt64 firstLine, UInt64 lastLine);

        void SearchAndReplace(string regex, string replace);
    }

    public class Model : IModel
    {
        LinkedList<string> lines;

        public Model()
        {
            lines = new LinkedList<string>();
        }

        public UInt64 LineCount
        {
            get
            {
                return (UInt64)lines.Count;
            }
        }

        LinkedListNode<string> GetLineAtNumber(UInt64 lineNumber)
        {
            LinkedListNode<string> returnLine = null;
            if(lineNumber == 0)
            {
                returnLine = lines.First;
            }
            else if(lineNumber == (UInt64) lines.Count - 1)
            {
                returnLine = lines.Last;
            }
            else if((UInt64)lines.Count - lineNumber <= lineNumber)
            {
                UInt64 i = 0;
                LinkedListNode<string> iter = lines.Last;
                while(iter != lines.First && ((UInt64) lines.Count - lineNumber) != i)
                {
                    ++i;
                    iter = iter.Previous;
                }
                if(((UInt64) lines.Count - lineNumber) == i)
                {
                    returnLine = iter;
                } 
            }
            else if((UInt64)lines.Count - lineNumber > lineNumber)
            {
                UInt64 i = 0;
                LinkedListNode<string> iter = lines.First;
                while(iter != lines.Last && lineNumber != i )
                {
                    ++i;
                    iter = iter.Next;
                }
                if(lineNumber == i)
                {
                    returnLine = iter;
                } 
            }
            return returnLine;
        }

        public void RemoveLine(UInt64 lineNumber)
        {
            LinkedListNode<string> lineToRemove = GetLineAtNumber(lineNumber);
            lines.Remove(lineToRemove);
        }

        public void Clear()
        {
            lines.Clear();
        }

        public void InsertLine(UInt64 lineNumber, string line)
        {
            if(lineNumber == 0)
            {
                lines.AddFirst(line);
            }
            else if(lineNumber == (UInt64) lines.Count)
            {
                AppendLine(line);
            }
            else
            {
                LinkedListNode<string> currentLine = GetLineAtNumber(lineNumber);
                if(currentLine != null)
                {
                    lines.AddBefore(currentLine, line);
                }
            }
        }

        public  void AppendLine(string line)
        {
            lines.AddLast(line);
        }

        public  IEnumerable<string> List(string regex){return lines;}
        
        public  IEnumerable<string> List(){return lines;}
        
        public  IEnumerable<string> List(UInt64 firstLine, UInt64 lastLine){return lines;}

        public  void SearchAndReplace(string regex, string replace){}
    }

    public interface IState
    {
        void HandleLine(string line);
    }

    public class InsertState : IState
    {
        UInt64 lineNumber;
        IModel model;
        IStateMachine machine;

        public InsertState(UInt64 lineNumber, IModel model, IStateMachine machine)
        {
            this.lineNumber = lineNumber;
            this.model = model;
            this.machine = machine;
        }

        public void HandleLine(string line)
        {
            switch(line)
            {
                case ".":
                    machine.State = new CommandState(model, machine);
                    break;
                default:
                    model.InsertLine(lineNumber++, line);
                    break;
            }
        }
    }
    public class CommandState : IState
    {
        IModel model;
        IStateMachine machine;

        public CommandState(IModel model, IStateMachine machine)
        {
            this.model = model;
            this.machine = machine;
        }

        public void HandleLine(string line)
        {
            UInt64 lineNumber = 0;
            switch(line.Split(' ')[0])
            {
                case "a":
                    machine.State = new InsertState(model.LineCount, model, machine);
                    Console.WriteLine("Appending starting at line {0}",model.LineCount);
                    break;
                case "i":
                    lineNumber = UInt64.Parse(line.Split(' ')[1]);
                    machine.State = new InsertState(lineNumber - 1,model,machine);
                    Console.WriteLine("Inserting starting at line {0}",lineNumber);
                    break;
                case "r":
                    lineNumber = UInt64.Parse(line.Split(' ')[1]);
                    model.RemoveLine(lineNumber - 1);
                    Console.WriteLine("Removing line {0}",lineNumber);
                    break;
                case "l":
                    bool listLineNumbers = false;
                    bool listEndOfLine = false;
                    if(line.Split(' ').Length > 1)
                    {
                       switch(line.Split(' ')[1])
                        {
                            case "eol":
                                listEndOfLine = true;
                                break;
                            case "ln":
                                listLineNumbers = true;
                                break;
                            case "v":
                                listLineNumbers = true;
                                listEndOfLine = true;
                                break;
                            default:
                                break;
                        }
                    }
                    UInt64 i = 0;
                    foreach(string s in model.List())
                    {
                        if(listLineNumbers)
                        {
                            Console.Write("{0}: ",++i);
                        }
                        Console.Write(s);
                        if(listEndOfLine)
                        {
                            Console.Write("$");
                        }
                        Console.WriteLine();
                    }
                    break;
                case "w":
                    if(line.Split(' ').Length == 1)
                    {
                        machine.Write();
                    }
                    else
                    {
                        machine.Write(line.Split(' ')[1]);
                    }
                    break;
                case "q":
                    machine.Quit();
                    break;
                case "wq":
                    machine.Write();
                    machine.Quit();
                    break;
                default:
                    if(line != "")
                    {
                        Console.WriteLine("Unknown Command: {0}", line);
                    }
                    break;
            }
        }
    }

    public interface IStateMachine
    {
        void Quit();

        void Write();

        void Write(string filename);

        void Read();

        void Read(string filename);

        IState State
        {
            get;
            set;
        }
    }

    public class Controller: IStateMachine
    {
        private IState currentState;
        private bool running;
        IModel model;
        string currentFile;

        public Controller()
        {
            model = new Model();
            currentState = new CommandState(model,this);
        }

        public void Quit()
        {
            Console.WriteLine("Thank you for choosing Ed C#");
            running = false;
        }

        public void Write()
        {
            Write(currentFile);
        }
        
        public void Write(string filename)
        {
            currentFile = filename;
            System.IO.StreamWriter fs = new System.IO.StreamWriter(filename);
            try
            {
                foreach(string line in model.List())
               {
                    fs.WriteLine(line);
                }
                Console.WriteLine("Wrote {0} lines to file {1}", model.LineCount, filename);
            }
            catch (Exception e)
            {
                Console.Write("Exception thrown: " + e);
            }
            finally
            {
                fs.Close();
            }
        }

        public void Read()
        {
            Read(currentFile);
        }

        public void Read(string filename)
        {
            currentFile = filename;
            System.IO.StreamReader fs = null;
            try
            {
                fs = new System.IO.StreamReader(filename);
                while(!fs.EndOfStream)
                {
                    model.AppendLine(fs.ReadLine());
                }
            }
            catch (System.IO.FileNotFoundException)
            {
                Console.WriteLine("New File: " + filename);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception thrown: " + e);
            }
            finally
            {
                if(fs != null)
                {
                    fs.Close();
                }
            }
        }
        public IState State
        {
            get 
            {
                return currentState;
            }

            set 
            {
                currentState = value;
            }
        }

        public void Run()
        {
            running = true;
            while(running)
            {
                string newLine = Console.ReadLine();
                currentState.HandleLine(newLine);
            }
        }    
    }
    
    public class edcsMAIN
    {
        public static void Main(string[] args)
        {
            Controller controller = new Controller();
            if(args.Length == 1)
            {
                controller.Read(args[0]);
                controller.Run();
            }
        }
    }
}
