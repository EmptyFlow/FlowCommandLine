using FakeItEasy;
using FlowCommandLine;

namespace FlowCommandLineTests {

    public class CommandLineUnitTests {

        [Fact]
        public void RunCommand_Success_Empty_ShowHelp () {
            //arrange
            var messages = new List<string> ();
            var fakeProvider = A.Fake<ICommandLineProvider> ();
            A.CallTo ( () => fakeProvider.GetCommandLine () ).Returns ( "" );
            A.CallTo ( () => fakeProvider.WriteLine ( A<string>._ ) ).Invokes ( ( string fake ) => { messages.Add ( fake ); } );
            var commandLine = new CommandLine ( fakeProvider );

            //act
            commandLine
                .Application ( "TestApplication", "1.0.0" )
                .RunCommand ();

            //assert
            Assert.NotEmpty ( messages );
            Assert.Equal ( 2, messages.Count );
            Assert.Equal ( "TestApplication version 1.0.0", messages.First () );
            Assert.Equal ( " ", messages.ElementAt ( 1 ) );
        }

        [Fact]
        public void RunCommand_Success_OnlyDescription_ShowHelp () {
            //arrange
            var messages = new List<string> ();
            var fakeProvider = A.Fake<ICommandLineProvider> ();
            A.CallTo ( () => fakeProvider.GetCommandLine () ).Returns ( "" );
            A.CallTo ( () => fakeProvider.WriteLine ( A<string>._ ) ).Invokes ( ( string fake ) => { messages.Add ( fake ); } );
            var commandLine = new CommandLine ( fakeProvider );

            //act
            commandLine
                .Application ( "TestApplication", "1.0.0", "Application description!!!!" )
                .RunCommand ();

            //assert
            Assert.NotEmpty ( messages );
            Assert.Equal ( 4, messages.Count );
            Assert.Equal ( "TestApplication version 1.0.0", messages.First () );
            Assert.Equal ( " ", messages.ElementAt ( 1 ) );
            Assert.Equal ( "Application description!!!!", messages.ElementAt ( 2 ) );
            Assert.Equal ( " ", messages.ElementAt ( 3 ) );
        }

        [Fact]
        public void RunCommand_Success_OnlyExecutable_ShowHelp () {
            //arrange
            var messages = new List<string> ();
            var fakeProvider = A.Fake<ICommandLineProvider> ();
            A.CallTo ( () => fakeProvider.GetCommandLine () ).Returns ( "" );
            A.CallTo ( () => fakeProvider.WriteLine ( A<string>._ ) ).Invokes ( ( string fake ) => { messages.Add ( fake ); } );
            var commandLine = new CommandLine ( fakeProvider );

            //act
            commandLine
                .Application ( "TestApplication", "1.0.0", applicationExecutable: "testapp" )
                .RunCommand ();

            //assert
            Assert.NotEmpty ( messages );
            Assert.Equal ( 4, messages.Count );
            Assert.Equal ( "TestApplication version 1.0.0", messages.First () );
            Assert.Equal ( " ", messages.ElementAt ( 1 ) );
            Assert.Equal ( "usage: testapp [<command>] [<parameters>]", messages.ElementAt ( 2 ) );
            Assert.Equal ( " ", messages.ElementAt ( 3 ) );
        }

        public record RunCommand_Success_OnlyCommands_ShowHelp_Class { }

