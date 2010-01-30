using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AngelXNA.Infrastructure.Console
{
    public class ConsoleParser
    {
        #region Token Types
        private abstract class Token
        {
            public enum TokenType
            {
                Unknown,
                LiteralBool,
                LiteralNumber,
                LiteralString,
                Identifier,
                Assign,
                Add,
                Subtract,
                Divide,
                Multiply,
                BeginParen,
                EndParen,
                Comma,
                Dot,

                // Added by parser
                FunctionCall,
                PropertyGet,
                PropertySet
            }

            public TokenType Type;
            public List<Token> Children = new List<Token>();
            
            public Token(TokenType aType)
            {
                Type = aType;
            }

            public abstract object Execute(DeveloperConsole parent);
        }

        private class LiteralBool : Token
        {
            public bool Value;

            public LiteralBool(bool abValue)
                : base(TokenType.LiteralBool)
            {
                Value = abValue;
            }

            public override object Execute(DeveloperConsole parent)
            {
                return Value;
            }
        }

        private class LiteralNumber : Token
        {
            public float Value;

            public LiteralNumber(float afValue)
                : base(TokenType.LiteralNumber)
            {
                Value = afValue;
            }

            public override object Execute(DeveloperConsole parent)
            {
                return Value;
            }
        }

        private class LiteralString : Token
        {
            public string Value;

            public LiteralString(string asValue)
                : base(TokenType.LiteralString)
            {
                Value = asValue;
            }

            public override object Execute(DeveloperConsole parent)
            {
                return Value;
            }
        }

        private class Identifier : Token
        {
            public string Id;

            public Identifier(string asId)
                : base(TokenType.Identifier)
            {
                Id = asId;
            }

            public override object Execute(DeveloperConsole parent)
            {
                ConsoleVariable var = parent.ItemManager.FindCVar(Id);
                if (var == null)
                    throw new Exception("Unknown identifier " + Id);
                return var.Value;
            }
        }

        private class Operation : Token
        {
            public Operation(char acValue)
                : base(TokenType.Unknown)
            {
                switch (acValue)
                {
                    case '=': Type = TokenType.Assign; break;
                    case '+': Type = TokenType.Add; break;
                    case '-': Type = TokenType.Subtract; break;
                    case '*': Type = TokenType.Multiply; break;
                    case '/': Type = TokenType.Divide; break;
                    case '.': Type = TokenType.Dot; break;
                    case '(': Type = TokenType.BeginParen; break;
                    case ')': Type = TokenType.EndParen; break;
                    case ',': Type = TokenType.Comma; break;
                }
            }

            public override object Execute(DeveloperConsole parent)
            {
                object endVal = null;
                switch (Type)
                {
                    case TokenType.Assign:
                        {
                            // Evaluate RHS children, assign to identifier
                            Identifier lhs = (Identifier)Children[0];
                            object rhsValue = Children[1].Execute(parent);
                            // Try to do a PropertySet if current scope is not null.
                            if (parent.CurrentScope != null)
                            {
                                parent.ItemManager.SetPropertyValue(parent.CurrentScope, lhs.Id, rhsValue);
                            }
                            else
                            {
                                ConsoleVariable var = parent.ItemManager.GetCVar(lhs.Id);
                                var.Value = rhsValue;
                            }
                            endVal = rhsValue;
                        }
                        break;
                    case TokenType.Add:
                        {
                            float lhsValue = Convert.ToSingle(Children[0].Execute(parent));
                            float rhsValue = Convert.ToSingle(Children[1].Execute(parent));
                            float value = lhsValue + rhsValue;
                            endVal = value;
                        }
                        break;
                    case TokenType.Subtract:
                        {
                            float lhsValue = Convert.ToSingle(Children[0].Execute(parent));
                            float rhsValue = Convert.ToSingle(Children[1].Execute(parent));
                            float value = lhsValue - rhsValue;
                            endVal = value;
                        }
                        break;
                    case TokenType.Multiply:
                        {
                            float lhsValue = Convert.ToSingle(Children[0].Execute(parent));
                            float rhsValue = Convert.ToSingle(Children[1].Execute(parent));
                            float value = lhsValue * rhsValue;
                            endVal = value;
                        }
                        break;
                    case TokenType.Divide:
                        {
                            float lhsValue = Convert.ToSingle(Children[0].Execute(parent));
                            float rhsValue = Convert.ToSingle(Children[1].Execute(parent));
                            float value = lhsValue / rhsValue;
                            endVal = value;
                        }
                        break;
                    case TokenType.BeginParen:
                        endVal = Children[0].Execute(parent);
                        break;
                }

                return endVal;
            }
        }

        private class FunctionCall : Token
        {
            public string Function;
            public string CallOn;

            public FunctionCall(string asFunction)
                : base(TokenType.FunctionCall)
            {
                Function = asFunction;
            }

            public override object Execute(DeveloperConsole parent)
            {
                List<object> parameters = new List<object>();
                for (int i = 0; i < Children.Count; ++i)
                {
                    parameters.Add(Children[i].Execute(parent));
                }

                object[] paramArray = parameters.ToArray();
                
                object currentThis = null;
                ConsoleCommand cvarCommand;

                if (CallOn != null)
                {
                    ConsoleVariable cvarCallOn = parent.ItemManager.FindCVar(CallOn);

                    if (cvarCallOn != null)
                    {
                        currentThis = cvarCallOn.Value;
                        cvarCommand = parent.ItemManager.FindCommand(cvarCallOn.Value, Function, false);
                    }
                    else
                        cvarCommand = parent.ItemManager.FindCommand(CallOn, Function, false);
                }
                else 
                {
                    currentThis = parent.CurrentScope;
                    cvarCommand = parent.ItemManager.FindCommand(currentThis, Function, true);
                }

                return cvarCommand.Execute(currentThis, paramArray);
            }
        }

        private class PropertyGet : Token
        {
            public string CallOn;
            public string Property;

            public PropertyGet(string asOn, string asProperty)
                : base(TokenType.PropertyGet)
            {
                CallOn = asOn;
                Property = asProperty;
            }

            public override object Execute(DeveloperConsole parent)
            {
                ConsoleVariable cvarCallOn = parent.ItemManager.FindCVar(CallOn);
                return parent.ItemManager.GetPropertyValue(cvarCallOn.Value, Property);
            }
        }

        private class PropertySet : Token
        {
            public string CallOn;
            public string Property;

            public PropertySet(string asOn, string asProperty)
                : base(TokenType.PropertySet)
            {
                CallOn = asOn;
                Property = asProperty;
            }

            public override object Execute(DeveloperConsole parent)
            {
                ConsoleVariable cvarCallOn = parent.ItemManager.FindCVar(CallOn);
                parent.ItemManager.SetPropertyValue(cvarCallOn.Value, Property, Children[0].Execute(parent));

                return null;
            }
        }
        #endregion

        private static char[] s_SupportedSymbols = new char[] { '=', '+', '-', '*', '/', '.', '(', ')', ',' };

        private DeveloperConsole _parent;
        private int _iCurrentToken;
        private Token[] _tokens;

        public ConsoleParser(DeveloperConsole aParent)
        {
            _parent = aParent;
        }

        public void Execute(string input)
        {
            _tokens = Lex(input);
            if (_tokens.Length == 0)
                return;

            _iCurrentToken = 0;
            try
            {
                Token root = Statement();

                object endVal = root.Execute(_parent);

                if (_iCurrentToken < _tokens.Length)
                    _parent.Echo("Extra input found in execution line?");
            }
            catch (Exception e)
            {
                _parent.Echo("Error executing line: " + e.Message);
            }
        }

        // This is not a praticularly complex lexer, and thus we're not using complex lexing algorithms
        // that might be faster or at least more versitale.  This one is simple to read and only really 
        // supports identifiers, numbers literals, string literals, comments as "# or ;", and the 
        // symbols =,+,-,*,/,.,(,) and ,
        // It assumes that each line is an executable piece of code.
        private static Token[] Lex(string line)
        {
            List<Token> tokenList = new List<Token>();
            StringBuilder currentToken = new StringBuilder();
            for(int i = 0; i < line.Length; )
            {
                if (Char.IsWhiteSpace(line[i]))
                {
                    for (; i < line.Length && Char.IsWhiteSpace(line[i]); ++i)
                    {
                        // Do nothing, loop through white space
                    }
                }
                else if (line[i] == '#' || line[i] == ';')
                {
                    // The rest of this line is comment.  Ignore it.
                    break;
                }
                // Check for number literals
                else if (Char.IsDigit(line[i]))
                {
                    for (; i < line.Length && Char.IsDigit(line[i]); ++i)
                    {
                        currentToken.Append(line[i]);
                    }

                    // If we're not at the end of the line
                    if (i < line.Length)
                    {
                        if (line[i] == '.')
                        {
                            // Still got a number here, it's a floating point number!
                            currentToken.Append(line[i++]);
                            for (; i < line.Length && Char.IsDigit(line[i]); ++i)
                            {
                                currentToken.Append(line[i]);
                            }
                        }

                        if (i < line.Length && Char.IsLetter(line[i]))
                        {
                            // Assure that the next character not a letter
                            throw new Exception("Unexpected letter after digit.");
                        }
                    }

                    tokenList.Add(new LiteralNumber(float.Parse(currentToken.ToString())));
                    currentToken.Remove(0, currentToken.Length);
                }
                // Check for string literals
                else if (line[i] == '\"')
                {
                    // ignore starting quote
                    i++;
                    for (; i < line.Length && line[i] != '\"'; ++i)
                    {
                        currentToken.Append(line[i]);
                    }

                    if (i >= line.Length)
                        throw new Exception("Unexpected end of line found before string closed.");

                    tokenList.Add(new LiteralString(currentToken.ToString()));
                    currentToken.Remove(0, currentToken.Length);
                    // ignore ending quote
                    i++;
                }
                // Check for identifiers (starts with any letter or underscore)
                else if (Char.IsLetter(line[i]) || line[i] == '_')
                {
                    for (; i < line.Length && (Char.IsLetterOrDigit(line[i]) || line[i] == '_'); ++i)
                    {
                        currentToken.Append(line[i]);
                    }

                    string theToken = currentToken.ToString();
                    currentToken.Remove(0, currentToken.Length);
                    // Reserved words
                    if(theToken.Equals("true", StringComparison.CurrentCultureIgnoreCase))
                        tokenList.Add(new LiteralBool(true));
                    else if (theToken.Equals("false", StringComparison.CurrentCultureIgnoreCase))
                        tokenList.Add(new LiteralBool(false));
                    else
                        tokenList.Add(new Identifier(theToken));
                    
                }
                // All other symbols
                else if (s_SupportedSymbols.Contains(line[i]))
                {
                    tokenList.Add(new Operation(line[i++]));
                }
                else
                    throw new Exception("Unexpected symbol in input: " + line[i]);
            }

            return tokenList.ToArray();
        }


        private Token Statement()
        {
            if (_iCurrentToken >= _tokens.Length)
                throw new Exception("Unexpected end of line found!");

            // Look ahead, if the next token in an =, we have an assignment
            if (_iCurrentToken + 1 < _tokens.Length
                && _tokens[_iCurrentToken + 1].Type == Token.TokenType.Assign)
            {
                Token assign = _tokens[_iCurrentToken + 1];

                assign.Children.Add(Match(Token.TokenType.Identifier));
                Match(Token.TokenType.Assign);
                assign.Children.Add(Statement());

                return assign;
            }
            else
                return Expression();
        }

        private Token Expression()
        {
            if (_iCurrentToken >= _tokens.Length)
                throw new Exception("Unexpected end of line found!");

            Token lhs = Term();

            if(_iCurrentToken < _tokens.Length)
            {
                Token currentToken = _tokens[_iCurrentToken];
                switch (currentToken.Type)
                {
                    case Token.TokenType.Add:
                    case Token.TokenType.Subtract:
                        currentToken.Children.Add(lhs);
                        Match(currentToken.Type);
                        currentToken.Children.Add(Expression());
                        return currentToken;
                }
            }

            return lhs;
        }
        
        private Token Term()
        {
            if (_iCurrentToken >= _tokens.Length)
                throw new Exception("Unexpected end of line found!");

            Token lhs = Factor();
            
            if(_iCurrentToken < _tokens.Length)
            {
                // If we can look ahead, see what this is
                Token lookahead = _tokens[_iCurrentToken];
                switch(lookahead.Type)
                {
                    case Token.TokenType.Multiply:
                    case Token.TokenType.Divide:
                        lookahead.Children.Add(lhs);
                        Match(lookahead.Type);
                        lookahead.Children.Add(Factor());
                        return lookahead;
                }
            }

            return lhs;
        }

        private Token Factor()
        {
            Token currentToken = _tokens[_iCurrentToken];
            switch(currentToken.Type)
            {
                case Token.TokenType.BeginParen:
                    Match(Token.TokenType.BeginParen);
                    currentToken.Children.Add(Expression());
                    Match(Token.TokenType.EndParen);
                    return currentToken;
                case Token.TokenType.Identifier:
                    // Check if function call
                    if(_iCurrentToken + 1 < _tokens.Length &&
                        _tokens[_iCurrentToken + 1].Type == Token.TokenType.BeginParen)
                    {
                        return Function();
                    }
                    // Check if resolution
                    else if (_iCurrentToken + 1 < _tokens.Length &&
                        _tokens[_iCurrentToken + 1].Type == Token.TokenType.Dot)
                    {
                        Identifier id = Match(Token.TokenType.Identifier) as Identifier;
                        Match(Token.TokenType.Dot);
                        if (_iCurrentToken + 1 < _tokens.Length &&
                            _tokens[_iCurrentToken + 1].Type == Token.TokenType.BeginParen)
                        {
                            FunctionCall func = Function() as FunctionCall;
                            func.CallOn = id.Id;
                            return func;
                        }
                        else if (_iCurrentToken + 1 < _tokens.Length &&
                            _tokens[_iCurrentToken + 1].Type == Token.TokenType.Assign)
                        {
                            Identifier prop = Match(Token.TokenType.Identifier) as Identifier;
                            PropertySet set = new PropertySet(id.Id, prop.Id);
                            Match(Token.TokenType.Assign);
                            set.Children.Add(Expression());
                            return set;
                        }
                        else
                        {
                            Identifier prop = Match(Token.TokenType.Identifier) as Identifier;
                            PropertyGet get = new PropertyGet(id.Id, prop.Id);
                            return get;
                        }
                    }
                    goto default;
                case Token.TokenType.Add:
                case Token.TokenType.Subtract:
                    if (_iCurrentToken + 1 < _tokens.Length &&
                        _tokens[_iCurrentToken + 1].Type == Token.TokenType.LiteralNumber)
                    {
                        Token plusMinus = Match(currentToken.Type);
                        LiteralNumber literal = Match(Token.TokenType.LiteralNumber) as LiteralNumber;
                        if (plusMinus.Type == Token.TokenType.Subtract)
                            literal.Value = -literal.Value;

                        return literal;
                    }
                    goto default;
                case Token.TokenType.LiteralNumber:
                default:
                    return Match(currentToken.Type);
            }
        }

        private Token Function()
        {
            Identifier identifier = Match(Token.TokenType.Identifier) as Identifier;
            FunctionCall call = new FunctionCall(identifier.Id);
            Match(Token.TokenType.BeginParen);
            while (_tokens[_iCurrentToken].Type != Token.TokenType.EndParen)
            {
                call.Children.Add(Expression());
                if (_tokens[_iCurrentToken].Type != Token.TokenType.EndParen)
                    Match(Token.TokenType.Comma);
            }
            Match(Token.TokenType.EndParen);

            return call;
        }

        private Token Match(Token.TokenType type)
        {
            if (_tokens[_iCurrentToken].Type == type)
                return _tokens[_iCurrentToken++];

            throw new Exception("Invalid syntax.  Expected " + type + " found " + _tokens[_iCurrentToken].ToString());
        }
    }
}