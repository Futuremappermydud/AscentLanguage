# Ascent 
A quick, programmatic, interpreted language inspired by Molang.
# Where can Ascent be used?
Before you get too deep, please take note that as of right now Ascent can only be implemented in C# codebases. I plan on expanding to a C++ version of Ascent in the future.
# What Ascent *Isn't* for
Ascent isn't meant to be a full programming language. It's meant to be more simple and also very fast to quick.
# What Ascent *is* for
Ascent's primary use case is adding scripting to places that are user-controlled and otherwise hardcoded. 
- For example animating game elements in user-made objects through JSON. For this we have a feature inspired by Molang called Query Variables (Read more about these in the [Basics](#Basics) section!). These are especially helpful when thinking about implementation within games and other real-time applications.
# Performance
Basic performance metrics are laid out [here!](./PERFORMANCE.md)
# Basics
- The Evaluator
	- The Evaluator is the main part of Ascent. It's static so all you have to do to get started is call `AscentEvaluator.Evaluate(expression, variableMap?, cache?, debug?);` and get back the float value returned! It's super simple.
- Query Variables
	- Query Variables are passed into the evaluator using a `AscentVariableMap` and allow the implementing program to pass outside values into Ascent expressions. These are the basis of the language. Query Variables are prefixed with either `query.` or `q.` .
	- Example:
		- Implementation `var result = AscentEvaluator.Evaluate("q.x", new AscentVariableMap(new Dictionary<string, float>() { { "x", 5f }}));`
		- Result: `5`
- Expressions
	- Expressions are essentially a group of instructions. Expressions can also contain expressions inside (for example function arguments). Expressions are separated by semi-colons.
- Implicit Return
	- The return of a statement is defined by the last separate expression with neither a assignment, definition, or similar. For example an expression of `lerp(5, 10, 0.5)` will return 7.5. Returns can also be explicit! Just place return before your statement.
- Variables
	- Variables are similar to Query Variables in that they can be accessed within Ascent, but they are different in a few ways. One way is that Variables cannot be defined in the implementing code but they can be read after evaluation. Another way is that Variables are defined in the expression using `let` similar to JavaScript.
	- Example:
		- Ascent: `let queryExample = 15; queryExample;`
		- Result: `15`
- Operations
	- The currently supported operations include
	1. Addition `+`
	2. Subtraction `-`
	3. Multiplication `*`
	4. Division `/`
	5. Power `^`
	6. Modulus `%`
	7. Greater than `>`
	8. Lesser than `<`
	9. Addition Assignment `+=`
	10. Subtraction Assignment `-=`
	11. Increment `++`
	12. Decrement `--`
	14. Ternary `conditionExpression ? trueExpression : falseExpression`
- Functions
	- Ascent supports a set of functions, mostly focused on math operations.
	- Currently supported functions include
	1. Sin
	2. Cos
	3. Tan
	4. Clamp
	5. Sign
	6. Sqrt
	7. Int
	8. Abs
	9. Pow
	10. Exp
	11. Frac
	12. Lerp
	13. Debug (prints all given arguments to console)
	14. ... And more [Here](https://github.com/Futuremappermydud/AscentLanguage/blob/main/AscentLanguage/Lang/Functions/AscentFunctions.cs#L12)
- Function Definitions
	- Function definitions are written by you! They follow very similar syntax to javascript and also support nesting!
	- Example:
```
function test(add1, add2) {
	function k(a) {
	    return a * 2;
	}
	let result = k(add1) + k(add2);
    return result;
}
return test(2, 2);
```
- Loops
	- Currently there are two loops implemented, for loops and while loops. These also follow very similar syntax to other languages.
```
let g = 10;
while(g > 0) {
	g--;
	debug(g);
}
return g;
```

```
for(let i = 0; i < 10; i++)
{
	debug(i);
}
```
# Roadmap
- [x] Performance metrics.
- [ ] Syntax Error Handling
- [ ] API to add functions from implementing code.
- [ ] Translate to C++ as an additional language.
- [x] Support creating functions from within Ascent code.
- [x] Rewrite parser to be more modular/legible.
	- [ ] Further clean up to make the parser more pretty.
- [ ] Refactor to allow strings, booleans, etc.
- [ ] Translate to C++ as an additional language.
- [ ] Add Ascent code examples.
- [ ] Add more functions to the default list.
- [ ] More in the future?