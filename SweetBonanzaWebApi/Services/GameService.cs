using SweetBonanza.Models;

namespace SweetBonanza.WebApi.Services
{
    public class GameService
    {
        private readonly Random _rand = new();

        public List<CycleResult> RunGame()
        {
            var symbols = new Dictionary<string, double>
            {
                { "Banana", 0.161 },
                { "Grape", 0.148 },
                { "Watermelon", 0.119 },
                { "Plum", 0.097 },
                { "Apple", 0.082 },
                { "BlueCandy", 0.071 },
                { "GreenCandy", 0.058 },
                { "PurpleCandy", 0.046 },
                { "RedCandy", 0.018 }
            };

            var grid = GenerateGrid(5, 6, symbols, _rand);
            var results = Simulate(grid, symbols);
            return results;
        }

        public List<CycleResult> Simulate(string?[,] grid, Dictionary<string, double> symbols)
        {
            var results = new List<CycleResult>();
            int cycle = 1;

            while (true)
            {
                var spinGrid = GridToList(grid);

                var counts = CalculateSymbols(grid);
                var poppedSymbols = PopSymbols(grid, counts);

                var gridAfterPop = GridToList(grid);

                if (poppedSymbols.Count == 0)
                {
                    results.Add(new CycleResult
                    {
                        Cycle = cycle,
                        SpinGrid = spinGrid,
                        PoppedSymbols = null,
                        GridAfterPop = gridAfterPop,
                        GridAfterGravityFill = GridToList(grid)
                    });
                    break;
                }

                ApplyGravity(grid);
                FillTopEmptyCells(grid, symbols, _rand);

                var gridAfterGravityFill = GridToList(grid);

                results.Add(new CycleResult
                {
                    Cycle = cycle,
                    SpinGrid = spinGrid,
                    PoppedSymbols = poppedSymbols.ToArray(),
                    GridAfterPop = gridAfterPop,
                    GridAfterGravityFill = gridAfterGravityFill
                });

                cycle++;
            }

            return results;
        }


        private static List<List<string?>> GridToList(string?[,] grid)
        {
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);
            var columns = new List<List<string?>>();

            for (int col = 0; col < cols; col++)
            {
                var columnList = new List<string?>();
                for (int row = 0; row < rows; row++)
                {
                    columnList.Add(grid[row, col]);
                }
                columns.Add(columnList);
            }

            return columns;
        }

        private static string?[,] GenerateGrid(int rows, int columns, Dictionary<string, double> symbols, Random rand)
        {
            var arr = new string[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    arr[i, j] = GetRandomSymbol(symbols, rand);
                }
            }

            return arr;
        }

        private static Dictionary<string, double> CalculateSymbols(string?[,] grid)
        {
            var counts = new Dictionary<string, double>();
            for (int i = 0; i < grid.GetLength(0); i++)
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    var symbol = grid[i, j];
                    if (symbol != null)
                        counts[symbol] = counts.GetValueOrDefault(symbol) + 1;
                }
            return counts;
        }

        private static string GetRandomSymbol(Dictionary<string, double> symbols, Random rand)
        {
            double total = symbols.Values.Sum();
            if (total <= 0) return symbols.Keys.Last();

            double r = rand.NextDouble() * total;
            double cumulative = 0.0;
            foreach (var k in symbols)
            {
                cumulative += k.Value;
                if (r < cumulative) return k.Key;
            }

            return symbols.Keys.Last();
        }

        private static HashSet<string> PopSymbols(string?[,] grid, Dictionary<string, double> counts)
        {
            var poppedSymbols = counts
                .Where(v => v.Value >= 8)
                .Select(v => v.Key)
                .ToHashSet();

            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    var symbol = grid[i, j];
                    if (symbol != null && poppedSymbols.Contains(symbol))
                    {
                        grid[i, j] = null;
                    }
                }
            }

            return poppedSymbols;
        }

        private static void ApplyGravity(string?[,] grid)
        {
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);

            for (int col = 0; col < cols; col++)
            {
                var columnTiles = new List<string?>();

                for (int row = 0; row < rows; row++)
                {
                    if (grid[row, col] != null)
                    {
                        columnTiles.Add(grid[row, col]);
                    }
                }

                int writeRow = rows - 1;
                for (int k = columnTiles.Count - 1; k >= 0; k--)
                {
                    grid[writeRow, col] = columnTiles[k];
                    writeRow--;
                }

                for (int r = writeRow; r >= 0; r--)
                {
                    grid[r, col] = null;
                }
            }
        }

        private static void FillTopEmptyCells(string?[,] grid, Dictionary<string, double> symbols, Random rand)
        {
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);

            for (int col = 0; col < cols; col++)
            {
                int emptyCount = 0;
                for (int row = 0; row < rows; row++)
                {
                    if (grid[row, col] == null) emptyCount++;
                    else break;
                }

                for (int row = 0; row < emptyCount; row++)
                {
                    grid[row, col] = GetRandomSymbol(symbols, rand);
                }
            }
        }
    }
}
