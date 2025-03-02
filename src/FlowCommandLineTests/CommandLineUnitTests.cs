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

        public record RunCommand_NotSuccess_RequiredField_Class {
            public string Parameter1 { get; set; } = "";
        }

        [Fact]
        public void RunCommand_NotSuccess_RequiredField () {
            //arrange
            var messages = new List<string> ();
            var fakeProvider = A.Fake<ICommandLineProvider> ();
            A.CallTo ( () => fakeProvider.GetCommandLine () ).Returns ( "test" );
            A.CallTo ( () => fakeProvider.WriteLine ( A<string>._ ) ).Invokes ( ( string fake ) => { messages.Add ( fake ); } );
            var commandLine = new CommandLine ( fakeProvider );

            var isCorrect = true;

            //act
            commandLine
                .Application ( "TestApplication", "1.0.0" )
                .AddCommand (
                    "test",
                    ( RunCommand_NotSuccess_RequiredField_Class arg ) => {
                        isCorrect = false;
                    },
                    "",
                    new List<FlowCommandParameter> {
                        new FlowCommandParameter { FullName = "Parameter1", Description = "Description", Required = true }
                    }
                )
                .RunCommand ();

            //assert
            Assert.True ( isCorrect );
            Assert.Equal ( "Not all required parameters is defined!", messages.First () );
            Assert.Equal ( " ", messages.ElementAt ( 1 ) );
            Assert.Equal ( "The following parameters are available:", messages.ElementAt ( 2 ) );
            Assert.Equal ( "  --Parameter1 Description", messages.ElementAt ( 3 ) );
        }

        public record RunCommand_Success_QuotedParameters_Class {
            public string Parameter1 { get; set; } = "";
            public string Parameter2 { get; set; } = "";
        }

        [Fact]
        public void RunCommand_Success_QuotedParameters () {
            //arrange
            var messages = new List<string> ();
            var fakeProvider = A.Fake<ICommandLineProvider> ();
            A.CallTo ( () => fakeProvider.GetCommandLine () ).Returns ( "test --parameter1=\"Test string in quote\" --parameter2=\"Second string in quote\"" );
            A.CallTo ( () => fakeProvider.WriteLine ( A<string>._ ) ).Invokes ( ( string fake ) => { messages.Add ( fake ); } );
            var commandLine = new CommandLine ( fakeProvider );

            var isCorrect = true;

            //act
            commandLine
                .Application ( "TestApplication", "1.0.0" )
                .AddCommand (
                    "test",
                    ( RunCommand_Success_QuotedParameters_Class arg ) => {
                        isCorrect = arg.Parameter1 == "Test string in quote" && arg.Parameter2 == "Second string in quote";
                    },
                    "",
                    new List<FlowCommandParameter> {
                        new FlowCommandParameter { FullName = "Parameter1" },
                        new FlowCommandParameter { FullName = "Parameter2" }
                    }
                )
                .RunCommand ();

            //assert
            Assert.True ( isCorrect );
        }

        public record RunCommand_Success_DateOnlyTimeSpan_Class {
            public DateOnly Parameter1 { get; set; }
            public DateTime Parameter2 { get; set; }
            public TimeSpan Parameter3 { get; set; }
        }

        [Fact]
        public void RunCommand_Success_DateOnlyTimeSpan () {
            //arrange
            var messages = new List<string> ();
            var fakeProvider = A.Fake<ICommandLineProvider> ();
            A.CallTo ( () => fakeProvider.GetCommandLine () ).Returns ( "test --parameter1=2025-02-04 --parameter2=2025-02-04T12:35:20 --parameter3=18:10:20" );
            A.CallTo ( () => fakeProvider.WriteLine ( A<string>._ ) ).Invokes ( ( string fake ) => { messages.Add ( fake ); } );
            var commandLine = new CommandLine ( fakeProvider );

            var isCorrect = true;

            //act
            commandLine
                .Application ( "TestApplication", "1.0.0" )
                .AddCommand (
                    "test",
                    ( RunCommand_Success_DateOnlyTimeSpan_Class arg ) => {
                        isCorrect = arg.Parameter1 == new DateOnly ( 2025, 2, 4 ) &&
                            arg.Parameter2 == new DateTime ( 2025, 2, 4, 12, 35, 20 ) &&
                            arg.Parameter3 == new TimeSpan ( 18, 10, 20 );
                    },
                    "",
                    new List<FlowCommandParameter> {
                        new FlowCommandParameter { FullName = "Parameter1" },
                        new FlowCommandParameter { FullName = "Parameter2" },
                        new FlowCommandParameter { FullName = "Parameter3" }
                    }
                )
                .RunCommand ();

            //assert
            Assert.True ( isCorrect );
        }

        public record RunOptions_Success_SingleParameter_Class {
            public string Parameter1 { get; set; } = "";
        }

        [Fact]
        public void RunOptions_Success_SingleParameter () {
            //arrange
            var messages = new List<string> ();
            var fakeProvider = A.Fake<ICommandLineProvider> ();
            A.CallTo ( () => fakeProvider.GetCommandLine () ).Returns ( "test --parameter1=blablabla" );
            A.CallTo ( () => fakeProvider.WriteLine ( A<string>._ ) ).Invokes ( ( string fake ) => { messages.Add ( fake ); } );
            var commandLine = new CommandLine ( fakeProvider );

            //act
            var result = commandLine
                .Application ( "TestApplication", "1.0.0" )
                .AddOption(fullName: "Parameter1")
                .RunOptions<RunOptions_Success_SingleParameter_Class> ();

            //assert
            Assert.Equal ( "blablabla", result.Parameter1 );
        }

        public record RunOptions_Success_FewParameters_Class {
            public string Parameter1 { get; set; } = "";
            public string Parameter2 { get; set; } = "";
            public int Parameter3 { get; set; }
        }

        [Fact]
        public void RunOptions_Success_FewParameters () {
            //arrange
            var messages = new List<string> ();
            var fakeProvider = A.Fake<ICommandLineProvider> ();
            A.CallTo ( () => fakeProvider.GetCommandLine () ).Returns ( "test --parameter1=blablabla --parameter2=\"pirdesh\" --parameter3=144" );
            A.CallTo ( () => fakeProvider.WriteLine ( A<string>._ ) ).Invokes ( ( string fake ) => { messages.Add ( fake ); } );
            var commandLine = new CommandLine ( fakeProvider );

            //act
            var result = commandLine
                .Application ( "TestApplication", "1.0.0" )
                .AddOption ( fullName: "Parameter1" )
                .AddOption ( fullName: "Parameter2" )
                .AddOption ( fullName: "Parameter3" )
                .RunOptions<RunOptions_Success_FewParameters_Class> ();

            //assert
            Assert.Equal ( "blablabla", result.Parameter1 );
            Assert.Equal ( "pirdesh", result.Parameter2 );
            Assert.Equal ( 144, result.Parameter3 );
        }

    }

}
