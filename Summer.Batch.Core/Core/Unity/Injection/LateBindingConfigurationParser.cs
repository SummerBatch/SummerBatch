//
//   Copyright 2015 Blu Age Corporation - Plano, Texas
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Summer.Batch.Core.Unity.Injection
{
    /// <summary>
    /// Parser for late binding expressions.
    /// </summary>
    public class LateBindingConfigurationParser
    {
        private readonly char[] _configuration;

        /// <summary>
        /// Constructs a new <see cref="LateBindingConfigurationParser"/>
        /// </summary>
        /// <param name="configuration">the late binding configuration to parse</param>
        public LateBindingConfigurationParser(string configuration)
        {
            _configuration = configuration.ToCharArray();
        }

        /// <summary>
        /// Parses the configuration and builds the corresponding <see cref="LateBindingNode"/>.
        /// </summary>
        /// <returns>a node representing the AST of the configuration</returns>
        public LateBindingNode Parse()
        {
            LateBindingNode currentNode = null;
            foreach (var node in ParseTokens())
            {
                if (currentNode == null)
                {
                    currentNode = node;
                    continue;
                }
                var concatenationNode = currentNode as ConcatenationNode;
                if (concatenationNode != null)
                {
                    concatenationNode.Add(node);
                }
                else
                {
                    currentNode = new ConcatenationNode(currentNode, node);
                }
            }
            return currentNode;
        }

        // Tokens : (StringNode? ExpressionNode?)*
        private IEnumerable<LateBindingNode> ParseTokens()
        {
            var index = 0;
            while (index < _configuration.Length)
            {
                var stringNode = ParseStringNode(ref index);
                if (stringNode != null)
                {
                    yield return stringNode;
                }
                var expressionNode = ParseExpressionNode(ref index);
                if (expressionNode != null)
                {
                    yield return expressionNode;
                }
            }
        }

        // A string node ends when there are no more characters or when the start
        // of an expression ('#{') is encountered. Any sequence within single quotes
        // is kept as is. A single quote can be printed by doubling it.
        private StringNode ParseStringNode(ref int index)
        {
            var start = index;
            var sb = new StringBuilder();
            var inQuote = false;
            while (index < _configuration.Length)
            {
                var c = _configuration[index];
                if (c == '\'')
                {
                    if (index < _configuration.Length - 1 && _configuration[index + 1] == '\'')
                    {
                        // Double quote, we keep one and skip the other
                        index++;
                        sb.Append(c);
                    }
                    else
                    {
                        inQuote = !inQuote;
                    }
                }
                else
                {
                    if (!inQuote && c == '#' && index < _configuration.Length - 1 && _configuration[index + 1] == '{')
                    {
                        // start of an expression encoutered, the string node is over
                        // we also don't consume the two characters of '#{'
                        break;
                    }
                    sb.Append(c);
                }
                index++;
            }
            return sb.Length > 0
                ? new StringNode(sb.ToString(), new Position(start, index))
                : null;
        }

        // ExpressionNode : '#{' Identifier ('[' Index ']')? '}'
        private ExpressionNode ParseExpressionNode(ref int index)
        {
            // the previous string node either ended because it reached the end or because the start
            // of an expression was encoutered, so we can safely consume two characters
            index += 2;
            if (index >= _configuration.Length)
            {
                return null;
            }
            ExpressionNode result = ParseIdentifier(ref index);
            if (_configuration[index] == '[')
            {
                index++;
                result = new IndexAccessorNode((IdentifierNode)result, ParseIndex(ref index));
                ParseChar(']', ref index);
            }
            ParseChar('}', ref index);
            return result;
        }

        // Identifer : [a-zA-Z][0-9a-zA-Z_]*
        private IdentifierNode ParseIdentifier(ref int index)
        {
            var start = index;
            var sb = new StringBuilder();
            if (index >= _configuration.Length || !char.IsLetter(_configuration[index]))
            {
                throw new LateBindingParserException(start,
                    string.Format("Parsing error at {0} ({1}): identifier must start with a letter", start,
                        _configuration[index]));
            }
            while (index < _configuration.Length &&
                (char.IsLetterOrDigit(_configuration[index]) || _configuration[index] == '_'))
            {
                sb.Append(_configuration[index]);
                index++;
            }
            if (start >= index)
            {
                throw new LateBindingParserException(start,
                        string.Format("Parsing error at {0}: expecting identifier.", start));
            }
            return new IdentifierNode(sb.ToString(), new Position(start, index));
        }

        // Index : '\'' [^']+ '\''
        private StringNode ParseIndex(ref int index)
        {
            var start = index;
            ParseChar('\'', ref index);
            var sb = new StringBuilder();
            while (index < _configuration.Length && _configuration[index] != '\'')
            {
                sb.Append(_configuration[index]);
                index++;
            }
            ParseChar('\'', ref index);
            return new StringNode(sb.ToString(), new Position(start, index));
        }

        private void ParseChar(char c, ref int index)
        {
            if (index >= _configuration.Length || _configuration[index] != c)
            {
                throw new LateBindingParserException(index,
                    string.Format("Parsing error at {0}: expecting '{1}'", index, c));
            }
            index++;
        }

        /// <summary>
        /// Exception thrown when an error is encountered while parsing a late binding configuration.
        /// </summary>
        public class LateBindingParserException : Exception
        {
            /// <summary>
            /// The index in the configuration where the error occured
            /// </summary>
            public int Index { get; set; }

            /// <summary>
            /// Custom constructor using an index and a message.
            /// </summary>
            /// <param name="index"></param>
            /// <param name="message"></param>
            public LateBindingParserException(int index, string message)
                : base(message)
            {
                Index = index;
            }

            /// <summary>
            /// Constructor for deserialization.
            /// </summary>
            /// <param name="info">the info holding the serialization data</param>
            /// <param name="context">the serialization context</param>
            protected LateBindingParserException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
                Index = info.GetInt32("Index");
            }

            /// <summary>
            /// Sets the <see cref="SerializationInfo"/> with information about the exception.
            /// </summary>
            /// <param name="info">
            /// The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.
            /// </param>
            /// <param name="context">
            /// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
            /// </param>
            public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                base.GetObjectData(info, context);
                info.AddValue("Index", Index);
            }
        }
    }

    /// <summary>
    /// Represents the position of a node in the configuration.
    /// </summary>
    public struct Position
    {
        private readonly int _start;
        private readonly int _end;

        /// <summary>
        /// The first index of the node.
        /// </summary>
        public int Start
        {
            get { return _start; }
        }

        /// <summary>
        /// The index after the last character of the node.
        /// </summary>
        public int End
        {
            get { return _end; }
        }

        /// <summary>
        /// Custom constructor using fields.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public Position(int start, int end)
        {
            _start = start;
            _end = end;
        }
    }

    #region AST Classes

    /// <summary>
    /// Base class for all the nodes of an AST representing a late binding configuration.
    /// </summary>
    public abstract class LateBindingNode
    {
        private readonly Position _position;

        private readonly List<LateBindingNode> _children = new List<LateBindingNode>();

        /// <summary>
        /// The position of the node.
        /// </summary>
        public Position Position
        {
            get { return _position; }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="position">the position of the node</param>
        protected LateBindingNode(Position position)
        {
            _position = position;
        }

        /// <summary>
        /// The children of the current node.
        /// </summary>
        public IList<LateBindingNode> Children
        {
            get { return _children.AsReadOnly(); }
        }

        /// <summary>
        /// Adds a new child to the current node.
        /// </summary>
        /// <param name="child">the new child</param>
        public void Add(LateBindingNode child)
        {
            _children.Add(child);
        }

        /// <summary>
        /// Aggregates the position of several nodes.
        /// </summary>
        /// <param name="nodes">the nodes, the position of which to aggregate</param>
        /// <returns>the new position</returns>
        protected static Position GetPosition(params LateBindingNode[] nodes)
        {
            return new Position(nodes.First().Position.Start, nodes.Last().Position.End);
        }
    }

    /// <summary>
    /// A node representing a string literal.
    /// </summary>
    public class StringNode : LateBindingNode
    {
        private readonly string _literal;

        /// <summary>
        /// The value of the string literal.
        /// </summary>
        public string Literal
        {
            get { return _literal; }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="literal">the literal value of the node</param>
        /// <param name="position">the position of the node</param>
        public StringNode(string literal, Position position)
            : base(position)
        {
            _literal = literal;
        }
    }

    /// <summary>
    /// Abstract class for nodes representing a late binding expression.
    /// </summary>
    public abstract class ExpressionNode : LateBindingNode
    {
        /// <summary>
        /// Custom constructor using position.
        /// </summary>
        /// <param name="position"></param>
        protected ExpressionNode(Position position)
            : base(position)
        {
        }
    }

    /// <summary>
    /// A node representing an identifier.
    /// </summary>
    public class IdentifierNode : ExpressionNode
    {
        private readonly string _identifier;

        /// <summary>
        /// The value of the identifier.
        /// </summary>
        public string Identifier
        {
            get { return _identifier; }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="identifier">the value of the identifier</param>
        /// <param name="position">the position of the node</param>
        public IdentifierNode(string identifier, Position position)
            : base(position)
        {
            _identifier = identifier;
        }
    }

    /// <summary>
    /// A node representing an index access.
    /// </summary>
    public class IndexAccessorNode : ExpressionNode
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="identifier">the identifier node representing the accessed object</param>
        /// <param name="index">the string node representing the accessed index</param>
        public IndexAccessorNode(IdentifierNode identifier, StringNode index)
            : base(GetPosition(identifier, index))
        {
            Add(identifier);
            Add(index);
        }
    }

    /// <summary>
    /// A node representing the concatenation of different strings.
    /// </summary>
    public class ConcatenationNode : LateBindingNode
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="nodes">the nodes that must be concatenated</param>
        public ConcatenationNode(params LateBindingNode[] nodes)
            : base(GetPosition(nodes))
        {
            foreach (var node in nodes)
            {
                Add(node);
            }
        }
    }

    #endregion
}