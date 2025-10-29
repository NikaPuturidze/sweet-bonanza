namespace SweetBonanza.Models
{
    public class CycleResult
    {
        public int Cycle { get; set; }
        public required List<List<string?>> SpinGrid { get; set; } 
        public string[]? PoppedSymbols { get; set; } 
        public required List<List<string?>> GridAfterPop { get; set; }
        public required List<List<string?>> GridAfterGravityFill { get; set; }
    }
}
