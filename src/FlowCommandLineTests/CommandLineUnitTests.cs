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
                .AddOption ( fullName: "Parameter1" )
                .RunOptions<RunOptions_Success_SingleParameter_Class> ();

            //assert
            Assert.Equal ( "blablabla", result?.Parameter1 ?? null );
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
            Assert.Equal ( "blablabla", result?.Parameter1 ?? null );
            Assert.Equal ( "pirdesh", result?.Parameter2 ?? null );
            Assert.Equal ( 144, result?.Parameter3 ?? null );
        }

        public record RunOptions_Success_IntParameter_Class {
            public int Parameter1 { get; set; }
            public int Parameter2 { get; set; }
            public int Parameter3 { get; set; }
        }

        [Fact]
        public void RunOptions_Success_IntParameter () {
            //arrange
            var messages = new List<string> ();
            var fakeProvider = A.Fake<ICommandLineProvider> ();
            A.CallTo ( () => fakeProvider.GetCommandLine () ).Returns ( "--parameter1=1 --parameter2=100000 --parameter3=1212121212" );
            A.CallTo ( () => fakeProvider.WriteLine ( A<string>._ ) ).Invokes ( ( string fake ) => { messages.Add ( fake ); } );
            var commandLine = new CommandLine ( fakeProvider );

            //act
            var result = commandLine
                .Application ( "TestApplication", "1.0.0" )
                .AddOption ( fullName: "Parameter1" )
                .AddOption ( fullName: "Parameter2" )
                .AddOption ( fullName: "Parameter3" )
                .RunOptions<RunOptions_Success_IntParameter_Class> ();

            //assert
            Assert.NotNull ( result );
            Assert.Equal ( 1, result.Parameter1 );
            Assert.Equal ( 100000, result.Parameter2 );
            Assert.Equal ( 1212121212, result.Parameter3 );
        }

        public record RunOptions_Success_IEnumerableIntParameter_Class {
            public IEnumerable<int> Parameter1 { get; set; } = Enumerable.Empty<int> ();
            public IEnumerable<int> Parameter2 { get; set; } = Enumerable.Empty<int> ();
            public IEnumerable<int> Parameter3 { get; set; } = Enumerable.Empty<int> ();
        }

        [Fact]
        public void RunOptions_Success_IEnumerableIntParameter () {
            //arrange
            var messages = new List<string> ();
            var fakeProvider = A.Fake<ICommandLineProvider> ();
            A.CallTo ( () => fakeProvider.GetCommandLine () ).Returns ( "--parameter1=1,2,3,4 --parameter2=8124343,2374234,123412,23423423,8124343,2374234,123412,23423423 --parameter3=100,100,100,100,100,100" );
            A.CallTo ( () => fakeProvider.WriteLine ( A<string>._ ) ).Invokes ( ( string fake ) => { messages.Add ( fake ); } );
            var commandLine = new CommandLine ( fakeProvider );

            //act
            var result = commandLine
                .Application ( "TestApplication", "1.0.0" )
                .AddOption ( fullName: "Parameter1" )
                .AddOption ( fullName: "Parameter2" )
                .AddOption ( fullName: "Parameter3" )
                .RunOptions<RunOptions_Success_IEnumerableIntParameter_Class> ();

            //assert
            Assert.NotNull ( result );
            Assert.Equal ( new List<int> { 1, 2, 3, 4 }, result.Parameter1 );
            Assert.Equal ( new List<int> { 8124343, 2374234, 123412, 23423423, 8124343, 2374234, 123412, 23423423 }, result.Parameter2 );
            Assert.Equal ( new List<int> { 100, 100, 100, 100, 100, 100 }, result.Parameter3 );
        }

        public record RunOptions_Success_ListIntParameter_Class {
            public List<int> Parameter1 { get; set; } = Enumerable.Empty<int> ().ToList();
            public List<int> Parameter2 { get; set; } = Enumerable.Empty<int> ().ToList ();
            public List<int> Parameter3 { get; set; } = Enumerable.Empty<int> ().ToList ();
        }

        [Fact]
        public void RunOptions_Success_ListIntParameter () {
            //arrange
            var messages = new List<string> ();
            var fakeProvider = A.Fake<ICommandLineProvider> ();
            A.CallTo ( () => fakeProvider.GetCommandLine () ).Returns ( "--parameter1=1,2,3,4 --parameter2=8124343,2374234,123412,23423423,8124343,2374234,123412,23423423 --parameter3=100,100,100,100,100,100" );
            A.CallTo ( () => fakeProvider.WriteLine ( A<string>._ ) ).Invokes ( ( string fake ) => { messages.Add ( fake ); } );
            var commandLine = new CommandLine ( fakeProvider );

            //act
            var result = commandLine
                .Application ( "TestApplication", "1.0.0" )
                .AddOption ( fullName: "Parameter1" )
                .AddOption ( fullName: "Parameter2" )
                .AddOption ( fullName: "Parameter3" )
                .RunOptions<RunOptions_Success_ListIntParameter_Class> ();

            //assert
            Assert.NotNull ( result );
            Assert.Equal ( new List<int> { 1, 2, 3, 4 }, result.Parameter1 );
            Assert.Equal ( new List<int> { 8124343, 2374234, 123412, 23423423, 8124343, 2374234, 123412, 23423423 }, result.Parameter2 );
            Assert.Equal ( new List<int> { 100, 100, 100, 100, 100, 100 }, result.Parameter3 );
        }

        public record RunOptions_Success_LongParameter_Class {
            public long Parameter1 { get; set; }
            public long Parameter2 { get; set; }
            public long Parameter3 { get; set; }
        }

        [Fact]
        public void RunOptions_Success_LongParameter () {
            //arrange
            var messages = new List<string> ();
            var fakeProvider = A.Fake<ICommandLineProvider> ();
            A.CallTo ( () => fakeProvider.GetCommandLine () ).Returns ( "--parameter1=1 --parameter2=10000000000 --parameter3=121212345231212" );
            A.CallTo ( () => fakeProvider.WriteLine ( A<string>._ ) ).Invokes ( ( string fake ) => { messages.Add ( fake ); } );
            var commandLine = new CommandLine ( fakeProvider );

            //act
            var result = commandLine
                .Application ( "TestApplication", "1.0.0" )
                .AddOption ( fullName: "Parameter1" )
                .AddOption ( fullName: "Parameter2" )
                .AddOption ( fullName: "Parameter3" )
                .RunOptions<RunOptions_Success_LongParameter_Class> ();

            //assert
            Assert.NotNull ( result );
            Assert.Equal ( 1, result.Parameter1 );
            Assert.Equal ( 10000000000, result.Parameter2 );
            Assert.Equal ( 121212345231212, result.Parameter3 );
        }

        public record RunOptions_Success_IEnumerableLongParameter_Class {
            public IEnumerable<long> Parameter1 { get; set; } = Enumerable.Empty<long> ();
            public IEnumerable<long> Parameter2 { get; set; } = Enumerable.Empty<long> ();
            public IEnumerable<long> Parameter3 { get; set; } = Enumerable.Empty<long> ();
        }

        [Fact]
        public void RunOptions_Success_IEnumerableLongParameter () {
            //arrange
            var messages = new List<string> ();
            var fakeProvider = A.Fake<ICommandLineProvider> ();
            A.CallTo ( () => fakeProvider.GetCommandLine () ).Returns ( "--parameter1=1,2,3,4 --parameter2=8124343,2374234,123412,23423423,8124343,2374234,123412,23423423 --parameter3=100,100,100,100,100,100" );
            A.CallTo ( () => fakeProvider.WriteLine ( A<string>._ ) ).Invokes ( ( string fake ) => { messages.Add ( fake ); } );
            var commandLine = new CommandLine ( fakeProvider );

            //act
            var result = commandLine
                .Application ( "TestApplication", "1.0.0" )
                .AddOption ( fullName: "Parameter1" )
                .AddOption ( fullName: "Parameter2" )
                .AddOption ( fullName: "Parameter3" )
                .RunOptions<RunOptions_Success_IEnumerableLongParameter_Class> ();

            //assert
            Assert.NotNull ( result );
            Assert.Equal ( new List<long> { 1, 2, 3, 4 }, result.Parameter1 );
            Assert.Equal ( new List<long> { 8124343, 2374234, 123412, 23423423, 8124343, 2374234, 123412, 23423423 }, result.Parameter2 );
            Assert.Equal ( new List<long> { 100, 100, 100, 100, 100, 100 }, result.Parameter3 );
        }

        public record RunOptions_Success_ListLongParameter_Class {
            public List<long> Parameter1 { get; set; } = Enumerable.Empty<long> ().ToList ();
            public List<long> Parameter2 { get; set; } = Enumerable.Empty<long> ().ToList ();
            public List<long> Parameter3 { get; set; } = Enumerable.Empty<long> ().ToList ();
        }

        [Fact]
        public void RunOptions_Success_ListLongParameter () {
            //arrange
            var messages = new List<string> ();
            var fakeProvider = A.Fake<ICommandLineProvider> ();
            A.CallTo ( () => fakeProvider.GetCommandLine () ).Returns ( "--parameter1=1,2,3,4 --parameter2=8124343345345,23742343243,123412345345,23423423345345,814564624343,2374234,123415465642,23423456456423 --parameter3=100,100,100,100,100,100" );
            A.CallTo ( () => fakeProvider.WriteLine ( A<string>._ ) ).Invokes ( ( string fake ) => { messages.Add ( fake ); } );
            var commandLine = new CommandLine ( fakeProvider );

            //act
            var result = commandLine
                .Application ( "TestApplication", "1.0.0" )
                .AddOption ( fullName: "Parameter1" )
                .AddOption ( fullName: "Parameter2" )
                .AddOption ( fullName: "Parameter3" )
                .RunOptions<RunOptions_Success_ListLongParameter_Class> ();

            //assert
            Assert.NotNull ( result );
            Assert.Equal ( new List<long> { 1, 2, 3, 4 }, result.Parameter1 );
            Assert.Equal ( new List<long> { 8124343345345, 23742343243, 123412345345, 23423423345345, 814564624343, 2374234, 123415465642, 23423456456423 }, result.Parameter2 );
            Assert.Equal ( new List<long> { 100, 100, 100, 100, 100, 100 }, result.Parameter3 );
        }

        public record RunOptions_Success_DoubleParameter_Class {
            public double Parameter1 { get; set; }
            public double Parameter2 { get; set; }
            public double Parameter3 { get; set; }
        }

        [Fact]
        public void RunOptions_Success_DoubleParameter () {
            //arrange
            var messages = new List<string> ();
            var fakeProvider = A.Fake<ICommandLineProvider> ();
            A.CallTo ( () => fakeProvider.GetCommandLine () ).Returns ( "--parameter1=0.10 --parameter2=000000000.35 --parameter3=3333.33333333" );
            A.CallTo ( () => fakeProvider.WriteLine ( A<string>._ ) ).Invokes ( ( string fake ) => { messages.Add ( fake ); } );
            var commandLine = new CommandLine ( fakeProvider );

            //act
            var result = commandLine
                .Application ( "TestApplication", "1.0.0" )
                .AddOption ( fullName: "Parameter1" )
                .AddOption ( fullName: "Parameter2" )
                .AddOption ( fullName: "Parameter3" )
                .RunOptions<RunOptions_Success_DoubleParameter_Class> ();

            //assert
            Assert.NotNull ( result );
            Assert.Equal ( 0.10, result.Parameter1 );
            Assert.Equal ( 000000000.35, result.Parameter2 );
            Assert.Equal ( 3333.33333333, result.Parameter3 );
        }

        public record RunOptions_Success_IEnumerableDoubleParameter_Class {
            public IEnumerable<double> Parameter1 { get; set; } = Enumerable.Empty<double> ();
            public IEnumerable<double> Parameter2 { get; set; } = Enumerable.Empty<double> ();
            public IEnumerable<double> Parameter3 { get; set; } = Enumerable.Empty<double> ();
        }

        [Fact]
        public void RunOptions_Success_IEnumerableDoubleParameter () {
            //arrange
            var messages = new List<string> ();
            var fakeProvider = A.Fake<ICommandLineProvider> ();
            A.CallTo ( () => fakeProvider.GetCommandLine () ).Returns ( "--parameter1=0.1,0.2,0.3,0.4 --parameter2=8124.343,2374.234,1.23412,23423.423,81.24343,23742.34,123.412,2342.3423 --parameter3=100.0,100.01,100.02,100.03,100.04,100.05" );
            A.CallTo ( () => fakeProvider.WriteLine ( A<string>._ ) ).Invokes ( ( string fake ) => { messages.Add ( fake ); } );
            var commandLine = new CommandLine ( fakeProvider );

            //act
            var result = commandLine
                .Application ( "TestApplication", "1.0.0" )
                .AddOption ( fullName: "Parameter1" )
                .AddOption ( fullName: "Parameter2" )
                .AddOption ( fullName: "Parameter3" )
                .RunOptions<RunOptions_Success_IEnumerableDoubleParameter_Class> ();

            //assert
            Assert.NotNull ( result );
            Assert.Equal ( new List<double> { 0.1, 0.2, 0.3, 0.4 }, result.Parameter1 );
            Assert.Equal ( new List<double> { 8124.343, 2374.234, 1.23412, 23423.423, 81.24343, 23742.34, 123.412, 2342.3423 }, result.Parameter2 );
            Assert.Equal ( new List<double> { 100.0, 100.01, 100.02, 100.03, 100.04, 100.05 }, result.Parameter3 );
        }

        public record RunOptions_Success_ListDoubleParameter_Class {
            public List<double> Parameter1 { get; set; } = Enumerable.Empty<double> ().ToList ();
            public List<double> Parameter2 { get; set; } = Enumerable.Empty<double> ().ToList ();
            public List<double> Parameter3 { get; set; } = Enumerable.Empty<double> ().ToList ();
        }

        [Fact]
        public void RunOptions_Success_ListDoubleParameter () {
            //arrange
            var messages = new List<string> ();
            var fakeProvider = A.Fake<ICommandLineProvider> ();
            A.CallTo ( () => fakeProvider.GetCommandLine () ).Returns ( "--parameter1=0.1,0.2,0.3,0.4 --parameter2=812434.3345345,23742.343243,12.3412345345,23423.423345345,8145646.24343,2374.234,12341.5465642,234234.56456423 --parameter3=100.0,100.1,100.2,100.3,100.4,100.5" );
            A.CallTo ( () => fakeProvider.WriteLine ( A<string>._ ) ).Invokes ( ( string fake ) => { messages.Add ( fake ); } );
            var commandLine = new CommandLine ( fakeProvider );

            //act
            var result = commandLine
                .Application ( "TestApplication", "1.0.0" )
                .AddOption ( fullName: "Parameter1" )
                .AddOption ( fullName: "Parameter2" )
                .AddOption ( fullName: "Parameter3" )
                .RunOptions<RunOptions_Success_ListDoubleParameter_Class> ();

            //assert
            Assert.NotNull ( result );
            Assert.Equal ( new List<double> { 0.1, 0.2, 0.3, 0.4 }, result.Parameter1 );
            Assert.Equal ( new List<double> { 812434.3345345, 23742.343243, 12.3412345345, 23423.423345345, 8145646.24343, 2374.234, 12341.5465642, 234234.56456423 }, result.Parameter2 );
            Assert.Equal ( new List<double> { 100.0, 100.1, 100.2, 100.3, 100.4, 100.5 }, result.Parameter3 );
        }

    }

}
