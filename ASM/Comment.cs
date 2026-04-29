namespace Asm {
    public class Comment(string s) : Op {
        public readonly string comment = s;

        public override string ToString() {
            return $"/* {this.comment} */";
        }
    }
}