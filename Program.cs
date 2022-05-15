using Aoc2018.Library;
using System.Diagnostics;

var appArgs = args;

bool progressMode = true;

Console.ForegroundColor = AocCore.DefaultColor;

var actions = AocCore.Activation<ISolver>.Get()
    .OrderBy(solver => solver.Order)
    .Select(solver => new Action(solver.RunProblems));

if (progressMode)
{
    actions.Last().Invoke();
}
else
{
    Stopwatch st = new();
    AocCore.PrintTableHeader();

    st.Start();
    actions.ForEach(act => act.Invoke());
    st.Stop();

    Console.WriteLine();

    Console.WriteLine($"Total time solutions: {st.ElapsedMilliseconds}ms");
}

