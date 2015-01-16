grammar C;

/*
 * Parser Rules
 */

namespaceDeclaration
    :   'namespace' Identifier 
        namespaceBody
    ;

namespaceBody
    :   '{' classDeclaration* '}'
	;
classDeclaration
    :   'class' Identifier 
        (':' classDeclaration)?
        classBody
    ;

classBody
    :   '{' methodDeclaration* '}' ';'
    ;

methodDeclaration
	:	('virtual')? 
		type 
		(call)?
		Identifier '(' methodParameters? ')' ';'
	;

call
	:	Identifier 
	;

type
    :   Identifier ('.' Identifier)*
    ;

methodParameters
    :   methodParameter (',' methodParameter)*
    ;
	
methodParameter
    :   type variableDeclaratorId?
    ;

variableDeclaratorId
    :   Identifier ('[' ']')*
    ;

Identifier
    :   IdentifierNondigit
        (   IdentifierNondigit
        |   Digit
        )*
    ;

fragment
IdentifierNondigit
    :   Nondigit
    |   UniversalCharacterName
    //|   // other implementation-defined characters...
    ;

fragment
Nondigit
    :   [a-zA-Z_]
    ;

fragment
Digit
    :   [0-9]
    ;

fragment
UniversalCharacterName
    :   '\\u' HexQuad
    |   '\\U' HexQuad HexQuad
    ;

fragment
HexQuad
    :   HexadecimalDigit HexadecimalDigit HexadecimalDigit HexadecimalDigit
    ;

fragment
HexadecimalDigit
    :   [0-9a-fA-F]
    ;

compileUnit
	:	EOF
	;

/*
 * Lexer Rules
 */

 WS  :  [ \t\r\n\u000C]+ -> skip
    ;

COMMENT
    :   '/*' .*? '*/' -> skip
    ;

LINE_COMMENT
    :   '//' ~[\r\n]* -> skip
    ;
