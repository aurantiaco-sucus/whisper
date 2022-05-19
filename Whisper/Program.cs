// See https://aka.ms/new-console-template for more information

using Whisper.Interpreter;
using Whisper.Structure;

var parser = new Parser("(+ 1 (+ 2 3))");
var parserAdd = new Parser("(lambda (x y) (add x y))");
parser.Parse();
parserAdd.Parse();
var result = parser.Export();
var resultAdd = parserAdd.Export();
result.DebugPrint();
var interpreter = new Interpreter(
    new Dictionary<string, Element>
    {
        ["+"] = resultAdd.ToList()[0]
    });
Console.WriteLine();
interpreter.InterpretList(result).DebugPrint();
Console.WriteLine();

