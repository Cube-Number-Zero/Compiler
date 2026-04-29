using System.Text.RegularExpressions;
public static class Terminals{
    
    public static string terminalspec = @"
        COMMENT :: (//.*\n)|(/\*(.|\n)*?\*/)
        VAR :: var\b
        WHITESPACE :: \s+
        FUNC :: func\b
        TYPE :: (int|float|string|bool)\b
        CAST :: as\b
        NEW :: new\b
        CLASS :: class\b
        THIS :: this\b
        RETURN :: return\b
        WHILE :: while\b
        IF :: if\b
        ELSE :: else\b
        BOOLCONST :: (true\b)|(false\b)
        BOOLOP :: (and\b)|(or\b)
        BOOLNOT :: not\b
        ID :: [a-zA-Z_]\w*
        FNUM :: -?((\d*)\.(\d+)\b)|(((\d+)|((\d*)\.(\d+)))[eE]([\+\-]?(\d+))\b)
        NUM :: (\d+)\b
        LBRACE :: \{
        RBRACE :: \}
        LPAREN :: \(
        RPAREN :: \)
        LBRACKET :: \[
        RBRACKET :: \]
        ADDOP :: \+
        SUBOP :: \-
        SHIFTOP :: (>>>)|(<<)|(>>)
        RELOP :: (>=?)|(<=?)
        EQOP :: (==)|(!=)
        BITOP :: [&\|\^]
        POWOP :: \*\*
        MULOP :: [\*/%]
        BITNOT :: ~
        EQ :: =
        DOTOP :: \.
        COLON :: :
        COMMA :: ,
        STRINGCONST :: ""((\\([0abefnrtv\\'\?""]))|[^\\""])*""
    ";
    public class Terminal(string sym, Regex rex)
    {
        public string sym = sym;
        public Regex rex = rex;
    }
    public static List<Terminal> terminals = [];
    public static void Init() {
        foreach(string line_ in terminalspec.Split('\n')) {
            string line = line_.Trim();
            if(line.Length == 0)
                continue;
            string[] tmp = line.Split("::");
            string sym = tmp[0].Trim();
            string regex = tmp[1].Trim();
            terminals.Add(new Terminal(sym, new Regex("\\G(" + regex + ")")));
        }
    }
}
