public class Member(Token tok) : Term(tok) {
    public ClassType? declaringClassType;

    public override void SetType(){
        if (declaringClassType is null)
            return;
        foreach(var c in declaringClassType!.members) {
            if (c.name == token.lexeme) {
                this.type = c.type;
                return;
            }
        }
    }
}