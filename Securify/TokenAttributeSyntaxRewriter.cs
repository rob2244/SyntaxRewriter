using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Securify.Console
{
    public class TokenAttributeSyntaxRewriter : CSharpSyntaxRewriter
    {
        const string attributeName = "AttributeNameHere";

        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            var commented = node.DescendantTrivia()
                .Where(c => c.Kind() == SyntaxKind.SingleLineCommentTrivia)
                .FirstOrDefault(c => c.ToString().Contains(attributeName));

            if (commented != null)
                node = node.ReplaceTrivia(commented, SyntaxTrivia(SyntaxKind.WhitespaceTrivia, ""));

            var attribute = node.AttributeLists.SelectMany(c => c.Attributes)
                .FirstOrDefault(a => (a.Name as IdentifierNameSyntax)
                    .Identifier.Text.StartsWith(attributeName));

            if (node.AttributeLists.Any(al => al.Contains(attribute)))
                return node;

            var newAttrib =
                AttributeList(
                            SingletonSeparatedList(
                                Attribute(
                                    IdentifierName(attributeName)
                                    )
                                )
                            );

            return node.AddAttributeLists(newAttrib).NormalizeWhitespace();
        }
    }
}
