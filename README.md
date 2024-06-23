# Ascent 
A quick, programmatic, interpreted language inspired by Molang.
# What Ascent *Isn't* for
Ascent isn't meant to be a full programming language. It's meant to be more simple and also very fast to quick.
# What Ascent *is* for
Ascent is meant for adding scripting to places that are user-controlled and otherwise hardcoded. For example animating game elements in user-made objects through JSON. Another feature that was inspired by Molang are query variables (Read more about these in the basics section!), these are especially helpful when thinking about implementation inside games and other real-time applications.
# Basics
- Query Variables
	- Query Variables are passed into the evaluator using a `AscentVariableMap` and allow the implementing program to pass outside values into Molang expressions. These are the basis of the language. Query Variables are prefixed with either `query.` or `q.` .
	- Example:
		- Implementation `var result = AscentEvaluator.Evaluate("q.x", new AscentVariableMap(new Dictionary<string, float>() { { "x", 5f }}));`
		- Result: `5`
- Expressions
	- Expressions are essentially a group of instructions. Expressions can also contain expressions inside (for example function arguments). Expressions are separated by semi-colons.
- Implicit Return
	- The return of a statement is defined by the last separate expression with neither a assignment, definition, or similar. For example an expression of `lerp(5, 10, 0.5)` will return 7.5.
- Variables
	- Variables are similar to Query Variables in that they can be accessed within Molang, but they are different in a few ways. One way is that Variables cannot be defined in the implementing code but they can be read after evaluation. Another way is that Variables are defined in the expression using `let` similar to JavaScript.
	- Example:
		- Ascent: `let queryExample = 15; queryExample;`
		- Result: `15`
- Operations
	- The currently supported operations include
	1.  Addition `+`
	2. Subtraction `-`
	3. Multiplication `*`
	4. Division `/`
	5. Power `^`
	6. Modulus `%`
	7. Greater than `>`
	8. Lesser than `<`
	9. Ternary `conditionExpression ? trueExpression : falseExpression`
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
	15. ... And more [Here](https://github.com/Futuremappermydud/AscentLanguage/blob/main/AscentLanguage/Lang/Functions/AscentFunctions.cs#L12)
# Roadmap
- [ ] Performance metrics.
- [ ] API to add functions from implementing code.
- [ ] Translate to C++ as an additional language.
- [ ] Support creating functions from within Ascent code.
- [ ] Rewrite parser to be more modular/legible.
- [ ] Refactor to allow strings, booleans, etc.
- [ ] Add Ascent code examples.
- [ ] Add more functions to the default list.
- [ ] More in the future?