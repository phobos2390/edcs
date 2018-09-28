using System;
using System.Collections.Generic;

namespace edcs
{
    public interface IModel
    {
         UInt64 LineCount {get;}
        void InsertLine(UInt64 lineNumber, string line);

        void AppendLine(string line);

        IEnumerable<string> List(string regex);
        
        IEnumerable<string> List();
       
        IEnumerable<string> List(UInt64 firstLine, UInt64 lastLine);

        void SearchAndReplace(string regex, string replace);
    }

    public class Model : IModel
    {
        LinkedList<string> lines;

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
            if((UInt64)lines.Count - lineNumber <= lineNumber)
            {
                UInt64 i = 0;
                LinkedListNode<string> iter = lines.Last;
                while(iter != lines.Last && ((UInt64) lines.Count - lineNumber) != ++i )
                {
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
                while(iter != lines.Last && lineNumber != ++i )
                {
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
            else if(lineNumber == (UInt64) lines.Count - 1)
            {
                AppendLine(line);
            }
            else if((UInt64)lines.Count - lineNumber <= lineNumber)
            {
                UInt64 i = 0;
                LinkedListNode<string> iter = lines.Last;
                while(iter != lines.Last && ((UInt64) lines.Count - lineNumber) != ++i )
                {
                    iter = iter.Previous;
                }
                if(((UInt64) lines.Count - lineNumber) == i)
                {
                    lines.AddAfter(iter, line);
                } 
            }
            else if((UInt64)lines.Count - lineNumber > lineNumber)
            {
                UInt64 i = 0;
                LinkedListNode<string> iter = lines.First;
                while(iter != lines.Last && lineNumber != ++i )
                {
                    iter = iter.Next;
                }
                if(lineNumber == i)
                {
                    lines.AddAfter(iter, line);
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
        }

        public void HandleLine(string line)
        {
            switch(line)
            {
                case "w":
                    break;
                case "a":
                    machine.State = new InsertState(model.LineCount, model, machine);
                    break;
                case "q":
                    machine.Quit();
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

        public void Quit()
        {
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
            System.IO.StreamReader fs = new System.IO.StreamReader(filename);
            try
            {
                while(!fs.EndOfStream)
                {
                    model.AppendLine(fs.ReadLine());
                }
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
            Console.WriteLine("Hello World");
        }
    }
}