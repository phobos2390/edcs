using System;
using System.Text;
using NUnit.Framework;

namespace edcs.Tests
{
    class FakeStateMachine : edcs.IStateMachine
    {
        IState state;

        public FakeStateMachine()
        {
            state = new edcs.CommandState(new Model(),this);
        }

        public IState State 
        {
            get
            {
                return state;
            }
            set
            {
                state = value;
            }
        }

        public void Quit()
        {
            Console.WriteLine("Quitting");
        }

        public void Read()
        {
            Console.WriteLine("Reading");
        }

        public void Read(string filename)
        {
            Console.WriteLine("Reading " + filename);
        }

        public void Write()
        {
            Console.WriteLine("Writing");
        }

        public void Write(string filename)
        {
            Console.WriteLine("Writing " + filename);
        }
    }

    [TestFixture]
    public sealed class edcs_TESTS
    {
        [Test]
        public void edcs_model_Test()
        {
            Model model = new Model();
            model.AppendLine("Hello");
            model.InsertLine(model.LineCount,"World!");
            model.InsertLine(0,"It's me.");
            model.InsertLine(2,"You Wonderful");
            StringBuilder output = new StringBuilder();
            string[] expectedLines = {"It's me.","Hello","You Wonderful","World!"};
            int lc = 0;
            foreach(string line in model.List())
            {
                Assert.AreEqual(expectedLines[lc++],line);
                output.Append(line).Append("\n");
                System.Console.WriteLine(line);
            }
            Assert.AreEqual("It's me.\nHello\nYou Wonderful\nWorld!\n",output.ToString());
        }

        [Test]
        public void edcs_model_remove_line_Test()
        {
            Model model = new Model();
            model.AppendLine("It's me.");
            model.AppendLine("Hello");
            model.AppendLine("You Wonderful");
            model.AppendLine("World!");
            model.RemoveLine(1);
            StringBuilder output = new StringBuilder();
            string[] expectedLines = {"It's me.","You Wonderful","World!"};
            int lc = 0;
            foreach(string line in model.List())
            {
                Assert.AreEqual(expectedLines[lc++],line);
                output.Append(line).Append("\n");
                System.Console.WriteLine(line);
            }
            Assert.AreEqual("It's me.\nYou Wonderful\nWorld!\n",output.ToString());
        }

        [Test]
        public void edcs_state_machine_Test()
        {
            IStateMachine mock_machine = new FakeStateMachine();
            mock_machine.State.HandleLine("w");
            Assert.IsInstanceOf<edcs.CommandState>(mock_machine.State);
            mock_machine.State.HandleLine("w test.txt");
            Assert.IsInstanceOf<edcs.CommandState>(mock_machine.State);
            mock_machine.State.HandleLine("l");
            Assert.IsInstanceOf<edcs.CommandState>(mock_machine.State);
            mock_machine.State.HandleLine("a");
            Assert.IsInstanceOf<edcs.InsertState>(mock_machine.State);
            mock_machine.State.HandleLine("henlo world is me holualou");
            Assert.IsInstanceOf<edcs.InsertState>(mock_machine.State);
            mock_machine.State.HandleLine(".");
            Assert.IsInstanceOf<edcs.CommandState>(mock_machine.State);
            mock_machine.State.HandleLine("l");
            Assert.IsInstanceOf<edcs.CommandState>(mock_machine.State);
        }
    }
}
