﻿using FakeItEasy;
using FlowCommandLine;
using FlowCommandLine.SpecialTypes;

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
            var result = commandLine
                .Application ( "TestApplication", "1.0.0" )
                .RunCommand ();

            //assert
            Assert.NotEmpty ( messages );
            Assert.Equal ( 2, messages.Count );
            Assert.Equal ( "TestApplication version 1.0.0", messages.First () );
            Assert.Equal ( " ", messages.ElementAt ( 1 ) );
            Assert.True ( result.EmptyInput );
            Assert.False ( result.Handled );
            Assert.False ( result.CommandHandled );
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
            var result = commandLine
                .Application ( "TestApplication", "1.0.0", "Application description!!!!" )
                .RunCommand ();

            //assert
            Assert.NotEmpty ( messages );
            Assert.Equal ( 4, messages.Count );
            Assert.Equal ( "TestApplication version 1.0.0", messages.First () );
            Assert.Equal ( " ", messages.ElementAt ( 1 ) );
            Assert.Equal ( "Application description!!!!", messages.ElementAt ( 2 ) );
            Assert.Equal ( " ", messages.ElementAt ( 3 ) );
            Assert.True ( result.EmptyInput );
            Assert.False ( result.Handled );
            Assert.False ( result.CommandHandled );
        }

        [Fact]
        public void RunCommand_Success_OnlyDescription_ShowHelp_Parameter () {
            //arrange
            var messages = new List<string> ();
            var fakeProvider = A.Fake<ICommandLineProvider> ();
            A.CallTo ( () => fakeProvider.GetCommandLine () ).Returns ( "--help" );
            A.CallTo ( () => fakeProvider.WriteLine ( A<string>._ ) ).Invokes ( ( string fake ) => { messages.Add ( fake ); } );
            var commandLine = new CommandLine ( fakeProvider );

            //act
            var result = commandLine
                .Application ( "TestApplication", "1.0.0", "Application description!!!!" )
                .RunCommand ();

            //assert
            Assert.NotEmpty ( messages );
            Assert.Equal ( 4, messages.Count );
            Assert.Equal ( "TestApplication version 1.0.0", messages.First () );
            Assert.Equal ( " ", messages.ElementAt ( 1 ) );
            Assert.Equal ( "Application description!!!!", messages.ElementAt ( 2 ) );
            Assert.Equal ( " ", messages.ElementAt ( 3 ) );
            Assert.False ( result.EmptyInput );
            Assert.True ( result.Handled );
            Assert.False ( result.CommandHandled );
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
            var result = commandLine
                .Application ( "TestApplication", "1.0.0", applicationExecutable: "testapp" )
                .RunCommand ();

            //assert
            Assert.NotEmpty ( messages );
            Assert.Equal ( 4, messages.Count );
            Assert.Equal ( "TestApplication version 1.0.0", messages.First () );
            Assert.Equal ( " ", messages.ElementAt ( 1 ) );
            Assert.Equal ( "usage: testapp [<command>] [<parameters>]", messages.ElementAt ( 2 ) );
            Assert.Equal ( " ", messages.ElementAt ( 3 ) );
            Assert.True ( result.EmptyInput );
            Assert.False ( result.Handled );
            Assert.False ( result.CommandHandled );
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
            var result = commandLine
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
            Assert.False ( result.EmptyInput );
            Assert.False ( result.Handled );
            Assert.True ( result.CommandHandled );
        }

        public record RunCommand_Success_IntLongDoubleFloatDecimalTypes_Class {
            public int Parameter1 { get; set; }
            public long Parameter2 { get; set; }
            public double Parameter3 { get; set; }
            public float Parameter4 { get; set; }
            public decimal Parameter5 { get; set; }
        }

        [Fact]
        public void RunCommand_Success_IntLongDoubleFloatDecimalTypes () {
            //arrange
            var messages = new List<string> ();
            var fakeProvider = A.Fake<ICommandLineProvider> ();
            A.CallTo ( () => fakeProvider.GetCommandLine () ).Returns ( "test -parameter1=1200 -parameter2=5000000000 -parameter3=5.20 -parameter4=0.20 -parameter5=35.89" );
            var commandLine = new CommandLine ( fakeProvider );

            var isCorrect = false;

            //act
            commandLine
                .Application ( "TestApplication", "1.0.0" )
                .AddCommand (
                    "test",
                    ( RunCommand_Success_IntLongDoubleFloatDecimalTypes_Class arg ) => {
                        isCorrect = arg.Parameter1 == 1200 &&
                            arg.Parameter2 == 5000000000 &&
                            arg.Parameter3 == 5.20d &&
                            arg.Parameter4 == 0.20f &&
                            arg.Parameter5 == 35.89M;
                    },
                    "",
                    new List<FlowCommandParameter> {
                        new FlowCommandParameter { FullName = "Parameter1" },
                        new FlowCommandParameter { FullName = "Parameter2" },
                        new FlowCommandParameter { FullName = "Parameter3" },
                        new FlowCommandParameter { FullName = "Parameter4" },
                        new FlowCommandParameter { FullName = "Parameter5" },
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
                        new FlowCommandParameter { FullName = "Parameter1", Description = "Description", Required = true },
                        new FlowCommandParameter { FullName = "Parameter2", Description = "Description", Required = false }
                    }
                )
                .RunCommand ();

            //assert
            Assert.True ( isCorrect );
            Assert.Equal ( "Not all required parameters is defined!", messages.First () );
            Assert.Equal ( " ", messages.ElementAt ( 1 ) );
            Assert.Equal ( "The following arguments are available:", messages.ElementAt ( 2 ) );
            Assert.Equal ( "  --Parameter1 Description", messages.ElementAt ( 3 ) );
            Assert.Equal ( "The following options are available:", messages.ElementAt ( 4 ) );
            Assert.Equal ( "  --Parameter2 Description", messages.ElementAt ( 5 ) );
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

        [Fact]
        public void RunCommand_Success_DateOnlyTimeSpan_Names () {
            //arrange
            var messages = new List<string> ();
            var fakeProvider = A.Fake<ICommandLineProvider> ();
            A.CallTo ( () => fakeProvider.GetCommandLine () ).Returns ( "test -p1=2025-02-04 -p2=2025-02-04T12:35:20 -p3=18:10:20" );
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
                        FlowCommandParameter.Create("p1", property: "Parameter1"),
                        FlowCommandParameter.Create("p2", property: "Parameter2"),
                        FlowCommandParameter.Create("p3", property: "Parameter3"),
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
            public List<int> Parameter1 { get; set; } = Enumerable.Empty<int> ().ToList ();
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

        public record RunOptions_Success_IEnumerableDecimalParameter_Class {
            public IEnumerable<decimal> Parameter1 { get; set; } = Enumerable.Empty<decimal> ();
            public IEnumerable<decimal> Parameter2 { get; set; } = Enumerable.Empty<decimal> ();
            public IEnumerable<decimal> Parameter3 { get; set; } = Enumerable.Empty<decimal> ();
        }

        [Fact]
        public void RunOptions_Success_IEnumerableDecimalParameter () {
            //arrange
            var messages = new List<string> ();
            var fakeProvider = A.Fake<ICommandLineProvider> ();
            A.CallTo ( () => fakeProvider.GetCommandLine () ).Returns ( "--parameter1=1.8,28.30,39.2,43.89 --parameter2=8124343.5656,2374234.567565,123412.343434,23423423.234234,8124343.2342342,2374234.546546,123412.23123,23423423 --parameter3=100,100,100,100,100,100" );
            A.CallTo ( () => fakeProvider.WriteLine ( A<string>._ ) ).Invokes ( ( string fake ) => { messages.Add ( fake ); } );
            var commandLine = new CommandLine ( fakeProvider );

            //act
            var result = commandLine
                .Application ( "TestApplication", "1.0.0" )
                .AddOption ( fullName: "Parameter1" )
                .AddOption ( fullName: "Parameter2" )
                .AddOption ( fullName: "Parameter3" )
                .RunOptions<RunOptions_Success_IEnumerableDecimalParameter_Class> ();

            //assert
            Assert.NotNull ( result );
            Assert.Equal ( new List<decimal> { 1.8M, 28.30M, 39.2M, 43.89M }, result.Parameter1 );
            Assert.Equal ( new List<decimal> { 8124343.5656M, 2374234.567565M, 123412.343434M, 23423423.234234M, 8124343.2342342M, 2374234.546546M, 123412.23123M, 23423423M }, result.Parameter2 );
            Assert.Equal ( new List<decimal> { 100, 100, 100, 100, 100, 100 }, result.Parameter3 );
        }

        public record RunOptions_Success_ListDecimalParameter_Class {
            public List<decimal> Parameter1 { get; set; } = Enumerable.Empty<decimal> ().ToList ();
            public List<decimal> Parameter2 { get; set; } = Enumerable.Empty<decimal> ().ToList ();
            public List<decimal> Parameter3 { get; set; } = Enumerable.Empty<decimal> ().ToList ();
        }

        [Fact]
        public void RunOptions_Success_ListDecimalParameter () {
            //arrange
            var messages = new List<string> ();
            var fakeProvider = A.Fake<ICommandLineProvider> ();
            A.CallTo ( () => fakeProvider.GetCommandLine () ).Returns ( "--parameter1=1.8,28.30,39.2,43.89 --parameter2=8124343.5656,2374234.567565,123412.343434,23423423.234234,8124343.2342342,2374234.546546,123412.23123,23423423 --parameter3=100,100,100,100,100,100" );
            A.CallTo ( () => fakeProvider.WriteLine ( A<string>._ ) ).Invokes ( ( string fake ) => { messages.Add ( fake ); } );
            var commandLine = new CommandLine ( fakeProvider );

            //act
            var result = commandLine
                .Application ( "TestApplication", "1.0.0" )
                .AddOption ( fullName: "Parameter1" )
                .AddOption ( fullName: "Parameter2" )
                .AddOption ( fullName: "Parameter3" )
                .RunOptions<RunOptions_Success_ListDecimalParameter_Class> ();

            //assert
            Assert.NotNull ( result );
            Assert.Equal ( new List<decimal> { 1.8M, 28.30M, 39.2M, 43.89M }, result.Parameter1 );
            Assert.Equal ( new List<decimal> { 8124343.5656M, 2374234.567565M, 123412.343434M, 23423423.234234M, 8124343.2342342M, 2374234.546546M, 123412.23123M, 23423423M }, result.Parameter2 );
            Assert.Equal ( new List<decimal> { 100, 100, 100, 100, 100, 100 }, result.Parameter3 );
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

        public record RunOptions_Success_FloatParameter_Class {
            public float Parameter1 { get; set; }
            public float Parameter2 { get; set; }
            public float Parameter3 { get; set; }
        }

        [Fact]
        public void RunOptions_Success_FloatParameter () {
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
                .RunOptions<RunOptions_Success_FloatParameter_Class> ();

            //assert
            Assert.NotNull ( result );
            Assert.Equal ( 0.10f, result.Parameter1 );
            Assert.Equal ( 000000000.35f, result.Parameter2 );
            Assert.Equal ( 3333.33333333f, result.Parameter3 );
        }

        public record RunOptions_Success_IEnumerableFloatParameter_Class {
            public IEnumerable<float> Parameter1 { get; set; } = Enumerable.Empty<float> ();
            public IEnumerable<float> Parameter2 { get; set; } = Enumerable.Empty<float> ();
            public IEnumerable<float> Parameter3 { get; set; } = Enumerable.Empty<float> ();
        }

        [Fact]
        public void RunOptions_Success_IEnumerableFloatParameter () {
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
                .RunOptions<RunOptions_Success_IEnumerableFloatParameter_Class> ();

            //assert
            Assert.NotNull ( result );
            Assert.Equal ( new List<float> { 0.1f, 0.2f, 0.3f, 0.4f }, result.Parameter1 );
            Assert.Equal ( new List<float> { 8124.343f, 2374.234f, 1.23412f, 23423.423f, 81.24343f, 23742.34f, 123.412f, 2342.3423f }, result.Parameter2 );
            Assert.Equal ( new List<float> { 100.0f, 100.01f, 100.02f, 100.03f, 100.04f, 100.05f }, result.Parameter3 );
        }

        public record RunOptions_Success_ListFloatParameter_Class {
            public List<float> Parameter1 { get; set; } = Enumerable.Empty<float> ().ToList ();
            public List<float> Parameter2 { get; set; } = Enumerable.Empty<float> ().ToList ();
            public List<float> Parameter3 { get; set; } = Enumerable.Empty<float> ().ToList ();
        }

        [Fact]
        public void RunOptions_Success_ListFloatParameter () {
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
                .RunOptions<RunOptions_Success_ListFloatParameter_Class> ();

            //assert
            Assert.NotNull ( result );
            Assert.Equal ( new List<float> { 0.1f, 0.2f, 0.3f, 0.4f }, result.Parameter1 );
            Assert.Equal ( new List<float> { 812434.3345345f, 23742.343243f, 12.3412345345f, 23423.423345345f, 8145646.24343f, 2374.234f, 12341.5465642f, 234234.56456423f }, result.Parameter2 );
            Assert.Equal ( new List<float> { 100.0f, 100.1f, 100.2f, 100.3f, 100.4f, 100.5f }, result.Parameter3 );
        }

        public record RunOptions_Success_StringParameter_Class {
            public string Parameter1 { get; set; } = "";
            public string Parameter2 { get; set; } = "";
            public string Parameter3 { get; set; } = "";
        }

        [Fact]
        public void RunOptions_Success_StringParameter () {
            //arrange
            var messages = new List<string> ();
            var fakeProvider = A.Fake<ICommandLineProvider> ();
            A.CallTo ( () => fakeProvider.GetCommandLine () ).Returns ( "--parameter1=argus --parameter2=test values with spaces --parameter3=\"quotes string with spaces\"" );
            A.CallTo ( () => fakeProvider.WriteLine ( A<string>._ ) ).Invokes ( ( string fake ) => { messages.Add ( fake ); } );
            var commandLine = new CommandLine ( fakeProvider );

            //act
            var result = commandLine
                .Application ( "TestApplication", "1.0.0" )
                .AddOption ( fullName: "Parameter1" )
                .AddOption ( fullName: "Parameter2" )
                .AddOption ( fullName: "Parameter3" )
                .RunOptions<RunOptions_Success_StringParameter_Class> ();

            //assert
            Assert.NotNull ( result );
            Assert.Equal ( "argus", result.Parameter1 );
            Assert.Equal ( "test values with spaces", result.Parameter2 );
            Assert.Equal ( "quotes string with spaces", result.Parameter3 );
        }

        public record RunOptions_Success_IEnumerableStringParameter_Class {
            public IEnumerable<string> Parameter1 { get; set; } = Enumerable.Empty<string> ();
            public IEnumerable<string> Parameter2 { get; set; } = Enumerable.Empty<string> ();
            public IEnumerable<string> Parameter3 { get; set; } = Enumerable.Empty<string> ();
        }

        [Fact]
        public void RunOptions_Success_IEnumerableStringParameter () {
            //arrange
            var messages = new List<string> ();
            var fakeProvider = A.Fake<ICommandLineProvider> ();
            A.CallTo ( () => fakeProvider.GetCommandLine () ).Returns ( "--parameter1=argus bargus margus --parameter2=test values with spaces --parameter3=first1212121 \"quotes string with spaces\" last4534534" );
            A.CallTo ( () => fakeProvider.WriteLine ( A<string>._ ) ).Invokes ( ( string fake ) => { messages.Add ( fake ); } );
            var commandLine = new CommandLine ( fakeProvider );

            //act
            var result = commandLine
                .Application ( "TestApplication", "1.0.0" )
                .AddOption ( fullName: "Parameter1" )
                .AddOption ( fullName: "Parameter2" )
                .AddOption ( fullName: "Parameter3" )
                .RunOptions<RunOptions_Success_IEnumerableStringParameter_Class> ();

            //assert
            Assert.NotNull ( result );
            Assert.Equal ( new List<string> { "argus", "bargus", "margus" }, result.Parameter1 );
            Assert.Equal ( new List<string> { "test", "values", "with", "spaces" }, result.Parameter2 );
            Assert.Equal ( new List<string> { "first1212121", "quotes string with spaces", "last4534534" }, result.Parameter3 );
        }

        public record RunOptions_Success_ListStringParameter_Class {
            public List<string> Parameter1 { get; set; } = Enumerable.Empty<string> ().ToList ();
            public List<string> Parameter2 { get; set; } = Enumerable.Empty<string> ().ToList ();
            public List<string> Parameter3 { get; set; } = Enumerable.Empty<string> ().ToList ();
        }

        [Fact]
        public void RunOptions_Success_ListStringParameter () {
            //arrange
            var messages = new List<string> ();
            var fakeProvider = A.Fake<ICommandLineProvider> ();
            A.CallTo ( () => fakeProvider.GetCommandLine () ).Returns ( "--parameter1=argus bargus margus --parameter2=\"test\" values with \"spaces\" --parameter3=first1212121 \"quotes string with spaces\" last4534534" );
            A.CallTo ( () => fakeProvider.WriteLine ( A<string>._ ) ).Invokes ( ( string fake ) => { messages.Add ( fake ); } );
            var commandLine = new CommandLine ( fakeProvider );

            //act
            var result = commandLine
                .Application ( "TestApplication", "1.0.0" )
                .AddOption ( fullName: "Parameter1" )
                .AddOption ( fullName: "Parameter2" )
                .AddOption ( fullName: "Parameter3" )
                .RunOptions<RunOptions_Success_ListStringParameter_Class> ();

            //assert
            Assert.NotNull ( result );
            Assert.Equal ( new List<string> { "argus", "bargus", "margus" }, result.Parameter1 );
            Assert.Equal ( new List<string> { "test", "values", "with", "spaces" }, result.Parameter2 );
            Assert.Equal ( new List<string> { "first1212121", "quotes string with spaces", "last4534534" }, result.Parameter3 );
        }

        public record RunOptions_Success_RunOptions_Success_BooleanParameter_Class {
            public string Parameter1 { get; set; } = "";
            public bool Parameter2 { get; set; }
            public string Parameter3 { get; set; } = "";
        }

        [Fact]
        public void RunOptions_Success_BooleanParameter () {
            //arrange
            var messages = new List<string> ();
            var fakeProvider = A.Fake<ICommandLineProvider> ();
            A.CallTo ( () => fakeProvider.GetCommandLine () ).Returns ( "--parameter1=Lala --parameter2 --parameter3=Muhaha" );
            A.CallTo ( () => fakeProvider.WriteLine ( A<string>._ ) ).Invokes ( ( string fake ) => { messages.Add ( fake ); } );
            var commandLine = new CommandLine ( fakeProvider );

            //act
            var result = commandLine
                .Application ( "TestApplication", "1.0.0" )
                .AddOption ( fullName: "Parameter1" )
                .AddOption ( fullName: "Parameter2" )
                .AddOption ( fullName: "Parameter3" )
                .RunOptions<RunOptions_Success_RunOptions_Success_BooleanParameter_Class> ();

            //assert
            Assert.NotNull ( result );
            Assert.Equal ( "Lala", result.Parameter1 );
            Assert.True ( result.Parameter2 );
            Assert.Equal ( "Muhaha", result.Parameter3 );
        }

        [Fact]
        public void RunOptions_Success_BooleanParameter_AsTrue () {
            //arrange
            var messages = new List<string> ();
            var fakeProvider = A.Fake<ICommandLineProvider> ();
            A.CallTo ( () => fakeProvider.GetCommandLine () ).Returns ( "--parameter1=Lala --parameter2=true --parameter3=Muhaha" );
            A.CallTo ( () => fakeProvider.WriteLine ( A<string>._ ) ).Invokes ( ( string fake ) => { messages.Add ( fake ); } );
            var commandLine = new CommandLine ( fakeProvider );

            //act
            var result = commandLine
                .Application ( "TestApplication", "1.0.0" )
                .AddOption ( fullName: "Parameter1" )
                .AddOption ( fullName: "Parameter2" )
                .AddOption ( fullName: "Parameter3" )
                .RunOptions<RunOptions_Success_RunOptions_Success_BooleanParameter_Class> ();

            //assert
            Assert.NotNull ( result );
            Assert.Equal ( "Lala", result.Parameter1 );
            Assert.True ( result.Parameter2 );
            Assert.Equal ( "Muhaha", result.Parameter3 );
        }

        [Fact]
        public void RunOptions_Success_BooleanParameter_AsFalse () {
            //arrange
            var messages = new List<string> ();
            var fakeProvider = A.Fake<ICommandLineProvider> ();
            A.CallTo ( () => fakeProvider.GetCommandLine () ).Returns ( "--parameter1=Lala --parameter2=false --parameter3=Muhaha" );
            A.CallTo ( () => fakeProvider.WriteLine ( A<string>._ ) ).Invokes ( ( string fake ) => { messages.Add ( fake ); } );
            var commandLine = new CommandLine ( fakeProvider );

            //act
            var result = commandLine
                .Application ( "TestApplication", "1.0.0" )
                .AddOption ( fullName: "Parameter1" )
                .AddOption ( fullName: "Parameter2" )
                .AddOption ( fullName: "Parameter3" )
                .RunOptions<RunOptions_Success_RunOptions_Success_BooleanParameter_Class> ();

            //assert
            Assert.NotNull ( result );
            Assert.Equal ( "Lala", result.Parameter1 );
            Assert.False ( result.Parameter2 );
            Assert.Equal ( "Muhaha", result.Parameter3 );
        }

        [Fact]
        public void RunOptions_Success_BooleanParameter_AsFalse_NotFilled () {
            //arrange
            var messages = new List<string> ();
            var fakeProvider = A.Fake<ICommandLineProvider> ();
            A.CallTo ( () => fakeProvider.GetCommandLine () ).Returns ( "--parameter1=Lala --parameter3=Muhaha" );
            A.CallTo ( () => fakeProvider.WriteLine ( A<string>._ ) ).Invokes ( ( string fake ) => { messages.Add ( fake ); } );
            var commandLine = new CommandLine ( fakeProvider );

            //act
            var result = commandLine
                .Application ( "TestApplication", "1.0.0" )
                .AddOption ( fullName: "Parameter1" )
                .AddOption ( fullName: "Parameter2" )
                .AddOption ( fullName: "Parameter3" )
                .RunOptions<RunOptions_Success_RunOptions_Success_BooleanParameter_Class> ();

            //assert
            Assert.NotNull ( result );
            Assert.Equal ( "Lala", result.Parameter1 );
            Assert.False ( result.Parameter2 );
            Assert.Equal ( "Muhaha", result.Parameter3 );
        }

        public record RunOptions_Success_RangeIntParameter_Completed_Class {

            public CommandLineRange<int> Parameter1 { get; set; } = new CommandLineRange<int> ();

        }

        [Fact]
        public void RunOptions_Success_RangeIntParameter_Completed () {
            //arrange
            var messages = new List<string> ();
            var fakeProvider = A.Fake<ICommandLineProvider> ();
            A.CallTo ( () => fakeProvider.GetCommandLine () ).Returns ( "--parameter1=1-100" );
            A.CallTo ( () => fakeProvider.WriteLine ( A<string>._ ) ).Invokes ( ( string fake ) => { messages.Add ( fake ); } );
            var commandLine = new CommandLine ( fakeProvider );

            //act
            var result = commandLine
                .Application ( "TestApplication", "1.0.0" )
                .AddOption ( fullName: "Parameter1" )
                .RunOptions<RunOptions_Success_RangeIntParameter_Completed_Class> ();

            //assert
            Assert.NotNull ( result );
            Assert.Equal ( 1, result.Parameter1.Start );
            Assert.Equal ( 100, result.Parameter1.End );
        }

        public record RunOptions_Success_RangeIntParameter_NotParsed_Class {

            public CommandLineRange<int> Parameter1 { get; set; } = new CommandLineRange<int> ();

            public CommandLineRange<int> Parameter2 { get; set; } = new CommandLineRange<int> ();

            public CommandLineRange<int> Parameter3 { get; set; } = new CommandLineRange<int> ();

            public CommandLineRange<int> Parameter4 { get; set; } = new CommandLineRange<int> ();

        }

        [Fact]
        public void RunOptions_Success_RangeIntParameter_NotParsed () {
            //arrange
            var messages = new List<string> ();
            var fakeProvider = A.Fake<ICommandLineProvider> ();
            A.CallTo ( () => fakeProvider.GetCommandLine () ).Returns ( "--parameter1=1 --parameter2=- --parameter3=-100 --parameter4=1-" );
            A.CallTo ( () => fakeProvider.WriteLine ( A<string>._ ) ).Invokes ( ( string fake ) => { messages.Add ( fake ); } );
            var commandLine = new CommandLine ( fakeProvider );

            //act
            var result = commandLine
                .Application ( "TestApplication", "1.0.0" )
                .AddOption ( fullName: "Parameter1" )
                .AddOption ( fullName: "Parameter2" )
                .AddOption ( fullName: "Parameter3" )
                .AddOption ( fullName: "Parameter4" )
                .RunOptions<RunOptions_Success_RangeIntParameter_NotParsed_Class> ();

            //assert
            Assert.NotNull ( result );
            Assert.Equal ( 0, result.Parameter1.Start );
            Assert.Equal ( 0, result.Parameter1.End );
            Assert.Equal ( 0, result.Parameter2.Start );
            Assert.Equal ( 0, result.Parameter2.End );
            Assert.Equal ( 0, result.Parameter3.Start );
            Assert.Equal ( 0, result.Parameter3.End );
            Assert.Equal ( 0, result.Parameter4.Start );
            Assert.Equal ( 0, result.Parameter4.End );
        }

        public record RunOptions_Success_RangeDoubleParameter_NotParsed_Class {

            public CommandLineRange<double> Parameter1 { get; set; } = new CommandLineRange<double> ();

            public CommandLineRange<double> Parameter2 { get; set; } = new CommandLineRange<double> ();

            public CommandLineRange<double> Parameter3 { get; set; } = new CommandLineRange<double> ();

            public CommandLineRange<double> Parameter4 { get; set; } = new CommandLineRange<double> ();

            public CommandLineRange<double> Parameter5 { get; set; } = new CommandLineRange<double> ();

        }

        [Fact]
        public void RunOptions_Success_RangeDoubleParameter () {
            //arrange
            var messages = new List<string> ();
            var fakeProvider = A.Fake<ICommandLineProvider> ();
            A.CallTo ( () => fakeProvider.GetCommandLine () ).Returns ( "--parameter1=235.30 --parameter2=- --parameter3=-100.30 --parameter4=178.56- --parameter5=178.56-895.450" );
            A.CallTo ( () => fakeProvider.WriteLine ( A<string>._ ) ).Invokes ( ( string fake ) => { messages.Add ( fake ); } );
            var commandLine = new CommandLine ( fakeProvider );

            //act
            var result = commandLine
                .Application ( "TestApplication", "1.0.0" )
                .AddOption ( fullName: "Parameter1" )
                .AddOption ( fullName: "Parameter2" )
                .AddOption ( fullName: "Parameter3" )
                .AddOption ( fullName: "Parameter4" )
                .AddOption ( fullName: "Parameter5" )
                .RunOptions<RunOptions_Success_RangeDoubleParameter_NotParsed_Class> ();

            //assert
            Assert.NotNull ( result );
            Assert.Equal ( 0, result.Parameter1.Start );
            Assert.Equal ( 0, result.Parameter1.End );
            Assert.Equal ( 0, result.Parameter2.Start );
            Assert.Equal ( 0, result.Parameter2.End );
            Assert.Equal ( 0, result.Parameter3.Start );
            Assert.Equal ( 0, result.Parameter3.End );
            Assert.Equal ( 0, result.Parameter4.Start );
            Assert.Equal ( 0, result.Parameter4.End );
            Assert.Equal ( 178.56, result.Parameter5.Start );
            Assert.Equal ( 895.450, result.Parameter5.End );
        }

        public record RunOptions_Success_RangeFloatParameter_Class {

            public CommandLineRange<float> Parameter1 { get; set; } = new CommandLineRange<float> ();

            public CommandLineRange<float> Parameter2 { get; set; } = new CommandLineRange<float> ();

            public CommandLineRange<float> Parameter3 { get; set; } = new CommandLineRange<float> ();

            public CommandLineRange<float> Parameter4 { get; set; } = new CommandLineRange<float> ();

            public CommandLineRange<float> Parameter5 { get; set; } = new CommandLineRange<float> ();

        }

        [Fact]
        public void RunOptions_Success_RangeFloatParameter () {
            //arrange
            var messages = new List<string> ();
            var fakeProvider = A.Fake<ICommandLineProvider> ();
            A.CallTo ( () => fakeProvider.GetCommandLine () ).Returns ( "--parameter1=235.30 --parameter2=- --parameter3=-100.30 --parameter4=178.56- --parameter5=178.56-895.450" );
            A.CallTo ( () => fakeProvider.WriteLine ( A<string>._ ) ).Invokes ( ( string fake ) => { messages.Add ( fake ); } );
            var commandLine = new CommandLine ( fakeProvider );

            //act
            var result = commandLine
                .Application ( "TestApplication", "1.0.0" )
                .AddOption ( fullName: "Parameter1" )
                .AddOption ( fullName: "Parameter2" )
                .AddOption ( fullName: "Parameter3" )
                .AddOption ( fullName: "Parameter4" )
                .AddOption ( fullName: "Parameter5" )
                .RunOptions<RunOptions_Success_RangeFloatParameter_Class> ();

            //assert
            Assert.NotNull ( result );
            Assert.Equal ( 0, result.Parameter1.Start );
            Assert.Equal ( 0, result.Parameter1.End );
            Assert.Equal ( 0, result.Parameter2.Start );
            Assert.Equal ( 0, result.Parameter2.End );
            Assert.Equal ( 0, result.Parameter3.Start );
            Assert.Equal ( 0, result.Parameter3.End );
            Assert.Equal ( 0, result.Parameter4.Start );
            Assert.Equal ( 0, result.Parameter4.End );
            Assert.Equal ( 178.56f, result.Parameter5.Start );
            Assert.Equal ( 895.450f, result.Parameter5.End );
        }

        public record RunOptions_Success_RangeDecimalParameter_Class {

            public CommandLineRange<decimal> Parameter1 { get; set; } = new CommandLineRange<decimal> ();

            public CommandLineRange<decimal> Parameter2 { get; set; } = new CommandLineRange<decimal> ();

            public CommandLineRange<decimal> Parameter3 { get; set; } = new CommandLineRange<decimal> ();

            public CommandLineRange<decimal> Parameter4 { get; set; } = new CommandLineRange<decimal> ();

            public CommandLineRange<decimal> Parameter5 { get; set; } = new CommandLineRange<decimal> ();

        }

        [Fact]
        public void RunOptions_Success_RangeDecimalParameter () {
            //arrange
            var messages = new List<string> ();
            var fakeProvider = A.Fake<ICommandLineProvider> ();
            A.CallTo ( () => fakeProvider.GetCommandLine () ).Returns ( "--parameter1=235.30 --parameter2=- --parameter3=-100.30 --parameter4=178.56- --parameter5=178.56-895.450" );
            A.CallTo ( () => fakeProvider.WriteLine ( A<string>._ ) ).Invokes ( ( string fake ) => { messages.Add ( fake ); } );
            var commandLine = new CommandLine ( fakeProvider );

            //act
            var result = commandLine
                .Application ( "TestApplication", "1.0.0" )
                .AddOption ( fullName: "Parameter1" )
                .AddOption ( fullName: "Parameter2" )
                .AddOption ( fullName: "Parameter3" )
                .AddOption ( fullName: "Parameter4" )
                .AddOption ( fullName: "Parameter5" )
                .RunOptions<RunOptions_Success_RangeDecimalParameter_Class> ();

            //assert
            Assert.NotNull ( result );
            Assert.Equal ( 0, result.Parameter1.Start );
            Assert.Equal ( 0, result.Parameter1.End );
            Assert.Equal ( 0, result.Parameter2.Start );
            Assert.Equal ( 0, result.Parameter2.End );
            Assert.Equal ( 0, result.Parameter3.Start );
            Assert.Equal ( 0, result.Parameter3.End );
            Assert.Equal ( 0, result.Parameter4.Start );
            Assert.Equal ( 0, result.Parameter4.End );
            Assert.Equal ( 178.56M, result.Parameter5.Start );
            Assert.Equal ( 895.450M, result.Parameter5.End );
        }

        public class DatabaseAdjustments {

            public IEnumerable<string> Files { get; set; } = Enumerable.Empty<string> ();

            public IEnumerable<string> ConnectionStrings { get; set; } = Enumerable.Empty<string> ();

            public string Strategy { get; set; } = "";

            public string Group { get; set; } = "";

            public string MigrationTable { get; set; } = "";

        }

        [Fact]
        public void RunCommand_Success_ComplexParameters () {
            //arrange
            var databaseAdjustments = new List<FlowCommandParameter> {
                FlowCommandParameter.Create("f", "files", "List of files containing migrations."),
                FlowCommandParameter.CreateRequired("c", "connectionStrings", "List of connection strings to which migrations will be applied."),
                FlowCommandParameter.CreateRequired("s", "strategy", "Select strategy for read migrations."),
                FlowCommandParameter.Create("g", "group", "If you specify some group or groups (separated by commas), migrations will be filtered by these groups."),
                FlowCommandParameter.Create("t", "tablename", "You can change the name of the table in which the migrations will be stored.", "MigrationTable"),
            };
            var messages = new List<string> ();
            var fakeProvider = A.Fake<ICommandLineProvider> ();
            A.CallTo ( () => fakeProvider.GetCommandLine () ).Returns ( "apply -f=src/ONielCms/bin/Debug/net8.0/ONielCommon.dll -c=Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=onielcms -s=CSharpClasses" );
            A.CallTo ( () => fakeProvider.WriteLine ( A<string>._ ) ).Invokes ( ( string fake ) => { messages.Add ( fake ); } );
            var commandLine = new CommandLine ( fakeProvider );
            DatabaseAdjustments result = new DatabaseAdjustments ();

            //act
            commandLine
                .Application ( "TestApplication", "1.0.0" )
                .AddCommand<DatabaseAdjustments> (
                    "apply",
                    ( DatabaseAdjustments options ) => {
                        result = options;
                    },
                    "",
                    databaseAdjustments
                )
                .RunCommand ();

            //assert
            Assert.NotNull ( result );
            Assert.Equal ( result.Files, new List<string> { "src/ONielCms/bin/Debug/net8.0/ONielCommon.dll" } );
            Assert.Equal ( result.ConnectionStrings, new List<string> { "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=onielcms" } );
            Assert.Equal ( "CSharpClasses", result.Strategy );
        }

        [Fact]
        public void ParseParameters_Success_DefaultParameter_OnlyDefault () {
            //arrange
            List<string> parts = ["command", "defaultValue"];

            //act
            CommandLine.ParseParameters ( parts, out var command, out var parameters, out var defaultParameter );

            //assert
            Assert.Equal ( "command", command );
            Assert.Equal ( "defaultValue", defaultParameter );
            Assert.Empty ( parameters );
        }

        [Fact]
        public void RunCommand_Success_ComplexParameters_Case1 () {
            //arrange
            var databaseAdjustments = new List<FlowCommandParameter> {
                FlowCommandParameter.Create("f", "files", "List of files containing migrations."),
                FlowCommandParameter.CreateRequired("c", "connectionStrings", "List of connection strings to which migrations will be applied."),
                FlowCommandParameter.CreateDefault("s", "strategy", "Select strategy for read migrations."),
                FlowCommandParameter.Create("g", "group", "If you specify some group or groups (separated by commas), migrations will be filtered by these groups."),
                FlowCommandParameter.Create("t", "tablename", "You can change the name of the table in which the migrations will be stored.", "MigrationTable"),
            };
            var messages = new List<string> ();
            var fakeProvider = A.Fake<ICommandLineProvider> ();
            A.CallTo ( () => fakeProvider.GetCommandLine () ).Returns ( "apply CSharpClasses -f=src/ONielCms/bin/Debug/net8.0/ONielCommon.dll -c=Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=onielcms" );
            A.CallTo ( () => fakeProvider.WriteLine ( A<string>._ ) ).Invokes ( ( string fake ) => { messages.Add ( fake ); } );
            var commandLine = new CommandLine ( fakeProvider );
            DatabaseAdjustments result = new DatabaseAdjustments ();

            //act
            commandLine
                .Application ( "TestApplication", "1.0.0" )
                .AddCommand<DatabaseAdjustments> (
                    "apply",
                    ( DatabaseAdjustments options ) => {
                        result = options;
                    },
                    "",
                    databaseAdjustments
                )
                .RunCommand ();

            //assert
            Assert.NotNull ( result );
            Assert.Equal ( result.Files, new List<string> { "src/ONielCms/bin/Debug/net8.0/ONielCommon.dll" } );
            Assert.Equal ( result.ConnectionStrings, new List<string> { "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=onielcms" } );
            Assert.Equal ( "CSharpClasses", result.Strategy );
        }

        [Fact]
        public void RunOptions_Success_DefaultParameter () {
            //arrange
            var messages = new List<string> ();
            var fakeProvider = A.Fake<ICommandLineProvider> ();
            A.CallTo ( () => fakeProvider.GetCommandLine () ).Returns ( "178.56-895.450 --parameter1=235.30 --parameter2=- --parameter3=-100.30 --parameter4=178.56-" );
            A.CallTo ( () => fakeProvider.WriteLine ( A<string>._ ) ).Invokes ( ( string fake ) => { messages.Add ( fake ); } );
            var commandLine = new CommandLine ( fakeProvider );

            //act
            var result = commandLine
                .Application ( "TestApplication", "1.0.0" )
                .AddOptions (
                    [
                        FlowCommandParameter.Create(name: "Parameter1"),
                        FlowCommandParameter.Create(name: "Parameter2"),
                        FlowCommandParameter.Create(name: "Parameter3"),
                        FlowCommandParameter.Create(name: "Parameter4"),
                        FlowCommandParameter.CreateDefault(name: "Parameter5"),
                    ]
                )
                .RunOptions<RunOptions_Success_RangeDecimalParameter_Class> ();

            //assert
            Assert.NotNull ( result );
            Assert.Equal ( 0, result.Parameter1.Start );
            Assert.Equal ( 0, result.Parameter1.End );
            Assert.Equal ( 0, result.Parameter2.Start );
            Assert.Equal ( 0, result.Parameter2.End );
            Assert.Equal ( 0, result.Parameter3.Start );
            Assert.Equal ( 0, result.Parameter3.End );
            Assert.Equal ( 0, result.Parameter4.Start );
            Assert.Equal ( 0, result.Parameter4.End );
            Assert.Equal ( 178.56M, result.Parameter5.Start );
            Assert.Equal ( 895.450M, result.Parameter5.End );
        }

    }

}