        [Fact]
        public void RunCommand_Success_OnlyCommands_ShowHelp () {
            //arrange
            var messages = new List<string> ();
            var fakeProvider = A.Fake<ICommandLineProvider> ();
            A.CallTo ( () => fakeProvider.GetCommandLine () ).Returns ( "" );
            A.CallTo ( () => fakeProvider.WriteLine ( A<string>._ ) ).Invokes ( ( string fake ) => { messages.Add ( fake ); } );
            var commandLine = new CommandLine ( fakeProvider );

            //act
            commandLine
                .Application ( "TestApplication", "1.0.0" )
                .AddCommand ( "test", ( RunCommand_Success_OnlyCommands_ShowHelp_Class arg ) => { }, "description", Enumerable.Empty<FlowCommandParameter> () )
                .AddCommand ( "test-borod", ( RunCommand_Success_OnlyCommands_ShowHelp_Class arg ) => { }, "description1", Enumerable.Empty<FlowCommandParameter> () )
                .AddCommand ( "longcommandname", ( RunCommand_Success_OnlyCommands_ShowHelp_Class arg ) => { }, "description2", Enumerable.Empty<FlowCommandParameter> () )
                .RunCommand ();

            //assert
            Assert.NotEmpty ( messages );
            Assert.Equal ( 6, messages.Count );
            Assert.Equal ( "TestApplication version 1.0.0", messages.First () );
            Assert.Equal ( " ", messages.ElementAt ( 1 ) );
            Assert.Equal ( "The following commands are available:", messages.ElementAt ( 2 ) );
            Assert.Equal ( "  test            description", messages.ElementAt ( 3 ) );
            Assert.Equal ( "  test-borod      description1", messages.ElementAt ( 4 ) );
            Assert.Equal ( "  longcommandname description2", messages.ElementAt ( 5 ) );
        }

        public record RunCommand_Success_OnlyCommands_SingleCommand_Class {
            public string Parameter1 { get; set; } = "";
            public string Parameter2 { get; set; } = "";
            public string Parameter3 { get; set; } = "";
        }

        [Fact]
        public void RunCommand_Success_OnlyCommands_SingleCommand () {
            //arrange
            var messages = new List<string> ();
            var fakeProvider = A.Fake<ICommandLineProvider> ();
            A.CallTo ( () => fakeProvider.GetCommandLine () ).Returns ( "test -parameter1=param1value -parameter2=param2value -parameter3=param3value" );
            var commandLine = new CommandLine ( fakeProvider );

            var isCorrect = false;

            //act
            commandLine
                .Application ( "TestApplication", "1.0.0" )
                .AddCommand (
                    "test",
                    ( RunCommand_Success_OnlyCommands_SingleCommand_Class arg ) => {
                        isCorrect = arg.Parameter1 == "param1value" &&
                            arg.Parameter2 == "param2value" &&
                            arg.Parameter3 == "param3value";
                    },
                    "",
                    new List<FlowCommandParameter> {
                        new FlowCommandParameter { FullName = "Parameter1" },
                        new FlowCommandParameter { FullName = "Parameter2" },
                        new FlowCommandParameter { FullName = "Parameter3" },
                    }
                )
                .RunCommand ();

            //assert
            Assert.True ( isCorrect );
        }

        public record RunCommand_Success_IntLongDoubleFloatTypes_Class {
            public int Parameter1 { get; set; }
            public long Parameter2 { get; set; }
            public double Parameter3 { get; set; }
            public float Parameter4 { get; set; }
        }

        [Fact]
        public void RunCommand_Success_IntLongDoubleFloatTypes () {
            //arrange
            var messages = new List<string> ();
            var fakeProvider = A.Fake<ICommandLineProvider> ();
            A.CallTo ( () => fakeProvider.GetCommandLine () ).Returns ( "test -parameter1=1200 -parameter2=5000000000 -parameter3=5.20 -parameter4=0.20" );
            var commandLine = new CommandLine ( fakeProvider );

            var isCorrect = false;

            //act
            commandLine
                .Application ( "TestApplication", "1.0.0" )
                .AddCommand (
                    "test",
                    ( RunCommand_Success_IntLongDoubleFloatTypes_Class arg ) => {
                        isCorrect = arg.Parameter1 == 1200 &&
                            arg.Parameter2 == 5000000000 &&
                            arg.Parameter3 == 5.20d &&
                            arg.Parameter4 == 0.20f;
                    },
                    "",
                    new List<FlowCommandParameter> {
                        new FlowCommandParameter { FullName = "Parameter1" },
                        new FlowCommandParameter { FullName = "Parameter2" },
                        new FlowCommandParameter { FullName = "Parameter3" },
                        new FlowCommandParameter { FullName = "Parameter4" },
                    }
                )
                .RunCommand ();

            //assert
            Assert.True ( isCorrect );
        }

    }

}
