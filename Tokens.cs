public class Token(string sym, int line, int column, string lexeme) : Object {
    public string sym = sym;
    public string lexeme = lexeme;
    public int line = line;
    public int column = column;

    public override string ToString() {
        return $"[{this.sym} at line {this.line} column {this.column}: {this.lexeme}]";
    }
}