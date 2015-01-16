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
    :   classAttribute?
		'class' Identifier 
        (':' classExtends)?
        classBody
    ;

classExtends
	:	classNames
	;

classNames
	: className (',' className)?
	;

className
	: Identifier
	;

classAttribute
	: '//' '[' classAttributeName ']'
	;

classAttributeName
	: Identifier
	;

classBody
    :   '{' methodDeclaration* '}' ';'
    ;

methodDeclaration
	:	'virtual'? 
		returnType 
		call?
		Identifier '(' methodParameters? ')' ('=''0')? ';'
	;

returnType 
	: type
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
    :   parameterType parameterName?
    ;

parameterType
	:	type
	;

parameterName
	:	variableDeclaratorId
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
/*
LINE_COMMENT
    :   '//' ~[\r\n]* -> skip
    ;
*/