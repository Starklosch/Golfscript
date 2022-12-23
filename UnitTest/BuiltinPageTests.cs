
namespace Golfscript.UnitTest
{
    public class BuiltinPageTests
    {
        Golfscript Golfscript { get; } = new();

        void Test(string expected, string code)
        {
            Golfscript.Stack.Clear();
            Golfscript.Run(code);
            Assert.Equal(expected, Golfscript.Stack.TestString());
        }

        [Fact]
        void Evaluate()
        {
            Test("-6", "5~");
            Test("3", "\"1 2+\"~");
            Test("3", "{1 2+}~");
            Test("1 2 3", "[1 2 3]~");
        }


        [Fact]
        void Inspect()
        {
            Test("\"1\"", "1`");
            Test("\"[1 [2] \\\"asdf\\\"]\"", "[1 [2] 'asdf']`");
            Test("\"\\\"1\\\"\"", "\"1\"`");
            Test("\"{1}\"", "{1}`");
        }


        [Fact]
        void Negate()
        {
            Test("0", "1!");
            Test("0", "{asdf}!");
            Test("1", "\"\"!");
        }


        [Fact]
        void Rotate()
        {
            Test("1 3 4 2", "1 2 3 4 @");
        }

        [Fact]
        void Sort()
        {
            Test("1 2 3 4 5 4", "1 2 3 4 5  1$");
            Test("\"adfs\"", "'asdf'$");
            Test("[5 4 3 2 1]", "[5 4 3 1 2]{-1*}$");
        }


        [Fact()]
        void Add()
        {
            Test("12", "5 7+");
            Test("{asdf 1234}", "'asdf'{1234}+");
            Test("[1 2 3 4 5]", "[1 2 3][4 5]+");
        }


        [Fact]
        void Subtract()
        {
            Test("1 -1", "1 2-3+");
            Test("1 -1", "1 2 -3+");
            Test("2", "1 2- 3+");
            Test("[5 5 4]", "[5 2 5 4 1 1][1 2]-");
        }


        [Fact]
        void Multiply()
        {
            Test("8", "2 4*");
            Test("64", "2 {2*} 5*");
            Test("[1 2 3 1 2 3]", "[1 2 3]2*");
            Test("\"asdfasdfasdf\"", "3'asdf'*");
            Test("\"1,2,3\"", "[1 2 3]','*");
            Test("[1 4 2 4 3]", "[1 2 3][4]*");
            Test("\"a s d f\"", "'asdf'' '*");
            Test("\"1-\\x02-\\x03\\x04\\x05\"", "[1 [2] [3 [4 [5]]]]'-'*");
            Test("[1 6 7 2 6 7 3 [4 [5]]]", "[1 [2] [3 [4 [5]]]][6 7]*");
            Test("10", "[1 2 3 4]{+}*");
            Test("414", "'asdf'{+}*");
        }


        [Fact]
        void Divide()
        {
            Test("2", "7 3 /");
            Test("[[1] [4] [5]]", "[1 2 3 4 2 3 5][2 3]/");
            Test("[\"a\" \"s\" \"d\" \"f\"]", "'a s d f'' '/");
            Test("[[1 2] [3 4] [5]]", "[1 2 3 4 5] 2/");
            Test("89 [1 1 2 3 5 8 13 21 34 55 89]", "0 1 {100<} { .@+ } /");
            Test("2 3 4", "[1 2 3]{1+}/");
        }


        [Fact]
        void Modulus()
        {
            Test("1", "7 3 %");
            Test("[\"a\" \"df\"]", "'assdfs' 's'%");
            Test("[\"a\" \"\" \"df\" \"\"]", "'assdfs' 's'/");
            Test("[1 3 5]", "[1 2 3 4 5] 2%");
            Test("[5 4 3 2 1]", "[1 2 3 4 5] -1%");
            Test("[1 1 2 2 3 3]", "[1 2 3]{.}%");
        }


