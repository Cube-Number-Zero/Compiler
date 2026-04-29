using System.Text.RegularExpressions;

public class Tokenizer{
    private string? input;
    private int index;
    private int line;
    private int column;
    readonly Stack<Token> past = new();
    readonly Stack<Token> future = new();
    public Tokenizer() {
        
    }
    readonly Dictionary<char, char> escapes = new() {
        {'a', '\a'},
        {'b', '\b'},
        {'e', '\e'},
        {'f', '\f'},
        {'n', '\n'},
        {'r', '\r'},
        {'t', '\t'},
        {'v', '\v'},
        {'0', '\0'},
        {'\\', '\\'},
        {'\'', '\''},
        {'\"', '\"'}
    };
    public void SetInput(string input) {
        this.input = input;
        this.index = 0;
        this.line = 1;
        this.column = 0;
    }
    public Token Next() {
        Token tok;
        if (future.Count() > 0) {
            tok = future.Pop();
            past.Push(tok);
            return tok;
        }
        int bestlength = 0;
        Match? bestmatch = null;
        Terminals.Terminal? bestterminal = null;
        Terminals.Init();
        foreach(Terminals.Terminal t in Terminals.terminals) {
            Match M = t.rex.Match(input!, index);
            //if M is the best match we've found so far:
            if (M.Length > bestlength) {
            //  remember that
                bestlength = M.Length;
                bestmatch = M;
                bestterminal = t;
            }
        }
    
        if (bestlength == 0) {
            if (index == input!.Length) {
                tok = new("$", line, column, "");
                past.Push(tok);
                return tok;
            }
            Utils.Error($"Could not find match for terminal at line {line} column {column}");
            throw new Exception();
        } else {

            tok = new Token(bestterminal!.sym, line, column, bestmatch!.Value);
            
            //advance index, line, and column
            foreach (char c in tok.lexeme) {
                if (c == '\n') {
                    ++line;
                    column = 0;
                } else {
                    ++column;
                }
            }

            index += bestlength;

            // Fix string lexeme
            if (tok.sym == "STRINGCONST") {
                tok.lexeme = tok.lexeme.Substring(1, tok.lexeme.Length - 2);
                string newLexeme = "";

                bool escape = false;
                foreach (char c in tok.lexeme) {
                    if (escape) {
                        newLexeme += escapes[c];
                        escape = false;
                    }
                    else {
                        if (c == '\\') {
                            escape = true;
                        } else {
                            newLexeme += c;
                        }
                    }
                }

                tok.lexeme = newLexeme;
            }

            if (tok.sym == "WHITESPACE") {
                if (tok.lexeme.Contains('\n'))
                    tok = new("$$", line, column, tok.lexeme);
                else
                    return this.Next();
            } else if (tok.sym == "COMMENT") {
                if (tok.lexeme.Contains('\n'))
                    tok = new("$$", line, column, tok.lexeme);
                else
                    return this.Next();
            }
            past.Push(tok);
            return tok;
        }
    }

    public string Peek() {
        if (future.Count() > 0) {
            return future.Peek().sym;
        }
        int bestlength = 0;
        Match? bestmatch = null;
        Terminals.Terminal? bestterminal = null;
        Terminals.Init();
        foreach(Terminals.Terminal t in Terminals.terminals) {
            Match M = t.rex.Match(input!, index);
            //if M is the best match we've found so far:
            if (M.Length > bestlength) {
            //  remember that
                bestlength = M.Length;
                bestmatch = M;
                bestterminal = t;
            }
        }
    
        if (bestlength == 0) {
            if (index == input!.Length) {
                return "$";
            }
            Utils.Error($"Could not find match for terminal at line {line} column {column}");
            throw new Exception();
        } else {

            Token tok = new(bestterminal!.sym, line, column, bestmatch!.Value);

            // Fix string lexeme
            if (tok.sym == "STRINGCONST") {
                tok.lexeme = tok.lexeme.Substring(1, tok.lexeme.Length - 2);
                string newLexeme = "";

                bool escape = false;
                foreach (char c in tok.lexeme) {
                    if (escape) {
                        newLexeme += escapes[c];
                        escape = false;
                    }
                    else {
                        if (c == '\\') {
                            escape = true;
                        } else {
                            newLexeme += c;
                        }
                    }
                }

                tok.lexeme = newLexeme;
            }

            if (tok.sym == "WHITESPACE") {
                if (tok.lexeme.Contains('\n'))
                    return "$$";
                else {
                    index += bestlength;
                    string ret = this.Peek();
                    index -= bestlength;
                    return ret;
                }
            } else if (tok.sym == "COMMENT") {
                if (tok.lexeme.Contains('\n'))
                    return "$$";
                else {
                    index += bestlength;
                    string ret = this.Peek();
                    index -= bestlength;
                    return ret;
                }
            }
            return tok.sym;
        }
    }

    public int GetCurrentLine() {
        return line;
    }

    public int GetCurrentColumn() {
        return column;
    }

    public void Rewind() {
        future.Push(past.Pop());
    }

    public Token Expect(string expected) {
        Token t = this.Next();
        if (t.sym != expected) {
            Utils.Error($"Expected {expected} at line {line} column {column}, but got {t.sym}");
            throw new Exception();
        }
        return t;
    }

    public List<Token> ReadUntil( params string[] terminatingSymbol){
        List<Token> L = [];
        while (true) {
            Token t = this.Next();
            foreach(string sym in terminatingSymbol) {
                if (sym == t.sym) {
                    this.Rewind();
                    return L;
                }
            }
            L.Add(t);
        }
    }
}