        [Fact]
        void Or()
        {
            Test("7", "5 3 |");
        }


        [Fact]
        void And()
        {
            Test("[1]", "[1 1 2 2][1 3]&");
        }


        [Fact]
        void Xor()
        {
            Test("[2 3]", "[1 1 2 2][1 3]^");
        }

        [Fact]
        void RawString()
        {
            Test("\"\\\\n\"", "'\n'");
            Test("\" ' \"", "' \\' '");
        }

        [Fact]
        void String()
        {
            Test("\"\n\"", "\"\n\"");
            Test("\"d\"", "\"\\144\"");
        }

        [Fact]
        void Stack()
        {
            Test("[2 1]", "1 2 [\\]");
        }


        [Fact]
        void Swap()
        {
            Test("1 3 2", "1 2 3 \\");
        }


        [Fact]
        void Asign()
        {
            Test("1 1", "1:a a");
            Test("1", "1:0;0");
        }


        [Fact]
        void Pop()
        {
            Test("1 2", "1 2 3;");
        }


        [Fact]
        void Less()
        {
            Test("1", "3 4 <");
            Test("1", "\"asdf\" \"asdg\" <");
            Test("[1 2]", "[1 2 3] 2 <");
            Test("{asd}", "{asdf} -1 <");
        }


        [Fact]
        void Greater()
        {
            Test("0", "3 4 >");
            Test("0", "\"asdf\" \"asdg\" >");
            Test("[3]", "[1 2 3] 2 >");
            Test("{f}", "{asdf} -1 >");
        }


        [Fact]
        void Equal()
        {
            Test("0", "3 4 =");
            Test("0", "\"asdf\" \"asdg\" =");
            Test("3", "[1 2 3] 2 =");
            Test("102", "{asdf} -1 =");
        }


        [Fact]
        void Comma()
        {
            Test("[0 1 2 3 4 5 6 7 8 9]", "10,");
            Test("10", "10,,");
            Test("[1 2 4 5 7 8]", "10,{3%},");
        }


        [Fact]
        void Dot()
        {
            Test("1 2 3 3", "1 2 3.");
        }


        [Fact]
        void Order()
        {
            Test("256", "2 8?");
            Test("2", "5 [4 3 5 1] ?");
            Test("5", "[1 2 3 4 5 6] {.* 20>} ?");
        }


        [Fact]
        void Decrement()
        {
            Test("4", "5(");
            Test("[2 3] 1", "[1 2 3](");
        }


        [Fact]
        void Increment()
        {
            Test("6", "5)");
            Test("[1 2] 3", "[1 2 3])");
        }


        [Fact]
        void LazyBool()
        {
            Test("5", "5 {1 0/} or");
            Test("2", "5 {1 1+} and");
            Test("[3]", "0 [3] xor");
            Test("0", "2 [3] xor");
        }


        [Fact]
        void Rand()
        {
            Test("2", "5 rand -");
        }


        [Fact]
        void Do()
        {
            Test("4 3 2 1 0 0", "5{1-..}do");
        }


        [Fact]
        void While()
        {
            Test("4 3 2 1 0 0", "5{.}{1-.}while");
            Test("5", "5{.}{1-.}until");
        }


        [Fact]
        void If()
        {
            Test("2", "1 2 3 if");
            Test("1 1", "0 2 {1.} if");
        }


        [Fact]
        void Abs()
        {
            Test("2", "-2 abs");
        }


        [Fact]
        void Zip()
        {
            Test("[[1 4 7] [2 5 8] [3 6 9]]", "[[1 2 3][4 5 6][7 8 9]]zip");
            Test("[\"a1\" \"s2\" \"d3\" \"f4\"]", "['asdf''1234']zip");
        }


        [Fact]
        void Base()
        {
            Test("6", "[1 1 0] 2 base");
            Test("[1 1 0]", "6 2 base");
        }
    }
}
